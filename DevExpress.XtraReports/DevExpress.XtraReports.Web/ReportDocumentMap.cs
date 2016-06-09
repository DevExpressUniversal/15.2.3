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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.DocumentMap;
namespace DevExpress.XtraReports.Web {
#if !DEBUG
#endif // DEBUG
	[Designer("DevExpress.XtraReports.Web.Design.ReportDocumentMapDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[DefaultProperty(Constants.ReportViewer.ReportViewerIDPropertyName)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ReportDocumentMap.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	[ToolboxItem(false)]
	public class ReportDocumentMap : ASPxWebControl {
		internal const bool
			DefaultAllowSelectNode = true,
			DefaultEnableAnimation = true,
			DefaultShowTreeLines = false;
		IBookmarkFiller<TreeViewNode> documentMapFiller;
		ASPxCallbackPanel callbackPanel;
		ASPxTreeView treeView;
		WebControl rootContainer;
		ReportViewer reportViewer;
		ReportViewer reportViewerInternal;
		bool allowSelectNode = DefaultAllowSelectNode;
		bool showTreeLines = DefaultShowTreeLines;
		bool showLoadingPanel = true;
		ImagePosition loadingPanelImagePosition = ImagePosition.Left;
		string loadingPanelText = StringResources.LoadingPanelText;
		[Browsable(false)]
		public ReportViewer ReportViewer {
			get { return reportViewer; }
			set {
				reportViewer = value;
				LayoutChanged();
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
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapStyles")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportDocumentMapStyles Styles {
			get { return (ReportDocumentMapStyles)StylesInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapAccessibilityCompliant")]
#endif
		[Category("Accessibility")]
		[DefaultValue(false)]
		[AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapAllowSelectNode")]
#endif
		[Category("Appearance")]
		[DefaultValue(true)]
		[AutoFormatDisable]
		public bool AllowSelectNode {
			get { return allowSelectNode; }
			set { allowSelectNode = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapShowTreeLines")]
#endif
		[Category("Appearance")]
		[DefaultValue(DefaultShowTreeLines)]
		[AutoFormatEnable]
		public bool ShowTreeLines {
			get { return showTreeLines; }
			set { showTreeLines = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapShowLoadingPanel")]
#endif
		[Category("Behavior")]
		[DefaultValue(true)]
		[AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return showLoadingPanel; }
			set { showLoadingPanel = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapShowLoadingPanelImage")]
#endif
		[Category("Appearance")]
		[DefaultValue(true)]
		[AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return showLoadingPanel; }
			set { showLoadingPanel = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapLoadingPanelImagePosition")]
#endif
		[Category("Appearance")]
		[DefaultValue(ImagePosition.Left)]
		[AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return loadingPanelImagePosition; }
			set { loadingPanelImagePosition = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapEnableAnimation")]
#endif
		[Category("Behavior")]
		[DefaultValue(true)]
		[AutoFormatDisable]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", DefaultEnableAnimation); }
			set { SetBoolProperty("EnableAnimation", DefaultEnableAnimation, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapEnableHotTrack")]
#endif
		[Category("Behavior")]
		[DefaultValue(true)]
		[AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapImages")]
#endif
		[Category("Images")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewImages Images {
			get { return (TreeViewImages)ImagesInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapLoadingPanelDelay")]
#endif
		[Category("Behavior")]
		[DefaultValue(SettingsLoadingPanel.DefaultDelay)]
		[AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapLoadingPanelText")]
#endif
		[DefaultValue(StringResources.LoadingPanelText)]
		[AutoFormatEnable]
		[Localizable(true)]
		public string LoadingPanelText {
			get { return loadingPanelText; }
			set { loadingPanelText = value; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapLoadingPanelImage")]
#endif
		[Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportDocumentMapClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category("Client-Side")]
		[DefaultValue("")]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		internal Func<IBookmarkFiller<TreeViewNode>> ResolveBookmarkFiller { get; set; }
		internal BookmarkFiller<TreeViewNode> CreateDefaultBookmarkFiller() {
			return new ReportDocumentMapFiller(ReportViewerInternal);
		}
		IBookmarkFiller<TreeViewNode> DocumentMapFiller {
			get {
				if(documentMapFiller == null)
					documentMapFiller = ResolveBookmarkFiller();
				return documentMapFiller;
			}
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
		public ReportDocumentMap() {
			ResolveBookmarkFiller = CreateDefaultBookmarkFiller;
		}
		public new SettingsLoadingPanel SettingsLoadingPanel { get { return base.SettingsLoadingPanel; } }
		protected override void InitInternal() {
			base.InitInternal();
			if(!DesignMode) {
				ForceReportViewerInternal();
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			callbackPanel = new ASPxCallbackPanel { ID = "CallbackPanel" };
			callbackPanel.SettingsLoadingPanel.ImagePosition = LoadingPanelImagePosition;
			callbackPanel.Images.LoadingPanel.Assign(LoadingPanelImage);
			if(DesignMode) {
				rootContainer = RenderUtils.CreateDiv();
				rootContainer.ID = "rootContainer";
				rootContainer.Controls.Add(callbackPanel);
			} else {
				rootContainer = callbackPanel;
				rootContainer.ID = "callbackPanel";
			}
			ApplyParentInfo(callbackPanel);
			callbackPanel.Callback += callbackPanel_Callback;
			treeView = new ASPxTreeView(this) {
				ID = "TreeView",
				ParentStyles = RenderStylesInternal,
				ParentImages = RenderImagesInternal,
				EnableClientSideAPI = true,
				AccessibilityCompliant = AccessibilityCompliant
			};
			ApplyParentInfo(treeView);
			callbackPanel.Controls.Add(treeView);
			Controls.Add(rootContainer);
			if(DesignMode) {
				FillDesignModeStub();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach(string styleKey in Style.Keys) {
				callbackPanel.Style[styleKey] = Style[styleKey];
			}
			callbackPanel.ControlStyle.CopyFrom(ControlStyle);
			callbackPanel.SettingsLoadingPanel.Enabled = ShowLoadingPanel;
			callbackPanel.SettingsLoadingPanel.ShowImage = ShowLoadingPanelImage;
			callbackPanel.SettingsLoadingPanel.Text = LoadingPanelText;
			callbackPanel.Styles.LoadingPanel.Assign(Styles.LoadingPanel);
			if(DesignMode) {
				callbackPanel.Width = Unit.Percentage(100);
				callbackPanel.Height = Unit.Percentage(100);
			}
			rootContainer.Width = Width;
			rootContainer.Height = Height;
			treeView.AllowSelectNode = AllowSelectNode;
			treeView.ShowTreeLines = ShowTreeLines;
			treeView.EnableAnimation = EnableAnimation;
			treeView.EnableHotTrack = EnableHotTrack;
			treeView.Styles.Assign(Styles);
		}
		protected override void ClearControlFields() {
			callbackPanel = null;
			treeView = null;
			base.ClearControlFields();
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientReportDocumentMap";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			base.RegisterIncludeScript(typeof(ReportDocumentMap), WebResourceNames.DocumentMap.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.reportViewerId = '{1}';\n", localVarName, ReportViewerClientID);
			stb.AppendFormat("{0}.callbackPanelId = '{1}';\n", localVarName, callbackPanel.ClientID);
		}
		protected override ImagesBase CreateImages() {
			return new TreeViewImages(this);
		}
		protected override StylesBase CreateStyles() {
			return new ReportDocumentMapStyles(this);
		}
		void ForceReportViewerInternal() {
			if(reportViewerInternal != null)
				return;
			reportViewerInternal = ReportViewer ??
				(!string.IsNullOrEmpty(ReportViewerID) ? (ReportViewer)FindControlHelper.LookupControlRecursive(Page, ReportViewerID) : null);
		}
		void FillDocumentMap() {
			TreeViewNode root = treeView.RootNode;
			root.Nodes.Clear();
			DocumentMapFiller.Fill(root);
		}
		void callbackPanel_Callback(object sender, CallbackEventArgsBase e) {
			FillDocumentMap();
			PrepareControlHierarchy();
		}
		void FillDesignModeStub() {
			TreeViewNode root = treeView.RootNode;
			root.Nodes.Clear();
			var documentNode = new TreeViewNode("Document") { Expanded = true };
			root.Nodes.Add(documentNode);
			documentNode.Nodes.Add("Bookmark 1");
			documentNode.Nodes.Add("Bookmark 2");
		}
		void ApplyParentInfo(ASPxWebControl control) {
			control.ParentSkinOwner = this;
		}
		protected override void EnsurePreRender() {
			if(!DesignMode) {
				if(treeView == null) {
					EnsureChildControls();
				}
				FillDocumentMap();
			}
			base.EnsurePreRender();
		}
	}
}
