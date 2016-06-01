namespace TrailingStopLoss
{
    using System.Collections.Generic;
    using System.Linq;
    using Commands;
    using TrailingStopLoss.Events;

    public class ProcessManager
    {
        // TODO (Cameron): This type is wrong.
        private readonly List<int> window10s = new List<int>();
        private readonly List<int> window13s = new List<int>();

        private readonly IMessagePublisher messagePublisher;

        public ProcessManager(IMessagePublisher messagePublisher)
        {
            Guard.Against.Null(() => messagePublisher);

            this.messagePublisher = messagePublisher;
        }

        public void Handle(PositionAcquired @event)
        {
            this.window10s.Add(@event.Price);
            this.window13s.Add(@event.Price);

            this.messagePublisher.Publish(new StopLossPriceUpdated { Price = @event.Price });
        }

        public void Handle(PriceUpdated @event)
        {
            this.window10s.Add(@event.Price);
            this.window13s.Add(@event.Price);

            this.messagePublisher.Publish(new StopLossPriceUpdated { Price = @event.Price });
        }

        public void Handle(RemoveFrom10sWindow @event)
        {
            var index = this.window10s.IndexOf(this.window10s.First(p => p == @event.Price));
            this.window10s.RemoveAt(index);
        }

        public void Handle(RemoveFrom13sWindow @event)
        {
            var index = this.window13s.IndexOf(this.window13s.First(p => p == @event.Price));
            this.window13s.RemoveAt(index);
        }
    }
}
