using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class FrontWomanCardController : StagePresenceCardController
    {

        public FrontWomanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //  Reduce damage dealt  to {Rockstar} by Villain Character Cards by 1.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsVillainCharacterCard, 1, null, targetCriteria: (Card c) => c == CharacterCard);
            //  Any time a non-Hero card would deal damage to the Hero target with the highest HP, you may redirect that damage to {Rockstar}
            Func<DealDamageAction, bool> criteria = (DealDamageAction dealDamage) => (!dealDamage.DamageSource.Card.IsHero && GameController.IsCardVisibleToCardSource(dealDamage.DamageSource.Card, GetCardSource())) && (dealDamage.Target.IsHero && GameController.IsCardVisibleToCardSource(dealDamage.Target, GetCardSource())) && CanCardBeConsideredHighestHitPoints(dealDamage.Target, (Card c) => c.IsHero && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
            AddTrigger(criteria, RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dd)
        {
            List<bool> shouldRedirect = new List<bool>();
            IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.Target, highest: true, (Card card) => card.IsHero && GameController.IsCardVisibleToCardSource(card, GetCardSource()), dd, shouldRedirect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (!shouldRedirect.First())
            {
                yield break;
            }
           
            coroutine = GameController.RedirectDamage(dd, CharacterCard, isOptional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }
    }
}