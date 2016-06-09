#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.DocumentViewer.ParametersPanel;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
using XRParameter = DevExpress.XtraReports.Parameters.Parameter;
namespace DevExpress.XtraReports.Web {
#if !DEBUG
#endif // DEBUG
	[Designer("DevExpress.XtraReports.Web.Design.ReportParametersPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[DefaultProperty(Constants.ReportViewer.ReportViewerIDPropertyName)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ReportParametersPanel.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	[ToolboxItem(false)]
	public class ReportParametersPanel : ASPxWebControl {
		#region types
		struct CreateParameterEventResult {
			public ASPxEditBase Editor { get; set; }
			public bool ShouldSetParameterValue { get; set; }
		}
		#endregion
		#region resources
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		const string WebScriptResourcePath = WebResourceNames.WebScriptResourcePath;
		internal const string
			SystemCssResourceName = WebCssResourcePath + "ParametersPanel.System.css",
			ScriptResourceName = WebScriptResourcePath + "ReportParametersPanel.js";
		#endregion
		ASPxParameterInfo[] parametersInfo;
		ASPxButton resetBtn;
		ASPxButton submitBtn;
		ReportViewer reportViewer;
		InternalTable controlButtonsContainer;
		TableCell resetCell;
		TableCell buttonsMiddleCell;
		TableCell submitCell;
		ReportViewer reportViewerInternal;
		Unit controlButtonsContainerWidth = Unit.Percentage(100);
		Unit controlButtonWidth = Unit.Empty;
#if DEBUGTEST
		internal ASPxParameterInfo[] ParametersInfo_TEST {
			get { return parametersInfo; }
		}
#endif
		#region events
		static readonly object CustomizeParameterEditorsEvent = new object();
		public event EventHandler<CustomizeParameterEditorsEventArgs> CustomizeParameterEditors {
			add { Events.AddHandler(CustomizeParameterEditorsEvent, value); }
			remove { Events.RemoveHandler(CustomizeParameterEditorsEvent, value); }
		}
		void RaiseCustomizeParameterEditorsEvent(CustomizeParameterEditorsEventArgs arg) {
			var handler = Events[CustomizeParameterEditorsEvent] as EventHandler<CustomizeParameterEditorsEventArgs>;
			if(handler != null) {
				handler(this, arg);
			}
		}
		#endregion
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ReportViewer ReportViewer {
			get { return reportViewer; }
			set {
				reportViewer = value;
				LayoutChanged();
				if(reportViewer != null)
					reportViewer.UseClientParameters = true;
			}
		}
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SRCategory(ReportStringId.CatData)]
		[Bindable(true)]
		[AutoFormatDisable]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportViewerConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[Themeable(false)]
		[Localizable(false)]
		public string ReportViewerID {
			get { return GetStringProperty(Constants.ReportViewer.ReportViewerIDPropertyName, ""); }
			set { SetStringProperty(Constants.ReportViewer.ReportViewerIDPropertyName, "", value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelStylesEditors")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles StylesEditors { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelStylesParameterEditor")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportParametersPanelEditorStyles StylesParameterEditor { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelStylesButton")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonControlStyles StylesButton { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelSpriteCssFilePath")]
#endif
		[Category("Images")]
		[DefaultValue("")]
		[Localizable(false)]
		[UrlProperty]
		[AutoFormatEnable]
		[AutoFormatUrlProperty]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[DefaultValue("100%")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SRCategory(ReportStringId.CatLayout)]
		[Bindable(true)]
		[AutoFormatDisable]
		[Localizable(false)]
		public Unit ControlButtonsContainerWidth {
			get { return controlButtonsContainerWidth; }
			set { controlButtonsContainerWidth = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category("Client-Side")]
		[DefaultValue("")]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelControlButtonWidth")]
#endif
		[DefaultValue(typeof(Unit), "")]
		public Unit ControlButtonWidth {
			get { return controlButtonWidth; }
			set { controlButtonWidth = value; }
		}
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportParametersPanelCaptionSettings")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ReportParametersPanelEditorCaptionSettings CaptionSettings { get; private set; }
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public DropDownButton DropDownEditButtonSettings { get; private set; }
		internal Func<ParametersEditorCreatorBase<ASPxEditBase>, IParametersProvider> ResolveParametersProvider { get; set; }
		internal Func<ICallbackEventProcessor> ResolveCallbackEventProcessor { get; set; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		ReportViewer ReportViewerInternal {
			get {
				ForceReportViewerInternal();
				return reportViewerInternal;
			}
		}
		string ReportViewerClientID {
			get { return ReportViewerInternal != null ? ReportViewerInternal.ClientID : null; }
		}
		protected XtraReport Report {
			get { return ReportViewerInternal != null ? ReportViewerInternal.ForcedReport : null; }
		}
		public ReportParametersPanel() {
			StylesEditors = new EditorStyles(this);
			StylesParameterEditor = new ReportParametersPanelEditorStyles(this);
			StylesButton = new ButtonControlStyles(this);
			DropDownEditButtonSettings = new DropDownButton(this);
			CaptionSettings = new ReportParametersPanelEditorCaptionSettings(this);
			ResolveParametersProvider = CreateDefaultParametersProvider;
			ResolveCallbackEventProcessor = CreateDefaultCallbackEventProcessor;
		}
		protected ASPxParameterInfo[] ParametersInfo {
			get {
				if(parametersInfo == null) {
					var editorCreator = new ParametersASPxEditorCreator(!IsMvcRender(), Report);
					IParametersProvider provider = ResolveParametersProvider(editorCreator);
					parametersInfo = provider.GetParameters(GenerateParameterInfo);
				}
				return parametersInfo;
			}
		}
		protected override void BeforeRender() {
			if(DesignMode) {
				ResolveParametersProvider = x => new DesignTimeParametersProvider();
			}
			base.BeforeRender();
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			var editorIndex = 0;
			foreach(ASPxParameterInfo info in ParametersInfo) {
				if(info.EditorInformation == null)
					continue;
				InitParameterInfo(info);
				UpdateEditorId(info.EditorInformation, ++editorIndex);
				Controls.Add(info.EditorInformation);
			}
			controlButtonsContainer = RenderUtils.CreateTable(false);
			var buttonsRow = RenderUtils.CreateTableRow();
			resetCell = RenderUtils.CreateTableCell();
			buttonsMiddleCell = RenderUtils.CreateTableCell();
			submitCell = RenderUtils.CreateTableCell();
			resetBtn = new ASPxButton {
				ID = "Reset",
				ParentStyles = StylesButton,
				ClientEnabled = false,
				Text = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_Reset)
			};
			ConfigureControlButton(resetBtn);
			submitBtn = new ASPxButton {
				ID = "Submit",
				ParentStyles = StylesButton,
				Text = ASPxReportsLocalizer.GetString(ASPxReportsStringId.ParametersPanel_Submit)
			};
			ConfigureControlButton(submitBtn);
			resetCell.Controls.Add(resetBtn);
			submitCell.Controls.Add(submitBtn);
			buttonsRow.Cells.Add(resetCell);
			buttonsRow.Cells.Add(buttonsMiddleCell);
			buttonsRow.Cells.Add(submitCell);
			controlButtonsContainer.Rows.Add(buttonsRow);
			Controls.Add(controlButtonsContainer);
			var bottomEnd = RenderUtils.CreateDiv();
			bottomEnd.Style["clear"] = "both";
			Controls.Add(bottomEnd);
		}
		protected override void InitInternal() {
			base.InitInternal();
			ForceReportViewerInternal();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareControlHierarchyStyles();
			resetBtn.Width = ControlButtonWidth;
			submitBtn.Width = ControlButtonWidth;
			var fullWidth = Unit.Percentage(100);
			foreach(var editor in ParametersInfo.Where(x => x.EditorInformation != null).Select(x => x.EditorInformation)) {
				AssignParentInfo(editor);
				editor.ParentStyles = StylesEditors;
				if(editor.Width.IsEmpty) {
					editor.Width = fullWidth;
				}
				editor.CaptionStyle.Assign(StylesParameterEditor.Caption);
				editor.CaptionCellStyle.Assign(StylesParameterEditor.CaptionCell);
				editor.RootStyle.Assign(StylesParameterEditor.EditorRoot);
				editor.RootStyle.CssClass = RenderUtils.CombineCssClasses(editor.RootStyle.CssClass, "dxXtraReports_parametersPanel_editor");
			}
			controlButtonsContainer.CssClass = "dxXtraReports_parametersPanel_controlButtonsContainer";
			buttonsMiddleCell.CssClass = "dxXtraReports_parametersPanel_buttonsMiddleCell";
			buttonsMiddleCell.Text = "&nbsp;";
			if(!DesignMode) {
				var submitBtnClientId = RenderUtils.GetReferentControlClientID(this, submitBtn.ClientID, null);
				var fireButtonJS = string.Format(ASPxWebControl.FireDefaultButtonHandlerName, submitBtnClientId);
				RenderUtils.SetStringAttribute(this, "onkeypress", fireButtonJS);
			}
		}
		protected virtual void PrepareControlHierarchyStyles() {
			if(Browser.IsIE) {
				Style.Add(HtmlTextWriterStyle.PaddingRight, Unit.Pixel(1).ToString());
			}
		}
		protected override void ClearControlFields() {
			parametersInfo = null;
			resetBtn = null;
			submitBtn = null;
			controlButtonsContainer = null;
			resetCell = null;
			buttonsMiddleCell = null;
			submitBtn = null;
			base.ClearControlFields();
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientReportParametersPanel";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ReportParametersPanel), ReportParametersPanel.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(ReportViewerClientID)) {
				stb.AppendFormat("{0}.reportViewerId = '{1}';\n", localVarName, ReportViewerClientID);
				var parametersJson = HtmlConvertor.ToJSON(new ParametersJSON(ParametersInfo), false, false, true);
				stb.AppendFormat("{0}.parameters = {1};\n", localVarName, parametersJson);
			}
		}
		protected virtual void BeforeOnLoad() {
			DataBind();
		}
		protected override void OnLoad(EventArgs e) {
			BeforeOnLoad();
			base.OnLoad(e);
			AfterOnLoad();
		}
		protected virtual void AfterOnLoad() {
			if(ReportViewerInternal != null) {
				ReportViewerInternal.UseClientParameters = true;
				LayoutChanged();
			}
		}
		protected override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			reportViewerInternal = null;
			reportViewer = null;
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ReportParametersPanel), SystemCssResourceName);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] {
					StylesEditors,
					StylesParameterEditor,
					StylesButton,
					DropDownEditButtonSettings,
					CaptionSettings
				});
		}
		CreateParameterEventResult RaiseCustomizeParameterEditors(XRParameter parameter, ASPxEditBase editor) {
			var shouldSetParameterValue = CustomizeParameterEditorsEventArgs.ShouldSetParameterValueDefault;
			if(ComboboxDateTimeLookupFix.HasMark(editor) || MultiValueEditorFactory.HasMark(editor)) {
				shouldSetParameterValue = false;
			}
			var arg = new CustomizeParameterEditorsEventArgs(parameter, editor, Report, shouldSetParameterValue);
			RaiseCustomizeParameterEditorsEvent(arg);
			return new CreateParameterEventResult {
				ShouldSetParameterValue = arg.ShouldSetParameterValue,
				Editor = arg.Editor
			};
		}
		internal void ForceReportViewerInternal() {
			if(reportViewerInternal != null)
				return;
			if(reportViewer != null && reportViewer.Visible) {
				reportViewerInternal = reportViewer;
			} else if(!string.IsNullOrEmpty(ReportViewerID) && Page != null) {
				var viewer = (ReportViewer)FindControlHelper.LookupControlRecursive(Page, ReportViewerID);
				if(viewer != null && viewer.Visible) {
					reportViewerInternal = viewer;
				}
			}
		}
		void ConfigureControlButton(ASPxButton button) {
			button.EnableClientSideAPI = true;
			button.AutoPostBack = false;
			AssignParentInfo(button);
		}
		void AssignParentInfo(ASPxWebControl control) {
			control.ParentSkinOwner = this;
			control.ParentImages = RenderImagesInternal;
		}
		protected virtual ASPxParameterInfo GenerateParameterInfo(ParameterPath parameterPath, ASPxEditBase editor) {
			if(editor != null) {
				XRParameter parameter = parameterPath.Parameter;
				CreateParameterEventResult result = RaiseCustomizeParameterEditors(parameter, editor);
				editor = result.Editor;
				ClientIDHelper.EnableClientIDGeneration(editor);
				if(result.ShouldSetParameterValue) {
					editor.Value = parameter.Value;
				}
				editor.EnableClientSideAPI = true;
			}
			return new ASPxParameterInfo(parameterPath, editor, Report);
		}
		protected virtual void InitParameterInfo(ASPxParameterInfo info) {
			if(info.EditorInformation is ASPxDropDownEditBase) {
				((ASPxDropDownEditBase)info.EditorInformation).DropDownButton.Assign(DropDownEditButtonSettings);
			}
			if(string.IsNullOrEmpty(info.EditorInformation.Caption)) {
				info.EditorInformation.Caption = info.Caption;
			}
			var edit = info.EditorInformation as ASPxEdit;
			if(edit != null) {
				edit.CaptionSettings.Assign(CaptionSettings);
			}
		}
		IParametersProvider CreateDefaultParametersProvider(ParametersEditorCreatorBase<ASPxEditBase> editorCreator) {
			return new ReportParametersProvider(Report, editorCreator);
		}
		ICallbackEventProcessor CreateDefaultCallbackEventProcessor() {
			return new CallbackEventProcessor(Report);
		}
		static void UpdateEditorId(ASPxEditBase editor, int index) {
			if(string.IsNullOrEmpty(editor.ID)) {
				editor.ID = "dxxrppEditor" + index.ToString();
			}
		}
	}
}
