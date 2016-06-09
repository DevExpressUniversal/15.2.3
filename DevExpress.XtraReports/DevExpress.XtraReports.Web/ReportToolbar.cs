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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.Toolbar;
using MenuItem = DevExpress.Web.MenuItem;
namespace DevExpress.XtraReports.Web {
#if !DEBUG
#endif // DEBUG
	[Designer("DevExpress.XtraReports.Web.Design.ReportToolbarDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[DefaultProperty(Constants.ReportViewer.ReportViewerIDPropertyName)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ReportToolbar.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	[ToolboxData(
		"<{0}:ReportToolbar runat='server' ShowDefaultButtons='False'>" +
		"<Items>" +
		"<{0}:ReportToolbarButton ItemKind='Search' />" +
		"<{0}:ReportToolbarSeparator />" +
		"<{0}:ReportToolbarButton ItemKind='PrintReport' />" +
		"<{0}:ReportToolbarButton ItemKind='PrintPage' />" +
		"<{0}:ReportToolbarSeparator />" +
		"<{0}:ReportToolbarButton Enabled='False' ItemKind='FirstPage' />" +
		"<{0}:ReportToolbarButton Enabled='False' ItemKind='PreviousPage' />" +
		"<{0}:ReportToolbarLabel ItemKind='PageLabel' />" +
		"<{0}:ReportToolbarComboBox ItemKind='PageNumber' Width='65px'></{0}:ReportToolbarComboBox>" +
		"<{0}:ReportToolbarLabel ItemKind='OfLabel' />" +
		"<{0}:ReportToolbarTextBox IsReadOnly='True' ItemKind='PageCount' />" +
		"<{0}:ReportToolbarButton ItemKind='NextPage' />" +
		"<{0}:ReportToolbarButton ItemKind='LastPage' />" +
		"<{0}:ReportToolbarSeparator />" +
		"<{0}:ReportToolbarButton ItemKind='SaveToDisk' />" +
		"<{0}:ReportToolbarButton ItemKind='SaveToWindow' />" +
		"<{0}:ReportToolbarComboBox ItemKind='SaveFormat' Width='70px'>" +
		"<Elements>" +
		"<{0}:ListElement Value='pdf' />" +
		"<{0}:ListElement Value='xls' />" +
		"<{0}:ListElement Value='xlsx' />" +
		"<{0}:ListElement Value='rtf' />" +
		"<{0}:ListElement Value='mht' />" +
		"<{0}:ListElement Value='html' />" +
		"<{0}:ListElement Value='txt' />" +
		"<{0}:ListElement Value='csv' />" +
		"<{0}:ListElement Value='png' />" +
		"</Elements>" +
		"</{0}:ReportToolbarComboBox>" +
		"</Items>" +
		"<Styles>" +
		"<LabelStyle>" +
		"<Margins MarginLeft='3px' MarginRight='3px' />" +
		"</LabelStyle>" +
		"</Styles>" +
		"</{0}:ReportToolbar>")]
	[ToolboxItem(false)]
	public class ReportToolbar : ASPxWebControl, IControlDesigner {
		#region resources
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		const string WebScriptResourcePath = WebResourceNames.WebScriptResourcePath;
		internal const string
			SpriteCssResourceName = WebCssResourcePath + "Sprite.css",
			CssResourceName = WebCssResourcePath + "Default.css",
			ScriptResourceName = WebScriptResourcePath + "ReportToolbar.js";
		#endregion
		internal const string ComboBoxExtraWidthName = "ComboBoxExtraWidth";
		static readonly ReportToolbarItemKind[] ignoreKinds = new[] {
			ReportToolbarItemKind.FirstPage,
			ReportToolbarItemKind.PreviousPage,
			ReportToolbarItemKind.NextPage,
			ReportToolbarItemKind.LastPage
		};
		public static ReportToolbarItemCollection CreateDefaultItemCollection() {
			var items = new ReportToolbarItemCollection();
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.Search));
			items.Add(new ReportToolbarSeparator());
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.PrintReport));
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.PrintPage));
			items.Add(new ReportToolbarSeparator());
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.FirstPage, false));
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.PreviousPage, false));
			items.Add(new ReportToolbarLabel(ReportToolbarItemKind.PageLabel));
			var toolbarComboBox = new ReportToolbarComboBox(ReportToolbarItemKind.PageNumber) { Width = 65 };
			items.Add(toolbarComboBox);
			items.Add(new ReportToolbarLabel(ReportToolbarItemKind.OfLabel));
			items.Add(new ReportToolbarTextBox(ReportToolbarItemKind.PageCount, String.Empty, true));
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.NextPage));
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.LastPage));
			items.Add(new ReportToolbarSeparator());
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.SaveToDisk));
			items.Add(new ReportToolbarButton(ReportToolbarItemKind.SaveToWindow));
			toolbarComboBox = new ReportToolbarComboBox(ReportToolbarItemKind.SaveFormat) { Width = 70 };
			toolbarComboBox.Elements.Assign(ListElementCollection.CreateInstance(ReportToolbarResources.ExportValues));
			items.Add(toolbarComboBox);
			return items;
		}
		#region fields & properties
		object reportViewer = string.Empty;
		string classBase = string.Empty;
		ReportToolbarItemCollection items;
		bool showDefaultButtons = true;
		ASPxMenu menu;
		readonly Dictionary<ReportToolbarTextBox, ASPxTextBox> textBoxes = new Dictionary<ReportToolbarTextBox, ASPxTextBox>();
		readonly Dictionary<ReportToolbarComboBox, ASPxComboBox> comboBoxes = new Dictionary<ReportToolbarComboBox, ASPxComboBox>();
		readonly Dictionary<ReportToolbarLabel, ASPxLabel> labels = new Dictionary<ReportToolbarLabel, ASPxLabel>();
		internal bool ShouldDataBindOnLoad { get; set; }
		#region hidden properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return false; }
			set { }
		}
		#endregion
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category("Client-Side")]
		[DefaultValue("")]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarItemSpacing")]
