﻿using Newtonsoft.Json;
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

        private List<CurrencyUnit> ListCurrencyUnit { get; set; }
        private List<TradeItem> ListItemTrading { get; set; }

        //Load data for CurrencyUnit & Trading items
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

        /// <summary>
        /// save down trading units & trading items to files
        /// </summary>
        private void Save()
        {
            File.WriteAllText(Constants.ITEMS_JS_FILE, JsonConvert.SerializeObject(ListItemTrading));
            File.WriteAllText(Constants.UNITS_JS_FILE, JsonConvert.SerializeObject(ListCurrencyUnit));
        }

        /// <summary>
        /// load trading items from json file, if file not exist, return null
        /// </summary>
        /// <returns></returns>
        private List<TradeItem> LoadItemTrading()
        {
            //load from json items file
            try
            {
                var items = JsonConvert.DeserializeObject<List<TradeItem>>(File.ReadAllText(Constants.ITEMS_JS_FILE));
                return items;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// load currency from json file, if not exist, instance default with Roman numer and value
        /// </summary>
        /// <returns></returns>
        private List<CurrencyUnit> LoadCurrencyUnit()
        {
            //load from unit json file
            try
            {
                var units = JsonConvert.DeserializeObject<List<CurrencyUnit>>(File.ReadAllText(Constants.UNITS_JS_FILE));
                return units;
            }
            catch (Exception)
            {
                //instance new default, can be update by query
                return new List<CurrencyUnit>
                    {
                        new CurrencyUnit{ Value = 1, RomanNumeral = "I"},
                        new CurrencyUnit{ Value = 5, RomanNumeral = "V"},
                        new CurrencyUnit{ Value = 10, RomanNumeral = "X" },
                        new CurrencyUnit{ Value = 50, RomanNumeral = "L"},
                        new CurrencyUnit{ Value = 100, RomanNumeral = "C"},
                        new CurrencyUnit{ Value = 500, RomanNumeral = "D"},
                        new CurrencyUnit{ Value = 1000, RomanNumeral = "M"}
                    };
            }
        }

        /// <summary>
        /// validate input query string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool ValidateInput(string input)
        {
            string[] arr = input.Split(null);
            int len = arr.Length;
            string pattern = "";

            Regex regexRomanNumber = new Regex(Constants.RX_ROMANNUMBER);

            //input query purpose is defined trading item
            if (input.Contains(Constants.IS) && input.Contains(Constants.CREDITS))
            {
                Regex regexDefinedTradeItem = new Regex(Constants.RX_DEFINEDTRADEITEM);
                if (!regexDefinedTradeItem.Match(input).Success)
                {
                    return false;
                }
                //loop in galactic units, exclude after key itemtrade
                for (int i = 0; i < len - 4; i++)
                {
                    // if intergalactic unit in input string not exist in List units defined, exception throw
                    if (!ListCurrencyUnit.Any(u => u.GalaxyUnit == arr[i]))
                    {

                        throw new ArgumentException($"{Constants.ER_MS_UNDEFINED_UNIT}: {arr[i]}");
                    }
                    //else if intergalactic unit is valid, convert it to Roman numeral 
                    arr[i] = ListCurrencyUnit.FirstOrDefault(u => u.GalaxyUnit == arr[i]).RomanNumeral;
                }
                Array.Resize(ref arr, len - 4);
                pattern = string.Join("", arr);

                if (!regexRomanNumber.Match(pattern).Success)
                {
                    return false;
                }
                return true;
            }

            //input query string is use to mapping intergalactic unit name with roman numeral
            if (input.Contains(Constants.IS) && !input.Contains(Constants.CREDITS))
            {
                //just defined roman number
                Regex regexDefinedRomanNumber = new Regex(Constants.RX_DEFINED_ROMANNAME);
                return regexDefinedRomanNumber.Match(input).Success;
            }

            //input query string is use to calculate value 
            //loop for unit arrays
            for (int i = 0; i < len; i++)
            {
                //if this unit in query is not defined
                if (!ListCurrencyUnit.Any(p => p.GalaxyUnit == arr[i]))
                {
                    //check last item in query input, for trading item is it?
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
                                throw new ArgumentException($"{Constants.ER_MS_UNDEFINED_ITEM}: {arr[i]}");
                            }
                        }
                        else
                        {
                            //it's not unit or item.
                            throw new ArgumentException(Constants.ER_MS_NO_IDEA);
                        }
                    }
                    // if this item in query in middle of string query input, and it is not defined, then throw exception
                    if (i != len - 1)
                    {
                        throw new ArgumentException($"{Constants.ER_MS_UNDEFINED_UNIT}");
                    }
                }
                else
                {
                    arr[i] = ListCurrencyUnit.FirstOrDefault(u => u.GalaxyUnit == arr[i]).RomanNumeral;
                }
            }
            pattern = string.Join("", arr);
            //just validate romman number
            return regexRomanNumber.Match(pattern).Success;
        }

        /// <summary>
        /// Defined currency Unit and Trade item information
        /// </summary>
        /// <param name="def"></param>
        private void CurrencyUnitDefined(string def)
        {
            string[] arr = def.Split(null);
            int len = arr.Length;

            if (len == 3 && arr[len - 2].Equals(Constants.IS))
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

            if (len > 4 && arr[len - 1].Equals(Constants.CREDITS) && arr[len - 3].Equals(Constants.IS))
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
                if (intergalacticString.Contains(Constants.IS) || intergalacticString.Contains(Constants.CREDITS))
                {
                    CurrencyUnitDefined(intergalacticString);
                    return new ExchangeResult { Message = Constants.MS_DEFINED, Status = true };
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
                            throw new ArgumentException(Constants.ER_MS_NO_IDEA);
                        }
                        Array.Resize(ref intergalaticArr, len - 1);
                    }
                }

                var arrCur = ConvertIntergalacticToArrayUnit(intergalaticArr);
                var calc = CalculateValueOfCurrencyArray(arrCur) * itemRateChange;
                return new ExchangeResult { Message = Constants.MS_EXSUCCESS, Status = true, Result = calc, IsCredit = isCredit };
            }
            return new ExchangeResult { Message = Constants.MS_ERR_VALIDATE, Status = false };
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
                arr[i] = ListCurrencyUnit.FirstOrDefault(p => p.GalaxyUnit == intergalacticUnitString[i].ToString());
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
