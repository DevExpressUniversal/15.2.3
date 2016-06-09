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
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public enum DragItemState { Normal, Hot, Selected, DropTarget, PlaceHolder, PlaceHolderDropDestination }
	public enum DragItemSizeState { Normal, ShrunkFromPrevious, ShrunkFromNext }
	public class DragItem : IDragAreaElementWithButton {
		const string DragItemIconsPath = "DragItem.{0}";
		public const string HierarchyIconName = "Hierarchy";
		public const string SortOrderAscendingIconName = "SortOrderAscending";
		public const string SortOrderDescendingIconName = "SortOrderDescending";
		public const string ColoringIconName = "Colorization";
		public const int StatusIconInnerIndent = 2;
		public const int StatusIconOuterIndent = 6;
		const int ShrinkSize = 4;
		IDataSourceSchema dataSource;
		DragGroup group;
		readonly string caption;
		readonly string pluralCaption;
		IList<DataItem> dataItems;
		IList<Image> statusIcons;
		DragItemSizeState sizeState = DragItemSizeState.Normal;
		DragItemState state;
		DragAreaButtonState popupButtonState = DragAreaButtonState.Invisible;
		Rectangle bounds;
		Rectangle captionBounds;
		Rectangle popupButtonHitTestBounds;
		Rectangle popupButtonPaintedBounds;
		public IDataSourceSchema DataSourceSchema {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public string Caption {
			get {
				if(DataItems.Count > 1 && !String.IsNullOrEmpty(pluralCaption))
					return pluralCaption;
				return caption;
			}
		}
		public string ActualCaption {
			get {
				if(DataItem != null)
					return DataItem.GroupName;
				return caption;
			}
		}
		public int DataItemsGroupIndex { get { return DataItem != null ? DataItem.ActualGroupIndex : Dimension.DefaultGroupIndex; } }
		public DataItem DataItem {
			get { return dataItems.Count > 0 ? dataItems[0] : null; }
		}
		public IList<DataItem> DataItems {
			get { return dataItems; }
		}
		public DragItemSizeState SizeState {
			get { return sizeState; }
			set { sizeState = value; }
		}
		public DragItemState State { get { return state; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle DrawingBounds {
			get {
				if(sizeState == DragItemSizeState.Normal)
					return bounds;
				Rectangle drawingBounds = bounds;
				drawingBounds.Height -= ShrinkSize;
				if (sizeState == DragItemSizeState.ShrunkFromPrevious)
					drawingBounds.Y += ShrinkSize;
				return drawingBounds;
			}
		}
		Rectangle ActualCaptionBounds {
			get {
				if(sizeState == DragItemSizeState.Normal)
					return captionBounds;
				Rectangle actualCaptionBounds = captionBounds;
				if(sizeState == DragItemSizeState.ShrunkFromNext)
					actualCaptionBounds.Y -= ShrinkSize / 2;
				if(sizeState == DragItemSizeState.ShrunkFromPrevious)
					actualCaptionBounds.Y += ShrinkSize / 2;
				return actualCaptionBounds;
			}
		}
		public DragGroup Group { get { return group; } }
		public IList<Image> StatusIcons { get { return statusIcons; } }
		public DragItem(IDataSourceSchema dataSource, DataItem dataItem, DragGroup group, string caption, string pluralCaption) {
			this.dataSource = dataSource;
			this.dataItems = new List<DataItem>();
			if(dataItem != null)
				dataItems.Add(dataItem);
			this.caption = caption;
			this.pluralCaption = pluralCaption;
			this.group = group;
			ResetState();
		}
		public DragItem(IDataSourceSchema dataSource, DragGroup group, string caption)
			: this(dataSource, null, group, caption, null) {
		}
		public DragItem(IDataSourceSchema dataSource, DragGroup group, string caption, string pluralCaption)
			: this(dataSource, null, group, caption, pluralCaption) {
		}
		public DragItem(IDataSourceSchema dataSource, DataItem dataItem, DragGroup group)
			: this(dataSource, dataItem, group, String.Empty, null) {
		}
		public void Highlight() {
			state = DragItemState.Hot;
		}
		public void Select() {
			state = DragItemState.Selected;
		}
		public void SetDropDestinationState() {
			state = DragItemState.PlaceHolderDropDestination;
		}
		public void ResetState() {
			state = DataItem == null ? DragItemState.PlaceHolder : DragItemState.Normal;
		}
		public void SetPopupButtonState(DragAreaButtonState state) {
			popupButtonState = state;
		}
		public bool HitDragAreaButton(Point point) {
			return popupButtonState != DragAreaButtonState.Disabled && popupButtonHitTestBounds.Contains(point);
		}
		public Rectangle Arrange(DragAreaDrawingContext drawingContext, GraphicsCache cache, Rectangle bounds, bool isDragging, bool isHierarchy) {
			this.bounds = bounds;
			DragAreaPainters painters = drawingContext.Painters;
			statusIcons = GetStatusIcon(isDragging, isHierarchy);
			captionBounds = painters.DragItemPainter.GetObjectClientRectangle(new StyleObjectInfoArgs(cache, bounds, drawingContext.Appearances.ItemAppearance));
			if(isDragging && isHierarchy)
				captionBounds = new Rectangle(new Point(captionBounds.X, captionBounds.Y + 2), captionBounds.Size);
			if(statusIcons.Count > 0) {
				captionBounds.X = captionBounds.X + StatusIconOuterIndent;
				int squeezingIconsCount = statusIcons.Count - 1;
				int captionCenter = captionBounds.X + captionBounds.Width / 2;
				captionBounds.X = squeezingIconsCount * (statusIcons[0].Width + StatusIconInnerIndent) + captionBounds.X;
				captionBounds.Width = (captionCenter - captionBounds.X) * 2;
			}
			int right = captionBounds.Right;
			popupButtonHitTestBounds = new Rectangle(right, bounds.Top, bounds.Right - right, bounds.Height);
			popupButtonPaintedBounds = painters.DragItemOptionsButtonPainter.GetActualBounds(popupButtonHitTestBounds);
			return painters.GroupPainter.ObjectPainter.CalcBoundsByClientRectangle(new StyleObjectInfoArgs(cache, bounds, drawingContext.Appearances.GroupAppearance));
		}
		public void Paint(DragAreaDrawingContext drawingContext, GraphicsCache cache, bool isDragging, bool isHierarchy) {
			DragItemState currentState = isDragging ? DragItemState.Normal : state;
			DragAreaPainters painters = drawingContext.Painters;
			DragAreaAppearances appearances = drawingContext.Appearances;
			AppearanceObject appearance = appearances.GetItemAppearance(currentState);
			ObjectPainter.DrawObject(cache, painters.DragItemPainter, new DragItemInfoArgs(cache, DrawingBounds, bounds.Height, StatusIconInnerIndent, appearance, currentState, statusIcons, isDragging && isHierarchy));
			DragAreaDrawingContext.DrawString(ActualCaption, appearance, cache, ActualCaptionBounds);
			if (popupButtonState != DragAreaButtonState.Invisible)
				ObjectPainter.DrawObject(cache, painters.DragItemOptionsButtonPainter.ObjectPainter,
					new ElementWithButtonInfoArgs(cache, popupButtonPaintedBounds, appearances.ItemOptionsButtonAppearance, popupButtonState));
		}
		public void SetDataItem(DataItem dataItem, int index) {
			if(dataItems.Count > index)
				dataItems[index] = dataItem;
			else
				dataItems.Add(dataItem);
			ResetState();
		}
		public void AddDataItem(DataItem dataItem){
			dataItems.Add(dataItem);
		}
		void IDragAreaElementWithButton.SetButtonState(DragAreaButtonState state) {
			SetPopupButtonState(state);
		}
		void IDragAreaElementWithButton.ExecuteButtonClick(DragAreaControl dragArea) {
			if (dragArea.ShowPopupMenu(this, new Point(popupButtonPaintedBounds.Left, popupButtonPaintedBounds.Bottom)))
				SetPopupButtonState(DragAreaButtonState.Selected);
		}
		IList<Image> GetStatusIcon(bool isDragging, bool isHierarchy) {
			List<Image> glyphs = new List<Image>();
			DataDashboardItem dashboardItem = group != null ? group.Section.Area.ParentControl.DashboardItem : null;
			if(isHierarchy) {
				Image hierarchyIcon = ImageHelper.GetImage(String.Format(DragItemIconsPath, HierarchyIconName));
				hierarchyIcon.Tag = HierarchyIconName;
				glyphs.Add(hierarchyIcon);
			}
			else {
				if(!isDragging) {
					Dimension dimension = DataItem as Dimension;
					bool isSortingEnabled = dimension != null && dashboardItem != null && dashboardItem.IsSortingEnabled(dimension);
					if(isSortingEnabled) {
						DimensionSortOrder actualSortOrder = dimension.ActualSortOrder;
						if(actualSortOrder != DimensionSortOrder.None) {
							string iconName = actualSortOrder == DimensionSortOrder.Ascending ? SortOrderAscendingIconName : SortOrderDescendingIconName;
							Image icon = ImageHelper.GetImage(String.Format(DragItemIconsPath, iconName));
							icon.Tag = iconName;
							glyphs.Add(icon);
						}
					}
				}
			}
			foreach(DataItem dataItem in DataItems) {
				if(dataItem != null && dashboardItem.IsColoringEnabled(dataItem)) {
					Image icon = ImageHelper.GetImage(String.Format(DragItemIconsPath, ColoringIconName));
					icon.Tag = ColoringIconName;
					glyphs.Add(icon);
					break;
				}
			}
			return glyphs;
		}
	}
	public class DragItemInfoArgs : StyleObjectInfoArgs {
		readonly DragItemState itemState;
		readonly IList<Image> glyphs;
		readonly bool isDraggingHierarchy;
		readonly int normalGroupHeight;
		readonly int statusIconInnerIndent;
		Rectangle currentBounds;
		public DragItemState ItemState { get { return itemState; } }
		public IList<Image> Glyphs { get { return glyphs; } }
		public bool IsDraggingHierarchy { get { return isDraggingHierarchy; } }
		public Rectangle CurrentBounds {
			get {
				return currentBounds;
			}
			set {
				currentBounds = value;
			}
		}
		public int NormalGroupHeight { get { return normalGroupHeight; } }
		public int StatusIconInnerIndent { get { return statusIconInnerIndent; } }
		public DragItemInfoArgs(GraphicsCache cache, Rectangle bounds, int normalGroupHeight, int statusIconInnerIndent, AppearanceObject appearance, DragItemState itemState, IList<Image> glyphs, bool isDraggingHierarchy)
			: base(cache, bounds, appearance) {
			this.itemState = itemState;
			this.isDraggingHierarchy = isDraggingHierarchy;
			this.currentBounds = bounds;
			this.normalGroupHeight = normalGroupHeight;
			this.statusIconInnerIndent = statusIconInnerIndent;
			if(glyphs != null && Appearance != null)
				this.glyphs = glyphs.Select(image => (Image)ImageHelper.ColorBitmap(image, Appearance.ForeColor, 0.35f)).ToList();
		}
	}
}
