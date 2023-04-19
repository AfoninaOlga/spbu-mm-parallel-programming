using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProducerConsumer.DataVerification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace ProducerConsumer.DataVerification.Tests
{
    [TestClass()]
    public class ConsoleInputVerifierTests
    {
        [TestMethod()]
        public void ReadInputFromConsoleTest()
        {
            var inputVerifier = ConsoleInputVerifier.ReadInputFromConsole("1");

            Assert.That(inputVerifier, Is.EqualTo(1));
        }
    }
}