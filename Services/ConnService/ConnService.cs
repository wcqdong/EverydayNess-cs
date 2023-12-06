using Core.Attributes;
using Core.Core;

namespace ConnService;

[Service(EServiceType.Normal)]
public class ConnService : Service
{
    public ConnService(string serviceId) : base(serviceId)
    {
    }
}