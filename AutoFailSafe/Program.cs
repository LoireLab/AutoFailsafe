using BlockEntities.Implementations;
using Chatting;
using Shared.Notifications;

namespace Loirelab.AutoFailsafe
{
    [ModLoader.ModManager]
    public static class Main
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, "com.Loirelab.AutoFailsafe.onassemblyloaded")]
        public static void OnAssemblyLoaded(string path)
        {
             Pipliz.Log.Write("AutoFailsafe is loaded and ready to purge");
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnNPCHit, "Loirelab.AutoFailsafe.OnNPCHit")]
        public static void OnNPCHit(NPC.NPCBase poorGuy, ModLoader.OnHitData beatingData)
        {
            //Pipliz.Log.Write($"{poorGuy.Job.NPCType} is being beated... {beatingData.HitSourceType} ");
        if (beatingData.HitSourceType == ModLoader.OnHitData.EHitSourceType.Monster) 
        {
            //Pipliz.Log.Write("Monster attacked. Let's see if you have a failsafe...");
                Colony colony = poorGuy.Colony;
                ColonyGroup group = colony.ColonyGroup;

                if (group.SanctifiedPoints <= 0)
                    {
                        return;
                    }

                for (int i = 0; i < group.Colonies.Count; i++)
                    {
                        if (group.Colonies.GetAt(i).MonsterSpawnData.NextHPReduction != 0f)
                        {
                            return;
                        }
                    }

            if (ServerManager.BlockEntityCallbacks.TryGetAutoLoadedInstance<Failsafes>(out Failsafes tracker))
                {
                    var bannerPosition = colony.GetClosestBanner(poorGuy.Position).Position;
                    if (tracker.Positions.TryGetClosest(bannerPosition, out Pipliz.Vector3Int vector3Failsafe, out Failsafes.FailsafeInstance closestFailsafe) && closestFailsafe.OwnerColony.ColonyGroup == group && group.SanctifiedPoints > 0)
                    {

                        Failsafes.TriggerFailsafe(group, poorGuy.Position, closestFailsafe);
                        //Pipliz.Log.Write($"Failsafe was automaticly triggered in {colony.Name}. Deus Vult!");
                        Chat.Send(group.Owners, $"Failsafe was automaticly triggered in {colony.Name}. Deus Vult!");
                    }
                //else Pipliz.Log.Write($"Forces above found your faith lacking. Failsafe: {closestFailsafe}, OwnerColonyGroup: {closestFailsafe?.OwnerColony?.ColonyGroup}, Group: {group}, SanctifiedPoints: {group?.SanctifiedPoints}");
            }
        }
        }
    }
    
}

