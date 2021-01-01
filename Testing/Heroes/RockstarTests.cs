using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Chasejyd.Rockstar;

namespace ChasejydTests
{
    [TestFixture()]
    public class RockstarTests : BaseTest
    {
        protected HeroTurnTakerController rockstar { get { return FindHero("Rockstar"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(rockstar.CharacterCard, 1);
            DealDamage(villain, rockstar, 2, DamageType.Melee, true);
        }

        [Test()]
        public void TestLoadRockstar()
        {
            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(rockstar);
            Assert.IsInstanceOf(typeof(RockstarCharacterCardController), rockstar.CharacterCardController);

            Assert.AreEqual(31, rockstar.CharacterCard.HitPoints);
        }
    }
}
