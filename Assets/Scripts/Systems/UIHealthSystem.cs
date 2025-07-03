using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class UIHealthSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<UIHealthData>();
    }

    protected override void OnUpdate()
    {
        var healthData = SystemAPI.GetSingleton<UIHealthData>();
        var uiDocument = Object.FindObjectOfType<UIDocument>();
        
        if (uiDocument != null)
        {
            var healthLabel = uiDocument.rootVisualElement.Q<Label>("healthLabel");
            if (healthLabel != null)
            {
                healthLabel.text = $"Health: {healthData.CurrentHealth}/{healthData.MaxHealth}";
            }
        }
    }
}