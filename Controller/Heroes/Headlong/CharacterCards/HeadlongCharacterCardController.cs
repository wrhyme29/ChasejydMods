using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Headlong
{
	public class HeadlongCharacterCardController : HeadlongUtilityCharacterCardController
	{
		public HeadlongCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, cardCriteria: new LinqCardCriteria(c => IsMomentum(c), "momentum"));
		}
		public override IEnumerator UsePower(int index = 0)
		{
            //Reveal the top Card of {Headlong}'s deck.

            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, 1, storedResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			if(storedResults is null || !storedResults.Any())
            {
				yield break;
            }

			Card revealedCard = storedResults.First();
			if(IsMomentum(revealedCard))
            {
				//If it is a Momentum Card, put it into play.
				coroutine = GameController.PlayCard(TurnTakerController, revealedCard, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			} else
            {
				//If it is not, place it in your hand and Draw a Card.
				coroutine = GameController.MoveCard(TurnTakerController, revealedCard, HeroTurnTaker.Hand, showMessage: true, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}

				coroutine = DrawCard();
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}

			yield break;
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//Each Player may Discard a Card. Any Player who does so may Draw a Card.
						IEnumerator coroutine = DoActionToEachTurnTakerInTurnOrder(tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, DiscardToDraw);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						//Until the start of Headlong's next turn, Hero Targets are Immune to Damage from the Environment.
						ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
						effect.SourceCriteria.IsEnvironment = true;
						effect.TargetCriteria.IsHero = true;
						effect.UntilStartOfNextTurn(TurnTaker);
						IEnumerator coroutine = AddStatusEffect(effect);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 2:
					{
						//You may look at the top card of a deck, then replace it or Discard it.
						List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
						IEnumerator coroutine = base.GameController.SelectADeck(DecisionMaker, SelectionType.MoveCard, (Location l) => true, storedResults, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						if (DidSelectDeck(storedResults))
						{
							Location selectedLocation = GetSelectedLocation(storedResults);
							coroutine = RevealCard_DiscardItOrPutItOnDeck(DecisionMaker, TurnTakerController, selectedLocation, false);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}
						break;
					}
			}
			yield break;
		}

        private IEnumerator DiscardToDraw(TurnTakerController ttc)
        {
			List<DiscardCardAction> storedResults = new List<DiscardCardAction>() ;
            IEnumerator coroutine = GameController.SelectAndDiscardCard(ttc.ToHero(), optional: true, storedResults: storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(DidDiscardCards(storedResults))
            {
				coroutine = DrawCard(ttc.ToHero().HeroTurnTaker);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}
    }
}
