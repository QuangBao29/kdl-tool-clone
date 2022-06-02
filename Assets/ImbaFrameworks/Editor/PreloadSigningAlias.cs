using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class PreloadSigningAlias
{

	static PreloadSigningAlias ()
	{
		#if UNITY_ANDROID
		PlayerSettings.Android.keystorePass = "Pw$suga@123";
		PlayerSettings.Android.keyaliasName = "release";
		PlayerSettings.Android.keyaliasPass = "Pw$suga@deathrace";
		#endif
	}

}