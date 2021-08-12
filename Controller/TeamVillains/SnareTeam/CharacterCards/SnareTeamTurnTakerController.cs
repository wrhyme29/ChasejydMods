using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
	public class SnareTeamTurnTakerController : TurnTakerController
	{
		public SnareTeamTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
		{

		}

		public override IEnumerator StartGame()
		{
			//Put Crimson Shield and Giger Mobility Chair into play, then shuffle the villain deck.
			IEnumerator coroutine = PutCardIntoPlay("CrimsonShield");
			IEnumerator coroutine2 = PutCardIntoPlay("GigerMobilityChair");
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