using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class RockstarRoarCardController : RockstarCardController
    {

        public RockstarRoarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDestroyCard);
        }

        private const string FirstTimeDestroyCard = "FirstTimeDestroyCard";


        public override void AddTriggers()
        {

            //At the Start of her Turn, Rockstar draws a Card.

            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, pca => DrawCard(), TriggerType.DrawCard);

            //The first time each turn that {Rockstar} destroys a Non-Hero card she may deal 1 Target 2 Melee Damage and gain 1 HP.
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => FirstTimeDestroyingNonHeroCriteria(dca), DealDamageAndHealResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.GainHP
            }, TriggerTiming.After);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeDestroyCard), TriggerType.Hidden);
        }

        public bool FirstTimeDestroyingNonHeroCriteria(DestroyCardAction dca)
        {
            return dca.WasCardDestroyed && dca.CardToDestroy != null && !dca.CardToDestroy.Card.IsHero && dca.CardSource.CardController == CharacterCardController && !HasBeenSetToTrueThisTurn(FirstTimeDestroyCard);
        }

        private IEnumerator DealDamageAndHealResponse(DestroyCardAction arg)
        {
            //she may deal 1 Target 2 Melee Damage and gain 1 HP
            SetCardPropertyToTrueIfRealAction(FirstTimeDestroyCard);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 0, storedResultsDecisions: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(storedResults))
            {
                coroutine = GameController.GainHP(CharacterCard, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
           

            yield break;
        }
    }
}