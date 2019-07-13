

namespace SpaceTransfer
{
    public class ExchangeResult
    {
        public string Message { get; set; }
        public bool Status { get; set; }
        public decimal Result { get; set; }
        /// <summary>
        /// input string query calculate value have trading item : true, else :false
        /// </summary>
        public bool IsCredit { get; set; }
    }
}
