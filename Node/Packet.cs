namespace Node
{
    public class Packet
    {
        public string From { get; set; }

        public string Message { get; set; }

        public static Packet Parse(string data)
        {
            var delimIndex = data.IndexOf(':');
            var name = data.Substring(0, delimIndex);

            return new Packet
            {
                From = name,
                Message = data.Substring(delimIndex+2)
            };
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", From, Message);
        }
    }
}
