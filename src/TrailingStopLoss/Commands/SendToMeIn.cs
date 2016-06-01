namespace TrailingStopLoss.Commands
{
    using System;

    public class SendToMeIn
    {
        public Guid Id { get; set; }

        public int Seconds { get; set; }

        public object Message { get; set; }
    }
}
