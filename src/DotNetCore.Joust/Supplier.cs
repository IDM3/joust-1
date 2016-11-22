using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetCore.Joust
{
    /// <summary>
    /// Information about a particular carpet supplier
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// Name of Supplier
        /// </summary>
        public string Name {get; set;}

        /// <summary>
        /// Date carpet inventory was received
        /// </summary>
        public DateTime DateRecieved {get; set;}

        //Private variable to cache inventory data to allow lazy load of csv data
        private List<Carpet> _Inventory;

        /// <summary>
        /// Inventory of avalible carpets
        /// </summary>
        public List<Carpet> Inventory 
        {
            get
            {
                //check if inventory data is cached
                if(_Inventory == null)
                {
                    //fill cache with inventory data
                   _Inventory = ParseCsvData();
                }
                //return the inventor data;
                return _Inventory;
            }
        }

        /// <summary>
        /// Location of CSV File
        /// </summary>
        public string Filelocation {get; private set;}

        /// <summary>
        /// Create supplier from a  csv file location
        /// </summary>
        /// <exception cref="ArgumentException">Supplier csv file not in {name}.{yyyy}.{mm}.{dd}.csv format</exception>
        /// <param name="location">file location of csv file</param>
        public Supplier(string location)
        {
            //record file location for easy refrence latter
            Filelocation = location;
            //get file name from that info
            string fileName = Path.GetFileName(location);
            //make sure file matches required metrics
            bool isValidFileName = fileName.Contains('.');
            if(isValidFileName)
            {
                //seperate at periods, predeteremened seperator
                string[] fileNameParts = fileName.Split('.');
                //first part is the company name
                bool hasRightNumberOfParts = fileNameParts.Length >= 4;
                if(hasRightNumberOfParts)
                {
                    fileNameParts = fileNameParts.Reverse().ToArray();
                    if (!fileNameParts[0].Equals("csv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //not csv extension
                        throw new ArgumentException(nameof(location));
                    }
                    
                    Name = string.Join(".", fileNameParts.Skip(4).Reverse());
                    //second part is received date in yyyy.mm.dd format
                    int year = 0;
                    int month = 0;
                    int day = 0;
                    //checks to make sure strings are all ints
                    bool hasDateParts =
                        int.TryParse(fileNameParts[3], out year)
                        && int.TryParse(fileNameParts[2], out month)
                        && int.TryParse(fileNameParts[1], out day);
                    if(hasDateParts)
                    {
                        //record date received
                        DateRecieved = new DateTime(year, month, day);
                    }
                    else
                    {
                        //invalid date parts so bad file
                        throw new ArgumentException(nameof(location));
                    }
                }
                else
                {
                    //must have at least 4 parts could have more due to . in name
                    throw new ArgumentException(nameof(location));
                }
            }
            

        }
        
        /// <summary>
        /// Gets the carpet data from the file data
        /// </summary>
        /// <returns>List of <see cref="Carpet"/> from csv contents</returns>
        private List<Carpet> ParseCsvData()
        {
            //get all the lines from the file
            string[] lines = System.IO.File.ReadAllLines(Filelocation);
            //parse each line into a carpet object
            List<Carpet> inventoryData = lines.
                Select(rawDataLine => new Carpet(rawDataLine, this))
                .ToList();
            //return the carpet inventory
            return inventoryData;
        }
    }
}