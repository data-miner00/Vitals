namespace Vitals.Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEventPublisher
{
    Task PublishAsync(object @event, CancellationToken cancellationToken);
}
