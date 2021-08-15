using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class BuyingTimeCardController : SnareTeamCardController
    {

        public BuyingTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP(numberOfTargets: X);
        }

        private int X => FindCardsWhere(c => IsBarrier(c) && c.IsInPlayAndHasGameText).Count();
        public override IEnumerator Play()
        {
            //Find the X villain targets with the lowest HP, where X is equal to the number of Barrier cards in play.
            
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, X, c => c.IsVillainTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //those X targets gain 2 HP
            coroutine = GameController.GainHP(DecisionMaker, c => storedResults.Contains(c), 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Play the top card of {Snare}'s deck.
            coroutine = GameController.SendMessageAction(Card.Title + " plays the top card of " + TurnTaker.Name + "'s deck...", Priority.High, GetCardSource(), null, showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, cardSource: GetCardSource());
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