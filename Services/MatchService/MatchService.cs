using Core.Attributes;
using Core.Core;
using Core.Utils;

namespace MatchService;

[Service(EServiceType.Global)]
public class MatchService : Service
{
    public MatchService(string serviceId) : base(serviceId)
    {
    }

    [Rpc]
    public string Test4(int a)
    {
        CoreUtils.WriteLine($"{ServiceId}::Test4 收到");

        return "aaaa";
    }


}