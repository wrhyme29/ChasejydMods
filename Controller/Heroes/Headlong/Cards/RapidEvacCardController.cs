using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class RapidEvacCardController : HeadlongCardController
    {

        public RapidEvacCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Destroy an Environment Card. 
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment"), optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, Reveal the top Card of the Environment Deck. Play it or Discard it. 
            coroutine = RevealCard_PlayItOrDiscardIt(HeroTurnTakerController, FindEnvironment().TurnTaker.Deck,responsibleTurnTaker: TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then each Hero and Environment Target gains 1 HP.
            coroutine = GameController.GainHP(HeroTurnTakerController, (Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (c.IsHero || c.IsEnvironment), 1, cardSource: GetCardSource());
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