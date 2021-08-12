using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.SnareTeam
{
    public class ExtendedCardController : SnareTeamCardController
    {

        public ExtendedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private ITrigger _reduceDamageTrigger;

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (!base.GameController.PreviewMode)
                {
                    return IsLowestHitPointsUnique((Card c) => c != CharacterCard && IsVillainTarget(c) && c.IsInPlayAndHasGameText);
                }
                return true;
            }
        }

        private bool? PerformReduction
        {
            get;
            set;
        }


        public override void AddTriggers()
        {
            // Reduce damage dealt to the villain target with the lowest HP, other than {Snare}, by 2.
            Func<DealDamageAction, bool> villainCriteria = (DealDamageAction dd) => CanCardBeConsideredLowestHitPoints(dd.Target, (Card c) => IsVillainTarget(c) && c != CharacterCard && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
            _reduceDamageTrigger = AddTrigger<DealDamageAction>(villainCriteria, MaybeReduceDamageToVillainTargetResponse, TriggerType.ReduceDamage, TriggerTiming.Before);


            // At the end of {Snare}'s turn, each villain character card gains 1 HP.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => c.IsVillainTeamCharacter && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, cardSource: GetCardSource()), TriggerType.GainHP);
        }

        private IEnumerator MaybeReduceDamageToVillainTargetResponse(DealDamageAction dd)
        {
            if (GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();
                IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.Target, highest: false, (Card c) => IsVillainTarget(c) && c != CharacterCard && c.IsInPlayAndHasGameText, dd, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                PerformReduction = storedResults.Count() > 0 && storedResults.First();
            }
            if (PerformReduction.HasValue && PerformReduction.Value)
            {
                IEnumerator coroutine2 = GameController.ReduceDamage(dd, 2, _reduceDamageTrigger, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            if (!GameController.PretendMode)
            {
                PerformReduction = null;
            }
        }


       
    }
}