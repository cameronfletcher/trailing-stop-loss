namespace TrailingStopLoss.Events
{
    using System;

    public class PositionAcquired
    {
        public Guid Id { get; set; }

        public int Price { get; set; }
    }
}
