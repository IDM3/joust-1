using System;

namespace DotNetCore.Joust
{
    /// <summary>
    /// Carpet Information
    /// </summary>
    public class Carpet
    {
        /// <summary>
        /// Was the carpet parsed correctly from the CSV file
        /// </summary>
        public bool ParsedCorrectly { get; set; }

        /// <summary>
        /// Inventory Id of carpet
        /// </summary>
        public string InventoryId { get; set; }

        //implemented in the long way to allow use in out parameters
        private int _grade;

        /// <summary>
        /// Grade of carpet should be between <see cref="MinimumGrade"/>  and <see cref="MaximumGrade"/>
        /// </summary>
        public int Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                //if value is less then minimum set minimum
                if (value < MinimumGrade)
                {
                    _grade = MinimumGrade;
                }
                //if value is greater than maximum set maximum
                else if (value > MaximumGrade)
                {
                    _grade = MaximumGrade;
                }
                //other wise keep value inputed
                else
                {
                    _grade = value;
                }
            }
        }

        /// <summary>
        /// Minimum Grade allowed
        /// </summary>
        public const int MinimumGrade = 1;

        /// <summary>
        /// Maximum Grade allowed
        /// </summary>
        public const int MaximumGrade = 9;

        //implemented in the long way to allow use in out parameters
        private int _length;
        /// <summary>
        /// Length of the carpet used in calculating <see cref="SquareFootage"/> 
        /// </summary>
        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                //set square footage to null so it will be refigured
                _squareFootage = null;
                _length = value;
            }
        }

        //implemented in the long way to allow use in out parameters
        private int _width;
        /// <summary>
        /// Width of the carpet used in calculating <see cref="SquareFootage"/> 
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                //set square footage to null so it will be refigured
                _squareFootage = null;
                _width = value;
            }
        }

        //implemented in the logn way to allow use in out parameters
        private decimal _unitPrice;

        /// <summary>
        /// Unit price of the carpet
        /// </summary>
        public decimal UnitPrice
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

        /// <summary>
        /// String data was parsed from for checking latter
        /// </summary>
        public string RawData { get; private set; }

        //cache for square footage so we aren't finding it more than once
        private int? _squareFootage;
        /// <summary>
        /// Square footage of carpet
        /// </summary>
        /// <value><see cref="Width"/> * <see cref="Length"/> </value>
        public int SquareFootage
        {
            get
            {
                if (!_squareFootage.HasValue)
                {
                    _squareFootage = Width * Length;
                }
                return _squareFootage.Value;
            }
        }

        /// <summary>
        /// Price per square foot of material
        /// </summary>
        /// <value><see cref="UnitPrice"/> / <see cref="SquareFootage"/>  </value>
        public decimal PricePerSquareFoot
        {
            get
            {
                return UnitPrice / SquareFootage;
            }
        }

        /// <summary>
        /// Supplier of  Carpet
        /// </summary>
        public Supplier @Supplier {get; private set;}

        /// <summary>
        /// Carpet constructor from csv file
        /// </summary>
        /// <param name="data">string data from line of csv file</param>
        /// <param name="supplierOfCarpet"><see cref="Supplier"/> of Carpet for backreference</param>
        internal Carpet(string data, Supplier supplierOfCarpet)
        {
            @Supplier = supplierOfCarpet;
            RawData = data;
            //is the data in csv format
            bool isCsvValue = data.Contains(",");
            if (isCsvValue)
            {
                //split csv values
                string[] subValues = data.Split(',');
                //id is string value and is first so can be grabbed directly from file
                InventoryId = subValues[0];
                //parses basic values into value fields, if any fail Parsed Correctly is false
                ParsedCorrectly = int.TryParse(subValues[1], out _grade)
                    && int.TryParse(subValues[2], out _length)
                    && int.TryParse(subValues[3], out _width)
                    && decimal.TryParse(subValues[4], out _unitPrice);
                //Grade is a constrained value so we have an additional check ot make sure it's within constraints
                if(Grade < MinimumGrade || Grade > MaximumGrade)
                {
                    //sends current grade through grade formatting logic
                    Grade = Grade;
                }
            }
            else
            {
                //system generated values are always parsed correctly
                ParsedCorrectly = true;
                //do nothing with non CSV Values
            }
        }

        

        /// <summary>
        /// For making carpet not assocaitated with csv
        /// </summary>
        public Carpet() : this("Not From Data", null)
        {

        }
    }
}