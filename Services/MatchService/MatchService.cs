using Core.Attributes;
using Core.Core;

namespace MatchService;

[Service(EServiceType.Global)]
public class MatchService : Service
{
    public MatchService(string serviceId) : base(serviceId)
    {
    }

}