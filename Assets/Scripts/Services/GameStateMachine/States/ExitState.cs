public class ExitState : State<GameManager> {
    private ISaveLoadService saveLoadService;
    public ExitState(ISaveLoadService saveLoadService)  {
        this.saveLoadService = saveLoadService;
    }

    public override void Enter() {
        saveLoadService.SaveAll();
    }

    public override void Exit() {
    }
}
