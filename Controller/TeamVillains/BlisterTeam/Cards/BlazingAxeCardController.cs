using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class BlazingAxeCardController : BlisterTeamUtilityCardController
    {

        public BlazingAxeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            
            return card == Card && CharacterCardController.IsGameAdvanced && !CharacterCard.IsFlipped;
        }


    }
}