﻿using Handelabra;
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

    }
}
