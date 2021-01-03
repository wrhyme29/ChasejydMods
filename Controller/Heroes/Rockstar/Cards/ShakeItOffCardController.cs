using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class ShakeItOffCardController : RockstarCardController
    {
		public Guid? PerformDestroyForDamage
		{
			get;
			set;
		}

		public ShakeItOffCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When {Rockstar} would take 3 or more damage you may destroy this card. If you do, prevent that damage and you may draw a card.
            AddTrigger((DealDamageAction dd) => dd.Target == CharacterCard && dd.Amount >= 3 && !base.IsBeingDestroyed, PreventDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isConditional: true, requireActionSuccess: true, isActionOptional: true);
        }

		private IEnumerator PreventDamageResponse(DealDamageAction dd)
		{
			//taken from Planquez Vous with slight modifications
			if (!PerformDestroyForDamage.HasValue || PerformDestroyForDamage.Value != dd.InstanceIdentifier)
			{
				List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
				IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, base.Card, dd, storedResults, null, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (DidPlayerAnswerYes(storedResults))
				{
					PerformDestroyForDamage = dd.InstanceIdentifier;
				}
			}
			if (PerformDestroyForDamage.HasValue && PerformDestroyForDamage.Value == dd.InstanceIdentifier)
			{
				IEnumerator coroutine2 = CancelAction(dd, showOutput: true, cancelFutureRelatedDecisions: true, null, isPreventEffect: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
				if (IsRealAction(dd))
				{
					coroutine2 = base.GameController.DestroyCard(DecisionMaker, base.Card, optional: false, null, null, null, null, null, null, null, null, GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}

					coroutine2 = DrawCard(hero: HeroTurnTaker, optional: true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}
				}
			}
			if (IsRealAction(dd))
			{
				PerformDestroyForDamage = null;
			}
		}


	}
}