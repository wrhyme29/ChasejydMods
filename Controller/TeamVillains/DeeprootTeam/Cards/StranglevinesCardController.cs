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

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
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