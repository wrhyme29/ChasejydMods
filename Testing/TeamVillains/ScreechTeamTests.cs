using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Chasejyd.ScreechTeam;

namespace ChasejydTests
{
    [TestFixture()]
    public class ScreechTeamTests : CustomBaseTest
    {
        [Test()]
        public void TestLoadScreechTeam()
        {
            SetupGameController("Chasejyd.ScreechTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "TheScholar", "Megalopolis");

            Assert.AreEqual(7, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(screechTeam);
            Assert.IsInstanceOf(typeof(ScreechTeamCharacterCardController), screechTeam.CharacterCardController);
            Assert.IsInstanceOf(typeof(ScreechTeamTurnTakerController), screechTeam);

            Assert.AreEqual(23, screechTeam.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestScreechTeamStartOfGame()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Chasejyd.Rockstar", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            //Put Total Chaos into play, then shuffle the villain deck.

            Card chaos = GetCard("TotalChaos");
            AssertInPlayArea(screechTeam, chaos);
        }

        [Test()]
        public void TestScreechFront_EndOfTurn()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of {Screech}'s turn, he deals the Hero Target with the highest HP 3 Sonic Damage. Then one player must discard a card.
            //highest HP is Legacy
            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHPStorage(legacy, bunker, tachyon);
            QuickHandStorage(legacy, bunker, tachyon);
            GoToEndOfTurn(screechTeam);
            QuickHPCheck(-3, 0, 0);
            QuickHandCheck(0, -1, 0);
        }

        [Test()]
        public void TestScreechFront_ImmuneToSonic()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //Screech is immune to Sonic Damage

            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController villainTarget in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillainTeam))
                {
                    QuickHPStorage(villainTarget);
                    expectedDamage = dt == DamageType.Sonic && villainTarget == screechTeam ? 0 : -2;
                    DealDamage(legacy, villainTarget, 2, dt, isIrreducible: true);
                    QuickHPCheck(expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }
        }

        [Test()]
        public void TestScreechFront_Advanced()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            //When {Screech} deals a Hero Character 3 or more Sonic damage, that player must discard a card.

            QuickHandStorage(tachyon);
            DealDamage(screechTeam, tachyon, 3, DamageType.Sonic);
            QuickHandCheck(-1);

            QuickHandUpdate();
            DealDamage(screechTeam, tachyon, 3, DamageType.Fire);
            QuickHandCheck(0);


            QuickHandUpdate();
            DealDamage(screechTeam, tachyon, 2, DamageType.Sonic);
            QuickHandCheck(0);

            QuickHandUpdate();
            DealDamage(ermineTeam, tachyon, 3, DamageType.Sonic);
            QuickHandCheck(0);


            QuickHandUpdate();
            DealDamage(screechTeam, tachyon, 5, DamageType.Sonic);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestScreechFront_Challenge()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Bunker", "TheOperativeTeam", "Tachyon", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();

