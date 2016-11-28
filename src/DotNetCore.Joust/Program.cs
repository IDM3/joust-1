using System;
using System.Linq;
using System.IO;

namespace DotNetCore.Joust
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OrderFulfiller orderFiller = new OrderFulfiller();
            bool takingOrders = true;
            do
            {
                string input = AskQuestion("Place carpet order?", "Yes", "No");
                if(input == "Yes")
                {
                    Console.WriteLine("Please provide the order specifications.");
                    int squareFootageNeeded = GetIntInput("square footage needed", 0);
                    int roomsBeingCarpeted = GetIntInput("rooms to be carpeted", 0);
                    int hourlyLabor = GetIntInput("hourly labor cost", 0);
                    int grade = GetIntInput("quality grade", 1, 9);
                    IQuote rawQuote = orderFiller.GetQuote(squareFootageNeeded, roomsBeingCarpeted, hourlyLabor, grade);
                    Quote quote = (Quote)rawQuote;
                    Console.Clear();                    
                    string filePath = AppContext.BaseDirectory + "\\OrderHistory.txt";
                    if(quote != null)
                    {
                        string[] suppliers = quote.CarpetsInOrder.Select(x => x.Supplier.Name).Distinct().ToArray();
                        Console.WriteLine("The Cheapest Quote is {0}", quote.Price.ToString("$0.00"));
                        Console.WriteLine("Order the following");
                        foreach(string supplier in suppliers)
                        {
                            var carpetForSupplier = 
                                string.Join(", ", quote.CarpetsInOrder
                                    .Where(x => x.Supplier.Name == supplier).Select(x => x.InventoryId));
                            Console.WriteLine("{0}: {1}", supplier, carpetForSupplier);
                        }
                        File.AppendAllText(filePath,
                            $"[{squareFootageNeeded}, {roomsBeingCarpeted}, {hourlyLabor}, {grade}] {quote.Price.ToString("$0.00")} ({string.Join(", ", quote.RollOrders)}){Environment.NewLine}");
                    }
                    else
                    {
                        Console.WriteLine("Current carpet inventory can not support this order.");
                        File.AppendAllText(filePath,
                            $"[{squareFootageNeeded}, {roomsBeingCarpeted}, {hourlyLabor}, {grade}] Unable to fill order.{Environment.NewLine}");
                    }
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey(false);
                }
                else
                {
                    Console.WriteLine("Good Bye");
                    takingOrders = false;
                }
            }
            while (takingOrders);
        }

        public static int GetIntInput(string inputFor, int minimumRange = int.MinValue, int maximumRange = int.MaxValue)
        {
            bool validEntry = true;
            int enteredValue = 0;
            do
            {
                if(!validEntry)
                {
                    Console.WriteLine("Invalid Entry please try again.");
                }
                Console.Write("Please input a value for {0} ({1} - {2}) then press enter: ", inputFor, minimumRange, maximumRange);
                string input = Console.ReadLine();
                if(int.TryParse(input, out enteredValue))
                {
                    if(enteredValue < minimumRange || enteredValue > maximumRange)
                    {
                        validEntry = false;
                    }
                    else
                    {
                        validEntry = true;
                    }
                }
                else
                {
                    validEntry = false;
                }
            }
            while (!validEntry);
            return enteredValue;
        }

        public static string AskQuestion(string question, params string[] options)
        {
            bool validEntry = true;
            string input = "";
            do
            {
                Console.Clear();
                Console.WriteLine(question);
                for(int i =  1; i <= options.Length; i++)
                {
                    Console.WriteLine(i + ") " + options[i - 1]);
                }
                if(validEntry)
                {
                    Console.Write("Type entry then press Enter: ");
                }
                else
                {
                    Console.Write("Invalid entry, try again then press Enter: ");
                }
                input = Console.ReadLine();
                int selectedOption;
                if(int.TryParse(input, out selectedOption))
                {
                    selectedOption -= 1;
                    if(selectedOption >= 0 && selectedOption < options.Length)
                    {
                        input = options[selectedOption];
                        validEntry = true;
                    }
                    else
                    {
                        validEntry = false;
                    }
                }
                else
                {
                    validEntry = false;
                }
            }
            while(!validEntry);
            return input;
        }
    }
}
