using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.ScreechTeam
{
	public class ScreechTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public ScreechTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !Card.IsFlipped;
			SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => Card.IsFlipped;
			SpecialStringMaker.ShowHeroCharacterCardWithLowestHP().Condition = () => Card.IsFlipped;
		}

		public static readonly string CannotDrawCardsStatusEffectKey = "CannotDrawCardsStatusEffect";

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{

				//At the end of {Screech}'s turn, he deals the Hero Target with the highest HP 3 Sonic Damage. Then one player must discard a card.
				AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DiscardCard });

				//{Screech} is Immune to Sonic Damage.
				AddImmuneToDamageTrigger(dd => dd.Target != null && dd.Target == CharacterCard && dd.DamageType == DamageType.Sonic);
				if (TurnTaker.IsAdvanced)
                {
					//When {Screech} deals a Hero Character 3 or more Sonic damage, that player must discard a card.
					AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.DamageType == DamageType.Sonic && dd.Target.IsHeroCharacterCard && dd.Amount >= 3 && dd.DidDealDamage && !dd.Target.IsIncapacitatedOrOutOfGame, AdvancedResponse, TriggerType.DiscardCard, TriggerTiming.After);
				}

				if (TurnTaker.IsChallenge)
                {
					//Increase Sonic Damage by 1.
					AddIncreaseDamageTrigger(dd => dd.DamageType == DamageType.Sonic, 1);
				}
			}
			else
			{
				//At the start of each of {Screech}'s turns, the Hero Characters with the highest and lowest HP must discard 1 card.
				AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnIncapResponse, TriggerType.DiscardCard);
			}

			//add triggers for fake status effect of "ShakeTheirNerves"
			CannotDrawCards(CannotDrawCardsCriteria);
			AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnClearStatusEffectsResponse, TriggerType.Hidden, additionalCriteria: pca => Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey) != null);
		}

        private IEnumerator StartOfTurnIncapResponse(PhaseChangeAction arg)
        {
			//the Hero Characters with the highest and lowest HP must discard 1 card.

			List<Card> highestHeroes = new List<Card>();
			IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, c => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), highestHeroes, evenIfCannotDealDamage: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<Card> lowestHeroes = new List<Card>();
			coroutine = GameController.FindTargetsWithLowestHitPoints(1, 1, c => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), lowestHeroes, evenIfCannotDealDamage: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<Card> heroesToDiscard = highestHeroes.Concat(lowestHeroes).ToList();

			HeroTurnTakerController hero;
			foreach(Card heroCC in heroesToDiscard)
            {
				hero = FindHeroTurnTakerController(heroCC.Owner.ToHero());
				coroutine = GameController.SelectAndDiscardCard(hero, cardSource: GetCardSource());
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

		private IEnumerator AdvancedResponse(DealDamageAction dd)
        {
			//That player must discard a card.
			HeroTurnTakerController hero = FindHeroTurnTakerController(dd.Target.Owner.ToHero());
			return GameController.SelectAndDiscardCard(hero, cardSource: GetCardSource());
			
		}

		private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
			//Screech deals the Hero Target with the highest HP 3 Sonic Damage. 
			IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Sonic);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Then one player must discard a card.
			coroutine = GameController.SelectHeroToDiscardCard(DecisionMaker, optionalDiscardCard: false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator StartOfTurnClearStatusEffectsResponse(PhaseChangeAction pca)
		{

			List<string> cannotDrawCardsIdentifiers = Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey).ToList();
			IEnumerator coroutine;
			foreach (TurnTaker tt in cannotDrawCardsIdentifiers.Select(id => FindTurnTakersWhere(turnTaker => turnTaker.Identifier == id).First()))
			{
				coroutine = GameController.SendMessageAction($"Notice: an ongoing effect just expired: {tt.Name} cannot draw cards.", Priority.High, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			Game.Journal.RecordCardProperties(CharacterCard, CannotDrawCardsStatusEffectKey, new List<string>());

			yield break;
		}

		private bool CannotDrawCardsCriteria(TurnTakerController ttc)
		{
			return Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey) != null && Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey).Contains(ttc.TurnTaker.Identifier);
		}
	}
}