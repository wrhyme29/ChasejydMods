using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class StandingOvationCardController : RockstarCardController
    {

        public StandingOvationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override void AddTriggers()
		{
			//When a Non-Hero Target is destroyed outside of {Rockstar}'s turn, {Rockstar} may draw a card."

			AddTrigger((DestroyCardAction destroyCard) => !destroyCard.CardToDestroy.Card.IsHero && destroyCard.CardToDestroy.Card.IsTarget && destroyCard.WasCardDestroyed && Game.ActiveTurnTaker != TurnTaker, DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);
		}

		private IEnumerator DrawCardResponse(DestroyCardAction destroyCard)
		{
			string message = $"{Card.Title} allows {TurnTaker.Name} to draw a card.";
			IEnumerator coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
			IEnumerator drawCardE = DrawCard(null, optional: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(drawCardE);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(drawCardE);
			}
		}


	}
}