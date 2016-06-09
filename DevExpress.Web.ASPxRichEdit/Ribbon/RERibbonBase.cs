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
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Internal;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Localization;
using System.Drawing;
namespace DevExpress.Web.ASPxRichEdit {
	public abstract class RERTab : RibbonTab {
		public RERTab() { }
		public RERTab(string text) {
			Text = RichEditRibbonHelper.ClearAmpersand(text);
		}
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return string.Format("re{0}", DefaultName);
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(DefaultName))
				return string.Format("{0} ({1})", base.ToString(), GetName());
			return base.ToString();
		}
	}
	public abstract class RERGroup : RibbonGroup {
		public RERGroup() { }
		public RERGroup(string text) : this(text, false) { }
		public RERGroup(string text, bool showDialogBoxLauncher) {
			Text = RichEditRibbonHelper.ClearAmpersand(text);
			ShowDialogBoxLauncher = showDialogBoxLauncher;
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultImage { get { return string.Empty; } }
		protected override string GetName() {
			return ((int)Command).ToString();
		}
		protected override ItemImagePropertiesBase GetImage() {
			if(!base.Image.IsEmpty)
				return base.GetImage();
			string imageName = string.Empty;
			if(!string.IsNullOrEmpty(DefaultImage))
				imageName = DefaultImage + RichEditRibbonImages.LargeImagePostfix;
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
	}
	public class RERContextTabCategory : RibbonContextTabCategory {
		public RERContextTabCategory() {
			ResetName();
			ResetColor();
		}
		public RERContextTabCategory(string name) {
			Name = name;
			ResetColor();
		}
		public RERContextTabCategory(string name, Color color) {
			Name = name;
			Color = color;
		}
		protected virtual string DefaultName { get { return string.Empty; } }
		protected virtual Color DefaultColor { get { return Color.Empty; } }
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected bool ShouldSerializeName() {
			return Name != DefaultName;
		}
		protected void ResetName() {
			Name = DefaultName;
		}
		protected bool ShouldSerializeColor() {
			return Color != DefaultColor;
		}
		protected void ResetColor() {
			Color = DefaultColor;
		}
	}
	public abstract class RERButtonCommandBase : RibbonButtonItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string ImageName { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool ShowText { get { return true; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERButtonCommandBase()
			: base() { }
		public RERButtonCommandBase(RibbonItemSize size)
			: base(string.Empty, size) { }
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			return GetImage(ImageName);
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			return GetImage(ImageName + RichEditRibbonImages.LargeImagePostfix);
		}
		protected ItemImagePropertiesBase GetImage(string imageName) {
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERToggleButtonCommandBase : RibbonToggleButtonItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string ImageName { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool ShowText { get { return true; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERToggleButtonCommandBase()
			: base() { }
		public RERToggleButtonCommandBase(RibbonItemSize size)
			: base(string.Empty, size) { }
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			return GetImage(ImageName);
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			return GetImage(ImageName + RichEditRibbonImages.LargeImagePostfix);
		}
		protected ItemImagePropertiesBase GetImage(string imageName) {
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERComboBoxCommandBase : RibbonComboBoxItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual ListEditItemCollection DefaultItems { get { return null; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual int DefaultWidth { get { return 50; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERComboBoxCommandBase()
			: base() {
			this.PropertiesComboBox.Width = System.Web.UI.WebControls.Unit.Pixel(DefaultWidth);
			this.PropertiesComboBox.ValueType = typeof(int);
		}
		public void FillItems() {
			if((DefaultItems != null) && (DefaultItems.Count > 0))
				Items.AddRange(DefaultItems);
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERDropDownCommandBase : RibbonDropDownButtonItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool DefaultDropDownMode { get { return true; } }
		protected virtual bool ShowText { get { return true; } }
		protected virtual string ImageName { get { return string.Empty; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERDropDownCommandBase()
			: this(RibbonItemSize.Small) { }
		public RERDropDownCommandBase(RibbonItemSize size)
			: base(string.Empty, size) {
			DropDownMode = DefaultDropDownMode;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected internal RibbonDropDownButtonCollection GetItemsInternal() {
			return GetItems();
		}
		protected override RibbonDropDownButtonCollection GetItems() {
			if(base.Items.Count == 0) {
				FillItems();
				return Items;
			}
			return base.GetItems();
		}
		protected virtual void FillItems() {
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			return GetImage(ImageName);
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			return GetImage(ImageName + RichEditRibbonImages.LargeImagePostfix);
		}
		protected ItemImagePropertiesBase GetImage(string imageName) {
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERDropDownToggleCommandBase : RibbonDropDownToggleButtonItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool DefaultDropDownMode { get { return true; } }
		protected virtual bool ShowText { get { return true; } }
		protected virtual string ImageName { get { return string.Empty; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERDropDownToggleCommandBase()
			: this(RibbonItemSize.Small) { }
		public RERDropDownToggleCommandBase(RibbonItemSize size)
			: base(string.Empty, size) {
			DropDownMode = DefaultDropDownMode;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			return GetImage(ImageName);
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			return GetImage(ImageName + RichEditRibbonImages.LargeImagePostfix);
		}
		protected ItemImagePropertiesBase GetImage(string imageName) {
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERColorCommandBase : RibbonColorButtonItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool ShowText { get { return true; } }
		protected virtual string ImageName { get { return string.Empty; } }
		protected virtual string DefaultAutomaticColorItemCaption { get { return string.Empty; } }
		protected virtual Color DefaultAutomaticColor { get { return Color.Empty; } }
		protected virtual string DefaultAutomaticColorItemValue { get { return string.Empty; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERColorCommandBase()
			: this(RibbonItemSize.Small) { }
		public RERColorCommandBase(RibbonItemSize size)
			: base(string.Empty, size) {
				EnableCustomColors = true;
				EnableAutomaticColorItem = true;
				AutomaticColorItemCaption = DefaultAutomaticColorItemCaption;
				AutomaticColor = DefaultAutomaticColor;
				AutomaticColorItemValue = DefaultAutomaticColorItemValue;
				IsAutomaticColorSelected = true;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override ItemImagePropertiesBase GetSmallImage() {
			if(!base.GetSmallImage().IsEmpty)
				return base.GetSmallImage();
			return GetImage(ImageName);
		}
		protected override ItemImagePropertiesBase GetLargeImage() {
			if(!base.GetLargeImage().IsEmpty)
				return base.GetLargeImage();
			return GetImage(ImageName + RichEditRibbonImages.LargeImagePostfix);
		}
		protected ItemImagePropertiesBase GetImage(string imageName) {
			return RichEditRibbonHelper.GetImage(RichEditRibbonHelper.GetRibbonControl(this), imageName);
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERCheckBoxCommandBase : RibbonCheckBoxItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual bool ShowText { get { return true; } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERCheckBoxCommandBase()
			: base() { }
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
	}
	public abstract class RERGalleryBarCommandBase : RibbonGalleryBarItem, IRibbonInternalItem {
		protected abstract RichEditClientCommand Command { get; }
		protected virtual string DefaultText { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual int DefaultMinColumnsCount { get { return 2; } }
		protected virtual int DefaultMaxColumnsCount { get { return 10; } }
		protected virtual int DefaultDropDownRowsCount { get { return 3; } }
		protected virtual Unit DefaultMaxTextWidth { get { return Unit.Pixel(65); } }
		[Browsable(false)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		public RERGalleryBarCommandBase()
			: base() {
				this.MinColumnCount = DefaultMinColumnsCount;
				this.MaxColumnCount = DefaultMaxColumnsCount;
				this.PropertiesDropDownGallery.RowCount = DefaultDropDownRowsCount;
				this.MaxTextWidth = DefaultMaxTextWidth;
		}
		protected override string GetText() {
			if(!string.IsNullOrEmpty(base.Text))
				return base.GetText();
			return ShowText ? RichEditRibbonHelper.ClearAmpersand(DefaultText) : string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip.Trim();
		}
		protected override string GetName() {
			if(!string.IsNullOrEmpty(base.Name))
				return base.GetName();
			return ((int)Command).ToString();
		}
	}
	public abstract class RERGalleryItemBase : RibbonGalleryItem, IRibbonInternalItem {
		protected virtual string DefaultImage { get { return string.Empty; } }
		public RERGalleryItemBase()
			: base() { }
		public RERGalleryItemBase(ASPxRibbon ribbon, string text, string name)
			: base(text) {
				Value = name;
				ToolTip = text;
				Image.CopyFrom(RichEditRibbonHelper.GetImage(ribbon, DefaultImage));
		}
	}
	public class RERParagraphStyleItem : RERGalleryItemBase {
		protected override string DefaultImage { get { return RichEditRibbonImages.ParagraphStyleLarge; } }
		public RERParagraphStyleItem()
			: base() { }
		public RERParagraphStyleItem(ASPxRibbon ribbon, string text, string name)
			: base(ribbon, text, "PS-" + name) { }
	}
	public class RERCharacterStyleItem : RERGalleryItemBase {
		protected override string DefaultImage { get { return RichEditRibbonImages.CharacterStyleLarge; } }
		public RERCharacterStyleItem()
			: base() { }
		public RERCharacterStyleItem(ASPxRibbon ribbon, string text, string name)
			: base(ribbon, text, "CS-" + name) { }
	}
	public static class RichEditRibbonHelper {
		public static string ClearAmpersand(string text) {
			return text.Contains("&&") ? text.Replace("&&", "&") : text.Replace("&", "");
		}
		public static ItemImagePropertiesBase GetImage(ASPxRibbon ribbon, string imageName) {
			var properties = new ItemImageProperties();
			if(!string.IsNullOrEmpty(imageName)) {
				properties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, RichEditRibbonImages.RibbonRESpriteName,
					delegate(ISkinOwner skinOwner) { return new RichEditRibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, string.Empty));
			}
			return properties;
		}
		public static void AddTabCollectionToControl(ASPxRibbon ribbon, RibbonTab[] tabs, bool clearExistingTabs) {
			if(clearExistingTabs) {
				foreach(RibbonTab tab in tabs) {
					RibbonTab foundTab = ribbon.Tabs.FindByName(tab.Name);
					if(foundTab != null)
						ribbon.Tabs.RemoveAll(i => i.Name == foundTab.Name);
				}
			}
			ribbon.Tabs.AddRange(tabs);
		}
		public static void AddContextCategoriesToControl(ASPxRibbon ribbon, RibbonContextTabCategory[] categories, bool clearExistingCategories) {
			if (clearExistingCategories) {
				foreach (RibbonContextTabCategory category in categories) {
					RibbonContextTabCategory foundCategory = ribbon.ContextTabCategories.FindByName(category.Name);
					if (foundCategory != null)
						ribbon.ContextTabCategories.RemoveAll(i => i.Name == foundCategory.Name);
				}
			}
			ribbon.ContextTabCategories.AddRange(categories);
		}
		public static ASPxRibbon GetRibbonControl(object owner) {
			RibbonItemBase item = owner as RibbonItemBase;
			if(item != null)
				return GetRibbonControl(item.Ribbon);
			RibbonGroup group = owner as RibbonGroup;
			if(group != null)
				return GetRibbonControl(group.Ribbon);
			return owner as ASPxRibbon;
		}
		public static void UpdateRibbonTabCollection(RibbonTabCollection ribbonTabs, RibbonContextTabCategoryCollection contextCategories, RichEditWorkSession session, ASPxRichEditSettings settings) {
			if(!ribbonTabs.IsEmpty) {
				foreach(RibbonTab tab in ribbonTabs) {
					foreach(RibbonGroup group in tab.Groups) {
						foreach(RibbonItemBase item in group.Items) {
							UpdateRibbonItemVisibility(item, settings);
							FillRibbonItemContent(item);
						}
					}
				}
			}
			if(!contextCategories.IsEmpty) {
				foreach(RibbonContextTabCategory category in contextCategories) {
					foreach (RibbonTab tab in category.Tabs) {
						foreach (RibbonGroup group in tab.Groups) {
							foreach (RibbonItemBase item in group.Items) {
								FillRibbonItemContent(item);
							}
						}
					}
				}
			}
		}
		static void FillRibbonItemContent(RibbonItemBase item) {
			if ((item is RERFontNameCommand) || (item is RERFontSizeCommand) || (item is RERChangeCurrentBorderRepositoryItemLineStyleCommand) || (item is RERChangeCurrentBorderRepositoryItemLineThicknessCommand)) {
				var cmbItem = item as RERComboBoxCommandBase;
				if (cmbItem.Items.IsEmpty)
					cmbItem.FillItems();
			}
		}
		static void UpdateRibbonItemVisibility(RibbonItemBase item, ASPxRichEditSettings settings) {
			if(CheckVisibilityDictionary.ContainsKey(item.GetType()))
				item.Visible = CheckVisibilityDictionary[item.GetType()](settings);
			var ddItem = item as RERDropDownCommandBase;
			if(ddItem != null) {
				foreach(RibbonItemBase sItem in ddItem.GetItemsInternal()) {
					UpdateRibbonItemVisibility(sItem, settings);
				}
			}
			UpdateRibbonGroupVisibility(item.Group);
		}
		static void UpdateRibbonGroupVisibility(RibbonGroup group) {
			bool needHideGroup = group.Items.Find(gi => gi.Visible) == null;
			if(needHideGroup) {
				group.Visible = false;
				UpdateRibbonTabVisibility(group.Tab);
			}
		}
		static void UpdateRibbonTabVisibility(RibbonTab tab) {
			bool needHideTab = tab.Groups.Find(g => g.Visible) == null;
			if(needHideTab)
				tab.Visible = false;
		}
		static Dictionary<Type, Func<ASPxRichEditSettings, Boolean>> CheckVisibilityDictionary = new Dictionary<Type, Func<ASPxRichEditSettings, bool>>() {
			{ typeof(RERPrintCommand), (opt) => opt.Behavior.Printing != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERCopyCommand), (opt) => opt.Behavior.Copy != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERNewCommand), (opt) => opt.Behavior.CreateNew != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERCutCommand), (opt) => opt.Behavior.Cut != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(REROpenCommand), (opt) => opt.Behavior.Open != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERPasteCommand), (opt) => opt.Behavior.Paste != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERSaveCommand), (opt) => opt.Behavior.Save != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERSaveAsCommand), (opt) => opt.Behavior.SaveAs != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERToggleFullScreenCommand), (opt) => opt.Behavior.FullScreen != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontNameCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontSizeCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERIncreaseFontSizeCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERDecreaseFontSizeCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontBoldCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontItalicCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontUnderlineCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontStrikeoutCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontSubscriptCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontSuperscriptCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontColorCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERFontBackColorCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERClearFormattingCommand), (opt) => opt.DocumentCapabilities.CharacterFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERToggleShowAllFieldCodesCommand), (opt) => opt.DocumentCapabilities.Fields != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERToggleShowAllFieldResultsCommand), (opt) => opt.DocumentCapabilities.Fields != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERCreateFieldCommand), (opt) => opt.DocumentCapabilities.Fields != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERUpdateAllFieldsCommand), (opt) => opt.DocumentCapabilities.Fields != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERShowBookmarksFormCommand), (opt) => opt.DocumentCapabilities.Bookmarks != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERShowHyperlinkFormCommand), (opt) => opt.DocumentCapabilities.Hyperlinks != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERInsertPictureCommand), (opt) => opt.DocumentCapabilities.InlinePictures != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERBulletedListCommand), (opt) => opt.DocumentCapabilities.Numbering.Bulleted != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERMultilevelListCommand), (opt) => opt.DocumentCapabilities.Numbering.MultiLevel != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERNumberingListCommand), (opt) => opt.DocumentCapabilities.Numbering.Simple != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERDecreaseIndentCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERIncreaseIndentCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERAlignCenterCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERAlignLeftCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERAlignRightCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERAlignJustifyCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERParagraphLineSpacingCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERParagraphBackColorCommand), (opt) => opt.DocumentCapabilities.ParagraphFormatting != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERPageMarginsCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERChangeSectionPageOrientationCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERChangeSectionPaperKindCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERSetSectionColumnsCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERInsertColumnBreakCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERInsertSectionBreakEvenPageCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERInsertSectionBreakNextPageCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERInsertSectionBreakOddPageCommand), (opt) => opt.DocumentCapabilities.Sections != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERUndoCommand), (opt) => opt.DocumentCapabilities.Undo != XtraRichEdit.DocumentCapability.Hidden },
			{ typeof(RERRedoCommand), (opt) => opt.DocumentCapabilities.Undo != XtraRichEdit.DocumentCapability.Hidden }
		};
		static bool IsStyleVisible(DevExpress.XtraRichEdit.Model.IStyle style) {
			return !(style.Deleted || style.Hidden || style.Semihidden);
		}
	}
}
