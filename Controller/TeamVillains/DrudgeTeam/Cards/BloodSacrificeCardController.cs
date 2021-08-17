using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class BloodSacrificeCardController : DrudgeTeamCardController
    {

        public BloodSacrificeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithLowestHP();
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, cardCriteria: new LinqCardCriteria(c => c.Identifier == ThrallIdentifier, "", useCardsSuffix: true, singular: "Thrall", plural: "Thralls"));
        }

        public override void AddTriggers()
        {
            //At the start of {DrudgeTeam}'s turn, {DrudgeTeam} deals 2 Infernal Damage to the non-Villain Target with the lowest HP.
            //Then reveal cards from the top of his deck until Thrall is revealed. Put Thrall into play. Shuffle all other cards revealed back into his deck.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.RevealCard });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //{DrudgeTeam} deals 2 Infernal Damage to the non-Villain Target with the lowest HP.
            IEnumerator coroutine = DealDamageToLowestHP(CharacterCard, 1, c => !IsVillainTarget(c), c => 2, DamageType.Infernal);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then reveal cards from the top of his deck until Thrall is revealed. Put Thrall into play. Shuffle all other cards revealed back into his deck.
            coroutine = RevealCardsFromDeckToPlayThrall();
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