#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public class BarChooseSkinActionControl : BarItemSingleChoiceActionControl<BarButtonItem> {
		private const string SkinsChoiceActionItemId = "Skins";
		private Dictionary<BarItem, ChoiceActionItem> actionItemByBarItem = new Dictionary<BarItem, ChoiceActionItem>();
		private Dictionary<ChoiceActionItem, BarButtonItem> barItemByActionItem = new Dictionary<ChoiceActionItem, BarButtonItem>();
		private Dictionary<GalleryItem, ChoiceActionItem> actionItemByGalleryItem = new Dictionary<GalleryItem, ChoiceActionItem>();
		private Dictionary<ChoiceActionItem, GalleryItem> galleryItemByActionItem = new Dictionary<ChoiceActionItem, GalleryItem>();
		private GalleryDropDown skinGalleryDropDown;
		private bool canRebuildItems = false;
		private void BarItem_ItemClick(object sender, ItemClickEventArgs e) {
			if(!BarItem.ActAsDropDown) {
				RaiseExecute(null);
			}
		}
		private void PopupMenu_BeforePopup(object sender, CancelEventArgs e) {
			if(!canRebuildItems) {
				canRebuildItems = true;
				RebuildItems();
			}
		}
		private void OnChoiceBarItemClick(object sender, ItemClickEventArgs e) {
			ChoiceActionItem actionItem = actionItemByBarItem[e.Item];
			RaiseExecute(actionItem);
		}
		private void SkinGallery_GalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			ChoiceActionItem actionItem = actionItemByGalleryItem[e.Item];
			RaiseExecute(actionItem);
		}
		private PopupMenu CreatePopupMenu() {
			PopupMenu popupMenu = new PopupMenu();
			if(BarItem.Manager is RibbonBarManager) {
				popupMenu.Ribbon = ((RibbonBarManager)BarItem.Manager).Ribbon;
			}
			else {
				popupMenu.Manager = BarItem.Manager;
			}
			return popupMenu;
		}
		private PopupMenu PopupMenu {
			get { return (PopupMenu)BarItem.DropDownControl; }
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			BarItem.ButtonStyle = BarButtonStyle.DropDown;
			BarItem.AllowDrawArrow = true;
			BarItem.DropDownControl = CreatePopupMenu();
			BarItem.ItemClick += BarItem_ItemClick;
			PopupMenu.BeforePopup += PopupMenu_BeforePopup;
		}
		protected override void SetShowItemsOnClickCore(bool value) {
			BarItem.ActAsDropDown = value;
		}
		protected override void BeginUpdate() {
			BarItem.BeginUpdate();
			PopupMenu.BeginUpdate();
		}
		protected override void EndUpdate() {
			PopupMenu.EndUpdate();
			BarItem.CancelUpdate();
		}
		protected override void ClearItems() {
			if(skinGalleryDropDown != null) {
				skinGalleryDropDown.Dispose();
				skinGalleryDropDown = null;
			}
			actionItemByGalleryItem.Clear();
			galleryItemByActionItem.Clear();
			PopupMenu.ClearLinks();
			foreach(BarButtonItem barItem in barItemByActionItem.Values) {
				barItem.ItemClick -= OnChoiceBarItemClick;
				barItem.Dispose();
			}
			actionItemByBarItem.Clear();
			barItemByActionItem.Clear();
		}
		protected override void BuildItems() {
			if(!canRebuildItems && !ChoiceActionItemsHelper.HasAnyItemShortcut(ChoiceActionItems)) {
				return;
			}
			foreach(ChoiceActionItem actionItem in ChoiceActionItems) {
				BarButtonItem barItem = new BarButtonItem();
				actionItemByBarItem.Add(barItem, actionItem);
				barItemByActionItem.Add(actionItem, barItem);
				barItem.Manager = BarItem.Manager;
				PopupMenu.ItemLinks.Add(barItem, actionItem.BeginGroup);
				barItem.Visibility = actionItem.Active ? BarItemVisibility.Always : BarItemVisibility.Never;
				barItem.Enabled = actionItem.Enabled;
				barItem.Caption = actionItem.Caption;
				barItem.Hint = actionItem.ToolTip;
				barItem.Glyph = GetImage(actionItem.ImageName);
				barItem.LargeGlyph = GetLargeImage(actionItem.ImageName);
				if(actionItem.Id == SkinsChoiceActionItemId) {
					barItem.ButtonStyle = BarButtonStyle.DropDown;
					barItem.AllowDrawArrow = true;
					barItem.ActAsDropDown = true;
					skinGalleryDropDown = CreateSkinGalleryDropDown();
					skinGalleryDropDown.GalleryItemClick += SkinGallery_GalleryItemClick;
					barItem.DropDownControl = skinGalleryDropDown;
				}
				else {
					barItem.ItemShortcut = ShortcutHelper.ParseBarShortcut(actionItem.Shortcut);
					barItem.ItemClick += OnChoiceBarItemClick;
					barItem.ButtonStyle = BarButtonStyle.Check;
					barItem.AllowAllUp = true;
					barItem.GroupIndex = GroupIndex;
				}
			}
		}
		private GalleryDropDown CreateSkinGalleryDropDown() {
			GalleryDropDown skinGalleryDropDown = new GalleryDropDown();
			skinGalleryDropDown.Manager = BarItem.Manager;
			SkinHelper.CreateCustomGalleryItem += SkinHelper_CreateCustomGalleryItem;
			try {
				SkinHelper.InitSkinGalleryDropDown(skinGalleryDropDown, false);
			}
			finally {
				SkinHelper.CreateCustomGalleryItem -= SkinHelper_CreateCustomGalleryItem;
			}
			return skinGalleryDropDown;
		}
		private void SkinHelper_CreateCustomGalleryItem(object sender, CreateCustomGalleryItemEventArgs e) {
			ChoiceActionItem skinsChoiceActionItem = ChoiceActionItems.FindItemByID(SkinsChoiceActionItemId);
			ChoiceActionItem targetActionItem = skinsChoiceActionItem.Items.FindItemByID(e.SkinName);
			if(targetActionItem != null && targetActionItem.Active && targetActionItem.Enabled) {
				GalleryItem galleryItem = new GalleryItem();
				actionItemByGalleryItem.Add(galleryItem, targetActionItem);
				galleryItemByActionItem.Add(targetActionItem, galleryItem);
				if(targetActionItem.Caption != e.SkinName) {
					galleryItem.Caption = targetActionItem.Caption;
				}
				e.GalleryItem = galleryItem;
			}
		}
		protected override void UpdateCore(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(KeyValuePair<object, ChoiceActionItemChangesType> pair in itemsChangedInfo) {
				ChoiceActionItem actionItem = pair.Key as ChoiceActionItem;
				BarButtonItem barItem;
				if(actionItem != null && barItemByActionItem.TryGetValue(actionItem, out barItem)) {
					Update(barItem, actionItem, pair.Value);
				}
			}
		}
		private void Update(BarButtonItem barItem, ChoiceActionItem actionItem, ChoiceActionItemChangesType changesType) {
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Active)) {
				barItem.Visibility = actionItem.Active ? BarItemVisibility.Always : BarItemVisibility.Never;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Enabled)) {
				barItem.Enabled = actionItem.Enabled;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Caption)) {
				barItem.Caption = actionItem.Caption;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.ToolTip)) {
				barItem.Hint = actionItem.ToolTip;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Image)) {
				barItem.Glyph = GetImage(actionItem.ImageName);
				barItem.LargeGlyph = GetLargeImage(actionItem.ImageName);
			}
		}
		protected override void SetSelectedItemCore(ChoiceActionItem selectedItem) {
			foreach(BarButtonItem barButtonItem in barItemByActionItem.Values) {
				if(barButtonItem.Down) {
					barButtonItem.Down = false;
				}
			}
			foreach(GalleryItem galleryItem in galleryItemByActionItem.Values) {
				if(galleryItem.Checked) {
					galleryItem.Checked = false;
				}
			}
			if(selectedItem != null){
				BarButtonItem barItem;
				if(barItemByActionItem.TryGetValue(selectedItem, out barItem)) {
					barItem.Down = true;
				}
				GalleryItem galleryItem;
				if(galleryItemByActionItem.TryGetValue(selectedItem, out galleryItem)) {
					galleryItem.Checked = true;
				}
			}
		}
		public BarChooseSkinActionControl() { }
		public BarChooseSkinActionControl(string actionId, BarButtonItem item) : base(actionId, item) { }
	}
}
