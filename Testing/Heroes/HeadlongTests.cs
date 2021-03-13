using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Chasejyd.Headlong;

namespace ChasejydTests
{
    [TestFixture()]
    public class HeadlongTests : CustomBaseTest
    {

        #region HeadlongTestHelpers
        public static readonly string MomentumKeyword = "momentum";

        protected bool IsMomentum(Card card)
        {
            return card.DoKeywordsContain(MomentumKeyword);
        }
        #endregion

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(headlong.CharacterCard, 1);
            DealDamage(villain, headlong, 2, DamageType.Melee, true);
        }

        [Test()]
        public void TestLoadHeadlong()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(headlong);
            Assert.IsInstanceOf(typeof(HeadlongCharacterCardController), headlong.CharacterCardController);

            Assert.AreEqual(27, headlong.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestHeadlongInnatePower_MomentumOnTop()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card momentum = PutOnDeck("SpinOut");

            //Reveal the top Card of {Headlong}'s deck. If it is a Momentum Card, put it into play. If it is not, place it in your hand and Draw a Card.
            QuickHandStorage(headlong);
            UsePower(headlong);
            QuickHandCheckZero();
            AssertInTrash(momentum);
            AssertNumberOfCardsInRevealed(headlong, 0);
        }

        [Test()]
        public void TestHeadlongInnatePower_NotMomentumOnTop()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card nonMomentum = PutOnDeck("AreaKnowledge");

