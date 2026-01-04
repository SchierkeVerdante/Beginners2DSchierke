using UnityEngine;
using Zenject;

public class AudioInstaller : MonoInstaller {
    [SerializeField] private AudioStateConfig audioConfig;

    public override void InstallBindings() {
        Container.Bind<AudioSystem>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<FmodAudioService>()
            .AsSingle()
            .WithArguments(audioConfig);
    }

}