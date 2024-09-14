using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Node
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Input your nickname: ");
            var name = Console.ReadLine();

            try
            {
                var node = new Node(name);
                node.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }
    }
}
