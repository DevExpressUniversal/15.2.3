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
using System.Configuration;
using System.IO;
using DevExpress.XtraReports.Service.ConfigSections.Native;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
namespace DevExpress.XtraReports.Service.Design {
	class ReportTemplateWizard : IWizard {
		DTE dte;
		Project project;
		string dbFile = ConfigurationDefaultConstants.ReportServiceDbFileName;
		Dictionary<string, string> replacementsDictionary;
		string RootNamespace {
			get {
				string rootNamespace;
				replacementsDictionary.TryGetValue("$rootnamespace$", out rootNamespace);
				return rootNamespace;
			}
		}
		string ServiceItemName {
			get {
				string serviceItemName;
				if(!replacementsDictionary.TryGetValue("$rootname$", out serviceItemName))
					return null;
				serviceItemName = Path.GetFileNameWithoutExtension(serviceItemName);
				if(serviceItemName.Length > 0 && !char.IsLetter(serviceItemName, 0))
					serviceItemName = '_' + serviceItemName;
				return serviceItemName;
			}
		}
		string ProjectDirectory {
			get {
				return project != null
					? Path.GetDirectoryName(project.FullName)
					: string.Empty;
			}
		}
		#region IWizard Members
		public virtual void BeforeOpeningFile(ProjectItem projectItem) {
		}
		public virtual void ProjectFinishedGenerating(Project project) {
		}
		public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem) {
			SafeAssignProject(projectItem.ContainingProject);
			SafeAssignDTE(projectItem.DTE);
		}
		public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
			this.replacementsDictionary = replacementsDictionary;
			SafeAssignDTE(automationObject as DTE);
		}
		public virtual void RunFinished() {
			string path = Path.Combine(ProjectDirectory, "web.config");
			if(!File.Exists(path)) {
				path = Path.Combine(ProjectDirectory, "app.config");
				if(!File.Exists(path))
					return;
			}
			var fileMap = new ExeConfigurationFileMap();
			Configuration configuration = ConfigurationManager.OpenMachineConfiguration();
			fileMap.MachineConfigFilename = configuration.FilePath;
			fileMap.ExeConfigFilename = path;
			try {
				Configuration projectConfiguration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
				var configModifier = new ConfigModifier(project, projectConfiguration, dbFile, RootNamespace, ServiceItemName);
				configModifier.Modify();
			} catch(ConfigurationException) {
			}
		}
		public virtual bool ShouldAddProjectItem(string filePath) {
			if(dte != null && filePath.Equals(dbFile, StringComparison.InvariantCultureIgnoreCase)) {
				string path = Path.Combine(ProjectDirectory, "App_Data", dbFile);
				return dte.Solution.FindProjectItem(path) == null;
			}
			return true;
		}
		#endregion
		void SafeAssignProject(Project project) {
			if(this.project == null && project != null) {
				this.project = project;
			}
		}
		void SafeAssignDTE(DTE dte) {
			if(this.dte == null && dte != null) {
				this.dte = dte;
			}
		}
	}
}
