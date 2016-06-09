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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class RibbonStyles : StylesBase {
		public RibbonStyles(ISkinOwner owner)
			:base(owner) { }
		const string CssClassPrefix = "dxr";
		const string GroupStyleName = "GroupStyle";
		const string ButtonItemStyleName = "ButtonItem";
		const string ItemStyleName = "Item";
		const string GroupSeparatorStyleName = "GroupSep";
		const string GroupLabelStyleName = "GroupLabel";
		const string GroupDialogBoxLauncherStyleName = "DialogBoxLauncher";
		const string GroupPopupStyleName = "GroupPopup";
		const string GroupExpandButtonStyleName = "GroupExpandButton";
		const string OneLineModeGroupExpandButtonStyleName = "OneLineModeGroupExpandButton";
		const string MinimizePopupStyleName = "MinimizePopup";
		const string MinimizeButtonStyleName = "MinimizeButton";
		const string FileTabStyleName = "FileTab";
		const string ItemDropDownPopupStyleName = "ItemDDPopup";
		const string TabContentStyleName = "TabContent";
		protected internal const string
			SystemCssClassName = "dxrSys",
			GroupListCssClass = CssClassPrefix + "-groupList",
			GroupCssClass = CssClassPrefix + "-group",
			GroupContentCssClass = CssClassPrefix + "-groupContent",
			GroupSeparatorCssClass = CssClassPrefix + "-groupSep",
			GroupLabelCssClass = CssClassPrefix + "-groupLabel",
			GroupExpandButtonCssClass = CssClassPrefix + "-grExpBtn",
			GroupExpandButtonHoverCssClass = CssClassPrefix + "-grExpBtnHover",
			GroupExpandButtonPressedCssClass = CssClassPrefix + "-grExpBtnPressed",
			GroupExpandButtonDisabledCssClass = CssClassPrefix + "-grExpBtnDisabled",
			OneLineModeGroupExpandButtonCssClass = CssClassPrefix + "-olmGrExpBtn",
			OneLineModeGroupExpandButtonDisabledCssClass = CssClassPrefix + "-olmGrExpBtnDisabled",
			GroupLabelsHiddenCssClass = CssClassPrefix + "-grLabelsHidden",
			GroupDialogBoxLauncherCssClass = CssClassPrefix + "-grDialogBoxLauncher",
			GroupDialogBoxLauncherHoverCssClass = CssClassPrefix + "-grDialogBoxLauncherHover",
			GroupDialogBoxLauncherPressedCssClass = CssClassPrefix + "-grDialogBoxLauncherPressed",
			GroupDialogBoxLauncherDisabledCssClass = CssClassPrefix + "-grDialogBoxLauncherDisabled",
			GroupPopupWindowCssClass = CssClassPrefix + "-groupPopupWindow",
			GroupPopupCssClass = CssClassPrefix + "-groupPopup",
			MinimizePopupWindowCssClass = CssClassPrefix + "-minPopupWindow",
			MinimizePopupCssClass = CssClassPrefix + "-minPopup",
			MinimizeButtonCssClass = CssClassPrefix + "-minBtn",
			MinimizeButtonHoverCssClass = CssClassPrefix + "-minBtnHover",
			MinimizeButtonPressedCssClass = CssClassPrefix + "-minBtnPressed",
			MinimizeButtonSelectedCssClass = CssClassPrefix + "-minBtnChecked",
			MinimizeButtonDisabledCssClass = CssClassPrefix + "-minBtnDisabled",
			ItemCssClass = CssClassPrefix + "-item",
			ItemHoverCssClass = CssClassPrefix + "-itemHover",
			ItemDisabledCssClass = CssClassPrefix + "-itemDisabled",
			ItemPressedCssClass = CssClassPrefix + "-itemPressed",
			ItemCheckedCssClass = CssClassPrefix + "-itemChecked",
			ItemDropDownPopupCssClass = CssClassPrefix + "-itemDDPopup",
			FileTabCssClass = CssClassPrefix + "-fileTab",
			FileTabPressedCssClass = CssClassPrefix + "-fileTabPressed",
			BlockCssClass = CssClassPrefix + "-block",
			ButtonItemCssClass = CssClassPrefix + "-buttonItem",
			EditorItemCssClass = CssClassPrefix + "-edtItem",
			CheckBoxItemCssClass = CssClassPrefix + "-cbItem",
			LabelCssClass = CssClassPrefix + "-label",
			Image32CssClass = CssClassPrefix + "-img32",
			Image16CssClass = CssClassPrefix + "-img16",
			SpriteImageClass = CssClassPrefix + "-spriteImage",
			DropDownButtonPopOutCssClass = CssClassPrefix + "-popOut",
			DropDownButtonMode = CssClassPrefix + "-ddMode",
			DropDownButtonImageContainer = CssClassPrefix + "-ddImageContainer",
			ItemSeparatorCssClass = CssClassPrefix + "-itemSep",
			BlockRegularItemsCssClass = CssClassPrefix + "-blRegItems",
			BlockHorizontalItemsCssClass = CssClassPrefix + "-blHorItems",
			BlockLargeItemsCssClass = CssClassPrefix + "-blLrgItems",
			BlockSeparateItemsCssClass = CssClassPrefix + "-blSepItems",
			TabContentCssClass = CssClassPrefix + "-tabContent",
			TabWrapperCssClass = CssClassPrefix + "-tabWrapper",
			ContextTabCssClass = CssClassPrefix + "-contextTab",
			ContextTabColorCssClass = CssClassPrefix + "-contextTabColor",
			ContextTabBodyColorCssClass = CssClassPrefix + "-contextTabBodyColor",
			ContextTabLink = CssClassPrefix + "-link",
			EmptyTabCssClass = CssClassPrefix + "-inactiveTab",
			TabControlHasFileTabCssClass = CssClassPrefix + "-hasFileTab",
			TabControlFileTabSpacingCssClass = CssClassPrefix + "-fileTabSpacing",
			ColorDivCssClass = CssClassPrefix + "-colorDiv",
			ColorButtonCssClass = CssClassPrefix + "-colorBtn",
			ColorButtonImageContainer16NoImageCssClass = CssClassPrefix + "-colorBtnNoImg16",
			ColorButtonImageContainer32NoImageCssClass = CssClassPrefix + "-colorBtnNoImg32",
			TemplateItemCssClass = CssClassPrefix + "-tmplItem",
			LabelContentCssClass = CssClassPrefix + "-lblContent",
			LabelTextCssClass = CssClassPrefix + "-lblText",
			ItemHasWidth = CssClassPrefix + "-hasWidth",
			TabsHiddenCssClass = CssClassPrefix + "-tabsHidden",
			GalleryBarItemCssClass = CssClassPrefix + "-glrBarItem",
			GalleryBarContainerCssClass = CssClassPrefix + "-glrBarContainer",
			GalleryBarDivCssClass = CssClassPrefix + "-glrBarDiv",
			GalleryItemCssClass = CssClassPrefix + "-glrItem",
			GalleryItemContentCssClass = CssClassPrefix + "-glrItemContent",
			GalleryItemTextContainterCssClass = CssClassPrefix + "-glrItemTextContainer",
			GalleryItemTextCssClass = CssClassPrefix + "-glrItemText",
			GalleryItemImagePositionTopCssClass = CssClassPrefix + "-glrImgTop",
			GalleryItemImagePositionBottomCssClass = CssClassPrefix + "-glrImgBottom",
			GalleryItemImagePositionRightCssClass = CssClassPrefix + "-glrImgRight",
			GalleryItemImagePositionLeftCssClass = CssClassPrefix + "-glrImgLeft",
			GalleryItemNoTextCssClass = CssClassPrefix + "-glrNoText",
			GalleryMainDivCssClass = CssClassPrefix + "-glrMainDiv",
			GalleryBarButtonsCssClass = CssClassPrefix + "-glrButtons",
			GalleryButtonHoverCssClass = CssClassPrefix + "-glrBtnHover",
			GalleryButtonPressedCssClass = CssClassPrefix + "-glrBtnPressed",
			GalleryGroupCssClass = CssClassPrefix + "-glrGroup",
			OneLineModeCssClass = CssClassPrefix + "-oneLineMode",
			HasContextTabsCssClass = CssClassPrefix + "-hasContextTabs";
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonItemStyle Item
		{
			get { return (RibbonItemStyle)GetStyle(ItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesButtonItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonItemStyle ButtonItem
		{
			get { return (RibbonItemStyle)GetStyle(ButtonItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesGroup"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase Group
		{
			get { return GetStyle(GroupStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesGroupExpandButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonGroupExpandButtonStyle GroupExpandButton
		{
			get { return (RibbonGroupExpandButtonStyle)GetStyle(GroupExpandButtonStyleName); }
		}
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonOneLineModeGroupExpandButtonStyle OneLineModeGroupExpandButton {
			get { return (RibbonOneLineModeGroupExpandButtonStyle)GetStyle(OneLineModeGroupExpandButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesGroupSeparator"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonGroupSeparatorStyle GroupSeparator
		{
			get { return (RibbonGroupSeparatorStyle)GetStyle(GroupSeparatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesGroupLabel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonGroupLabelStyle GroupLabel
		{
			get { return (RibbonGroupLabelStyle)GetStyle(GroupLabelStyleName); }
		}
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase GroupDialogBoxLauncher {
			get { return (AppearanceStyleBase)GetStyle(GroupDialogBoxLauncherStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesGroupPopup"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle GroupPopup
		{
			get { return (AppearanceStyle)GetStyle(GroupPopupStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesMinimizePopup"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle MinimizePopup
		{
			get { return (AppearanceStyle)GetStyle(MinimizePopupStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesItemDropDownPopup"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupWindowContentStyle ItemDropDownPopup
		{
			get { return (PopupWindowContentStyle)GetStyle(ItemDropDownPopupStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesMinimizeButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle MinimizeButton
		{
			get { return (ButtonStyle)GetStyle(MinimizeButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesFileTab"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonFileTabStyle FileTab
		{
			get { return (RibbonFileTabStyle)GetStyle(FileTabStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonStylesTabContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonTabContentStyle TabContent
		{
			get { return (RibbonTabContentStyle)GetStyle(TabContentStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(GroupStyleName));
			list.Add(new StyleInfo(ItemStyleName, typeof(RibbonItemStyle)));
			list.Add(new StyleInfo(ButtonItemStyleName, typeof(RibbonItemStyle)));
			list.Add(new StyleInfo(GroupSeparatorStyleName, typeof(RibbonGroupSeparatorStyle)));
			list.Add(new StyleInfo(GroupLabelStyleName, typeof(RibbonGroupLabelStyle)));
			list.Add(new StyleInfo(GroupDialogBoxLauncherStyleName, typeof(AppearanceStyleBase)));
			list.Add(new StyleInfo(MinimizeButtonStyleName, typeof(ButtonStyle)));
			list.Add(new StyleInfo(FileTabStyleName, typeof(RibbonFileTabStyle)));
			list.Add(new StyleInfo(ItemDropDownPopupStyleName, typeof(PopupWindowContentStyle)));
			list.Add(new StyleInfo(GroupPopupStyleName, typeof(AppearanceStyle)));
			list.Add(new StyleInfo(MinimizePopupStyleName, typeof(AppearanceStyle)));
			list.Add(new StyleInfo(TabContentStyleName, typeof(RibbonTabContentStyle)));
			list.Add(new StyleInfo(GroupExpandButtonStyleName, typeof(RibbonGroupExpandButtonStyle)));
			list.Add(new StyleInfo(OneLineModeGroupExpandButtonStyleName, typeof(RibbonOneLineModeGroupExpandButtonStyle)));
		}
		protected internal AppearanceStyleBase GetItemStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemCssClass);
			style.CopyFrom(Item);
			if(item is RibbonButtonItem || item.IsButtonMode())
				style.CopyFrom(GetButtonStyle());
			style.CopyFrom(item.ItemStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetItemDisabledStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemDisabledCssClass);
			style.CopyFrom(Item.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetItemHoverStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemHoverCssClass);
			style.CopyFrom(ButtonItem.HoverStyle);
			style.CopyFrom(item.ItemStyle.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetItemPressedStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemPressedCssClass);
			style.CopyFrom(ButtonItem.PressedStyle);
			style.CopyFrom(item.ItemStyle.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGalleryButtonHoverStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GalleryButtonHoverCssClass);
			return style;
		}
		protected internal AppearanceStyleBase GetGalleryButtonPressedStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GalleryButtonPressedCssClass);
			return style;
		}
		protected internal AppearanceStyleBase GetGalleryItemCheckedStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemCheckedCssClass);
			style.CopyFrom(ButtonItem.CheckedStyle);
			style.CopyFrom(item.ItemStyle.CheckedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetButtonStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ButtonItemCssClass);
			style.CopyFrom(ButtonItem);
			return style;
		}
		protected internal AppearanceStyleBase GetButtonItemHoverStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemHoverCssClass);
			style.CopyFrom(ButtonItem.HoverStyle);
			style.CopyFrom(item.ItemStyle.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetButtonItemDisabledStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemDisabledCssClass);
			style.CopyFrom(ButtonItem.DisabledStyle);
			style.CopyFrom(item.ItemStyle.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetButtonItemPressedStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemPressedCssClass);
			style.CopyFrom(ButtonItem.PressedStyle);
			style.CopyFrom(item.ItemStyle.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetButtonItemCheckedStyle(RibbonItemBase item) {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemCheckedCssClass);
			style.CopyFrom(ButtonItem.CheckedStyle);
			style.CopyFrom(item.ItemStyle.CheckedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupCssClass);
			style.CopyFrom(Group);
			return style;
		}
		protected internal RibbonGroupSeparatorStyle GetGroupSeparatorStyle() {
			RibbonGroupSeparatorStyle style = GetStyleWithCssClass<RibbonGroupSeparatorStyle>(GroupSeparatorCssClass);
			style.CopyFrom(GroupSeparator);
			return style;
		}
		protected internal RibbonGroupLabelStyle GetGroupLabelStyle() {
			RibbonGroupLabelStyle style = GetStyleWithCssClass<RibbonGroupLabelStyle>(GroupLabelCssClass);
			style.CopyFrom(GroupLabel);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupExpandButtonStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupExpandButtonCssClass);
			style.CopyFrom(GroupExpandButton);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupExpandButtonHoverStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupExpandButtonHoverCssClass);
			style.CopyFrom(GroupExpandButton.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupExpandButtonPressedStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupExpandButtonPressedCssClass);
			style.CopyFrom(GroupExpandButton.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupExpandButtonDisabledStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupExpandButtonDisabledCssClass);
			style.CopyFrom(GroupExpandButton.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetOneLineModeGroupExpandButtonStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(OneLineModeGroupExpandButtonCssClass);
			style.CopyFrom(OneLineModeGroupExpandButton);
			return style;
		}
		protected internal AppearanceStyleBase GetOneLineModeGroupExpandButtonHoverStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemHoverCssClass);
			style.CopyFrom(OneLineModeGroupExpandButton.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetOneLineModeGroupExpandButtonPressedStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(ItemPressedCssClass);
			style.CopyFrom(OneLineModeGroupExpandButton.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetOneLineModeGroupExpandButtonDisabledStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(OneLineModeGroupExpandButtonDisabledCssClass);
			style.CopyFrom(OneLineModeGroupExpandButton.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupDialogBoxLauncherStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupDialogBoxLauncherCssClass);
			style.CopyFrom(GroupDialogBoxLauncher);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupDialogBoxLauncherHoverStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupDialogBoxLauncherHoverCssClass);
			style.CopyFrom(GroupDialogBoxLauncher.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupDialogBoxLauncherPressedStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupDialogBoxLauncherPressedCssClass);
			style.CopyFrom(GroupDialogBoxLauncher.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetGroupDialogBoxLauncherDisabledStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(GroupDialogBoxLauncherDisabledCssClass);
			style.CopyFrom(GroupDialogBoxLauncher.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyle GetGroupPopupStyle() {
			AppearanceStyle style = GetStyleWithCssClass<AppearanceStyle>(GroupPopupCssClass);
			style.CopyFrom(GroupPopup);
			return style;
		}
		protected internal ButtonStyle GetMinimizeButtonStyle() {
			ButtonStyle style = GetStyleWithCssClass<ButtonStyle>(MinimizeButtonCssClass);
			style.CopyFrom(MinimizeButton);
			return style;
		}
		protected internal AppearanceStyleBase GetMinimizeButtonHoverStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(MinimizeButtonHoverCssClass);
			style.CopyFrom(MinimizeButton.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetMinimizeButtonPressedStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(MinimizeButtonPressedCssClass);
			style.CopyFrom(MinimizeButton.PressedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetMinimizeButtonCheckedStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(MinimizeButtonSelectedCssClass);
			style.CopyFrom(MinimizeButton.SelectedStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetMinimizeButtonDisabledStyle() {
			AppearanceStyleBase style = GetStyleWithCssClass<AppearanceStyleBase>(MinimizeButtonDisabledCssClass);
			style.CopyFrom(MinimizeButton.DisabledStyle);
			return style;
		}
		protected internal AppearanceStyle GetMinimizePopupStyle() {
			AppearanceStyle style = GetStyleWithCssClass<AppearanceStyle>(MinimizePopupCssClass);
			style.CopyFrom(MinimizePopup);
			return style;
		}
		protected internal RibbonFileTabStyle GetFileTabStyle() {
			RibbonFileTabStyle style = GetStyleWithCssClass<RibbonFileTabStyle>(FileTabCssClass);
			style.CopyFrom(FileTab);
			return style;
		}
		protected internal RibbonFileTabStyle GetFileTabPressedStyle() {
			RibbonFileTabStyle style = GetStyleWithCssClass<RibbonFileTabStyle>(FileTabPressedCssClass);
			style.CopyFrom(FileTab.PressedStyle);
			return style;
		}
		protected internal PopupWindowContentStyle GetItemDropDownContentStyle() {
			PopupWindowContentStyle style = GetStyleWithCssClass<PopupWindowContentStyle>(ItemDropDownPopupCssClass);
			style.CopyFrom(ItemDropDownPopup);
			return style;
		}
		protected internal RibbonTabContentStyle GetTabContentStyle() {
			RibbonTabContentStyle style = GetStyleWithCssClass<RibbonTabContentStyle>(TabContentCssClass);
			style.CopyFrom(TabContent);
			return style;
		}
		T GetStyleWithCssClass<T>(params string[] cssClasses) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses);
			return style;
		}
	}
	public class RibbonEditorStyles : EditorStyles {
		public RibbonEditorStyles(ISkinOwner owner)
			:base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ColorIndicatorStyle ColorIndicator { get { return base.ColorIndicator; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase Hyperlink { get { return base.Hyperlink; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override LoadingDivStyle LoadingDiv { get { return base.LoadingDiv; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override LoadingPanelStyle LoadingPanel { get { return base.LoadingPanel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditStyleBase Memo { get { return base.Memo; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ProgressBarStyle ProgressBar { get { return base.ProgressBar; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ProgressBarIndicatorStyle ProgressBarIndicator { get { return base.ProgressBarIndicator; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle RadioButton { get { return base.RadioButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle RadioButtonFocused { get { return base.RadioButtonFocused; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase RadioButtonList { get { return base.RadioButtonList; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase TrackBar { get { return base.TrackBar; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTrackElementStyle TrackBarBarHighlight { get { return base.TrackBarBarHighlight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarButtonStyle TrackBarDecrementButton { get { return base.TrackBarDecrementButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarButtonStyle TrackBarIncrementButton { get { return base.TrackBarIncrementButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTickStyle TrackBarItem { get { return base.TrackBarItem; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTickStyle TrackBarLargeTick { get { return base.TrackBarLargeTick; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase TrackBarLeftTopLabel { get { return base.TrackBarLeftTopLabel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarButtonStyle TrackBarMainDragHandle { get { return base.TrackBarMainDragHandle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase TrackBarRightBottomLabel { get { return base.TrackBarRightBottomLabel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceStyleBase TrackBarScale { get { return base.TrackBarScale; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarButtonStyle TrackBarSecondaryDragHandle { get { return base.TrackBarSecondaryDragHandle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTickStyle TrackBarSelectedItem { get { return base.TrackBarSelectedItem; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTickStyle TrackBarSelectedTick { get { return base.TrackBarSelectedTick; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTickStyle TrackBarSmallTick { get { return base.TrackBarSmallTick; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarValueToolTipStyle TrackBarValueToolTip { get { return base.TrackBarValueToolTip; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarTrackElementStyle TrackBarTrack { get { return base.TrackBarTrack; } }
	}
	public class RibbonMenuStyles : MenuStyles {
		public RibbonMenuStyles(ISkinOwner owner)
			: base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuScrollButtonStyle ScrollButton {
			get { return base.ScrollButton; }
		}
	}
	public class RibbonTabControlStyles : TabControlStyles {
		public RibbonTabControlStyles(ISkinOwner owner)
			:base(owner) { }
	}
	public class RibbonGroupExpandButtonStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupExpandButtonStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupExpandButtonStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle
		{
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupExpandButtonStylePressedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle
		{
			get { return base.PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupExpandButtonStyleHoverStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle HoverStyle
		{
			get { return base.HoverStyle; }
		}
	}
	public class RibbonOneLineModeGroupExpandButtonStyle : AppearanceStyleBase {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
	}
	public class RibbonItemStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle
		{
			get { return base.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemStylePressedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle
		{
			get { return base.PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemStyleCheckedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual AppearanceSelectedStyle CheckedStyle
		{
			get { return base.SelectedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemStyleHoverStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle HoverStyle
		{
			get { return base.HoverStyle; }
		}
	}
	public class RibbonGroupSeparatorStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupSeparatorStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
	}
	public class RibbonGroupLabelStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupLabelStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
	}
	public class RibbonTabContentStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonTabContentStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
	}
	public class RibbonFileTabStyle : DevExpress.Web.TabStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonFileTabStyleSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Spacing
		{
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonFileTabStylePressedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual AppearanceSelectedStyle PressedStyle
		{
			get { return base.PressedStyle; }
		}
	}
}
