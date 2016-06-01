namespace TrailingStopLoss.Events
{
    using System;

    public class StopLossPriceUpdated
    {
        public Guid InstrumentId { get; set; }

        public int Price { get; set; }
    }
}
