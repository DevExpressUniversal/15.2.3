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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Reports.Win {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains Controllers that allow end-user interaction with reports in Windows Forms XAF applications. Allows a user to design, customize, filter, view, export and print reports. This module is based on UI independent elements that are implemented in the ReportsModule.")]
	[ToolboxBitmap(typeof(ReportsWindowsFormsModule), "Resources.Toolbox_Module_Reports_Win.ico")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed class ReportsWindowsFormsModule : ModuleBase {
		private static void CreateCustomRepositoryItem(CreateCustomReportDesignRepositoryItemEventArgs e) {
			XafReport report = e.Report as XafReport;
			if(report == null) {
				return;
			}
			Guard.ArgumentNotNull(e.Application, "e.Application");
			RepositoryEditorsFactory factory = new RepositoryEditorsFactory(e.Application, report.ObjectSpace);
			RepositoryItem item = factory.CreateStandaloneRepositoryItem(e.DataType);
			if(item is RepositoryItemLookupEdit) {
				((RepositoryItemLookupEdit)item).ShowActionContainersPanel = false;
			}
			e.RepositoryItem = item;
			e.Handled = true;
		}
		private static void extension_CreateCustomRepositoryItem(object sender, CreateCustomReportDesignRepositoryItemEventArgs e) {
			if(CreateCustomReportDesignRepositoryItem != null) {
				CreateCustomReportDesignRepositoryItem(null, e);
			}
			if(!e.Handled) {
				ReportsWindowsFormsModule.CreateCustomRepositoryItem(e);
			}
		}
		private static void XafReportDesignExtensionManager_CustomizeReportExtension(object sender, CustomizeReportExtensionEventArgs e) {
			ReportExtension reportExtension = e.ReportDesignExtension as ReportExtension;
			if(reportExtension != null) {
				reportExtension.CreateCustomReportRepositoryItem += extension_CreateCustomRepositoryItem;
			}
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(NewXafReportWizardParameters),
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(EditReportController),
				typeof(NewReportWizardController),
				typeof(PreviewReportDialogController),
				typeof(ReportsController),
				typeof(WinReportServiceController)
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
			requiredModules.Add(typeof(ExpressApp.Reports.ReportsModule));
			requiredModules.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			return requiredModules;
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
			PrintSelectionBaseController.ShowInReportActionEnableModeDefault = PrintSelectionBaseController.ActionEnabledMode.ModifiedChanged;
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		static ReportsWindowsFormsModule() {
			ReportDesignExtensionManager.CustomizeReportExtension += new EventHandler<CustomizeReportExtensionEventArgs>(XafReportDesignExtensionManager_CustomizeReportExtension);
		}
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			Application.LoggedOn -= new EventHandler<LogonEventArgs>(application_LoggedOn);
			ReportDesignExtensionManager.Initialize(Application);
		}
		public static event EventHandler<CreateCustomReportDesignRepositoryItemEventArgs> CreateCustomReportDesignRepositoryItem;
#if DebugTest
		public static void DebugTest_CreateCustomRepositoryItem(CreateCustomReportDesignRepositoryItemEventArgs e) {
			CreateCustomRepositoryItem(e);
		}
#endif
		#region Obsolete 10.1
		[Obsolete("Use the 'ExpressApp.Reports.ReportsModule.ReportDataType' property instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public Type ReportDataType {
			get { return null; }
			set { }
		}
		[Obsolete("Use the 'ExpressApp.Reports.ReportsModule.EnableInplaceReports' property instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public bool EnableInplaceReports {
			get { return false; }
			set { }
		}
		#endregion
	}
}
