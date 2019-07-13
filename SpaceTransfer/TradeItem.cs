
namespace SpaceTransfer
{
    public class TradeItem
    {
        /// <summary>
        /// trading item name : ex gold, iron ..
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// trading item rate per unit, ex: 1 gold = 10 Credits , rate is 10
        /// </summary>
        public decimal RatePerUnit { get; set; }
    }
}
