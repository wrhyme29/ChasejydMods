using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class ResonantCardController : SnareTeamCardController
    {

        public ResonantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When a hero target deals damage to {Snare}, {Snare} then deals that target 2 energy damage.
            AddCounterDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard, () => CharacterCard, () => CharacterCard, oncePerTargetPerTurn: false, 2, DamageType.Energy);
        }
    }
}