using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class BurnItDownCardController : BlisterTeamUtilityCardController
    {

        public BurnItDownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Whenever an Environment Card is destroyed, {Blister} deals the Hero Target with the highest HP 2 Fire Damage.
            AddTrigger((DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsEnvironment, EnvironmentDestroyedResponse, TriggerType.DealDamage, TriggerTiming.After);
            //At the end of {Blister}'s turn, Destroy an Environment Card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, TriggerType.DestroyCard);
        }

        private IEnumerator EnvironmentDestroyedResponse(DestroyCardAction pca)
        {
            //{Blister} deals the Hero Target with the highest HP 2 Fire Damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 2, DamageType.Fire);
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

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
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
            yield break;
        }
    }
}