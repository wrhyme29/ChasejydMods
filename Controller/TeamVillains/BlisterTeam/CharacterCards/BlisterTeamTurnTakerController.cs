using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
	public class BlisterTeamTurnTakerController : TurnTakerController
	{
		public BlisterTeamTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
		{
		}

        public override IEnumerator StartGame()
        {
			//Blazing Axe and Firestarter are put into play, and the villain deck is shuffled.
			IEnumerator coroutine = PutCardIntoPlay("BlazingAxe");
			IEnumerator coroutine2 = PutCardIntoPlay("Firestarter");
			IEnumerator coroutine3 = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: CharacterCardController.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
				base.GameController.ExhaustCoroutine(coroutine3);
			}
			yield break;
        }

    }
}