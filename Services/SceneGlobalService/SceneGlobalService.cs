using Core.Attributes;
using Core.Core;

namespace SceneGlobalService;

[Service(EServiceType.Global)]
public class SceneGlobalService : Service
{
    public SceneGlobalService(string serviceId) : base(serviceId)
    {
    }
}