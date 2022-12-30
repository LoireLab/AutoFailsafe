using Jobs;
using Newtonsoft.Json.Linq;
using NPC;
using Pipliz;

namespace AdvancedForester
{
    [AreaJobDefinitionAutoLoader]
    public class AutummYellowForesterDefinition : AbstractFarmAreaJobDefinition
    {
        public AutummYellowForesterDefinition()
        {
            AllowGoalOffset = true;
            Identifier = "Khanx.AutummYellowForester";
            UsedNPCType = NPCType.GetByKeyNameOrDefault("pipliz.forester");
            Stages = new ushort[0];
        }

        public override IAreaJob CreateAreaJob(Colony owner, Vector3Int min, Vector3Int max)
        {
            return new ForesterJob(this, owner, min, max, ItemTypes.GetType("Khanx.AdvancedForester.saplingfallyellow"));
        }

        public override IAreaJob LoadAreaJob(Colony owner, Vector3Int min, Vector3Int max, NPCID? npcID, JObject miscData)
        {
            return new ForesterJob(this, owner, min, max, ItemTypes.GetType("Khanx.AdvancedForester.saplingfallyellow"), npcID);
        }
    }
}
