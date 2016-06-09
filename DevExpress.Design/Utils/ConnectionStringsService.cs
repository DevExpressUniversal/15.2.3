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
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
namespace DevExpress.Utils.Design {
	using System.Configuration;
	using System.IO;	
	using DevExpress.Data.Utils;
	public class ConnectionStringsService : IConnectionStringsService {
		static object nullObject = new object();
		ConnectionStringSettings[] difference = null;
		object savedDataDirectory = nullObject;
		IServiceProvider serviceProvider;
		public ConnectionStringsService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		void IConnectionStringsService.PatchConnection() {
			System.Diagnostics.Debug.Assert(difference == null, "Missing RestoreConnection method invocation");
			ProjectItem projectItem = (ProjectItem)serviceProvider.GetService(typeof(ProjectItem));
			if(projectItem == null)
				return;
			Project project = projectItem.ContainingProject;
			if(project == null)
				return;
			string dataDirectory = GetDataDirectory(project, serviceProvider);
			if(!string.IsNullOrEmpty(dataDirectory)) {
				savedDataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
				AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
			}
			ProjectItem configItem = FindProjectItem(project, delegate(ProjectItem item) {
				string fileName = GetPropertyValue(item, "FileName").ToLower();
				return fileName == "app.config" || fileName == "web.config";
			});
			if(configItem == null)
				return;
			try {
				ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
				fileMap.ExeConfigFilename = GetPropertyValue(configItem, "FullPath");
				Configuration fileCnf = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
				Configuration cnf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				difference = GetDifference(fileCnf.ConnectionStrings.ConnectionStrings, cnf.ConnectionStrings.ConnectionStrings);
				AddConnectionStrings(cnf.ConnectionStrings.ConnectionStrings, difference);
				cnf.Save();
				ConfigurationManager.RefreshSection("connectionStrings");
			} catch { }
		}
		string GetDataDirectory(Project project, IServiceProvider serviceProvider) {
			ProjectItem appDataItem = FindProjectItem(project, delegate(ProjectItem item) {
				return item.Name == "App_Data";
			});
			IDTEService dteService = (IDTEService)serviceProvider.GetService(typeof(IDTEService));
			return appDataItem != null && dteService != null ?
				Path.Combine(dteService.ProjectFullName, appDataItem.Name) :
				dteService != null ? dteService.ProjectFullName :
				string.Empty;
		}
		static ProjectItem FindProjectItem(Project project, Predicate<ProjectItem> predicate) {
			foreach(ProjectItem item in project.ProjectItems) {
				if(item != null && predicate(item))
					return item;
			}
			return null;
		}
		static string GetPropertyValue(ProjectItem projectItem, string propertyName) {
			try {
				foreach(Property prop in projectItem.Properties)
					if(prop.Name == propertyName)
						return (string)prop.Value;
			} catch { }
			return String.Empty;
		}
		void IConnectionStringsService.RestoreConnection() {
			if(difference != null) {
				Configuration cnf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				RemoveConnectionStrings(cnf.ConnectionStrings.ConnectionStrings, difference);
				cnf.Save();
				ConfigurationManager.RefreshSection("connectionStrings");
				difference = null;
			}
			if(savedDataDirectory != nullObject) {
				AppDomain.CurrentDomain.SetData("DataDirectory", savedDataDirectory);
				savedDataDirectory = nullObject;
			}
		}
		static ConnectionStringSettings[] GetDifference(ConnectionStringSettingsCollection src1, ConnectionStringSettingsCollection src2) {
			List<ConnectionStringSettings> connectionStrings = new List<ConnectionStringSettings>();
			foreach(ConnectionStringSettings sets in src1)
				if(src2[sets.Name] == null)
					connectionStrings.Add(sets);
			return connectionStrings.ToArray();
		}
		static void AddConnectionStrings(ConnectionStringSettingsCollection dest, ConnectionStringSettings[] src) {
			foreach(ConnectionStringSettings sets in src)
				dest.Add(sets);
		}
		static void RemoveConnectionStrings(ConnectionStringSettingsCollection dest, ConnectionStringSettings[] src) {
			foreach(ConnectionStringSettings sets in src)
				dest.Remove(sets);
		}
	}
}
