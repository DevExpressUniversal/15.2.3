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

using DevExpress.Utils.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	[ToolboxItem(false)]
	public class ToolboxPopupForm : TopFormBase {
		public ToolboxPopupForm(ToolboxControl owner) : base() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.toolbox = CreateToolbox(owner);
			SubscribeOnToolboxEvents();
			CloneOwnerProperties(owner);
			Controls.Add(toolbox);
			Size = this.toolbox.Size;
			this.owner = owner;
		}
		protected void SubscribeOnToolboxEvents() {
			this.toolbox.DragItemStart += DragItemStart;
			this.toolbox.DragItemDrop += DragItemDrop;
			this.toolbox.ItemDoubleClick += OnItemDoubleClick;
		}
		protected void UnsubscribeOnToolboxEvents() {
			this.toolbox.DragItemStart -= DragItemStart;
			this.toolbox.DragItemDrop -= DragItemDrop;
			this.toolbox.ItemDoubleClick -= OnItemDoubleClick;
		}
		protected virtual void OnItemDoubleClick(object sender, ToolboxItemDoubleClickEventArgs e) {
			this.owner.RaiseItemDoubleClick(new ToolboxItemDoubleClickEventArgs(e.Item.Tag as ToolboxItem));
		}
		protected virtual void DragItemDrop(object sender, ToolboxDragItemDropEventArgs e) {
			this.owner.RaiseDragItemDrop(new ToolboxDragItemDropEventArgs(e.Items));
		}
		protected virtual void DragItemStart(object sender, ToolboxDragItemStartEventArgs e) {
			ToolboxDragItemStartEventArgs args = new ToolboxDragItemStartEventArgs(e.Items);
			this.owner.RaiseDragItemStart(args);
			e.Image = args.Image;
		}
		protected virtual void CloneOwnerProperties(ToolboxControl owner) {
			Toolbox.SetAppearance(owner.Appearance);
			Toolbox.OptionsView.ImageToTextDistance = owner.OptionsView.ImageToTextDistance;
			Toolbox.OptionsView.ColumnCount = owner.OptionsView.ColumnCount;
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			Dispose();
		}
		ToolboxControl toolbox, owner;
		public ToolboxControl Toolbox { get { return toolbox; } }
		public virtual ToolboxControl CreateToolbox(ToolboxControl owner) {
			ToolboxControl newToolbox = new ToolboxControl();
			ToolboxGroup rootGroup = new ToolboxGroup();
			newToolbox.Groups.Add(rootGroup);
			rootGroup.Items.BeginUpdate();
			for (int i = owner.ViewInfo.FirstInvisibleMinimizedItemIndex; i < owner.SelectedGroup.Items.Count; i++) {
				ToolboxItem baseItem = owner.SelectedGroup.Items[i];
				ToolboxItem item = CloneToolboxItem(owner, baseItem);
				rootGroup.Items.Add(item);
			}
			rootGroup.Items.EndUpdate();
			newToolbox.SelectedGroup = rootGroup;
			newToolbox.ShouldDrawOnlyItems = true;
			newToolbox.OptionsView.ItemViewMode = ToolboxItemViewMode.IconOnly;
			newToolbox.OptionsView.ItemImageSize = owner.OptionsView.ItemImageSize;
			return newToolbox;
		}
		protected ToolboxItem CloneToolboxItem(ToolboxControl owner, ToolboxItem baseItem) {
			ToolboxItem item = new ToolboxItem();
			item.Caption = baseItem.Caption;
			item.Image = owner.ViewInfo.GetItemImage(baseItem);
			item.BeginGroup = baseItem.BeginGroup;
			item.SetAppearance(baseItem.Appearance.Clone());
			item.Tag = baseItem;
			return item;
		}
		public void Show(Point p) {
			Location = p;
			Show();
			Toolbox.Focus();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.toolbox != null) {
					UnsubscribeOnToolboxEvents();
					this.toolbox.Dispose();
					this.toolbox = null;
				}
				if(this.owner != null) {
					this.owner.Focus();
				}
			}
			base.Dispose(disposing);
		}
		protected internal virtual void DoShow() {
			Rectangle ownerElementRect = this.owner.ViewInfo.MoreItemsButton.Bounds;
			Point loc = this.owner.PointToScreen(new Point(ownerElementRect.Right, ownerElementRect.Top));
			Rectangle workingArea = Screen.GetWorkingArea(loc);
			if(loc.Y + Height > workingArea.Bottom)
				loc.Y = workingArea.Bottom - Height;
			if(ShouldShowPopupLeft) {
				if(loc.X - ownerElementRect.Width - Width > workingArea.Left)
					loc.X -= ownerElementRect.Width + Width;
			}
			else {
				if(loc.X + Width > workingArea.Right)
					loc.X -= ownerElementRect.Width + Width;
			}
			Show(loc);
		}
		protected internal bool ShouldShowPopupLeft {
			get { return Dock == DockStyle.Right ^ Toolbox.IsRightToLeft; }
		}
	}
}
