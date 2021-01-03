using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class HitMeBabyCardController : RockstarCardController
    {

        public HitMeBabyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimePlayOrPower).Condition = () => Game.ActiveTurnTaker != TurnTaker;
        }


        private const string FirstTimePlayOrPower = "FirstTimePlayOrPower";

        public override IEnumerator Play()
        {
            //When this card enters play, {Rockstar} gains 2 HP.
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //The first time per turn outside of her own that another Hero allows {Rockstar} to Use a Power or Play a Card, she may first Play a Card.
            AddTrigger<PlayCardAction>((PlayCardAction pca) => Game.ActiveTurnTaker != TurnTaker && pca.ResponsibleTurnTaker == TurnTaker && !HasBeenSetToTrueThisTurn(FirstTimePlayOrPower), PlayCardResponse, TriggerType.PlayCard, TriggerTiming.Before);
            AddTrigger<UsePowerAction>((UsePowerAction upa) => Game.ActiveTurnTaker != TurnTaker && upa.HeroUsingPower == HeroTurnTakerController && !HasBeenSetToTrueThisTurn(FirstTimePlayOrPower), PlayCardResponse, TriggerType.PlayCard, TriggerTiming.Before);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimePlayOrPower), TriggerType.Hidden);
        }

        private IEnumerator PlayCardResponse(GameAction action)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimePlayOrPower);
            IEnumerator coroutine = SelectAndPlayCardFromHand(HeroTurnTakerController);
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