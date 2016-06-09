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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	[ToolboxItem(false)
	]
	public class HtmlEditorRibbon : ASPxRibbon {
		public ASPxHtmlEditor HtmlEditor { get; protected set; }
		public HtmlEditorRibbon(ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
		}
	}
	[ToolboxItem(false)]
	public class HtmlEditorBarDockControl : ASPxWebControl {
		protected internal const string BarDockScriptResourceName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "BarDockControl.js";
		protected const string HtmlEditorRibbonID = "T0";
		protected const string ToolbarCommandHandlerName = "ASPx.ToolbarCommand(s, e.item, e.value)";
		protected const string ToolbarDropDownItemBeforeFocusHandlerName = "ASPx.ToolbarDropDownItemBeforeFocus(s, e)";
		protected const string ToolbarDropDownItemCloseUpHandlerName = "ASPx.ToolbarDropDownItemCloseUp(s, e)";
		protected const string ToolbarCustomComboBoxInitHandlerName = "ASPx.ToolbarCustomComboBoxInit(s, e)";
		protected internal ASPxHtmlEditor htmlEditor;
		HtmlEditorToolbarCollection toolbars = null;
		BarDockRenderControl renderControl = null;
		HtmlEditorEditorStyles editorStyles = null;
		EditorImages editorImages = null;
		protected internal HtmlEditorBarDockControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
			ParentSkinOwner = htmlEditor;
			this.htmlEditor = htmlEditor;
		}
		[Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public BarDocControlClientSideEvents ClientSideEvents {
			get { return (BarDocControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public HtmlEditorToolbarCollection Toolbars {
			get {
				if(toolbars == null)
					toolbars = new HtmlEditorToolbarCollection(this);
				return toolbars;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ToolbarStyle ToolbarStyle {
			get { return Styles.Toolbar; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ToolbarItemStyle ToolbarItemStyle {
			get { return Styles.ToolbarItem; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BarDockStyle BarDockStyle {
			get { return Styles.BarDockControl; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HtmlEditorEditorStyles EditorStyles {
			get {
				if(editorStyles == null)
					editorStyles = new HtmlEditorEditorStyles(this);
				return editorStyles;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HtmlEditorRibbonStyles StylesRibbon {
			get { return Styles.Ribbon; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EditorImages EditorImages {
			get {
				if (editorImages == null)
					editorImages = new EditorImages(this);
				return editorImages;
			}
		}
		protected HtmlEditorIconImages Images {
			get { return BarDockHtmlEditor.Images.IconImages; }
		}
		protected internal ASPxHtmlEditor BarDockHtmlEditor {
			get { return this.htmlEditor; }
		}
		protected internal BarDockRenderControl BarDockRender { get { return renderControl; } }
		protected ToolbarsStyles Styles { get { return StylesInternal as ToolbarsStyles; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new BarDocControlClientSideEvents();
		}
		protected override Style CreateControlStyle() {
			return new BarDockStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ToolbarsStyles(this);
		}
		static object barDockStyleKey = new object();
		protected internal BarDockStyle GetBarDockStyle() {
			return (BarDockStyle)CreateStyle(delegate() {
				BarDockStyle style = Styles.GetDefaultBarDockStyle();
				style.CopyFrom(Styles.BarDockControl);
				return style;
			}, barDockStyleKey);
		}
		static object toolbarStyleKey = new object();
		protected internal ToolbarStyle GetToolbarStyle() {
			return (ToolbarStyle)CreateStyle(delegate() {
				ToolbarStyle style = new ToolbarStyle();
				style.CopyFontFrom(GetBarDockStyle());
				style.CopyFrom(Styles.Toolbar);
				return style;
			}, toolbarStyleKey);
		}
		static object toolbarItemStyleKey = new object();
		protected internal ToolbarItemStyle GetToolbarItemStyle() {
			return (ToolbarItemStyle)CreateStyle(delegate() {
				ToolbarItemStyle style = new ToolbarItemStyle();
				style.CopyFrom(Styles.ToolbarItem);
				return style;
			}, toolbarItemStyleKey);
		}
		static object comboBoxToolbarItemStyleKey = new object();
		protected internal AppearanceStyle GetComboBoxToolbarItemStyle() {
			return (AppearanceStyle)CreateStyle(delegate() {
				return Styles.GetDefaultComboBoxToolbarItemStyle();
			}, comboBoxToolbarItemStyleKey);
		}
		protected internal AppearanceStyle GetToolbarSpacingStyle() {
			AppearanceStyle style = Styles.GetDefaultToolbarSpacingStyle();
			if(!GetBarDockStyle().ToolbarSpacing.IsEmpty)
				style.Height = GetBarDockStyle().ToolbarSpacing;
			return style;
		}
		protected internal EditorImages GetToolbarEditorImages() {
			return this.EditorImages;
		}
		protected internal ImageProperties GetPopOutImage() {
			return BarDockHtmlEditor.Images.GetImageProperties(Page, HtmlEditorImages.ToolbarPopOutImageName);
		}
		protected internal ToolbarItemImageProperties GetImageProperties(string resourceImageName) {
			ToolbarItemImageProperties imageProps = new ToolbarItemImageProperties();
			imageProps.CopyFrom(Images.GetImageProperties(Page, resourceImageName));
			return imageProps;
		}
		protected override void ClearControlFields() {
			this.renderControl = null;
		}
		protected override void CreateControlHierarchy() {
			if(BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.None || BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon)
				return;
			this.renderControl = new BarDockRenderControl(this, BarDockHtmlEditor);
			Controls.Add(this.renderControl);
		}
		protected internal string GetToolbarCommand() {
			return RenderUtils.CreateClientEventHandler(ToolbarCommandHandlerName);
		}
		protected internal string GetToolbarDropDownItemBeforeFocus() {
			return RenderUtils.CreateClientEventHandler(ToolbarDropDownItemBeforeFocusHandlerName);
		}
		protected internal string GetToolbarDropDownItemCloseUp() {
			return RenderUtils.CreateClientEventHandler(ToolbarDropDownItemCloseUpHandlerName);
		}
		protected internal string GetToolbarID(HtmlEditorToolbar toolbar) {
			return "T" + GetToolbarVisibleIndex(toolbar);
		}
		protected internal string GetToolbarCustomComboBoxInit() {
			return RenderUtils.CreateClientEventHandler(ToolbarCustomComboBoxInitHandlerName);
		}
		protected int GetToolbarVisibleIndex(HtmlEditorToolbar toolbar) {
			int index = toolbar.VisibleIndex;
			int ret = index;
			for (int i = index - 1; i >= 0; i--) {
				HtmlEditorToolbar curToolbar = Toolbars.GetVisibleItem(i);
				if (curToolbar.Items.Count <= 0)
					ret--;
			}
			return ret;
		}
		protected internal bool IsSaveSelectionBeforeFocusNeeded() {
			return Browser.IsIE;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(HtmlEditorBarDockControl), BarDockScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return BarDockHtmlEditor.ToolbarMode != HtmlEditorToolbarMode.ExternalRibbon && GetVisibleToolbarsCount() == 0 ? "" : "ASPx.HtmlEditorClasses.Controls.BarDockControl";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(htmlEditor.ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon) {
				ASPxRibbon ribbon = RibbonHelper.LookupRibbonControl(this, BarDockHtmlEditor.AssociatedRibbonID);
				stb.AppendFormat("{0}.extToolbarID = '{1}';", localVarName, IsMvcRender() ? BarDockHtmlEditor.AssociatedRibbonID : ribbon != null ? ribbon.ClientID : string.Empty);
			}
			else
				stb.AppendFormat("{0}.innerToolbarsCount = {1};", localVarName, GetVisibleToolbarsCount());
		}
		protected internal int GetVisibleToolbarsCount() {
			int count = 0;
			if(BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Ribbon)
				return 1;
			else {
				for(int i = 0; i < Toolbars.GetVisibleItemCount(); i++) {
					HtmlEditorToolbar toolbar = Toolbars.GetVisibleItem(i);
					if(toolbar.IsHasRenderItems)
						count++;
				}
			}
			return count;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Toolbars, BarDockStyle, EditorImages, EditorStyles, ToolbarStyle, ToolbarItemStyle, StylesRibbon });
		}
		protected internal void CreateDefaultToolbars() {
			if(BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu && Toolbars.Count == 0)
				Toolbars.CreateDefaultToolbars(BarDockHtmlEditor.IsRightToLeft());
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(!DesignMode)
				CreateDefaultToolbars();
		}
		public class BarDockRenderControl : ASPxInternalWebControl {
			private HtmlEditorBarDockControl barDock = null;
			private Dictionary<ToolbarControl, HtmlEditorToolbar> toolbarDictionary = null;
			private Table mainTable = null;
			private TableCell mainCell = null;
			private ArrayList separators = null;
			private Table table = null;
			ASPxHtmlEditor HtmlEditor = null;
			public BarDockRenderControl(HtmlEditorBarDockControl barDock, ASPxHtmlEditor htmlEditor) {
				this.barDock = barDock;
				HtmlEditor = htmlEditor;
			}
			protected HtmlEditorBarDockControl BarDock {
				get { return barDock; }
			}
			protected Table MainTable {
				get { return mainTable; }
			}
			protected TableCell MainCell {
				get { return mainCell; }
			}
			protected ArrayList Separators {
				get {
					if(separators == null)
						separators = new ArrayList();
					return separators;
				}
			}
			protected Table Table {
				get { return table; }
			}
			protected internal Dictionary<ToolbarControl, HtmlEditorToolbar> ToolbarDictionary {
				get {
					if(toolbarDictionary == null)
						toolbarDictionary = new Dictionary<ToolbarControl, HtmlEditorToolbar>();
					return toolbarDictionary;
				}
			}
			protected internal HtmlEditorRibbon RibbonControl { get; set; }
			protected override void ClearControlFields() {
				Separators.Clear();
				ToolbarDictionary.Clear();
				this.mainTable = null;
				this.mainCell = null;
				this.table = null;
			}
			protected override void CreateControlHierarchy() {
				Controls.Add(CreateMainTable());
				if(BarDock.BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu)
					MainCell.Controls.Add(CreateToolbarsTable());
				else {
					RibbonControl = CreateRibbon();
					MainCell.Controls.Add(RibbonControl);
					RibbonControl.Images.IconSet = BarDock.BarDockHtmlEditor.GetIconSet();
				}
			}
			protected Table CreateMainTable() {
				this.mainTable = RenderUtils.CreateTable(true);
				MainTable.Rows.Add(RenderUtils.CreateTableRow());
				this.mainCell = RenderUtils.CreateTableCell();
				MainTable.Rows[0].Cells.Add(MainCell);
				return MainTable;
			}
			protected Table CreateToolbarsTable() {
				this.table = RenderUtils.CreateTable(true);
				bool isFirstToolbar = true;
				for(int i = 0; i < BarDock.Toolbars.GetVisibleItemCount(); i++) {
					HtmlEditorToolbar toolbar = BarDock.Toolbars.GetVisibleItem(i);
					if(toolbar.Items.Count > 0) {
						if(!isFirstToolbar)
							CreateToolbarSeparator();
						ToolbarDictionary.Add(CreateToolbar(toolbar), toolbar);
						isFirstToolbar = false;
					}
				}
				return Table;
			}
			protected ToolbarControl CreateToolbar(HtmlEditorToolbar toolbar) {
				ToolbarControl toolbarControl = CreateToolbarControl(toolbar);
				TableRow row = RenderUtils.CreateTableRow();
				TableCell cell = RenderUtils.CreateTableCell();
				Table.Rows.Add(row);
				row.Cells.Add(cell);
				cell.Controls.Add(toolbarControl);
				return toolbarControl;
			}
			protected ToolbarControl CreateToolbarControl(HtmlEditorToolbar toolbar) {
				ToolbarControl toolbarControl = new ToolbarControl(BarDock, toolbar);
				toolbarControl.EncodeHtml = BarDock.EncodeHtml;
				toolbarControl.ID = BarDock.GetToolbarID(toolbar);
				toolbarControl.AutoSeparators = AutoSeparatorMode.None;
				toolbarControl.BorderBetweenItemAndSubMenu = BorderBetweenItemAndSubMenuMode.HideAll;
				toolbarControl.ShowPopOutImages = DefaultBoolean.True;
				toolbarControl.HorizontalPopOutImage.Assign(BarDock.GetPopOutImage());
				return toolbarControl;
			}
			protected void CreateToolbarSeparator() {
				TableRow row = RenderUtils.CreateTableRow();
				TableCell cell = RenderUtils.CreateTableCell();
				Table.Rows.Add(row);
				row.Cells.Add(cell);
				Separators.Add(cell);
			}
			protected HtmlEditorRibbon CreateRibbon() {
				HtmlEditorRibbon ribbonControl = new HtmlEditorRibbon(HtmlEditor);
				ribbonControl.ID = HtmlEditorRibbonID;
				ribbonControl.Theme = BarDock.BarDockHtmlEditor.Theme;
				ribbonControl.ClientSideEvents.CommandExecuted = BarDock.BarDockHtmlEditor.RenderHelper.Scripts.RibbonCommandHandler;
				ribbonControl.ClientSideEvents.MinimizationStateChanged = BarDock.BarDockHtmlEditor.RenderHelper.Scripts.RibbonMinimizationStateChangedHandler;
				ribbonControl.ClientSideEvents.ActiveTabChanged = BarDock.BarDockHtmlEditor.RenderHelper.Scripts.RibbonActiveTabChanged;
				ribbonControl.ClientLayout += BarDock.BarDockHtmlEditor.OnRibbonClientLayout;
				ribbonControl.ShowFileTab = false;
				ribbonControl.ActiveTabIndex = 0;
				ribbonControl.EncodeHtml = BarDock.EncodeHtml;
				HtmlEditorDefaultRibbon ribbonHelper = new HtmlEditorDefaultRibbon(BarDock.BarDockHtmlEditor);
				if(ribbonControl.Tabs.IsEmpty) {
					if(BarDock.BarDockHtmlEditor.RibbonTabs.IsEmpty)
						ribbonControl.Tabs.AddRange(ribbonHelper.DefaultRibbonTabs);
					else
						ribbonControl.Tabs.Assign(BarDock.BarDockHtmlEditor.RibbonTabs);
				}
				if(ribbonControl.ContextTabCategories.IsEmpty) {
					if(BarDock.BarDockHtmlEditor.RibbonContextTabCategories.IsEmpty)
						ribbonControl.ContextTabCategories.AddRange(ribbonHelper.DefaultRibbonContextTabCategories);
					else
						ribbonControl.ContextTabCategories.Assign(BarDock.BarDockHtmlEditor.RibbonContextTabCategories);
				}
				foreach(RibbonTab tab in ribbonControl.Tabs) {
					foreach(RibbonGroup group in tab.Groups) {
						foreach(RibbonItemBase item in group.Items) {
							HERibbonColorCommandBase ribbonColorCommand = item as HERibbonColorCommandBase;
							AddShortcutToCustomItemTooltip(item);
							if(ribbonColorCommand != null)
								ribbonColorCommand.EnableCustomColorsDefaultValue = BarDock.htmlEditor.Settings.AllowCustomColorsInColorPickers;
							else if(BarDock.BarDockHtmlEditor.RibbonTabs.Count > 0 && typeof(RibbonComboBoxItem).IsAssignableFrom(item.GetType())) {
								RibbonComboBoxItem ribbonComboBoxItem = item as RibbonComboBoxItem;
								RibbonTab heTab = BarDock.BarDockHtmlEditor.RibbonTabs[tab.Index];
								RibbonGroup heGroup = heTab.Groups[group.Index];
								RibbonComboBoxItem comboBoxItem = heGroup.Items[item.Index] as RibbonComboBoxItem;
								if(comboBoxItem != null) {
									foreach(ListEditItem listEditItem in comboBoxItem.Items) {
										if(listEditItem.Selected)
											ribbonComboBoxItem.Items[listEditItem.Index].Selected = true;
									}
								}
							}
						}
					}
				}
				return ribbonControl;
			}
			void AddShortcutToCustomItemTooltip(RibbonItemBase item) {
				if(!(item is IHERibbonItem) && String.IsNullOrEmpty(item.ToolTip)) {
					item.ToolTip = HtmlEditor.AddShortcutToTooltip(item.ToolTip, item.Name);
				}
			}
			protected override void PrepareControlHierarchy() {
				RenderUtils.AssignAttributes(BarDock, MainTable);
				MainTable.Width = Unit.Percentage(100);
				BarDockStyle style = BarDock.GetBarDockStyle();
				if(BarDock.BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu) {
					Table.Width = Unit.Percentage(100);
					PrepareSeparators();
					PrepareToolbars();
				}
				else if(BarDock.BarDockHtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Ribbon) {
					style.CopyFrom(BarDock.Styles.GetRibbonBarDockStyle());
					RibbonControl.Styles.CopyFrom(BarDock.StylesRibbon);
					RibbonControl.ControlStyle.CopyFrom(BarDock.StylesRibbon.Control);
					RibbonControl.StylesTabControl.CopyFrom(BarDock.StylesRibbon.TabControl);
					RibbonControl.StylesPopupMenu.CopyFrom(BarDock.StylesRibbon.PopupMenu);
					RibbonControl.StylesEditors.Assign(BarDock.BarDockHtmlEditor.StylesEditors);
				}
				style.AssignToControl(MainTable, AttributesRange.Common | AttributesRange.Font);
				style.AssignToControl(MainCell, AttributesRange.Cell);
			}
			protected void PrepareSeparators() {
				AppearanceStyleBase style = BarDock.GetToolbarSpacingStyle();
				foreach(TableCell cell in separators) {
					style.AssignToControl(cell);
					cell.Height = style.Height;
					cell.Width = style.Width;
				}
			}
			protected void PrepareToolbars() {
				foreach(ToolbarControl toolbarControl in ToolbarDictionary.Keys)
					PrepareToolbar(toolbarControl);
			}
			protected void PrepareToolbar(ToolbarControl toolbar) {
				toolbar.ControlStyle.Reset();
				toolbar.ControlStyle.CopyFrom(BarDock.GetToolbarStyle());
				toolbar.ItemStyle.Assign(BarDock.GetToolbarItemStyle());
				toolbar.ClientSideEvents.Command = BarDock.GetToolbarCommand();
				toolbar.ClientSideEvents.CustomComboBoxInit = BarDock.GetToolbarCustomComboBoxInit();
				if(BarDock.IsSaveSelectionBeforeFocusNeeded()) {
					toolbar.ClientSideEvents.DropDownItemBeforeFocus = BarDock.GetToolbarDropDownItemBeforeFocus();
					toolbar.ClientSideEvents.DropDownItemCloseUp = BarDock.GetToolbarDropDownItemCloseUp();
				}
			}
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
	}
}
