using UnityEngine;

public class MainMenuState : State<GameManager> {
    private IAudioService _audioService;
    public MainMenuState(IAudioService audioService) {
        _audioService = audioService;
    }
    public override void Enter() {
        _audioService.StartMusicPlaylist(MusicPlaylist.MainMenu);
    }
    public override void Exit() {
    }
}