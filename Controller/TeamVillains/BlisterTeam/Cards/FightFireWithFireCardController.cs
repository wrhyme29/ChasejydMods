using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class FightFireWithFireCardController : BlisterTeamUtilityCardController
    {

        public FightFireWithFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDeviceDestroyed).Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public static readonly string FirstTimeDeviceDestroyed = "FirstTimeDeviceDestroyed";

        public override void AddTriggers()
        {
            //The first time each turn that a Device is destroyed, destroy an Equipment Card and deal the Hero Target with the Highest HP 2 Fire Damage.
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsDevice && !HasBeenSetToTrueThisTurn(FirstTimeDeviceDestroyed), FirstTimeDeviceDestroyedResponse, new TriggerType[]
            {
                TriggerType.DestroyCard,
                TriggerType.DealDamage
            }, TriggerTiming.After);
        }

        private IEnumerator FirstTimeDeviceDestroyedResponse(DestroyCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDeviceDestroyed);
            //Destroy an Environment Card.
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //{Blister} deals the Hero Target with the highest HP 2 Fire Damage.
            coroutine = DealDamageToHighestHP(Card, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 2, DamageType.Fire);
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