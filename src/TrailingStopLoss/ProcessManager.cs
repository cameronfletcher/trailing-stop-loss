namespace TrailingStopLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands;
    using TrailingStopLoss.Events;

    public class ProcessManager
    {
        // TODO (Cameron): This type is wrong.
        private readonly Dictionary<Guid, List<int>> allWindows10s = new Dictionary<Guid, List<int>>();
        private readonly Dictionary<Guid, List<int>> allWindows13s = new Dictionary<Guid, List<int>>();

        private readonly IMessagePublisher messagePublisher;

        public ProcessManager(IMessagePublisher messagePublisher)
        {
            Guard.Against.Null(() => messagePublisher);

            this.messagePublisher = messagePublisher;
        }

        public void Handle(PositionAcquired @event)
        {
            this.allWindows10s.Add(@event.Id, new List<int> { @event.Price });
            this.allWindows13s.Add(@event.Id, new List<int> { @event.Price });

            this.messagePublisher.Publish(new StopLossPriceUpdated { Id = @event.Id, Price = @event.Price });
        }

        public void Handle(PriceUpdated @event)
        {
            this.allWindows10s[@event.Id].Add(@event.Price);
            this.allWindows13s[@event.Id].Add(@event.Price);

            this.messagePublisher.Publish(new StopLossPriceUpdated { Id = @event.Id, Price = @event.Price });
            this.messagePublisher.Publish(new SendToMeIn { Seconds = 10, Message = new RemoveFrom10sWindow { Id = @event.Id, Price = @event.Price } });
            this.messagePublisher.Publish(new SendToMeIn { Seconds = 13, Message = new RemoveFrom10sWindow { Id = @event.Id, Price = @event.Price } });
        }

        public void Handle(RemoveFrom10sWindow @event)
        {
            var list = this.allWindows10s[@event.Id];
            var index = list.IndexOf(list.First(p => p == @event.Price));

            list.RemoveAt(index);
        }

        public void Handle(RemoveFrom13sWindow @event)
        {
            var list = this.allWindows13s[@event.Id];
            var index = list.IndexOf(list.First(p => p == @event.Price));

            list.RemoveAt(index);
        }
    }
}
