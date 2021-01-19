using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.BlisterTeam
{
	public class BlisterTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public BlisterTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2).Condition = () => !CharacterCard.IsFlipped;
			SpecialStringMaker.ShowVillainTargetWithLowestHP().Condition = () => CharacterCard.IsFlipped;
			SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => CharacterCard.IsFlipped;
		}

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				//{Blister} is immune to Fire Damage.
				AddSideTrigger(AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard && dd.DamageType == DamageType.Fire));

				//At the End of her Turn, {Blister} deals the two Non-Villain Targets with the Highest HP 1 Fire Damage.
				AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, (Card c) => c.IsNonVillainTarget, TargetType.HighestHP, 1, DamageType.Fire, numberOfTargets: 2));
				if(TurnTaker.IsChallenge)
                {
					//Whenever {Blister} deals a Non-Villain Target Fire Damage, she also deals that Target 1 Toxic Damage.
					AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.DamageType == DamageType.Fire && dd.Target != null && dd.Target.IsNonVillainTarget, ChallengeDamageResponse, TriggerType.DealDamage, TriggerTiming.After));
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

        private IEnumerator ChallengeDamageResponse(DealDamageAction dd)
        {
			//she also deals that Target 1 Toxic Damage
			IEnumerator coroutine = DealDamage(CharacterCard, dd.Target, 1, DamageType.Toxic, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}

		private IEnumerator IncapacitatedResponse(PhaseChangeAction p)
		{
			//Destroy an Environment Card 
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//The villain target with the lowest HP deals the Hero Target with the Highest HP 3 Fire Damage.
			List<Card> storedResults = new List<Card>();
			coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsVillainTarget, storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card = storedResults.FirstOrDefault();
			if (card != null && card.IsTarget)
			{
				coroutine = DealDamageToHighestHP(card, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 3, DamageType.Fire);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}

	}
}