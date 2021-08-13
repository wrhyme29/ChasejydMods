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

            Assert.AreEqual(34, deeprootTeam.CharacterCard.HitPoints);
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

            //At the end of {Deeproot}'s turn, the 2 Villain or Environment Targets with the lowest HP gain 2 HP.
            //Then {Deeproot} deals the X Hero Targets with the highest HP 2 Toxic Damage each where X is equal to the number of Plant Growth Cards in play

            PrintSpecialStringsForCard(deeprootTeam.CharacterCard);

            QuickHPStorage(deeprootTeam.CharacterCard, ermineTeam.CharacterCard, operativeTeam.CharacterCard, haka.CharacterCard, bunker.CharacterCard, tachyon.CharacterCard, traffic);
            GoToEndOfTurn(deeprootTeam);
            QuickHPCheck(0, 2, 0, -2, 0, 0, 2);
        }


        [Test()]
        public void TestDeeprootTeamIncap()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            DealDamage(haka, deeprootTeam, 35, DamageType.Melee, isIrreducible: true);
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

            //Play this card next to the Villain character card with the lowest HP.
            // Reduce non-fire Damage to all Villain Targets in that Villain’s play area by 2.
            Card mayor = PlayCard("MayorOverbrook");
            Card bark = PlayCard("BarkShield");
            AssertNextToCard(bark, operativeTeam.CharacterCard);

            QuickHPStorage(operativeTeam.CharacterCard, bark, mayor);
            DealDamage(haka, bark, 3, DamageType.Melee);
            QuickHPCheck(0, -1, 0);

            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3 , DamageType.Melee);
            QuickHPCheck(-1, 0, 0);

            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3, DamageType.Fire);
            QuickHPCheck(-3, 0, 0);

            QuickHPUpdate();
            DealDamage(bunker,mayor, 3, DamageType.Melee);
            QuickHPCheck(0, 0, -1);

            QuickHPStorage(ermineTeam);
            DealDamage(bunker, ermineTeam, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestCantStopTheBeatdown()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Deeproot} Deals the Hero Target with the second highest HP 4 Melee Damage. 
            //Redirect the next damage that Target would deal to {Deeproot} and reduce it by 2.

            QuickHPStorage(haka, bunker, tachyon);
            PlayCard("CantStopTheBeatdown");
            QuickHPCheck(0, -4, 0);

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

            //Increase damage from Environment Cards to Hero Targets by 1.
            QuickHPStorage(haka, ermineTeam);
            List<Card> targets = new List<Card>() { haka.CharacterCard, ermineTeam.CharacterCard };
            DealDamage(traffic, targets, 2, DamageType.Melee);
            QuickHPCheck(-3, -2);

        }

        [Test()]
        public void TestHealingSalve()
        {
            SetupGameController(new string[] { "Chasejyd.DeeprootTeam", "Haka", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(deeprootTeam.CharacterCard, 8);
            SetHitPoints(haka.CharacterCard, 8);
            SetHitPoints(ermineTeam.CharacterCard, 8);
            SetHitPoints(bunker.CharacterCard, 8);
            SetHitPoints(operativeTeam.CharacterCard, 8);
            SetHitPoints(tachyon.CharacterCard, 8);

            PreventEndOfTurnEffects(deeprootTeam, deeprootTeam.CharacterCard);

            GoToPlayCardPhase(deeprootTeam);
            PlayCard("HealingSalve");

            //Increase HP Recovery by Non-Hero Targets by 1.
            QuickHPStorage(deeprootTeam, haka, ermineTeam, bunker, operativeTeam, tachyon);
            foreach(Card cc in FindCardsWhere(c => c.IsCharacter))
            {
                GainHP(cc, 1);
            }
            QuickHPCheck(2, 1, 2, 1, 2, 1);

            //At the End of { Deeproot}’s Turn, all Non - Hero Targets gain 1 HP.
            QuickHPUpdate();
            GoToEndOfTurn(deeprootTeam);
            QuickHPCheck(2, 0, 2, 0, 2, 0);
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

            DestroyNonCharacterVillainCards();
            PlayCard("PlantLifeOfTheParty");
            //{Deeproot} is immune to Damage from Environment Cards.
            Card police = PlayCard("PoliceBackup");
            QuickHPStorage(deeprootTeam, haka, ermineTeam, bunker, operativeTeam, tachyon);
            foreach(Card target in FindCardsWhere(c => c.IsTarget))
            {
                DealDamage(police, target, 2, DamageType.Projectile, isIrreducible: true);
            }
            QuickHPCheck(0, -2, -2, -2, -2, -2)
;        }

        [Test()]
        public void TestThornwhips()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("TrafficPileup");

            GoToPlayCardPhase(deeprootTeam);
            PreventEndOfTurnEffects(bunker, deeprootTeam.CharacterCard);
            SetAllTargetsToMaxHP();

            PlayCard("Thornwhips");
            //At the End of Deeproot’s Turn, he deals the X + 1 Hero Targets with the highest HP 2 Melee Damage each, where X is equal to the number of Environment Cards in Play.
            //X = 1
            QuickHPStorage(haka, bunker, tachyon);
            GoToEndOfTurn(deeprootTeam);
            QuickHPCheck(-2, -2, 0);

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

            //Redirect all damage dealt by that target to non-hero targets to {Deeproot}
            QuickHPStorage(ermineTeam, deeprootTeam, operativeTeam, haka, bunker, tachyon);
            DealDamage(haka, c => c.IsTarget && c.IsCharacter, 4, DamageType.Projectile);
            QuickHPCheck(0, -12, 0, -4, -4, -4);

            GoToEndOfTurn(haka);

            //At the Start of {Deeproot}'s Turn, this Card deals the Hero Character it is next to 2 Melee and 2 Toxic Damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(deeprootTeam);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestWildbond()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(deeprootTeam.CharacterCard, 10);
            SetHitPoints(haka.CharacterCard, 10);
            SetHitPoints(bunker.CharacterCard, 16);
            SetHitPoints(tachyon.CharacterCard, 23);

            PlayCard("Wildbond");

            // Each time an Environment Card enters play, {Deeproot} gains 2 HP and deals the Hero Target with the highest HP 3 Melee Damage.
            QuickHPStorage(deeprootTeam, haka, bunker, tachyon);
            Card trafficPileup = PlayCard("TrafficPileup");
            QuickHPCheck(2, 0, 0, -3);

            // Each time an Environment Card is Destroyed, {Deeproot} deals the Hero Target with the Lowest HP 2 Toxic Damage.
            QuickHPUpdate();
            DestroyCard(trafficPileup, tachyon.CharacterCard);
            QuickHPCheck(0, -2, 0, 0);

        }

        [Test()]
        public void TestWildGrowth_PlantGrowthOnTop()
        {
            SetupGameController(new string[] { "ErmineTeam", "Haka", "Chasejyd.DeeprootTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(deeprootTeam, deeprootTeam.TurnTaker.Deck, deeprootTeam.TurnTaker.Trash);

            SetHitPoints(deeprootTeam, 10);
            StackAfterShuffle(deeprootTeam.TurnTaker.Deck, new string[] { "BarkShield", "Photosynthestrike", "Wildbond" });

            //Shuffle {Deeproot}'s trash into his deck.
            //Reveal the top {H} card of {Deeproot}'s deck. Put any Plant Growth Cards into Play. Discard all other cards. {Deeproot} gains 2 HP for each card discarded this way.

            QuickHPStorage(deeprootTeam);
            PlayCard("WildGrowth");
            AssertNumberOfCardsInTrash(deeprootTeam, 3);
            QuickHPCheck(4);
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
            StackAfterShuffle(deeprootTeam.TurnTaker.Deck, new string[] { "CantStopTheBeatdown", "Photosynthestrike", "Wildbond" });

            //Shuffle {Deeproot}'s trash into his deck.
            //Reveal the top {H} card of {Deeproot}'s deck. Put any Plant Growth Cards into Play. Discard all other cards. {Deeproot} gains 2 HP for each card discarded this way.

            QuickHPStorage(deeprootTeam);
            PlayCard("WildGrowth");
            AssertNumberOfCardsInTrash(deeprootTeam, 4);
            QuickHPCheck(6);
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
