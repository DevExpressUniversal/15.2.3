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

using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	class TileBarNavigator : TileControlNavigator {
		public TileBarNavigator(ITileControl control) : base(control) {
			CanSelectItem = true;
		}
		TileBarViewInfo ViewInfoCore { get { return Control.ViewInfo as TileBarViewInfo; } }
		bool IsHorizontal { get { return Control.Properties.Orientation == Orientation.Horizontal; } }
		protected internal void MovePageNoItemFocus(bool left) {
			CanSelectItem = false;
			if(SelectedItem == null) {
				MoveStart();
			}
			if(Control != null && Control.Properties.Orientation == Orientation.Horizontal)
				MovePageHorizontal(left);
			else
				MovePageVertical(left);
			SelectedItem = null;
			CanSelectItem = true;
		}
		bool CanSelectItem { get; set; }
		protected override bool ShouldUseAutoSelectFocusedItem {
			get { return base.ShouldUseAutoSelectFocusedItem && CanSelectItem; }
		}
		public override void MoveDown() {
			if(SelectedItem == null)
				SelectedItem = TrySyncWithSelectedItem();
			if(Control.Properties.Orientation == Orientation.Horizontal && SelectedItem != null) {
				TileBarItemViewInfo itemInfo = SelectedItem.Item as TileBarItemViewInfo;
				if(itemInfo == null || itemInfo.ItemCore == null)
					return;
				itemInfo.ControlInfoCore.ProcessDropDownPress(itemInfo);
			}
			base.MoveDown();
		}
		public override void MoveUp() {
			if(SelectedItem == null)
				SelectedItem = TrySyncWithSelectedItem();
			if(Control.Properties.Orientation == Orientation.Horizontal && SelectedItem != null) {
				TileBarItemViewInfo itemInfo = SelectedItem.Item as TileBarItemViewInfo;
				if(itemInfo == null || itemInfo.ItemCore == null || !itemInfo.DropDownInPressedState)
					return;
				itemInfo.ControlInfoCore.ProcessDropDownPress(itemInfo);
			}
			base.MoveUp();
		}
		public override void MoveStart() {
			if(IsHorizontal)
				base.MoveStart();
			else
				MoveStartVertical();
		}
		protected virtual void MoveStartVertical() {
			var item = GetFirstItem();
			if(item == null && SelectedItem != null)
				LastSelectedItem = SelectedItem;
			SelectedItem = item;
			Control.Invalidate(Control.ViewInfo.Bounds);
			if(item == null)
				return;
			if(ShouldUseAutoSelectFocusedItem)
				Control.SelectedItem = item.Item.Item;
			else
				ViewInfoCore.ScrollToStartVertical();
		}
		protected override List<List<TileNavigationItem>> GetNavigationGridHorizontal() {
			TileItemViewInfo prevItemInfo = null;
			TileItemViewInfo currItemInfo = null;
			List<List<TileNavigationItem>> res = new List<List<TileNavigationItem>>();
			for(int rowIndex = 0; rowIndex < Control.ViewInfo.RealRowCount * 2; rowIndex++) {
				List<TileNavigationItem> row = new List<TileNavigationItem>();
				int column = 0;
				int itemSize = Control.Properties.ItemSize;
				int indent = Control.Properties.IndentBetweenItems;
				foreach(TileGroupViewInfo groupInfo in Control.ViewInfo.Groups) {
					Point pt = new Point(groupInfo.Bounds.X + (itemSize - indent) / 4, groupInfo.Bounds.Y + ((itemSize + indent) / 2) * rowIndex + (itemSize - indent) / 4);
					for(; pt.X < groupInfo.Bounds.Right; pt.X += indent) {
						currItemInfo = groupInfo.Items.GetItemByPoint(pt);
						if(currItemInfo != null && currItemInfo != prevItemInfo) {
							row.Add(new TileNavigationItem() { Item = currItemInfo, Column = column, Row = rowIndex });
							prevItemInfo = currItemInfo;
							column++;
						}
					}
				}
				if(row.Count > 0)
					res.Add(row);
			}
			prevItemInfo = null;
			currItemInfo = null;
			return res;
		}
	}
}
