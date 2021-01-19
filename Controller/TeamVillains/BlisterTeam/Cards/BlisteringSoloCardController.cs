using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.BlisterTeam
{
    public class BlisteringSoloCardController : BlisterTeamUtilityCardController
    {

        public BlisteringSoloCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Damage from { Blister} is Irreducible.
            AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard));
            //Heroes damaged by Blister cannot use powers or play cards outside of their own turn until the Start of Blister's next turn
            AddTrigger((DealDamageAction dd) => dd.Target != null && dd.Target.IsHeroCharacterCard && dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.DidDealDamage, BlisterDamagedHeroResponse, TriggerType.ShowMessage, TriggerTiming.After);
            CannotPlayCards((TurnTakerController ttc) => ttc.IsHero && Game.ActiveTurnTaker != ttc.TurnTaker && HasHeroBeenDealtDamageByBlisterSinceStartOfLastBlisterTurn(ttc.ToHero()));
            CannotUsePowers((TurnTakerController ttc) => ttc.IsHero && Game.ActiveTurnTaker != ttc.TurnTaker && HasHeroBeenDealtDamageByBlisterSinceStartOfLastBlisterTurn(ttc.ToHero()));
        }

        private bool HasHeroBeenDealtDamageByBlisterSinceStartOfLastBlisterTurn(HeroTurnTakerController httc)
        {
            int turnIndexOfBlister = GameController.AllTurnTakers.ToList().IndexOf(TurnTaker);
            bool hasBeenDealtDamage =    (from e in Game.Journal.DealDamageEntries()
                                          where httc.CharacterCards.Contains(e.TargetCard) && e.SourceCard == CharacterCard && ((e.Round == Game.Round && e.TurnIndex >= turnIndexOfBlister) || (e.Round < Game.Round && e.TurnIndex < turnIndexOfBlister))
                                          select e).Any();
            return hasBeenDealtDamage;
        }

        private IEnumerator BlisterDamagedHeroResponse(DealDamageAction dd)
        {
            HeroTurnTaker hero = dd.Target.Owner.ToHero();
            IEnumerator coroutine = GameController.SendMessageAction($"{hero.Name} cannot use powers or play cards outside of their own turn until the start of {TurnTaker.Name}'s next turn.", Priority.High, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}