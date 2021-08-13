using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class HealingSalveCardController : DeeprootTeamCardController
    {

        public HealingSalveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase HP Recovery by Non-Hero Targets by 1.
            AddTrigger((GainHPAction ghp) => ghp.HpGainer != null && !ghp.HpGainer.IsHero && ghp.HpGainer.IsTarget && GameController.IsCardVisibleToCardSource(ghp.HpGainer, GetCardSource()), IncreaseHPRecoveryResponse, TriggerType.IncreaseHPGain, TriggerTiming.Before);

            //At the End of {Deeproot}’s Turn, all Non - Hero Targets gain 1 HP.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => !c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1,  cardSource: GetCardSource()), TriggerType.GainHP);
        }

        private IEnumerator IncreaseHPRecoveryResponse(GainHPAction ghp)
        {
            IncreaseHPGainAction hpGainAction = new IncreaseHPGainAction(GetCardSource(), ghp, 1);
            return DoAction(hpGainAction);
        }
    }
}