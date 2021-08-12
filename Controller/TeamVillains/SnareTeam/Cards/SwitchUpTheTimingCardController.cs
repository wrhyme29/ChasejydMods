using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class SwitchUpTheTimingCardController : SnareTeamCardController
    {

        public SwitchUpTheTimingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Heroes may not play cards, use powers, or draw Cards outside of their own turn.
            CannotPlayCards(ttc => ttc.IsHero && GameController.ActiveTurnTaker != ttc.TurnTaker);
            CannotUsePowers(ttc => ttc.IsHero && GameController.ActiveTurnTaker != ttc.TurnTaker);
            CannotDrawCards(ttc => ttc.IsHero && GameController.ActiveTurnTaker != ttc.TurnTaker);

            //At the start of {Snare}’s turn, destroy this card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

            //When this card is destroyed, {Snare} deals the hero target with the lowest HP 3 psychic damage
            AddWhenDestroyedTrigger(dca => DealDamageToLowestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Psychic), TriggerType.DealDamage);
        }

    }
}