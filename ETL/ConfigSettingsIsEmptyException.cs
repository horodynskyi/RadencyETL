namespace ETL;

public class ConfigSettingsIsEmptyException:Exception
{
    public ConfigSettingsIsEmptyException() : base("Config file is null or empty!")
    {

    }
}