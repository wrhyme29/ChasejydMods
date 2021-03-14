using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class CourierCardController : HeadlongCardController
    {

        public CourierCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeEnvironmentDestroyedKey, trueFormat: "An environment card has been destroyed this turn.", falseFormat: "An environment card has not been destroyed this turn.");
        }

        public readonly string FirstTimeEnvironmentDestroyedKey = "FirstTimeEnvironmentDestroyed";

        public override void AddTriggers()
        {
            //The first time each Turn an Environment Card is Destroyed, you may look at the Top Card of the Environment Deck and replace it or Discard it.
            AddTrigger((DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card.IsEnvironment && dca.WasCardDestroyed && !HasBeenSetToTrueThisTurn(FirstTimeEnvironmentDestroyedKey), RevealAndReplaceEnvironmentResponse, TriggerType.RevealCard, TriggerTiming.After);
        }

        private IEnumerator RevealAndReplaceEnvironmentResponse(DestroyCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeEnvironmentDestroyedKey);
            IEnumerator coroutine = RevealCard_DiscardItOrPutItOnDeck(DecisionMaker, FindEnvironment(), FindEnvironment().TurnTaker.Deck, false);
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

        public override IEnumerator UsePower(int index = 0)
        {
            //Each Player may Draw 2 Cards. Then Destroy this Card.
            LinqTurnTakerCriteria heroCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes");
            IEnumerator coroutine = GameController.DrawCards(heroCriteria, 2, optional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(null);
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