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
using System.Text;
using System.Collections;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Helpers {
	class SkinGalleryManager {
		GalleryControl galleryControl;
		RibbonGalleryBarItem galleryBarItem;
		PopupGalleryEdit popupGalleryEdit; 
		public SkinGalleryManager(RibbonGalleryBarItem galleryBarItem) {
			this.galleryBarItem = galleryBarItem;
			if(galleryBarItem.Manager.Form != null) {
				UserLookAndFeel.Default.StyleChanged += new EventHandler(Default_StyleChanged);
				galleryBarItem.Manager.Form.Disposed += new EventHandler(Form_Disposed);
			}
		}
		public SkinGalleryManager(GalleryControl galleryControl) {
			this.galleryControl = galleryControl;
		}
		public SkinGalleryManager(PopupGalleryEdit popupGalleryEdit) {
			this.popupGalleryEdit = popupGalleryEdit;
		}
		void Default_StyleChanged(object sender, EventArgs e) {
			InitGalleryBarItemStyles();
		}
		public void InitGalleryBarItemStyles() {
			InitBaseGalleryStyles(galleryBarItem.Gallery);
			galleryBarItem.Glyph = SkinCollectionHelper.GetSkinIcon(UserLookAndFeel.Default.SkinName, SkinIconsSize.Small);
		}
		public void InitGalleryControlStyles() {
			InitBaseGalleryStyles(galleryControl.Gallery);
		}
		public void InitPopupGalleryEditStyles() {
			InitBaseGalleryStyles(popupGalleryEdit.Properties.Gallery);
		}
		void InitBaseGalleryStyles(BaseGallery baseGallery) {
			foreach(GalleryItemGroup gGroup in baseGallery.Groups) {
				foreach(GalleryItem gItem in gGroup.Items) {
					gItem.Checked = gItem.Tag.Equals(UserLookAndFeel.Default.SkinName) && UserLookAndFeel.Default.ActiveStyle == ActiveLookAndFeelStyle.Skin;
					if(gItem.Checked)
						baseGallery.MakeVisible(gItem);
				}
			}
		}
		void Form_Disposed(object sender, EventArgs e) {
			UserLookAndFeel.Default.StyleChanged -= new EventHandler(Default_StyleChanged);
		}
	}
	public class CreateCustomGalleryItemEventArgs : EventArgs {
		private GalleryItem galleryItem;
		private string skinName;
		public CreateCustomGalleryItemEventArgs(GalleryItem galleryItem, string skinName) {
			this.galleryItem = galleryItem;
			this.skinName = skinName;
		}
		public GalleryItem GalleryItem {
			get { return galleryItem; }
			set { galleryItem = value; }
		}
		public string SkinName {
			get { return skinName; }
		}
	}
	public class CreateGalleryItemEventArgs : CreateCustomGalleryItemEventArgs {
		bool useLargeIcons = false;
		public CreateGalleryItemEventArgs(GalleryItem galleryItem, string skinName, bool useLargeIcons) : base(galleryItem, skinName) {
			this.useLargeIcons = useLargeIcons;
		}
		public bool UseLargeIcons { get { return useLargeIcons; } }
	}
	public class SkinHelper {
		class SkinInfo : IComparable {
			string name, caption;
			int order;
			public SkinInfo(string name, string caption, int order) {
				this.name = name;
				this.caption = caption;
				this.order = order;
			}
			public string Name { get { return name; } }
			public string Caption { get { return caption; } }
			#region IComparable Members
			public int CompareTo(object obj) {
				SkinInfo info = obj as SkinInfo;
				if(info == null) return 0;
				if(info.order == -1 && this.order == -1)
					return Caption.CompareTo(info.Caption);
				if(info.order == -1) return -1;
				if(this.order == -1) return 1;
				return order.CompareTo(info.order);
			}
			#endregion
		}
		public static string GetSkinCaptionBySkinName(string name) {
			string caption = name;
			int index = SkinCollectionHelper.GetSkinIndexByName(name);
			if(index == 0) return caption;
			string captions = BarLocalizer.Active.GetLocalizedString(BarString.SkinCaptions);
			int pos = 0;
			while(true) {
				pos = captions.IndexOf('|', pos) + 1;
				index--;
				if(pos == 0)
					break;
				if(index == 0) {
					int len = captions.IndexOf('|', pos) - pos;
					if(len < 0)
						len = captions.Length - pos;
					if(len > 0)
						caption = captions.Substring(pos, len);
					break;
				}
			}
			return caption;
		}
		public static void InitSkinGalleryDropDown(GalleryDropDown gallery) {
			InitSkinGalleryDropDown(gallery, true);
		}
		public static void InitSkinGalleryDropDown(GalleryDropDown gallery, bool useDefaultEventHandler) {
			AddSkinGroups(gallery.Gallery);
			InitSkinPopupGallery(gallery.Gallery);
			AddSkinItems(gallery.Gallery, true);
			gallery.Gallery.ColumnCount = gallery.Gallery.Groups[2].Visible ? 8 : 4;
			if(useDefaultEventHandler) {
				gallery.Gallery.ItemClick += new GalleryItemClickEventHandler(delegate(object sender, GalleryItemClickEventArgs e) {
					UserLookAndFeel.Default.SetSkinStyle(string.Format("{0}", e.Item.Tag));
				});
			}
			gallery.MenuCaption = "Skins";
			gallery.Popup += new EventHandler(PopupGalleryDropDown);
		}
		static void PopupGalleryDropDown(object sender, EventArgs e) {
			GalleryDropDown gallery = sender as GalleryDropDown;
			foreach(GalleryItemGroup galleryGroup in gallery.Gallery.Groups) {
				foreach(GalleryItem item in galleryGroup.Items) {
					item.Checked = item.Tag.Equals(UserLookAndFeel.Default.SkinName) && UserLookAndFeel.Default.ActiveStyle == ActiveLookAndFeelStyle.Skin;
				}
			}
		}		
		public static void InitSkinGallery(RibbonGalleryBarItem galleryBarItem) {
			InitSkinGallery(galleryBarItem, true);
		}
		public static void InitSkinGallery(RibbonGalleryBarItem galleryBarItem, bool useDefaultCustomization) {
			InitSkinGallery(galleryBarItem, useDefaultCustomization, true);
		}
		public static void InitSkinGallery(RibbonGalleryBarItem galleryBarItem, bool useDefaultCustomization, bool useDefaultEventHandler) {
			SkinGalleryManager sgManager = new SkinGalleryManager(galleryBarItem);
			InitSkinGalleryItem(galleryBarItem, useDefaultCustomization, useDefaultEventHandler);
			AddSkinItems(galleryBarItem.Gallery, false);
			sgManager.InitGalleryBarItemStyles();
		}
		public static void InitSkinGallery(PopupGalleryEdit popupGalleryEdit) {
			InitSkinGallery(popupGalleryEdit, true);
		}
		public static void InitSkinGallery(PopupGalleryEdit popupGalleryEdit, bool useDefaultCustomization) {
			InitSkinGallery(popupGalleryEdit, useDefaultCustomization, true);
		}
		public static void InitSkinGallery(PopupGalleryEdit popupGalleryEdit, bool useDefaultCustomization, bool useDefaultEventHandler) {
			SkinGalleryManager sgManager = new SkinGalleryManager(popupGalleryEdit);
			InitSkinBaseGallery(popupGalleryEdit.Properties.Gallery, useDefaultCustomization, useDefaultEventHandler, false);
			AddSkinItems(popupGalleryEdit.Properties.Gallery, false);
			sgManager.InitPopupGalleryEditStyles();
		}
		public static void InitSkinGallery(GalleryControl galleryControl) {
			InitSkinGallery(galleryControl, true, false, true);
		}
		public static void InitSkinGallery(GalleryControl galleryControl, bool useDefaultCustomization) {
			InitSkinGallery(galleryControl, useDefaultCustomization, false, true);
		}
		public static void InitSkinGallery(GalleryControl galleryControl, bool useDefaultCustomization, bool largeIcons) {
			InitSkinGallery(galleryControl, useDefaultCustomization, largeIcons, true);
		}
		public static void InitSkinGallery(GalleryControl galleryControl, bool useDefaultCustomization, bool largeIcons, bool useDefaultEventHandler) {
			SkinGalleryManager sgManager = new SkinGalleryManager(galleryControl);
			InitSkinBaseGallery(galleryControl.Gallery, useDefaultCustomization, useDefaultEventHandler, largeIcons);
			AddSkinItems(galleryControl.Gallery, largeIcons);
			sgManager.InitGalleryControlStyles();
		}
		public static event EventHandler<CreateCustomGalleryItemEventArgs> CreateCustomGalleryItem;
		public static event EventHandler<CreateGalleryItemEventArgs> CreateGalleryItem;
		static void AddSkinItems(BaseGallery gallery, bool largeIcons) {
			ArrayList skins = new ArrayList();
			foreach(SkinContainer cnt in SkinManager.Default.Skins)
				skins.Add(new SkinInfo(cnt.SkinName, GetSkinCaptionBySkinName(cnt.SkinName), SkinCollectionHelper.GetSkinOrder(cnt.SkinName)));
			skins.Sort();			
			foreach(SkinInfo info in skins) {
				GalleryItem gItem = null;
				if(CreateCustomGalleryItem != null) {
					CreateCustomGalleryItemEventArgs e = new CreateCustomGalleryItemEventArgs(gItem, info.Name);
					CreateCustomGalleryItem(null, e);
					gItem = e.GalleryItem;
				}
				else {
					gItem = new GalleryItem();
				}
				if(gItem != null) {
					int groupIndex = 0;
					if(SkinCollectionHelper.IsBonusSkin(info.Name)) groupIndex = 1;
					if(SkinCollectionHelper.IsThemeSkin(info.Name)) groupIndex = 2;
					if(SkinCollectionHelper.IsCustomSkin(info.Name)) groupIndex = 3;
					gallery.Groups[groupIndex].Items.Add(gItem);
					gItem.Image = SkinCollectionHelper.GetSkinIcon(info.Name, largeIcons ? SkinIconsSize.Large : SkinIconsSize.Small);
					gItem.HoverImage = SkinCollectionHelper.GetSkinIcon(info.Name, SkinIconsSize.Large);
					gItem.Tag = info.Name;
					if(string.IsNullOrEmpty(gItem.Caption)) {
						gItem.Caption = info.Caption;
					}
					if(!largeIcons)
						gItem.Hint = gItem.Caption;
					if(CreateGalleryItem != null) 
						CreateGalleryItem(gallery, new CreateGalleryItemEventArgs(gItem, info.Name, largeIcons));
				}
			}
			foreach(GalleryItemGroup galleryGroup in gallery.Groups)
				galleryGroup.Visible = galleryGroup.Items.Count > 0;
		}
		static void AddSkinGroups(BaseGallery gallery) {
			GalleryItemGroup gigMain = new GalleryItemGroup();
			GalleryItemGroup gigTheme = new GalleryItemGroup();
			GalleryItemGroup gigBonus = new GalleryItemGroup();
			GalleryItemGroup gigCustom = new GalleryItemGroup();
			gigMain.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsMain);
			gigTheme.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsTheme);
			gigBonus.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsBonus);
			gigCustom.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsCustom);
			gallery.Groups.Clear();
			gallery.Groups.AddRange(new GalleryItemGroup[] { gigMain, gigBonus, gigTheme, gigCustom });
		}
		static void InitSkinGalleryItem(RibbonGalleryBarItem galleryBarItem, bool useDefaultCustomization, bool useDefaultEventHandler) {
			InitSkinBaseGallery(galleryBarItem.Gallery, useDefaultCustomization, useDefaultEventHandler, false);
			galleryBarItem.Gallery.InitDropDownGallery += new InplaceGalleryEventHandler(InitDropDownGallery);
		}
		static void InitSkinBaseGallery(BaseGallery gallery, bool useDefaultCustomization, bool useDefaultEventHandler, bool largeIcons) {
			AddSkinGroups(gallery);
			if(useDefaultCustomization) {
				gallery.ImageSize = largeIcons ? new Size(48, 48) : new Size(32, 16);
				gallery.AllowHoverImages = !largeIcons;
				gallery.ColumnCount = 4;
				gallery.FixedHoverImageSize = false;
				gallery.ItemImageLocation = DevExpress.Utils.Locations.Top;
				gallery.ItemCheckMode = ItemCheckMode.SingleRadio;
				gallery.AllowMarqueeSelection = false;
			}
			if(useDefaultEventHandler) {
				gallery.ItemClick += new GalleryItemClickEventHandler(delegate(object sender, GalleryItemClickEventArgs e) {
					UserLookAndFeel.Default.SetSkinStyle(string.Format("{0}", e.Item.Tag));
				});
			}
		}
		static void InitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			e.PopupGallery.Assign(e.Item.Gallery);
			InitSkinPopupGallery(e.PopupGallery);
			e.PopupGallery.ShowScrollBar = ShowScrollBar.Auto;
			foreach(GalleryItemGroup galleryGroup in e.PopupGallery.Groups) {
				foreach(GalleryItem item in galleryGroup.Items) {
					item.Image = item.HoverImage;
					item.Hint = string.Empty;
				}
				galleryGroup.Visible = galleryGroup.Items.Count > 0;
			}
			e.PopupGallery.ColumnCount = e.PopupGallery.Groups[2].Visible ? 8 : 4;
		}
		static void InitSkinPopupGallery(InDropDownGallery popupGallery) {
			popupGallery.AllowFilter = false;
			popupGallery.AllowHoverImages = false;
			popupGallery.AutoSize = GallerySizeMode.Vertical;
			popupGallery.SizeMode = GallerySizeMode.None;
			popupGallery.ImageSize = DevExpress.Skins.SkinCollectionHelper.SkinIconsLarge.ImageSize;
			popupGallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleCheck;
			popupGallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.TopCenter;
			popupGallery.ShowItemText = true;
			popupGallery.ShowGroupCaption = true;
			popupGallery.SynchWithInRibbonGallery = true;
			popupGallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
		}
		static int SkinMenuItemGroupIndex = 8889;
		public static void InitSkinPopupMenu(BarLinksHolder skinMenu) {
			BarSubItem themeSkinMenu = new BarSubItem();
			BarSubItem bonusSkinMenu = new BarSubItem();
			BarSubItem customSkinMenu = new BarSubItem();
			themeSkinMenu.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsTheme);
			bonusSkinMenu.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsBonus);
			customSkinMenu.Caption = BarLocalizer.Active.GetLocalizedString(BarString.SkinsCustom);
			ArrayList skins = new ArrayList();
			foreach(SkinContainer cnt in SkinManager.Default.Skins)
				skins.Add(new SkinInfo(cnt.SkinName, GetSkinCaptionBySkinName(cnt.SkinName), SkinCollectionHelper.GetSkinOrder(cnt.SkinName)));
			skins.Sort();
			foreach(SkinInfo info in skins) {
				BarButtonItem item = new BarButtonItem(skinMenu.Manager, info.Caption);
				item.Tag = info.Name;
				item.Glyph = SkinCollectionHelper.GetSkinIcon(info.Name, SkinIconsSize.Small);
				item.ButtonStyle = BarButtonStyle.Check;
				item.GroupIndex = SkinMenuItemGroupIndex;
				item.ItemClick += new ItemClickEventHandler(delegate(object sender, ItemClickEventArgs e) {
					UserLookAndFeel.Default.SetSkinStyle(string.Format("{0}", e.Item.Tag)); 
				});
				if(SkinCollectionHelper.IsThemeSkin(info.Name) && themeSkinMenu != null)
					themeSkinMenu.ItemLinks.Add(item);
				else if(SkinCollectionHelper.IsBonusSkin(info.Name) && bonusSkinMenu != null)
					bonusSkinMenu.ItemLinks.Add(item);
				else if(SkinCollectionHelper.IsCustomSkin(info.Name) && customSkinMenu != null)
					customSkinMenu.ItemLinks.Add(item);
				else
					skinMenu.ItemLinks.Add(item);
			}
			if(bonusSkinMenu.ItemLinks.Count > 0)
				skinMenu.ItemLinks.Add(bonusSkinMenu);
			if(themeSkinMenu.ItemLinks.Count > 0)
				skinMenu.ItemLinks.Add(themeSkinMenu);
			if(customSkinMenu.ItemLinks.Count > 0)
				skinMenu.ItemLinks.Add(customSkinMenu);
			BarCustomContainerItem bccItem = skinMenu as BarCustomContainerItem;
			if(bccItem != null) bccItem.Popup += new EventHandler(SkinMenu_Popup);
			PopupMenu menu = skinMenu as PopupMenu;
			if(menu != null) menu.Popup += new EventHandler(SkinMenu_Popup);
		}
		static void SkinMenu_Popup(object sender, EventArgs e) {
			BarLinksHolder holder = sender as BarLinksHolder;
			CheckSkinItems(holder.ItemLinks);
		}
		static void CheckSkinItems(BarItemLinkCollection collection) {
			foreach(BarItemLink link in collection) {
				BarSubItem bsItem = link.Item as BarSubItem;
				BarButtonItem item = link.Item as BarButtonItem;
				if(bsItem != null)
					CheckSkinItems(bsItem.ItemLinks);
				if(item != null)
					item.Down = UserLookAndFeel.Default.SkinName.Equals(link.Item.Tag);
			}
		}
	}
}
