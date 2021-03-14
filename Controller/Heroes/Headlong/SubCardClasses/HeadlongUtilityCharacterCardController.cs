using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Headlong
{
	public class HeadlongUtilityCharacterCardController : HeroCharacterCardController
	{
		public HeadlongUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public static readonly string MomentumKeyword = "momentum";

		protected bool IsMomentum(Card card)
		{
			return card.DoKeywordsContain(MomentumKeyword);
		}
	}
}
