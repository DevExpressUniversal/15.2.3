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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.DocumentViewer;
using DevExpress.XtraReports.Web.Native.DocumentViewer.Remote;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
using ConstantsReportViewer = DevExpress.XtraReports.Web.Native.Constants.ReportViewer;
using XRParameter = DevExpress.XtraReports.Parameters.Parameter;
namespace DevExpress.Web.Mvc {
	public class DocumentViewerExtension : ExtensionBase {
		const string ActionArgument = "DXArgument";
		const string StateArgument = "DXRVState";
		#region static
		static DocumentViewerExtension() {
			ServiceOperationBase.DelayerFactory = new SynchronizedDelayerFactory(null);
		}
		public static FileResult ExportTo(XtraReport report) {
			return ExportTo(report, new HttpContextWrapper(HttpContext.Current));
		}
		public static FileResult ExportTo(XtraReport report, HttpContextBase httpContext) {
			Guard.ArgumentNotNull(httpContext, "httpContext");
			return ExportTo(report, httpContext.Request);
		}
		public static FileResult ExportTo(XtraReport report, HttpRequestBase request) {
			return ExportTo(report, request, Noop);
		}
		public static FileResult ExportTo(XtraReport report, HttpRequestBase request, Action<DeserializeClientParameterEventArgs> deserializeClientParameter) {
			Guard.ArgumentNotNull(request, "request");
			var argument = request.Params[ActionArgument];
			Hashtable clientState = MVCxDocumentViewer.LoadClientObjectStateInternal(request.Params, StateArgument);
			Hashtable drillDownKeys = null, clientParameters = null;
			if(clientState != null) {
				clientParameters = (Hashtable)clientState[ConstantsReportViewer.ClientParametersKey];
				drillDownKeys = (Hashtable)clientState[ConstantsReportViewer.ClientDrillDownKey];
			}
			if(string.IsNullOrEmpty(argument))
				return null;
			var mediator = new ReportWebMediator(report, argument, clientParameters, drillDownKeys, deserializeClientParameter);
			mediator.AssignClientStateToReport();
			if(report.PrintingSystem.Document.PageCount == 0)
				report.CreateDocument(false);
			return new XtraReportFileResult(mediator.GetExportInfoAsync().Result);
		}
		public static IList<RibbonTab> DefaultRibbonTabs {
			get { return DocumentViewerDefaultRibbon.CreateRibbonTabs(); }
		}
		public static FileResult ExportTo(MVCxDocumentViewerRemoteSourceSettings settings, HttpRequestBase request) {
			return ExportTo(settings, Noop, request);
		}
		public static FileResult ExportTo(MVCxDocumentViewerRemoteSourceSettings settings, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, HttpRequestBase request) {
			return ExportToAsync(settings, deserializeClientParameter, request).Result;
		}
		public static FileResult ExportTo(DocumentViewerRemoteSourceConfiguration configuration, HttpRequestBase request) {
			return ExportTo(configuration, Noop, request);
		}
		public static FileResult ExportTo(DocumentViewerRemoteSourceConfiguration configuration, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, HttpRequestBase request) {
			return ExportToAsync(configuration, deserializeClientParameter, request).Result;
		}
		public static Task<FileResult> ExportToAsync(MVCxDocumentViewerRemoteSourceSettings settings, HttpRequestBase request) {
			return ExportToAsync(settings, Noop, request);
		}
		public static Task<FileResult> ExportToAsync(MVCxDocumentViewerRemoteSourceSettings settings, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, HttpRequestBase request) {
			return RemoteExportToAsync(request, x => new ReportWebRemoteMediatorFactory().Create(settings, null, () => new MVCxClientExportTokenStorage(x)), deserializeClientParameter);
		}
		public static Task<FileResult> ExportToAsync(DocumentViewerRemoteSourceConfiguration configuration, HttpRequestBase request) {
			return ExportToAsync(configuration, Noop, request);
		}
		public static Task<FileResult> ExportToAsync(DocumentViewerRemoteSourceConfiguration configuration, Action<DeserializeClientParameterEventArgs> deserializeClientParameter, HttpRequestBase request) {
			Guard.ArgumentNotNull(configuration, "configuration");
			return RemoteExportToAsync(request, _ => new DocumentViewerReportWebRemoteMediator(configuration.ReportServiceClientFactory, configuration.InstanceIdentity), deserializeClientParameter); 
		}
		static Task<FileResult> RemoteExportToAsync(HttpRequestBase request, Func<RSRemoteDocumentInformation, IReportWebRemoteMediator> createMediator, Action<DeserializeClientParameterEventArgs> deserializeClientParameter) {
			Guard.ArgumentNotNull(request, "request");
			var argument = request.Params[ActionArgument];
			Hashtable clientState = MVCxDocumentViewer.LoadClientObjectStateInternal(request.Params, StateArgument);
			Hashtable clientParameters = null, 
				clientDrillDownKeys = null;
			string documentInformationString = null;
			if(clientState != null) {
				clientParameters = (Hashtable)clientState[ConstantsReportViewer.ClientParametersKey];
				clientDrillDownKeys = (Hashtable)clientState[ConstantsReportViewer.ClientDrillDownKey];
				documentInformationString = (string)clientState[DocumentViewerReportViewer.RemoteKey];
			}
			if(string.IsNullOrEmpty(argument))
				return null;
			var documentInformation = RSRemoteDocumentInformation.Deserialize(documentInformationString);
			var remoteMediator = createMediator(documentInformation);
			var remoteParameters = remoteMediator.GetReportParameters();
			var mediator = new RemoteReportWebMediator(remoteMediator, clientParameters, clientDrillDownKeys, remoteParameters.Parameters, deserializeClientParameter, () => documentInformation.DocumentInformation.DocumentId);
			mediator.InitEventInfo(argument);
			mediator.AssignClientStateToReport();
			return mediator.GetExportInfoAsync()
				.ContinueWith<FileResult>(t => new XtraReportFileResult(t.Result));
		}
		#endregion
		public DocumentViewerExtension(SettingsBase settings)
			: base(settings) {
		}
		public DocumentViewerExtension(SettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxDocumentViewer Control {
			get { return (MVCxDocumentViewer)base.Control; }
			protected set { base.Control = value; }
		}
		internal EditorStyles Styles {
			get { return new EditorStyles(Control); }
		}
		internal EditorImages Images {
			get { return new EditorImages(Control); }
		}
		internal new DocumentViewerSettings Settings {
			get { return (DocumentViewerSettings)base.Settings; }
		}
		bool InplaceAllowEditorSizeRecalc {
			get { return !Control.Width.IsEmpty && Control.Width.Type != UnitType.Percentage; } 
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.Report = Settings.Report;
			Control.ReportTypeName = Settings.ReportTypeName;
			Control.ToolbarMode = Settings.ToolbarMode;
			Control.AssociatedRibbonID = Settings.AssociatedRibbonName;
			Control.AutoHeight = Settings.AutoHeight;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ExportRouteValues = Settings.ExportRouteValues;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.ToolbarItems.Assign(Settings.ToolbarItems);
			Control.ConfigurationRemoteSource = Settings.ConfigurationRemoteSource;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.StylesReportViewer.CopyFrom(Settings.StylesReportViewer);
			Control.StylesToolbar.Assign(Settings.StylesToolbar);
			Control.StylesDocumentMap.Assign(Settings.StylesDocumentMap);
			Control.SettingsRemoteSource.Assign(Settings.SettingsRemoteSource);
			Control.SettingsReportViewer.Assign(Settings.SettingsReportViewer);
			Control.SettingsDocumentMap.Assign(Settings.SettingsDocumentMap);
			Control.SettingsParametersPanelCaption.Assign(Settings.SettingsParametersPanelCaption);
			Control.SettingsSplitter.Assign(Settings.SettingsSplitter);
			Control.SettingsRibbon.Assign(Settings.SettingsRibbon);
			Control.SettingsToolbar.Assign(Settings.SettingsToolbar);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.StylesParametersPanelButtons.Assign(Settings.StylesParametersPanelButtons);
			Control.StylesEditors.Assign(Settings.StylesEditors);
			Control.StylesParametersPanelEditors.Assign(Settings.StylesParametersPanelEditors);
			Control.StylesSplitter.Assign(Settings.StylesSplitter);
			Control.StylesRibbon.Assign(Settings.StylesRibbon);
			Control.LoadingPanelStyle.Assign(Settings.LoadingPanelStyle);
			Control.LoadingPanelImage.CopyFrom(Settings.LoadingPanelImage);
			Control.EnableTheming = Settings.EnableTheming;
			if(!string.IsNullOrEmpty(Settings.Theme))
				Control.Theme = Settings.Theme;
			Control.CacheReportDocument += Settings.CacheReportDocument;
			Control.RestoreReportDocumentFromCache += Settings.RestoreReportDocumentFromCache;
			Control.CustomizeParameterEditors += (_, e) => CustomizeParameterEditors(e);
			Control.DeserializeClientParameters += (_, e) => DeserializeClientParameters(e);
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxDocumentViewer();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			RenderUtils.EnsureChildControlsRecursive(Control, false);
			Control.PrepareControl();
		}
		void CustomizeParameterEditors(CustomizeParameterEditorsEventArgs e) {
			DoWithCustomParameterIfExist(
				e.Parameter.Name,
				x => x.PropertiesEdit != null,
				x => e.Editor = CreateEditor(x, e.Parameter));
		}
		void DeserializeClientParameters(DeserializeClientParameterEventArgs e) {
			DoWithCustomParameterIfExist(
				e.Path,
				x => x.JsObjectToPoco != null,
				x => e.Value = x.JsObjectToPoco(e.Value));
		}
		void DoWithCustomParameterIfExist(string parameterPath, Predicate<MVCxDocumentViewerParameter> optionalCheck, Action<MVCxDocumentViewerParameter> action) {
			MVCxDocumentViewerParameter customParameter;
			if(Settings.CustomParameters.TryGetValue(parameterPath, out customParameter) && (optionalCheck == null || optionalCheck(customParameter))) {
				action(customParameter);
			}
		}
		ASPxEditBase CreateEditor(MVCxDocumentViewerParameter dvParameter, XRParameter parameter) {
			var arg = new CreateEditControlArgs(
				parameter.Value,
				parameter.Type,
				null,
				null,
				Control,
				EditorInplaceMode.Inplace,
				InplaceAllowEditorSizeRecalc);
			ASPxEditBase editor = dvParameter.PropertiesEdit.CreateEdit(arg);
			if(!string.IsNullOrEmpty(dvParameter.EditorID)) {
				editor.ID = dvParameter.EditorID;
			}
			var tokenBox = editor as ASPxTokenBox;
			if(tokenBox != null && parameter.MultiValue) {
				MultiValueEditorFactory.ApplyMark(tokenBox);
			}
			editor.DataBind();
			return editor;
		}
		static void Noop(DeserializeClientParameterEventArgs args) {
		}
	}
	class MVCxClientExportTokenStorage : ITokenStorage {
		readonly RSRemoteDocumentInformation rsRemoteDocumentInformation;
		public MVCxClientExportTokenStorage(RSRemoteDocumentInformation rsRemoteDocumentInformation) {
			Guard.ArgumentNotNull(rsRemoteDocumentInformation, "rsRemoteDocumentInformation");
			this.rsRemoteDocumentInformation = rsRemoteDocumentInformation;
		}
		public string Token {
			get { return rsRemoteDocumentInformation.Token; }
			set { rsRemoteDocumentInformation.Token = value; }
		}
	}
}
