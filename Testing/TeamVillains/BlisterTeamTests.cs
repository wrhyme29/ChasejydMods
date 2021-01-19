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

        [Test()]
        public void TestBisterTeamStartOfGame()
        {
            SetupGameController(new string[] { "Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();
            Card ax = GetCard("BlazingAxe");
            Card firestarter = GetCard("Firestarter");
            AssertInPlayArea(blisterTeam, ax);
            AssertInPlayArea(blisterTeam, firestarter);

        }

        [Test()]
        public void TestBisterTeamFront()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            Card ax = GetCardInPlay("BlazingAxe");
            DestroyCard(ax, rockstar.CharacterCard);
            AssertInTrash(ax);

            //{Blister} is immune to Fire Damage.
            QuickHPStorage(blisterTeam);
            DealDamage(rockstar, blisterTeam, 3, DamageType.Fire);
            QuickHPCheckZero();

            //not immune to other damage
            QuickHPUpdate();
            DealDamage(bunker, blisterTeam, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //At the End of her Turn, {Blister} deals the two Non-Villain Targets with the Highest HP 1 Fire Damage.
            QuickHPStorage(blisterTeam, rockstar, ermineTeam, bunker, operativeTeam, tachyon);
            GoToEndOfTurn(blisterTeam);
            QuickHPCheck(0, -2, 0, -1, 0, 0); //rockstar is nemesis

            

        }

        [Test()]
        public void TestBisterTeamFrontAdvanced()
        {
            SetupGameController(new string[] { "Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true, advancedIdentifiers: new string[] { "Chasejyd.BlisterTeam" });
            StartGame();
            Card ax = GetCardInPlay("BlazingAxe");
            DestroyCard(ax, rockstar.CharacterCard);
            AssertInPlayArea(blisterTeam, ax);

        }

        [Test()]
        public void TestBisterTeamFrontChallenge()
        {
            SetupGameController(new string[] { "Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Blister} is immune to Fire Damage.
            QuickHPStorage(blisterTeam);
            DealDamage(rockstar, blisterTeam, 3, DamageType.Fire);
            QuickHPCheckZero();

            //not immune to other damage
            QuickHPUpdate();
            DealDamage(bunker, blisterTeam, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //At the End of her Turn, {Blister} deals the two Non-Villain Targets with the Highest HP 1 Fire Damage.
            //When Blister would deal a Non-Villain Target Fire Damage, she also deals that Target 1 Toxic Damage.
            QuickHPStorage(blisterTeam, rockstar, ermineTeam, bunker, operativeTeam, tachyon);
            GoToEndOfTurn(blisterTeam);
            QuickHPCheck(0, -4, 0, -2, 0, 0); //rockstar is nemesis

        }

        [Test()]
        public void TestBisterTeamIncap()
        {
            SetupGameController("ErmineTeam", "Haka", "Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            DealDamage(rockstar, blisterTeam, 1000, DamageType.Projectile, true);
            AssertFlipped(blisterTeam);

            Card police = PlayCard("PoliceBackup");
            GoToEndOfTurn(haka);
            //Destroy an Environment Card 
            //and deal the Hero Target with the Highest HP 3 Fire Damage.

            QuickHPStorage(ermineTeam, haka, rockstar, operativeTeam, tachyon);
            GoToStartOfTurn(blisterTeam);
            AssertInTrash(police);
            QuickHPCheck(0, -3, 0, 0, 0);


        }

        [Test()]
        public void TestBlazingAxe()
        {
            SetupGameController("ErmineTeam", "Haka", "Chasejyd.BlisterTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Legacy", "Megalopolis");
            StartGame();

            //Increase Damage Dealt by {Blister} by 1.
            QuickHPStorage(haka);
            DealDamage(blisterTeam, haka, 2, DamageType.Fire);
            QuickHPCheck(-3);

            //Damage from {Blister} cannot be redirected.
            PlayCard("LeadFromTheFront");
            DecisionYesNo = true;
            QuickHPStorage(haka, rockstar, legacy);
            DealDamage(blisterTeam, haka, 2, DamageType.Fire);
            QuickHPCheck(-3, 0, 0);

        }

        [Test()]
        public void TestBisteringSolo()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka","ErmineTeam" , "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            // Damage from { Blister} is Irreducible.
            PlayCard("BlisteringSolo");
            AddReduceDamageTrigger(haka, true, false, 2);
            QuickHPStorage(haka);
            DealDamage(blisterTeam, haka, 5, DamageType.Fire);
            QuickHPCheck(-5);

            DecisionSelectTarget = blisterTeam.CharacterCard;
            DecisionAutoDecideIfAble = true;
            //Heroes damaged by Blister cannot use powers or play cards outside of their own turn until the Start of Blister's next turn
            Card mere = PlayCard("Mere");
            AssertNotInPlay(mere);
            QuickHPStorage(blisterTeam);
            UsePower(haka.CharacterCard);
            QuickHPCheckZero();

            GoToPlayCardPhase(haka);
            PlayCard(mere);
            AssertIsInPlay(mere);
            QuickHPUpdate();
            UsePower(haka.CharacterCard);
            QuickHPCheck(-2);

            GoToPlayCardPhase(rockstar);
            Card dominion = PlayCard("Dominion");
            AssertNotInPlay(dominion);
            QuickHPUpdate();
            UsePower(haka.CharacterCard);
            QuickHPCheckZero();

            GoToStartOfTurn(blisterTeam);
            GoToPlayCardPhase(haka);
            PlayCard(dominion);
            AssertIsInPlay(dominion);
            QuickHPUpdate();
            UsePower(haka.CharacterCard);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestBurningMelody()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card dominion = PlayCard("Dominion");
            //Deal the Hero Target with the highest HP 2 Fire Damage.
            //Destroy an Ongoing Card in that Hero's play area.

            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon);
            PlayCard("BurningMelody");
            QuickHPCheck(0, -2, 0, 0, 0, 0);
            AssertInTrash(dominion);

        }

        [Test()]
        public void TestBurningMelody_DestroyWhenNoDamage()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card dominion = PlayCard("Dominion");
            //Deal the Hero Target with the highest HP 2 Fire Damage.
            //Destroy an Ongoing Card in that Hero's play area.
            AddImmuneToDamageTrigger(haka, true, false);
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon);
            PlayCard("BurningMelody");
            QuickHPCheck(0, 0, 0, 0, 0, 0);
            AssertInTrash(dominion);

        }

        [Test()]
        public void TestBurnItDown()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            PreventEndOfTurnEffects(haka, blisterTeam.CharacterCard);

            Card police = PlayCard("PoliceBackup");
            PlayCard("BurnItDown");
            //At the end of {Blister}'s turn, Destroy an Environment Card.
            //Whenever an Environment Card is destroyed, {Blister} deals the Hero Target with the highest HP 2 Fire Damage.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon);
            GoToEndOfTurn(blisterTeam);
            QuickHPCheck(0, -2, 0, 0, 0, 0);
            AssertInTrash(police);

        }

    }
}
