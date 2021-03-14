using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class SmoothMovesCardController : HeadlongCardController
    {

        public SmoothMovesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
			//Reveal the top 3 Cards of your Deck. Put one of those Cards into your hand, one into play, and one into your Trash.

			List<MoveCardDestination> destinations = new List<MoveCardDestination>();
			destinations.Add(new MoveCardDestination(base.HeroTurnTaker.Hand));
			destinations.Add(new MoveCardDestination(base.TurnTaker.PlayArea));
			destinations.Add(new MoveCardDestination(base.TurnTaker.Trash));

			IEnumerator coroutine = RevealCardsFromDeckToMoveToOrderedDestinations(DecisionMaker, base.TurnTaker.Deck, destinations, isPutIntoPlay: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
        }

    }
}