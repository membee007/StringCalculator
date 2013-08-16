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
        public void GivenemptyshouldreturnZero()
        {
            Assert.That(_sut.Add(string.Empty),Is.EqualTo(0));
        }
        [Test]
        public void GivenNumberShouldReturnTheNumber()
        {
            Assert.That(_sut.Add("1"),Is.EqualTo(1));
        }
        [Test]
        public void GivenNumbersShouldAddThem()
        {
            Assert.That(_sut.Add("1,2"),Is.EqualTo(3));
        }

    
        [Test]
        public void AllowNewLineAsdelim()
        {
            Assert.That(_sut.Add("1\n2,3"),Is.EqualTo(6));
        }
    
        [Test]
        public void AllowcustomDelims()
        {
            Assert.That(_sut.Add("//;\n1;2"), Is.EqualTo(3));
        }

        [TestCase("Negatives not allowed: -1","-1,2")]
        [TestCase("Negatives not allowed: -4,-5","2,-4,3,-5")]
        public void NegativeShouldthrowException(string message, string input)
        {
            Assert.Throws(Is.TypeOf<NegativeNumberException>().And.Message.EqualTo(message),()=>_sut.Add(input));
        }
        
  [Test]
  public void NumbersgreaterthanThousandShouldBeIgnored()
  {
		Assert.That(_sut.Add("1001,2"),Is.EqualTo(2));
  }
 
    
  [Test]
  public void AnyLengthDelims()
  {
      Assert.That(_sut.Add("//[***]\n1***2***3"), Is.EqualTo(6));
  }

  [Test]
  public void AllowMultipledelims()
  {
      Assert.That(_sut.Add("//[*][%]\n1*2%3"), Is.EqualTo(6));
  }

  [Test]
  public void AllowMultipledelimsanylength()
  {
      Assert.That(_sut.Add("//[**][%%%]\n1**2%%%3"), Is.EqualTo(6));
  }
    }

    public class NegativeNumberException:Exception
    {
        public NegativeNumberException(string error)
            :base(error){
        
            }
    }

    public class StringCalculator
    {
        public int Add(string val)
        {if(string.IsNullOrEmpty(val))
            return 0;
            var delimis = new List<string> {",", "\n"};
            if (val.StartsWith("//["))
            {
                var dels = val.Substring(3, val.IndexOf("]\n") - 3)
                              .Split(new[] {"]["}, StringSplitOptions.RemoveEmptyEntries);
                delimis.AddRange(dels);
                val = val.Substring(val.IndexOf('\n') + 1);
            }
            else if (val.StartsWith("//"))
            {
                delimis.Add(val[2].ToString());
                val = val.Substring(val.IndexOf('\n') + 1);
            }
            var numbers = val.Split(delimis.ToArray(),StringSplitOptions.None);
            checkForNegatives(numbers);
            return numbers.Select(int.Parse).Where(x=>x<=1000).Sum();
        }

        private void checkForNegatives(IEnumerable<string> numbers)
        {
            var negs = numbers.Select(int.Parse).Where(x => x < 0);
            if(!negs.Any())return;
            const string error = "Negatives not allowed: {0}";
            throw new NegativeNumberException(string.Format(error,string.Join(",",negs)));
        }
    }
}