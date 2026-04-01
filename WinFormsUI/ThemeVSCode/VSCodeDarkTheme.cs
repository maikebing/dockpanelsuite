namespace WeifenLuo.WinFormsUI.Docking
{
    using ThemeVSCode;

    /// <summary>
    /// Visual Studio Code dark theme.
    /// </summary>
    public class VSCodeDarkTheme : VSCodeThemeBase
    {
        public VSCodeDarkTheme()
            : base(Decompress(VSCodeThemeResources.LoadVSCodeDarkTheme()))
        {
        }
    }
}