            //Reveal the top Card of {Headlong}'s deck. If it is a Momentum Card, put it into play. If it is not, place it in your hand and Draw a Card.
            QuickHandStorage(headlong);
            UsePower(headlong);
            QuickHandCheck(2);
            AssertInHand(nonMomentum);
            AssertNumberOfCardsInRevealed(headlong, 0);

        }

        [Test()]
        public void TestHeadlongIncap1()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            GoToUseIncapacitatedAbilityPhase(headlong);

            Card legacyHand = GetRandomCardFromHand(legacy);
            Card bunkerHand = GetRandomCardFromHand(bunker);
            Card scholarHand = GetRandomCardFromHand(scholar);
            Card legacyTop = legacy.TurnTaker.Deck.TopCard;
            Card bunkerTop = bunker.TurnTaker.Deck.TopCard;
            Card scholarTop = scholar.TurnTaker.Deck.TopCard;

            DecisionSelectCards = new Card[] { legacyHand, null, scholarHand };
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker, scholar.TurnTaker };

            //Each Player may Discard a Card. Any Player who does so may Draw a Card.
            UseIncapacitatedAbility(headlong.CharacterCard, 0);

            AssertInTrash(legacyHand, scholarHand);
            AssertInHand(bunkerHand, legacyTop, scholarTop);
            AssertOnTopOfDeck(bunkerTop);
        }

        [Test()]
        public void TestHeadlongIncap2()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);

            Card police = PlayCard("PoliceBackup");

            GoToUseIncapacitatedAbilityPhase(headlong);

            //Until the start of Headlong's next turn, Hero Targets are Immune to Damage from the Environment.
            UseIncapacitatedAbility(headlong, 1);

            //should be immune to environment
            QuickHPStorage(baron, legacy, bunker, scholar);
            DealDamage(police, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, 0, 0, 0);

            //should not be immune to villain
            QuickHPUpdate();
            DealDamage(baron, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -4, -3, -3);

            //should not be immune to hero
            QuickHPUpdate();
            DealDamage(bunker, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -3, -3, -3);

            //should not be immune to once effect has expired
            GoToStartOfTurn(headlong);
            QuickHPUpdate();
            DealDamage(police, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -3, -3, -3);

        }

        [Test()]
        public void TestHeadlongIncap3_Discard()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);

            Card legacyTop = legacy.TurnTaker.Deck.TopCard;

            GoToUseIncapacitatedAbilityPhase(headlong);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Trash);

            //You may look at the top card of a deck, then replace it or Discard it.
            UseIncapacitatedAbility(headlong, 2);
            AssertInTrash(legacyTop);

        }

        [Test()]
        public void TestHeadlongIncap3_Replace()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetupIncap(baron);

            Card legacyTop = legacy.TurnTaker.Deck.TopCard;

            GoToUseIncapacitatedAbilityPhase(headlong);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Deck);

            //You may look at the top card of a deck, then replace it or Discard it.
            UseIncapacitatedAbility(headlong, 2);
            AssertOnTopOfDeck(legacyTop);

        }

        [Test()]
        public void TestAreaKnowledge()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card police = PutOnDeck("PoliceBackup");
            Card hostage = PutOnDeck("HostageSituation");
            Card traffic = PutOnDeck("TrafficPileup");

            DecisionSelectCards = new Card[] { police, traffic, hostage };
            DecisionMoveCardDestination = new MoveCardDestination(env.TurnTaker.Deck, toBottom: true);

            //Look at the Top 3 Cards of the Environment Deck. Place one into Play, one at the top or bottom of the Environment Deck, and one into the Trash. 
            PlayCard("AreaKnowledge");
            AssertNumberOfCardsInRevealed(headlong, 0);
            AssertInTrash(hostage);
            AssertInPlayArea(env, police);
            AssertOnBottomOfDeck(traffic);

            //Until the Start of {Headlong}’s next Turn, Heroes are Immune to Damage from Environment Cards.
            //should be immune to environment
            DecisionAutoDecideIfAble = true;

            QuickHPStorage(baron, legacy, bunker, scholar);
            DealDamage(police, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, 0, 0, 0);

            //should not be immune to villain
            QuickHPUpdate();
            DealDamage(baron, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -4, -3, -3);

            //should not be immune to hero
            QuickHPUpdate();
            DealDamage(bunker, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -3, -3, -3);

            //should not be immune to once effect has expired
            GoToStartOfTurn(headlong);
            QuickHPUpdate();
            DealDamage(police, (Card c) => true, 3, DamageType.Projectile);
            QuickHPCheck(-3, -3, -3, -3);

        }

        [Test()]
        public void TestAreaKnowledge_NoCardsInDeck()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);

            //Look at the Top 3 Cards of the Environment Deck. Place one into Play, one at the top or bottom of the Environment Deck, and one into the Trash. 
            PlayCard("AreaKnowledge");
            AssertNumberOfCardsInRevealed(headlong, 0);
            AssertNumberOfCardsInPlay(env, 0);


        }

        [Test()]
        public void TestAreaKnowledge_1CardInDeck()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);
            Card police = PutOnDeck("PoliceBackup");

            //Look at the Top 3 Cards of the Environment Deck. Place one into Play, one at the top or bottom of the Environment Deck, and one into the Trash. 
            PlayCard("AreaKnowledge");
            AssertNumberOfCardsInRevealed(headlong, 0);
            AssertInPlayArea(env, police);

        }

        [Test()]
        public void TestAreaKnowledge_2CardsInDeck()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);
            Card police = PutOnDeck("PoliceBackup");
            Card traffic = PutOnDeck("TrafficPileup");

            DecisionSelectCards = new Card[] { police, traffic };
            DecisionMoveCardDestination = new MoveCardDestination(env.TurnTaker.Deck, toBottom: true);

            //Look at the Top 3 Cards of the Environment Deck. Place one into Play, one at the top or bottom of the Environment Deck, and one into the Trash. 
            PlayCard("AreaKnowledge");
            AssertNumberOfCardsInRevealed(headlong, 0);
            AssertInPlayArea(env, police);
            AssertOnBottomOfDeck(traffic);
        }

        [Test()]
        public void TestBlindside()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card police = PlayCard("PoliceBackup");

            //Deal one Non-Hero Target X Melee Damage where X is equal to the number of Environment Cards in play +1. 
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron, headlong, legacy, bunker, scholar);
            QuickHandStorage(headlong, legacy, bunker, scholar);
            PlayCard("Blindside");
            QuickHPCheck(-2, 0, 0, 0, 0);

            //Each other Player may Draw a Card.
            QuickHandCheck(0, 1, 1, 1);

        }

        [Test()]
        public void TestBowlOver()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");
            Card battalion = PlayCard("BladeBattalion");

            //Deal up to X Targets 3 Melee Damage, where X is equal to the number of Environment Cards in play +1.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("BowlOver");
            QuickHPCheck(-3, -3, 0, 0, 0, 0);

        }

        [Test()]
        public void TestBowlOver_Dynamic()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");
            Card battalion = PlayCard("BladeBattalion");

            //Deal up to X Targets 3 Melee Damage, where X is equal to the number of Environment Cards in play +1.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            AddDestroyEnvironmentCardCounterAttackTrigger(legacy, baron.CharacterCard, legacy.CharacterCardController.GetCardSource());
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("BowlOver");
            QuickHPCheck(-3, 0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestBuildingMomentum()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the Start of {Headlong}'s Turn, he may Draw a Card. 
            PlayCard("BuildingMomentum");

            QuickHandStorage(headlong);
            DecisionYesNo = true;
            GoToStartOfTurn(headlong);
            QuickHandCheck(1);

            //{Headlong} may skip his Draw phase. If he does so, he may play a Momentum Card.
            Card setUp = PutInHand("SetUp");
            DecisionSelectCardToPlay = setUp;
            GoToDrawCardPhase(headlong);
            RunActiveTurnPhase();
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(headlong, Phase.End);
            AssertInTrash(headlong, setUp);


        }

        [Test()]
        public void TestCourier_Power()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Each Player may Draw 2 Cards. Then Destroy this Card.
            Card courier = PlayCard("Courier");
            QuickHandStorage(headlong, legacy, bunker, scholar);
            UsePower(courier);
            QuickHandCheck(2, 2, 2, 2);
            AssertInTrash(courier);

        }

        [Test()]
        public void TestCourier_Trigger()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card courier = PlayCard("Courier");

            Card hostage = PlayCard("HostageSituation");
            Card police = PlayCard("PoliceBackup");
            Card traffic = PlayCard("TrafficPileup");

            Card envTopCard = env.TurnTaker.Deck.TopCard;

            //The first time each Turn an Environment Card is Destroyed, you may look at the Top Card of the Environment Deck and replace it or Discard it.
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(env.TurnTaker.Trash), new MoveCardDestination(env.TurnTaker.Trash) };
            DestroyCard(hostage, bunker.CharacterCard);
            AssertInTrash(envTopCard);

            envTopCard = env.TurnTaker.Deck.TopCard;

            //only first time per turn
            DestroyCard(police, legacy.CharacterCard);
            AssertOnTopOfDeck(envTopCard);

            //resets at the next turn
            GoToNextTurn();
            DestroyCard(traffic, bunker.CharacterCard);
            AssertInTrash(envTopCard);
        }

        [Test()]
        public void TestFrictionlessShove()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = PlayCard("BladeBattalion");

            DecisionSelectCard = mdp;
            DecisionSelectTarget = battalion;
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);

            //Place a non-Character Villain Target in play on the top of its Deck. 
            PlayCard("FrictionlessShove");
            AssertOnTopOfDeck(mdp);

            //{Headlong} may Deal a non-Character Target 3 Projectile Damage.
            QuickHPCheck(0, -3, 0, 0, 0, 0);
        }

        [Test()]
        public void TestFrictionTransfer()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("FrictionTransfer");

            //Increase the next Damage dealt by {Headlong} by 2. 

            //should not fire for non-headlong heroes
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Projectile);
            QuickHPCheck(-2);

            //should fire for headlong
            QuickHPUpdate();
            DealDamage(headlong, baron, 2, DamageType.Melee);
            QuickHPCheck(-4);

            //should expire after use
            QuickHPUpdate();
            DealDamage(headlong, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Reduce the next Damage dealt to a Hero Target by 2.
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 4, DamageType.Lightning);
            QuickHPCheck(-2);

            //should expire after use
            QuickHPUpdate();
            DealDamage(baron, bunker, 4, DamageType.Lightning);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestOutmanuever()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play the top Card of the Environment Deck. Then Destroy an Ongoing. {Headlong} may Draw a Card.
            Card police = PutOnDeck("PoliceBackup");
            Card ongoing = PlayCard("NextEvolution");

            QuickHandStorage(headlong, legacy, bunker, scholar);
            PlayCard("Outmaneuver");
            QuickHandCheck(1, 0, 0, 0);

            AssertInPlayArea(env, police);
            AssertInTrash(ongoing);

        }

        [Test()]
        public void TestOutmanuever_EmptyEnvDeck()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play the top Card of the Environment Deck. Then Destroy an Ongoing. {Headlong} may Draw a Card.
            Card police = GetCard("PoliceBackup");
            Card ongoing = PlayCard("NextEvolution");

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);
            StackAfterShuffle(env.TurnTaker.Deck, new string[] { police.Identifier });

            QuickHandStorage(headlong, legacy, bunker, scholar);
            PlayCard("Outmaneuver");
            QuickHandCheck(1, 0, 0, 0);
            AssertIsInPlay("PoliceBackup");
            AssertInTrash(ongoing);

        }

        [Test()]
        public void TestPerfectPlacement()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card legacyPlay = PutInHand("NextEvolution");

            Card momentumToPlayWithPower = PutOnDeck("BowlOver");

            //Deal a Hero Character 2 Projectile Damage. If a Hero takes Damage this way, that Hero may Play a Card. Then {Headlong} may use a Power.
            DecisionSelectTarget = legacy.CharacterCard;
            DecisionSelectCardToPlay = legacyPlay;
            PlayCard("PerfectPlacement");
            AssertInPlayArea(legacy, legacyPlay);
            AssertInTrash(momentumToPlayWithPower);
            
        }


        [Test()]
        public void TestPerfectTiming()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card police = PlayCard("PoliceBackup");
            Card timing = PlayCard("PerfectTiming");
            Card momentum = PutInHand("BowlOver");
            PutInHand("PerfectPlacement");

            //Headlong is Immune to Damage from Environment Cards.
            QuickHPStorage(baron, headlong, legacy, bunker, scholar);
            DealDamage(police, (Card c) => c.IsTarget, 3, DamageType.Projectile);
            QuickHPCheck(-3, 0, -3, -3, -3);

            //The first time each turn that an Environment Card enters play, you may play a Momentum Card.
            DecisionSelectCard = momentum;
            Card pileup = PlayCard("TrafficPileup");
            AssertInTrash(momentum);

            PutInHand(momentum);
            DestroyCard(pileup, baron.CharacterCard);

            //should be only first in a turn
            PlayCard(pileup);
            AssertInHand(momentum);

            DestroyCard(pileup, baron.CharacterCard);
            GoToNextTurn();

            //resets at next turn
            PlayCard(pileup);
            AssertInTrash(momentum);

        }

        [Test()]
        public void TestRapidLeadership_Power()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card rapid = PlayCard("RapidLeadership");
            Card legacyPlay = PutInHand("NextEvolution");
            Card bunkerDraw = bunker.TurnTaker.Deck.TopCard;

            //One other Hero may Play a Card. Then one other Hero may Draw a Card.
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker };
            DecisionSelectCardToPlay = legacyPlay;

            UsePower(rapid);

            AssertInPlayArea(legacy, legacyPlay);
            AssertInHand(bunkerDraw);

        }

        [Test()]
        public void TestRapidLeadership_Play()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card rapid = PutInTrash("RapidLeadership");

            //When this Card comes into Play, each other Player may Draw a Card.
            QuickHandStorage(headlong, legacy, bunker, scholar);
            PlayCard(rapid);
            QuickHandCheck(0, 1, 1, 1);

        }

        [Test()]
        public void TestRecklessMomentum()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            //prep targets
            Card battalion = PlayCard("BladeBattalion");
            Card elemental = PlayCard("ElementalRedistributor");

            Card reckless = PutInHand("RecklessMomentum");
            //prep for shuffle
            int numCardsToMove = 3; //needs to be >=1 
            MoveCards(headlong, FindCardsWhere(c => c != reckless && IsMomentum(c) && c.Owner == headlong.TurnTaker).Distinct().TakeRandom(numCardsToMove, new Random()), headlong.TurnTaker.Trash);

            //Shuffle any number of Momentum Cards from your Trash into your Deck. 
            //For each Card Shuffled into your Deck this way, {Headlong} deals himself 1 Fire Damage, and then deals 1 Non-Hero Target 2 Melee Damage. 
            //He must choose a different Non-Hero Target each time.

            DecisionSelectCards = FindCardsWhere(c => IsMomentum(c) && headlong.TurnTaker.Trash.HasCard(c)).Take(numCardsToMove - 1).Concat(new List<Card>() { null, battalion, elemental });

            QuickHPStorage(baron.CharacterCard, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, battalion, elemental);
            PlayCard(reckless);
            QuickHPCheck(0, -2, 0, 0, 0, -2, -2);

            //should be 2 left, the one ignored + reckless
            AssertNumberOfCardsAtLocation(headlong.TurnTaker.Trash, 2, cardCriteria: c => IsMomentum(c));
        }
    }
}
