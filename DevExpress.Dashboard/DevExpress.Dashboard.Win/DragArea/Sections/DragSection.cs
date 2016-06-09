#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.Utils.Controls;
using System;
namespace DevExpress.DashboardWin.Native {
	public abstract class DragSection : IDisposable {
		readonly DragArea area;
		readonly string itemName;
		readonly string itemNamePlural;
		public DragArea Area { get { return area; } }
		public int SectionIndex { get { return area.Sections.IndexOf(this); } }
		public string ItemName { get { return itemName; } }
		public string ItemNamePlural { get { return itemNamePlural; } }
		protected internal abstract IEnumerable<DragGroup> ActualGroups { get; }
		public virtual int ActualGroupCount { get { return 1; } }
		protected abstract Rectangle ContentBounds { get; }
		public virtual bool AllowDragGroups { get { return false; } }
		int SpaceBetweenGroups { get { return area.ParentControl.DrawingContext.Painters.GroupPainter.GroupIndent; } }
		protected DragSection(DragArea area, string itemName) 
			: this(area, itemName, null) {
		}
		protected DragSection(DragArea area, string itemName, string itemNamePlural) {
			this.area = area;
			this.itemName = itemName;
			this.itemNamePlural = itemNamePlural;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		protected internal virtual void Initialize() {
		}
		protected abstract void ArrangeContent(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location);
		protected abstract void PaintContent(DragAreaDrawingContext drawingContext, GraphicsCache cache);
		protected virtual DragAreaSelection GetSelectionInternal(Point point) {
			return null;
		}
		public void SetDataSourceSchema(IDataSourceSchema dataSourceSchema) {
			foreach (DragGroup group in ActualGroups) {
				group.SetDataSourceSchema(dataSourceSchema);
			}
		}
		public Rectangle Arrange(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location) {
			ArrangeContent(drawingContext, cache, location);
			Rectangle contentBounds = ContentBounds;
			int groupIndent = drawingContext.Painters.GroupPainter.GroupIndent;			
			location.Y += contentBounds.Height + groupIndent;
			Rectangle bounds = contentBounds;
			foreach (DragGroup group in ActualGroups) {
				Rectangle groupBounds = group.Arrange(drawingContext, cache, location);
				bounds = Rectangle.Union(bounds, groupBounds);
				location.Y += groupBounds.Height + groupIndent;
			}
			return bounds;
		}
		public void Paint(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			PaintContent(drawingContext, cache);
			foreach(DragGroup group in ActualGroups) {
				group.Paint(drawingContext, cache);
				if(group.SizeState == DragItemSizeState.ShrunkFromPrevious)
					PaintSeparatingLine(drawingContext, cache, group);
			}
		}
		void PaintSeparatingLine(DragAreaDrawingContext drawingContext, GraphicsCache cache, DragGroup group) {
			Rectangle separator = new Rectangle(group.Bounds.X, group.Bounds.Y - SpaceBetweenGroups - 1, group.Bounds.Width, SpaceBetweenGroups + 2);
			ObjectPainter.DrawObject(cache, drawingContext.Painters.GroupPainter.ObjectPainter,
				new GroupInfoArgs(cache, separator, drawingContext.Appearances.GroupAppearance, DragGroupState.Hot));
		}
		public DragAreaSelection GetSelection(Point point) {
			DragAreaSelection selection = GetSelectionInternal(point);
			if (selection != null)
				return selection;
			foreach (DragGroup group in ActualGroups) {
				selection = group.GetSelection(point);
				if (selection != null)
					return selection;
			}
			return null;
		}
		public IDropAction GetDropAction(Point point, IDragObject dragObject) {
			IDropAction dropAction = null;
			if(AcceptableDragObject(dragObject)) {
				dropAction = GetGroupDropAction(dragObject, point);
				if(dropAction != null)
					return dropAction;
				foreach(DragGroup group in ActualGroups)
					if(group.Bounds.Contains(point)) {
						dropAction = group.GetDropAction(point, dragObject);
						if(dropAction != null)
							return dropAction;
					}
			}
			return null;
		}
		public void ClearContent(DragAreaHistoryItem historyItem) {
			foreach(DragGroup group in ActualGroups)
				if(!group.IsEmpty)
					group.ClearContent(historyItem);
		}
		protected virtual IDropAction GetGroupDropAction(IDragObject dragObject, Point point) {
			return null;
		}
		public virtual XtraForm CreateOptionsForm(DashboardDesigner designer, DragGroup group) {
			return null;
		}
		public virtual void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record) {
		}
		public abstract bool AcceptableDragObject(IDragObject dragObject);
		public virtual int IndexOf(DragGroup item) {
			return -1;
		}
		public virtual DragGroup GetActualGroup(int index) {
			if(index == 0)
				foreach(DragGroup group in ActualGroups)
					return group;
			throw new IndexOutOfRangeException();
		}
	}
}
