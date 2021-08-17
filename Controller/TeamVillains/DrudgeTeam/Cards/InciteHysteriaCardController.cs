using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class InciteHysteriaCardController : DrudgeTeamCardController
    {

        public InciteHysteriaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithLowestHP(numberOfTargets: 3);
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            // {DrudgeTeam} deals the 3 non-Villain Targets with the lowest HP 2 Psychic Damage each.
            IEnumerator coroutine = DealDamageToLowestHP(CharacterCard, 1, c => !IsVillainTarget(c), c => 2, DamageType.Psychic, numberOfTargets: 3);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //  Each Thrall in play deals the Hero Target with the highest HP 2 Melee Damage.
            foreach(Card thrall in FindCardsWhere(c => IsThrall(c) && c.IsInPlayAndHasGameText))
            {
                coroutine = DealDamageToHighestHP(thrall, 1, c => c.IsHero, c => 2, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }


            //  Shuffle all Thralls in {DrudgeTeam}'s trash into his deck.
            IEnumerable<Card> thralls = FindCardsWhere(c => IsThrall(c) && TurnTaker.Trash.HasCard(c));
            coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, thralls, TurnTaker.Deck, cardSource: GetCardSource());
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