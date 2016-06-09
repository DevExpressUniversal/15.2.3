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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	using DevExpress.Web;
	[ToolboxItem(false)]
	public class ToolbarControl : ASPxMenu {
		protected internal const string ToolbarScriptResourceName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "Toolbar.js";
		protected const string ColorButtonValueChangedHandlerName = "function(s, e){{ASPx.TBColorButtonValueChanged('{0}', '{1}', s.GetValue());}}";
		protected const string ItemPickerItemClick = "function(s, e){{ASPx.TBItemPickerItemClick('{0}', '{1}', s.GetValue(), s.menuIndex);}}";
		protected const string DropDownValueChangedHandlerName = "function(s, e){{ASPx.TBCBValueChanged('{0}', '{1}', s.GetValue());}}";
		protected const string DropDownBeforeFocusHandlerName = "function(s, e){{ASPx.TBCBBeforeFocus('{0}', '{1}');}}";
		protected const string DropDownCloseUpHandlerName = "function(s, e){{ASPx.TBCBCloseUp('{0}', '{1}');}}";
		protected const string DropDownItemClickHandlerName = "function(s, e){{ASPx.TBCBItemClick('{0}', '{1}', s.GetValue());}}";
		protected const string CustomComboBoxInitHandlerName = "function(s, e){{ASPx.TBCCBInit('{0}', s);}}";
		protected const string ToolbarItemMouseOverName = "function(s, e){ASPx.ToolbarItemMouseOver(s, e.item);}";
		protected const string ColorPickerCustomColorTableUpdatedName = "function(s, e){{ASPx.HEToolbarColorPickerCustomColorTableUpdated('{0}', s, '{1}');}}";
		protected const string CustomDDMenuItemParamSelected = "selectedItemIndex";
		protected const string CustomDDMenuItemParamMode = "itemPickerMode";
		HtmlEditorToolbar toolbar = null;
		Hashtable customDropDownMenuItemsParams = new Hashtable();
		public ToolbarControl(ASPxWebControl ownerControl, HtmlEditorToolbar toolbar)
			: base(ownerControl) {
			this.toolbar = toolbar;
			EnableClientSideAPI = true;
			ParentSkinOwner = ownerControl;
			ShowAsToolbar = true;
			if(BarDock != null) {
				RenderIFrameForPopupElementsInternal = BarDock.BarDockHtmlEditor.RenderIFrameForPopupElements;
				ShowSubMenuShadow = RenderIFrameForPopupElementsInternal != Utils.DefaultBoolean.True;
			}
		}
		[Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new ToolbarClientSideEvents ClientSideEvents {
			get { return (ToolbarClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected internal HtmlEditorToolbar Toolbar { get { return toolbar; } }
		protected Hashtable CustomDropDownMenuItemsParams { get { return customDropDownMenuItemsParams; } }
		protected internal HtmlEditorBarDockControl BarDock { get { return Toolbar.BarDock; } }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ToolbarClientSideEvents();
		}
		protected override bool IsClickableItem(MenuItem item) {
			return true;
		}
		protected override bool HasContent() {
			return Toolbar.IsHasRenderItems;
		}
		protected override void ClearControlFields() {
			Items.Clear();
			CustomDropDownMenuItemsParams.Clear();
		}
		protected override void CreateControlHierarchy() {
			MenuItem menuItem = null;
			int menuItemIndex = 0;
			foreach(HtmlEditorToolbarItem toolbarItem in Toolbar.Items) {
				if(Toolbar.IsEmptyToolbarItem(toolbarItem)) continue; 
				menuItem = CreateMenuItem(toolbarItem);
				Items.Add(menuItem);
				ToolbarCustomDropDownBase cddItem = toolbarItem as ToolbarCustomDropDownBase;
				if(cddItem != null) {
					DropDownItemPopulator populator = new DropDownItemPopulator();
					populator.ToolbarItem = cddItem;
					populator.MainOwnerControl = MainOwnerControl;
					populator.Populate();
				}
				ToolbarDropDownBase ddbaseItem = toolbarItem as ToolbarDropDownBase;
				if(ddbaseItem != null) {
					if(ddbaseItem.IsMenu)
						CustomDropDownMenuItemsParams.Add(menuItemIndex, GetCustomMenuItemParams(ddbaseItem));
					CreateToolbarDropDownButton(menuItem, ddbaseItem);
				}
				CreateTemplates(menuItem, toolbarItem, menuItemIndex);
				menuItemIndex++;
			}
			base.CreateControlHierarchy();
		}
		void PrepareItemTooltip(MenuItem menuItem, ToolbarItemBase toolbarItem) {
			ToolbarDropDownBase ddbaseItem = toolbarItem as ToolbarDropDownBase;
			if(ddbaseItem == null || !ddbaseItem.IsMenu) {
				if(toolbarItem is ToolbarExportDropDownItem) {
					menuItem.ToolTip = toolbarItem.GetToolTip();
				} else {
					string ToolTip = string.IsNullOrEmpty(toolbarItem.ToolTip) ? toolbarItem.Text : toolbarItem.GetToolTip();
					menuItem.ToolTip = BarDock.BarDockHtmlEditor.AddShortcutToTooltip(ToolTip, menuItem.Name);
					if(!Browser.IsIE && !String.IsNullOrEmpty(toolbarItem.GetNotAllowedMessage()))
						menuItem.ToolTip = menuItem.ToolTip + ". " + toolbarItem.GetNotAllowedMessage();
				}
			}
		}
		MenuItem CreateMenuItemCore(ToolbarItemBase toolbarItem) {
			MenuItem menuItem = new MenuItem();
			menuItem.Text = toolbarItem.GetText();
			menuItem.Name = toolbarItem.GetCommandName();
			PrepareItemTooltip(menuItem, toolbarItem);
			menuItem.Image.Assign(toolbarItem.GetImageProperties());
			if(!menuItem.Image.IsEmpty && string.IsNullOrEmpty(menuItem.Image.AlternateText))
				menuItem.Image.AlternateText = menuItem.ToolTip;
			return menuItem;
		}
		protected MenuItem CreateMenuItem(HtmlEditorToolbarItem toolbarItem) {
			MenuItem menuItem = CreateMenuItemCore(toolbarItem);
			menuItem.Visible = toolbarItem.Visible;
			menuItem.BeginGroup = toolbarItem.BeginGroup;
			if(toolbarItem.Checkable) {
				menuItem.GroupName = toolbarItem.CommandName;
				if(DesignMode)
					menuItem.Checked = toolbarItem.GetDesignTimeCheckedState(BarDock.BarDockHtmlEditor.StylesDocument);
			}
			return menuItem;
		}
		protected MenuItem CreateMenuItem(ToolbarMenuItem toolbarItem) {
			MenuItem menuItem = CreateMenuItemCore(toolbarItem);
			menuItem.BeginGroup = toolbarItem.BeginGroup;
			return menuItem;
		}
		protected void CreateTemplates(MenuItem menuItem, HtmlEditorToolbarItem toolbarItem, int menuItemIndex) {
			if(toolbarItem is ToolbarComboBoxBase)
				CreateToolbarComboBox(menuItem, toolbarItem as ToolbarComboBoxBase);
			else if(toolbarItem is ToolbarColorButtonBase)
				CreateToolbarColorButton(menuItem, toolbarItem as ToolbarColorButtonBase);
			else {
				ToolbarDropDownBase ddbaseItem = toolbarItem as ToolbarDropDownBase;
				if(ddbaseItem != null && (ddbaseItem.IsMenu || ddbaseItem is ToolbarCustomDropDownBase))
					CreateToolbarCustomDropDownButton(menuItem, ddbaseItem, menuItemIndex);
			}
		}
		void CreateToolbarCustomDropDownButton(MenuItem menuItem, ToolbarDropDownBase ddbaseItem, int menuItemIndex) {
			menuItem.Image.Reset();
			if(ddbaseItem.ClickModeInternal == DropDownItemClickMode.ExecuteSelectedItemAction) {
				ToolbarCustomItemSelectorButton customButton = new ToolbarCustomItemSelectorButton(this, ddbaseItem);
				ClientIDHelper.EnableClientIDGeneration(customButton);
				customButton.ID = ddbaseItem.GetButtonTemplateID();
				customButton.ToolTip = menuItem.ToolTip;
				menuItem.TextTemplate = customButton;
			}
			else {
				if(ddbaseItem.ViewStyle == ViewStyle.Text || ddbaseItem.ViewStyle == ViewStyle.ImageAndText)
					menuItem.Text = ddbaseItem.Text;
				if(ddbaseItem.ViewStyle == ViewStyle.Image || ddbaseItem.ViewStyle == ViewStyle.ImageAndText)
					menuItem.Image.Assign(ddbaseItem.GetImageProperties());
				menuItem.ToolTip = ddbaseItem.ToolTip;
			}
			if(ddbaseItem is ToolbarDropDownItemPicker)
				menuItem.SubMenuTemplate = CreateItemPicker(ddbaseItem as ToolbarDropDownItemPicker, menuItemIndex);
			if(!ddbaseItem.IsMenu)
				menuItem.Items.Add(new MenuItem()); 
		}
		ToolbarCustomDropDownItemPicker CreateItemPicker(ToolbarDropDownItemPicker cddItemPickerButton, int menuItemIndex) {
			ToolbarCustomDropDownItemPicker itemPicker = new ToolbarCustomDropDownItemPicker(this, cddItemPickerButton);
			ClientIDHelper.EnableClientIDGeneration(itemPicker);
			itemPicker.MenuIndex = menuItemIndex;
			return itemPicker;
		}
		protected void CreateToolbarDropDownButton(MenuItem menuItem, ToolbarDropDownBase dropDownBtn) {
			menuItem.DropDownMode = dropDownBtn.ClickModeInternal == DropDownItemClickMode.ExecuteSelectedItemAction || 
				dropDownBtn.ClickModeInternal == DropDownItemClickMode.ExecuteAction; ;
			if(dropDownBtn.ItemsInternal is HtmlEditorToolbarItemCollection) {
				foreach(HtmlEditorToolbarItem item in dropDownBtn.ItemsInternal)
					menuItem.Items.Add(CreateMenuItem(item));
			}
			else if(dropDownBtn.ItemsInternal is ToolbarMenuItemCollection) {
				foreach(ToolbarMenuItem item in dropDownBtn.ItemsInternal)
					menuItem.Items.Add(CreateMenuItem(item));
			}
		}
		protected void CreateToolbarComboBox(MenuItem menuItem, ToolbarComboBoxBase dropDownItem) {
			ToolbarComboBoxProperties properties = dropDownItem.CreateComboBoxProperties(null);
			var dtValue = dropDownItem.GetDesignTimeValue(BarDock.BarDockHtmlEditor.StylesDocument);
			if(DesignMode && !string.IsNullOrEmpty(dtValue))
				((ToolbarListEditItemCollection)dropDownItem.ItemsInternal).Add(new ToolbarListEditItem(dtValue, 0));
			properties.Assign(dropDownItem.Properties);
			ToolbarComboBoxControl comboBox = dropDownItem.CreateComboBoxInstance(this, properties);
			if(DesignMode && !string.IsNullOrEmpty(dtValue))
				comboBox.SelectedIndex = comboBox.Items.Count - 1;
			if(BarDock != null) {
				comboBox.RenderIFrameForPopupElements = BarDock.BarDockHtmlEditor.RenderIFrameForPopupElements;
				comboBox.ShowShadow = comboBox.RenderIFrameForPopupElements != Utils.DefaultBoolean.True;
			}
			ClientIDHelper.EnableClientIDGeneration(comboBox);
			comboBox.ParentImages = Toolbar.GetEditorImages();
			comboBox.Width = dropDownItem.Width;
			comboBox.ToolTip = menuItem.ToolTip;
			menuItem.Template = comboBox;
		}
		protected void CreateToolbarColorButton(MenuItem menuItem, ToolbarColorButtonBase colorButtonBase) {
			menuItem.Image.Reset();
			string color = colorButtonBase.GetDesignTimeValue(BarDock.BarDockHtmlEditor.StylesDocument);
			ToolbarColorButton colorButton = new ToolbarColorButton(this,
				DesignMode && !string.IsNullOrEmpty(color) ? color : colorButtonBase.Color);
			ClientIDHelper.EnableClientIDGeneration(colorButton);
			colorButton.ID = colorButtonBase.GetMainTemplateID();
			colorButton.Enabled = BarDock.BarDockHtmlEditor.Enabled;
			colorButton.Image.Assign(colorButtonBase.GetImageProperties());
			colorButton.ToolTip = menuItem.ToolTip;
			colorButtonBase.ColorNestedControlProperties.EnableCustomColors = colorButtonBase.EnableCustomColors == Utils.DefaultBoolean.Default ?
				BarDock.BarDockHtmlEditor.Settings.AllowCustomColorsInColorPickers : colorButtonBase.EnableCustomColors == Utils.DefaultBoolean.True;
			ToolbarColorPicker colorPicker = new ToolbarColorPicker(this, BarDock.EditorStyles, colorButtonBase.ColorNestedControlProperties);
			ClientIDHelper.EnableClientIDGeneration(colorPicker);
			colorPicker.ID = colorButtonBase.GetDropDownTemplateID();
			menuItem.TextTemplate = colorButton;
			menuItem.SubMenuStyle.Paddings.Padding = 0;
			menuItem.SubMenuStyle.Border.BorderWidth = 0;
			menuItem.SubMenuTemplate = colorPicker;
			menuItem.Items.Add(new MenuItem()); 
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach(MenuItem item in Items) {
				if(item.Template is ToolbarComboBoxControl) {
					ToolbarComboBoxControl ddTemplate = item.Template as ToolbarComboBoxControl;
					ddTemplate.AssignStyles(BarDock.EditorStyles, BarDock.BarDockStyle, BarDock.ToolbarStyle, BarDock.ToolbarItemStyle);
					if(ddTemplate is ToolbarCustomComboBoxControl) {
						ddTemplate.ClientSideEvents.ItemClick = GetDropDownItemClick(item.Name);
						ddTemplate.ClientSideEvents.Init = GetCustomComboBoxInit(item.Name);
					}
					else
						ddTemplate.ClientSideEvents.ValueChanged = GetDropDownValueChanged(item.Name);
					if(Toolbar.BarDock.IsSaveSelectionBeforeFocusNeeded()) {
						ddTemplate.ClientSideEvents.BeforeFocus = GetComboBoxBeforeFocus(item.Name);
						ddTemplate.ClientSideEvents.CloseUp = GetComboBoxCloseUp(item.Name);
					}
				}
				else if(item.TextTemplate is ToolbarColorButton) {
					item.ItemStyle.CssClass = MenuStyles.ToolbarColorButtonItemCssClass;
					item.SubMenuStyle.GutterWidth = Unit.Pixel(0);
					ToolbarColorButton colorButton = item.TextTemplate as ToolbarColorButton;
					ToolbarColorPicker colorPicker = item.SubMenuTemplate as ToolbarColorPicker;
					colorButton.AssignColorPicker(colorPicker);
					colorButton.ClientSideEvents.ColorChanged = GetColorButtonValueChanged(item.Name);
					ClientSideEvents.ItemMouseOver = ToolbarItemMouseOverName;
					colorPicker.ClientSideEvents.CustomColorTableUpdated = GetColorPickerCustomColorTableUpdated(BarDock.BarDockHtmlEditor.ClientID, item.Name);
				} 
				else if(item.TextTemplate is ToolbarCustomItemSelectorButton) {
					ToolbarCustomItemSelectorButton customButton = item.TextTemplate as ToolbarCustomItemSelectorButton;
					item.ItemStyle.CssClass = customButton.IsImageVisible ? MenuStyles.ToolbarCustomDDImageItemCssClass : MenuStyles.ToolbarCustomDDTextItemCssClass;
				}
				if(item.SubMenuTemplate is ToolbarCustomDropDownItemPicker) {
					ToolbarCustomDropDownItemPicker customPicker = item.SubMenuTemplate as ToolbarCustomDropDownItemPicker;
					item.SubMenuStyle.GutterWidth = Unit.Pixel(0);
					item.SubMenuStyle.Border.Assign(BarDock.EditorStyles.DropDownItemPickerStyle.Border);
					customPicker.ClientSideEvents.ItemPickerItemClick = GetItemPickerItemClick(item.Name);
				}
			}
		}
		protected internal string GetItemPickerItemClick(string name) {
			return string.Format(ItemPickerItemClick, ClientID, name);
		}
		protected internal string GetColorButtonValueChanged(string name) {
			return string.Format(ColorButtonValueChangedHandlerName, ClientID, name);
		}
		protected internal string GetColorPickerCustomColorTableUpdated(string htmlEditorID, string name) {
			return string.Format(ColorPickerCustomColorTableUpdatedName, htmlEditorID, name);
		}
		protected internal string GetDropDownValueChanged(string name) {
			return string.Format(DropDownValueChangedHandlerName, ClientID, name);
		}
		protected internal string GetDropDownItemClick(string name) {
			return string.Format(DropDownItemClickHandlerName, ClientID, name);
		}
		protected internal string GetCustomComboBoxInit(string name) {
			return string.Format(CustomComboBoxInitHandlerName, ClientID, name);
		}
		protected internal string GetComboBoxBeforeFocus(string name) {
			return string.Format(DropDownBeforeFocusHandlerName, ClientID, name);
		}
		protected internal string GetComboBoxCloseUp(string name) {
			return string.Format(DropDownCloseUpHandlerName, ClientID, name);
		}
		Hashtable GetCustomMenuItemParams(ToolbarDropDownBase cmddButton) {
			Hashtable result = new Hashtable();
			result.Add(CustomDDMenuItemParamMode, (int)cmddButton.ClickModeInternal);
			result.Add(CustomDDMenuItemParamSelected, cmddButton.SelectedItemIndexInternal);
			return result;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ToolbarControl), ToolbarScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorClasses.Controls.Toolbar";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(BarDock != null)
				stb.AppendLine(string.Format("{0}.barDockControlName='{1}'", localVarName, Toolbar.BarDock.ClientID));
			if(CustomDropDownMenuItemsParams.Count > 0)
				stb.AppendLine(string.Format("{0}.customDropDownItemsParams={1}", localVarName, HtmlConvertor.ToJSON(CustomDropDownMenuItemsParams, false, false, true)));
		}
		protected override AppearanceStyleBase GetItemTemplateStyle(MenuItem item) {
			return BarDock.GetComboBoxToolbarItemStyle();
		}
		protected override string GetItemTemplateToolTip(MenuItem item) {
			return item.ToolTip;
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
	}
	class DropDownItemPopulator {
		public ASPxWebControl MainOwnerControl;		
		public ToolbarCustomDropDownBase ToolbarItem;
		public void Populate() {
			if(MainOwnerControl != null && MainOwnerControl.DesignMode) {
				ToolbarCustomItem item = CreateItemInstance();
				(ToolbarItem.ItemsInternal as IList).Add(item);
			}
			else {
				if(ToolbarItem.DropDownItemsPopulated)
					return;
				IEnumerable data = LoadData();
				if(data == null)
					return;
				ToolbarItem.DropDownItemsPopulated = true;
				ToolbarItem.ItemsInternal.Clear();
				foreach(object dataItem in data) {
					ToolbarCustomItem item = CreateItemInstance();
					PrepareSubItem(dataItem, item);
					(ToolbarItem.ItemsInternal as IList).Add(item);
				}
			}
		}
		IEnumerable LoadData() {
			return new DataHelperCore(MainOwnerControl, ToolbarItem.DataSource, ToolbarItem.DataSourceID, "").Select();
		}
		ToolbarCustomItem CreateItemInstance() {
			if(ToolbarItem is ToolbarDropDownItemPicker)
				return new ToolbarItemPickerItem();
			return new ToolbarMenuItem();
		}
		void PrepareSubItem(object dataItem, ToolbarCustomItem target) {
			target.Text = GetDataItemStringValue(dataItem, ToolbarItem.TextField);
			target.Value = GetDataItemStringValue(dataItem, ToolbarItem.ValueField);
			target.Image.Url = GetDataItemStringValue(dataItem, ToolbarItem.ImageUrlField);
			target.ToolTip = GetDataItemStringValue(dataItem, ToolbarItem.TooltipField);
			ToolbarMenuItem menuItem = target as ToolbarMenuItem;
			if(menuItem != null && ToolbarItem is ToolbarDropDownMenu) {
				menuItem.BeginGroup = GetDataItemStringValue(dataItem, ((ToolbarDropDownMenu)ToolbarItem).BeginGroupField).ToLower() == "true";
			}
		}
		string GetDataItemStringValue(object dataItem, string fieldName) {
			if(String.IsNullOrEmpty(fieldName))
				return "";
			object value = ReflectionUtils.GetPropertyValue(dataItem, fieldName);
			if(value == null)
				return "";
			return value.ToString();
		}
	}
	[ToolboxItem(false)]
	public class ToolbarColorPicker : ColorNestedControl, System.Web.UI.ITemplate {
		public ToolbarColorPicker(ASPxWebControl ownerControl, EditorStyles editorStyles, ColorNestedControlProperties colorNestedControlProperties)
			: base(colorNestedControlProperties) {
			ParentSkinOwner = ownerControl;
			ColorTableStyle.CopyFrom(editorStyles.ColorTable);
			ColorTableCellStyle.CopyFrom(editorStyles.ColorTableCell);
			ColorPickerStyle.CopyFrom(editorStyles.ColorPicker);
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class ToolbarColorButton : ASPxWebControl, ITemplate {
		protected internal const string ColorPickerInitHandlerName = "function(s, e){{ASPx.TBCPInit('{0}', s);}}";
		protected internal const string ColorDivID = "CD";
		protected internal const string ImageID = "P";
		private ImageProperties image = null;
		private string color = null;
		public ToolbarColorButton(ASPxWebControl ownerControl, string color)
			: base(ownerControl) {
			ParentSkinOwner = ownerControl;
			this.color = color;
			this.image = new ImageProperties(this);
		}
		[AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Image { get { return image; } }
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ColorButtonClientSideEvents ClientSideEvents {
			get { return (ColorButtonClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected ToolbarColorButtonStyles Styles { get { return StylesInternal as ToolbarColorButtonStyles; } }
		public void AssignColorPicker(ColorNestedControl colorNestedControl) {
			colorNestedControl.ClientSideEvents.Init = string.Format(ColorPickerInitHandlerName, ClientID);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ColorButtonClientSideEvents();
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new ToolbarColorButtonControl(this));
		}
		protected override StylesBase CreateStyles() {
			return new ToolbarColorButtonStyles(this);
		}
		protected internal AppearanceStyle GetColorDivStyle() {
			AppearanceStyle style = Styles.GetDefaultColorDivStyle();
			style.BackColor = System.Drawing.ColorTranslator.FromHtml(this.color);
			return style;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorClasses.Controls.ToolbarColorButton";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(this.color))
				stb.AppendFormat("{0}.defaultColor='{1}';\n", localVarName, this.color);
		}
		protected override bool HasDisabledScripts() {
			return true;
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyle(new DisabledStyle(), string.Empty, new string[0] {}, Image.GetDisabledScriptObject(Page), "_P", true);
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
		#endregion
	}
	public class ToolbarColorButtonControl : ASPxInternalWebControl {
		ToolbarColorButton colorButton;
		HyperLink imageHyperLink;
		Image image;
		WebControl colorDiv;
		public ToolbarColorButtonControl(ToolbarColorButton colorButton)
			: base() {
			this.colorButton = colorButton;
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected ToolbarColorButton ColorButton { get { return colorButton; } }
		protected HyperLink ImageHyperLink { get { return imageHyperLink; } set { imageHyperLink = value; } }
		protected Image Image { get { return image; } set { image = value; } }
		protected WebControl ColorDiv { get { return colorDiv; } set { colorDiv = value; } }
		protected ImageProperties ImageProperties { get { return ColorButton.Image; } }
		protected override void ClearControlFields() {
			ImageHyperLink = null;
			Image = null;
			ColorDiv = null;
		}
		protected override void CreateControlHierarchy() {
			if(ImageProperties != null)
				CreateImage();
			CreateColorDiv();
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(ColorButton, this);
			ColorButton.GetControlStyle().AssignToControl(
				this,
				AttributesRange.Common | AttributesRange.Font
			);
			if(ImageHyperLink != null) {
				ImageHyperLink.NavigateUrl = RenderUtils.AccessibilityEmptyUrl;
				RenderUtils.PrepareHyperLinkForAccessibility(ImageHyperLink, Enabled, true, true);
			}
			if(Image != null) {
				ImageProperties.AssignToControl(Image, DesignMode, !ColorButton.Enabled);
				Image.ToolTip = ColorButton.ToolTip;
				Image.AlternateText = Image.ToolTip;
			}
			ColorButton.GetColorDivStyle().AssignToControl(ColorDiv);
		}
		protected void CreateImage() {
			Image = RenderUtils.CreateImage();
			Image.ID = ToolbarColorButton.ImageID;
			if(ColorButton.IsAccessibilityCompliantRender()) {
				ImageHyperLink = RenderUtils.CreateHyperLink();
				Controls.Add(ImageHyperLink);
				ImageHyperLink.Controls.Add(Image);
			}
			else
				Controls.Add(Image);
		}
		protected void CreateColorDiv() {
			ColorDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			ColorDiv.ID = ToolbarColorButton.ColorDivID;
			Controls.Add(ColorDiv);
		}
	}
	[ToolboxItem(false)]
	public class ToolbarCustomItemSelectorButton : ASPxWebControl, ITemplate {
		protected internal const string ImageID = "P";
		ImageProperties image;
		ToolbarDropDownBase toolbarItem;
		public ToolbarCustomItemSelectorButton(ASPxWebControl ownerControl, ToolbarDropDownBase toolbarItem)
			: base(ownerControl) {
			ParentSkinOwner = ownerControl;
			Image.Assign(toolbarItem.GetImageProperties());
			this.toolbarItem = toolbarItem;
		}
		[AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Image { 
			get { 
				if(this.image == null)
					this.image = new ImageProperties(this);
				return this.image; 
			} 
		}
		public ToolbarCustomDropDownButtonStyles Styles {
			get { return StylesInternal as ToolbarCustomDropDownButtonStyles; }
		}
		protected internal ToolbarDropDownBase ToolbarItem {
			get { return toolbarItem; }
		}
		protected internal bool IsImageVisible {
			get {
				ToolbarCustomItem item = ToolbarItem.GetSelectedItem();
				return item != null && !item.GetImageProperties().IsEmpty;
			}
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new ToolbarCustomItemSelectorButtonControl(this));
		}
		protected override StylesBase CreateStyles() {
			return new ToolbarCustomDropDownButtonStyles(this);
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
		#endregion
	}
	public class ToolbarCustomItemSelectorButtonControl : ASPxInternalWebControl {
		ToolbarCustomItemSelectorButton dropDownButton;
		HyperLink imageHyperLink;
		InternalImage image;
		public ToolbarCustomItemSelectorButtonControl(ToolbarCustomItemSelectorButton dropDownButton) {
			DropDownButton = dropDownButton;
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		public ToolbarCustomItemSelectorButton DropDownButton { get { return dropDownButton; } set { dropDownButton = value; } }
		public HyperLink ImageHyperLink { get { return imageHyperLink; } set { imageHyperLink = value; } }
		public InternalImage Image { get { return image; } set { image = value; } }
		protected ImageProperties ImageProperties { get { return DropDownButton.Image; } }
		protected override void ClearControlFields() {
			ImageHyperLink = null;
			Image = null;
		}
		protected override void CreateControlHierarchy() {
			DropDownButton.ToolTip = DropDownButton.ToolbarItem.GetToolTip();
			ToolbarCustomItem customItem = DropDownButton.ToolbarItem.GetSelectedItem();
			if(customItem != null) {
				if(!DropDownButton.ToolbarItem.IsItemsImageLess()) {
					CreateImage();
					ImagePropertiesBase imageProperties = customItem.GetImageProperties();
					if(!imageProperties.IsEmpty)
						imageProperties.AssignToControl(Image, DesignMode);
					else {
						Controls.Add(RenderUtils.CreateLiteralControl(customItem.Text));
						RenderUtils.SetStyleStringAttribute(Image, "display", "none");
					}
				}
				else {
					Controls.Add(RenderUtils.CreateLiteralControl(customItem.Text));
				}
				DropDownButton.ToolTip = customItem.GetToolTip();
			}
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(DropDownButton, this, true);
			DropDownButton.GetControlStyle().AssignToControl(
				this,
				AttributesRange.Common | AttributesRange.Font
			);
			if(ImageHyperLink != null) {
				ImageHyperLink.NavigateUrl = RenderUtils.AccessibilityEmptyUrl;
				RenderUtils.PrepareHyperLinkForAccessibility(ImageHyperLink, Enabled, true, true);
			}
			if(Image != null) {
				Image.ToolTip = DropDownButton.ToolTip;
				Image.AlternateText = Image.ToolTip;
			}
		}
		protected void CreateImage() {
			Image = new InternalImage();
			Image.ID = ToolbarCustomItemSelectorButton.ImageID;
			if(DropDownButton.IsAccessibilityCompliantRender()) {
				ImageHyperLink = RenderUtils.CreateHyperLink();
				Controls.Add(ImageHyperLink);
				ImageHyperLink.Controls.Add(Image);
			}
			else
				Controls.Add(Image);
		}
	}
	[ToolboxItem(false)]
	public class ToolbarCustomDropDownItemPicker : ItemPickerBase, System.Web.UI.ITemplate {
		protected internal const string CustomDropDownItemPickerScriptResourceName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "ToolbarItemPicker.js";
		ToolbarDropDownItemPicker itemPickerButton;
		int menuIndex;
		public ToolbarCustomDropDownItemPicker(ASPxWebControl ownerControl, ToolbarDropDownItemPicker itemPickerButton)
			: base(ownerControl) {
			ParentSkinOwner = ownerControl;
			this.itemPickerButton = itemPickerButton;
			ID = this.itemPickerButton.GetDropDownTemplateID();
			Items.Assign(this.itemPickerButton.Items);
			ColumnCount = this.itemPickerButton.ColumnCount;
		}
		public int SelectedItemIndex { get { return itemPickerButton.SelectedItemIndex; } }
		public ToolbarItemPickerImagePosition ImagePosition { get { return itemPickerButton.ImagePosition; } }
		public int ItemHeight { get { return itemPickerButton.ItemHeight; } }
		public int ItemWidth { get { return itemPickerButton.ItemWidth; } }
		public DropDownItemClickMode ClickMode { get { return itemPickerButton.ClickMode; } }
		public ToolbarControl ToolbarControl { get { return (ToolbarControl)OwnerControl; } }
		public int MenuIndex {
			get { return menuIndex; }
			set { menuIndex = value; }
		}
		protected override Collection CreateItemsCollection() {
			return new ToolbarItemPickerItemCollection();
		}
		protected override void CreateControlHierarchy() {
			if(Items.Count > 0)
				Controls.Add(new ToolbarCustomDropDownItemPickerControl(this));
		}
		protected override void PrepareControlHierarchy() {
			TableStyle.MergeWith(this.itemPickerButton.ItemPickerStyle);
			TableStyle.MergeWith(ToolbarControl.BarDock.EditorStyles.DropDownItemPickerStyle);
			TableCellStyle.MergeWith(this.itemPickerButton.ItemPickerItemStyle);
			TableCellStyle.MergeWith(ToolbarControl.BarDock.EditorStyles.DropDownItemPickerItemStyle);
			base.PrepareControlHierarchy();
		}
		protected override AppearanceStyle GetTableStyle() {
			AppearanceStyle style = base.GetTableStyle();
			style.Border.BorderStyle = BorderStyle.None;
			return style;
		}
		protected override AppearanceStyle GetTableCellStyle() {
			return base.GetTableCellStyle();
		}
		protected override StylesBase CreateStyles() {
			return new CustomDropDownStyles(this);
		}	  
		protected new CustomDropDownStyles Styles {
			get { return StylesInternal as CustomDropDownStyles; }
		}
		protected internal AppearanceStyle GetItemPickerTableStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.Assign(Styles.GetDefaultItemPickerTableStyle());
			return style;
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ItemPickerButtonClientSideEvents ClientSideEvents {
			get { return (ItemPickerButtonClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorClasses.Controls.ToolbarItemPicker";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ToolbarCustomDropDownItemPicker), CustomDropDownItemPickerScriptResourceName);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ItemPickerButtonClientSideEvents();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Items.Count > 0)
				stb.AppendFormat("{0}.itemsValues = {1};\r\n", localVarName, HtmlConvertor.ToJSON(((ToolbarItemPickerItemCollection)Items).GetValuesArray()));
			stb.AppendFormat("{0}.curIndex = {1};\r\n", localVarName, SelectedItemIndex);
			if(ClickMode != DropDownItemClickMode.ExecuteSelectedItemAction)
				stb.AppendFormat("{0}.useItemPickerImageMode = {1};\r\n", localVarName, (int)ClickMode);
			stb.AppendFormat("{0}.tableCellStyleCssClassName = '{1}';\r\n", localVarName, GetTableCellStyle().CssClass);
			AppearanceStyle tableCellStyle = GetTableCellStyle();
			tableCellStyle.HorizontalAlign = HorizontalAlign.Center;
			string tableCellStyleValue = tableCellStyle.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(tableCellStyleValue))
				stb.AppendFormat("{0}.tableCellStyleCssText = {1};\r\n", localVarName, HtmlConvertor.ToScript(tableCellStyleValue));
			if(ImagePosition != ToolbarItemPickerImagePosition.Left)
				stb.AppendFormat("{0}.imagePosition = '{1}';\n", localVarName, ImagePosition);
			if(ItemWidth != 0)
				stb.AppendFormat("{0}.itemWidth = {1};\n", localVarName, ItemWidth);
			if(ItemHeight != 0)
				stb.AppendFormat("{0}.itemHeight = {1};\n", localVarName, ItemHeight);
			if(!tableCellStyle.ImageSpacing.IsEmpty)
				stb.AppendFormat("{0}.tableCellStyleImageSpacing = '{1}';\r\n", localVarName, tableCellStyle.ImageSpacing.ToString());
			stb.AppendFormat("{0}.menuIndex = '{1}';\r\n", localVarName, MenuIndex);
		}
		protected override string GetItemsTableID() {
			return "IT";
		}
		protected override string GetItemCellID(int index) {
			return "IC" + index.ToString();
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			container.Controls.Add(this);
		}
		#endregion
	}
	public class ToolbarCustomDropDownItemPickerControl : ItemPickerBaseControl {
		public ToolbarCustomDropDownItemPickerControl(ToolbarCustomDropDownItemPicker itemPicker)
			: base(itemPicker) {
		}
		protected override void CreateItemsTableCellContent(TableCell cell, int index) {
			if(index < ItemPicker.Items.Count) {
				ToolbarItemPickerItem item = ((ToolbarItemPickerItemCollection)ItemPicker.Items)[index];
				cell.ToolTip = item.GetToolTip();
				ImagePropertiesBase imageProperties = item.GetImageProperties();
				if(imageProperties.IsEmpty) {
					cell.Controls.Add(RenderUtils.CreateLiteralControl(item.Text));
				}
				else {
					InternalImage image = new InternalImage();
					imageProperties.AssignToControl(image, DesignMode);
					image.ToolTip = item.GetToolTip();
					image.AlternateText = item.GetToolTip();
					LiteralControl text = RenderUtils.CreateLiteralControl(item.Text);
					cell.Controls.Add(image);
					if(ItemPicker.ImagePosition == ToolbarItemPickerImagePosition.Top)
						cell.Controls.Add(RenderUtils.CreateLiteralControl("<br />"));
					cell.Controls.Add(text);
				}
			}   
		}
		protected new ToolbarCustomDropDownItemPicker ItemPicker {
			get { return base.ItemPicker as ToolbarCustomDropDownItemPicker; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetStringAttribute(ItemPickerTable, "onclick", ItemPicker.GetControlOnClick());
			ItemPicker.GetItemPickerTableStyle().AssignToControl(ItemPickerTable);
		}
	}
}
