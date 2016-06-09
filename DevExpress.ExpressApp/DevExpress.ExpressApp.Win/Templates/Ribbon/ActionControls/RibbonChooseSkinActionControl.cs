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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls {
	public class RibbonChooseSkinActionControl : BarItemSingleChoiceActionControl<RibbonGalleryBarItem>, ISingleChoiceActionControl {
		private const string SkinsChoiceActionItemId = "Skins"; 
		private Dictionary<GalleryItem, ChoiceActionItem> actionItemByGalleryItem = new Dictionary<GalleryItem, ChoiceActionItem>();
		private Dictionary<ChoiceActionItem, GalleryItem> galleryItemByActionItem = new Dictionary<ChoiceActionItem, GalleryItem>();
		private void SkinGallery_GalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			GalleryItem galleryItem = e.Item.OriginItem ?? e.Item;
			ChoiceActionItem actionItem = actionItemByGalleryItem[galleryItem];
			RaiseExecute(actionItem);
		}
		protected override void BeginUpdate() {
			BarItem.BeginUpdate();
		}
		protected override void EndUpdate() {
			BarItem.EndUpdate();
		}
		protected override void ClearItems() {
			actionItemByGalleryItem.Clear();
			galleryItemByActionItem.Clear();
			BarItem.GalleryItemClick -= SkinGallery_GalleryItemClick;
			BarItem.Gallery.DestroyItems();
		}
		protected override void BuildItems() {
			if(ChoiceActionItems.FindItemByID(SkinsChoiceActionItemId) != null) {
				SkinHelper.CreateCustomGalleryItem += SkinHelper_CreateCustomGalleryItem;
				try {
					SkinHelper.InitSkinGallery(BarItem, true, false);
				}
				finally {
					SkinHelper.CreateCustomGalleryItem -= SkinHelper_CreateCustomGalleryItem;
				}
			}
			BarItem.GalleryItemClick += SkinGallery_GalleryItemClick;
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
		}
		protected override void SetSelectedItemCore(ChoiceActionItem selectedItem) {
			GalleryItem galleryItem = null;
			if(selectedItem != null && galleryItemByActionItem.TryGetValue(selectedItem, out galleryItem)) {
				galleryItem.Checked = true;
			}
		}
		public RibbonChooseSkinActionControl() { }
		public RibbonChooseSkinActionControl(string actionId, RibbonGalleryBarItem item) : base(actionId, item) { }
	}
}
