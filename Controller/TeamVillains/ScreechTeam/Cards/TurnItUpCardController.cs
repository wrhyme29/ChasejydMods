using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class TurnItUpCardController : ScreechTeamCardController
    {

        public TurnItUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDiscardKey, "A hero has already discarded a card this turn", "A hero has not discarded a card this turn").Condition = () => Card.IsInPlayAndHasGameText;
        }

        public static readonly string FirstTimeDiscardKey = "FirstTimeDiscard";

        public override void AddTriggers()
        {
            //The first time each turn that a Hero discards a card, {Screech} deals that Target 1 Irreducible Sonic Damage.
            AddTrigger((DiscardCardAction dca) => !HasBeenSetToTrueThisTurn(FirstTimeDiscardKey), HeroDiscardedResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator HeroDiscardedResponse(DiscardCardAction dca)
        {
            //{Screech} deals that Target 1 Irreducible Sonic Damage.
            SetCardPropertyToTrueIfRealAction(FirstTimeDiscardKey);
            HeroTurnTakerController httc = dca.HeroTurnTakerController;
            Card target = httc.CharacterCard;
            IEnumerator coroutine;
            if(httc.HasMultipleCharacterCards)
            {
                List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
                DealDamageAction gameAction = new DealDamageAction(GetCardSource(), null, null, 1, DamageType.Sonic, isIrreducible: true);
                coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DealDamage, new LinqCardCriteria((Card c) => c.Owner == httc.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, $"active {httc.TurnTaker.Name} heroes"), storedDecision, false, gameAction: gameAction, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
                if (DidSelectCard(storedDecision))
                {
                    target = GetSelectedCard(storedDecision);
                }
            }

            coroutine = DealDamage(CharacterCard, target, 1, DamageType.Sonic, isIrreducible: true, cardSource: GetCardSource());
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