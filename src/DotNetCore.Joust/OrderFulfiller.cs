using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace DotNetCore.Joust
{
    public class OrderFulfiller : IJoust
    {
        public IQuote GetQuote(int[] input)
        {
            if(input.Length != 4)
            {
                throw new InvalidDataException("Invalid data to make a quote");
            }
            //get order details
            Order details = new Order();
            //first input is square footage needed
            details.SquareFootageNeeded = input[0];
            //second input is number of rooms
            details.NumberOfRooms = input[1];
            //thirdsecond input is hourly labor cost
            details.HourlyLaborCost = input[2];
            //fourth input is carget grade needed
            details.DesiredCarpetGrade = input[3];
            //get supplier details
            string dataLocation = GetDataLoction();
            SupplierRepository Rep = new SupplierRepository(dataLocation);
            //get valid carpet selection
            Func<Carpet, bool> isRightGrade = carpet => carpet.Grade == details.DesiredCarpetGrade;
            List<Carpet> carpetSelection = Rep.SearchInventory(isRightGrade).OrderByDescending(carpet => carpet.SquareFootage).ToList();


            List<Quote> quotes = new List<Quote>();
            //get all quotes where one carpet can fulfill order

            List<Carpet[]> validCarpetCombinations = GetPotentialCarpetPair(carpetSelection, details.SquareFootageNeeded);
            Func<Carpet[], Quote> quoteFromCarpetSelection = carpets =>
            {
                Quote newQuote = new Quote(details);
                newQuote.MaterialCost = carpets.Sum(x => x.UnitPrice);
                newQuote.RollOrders = carpets.Select(x => x.InventoryId).ToArray();
                return newQuote;
            };
            quotes = validCarpetCombinations.Select(quoteFromCarpetSelection).ToList();

            Quote lowestPricedQuote = null;
            if(quotes.Any())
            {
                lowestPricedQuote = quotes.OrderByDescending(quote => quote.Price).FirstOrDefault();
            }
            return lowestPricedQuote;
        }

        public List<Carpet[]> GetPotentialCarpetPair(List<Carpet> unusedCarpet, int minimumSize)
        {
            List<Carpet[]> carpetPossibilities = new List<Carpet[]>();
            Carpet[] shallowCopyOfUnusedCarpet = new Carpet[unusedCarpet.Count];
            unusedCarpet.CopyTo(shallowCopyOfUnusedCarpet);
            List<Carpet> checkedCarpetCombo = shallowCopyOfUnusedCarpet.ToList();
            foreach (Carpet carpet in unusedCarpet)
            {
                int newMinimumSize = minimumSize - carpet.SquareFootage;
                checkedCarpetCombo.Remove(carpet);
                if(newMinimumSize > 0)
                {
                    if (checkedCarpetCombo.Any())
                    {
                        foreach (Carpet[] additionalCarpets in GetPotentialCarpetPair(checkedCarpetCombo, newMinimumSize))
                        {
                            Carpet[] potentialFit = new Carpet[additionalCarpets.Length + 1];
                            potentialFit[0] = carpet;
                            additionalCarpets.CopyTo(potentialFit, 1);
                            carpetPossibilities.Add(potentialFit);
                        }
                    }
                }
                else
                {
                    Carpet[] potentialFit = { carpet };
                    carpetPossibilities.Add(potentialFit);
                }
            }
            return carpetPossibilities;
        }        
        public string GetDataLoction()
        {
            string currentLocation = System.AppContext.BaseDirectory;
            string dataLocation = null;
            string dataStorageFolderName = "data";
            while(dataLocation == null)
            {
                DirectoryInfo info = new DirectoryInfo(currentLocation);
                if(info.Name == dataStorageFolderName)
                {
                    dataLocation = info.FullName;
                }
                else
                {
                    DirectoryInfo possibleDataDirectory = 
                        info.EnumerateDirectories(dataStorageFolderName)
                        .FirstOrDefault(dir => dir.Name == dataStorageFolderName);
                    if(possibleDataDirectory != null)
                    {
                        dataLocation = possibleDataDirectory.FullName;
                    }
                }
                if(info.Parent == null)
                {
                    break;
                }
                currentLocation = info.Parent.FullName;
            }
            return dataLocation;
        }
    }
}