﻿using Jobs;
using NPC;
using Pipliz;

namespace AdvancedForester
{
    [AreaJobDefinitionAutoLoader]
    public class AutummForesterDefinition : AbstractFarmAreaJobDefinition
    {
        public AutummForesterDefinition()
        {
            Identifier = "Khanx.AutummForester";
            UsedNPCType = NPCType.GetByKeyNameOrDefault("pipliz.forester");
            MaxGathersPerRun = 1;
            Stages = new ushort[] {
                ItemTypes.IndexLookup.GetIndex("sappling"),
                ItemTypes.IndexLookup.GetIndex("logtemperate")
            };
        }

        public override IAreaJob CreateAreaJob(Colony owner, Vector3Int min, Vector3Int max, bool isLoaded, int npcID = 0)
        {
            return new ForesterJob(this, owner, min, max, ItemTypes.GetType("saplingfallorange"), npcID);
        }
    }
}
