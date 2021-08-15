using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class TotalChaosCardController : ScreechTeamCardController
    {

        public TotalChaosCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of {Screech}'s turn, destroy a Hero Ongoing, then {Screech} deals the Hero Target with the highest HP 1 Sonic and 1 Projectile damage.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage });

            //Increase Damage dealt to Screech by other Villain Targets by 1.
            AddIncreaseDamageTrigger(dd => dd.Target != null && dd.Target == CharacterCard && dd.DamageSource != null && dd.DamageSource.IsVillainTarget, 1);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //destroy a Hero Ongoing,
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsHero && c.IsOngoing && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //then {Screech} deals the Hero Target with the highest HP 1 Sonic and 1 Projectile damage.
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Sonic));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Projectile));
            coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(list, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HighestLowestHP.HighestHP);
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