using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class SoWhatCardController : RockstarCardController
    {

        public SoWhatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override void AddTriggers()
		{
			//Whenever exactly 1 Damage would be dealt to {Rockstar}, prevent that damage.
			AddPreventDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount == 1, isPreventEffect: true);
		}

		public override bool CanOrderAffectOutcome(GameAction action)
		{
			if (action is DealDamageAction)
			{
				return (action as DealDamageAction).Target == base.CharacterCard;
			}
			return false;
		}


	}
}