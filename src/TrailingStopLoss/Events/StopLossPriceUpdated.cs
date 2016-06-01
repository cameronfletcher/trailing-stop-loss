namespace TrailingStopLoss.Events
{
    using System;

    public class StopLossPriceUpdated
    {
        public Guid Id { get; set; }

        public int Price { get; set; }
    }
}
