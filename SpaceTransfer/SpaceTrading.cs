using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpaceTransfer
{
    public sealed class SpaceTrading : ISpaceTrading
    {
        static readonly SpaceTrading instance = new SpaceTrading();
        static SpaceTrading() { }
        SpaceTrading()
        {
            LoadData();
        }
        public static SpaceTrading Instance
        {
            get
            {
                return instance;
            }
        }
        //string key
        private const string CREDITS = "Credits";
        private const string IS = "is";
        private const string MS_DEFINED = "you have just added unit successfully";
        private const string MS_ERR_VALIDATE = "Invalid format";
        private const string MS_EXSUCCESS = "Exchange successfully";

        private const string UNITS_JS_FILE = @"C:\units.json";
        private const string ITEMS_JS_FILE = @"C:\items.json";

        //for match intergalactic unit name vs Roman number
        private const string RX_DEFINED_ROMANNAME = @"\b\w*\s*(is)\s*(I|V|X|L|C|D|M)\b";
        // intergalactic unit name vs trade item
        private const string RX_DEFINEDTRADEITEM = @"^(\w+\s+)+(is){1}\s+([0-9]+)+\s+(Credits){1}$";
        private const string RX_ROMANNUMBER = @"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";


        private List<CurrencyUnit> ListCurrencyUnit { get; set; }
        private List<TradeItem> ListItemTrading { get; set; }

        private void LoadData()
        {
            ListCurrencyUnit = LoadCurrencyUnit();
            ListItemTrading = LoadItemTrading();
        }
        /// <summary>
        /// use for test only
        /// </summary>
        public void ResetData()
        {
            ListCurrencyUnit = new List<CurrencyUnit>
                    {
                        new CurrencyUnit{ Value = 1, RomanNumeral = "I"},
                        new CurrencyUnit{ Value = 5, RomanNumeral = "V"},
                        new CurrencyUnit{ Value = 10, RomanNumeral = "X" },
                        new CurrencyUnit{ Value = 50, RomanNumeral = "L"},
                        new CurrencyUnit{ Value = 100, RomanNumeral = "C"},
                        new CurrencyUnit{ Value = 500, RomanNumeral = "D"},
                        new CurrencyUnit{ Value = 1000, RomanNumeral = "M"}
                    };
            ListItemTrading = null;
        }
        private void Save()
        {
            File.WriteAllText(ITEMS_JS_FILE, JsonConvert.SerializeObject(ListItemTrading));
            File.WriteAllText(UNITS_JS_FILE, JsonConvert.SerializeObject(ListCurrencyUnit));
        }

        private List<TradeItem> LoadItemTrading()
        {
            //load from json
            List<TradeItem> items;
            try
            {
                items = JsonConvert.DeserializeObject<List<TradeItem>>(File.ReadAllText(ITEMS_JS_FILE));
                return items;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private List<CurrencyUnit> LoadCurrencyUnit()
        {
            //load from json
            List<CurrencyUnit> units;
            try
            {
                units = JsonConvert.DeserializeObject<List<CurrencyUnit>>(File.ReadAllText(UNITS_JS_FILE));
                return units;
            }
            catch (Exception)
            {

                //instance new default, can be update by query
                units = new List<CurrencyUnit>
                    {
                        new CurrencyUnit{ Value = 1, RomanNumeral = "I"},
                        new CurrencyUnit{ Value = 5, RomanNumeral = "V"},
                        new CurrencyUnit{ Value = 10, RomanNumeral = "X" },
                        new CurrencyUnit{ Value = 50, RomanNumeral = "L"},
                        new CurrencyUnit{ Value = 100, RomanNumeral = "C"},
                        new CurrencyUnit{ Value = 500, RomanNumeral = "D"},
                        new CurrencyUnit{ Value = 1000, RomanNumeral = "M"}
                    };

                return units;
            }
        }

        public bool ValidateInput(string input)
        {
            string[] arr = input.Split(null);
            int len = arr.Length;
            string pattern = "";

            //
            if (input.Contains(IS) && input.Contains(CREDITS))
            {
                Regex regexDefinedTradeItem = new Regex(RX_DEFINEDTRADEITEM);
                if (!regexDefinedTradeItem.Match(input).Success)
                {
                    return false;
                }
                //loop in galactic units, exclude after key itemtrade
                for (int i = 0; i < len - 4; i++)
                {
                    if (!ListCurrencyUnit.Any(u => u.GalaxyUnit == arr[i]))
                    {

                        throw new ArgumentException($"intergalactic unit was not defined: {arr[i]}");
                    }
                    else
                    {
                        arr[i] = ListCurrencyUnit.FirstOrDefault(u => u.GalaxyUnit == arr[i]).RomanNumeral;
                    }
                }
                Array.Resize(ref arr, len - 4);
                pattern = string.Join("", arr);
                //convert galactic unit to Roman number 
                Regex regexRomanNumber = new Regex(RX_ROMANNUMBER);
                if (!regexRomanNumber.Match(pattern).Success)
                {
                    return false;
                }
                return true;
            }
            else if (input.Contains(IS) && !input.Contains(CREDITS))
            {
                //just defined roman number
                Regex regexDefinedRomanNumber = new Regex(RX_DEFINED_ROMANNAME);
                return regexDefinedRomanNumber.Match(input).Success;
            }
            else
            {
                for (int i = 0; i < len; i++)
                {

                    if (!ListCurrencyUnit.Any(p => p.GalaxyUnit == arr[i]))
                    {
                        if (i == len - 1)
                        {
                            //this is itemtrading if it exist, else exception
                            if (ListItemTrading != null)
                            {
                                if (ListItemTrading.Any(p => p.ItemName == arr[i]))
                                {
                                    Array.Resize(ref arr, len - 1);
                                }
                                else
                                {
                                    throw new ArgumentException($"trading item was not defined: {arr[i]}");
                                }
                            }
                            else
                            {
                                //it's not unit or item.
                                throw new ArgumentException("I have no idea what you are talking about");
                            }

                        }

                        if (i != len - 1)
                        {
                            throw new ArgumentException($"intergalactic unit was not defined: {arr[i]}");
                        }
                    }
                    else
                    {
                        arr[i] = ListCurrencyUnit.FirstOrDefault(u => u.GalaxyUnit == arr[i]).RomanNumeral;
                    }
                }
                pattern = string.Join("", arr);
                //just validate romman number
                Regex regexRomanNumber = new Regex(RX_ROMANNUMBER);
                return regexRomanNumber.Match(pattern).Success;
            }

        }

        /// <summary>
        /// Defined currency Unit and Trade item information
        /// </summary>
        /// <param name="def"></param>
        private void CurrencyUnitDefined(string def)
        {
            string[] arr = def.Split(null);
            int len = arr.Length;

            if (len == 3 && arr[len - 2].Equals(IS))
            {
                //defined name of roman number
                foreach (var cr in ListCurrencyUnit)
                {
                    if (cr.RomanNumeral.Equals(arr[len - 1]))
                    {
                        cr.GalaxyUnit = arr[0];
                    }
                }
            }

            if (len > 4 && arr[len - 1].Equals(CREDITS) && arr[len - 3].Equals(IS))
            {
                //defined or update trade item rate
                decimal total = Convert.ToDecimal(arr[len - 2]);
                string tradeItem = arr[len - 4];
                Array.Resize(ref arr, len - 4);

                string galacticUnit = string.Join(" ", arr);
                var calculateIntergalatic = CalculateInterglacticUnitValue(galacticUnit);

                var unitRate = Math.Round(total / calculateIntergalatic, 3);
                if (ListItemTrading != null)
                {
                    if (ListItemTrading.Any(i => i.ItemName == tradeItem)) //update exist trading item
                    {
                        var item = ListItemTrading.FirstOrDefault(i => i.ItemName == tradeItem);
                        item.RatePerUnit = unitRate;
                    }
                    else  //add new trading item
                    {
                        ListItemTrading.Add(new TradeItem { ItemName = tradeItem, RatePerUnit = unitRate });
                    }
                }
                else //instance ListItemTrading, add new trading item
                {
                    ListItemTrading = new List<TradeItem>();
                    ListItemTrading.Add(new TradeItem { ItemName = tradeItem, RatePerUnit = unitRate });
                }
            }
            Save();
        }

        /// <summary>
        /// exchange intergalactic unit string 
        /// </summary>
        /// <param name="intergalacticString"></param>
        /// <returns></returns>
        public ExchangeResult ExchangeSpaceCredits(string intergalacticString)
        {
            //ignore multipe whitespace is accepted
            intergalacticString = Regex.Replace(intergalacticString.Trim(), " {2,}", " ");

            bool validateString = ValidateInput(intergalacticString);
            bool isCredit = false;
            if (validateString)
            {
                if (intergalacticString.Contains(IS) || intergalacticString.Contains(CREDITS))
                {
                    CurrencyUnitDefined(intergalacticString);
                    return new ExchangeResult { Message = MS_DEFINED, Status = true };
                }

                var intergalaticArr = intergalacticString.Split(null);
                int len = intergalaticArr.Length;
                decimal itemRateChange = 1;


                if (ListItemTrading != null)
                {
                    //if item tradding in query, find it rate.
                    if (ListItemTrading.Any(i => i.ItemName == intergalaticArr[len - 1]))
                    {
                        isCredit = true;
                        itemRateChange = ListItemTrading.FirstOrDefault(i => i.ItemName == intergalaticArr[len - 1]).RatePerUnit;
                        if (len == 1)
                        {
                            //query input is item trading, throw argument exception
                            throw new ArgumentException($"Invalid format, How many {intergalaticArr[len - 1]} you wanna exchange to credit?");
                        }
                        Array.Resize(ref intergalaticArr, len - 1);
                    }
                }

                var arrCur = ConvertIntergalacticToArrayUnit(intergalaticArr);
                var calc = CalculateValueOfCurrencyArray(arrCur) * itemRateChange;
                return new ExchangeResult { Message = MS_EXSUCCESS, Status = true, Result = calc, IsCredit = isCredit };
            }
            return new ExchangeResult { Message = MS_ERR_VALIDATE, Status = false };
        }

        /// <summary>
        /// calculate value of interglactic unit string
        /// </summary>
        /// <param name="intergalacticString"> parameter is validated </param>
        /// <returns></returns>
        private decimal CalculateInterglacticUnitValue(string intergalacticString)
        {
            var intergalaticArr = intergalacticString.Split(null);
            var arrCur = ConvertIntergalacticToArrayUnit(intergalaticArr);
            var calc = CalculateValueOfCurrencyArray(arrCur);
            return calc;
        }

        /// <summary>
        /// convert string array intergalactic to array of CurrencyUnit 
        /// </summary>
        /// <param name="intergalacticUnitString"></param>
        /// <returns></returns>
        public CurrencyUnit[] ConvertIntergalacticToArrayUnit(string[] intergalacticUnitString)
        {
            CurrencyUnit[] arr = new CurrencyUnit[intergalacticUnitString.Length];

            for (int i = 0; i < intergalacticUnitString.Length; i++)
            {
                arr[i] = ListCurrencyUnit.Where(p => p.GalaxyUnit.Equals(intergalacticUnitString[i].ToString())).First();
            }
            return arr;
        }

        /// <summary>
        /// calculate value of array intergalactic unit.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int CalculateValueOfCurrencyArray(CurrencyUnit[] c)
        {
            var total = c[0].Value;
            int len = c.Length;
            for (int i = 1; i < len; i++)
            {
                if (c[i].Value > c[i - 1].Value)
                {
                    total = c[i].Value - total;
                }
                else if (c[i].Value == c[i - 1].Value)
                {
                    total += c[i].Value;
                }
                else // next number is smaller
                {
                    //check next number if greater, ex: case XIV = X + (V-I)
                    if (i + 1 < len && c[i + 1].Value > c[i].Value)
                    {
                        total += c[i + 1].Value - c[i].Value;
                        i = i + 1;
                    }
                    else
                    {
                        total += c[i].Value;
                    }
                }
            }
            return total;
        }

        public string CurrencyArrayToRomanString(CurrencyUnit[] cr)
        {
            var romanArr = cr.Select(p => p.RomanNumeral).ToArray();
            return string.Join("", romanArr);
        }

        public List<CurrencyUnit> GetAllCurrencyUnit()
        {
            return ListCurrencyUnit;
        }
        public List<TradeItem> GetAllItemsTrading()
        {
            return ListItemTrading;
        }
    }
}
