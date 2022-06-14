using Cafeo.Data;
using NUnit.Framework;

namespace Cafeo.Editor.Tests
{
    public class FashionNaiveTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FashionNaiveTestSimplePasses()
        {
            // Use the Assert class to test conditions
            var shoeSize = new GarmentSize.ShoeSize(250);
            Assert.That(shoeSize.EuropeanSize(), Is.EqualTo(40));
        }
    }
}