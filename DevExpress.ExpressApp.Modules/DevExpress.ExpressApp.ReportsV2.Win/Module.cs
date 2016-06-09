#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains Controllers that allow end-user interaction with reports in Windows Forms XAF applications. Allows a user to design, customize, filter, view, export and print reports. This module is based on UI independent elements that are implemented in the ReportsModuleV2.")]
	[ToolboxBitmap(typeof(ReportsWindowsFormsModuleV2), "Resources.Toolbox_Module_ReportsV2_Win.ico")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	[DesignerCategory("Component")]
	public sealed class ReportsWindowsFormsModuleV2 : ModuleBase {
		static ReportsWindowsFormsModuleV2() {
			ReportDesignExtensionManager.CustomizeReportExtension += new EventHandler<CustomizeReportExtensionEventArgs>(XafReportDesignExtensionManager_CustomizeReportExtension);
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(EditReportController),
				typeof(WinReportsController),
				typeof(WinReportServiceController),
				typeof(ReportDataSelectionDialogController),
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ReportControlLocalizer));
			return result;
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModules = base.GetRequiredModuleTypesCore();
			requiredModules.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
			requiredModules.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			return requiredModules;
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			PrintSelectionBaseController.ShowInReportActionEnableModeDefault = PrintSelectionBaseController.ActionEnabledMode.ModifiedChanged;
			application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		private static void XafReportDesignExtensionManager_CustomizeReportExtension(object sender, CustomizeReportExtensionEventArgs e) {
			XtraReportExtension reportExtension = e.ReportDesignExtension as XtraReportExtension;
			if(reportExtension != null) {
				reportExtension.CreateCustomReportRepositoryItem += extension_CreateCustomRepositoryItem;
			}
		}
		private static void extension_CreateCustomRepositoryItem(object sender, CreateCustomReportDesignRepositoryItemEventArgs e) {
			if(CreateCustomReportDesignRepositoryItem != null) {
				CreateCustomReportDesignRepositoryItem(null, e);
			}
			if(!e.Handled) {
				CreateCustomRepositoryItem(e);
			}
		}
		private static void CreateCustomRepositoryItem(CreateCustomReportDesignRepositoryItemEventArgs e) {
			if(e.Parameter == null || !e.Parameter.MultiValue) {
				Guard.ArgumentNotNull(e.Application, "e.Application");
				IObjectSpace objectSpace = DataSourceBase.CreateObjectSpace(e.DataType, e.Report);
				RepositoryEditorsFactory factory = new RepositoryEditorsFactory(e.Application, objectSpace);
				RepositoryItem item = factory.CreateStandaloneRepositoryItem(e.DataType);
				if(item is RepositoryItemLookupEdit) {
					((RepositoryItemLookupEdit)item).ShowActionContainersPanel = false;
				}
				e.RepositoryItem = item;
				e.Handled = true;
			}
		}
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			Application.LoggedOn -= new EventHandler<LogonEventArgs>(application_LoggedOn);
			ReportDesignExtensionManager.CreateReportExtension += ReportDesignExtensionManager_CreateReportExtension;
			ReportDesignExtensionManager.Initialize(Application);
		}
		private void ReportDesignExtensionManager_CreateReportExtension(object sender, CreateCustomReportExtensionEventArgs e) {
			ReportDesignExtensionManager.CreateReportExtension -= ReportDesignExtensionManager_CreateReportExtension;
			e.ReportDesignExtension = new XtraReportExtension();
		}
		public static event EventHandler<CreateCustomReportDesignRepositoryItemEventArgs> CreateCustomReportDesignRepositoryItem;
	}
}
