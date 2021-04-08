using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
	public class DeeprootTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public DeeprootTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			
		}

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				
			}
			else
			{
				
			}
		}

	}
}