namespace TrailingStopLoss.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using FakeItEasy;
    using FluentAssertions;
    using TrailingStopLoss;
    using Xbehave;

    public class Tests2
    {
        private List<object> messagesPublished;
        private IMessagePublisher messagePublisher;
        private ProcessManager processorManager;
        private Guid instrumentId;

        [Background]
        public void Background()
        {
            "Given a message publisher"
                .f(() =>
                {
                    messagesPublished = new List<object>();
                    messagePublisher = A.Fake<IMessagePublisher>();

                    A.CallTo(() => messagePublisher.Publish(A<object>.Ignored))
                        .Invokes(call =>
                        {
                            var message = (call.Arguments.First());
                            messagesPublished.Add(message);
                        });
                });

            "And a process manager"
                .f(() => processorManager = new ProcessManager(messagePublisher));

            "And an instrument ID"
                .f(() => instrumentId = Guid.NewGuid());
        }

        [Scenario]
        public void AquireAPositionAndRemoveThePriceIn10Seconds(
            IMessagePublisher messagePublisher,
            ProcessManager processorManager,
            int initialPrice,
            List<object> messagesPublished,
            Guid instrumentId)
        {
            "Given an initial price"
                .f(() => initialPrice = 12345);

            "When I acquire a position with that initial price"
                .f(() => this.processorManager.Handle(new PositionAcquired { InstrumentId = instrumentId, Price = initialPrice }));

            "Then a message is published to remove the price in 10 seconds"
                .f(() =>
                {
                    this.messagesPublished.Count.Should().Be(1);
                    var message = messagesPublished.Single() as StopLossPriceUpdated;
                    message.Should().NotBeNull();
                    message.InstrumentId.Should().Be(instrumentId);
                    message.Price.Should().Be(initialPrice);
                });
        }
    }
}
