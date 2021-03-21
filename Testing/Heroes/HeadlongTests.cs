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
        public void TestDaybreakHeadlongInnatePower()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong/DaybreakHeadlongCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card police = PutOnDeck("PoliceBackup");
            //Two Players may draw a card. 
            //You may Destroy an Environment Card or Play the top Card of the Environment Deck.
            DecisionSelectFunction = 1;
            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker, scholar.TurnTaker };
            QuickHandStorage(headlong, legacy, bunker, scholar);
            UsePower(headlong);
            QuickHandCheck(0, 0, 1, 1);
            AssertInPlayArea(env, police);
        }

        [Test()]
        public void TestDaybreakHeadlongIncap1()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong/DaybreakHeadlongCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            //Increase Damage dealt by Environment Cards to Villain Targets by 1 until the Start of your next turn

            UseIncapacitatedAbility(headlong, 0);

            Card police = PlayCard("PoliceBackup");
            QuickHPStorage(baron, legacy, bunker, scholar);
            DealDamage(police, c => c.IsTarget, 3, DamageType.Projectile);
            QuickHPCheck(-4, -3, -3, -3);

            GoToStartOfTurn(headlong);
            QuickHPUpdate();
            DealDamage(police, c => c.IsTarget, 3, DamageType.Projectile);
            QuickHPCheck(-3, -3, -3, -3);
        }

        [Test()]
        public void TestDaybreakHeadlongIncap2()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong/DaybreakHeadlongCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            //Two Players may draw a card. 
            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker, scholar.TurnTaker };
            QuickHandStorage(legacy, bunker, scholar);

            UseIncapacitatedAbility(headlong, 1);

            QuickHandCheck( 0, 1, 1);
        }

        [Test()]
        public void TestDaybreakHeadlongIncap3()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong/DaybreakHeadlongCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            //One Hero may Play a Card.
            Card ammoDrop = PutInHand("AmmoDrop");
            AssertIncapLetsHeroPlayCard(headlong, 2, bunker, "AmmoDrop");
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

            //Deal up to X Targets 2 Melee Damage, where X is equal to the number of Environment Cards in play +1.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("BowlOver");
            QuickHPCheck(-2, -2, 0, 0, 0, 0);

        }

        [Test()]
        public void TestBowlOver_Dynamic()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");
            Card battalion = PlayCard("BladeBattalion");

            //Deal up to X Targets 2 Melee Damage, where X is equal to the number of Environment Cards in play +1.
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            AddDestroyEnvironmentCardCounterAttackTrigger(legacy, baron.CharacterCard, legacy.CharacterCardController.GetCardSource());
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("BowlOver");
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

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
        public void TestFrictionlessShove_onlyVillainTargets()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card backlash = PlayCard("BacklashField");
            Card battalion = PlayCard("BladeBattalion");

            DecisionSelectCard = mdp;
            DecisionSelectTarget = battalion;
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);

            //Place a non-Character Villain Target in play on the top of its Deck. 
            AssertNextDecisionChoices(notIncluded: backlash.ToEnumerable());
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
        public void TestRapidEvac()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");
            Card hostage = PutOnDeck("HostageSituation");
            SetHitPoints(baron, 5);
            SetHitPoints(traffic, 5);
            SetHitPoints(headlong, 5);
            SetHitPoints(legacy, 5);
            SetHitPoints(bunker, 5);
            SetHitPoints(scholar, 5);

            //Destroy an Environment Card. 
            //Then, Reveal the top Card of the Environment Deck. Play it or Discard it. 
            //Then each Hero and Environment Target gains 1 HP.

            DecisionSelectCard = police;
            DecisionMoveCardDestination = new MoveCardDestination(env.TurnTaker.Trash); // discard
            QuickHPStorage(baron.CharacterCard, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            
            PlayCard("RapidEvac");

            AssertInTrash(police, hostage);
            QuickHPCheck(0, 1, 1, 1, 1, 1);
           

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
        public void TestSetUpCard()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PlayCard("BladeBattalion");
            Card setup = PutInTrash("SetUp");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker, legacy.TurnTaker };

            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(headlong, legacy, bunker, scholar);

            PlayCard(setup);

            //Deal one Non-Hero Target 1 Melee Damage. 
            //If a Target takes Damage this way, another Hero deals that same Target 3 Irreducible Melee Damage. 
            QuickHPCheck(-4, 0, 0, 0, 0, 0);

            //One Hero may Draw a Card.
            QuickHandCheck(0, 1, 0, 0);

        }

        [Test()]
        public void TestSetUpCard_Sentinels()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "TheSentinels", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PlayCard("BladeBattalion");
            Card setup = PutInTrash("SetUp");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCard = writhe;
            DecisionSelectTurnTakers = new TurnTaker[] { sentinels.TurnTaker, legacy.TurnTaker };

            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(headlong, legacy, bunker, scholar);

            PlayCard(setup);

            //Deal one Non-Hero Target 1 Melee Damage. 
            //If a Target takes Damage this way, another Hero deals that same Target 3 Irreducible Melee Damage. 
            QuickHPCheck(-4, 0, 0, 0, 0, 0);

            //One Hero may Draw a Card.
            QuickHandCheck(0, 1, 0, 0);

        }

        [Test()]
        public void TestSlipAndSlide()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PlayCard("BladeBattalion");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };

            //Deal one Target 2 Projectile Damage. If a Target takes Damage this way, that Target Deals a Non-Hero Target 3 Melee Damage.
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("SlipAndSlide");
            QuickHPCheck(-2, -3, 0, 0, 0, 0);
        }

        [Test()]
        public void TestSlipperySurface()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card police = PlayCard("PoliceBackup");

            PlayCard("SlipperySurface");

            //Reduce damage dealt to Hero Targets from Environment Cards by 1. 
            //Increase Damage dealt to Villain Targets by Environment Cards by 1.

            QuickHPStorage(baron, legacy, bunker, scholar, headlong);
            DealDamage(police, c => c.IsTarget, 3, DamageType.Projectile);
            QuickHPCheck(-4, -2, -2, -2, -2);
        }

        [Test()]
        public void TestSmoothMoves()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card toHand = PutOnDeck("BowlOver");
            Card toPlay = PutOnDeck("SlipperySurface");
            Card toTrash = PutOnDeck("FrictionTransfer");
            //Reveal the top 3 Cards of your Deck. Put one of those Cards into your hand, one into play, and one into your Trash.
            DecisionSelectCards = new Card[] { toHand, toPlay, toTrash };
            PlayCard("SmoothMoves");
            AssertInHand(toHand);
            AssertInPlayArea(headlong, toPlay);
            AssertInTrash(toTrash);

        }

        [Test()]
        public void TestSpinOut()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");

            //{Headlong} deals 1 Non-Hero Target 3 Melee Damage. Then he may deal a second Non-Hero Target 3 Projectile Damage.

            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion, headlong.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            PlayCard("SpinOut");
            QuickHPCheck(-3, -3, 0, 0, 0, 0);

        }

        [Test()]
        public void TestUpTempo()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card momentum = PutInTrash("BowlOver");

            PlayCard("UpTempo");

            //The first time {Headlong} plays a Momentum Card each turn, one Player may Draw a card.
            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker, scholar.TurnTaker };
            QuickHandStorage(headlong, legacy, bunker, scholar);
            PlayCard(momentum);
            QuickHandCheck(0, 0, 1, 0);

            QuickHandUpdate();
            PlayCard(momentum);
            QuickHandCheck(0, 0, 0, 0);

            GoToNextTurn();
            QuickHandUpdate();
            PlayCard(momentum);
            QuickHandCheck(0, 0, 0, 1);
        }

        [Test()]
        public void TestUpWheelinAndDealin()
        {
            SetupGameController("BaronBlade", "Chasejyd.Headlong", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("WheelinAndDealin");

            //Each time an Environment Card comes into Play, {Headlong} may Draw a Card.
            QuickHandStorage(headlong, legacy, bunker, scholar);
            Card police = PlayCard("PoliceBackup");
            QuickHandCheck(1, 0, 0, 0);

            DestroyCard(police);
            QuickHandUpdate();
            PlayCard(police);
            QuickHandCheck(1, 0, 0, 0);


            //When a Hero Target would be dealt 4 or more Damage, you may Destroy this Card. If you do so, Prevent the Damage, then {Headlong} and one other Hero may Draw a Card.
            DecisionYesNo = true;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandUpdate();
            QuickHPStorage(scholar);
            DealDamage(baron, scholar, 4, DamageType.Projectile);
            QuickHPCheckZero();
            QuickHandCheck(1, 0, 1, 0);

        }


    }
}
