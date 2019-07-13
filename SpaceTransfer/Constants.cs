
namespace SpaceTransfer
{
    public static class Constants
    {
        //string query key
        public static readonly string CREDITS = "Credits";
        public static readonly string IS = "is";

        //error message
        
        public static readonly string MS_ERR_VALIDATE = "Invalid format";
        public static readonly string ER_MS_UNDEFINED_UNIT = "Intergalactic unit was not defined";
        public static readonly string ER_MS_UNDEFINED_ITEM = "Trading item was not defined";
        public static readonly string ER_MS_NO_IDEA = "I have no idea what you are talking about";
       
        //success message
        public static readonly string MS_EXSUCCESS = "Exchange successfully";
        public static readonly string MS_DEFINED = "you have just added unit successfully";
        
        //path locate json data 
        public static readonly string UNITS_JS_FILE = @"C:\units.json";
        public static readonly string ITEMS_JS_FILE = @"C:\items.json";

        //regex pattern for match intergalactic unit name vs Roman number 
        public static readonly string RX_DEFINED_ROMANNAME = @"\b\w*\s*(is)\s*(I|V|X|L|C|D|M)\b";
        //regex pattern for intergalactic unit name vs trade item
        public static readonly string RX_DEFINEDTRADEITEM = @"^(\w+\s+)+(is){1}\s+([0-9]+)+\s+(Credits){1}$";
        //regex pattern for roman number
        public static readonly string RX_ROMANNUMBER = @"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";
    }
}
