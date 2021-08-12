using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class ReinforcedCardController : SnareTeamCardController
    {

        public ReinforcedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDamageDealtToSnareKey).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public static readonly string FirstTimeDamageDealtToSnareKey = "FirstTimeDamageDealtToSnare";

        public override void AddTriggers()
        {
            //Prevent the first damage that would be dealt to {Snare} each turn.
            AddTrigger((DealDamageAction dd) => dd.Target == CharacterCard && !HasBeenSetToTrueThisTurn(FirstTimeDamageDealtToSnareKey), PreventDamageResponse, TriggerType.CancelAction, TriggerTiming.Before);

            //Increase damage dealt by {Snare} by 1.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.IsSameCard(CharacterCard), 1);
        }

        private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealtToSnareKey);
            IEnumerator coroutine = CancelAction(dd,  isPreventEffect: true);
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