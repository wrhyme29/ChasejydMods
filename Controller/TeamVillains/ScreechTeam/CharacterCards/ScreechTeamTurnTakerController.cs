﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
	public class ScreechTeamTurnTakerController : TurnTakerController
	{
		public ScreechTeamTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
		{
		}

        public override IEnumerator StartGame()
        {
			//Put Total Chaos into play and the villain deck is shuffled.
			IEnumerator coroutine = PutCardIntoPlay("TotalChaos");
			IEnumerator coroutine2 = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: CharacterCardController.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);
			} 
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
        }

    }
}