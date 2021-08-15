using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.SnareTeam
{
	public class SnareTeamCharacterCardController : VillainTeamCharacterCardController
	{
		public SnareTeamCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
			SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(ranking: 2).Condition = () => !Card.IsFlipped;
			SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2).Condition = () => !Card.IsFlipped;
		}

		public static readonly string BarrierKeyword = "barrier";
        public static readonly string GigerMobilityChairIdentifier = "GigerMobilityChair";


		public override bool AskIfCardIsIndestructible(Card card)
		{
			//Giger Mobility Chair is indestructible.
			return TurnTaker.IsChallenge && card.Identifier == GigerMobilityChairIdentifier;
		}

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				//At the end of {Snare}'s turn, {Snare} deals the hero character with the second lowest HP 2 energy Damage. Reduce the next damage dealt by that target by 2.
				AddEndOfTurnTrigger(tt => tt == TurnTaker, DealDamageAtEndOfTurnResponse, TriggerType.DealDamage);

				//When a barrier card is destroyed, {Snare} deals the hero target with the second highest HP 3 energy damage, then deals herself 2 irreducible psychic Damage.
				AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && IsBarrier(dca.CardToDestroy.Card), BarrierDestroyedResponse, TriggerType.DealDamage, TriggerTiming.After);

				if (TurnTaker.IsAdvanced)
                {
					//When {Snare} deals a target energy damage, she also deals them 1 irreducible sonic damage.
					AddTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(CharacterCard) && dd.DidDealDamage && !dd.DidDestroyTarget && dd.DamageType == DamageType.Energy, dd => DealDamage(CharacterCard, dd.Target, 1, DamageType.Sonic, isIrreducible: true, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
				}
			}
			else
			{
				//Reduce damage to villain character cards by 1.
				AddReduceDamageTrigger(c => c.IsVillainTeamCharacter && !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);
			}
		}

        private IEnumerator BarrierDestroyedResponse(DestroyCardAction arg)
        {
			//{Snare} deals the hero target with the second highest HP 3 energy damage, then deals herself 2 irreducible psychic Damage.
			IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 2, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Energy);
			IEnumerator coroutine2 = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Psychic, isIrreducible: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);

			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}

		private IEnumerator DealDamageAtEndOfTurnResponse(PhaseChangeAction pca)
        {
			//{Snare} deals the hero character with the second lowest HP 2 energy Damage. Reduce the next damage dealt by that target by 2.
			IEnumerator coroutine = DealDamageToLowestHP(CharacterCard, 2, c => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 2, DamageType.Energy, addStatusEffect: dd => ReduceNextDamageDealtByThatTargetResponse(dd, 2), evenIfCannotDealDamage: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
        }

		private IEnumerator ReduceNextDamageDealtByThatTargetResponse(DealDamageAction action, int amount)
		{
			ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
			reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = action.Target;
			reduceDamageStatusEffect.NumberOfUses = 1;
			reduceDamageStatusEffect.UntilTargetLeavesPlay(action.Target);
			return AddStatusEffect(reduceDamageStatusEffect);
		}

		protected bool IsBarrier(Card card)
		{
			return card.DoKeywordsContain(BarrierKeyword);
		}
	}
}