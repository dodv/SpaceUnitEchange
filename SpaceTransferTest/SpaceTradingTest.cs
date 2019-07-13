using System;
using NUnit.Framework;
using SpaceTransfer;

namespace SpaceTransferTest
{
    [TestFixture]
    public class SpaceTradingTest
    {
        //[SetUp]
        //public void initTest()
        //{
        //    SpaceTrading.Instance.ResetData();
        //    SpaceTrading.Instance.ExchangeSpaceCredits("glek is I");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("prob is V");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("pash is X");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("teskj is L");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("glek glek Silver is 34 Credits");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("glek prob Gold is 57800 Credits");
        //    SpaceTrading.Instance.ExchangeSpaceCredits("pash pash Iron is 3910 Credits");
        //}

        [Test]
        public void ExchangeSpaceCreditsTest()
        {
            //setup perfect enviroment
            SpaceTrading.Instance.ResetData();
            // valid query defied intergalactic units
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek is I").Status,true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("prob is V").Status,true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("pash is X").Status, true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("teskj is L").Status, true);

            //invalid query, Roman number QK not exist
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("TEST is QK").Message, "Invalid format");

            //valid query define trading item rate per unit
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek glek Silver is 34 Credits").Status,true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek prob Gold is 57800 Credits").Status, true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("pash pash Iron is 3910 Credits").Status, true);

            //invalid query defined trading item rate per unit
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek glek Silver is 34 credits").Status, false); //Must be Credits wit upper C

            //calculate query string inputted
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek").Status, true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek prob Silver").Result, 68);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek prob Silver").IsCredit, true);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("pash teskj glek glek").Result, 42);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("pash teskj glek glek").IsCredit, false);
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek glek Gold").Result, 28900);

            //invalid format IIII
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek glek glek glek").Message, "Invalid format");

            //invalid format I can not subtract by L
            Assert.AreEqual(SpaceTrading.Instance.ExchangeSpaceCredits("glek teskj").Message, "Invalid format");



            try
            {
                SpaceTrading.Instance.ExchangeSpaceCredits("how much coffee ");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("intergalactic unit was not defined: how", ex.Message);
            }
        }

        [Test]
        public void ExchangeSpaceCreditsFailedTest()
        {
            //setup test with imperfect data
            SpaceTrading.Instance.ResetData();
            try
            {
                SpaceTrading.Instance.ExchangeSpaceCredits("glek");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("I have no idea what you are talking about", ex.Message);
            }
        }

    }
}
