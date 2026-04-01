namespace WeifenLuo.WinFormsUI.ThemeVSCode
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class VSCodeThemeResources
    {
        private const string VSCodeDarkThemeResourceName = "WeifenLuo.WinFormsUI.ThemeVSCode.Resources.vscodedark.vstheme.gz";

        internal static byte[] LoadVSCodeDarkTheme()
        {
            Assembly assembly = typeof(VSCodeThemeResources).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(VSCodeDarkThemeResourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Unable to load the embedded VSCode theme resource.");

                using (MemoryStream buffer = new MemoryStream())
                {
                    stream.CopyTo(buffer);
                    return buffer.ToArray();
                }
            }
        }
    }
}
