using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class SteadyRhythmCardController : DeeprootTeamCardController
    {

        public SteadyRhythmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsEnvironment, "environment"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: GetNumberOfEnvironmentCardsInPlay()).Condition = () => GetNumberOfEnvironmentCardsInPlay() > 0;
        }


        public override IEnumerator Play()
        {
            //{Deeproot} Deals the  X Hero Targets with the Highest HP 2 Melee Damage, where X is equal to the number of Environment Cards in Play
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), (Card c) => 2, DamageType.Melee, numberOfTargets: () => GetNumberOfEnvironmentCardsInPlay());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Until the start of {Deeproot}'s next turn, Villain cards are Immune to damage from Villains and the Environment.
            ImmuneToDamageStatusEffect envImmune = new ImmuneToDamageStatusEffect();
            envImmune.SourceCriteria.IsEnvironment = true;
            envImmune.TargetCriteria.IsVillain = true;
            envImmune.UntilStartOfNextTurn(TurnTaker);

            ImmuneToDamageStatusEffect villainImmune = new ImmuneToDamageStatusEffect();
            villainImmune.SourceCriteria.IsVillain = true;
            villainImmune.TargetCriteria.IsVillain = true;
            villainImmune.UntilStartOfNextTurn(TurnTaker);

            coroutine = AddStatusEffect(envImmune);
            IEnumerator coroutine2 = AddStatusEffect(villainImmune);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }

    }
}