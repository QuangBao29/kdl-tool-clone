using UnityEngine;
using UnityEditor;
using System;

//Sets our settings for all new Models and Textures upon first import
public class ImportSettings : AssetPostprocessor
{
	//public const float importScale = 1.0f;
	
	void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		
		importer.globalScale  = 1f;
		importer.materialImportMode = ModelImporterMaterialImportMode.None;
		importer.generateAnimations = ModelImporterGenerateAnimations.None;
		importer.importAnimation = false;
	}
}