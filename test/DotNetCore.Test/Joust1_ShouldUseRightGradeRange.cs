using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using DotNetCore.Joust;
using Xunit;

namespace DotNetCore.Test
{
    public class Joust1_ShouldUseRightGradeRange
    {
        private readonly IJoust _joust;
        public Joust1_ShouldUseRightGradeRange()
        {
            Type joustInterfaceType = typeof(IJoust);
            Type joust = Assembly.Load(new AssemblyName("DotNetCore.Joust")).GetTypes()
                .FirstOrDefault(type => joustInterfaceType.IsAssignableFrom(type) && type != joustInterfaceType);
            _joust = (IJoust)Activator.CreateInstance(joust);
        }

        [Fact]
        public void ShouldErrorIfGradeIsLessThanOne()
        {
            Assert.ThrowsAny<Exception>(() => _joust.GetQuote(new int[] { 1000, 1, 1, 0 }));
        }

        [Fact]
        public void ShouldErrorIfGradeIsGreaterThanNine()
        {
            Assert.ThrowsAny<Exception>(() => _joust.GetQuote(new int[] { 1000, 1, 1, 10 }));
        }
    }
}
