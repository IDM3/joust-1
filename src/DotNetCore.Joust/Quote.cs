using System;

namespace DotNetCore.Joust
{
    /// <summary>
    /// Order quote
    /// </summary>
    public class Quote : IQuote
    { 
        /// <summary>
        /// Total price including material cost, labor cost, and margin
        /// </summary>
        public float Price 
        {
            get
            {
                return  ( MaterialCost + LaborCost ) * Order.Padding;
            }
        }
        
        /// <summary>
        /// Cost of all carpet orders from suppliers
        /// </summary>
        public float MaterialCost {get; set;}
        
        /// <summary>
        /// Total cost of installation labor
        /// </summary>
        public float LaborCost 
        {
            get
            {
                return (
                        //hours for trimming on rooms
                        (Order.HoursPerRoom * OrderDetails.NumberOfRooms) 
                        //hours for roll installation
                        + (Order.HoursPerRoll * RollOrders.Length)
                    //multiplied by labor cost
                    ) * OrderDetails.HourlyLaborCost;
            }
        }

        /// <summary>
        /// Inventory IDs of all rolls of carpet to be purchased
        /// </summary>
        public string[] RollOrders {get;set;}

        /// <summary>
        /// Carpets that are in the order
        /// </summary>
        public Carpet[] CarpetsInOrder { get; set; }
        
        /// <summary>
        /// Order detailed needed for calculation
        /// </summary>
        public Order OrderDetails {get;set;}

        /// <summary>
        /// Creates a quote
        /// </summary>
        /// <param name="details">Details used for calculating costs</param>
        public Quote(Order details)
        {
            //details is required ot not be null
            if(details == null)
            {
                //create default instsance if one is not provided (will not provide good calculations, but will prevent errors
                details = new Order();
            }
            OrderDetails = details;
            //intialize default materials cost
            MaterialCost = 0F;
        }
    }
}