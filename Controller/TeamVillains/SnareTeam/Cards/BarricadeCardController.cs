using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class BarricadeCardController : SnareTeamCardController
    {

        public BarricadeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by villain character cards other than {Snare} by 1.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsVillainTeamCharacter && !dd.DamageSource.IsSameCard(CharacterCard), 1);

            //Reduce damage dealt to villain character cards other than {Snare} by 1.
            AddReduceDamageTrigger(c => c.IsVillainTeamCharacter && c != CharacterCard, 1);

            //At the start of {Snare}’s turn, destroy this card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

        }


    }
}