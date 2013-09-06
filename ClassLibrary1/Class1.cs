using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ClassLibrary1
{
    [TestFixture]
    public class Tests
    {
        private StringCalculator _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new StringCalculator();
        }

        [Test]
        public void GivenEmptyshouldReturnZero()
        {
            Assert.That(_sut.Add(""),Is.EqualTo(0));
        }
        [TestCase("1",1)]
        [TestCase("100",100)]
        public void GivennumberShouldReturnThatNumber(string input, int expected)
        {
            Assert.That(_sut.Add(input),Is.EqualTo(expected));
        }
    
        [Test]
        public void GiventwonumbersShouldAddThem()
        {
            Assert.That(_sut.Add("1,2"),Is.EqualTo(3));
        }
    
        [Test]
        public void AllowNewLineAsDelimiter()
        {
            Assert.That(_sut.Add("1\n2,3"),Is.EqualTo(6));
        }
    
        [Test]
        public void AllowCustomDelimiters()
        {
            Assert.That(_sut.Add("//;\n1;2"),Is.EqualTo(3));
        }

        [TestCase("Negativen numbers not allowed: -1","-1,2")]
        [TestCase("Negativen numbers not allowed: -4,-5","2,-4,3,-5")]
        public void NegativeNumbersShouldThrowException(string msg, string input)
        {
            Assert.Throws(Is.TypeOf<NegativeNumberException>().And.Message.EqualTo(msg),
                          () => _sut.Add(input));
        }
    
        [Test]
        public void AllowMultipleLengthDelimiters()
        {
            Assert.That(_sut.Add("//[***]\n1***2"),Is.EqualTo(3));
        }
        
  [Test]
  public void AllowMultipleDelimiters()
  {
		Assert.That(_sut.Add("//[@][#][$]\n1@2#3$4"),Is.EqualTo(10));
  }
  [Test]
  public void AllowMultipleDelimitersofanyLength()
  {
      Assert.That(_sut.Add("//[@@][#][$$$]\n1@@2#3$$$4"), Is.EqualTo(10));
  }

    }

    public class NegativeNumberException:Exception
    {
        public NegativeNumberException(string error)
            : base(error)
        {

        }
    }

    public class StringCalculator
    {
        public int Add(string val)
        {if(string.IsNullOrEmpty(val))
            return 0;
            var numbers = getNumbers(val);
            CheckForNegatives(numbers);
            return numbers.Select(int.Parse).Sum();
        }

        private IEnumerable<string> getNumbers(string val)
        {
            var delimiters = new List<string> {",", "\n"};
            if (val.StartsWith("//["))
            {
                var delims = val.Substring(3, val.IndexOf("]\n", StringComparison.Ordinal) - 3)
                                .Split(new[] {"]["}, StringSplitOptions.RemoveEmptyEntries);
                delimiters.AddRange(delims);
                val = val.Substring(val.IndexOf("]\n", StringComparison.Ordinal) + 2);
            }
            else if (val.StartsWith("//"))
            {
                delimiters.Add(val[2].ToString());
                val = val.Substring(val.IndexOf('\n') + 1);
            }
            var numbers = val.Split(delimiters.ToArray(), StringSplitOptions.None);
            return numbers;
        }

        private void CheckForNegatives(IEnumerable<string> numbers)
        {
            var negs = numbers.Select(int.Parse).Where(x => x < 0);
            if(!negs.Any())return;
            throw  new NegativeNumberException(string.Format("Negativen numbers not allowed: {0}",string.Join(",",negs)));
        }
    }
}