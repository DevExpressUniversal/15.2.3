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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.ExpressApp.Templates;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Core;
#if DebugTest
using DevExpress.ExpressApp.Tests;
#endif
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public class ImageComboBoxMenuSubItem : ChoiceActionItemWrapper {
		private ImageCollection smallImageList;
		private ImageComboBoxItem imageComboBoxItem;
		public override void SetImageName(string imageName) {
			if(!string.IsNullOrEmpty(imageName)) {
				System.Drawing.Image smallImage = ImageLoader.Instance.GetImageInfo(imageName).Image;
				if(smallImage != null) {
					int index = smallImageList.Images.IndexOf(smallImage);
					if(index == -1) {
						smallImageList.Images.Add(smallImage);
						index = smallImageList.Images.Count - 1;
					}
					imageComboBoxItem.ImageIndex = index;
				}
			}
		}
		public override void SetCaption(string catpion) {
			imageComboBoxItem.Description = catpion;
		}
		public override void SetData(object data) {}
		public override void SetShortcut(string shortcutString) {}
		public override void SetEnabled(bool enabled) {}
		public override void SetVisible(bool visible) {}
		public override void SetToolTip(string toolTip) {}
		public ImageComboBoxMenuSubItem(ChoiceActionItem item, ImageCollection smallImageList, ChoiceActionBase action) : base(item, action) {
			this.smallImageList = smallImageList;
			imageComboBoxItem = new ImageComboBoxItem();
			imageComboBoxItem.Value = ActionItem;
			SyncronizeWithItem();
		}
		public ImageComboBoxItem ImageComboBoxItem { get { return imageComboBoxItem; } }
	}
	public class BarSingleChoiceComboBoxActionItem : BarActionBaseItem
#if DebugTest
		, ISingleChoiceActionItemUnitTestable 