            //Increase Sonic Damage by 1.
            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController villainTarget in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillainTeam))
                {
                    QuickHPStorage(legacy, bunker, tachyon);
                    expectedDamage = dt == DamageType.Sonic ? -3 : -2;
                    DealDamage(villainTarget.CharacterCard, FindCardsWhere(c => c.IsHeroCharacterCard), 2, dt, isIrreducible: true);
                    QuickHPCheck(expectedDamage, expectedDamage, expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }
        }


        [Test()]
        public void TestScreechIncap()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            DealDamage(legacy, screechTeam, 23, DamageType.Melee, isIrreducible: true);
            AssertFlipped(screechTeam.CharacterCard);

            GoToEndOfTurn(legacy);
            //At the start of each of {Screech}'s turns, the Hero Characters with the highest and lowest HP must discard 1 card.
            QuickHandStorage(legacy, ra, tachyon);
            GoToStartOfTurn(screechTeam);
            QuickHandCheck(-1, 0, -1);
        }

        [Test()]
        public void TestBringTheNoise()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();


            GoToEndOfTurn(legacy);

            PlayCard("BringTheNoise");

            Card discord = PutOnDeck("TurnItUp");
            PutOnDeck("RattleTheirBrains");

            //At the start of {Screech}'s turn, {Screech} deals himself 2 Irreducible Toxic Damage. Then reveal cards from his deck until a Discord card is revealed and put it into play. Shuffle the other revealed cards back into {Screech}'s deck.
            QuickShuffleStorage(screechTeam.TurnTaker.Deck);
            QuickHPStorage(screechTeam);
            GoToStartOfTurn(screechTeam);
            QuickShuffleCheck(1);
            AssertInPlayArea(screechTeam, discord);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestEarworm()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of {Screech}'s turn, {Screech} deals each Hero Target 1 Sonic Damage.
            GoToPlayCardPhase(screechTeam);
            PreventEndOfTurnEffects(screechTeam, screechTeam.CharacterCard);
            PlayCard("Earworm");
            QuickHPStorage(legacy, ra, tachyon);
            GoToEndOfTurn(screechTeam);
            QuickHPCheck(-1, -1, -1);


        }
        [Test()]
        public void TestLeadSinger()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(legacy);
            SetHitPoints(screechTeam, 10);
            Card leadSinger = PlayCard("LeadSinger");

            //At the Start of {Screech}'s turn, he gains X HP, where X is equal to the number of Villain Character cards in play.
            QuickHPStorage(screechTeam);
            GoToStartOfTurn(screechTeam);
            QuickHPCheck(3);

            //If a Villain Character deals damage to {Screech}, Destroy this card.
            DealDamage(ermineTeam, screechTeam, 2, DamageType.Projectile, isIrreducible: true);
            AssertInTrash(leadSinger);

        }

        [Test()]
        public void TestMaximumDecibels()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(screechTeam);
            PreventEndOfTurnEffects(screechTeam, screechTeam.CharacterCard);
            Card maximumDecibels = PlayCard("MaximumDecibels");

            //Increase Sonic Damage dealt by Screech by 2.
            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController villainTarget in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillainTeam))
                {
                    QuickHPStorage(legacy, ra, tachyon);
                    expectedDamage = dt == DamageType.Sonic && villainTarget == screechTeam ? -4 : -2;
                    DealDamage(villainTarget.CharacterCard, FindCardsWhere(c => c.IsHeroCharacterCard), 2, dt, isIrreducible: true);
                    QuickHPCheck(expectedDamage, expectedDamage, expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }

            Card leadSinger = PlayCard("LeadSinger");

            //At the end of {Screech}'s turn, if there are no other Discord cards in play, destroy this card.
            GoToEndOfTurn(screechTeam);
            AssertInPlayArea(screechTeam, maximumDecibels);


            GoToPlayCardPhase(screechTeam);
            PreventEndOfTurnEffects(screechTeam, screechTeam.CharacterCard);
            PrintSeparator("Destroying other discord and going to end phase");
            DestroyCard(leadSinger, tachyon.CharacterCard);
            GoToEndOfTurn(screechTeam);
            AssertInTrash(maximumDecibels);
        }

        [Test()]
        public void TestMaximumDisruption()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card trafficPileup = PlayCard("TrafficPileup"); //env1
            Card police = PlayCard("PoliceBackup"); //env2
            Card bringTheNoise = PlayCard("BringTheNoise"); //v-ongoing1
            Card leadSinger = PlayCard("LeadSinger"); //v-ongoing2
            Card inspiringPresence = PlayCard("InspiringPresence"); //h-ongoing1
            Card legacyRing = PlayCard("TheLegacyRing"); //h-equip1
            Card pushingTheLimits = PlayCard("PushingTheLimits"); //h-ongoing2
            Card staffOfRa = PlayCard("TheStaffOfRa"); //h-equip2

            //Destroy 1 Environment Card, 1 Villain Ongoing, and 3 Hero Ongoings or Equipment Cards.
            //{Screech} deals each Hero Target 1 Projectile Damage.

            DecisionSelectCards = new Card[] { trafficPileup, leadSinger, legacyRing, pushingTheLimits, staffOfRa };
            DecisionAutoDecideIfAble = true;

            QuickHPStorage(legacy, ra, tachyon);
            PlayCard("MaximumDisruption");
            QuickHPCheck(-1, -1, -1);

            AssertInTrash(trafficPileup);
            AssertInPlayArea(env, police);
            AssertInTrash(leadSinger);
            AssertInPlayArea(screechTeam, bringTheNoise);
            AssertInPlayArea(legacy, inspiringPresence);
            AssertInTrash(legacyRing);
            AssertInTrash(pushingTheLimits);
            AssertInTrash(staffOfRa);
        }


        [Test()]
        public void TestNoSelfControl()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "BaronBladeTeam", "TheArgentAdept", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Screech} deals himself 3 Irreducible Toxic Damage. Then {Screech} deals the {H - 2} Hero Targets with the highest HP 3 Sonic Damage each. Any Hero Characters Dealt Damage this way must Discard a Card.
            QuickHPStorage(ermineTeam, legacy, screechTeam, ra, operativeTeam, tachyon, baronTeam, adept);
            QuickHandStorage(legacy, ra, tachyon, adept);
            PlayCard("NoSelfControl");
            QuickHPCheck(0, -3, -3, -3, 0, 0, 0, 0);
            QuickHandCheck(-1, -1, 0, 0);
        }

        [Test()]
        public void TestRattleTheirBrains()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHandStorage(legacy, ra, tachyon);
            QuickHPStorage(legacy, ra, tachyon);

            PlayCard("Fortitude"); // add a -1 for legacy
            PlayCard("RattleTheirBrains");
            //Each player must Discard 1 Card.
            QuickHandCheck(-1, -1, -1);
            //{Screech} deals each hero target 1 Sonic Damage. Any targets dealt damage this way then deals themselves 1 Psychic Damage.
            QuickHPCheck(0, -2, -2);
        }

        [Test()]
        public void TestRingTheirEars()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Screech} deals the Hero Target with the second lowest HP 2 Sonic Damage. That Target then deals the Hero Target with the highest HP 3 Melee Damage.
            //second lowest is Ra, highest is Legacy
            QuickHPStorage(legacy, ra, tachyon);
            PlayCard("RingTheirEars");
            QuickHPCheck(-3, -2, 0);

        }

        [Test()]
        public void TestScreamOfAnger()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //{Screech} deals the Non-Villain Target with the highest HP 3 Sonic Damage. If a Target takes damage this way, reduce Damage dealt by that Target by 1 until the start of {Screech}'s next turn.
            QuickHPStorage(legacy, ra, tachyon);
            PlayCard("ScreamOfAnger");
            QuickHPCheck(-3, 0, 0);

            QuickHPStorage(ra);
            DealDamage(legacy, ra, 2, DamageType.Melee);
            QuickHPCheck(-1);

            GoToStartOfTurn(screechTeam);
            QuickHPUpdate();
            DealDamage(legacy, ra, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestShakeTheirNerves()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("Fortitude"); //give legacy a -1

            Card shakeTheirNerves = PutInTrash("ShakeTheirNerves");

            List<Card> topCards = new List<Card>();

            foreach (Location deck in GameController.Game.TurnTakers.Select(tt => tt.Deck).Where(loc => loc.HasCards))
            {
                topCards.Add(deck.TopCard);
            }

            QuickHPStorage(legacy, ra, tachyon);

            //Discard the top Card of each Deck.
            //{Screech} deals each Hero Target 1 Sonic Damage. Heroes dealt Damage this way cannot Draw cards until the start of {Screech}'s next turn.
            PlayCard(shakeTheirNerves);

            AssertInTrash(topCards);
            QuickHPCheck(0, -1, -1);

            AssertCanDrawCards(legacy);
            AssertCannotDrawCards(ra);
            AssertCannotDrawCards(tachyon);

            GoToStartOfTurn(screechTeam);

            AssertCanDrawCards(legacy);
            AssertCanDrawCards(ra);
            AssertCanDrawCards(tachyon);
        }

        [Test()]
        public void TestShatter()
        {
            SetupGameController(new string[] { "ErmineTeam", "Unity", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card constructionPylon = PlayCard("ConstructionPylon");
            Card staffOfRa = PlayCard("TheStaffOfRa");

            Card turretBot = PutIntoPlay("TurretBot");
            Card champtionBot = PutIntoPlay("ChampionBot");
            DecisionSelectCard = constructionPylon;
            DecisionAutoDecideIfAble = true;

            //Destroy a hero equipment card. Then {Screech} Deals all Targets in the play area the Equipment card was in 1 Sonic Damage and 1 Projectile Damage.
            QuickHPStorage(unity.CharacterCard, turretBot, champtionBot, ra.CharacterCard, tachyon.CharacterCard);
            PlayCard("Shatter");
            AssertInTrash(constructionPylon);
            AssertInPlayArea(ra, staffOfRa);
            QuickHPCheck(-2, -2, -2, 0, 0);
        
        }

        [Test()]
        public void TestShoutItOut()
        {
            SetupGameController(new string[] { "ErmineTeam", "Legacy", "Chasejyd.ScreechTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("LeadSinger");
            PlayCard("TurnItUp");
            PlayCard("MaximumDecibels");

            //{Screech} deals the Hero Target with the highest HP X Sonic Damage, where X is equal to the number of Discord cards in play.
            //Then {Screech} deals each other Hero Target 1 Projectile Damage.

            QuickHPStorage(legacy, ra, tachyon);
            PlayCard("ShoutItOut");
            QuickHPCheck(-5, -1, -1);
        }

        [Test()]
        public void TestTotalChaos()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();
            Card totalChaos = PlayCard("TotalChaos");
            Card inspiringPresence = PlayCard("InspiringPresence");
            Card pushingTheLimits = PlayCard("PushingTheLimits");
            DecisionSelectCard = inspiringPresence;

            PreventEndOfTurnEffects(screechTeam, screechTeam.CharacterCard);

            //At the end of {Screech}'s turn, destroy a Hero Ongoing, then {Screech} deals the Hero Target with the highest HP 1 Sonic and 1 Projectile damage.
            QuickHPStorage(legacy, ra, tachyon);
            GoToEndOfTurn(screechTeam);
            QuickHPCheck(-2, 0, 0);

            AssertInTrash(inspiringPresence);
            AssertInPlayArea(tachyon, pushingTheLimits);

            //Increase Damage dealt to Screech by other Villain Targets by 1.

            QuickHPStorage(screechTeam);
            int expectedDamage;
            foreach(Card source in FindCardsWhere(c => c.IsTarget && c.IsInPlayAndHasGameText))
            {
                expectedDamage = source.IsVillainTarget ? -3 : -2;
                DealDamage(source, screechTeam.CharacterCard, 2, DamageType.Radiant);
                QuickHPCheck(expectedDamage);
                SetAllTargetsToMaxHP();
                QuickHPUpdate();
            }
        }

        [Test()]
        public void TestTurnItUp()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "Ra", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //The first time each turn that a Hero discards a card, {Screech} deals that Target 1 Irreducible Sonic Damage.

            Card turnItUp = PlayCard("TurnItUp");

            PrintSpecialStringsForCard(turnItUp);
            QuickHPStorage(legacy, ra, tachyon);
            DiscardCard(legacy);
            QuickHPCheck(-1, 0, 0);

            PrintSpecialStringsForCard(turnItUp);

            QuickHPUpdate();
            DiscardCard(ra);
            QuickHPCheckZero();

            GoToNextTurn();

            QuickHPUpdate();
            DiscardCard(tachyon);
            QuickHPCheck(0,0,-1);

        }

        [Test()]
        public void TestTurnItUp_MultipleCC()
        {
            SetupGameController(new string[] { "Chasejyd.ScreechTeam", "Legacy", "ErmineTeam", "TheSentinels", "TheOperativeTeam", "Tachyon", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            //The first time each turn that a Hero discards a card, {Screech} deals that Target 1 Irreducible Sonic Damage.

            PlayCard("TurnItUp");
            DecisionSelectCard = mainstay;
            QuickHPStorage(legacy.CharacterCard, medico, mainstay, idealist, writhe, tachyon.CharacterCard);
            DiscardCard(sentinels);
            QuickHPCheck(0, 0, -1, 0, 0, 0);

        }
    }
}