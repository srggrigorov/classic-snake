using UnityEngine;

[CreateAssetMenu(fileName = "SnakeSpeedSettings", menuName = "Settings/Snake Speed")]
public class SnakeSpeedSettings : ModuleSettings
{
    [field: Min(0.01f), SerializeField]
    public float MoveDurationSlow { get; private set; }
    [field: Min(0.01f), SerializeField]
    public float MoveDurationFast { get; private set; }
}
