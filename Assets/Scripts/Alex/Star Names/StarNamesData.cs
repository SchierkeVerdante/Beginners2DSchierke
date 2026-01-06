using System;

[Serializable]
[DataSource(DataSourceType.Resources, "star_names")]
public class StarNamesData {
    public string[] starNames;
}