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

        [Test]
        public void Onenumbershouldretunnumber()
        {
            Assert.That(_sut.Add("1"), Is.EqualTo(1));
        }

        [Test]
        public void Addnumbers()
        {
            Assert.That(_sut.Add("1,2"), Is.EqualTo(3));
        }

        [Test]
        public void Newlineasdelim()
        {
            Assert.That(_sut.Add("1,2\n3"), Is.EqualTo(6));
        }

        [Test]
        public void Customdelimetershouldbeallowed()
        {
            Assert.That(_sut.Add("//;\n1;2"), Is.EqualTo(3));
        }

        [TestCase("Negatives not allowed: -1", "-1,2")]
        [TestCase("Negatives not allowed: -4,-5", "2,-4,3,-5")]
        public void Negativenumbersshouldthrowexception(string msg, string input)
        {
            Assert.Throws(Is.TypeOf<Negativeexception>().And.Message.EqualTo(msg), () => _sut.Add(input));
        }

        [Test]
        public void Numbersgreaterthan1000Shouldbeignored()
        {
            Assert.That(_sut.Add("1001,2"), Is.EqualTo(2));
        }


        [Test]
        public void Delimitorsofanylengthshouldbeaccepted()
        {
            Assert.That(_sut.Add("//[***]\n1***2***3"), Is.EqualTo(6));
        }

        [Test]
        public void Multipledelimitorsofanylengthshouldbeaccepted()
        {
            Assert.That(_sut.Add("//[*][%]\n1*2%3"), Is.EqualTo(6));
        }

        [Test]
        public void AnylenMultipledelimitorsofanylengthshouldbeaccepted()
        {
            Assert.That(_sut.Add("//[**][%%%%]\n1**2%%%%3"), Is.EqualTo(6));
        }

    }

    public class Negativeexception : Exception
    {
        public Negativeexception(string error)
            : base(error)
        {

        }
    }

    internal class Calculator
    {
        public int Add(string val)
        {
            if (string.IsNullOrEmpty(val))
                return 0;
            var delims = new List<string> {",", "\n"};
            if (val.StartsWith("//["))
            {
                var newdels = val.Substring(3, val.IndexOf("]\n", StringComparison.Ordinal) - 3)
                                 .Split(new[] {"]["}, StringSplitOptions.RemoveEmptyEntries);
                delims.AddRange(newdels);
                val = val.Substring(val.IndexOf("]\n", StringComparison.Ordinal) + 2);
            }
            else if (val.StartsWith("//"))
            {
                delims.Add(val.Substring(2, 1));
                val = val.Substring(val.IndexOf("\n", StringComparison.Ordinal) + 1);
            }
            var nums = val.Split(delims.ToArray(), StringSplitOptions.None);
            Checkfornegs(nums);
            return nums.Select(int.Parse).Where(x => x <= 1000).Sum();
        }

        private static void Checkfornegs(IEnumerable<string> nums)
        {
            var negs = nums.Select(int.Parse).Where(x => x < 0);
            if (!negs.Any()) return;
            const string msg = "Negatives not allowed: {0}";
            throw new Negativeexception(string.Format(msg, string.Join(",", negs)));

        }
    }
}