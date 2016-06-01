namespace TrailingStopLoss.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands;
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
        public void AquireAPosition(int initialPrice)
        {
            "Given an initial price"
                .f(() => initialPrice = 12345);

            "When I acquire a position with that initial price"
                .f(() => this.processorManager.Handle(new PositionAcquired { InstrumentId = instrumentId, Price = initialPrice }));

            "Then a message is published update the stop loss price"
                .f(() =>
                {
                    var message = (StopLossPriceUpdated)this.messagesPublished[0];
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(initialPrice);
                });

            "And a message is published to remove the price in 10 seconds"
                .f(() =>
                {
                    var callback = (SendToMeIn)this.messagesPublished[1];
                    var message = (RemoveFrom10sWindow)callback.Message;
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(initialPrice);
                });

            "And a message is published to remove the price in 13 seconds"
                .f(() =>
                {
                    var callback = (SendToMeIn)this.messagesPublished[2];
                    var message = (RemoveFrom13sWindow)callback.Message;
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(initialPrice);
                });
        }

        [Scenario]
        public void AquireAPositionAndPriceUpdated(int initialPrice, int secondPrice)
        {
            "Given an initial price"
                .f(() => initialPrice = 10);

            "Given an second price"
                .f(() => secondPrice = 20);

            "When I acquire a position with that initial price"
                .f(() => this.processorManager.Handle(new PositionAcquired { InstrumentId = instrumentId, Price = initialPrice }));

            "And I clear the published messages"
                .f(() => this.messagesPublished.Clear());

            "And I get a price update"
                .f(() => this.processorManager.Handle(new PriceUpdated { InstrumentId = instrumentId, Price = secondPrice }));

            "Then a message is published to remove the second price in 10 seconds"
                .f(() =>
                {
                    var callback = (SendToMeIn)this.messagesPublished[1];
                    var message = (RemoveFrom10sWindow)callback.Message;
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(secondPrice);
                });

            "And a message is published to remove the second price in 13 seconds"
                .f(() =>
                {
                    var callback = (SendToMeIn)this.messagesPublished[2];
                    var message = (RemoveFrom13sWindow)callback.Message;
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(secondPrice);
                });
        }

        [Scenario]
        public void AquireAPositionAndPriceUpdatedAnd10sWindowHit(int initialPrice, int secondPrice)
        {
            "Given an initial price"
                .f(() => initialPrice = 10);

            "Given an second price"
                .f(() => secondPrice = 20);

            "When I acquire a position with that initial price"
                .f(() => this.processorManager.Handle(new PositionAcquired { InstrumentId = instrumentId, Price = initialPrice }));

            "And I get a price update"
                .f(() => this.processorManager.Handle(new PriceUpdated { InstrumentId = instrumentId, Price = secondPrice }));

            "And I clear the published messages"
                .f(() => this.messagesPublished.Clear());

            "And I get a price update"
                .f(() => this.processorManager.Handle(new RemoveFrom10sWindow { InstrumentId = instrumentId, Price = initialPrice }));

            "Then a message is published to update the stop loss price"
                .f(() =>
                {
                    var message = (StopLossPriceUpdated)this.messagesPublished[0];
                    message.InstrumentId.Should().Be(this.instrumentId);
                    message.Price.Should().Be(secondPrice);
                });
        }

    }
}
