﻿namespace TrailingStopLoss.Events
{
    using System;

    public class PriceUpdated
    {
        public Guid InstrumentId { get; set; }

        public int Price { get; set; }
    }
}
