using System;

namespace DotNetCore.Joust
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OrderFulfiller test = new OrderFulfiller();
            var exampleQuote = test.GetQuote(new int[] { 100,100,100,7});
            Console.WriteLine($"Hello world!");
            Console.ReadKey();
        }
    }
}
