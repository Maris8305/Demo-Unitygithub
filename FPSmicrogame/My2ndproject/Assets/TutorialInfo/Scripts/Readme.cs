using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ReadmeData", menuName = "Tutorial/Readme Data")]
public class ReadmeTutorial : ScriptableObject
{
    public Texture2D icon;
    public string title;
    public Section[] sections;
    public bool loadedLayout;

    [System.Serializable]
    public class Section
    {
        public string heading;
        public string text;
        public string linkText;
        public string url;
    }
}

