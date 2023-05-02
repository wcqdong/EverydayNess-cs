// -----------------------------------
//  Copyright(C) kuro Co.Ltd
//
//  模块说明：模板辅助类
//
//  创建人员：魏川骐
//  创建日期：2023-4-22
// -----------------------------------

using Scriban;

namespace RpcGenerator.Helpers;

public static class TemplateHelper
{
    /// <summary>
    /// 从模板文件创建模板对象，异步方法
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <returns>模板</returns>
    public static async Task<Template> CreateAsync(string filepath)
    {
        var text = await File.ReadAllTextAsync(filepath);
        return Template.Parse(text);
    }

    public static Template Create(string filepath)
    {
        var text = File.ReadAllText(filepath);
        return Template.Parse(text);
    }
}