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


        public override IEnumerator UsePower(int index = 0)
        {
			//{Rockstar} gains 2 HP and may draw a card.
			IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = DrawCard(optional: true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
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