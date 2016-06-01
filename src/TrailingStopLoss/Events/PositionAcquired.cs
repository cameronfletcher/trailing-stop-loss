namespace TrailingStopLoss.Events
{
    using System;

    public class PositionAcquired
    {
        public Guid InstrumentId { get; set; }

        public int Price { get; set; }
    }
}
