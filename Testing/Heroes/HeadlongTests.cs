using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Chasejyd.Headlong;

namespace ChasejydTests
{
    [TestFixture()]
    public class HeadlongTests : CustomBaseTest
    {

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(rockstar.CharacterCard, 1);
            DealDamage(villain, headlong, 2, DamageType.Melee, true);
        }

        [Test()]
        public void TestLoadHeadlong()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(headlong);
            Assert.IsInstanceOf(typeof(HeadlongCharacterCardController), headlong.CharacterCardController);

            Assert.AreEqual(27, headlong.CharacterCard.HitPoints);
        }

    }
}
