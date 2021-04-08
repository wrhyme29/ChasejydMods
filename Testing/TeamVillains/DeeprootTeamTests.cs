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
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Ra", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            Card plantParty = GetCard("PlantLifeOfTheParty");
            AssertInPlayArea(deeprootTeam, plantParty);

        }

        [Test()]
        public void TestDeeprootTeamFront()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            Card plantParty = GetCard("PlantLifeOfTheParty");
            PreventEndOfTurnEffects(deeprootTeam, plantParty);

            SetHitPoints(ermineTeam.CharacterCard, 3);
            Card traffic = PlayCard("TrafficPileup");
            SetHitPoints(traffic, 2);

            //At the end of {Deeproot}'s turn, the 2 Villain or Environment Targets with the lowest HP gain 1 HP.
            //Then {Deeproot} deals the X Hero Targets with the highest HP 2 Toxic Damage each where X is equal to the number of Plant Growth Cards in play

            PrintSpecialStringsForCard(deeprootTeam.CharacterCard);

            QuickHPStorage(deeprootTeam.CharacterCard, ermineTeam.CharacterCard, operativeTeam.CharacterCard, haka.CharacterCard, bunker.CharacterCard, tachyon.CharacterCard, traffic);
            GoToEndOfTurn(deeprootTeam);
            QuickHPCheck(0, 1, 0, -2, 0, 0, 1);
        }


        [Test()]
        public void TestDeeprootTeamIncap()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            DealDamage(haka, deeprootTeam, 32, DamageType.Melee, isIrreducible: true);
            AssertFlipped(deeprootTeam);

            Card traffic = PlayCard("TrafficPileup");

            //Reduce Damage dealt to Environment Targets by 1.
            QuickHPStorage(traffic);
            DealDamage(haka, traffic, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // At the start of Deeproot's turn, play the top card of the Environment deck.
            Card police = PutOnDeck("PoliceBackup");
            GoToStartOfTurn(deeprootTeam);
            AssertInPlayArea(env, police);
        }

        [Test()]
        public void TestDeeprootTeamAdvanced()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();
            SetHitPoints(deeprootTeam, 15);
            DestroyNonCharacterVillainCards();

            //The first time each turn a Villain Ongoing or Plant Growth is played, {Deeproot} gains 2 HP.
            PrintSpecialStringsForCard(deeprootTeam.CharacterCard);

            QuickHPStorage(deeprootTeam);
            PlayCard("WrithingFlora");
            QuickHPCheck(2);

            PrintSpecialStringsForCard(deeprootTeam.CharacterCard);

            QuickHPUpdate();
            PlayCard("DeepRoots");
            QuickHPCheckZero();

            GoToNextTurn();

            PrintSpecialStringsForCard(deeprootTeam.CharacterCard);

            QuickHPUpdate();
            PlayCard("ImpromptuHeist");
            QuickHPCheck(2);

            QuickHPUpdate();
            PlayCard("Wildbond");
            QuickHPCheckZero();

        }

        [Test()]
        public void TestDeeprootTeamChallenge()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();

            //Villain Character Cards are Immune to Damage from the Environment.

            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(deeprootTeam, haka, ermineTeam, bunker, operativeTeam, tachyon);
            DealDamage(traffic, (Card c) => c.IsNonEnvironmentTarget, 5, DamageType.Projectile);
            QuickHPCheck(0, -5, 0, -5, 0, -5);

        }

    }
}
