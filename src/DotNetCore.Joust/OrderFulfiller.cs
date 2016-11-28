using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace DotNetCore.Joust
{
    public class OrderFulfiller : IJoust
    {
        /// <summary>
        /// Get quote from inputed order details
        /// </summary>
        /// <param name="input">array that contains Square Footage Needed, number of rooms, hourly labor rate, 
        /// and desired grade inclusively between <see cref="Carpet.MinimumGrade"/> and <see cref="Carpet.MaximumGrade"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is null </exception>
        /// <exception cref="ArgumentException"><paramref name="input"/> contains more or less than 4 ints </exception>
        /// <exception cref="ArgumentOutOfRangeException">if it receives an input for <paramref name="input"/>[3] 
        /// that is not inclusively between <see cref="Carpet.MinimumGrade"/> and <see cref="Carpet.MaximumGrade"/> </exception> 
        /// <returns>Lowest <see cref="IQuote.Price"/> <see cref="IQuote"/> found by order fulfilling algorithm, 
        /// if the order can not be fulfilled it returns null</returns>
        public IQuote GetQuote(int[] input)
        {
            if(input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            //validate input contains only number of inputs required
            if (input.Length != 4)
            {
                throw new ArgumentException(nameof(input));
            }
            //call more clearly defined method
            return GetQuote(input[0], input[1], input[2], input[3]);
        }

        /// <summary>
        /// Get quote from inputed order details
        /// </summary>
        /// <param name="squareFootageNeeded">Minimum square footage that should be present in a quote</param>
        /// <param name="numberOfRooms">Number of rooms needed by quote used for labor cost</param>
        /// <param name="hourlyLaborRate">Cost per hour of Labor</param>
        /// <param name="desiredGrade">Minimum grade of carpet needed inclusively between <see cref="Carpet.MinimumGrade"/> 
        /// and <see cref="Carpet.MaximumGrade"/></param>
        /// <exception cref="ArgumentOutOfRangeException">if it receives an input for <paramref name="desiredGrade"/> 
        /// that is not inclusively between <see cref="Carpet.MinimumGrade"/> and <see cref="Carpet.MaximumGrade"/> </exception> 
        /// <returns>Lowest <see cref="IQuote.Price"/> <see cref="IQuote"/> found by order fulfilling algorithm, 
        /// if the order can not be fulfilled it returns null</returns>
        public IQuote GetQuote(int squareFootageNeeded, int numberOfRooms, int hourlyLaborRate, int desiredGrade)
        {
            //validate desired grade is within acceptable limits
            if(desiredGrade < Carpet.MinimumGrade || desiredGrade > Carpet.MaximumGrade)
            {
                throw new ArgumentOutOfRangeException(nameof(desiredGrade));
            }
            //get order details
            Order details = new Order();
            //first input is square footage needed
            details.SquareFootageNeeded = squareFootageNeeded;
            //second input is number of rooms
            details.NumberOfRooms = numberOfRooms;
            //thirdsecond input is hourly labor cost
            details.HourlyLaborCost = hourlyLaborRate;
            //fourth input is carget grade needed
            details.DesiredCarpetGrade = desiredGrade;
            //get supplier details
            string dataLocation = GetDataLoction();
            SupplierRepository Rep = new SupplierRepository(dataLocation);
            //get all valid grades present in inventory
            int[] potentialGrades = Rep.SearchInventory(x => true).Select(x => x.Grade).Distinct().OrderBy(x => x).ToArray();
            //limit grades avalible to desired grade or higher
            potentialGrades = potentialGrades.Where(grade => grade >= details.DesiredCarpetGrade).ToArray();
            List<Quote> quotes = new List<Quote>();
            //function to create quotes from a selection of carpets
            Func<Carpet[], Quote> quoteFromCarpetSelection = carpets => new Quote(details)
                {
                    //material cost is equal to the unit price of all carpets in order
                    MaterialCost = carpets.Sum(carpet => carpet.UnitPrice),
                    //roll order should contain inventory of all ids in order
                    RollOrders = carpets.Select(carpet => carpet.InventoryId).ToArray(),
                    //carpets in order contains reference to carpets made of order for testing purposes
                    CarpetsInOrder = carpets
                };
            //since we want only one grade in an order we loop through each potential grade and create orders for each
            foreach (int grade in potentialGrades)
            {
                //simple function to check if a carpet is the right grade here for latter readability
                Func<Carpet, bool> isRightGrade = carpet => carpet.Grade == grade;
                //selects carpets of the valid grade and orders them by descending square footage for latter optimization
                List<Carpet> carpetSelection = Rep.SearchInventory(isRightGrade).OrderByDescending(carpet => carpet.SquareFootage).ToList();
                //patch for potential negative labor
                Func<Carpet, bool> carpetIsFreeOrGivesYouMoney = carpet => carpet.UnitPrice <= (-1 * hourlyLaborRate * Order.HoursPerRoll);
                List<Carpet> profitCarpets = carpetSelection.Where(carpetIsFreeOrGivesYouMoney).ToList();
                //remove free carpets
                carpetSelection.RemoveAll(carpet => profitCarpets.Contains(carpet));
                //get all quotes where one carpet can fulfill order at this grade
                List<Carpet[]> validCarpetCombinations = GetPotentialUniqueCarpetPair(
                    carpetSelection, 
                    (details.SquareFootageNeeded - profitCarpets.Sum(carpet => carpet.SquareFootage)));
                //if we have a negative labor value we need to add our profit carpets back in
                if (profitCarpets.Any())
                {
                    //add profit carpets to all selections            
                    for (int index = 0; index < validCarpetCombinations.Count; index++)
                    {
                        validCarpetCombinations[index] = validCarpetCombinations[index].Union(profitCarpets).ToArray();
                    }
                    //can an combo be made of the profit carpets?
                    if (profitCarpets.Sum(carpet => carpet.SquareFootage) >= details.SquareFootageNeeded)
                    {
                        //if so we should add it as a valid combinations
                        validCarpetCombinations.Add(profitCarpets.ToArray());
                    }
                }
                //add qutoes found to the quote list           
                quotes.AddRange(validCarpetCombinations.Select(quoteFromCarpetSelection).ToList());
            }
            
            //create place holder for lowest
            Quote lowestPricedQuote = null;
            //are there any quotes to be found
            if(quotes.Any())
            {
                //take the first lowest quote as the lowest quote
                lowestPricedQuote = quotes.OrderBy(quote => quote.Price).FirstOrDefault();
            }
            //return the lowest quote, or null if no quotes exist
            return lowestPricedQuote;
        }

        /// <summary>
        /// Gets all the potential <see cref="Carpet"/> combinations that add up to the minimum size
        /// </summary>
        /// <param name="unusedCarpet">Valid list of <see cref="Carpet"/> to be used for combination </param>
        /// <param name="minimumSize">Squate footage needed to be a valid <see cref="IQuote"/></param>
        /// <returns>List of <see cref="Carpet[]"/> combinations made from <paramref name="unusedCarpet"/></returns>
        public List<Carpet[]> GetPotentialUniqueCarpetPair(List<Carpet> unusedCarpet, int minimumSize)
        {
            //list of new carpet possibilities
            List<Carpet[]> carpetPossibilities = new List<Carpet[]>();
            //need a copy of carpets to eliminate unused carpets from so we don't have the same carpet in one order
            Carpet[] shallowCopyOfUnusedCarpet = new Carpet[unusedCarpet.Count];
            unusedCarpet.CopyTo(shallowCopyOfUnusedCarpet);
            List<Carpet> checkedCarpetCombo = shallowCopyOfUnusedCarpet.ToList();
            //cycle through all the carpets and find all combinations that will add up to the minimum size needed
            foreach (Carpet carpet in unusedCarpet)
            {
                //find the new size of carpet needed to complete order
                int newMinimumSize = minimumSize - carpet.SquareFootage;
                //remove this carpet from selection of valid carpets
                checkedCarpetCombo.Remove(carpet);
                //do we need more carpet to fulfil the order
                if(newMinimumSize > 0)
                {
                    //are there any possible carpets that can fulfile the order
                    if (checkedCarpetCombo.Any())
                    {
                        //for each combination that will make this carpet fulfill the minimum square footage requirement
                        foreach (Carpet[] additionalCarpets in GetPotentialUniqueCarpetPair(checkedCarpetCombo, newMinimumSize))
                        {
                            //create the array to hold the carpets
                            Carpet[] potentialFit = new Carpet[additionalCarpets.Length + 1];
                            //the first index will be this carpet because that makes sense
                            potentialFit[0] = carpet;
                            //take the carpets that complete this quote and put them into the array of carpets after this one
                            additionalCarpets.CopyTo(potentialFit, 1);
                            //add this combination of the possible combinations 
                            carpetPossibilities.Add(potentialFit);
                        }
                    }
                    else
                    {
                        //no carpets fulfil order so this branch will fall out and not provide a potential carpet combination
                    }
                }
                else
                {
                    //this carpet by itself fulfills the requiremetns of a valid combination
                    Carpet[] potentialFit = { carpet };
                    //add it to the potential list
                    carpetPossibilities.Add(potentialFit);
                }
            }
            //return all the carpet possibilities
            return carpetPossibilities;
        }        

        /// <summary>
        /// Gets the location of the CSV files that hold the supplier info
        /// This one is pretty murky, but we were told by requirements data should be in the top directory of the project 
        /// so that is how I search for it
        /// </summary>
        /// <returns>the directory that holds the csv files or null if none are found</returns>
        public string GetDataLoction()
        {
            //get the location we are running from
            string currentLocation = System.AppContext.BaseDirectory;
            //this will hold where the data location is if we ever find it in the crawl
            string dataLocation = null;
            //name of the directory the data is stored in
            string dataStorageFolderName = "data";
            //crawl up parents until we find the directory that holds our data
            while(dataLocation == null)
            {
                //get the information of where we are at now
                DirectoryInfo info = new DirectoryInfo(currentLocation);
                //are we the right directory?
                if(info.Name == dataStorageFolderName)
                {
                    //cool then we'll return our name
                    dataLocation = info.FullName;
                }
                else
                {
                    //well then do I contain the directory that we need?
                    DirectoryInfo possibleDataDirectory = 
                        info.EnumerateDirectories(dataStorageFolderName)
                        .FirstOrDefault(dir => dir.Name == dataStorageFolderName);
                    if(possibleDataDirectory != null)
                    {
                        //I do so we'll record that name
                        dataLocation = possibleDataDirectory.FullName;
                    }
                }
                //am I a top level directory
                if(info.Parent == null)
                {
                    //i am so we can't look any further than me
                    break;
                }
                //well then lets look in my parent directory next
                currentLocation = info.Parent.FullName;
            }
            //returns null or the location of the data directory
            return dataLocation;
        }
    }
}