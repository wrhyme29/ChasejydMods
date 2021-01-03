using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class BallroomBlitzCardController : RockstarCardController
    {

        public BallroomBlitzCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Destroy an Environment Card. 

            IEnumerator coroutine = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Select a Villain Deck. Until the start of {Rockstar}'s next turn, the target deck may not play cards.
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.CannotPlayCards, loc => loc.IsVillain, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectLocation(storedResults))
            {
                Location targetDeck = GetSelectedLocation(storedResults);
                CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
                cannotPlayCardsStatusEffect.CardCriteria.NativeDeck = targetDeck;
                cannotPlayCardsStatusEffect.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(cannotPlayCardsStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (!targetDeck.IsSubDeck)
                {
                    PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                    preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = Phase.PlayCard;
                    preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = targetDeck.OwnerTurnTaker;
                    preventPhaseActionStatusEffect.UntilStartOfNextTurn(TurnTaker);
                    coroutine = AddStatusEffect(preventPhaseActionStatusEffect);
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

            //The Villain target with the highest HP deals Rockstar 3 Irreducible Projectile Damage.
            List<Card> storedVillainResults = new List<Card>();
            DealDamageAction gameAction = new DealDamageAction(GameController, null, CharacterCard, 3, DamageType.Projectile, isIrreducible: true);
            coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card card) => IsVillainTarget(card), storedVillainResults, gameAction, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card2 = storedVillainResults.FirstOrDefault();
            if (card2 != null)
            {
                coroutine = DealDamage(card2, CharacterCard, 3, DamageType.Projectile, isIrreducible: true, cardSource: GetCardSource());
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