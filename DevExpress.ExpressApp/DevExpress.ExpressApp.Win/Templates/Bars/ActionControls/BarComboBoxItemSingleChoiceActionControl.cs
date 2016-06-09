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
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public class BarComboBoxItemSingleChoiceActionControl : BarItemSingleChoiceActionControl<BarEditItem> {
		private Dictionary<ChoiceActionItem, ImageComboBoxItem> editItemByActionItem = new Dictionary<ChoiceActionItem, ImageComboBoxItem>();
		protected override void UpdatePaintStyle() {
			if(PaintStyle == ActionItemPaintStyle.Image) {
				BarItem.PaintStyle = BarItemPaintStyle.CaptionGlyph;
			}
			else {
				base.UpdatePaintStyle();
			}
			UpdateCaption();
			UpdateImage();
		}
		protected override void UpdateCaption() {
			if(PaintStyle == ActionItemPaintStyle.Caption || PaintStyle == ActionItemPaintStyle.CaptionAndImage) {
				BarItem.Caption = Caption;
			}
			else {
				BarItem.Caption = null;
			}
		}
		protected override void UpdateImage() {
			if(PaintStyle == ActionItemPaintStyle.Image || PaintStyle == ActionItemPaintStyle.CaptionAndImage) {
				BarItem.Glyph = GetImage(ImageName);
				BarItem.LargeGlyph = GetLargeImage(ImageName);
			}
			else {
				BarItem.Glyph = null;
				BarItem.LargeGlyph = null;
			}
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			if(!(BarItem.Edit is RepositoryItemImageComboBox)) {
				string message = string.Format("Cannot initialize the '{0}' Action Control because its 'BarItem.Edit' property value is not of the RepositoryItemImageComboBox type.", ActionId);
				throw new InvalidOperationException(message);
			}
			Edit.EditValueChanging += Edit_EditValueChanging;
			Edit.EditValueChanged += Edit_EditValueChanged;
			Edit.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		protected override void BeginUpdate() {
			BarItem.BeginUpdate();
			Edit.BeginUpdate();
			Edit.Items.BeginUpdate();
		}
		protected override void EndUpdate() {
			Edit.Items.EndUpdate();
			Edit.EndUpdate();
			BarItem.EndUpdate();
		}
		protected override void ClearItems() {
			editItemByActionItem.Clear();
			Edit.Items.Clear();
		}
		protected override void BuildItems() {
			ImageCollection imageCollection = new ImageCollection();
			Images images = imageCollection.Images;
			foreach(ChoiceActionItem actionItem in ChoiceActionItems) {
				if(actionItem.Active && actionItem.Enabled) {
					ImageComboBoxItem comboBoxItem = new ImageComboBoxItem();
					editItemByActionItem.Add(actionItem, comboBoxItem);
					comboBoxItem.Description = actionItem.Caption;
					Image image = GetImage(actionItem.ImageName);
					if(image != null) {
						int index = images.IndexOf(image);
						if(index == -1) {
							index = images.Add(image);
						}
						comboBoxItem.ImageIndex = index;
					}
					comboBoxItem.Value = actionItem;
					Edit.Items.Add(comboBoxItem);
				}
			}
			Edit.SmallImages = images.Count > 0 ? imageCollection : null;
		}
		protected override bool ShouldRebuildItems(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(KeyValuePair<object, ChoiceActionItemChangesType> pair in itemsChangedInfo) {
				if(ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.Active) || ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.Enabled)) {
					return true;
				}
			}
			return base.ShouldRebuildItems(itemsChangedInfo);
		}
		protected override void UpdateCore(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(KeyValuePair<object, ChoiceActionItemChangesType> pair in itemsChangedInfo) {
				ChoiceActionItemChangesType changesType = pair.Value;
				ChoiceActionItem actionItem = pair.Key as ChoiceActionItem;
				if(actionItem != null) {
					ImageComboBoxItem comboBoxItem;
					if(editItemByActionItem.TryGetValue(actionItem, out comboBoxItem)) {
						if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Caption)) {
							comboBoxItem.Description = actionItem.Caption;
						}
						if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Image)) {
							Image image = GetImage(actionItem.ImageName);
							if(image != null) {
								ImageCollection imageCollection = (ImageCollection)Edit.SmallImages;
								if(imageCollection == null) {
									imageCollection = new ImageCollection();
									Edit.SmallImages = imageCollection;
								}
								Images images = imageCollection.Images;
								int index = images.IndexOf(image);
								if(index == -1) {
									index = images.Add(image);
								}
								comboBoxItem.ImageIndex = index;
							}
						}
					}
				}
			}
		}
		protected override void SetSelectedItemCore(ChoiceActionItem selectedItem) {
			BarItem.EditValue = selectedItem;
		}
		private void Edit_EditValueChanging(object sender, ChangingEventArgs e) {
			if(IsConfirmed()) {
				BarItem.EditValue = e.NewValue;
			}
			else {
				e.Cancel = true;
			}
		}
		private void Edit_EditValueChanged(object sender, EventArgs e) {
			ChoiceActionItem actionItem = (ChoiceActionItem)BarItem.EditValue;
			RaiseExecute(actionItem, false);
		}
		public BarComboBoxItemSingleChoiceActionControl() { }
		public BarComboBoxItemSingleChoiceActionControl(string actionId, BarEditItem barItem) : base(actionId, barItem) { }
		public RepositoryItemImageComboBox Edit {
			get { return (RepositoryItemImageComboBox)BarItem.Edit; }
		}
	}
}
