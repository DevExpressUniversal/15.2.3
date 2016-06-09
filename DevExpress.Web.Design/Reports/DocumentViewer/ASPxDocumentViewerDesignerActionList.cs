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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.Design;
using DevExpress.Utils;
using DevExpress.Web.Design.Reports.Converters;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Design;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using EnvDTE;
namespace DevExpress.Web.Design.Reports.DocumentViewer {
	public class ASPxDocumentViewerDesignerActionList : XRWebControlDesignerActionListBase<ASPxDocumentViewer> {
		delegate Expression<Func<ASPxDocumentViewer, object>>[] RegisterExpressionsCallback(params Expression<Func<ASPxDocumentViewer, object>>[] expressions);
		internal const string CreateDefaultTabsDisplay = "Create Default Ribbon Tabs";
		readonly ASPxDocumentViewerDesigner designer;
		readonly Wrapper<string> remoteSourceCachedUserName;
		readonly IComponentChangeService componentChangeService;
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		[TypeConverter(typeof(RibbonControlIDConverter))]
		public string AssociatedRibbonID {
			get { return Component.AssociatedRibbonID; }
			set {
				ChangeComponents(
					rpx => rpx(
						x => x.AssociatedRibbonID,
						x => x.ToolbarMode),
					() => {
						Component.AssociatedRibbonID = value;
						Component.ToolbarMode = string.IsNullOrEmpty(value)
							? DocumentViewerToolbarMode.Ribbon
							: DocumentViewerToolbarMode.ExternalRibbon;
					});
			}
		}
		[TypeConverter(typeof(ReportTypeNameConverter))]
		[Editor("DevExpress.XtraReports.Design.ReportNameEditor," + AssemblyInfo.SRAssemblyReportsDesign, typeof(UITypeEditor))]
		public string ReportTypeName {
			get { return Component.ReportTypeName; }
			set {
				ChangeComponents(
					rpx => rpx(
						x => x.ReportTypeName,
						x => x.SettingsRemoteSource),
					() => {
						Component.SettingsRemoteSource.Reset();
						Component.ReportTypeName = value;
					});
			}
		}
		[Editor(typeof(RemoteReportEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RemoteReportConverter))]
		public string RemoteReport {
			get {
				var settings = Component.SettingsRemoteSource;
				return !string.IsNullOrEmpty(settings.ServerUri)
					? settings.ServerUri
					: ReportTypeNameConverter.NoneValue;
			}
		}
		public bool PrintUsingAdobePlugIn {
			get { return Component.SettingsReportViewer.PrintUsingAdobePlugIn; }
			set {
				ChangeComponents(
					rpx => rpx(x => x.SettingsReportViewer),
					() => Component.SettingsReportViewer.PrintUsingAdobePlugIn = value);
			}
		}
		public DocumentViewerToolbarMode ToolbarMode {
			get { return Component.ToolbarMode; }
			set { SetComponentProperty(x => x.ToolbarMode, value); }
		}
		public ASPxDocumentViewerDesignerActionList(ASPxDocumentViewerDesigner designer, Wrapper<string> remoteSourceCachedUserName, IComponentChangeService componentChangeService)
			: base(designer) {
			this.designer = designer;
			this.remoteSourceCachedUserName = remoteSourceCachedUserName;
			this.componentChangeService = componentChangeService;
		}
		protected override void BuildActionItems(DesignerActionItemsBuilder builder) {
			const string ViewerCategory = "Viewer";
			const string ToolbarCategory = "Toolbar";
			builder.AddMethod(() => SwitchToWebDocumentViewer(), "Switch to HTML5 Document Viewer", "Designer...", includeAsDesignerVerb: true);
			builder.AddHeader(ViewerCategory, ViewerCategory);
			builder.AddProperty(() => ReportTypeName, "Local Report", ViewerCategory);
			builder.AddProperty(() => RemoteReport, "Remote Report", ViewerCategory);
			builder.AddMethod(() => UnassignReport(), "Unassign Report", ViewerCategory, "Unassign the current local or remote report");
			builder.AddProperty(() => PrintUsingAdobePlugIn, "Print Using Adobe Plug-In", ViewerCategory);
			builder.AddHeader(ToolbarCategory, ToolbarCategory);
			builder.AddProperty(() => ToolbarMode, "Toolbar Mode", ToolbarCategory);
			if(ToolbarMode == DocumentViewerToolbarMode.ExternalRibbon) {
				builder.AddProperty(() => AssociatedRibbonID, "Associated Ribbon ID", ToolbarCategory);
				if(!string.IsNullOrWhiteSpace(AssociatedRibbonID)) {
					builder.AddMethod(() => CreateDefaultRibbonTabs(), CreateDefaultTabsDisplay, ToolbarCategory, "Create Default Ribbon Tabs for External Ribbon");
				}
			}
		}
		public void ResetToolbarItems() {
			InvokeTransactedChange(
				x => x.ToolbarItems,
				"Reset Toolbar Items",
				() => Component.ToolbarItems.Assign(ReportToolbar.CreateDefaultItemCollection()));
		}
		public void CreateDefaultRibbonTabs() {
			if(Component.ToolbarMode != DocumentViewerToolbarMode.ExternalRibbon || string.IsNullOrEmpty(Component.AssociatedRibbonID)) {
				return;
			}
			var tabs = DocumentViewerDefaultRibbon.CreateRibbonTabs().ToArray();
			InvokeTransactedChange(
				null,
				ASPxDocumentViewerDesignerActionList.CreateDefaultTabsDisplay,
				() => RibbonDesignerHelper.AddTabCollectionToRibbonControl(Component.AssociatedRibbonID, tabs, Component)
			);
		}
		public void ConfigureRemoteReport() {
			RemoteDocumentSourceModel model;
			if(RemoteReportConfigurer.Configure(Component, remoteSourceCachedUserName.Value, out model)) {
				AssignWizardModel(model);
			}
		}
		public void UnassignReport() {
			ChangeComponents(
				rpx => rpx(
					x => x.SettingsRemoteSource,
					x => x.ReportTypeName),
				() => {
					Component.SettingsRemoteSource.Reset();
					Component.ReportTypeName = string.Empty;
				});
		}
		public void SwitchToWebDocumentViewer() {
			ViewersSwitcher.Switch(Component, designer.DesignerHost);
		}
		static DesignerActionPropertyItem CreateDesignerActionPropertyItem(Expression<Func<object>> propertyExpression, string displayName, string category) {
			var memberName = ExpressionHelper.GetPropertyName(propertyExpression);
			return new DesignerActionPropertyItem(memberName, displayName, category);
		}
		DesignerActionMethodItem CreateDesignerActionMethodItem(Expression<Action> methodExpression, string displayName, string category, string description = null, bool includeAsDesignerVerb = false) {
			var memberName = ExpressionHelper.GetMethodName(methodExpression);
			return new DesignerActionMethodItem(this, memberName, displayName, category, description, includeAsDesignerVerb);
		}
		void ChangeComponents(Func<RegisterExpressionsCallback, Expression<Func<ASPxDocumentViewer, object>>[]> registerPropertiesExpressions, Action action) {
			Guard.ArgumentNotNull(registerPropertiesExpressions, "registerPropertiesExpressions");
			Guard.ArgumentNotNull(action, "action");
			Expression<Func<ASPxDocumentViewer, object>>[] propertyiesExpression = registerPropertiesExpressions(x => x);
			PropertyDescriptor[] properties = propertyiesExpression
				.Select(ExpressionHelper.GetPropertyName)
				.Select(x => TypeDescriptor.GetProperties(Component)[x])
				.ToArray();
			using(var transaction = designer.DesignerHost.CreateTransaction()) {
				foreach(var property in properties) {
					componentChangeService.OnComponentChanging(Component, property);
				}
				try {
					action();
					transaction.Commit();
				} finally {
					foreach(var property in properties) {
						componentChangeService.OnComponentChanged(Component, property, null, null);
					}
				}
			}
		}
		void AssignWizardModel(RemoteDocumentSourceModel model) {
			ChangeComponents(
				rpx => rpx(
					x => x.SettingsRemoteSource,
					x => x.ReportTypeName,
					x => x.SettingsReportViewer),
				() => AssignWizardModelCore(model));
		}
		void AssignWizardModelCore(RemoteDocumentSourceModel model) {
			Component.ReportTypeName = string.Empty;
			Component.SettingsReportViewer.PageByPage = true;
			var settings = Component.SettingsRemoteSource;
			settings.Reset();
			settings.AuthenticationType = model.AuthenticationType;
			settings.ServerUri = model.ServiceUri;
			if(model.ReportId != DocumentViewerRemoteSourceSettings.DefaultReportId) {
				settings.ReportId = model.ReportId;
			} else {
				settings.ReportTypeName = model.ReportName;
			}
			if(model.GenerateEndpoints) {
				settings.EndpointConfigurationName = model.Endpoint;
			}
			remoteSourceCachedUserName.Value = model.UserName;
		}
		void InvokeTransactedChange(Expression<Func<ASPxDocumentViewer, object>> propertyExpression, string description, Action action) {
			PropertyDescriptor property = null;
			if(propertyExpression != null) {
				var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);
				property = TypeDescriptor.GetProperties(Component)[propertyName];
			}
			ControlDesigner.InvokeTransactedChange(
				Component,
				_ => {
					action();
					return true;
				},
				null,
				description,
				property);
		}
	}
}
