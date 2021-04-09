using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
    public class BarkShieldCardController : DeeprootTeamCardController
    {

        public BarkShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Redirect damage that Target would take to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => GetCardThisCardIsNextTo() != null && dd.Target == GetCardThisCardIsNextTo(), () => Card);

            //Reduce non-Fire Damage dealt to this card by 1.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageType != DamageType.Fire && dd.Target == Card, dd => 1);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            if (storedResults is null)
            {
                yield break;
            }

            List<Card> foundTarget = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsVillainCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (overridePlayArea == null || c.IsAtLocationRecursive(overridePlayArea)), foundTarget, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card lowest = foundTarget.FirstOrDefault();
            if (lowest != null)
            {
                //Play this card next to the Villain character card with the lowest HP. 
                storedResults.Add(new MoveCardDestination(lowest.NextToLocation));
            }
           
            yield break;
        }


    }
}