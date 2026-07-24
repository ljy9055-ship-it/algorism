using System.Collections.Generic;
using UnityEngine;

public class EquipmentDatabase : MonoBehaviour
{
    [SerializeField]
    private List<EquipmentData> equipments =
        new List<EquipmentData>();

    public EquipmentData GetEquipment(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
            return null;

        return equipments.Find(
            equipment => equipment != null &&
                         equipment.equipmentId == equipmentId
        );
    }
}