using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class DivaCardController : RockstarCardController
    {

        public DivaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimePlayOrPower).Condition = () => Game.ActiveTurnTaker != TurnTaker;
        }


        private const string FirstTimePlayOrPower = "FirstTimePlayOrPower";
        public override void AddTriggers()
        {
            //The first time per turn, outside of her own, a card allows {Rockstar} to play a card or use a power , {Rockstar} may first destroy an Ongoing, Device, or Environment Card.

            AddTrigger<PlayCardAction>((PlayCardAction pca) => Game.ActiveTurnTaker != TurnTaker && pca.ResponsibleTurnTaker == TurnTaker && !HasBeenSetToTrueThisTurn(FirstTimePlayOrPower), MaybeDestroyCardResponse, TriggerType.DestroyCard, TriggerTiming.Before);
            AddTrigger<UsePowerAction>((UsePowerAction upa) => Game.ActiveTurnTaker != TurnTaker && upa.HeroUsingPower == HeroTurnTakerController && !HasBeenSetToTrueThisTurn(FirstTimePlayOrPower), MaybeDestroyCardResponse, TriggerType.DestroyCard, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimePlayOrPower), TriggerType.Hidden);
        }

        private IEnumerator MaybeDestroyCardResponse(GameAction action)
        {
            //{Rockstar} may first destroy an Ongoing, Device, or Environment Card.
            SetCardPropertyToTrueIfRealAction(FirstTimePlayOrPower);
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment || c.IsOngoing || (c.IsDevice && !c.IsCharacter), "ongoing, device, or environment"), true, cardSource: GetCardSource());
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