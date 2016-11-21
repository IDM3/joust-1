using System;

namespace DotNetCore.Joust
{
    public class Carpet
    {
        //to be set on a failed parse set
        public bool ParsedCorrectly {get;set;}
        
        //Inventory Id of carpet
        public string InventoryId {get;set;}

        //implemented in the long way to allow use in out parameters
        private int _grade;

        //Grade of carpet
        public int Grade 
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = value;
            }
        }
        
        //implemented in the long way to allow use in out parameters
        private int _length;
        //Carpet Length
        public int Length 
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        //implemented in the long way to allow use in out parameters
        private int _width;
        //Carpet Length
        public int Width 
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        //implemented in the logn way to allow use in out parameters
        private float _unitPrice;
        //Implement ICarpet.UnitPrice
        public float UnitPrice 
        {
            get
            {
                return _unitPrice;
            }
            set
            {
                _unitPrice = value;
            }
        }

        //raw data we parsed values from
        private string RawData { get;set; }

        //finds square footage of carpet roll
        private int? _squareFootage;
        public int SquareFootage
        {
            get
            {
                if(!_squareFootage.HasValue)
                {
                    _squareFootage = Width * Length;
                }
                return _squareFootage.Value;
            }
        }

        //finds price per square foot of carpet roll
        public float PricePerSquareFoot
        {
            get
            {
                return UnitPrice / SquareFootage;
            }
        }

        public Carpet(string data)
        {
            RawData = data;
            bool isCsvValue = data.Contains(",");
            if(isCsvValue)
            {
                string[] subValues = data.Split(',');
                InventoryId = subValues[0];
                ParsedCorrectly = int.TryParse(subValues[1], out _grade)
                    && int.TryParse(subValues[2], out _length)
                    && int.TryParse(subValues[3], out _width)
                    && float.TryParse(subValues[4], out _unitPrice);
            }
            else
            {
                //system generated values are alwys parsed correctly
                ParsedCorrectly = true;
                //do nothing with non CSV Values
            }
        }

        //default constructor
        public Carpet() : this("Not From Data")
        {

        }
    }
}