using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.Joust;
using Xunit;

namespace DotNetCore.Test
{
    public class OrderFulfiller_ShouldUseRightGradeRange
    {
        private readonly OrderFulfiller _orderFulfiller;
        public OrderFulfiller_ShouldUseRightGradeRange()
        {
            _orderFulfiller = new OrderFulfiller();
        }

        [Fact]
        public void ShouldErrorIfGradeIsLessThanOne()
        {
            Assert.ThrowsAny<Exception>(() => _orderFulfiller.GetQuote(1000, 1, 1, 0));
        }

        [Fact]
        public void ShouldErrorIfGradeIsGreaterThanNine()
        {
            Assert.ThrowsAny<Exception>(() => _orderFulfiller.GetQuote(1000, 1, 1, 10));
        }
    }
}
