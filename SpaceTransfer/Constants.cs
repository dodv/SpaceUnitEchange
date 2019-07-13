using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTransfer
{
    public static class Constants
    {
        //string key
        public static readonly string CREDITS = "Credits";
        public static readonly string IS = "is";
        public static readonly string MS_DEFINED = "you have just added unit successfully";
        public static readonly string MS_ERR_VALIDATE = "Invalid format";
        public static readonly string MS_EXSUCCESS = "Exchange successfully";

        //path locate json data 
        public static readonly string UNITS_JS_FILE = @"C:\units.json";
        public static readonly string ITEMS_JS_FILE = @"C:\items.json";

        //for match intergalactic unit name vs Roman number
        public static readonly string RX_DEFINED_ROMANNAME = @"\b\w*\s*(is)\s*(I|V|X|L|C|D|M)\b";
        // intergalactic unit name vs trade item
        public static readonly string RX_DEFINEDTRADEITEM = @"^(\w+\s+)+(is){1}\s+([0-9]+)+\s+(Credits){1}$";
        public static readonly string RX_ROMANNUMBER = @"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";
    }
}
