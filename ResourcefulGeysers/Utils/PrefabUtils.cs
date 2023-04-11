using System.Collections;
using UnityEngine;

namespace ResourcefulGeysers.Utils
{
    internal static class PrefabUtils
    {
        internal static GameObject LimestoneChunk;
        internal static GameObject SandstoneChunk;
        internal static GameObject ShaleChunk;

        internal static IEnumerator GetPrefabs()
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.LimestoneChunk, true);
            yield return task;
            LimestoneChunk = task.GetResult();

            task = CraftData.GetPrefabForTechTypeAsync(TechType.SandstoneChunk, true);
            yield return task;
            SandstoneChunk = task.GetResult();

            task = CraftData.GetPrefabForTechTypeAsync(TechType.ShaleChunk, true);
            yield return task;
            ShaleChunk = task.GetResult();
        }
    }
}
