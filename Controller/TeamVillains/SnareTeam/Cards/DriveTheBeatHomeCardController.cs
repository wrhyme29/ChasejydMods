using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class DriveTheBeatHomeCardController : SnareTeamCardController
    {

        public DriveTheBeatHomeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //{Snare} deals the hero target with the second highest HP 2 sonic and 2 energy damage.
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(2, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.Count() > 0)
            {
                Card selectedCard = storedResults.First();
                List<DealDamageAction> list = new List<DealDamageAction>();
                list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), selectedCard, 2, DamageType.Sonic));
                list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), selectedCard, 2, DamageType.Energy));
                coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, targetCriteria: c => c == selectedCard, minNumberOfTargets: 1, maxNumberOfTargets: 1, addStatusEffect: AddCannotUsePowersResponse);
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

        private IEnumerator AddCannotUsePowersResponse(DealDamageAction dd)
        {
            if(!dd.Target.IsHeroCharacterCard)
            {
                yield break;
            }

            //A hero character card dealt damage this way cannot use powers until the start of {Snare}'s next turn.

            Card heroCharacterCard = dd.Target;
            CannotUsePowersStatusEffect cannotUsePowersStatusEffect = new CannotUsePowersStatusEffect();
            cannotUsePowersStatusEffect.CardSource = Card;
            cannotUsePowersStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = heroCharacterCard.Owner;
            cannotUsePowersStatusEffect.UntilStartOfNextTurn(TurnTaker);
            
            IEnumerator coroutine =  AddStatusEffect(cannotUsePowersStatusEffect);
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