﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StorAmp.Core {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("StorAmp.Core.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can press &quot;validate&quot; button to log in and remember your credentials.
        /// </summary>
        internal static string Aad_Validate {
            get {
                return ResourceManager.GetString("Aad_Validate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Indicates access tier for a blob. Question mark at the end indicates that access tier is inferred..
        /// </summary>
        internal static string AccessTierHint {
            get {
                return ResourceManager.GetString("AccessTierHint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Get Shared Access Signature (SAS).
        /// </summary>
        internal static string AccountCommand_GetSharedAccessSignature {
            get {
                return ResourceManager.GetString("AccountCommand_GetSharedAccessSignature", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete.
        /// </summary>
        internal static string BackgroundTask_Delete_DialogTitle {
            get {
                return ResourceManager.GetString("BackgroundTask_Delete_DialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to delete.
        /// </summary>
        internal static string BackgroundTask_Delete_TypeName {
            get {
                return ResourceManager.GetString("BackgroundTask_Delete_TypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty cell indicates to lease information. Otherwise in the following format:
        ///1. 🔑/🔓 icon - lease locked or unlocked.
        ///2. ∞/⌛ - lease duration is infinite or fixed in time.
        ///3. Lease status..
        /// </summary>
        internal static string LeaseHint {
            get {
                return ResourceManager.GetString("LeaseHint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to loading....
        /// </summary>
        internal static string LoadingBlobName {
            get {
                return ResourceManager.GetString("LoadingBlobName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data Lake (Gen 1).
        /// </summary>
        internal static string SideBar_Adls1 {
            get {
                return ResourceManager.GetString("SideBar_Adls1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Accounts.
        /// </summary>
        internal static string SideBar_AttachedAccounts {
            get {
                return ResourceManager.GetString("SideBar_AttachedAccounts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Key Vaults.
        /// </summary>
        internal static string SideBar_KeyVaults {
            get {
                return ResourceManager.GetString("SideBar_KeyVaults", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage Accounts.
        /// </summary>
        internal static string SideBar_StorageAccounts {
            get {
                return ResourceManager.GetString("SideBar_StorageAccounts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed.
        /// </summary>
        internal static string TaskManager_NFailed {
            get {
                return ResourceManager.GetString("TaskManager_NFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to no tasks running.
        /// </summary>
        internal static string TaskManager_NoTasks {
            get {
                return ResourceManager.GetString("TaskManager_NoTasks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} running.
        /// </summary>
        internal static string TaskManager_NTasks {
            get {
                return ResourceManager.GetString("TaskManager_NTasks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to task.
        /// </summary>
        internal static string TaskManager_Task {
            get {
                return ResourceManager.GetString("TaskManager_Task", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ASPX|.aspx
        ///Boo|.boo
        ///C#|.cs
        ///C++|.h,.cpp,.c,.hpp
        ///CSV|.csv
        ///CSS|.css
        ///HTML|.html
        ///Java|.java
        ///JavaScript|.js
        ///JSON|.json
        ///Markdown|.md
        ///PHP|.php
        ///PowerShell|.ps1
        ///Python|.py
        ///SQL|.sql
        ///VB|.vb
        ///XML|.xml
        ///INI|.ini,.inf,.wer,.dof
        ///YAML|.yml,.yaml.
        /// </summary>
        internal static string TextEditor_SupportedFormats {
            get {
                return ResourceManager.GetString("TextEditor.SupportedFormats", resourceCulture);
            }
        }
    }
}