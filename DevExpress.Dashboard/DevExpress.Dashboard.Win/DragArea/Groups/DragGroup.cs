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

using System;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Controls;
namespace DevExpress.DashboardWin.Native {
	public enum DragGroupState { 
		Normal, 
		Hot, 
		Selected, 
		PlaceHolderDropDestination 
	}
	public abstract class DragGroup : IDisposable, IDragAreaElementWithButton, IDragSource {
		public static Measure GetNumericMeasure(IDragObject dragObject, DataDashboardItem item, bool numericOnly) {
			Measure measure = dragObject.GetMeasure(item);
			if(measure == null)
				return null;
			if(!numericOnly || dragObject.DataSourceSchema == null || !dragObject.DataSourceSchema.IsAggregateCalcField(measure.DataMember))
				return measure;
			DataFieldType type = dragObject.DataSourceSchema.GetFieldType(measure.DataMember);
			if(type.IsNumericType())
				return measure;
			return null;
		}
		readonly OptionsButton optionsButton;
		Rectangle bounds;
		DragSection section;
		DragGroupState state;
		DragItemSizeState sizeState = DragItemSizeState.Normal;
		const int ShrinkSize = 4;
		protected DragGroup(string optionsButtonImageName) {
			if(!String.IsNullOrEmpty(optionsButtonImageName))
				optionsButton = new OptionsButton(this, ImageHelper.GetImage(optionsButtonImageName));
		}
		protected IDataSourceSchema DataSource { 
			get {
				DataDashboardItem dashboardItem = Section.Area.DashboardItem;
				return dashboardItem == null ? null : dashboardItem.DataSourceSchema; 
			} 
		}
		public bool AllowDropWhenNoDropAction { get { return true; } }
		public DragSection Section { get { return section; } }
		public Rectangle Bounds { get { return bounds; } }
		public int SectionIndex { get { return Section.SectionIndex; } }
		public abstract IEnumerable<DragItem> Items { get; }
		public virtual List<DragItem> ItemList {
			get {
				List<DragItem> result = new List<DragItem>();
				foreach(DragItem item in Items)
					result.Add(item);
				return result;
			}
		}
		public abstract int DataItemsCount { get; }
		public DragGroupState State { get { return state; } }
		public DragItemSizeState SizeState {
			get { return sizeState; }
			set { sizeState = value; }
		}
		public Rectangle DrawingBounds {
			get {
				if(sizeState == DragItemSizeState.Normal)
					return bounds;
				Rectangle drawingBounds = bounds;
				drawingBounds.Height -= ShrinkSize;
				if(sizeState == DragItemSizeState.ShrunkFromPrevious)
					drawingBounds.Y += ShrinkSize;
				return drawingBounds;
			}
		}
		public bool IsEmpty {
			get {
				foreach(DragItem item in Items)
					if(item.DataItem != null)
						return false;
				return true;
			}
		}
		int SpaceBetweenGroups { get { return Section.Area.ParentControl.DrawingContext.Painters.GroupPainter.GroupIndent; } }
		public abstract void ClearContent(DragAreaHistoryItem historyItem);
		public void Highlight() {
			if(optionsButton != null)
				state = DragGroupState.Hot;
		}
		public void Select() {
			if(optionsButton != null)
				state = DragGroupState.Selected;
		}
		public void ResetState() {
			state = DragGroupState.Normal;
		}
		public virtual void Initialize(DragSection section) {
			this.section = section;
		}
		public void SetDataSourceSchema(IDataSourceSchema dataSource) {
			foreach (DragItem dragItem in Items) {
				dragItem.DataSourceSchema = dataSource;
			}
		}
		public DragAreaSelection GetSelection(Point point) {
			DragItemPopupMenu popupMenu = Section.Area.ParentControl.PopupMenu;
			Rectangle halfSpaceBefore = new Rectangle(Bounds.X, Bounds.Y - SpaceBetweenGroups / 2, Bounds.Width, SpaceBetweenGroups / 2);
			Rectangle halfSpaceAfter = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height, Bounds.Width, SpaceBetweenGroups / 2);
			if(optionsButton != null && optionsButton.Bounds.Contains(point))
				return new DragAreaSelection(this, null, this, DragAreaSelectionType.OptionsButton);
			foreach(DragItem dragItem in Items)
				if(dragItem.Bounds.Contains(point) && dragItem.DataItem != null) {
					bool hasPopupMenu = popupMenu != null && popupMenu.HasPopupMenu(dragItem);
					return new DragAreaSelection(this, dragItem, hasPopupMenu ? dragItem : null,
						hasPopupMenu && dragItem.HitDragAreaButton(point) ? DragAreaSelectionType.DragItemPopupButton : DragAreaSelectionType.NonEmptyDragItem);
				}
			if((Bounds.Contains(point) || halfSpaceBefore.Contains(point) || halfSpaceAfter.Contains(point)) && section.AllowDragGroups && (DataItemsCount > 0 || IsNotLastEmptyGroup()))
				return new DragAreaSelection(this, null, null, DragAreaSelectionType.Group);
			return null;
		}
		bool IsNotLastEmptyGroup() {
			return Section.IndexOf(this) < Section.ActualGroupCount - 1;
		}
		public void CancelDrop() {
			foreach (DragItem item in Items)
				item.ResetState();
			Section.Area.Invalidate();
		}
		public int CalcWidthWithoutOptionsButton(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			if (optionsButton == null)
				return drawingContext.SectionWidth;
			ObjectPainter painter = drawingContext.Painters.GroupPainter.ObjectPainter;
			int size = drawingContext.DragItemHeight;
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(cache, new Rectangle(0, 0, drawingContext.SectionWidth, size), drawingContext.Appearances.GroupAppearance);
			Rectangle clientRect = painter.GetObjectClientRectangle(args);
			clientRect.Width -= size + drawingContext.Painters.GroupPainter.ButtonIndent;
			args.Bounds = clientRect;
			return painter.CalcBoundsByClientRectangle(args).Width;
		}
		public Rectangle Arrange(DragAreaDrawingContext drawingContext, GraphicsCache cache, Point location) {
			int widthWithoutOptionsButton = CalcWidthWithoutOptionsButton(drawingContext, cache);
			IDragGroupPainter groupPainter = drawingContext.Painters.GroupPainter;
			ObjectPainter painter = groupPainter.ObjectPainter;
			int dragItemHeight = drawingContext.DragItemHeight;
			int verticalItemOffset = dragItemHeight + groupPainter.ItemIndent;
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(cache, 
				new Rectangle(location, new Size(widthWithoutOptionsButton, dragItemHeight)), drawingContext.Appearances.GroupAppearance);
			Rectangle clientRect = painter.GetObjectClientRectangle(args);
			Point itemLocation = clientRect.Location;
			Size dragItemSize = new Size(clientRect.Width, dragItemHeight);
			bounds = Rectangle.Empty;
			foreach (DragItem dragItem in Items) {
				Rectangle dragItemBounds = new Rectangle(itemLocation, dragItemSize);
				Rectangle itemBounds = dragItem.Arrange(drawingContext, cache, dragItemBounds, false, dragItem.DataItemsGroupIndex != Dimension.DefaultGroupIndex);
				bounds = bounds.IsEmpty ? itemBounds : Rectangle.Union(bounds, itemBounds);
				itemLocation.Y += verticalItemOffset;
			}
			AfterBoundsCalculation(bounds);
			bounds.Width = drawingContext.SectionWidth;
			if(optionsButton != null) {
				optionsButton.Arrange(drawingContext, cache);
				SetOptionsButtonState(DragAreaButtonState.Normal);
			}
			return bounds;
		}
		void IDragAreaElementWithButton.SetButtonState(DragAreaButtonState state) {
			SetOptionsButtonState(state);
		}
		void IDragAreaElementWithButton.ExecuteButtonClick(DragAreaControl dragArea) {
			if (optionsButton != null)
				optionsButton.ShowDialog(dragArea);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && optionsButton != null)
				optionsButton.Dispose();
		}
		protected virtual void AfterBoundsCalculation(Rectangle bounds) {
		}
		protected virtual void SetOptionsButtonState(DragAreaButtonState state) {
			if(optionsButton != null)
				optionsButton.SetOptionsButtonState(state);
		}
		public virtual void Paint(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			ObjectPainter.DrawObject(cache, drawingContext.Painters.GroupPainter.ObjectPainter, 
				new GroupInfoArgs(cache, DrawingBounds, drawingContext.Appearances.GroupAppearance, state));
			foreach (DragItem dragItem in Items)
				dragItem.Paint(drawingContext, cache, false, dragItem.DataItemsGroupIndex != Dimension.DefaultGroupIndex);
			if (optionsButton != null)
				optionsButton.PaintGlyph(drawingContext, cache);
		}
		public virtual void Cleanup() {
			ResetState();
		}
		public abstract IDropAction GetDropAction(Point point, IDragObject dragObject);
		public abstract void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record);
		bool IDragSource.AllowNullDrop {
			get { return true; }
		}
		IHistoryItem IDragSource.PerformDrag(IDragObject dragObject, bool isSameDragGroup) {
			DataDashboardItem dashboardItem = Section.Area.DashboardItem;
			DragAreaHistoryItem historyItem = new DragAreaHistoryItem(dashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			ExecuteDragAction(historyItem, dragObject, isSameDragGroup);
			return historyItem;
		}
		void IDragSource.Cancel() {
			foreach(DragItem item in Items)
				item.ResetState();
			Section.Area.Invalidate();
		}
		protected abstract void ExecuteDragAction(DragAreaHistoryItem historyItem, IDragObject dragObject, bool isSameDragGroup);
		public abstract int ActualIndexOf(DragItem dragItem);
		public abstract DataItem GetDataItemByIndex(int index);
	}
	public class GroupInfoArgs : StyleObjectInfoArgs {
		readonly DragGroupState state;
		public new DragGroupState State { get { return state; } }
		public GroupInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, DragGroupState state)
			: base(cache, bounds, appearance) {
			this.state = state;
		}
	}
}
