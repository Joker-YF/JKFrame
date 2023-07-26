using UnityEngine;

public abstract class LocalizationDataBase
{
}
public class LocalizationStringData : LocalizationDataBase
{
    public string content;
}
public class LocalizationImageData : LocalizationDataBase
{
    public Sprite content;
}
