using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class FeastOnTheLivingCardController : DrudgeTeamCardController
    {

        public FeastOnTheLivingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //{DrudgeTeam} deals the Hero Target with the second highest HP 2 Melee and 2 Infernal Damage. 
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 2, DamageType.Melee));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 2, DamageType.Infernal));
            IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(list, c => c.IsHero, HighestLowestHP.HighestHP, ranking: 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then {DrudgeTeam} gains 3 HP.
            coroutine = GameController.GainHP(CharacterCard, 3, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle all Thralls in {DrudgeTeam}'s trash into his deck.
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