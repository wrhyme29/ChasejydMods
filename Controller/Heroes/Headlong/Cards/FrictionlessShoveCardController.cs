using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class FrictionlessShoveCardController : HeadlongCardController
    {

        public FrictionlessShoveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Place a non-Character Villain Target in play on the top of its Deck. 
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => IsVillainTarget(c) && c.IsInPlay && !c.IsCharacter &&  !c.IsOneShot && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-character villain cards in play", useCardsSuffix: false), storedResults, optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

           if(DidSelectCard(storedResults))
            {
                Card selectedCard = GetSelectedCard(storedResults);
                coroutine = GameController.MoveCard(TurnTakerController, selectedCard, selectedCard.NativeDeck, showMessage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{Headlong} may Deal a non-Character Target 3 Projectile Damage.
            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 3, DamageType.Projectile, 1, false, 0, additionalCriteria: (Card c) => !c.IsCharacter, cardSource: GetCardSource());
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