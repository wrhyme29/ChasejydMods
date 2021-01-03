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
			//{Rockstar} deals 1 target 2 melee damage.
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Until the start of your next turn, increase HP recovery by {Rockstar} by 1.
			IncreaseGainHPStatusEffect effect = new IncreaseGainHPStatusEffect(1);
			effect.TargetCriteria.IsSpecificCard = CharacterCard;
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
