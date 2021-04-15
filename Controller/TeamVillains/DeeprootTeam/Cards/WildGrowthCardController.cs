using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class WildGrowthCardController : DeeprootTeamCardController
    {

        public WildGrowthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Deeproot} gains 1 HP.
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle {Deeproot}'s trash into his deck.
            coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			//Reveal the top card of his deck. If it is a Plant Growth, put it into play. If it is not a Plant Growth, discard it and {Deeproot} gains 2 more HP
			coroutine = RevealTopCard_TakeActionBasedOnPlantGrowth();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator RevealTopCard_TakeActionBasedOnPlantGrowth()
		{
			List<Card> cards = new List<Card>();
			IEnumerator coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, 1, cards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			Card revealedCard = GetRevealedCard(cards);
			if (revealedCard != null)
			{
				
				if(IsPlantGrowth(revealedCard))
                {
					//If it is a Plant Growth, put it into play.
					coroutine = GameController.PlayCard(TurnTakerController, revealedCard, isPutIntoPlay: true, cardSource: GetCardSource());
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine);
					}
				}
				else
                {
					//If it is not a Plant Growth, discard it and { Deeproot} gains 2 more HP
					coroutine = GameController.MoveCard(TurnTakerController, revealedCard, TurnTaker.Trash, cardSource: GetCardSource());
					IEnumerator coroutine2 = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine);
						yield return GameController.StartCoroutine(coroutine2);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine);
						GameController.ExhaustCoroutine(coroutine2);
					}
				}	
			}
			List<Location> list = new List<Location>();
			list.Add(TurnTaker.Revealed);
			coroutine = CleanupCardsAtLocations(list, TurnTaker.Deck, cardsInList: cards);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}
	}
}