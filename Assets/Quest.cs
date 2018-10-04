public abstract class Quest {

    public string descriptionLocKey;
    public string defaultDescription;

    public abstract bool IsQuestCompleted();

    public abstract string SpecificInformation();
}
