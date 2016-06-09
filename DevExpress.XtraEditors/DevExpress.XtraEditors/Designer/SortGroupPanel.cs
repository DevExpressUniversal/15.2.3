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
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public class SortGroupPanel : GroupControl {
		public bool AllowSort = false;
		int sortOrder = -1;
		int sortState = -1;
		public event EventHandler SortOrderChanged;
		public Rectangle SortRectangle {
			get {
				int dx = ViewInfo.CaptionBounds.Height;
				Rectangle rect = ViewInfo.CaptionBounds;
				rect.X += rect.Width - dx;
				rect.Width = dx;
				rect.Inflate(-2, -2);
				return rect;
			}
		}
		public int SortOrder {
			get {
				if(!AllowSort) return -1;
				return sortOrder;
			}
			set {
				if(sortOrder == value) return;
				sortOrder = value;
				RaiseSortOrderChanged();
				Invalidate();
			}
		}
		protected virtual void RaiseSortOrderChanged() {
			if(!AllowSort) return;
			if(SortOrderChanged != null)
				SortOrderChanged(this, EventArgs.Empty);
		}
		public int SortState {
			get {
				if(!AllowSort) return -1;
				return sortState;
			}
			set {
				if(sortState == value) return;
				sortState = value;
				Invalidate();
			}
		}
		protected override ObjectPainter CreatePainter() {
			return new SortSkinGroupObjectPainter(this, LookAndFeel.ActiveLookAndFeel);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button != System.Windows.Forms.MouseButtons.Left || SortState == -1) return;
			if(SortRectangle.Contains(new Point(e.X, e.Y))) {
				if(Control.ModifierKeys != Keys.Control) {
					if(SortOrder == 1) SortOrder = 0;
					else SortOrder = 1;
				}
				else SortOrder = -1;
			}
		}
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			if(SortRectangle.Contains(new Point(e.X, e.Y))) {
				if(Control.MouseButtons == MouseButtons.Left) SortState = 1;
				else SortState = 0;
			}
			else SortState = -1;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			SortState = -1;
		}
	}
	public class SortSkinGroupObjectPainter : SkinGroupObjectPainter {
		public SortSkinGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider) : base(owner, provider) { }
		protected override void DrawCaption(GroupObjectInfoArgs info) {
			base.DrawCaption(info);
			DrawSort(info);
		}
		void DrawSort(GroupObjectInfoArgs info) {
			SortGroupPanel owner = Owner as SortGroupPanel;
			if(owner.SortState != -1) DrawButton(info, owner);
			if(owner.SortOrder == -1) return;
			Skin skin = GridSkins.GetSkin(owner.LookAndFeel.ActiveLookAndFeel);
			SkinElement element = skin[GridSkins.SkinSortShape];
			SkinElementInfo eInfo = new SkinElementInfo(element, owner.SortRectangle);
			eInfo.ImageIndex = owner.SortOrder;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, eInfo);
		}
		void DrawButton(GroupObjectInfoArgs info, SortGroupPanel owner) {
			Skin skin = BarSkins.GetSkin(owner.LookAndFeel.ActiveLookAndFeel);
			SkinElement element = skin[BarSkins.SkinAlertBarItem];
			SkinElementInfo eInfo = new SkinElementInfo(element, owner.SortRectangle);
			eInfo.ImageIndex = owner.SortState;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, eInfo);
		}
	}
}
