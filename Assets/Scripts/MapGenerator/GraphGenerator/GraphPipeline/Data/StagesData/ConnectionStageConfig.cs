using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConnectionStageConfig", menuName = "Graph Generation/Stages/Connection Config")]
public class ConnectionStageConfig : GenericInstanceConfig<ConnectionStage> {

    [Min(min: 0f)]
    public float MaxConnectionDistance = 1.5f;

    [Min(min: 1)]
    public int MaxConnectionsPerNode = 50;

    [Range(0f, 1f)]
    public float RandomConnectionChance = 0.3f;

    [Min(min: 1)]
    public int ConnectionCandidatesCount = 3;
   

    protected override void OnValidate() {
        base.OnValidate();
        if (MaxConnectionsPerNode < 1) {
            MaxConnectionsPerNode = 1;
        }
        if (MaxConnectionDistance < 0f) {
            MaxConnectionDistance = 0f;
        }

        if (RandomConnectionChance < 0f) {
            RandomConnectionChance = 0f;
        } else if (RandomConnectionChance > 1f) {
            RandomConnectionChance = 1f;
        }
    }

}
