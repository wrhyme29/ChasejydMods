using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class PowerChordCardController : RockstarCardController
    {

        public PowerChordCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal 1 Target Melee or Projectile Damage equal to the number of unique Ongoings {Rockstar} has in play, plus 2.
            List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, new DamageType[]
            {
            DamageType.Melee,
            DamageType.Projectile
            }, null, SelectionType.DamageType, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            DamageType value = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;

            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), GetNumberOfUniqueOngoingInPlay() + 2, value, 1, false, 1, cardSource: GetCardSource());
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

        private int GetNumberOfUniqueOngoingInPlay()
        {
            return FindCardsWhere(c => c.IsOngoing && c.Owner == TurnTaker && c.IsInPlayAndHasGameText).Distinct().Count();
        }
    }
}