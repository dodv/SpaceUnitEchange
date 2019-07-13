using System.Collections.Generic;

namespace SpaceTransfer
{
    interface ISpaceTrading
    {
        bool ValidateInput(string input);
        ExchangeResult ExchangeSpaceCredits(string intergalacticString);

        string CurrencyArrayToRomanString(CurrencyUnit[] cr);
        List<CurrencyUnit> GetAllCurrencyUnit();
        List<TradeItem> GetAllItemsTrading();
    }
}
