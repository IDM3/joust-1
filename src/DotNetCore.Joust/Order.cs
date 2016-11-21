using System;

namespace DotNetCore.Joust
{
    public class Order
    {
        //square footage required
        public int SquareFootageNeeded {get;set;}
        //rooms to be filled
        public int NumberOfRooms {get;set;}
        //cost per hour of work
        public int HourlyLaborCost {get;set;}
        //desired grade of carpet
        public int DesiredCarpetGrade {get;set;}

        //static labor rate set by requirements per roll
        public const float HoursPerRoll = .5F;
        //static labor rate set by requirments per room
        public const float HoursPerRoom = .5F;
        //static padding rate set by requiremnts for order
        public const float Padding = 1.4F;
    }
}