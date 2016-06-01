namespace TrailingStopLoss
{
    using TrailingStopLoss.Events;

    public class ProcessManager
    {
        private readonly IMessagePublisher messagePublisher;

        public ProcessManager(IMessagePublisher messagePublisher)
        {
            Guard.Against.Null(() => messagePublisher);

            this.messagePublisher = messagePublisher;
        }

        public void Handle(PositionAcquired @event)
        {

        }

        public void Handle(PriceUpdated @event)
        {

        }

        public void Handle(StopLossHit @event)
        {

        }

        public void Handle(StopLossPriceUpdated @event)
        {

        }
    }
}
