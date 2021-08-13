using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
    public class StranglevinesCardController : DeeprootTeamCardController
    {

        public StranglevinesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            //Redirect all damage that target would deal to non-Hero targets to {Deeproot}.
            AddRedirectDamageTrigger((DealDamageAction dd) => GetCardThisCardIsNextTo() != null && dd.DamageSource.IsSameCard(GetCardThisCardIsNextTo()) && dd.Target != null && !dd.Target.IsHero, () => CharacterCard);

            //At the Start of {Deeproot}'s Turn, this Card deals the Hero Character it is next to 2 Melee and 2 Toxic Damage.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, StartOfTurnResponse, TriggerType.DealDamage, additionalCriteria: pca => GetCardThisCardIsNextTo() != null);

        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), GetCardThisCardIsNextTo(), 2, DamageType.Melee));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), GetCardThisCardIsNextTo(), 2, DamageType.Toxic));
            IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, targetCriteria: c => c == GetCardThisCardIsNextTo());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            if (storedResults is null)
            {
                yield break;
            }

            List<Card> foundTarget = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (overridePlayArea == null || c.IsAtLocationRecursive(overridePlayArea)), storeHighest: foundTarget, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card highest = foundTarget.FirstOrDefault();
            if (highest != null)
            {
                //Play this card next to the Hero Character with the highest HP. 
                storedResults.Add(new MoveCardDestination(highest.NextToLocation));
            }

            yield break;
        }

    }
}