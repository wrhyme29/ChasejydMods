using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class FirestarterCardController : BlisterTeamUtilityCardController
    {

        public FirestarterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeHeroCardDestroyed);
        }

        public static readonly string FirstTimeHeroCardDestroyed = "FirstTimeHeroCardDestroyed";

        public override void AddTriggers()
        {
            //The first time each turn that a Hero Card is destroyed, Blister deals the Hero Target with the second highest HP 2 Fire Damage.
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsHero && !HasBeenSetToTrueThisTurn(FirstTimeHeroCardDestroyed), FirstTimeHeroCardDestroyedResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator FirstTimeHeroCardDestroyedResponse(DestroyCardAction dca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeHeroCardDestroyed);
            //Blister deals the Hero Target with the second highest HP 2 Fire Damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 2, (Card c) => c.IsHero && c.IsTarget, (Card c) => 2, DamageType.Fire);
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