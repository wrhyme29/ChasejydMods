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
    public class RockstarTeamTests : CustomBaseTest
    {

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

       

        [Test()]
        public void TestRockstarIncap1()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);
            AssertIncapacitated(rockstar);
            //Increase HP Recovery by Heroes by 1 until the start of your next turn.

            GoToUseIncapacitatedAbilityPhase(rockstar);
            UseIncapacitatedAbility(rockstar.CharacterCard, 0);

            SetHitPoints(new TurnTakerController[] { legacy, bunker, scholar }, 10);
            QuickHPStorage(legacy, bunker, scholar);
            Card hpGain = PlayCard("InspiringPresence");
            QuickHPCheck(2, 2, 2);
            DestroyCard(hpGain, legacy.CharacterCard);

            //should stil apply on the next turn
            GoToNextTurn();
            QuickHPUpdate();
            hpGain = PlayCard("InspiringPresence");
            QuickHPCheck(2, 2, 2);
            DestroyCard(hpGain, legacy.CharacterCard);

            //should expire by the next turn
            GoToStartOfTurn(rockstar);
            QuickHPUpdate();
            hpGain = PlayCard("InspiringPresence");
            QuickHPCheck(1, 1, 1);
            DestroyCard(hpGain, legacy.CharacterCard);
        }

        [Test()]
        public void TestRockstarIncap2()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);
            AssertIncapacitated(rockstar);
            //Select one Target. Reduce damage dealt by that Target by 1 until the start of your next turn.
            GoToUseIncapacitatedAbilityPhase(rockstar);
            DecisionSelectTarget = baron.CharacterCard;
            UseIncapacitatedAbility(rockstar.CharacterCard, 1);
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Lightning);
            QuickHPCheck(-1);

            //should still be active next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(baron, bunker, 2, DamageType.Lightning);
            QuickHPCheck(-1);

            //should expire at start of next turn
            GoToStartOfTurn(rockstar);
            QuickHPUpdate();
            DealDamage(baron, bunker, 2, DamageType.Lightning);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestRockstarIncap3()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);
            AssertIncapacitated(rockstar);

            Card envTarget = PlayCard("PlummetingMonorail");
            //Reduce damage to Hero Targets from the Environment by 1 until the start of your next turn.
            GoToUseIncapacitatedAbilityPhase(rockstar);
            UseIncapacitatedAbility(rockstar.CharacterCard, 2);

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, envTarget);
            DealDamage(envTarget, c => true, 3, DamageType.Fire);
            QuickHPCheck(-3, -2, -2, -2, -3);

            //should only reduce from environment
            SetAllTargetsToMaxHP();
            QuickHPUpdate();
            DealDamage(baron, c => true, 3, DamageType.Fire);
            QuickHPCheck(-3, -4, -3, -3, -3);

            //should still be active on the next turn
            GoToNextTurn();
            SetAllTargetsToMaxHP();
            QuickHPUpdate();
            DealDamage(envTarget, c => true, 3, DamageType.Fire);
            QuickHPCheck(-3, -2, -2, -2, -3);

            //should have expired by the start of the next turn
            GoToStartOfTurn(rockstar);
            SetAllTargetsToMaxHP();
            QuickHPUpdate();
            DealDamage(envTarget, c => true, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, -3);

        }


        [Test()]
        public void TestDivaDestroyEffect_Power()
        {

            SetupGameController("BaronBlade", "Unity", "Legacy", "Bunker", "CaptainCosmic", "Chasejyd.Rockstar", "Megalopolis");
            StartGame();
            Card ongoing = PlayCard("LivingForceField");
            Card device = GetCardInPlay("MobileDefensePlatform");
            Card envTarget = PlayCard("PlummetingMonorail");
            PlayCard("Diva");

            //The first time per turn, outside of her own, a card allows {Rockstar} to play a card or use a power , {Rockstar} may first destroy an Ongoing, Device, or Environment Card.
            GoToPlayCardPhase(unity);
            DecisionSelectTurnTaker = rockstar.TurnTaker;
            DecisionSelectCards = new Card[] {ongoing, baron.CharacterCard,  device, baron.CharacterCard, envTarget, baron.CharacterCard};
            AssertIsInPlay(ongoing);
            PlayCard("HastyAugmentation");
            AssertInTrash(ongoing);

            GoToNextTurn();
            AssertIsInPlay(device);
            //should also work for devices
            PlayCard("HastyAugmentation");
            AssertInTrash(device);

            GoToNextTurn();
            //should also work for env cards
            AssertIsInPlay(envTarget);
            PlayCard("HastyAugmentation");
            AssertInTrash(envTarget);

            //should not work in rockstar's turn
            GoToStartOfTurn(rockstar);
            PlayCard(envTarget);
            AssertIsInPlay(envTarget);
            PlayCard("HastyAugmentation");
            AssertIsInPlay(envTarget);
        }

        [Test()]
        public void TestDivaDestroyEffect_Play()
        {

            SetupGameController("BaronBlade", "Legacy/FreedomFiveLegacyCharacter", "Bunker", "CaptainCosmic", "Benchmark", "Chasejyd.Rockstar", "Megalopolis");
            StartGame();

            DealDamage(baron, bench, 99, DamageType.Cold, true);

            Card ongoing = PlayCard("LivingForceField");
            Card device = GetCardInPlay("MobileDefensePlatform");
            Card envTarget = PlayCard("PlummetingMonorail");
            PlayCard("Diva");

            //The first time per turn, outside of her own, a card allows {Rockstar} to play a card or use a power , {Rockstar} may first destroy an Ongoing, Device, or Environment Card.
            GoToUsePowerPhase(legacy);
            DecisionSelectFunction = 1;
            DecisionSelectTurnTaker = rockstar.TurnTaker;
            Card playCard = PutInHand("OwnTheStage");
            DecisionSelectCards = new Card[] { playCard, ongoing, playCard, playCard, device, playCard, envTarget,playCard };
            UsePower(legacy);
            AssertInTrash(ongoing);

            //only first time
            PlayCard(ongoing);
            PutInHand(playCard);
            UseIncapacitatedAbility(bench.CharacterCard, 0);
            AssertIsInPlay(ongoing);

            //also for devices
            GoToNextTurn();
            PutInHand(playCard);
            UsePower(legacy);
            AssertInTrash(device);

            //also for env
            GoToNextTurn();
            PutInHand(playCard);
            UsePower(legacy);
            AssertInTrash(envTarget);

            //not on her turn
            GoToStartOfTurn(rockstar);
            PlayCard(envTarget);
            PutInHand(playCard);
            int num = GetNumberOfCardsInPlay(c => true);
            UsePower(legacy);
            AssertNumberOfCardsInPlay(c => true, num + 1);
            
        }




       
        [Test()]
        public void TestRoar_DestroyEffect()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy/FreedomFiveLegacyCharacter", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(rockstar, 10);
            //The first time each turn that {Rockstar} destroys a Non-Hero card she may deal 1 Target 2 Melee Damage and gain 1 HP.
            PlayCard("RockstarRoar");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, baron.CharacterCard, null };
            QuickHPStorage(baron, rockstar);
            DestroyCard(battalion, rockstar.CharacterCard);
            QuickHPCheck(-2, 1);

            //first time only
            PlayCard(battalion);
            QuickHPUpdate();
            DestroyCard(battalion, rockstar.CharacterCard);
            QuickHPCheck(0, 0);

            //Resets at next turn
            GoToNextTurn();
            PlayCard(battalion);
            DestroyCard(battalion, rockstar.CharacterCard);
            QuickHPCheck(-2, 1);

            //only when rockstar destroys
            GoToNextTurn();
            QuickHPUpdate();
            DestroyCard(battalion, unity.CharacterCard);
            QuickHPCheck(0, 0);

            //optional damage
            DestroyCard(battalion, unity.CharacterCard);
            QuickHPCheck(0, 0);

        }

        [Test()]
        public void TestConfident_PlayOneCardOnly()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy/FreedomFiveLegacyCharacter", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card ongoing = PlayCard("StandingOvation");
            Card toPlay = PutInHand("FrontWoman");

            PlayCard("Confident");
            //The first time each turn that one of {Rockstar}'s cards is destroyed, she may play a card.
            DecisionSelectCard = toPlay;

            DestroyCard(ongoing, baron.CharacterCard);
            AssertInPlayArea(rockstar, toPlay);

            MoveCard(rockstar, toPlay, rockstar.HeroTurnTaker.Hand);
            PlayCard(ongoing);

            //only first time per turn
            DestroyCard(ongoing, baron.CharacterCard);
            AssertInHand(rockstar, toPlay);

            MoveCard(rockstar, toPlay, rockstar.HeroTurnTaker.Hand);
            PlayCard(ongoing);
            GoToNextTurn();

            //resets at next turn
            DestroyCard(ongoing, baron.CharacterCard);
            AssertInPlayArea(rockstar, toPlay);

        }

        [Test()]
        public void TestRockstarInnate()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy/FreedomFiveLegacyCharacter", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(rockstar);
            QuickHPCheck(-1);

            //when a stage presence is in play, increase this damage by 2
            PlayCard("FrontWoman");
            QuickHPUpdate();
            UsePower(rockstar);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestIntermission()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy/FreedomFiveLegacyCharacter", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(rockstar, 20);

            Card stage = PutInHand("FrontWoman");

            //{Rockstar} gains 1 HP and draws 2 Cards. {Rockstar} may Play a Stage Presence Card.
            QuickHPStorage(rockstar);
            DecisionSelectCardToPlay = stage;
            QuickHandStorage(rockstar);

            PlayCard("Intermission");

            QuickHPCheck(1);
            AssertInPlayArea(rockstar, stage);
            QuickHandCheck(1); // +2 from draws, -1 from playing stage presence


        }

        [Test()]
        public void TestEncore()
        {

            SetupGameController("BaronBlade", "Chasejyd.Rockstar", "Legacy/FreedomFiveLegacyCharacter", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card otherPowerCard = PlayCard("WickedSolo");

            Card card1 = PutInTrash("Diva");
            Card card2 = PutInTrash("OwnTheStage");
            Card card3 = PutInTrash("SoWhat");

            //Rockstar takes up to 2 Cards from her Trash and puts them in her hand.
            //Then Rockstar may use a Power.
            DecisionSelectCards = new Card[] { card2, card3 , baron.CharacterCard};
            DecisionSelectPower = otherPowerCard;
            QuickHPStorage(baron.CharacterCard);

            PlayCard("Encore");

            AssertInHand(card2, card3);
            AssertInTrash(card1);
            QuickHPCheck(-2);



            
            





        }
    }
}
