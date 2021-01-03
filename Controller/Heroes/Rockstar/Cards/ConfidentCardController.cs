using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class ConfidentCardController : RockstarCardController
    {

        public ConfidentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDestroy);
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDiscard);
        }

        private const string FirstTimeDestroy = "FirstTimeDestroy";
        private const string FirstTimeDiscard = "FirstTimeDiscard";

        public override void AddTriggers()
        {
            //The first time each turn that one of {Rockstar}'s cards is destroyed, she may play a card.
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card.Owner == TurnTaker && !HasBeenSetToTrueThisTurn(FirstTimeDestroy), DestroyCardResponse, TriggerType.PlayCard, TriggerTiming.After);

            //The first time each turn that {Rockstar} is forced to discard, she may draw a card.
            AddTrigger<DiscardCardAction>((DiscardCardAction dca) => dca.WasCardDiscarded && dca.Origin.IsHand && dca.Origin.OwnerTurnTaker == base.TurnTaker && !HasBeenSetToTrueThisTurn(FirstTimeDiscard), DiscardCardResponse, TriggerType.DrawCard, TriggerTiming.After);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeDiscard), TriggerType.Hidden);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeDestroy), TriggerType.Hidden);
        }

        private IEnumerator DiscardCardResponse(DiscardCardAction arg)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDiscard);
            IEnumerator coroutine = DrawCard(hero: HeroTurnTaker, optional: true);
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

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDestroy);
            IEnumerator coroutine = SelectAndPlayCardFromHand(HeroTurnTakerController);
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