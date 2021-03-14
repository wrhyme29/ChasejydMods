using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class BuildingMomentumCardController : HeadlongCardController
    {

        public BuildingMomentumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the Start of {Headlong}'s Turn, he may Draw a Card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, pca => DrawCard(optional: true), TriggerType.DrawCard);

            //{Headlong} may skip his Draw phase. If he does so, he may play a Momentum Card.
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction p) => p.ToPhase.TurnTaker == base.TurnTaker && p.ToPhase.IsDrawCard, SkipToPlayResponse, new TriggerType[] { TriggerType.FirstTrigger, TriggerType.SkipPhase, TriggerType.PlayCard }, TriggerTiming.After);
        }

        private IEnumerator SkipToPlayResponse(PhaseChangeAction pca)
        {
            SkipToTurnPhaseAction skipAction = new SkipToTurnPhaseAction(GetCardSource(), TurnTaker.GetNextTurnPhase(TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.DrawCard).FirstOrDefault()), false, goImmediatelyToPhase: true, interruptActions: true);
            YesNoCardDecision decision = new YesNoCardDecision(GameController, DecisionMaker, SelectionType.SkipTurn, Card, action: skipAction, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.MakeDecisionAction(decision);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(decision))
            {
                coroutine = DoAction(skipAction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, true, cardCriteria: new LinqCardCriteria(c => IsMomentum(c), "momentum"), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

            }
        }
    }
}