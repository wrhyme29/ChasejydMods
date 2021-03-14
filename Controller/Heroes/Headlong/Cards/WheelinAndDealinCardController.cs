using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class WheelinAndDealinCardController : HeadlongCardController
    {

        public WheelinAndDealinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //Each time an Environment Card comes into Play, {Headlong} may Draw a Card.
            AddTrigger((PlayCardAction pca) => pca.CardToPlay.IsEnvironment && pca.IsSuccessful, pca => DrawCard(optional: true), TriggerType.DrawCard, TriggerTiming.After);

            //When a Hero Target would be dealt 4 or more Damage, you may Destroy this Card. If you do so, Prevent the Damage, then {Headlong} and one other Hero may Draw a Card.
            AddTrigger((DealDamageAction dd) => dd.Target.IsHero && dd.Amount >= 4, DestroyAndDrawResponse, new TriggerType[] { TriggerType.DestroySelf, TriggerType.CancelAction, TriggerType.DrawCard }, TriggerTiming.Before);
        }

        private IEnumerator DestroyAndDrawResponse(DealDamageAction dd)
        {

            List<YesNoCardDecision> yesNoDecision = new List<YesNoCardDecision>();

            IEnumerator askPrevent = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.PreventDamage, Card, dd, yesNoDecision, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(askPrevent);
            }
            else
            {
                GameController.ExhaustCoroutine(askPrevent);
            }
            if (!DidPlayerAnswerYes(yesNoDecision))
            {
                yield break;
            }

            IEnumerator preventDamage = CancelAction(dd, isPreventEffect: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(preventDamage);
            }
            else
            {
                GameController.ExhaustCoroutine(preventDamage);
            }


            IEnumerator coroutine = DrawCard(optional: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

             coroutine = DestroyThisCardResponse(dd);
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

    }
}