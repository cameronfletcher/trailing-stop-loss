namespace TrailingStopLoss.Commands
{
    using System;

    public class RemoveFrom10sWindow
    {
        public Guid InstrumentId { get; set; }

        public int Price { get; set; }
    }
}
