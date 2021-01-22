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

            Assert.AreEqual(24, blisterTeam.CharacterCard.HitPoints);
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
            QuickHPCheck(0, -3, 0, -2, 0, 0); //rockstar is nemesis

            

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
            QuickHPCheck(0, -5, 0, -3, 0, 0); //rockstar is nemesis

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
            SetupGameController(new string[] { "Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();
            // Damage from { Blister} is Irreducible.
            PlayCard("BlisteringSolo");
            AddReduceDamageTrigger(haka, true, false, 2);
            QuickHPStorage(haka);
            DealDamage(blisterTeam, haka, 5, DamageType.Fire);
            QuickHPCheck(-5);
            PreventEndOfTurnEffects(haka, blisterTeam.CharacterCard);
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
            PreventEndOfTurnEffects(haka, blisterTeam.CharacterCard);
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

       

        [Test()]
        public void TestFireAway()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //blister deals the 3 Non-Villain Targets with the Highest HP 2 Fire Damage each.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            PlayCard("FireAway");
            QuickHPCheck(0, -2, 0, -3, 0, 0, 0, -2); //rockstar is nemesis
        }

        [Test()]
        public void TestFireball_NoAxe()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            PutInTrash("PoliceBackup");
            Card heroOngoing = PlayCard("Dominion");
            Card blisterOngoing = PlayCard("BlisteringSolo");
            //Blister deals X Hero Targets with the highest HP 3 Fire Damage, where X is number of cards in env trash + 1 
            //Destroy 1 Hero Ongoing and 1 of {Blister}'s Ongoings. 
            //Then if Blazing Axe is in play, Blister deals it 2 Fire Damage.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            PlayCard("Fireball");
            QuickHPCheck(0, -3, 0, -4, 0, 0, 0, 0);
            AssertInTrash(heroOngoing);
            AssertInTrash(blisterOngoing);

        }

        [Test()]
        public void TestFireball_WithAxe()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card heroOngoing = PlayCard("Dominion");
            Card blisterOngoing = PlayCard("BlisteringSolo");
            Card axe = PlayCard("BlazingAxe");
            //Blister deals the Hero Target with the highest HP 3 Fire Damage. 
            //Destroy 1 Hero Ongoing and 1 of {Blister}'s Ongoings. 
            //Then if Blazing Axe is in play, Blister deals it 2 Fire Damage.
            QuickHPStorage(blisterTeam.CharacterCard, haka.CharacterCard, ermineTeam.CharacterCard, rockstar.CharacterCard, operativeTeam.CharacterCard, tachyon.CharacterCard, baronTeam.CharacterCard, bunker.CharacterCard, axe);
            PlayCard("Fireball");
            QuickHPCheck(0, -4, 0, 0, 0, 0, 0, 0, -3);
            AssertInTrash(heroOngoing);
            AssertInTrash(blisterOngoing);
        }

        [Test()]
        public void TestFirestarter()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card dominion = PlayCard("Dominion");
            Card police = PlayCard("PoliceBackup");
            //The first time each turn that a Hero Card is destroyed, Blister deals the Hero Target with the second highest HP 2 Fire Damage.
            PlayCard("Firestarter");

            //don't trigger on non-hero
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            DestroyCard(police, haka.CharacterCard);
            QuickHPCheckZero();

            //triggers on first hero

            QuickHPUpdate();
            DestroyCard(dominion, blisterTeam.CharacterCard);
            QuickHPCheck(0, 0, 0, -3, 0, 0, 0, 0); //rockstar is nemesis

            //only first damage per turn
            SetAllTargetsToMaxHP();
            PlayCard(dominion);
            QuickHPUpdate();
            DestroyCard(dominion, blisterTeam.CharacterCard);
            QuickHPCheckZero();

            //resets next turn
            GoToNextTurn();
            SetAllTargetsToMaxHP();
            PlayCard(dominion);
            QuickHPUpdate();
            DestroyCard(dominion, blisterTeam.CharacterCard);
            QuickHPCheck(0, 0, 0, -3, 0, 0, 0, 0); //rockstar is nemesis
        }
        [Test()]
        public void TestSetFireToTheRain_NoAx()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Blister deals each Non-Villain Target 1 Fire Damage. 
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            PlayCard("SetFireToTheRain");
            QuickHPCheck(0, -1, 0, -2, 0, -1, 0, -1); //rockstar is nemesis

        }

        [Test()]
        public void TestSetFireToTheRain_WithAx()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Blister deals each Non-Villain Target 1 Fire Damage. 
            Card axe = PlayCard("BlazingAxe");
            QuickHPStorage(blisterTeam.CharacterCard, haka.CharacterCard, ermineTeam.CharacterCard, rockstar.CharacterCard, operativeTeam.CharacterCard, tachyon.CharacterCard, baronTeam.CharacterCard, bunker.CharacterCard, axe);
            PlayCard("SetFireToTheRain");
            QuickHPCheck(0, -2, 0, -3, 0, -2, 0,-2, -3); //rockstar is nemesis

        }

        [Test()]
        public void TestSleepNowInTheFire()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(blisterTeam, 10);
            PlayCard("SleepNowInTheFire");
            //The first time each turn that {Blister} deals Fire Damage, she also heals HP equal to the amount of Fire Damage dealt.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            DealDamage(blisterTeam, haka, 2, DamageType.Fire);
            QuickHPCheck(2, -2, 0, 0, 0, 0, 0, 0);

            //only first damage
            QuickHPUpdate();
            DealDamage(blisterTeam, haka, 2, DamageType.Fire);
            QuickHPCheck(0, -2, 0, 0, 0, 0, 0, 0);

            //resets next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(blisterTeam, haka, 4, DamageType.Fire);
            QuickHPCheck(4, -4, 0, 0, 0, 0, 0, 0);

            //only fire
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(blisterTeam, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestSoloAct()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //{Blister} deals the Hero Target with the Highest HP 1 Sonic Damage, 1 Fire Damage, and 1 Toxic Damage.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            PlayCard("SoloAct");
            QuickHPCheck(0, -3, 0, 0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestStageDive()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card police = PlayCard("PoliceBackup");
            Card top = PutOnDeck("HostageSituation");
            //Destroy an Environment Card. 
            //Deal the Hero with the Second Highest HP 2 Melee Damage and 2 Fire Damage.
            //Play the top Card of the Environment Deck.
            QuickHPStorage(blisterTeam, haka, ermineTeam, rockstar, operativeTeam, tachyon, baronTeam, bunker);
            PlayCard("StageDive");
            AssertInTrash(police);
            QuickHPCheck(0, 0, 0, -6, 0, 0, 0, 0); //rockstar is nemesis
            AssertInPlayArea(env, top);



        }

        [Test()]
        public void TestPlayWithFire_AxeInPlay()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //If Blazing Axe is in play, restore it to 10 HP. 
            Card axe = PlayCard("BlazingAxe");
            SetHitPoints(axe, 2);
            SetHitPoints(blisterTeam.CharacterCard, 10);
            Card firestarter = PutOnDeck("Firestarter");
            QuickHPStorage(blisterTeam);
            PlayCard("PlayWithFire");
            AssertHitPoints(axe, 10);
            //Blister recovers 3 HP
            QuickHPCheck(3);
            //then Play the top Card of Blister's deck.
            AssertInPlayArea(blisterTeam, firestarter);
        }


        [Test()]
        public void TestPlayWithFire_AxeInTrash()
        {
            SetupGameController("Chasejyd.BlisterTeam", "Haka", "ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            //If Blazing Axe is in Blister's Trash, shuffle it back into Blister's deck.
            Card axe = PutInTrash("BlazingAxe");
            SetHitPoints(blisterTeam.CharacterCard, 10);
            Card firestarter = PutOnDeck("Firestarter");
            StackAfterShuffle(blisterTeam.TurnTaker.Deck, new string[] { firestarter.Identifier });
            QuickShuffleStorage(blisterTeam.TurnTaker.Deck);
            QuickHPStorage(blisterTeam);
            PlayCard("PlayWithFire");
            AssertInDeck(axe);
            QuickShuffleCheck(1);

            //Blister recovers 3 HP
            QuickHPCheck(3);
            //then Play the top Card of Blister's deck.
            AssertInPlayArea(blisterTeam, firestarter);
        }

    }
}
