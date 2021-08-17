using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chasejyd.DrudgeTeam;

namespace ChasejydTests
{
    [TestFixture()]
    public class DrudgeTeamTests : CustomBaseTest
    {
        [Test()]
        public void TestLoadDrudgeTeam()
        {
            SetupGameController("Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "TheScholar", "Megalopolis");

            Assert.AreEqual(7, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drudgeTeam);
            Assert.IsInstanceOf(typeof(DrudgeTeamCharacterCardController), drudgeTeam.CharacterCardController);
            Assert.IsInstanceOf(typeof(DrudgeTeamTurnTakerController), drudgeTeam);

            Assert.AreEqual(27, drudgeTeam.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDrudgeTeamStartOfGame()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();

            //Put Immortal Form and Consume Lifeforce into play, then shuffle the villain deck.

            Card immortalForm = GetCard("ImmortalForm");
            Card consumeLifeForce = GetCard("ConsumeLifeforce");
            AssertInPlayArea(drudgeTeam, immortalForm);
            AssertInPlayArea(drudgeTeam, consumeLifeForce);
        }

        [Test()]
        public void TestDrudgeTeamFront_EndOfTurn()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of {Drudge}'s turn, {Drudge} deals the hero target with the lowest HP 2 infernal Damage.
            QuickHPStorage(legacy, bunker, tachyon);
            GoToEndOfTurn(drudgeTeam);
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestDrudgeTeamFront_RadiantIncrease()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //Increase radiant damage dealt to {Drudge} by 1.
            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController villainTarget in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillainTeam))
                {
                    QuickHPStorage(villainTarget);
                    expectedDamage = dt == DamageType.Radiant && villainTarget == drudgeTeam ? -3 : -2;
                    DealDamage(legacy, villainTarget, 2, dt, isIrreducible: true);
                    QuickHPCheck(expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }
        }

        [Test()]
        public void TestDrudgeTeamFront_VampiresIncrease()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "TheCourtOfBlood" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card dameKaterina = PlayCard("DameKatarina");

            QuickHPStorage(drudgeTeam.CharacterCard, dameKaterina);
            DealDamage(drudgeTeam, dameKaterina, 3, DamageType.Projectile);
            DealDamage(dameKaterina, drudgeTeam, 3, DamageType.Projectile);
            QuickHPCheck(-4, -4);
        }

        [Test()]
        public void TestDrudgeTeamFront_Advanced()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(drudgeTeam.CharacterCard, 20);
            Card villainTarget = PlayCard("MayorOverbrook");

