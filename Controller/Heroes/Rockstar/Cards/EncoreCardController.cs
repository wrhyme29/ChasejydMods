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
            //{Rockstar} gains 2 HP.
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
               GameController.ExhaustCoroutine(coroutine);
            }

            // You may take an Ongoing from your Trash and put it on top of your deck.
            MoveCardDestination destinations = new MoveCardDestination(HeroTurnTakerController.TurnTaker.Deck);
            coroutine = GameController.SelectCardsFromLocationAndMoveThem(HeroTurnTakerController, HeroTurnTakerController.TurnTaker.Trash, 0, 1, new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing"), destinations.ToEnumerable(), cardSource: GetCardSource());
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