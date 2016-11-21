using System;

namespace DotNetCore.Joust
{
    public class Quote : IQuote
    {
        // Total price including material cost, labor cost, and margin
        public float Price 
        {
            get
            {
                return  ( MaterialCost + LaborCost ) * Order.Padding;
            }
        }
        
        // Cost of all carpet orders from suppliers
        public float MaterialCost {get; set;}

        // Total cost of installation labor
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
        // Inventory IDs of all rolls of carpet to be purchased
        public string[] RollOrders {get;set;}

        // Order detailed needed for calculation
        public Order OrderDetails {get;set;}

        public Quote(Order details)
        {
            OrderDetails = details;
            MaterialCost = 0F;
        }
    }
}