using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.Joust;
using Xunit;

namespace DotNetCore.Test
{
    public class OrderFulfiller_ShouldAcceptFourInts
    {
        private readonly OrderFulfiller _orderFulfiller;
        public OrderFulfiller_ShouldAcceptFourInts()
        {
            _orderFulfiller = new OrderFulfiller();
        }

        [Fact]
        public void ThrowErrorGivenANullArgument()
        {
            Assert.ThrowsAny<Exception>(() => _orderFulfiller.GetQuote(null));
        }

        [Fact]
        public void ThrowErrorGivenToFewArguments()
        {
            Assert.ThrowsAny<Exception>(() => _orderFulfiller.GetQuote(new int[] {0,0,0}));
        }

        [Fact]
        public void ThrowErrorGivenToManyArguments()
        {
            Assert.ThrowsAny<Exception>(() => _orderFulfiller.GetQuote(new int[] { 0, 0, 0, 0, 0 }));
        }
    }
}
