﻿using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace Netherlands3D.Utilities
{
    public class SwapBuildTarget : MonoBehaviour
    {
        public static void DataTarget() { }

        private const string webGlBuildPrefix = "BuildWebGL_";
        private const string desktopBuildPrefix = "BuildDesktop_";

        private const string branchBuildsFolder = "BranchBuilds";

        private const string consistentBuildFolderName = "3DAmsterdamWebGL";

        [MenuItem("Netherlands 3D/Set data target/Production")]
        public static void SwitchBranchMaster()
        {
            PlayerSettings.bundleVersion = "production"; //The place to assign release versioning
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, "PRODUCTION");
            Debug.Log("Set scripting define symbols to PRODUCTION");
        }
        [MenuItem("Netherlands 3D/Set data target/Development")]
        public static void SwitchBranchDevelop()
        {
            PlayerSettings.bundleVersion = "develop";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, "DEVELOPMENT");
            Debug.Log("Set scripting define symbols to DEVELOPMENT");
        }
        [MenuItem("Netherlands 3D/Set data target/Specific feature")]
        public static void SwitchBranchFeature()
        {
            PlayerSettings.bundleVersion = ReadGitHead();

            Debug.Log("Version set to feature name: " + Application.version);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, "DEVELOPMENT_FEATURE");
            Debug.Log("Set scripting define symbols to DEVELOPMENT_FEATURE");
        }

        private static string ReadGitHead()
        {
            var gitHeadFile = Application.dataPath + "/../../.git/HEAD";
            var headLine = File.ReadAllText(gitHeadFile);
            Debug.Log("Reading git HEAD file:" + headLine);

            headLine = headLine.Replace("feature/", "feature-");
            var positionLastSlash = headLine.LastIndexOf("/") + 1;
            var headName = headLine.Substring(positionLastSlash, headLine.Length - positionLastSlash);

            Debug.Log("Got head name: " + headName);

            return headName;
        }

        [MenuItem("Netherlands 3D/Build by feature name")]
        public static void BuildWebGL()
        {
            TargetedBuild(BuildTarget.WebGL);
        }


        public static void TargetedBuild(BuildTarget buildTarget = BuildTarget.WebGL)
        {
            var gitHeadName = ReadGitHead();
            var headNameWithoutControlCharacters = new string(gitHeadName.Where(c => !char.IsControl(c)).ToArray());

            var buildMainName = $"{headNameWithoutControlCharacters}_{DateTime.Now.Ticks}";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(),
                target = buildTarget,
                locationPathName = $"../../{branchBuildsFolder}/{buildMainName}/{consistentBuildFolderName}/"
            };

            Debug.Log("Building to: " + buildPlayerOptions.locationPathName);

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary buildSummary = report.summary;
            if (buildSummary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build " + buildSummary.outputPath + " succeeded: " + buildSummary.totalSize + " bytes");
                ZipAndDeploy(buildMainName, buildSummary);
            }

            if (buildSummary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }

        private static void ZipAndDeploy(string mainName, BuildSummary buildSummary)
        {
            var zipFilePath = $"{buildSummary.outputPath}../{mainName.Replace("feature-", "")}.zip";

            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
            ZipFile.CreateFromDirectory(buildSummary.outputPath, zipFilePath);
            Debug.Log("Zipped build in: " + zipFilePath);

            DeployZipFile(zipFilePath);
        }

        private static void DeployZipFile(string zipFilePath)
        {
            var autodeployBatchFile = Path.GetDirectoryName(zipFilePath) + "/../../deploy.bat";
            Debug.Log($"Checking if we have an autodeploy file at: {autodeployBatchFile}");

            if (File.Exists(autodeployBatchFile))
            {
                Debug.Log("Autodeploying");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                startInfo.FileName = autodeployBatchFile;
                startInfo.Arguments = Path.GetFileNameWithoutExtension(zipFilePath);
                process.StartInfo = startInfo;
                process.Start();
            }
        }
    }
}