namespace TrailingStopLoss.Events
{
    using System;

    public class PriceUpdated
    {
        public Guid Id { get; set; }

        public int Price { get; set; }
    }
}
