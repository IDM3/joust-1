using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using DotNetCore.Joust;
using Xunit;

namespace DotNetCore.Test
{
    public class Joust1_ShouldAcceptFourInts
    {
        private readonly IJoust _joust;
        public Joust1_ShouldAcceptFourInts()
        {
            Type joustInterfaceType = typeof(IJoust);
            Type joust = Assembly.Load(new AssemblyName("DotNetCore.Joust")).GetTypes()
                .FirstOrDefault(type => joustInterfaceType.IsAssignableFrom(type) && type != joustInterfaceType);
            _joust = (IJoust)Activator.CreateInstance(joust);
        }

        [Fact]
        public void ThrowErrorGivenANullArgument()
        {
            Assert.ThrowsAny<Exception>(() => _joust.GetQuote(null));
        }

        [Fact]
        public void ThrowErrorGivenToFewArguments()
        {
            Assert.ThrowsAny<Exception>(() => _joust.GetQuote(new int[] {0,0,0}));
        }

        [Fact]
        public void ThrowErrorGivenToManyArguments()
        {
            Assert.ThrowsAny<Exception>(() => _joust.GetQuote(new int[] { 0, 0, 0, 0, 0 }));
        }
    }
}
