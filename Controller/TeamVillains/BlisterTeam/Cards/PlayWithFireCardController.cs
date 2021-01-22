using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class PlayWithFireCardController : BlisterTeamUtilityCardController
    {

        public PlayWithFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //If Blazing Axe is in play, restore it to 10 HP. 
            if(IsBlazingAxeInPlay())
            {
                Card axe = FindBlazingAxeInPlay();
                coroutine = GameController.SetHP(axe, 10, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //If Blazing Axe is in Blister's Trash, shuffle it back into Blister's deck.
            if(IsBlazingAxeInTrash())
            {
                Card axe = FindBlazingAxeInTrash();
                coroutine = GameController.ShuffleCardIntoLocation(HeroTurnTakerController, axe, TurnTaker.Deck, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Blister recovers 3 HP
            coroutine = GameController.GainHP(CharacterCard, 3, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //then Play the top Card of Blister's deck.
            coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, showMessage: true, cardSource: GetCardSource());
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