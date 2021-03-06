using System;

namespace DotNetCore.Joust
{
    /// <summary>
    /// User provided details for order needing quotes
    /// </summary>
    public class Order
    {
        /// <summary>
        /// square footaged needing carpeted provided by user
        /// </summary>
        public int SquareFootageNeeded {get;set;}
        
        /// <summary>
        /// Number of rooms needing carpeted provided by user
        /// </summary>
        public int NumberOfRooms {get;set;}

        /// <summary>
        /// Cost per hour of labor provided by user
        /// </summary>
        public int HourlyLaborCost {get;set;}
        
        /// <summary>
        /// Minimum desired carpet grade desired by user
        /// </summary>
        public int DesiredCarpetGrade {get;set;}
        
        /// <summary>
        /// hours taken per roll provided by requirements
        /// </summary>
        public const decimal HoursPerRoll = .5m;
        /// <summary>
        /// hours taken per room provided by requirements
        /// </summary>
        public const decimal HoursPerRoom = .5m;
        /// <summary>
        /// Padding to be added to quote total as per requirements
        /// </summary>
        public const decimal Padding = 1.4m;
    }
}