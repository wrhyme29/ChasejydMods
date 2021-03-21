using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class EncoreCardController : RockstarCardController
    {

        public EncoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Rockstar takes up to 2 Cards from her Trash and puts them in her hand. 
            List<MoveCardDestination> destinations = new List<MoveCardDestination>() { new MoveCardDestination(HeroTurnTaker.Hand) };
            IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, 0, 2, new LinqCardCriteria(c => true), destinations, selectionType: SelectionType.MoveCardToHand, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //Then Rockstar may use a Power.
            coroutine = GameController.SelectAndUsePower(HeroTurnTakerController, cardSource: GetCardSource());
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