

namespace SpaceTransfer
{
    public class ExchangeResult
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public decimal Result { get; set; }

        public bool IsCredit { get; set; }
    }
}
