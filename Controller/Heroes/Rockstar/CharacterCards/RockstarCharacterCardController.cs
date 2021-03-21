using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
	public class RockstarCharacterCardController : RockstarUtilityCharacterCardController
	{
		public RockstarCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//{Rockstar} deals 1 target 1 melee damage.

			//Increase this Damage by 2 if she has a Stage Presence Card in Play.
			int targets = GetPowerNumeral(0, 1);
			int dmg = GetPowerNumeral(1, 1);
			int boost = GetPowerNumeral(2, 2);

			ITrigger previewBoost = null;
			bool isStagePresenceInPlay = GetNumberOfStagePresenceInPlay() > 0;
			if (isStagePresenceInPlay)
			{
				previewBoost = AddIncreaseDamageTrigger((DealDamageAction dd) => !IsRealAction() && dd.CardSource != null && dd.CardSource.Card == Card && dd.CardSource.PowerSource != null, dd => boost);
			}
			//select the targets
			var targetDecision = new SelectTargetsDecision(GameController,
											DecisionMaker,
											(Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
											targets,
											false,
											targets,
											false,
											new DamageSource(GameController, CharacterCard),
											dmg,
											DamageType.Melee,
											cardSource: GetCardSource());
			IEnumerator coroutine = GameController.SelectCardsAndDoAction(targetDecision, _ => DoNothing());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (isStagePresenceInPlay)
			{
				RemoveTrigger(previewBoost);
			}

			var selectedTargets = targetDecision.SelectCardDecisions.Select(scd => scd.SelectedCard).Where((Card c) => c != null);
			if (selectedTargets.Count() == 0)
			{
				yield break;
			}

			ITrigger boostTrigger = null;
			if (isStagePresenceInPlay)
			{	
				boostTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsCard && dd.DamageSource.Card == CharacterCard && dd.CardSource != null && dd.CardSource.Card == this.Card && dd.CardSource.PowerSource != null, dd => GameController.IncreaseDamage(dd, boost, false, GetCardSource()), null, TriggerPriority.Medium, false, GetCardSource());
				AddToTemporaryTriggerList(AddTrigger(boostTrigger));	
			}

			//actually deal the damage
			coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => selectedTargets.Contains(c), dmg, DamageType.Melee, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (targets > 1)
			{
				coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), dmg, DamageType.Melee, targets - 1, false, targets - 1, additionalCriteria: (Card c) => !selectedTargets.Contains(c), cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			if (isStagePresenceInPlay)
			{
				RemoveTemporaryTrigger(boostTrigger);
			}
			yield break;
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//Increase HP Recovery by Heroes by 1 until the start of your next turn.
						IncreaseGainHPStatusEffect effect = new IncreaseGainHPStatusEffect(1);
						effect.TargetCriteria.IsHero = true;
						effect.UntilStartOfNextTurn(TurnTaker);
						IEnumerator coroutine = AddStatusEffect(effect);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						//Select one Target. Reduce damage dealt by that Target by 1 until the start of your next turn.
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
						IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTarget, FindCardsWhere(c => c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						if(DidSelectCard(storedResults))
                        {
							Card target = GetSelectedCard(storedResults);
							ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(1);
							effect.SourceCriteria.IsSpecificCard = target;
							effect.UntilStartOfNextTurn(TurnTaker);
							coroutine = AddStatusEffect(effect);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}
						break;
					}
				case 2:
					{
						//Reduce damage to Hero Targets from the Environment by 1 until the start of your next turn.
						ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(1);
						effect.TargetCriteria.IsHero = true;
						effect.TargetCriteria.IsTarget = true;
						effect.SourceCriteria.IsEnvironment = true;
						effect.UntilStartOfNextTurn(TurnTaker);
						IEnumerator coroutine = AddStatusEffect(effect);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
			}
			yield break;
		}
	}
}
