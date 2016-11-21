using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetCore.Joust
{
    public class Supplier
    {
        //Suppliers Name
        public string Name {get;set;}

        //Date Received from Supplier
        public DateTime DateRecieved {get;set;}

        //Private variable to cache inventory data to allow lazy load of csv data
        private List<Carpet> _Inventory;

        //Inventory of carpet avalible
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

        //location of file for latter refrence
        private string _filelocation {get;set;}
        public Supplier(string location)
        {
            //record file location for easy refrence latter
            _filelocation = location;
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
                    Name = fileNameParts[0];
                    //second part is received date in yyyy.mm.dd format
                    int year = 0;
                    int month = 0;
                    int day = 0;
                    bool hasDateParts =
                        int.TryParse(fileNameParts[1], out year)
                        && int.TryParse(fileNameParts[2], out month)
                        && int.TryParse(fileNameParts[3], out day);
                    if(hasDateParts)
                    {
                        DateRecieved = new DateTime(year, month, day);
                    }
                    else
                    {
                        throw new Exception("Invalid File Name Encountered");
                    }
                }
                else
                {
                    throw new Exception("Invalid File Name Encountered");
                }
            }
            

        }

        //Gets the carpet data from the file data
        private List<Carpet> ParseCsvData()
        {
            //get all the lines from the file
            string[] lines = System.IO.File.ReadAllLines(_filelocation);
            //parse each line into a carpet object
            List<Carpet> inventoryData = lines.
                Select(rawDataLine => new Carpet(rawDataLine))
                .ToList();
            //return the carpet inventory
            return inventoryData;
        }
    }
}