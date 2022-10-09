using AI;
using BlockTypes;
using Jobs;
using Newtonsoft.Json.Linq;
using NPC;
using Pipliz;
using Shared;
using System.Collections.Generic;

namespace AdvancedForester
{
    public class ForesterJob : AbstractAreaJob
    {
        // store treeLocation separately from positionSub because the farmer will move next to these positions(they're not equal)
        protected Vector3Int treeLocation = Vector3Int.invalidPos;
        protected ItemTypes.ItemType saplingTree = null;

		private int MaxGathersPerRun = 1;
		private ItemTypes.EFertileState RequiredFertility = ItemTypes.EFertileState.Normal;

        private static List<ItemTypes.ItemTypeDrops> GatherResults = new List<ItemTypes.ItemTypeDrops>();
        private static ItemTypes.ItemType[] yTypesBuffer = new ItemTypes.ItemType[5]; // max 3 Y + 1 below + 1 above

        private static readonly Vector3Int[] LeaveOffsets = new Vector3Int[9]
        {
            new Vector3Int(1, 3, 1),
            new Vector3Int(1, 3, 0),
            new Vector3Int(1, 3, -1),
            new Vector3Int(0, 3, -1),
            new Vector3Int(-1, 3, -1),
            new Vector3Int(-1, 3, 0),
            new Vector3Int(-1, 3, 1),
            new Vector3Int(0, 3, 1),
            new Vector3Int(0, 4, 0)
        };

        private static readonly Vector3Int[] LogOffsets = new Vector3Int[4]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(0, 3, 0)
        };

        public ForesterJob(AbstractAreaJobDefinition def, Colony owner, Vector3Int min, Vector3Int max, ItemTypes.ItemType sapling) : base(def, owner, min, max, null)
        {
            saplingTree = sapling;
            base.IsDirty = true;
        }

        public ForesterJob(AbstractAreaJobDefinition def, Colony owner, Vector3Int min, Vector3Int max, ItemTypes.ItemType sapling, NPCID? npcID) : base(def, owner, min, max, npcID)
        {
            saplingTree = sapling;
        }

		//Original code of the game, Thanx ZUN for providing
		public override void CalculateSubPosition()
		{
			ThreadManager.AssertIsMainThread();

			Vector3Int min = Minimum;
			Vector3Int max = Maximum;
			int ySize = max.y - min.y + 1;

			for (int x = min.x + 1; x < max.x; x += 3)
			{
				for (int z = min.z + 1; z < max.z; z += 3)
				{
					if (!World.TryGetColumn(new Vector3Int(x, min.y - 1, z), ySize + 2, yTypesBuffer))
					{
						goto DUMB_RANDOM;
					}

					for (int y = 0; y < ySize; y++)
					{
						ItemTypes.ItemType typeBelow = yTypesBuffer[y];
						ItemTypes.ItemType type = yTypesBuffer[y + 1];
						ItemTypes.ItemType typeAbove = yTypesBuffer[y + 2];

						if (
							(type == BuiltinBlocks.Types.air || type.HasBehaviour("log"))
							&& (typeAbove == BuiltinBlocks.Types.air || typeAbove.HasBehaviour("log"))
							&& ((RequiredFertility & typeBelow.BlockFertility) == RequiredFertility)
						)
						{
							treeLocation = new Vector3Int(x, min.y + y, z);
							if (AI.PathingManager.TryGetClosestPositionWorldNotAt(treeLocation, NPC.Position, out bool canStand, out Vector3Int position))
							{
								if (canStand)
								{
									positionSub = position;
								}
								else
								{
									positionSub = Vector3Int.invalidPos;
								}
							}
							else
							{
								positionSub = Vector3Int.invalidPos;
							}
							return;
						}
					}
				}
			}

			for (int i = 0; i < 15; i++)
			{
				// give the random positioning 5 chances to become valid
				int testX = min.x + Random.Next(0, (max.x - min.x - 2) / 3 + 1) * 3;
				int testZ = min.z + Random.Next(0, (max.z - min.z - 2) / 3 + 1) * 3;

				if (!World.TryGetColumn(new Vector3Int(testX, min.y - 1, testZ), ySize + 2, yTypesBuffer))
				{
					goto DUMB_RANDOM;
				}

				for (int y = 0; y < ySize; y++)
				{
					ItemTypes.ItemType typeBelow = yTypesBuffer[y];
					ItemTypes.ItemType type = yTypesBuffer[y + 1];
					ItemTypes.ItemType typeAbove = yTypesBuffer[y + 2];

					if (typeBelow.PathingImpactCanStandOn
						&& type.PathingImpactAsAir
						&& typeAbove.PathingImpactAsAir)
					{
						positionSub = new Vector3Int(testX, min.y + y, testZ);
						treeLocation = Vector3Int.invalidPos;
						return;
					}
				}
			}

		DUMB_RANDOM:
			treeLocation = Vector3Int.invalidPos;
			positionSub = min.Add(
				Random.Next(0, (max.x - min.x) / 3) * 3,
				(max.y - min.y) / 2,
				Random.Next(0, (max.z - min.z) / 3) * 3
			);
		}

