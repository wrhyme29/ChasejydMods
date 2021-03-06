﻿using Handelabra;
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

        [Test()]
        public void TestBarkShield()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();

            SetHitPoints(operativeTeam.CharacterCard, 10);

            //Play this card next to the Villain character card with the lowest HP. Redirect damage that Target would take to this card.
            //Reduce non-Fire Damage dealt to this card by 1.
            Card bark = PlayCard("BarkShield");
            AssertNextToCard(bark, operativeTeam.CharacterCard);

            QuickHPStorage(operativeTeam.CharacterCard, bark);
            DealDamage(haka, operativeTeam, 2, DamageType.Melee);
            QuickHPCheck(0, -1);

            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 2, DamageType.Fire);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestCantStopTheBeatdown()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Deeproot} Deals the Hero Target with the second highest HP 3 Melee Damage. 
            //Redirect the next damage that Target would deal to {Deeproot} and reduce it by 2.

            QuickHPStorage(haka, bunker, tachyon);
            PlayCard("CantStopTheBeatdown");
            QuickHPCheck(0, -3, 0);

            QuickHPStorage(operativeTeam, deeprootTeam);
            DealDamage(bunker, operativeTeam, 3, DamageType.Toxic);
            QuickHPCheck(0, -1);

            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3, DamageType.Toxic);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestDeepRoots()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();

            Card traffic = PlayCard("TrafficPileup");

            Card roots = PlayCard("DeepRoots");

            //Reduce Damage dealt to Environment Targets by 1.
            QuickHPStorage(traffic);
            DealDamage(haka, traffic, 3, DamageType.Melee);
            QuickHPCheck(-2);

            //{Deeproot} is Immune to Damage from the Environment.
            QuickHPStorage(deeprootTeam);
            DealDamage(traffic, deeprootTeam, 3, DamageType.Melee);
            QuickHPCheckZero();

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);

            //At the end of {Deeproot}'s turn, shuffle the Environment Trash into its deck
            QuickShuffleStorage(env);
            GoToEndOfTurn(deeprootTeam);
            QuickShuffleCheck(1);

            AssertNumberOfCardsInTrash(env, 0);

        }

        [Test()]
        public void TestHeartOfTheTeam()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");

            SetHitPoints(new Card[] { traffic, deeprootTeam.CharacterCard, ermineTeam.CharacterCard, operativeTeam.CharacterCard }, 5);

            //All Villain and Environment Targets gain 2 HP.
            QuickHPStorage(deeprootTeam.CharacterCard, ermineTeam.CharacterCard, operativeTeam.CharacterCard, haka.CharacterCard, bunker.CharacterCard, tachyon.CharacterCard, traffic);
            PlayCard("HeartOfTheTeam");
            QuickHPCheck(2, 2, 2, 0, 0, 0, 2);

            //Redirect the next damage that would be dealt to a Villain Character Card to {Deeproot}.
            QuickHPStorage(operativeTeam, deeprootTeam);
            DealDamage(bunker, operativeTeam, 3, DamageType.Projectile);
            QuickHPCheck(0, -3);

            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3, DamageType.Projectile);
            QuickHPCheck(-3, 0);

        }

        [Test()]
        public void TestPhotosynthestrike()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();

            PlayCard("PoliceBackup");
            PlayCard("TrafficPileup");

            //{Deeproot} deals the Hero Target with the Highest HP X+1 toxic Damage, where X equals the number of Environment Cards in play.
            QuickHPStorage(haka, bunker, tachyon);
            PlayCard("Photosynthestrike");
            QuickHPCheck(-3, 0, 0);
            
        }

        [Test()]
        public void TestPlantLifeOfTheParty()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();

            PlayCard("BarkShield");

            //Reduce Non-Fire Damage dealt to {Deeproot} by 1
            QuickHPStorage(deeprootTeam);
            DealDamage(haka, deeprootTeam, 3, DamageType.Melee);
            QuickHPCheck(-2);

            //Increase Fire Damage dealt to {Deeproot} by 1.
            QuickHPUpdate();
            DealDamage(haka, deeprootTeam, 3, DamageType.Fire);
            QuickHPCheck(-4);

            //At the end of {Deeproot}'s turn, he gains 1 HP for each Plant Growth Card in play."

            PreventEndOfTurnEffects(deeprootTeam, deeprootTeam.CharacterCard);

            QuickHPUpdate();
            GoToEndOfTurn(deeprootTeam);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestSteadyRhythm()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");
            PlayCard("PoliceBackup");

            //{Deeproot} Deals the  X Hero Targets with the Highest HP 2 Melee Damage, where X is equal to the number of Environment Cards in Play
            //Until the start of {Deeproot}'s next turn, Villain cards are Immune to damage from Villains and the Environment.

            QuickHPStorage(haka, bunker, tachyon);
            PlayCard("SteadyRhythm");
            QuickHPCheck(-2, -2, 0);

            QuickHPStorage(deeprootTeam, ermineTeam, operativeTeam, haka, bunker, tachyon);
            DealDamage(traffic, (Card c) => c.IsNonEnvironmentTarget, 4, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -4, -4, -4);

            QuickHPUpdate();
            DealDamage(ermineTeam, (Card c) => c.IsNonEnvironmentTarget, 4, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -4, -4, -4);

            GoToStartOfTurn(deeprootTeam);
            PrintSeparator("Should be at start of turn");

            QuickHPUpdate();
            DealDamage(ermineTeam, (Card c) => c.IsNonEnvironmentTarget, 4, DamageType.Melee);
            QuickHPCheck(-4,-4,-4, -4, -4, -4);


            QuickHPUpdate();
            DealDamage(traffic, (Card c) => c.IsNonEnvironmentTarget, 4, DamageType.Melee);
            QuickHPCheck(-4, -4, -4, -4, -4, -4);

        }


        [Test()]
        public void TestStranglevines()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play this card next to the Hero Character with the highest HP. 
            Card vines = PlayCard("Stranglevines");
            AssertNextToCard(vines, haka.CharacterCard);

            //Redirect all damage dealt by that target to {Deeproot}
            QuickHPStorage(ermineTeam, deeprootTeam, operativeTeam);
            DealDamage(haka, c => c.IsVillainCharacterCard, 4, DamageType.Projectile);
            QuickHPCheck(0, -12, 0);

            GoToEndOfTurn(haka);

            //At the Start of {Deeproot}'s Turn, this Card deals the Hero Character it is next to 1 Melee and 1 Toxic Damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(deeprootTeam);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestWildbond()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(deeprootTeam.CharacterCard, 10);

            Card traffic = PlayCard("TrafficPileup");

            Card wild = PlayCard("Wildbond");

            //Reduce Damage dealt to Environment Targets by 2.",
            QuickHPStorage(traffic);
            DealDamage(haka, traffic, 5, DamageType.Melee);
            QuickHPCheck(-3);

            //Redirect Damage that would be Dealt to {Deeproot} by Environment Cards to the Hero Target with the highest HP.
            QuickHPStorage(deeprootTeam, haka);
            DealDamage(traffic, deeprootTeam, 4, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(0, -4);

            //When this Card is Destroyed, {Deeproot} gains 2 HP."
            QuickHPStorage(deeprootTeam);
            DestroyCard(wild, haka.CharacterCard);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestWildGrowth_PlantGrowthOnTop()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(deeprootTeam, deeprootTeam.TurnTaker.Deck, deeprootTeam.TurnTaker.Trash);

            SetHitPoints(deeprootTeam, 10);
            StackAfterShuffle(deeprootTeam.TurnTaker.Deck, new string[] { "BarkShield" });

            //{Deeproot} gains 1 HP.",
            //Shuffle {Deeproot}'s trash into his deck and reveal the top card of his deck. If it is a Plant Growth, put it into play. If it is not a Plant Growth, discard it and {Deeproot} gains 2 more HP."

            QuickHPStorage(deeprootTeam);
            PlayCard("WildGrowth");
            AssertNumberOfCardsInTrash(deeprootTeam, 1);
            QuickHPCheck(1);
            AssertIsInPlay("BarkShield");
        }

        [Test()]
        public void TestWildGrowth_NonPlantGrowthOnTop()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(deeprootTeam, deeprootTeam.TurnTaker.Deck, deeprootTeam.TurnTaker.Trash);

            SetHitPoints(deeprootTeam, 10);
            StackAfterShuffle(deeprootTeam.TurnTaker.Deck, new string[] { "CantStopTheBeatdown" });

            //{Deeproot} gains 1 HP.",
            //Shuffle {Deeproot}'s trash into his deck and reveal the top card of his deck. If it is a Plant Growth, put it into play. If it is not a Plant Growth, discard it and {Deeproot} gains 2 more HP."

            QuickHPStorage(deeprootTeam);
            PlayCard("WildGrowth");
            AssertNumberOfCardsInTrash(deeprootTeam, 2);
            QuickHPCheck(3);
        }
        [Test()]
        public void TestWrithingFlora()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card flora = PlayCard("WrithingFlora");

            //Reduce Damage dealt by Hero Targets by 1.
            QuickHPStorage(ermineTeam);
            DealDamage(haka, ermineTeam, 3, DamageType.Melee);
            QuickHPCheck(-2);

            //When this card is destroyed shuffle {Deeproot}'s Trash into his deck.
            MoveAllCards(deeprootTeam, deeprootTeam.TurnTaker.Deck, deeprootTeam.TurnTaker.Trash);
            DestroyCard(flora, haka.CharacterCard);
            AssertNumberOfCardsInTrash(deeprootTeam, 1);

        }
    }
}
