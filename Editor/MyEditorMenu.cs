public static class LightRpEditorMenu
{
	[Menu( "Editor", "LightRP/About" )]
	public static void OpenAbout()
	{
		EditorUtility.DisplayDialog(
			"LightRP",
			"A simple, easily editable RP framework for S&Box.\nInspired by DarkRP." );
	}
}
