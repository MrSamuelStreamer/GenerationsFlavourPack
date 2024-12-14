using RimWorld;
using Verse;

namespace MSS_Gen;

public class StatPart_FertilityBoost: StatPart
{
    public float factor = .5f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (req.Thing is Pawn pawn && val > 0f)
        {
            val += factor;
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (req.Thing is Pawn pawn)
        {
            return "MSS_Gen_StatPart_FertilityBoost_Explanation".Translate(factor.ToStringPercentSigned()).CapitalizeFirst();
        }

        return null;
    }
}
