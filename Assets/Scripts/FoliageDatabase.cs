using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FoliageDatabase", menuName = "Foliage/Foliage Database")]
public class FoliageDatabase : ScriptableObject
{
    public List<FoliageType> foliageTypes;
}
