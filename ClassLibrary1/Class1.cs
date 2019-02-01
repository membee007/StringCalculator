using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ClassLibrary1
{
    [TestFixture]
    public class Tests
    {
        private Calculator _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new Calculator();
        }

        [Test]
        public void Emptyreturns0()
        {
            Assert.That(_sut.Add(string.Empty), Is.EqualTo(0));
        }

      
    }

    internal class Calculator
    {
    }
}