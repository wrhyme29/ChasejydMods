using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DrudgeTeam
{
	public class DrudgeTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public DrudgeTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroTargetWithLowestHP();
			SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2).Condition = () => !Card.IsFlipped && TurnTaker.IsChallenge;
			SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => Card.IsFlipped;
		}

		public static readonly string FirstTimeDrudgeDealtDamageKey = "FirstTimeDrudgeDealtDamageKey";

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				//At the end of {Drudge}'s turn, {Drudge} deals the hero target with the lowest HP 2 infernal Damage.
				AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 2, DamageType.Infernal);

				//Increase radiant damage dealt to {Drudge} by 1.
				AddIncreaseDamageTrigger(dd => dd.Target != null && dd.Target == CharacterCard && dd.DamageType == DamageType.Radiant, 1);

				//Increase damage dealt by other vampire targets to {Drudge} by 1.
				AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsVampire && dd.DamageSource.Card != CharacterCard && dd.Target != null && dd.Target == CharacterCard, 1);

				//Increase damage dealt by {Drudge} to other vampire targets by 1.
				AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.Target != null && dd.Target.IsVampire && dd.Target != CharacterCard, 1);

				if (TurnTaker.IsAdvanced)
                {
					//Whenever a Villain Target is destroyed, {Drudge} gains 3 HP.
					AddTrigger((DestroyCardAction dca) => dca.CardToDestroy != null && IsVillainTarget(dca.CardToDestroy.Card) && GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()), dca => GameController.GainHP(CharacterCard, 3, cardSource: GetCardSource()), TriggerType.GainHP, TriggerTiming.After);
				}

				if (TurnTaker.IsChallenge)
                {
					//The first time per turn that {Drudge} would take 2 or less damage, redirect that damage to the Hero Target with the second highest HP.
					AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => dd.Target == CharacterCard, FirstTimeDrudgeDealtDamageKey, TargetType.HighestHP, (Card c) => c.IsHero, ranking: 2);
				}
			}
			else
			{
				//At the start of each of {Drudge}'s turns, the Hero Target with the lowest HP deals the Hero Target with the highest HP 1 Psychic and 1 Infernal Damage.
				AddStartOfTurnTrigger(tt => tt == TurnTaker, IncapStartOfTurnResponse, TriggerType.DealDamage);
			}
		}

		private IEnumerator IncapStartOfTurnResponse(PhaseChangeAction pca)
        {
			//the Hero Target with the lowest HP deals the Hero Target with the highest HP 1 Psychic and 1 Infernal Damage
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero, storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResults.FirstOrDefault() != null)
			{
				Card source = storedResults.FirstOrDefault();
				List<DealDamageAction> list = new List<DealDamageAction>();
				list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, source), null, 1, DamageType.Psychic));
				list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, source), null, 1, DamageType.Infernal));
				coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(list, c => c.IsHero, HighestLowestHP.HighestHP);
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