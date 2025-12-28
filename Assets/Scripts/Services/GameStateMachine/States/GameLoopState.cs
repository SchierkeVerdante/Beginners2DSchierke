using UnityEngine;

public class GameLoopState : State<GameManager> {
    private IAudioService _audioService;

    public GameLoopState(IAudioService audioService) {
        _audioService = audioService;
    }

    public override void Enter() {
        _audioService.StartMusicPlaylist(MusicPlaylist.GameLoop);
    }


    public override void Exit() {
    }
}


