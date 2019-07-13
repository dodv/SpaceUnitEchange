
using System.Collections.Generic;
using SpaceTransfer;

namespace WebApplication.Models
{
    public class ViewModel
    {
        public List<CurrencyUnit> ListCurrencyUnit { get; set; }
        public List<TradeItem> ListItemTrading { get; set; }
        public string InputString { get; set; }
        public decimal Result { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public bool IsCredit { get; set; }
    }
}