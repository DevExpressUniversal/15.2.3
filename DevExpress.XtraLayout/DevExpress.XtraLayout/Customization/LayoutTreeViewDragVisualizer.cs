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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Customization;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Localization;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Customization.Controls;
namespace DevExpress.XtraLayout.Customization {
	public enum DropType { Custom, Inside, OutSide, Illegal }
	public abstract class BaseLayoutTreeViewPainter {
		Control owner;
		protected Graphics graphic;
		protected internal Control Owner {
			get { return owner; }
		}
		protected Graphics OwnerGraphic {
			get {
				if(graphic == null) graphic = CreateGraphics();
				return graphic;
			}
		}
		public BaseLayoutTreeViewPainter(Control owner) { this.owner = owner; }
		protected abstract Graphics CreateGraphics();
		public abstract void Reset();
	}
	public class LayoutTreeViewHitInfoPainter : BaseLayoutTreeViewPainter {
		LayoutTreeViewHitInfo lastHitInfo = null;
		public LayoutTreeViewHitInfoPainter(Control owner)
			: base(owner) {
			InitNodePaintAppearanceSetDefaults();
		}
		protected override Graphics CreateGraphics() {
			return Graphics.FromHwnd(Owner.Handle);
		}
		public void Draw(Graphics g, LayoutTreeViewHitInfo hi, bool justClear) {
			graphic = g;
			if(hi != null) {
				if(lastHitInfo != hi) {
					ClearHitInfo(lastHitInfo);
					if(!justClear) DrawHitInfo(hi);
					lastHitInfo = hi;
				}
			}
		}
		Rectangle GetTreeNodeRowRect(TreeNode node) {
			return new Rectangle(Owner.ClientRectangle.Left, node.Bounds.Top,
				Owner.ClientRectangle.Width, node.Bounds.Height);
		}
		public override void Reset() {
			Draw(OwnerGraphic, new LayoutTreeViewHitInfo(Point.Empty, null, LayoutTreeViewHitType.None), true);
		}
		public virtual void ClearHitInfo(LayoutTreeViewHitInfo hi) {
			if(hi != null && hi.Node != null) {
				Rectangle r = GetTreeNodeRowRect(hi.Node);
				Rectangle clearRect = new Rectangle(r.Left - 1, r.Top - r.Height, r.Width + 3, 3 * r.Height + 1);
				Owner.Invalidate(clearRect);
				Owner.Update();
			}
		}
		protected Rectangle GetPainterNodeRect(LayoutTreeViewHitInfo hi) {
			Rectangle nodeRect = hi.Node.Bounds;
			nodeRect = new Rectangle(new Point(nodeRect.Left - 20, nodeRect.Top - 4), new Size(nodeRect.Width + 23, (int)Math.Round(23 * Skins.DpiProvider.Default.DpiScaleFactor)));
			nodeRect.Inflate(-1, -3);
			return nodeRect;
		}
		protected void InitNodePaintAppearanceSetDefaults() {
			AppearanceObject appearance = new AppearanceObject();
			appearance.BorderColor = Color.FromArgb(108,157,213);
			appearance.BackColor = Color.FromArgb(186, 228, 254);
			NodePaintAppearance = appearance;
		}
		AppearanceObject nodePaintAppearanceCore;
		public AppearanceObject NodePaintAppearance {
			get { return nodePaintAppearanceCore; }
			set { nodePaintAppearanceCore = value; }
		}
		public virtual void DrawHitInfo(LayoutTreeViewHitInfo hi) {
			if(hi.Node != null) {
				Rectangle nodeRect = GetPainterNodeRect(hi);
				using(GraphicsCache cache = new GraphicsCache(OwnerGraphic)) {
					NodePaintAppearance.DrawBackground(cache, nodeRect);
					OwnerGraphic.DrawRectangle(NodePaintAppearance.GetBorderPen(cache), nodeRect);
				}
			}
		}
		protected virtual void DrawTopSideHitInfo(Rectangle rowRect, Pen penSide) {
			int HSpacing = 5;
			int VSpacing = 2;
			int MarkSize = 3;
			Point pt1 = new Point(rowRect.Left + HSpacing, rowRect.Top + VSpacing);
			Point pt2 = new Point(rowRect.Right - HSpacing, rowRect.Top + VSpacing);
			OwnerGraphic.DrawLine(penSide, pt1, pt2);
			OwnerGraphic.DrawLine(penSide, new Point(pt1.X, pt1.Y - MarkSize), new Point(pt1.X, pt1.Y + MarkSize));
			OwnerGraphic.DrawLine(penSide, new Point(pt2.X, pt2.Y - MarkSize), new Point(pt2.X, pt2.Y + MarkSize));
		}
		protected virtual void DrawBottomSideHitInfo(Rectangle rowRect, Pen penSide) {
			int HSpacing = 5;
			int VSpacing = 2;
			int MarkSize = 3;
			Point pt1 = new Point(rowRect.Left + HSpacing, rowRect.Bottom - VSpacing);
			Point pt2 = new Point(rowRect.Right - HSpacing, rowRect.Bottom - VSpacing);
			OwnerGraphic.DrawLine(penSide, pt1, pt2);
			OwnerGraphic.DrawLine(penSide, new Point(pt1.X, pt1.Y - MarkSize), new Point(pt1.X, pt1.Y + MarkSize));
			OwnerGraphic.DrawLine(penSide, new Point(pt2.X, pt2.Y - MarkSize), new Point(pt2.X, pt2.Y + MarkSize));
		}
		protected virtual void DrawRightSideHitInfo(Rectangle nodeRect, Pen penSide) {
			int HSpacing = 2;
			int VSpacing = 2;
			int MarkSize = 2;
			Point pt1 = new Point(nodeRect.Right + HSpacing, nodeRect.Top + VSpacing);
			Point pt2 = new Point(nodeRect.Right + HSpacing, nodeRect.Bottom - VSpacing);
			OwnerGraphic.DrawLine(penSide, pt1, pt2);
			OwnerGraphic.DrawLine(penSide, new Point(pt1.X - MarkSize, pt1.Y), new Point(pt1.X + MarkSize, pt1.Y));
			OwnerGraphic.DrawLine(penSide, new Point(pt2.X - MarkSize, pt2.Y), new Point(pt2.X + MarkSize, pt2.Y));
		}
		protected virtual void DrawLeftSideHitInfo(Rectangle nodeRect, Pen penSide) {
			int HSpacing = 2;
			int VSpacing = 2;
			int MarkSize = 2;
			Point pt1 = new Point(nodeRect.Left - HSpacing, nodeRect.Top + VSpacing);
			Point pt2 = new Point(nodeRect.Left - HSpacing, nodeRect.Bottom - VSpacing);
			OwnerGraphic.DrawLine(penSide, pt1, pt2);
			OwnerGraphic.DrawLine(penSide, new Point(pt1.X - MarkSize, pt1.Y), new Point(pt1.X + MarkSize, pt1.Y));
			OwnerGraphic.DrawLine(penSide, new Point(pt2.X - MarkSize, pt2.Y), new Point(pt2.X + MarkSize, pt2.Y));
		}
	}
	public class DragFrameHitInfoPainter : LayoutTreeViewHitInfoPainter {
		public DragFrameHitInfoPainter(Control owner) : base(owner) { }
	}
	public class DragCursorFramePainter : DragHeaderPainter {
		public DragCursorFramePainter()
			: base(UserLookAndFeel.Default.ActiveLookAndFeel, UserLookAndFeel.Default.ActiveStyle) {
		}
		public void DrawItem(Graphics graphic, BaseLayoutItem item, Rectangle itemRect) {
			Paint(graphic, item, ObjectState.Normal, itemRect, null);
		}
	}
	public class TreeViewVisualizerFrame : BaseToolFrame {
		[ThreadStatic]
		private static TreeViewVisualizerFrame _default;
		public static TreeViewVisualizerFrame Default {
			get {
				if(_default == null) {
					_default = new TreeViewVisualizerFrame();
				}
				return _default;
			}
		}
		public static void Reset() {
			if(_default != null) {
				_default.Dispose();
				_default = null;
			}
		}
		TreeViewVisualizerFrame() {
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			StartPosition = FormStartPosition.Manual;
			BackColor = Color.Transparent;
			MinimumSize = Size.Empty;
			Size = Size.Empty;
			Visible = false;
			TabStop = false;
			Opacity = 0.4;
			TopMost = true;
		}
	}
}
