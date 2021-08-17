using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class InfernalRitesCardController : DrudgeTeamCardController
    {

        public InfernalRitesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsVampiric(c), "vampiric"));
        }

        private int X => FindCardsWhere(c => IsVampiric(c) && c.IsInPlayAndHasGameText).Count();


        public override void AddTriggers()
        {
            //At the start of {DrudgeTeam}'s turn, he deals the X Non-Villain Targets with the highest HP 1 Irreducible Infernal and 1 Irreducible Psychic Damage each, where X is equal to the number of Vampiric cards in play. If Drudge destroys a Target with this damage he gain 2 HP and then play the top Card of his Deck.",
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DealDamageAtStartOfTurnResponse, TriggerType.DealDamage);
            //If {DrudgeTeam} takes 3 or more radiant damage, destroy this card.
            AddTrigger((DealDamageAction dd) => dd.Target != null && dd.Target == CharacterCard && dd.DamageType == DamageType.Radiant && dd.Amount >= 3, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DealDamageAtStartOfTurnResponse(PhaseChangeAction arg)
        {
            //he deals the X Non-Villain Targets with the highest HP 1 Irreducible Infernal and 1 Irreducible Psychic Damage each, where X is equal to the number of Vampiric cards in play. 
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Infernal, isIrreducible: true));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Psychic, isIrreducible: true));
			List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(list, c => !IsVillainTarget(c), HighestLowestHP.HighestHP, numberOfTargets: X, storedDamage: storedDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			if(storedDamage.Any(dd => dd.DidDestroyTarget))
            {
				//If Drudge destroys a Target with this damage he gain 2 HP
				coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				// and then play the top Card of his Deck.
				coroutine = GameController.SendMessageAction($"{Card.Title} plays the top card of {TurnTaker.Deck.GetFriendlyName()}...", Priority.High, GetCardSource(), showCardSource: true);
				IEnumerator e2 = GameController.PlayTopCardOfLocation(DecisionMaker, TurnTaker.Deck, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
					yield return GameController.StartCoroutine(e2);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
					GameController.ExhaustCoroutine(e2);
				}
			}
		}

		protected IEnumerator DealMultipleInstancesOfDamageToHighestLowestHP(List<DealDamageAction> damageInfo, Func<Card, bool> cardCriteria, HighestLowestHP highestLowest, int ranking = 1, int numberOfTargets = 1, List<DealDamageAction> storedDamage = null)
		{
			List<Card> storedResults = new List<Card>();
			switch (highestLowest)
			{
				case HighestLowestHP.HighestHP:
					{
						IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(ranking, numberOfTargets, cardCriteria, storedResults, null, damageInfo, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case HighestLowestHP.LowestHP:
					{
						IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(ranking, numberOfTargets, cardCriteria, storedResults, null, damageInfo, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
			}
			if (storedResults.Count() > 0)
			{
				IEnumerator coroutine = DealMultipleInstancesOfDamage(damageInfo, (Card c) => storedResults.Contains(c), numberOfTargets: numberOfTargets, storedResults: storedDamage);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}
		}
	}
}