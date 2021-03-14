using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class RapidLeadershipCardController : HeadlongCardController
    {

        public RapidLeadershipCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this Card comes into Play, each other Player may Draw a Card.
            IEnumerator coroutine = EachPlayerDrawsACard(heroCriteria: htt => htt != HeroTurnTaker);
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

        public override IEnumerator UsePower(int index = 0)
        {
            //One other Hero may Play a Card. 
            IEnumerator coroutine = SelectHeroToPlayCard(DecisionMaker, heroCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then one other Hero may Draw a Card.
            coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker), cardSource: GetCardSource());
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