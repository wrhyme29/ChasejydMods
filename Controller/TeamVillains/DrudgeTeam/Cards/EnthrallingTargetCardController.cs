using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DrudgeTeam
{
    public class EnthrallingTargetCardController : DrudgeTeamCardController
    {

        public EnthrallingTargetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

		public bool? PerformImmunity
		{
			get;
			set;
		}

		public override bool AllowFastCoroutinesDuringPretend
		{
			get
			{
				if (!GameController.PreviewMode)
				{
					return IsLowestHitPointsUnique((Card card) => card.IsHero && card.IsTarget);
				}
				return true;
			}
		}

		public override void AddTriggers()
        {
            //Reduce Non-Radiant Damage dealt to {DrudgeTeam} by 1
            AddReduceDamageTrigger(dd => dd.Target != null && dd.Target == CharacterCard && dd.DamageType != DamageType.Radiant, dd => 1);

			//{DrudgeTeam} is Immune to damage from the Hero Target with the lowest HP
			AddTrigger((DealDamageAction dd) => dd.Target != null && dd.Target == CharacterCard, ImmuneToDamageIfLowest, TriggerType.ImmuneToDamage, TriggerTiming.Before);
		}

		private IEnumerator ImmuneToDamageIfLowest(DealDamageAction dd)
		{
			if (base.GameController.PretendMode)
			{
				List<bool> storedResults = new List<bool>();
				IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.DamageSource.Card, highest: false, (Card c) => c.IsHero && c.IsTarget, dd, storedResults);
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
					PerformImmunity = storedResults.First();
				}
				else
				{
					PerformImmunity = null;
				}
			}
			if (PerformImmunity.HasValue && PerformImmunity.Value)
			{
				IEnumerator coroutine2 = GameController.ImmuneToDamage(dd, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (!GameController.PretendMode)
			{
				PerformImmunity = null;
			}
		}

	}
}