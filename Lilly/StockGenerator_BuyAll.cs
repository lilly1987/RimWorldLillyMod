using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class StockGenerator_BuyAll : StockGenerator
    {
        public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
        {
            return Enumerable.Empty<Thing>();
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return true;
        }

        public override Tradeability TradeabilityFor(ThingDef thingDef)
        {
            if (thingDef.tradeability == Tradeability.None || !this.HandlesThingDef(thingDef))
            {
                return Tradeability.None;
            }
            return Tradeability.Sellable;
        }
    }
}
