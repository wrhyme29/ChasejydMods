using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra;

namespace Chasejyd.Headlong
{
    public class SetUpCardController : HeadlongCardController
    {

        public SetUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal one Non-Hero Target 1 Melee Damage. 
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 1, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If a Target takes Damage this way, another Hero deals that same Target 3 Irreducible Melee Damage. 
            if(DidDealDamage(storedResults))
            {
                Card target = storedResults.First().Target;
                if(target.IsInPlayAndHasGameText)
                {
                    DealDamageAction fakeDamage = new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), target, 3, DamageType.Melee, isIrreducible: true);
                    List<SelectTurnTakerDecision> storedHero = new List<SelectTurnTakerDecision>();
                    coroutine = GameController.SelectHeroTurnTaker(HeroTurnTakerController, SelectionType.DealDamage, optional: false, allowAutoDecide: false, storedResults: storedHero, heroCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker), gameAction: fakeDamage, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (DidSelectTurnTaker(storedHero))
                    {
                        TurnTaker selectedTurnTaker = GetSelectedTurnTaker(storedHero);
                        Card source = selectedTurnTaker.CharacterCard;
                        if (selectedTurnTaker.HasMultipleCharacterCards)
                        { 
                            List<Card> storedSource = new List<Card>();
                            coroutine = FindCharacterCard(selectedTurnTaker, SelectionType.HeroToDealDamage, storedSource, additionalCriteria: c => c.IsInPlayAndHasGameText, damageInfo: fakeDamage.ToEnumerable());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            source = storedSource.First();
                        }

                        coroutine = DealDamage(source, target, 3, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
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

            //One Hero may Draw a Card.
            coroutine = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
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