using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class CantStopTheBeatdownCardController : DeeprootTeamCardController
    {

        public CantStopTheBeatdownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }


		public override IEnumerator Play()
		{
			//{Deeproot} Deals the Hero Target with the second highest HP 4 Melee Damage. 
			//Redirect the next damage that Target would deal to {Deeproot} and reduce it by 2.
			IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 2, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && !c.IsIncapacitated && GameController.IsCardVisibleToCardSource(c, GetCardSource()), (Card c) => 4, DamageType.Melee, addStatusEffect: ApplyStatusEffects, selectTargetEvenIfCannotDealDamage: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator RedirectAndReduceResponse(DealDamageAction dd, TurnTaker tt, StatusEffect effect, int[] powerNumerals = null)
		{
			IEnumerator coroutine;
			if (dd.Target != tt.CharacterCard)
			{
				coroutine = GameController.RedirectDamage(dd, tt.CharacterCard, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			OnDealDamageStatusEffect onDealDamageStatusEffect = (OnDealDamageStatusEffect)effect;
			coroutine = GameController.ReduceDamage(dd, onDealDamageStatusEffect.EffectValue, null, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator ApplyStatusEffects(DealDamageAction dd)
		{
			OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(RedirectAndReduceResponse), "Redirect the next damage dealt by " + dd.Target.Title + " to " + CharacterCard.Title + ". Reduce that damage by {EFFECT_VALUE}.", new TriggerType[2]
				{
				TriggerType.RedirectDamage,
				TriggerType.ReduceDamage
				}, TurnTaker, Card);
			List<Card> list = new List<Card>();
			list.Add(CharacterCard);
			list.Add(dd.Target);
			onDealDamageStatusEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards = list;
			onDealDamageStatusEffect.NumberOfUses = 1;
			onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
			onDealDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
			onDealDamageStatusEffect.EffectValue = 2;
			IEnumerator coroutine = AddStatusEffect(onDealDamageStatusEffect);
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