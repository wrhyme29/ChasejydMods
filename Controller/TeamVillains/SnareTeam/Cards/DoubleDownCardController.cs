using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class DoubleDownCardController : SnareTeamCardController
    {

        public DoubleDownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeVillainOngoingDestroyedKey, "A villain ongoing has already been destroyed this turn", "A villain ongoing has not been destroyed this turn");
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public static readonly string FirstTimeVillainOngoingDestroyedKey = "FirstTimeVillainOngoingDestroyed";

        public override void AddTriggers()
        {
            //The first time each turn that a villain ongoing would be destroyed, {Snare} destroys a hero ongoing and deals the hero target with the second highest HP 2 energy damage.
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsOngoing && IsVillain(dca.CardToDestroy.Card) && !HasBeenSetToTrueThisTurn(FirstTimeVillainOngoingDestroyedKey), FirstTimeVillainOngoingDestroyedResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage }, TriggerTiming.After);
        }

        private IEnumerator FirstTimeVillainOngoingDestroyedResponse(DestroyCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeVillainOngoingDestroyedKey);

            //{ Snare} destroys a hero ongoing...
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && c.IsHero && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and deals the hero target with the second highest HP 2 energy damage
            coroutine = DealDamageToHighestHP(CharacterCard, 2, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 2, DamageType.Energy);
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