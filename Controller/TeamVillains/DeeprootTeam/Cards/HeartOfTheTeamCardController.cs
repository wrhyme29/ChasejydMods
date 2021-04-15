using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class HeartOfTheTeamCardController : DeeprootTeamCardController
    {

        public HeartOfTheTeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //All Villain and Environment Targets gain 2 HP.
            IEnumerator coroutine = GameController.GainHP(DecisionMaker, (Card c) => (IsVillainTarget(c) || c.IsEnvironmentTarget) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Redirect the next damage that would be dealt to a Villain Character Card to {Deeproot}.
            RedirectDamageStatusEffect effect = new RedirectDamageStatusEffect();
            effect.NumberOfUses = 1;
            effect.RedirectTarget = CharacterCard;
            effect.TargetCriteria.IsVillain = true;
            effect.TargetCriteria.IsCharacter = true;
            coroutine = AddStatusEffect(effect);
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