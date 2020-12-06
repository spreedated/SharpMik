using System;

namespace SharpMik.Attributes
{
    public class ModFileExtentionsAttribute : Attribute
    {
        public String[] FileExtentions { get; }


        public ModFileExtentionsAttribute(params String[] extentions)
        {
            FileExtentions = extentions;
        }
    }
}
