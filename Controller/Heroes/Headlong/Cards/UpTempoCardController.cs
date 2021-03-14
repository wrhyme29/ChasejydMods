using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class UpTempoCardController : HeadlongCardController
    {

        public UpTempoCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimePlayingMomentumKey);
        }

        private readonly string FirstTimePlayingMomentumKey = "FirstTimePlayingMomentum";

        public override void AddTriggers()
        {
            //The first time {Headlong} plays a Momentum Card each turn, one Player may Draw a card.
            AddTrigger((PlayCardAction pca) => IsMomentum(pca.CardToPlay) && !HasBeenSetToTrueThisTurn(FirstTimePlayingMomentumKey) && pca.IsSuccessful, DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);
        }

        private IEnumerator DrawCardResponse(PlayCardAction pca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimePlayingMomentumKey);
            IEnumerator coroutine = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
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