#endif
		[DefaultValue(typeof(Unit), "")]
		[Category("Layout")]
		public Unit ItemSpacing {
			get { return MenuStyle.ItemSpacing; }
			set { MenuStyle.ItemSpacing = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarPaddings")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Category("Layout")]
		public Paddings Paddings {
			get { return MenuStyle.Paddings; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarSeparatorBackgroundImage")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackgroundImage SeparatorBackgroundImage {
			get { return MenuStyle.SeparatorBackgroundImage; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarSeparatorColor")]
#endif
		[DefaultValue(typeof(Color), "")]
		[Category("Appearance")]
		[TypeConverter(typeof(WebColorConverter))]
		public Color SeparatorColor {
			get { return MenuStyle.SeparatorColor; }
			set { MenuStyle.SeparatorColor = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarSeparatorHeight")]
#endif
		[DefaultValue(typeof(Unit), "")]
		[Category("Layout")]
		public Unit SeparatorHeight {
			get { return MenuStyle.SeparatorHeight; }
			set { MenuStyle.SeparatorHeight = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarSeparatorPaddings")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Layout")]
		public Paddings SeparatorPaddings {
			get { return MenuStyle.SeparatorPaddings; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarSeparatorWidth")]
#endif
		[Category("Layout")]
		[DefaultValue(typeof(Unit), "")]
		public Unit SeparatorWidth {
			get { return MenuStyle.SeparatorWidth; }
			set { MenuStyle.SeparatorWidth = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarImages")]
#endif
		[Category("Images")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportToolbarImages Images {
			get { return (ReportToolbarImages)ImagesInternal; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		[AutoFormatDisable]
		public bool ShowDefaultButtons {
			get { return this.showDefaultButtons; }
			set { this.showDefaultButtons = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarReportViewer")]
#endif
		[DefaultValue("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory(ReportStringId.CatData)]
		[Bindable(true)]
		[AutoFormatDisable()]
		[Browsable(false)]
		public object ReportViewer {
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue(typeof(Unit), "")]
		public Unit ComboBoxExtraWidth {
			get { return GetUnitProperty(ComboBoxExtraWidthName, Unit.Empty); }
			set { SetUnitProperty(ComboBoxExtraWidthName, Unit.Empty, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarItems")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue((string)null)]
		[MergableProperty(false)]
		[AutoFormatDisable()]
		public ReportToolbarItemCollection Items {
			get {
				if(items == null) {
					items = new ReportToolbarItemCollection(this);
				}
				return this.items;
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarClientSideEvents")]
#endif
		[MergableProperty(false)]
		[Category("Client-Side")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		public ReportToolbarClientSideEvents ClientSideEvents {
			get { return (ReportToolbarClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected ReportViewer ReportViewerInternal {
			get {
				ReportViewer obj = reportViewer as ReportViewer;
				if(obj != null && obj.Visible)
					return obj;
				if(!string.IsNullOrEmpty(ReportViewerID) && Page != null) {
					var viewer = (ReportViewer)FindControlHelper.LookupControlRecursive(Page, ReportViewerID);
					if(viewer != null && viewer.Visible)
						return viewer;
				}
				return null;
			}
		}
		protected string ClassBase {
			get {
				if(classBase.Length == 0)
					classBase = "dx" + Regex.Replace(ClientID, "_", "");
				return classBase;
			}
		}
		internal ReportToolbarMenuStyle MenuStyle {
			get { return (ReportToolbarMenuStyle)base.ControlStyle; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarStyles")]
#endif
		[Category("Styles")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportToolbarStyles Styles {
			get { return (ReportToolbarStyles)StylesInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarAccessibilityCompliant")]
#endif
		[Category("Accessibility")]
		[DefaultValue(false)]
		[AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarCaptionSettings")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ReportToolbarEditorCaptionSettings CaptionSettings { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarDropDownEditButtonSettings")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DropDownButton DropDownEditButtonSettings { get; private set; }
		string IControlDesigner.DesignerType {
			get { return "DevExpress.Web.Design.Reports.Toolbar.ReportToolbarCommonFormDesigner"; }
		}
		protected virtual string ReportViewerClientID {
			get { return ReportViewerInternal != null ? ReportViewerInternal.ClientID : null; }
		}
		#endregion
		public ReportToolbar() {
			ShouldDataBindOnLoad = true;
			DropDownEditButtonSettings = new DropDownButton(this);
			CaptionSettings = new ReportToolbarEditorCaptionSettings(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ReportToolbarClientSideEvents();
		}
		protected override Style CreateControlStyle() {
			return new ReportToolbarMenuStyle();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] {
					Items,
					DropDownEditButtonSettings,
					CaptionSettings
				});
		}
		protected override ImagesBase CreateImages() {
			return new ReportToolbarImages(this);
		}
		protected override string GetSkinControlName() {
			return "XtraReports";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Web", "Editors" };
		}
		protected override bool HasRenderCssFile() {
			return false;
		}
		public override void RegisterStyleSheets() {
			EnsureChildControls();
			((ToolbarASPxMenu)menu).BaseRegisterStyleSheets();
			base.RegisterStyleSheets();
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!DesignMode) {
				writer = new HtmlTextWriter(writer.InnerWriter);
			}
			base.Render(writer);
		}
		protected override StylesBase CreateStyles() {
			return new ReportToolbarStyles(this);
		}
		protected override void OnLoad(EventArgs e) {
			if(ShouldDataBindOnLoad) {
				DataBind();
			}
			base.OnLoad(e);
		}
		void FillMenu(ASPxMenu menu, ReportToolbarItemCollection items) {
			for(int i = 0; i < items.Count; i++) {
				ReportToolbarItem item = items[i];
				if(item is ReportToolbarSeparator || CanConvertToCaption(item)) {
					continue;
				}
				var menuItem = new MenuItem { Name = item.Name };
				menu.Items.Add(menuItem);
				if(item.ItemKind == ReportToolbarItemKind.Custom && menuItem.Name == item.ItemKind.ToString())
					menuItem.Name += i;
				if(i > 0 && (items[i - 1] is ReportToolbarSeparator))
					menuItem.BeginGroup = true;
				if(item is ReportToolbarButton) {
					ReportToolbarButton button = (ReportToolbarButton)item;
					DisableSaveToWindowIfMobileBrowser(button);
					menuItem.Image.Assign(button.GetImageProperties(Page, Images));
					menuItem.Text = button.Text;
					menuItem.ToolTip = GetLocalizedText(button.ItemKind, button.ToolTip);
					if(!menuItem.Image.IsEmpty && string.IsNullOrEmpty(menuItem.Image.AlternateText))
						menuItem.Image.AlternateText = menuItem.ToolTip;
					if(Array.IndexOf(ignoreKinds, button.ItemKind) < 0) {
						menuItem.Enabled = button.Enabled;
					}
				} else if(item is ReportToolbarLabel) {
					var label = (ReportToolbarLabel)item;
					menuItem.Template = new WrappedTemplate(CreateAspLabel(menuItem.Name, label));
				} else if(item is ReportToolbarTextBox) {
					var textBox = (ReportToolbarTextBox)item;
					menuItem.Template = new WrappedTemplate(CreateAspTextBox(menuItem.Name, textBox));
				} else if(item is ReportToolbarComboBox) {
					var aspxComboBox = (ASPxComboBox)CreateAspComboBox(menuItem.Name, (ReportToolbarComboBox)item);
					aspxComboBox.DropDownButton.Assign(DropDownEditButtonSettings);
					menuItem.Template = new WrappedTemplate(aspxComboBox);
				} else if(item is ReportToolbarTemplateItem) {
					var templateItem = (ReportToolbarTemplateItem)item;
					menuItem.Template = templateItem.Template;
				}
			}
			UpdateLabelTargets(menu, items);
			var scriptBuider = new StringBuilder();
			scriptBuider.AppendLine("function(s, e) {");
			scriptBuider.AppendLine("ASPx.GetControlCollection().Get('" + ClientID + "').handleButton(e.item.name);"); 
			if(!IsEmptyString(ClientSideEvents.ItemClick)) {
				scriptBuider.AppendLine("var f = " + ClientSideEvents.ItemClick);
				scriptBuider.AppendLine("f(s, e);");
			}
			scriptBuider.AppendLine("}");
			menu.ClientSideEvents.ItemClick = scriptBuider.ToString();
		}
		void UpdateLabelTargets(ASPxMenu menu, ReportToolbarItemCollection items) {
			foreach(ReportToolbarLabel labelItem in labels.Keys) {
				string associatedItemName = labelItem.AssociatedItemName;
				if(string.IsNullOrEmpty(associatedItemName)) {
					associatedItemName = GetDefaultAssociatedItemName(labelItem);
				}
				if(string.IsNullOrEmpty(associatedItemName))
					continue;
				ReportToolbarItem item = items.FirstOrDefault(x => x.Name == associatedItemName);
				if(item == null)
					continue;
				ASPxLabel label = labels[labelItem];
				string targetId = null;
				if(item is ReportToolbarTextBox)
					targetId = textBoxes[(ReportToolbarTextBox)item].ID;
				else if(item is ReportToolbarComboBox)
					targetId = comboBoxes[(ReportToolbarComboBox)item].ID;
				if(string.IsNullOrEmpty(targetId))
					continue;
				Control controlWrapper = new AssociatedControlWrapper(targetId + "_WR", FindChildControl(menu, targetId));
				label.Parent.Controls.Add(controlWrapper);
				label.AssociatedControlID = controlWrapper.ID;
			}
		}
		protected override void ClearControlFields() {
			menu = null;
			textBoxes.Clear();
			comboBoxes.Clear();
			labels.Clear();
		}
		protected override void CreateControlHierarchy() {
			menu = new ToolbarASPxMenu(this) { ID = "Menu" };
			Controls.Add(menu);
			if(Items.Count > 0) {
				FillMenu(menu, Items);
			} else if(ShowDefaultButtons) {
				FillMenu(menu, CreateDefaultItemCollection());
			} else {
				var menuItem = new MenuItem("", "EmptyItem") { Enabled = false };
				menu.Items.Add(menuItem);
			}
		}
		AppearanceStyleBase CreateAppearanceStyle(AppearanceStyleBase sourceStyle) {
			var style = (AppearanceStyleBase)Activator.CreateInstance(sourceStyle.GetType());
			style.CopyFontFrom(ControlStyle);
			style.CopyFrom(sourceStyle);
			return style;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			base.RegisterIncludeScript(typeof(ReportToolbar), ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(ReportViewerClientID)) {
				stb.AppendLine(localVarName + ".reportViewerID = '" + ReportViewerClientID + "';");
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientReportToolbar";
		}
		#region PrepareControlHierarchy
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			int pageCount = 1;
			if(ReportViewerInternal != null) {
				var report = ReportViewerInternal.Report;
				Document document = report != null ? report.PrintingSystem.Document : null;
				pageCount = document != null ? Math.Max(1, document.PageCount) : pageCount;
			}
			foreach(var pair in comboBoxes) {
				PrepareComboBoxHierarchy(pair.Key, pair.Value, pageCount);
			}
			foreach(var pair in textBoxes) {
				PrepareTextBoxHierarchy(pair.Key, pair.Value, pageCount);
			}
			foreach(var pair in labels) {
				ASPxLabel aspLabel = pair.Value;
				((AppearanceStyleBase)aspLabel.ControlStyle).Assign(CreateAppearanceStyle(Styles.LabelStyle));
				Styles.LabelStyle.Margins.AssignToControl(aspLabel);
			}
			menu.ShowAsToolbar = true;
			menu.ApplyItemStyleToTemplates = true;
			RenderUtils.AssignAttributes(this, menu, true);
			((AppearanceStyleBase)menu.ControlStyle).Assign(MenuStyle);
			((AppearanceStyleBase)menu.ItemStyle).Assign(Styles.ButtonStyle);
			menu.AccessibilityCompliant = AccessibilityCompliant;
			foreach(MenuItem menuItem in menu.Items) {
				PrepareMenuItemHeirarchy(menuItem);
			}
		}
		void PrepareComboBoxHierarchy(ReportToolbarComboBox item, ASPxComboBox aspComboBox, int pageCount) {
			((AppearanceStyleBase)aspComboBox.ControlStyle).Assign(CreateAppearanceStyle(Styles.ComboBoxStyle));
			Styles.ComboBoxStyle.Margins.AssignToControl(aspComboBox);
			Styles.ComboBoxStyle.AssignCellSpacingToControl(aspComboBox);
			aspComboBox.ItemStyle.Assign(CreateAppearanceStyle(Styles.ComboBoxItemStyle));
			aspComboBox.ButtonStyle.Assign(CreateAppearanceStyle(Styles.ComboBoxButtonStyle));
			aspComboBox.ListBoxStyle.Assign(CreateAppearanceStyle(Styles.ComboBoxListStyle));
			aspComboBox.CaptionCellStyle.Assign(Styles.CaptionCellStyle);
			aspComboBox.CaptionStyle.Assign(Styles.CaptionStyle);
			if(!item.Width.IsEmpty) {
				aspComboBox.Width = !ComboBoxExtraWidth.IsEmpty && item.Width.Type == ComboBoxExtraWidth.Type
					? new Unit(item.Width.Value + ComboBoxExtraWidth.Value, item.Width.Type)
					: item.Width;
			}
			if(item.ItemKind == ReportToolbarItemKind.PageNumber) {
				FillComboBox(aspComboBox, ToStringArray(pageCount));
				aspComboBox.ClientSideEvents.ValueChanged = "function(s, e) { ASPx.GetControlCollection().Get('" + ClientID + "').handleButton('" + aspComboBox.ID + "'); }"; 
			}
			if(item.ItemKind == ReportToolbarItemKind.SaveFormat) {
				var dropDownRows = Math.Max(1, aspComboBox.Items.Count);
				aspComboBox.DropDownRows = dropDownRows;
			}
		}
		void PrepareTextBoxHierarchy(ReportToolbarTextBox item, ASPxTextBox aspTextBox, int pageCount) {
			((AppearanceStyleBase)aspTextBox.ControlStyle).Assign(CreateAppearanceStyle(Styles.TextBoxStyle));
			Styles.TextBoxStyle.Margins.AssignToControl(aspTextBox);
			aspTextBox.CaptionCellStyle.Assign(Styles.CaptionCellStyle);
			aspTextBox.CaptionStyle.Assign(Styles.CaptionStyle);
			if(!item.Width.IsEmpty)
				aspTextBox.Width = item.Width;
			if(item.ItemKind == ReportToolbarItemKind.PageCount) {
				aspTextBox.HorizontalAlign = HorizontalAlign.Center;
				aspTextBox.ReadOnly = true;
				aspTextBox.Value = pageCount.ToString();
			}
		}
		void PrepareMenuItemHeirarchy(MenuItem menuItem) {
			var tempalte = menuItem.Template as WrappedTemplate;
			if(tempalte == null) {
				return;
			}
			var itemStyle = (AppearanceStyleBase)menuItem.ItemStyle;
			var wrapControl = tempalte.WrapControl;
			if(wrapControl is ASPxComboBox || wrapControl is ASPxTextBox) {
				itemStyle.Assign(Styles.EditorStyle);
				itemStyle.CssClass = MenuStyles.ToolbarComboBoxCssClass;
			}
			if(wrapControl is ASPxLabel) {
				itemStyle.Assign(Styles.EditorStyle);
				itemStyle.CssClass = MenuStyles.ToolbarLabelCssClass;
			}
		}
		#endregion PrepareControlHierarchy
		Control CreateAspLabel(string name, ReportToolbarLabel toolbarLabel) {
			var aspLabel = new ASPxLabel {
				ParentSkinOwner = menu,
				Text = GetLocalizedText(toolbarLabel.ItemKind, toolbarLabel.Text),
				EncodeHtml = false
			};
			ClientIDHelper.EnableClientIDGeneration(aspLabel);
			labels.Add(toolbarLabel, aspLabel);
			aspLabel.ID = name;
			return aspLabel;
		}
		void FillComboBox(ASPxComboBox aspComboBox, string[] values) {
			foreach(string val in values)
				aspComboBox.Items.Add(new ListEditItem(val, val));
		}
		Control CreateAspTextBox(string name, ReportToolbarTextBox toolbarTextBox) {
			var aspTextBox = new ToolbarASPxTextBox();
			ClientIDHelper.EnableClientIDGeneration(aspTextBox);
			aspTextBox.ParentSkinOwner = menu;
			aspTextBox.EnableClientSideAPI = true;
			textBoxes.Add(toolbarTextBox, aspTextBox);
			aspTextBox.Size = 1;
			if(toolbarTextBox.ItemKind == ReportToolbarItemKind.PageCount) {
				if(IsPrevItemKindOf(toolbarTextBox, ReportToolbarItemKind.OfLabel)) {
					string prevLabelText = GetPrevItemText(toolbarTextBox);
					aspTextBox.Caption = string.IsNullOrEmpty(prevLabelText) ? GetLocalizedText(ReportToolbarItemKind.OfLabel, string.Empty) : prevLabelText;
					aspTextBox.CaptionSettings.Assign(CaptionSettings);
				}
				aspTextBox.EditAreaWidth = new Unit(1, UnitType.Em);
				aspTextBox.ID = toolbarTextBox.ItemKind.ToString();
				aspTextBox.Width = new Unit(1, UnitType.Em);
			} else {
				aspTextBox.Text = toolbarTextBox.Text;
				aspTextBox.Value = toolbarTextBox.Text;
				aspTextBox.ReadOnly = toolbarTextBox.IsReadOnly;
				aspTextBox.ID = name;
			}
			return aspTextBox;
		}
		Control CreateAspComboBox(string name, ReportToolbarComboBox toolbarComboBox) {
			var aspComboBox = new ASPxComboBox {
				ParentSkinOwner = menu
			};
			ClientIDHelper.EnableClientIDGeneration(aspComboBox);
			aspComboBox.ID = name;
			comboBoxes.Add(toolbarComboBox, aspComboBox);
			if(toolbarComboBox.ItemKind == ReportToolbarItemKind.PageNumber && IsPrevItemKindOf(toolbarComboBox, ReportToolbarItemKind.PageLabel)) {
				string prevLabelText = GetPrevItemText(toolbarComboBox);
				aspComboBox.Caption = string.IsNullOrEmpty(prevLabelText) ? GetLocalizedText(ReportToolbarItemKind.PageLabel, string.Empty) : prevLabelText;
				aspComboBox.CaptionSettings.Assign(CaptionSettings);
			} else {
				foreach(ListElement elem in toolbarComboBox.Elements) {
					string elementText = toolbarComboBox.ItemKind == ReportToolbarItemKind.SaveFormat
						? GetLocalizedExportName(elem.Value, elem.Text)
						: elem.Text;
					aspComboBox.Items.Add(new ListEditItem(elementText, elem.Value, elem.ImageUrl));
				}
				if(toolbarComboBox.ItemKind != ReportToolbarItemKind.SaveFormat && !IsEmptyString(ClientSideEvents.ItemValueChanged))
					aspComboBox.ClientSideEvents.ValueChanged = ClientSideEvents.ItemValueChanged;
				if(toolbarComboBox.Elements.Count > 0)
					aspComboBox.SelectedIndex = 0;
			}
			return aspComboBox;
		}
		static void DisableSaveToWindowIfMobileBrowser(ReportToolbarButton button) {
			var userAgent = Browser.UserAgent;
			var isMobileBrowser = string.IsNullOrEmpty(userAgent)
				? false
				: (Browser.UserAgent.IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) > 0 || Browser.UserAgent.IndexOf("Android", StringComparison.OrdinalIgnoreCase) > 0);
			if(button.ItemKind == ReportToolbarItemKind.SaveToWindow && isMobileBrowser) {
				button.Enabled = false;
			}
		}
		static bool CanConvertToCaption(ReportToolbarItem item) {
			var collection = item.Collection as ReportToolbarItemCollection;
			if(collection == null) {
				return false;
			}
			var nextIndex = item.Index + 1;
			if(nextIndex >= collection.Count) {
				return false;
			}
			var nextItem = collection[nextIndex];
			return (item.ItemKind == ReportToolbarItemKind.PageLabel && nextItem.ItemKind == ReportToolbarItemKind.PageNumber)
				|| (item.ItemKind == ReportToolbarItemKind.OfLabel && nextItem.ItemKind == ReportToolbarItemKind.PageCount);
		}
		static string GetPrevItemText(ReportToolbarItem toolbarEditor) {
			ReportToolbarItemCollection collection = toolbarEditor.Collection as ReportToolbarItemCollection;
			if(collection == null)
				return "";
			int prevItemIndex = toolbarEditor.Index - 1;
			if(prevItemIndex < 0)
				return "";
			ReportToolbarLabel prevItem = collection[prevItemIndex] as ReportToolbarLabel;
			if(prevItem == null)
				return "";
			return prevItem.Text;
		}
		static bool IsPrevItemKindOf(ReportToolbarItem toolbarEditor, ReportToolbarItemKind itemKind) {
			var collection = toolbarEditor.Collection as ReportToolbarItemCollection;
			if(collection == null)
				return false;
			int prevItemIndex = toolbarEditor.Index - 1;
			if(prevItemIndex < 0)
				return false;
			var prevItem = collection[prevItemIndex];
			return prevItem.ItemKind == itemKind;
		}
		static string[] ToStringArray(int count) {
			string[] items = new string[count];
			for(int i = 0; i < count; i++)
				items[i] = Convert.ToString(i + 1, 10);
			return items;
		}
		static string GetLocalizedText(ReportToolbarItemKind itemKind, string assingnedText) {
			if(itemKind == ReportToolbarItemKind.Custom || !string.IsNullOrEmpty(assingnedText))
				return assingnedText;
			var stringId = (ASPxReportsStringId)Enum.Parse(typeof(ASPxReportsStringId), ReportToolbarResources.ToolBarItemTextPrefix + itemKind);
			return ASPxReportsLocalizer.GetString(stringId);
		}
		static string GetLocalizedExportName(string value, string assingnedText) {
			if(!string.IsNullOrEmpty(assingnedText))
				return assingnedText;
			var stringId = (ASPxReportsStringId)Enum.Parse(typeof(ASPxReportsStringId), ReportToolbarResources.ExportNamePrefix + value);
			return ASPxReportsLocalizer.GetString(stringId);
		}
		static Control FindChildControl(Control parent, string id) {
			foreach(Control control in parent.Controls) {
				if(control.ID == id)
					return control;
				Control child = FindChildControl(control, id);
				if(child != null)
					return child;
			}
			return null;
		}
		static string GetDefaultAssociatedItemName(ReportToolbarLabel labelItem) {
			if(labelItem.ItemKind == ReportToolbarItemKind.PageLabel) {
				return ReportToolbarItemKind.PageNumber.ToString();
			} else {
				return labelItem.ItemKind == ReportToolbarItemKind.OfLabel
					? ReportToolbarItemKind.PageCount.ToString()
					: null;
			}
		}
		static bool IsEmptyString(string s) {
			return string.IsNullOrEmpty(s) || string.IsNullOrEmpty(s.Trim());
		}
	}
}
