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
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Helpers;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Resizing;
namespace DevExpress.XtraDashboardLayout {
	public class ResizeBehaviour : BaseBehaviour {
		public ResizeBehaviour(AdornerWindowHandler handler) : base(handler) { }
		public override void Paint(Graphics g) {
		}
		Point startSizingPoint;
		Size startSize;
		BaseLayoutItem sizingItem;
		Size sizingStartGroupSize;
		ResizeGlyphTypes resizeType;
		List<int> itemsPosition = new List<int>();
		bool wasMouseMoveSizing = false;
		bool CheckMouseEventArgs(MouseEventArgs e) {
			if(e != null && e.Button != MouseButtons.Left) return false;
			return true;
		}
		public override bool ProcessEvent(EventType eventType, MouseEventArgs e) {
			if(owner.State == AdornerWindowHandlerStates.Dragging) return false;
			if(owner.State == AdornerWindowHandlerStates.Sizing && eventType == EventType.DeactivateApp) {
				OnEndResize();
				return true;
			}
			if(e == null) {
				owner.SetCursor(Cursors.Arrow);
				return false;
			}
			Invalidate();
			Point p = e.Location;
			ResizeGlyph g = GetGlyphAtPoint(p) as ResizeGlyph;
			if(owner.State == AdornerWindowHandlerStates.Normal) {
				UpdateCursor(p, g);
				if(eventType == EventType.MouseDown && g != null && CheckMouseEventArgs(e)) {
					startSizingPoint = p;
					startSize = g.TargetItem.Size;
					sizingItem = g.TargetItem;
					resizeType = g.ResizeGlyphType;
					sizingStartGroupSize = owner.Owner.Root.Size;
					owner.Owner.Capture = true;
					owner.State = AdornerWindowHandlerStates.Sizing;
					itemsPosition.Clear();
					foreach(BaseLayoutItem bli in owner.Owner.Items) {
						itemsPosition.Add(resizeType == ResizeGlyphTypes.Right ? bli.ViewInfo.BoundsRelativeToControl.X: bli.ViewInfo.BoundsRelativeToControl.Y);
					}
					itemsPosition.Remove(resizeType == ResizeGlyphTypes.Right ? sizingItem.ViewInfo.BoundsRelativeToControl.X : sizingItem.ViewInfo.BoundsRelativeToControl.Y);
					((Resizer2)sizingItem.Owner.RootGroup.Resizer).SizingEdge = new Edge(sizingItem.Owner.RootGroup.CalcHitInfo(startSizingPoint, true), resizeType == ResizeGlyphTypes.Bottom || resizeType == ResizeGlyphTypes.Top ? LayoutType.Vertical : LayoutType.Horizontal);
					SetSizingEdgeForLockSizeItem(p, g);
					wasMouseMoveSizing = false;
					return true;
				}
			}
			if(owner.State == AdornerWindowHandlerStates.Sizing && CheckMouseEventArgs(e)) {
				if(eventType == EventType.MouseMove) {
					Size size = startSize;
					int delta;
					if(resizeType == ResizeGlyphTypes.Right) {
						size.Width += e.X - startSizingPoint.X;
						delta = ResizeHelper.GetDeltaPosition(itemsPosition,sizingItem.ViewInfo.BoundsRelativeToControl.X + size.Width);
						if(Math.Abs(delta) < ResizeHelper.StuckDelta && !itemsPosition.Contains(size.Width)) size.Width +=  delta;
					} else{ 
						size.Height += e.Y - startSizingPoint.Y;
						delta = ResizeHelper.GetDeltaPosition(itemsPosition, sizingItem.ViewInfo.BoundsRelativeToControl.Y + size.Height);
						if(Math.Abs(delta) < ResizeHelper.StuckDelta && !itemsPosition.Contains(size.Height)) size.Height += delta;
					}
					if(sizingItem.Parent != null) {
						sizingItem.Parent.ChangeItemSize(sizingItem, size, this.sizingStartGroupSize);
						wasMouseMoveSizing = true;
						owner.Owner.implementor.Invalidate();
						((ILayoutControl)owner.Owner).Control.Update();
						return true;
					}
				}
				if(eventType == EventType.MouseUp) {
					OnEndResize();
					return true;
				}
				if(eventType == EventType.KeyDown) {
					return true;
				}
			}
			return false;
		}
		private void SetSizingEdgeForLockSizeItem(Point p, ResizeGlyph g) {
			if(g.TargetItem == null || g.TargetItem.Parent == null) return;
			DashboardLayoutControlItemBase bottomItem = g.TargetItem.Parent.GetMovedOutsideNeighbor(g.TargetItem, InsertLocation.After, InsertLocation.After, LayoutType.Vertical) as DashboardLayoutControlItemBase;
			if(bottomItem != null && bottomItem.IsLocked) {
				((Resizer2)sizingItem.Owner.RootGroup.Resizer).SizingEdge = null;
			}
		}
		private bool OnEndResize() {
			if(sizingItem != null && sizingItem.Owner != null && wasMouseMoveSizing) {
				sizingItem.Owner.RootGroup.ResetResizerProportions();
				((Resizer2)sizingItem.Owner.RootGroup.Resizer).SizingEdge = null;
			}
			wasMouseMoveSizing = false;
			owner.Owner.Capture = false;
			owner.State = AdornerWindowHandlerStates.Normal;
			return true;
		}
		private void UpdateCursor(Point p, ResizeGlyph g) {
			if(g != null && g.Bounds.Contains(p)) {
				if(g.ResizeGlyphType == ResizeGlyphTypes.Top || g.ResizeGlyphType == ResizeGlyphTypes.Bottom) {
					owner.SetCursor(Cursors.HSplit);
					owner.Owner.Capture = true;
				}
				if(g.ResizeGlyphType == ResizeGlyphTypes.Left || g.ResizeGlyphType == ResizeGlyphTypes.Right) {
					owner.SetCursor(Cursors.VSplit);
					owner.Owner.Capture = true;
				}
			} else {
				owner.SetCursor(Cursors.Arrow);
				owner.Owner.Capture = false;
			}
		}
		public override void Invalidate() {
			base.Invalidate();
			CreateResizeGlyphsVisitor visitor = new CreateResizeGlyphsVisitor(owner.Owner);
			owner.Owner.Root.Accept(visitor);
			glyphs.AddRange(visitor.glyphs);
		}
		internal static void CalculateGlyphRegions(Rectangle bounds, out Rectangle right, out Rectangle bottom) {
			right = bottom = bounds;
			bottom.Y = bottom.Bottom - DashboardLayoutSettings.ResizingAreaThickness;
			bottom.X = bottom.X + DashboardLayoutSettings.ResizingAreaThickness;
			bottom.Width = bottom.Width - DashboardLayoutSettings.ResizingAreaThickness * 2;
			bottom.Height = DashboardLayoutSettings.ResizingAreaThickness * 2;
			right.X = right.Right - DashboardLayoutSettings.ResizingAreaThickness;
			right.Width = DashboardLayoutSettings.ResizingAreaThickness * 2;
			right.Y = right.Y + DashboardLayoutSettings.ResizingAreaThickness;
			right.Height = right.Height - DashboardLayoutSettings.ResizingAreaThickness * 2;
		}
	}
	public class CreateResizeGlyphsVisitor : BaseVisitor {
		public CreateResizeGlyphsVisitor(DashboardLayoutControl dlc) { owner = dlc; }
		protected DashboardLayoutControl owner;
		public List<Glyph> glyphs = new List<Glyph>();
		public override void Visit(BaseLayoutItem item) {
			if(item != null && item.Parent != null) {
				Rectangle bounds = item.ViewInfo.BoundsRelativeToControl;
				Rectangle pbounds = item.Parent.ViewInfo.ClientAreaRelativeToControl;
				Rectangle right, bottom;
				Rectangle pright, pbottom;
				ResizeBehaviour.CalculateGlyphRegions(bounds, out right, out bottom);
				ResizeBehaviour.CalculateGlyphRegions(pbounds, out pright, out pbottom);
				BaseLayoutItem itemToResize = null;
				if(item is DashboardLayoutControlItemBase) {
					DashboardLayoutControlItemBase tempItem = item as DashboardLayoutControlItemBase;
					while(tempItem != null && tempItem.SizeConstraintsType != SizeConstraintsType.Default && tempItem.IsLocked) {
						tempItem = tempItem.Parent.GetMovedOutsideNeighbor(tempItem, InsertLocation.Before, InsertLocation.Before, LayoutType.Vertical) as DashboardLayoutControlItemBase;
					}
					itemToResize = tempItem;
				}
				if(!pbottom.IntersectsWith(bottom)) glyphs.Add(new ResizeGlyph(owner) { TargetItem = itemToResize == null ? item : itemToResize, ResizeGlyphType = ResizeGlyphTypes.Bottom, Bounds = bottom, Brush = new SolidBrush(Color.Yellow) });
				if(!pright.IntersectsWith(right)) glyphs.Add(new ResizeGlyph(owner) { TargetItem = item, ResizeGlyphType = ResizeGlyphTypes.Right, Bounds = right, Brush = new SolidBrush(Color.Yellow) });
			}
		}
	}
	public enum ResizeGlyphTypes { Top, Bottom, Left, Right }
	public class ResizeGlyph : SimpleGlyph {
		public ResizeGlyphTypes ResizeGlyphType { get; set; }
		public BaseLayoutItem TargetItem { get; set; }
		public ResizeGlyph(DashboardLayoutControl layoutControl) : base(layoutControl) { }
	}
}
