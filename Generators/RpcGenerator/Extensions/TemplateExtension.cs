
using System.Text;
using RpcGenerator;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Gen;

public static class TemplateExtension
{

    /// <summary>
    /// 模板初始化器
    /// </summary>
    public delegate void TemplateInitializer(ScriptObject scriptObj);


    /// <summary>
    /// 将传入的数据模型转为模板上下文
    /// </summary>
    /// <param name="model">数据模型</param>
    /// <param name="initializer">初始化器</param>
    /// <returns>模板上下文</returns>
    private static TemplateContext GetContent(object? model = null, TemplateInitializer? initializer = null)
    {
        var content = new TemplateContext
        {
            // 关闭最大循环限制
            LoopLimit = 0,
            // 关闭自动缩进，该功能表现较为奇怪
            AutoIndent = false
        };
        if (model is null) return content;

        var scriptObj = new ScriptObject();
        initializer?.Invoke(scriptObj);
        scriptObj.Import(model);
        content.TemplateLoader = new IncludeLoader();
        content.PushGlobal(scriptObj);
        return content;
    }

    /// <summary>
    /// 渲染模板数据写入文件，异步方法
    /// </summary>
    /// <param name="template">模板</param>
    /// <param name="filepath">文件路径</param>
    /// <param name="model">数据模型</param>
    /// <param name="initializer">初始化器</param>
    /// <param name="encoding">文件编码</param>
    /// <returns>异步任务</returns>
    public static async Task WriteAsync(this Template template, string filepath, object? model = null,
        TemplateInitializer? initializer = null, Encoding? encoding = null)
    {
        var text = await template.RenderAsync(GetContent(model, initializer));
        await FileHelper.WriteAllTextAsync(filepath, text, encoding);
    }

    private class IncludeLoader : ITemplateLoader
    {
        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName) =>
            Path.Combine(GeneratorConst.TemplateDir, templateName);

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
            File.ReadAllText(templatePath);

        public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath) =>
            await File.ReadAllTextAsync(templatePath);
    }
}