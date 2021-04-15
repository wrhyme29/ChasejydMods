using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
	public class DeeprootTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public DeeprootTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowLowestHP(numberOfTargets: () => 2, cardCriteria: new LinqCardCriteria(c => IsVillainTarget(c) || c.IsEnvironmentTarget, "", useCardsSuffix: false, singular: "villain or environment target", plural: "villain or environment targets")).Condition = () => !Card.IsFlipped;
			SpecialStringMaker.ShowHighestHP(numberOfTargets: () => GetNumberOfPlantGrowthCardsInPlay(), cardCriteria: new LinqCardCriteria(c => c.IsHero && c.IsTarget, "", useCardsSuffix: false, singular: "hero target", plural: "hero targets")).Condition = () => !Card.IsFlipped && GetNumberOfPlantGrowthCardsInPlay() > 0;
			SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeVillainOngoingOrPlantGrowthEntersPlayKey, trueFormat: "A villain ongoing or plant growth card has entered play this turn.", falseFormat: "A villain ongoing or plant growth card has not entered play this turn.").Condition = () => TurnTaker.IsAdvanced && !Card.IsFlipped;
		}

		private readonly string FirstTimeVillainOngoingOrPlantGrowthEntersPlayKey = "FirstTimeVillainOngoingOrPlantGrowthEntersPlay";

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				//At the end of {Deeproot}'s turn, the 2 Villain or Environment Targets with the lowest HP gain 1 HP.
				//Then {Deeproot} deals the X Hero Targets with the highest HP 2 Toxic Damage each where X is equal to the number of Plant Growth Cards in play

				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnFrontResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.DealDamage }));

				if (TurnTaker.IsAdvanced)
                {
					//The first time each turn a Villain Ongoing or Plant Growth enters play, {Deeproot} gains 2 HP.
					AddSideTrigger(AddTrigger((CardEntersPlayAction cep) => !HasBeenSetToTrueThisTurn(FirstTimeVillainOngoingOrPlantGrowthEntersPlayKey) && ((IsVillain(cep.CardEnteringPlay) && cep.CardEnteringPlay.IsOngoing) || IsPlantGrowth(cep.CardEnteringPlay)) && GameController.IsCardVisibleToCardSource(cep.CardEnteringPlay, GetCardSource()), AdvancedResponse, TriggerType.GainHP, TriggerTiming.After));
				}

				if(TurnTaker.IsChallenge)
                {
					//Villain Character Cards are Immune to Damage from environment cards.
					AddSideTrigger(AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard && dd.Target.IsVillainCharacterCard));
				}
			}
			else
			{
				//Reduce Damage dealt to Environment Targets by 1.
				AddSideTrigger(AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1));

				// At the start of Deeproot's turn, play the top card of the Environment deck.
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard));
			}
		}

        private IEnumerator AdvancedResponse(CardEntersPlayAction cep)
        {
			//{Deeproot} gains 2 HP.
			SetCardPropertyToTrueIfRealAction(FirstTimeVillainOngoingOrPlantGrowthEntersPlayKey);
			IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

        private IEnumerator EndOfTurnFrontResponse(PhaseChangeAction pca)
        {
			//the 2 Villain or Environment Targets with the lowest HP gain 1 HP.
			IEnumerable<Card> foundTargets = base.GameController.FindAllTargetsWithLowestHitPoints(1, (Card c) => (IsVillainTarget(c) || c.IsEnvironmentTarget) && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, cardSource: base.GetCardSource(), numberOfTargets: 2);
			IEnumerator coroutine = GameController.GainHP(DecisionMaker, (Card c) => foundTargets.Contains(c), 1, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//Then {Deeproot} deals the X Hero Targets with the highest HP 2 Toxic Damage each where X is equal to the number of Plant Growth Cards in play
			coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), (Card c) => 2, DamageType.Toxic, numberOfTargets: () => GetNumberOfPlantGrowthCardsInPlay());
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

		public static readonly string PlantGrowthKeyword = "plant growth";

		protected bool IsPlantGrowth(Card card)
		{
			return card.DoKeywordsContain(PlantGrowthKeyword);
		}

		protected int GetNumberOfPlantGrowthCardsInPlay()
		{
			return FindCardsWhere(c => IsPlantGrowth(c) && c.IsInPlayAndHasGameText).Count();
		}
	}
}