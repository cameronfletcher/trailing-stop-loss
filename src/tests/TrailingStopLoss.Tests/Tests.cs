namespace TrailingStopLoss.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave;
    using TrailingStopLoss;
    using FakeItEasy;
    using Xunit;
    using Events;
    using Commands;
    public class Tests
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
        public void StopLossTriggerred(int initialPrice, int secondPrice, int thirdPrice)
        {
            "Given an initial price"
                .f(() => initialPrice = 10);

            "Given an second price"
                .f(() => secondPrice = 9);

            "Given an second price"
                .f(() => thirdPrice = 8);

            "When I acquire a position with that initial price"
                .f(() => this.processorManager.Handle(new PositionAcquired { InstrumentId = instrumentId, Price = initialPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new PriceUpdated { InstrumentId = instrumentId, Price = secondPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new PriceUpdated { InstrumentId = instrumentId, Price = thirdPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom10sWindow { InstrumentId = instrumentId, Price = initialPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom10sWindow { InstrumentId = instrumentId, Price = secondPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom10sWindow { InstrumentId = instrumentId, Price = thirdPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom13sWindow { InstrumentId = instrumentId, Price = initialPrice }));

            "And I clear the published messages"
                .f(() => this.messagesPublished.Clear());

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom13sWindow { InstrumentId = instrumentId, Price = secondPrice }));
  

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom13sWindow { InstrumentId = instrumentId, Price = thirdPrice }));

            "Then a message is published to update the stop loss price"
                .f(() =>
                {
                    var message = (StopLossHit)this.messagesPublished[0];
                    message.InstrumentId.Should().Be(this.instrumentId);
                });
        }
    }
}
