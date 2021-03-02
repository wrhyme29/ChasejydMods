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

            DecisionSelectLocation =  new LocationChoice(legacy.TurnTaker.Deck);
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
            QuickHPCheck(-3,0, 0, 0, 0, 0);

        }

    }
}
