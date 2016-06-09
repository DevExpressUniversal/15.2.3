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
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon;
using Constants = DevExpress.XtraReports.Web.Native.Constants.DocumentViewer;
namespace DevExpress.Web.Mvc {
	public class DocumentViewerSettings : SettingsBase {
		public XtraReport Report { get; set; }
		public string ReportTypeName { get; set; }
		public DocumentViewerToolbarMode ToolbarMode { get; set; }
		public string AssociatedRibbonName { get; set; }
		public bool AutoHeight { get; set; }
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public DocumentViewerRemoteSourceConfiguration ConfigurationRemoteSource { get; set; }
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		public DocumentViewerRemoteSourceSettings SettingsRemoteSource { get; private set; }
		public DocumentViewerReportViewerSettings SettingsReportViewer { get; private set; }
		public DocumentViewerDocumentMapSettings SettingsDocumentMap { get; private set; }
		public ReportParametersPanelEditorCaptionSettings SettingsParametersPanelCaption { get; private set; }
		public DocumentViewerSplitterSettings SettingsSplitter { get; private set; }
		public DocumentViewerRibbonSettings SettingsRibbon { get; private set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get; private set; }
		public DocumentViewerReportToolbarProperties SettingsToolbar { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public DocumentViewerViewerStyles StylesReportViewer { get; private set; }
		public EditorStyles StylesEditors { get; private set; }
		public ButtonControlStyles StylesParametersPanelButtons { get; private set; }
		public ReportParametersPanelEditorStyles StylesParametersPanelEditors { get; private set; }
		public DocumentViewerSplitterStyles StylesSplitter { get; private set; }
		public DocumentViewerRibbonStyles StylesRibbon { get; private set; }
		public DocumentViewerReportToolbarStyles StylesToolbar { get; private set; }
		public ReportDocumentMapStyles StylesDocumentMap { get; private set; }
		[Obsolete("Use StylesEditors property instead.")]
		public ReportParametersPanelEditorStyles StylesParametersPanelParameterEditors {
			get { return StylesParametersPanelEditors; }
		}
		public ReportToolbarItemCollection ToolbarItems { get; private set; }
		public MVCxDocumentViewerClientSideEvents ClientSideEvents { get { return (MVCxDocumentViewerClientSideEvents)ClientSideEventsInternal; } }
		public ImageProperties LoadingPanelImage { get; private set; }
		public CacheReportDocumentEventHandler CacheReportDocument { get; set; }
		public RestoreReportDocumentFromCacheEventHandler RestoreReportDocumentFromCache { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public DocumentViewerSettings() {
			SettingsRemoteSource = new DocumentViewerRemoteSourceSettings(null);
			SettingsReportViewer = new DocumentViewerReportViewerSettings(null);
			SettingsDocumentMap = new DocumentViewerDocumentMapSettings(null);
			SettingsParametersPanelCaption = new ReportParametersPanelEditorCaptionSettings(null);
			SettingsSplitter = new DocumentViewerSplitterSettings(null);
			SettingsRibbon = new DocumentViewerRibbonSettings(null);
			SettingsToolbar = new DocumentViewerReportToolbarProperties(null);
			StylesToolbar = new DocumentViewerReportToolbarStyles(null);
			StylesDocumentMap = new ReportDocumentMapStyles(null);
			StylesReportViewer = new DocumentViewerViewerStyles(null);
			ToolbarItems = new ReportToolbarItemCollection();
			ToolbarItems.Assign(ReportToolbar.CreateDefaultItemCollection());
			SettingsLoadingPanel = new SettingsLoadingPanel(null);
			LoadingPanelImage = new ImageProperties();
			LoadingPanelStyle = new LoadingPanelStyle();
			StylesEditors = new EditorStyles(null);
			StylesParametersPanelButtons = new ButtonControlStyles(null);
			StylesParametersPanelEditors = new ReportParametersPanelEditorStyles(null);
			StylesSplitter = new DocumentViewerSplitterStyles(null);
			StylesRibbon = new DocumentViewerRibbonStyles(null);
			ToolbarMode = Constants.ToolbarModeDefault;
			AutoHeight = Constants.AutoHeightDefault;
			CustomParameters = new Dictionary<string, MVCxDocumentViewerParameter>();
		}
		internal IDictionary<string, MVCxDocumentViewerParameter> CustomParameters { get; private set; }
		public void SetCustomParameter(string parameterName, Action<MVCxDocumentViewerParameter> parameterMethod) {
			SetCustomParameter(x => {
				x.ParameterName = parameterName;
				parameterMethod(x);
			});
		}
		public void SetCustomParameter(Action<MVCxDocumentViewerParameter> parameterMethod) {
			var customParameter = new MVCxDocumentViewerParameter();
			parameterMethod(customParameter);
			if(string.IsNullOrEmpty(customParameter.ParameterName)) {
				throw new InvalidOperationException("ParameterName property can not be null or empty");
			}
			CustomParameters.Add(customParameter.ParameterName, customParameter);
		}
		public void SetRemoteSourceSettings(Action<DocumentViewerRemoteSourceSettings> action) {
			Guard.ArgumentNotNull(action, "action");
			Report = null;
			ReportTypeName = string.Empty;
			ConfigurationRemoteSource = null;
			SettingsRemoteSource.Reset();
			action(SettingsRemoteSource);
		}
		public void SetRemoteSourceConfiguration(Action<DocumentViewerRemoteSourceConfiguration> action) {
			Guard.ArgumentNotNull(action, "action");
			Report = null;
			ReportTypeName = string.Empty;
			SettingsRemoteSource.Reset();
			ConfigurationRemoteSource = new DocumentViewerRemoteSourceConfiguration();
			action(ConfigurationRemoteSource);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxDocumentViewerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return new DocumentViewerViewerStyles(null);
		}
	}
	public class MVCxDocumentViewerClientSideEvents : DocumentViewerClientSideEvents {
		const string BeforeExportRequestName = "BeforeExportRequest";
		public string BeforeExportRequest {
			get { return GetEventHandler(BeforeExportRequestName); }
			set { SetEventHandler(BeforeExportRequestName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(BeforeExportRequestName);
		}
	}
	public class MVCxDocumentViewerRemoteSourceSettings : DocumentViewerRemoteSourceSettings {
		public MVCxDocumentViewerRemoteSourceSettings()
			: base(null) {
		}
	}
}
