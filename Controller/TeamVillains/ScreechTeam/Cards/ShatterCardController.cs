using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class ShatterCardController : ScreechTeamCardController
    {

        public ShatterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Destroy a hero equipment card.

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DestroyCard, new LinqCardCriteria(c => c.IsHero && IsEquipment(c) && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(storedResults))
            {
                yield break;
            }

            Card selectedCard = GetSelectedCard(storedResults);
            Location playArea = selectedCard.Location;

            coroutine = GameController.DestroyCard(DecisionMaker, selectedCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then {Screech} Deals all Targets in the play area the Equipment card was in 1 Sonic Damage and 1 Projectile Damage.
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Sonic));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Projectile));
            coroutine = DealMultipleInstancesOfDamage(list, c => c.Location == playArea && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
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