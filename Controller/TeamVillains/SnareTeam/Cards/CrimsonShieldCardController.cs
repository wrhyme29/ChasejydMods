using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class CrimsonShieldCardController : SnareTeamCardController
    {

        public CrimsonShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible.
            return card == Card;
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to {Snare} by 2.
            AddReduceDamageTrigger(c => c == CharacterCard, 2);
        }

    }
}