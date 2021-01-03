using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class WreckingBallCardController : RockstarCardController
    {

        public WreckingBallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal {Rockstar} 2 Irreducible Fire Damage. 
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Fire, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //You may destroy up to 3 Ongoing or Environment Cards.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            coroutine = base.GameController.SelectAndDestroyCards(HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment || c.IsOngoing, "ongoing or environment"), 3, requiredDecisions: 0, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //{Rockstar} may deal one Target Melee Damage equal to the number of cards destroyed this turn.
            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), GetNumberOfCardsDestroyedThisTurn(), DamageType.Melee, 1, false, 0, cardSource: GetCardSource());
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

        private int GetNumberOfCardsDestroyedThisTurn()
        {
            return Game.Journal.DestroyCardEntriesThisTurn().Count();
        }

    }
}