#endif
	{
		private bool isValueChangedEventLocked;
		private Dictionary<ChoiceActionItem, ImageComboBoxMenuSubItem> itemToWrapperMap;
		private void RefreshEditValue() {
			isValueChangedEventLocked = true;
			try {
				Control.EditValue = ((SingleChoiceAction)Action).SelectedItem;
			} finally {
				isValueChangedEventLocked = false;
			}
		}
		private void singleChoiceAction_SelectedItemChanged(object sender, EventArgs e) {
			if(!IsDisposed) {
				RefreshEditValue();
			}
		}
		private void singleChoiceActionItem_EditValueChanged(object sender, EventArgs e) {
			if(!isValueChangedEventLocked && this.Control != null && this.Control.Manager != null && this.Control.Manager.Form!=null) {
				this.Control.Manager.Form.Invoke(new ProcessEditValueChanged(delegate() {
					if(IsConfirmed()) {
						((SingleChoiceAction)Action).DoExecute((ChoiceActionItem)Control.EditValue);
					}
					else {
						RefreshEditValue();
					}
				}));
			}
		}
		private delegate void ProcessEditValueChanged();
		private void ComboBoxActionItem_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(IsDisposed) {
				return;
			}
			BarEditItem barItem = (BarEditItem)Control;
			ChoiceActionItem item = (ChoiceActionItem)(barItem).EditValue;
			InitializeItems(barItem.Edit as RepositoryItemImageComboBox);
			if(item != null)
				Control.EditValue = item;
		}
		private void ClearMap() {
			foreach(ImageComboBoxMenuSubItem itemWrapper in itemToWrapperMap.Values) {
				itemWrapper.Dispose();
			}
			itemToWrapperMap.Clear();
		}
		private void InitializeItems(RepositoryItemImageComboBox repositoryItem) {
			repositoryItem.Items.Clear();
			ClearMap();
			ImageCollection smallImageList = new ImageCollection();
			ChoiceActionBase action = (ChoiceActionBase)Action;
			foreach(ChoiceActionItem actionItem in action.Items) {
				if(actionItem.Active && actionItem.Enabled) {
					ImageComboBoxMenuSubItem imageItem = new ImageComboBoxMenuSubItem(actionItem, smallImageList, action);
					itemToWrapperMap.Add(actionItem, imageItem);
					repositoryItem.Items.Add(imageItem.ImageComboBoxItem);
				}
			}
			if(smallImageList.Images.Count != 0) {
				repositoryItem.SmallImages = smallImageList;
			}
			repositoryItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		}
		protected override BarItem CreateControlCore() {
			((SingleChoiceAction)Action).SelectedItemChanged += new EventHandler(singleChoiceAction_SelectedItemChanged);
			BarEditItem editItem = new BarEditItem();
			editItem.Width = 150;
			RepositoryItemImageComboBox repositoryItem = new RepositoryItemImageComboBox();
			editItem.Edit = repositoryItem;
			editItem.EditValue = ((SingleChoiceAction)Action).SelectedItem;
			editItem.EditValueChanged += new EventHandler(singleChoiceActionItem_EditValueChanged);
			editItem.ShowingEditor += new ItemCancelEventHandler(editItem_ShowingEditor);
			InitializeItems(repositoryItem);
			((SingleChoiceAction)Action).ItemsChanged += new EventHandler<ItemsChangedEventArgs>(ComboBoxActionItem_ItemsChanged);
			return editItem;
		}
		private void editItem_ShowingEditor(object sender, ItemCancelEventArgs e) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
		}
		private void UpdateItemAppearance() {
			ActionItemPaintStyle paintStyle = Action.PaintStyle;
			switch(paintStyle) {
				case ActionItemPaintStyle.Caption:
					Control.PaintStyle = BarItemPaintStyle.Caption;
					Control.Glyph = null;
					Control.LargeGlyph = null;
					break;
				case ActionItemPaintStyle.CaptionAndImage:
					Control.PaintStyle = BarItemPaintStyle.CaptionGlyph;
					break;
				case ActionItemPaintStyle.Default:
					Control.PaintStyle = BarItemPaintStyle.Caption;
					Control.Caption = "";
					Control.Glyph = null;
					Control.LargeGlyph = null;
					break;
				case ActionItemPaintStyle.Image:
					Control.PaintStyle = BarItemPaintStyle.CaptionGlyph;
					Control.Caption = "";
					break;
			}
		}
		protected override void SetCaption(string caption) {
			base.SetCaption(caption);
			UpdateItemAppearance();
		}
		protected override void SetImage(DevExpress.ExpressApp.Utils.ImageInfo imageInfo) {
			base.SetImage(imageInfo);
			UpdateItemAppearance();
		}
		protected override void SetPaintStyle(DevExpress.ExpressApp.Templates.ActionItemPaintStyle paintStyle) {
			base.SetPaintStyle(paintStyle);
			UpdateItemAppearance();
		}
		protected override void UnsubscribeFromActionItemsChangedEvent() {
			base.UnsubscribeFromActionItemsChangedEvent();
			if (Action != null) {
				((SingleChoiceAction)Action).ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(ComboBoxActionItem_ItemsChanged);
			}
		}
		public override void Dispose() {
			try {
				ClearMap();
				if(Control != null) {
					Control.ShowingEditor -= new ItemCancelEventHandler(editItem_ShowingEditor);
					Control.EditValueChanged -= new EventHandler(singleChoiceActionItem_EditValueChanged);
					Control.Edit.Dispose();
					Control.Edit = null;
				}
				if(Action != null) {
					((SingleChoiceAction)Action).SelectedItemChanged -= new EventHandler(singleChoiceAction_SelectedItemChanged);
				}
			}
			finally {
				base.Dispose();
			}
		}
		public BarSingleChoiceComboBoxActionItem(SingleChoiceAction singleChoiceAction, BarManager manager) : base(singleChoiceAction, manager) {
			itemToWrapperMap = new Dictionary<ChoiceActionItem, ImageComboBoxMenuSubItem>();
		}
		public new BarEditItem Control {
			get { return (BarEditItem)base.Control; }
		}
		#region ISingleChoiceActionItemUnitTestable Members
#if DebugTest
		private ImageComboBoxItem GetItemByPath(string itemPath) {
			ChoiceActionItem actionItem = ((SingleChoiceAction)Action).FindItemByCaptionPath(itemPath);
			if(actionItem != null && itemToWrapperMap.ContainsKey(actionItem)) {
				return itemToWrapperMap[actionItem].ImageComboBoxItem;
			}
			return null;
		}
		bool ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			ImageComboBoxItem control = GetItemByPath(itemPath);
			return control == null ? false : true;
		}
		bool ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			ImageComboBoxItem control = GetItemByPath(itemPath);
			return control == null ? false : true;
		}
		bool ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			ChoiceActionItem actionItem = ((SingleChoiceAction)Action).FindItemByCaptionPath(itemPath);
			return actionItem == null ? false : actionItem.BeginGroup;
		}
		bool ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			ImageComboBoxItem control = GetItemByPath(itemPath);
			return control == null ? false : control.ImageIndex > 0;
		}
		bool ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			ChoiceActionItem actionItem = ((SingleChoiceAction)Action).FindItemByCaptionPath(itemPath);
			return Control.EditValue == actionItem;
		}
#endif
		#endregion		
	}
}
