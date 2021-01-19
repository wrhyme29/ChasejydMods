using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chasejyd.BlisterTeam;

namespace ChasejydTests
{
    [TestFixture()]
    public class BlisterTeamTests : CustomBaseTest
    {
        protected TurnTakerController blisterTeam { get { return FindVillainTeamMember("Blister"); } }

        [Test()]
        public void TestLoadBisterTeam()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "TheScholar", "Megalopolis");

            Assert.AreEqual(7, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(blisterTeam);
            Assert.IsInstanceOf(typeof(BlisterTeamCharacterCardController), blisterTeam.CharacterCardController);
            Assert.IsInstanceOf(typeof(BlisterTeamTurnTakerController), blisterTeam);

            Assert.AreEqual(23, blisterTeam.CharacterCard.HitPoints);
        }

    }
}
