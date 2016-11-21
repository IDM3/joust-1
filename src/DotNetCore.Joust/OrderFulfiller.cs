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
            List<Carpet> carpetSelection = Rep.SearchInventory(isRightGrade);
            //make quote object
            Quote quote = new Quote(details);
            //running total of square footage fulfilled by sorter
            int currentSquareFootageNeeded = details.SquareFootageNeeded;
            //list of current products on order
            List<string> currentProducts = new List<string>();

            //sort carpet by preference
            while(currentSquareFootageNeeded > 0)
            {

            }
            quote.RollOrders = currentProducts.ToArray();
            return null;
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