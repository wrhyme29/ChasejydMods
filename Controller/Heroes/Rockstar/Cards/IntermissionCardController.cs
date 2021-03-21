using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class IntermissionCardController : RockstarCardController
    {

        public IntermissionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Rockstar} gains 1 HP. 
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Rockstar Draws 2 cards
            coroutine = DrawCards(HeroTurnTakerController, 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Rockstar may Play a Stage Presence Card.
            coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, optional: true, cardCriteria: new LinqCardCriteria(c => IsStagePresence(c), "stage presence"), cardSource: GetCardSource());
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