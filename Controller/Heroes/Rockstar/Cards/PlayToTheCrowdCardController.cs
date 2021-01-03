using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class PlayToTheCrowdCardController : StagePresenceCardController
    {

        public PlayToTheCrowdCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce Melee and Projectile Damage dealt to {Rockstar} by 1.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard && (dd.DamageType == DamageType.Melee || dd.DamageType == DamageType.Projectile), (DealDamageAction dd) => 1);

            //Whenever a Non-Hero card would deal damage to the Hero Target with the lowest HP, or any Target with 3 or fewer HP, you may redirect that damage to {Rockstar}.
            Func<DealDamageAction, bool> criteria = (DealDamageAction dealDamage) => (!dealDamage.DamageSource.Card.IsHero && GameController.IsCardVisibleToCardSource(dealDamage.DamageSource.Card, GetCardSource())) && (dealDamage.Target.IsHero && dealDamage.Target != CharacterCard && GameController.IsCardVisibleToCardSource(dealDamage.Target, GetCardSource())) && (CanCardBeConsideredLowestHitPoints(dealDamage.Target, (Card c) => c.IsHero && GameController.IsCardVisibleToCardSource(c, GetCardSource())) || dealDamage.Target.HitPoints <= 3);
            AddTrigger(criteria, RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dd)
        {
            List<bool> shouldRedirect = new List<bool>();
            IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.Target, highest: false, (Card card) => card.IsHero && GameController.IsCardVisibleToCardSource(card, GetCardSource()), dd, shouldRedirect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (dd.Target.HitPoints > 3 && !shouldRedirect.First())
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