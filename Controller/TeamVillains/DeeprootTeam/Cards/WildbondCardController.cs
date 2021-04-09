using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
    public class WildbondCardController : DeeprootTeamCardController
    {

        public WildbondCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce Damage dealt to Environment Targets by 2.
            AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 2);

			//Redirect Damage that would be Dealt to {Deeproot} by Environment Cards to the Hero Target with the highest HP.
			AddTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard && dd.Target == CharacterCard, RedirectDamageToHighestHitpointsResponse, TriggerType.RedirectDamage, TriggerTiming.Before);

			//When this Card is Destroyed, {Deeproot} gains 2 HP."
			AddWhenDestroyedTrigger(dca => GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource()), TriggerType.GainHP);
		}

		private IEnumerator RedirectDamageToHighestHitpointsResponse(DealDamageAction dealDamage)
		{
			
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card card) => card.IsHero && card.IsTarget && GameController.IsCardVisibleToCardSource(card, GetCardSource()), storedResults, gameAction: dealDamage, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResults.Count() > 0)
			{
				Card newTarget = storedResults.FirstOrDefault();
				coroutine = base.GameController.RedirectDamage(dealDamage, newTarget, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
		}


	}
}