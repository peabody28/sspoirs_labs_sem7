namespace Node
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Input your nickname: ");
            var name = Console.ReadLine();

            var node = new Node(name);
            try
            {
                node.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                node.Dispose();
            }

            Console.WriteLine("Bye! Press Any Key");
            Console.ReadKey();
        }
    }
}
