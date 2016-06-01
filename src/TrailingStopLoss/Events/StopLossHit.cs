namespace TrailingStopLoss.Events
{
    using System;

    public class StopLossHit
    {
        public Guid InstrumentId { get; set; }
    }
}
