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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.HitInfo;
namespace DevExpress.XtraLayout.Customization.Behaviours {
	public class DragDropLayoutBehaviour :LayoutBaseBehaviour {
		public DragDropLayoutBehaviour(LayoutAdornerWindowHandler handler)
			: base(handler) {
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.CastedOwner.Disposing) return;
			if(owner.dragController == null) return;
			if(!((ILayoutControl)owner.CastedOwner).DesignMode && !((ILayoutControl)owner.CastedOwner).EnableCustomizationMode) return;
			glyphs.Add(new DragDropGlyph(owner.CastedOwner, owner.dragController));
		}
	}
	public class DragDropGlyph :LayoutGlyph {
		LayoutItemDragController dragController;
		public DragDropGlyph(ILayoutControl layoutControl, LayoutItemDragController dragController)
			: base(layoutControl) {
			this.dragController = dragController;
		}
		bool ShouldShowTabPagesDragBounds {
			get {
				TabbedGroupHitInfo tgh = dragController.HitInfo as TabbedGroupHitInfo;
				if(tgh != null && tgh.TabbedGroupHotPageIndex >= 0 && dragController.DragItem is LayoutGroup) {
					return true;
				}
				return false;
			}
		}
		bool ShouldDrawEmptyDragBoundsInTabPage {
			get {
				LayoutGroup group = dragController.Item as LayoutGroup;
				if(group != null && group.ParentTabbedGroup != null && group.Items.Count == 0) {
					return true;
				}
				return false;
			}
		}
		static Pen dragBoundsPen;
		public static Pen DragBoundsPen {
			get {
				if(dragBoundsPen == null) {
					dragBoundsPen = new Pen(Color.FromArgb(168, 0, 182, 249));
				}
				return dragBoundsPen;
			}
		}
		static SolidBrush dragItemBrush;
		protected static SolidBrush DragItemBrush {
			get {
				if(dragItemBrush == null) dragItemBrush = new SolidBrush(Color.FromArgb(150, Color.LightGray));
				return dragItemBrush;
			}
		}
		public override void Paint(GraphicsCache graphicsCache) {
			PaintEventArgs pe = new PaintEventArgs(graphicsCache.Graphics, Rectangle.Round(graphicsCache.Graphics.ClipBounds));
			GraphicsState state = pe.Graphics.Save();
			pe.Graphics.SetClip(new Rectangle(Point.Empty, layoutControl.Size));
			if(layoutControl.CustomizationForm != null) {
				Point customizationFormPoint = (layoutControl as LayoutControl).PointToClient(layoutControl.CustomizationForm.Bounds.Location);
				graphicsCache.Graphics.SetClip(new Rectangle(customizationFormPoint, layoutControl.CustomizationForm.Bounds.Size), CombineMode.Exclude);
			}
			BaseLayoutItem dragItem = Handlers.DragDropDispatcherFactory.Default.DragItem;
			if(dragItem != null && dragItem.ViewInfo != null && dragItem.Visible && dragItem.Owner != null && Handlers.DragDropDispatcherFactory.Default.State != Handlers.DragDropDispatcherState.Regular)
				pe.Graphics.FillRectangle(DragItemBrush, dragItem.ViewInfo.BoundsRelativeToControl);
			DragFrameWindow.DrawDragBounds(pe, dragController.DragBounds, dragController, ShouldShowTabPagesDragBounds, ShouldDrawEmptyDragBoundsInTabPage, DragBoundsPen);
			pe.Graphics.Restore(state);
		}
	}
}