		//Original code of the game, Thanx ZUN for providing
		public override void OnNPCAtJob(ref NPCBase.NPCState state)
		{
			ThreadManager.AssertIsMainThread();
			state.JobIsDone = true;
			positionSub = Vector3Int.invalidPos;
			if (!treeLocation.IsValid)
			{ // probably idling about
				state.SetCooldown(8.0, 16.0);
				return;
			}

			ItemTypes.ItemType type;
			if (!World.TryGetTypeAt(treeLocation, out type))
			{
				state.SetCooldown(8.0, 12.0);
				return;
			}

			if (type.HasBehaviour("log"))
			{
				GatherResults.Clear();
				ItemTypes.ItemType lastRemovedType;
				if (ChopTree())
				{
					float cd = Random.NextFloat(8f, 12f);
					if (lastRemovedType != null)
					{
						state.SetIndicator(Shared.IndicatorState.NewItemIndicator(cd, BuiltinBlocks.Indices.logtemperate));
					}
					else
					{
						state.SetCooldown(cd);
					}

					AudioManager.SendAudio(treeLocation.Vector, "woodDeleteHeavy");

					ModLoader.Callbacks.OnNPCGathered.Invoke(this, treeLocation, GatherResults);

					NPC.Inventory.Add(GatherResults);
					GatheredItemsCount++;
					if (GatheredItemsCount >= MaxGathersPerRun)
					{
						shouldDumpInventory = true;
						GatheredItemsCount = 0;
					}
				}
				else
				{
					state.SetCooldown(3.0, 6.0);
				}
				return;

				bool ChopTree()
				{
					lastRemovedType = default;
					for (int i = 0; i < LeaveOffsets.Length; i++)
					{
						if (World.TryGetTypeAt(treeLocation + LeaveOffsets[i], out ItemTypes.ItemType foundType) && foundType.HasBehaviour("leaves"))
						{
							switch (ServerManager.TryChangeBlock(treeLocation + LeaveOffsets[i], foundType, BuiltinBlocks.Types.air, Owner))
							{
								case EServerChangeBlockResult.Success:
									lastRemovedType = foundType;
									GatherResults.Add(lastRemovedType.OnRemoveItems);
									break;
								case EServerChangeBlockResult.CancelledByCallback:
								case EServerChangeBlockResult.ChunkNotReady:
									return false;
								default:
									break;
							}
						}
					}
					for (int i = 0; i < LogOffsets.Length; i++)
					{
						if (World.TryGetTypeAt(treeLocation + LogOffsets[i], out ItemTypes.ItemType foundType) && foundType.HasBehaviour("log"))
						{
							switch (ServerManager.TryChangeBlock(treeLocation + LogOffsets[i], foundType, BuiltinBlocks.Types.air, Owner))
							{
								case EServerChangeBlockResult.Success:
									lastRemovedType = foundType;
									GatherResults.Add(lastRemovedType.OnRemoveItems);
									break;
								case EServerChangeBlockResult.CancelledByCallback:
								case EServerChangeBlockResult.ChunkNotReady:
									return false;
								default:
									break;
							}
						}
					}
					return true;
				}
			}
			else if (type == BuiltinBlocks.Types.air)
			{
				// maybe plant sapling?
				if (World.TryGetTypeAt(treeLocation.Add(0, -1, 0), out ItemTypes.ItemType typeBelow))
				{
					if ((RequiredFertility & typeBelow.BlockFertility) == RequiredFertility)
					{
						ServerManager.TryChangeBlock(treeLocation, BuiltinBlocks.Types.air, BuiltinBlocks.Types.sapling, Owner, ESetBlockFlags.DefaultAudio);
						state.SetCooldown(1.5, 2.5);
						return;
					}
				}
				else
				{
					state.SetCooldown(8.0, 12.0);
					return;
				}

			}

			// something unexpected
			state.SetCooldown(8.0, 16.0);
			return;
		}

	}
}
