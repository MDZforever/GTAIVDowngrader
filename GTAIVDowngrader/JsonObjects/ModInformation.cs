﻿using System.Collections.Generic;

namespace GTAIVDowngrader.JsonObjects
{
    public class ModInformation
    {

        #region Variables
        // Type
        public bool IsASILoader;
        public bool IsScriptHook;
        public bool IsScriptHookHook;
        public bool IsScriptHookDotNet;
        public bool IsIVSDKDotNet;

        // Requirements
        public bool RequiresASILoader;
        public bool RequiresScriptHook;
        public bool RequiresScriptHookDotNet;
        public bool RequiresIVSDKDotNet;

        // Other
        public bool CompatibleWithGFWL;
        public bool ShowInDowngrader;
        public bool CheckedByDefault;

        // File Details
        public string FileName;
        public long FileSize;

        // Mod Details
        public List<ModVersion> ForVersions;
        public List<OptionalComponentInfo> OptionalComponents;

        public string Title;
        public string Description;
        public string WarningMessage;
        public string OfficialModWebPage;
        public string DownloadURL;
        #endregion

        #region Constructor
        public ModInformation()
        {
            ForVersions = new List<ModVersion>();
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Title: {0}, Desc: {1}, FileName: {2}, CheckedByDefault: {3}, DownloadURL: {4}", Title, Description, FileName, CheckedByDefault.ToString(), DownloadURL);
        }
        #endregion

    }
}
