using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class PerfectTimingCardController : HeadlongCardController
    {

        public PerfectTimingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeEnvironmentEntersPlayKey).Condition = () => Card.IsInPlayAndHasGameText;
        }

        private readonly string FirstTimeEnvironmentEntersPlayKey = "FirstTimeEnvironmentEntersPlay";

        public override void AddTriggers()
        {
            //Headlong is Immune to Damage from Environment Cards.
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsEnvironment && dd.Target == CharacterCard);

            //The first time each turn that an Environment Card enters play, you may play a Momentum Card.
            AddTrigger((CardEntersPlayAction cpa) => cpa.CardEnteringPlay.IsEnvironment && !HasBeenSetToTrueThisTurn(FirstTimeEnvironmentEntersPlayKey), PlayMomentumResponse, TriggerType.PlayCard, TriggerTiming.After);

            ResetFlagAfterLeavesPlay(FirstTimeEnvironmentEntersPlayKey);
        }

        private IEnumerator PlayMomentumResponse(CardEntersPlayAction cpa)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeEnvironmentEntersPlayKey);

            //you may play a Momentum Card.
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(HeroTurnTakerController, optional: true, cardCriteria: new LinqCardCriteria(c => IsMomentum(c), "momentum"), cardSource: GetCardSource());
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