#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.ComponentModel;
using System.Configuration;
using System.Web.UI.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Web.Native;
using EnvDTE;
using Configuration = System.Configuration.Configuration;
namespace DevExpress.Web.Design.Reports.Viewer {
	public class FileStore {
		object tempDirectory;
		object checkInterval;
		object keepInterval;
		IServiceProvider provider;
		[NotifyParentProperty(true)]
		public string Directory {
			get { return XRFileStore.GetTempDirectory(GetValue(XRFileStore.directoryCfgKey)); }
			set { tempDirectory = value; }
		}
		[NotifyParentProperty(true)]
		public int CheckInterval {
			get { return (int)XRFileStore.ReadInterval(GetValue(XRFileStore.checkIntervalCfgKey), XRFileStore.defaultCheckInterval).TotalMilliseconds; }
			set { checkInterval = value; }
		}
		[NotifyParentProperty(true)]
		public long KeepInterval {
			get { return (long)XRFileStore.ReadInterval(GetValue(XRFileStore.keepIntervalCfgKey), XRFileStore.defaultKeepInterval).TotalMilliseconds; }
			set { keepInterval = value; }
		}
		public void SetServiceProvider(IServiceProvider provider) {
			this.provider = provider;
		}
		public void SaveNewValues() {
			if(provider == null) {
				return;
			}
			var webApplication = provider.GetService<IWebApplication>();
			Configuration configuration = webApplication.OpenWebConfiguration(false);
			SaveValue(configuration, XRFileStore.directoryCfgKey, tempDirectory);
			SaveValue(configuration, XRFileStore.checkIntervalCfgKey, checkInterval);
			SaveValue(configuration, XRFileStore.keepIntervalCfgKey, keepInterval);
			ForceSaveWebConfig(provider);
		}
		public override string ToString() {
			return string.Empty;
		}
		static void SaveValue(Configuration configuration, string name, object value) {
			if(value == null)
				return;
			KeyValueConfigurationCollection appSettings = configuration.AppSettings.Settings;
			if(Array.IndexOf(appSettings.AllKeys, name) != -1) {
				appSettings.Remove(name);
				configuration.Save();
			}
			appSettings.Add(name, value.ToString());
			configuration.Save();
		}
		string GetValue(string key) {
			if(provider == null) {
				return null;
			}
			var webApplication = provider.GetService<IWebApplication>();
			if(webApplication == null) {
				return null;
			}
			KeyValueConfigurationElement valueContainer = webApplication.OpenWebConfiguration(true).AppSettings.Settings[key];
			if(valueContainer == null) {
				return null;
			}
			return valueContainer.Value;
		}
		static void ForceSaveWebConfig(IServiceProvider provider) {
			var webConfigProjectItem = EnvDTEHelper.GetProjectItemByPath("/web.config", provider) as ProjectItem;
			if(webConfigProjectItem != null) {
				if(!webConfigProjectItem.Saved)
					webConfigProjectItem.Save();
			}
		}
	}
}
