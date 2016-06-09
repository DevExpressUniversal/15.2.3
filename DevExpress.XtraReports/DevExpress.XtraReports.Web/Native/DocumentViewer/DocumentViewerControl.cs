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
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.ReportServer.Printing;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native.DocumentMap;
using DevExpress.XtraReports.Web.Native.DocumentViewer.ParametersPanel;
using DevExpress.XtraReports.Web.Native.DocumentViewer.Remote;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	[ToolboxItem(false)]
	public class DocumentViewerControl : ASPxInternalWebControl {
		const string ToolbarPaneName = "ToolbarPane";
		const string CenterPaneName = "CenterPane";
		const string SideCenterPaneName = "SideCenterPane";
		const string ViewerPaneName = "ViewerPane";
		const string ParametersPanelPaneName = "ParametersPanelPane";
		const string DocumentMapPaneName = "DocumentMapPane";
		readonly ASPxDocumentViewer documentViewer;
		readonly IReportWebRemoteMediatorFactory reportWebRemoteMediatorFactory;
		ReportToolbar toolbar;
		ASPxRibbon ribbonToolbar;
		ASPxSplitter splitter;
		bool? forcedParametersPanelVisible;
		bool hasRemoteParameters;
		public DocumentViewerReportViewer Viewer { get; set; }
		public DocumentViewerReportDocumentMap DocumentMap { get; private set; }
		internal DocumentViewerReportParametersPanel ParametersPanel { get; private set; }
		SplitterPane ToolbarPane { get { return splitter.Panes[ToolbarPaneName]; } }
		SplitterPane CenterPane { get { return splitter.Panes[CenterPaneName]; } }
		SplitterPane ViewerPane { get { return CenterPane.Panes[ViewerPaneName]; } }
		SplitterPane SideCenterPane { get { return CenterPane.Panes[SideCenterPaneName]; } }
		SplitterPane ParametersPanelPane { get { return SideCenterPane.Panes[ParametersPanelPaneName]; } }
		SplitterPane DocumentMapPane { get { return SideCenterPane.Panes[DocumentMapPaneName]; } }
		internal bool RemoteMode { get; private set; }
		bool LocalReportHasBookmarks {
			get {
				if(RemoteMode || Viewer == null) {
					return false;
				}
				var report = Viewer.ForcedReport;
				if(report == null) {
					return false;
				}
				var document = report.PrintingSystem.Document;
				if(document == null) {
					return false;
				}
				return document.RootBookmark.Nodes.Count > 0;
			}
		}
		bool IsCallback {
			get { return (Page != null && Page.IsCallback) || MvcUtils.IsCallback(); }
		}
		public DocumentViewerControl(ASPxDocumentViewer documentViewer)
			: this(documentViewer, new ReportWebRemoteMediatorFactory()) {
		}
		internal DocumentViewerControl(ASPxDocumentViewer documentViewer, IReportWebRemoteMediatorFactory reportWebRemoteMediatorFactory) {
			this.documentViewer = documentViewer;
			this.reportWebRemoteMediatorFactory = reportWebRemoteMediatorFactory;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void PrepareControl() {
			Viewer.PrepareControl();
		}
		void SubscribeReportViewer() {
			if(Viewer != null) {
				Viewer.RestoreReportDocumentFromCache += documentViewer.RaiseRestoreReportDocumentFromCache;
				Viewer.CacheReportDocument += documentViewer.RaiseCacheReportDocument;
			}
		}
		void SubscribeParametersPanel() {
			if(ParametersPanel != null) {
				ParametersPanel.CustomizeParameterEditors += documentViewer.RaiseCustomizeParameterEditors;
			}
		}
		void UnsubscribeParametersPanel() {
			if(ParametersPanel != null) {
				ParametersPanel.CustomizeParameterEditors -= documentViewer.RaiseCustomizeParameterEditors;
			}
		}
		void UnsubscribeReportViewer() {
			if(Viewer != null) {
				Viewer.RestoreReportDocumentFromCache -= documentViewer.RaiseRestoreReportDocumentFromCache;
				Viewer.CacheReportDocument -= documentViewer.RaiseCacheReportDocument;
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			var settingsViewer = documentViewer.SettingsReportViewer;
			Viewer = new DocumentViewerReportViewer(documentViewer.IsCallback) {
				ID = "Viewer",
				Report = documentViewer.Report,
				ReportName = documentViewer.ReportTypeName,
				ParentSkinOwner = documentViewer,
				PageByPage = settingsViewer.PageByPage,
				PrintUsingAdobePlugIn = settingsViewer.PrintUsingAdobePlugIn,
				UseIFrame = settingsViewer.UseIFrame,
				TableLayout = settingsViewer.TableLayout,
				EnableReportMargins = settingsViewer.EnableReportMargins,
				EnableRequestParameters = settingsViewer.EnableRequestParameters,
				ImagesEmbeddingMode = settingsViewer.ImagesEmbeddingMode,
				ShouldDisposeReport = settingsViewer.ShouldDisposeReport,
				EnableViewState = false
			};
			Viewer.DeserializeClientParameters += (_, e) => documentViewer.RaiseDeserializeClientParameters(e);
			Viewer.SettingsLoadingPanel.Assign(documentViewer.SettingsLoadingPanel);
			SubscribeReportViewer();
			ParametersPanel = new DocumentViewerReportParametersPanel {
				ID = "ParametersPanel",
				ReportViewer = Viewer,
				ParentSkinOwner = documentViewer,
				EnableViewState = false
			};
			ParametersPanel.DropDownEditButtonSettings.Assign(documentViewer.SettingsDropDownEditButton);
			ParametersPanel.CaptionSettings.Assign(documentViewer.SettingsParametersPanelCaption);
			SubscribeParametersPanel();
			var settingsDocumentMap = documentViewer.SettingsDocumentMap;
			DocumentMap = new DocumentViewerReportDocumentMap {
				ID = "DocumentMap",
				ReportViewer = Viewer,
				ShowTreeLines = settingsDocumentMap.ShowTreeLines,
				AllowSelectNode = settingsDocumentMap.AllowSelectNode,
				ParentSkinOwner = documentViewer,
				AccessibilityCompliant = documentViewer.AccessibilityCompliant,
				EnableViewState = false
			};
			DocumentMap.SettingsLoadingPanel.Assign(documentViewer.SettingsLoadingPanel);
			CreateSplitter();
			Controls.Add(splitter);
			if(documentViewer.ToolbarMode == DocumentViewerToolbarMode.Ribbon) {
				CreateInternalRibbon();
			} else if(documentViewer.ToolbarMode == DocumentViewerToolbarMode.StandardToolbar) {
				CreateToolbar();
			}
			ViewerPane.Controls.Add(CreateViewerTable());
			ParametersPanelPane.Controls.Add(ParametersPanel);
			DocumentMapPane.Controls.Add(DocumentMap);
			RecreateSplitterHierarchy();
			RemoteMode = TryInitializeRemoteViewing();
			Viewer.RemoteMode = RemoteMode;
			if(!RemoteMode) {
				InitializeLocalViewing();
			}
			AssignClientSideEvents();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			splitter.ControlStyle.CopyFrom(documentViewer.ControlStyle);
			var viewerStyles = documentViewer.StylesReportViewer;
			if(!string.IsNullOrEmpty(documentViewer.ImageFolder))
				Viewer.ImageFolder = documentViewer.ImageFolder;
			if(!string.IsNullOrEmpty(documentViewer.SpriteImageUrl))
				Viewer.SpriteImageUrl = documentViewer.SpriteImageUrl;
			if(!string.IsNullOrEmpty(documentViewer.SpriteCssFilePath))
				Viewer.SpriteCssFilePath = documentViewer.SpriteCssFilePath;
			Viewer.LoadingPanelImage.Assign(documentViewer.LoadingPanelImage);
			Viewer.LoadingPanelStyle.Assign(documentViewer.LoadingPanelStyle);
			Viewer.BookmarkSelectionBorder.Assign(viewerStyles.BookmarkSelectionBorder);
			Viewer.Paddings.Assign(viewerStyles.ActualPaddings);
			if(!viewerStyles.Height.IsEmpty)
				Viewer.Height = viewerStyles.Height;
			if(!viewerStyles.Width.IsEmpty)
				Viewer.Width = viewerStyles.Width;
			if(toolbar != null) {
				toolbar.Styles.CopyFrom(documentViewer.StylesToolbar);
				toolbar.MenuStyle.Assign(documentViewer.StylesToolbar.ToolbarMenuStyle);
				RenderUtils.AppendDefaultDXClassName(toolbar, "dxXtraReports_dvToolbar");
				if(documentViewer.StylesToolbar.Alignment == ReportToolbarAlignment.Center) {
					MakeCenteredSplitterContentControlSafe(ToolbarPane);
				}
				toolbar.ComboBoxExtraWidth = documentViewer.StylesToolbar.ToolbarComboBoxExtraWidth;
			}
			if(ribbonToolbar != null) {
				ribbonToolbar.Styles.CopyFrom(documentViewer.StylesRibbon);
				ribbonToolbar.ControlStyle.CopyFrom(documentViewer.StylesRibbon.Control);
				ribbonToolbar.StylesEditors.Assign(documentViewer.StylesEditors);
				var ribbonTabContent = ribbonToolbar.Styles.TabContent;
				ribbonTabContent.CssClass = RenderUtils.CombineCssClasses(ribbonTabContent.CssClass, "dxXtraReports_dvRibbonTabContent");
			}
			DocumentMap.Styles.CopyFrom(documentViewer.StylesDocumentMap);
			DocumentMap.LoadingPanelImagePosition = documentViewer.SettingsLoadingPanel.ImagePosition;
			ParametersPanel.StylesButton.CopyFrom(documentViewer.StylesParametersPanelButtons);
			ParametersPanel.StylesEditors.CopyFrom(documentViewer.StylesEditors);
			ParametersPanel.StylesParameterEditor.CopyFrom(documentViewer.StylesParametersPanelEditors);
			ParametersPanel.ControlButtonWidth = documentViewer.StylesParametersPanelButtons.ControlButtonWidth;
			splitter.Styles.CopyFrom(documentViewer.StylesSplitter);
			CopyStyles(splitter.Style, documentViewer.Style);
			RenderUtils.AppendDefaultDXClassName(splitter, "dxXtraReports_dvSplControl");
			ViewerPane.PaneStyle.CopyFrom(documentViewer.StylesSplitter.Pane);
			SideCenterPane.Size = documentViewer.StylesSplitter.SidePaneWidth;
			SideCenterPane.MinSize = documentViewer.StylesSplitter.SidePaneMinWidth;
			CenterPane.Separator.Size = Unit.Pixel(1);
			CenterPane.Separator.SeparatorStyle.CssClass = "dxSplReportToolbarHSep";
			CenterPane.Separator.SeparatorStyle.Paddings.Padding = 0;
			CopyStyleAndAppendCssClass(ViewerPane.PaneStyle, documentViewer.StylesSplitter.Pane, "dxXtraReports_dvSplitterViewerPane");
			if(ParametersPanelPane != null) {
				CopyStyleAndAppendCssClass(ParametersPanelPane.PaneStyle, documentViewer.StylesSplitter.Pane, "dxXtraReports_dvSplitterParametersPanelPane");
			}
			if(DocumentMapPane != null) {
				DocumentMapPane.AutoHeight = documentViewer.SettingsSplitter.DocumentMapAutoHeight;
				CopyStyleAndAppendCssClass(DocumentMapPane.PaneStyle, documentViewer.StylesSplitter.Pane, "dxXtraReports_dvSplitterDocumentMapPane");
			}
			if(ToolbarPane != null) {
				CopyStyleAndAppendCssClass(ToolbarPane.PaneStyle, documentViewer.StylesSplitter.Pane, "dxmLiteReportToolbarPane");
				ToolbarPane.Size = toolbar != null
					? documentViewer.StylesSplitter.ToolbarPaneHeight
					: documentViewer.StylesSplitter.InternalRibbonToolbarPaneHeight;
				ToolbarPane.MinSize = Unit.Pixel(16);
			}
		}
		protected override void EnsurePreRender() {
			RefreshPanesLayout();
			base.EnsurePreRender();
		}
		void CreateInternalRibbon() {
			ribbonToolbar = new ASPxRibbon {
				ID = "RibbonToolbar",
				ShowFileTab = false,
				ShowTabs = false,
				ShowGroupLabels = documentViewer.SettingsRibbon.ShowGroupLabels,
				EnableViewState = false
			};
			ribbonToolbar.Images.IconSet = documentViewer.SettingsRibbon.IconSet;
			ribbonToolbar.Tabs.Assign(DocumentViewerDefaultRibbon.CreateRibbonTabs());
			ToolbarPane.Controls.Add(ribbonToolbar);
		}
		void CreateToolbar() {
			toolbar = new ReportToolbar {
				ID = "Toolbar",
				ReportViewer = Viewer,
				ParentSkinOwner = documentViewer,
				ShouldDataBindOnLoad = false,
				AccessibilityCompliant = documentViewer.AccessibilityCompliant,
				EnableViewState = false
			};
			toolbar.Items.Assign(documentViewer.ToolbarItems);
			toolbar.ClientSideEvents.Assign(documentViewer.ClientSideEvents);
			toolbar.CaptionSettings.Assign(documentViewer.SettingsToolbar.CaptionSettings);
			toolbar.DropDownEditButtonSettings.Assign(documentViewer.SettingsDropDownEditButton);
			ToolbarPane.Controls.Add(toolbar);
		}
		void CreateSplitter() {
			splitter = new ASPxSplitter {
				ID = "Splitter",
				ParentSkinOwner = documentViewer,
				CssClass = documentViewer.CssClass,
				EnableViewState = false,
				Width = documentViewer.Width,
				Height = documentViewer.Height,
				Orientation = Orientation.Vertical,
				ResizingMode = ResizingMode.Postponed
			};
			if(documentViewer.ToolbarMode != DocumentViewerToolbarMode.None && documentViewer.ToolbarMode != DocumentViewerToolbarMode.ExternalRibbon) {
				splitter.Panes.Add(ToolbarPaneName);
				ToolbarPane.AllowResize = DefaultBoolean.False;
				if(documentViewer.ToolbarMode == DocumentViewerToolbarMode.Ribbon) {
					ToolbarPane.AutoHeight = true;
				} else if(documentViewer.ToolbarMode == DocumentViewerToolbarMode.StandardToolbar) {
					ToolbarPane.PaneStyle.Wrap = DefaultBoolean.False;
				}
			}
			SplitterPane centerPane = splitter.Panes.Add(CenterPaneName);
			centerPane.ShowSeparatorImage = DefaultBoolean.False;
			centerPane.Panes.Add(ViewerPaneName);
			ViewerPane.AutoHeight = documentViewer.Height.IsEmpty;
			ViewerPane.ScrollBars = ScrollBars.Auto;
			centerPane.Panes.Add(SideCenterPaneName);
			SideCenterPane.ShowCollapseBackwardButton = DefaultBoolean.True;
			SideCenterPane.ShowCollapseForwardButton = DefaultBoolean.True;
			SideCenterPane.AllowResize = DefaultBoolean.True;
			SideCenterPane.Panes.Add(ParametersPanelPaneName);
			SideCenterPane.Panes.Add(DocumentMapPaneName);
			if(documentViewer.SettingsSplitter.SidePanePosition == DocumentViewerSidePanePosition.Left) {
				centerPane.Panes.Move(SideCenterPane.Index, ViewerPane.Index);
			}
			ParametersPanelPane.ShowCollapseBackwardButton = DefaultBoolean.True;
			ParametersPanelPane.AutoHeight = documentViewer.AutoHeight;
			ParametersPanelPane.ScrollBars = ScrollBars.Auto;
			DocumentMapPane.ShowCollapseForwardButton = DefaultBoolean.True;
			DocumentMapPane.ScrollBars = ScrollBars.Auto;
			DocumentMapPane.ShowSeparatorImage = DefaultBoolean.False;
		}
		static void CopyStyles(CssStyleCollection dest, CssStyleCollection source) {
			dest.Clear();
			foreach(string key in source.Keys) {
				dest.Add(key, source[key]);
			}
		}
		void RecreateSplitterHierarchy() {
			((IASPxWebControl)splitter).EnsureChildControls();
		}
		void RefreshPanesLayout() {
			if(ToolbarPane != null) {
				ToolbarPane.Visible = documentViewer.ToolbarMode != DocumentViewerToolbarMode.None && documentViewer.ToolbarMode != DocumentViewerToolbarMode.ExternalRibbon;
			}
			ParametersPanelPane.Collapsed = documentViewer.SettingsSplitter.ParametersPanelCollapsed;
			bool parametersPanelVisible = forcedParametersPanelVisible ?? (DesignMode || Viewer.ReportHasVisibleParametersInternal);
			ParametersPanel.Visible = parametersPanelVisible;
			ParametersPanelPane.Visible = parametersPanelVisible;
			bool documentMapVisible = true;
			bool useClientParameters = false;
			if(RemoteMode) {
				useClientParameters = hasRemoteParameters;
			} else {
				if(Viewer.ForcedReport != null) {
					useClientParameters = Viewer.ForcedReport.Parameters.Count > 0;
				}
				if(!Viewer.ShouldRequestParametersFirst) {
					Viewer.ForcePSDocument();
					documentMapVisible = LocalReportHasBookmarks;
				}
			}
			DocumentMapPane.Collapsed = !documentMapVisible || documentViewer.SettingsSplitter.DocumentMapCollapsed || RemoteMode;
			SideCenterPane.Collapsed = !parametersPanelVisible && !documentMapVisible || (RemoteMode && !hasRemoteParameters);
			SideCenterPane.Visible = documentViewer.SettingsSplitter.SidePaneVisible != DocumentViewerSplitterSettings.DefaultVisible
				? documentViewer.SettingsSplitter.SidePaneVisible
				: documentMapVisible || parametersPanelVisible || RemoteMode;
			Viewer.UseClientParameters = useClientParameters;
		}
		Table CreateViewerTable() {
			var pageTable = RenderUtils.CreateTable(false);
			pageTable.CssClass = AppendCssClassToPageShadow("dxXtraReports_PageTable", isSystemClass: true);
			if(documentViewer.IsRightToLeftInternal) {
				RenderUtils.AppendDefaultDXClassName(pageTable, "dxXtraReports_PageTable_Rtl");
			}
			pageTable.Rows.AddRange(new[] { RenderUtils.CreateTableRow(), RenderUtils.CreateTableRow(), RenderUtils.CreateTableRow() });
			var shadowTopLeftCornerCell = RenderUtils.CreateTableCell();
			shadowTopLeftCornerCell.Controls.Add(new Panel { CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_tlc") });
			var shadowTopCell = RenderUtils.CreateTableCell();
			shadowTopCell.CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_t");
			var shadowTopRightCornerCell = RenderUtils.CreateTableCell();
			shadowTopRightCornerCell.Controls.Add(new Panel { CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_trc") });
			var shadowLeftCell = RenderUtils.CreateTableCell();
			shadowLeftCell.CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_l");
			var pageCell = RenderUtils.CreateTableCell();
			pageCell.Controls.Add(Viewer);
			var shadowRightCell = RenderUtils.CreateTableCell();
			shadowRightCell.CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_r");
			var shadowBottomLeftCornerCell = RenderUtils.CreateTableCell();
			shadowBottomLeftCornerCell.Controls.Add(new Panel { CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_blc") });
			var shadowBottomCell = RenderUtils.CreateTableCell();
			shadowBottomCell.CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_b");
			var shadowBottomRightCornerCell = RenderUtils.CreateTableCell();
			shadowBottomRightCornerCell.Controls.Add(new Panel { CssClass = AppendCssClassToPageShadow("dxXtraReports_PageBorder_brc") });
			pageTable.Rows[0].Cells.AddRange(new[] { shadowTopLeftCornerCell, shadowTopCell, shadowTopRightCornerCell });
			pageTable.Rows[1].Cells.AddRange(new[] { shadowLeftCell, pageCell, shadowRightCell });
			pageTable.Rows[2].Cells.AddRange(new[] { shadowBottomLeftCornerCell, shadowBottomCell, shadowBottomRightCornerCell });
			return pageTable;
		}
		string AppendCssClassToPageShadow(string className, bool isSystemClass = false) {
			if(!isSystemClass && !documentViewer.StylesReportViewer.ShowDocumentShadow) {
				return string.Empty;
			}
			var cssPostfix = documentViewer.CssPostfix;
			return !isSystemClass && !string.IsNullOrEmpty(cssPostfix)
				? className + "_" + cssPostfix
				: className;
		}
		protected override void ClearControlFields() {
			toolbar = null;
			UnsubscribeParametersPanel();
			ParametersPanel = null;
			DocumentMap = null;
			UnsubscribeReportViewer();
			Viewer = null;
		}
		void AssignClientSideEvents() {
			const string EventFormat = "function(s, e) {{ ASPx.GetControlCollection().Get('{0}').fire{1}(s, e); }}";
			string jsName = documentViewer.ClientID;
			if(toolbar != null) {
				ReportToolbarClientSideEvents topToolbarEvents = toolbar.ClientSideEvents;
				topToolbarEvents.ItemClick = string.Format(EventFormat, jsName, DocumentViewerClientSideEvents.ToolbarItemClickName);
				topToolbarEvents.ItemValueChanged = string.Format(EventFormat, jsName, DocumentViewerClientSideEvents.ToolbarItemValueChangedName);
			}
		}
		bool TryInitializeRemoteViewing() {
			if(DesignMode) {
				return false;
			}
			if(!TryAssignRemoteMediator()) {
				return false;
			}
			ValidateLocalReportInRemoteMode();
			ValidateRemotePreview();
			Viewer.ReportWebRemoteMediator
				.OnReceiveDocumentData((documentMap, drillDownKeys) => {
					DocumentMap.ResolveBookmarkFiller = () => new RemoteBookmarkFiller(Viewer, documentMap);
					Viewer.AssignRemoteDrillDownKeys(drillDownKeys);
				});
			ConfigureRemoteParametersPanel();
			if(!IsCallback && !Viewer.ShouldRequestParametersFirst) {
				Viewer.EnsureRemoteDocumentInformation();
			}
			return true;
		}
		bool TryAssignRemoteMediator() {
			var isConfiguration = documentViewer.ConfigurationRemoteSource != null;
			var isSettings = documentViewer.SettingsRemoteSource.IsChanged;
			if(!isSettings && !isConfiguration) {
				return false;
			}
			if(isSettings && isConfiguration) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettingsAndConfiguration_Error));
			}
			try {
				DocumentViewerRemoteSourceConfiguration configuration = documentViewer.ConfigurationRemoteSource;
				Viewer.ReportWebRemoteMediator = isSettings
					? reportWebRemoteMediatorFactory.Create(documentViewer.SettingsRemoteSource, () => RSLogin(documentViewer.SettingsRemoteSource), Viewer.CreateClientTokenStorage)
					: new DocumentViewerReportWebRemoteMediator(configuration.ReportServiceClientFactory, configuration.InstanceIdentity);
			} catch {
				Viewer.ReportWebRemoteMediator = null;
				throw;
			}
			return true;
		}
		void ConfigureRemoteParametersPanel() {
			ReportParameterContainer parametersContainer = Viewer.ReportWebRemoteMediator.GetReportParameters();
			var clientParametersContainer = new ClientParameterContainer(parametersContainer);
			documentViewer.SettingsRemoteSource.RaiseRequestParameters(clientParametersContainer);
			ReportParameter[] parameters = parametersContainer.Parameters;
			MergeRemoteParameters(parameters, clientParametersContainer);
			hasRemoteParameters = parameters.Length > 0;
			Viewer.AssignRemoteParameters(parameters, IsCallback);
			var visibleParameters = parameters
				.Where(x => x.Visible)
				.ToArray();
			ParametersPanel.ResolveParametersProvider = x => new RemoteParametersProvider(visibleParameters, x);
			ParametersPanel.ResolveCallbackEventProcessor = () => new RemoteCallbackEventProcessor(Viewer.ReportWebRemoteMediator);
			forcedParametersPanelVisible = visibleParameters.Length > 0;
			Viewer.ForcedShouldRequestParametersFirst = parametersContainer.ShouldRequestParameters && forcedParametersPanelVisible.Value
				&& documentViewer.SettingsSplitter.SidePaneVisible && !documentViewer.SettingsSplitter.ParametersPanelCollapsed;
		}
		static string RSLogin(DocumentViewerRemoteSourceSettings settings) {
			var tcs = new TaskCompletionSource<string>();
			var factory = ClientFactory.CreateAuthFactory(settings);
			var endpointBehavior = new RSEndpointBehavior();
			endpointBehavior.CookieReceived += (_, e) => tcs.TrySetResult(e.CookieValue);
			factory.ChannelFactory.Endpoint.Behaviors.Add(endpointBehavior);
			var client = factory.Create();
			client.SetSynchronizationContext(null);
			var credentials = settings.AuthenticationType == AuthenticationType.Forms
				? settings.GetWebCredentials()
				: new WebCredential(null, null);
			client.Login(credentials.UserName, credentials.Password, null, x => {
				if(x.Cancelled) {
					tcs.TrySetCanceled();
				} else if(x.Error != null) {
					tcs.TrySetException(x.Error);
				} else if(!x.Result) {
					tcs.TrySetException(new SecurityException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorLogin_Error)));
				} else if(!tcs.Task.IsCompleted) {
					tcs.TrySetResult(null);
				}
			});
			return tcs.Task.Result;
		}
		void ValidateRemotePreview() {
			if(!documentViewer.SettingsReportViewer.PageByPage) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemotePageByPage_Error));
			}
		}
		void ValidateLocalReportInRemoteMode() {
			if(documentViewer.Report != null || !string.IsNullOrEmpty(documentViewer.ReportTypeName)) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_LocalAndRemoteSource_Error));
			}
		}
		void InitializeLocalViewing() {
			if(!IsCallback && (!documentViewer.SettingsSplitter.SidePaneVisible || documentViewer.SettingsSplitter.ParametersPanelCollapsed)) {
				Viewer.EnableRequestParameters = false;
			}
			DocumentMap.ResolveBookmarkFiller = () => new ReportDocumentMapFiller(documentViewer);
		}
		static void MergeRemoteParameters(ReportParameter[] parameters, ClientParameterContainer clientParametersContainer) {
			foreach(ReportParameter parameter in parameters) {
				var clientParameter = clientParametersContainer[parameter.Path];
				if(clientParameter == null) {
					continue;
				}
				parameter.Description = clientParameter.Description;
				parameter.MultiValue = clientParameter.MultiValue;
				parameter.Value = clientParameter.Value;
				parameter.Visible = clientParameter.Visible;
			}
		}
		static void CopyStyleAndAppendCssClass(AppearanceStyleBase dest, AppearanceStyleBase source, string appendCssClass) {
			dest.CopyFrom(source);
			dest.CssClass = RenderUtils.CombineCssClasses(dest.CssClass, appendCssClass);
		}
		static void MakeCenteredSplitterContentControlSafe(SplitterPane pane) {
			var collection = pane.ContentCollection;
			if(collection.Count == 0) {
				return;
			}
			ContentControl splitterContentControl = collection[0];
			RenderUtils.AppendDefaultDXClassName(splitterContentControl, "dxXtraReports_dvToolbarSplCC_center");
		}
	}
}
