namespace TrailingStopLoss.Tests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave;
    using TrailingStopLoss;
    using FakeItEasy;
    using System.Collections.Generic;
    using Xunit;
    public class Tests
    {
        [Scenario]
        public void AquireAPosition(
            IMessagePublisher messagePublisher,
            ProcessManager processorManager,
            Guid instrumentId,
            int initialPrice,
            List<object> messagePublished)
        {
            "Given a message publisher"
                .f(() =>
                {
                    messagePublished = new List<object>();
                    messagePublisher = A.Fake<IMessagePublisher>();

                    A.CallTo(() => messagePublisher.Publish(A<object>.Ignored))
                    .Invokes(call => 
                    {
                        var message = (call.Arguments.First());
                        messagePublished.Add(message);
                    });
                });

            "And a process manager"
                .f(() => { processorManager = new ProcessManager(messagePublisher); });

            "And an instrument ID"
                .f(() => { instrumentId = Guid.NewGuid(); });

            "When I aquire a position"
                .f(() =>
                {
                    initialPrice = 23556;
                    processorManager.Handle(
                        new Events.PositionAcquired()
                        {
                            InstrumentId = instrumentId,
                            Price = initialPrice
                        });
                });

            "Then I publish a message to update the stop loss price"
                .f(() =>
                {
                    messagePublished.Count.Should().Be(1);
                    var command = messagePublished.Single() as Events.StopLossPriceUpdated;
                    command.Should().NotBeNull();
                    command.InstrumentId.Should().Be(instrumentId);
                    command.Price.Should().Be(initialPrice);
                });
        }

        /*[Scenario]
        public void AquireAPositionAndRemoveThePriceIn10Seconds(
            IMessagePublisher messagePublisher, 
            ProcessManager processorManager, 
            int initialPrice,
            List<object> eventsPublished)
        {
            "Given a message publisher"
                .f(() => 
                {
                    messagePublisher = A.Fake<IMessagePublisher>();
                    A.CallTo(() => messagePublisher.Publish(A<object>._)).Invokes(x => eventsPublished.Add(x));
                });

            "And a process manager"
                .f(() => { processorManager = new ProcessManager(messagePublisher); });
             
            "When I aquire a position"
                .f(() => 
                {
                    initialPrice = 23556;
                    processorManager.Handle(new Events.PositionAcquired() { Id = Guid.NewGuid(), Price = initialPrice });
                });

            "Then I publish a message to remove the price in 10 seconds"
                .f(() => 
                {
                    eventsPublished.Count.Should().Be(1);
                    var command = eventsPublished.Single() as Commands.RemoveFrom10Window;
                    command.Should().NotBeNull();
                    command.Price.Should().Be(initialPrice);
                });
        }*/
    }
}
