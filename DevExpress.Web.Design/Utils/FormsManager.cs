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
using System.Text;
using DevExpress.Web.Design.Forms;
using Microsoft.Win32;
using DevExpress.Web.Internal;
using EnvDTE;
using System.ComponentModel.Design;
using System.Web.UI.Design;
namespace DevExpress.Web.Design {
	public abstract class FormsInfo {
		public const string DefaultVBFormsResourcePathPrefix = "Forms.VB.";
		public const string DefaultCSFormsResourcePathPrefix = "Forms.CS.";
		public string AppRelativeDirectoryPath {
			get { return string.Format(RenderUtils.DefaultFormsAppRelativeDirectoryPathTemplate, ControlName); }
		}
		public abstract string ControlName { get; }
		public abstract bool NeedCopyDesignerFileFromResource { get; }
		public abstract string[] FormNames { get; }
		public abstract Type Type { get; }
		public string RegisterDialogConfirmPath {
			get { return string.Format("Software\\Developer Express .NET\\{0}\\Designer\\FormConfirm", ControlName); }
		}
		public string GetResourcePathPrefix(IServiceProvider provider) {
			ProjectItem projectItem = provider.GetService(typeof(ProjectItem)) as ProjectItem;
			return SelectPathPrefix(DesignUtils.GetProjectLanguage(projectItem));
		}
		protected virtual string SelectPathPrefix(string language) {
			if (language == CodeModelLanguageConstants.vsCMLanguageCSharp)
				return DefaultCSFormsResourcePathPrefix;
			else
				return DefaultVBFormsResourcePathPrefix;
		}
	}
	public class FormsManager {
		FormsInfo[] formsInfoArray;
		IServiceProvider provider;
		bool needConfirmDialog = true;
		public FormsManager(FormsInfo[] formsInfoArray, bool needConfirmDialog, IServiceProvider provider) {
			this.formsInfoArray = formsInfoArray;
			this.needConfirmDialog = needConfirmDialog;
			this.provider = provider;
		}
		protected IServiceProvider Provider {
			get { return this.provider; }
		}
		protected List<string> GetRegistriedDialogConfirmProjects(FormsInfo formInfo) {
			List<string> directories = new List<string>();
			RegistryKey key = RootKey.OpenSubKey(formInfo.RegisterDialogConfirmPath, false);
			if (key != null) {
				foreach (string name in key.GetValueNames())
					directories.Add((string)key.GetValue(name));
			}
			return directories;
		}
		protected RegistryKey RootKey {
			get { return Registry.CurrentUser; }
		}
		public string[] CopyUserControlsToWebSite() {
			List<string> ret = new List<string>();
			for (int i = 0; i < formsInfoArray.Length; i++) {
				if (!IsDontAskMeAgain(formsInfoArray[i]))
					ret.AddRange(CopyUserControlsToWebSiteCore(formsInfoArray[i]));
			}
			return ret.ToArray();
		}
		protected string[] CopyUserControlsToWebSiteCore(FormsInfo formInfo) {
			List<string> ret = new List<string>();
			bool needConfirm = this.needConfirmDialog;
			for (int i = 0; i < formInfo.FormNames.Length; i++) {
				bool needCopyDefaultDialogFile = DesignUtils.IsNeedCopyDefaultDialogFile(formInfo.FormNames[i],
																formInfo.GetResourcePathPrefix(Provider),
																formInfo.AppRelativeDirectoryPath,
																formInfo.Type, provider);
				if (needCopyDefaultDialogFile) {
					if (!IsDirectoryExists(formInfo.AppRelativeDirectoryPath))
						needConfirm = false;
					if (needConfirm) {
						string dialogFormUserControlAppRelativePath = formInfo.AppRelativeDirectoryPath +
							formInfo.FormNames[i] + RenderUtils.DefaultUserControlFileExtension;
						DialogResultEx dlgRes = ExecuteFormUserControlConfirmDialog(formInfo, dialogFormUserControlAppRelativePath);
						needCopyDefaultDialogFile = (dlgRes == DialogResultEx.Yes) || (dlgRes == DialogResultEx.YesToAll);
						needConfirm = dlgRes != DialogResultEx.YesToAll;
						if (dlgRes == DialogResultEx.NoDontAskMe) {
							SaveDontAskMeAgainFlagToRegistry(formInfo);
							break;
						}
					}
					if (needCopyDefaultDialogFile)
						DesignUtils.CopyUserControlToWebSite(formInfo.FormNames[i], formInfo.GetResourcePathPrefix(Provider),
															 formInfo.AppRelativeDirectoryPath, formInfo.Type, formInfo.NeedCopyDesignerFileFromResource,
															 Provider, ret);
				}
			}
			if (!formInfo.NeedCopyDesignerFileFromResource && DesignUtils.IsWebApplication(provider) && ret.Count > 0)
				DesignUtils.ConvertUserControlsToWebApplication(formInfo.AppRelativeDirectoryPath, provider);
			return ret.ToArray();
		}
		public DialogResultEx ExecuteFormUserControlConfirmDialog(FormsInfo formInfo, string name) {
			bool dummy = false;
			return MessageBoxEx.Show(null, string.Format(StringResources.FormUserControlDialog, formInfo.ControlName, name),
				StringResources.MessageBox_DefaultCaption,
				MessageBoxButtonsEx.YesYesToAllNoNoDontAskMe, true, ref dummy);
		}
		public FormsInfo[] CopyAllUserControlsToWebSite(string messageTemplate) {
			List<FormsInfo> copiedForms = new List<FormsInfo>();
			Dictionary<String, DialogResultEx> dialogResults = new Dictionary<string, DialogResultEx>();
			foreach (FormsInfo formsInfo in formsInfoArray) {
				string controlName = formsInfo.ControlName;
				if (!dialogResults.ContainsKey(controlName)) {
					DialogResultEx dlgResult = MessageBoxEx.Show(null, string.Format(messageTemplate, formsInfo.ControlName),
						StringResources.MessageBox_DefaultCaption, MessageBoxButtonsEx.YesNo);
					dialogResults[controlName] = dlgResult;
				}
				if (dialogResults[controlName] == DialogResultEx.Yes) {
					DesignUtils.CopyUserControlsToWebSite(formsInfo.FormNames, formsInfo.GetResourcePathPrefix(Provider),
														  formsInfo.AppRelativeDirectoryPath, formsInfo.Type,
														  Provider, formsInfo.NeedCopyDesignerFileFromResource);
					copiedForms.Add(formsInfo);
				}
			}
			return copiedForms.ToArray();
		}
		public FormsInfo[] GenerateDefaultUserControlsToWebSite() {
			string textTemplate = GetCopyDefaultDialogFormsToTheProjectTextTemplate();
			return CopyAllUserControlsToWebSite(textTemplate);
		}
		protected virtual string GetCopyDefaultDialogFormsToTheProjectTextTemplate() {
			return StringResources.CopyDefaultDialogFormsToTheProjectText;
		}
		private bool IsDontAskMeAgain(FormsInfo formsInfo) {
			return GetRegistriedDialogConfirmProjects(formsInfo).IndexOf(GetDontAskMeAgainFlag()) != -1;
		}
		protected void SaveDontAskMeAgainFlagToRegistry(FormsInfo formsInfo) {
			string flagValue = GetDontAskMeAgainFlag();
			if (GetRegistriedDialogConfirmProjects(formsInfo).IndexOf(flagValue) == -1) {
				RegistryKey key = RootKey.OpenSubKey(formsInfo.RegisterDialogConfirmPath, true);
				if (key == null)
					key = RootKey.CreateSubKey(formsInfo.RegisterDialogConfirmPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
				if (key != null) {
					string[] names = key.GetValueNames();
					string name = GetUniqueRegistryName(names);
					key.SetValue(name, flagValue);
					key.Close();
				}
			}
		}
		private string GetUniqueRegistryName(string[] names) {
			List<int> indexes = new List<int>();
			foreach (string name in names) {
				int index;
				if (int.TryParse(name, out index))
					indexes.Add(index);
			}
			if (indexes.Count == 0)
				return 0.ToString();
			else {
				indexes.Sort();
				return (indexes[indexes.Count - 1] + 1).ToString();
			}
		}
		private string GetDontAskMeAgainFlag() {
			return AssemblyInfo.Version + "-" + DesignUtils.GetProjectName(Provider);
		}
		private bool IsDirectoryExists(string appRelativeDirectoryPath) {
			IWebApplication webApplication = (IWebApplication)Provider.GetService(typeof(IWebApplication));
			return EnvDTEHelper.IsExistProjectItem(appRelativeDirectoryPath, webApplication);
		}
		public bool IsProjectSupported() {
			return provider.GetService(typeof(ProjectItem)) != null;
		}
	}
}
