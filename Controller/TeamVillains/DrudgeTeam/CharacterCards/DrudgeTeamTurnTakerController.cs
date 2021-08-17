using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
	public class DrudgeTeamTurnTakerController : TurnTakerController
	{
		public DrudgeTeamTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
		{
		}

        public override IEnumerator StartGame()
        {
			//Put Immortal Form and Consume Lifeforce into play, then shuffle the villain deck.
			IEnumerator coroutine = PutCardIntoPlay("ImmortalForm");
			IEnumerator coroutine2 = PutCardIntoPlay("ConsumeLifeforce");
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