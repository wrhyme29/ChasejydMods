using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chasejyd.DeeprootTeam;

namespace ChasejydTests
{
    [TestFixture()]
    public class DeeprootTeamTests : CustomBaseTest
    {
        [Test()]
        public void TestLoadDeeprootTeam()
        {
            SetupGameController("Chasejyd.DeeprootTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "TheScholar", "Megalopolis");

            Assert.AreEqual(7, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(deeprootTeam);
            Assert.IsInstanceOf(typeof(DeeprootTeamCharacterCardController), deeprootTeam.CharacterCardController);
            Assert.IsInstanceOf(typeof(DeeprootTeamTurnTakerController), deeprootTeam);

            Assert.AreEqual(31, deeprootTeam.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDeeprootTeamStartOfGame()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Ra", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();
            Card plantParty = GetCard("PlantLifeOfTheParty");
            AssertInPlayArea(deeprootTeam, plantParty);

        }

    }
}
