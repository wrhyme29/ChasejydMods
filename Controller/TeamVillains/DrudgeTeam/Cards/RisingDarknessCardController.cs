using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class RisingDarknessCardController : DrudgeTeamCardController
    {

        public RisingDarknessCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => c.Identifier == ImmortalFormIdentifier, "immortal form"));
        }

        public static readonly string ImmortalFormIdentifier = "ImmortalForm";

        public override IEnumerator Play()
        {
            //Until the start of {DrudgeTeam}'s next turn, Non-Villain cards cannot gain HP.
            CannotGainHPStatusEffect statusEffect = new CannotGainHPStatusEffect();
            statusEffect.CardSource = Card;
            statusEffect.TargetCriteria.IsVillain = false;
            statusEffect.UntilStartOfNextTurn(TurnTaker);
            IEnumerator coroutine = AddStatusEffect(statusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If Immortal Form is in {DrudgeTeam}'s trash, shuffle it back into his deck.
            Card immortalForm = FindCard(ImmortalFormIdentifier);
            if(!TurnTaker.Trash.HasCard(immortalForm))
            {
                yield break;
            }
            coroutine = GameController.ShuffleCardIntoLocation(DecisionMaker, immortalForm, TurnTaker.Deck, false, cardSource: GetCardSource());
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