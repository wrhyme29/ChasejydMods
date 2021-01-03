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

            //Search your deck or your trash for a Stage Presence card and put it into play or into your hand. If you searched your deck, shuffle your deck.
            coroutine = SearchForCards(DecisionMaker, searchDeck: true, searchTrash: true, 1, 1, new LinqCardCriteria((Card c) => IsStagePresence(c), "stage presence"), putIntoPlay: true, putInHand: true, putOnDeck: false);
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