            //Whenever a Villain Target is destroyed, {Drudge} gains 3 HP.
            QuickHPStorage(drudgeTeam);
            DestroyCard(villainTarget, tachyon.CharacterCard);
            QuickHPCheck(3);

        }

        [Test()]
        public void TestDrudgeTeamFront_Challenge()
        {
            SetupGameController(new string[] { "Chasejyd.DrudgeTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(bunker.CharacterCard, 5);

            //The first time per turn that {Drudge} would take 2 or less damage, redirect that damage to the Hero Target with the second highest HP.
            //2nd highest will consistently be Tachyon

            QuickHPStorage(drudgeTeam, legacy, bunker, tachyon);
            DealDamage(legacy, drudgeTeam, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2);

            //only first time
            QuickHPUpdate();
            DealDamage(legacy, drudgeTeam, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);

            //resets at next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(legacy, drudgeTeam, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2);
        }

        [Test()]
        public void TestDrudgeTeamIncap_StartOfTurn()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(tachyon.CharacterCard, 10);

            DealDamage(legacy, drudgeTeam, 27, DamageType.Radiant);
            AssertFlipped(drudgeTeam);

            GoToEndOfTurn(legacy);

            //At the start of each of {Drudge}'s turns, the Hero Target with the lowest HP deals the Hero Target with the highest HP 1 Psychic and 1 Infernal Damage.
            QuickHPStorage(legacy, bunker, tachyon);
            AssertDamageSource(tachyon.CharacterCard);
            GoToStartOfTurn(drudgeTeam);
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestBloodSacrifice()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();


            GoToEndOfTurn(legacy);
            SetAllTargetsToMaxHP();
            PlayCard("BloodSacrifice");
            //At the start of {DrudgeTeam}'s turn, {DrudgeTeam} deals 2 Infernal Damage to the non-Villain Target with the lowest HP.
            QuickShuffleStorage(drudgeTeam.TurnTaker.Deck);
            QuickHPStorage(legacy, bunker, tachyon);
            GoToStartOfTurn(drudgeTeam);
            QuickHPCheck(0, 0, -2);

            //Then reveal cards from the top of his deck until Thrall is revealed. Put Thrall into play. Shuffle all other cards revealed back into his deck.
            QuickShuffleCheck(1);
            AssertNumberOfCardsInRevealed(drudgeTeam, 0);
            AssertIsInPlay("Thrall");
        }

        [Test()]
        public void TestConsumeLifeforce()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Unity", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card raptorBot = PlayCard("RaptorBot");
            PlayCard("ConsumeLifeforce");
            SetHitPoints(drudgeTeam.CharacterCard, 10);

            // The first time each turn that {DrudgeTeam} deals Infernal Damage, he gains 2 HP.
            QuickHPStorage(drudgeTeam, legacy);
            DealDamage(drudgeTeam, legacy, 2, DamageType.Infernal);
            QuickHPCheck(2, -2);

            //only first time
            QuickHPUpdate();
            DealDamage(drudgeTeam, legacy, 2, DamageType.Infernal);
            QuickHPCheck(0, -2);

            //next turn should reset
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(drudgeTeam, legacy, 2, DamageType.Infernal);
            QuickHPCheck(2, -2);

            //When {DrudgeTeam} destroys a Target, reveal cards from the top of his deck until Thrall is revealed. Put Thrall into play. Shuffle all other cards revealed back into his deck."
            QuickShuffleStorage(drudgeTeam.TurnTaker.Deck);
            DestroyCard(raptorBot, drudgeTeam.CharacterCard);
            QuickShuffleCheck(1);
            AssertNumberOfCardsInRevealed(drudgeTeam, 0);
            AssertIsInPlay("Thrall");
        }

        [Test()]
        public void TestEnthrallingTarget()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Unity", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("EnthrallingTarget");

            //Reduce Non-Radiant Damage dealt to {DrudgeTeam} by 1

            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController villainTarget in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillainTeam))
                {
                    QuickHPStorage(villainTarget);
                    expectedDamage = villainTarget != drudgeTeam ? -2 : dt != DamageType.Radiant ? -1 : -3;
                    DealDamage(legacy, villainTarget, 2, dt);
                    QuickHPCheck(expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }

            SetHitPoints(tachyon.CharacterCard, 10);

            //{DrudgeTeam} is Immune to damage from the Hero Target with the lowest HP
           
            foreach (TurnTakerController heroSource in GameController.TurnTakerControllers.Where(ttc => ttc.IsHero))
            {
                QuickHPStorage(drudgeTeam);
                expectedDamage = heroSource == tachyon ? 0 : -2;
                DealDamage(heroSource, drudgeTeam, 3, DamageType.Fire);
                QuickHPCheck(expectedDamage);
            }

        }

        [Test()]
        public void TestFanaticalLoyalty()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();


            PlayCard("FanaticalLoyalty");
            Card thrall = PlayCard("Thrall");

            // Increase Damage dealt by Thralls by 1.
            QuickHPStorage(legacy);
            DealDamage(thrall, legacy, 2, DamageType.Fire);
            QuickHPCheck(-3);

            // Reduce Damage dealt to Thralls by 1.
            QuickHPStorage(thrall);
            DealDamage(legacy, thrall, 2, DamageType.Melee);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestFeastOnTheLiving()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(drudgeTeam, 10);

            IEnumerable<Card> thralls = FindCardsWhere(c => c.Identifier == "Thrall");
            MoveCards(drudgeTeam, thralls, drudgeTeam.TurnTaker.Trash);

            //{DrudgeTeam} deals the Hero Target with the second highest HP 2 Melee and 2 Infernal Damage. Then {DrudgeTeam} gains 3 HP.
            //Shuffle all Thralls in {DrudgeTeam}'s trash into his deck.

            QuickShuffleStorage(drudgeTeam.TurnTaker.Deck);
            QuickHPStorage(drudgeTeam, legacy, bunker, tachyon);

            PlayCard("FeastOnTheLiving");

            QuickHPCheck(3, 0, -4, 0);
            QuickShuffleCheck(1);
            AssertInDeck(thralls);
        }

        [Test()]
        public void TestFiendishGuidance()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(legacy);

            Card thrall1 = PlayCard("Thrall", 0);
            Card thrall2 = PlayCard("Thrall", 1);

            SetHitPoints(ermineTeam.CharacterCard, 10);
            SetHitPoints(drudgeTeam.CharacterCard, 10);
            SetHitPoints(operativeTeam.CharacterCard, 10);
            SetHitPoints(thrall1, 1);
            SetHitPoints(thrall2, 1);

            PlayCard("FiendishGuidance");

            //At the start of {DrudgeTeam}'s turn, each Villain Character gains 1 HP and all Thralls in play gain 2 HP.
            QuickHPStorage(drudgeTeam.CharacterCard, ermineTeam.CharacterCard, operativeTeam.CharacterCard, thrall1, thrall2);
            GoToStartOfTurn(drudgeTeam);
            QuickHPCheck(1, 1, 1, 2, 2);

            DestroyNonCharacterVillainCards();

            PlayCard("FiendishGuidance");

            //{DrudgeTeam} is Immune to damage from Villain cards.
            QuickHPStorage(drudgeTeam);
            DealDamage(ermineTeam, drudgeTeam, 5, DamageType.Projectile, isIrreducible: true);
            QuickHPCheckZero();
        }

        [Test()]
        public void TestImmortalForm()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //When {DrudgeTeam} drops to 0 or fewer HP, restore {DrudgeTeam} to 5 HP. Then, destroy this card.
            Card immortalForm = PlayCard("ImmortalForm");

            DealDamage(legacy, drudgeTeam, 99, DamageType.Infernal, isIrreducible: true);

            AssertHitPoints(drudgeTeam.CharacterCard, 5);
            AssertInTrash(immortalForm);
        }

        [Test()]
        public void TestInciteHysteria()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            IEnumerable<Card> thralls = FindCardsWhere(c => c.Identifier == "Thrall");
            MoveCards(drudgeTeam, thralls, drudgeTeam.TurnTaker.Trash);

            PlayCards(new Card[] { thralls.ElementAt(0), thralls.ElementAt(1) });
            Card trafficPileup = PlayCard("TrafficPileup");
            QuickShuffleStorage(drudgeTeam.TurnTaker.Deck);

            // {DrudgeTeam} deals the 3 non-Villain Targets with the lowest HP 2 Psychic Damage each.
            //  Each Thrall in play deals the Hero Target with the highest HP 2 Melee Damage.
            //  Shuffle all Thralls in {DrudgeTeam}'s trash into his deck.
            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, tachyon.CharacterCard, trafficPileup);
            PlayCard("InciteHysteria");
            QuickHPCheck(-4, -2, -2, -2);
            QuickShuffleCheck(1);
            AssertInDeck(new Card[] { thralls.ElementAt(2), thralls.ElementAt(3) });

        }

        [Test()]
        public void TestInfernalRites()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Unity", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("ImmortalForm");
            PlayCard("VampiricAura");

            GoToEndOfTurn(legacy);
            SetHitPoints(drudgeTeam, 10);
            SetHitPoints(bunker, 1);
            DealDamage(legacy, bunker, 2, DamageType.Infernal);
            AssertIncapacitated(bunker);

            Card raptorBot = PlayCard("RaptorBot");
            Card mysticTraining = PutOnDeck("MysticTraining");

            Card infernalRites = PlayCard("InfernalRites");

            //At the start of {DrudgeTeam}'s turn, he deals the X Non-Villain Targets with the highest HP 1 Irreducible Infernal and 1 Irreducible Psychic Damage each, where X is equal to the number of Vampiric cards in play. If Drudge destroys a Target with this damage he gain 2 HP and then play the top Card of his Deck.",
            QuickHPStorage(legacy, drudgeTeam, unity);
            GoToStartOfTurn(drudgeTeam);
            QuickHPCheck(-2, 2, -2);
            AssertInTrash(raptorBot);
            AssertInPlayArea(drudgeTeam, mysticTraining);

            //If {DrudgeTeam} takes 3 or more radiant damage, destroy this card.
            DealDamage(legacy, drudgeTeam, 1, DamageType.Radiant);
            AssertInPlayArea(drudgeTeam, infernalRites);

            DealDamage(legacy, drudgeTeam, 3, DamageType.Fire);
            AssertInPlayArea(drudgeTeam, infernalRites);

            DealDamage(legacy, drudgeTeam, 3, DamageType.Radiant);
            AssertInTrash(infernalRites);

        }

        [Test()]
        public void TestMysticTraining()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //Increase Infernal and Psychic Damage dealt by {DrudgeTeam} by 1.
            PlayCard("MysticTraining");
            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                 QuickHPStorage(legacy);
                expectedDamage = dt == DamageType.Infernal || dt == DamageType.Psychic ? -3 : -2;
                DealDamage(drudgeTeam, legacy, 2, dt, isIrreducible: true);
                QuickHPCheck(expectedDamage);
                SetAllTargetsToMaxHP();
            }

        }

        [Test()]
        public void TestRisingDarkness()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card immortalForm = GetCard("ImmortalForm");
            QuickShuffleStorage(drudgeTeam.TurnTaker.Deck);
            PlayCard("RisingDarkness");
            //If Immortal Form is in {DrudgeTeam}'s trash, shuffle it back into his deck.
            AssertInDeck(immortalForm);
            QuickShuffleCheck(1);

            //Until the start of {DrudgeTeam}'s next turn, Non-Villain cards cannot gain HP.
            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null))
            {
                SetHitPoints(ttc.CharacterCard, 5);
            }

            QuickHPStorage(ermineTeam, legacy, drudgeTeam, bunker, operativeTeam, tachyon);
            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null))
            {
                GainHP(ttc.CharacterCard, 2);
            }
            QuickHPCheck(2, 0, 2, 0, 2, 0);

            GoToStartOfTurn(drudgeTeam);
            QuickHPUpdate();
            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null))
            {
                GainHP(ttc.CharacterCard, 2);
            }
            QuickHPCheck(2, 2, 2, 2, 2, 2);

        }

        [Test()]
        public void TestThrall()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(drudgeTeam);
            Card thrall = PlayCard("Thrall");
            PreventEndOfTurnEffects(drudgeTeam, drudgeTeam.CharacterCard);
            //At the end of {DrudgeTeam}'s turn, Thrall deals the Hero Target with the highest HP 2 Melee Damage.
            SetAllTargetsToMaxHP();
            QuickHPStorage(legacy, bunker, tachyon);
            GoToEndOfTurn(drudgeTeam);
            QuickHPCheck(-2, 0, 0);

            //The first time each turn that {DrudgeTeam} would be dealt non-Radiant Damage, redirect that Damage to the Thrall with the lowest HP.
            QuickHPStorage(drudgeTeam.CharacterCard, thrall);
            DealDamage(legacy, drudgeTeam, 1, DamageType.Melee);
            QuickHPCheck(0, -1);

            QuickHPStorage(drudgeTeam.CharacterCard, thrall);
            DealDamage(legacy, drudgeTeam, 1, DamageType.Melee);
            QuickHPCheck(-1, 0);

            GoToNextTurn();
            QuickHPStorage(drudgeTeam.CharacterCard, thrall);
            DealDamage(legacy, drudgeTeam, 1, DamageType.Radiant);
            QuickHPCheck(-2, 0);

            QuickHPStorage(drudgeTeam.CharacterCard, thrall);
            DealDamage(legacy, drudgeTeam, 1, DamageType.Melee);
            QuickHPCheck(0, -1);
        }

        [Test()]
        public void TestVampiricAura()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //Hero Targets damaged by {DrudgeTeam} cannot gain HP until the start of {DrudgeTeam}'s next turn.
            PlayCard("VampiricAura");

            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null && tc.IsHero))
            {
                SetHitPoints(ttc.CharacterCard, 15);
            }

            DealDamage(drudgeTeam, legacy, 2, DamageType.Infernal, isIrreducible: true);
            QuickHPStorage(legacy, bunker, tachyon);

            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null && tc.IsHero))
            {
                GainHP(ttc.CharacterCard, 2);
            }

            QuickHPCheck(0, 2, 2);

            GoToStartOfTurn(drudgeTeam);
            QuickHPStorage(legacy, bunker, tachyon);

            foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(tc => tc.CharacterCard != null && tc.IsHero))
            {
                GainHP(ttc.CharacterCard, 2);
            }

            QuickHPCheck(2, 2, 2);

        }

        [Test()]
        public void TestVampiricCunning_LessThan3()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card heroOngoing1 = PlayCard("InspiringPresence");
            Card heroOngoing2 = PlayCard("PushingTheLimits");
            Card heroOngoing3 = PlayCard("AmmoDrop");

            DecisionSelectCards = new Card[] { heroOngoing1, heroOngoing2 };

            //Destroy a Hero Ongoing. If there are 3 or more Vampiric Cards in play, destroy a second Hero Ongoing.
            PlayCard("VampiricCunning");

            AssertInTrash(heroOngoing1);
            AssertInPlayArea(tachyon, heroOngoing2);
            AssertInPlayArea(bunker, heroOngoing3);

        }

        [Test()]
        public void TestVampiricCunning_3OrMore()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.DrudgeTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("ImmortalForm");
            PlayCard("VampiricAura");
            PlayCard("ConsumeLifeforce");

            Card heroOngoing1 = PlayCard("InspiringPresence");
            Card heroOngoing2 = PlayCard("PushingTheLimits");
            Card heroOngoing3 = PlayCard("AmmoDrop");

            DecisionSelectCards = new Card[] { heroOngoing1, heroOngoing2 };

            //Destroy a Hero Ongoing. If there are 3 or more Vampiric Cards in play, destroy a second Hero Ongoing.
            PlayCard("VampiricCunning");

            AssertInTrash(heroOngoing1);
            AssertInTrash(heroOngoing2);
            AssertInPlayArea(bunker, heroOngoing3);

        }
    }
}
