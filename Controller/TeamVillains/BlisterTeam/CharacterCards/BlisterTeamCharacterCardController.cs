using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
	public class BlisterTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public BlisterTeamCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				//{Blister} is immune to Fire Damage.
				//At the End of her Turn, {Blister} deals the two Non-Villain Targets with the Highest HP 1 Fire Damage.
				if (base.TurnTaker.IsAdvanced)
				{
					//Blazing Axe is Indestructible.
				}

				if(TurnTaker.IsChallenge)
                {
					//When Blister would deal a Non-Villain Target Fire Damage, she also deals that Target 1 Toxic Damage.

				}
			}
			else
			{
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, IncapacitatedResponse, new TriggerType[]
					{
						TriggerType.DestroyCard,
						TriggerType.DealDamage
					}));
			}
		}

		private IEnumerator IncapacitatedResponse(PhaseChangeAction p)
		{
			//Destroy an Environment Card and deal the Hero Target with the Highest HP 3 Fire Damage.
			yield break;
		}

	}
}