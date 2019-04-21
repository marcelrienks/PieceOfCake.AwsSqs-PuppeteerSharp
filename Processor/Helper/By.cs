namespace Processor.Helper
{
    public class By
    {
        public string Selector { get; set; }

        public By(string selector)
        {
            Selector = selector;
        }

        public static By Type(string typeToFind)
        {
            return new By($"{typeToFind}");
        }

        public static By Css(string cssToFind)
        {
            return new By($"{cssToFind}");
        }

        public static By ClassName(string classNameToFind)
        {
            return new By($".{classNameToFind}");
        }

        public static By Id(string idToFind)
        {
            return new By($"#{idToFind}");
        }

        public static By Name(string nameToFind)
        {
            return new By($"input[name=\"{ nameToFind}\"]");
        }

        public static By Attribute(string attributeToFind)
        {
            return new By($"[{attributeToFind}]");
        }
    }
}
