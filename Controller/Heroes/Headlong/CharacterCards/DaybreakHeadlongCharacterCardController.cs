using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Headlong
{
	public class DaybreakHeadlongCharacterCardController : HeadlongUtilityCharacterCardController
	{
		public DaybreakHeadlongCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
        {
            //Two Players may draw a card. 
            IEnumerator coroutine = TwoPlayersMayDrawACard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //You may Destroy an Environment Card or Play the top Card of the Environment Deck.
            //Option 1: Destroy an Environment Card
            var response1 = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment"), optional: true, cardSource: GetCardSource());
            var op1 = new Function(this.DecisionMaker, $" Destroy an Environment Card.", SelectionType.DestroyCard, () => response1);

            //Option: 2 Play the top Card of the Environment Deck.
            var response2 = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
            var op2 = new Function(this.DecisionMaker, $"Play the top card of the environment deck.", SelectionType.PlayTopCardOfEnvironmentDeck, () => response2);

            //Execute
            var options = new Function[] { op1, op2 };
            var selectFunctionDecision = new SelectFunctionDecision(base.GameController, DecisionMaker, options, optional: true, cardSource: base.GetCardSource());
            coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
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

        private IEnumerator TwoPlayersMayDrawACard()
        {
            LinqTurnTakerCriteria criteria = new LinqTurnTakerCriteria(tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()));
            SelectTurnTakersDecision selectTurnTakersDecision = new SelectTurnTakersDecision(GameController, DecisionMaker, criteria, SelectionType.DrawCard, numberOfTurnTakers: 2, cardSource: GetCardSource()); ;
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, tt => DrawCard(hero: tt.ToHero(), optional: true));
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

        public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
                        //Increase Damage dealt by Environment Cards to Villain Targets by 1 until the Start of your next turn
                        IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(1);
                        effect.SourceCriteria.IsEnvironment = true;
                        effect.TargetCriteria.IsVillain = true;
                        effect.UntilStartOfNextTurn(TurnTaker);
                        IEnumerator coroutine = AddStatusEffect(effect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
					}
				case 1:
					{
                        //2 Players may Draw a Card.
                        IEnumerator coroutine = TwoPlayersMayDrawACard();
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
					}
				case 2:
					{
                        //One Hero may Play a Card.
                        IEnumerator coroutine = SelectHeroToPlayCard(DecisionMaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
			}
			yield break;
		}
	}
}
