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
using Microsoft.SharePoint;
using System.IO;
using DevExpress.Web.Internal;
using Microsoft.SharePoint.Administration;
using System.Globalization;
using DevExpress.Web;
namespace DevExpress.SharePoint {
	public abstract class HtmlEditorFeatureReceiver : SPFeatureReceiver {
		const string OwnerName = "DeveloperExpress";
		const string AddModuleTemplate = "<add name=\"{0}\" type=\"{1}\"/>";
		const string AddUploadControlHandlerTemplate = "<add name=\"{0}\" verb=\"{1}\" path=\"{2}\"  type=\"{3}\" preCondition=\"{4}\"/>";
		public HtmlEditorFeatureReceiver()
			: base() {
		}
		protected abstract string GetTemplateName();
		public override void FeatureActivated(SPFeatureReceiverProperties properties) {
			FeatureActivatedCore(properties);
		}
		public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
			FeatureDeactivatingCore(properties);
		}
		public override void FeatureInstalled(SPFeatureReceiverProperties properties) {
			AddDxHttpModuleAndHandler();
		}
		public override void FeatureUninstalling(SPFeatureReceiverProperties properties) {
			DeleteDxHttpModuleAndHandler();
		}
		protected void FeatureActivatedCore(SPFeatureReceiverProperties properties) {
			try {
				int sharePointMajorVersion = properties.Definition.Farm.BuildVersion.Major;
				SPSecurity.RunWithElevatedPrivileges(delegate() {
					File.Copy(properties.Definition.RootDirectory + @"\" + GetTemplateName(),
						GetTemplatePhysicalPath(sharePointMajorVersion), true);
				});
			} catch (Exception) { }
		}
		protected void FeatureDeactivatingCore(SPFeatureReceiverProperties properties) {
			try {
				SPSecurity.RunWithElevatedPrivileges(delegate() {
					int sharePointMajorVersion = properties.Definition.Farm.BuildVersion.Major;
					File.Delete(GetTemplatePhysicalPath(sharePointMajorVersion));
				});
			} catch(Exception) { }
		}
		protected string GetTemplatePhysicalPath(int sharePointMajorVersion) {
			return GetTemplateSharePointFolder(sharePointMajorVersion) + GetTemplateName();
		}
		protected string GetTemplateSharePointFolder(int sharePointMajorVersion) {
			return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) +
				string.Format(@"\Microsoft Shared\web server extensions\{0}\TEMPLATE\CONTROLTEMPLATES\", sharePointMajorVersion);
		}
		protected void AddDxHttpModuleAndHandler() {
			SPWebService service = SPWebService.ContentService;
			SPWebConfigModification modification = new SPWebConfigModification();
			modification.Path = ConfigurationSectionNames.ConfigurationSectionName + "/" + ConfigurationSectionNames.HttpModuleIIS7Section;
			modification.Name = string.Format("add[@name='{0}']", HttpUtils.HttpHandlerModuleName);
			modification.Sequence = 0;
			modification.Owner = OwnerName;
			modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
			modification.Value = string.Format(AddModuleTemplate, HttpUtils.HttpHandlerModuleName, typeof(ASPxHttpHandlerModule).AssemblyQualifiedName);
			service.WebConfigModifications.Add(modification);
			modification = new SPWebConfigModification();
			modification.Path = ConfigurationSectionNames.ConfigurationSectionName + "/" + ConfigurationSectionNames.HttpHandlerIIS7Section;
			modification.Name = string.Format("add[@name='{0}']", HttpUtils.UploadProgressHttpHandlerName);
			modification.Sequence = 0;
			modification.Owner = OwnerName;
			modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
			modification.Value = string.Format(AddUploadControlHandlerTemplate, HttpUtils.UploadProgressHttpHandlerName, "GET,POST",
				HttpUtils.UploadProgressHttpHandlerPagePath, typeof(DevExpress.Web.ASPxUploadProgressHttpHandler).AssemblyQualifiedName, "integratedMode");
			service.WebConfigModifications.Add(modification);
			service.Update();
			service.ApplyWebConfigModifications();
		}
		protected void DeleteDxHttpModuleAndHandler() {
			SPWebService service = SPWebService.ContentService;
			List<SPWebConfigModification> removeCollection = new List<SPWebConfigModification>();
			for (int i = 0; i < service.WebConfigModifications.Count; i++) {
				SPWebConfigModification modification = service.WebConfigModifications[i];
				if (modification.Owner == OwnerName)
					removeCollection.Add(modification);
			}
			if (removeCollection.Count > 0) {
				foreach (SPWebConfigModification modificationItem in removeCollection)
					service.WebConfigModifications.Remove(modificationItem);
				service.Update();
				service.ApplyWebConfigModifications();
			}
		}
	}
	public class TextFieldFeatureReceiver : HtmlEditorFeatureReceiver {
		protected override string GetTemplateName() {
			return "SPxTextField.ascx";
		}
	}
}
