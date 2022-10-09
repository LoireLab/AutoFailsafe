using Jobs;
using Newtonsoft.Json.Linq;
using NPC;
using Pipliz;

namespace AdvancedForester
{
    [AreaJobDefinitionAutoLoader]
    public class AutummOrangeForesterDefinition : AbstractFarmAreaJobDefinition
    {
        public AutummOrangeForesterDefinition()
        {
            AllowGoalOffset = true;
            Identifier = "Khanx.AutummOrangeForester";
            UsedNPCType = NPCType.GetByKeyNameOrDefault("pipliz.forester");
            Stages = new ushort[0];
        }

        public override IAreaJob CreateAreaJob(Colony owner, Vector3Int min, Vector3Int max)
        {
            return new ForesterJob(this, owner, min, max, ItemTypes.GetType("saplingfallorange"));
        }

        public override IAreaJob LoadAreaJob(Colony owner, Vector3Int min, Vector3Int max, NPCID? npcID, JObject miscData)
        {
            return new ForesterJob(this, owner, min, max, ItemTypes.GetType("saplingfallorange"),  npcID);
        }
    }
}
