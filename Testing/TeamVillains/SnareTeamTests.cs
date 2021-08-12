using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chasejyd.SnareTeam;

namespace ChasejydTests
{
    [TestFixture()]
    public class SnareTeamTests : CustomBaseTest
    {
        [Test()]
        public void TestLoadSnareTeam()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "TheScholar", "Megalopolis");

            Assert.AreEqual(7, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(snareTeam);
            Assert.IsInstanceOf(typeof(SnareTeamCharacterCardController), snareTeam.CharacterCardController);
            Assert.IsInstanceOf(typeof(SnareTeamTurnTakerController), snareTeam);

            Assert.AreEqual(19, snareTeam.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestSnareTeamStartOfGame()
        {
            SetupGameController(new string[] { "Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();

            //Put Crimson Shield and Giger Mobility Chair into play, then shuffle the villain deck.

            Card shield = GetCard("CrimsonShield");
            Card chair = GetCard("GigerMobilityChair");
            AssertInPlayArea(snareTeam, shield);
            AssertInPlayArea(snareTeam, chair);


        }

        [Test()]
        public void TestSnareTeamFront_EndOfTurn()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(rockstar.CharacterCard, 5);
            SetHitPoints(bunker.CharacterCard, 7);

            //  At the end of {Snare}'s turn, {Snare} deals the hero character with the second lowest HP 2 energy Damage. Reduce the next damage dealt by that target by 2.
            QuickHPStorage(rockstar, bunker, tachyon);
            GoToEndOfTurn(snareTeam);
            QuickHPCheck(0, -2, 0);

            //should be normal
            QuickHPStorage(operativeTeam);
            DealDamage(rockstar, operativeTeam, 3, DamageType.Melee);
            QuickHPCheck(-3);

            SetAllTargetsToMaxHP();

            //should be reduced by 2
            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3, DamageType.Projectile);
            QuickHPCheck(-1);

            SetAllTargetsToMaxHP();

            //should be normal
            QuickHPUpdate();
            DealDamage(tachyon, operativeTeam, 3, DamageType.Sonic);
            QuickHPCheck(-3);

            SetAllTargetsToMaxHP();

            //one time only, should be normal
            QuickHPUpdate();
            DealDamage(bunker, operativeTeam, 3, DamageType.Projectile);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSnareTeamFront_BarrierDestruction()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(rockstar.CharacterCard, 5);
            SetHitPoints(bunker.CharacterCard, 7);
            SetHitPoints(tachyon.CharacterCard, 9);

            Card barrier = PlayCard("Resonant");

            //  When a barrier card is destroyed, {Snare} deals the hero target with the second highest HP 3 energy damage, then deals herself 2 irreducible psychic Damage.

            QuickHPStorage(snareTeam, rockstar, bunker, tachyon);
            DestroyCard(barrier, bunker.CharacterCard);
            QuickHPCheck(-2, 0, -3, 0);
        }

        [Test()]
        public void TestSnareTeamFront_Advanced()
        {
            SetupGameController(new string[] { "Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();

            AddReduceDamageTrigger(tachyon, true, false, 2);

            //When {Snare} deals a target energy damage, she also deals them 1 irreducible sonic damage.
            QuickHPStorage(rockstar, bunker, tachyon);
            DealDamage(snareTeam, bunker, 2, DamageType.Energy, isIrreducible: true);
            QuickHPCheck(0, -3, 0);

            QuickHPUpdate();
            DealDamage(snareTeam, bunker, 2, DamageType.Fire, isIrreducible: true);
            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestSnareTeamFront_Challenge()
        {
            SetupGameController(new string[] { "Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();

            //Giger Mobility Chair is indestructible.
            Card chair = GetCardInPlay("GigerMobilityChair");
            DestroyCard(chair, tachyon.CharacterCard);
            AssertInPlayArea(snareTeam, chair);
        }

        [Test()]
        public void TestSnareTeamBack()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            DealDamage(tachyon, snareTeam, 19, DamageType.Sonic, isIrreducible: true);
            AssertFlipped(snareTeam);

            //Reduce damage to villain character cards by 1.
            QuickHPStorage(ermineTeam, operativeTeam);
            DealDamage(tachyon, c => c.IsVillainTeamCharacter, 3, DamageType.Sonic);
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestBarricade()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(snareTeam);
            Card barricade = PlayCard("Barricade");

            //Increase damage dealt by villain character cards other than {Snare} by 1.

            //should be normal
            QuickHPStorage(bunker);
            DealDamage(snareTeam, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //should be +1
            QuickHPUpdate();
            DealDamage(ermineTeam, bunker, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //should be +1
            QuickHPUpdate();
            DealDamage(operativeTeam, bunker, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //Reduce damage dealt to villain character cards other than {Snare} by 1.

            QuickHPStorage(snareTeam);
            DealDamage(tachyon, snareTeam, 4, DamageType.Sonic);
            QuickHPCheck(-2); //crimson shield -2, no barricade reduction


            QuickHPStorage(ermineTeam);
            DealDamage(tachyon, ermineTeam, 4, DamageType.Sonic);
            QuickHPCheck(-3); //barricade -1

            QuickHPStorage(operativeTeam);
            DealDamage(tachyon, operativeTeam, 4, DamageType.Sonic);
            QuickHPCheck(-3); //barricade -1

            //At the start of {Snare}’s turn, destroy this card.
            GoToStartOfTurn(snareTeam);
            AssertInTrash(barricade);

        }

        [Test()]
        public void TestBuyingTime()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(snareTeam.CharacterCard, 10);
            SetHitPoints(ermineTeam.CharacterCard, 8);
            SetHitPoints(operativeTeam.CharacterCard, 6);

            Card buyingTime = PutInTrash("BuyingTime");
            PlayCard("Extended"); //Play another barrier
            Card reinforced = PutOnDeck("Reinforced"); //will be played at end of effect

            // The X villain targets with the lowest HP gain 2 HP, where X is equal to the number of Barrier cards in play.
            //X = 2 - should be ermine and operative
            QuickHPStorage(snareTeam, ermineTeam, operativeTeam);
            PlayCard(buyingTime);
            QuickHPCheck(0, 2, 2);

            // Play the top card of {Snare}'s deck.
            AssertInPlayArea(snareTeam, reinforced);
        }

        [Test()]
        public void TestCrimsonShield()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            Card shield = GetCardInPlay("CrimsonShield");

            //This card is indestructible
            DestroyCard(shield, tachyon.CharacterCard);
            AssertInPlayArea(snareTeam, shield);

            //Reduce damage dealt to {Snare} by 2.
            QuickHPStorage(snareTeam, ermineTeam, operativeTeam);
            DealDamage(tachyon, c => c.IsVillainTeamCharacter, 3, DamageType.Sonic);
            QuickHPCheck(-1, -3, -3);
        }

        [Test()]
        public void TestDoubleDown()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            DecisionAutoDecideIfAble = true;

            //Prep Hero Ongoings
            Card heroOngoing1 = PlayCard("PushingTheLimits");
            Card heroOngoing2 = PlayCard("AmmoDrop");
            Card heroOngoing3 = PlayCard("Confident");

            //Prep Villain Ongoings
            Card villainOngoing1 = GetCardInPlay("ConstantPrattle");
            Card villainOngoing2 = GetCardInPlay("IaidoPractitioner");

            //Prep Hero Hitpoints
            SetHitPoints(rockstar.CharacterCard, 9);
            SetHitPoints(bunker.CharacterCard, 17);
            SetHitPoints(tachyon.CharacterCard, 19);

            Card doubleDown = PlayCard("DoubleDown");
            DecisionSelectCards = new Card[] { heroOngoing1, heroOngoing2 };

            //The first time each turn that a villain ongoing is destroyed, {Snare} destroys a hero ongoing and deals the hero target with the second highest HP 2 energy damage.
            QuickHPStorage(rockstar, bunker, tachyon);
            DestroyCard(villainOngoing1, tachyon.CharacterCard);
            QuickHPCheck(0, -2, 0);
            AssertInTrash(heroOngoing1);
            AssertInPlayArea(bunker, heroOngoing2);
            AssertInPlayArea(rockstar, heroOngoing3);

            QuickHPUpdate();
            DestroyCard(villainOngoing2, tachyon.CharacterCard);
            QuickHPCheckZero();
            AssertInPlayArea(bunker, heroOngoing2);
            AssertInPlayArea(rockstar, heroOngoing3);

            //resets the next turn
            PlayCard(villainOngoing2);
            GoToNextTurn();
            QuickHPUpdate();
            DestroyCard(villainOngoing2, tachyon.CharacterCard);
            QuickHPCheck(0, -2, 0);
            AssertInTrash(heroOngoing2);
            AssertInPlayArea(rockstar, heroOngoing3);
        }


        [Test()]
        public void TestDriveTheBeatHome()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            //Prep Hero Hitpoints
            SetHitPoints(rockstar.CharacterCard, 9);
            SetHitPoints(bunker.CharacterCard, 17);
            SetHitPoints(tachyon.CharacterCard, 19);

            //{Snare} deals the hero target with the second highest HP 2 sonic and 2 energy damage.
            QuickHPStorage(rockstar, bunker, tachyon);
            PlayCard("DriveTheBeatHome");
            QuickHPCheck(0, -4, 0);

            //A hero character card dealt damage this way cannot use powers until the start of {Snare}'s next turn.
            //bunker can't use power

            GoToUsePowerPhase(bunker);

            QuickHandStorage(bunker);
            UsePower(bunker.CharacterCard);
            QuickHandCheckZero();

            QuickHandUpdate();
            UsePower(bunker.CharacterCard);
            QuickHandCheckZero();

            GoToStartOfTurn(snareTeam);
            QuickHandUpdate();
            UsePower(bunker.CharacterCard);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestEncapsulate()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "Chasejyd.SnareTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            PlayCard("AmmoDrop"); //set bunker to have the most cards in play

            //Play this card next to the hero character with the most cards in play.
            Card encapsulate = PlayCard("Encapsulate");
            AssertNextToCard(encapsulate, bunker.CharacterCard);

            //The hero next to this card cannot play cards.
            AssertCannotPlayCards(bunker);

            GoToEndOfTurn(rockstar);

            //At the start of {Snare}’s turn, she deals the hero character next to this card 2 toxic damage.
            QuickHPStorage(rockstar, bunker, tachyon);
            GoToStartOfTurn(snareTeam);
            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestEncapsulate_MultipleCC()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "Chasejyd.SnareTeam", "TheSentinels", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();


            PlayCard("SentinelTactics"); //set bunker to have the most cards in play
            DecisionSelectCard = medico;
            //Play this card next to the hero character with the most cards in play.
            Card encapsulate = PlayCard("Encapsulate");
            AssertNextToCard(encapsulate, medico);

            //The hero next to this card cannot play cards.
            AssertCannotPlayCards(sentinels);

            GoToEndOfTurn(rockstar);

            //At the start of {Snare}’s turn, she deals the hero character next to this card 2 toxic damage.
            QuickHPStorage(rockstar.CharacterCard, medico, mainstay, idealist, writhe, tachyon.CharacterCard);
            GoToStartOfTurn(snareTeam);
            QuickHPCheck(0, -2, 0,0,0, 0);
        }

        [Test()]
        public void TestExtended()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "Chasejyd.SnareTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(snareTeam.CharacterCard, 6);
            SetHitPoints(ermineTeam.CharacterCard, 18);
            SetHitPoints(operativeTeam.CharacterCard, 19);

            Card extended = PlayCard("Extended");

            // Reduce damage dealt to the villain target with the lowest HP, other than {Snare}, by 2.
            //should be ermine
            QuickHPStorage(snareTeam, ermineTeam, operativeTeam);
            DealDamage(tachyon, c => c.IsVillainTeamCharacter, 3, DamageType.Melee);
            QuickHPCheck(-1, -1, -3); //shield gives snare a -2

            // At the end of {Snare}'s turn, each villain character card gains 1 HP.
            GoToPlayCardPhase(snareTeam);
            QuickHPUpdate();
            GoToEndOfTurn(snareTeam);
            QuickHPCheck(1, 1, 1);
        }

        [Test()]
        public void TestGigerMobilityChair()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(snareTeam.CharacterCard, 10);

            Card chair = GetCardInPlay("GigerMobilityChair");
            Card police = PlayCard("PoliceBackup");

            //Reduce damage dealt to Giger Mobility Chair by 1.
            QuickHPStorage(chair);
            DealDamage(tachyon, chair, 3, DamageType.Sonic);
            QuickHPCheck(-2);

            //{Snare} and Giger Mobility Chair are immune to damage from environment cards.
            QuickHPStorage(snareTeam.CharacterCard, chair, rockstar.CharacterCard, ermineTeam.CharacterCard, bunker.CharacterCard, operativeTeam.CharacterCard, tachyon.CharacterCard);
            foreach(Card target in FindCardsWhere(c => c.IsTarget))
            {
                DealDamage(police, target, 3, DamageType.Projectile, isIrreducible: true);
            }
            QuickHPCheck(0, 0, -3, -3, -3, -3, -3);


            //At the end of {Snare}’s turn, {Snare} gains 1 HP.
            QuickHPStorage(snareTeam);
            GoToEndOfTurn(snareTeam);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestReinforced()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            Card reinforced = PlayCard("Reinforced");

            //Prevent the first damage that would be dealt to {Snare} each turn.
            QuickHPStorage(snareTeam);
            DealDamage(tachyon, snareTeam, 2, DamageType.Sonic, isIrreducible: true);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(tachyon, snareTeam, 2, DamageType.Sonic, isIrreducible: true);
            QuickHPCheck(-2);

            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(tachyon, snareTeam, 2, DamageType.Sonic, isIrreducible: true);
            QuickHPCheckZero();

            //Increase damage dealt by {Snare} by 1.
            QuickHPStorage(tachyon);
            DealDamage(snareTeam, tachyon, 2, DamageType.Fire);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestResonant()
        {
            SetupGameController("Chasejyd.SnareTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis");
            StartGame();

            //When a hero target deals damage to {Snare}, {Snare} then deals that target 2 energy damage.
            Card resonant = PlayCard("Resonant");

            QuickHPStorage(snareTeam, tachyon);
            DealDamage(tachyon, snareTeam, 2, DamageType.Sonic, isIrreducible: true);
            QuickHPCheck(-2, -2);

        }

        [Test()]
        public void TestSwitchUpTheTiming()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Bunker", "Chasejyd.SnareTeam", "Tachyon", "Megalopolis");
            StartGame();

            //Prep Hero Hitpoints
            SetHitPoints(rockstar.CharacterCard, 9);
            SetHitPoints(bunker.CharacterCard, 16);
            SetHitPoints(tachyon.CharacterCard, 19);

            Card switchUpTheTiming = PlayCard("SwitchUpTheTiming");

            //Heroes may not play cards, use powers, or draw Cards outside of their own turn.

            GoToPlayCardPhase(rockstar);

            AssertCannotPlayCards(bunker);
            AssertCannotPlayCards(tachyon);
            AssertCanPlayCards(rockstar);

            AssertCannotDrawCards(bunker);
            AssertCannotDrawCards(tachyon);
            AssertCanDrawCards(rockstar);

            AssertCannotUsePowers(bunker);
            AssertCannotUsePowers(tachyon);
            AssertCanUsePowers(rockstar);

            GoToPlayCardPhase(bunker);

            AssertCannotPlayCards(rockstar);
            AssertCannotPlayCards(tachyon);
            AssertCanPlayCards(bunker);

            AssertCannotDrawCards(rockstar);
            AssertCannotDrawCards(tachyon);
            AssertCanDrawCards(bunker);

            AssertCannotUsePowers(rockstar);
            AssertCannotUsePowers(tachyon);
            AssertCanUsePowers(bunker);

            //At the start of {Snare}’s turn, destroy this card.
            QuickHPStorage(rockstar, bunker, tachyon);
            GoToStartOfTurn(snareTeam);
            AssertInTrash(switchUpTheTiming);

            //When this card is destroyed, {Snare} deals the hero target with the lowest HP 3 psychic damage
            QuickHPCheck(-3, 0, 0);
        }

        [Test()]
        public void TestSynchronicity()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Bunker", "Chasejyd.SnareTeam", "Tachyon", "Megalopolis");
            StartGame();


            SetHitPoints(snareTeam.CharacterCard, 6);
            SetHitPoints(ermineTeam.CharacterCard, 8);
            SetHitPoints(operativeTeam.CharacterCard, 10);

            Card impromptuHeist = PutOnDeck("ImpromptuHeist");

            //Play the top card of the villain character with the lowest HP, other than {Snare}. Then that villain character gains 2 HP.
            QuickHPStorage(ermineTeam, operativeTeam, snareTeam);
            PlayCard("Synchronicity");
            QuickHPCheck(2, 0, 0);
            AssertInPlayArea(ermineTeam, impromptuHeist);
        }

        [Test()]
        public void TestTimedAssault()
        {
            SetupGameController("ErmineTeam", "Chasejyd.Rockstar", "TheOperativeTeam", "Bunker", "Chasejyd.SnareTeam", "Tachyon", "Megalopolis");
            StartGame();

            //Prep Hero Hitpoints
            SetHitPoints(rockstar.CharacterCard, 9);
            SetHitPoints(bunker.CharacterCard, 17);
            SetHitPoints(tachyon.CharacterCard, 19);

            //Prep Villain Hitpoints
            SetHitPoints(snareTeam.CharacterCard, 16);
            SetHitPoints(ermineTeam.CharacterCard, 10);
            SetHitPoints(operativeTeam.CharacterCard, 8);

            Card impromptuHeist = PutOnDeck("ImpromptuHeist");

            //Play the top card of the villain character with the highest HP, other than {Snare}. Then that villain character deals the hero target with the highest HP 2 sonic damage.
            QuickHPStorage(rockstar, bunker, tachyon);
            PlayCard("TimedAssault");
            QuickHPCheck(0, 0, -2);
            AssertInPlayArea(ermineTeam, impromptuHeist);
        }
    }
}
