namespace WeifenLuo.WinFormsUI.Docking
{
    using ThemeVSCode;

    /// <summary>
    /// Visual Studio Code light theme.
    /// </summary>
    public class VSCodeLightTheme : VSCodeThemeBase
    {
        public VSCodeLightTheme()
            : base(VSCodeThemeResources.LoadVSCodeLightTheme())
        {
        }
    }
}
