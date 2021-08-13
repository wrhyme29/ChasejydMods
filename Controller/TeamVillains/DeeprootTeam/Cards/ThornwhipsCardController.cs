using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class ThornwhipsCardController : DeeprootTeamCardController
    {

        public ThornwhipsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsEnvironment, "environment"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: GetNumberOfEnvironmentCardsInPlay()).Condition = () => GetNumberOfEnvironmentCardsInPlay() > 0;
        }

        private int X => GetNumberOfEnvironmentCardsInPlay();

        public override void AddTriggers()
        {
            //At the End of Deeproot’s Turn, he deals the X + 1 Hero Targets with the highest HP 2 Melee Damage each, where X is equal to the number of Environment Cards in Play.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => c.IsHero && c.IsTarget, TargetType.HighestHP, 2, DamageType.Melee, numberOfTargets: X + 1);
        }

       
    }
}