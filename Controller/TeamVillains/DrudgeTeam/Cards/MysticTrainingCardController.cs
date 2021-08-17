using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class MysticTrainingCardController : DrudgeTeamCardController
    {

        public MysticTrainingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase Infernal and Psychic Damage dealt by {DrudgeTeam} by 1.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && (dd.DamageType == DamageType.Infernal || dd.DamageType == DamageType.Psychic), 1);
        }
    }
}