using System.Collections.Generic;
using Verse;

namespace MSS_Gen;

public class BirthdayGeneDef: Def
{
    public class GeneDefWithWeight: IExposable
    {
        public GeneDef gene;
        public float weight;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref gene, "gene");
            Scribe_Values.Look(ref weight, "weight");
        }
    }
    public List<GeneDefWithWeight> genes;


}
