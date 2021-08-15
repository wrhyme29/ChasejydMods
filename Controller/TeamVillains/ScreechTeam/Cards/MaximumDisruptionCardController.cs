using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class MaximumDisruptionCardController : ScreechTeamCardController
    {

        public MaximumDisruptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override  IEnumerator Play()
        {

            //Destroy 1 Environment Card, 1 Villain Ongoing, and 3 Hero Ongoings or Equipment Cards.
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment, "environment"), optional: false, cardSource: GetCardSource());
            IEnumerator coroutine2 = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && IsVillain(c), "villain ongoing"), optional: false, cardSource: GetCardSource());
            IEnumerator coroutine3 = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsHero && (c.IsOngoing || IsEquipment(c)), "hero ongoing or equipment"), 3, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
                yield return base.GameController.StartCoroutine(coroutine3);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
                base.GameController.ExhaustCoroutine(coroutine3);

            }

            //{Screech} deals each Hero Target 1 Projectile Damage.
            coroutine = DealDamage(CharacterCard, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, DamageType.Projectile);
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