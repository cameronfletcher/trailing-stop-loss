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
    public class Tests
    {
        [Scenario]
        public void AquireAPosition(
            IMessagePublisher messagePublisher,
            ProcessManager processorManager,
            Guid instrumentId,
            int initialPrice,
            List<object> messagePublished,
            int dummyPrice)
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
                    messagePublished.Count.Should().Be(3);

                    var stopLossPriceUpdatedMessage = messagePublished[0] as Events.StopLossPriceUpdated;
                    stopLossPriceUpdatedMessage.Should().NotBeNull();
                    stopLossPriceUpdatedMessage.InstrumentId.Should().Be(instrumentId);
                    stopLossPriceUpdatedMessage.Price.Should().Be(initialPrice);

                    var sendMeInCommand1 = messagePublished[1] as Commands.SendToMeIn;
                    sendMeInCommand1.Should().NotBeNull();
                    var removeIn10Seconds = sendMeInCommand1.Message as Commands.RemoveFrom10sWindow;
                    removeIn10Seconds.Should().NotBeNull();

                    removeIn10Seconds.InstrumentId.Should().Be(instrumentId);
                    removeIn10Seconds.Price.Should().Be(initialPrice);

                    var sendMeInCommand2 = messagePublished[2] as Commands.SendToMeIn;
                    sendMeInCommand2.Should().NotBeNull();
                    var removeIn13Seconds = sendMeInCommand2.Message as Commands.RemoveFrom13sWindow;
                    removeIn13Seconds.Should().NotBeNull();

                    removeIn13Seconds.InstrumentId.Should().Be(instrumentId);
                    removeIn13Seconds.Price.Should().Be(initialPrice);
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
