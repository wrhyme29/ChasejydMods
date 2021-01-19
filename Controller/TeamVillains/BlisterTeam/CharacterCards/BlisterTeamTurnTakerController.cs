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
            yield break;
        }

    }
}