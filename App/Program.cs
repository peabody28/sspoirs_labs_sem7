using Ping;
using System.Net;

namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            var pinger = new Pinger();
            while(true)
            {
                var action = SelectActionMessage();

                IPAddress address = null;
                string pingerName = null;

                if(action == SelectAction.StartPing || action == SelectAction.SwitchPingConsole || action == SelectAction.StopPing)
                {
                    Console.Write("Input pinger name: ");
                    pingerName = Console.ReadLine();
                }

                if(action == SelectAction.StartPing || action == SelectAction.StartTraceroute || action == SelectAction.SmurfAttack)
                {
                    Console.Write("Input target address: ");
                    address = IPAddress.Parse(Console.ReadLine());
                }

                if (action == SelectAction.StartPing)
                {
                    pinger.Create(pingerName, address);
                }
                else if(action == SelectAction.SwitchPingConsole)
                {
                    pinger.SwitchOutput(pingerName);
                }
                else if(action == SelectAction.StopPing)
                {
                    pinger.Stop(pingerName);
                }
                else if(action == SelectAction.ListPingers)
                {
                    foreach(var activePinger in pinger.GetActivePingers())
                    {
                        Console.WriteLine($"Name: {activePinger}");
                    }
                }
                else if(action == SelectAction.StartTraceroute)
                {
                    new Tracerouter().Start(address);
                }
                else if(action == SelectAction.SmurfAttack)
                {
                    Console.Write("Input router address: ");
                    var dest = IPAddress.Parse(Console.ReadLine());

                    new SmurfAttacker().Start(address, dest, TimeSpan.FromMicroseconds(10));
                }
                else if(action == SelectAction.Exit)
                {
                    break;
                }
            }

            Console.WriteLine("Press any key to exit....");
            Console.ReadKey();
        }

        static SelectAction SelectActionMessage()
        {
            Console.WriteLine();
            Console.WriteLine("1 - ping host");
            Console.WriteLine("2 - switch console from pinger");
            Console.WriteLine("3 - stop ping");
            Console.WriteLine("4 - pingers list");
            Console.WriteLine("5 - traceroute host");
            Console.WriteLine("6 - smurf host");
            Console.WriteLine("7 - exit");
            Console.Write("Input action: ");

            var action = Convert.ToInt32(Console.ReadLine());

            return (SelectAction)action;
        }
    }
}
