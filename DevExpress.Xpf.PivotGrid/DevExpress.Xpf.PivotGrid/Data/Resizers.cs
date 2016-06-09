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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class Resizer : IResizeHelperOwner {
		protected const int MinWidthToBorder = 20;
		public static Resizer Create(PivotGridWpfData data, UIElement element, PivotFieldValueItem valueItem, Orientation orientation) {
			if(valueItem == null || valueItem.ResizingField == null || !valueItem.IsLastFieldLevel && valueItem.IsColumn == (orientation == Orientation.Horizontal))
				return new Resizer(data, element, valueItem, orientation);
			if((valueItem.IsColumn && orientation == Orientation.Vertical) ||
					(!valueItem.IsColumn && orientation == Orientation.Horizontal)) {
						return new FieldResizer(data, element, valueItem, orientation);
			} else
				return new FieldCellResizer(data, element, valueItem, orientation);
		}
		PivotGridWpfData data;
		PivotFieldValueItem valueItem;
		Orientation orientation;
		UIElement element;
		public Resizer(PivotGridWpfData data, UIElement element, PivotFieldValueItem valueItem, Orientation orientation) {
			this.data = data;
			this.element = element;
			this.valueItem = valueItem;
			this.orientation = orientation;
		}
		protected PivotGridWpfData Data { get { return data; } }
		protected PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		protected PivotGridControl PivotGrid { get { return Data.PivotGrid; } }
		protected UIElement Element { get { return element; } }		
		protected PivotFieldValueItem ValueItem { get { return valueItem; } }
		protected bool IsColumn { get { return valueItem.IsColumn; } }
		protected Orientation Orientation { get { return orientation; } }
		public virtual bool CanResize { get { return false; } }
		public ControlTemplate ResizingIndicatorTemplate {
			get { return PivotGrid != null ? PivotGrid.ResizingIndicatorTemplate : null; }
		}
		#region IResizeHelperOwner Members
		public virtual double ActualSize { 
			get {
				return Orientation == Orientation.Horizontal ? PivotGridField.DefaultWidth : PivotGridField.DefaultHeight;
			}
			set { }
		}
		public virtual void ChangeSize(double delta) { }
		public virtual void OnDoubleClick() { }
		public virtual void SetIsResizing(bool isResizing) { }
		public virtual bool GetIsResizing() { return false; }
		public virtual SizeHelperBase SizeHelper {
			get { return SizeHelperBase.GetDefineSizeHelper(Orientation); }
		}
		#endregion
	}
	public class FieldResizer : Resizer, IDisposable {
		PivotGridField field;
		bool isResizing;
		double resizeDelta;
		public FieldResizer(PivotGridWpfData data, UIElement element, PivotFieldValueItem valueItem, Orientation orientation)
			: base(data, element, valueItem, orientation) {
			PivotFieldItemBase fieldItem = valueItem.ResizingField;
			if(fieldItem != null && fieldItem.Area == PivotArea.DataArea && orientation == System.Windows.Controls.Orientation.Vertical)
				fieldItem = Data.FieldItems.GetFieldItemByLevel(valueItem.IsColumn, valueItem.IsColumn ? valueItem.Level : valueItem.Index) ?? Data.FieldItems.DataFieldItem;
			this.field = fieldItem != null ? ((PivotFieldItem)fieldItem).Wrapper : null;
		}
		public PivotGridField Field { get { return field; } }
		public override bool CanResize { get { return Field != null && PivotGrid != null && PivotGrid.AllowResizing; } }
		public override double ActualSize {
			get {
				if(Field == null) 
					return base.ActualSize;
				return Orientation == Orientation.Vertical ? Field.Height : Field.Width;
			}
			set {
				if(Field == null)
					base.ActualSize = value;
				else {
					int roundedValue = (int)(value + 0.5);
					if(Orientation == Orientation.Vertical)
						Field.Height = Math.Max(roundedValue, Field.MinHeight);
					else
						Field.Width = Math.Max(roundedValue, Field.MinWidth);
				}
			}
		}
		public override void ChangeSize(double delta) {
			if(Field == null) return;
			this.resizeDelta = delta;
			UpdateIndicatorBounds(delta);
		}
		public override void OnDoubleClick() {
			if(PivotGrid != null && Field != null)
				PivotGrid.BestFit(Field, Orientation == Orientation.Horizontal, Orientation == Orientation.Vertical);
		}
		public override void SetIsResizing(bool isResizing) {
			if(Field == null) return;
			if(this.isResizing == isResizing) return;
			this.isResizing = isResizing;
			if(isResizing) {
				PivotGrid.UserAction = UserAction.FieldResize;
				this.resizeDelta = 0;
				if(PivotGrid != null && PivotGrid.PivotGridScroller != null && PivotGrid.PivotGridScroller.Resizer != null) {
					UpdateIndicatorBounds(0);
					PivotGrid.PivotGridScroller.Resizer.Visibility = Visibility.Visible;
				}
			} else {
				ActualSize += this.resizeDelta;
				DisposeIndicator();
				PivotGrid.UpdateScrollbars();
				PivotGrid.UserAction = UserAction.None;
			}
		}
		public override bool GetIsResizing() {
			return isResizing;
		}
		void DisposeIndicator() {
			if(PivotGrid != null && PivotGrid.PivotGridScroller != null && PivotGrid.PivotGridScroller.Resizer != null)
				PivotGrid.PivotGridScroller.Resizer.Visibility = Visibility.Collapsed;
		}
		public override SizeHelperBase SizeHelper {
			get {
				return SizeHelperBase.GetDefineSizeHelper(Orientation);
			}
		}
		void UpdateIndicatorBounds(double delta) {
			if(PivotGrid == null || PivotGrid.PivotGridScroller == null || PivotGrid.PivotGridScroller.Resizer == null) return;
			Rect r = Orientation == Orientation.Horizontal ? GetVerticalIndicatorBounds(delta) : GetHorizontalIndicatorBounds(delta);
			Rect resizerRect = LayoutHelper.GetRelativeElementRect((UIElement)PivotGrid.PivotGridScroller.Resizer.Parent, PivotGrid);
			PivotGrid.PivotGridScroller.Resizer.Margin = new Thickness(r.Left - resizerRect.Left, r.Top - resizerRect.Top, 0, 0);
			PivotGrid.PivotGridScroller.Resizer.Width = Orientation == System.Windows.Controls.Orientation.Vertical ? r.Width - 1 : r.Width;
			PivotGrid.PivotGridScroller.Resizer.Height = Orientation == System.Windows.Controls.Orientation.Horizontal ? r.Height - 1 : r.Height;
		}
		Rect GetVerticalIndicatorBounds(double delta) {
			if(NeedCheckCellsSize && PivotGrid.PivotGridScroller.Cells.ActualWidth - MinWidthToBorder - delta < 0 && PivotGrid.PivotGridScroller.Cells.ActualWidth > 1 && PivotGrid.FixedRowHeaders)
				delta = PivotGrid.PivotGridScroller.Cells.ActualWidth - MinWidthToBorder;
			if(ActualSize + delta < Field.MinWidth)
				delta = Field.MinWidth - ActualSize;
			this.resizeDelta = delta;
			Rect elementRect = IsColumn || !ValueItem.IsLastFieldLevel ? GetElementRect() : GetElementParentRect();
			double left = elementRect.Right + delta;
			left = Math.Min(left, PivotGrid.ActualWidth - MinWidthToBorder);
			return new Rect(Math.Round(left), Math.Round(elementRect.Top), 1, PivotGrid.ActualHeight - elementRect.Top);
		}
		Rect GetHorizontalIndicatorBounds(double delta) {
			Rect elementRect = GetElementRect();
			if(elementRect.Bottom + delta > PivotGrid.ActualHeight - MinWidthToBorder)
				delta = PivotGrid.ActualHeight - MinWidthToBorder - elementRect.Bottom;
			if(NeedCheckCellsSize && PivotGrid.PivotGridScroller.Cells.ActualHeight - MinWidthToBorder - delta < 0 && PivotGrid.PivotGridScroller.Cells.ActualHeight > 1)
				delta = PivotGrid.PivotGridScroller.Cells.ActualHeight - MinWidthToBorder;
			if(ActualSize + delta < Field.MinHeight)
				delta = Field.MinHeight - ActualSize;
			this.resizeDelta = delta;
			if(IsColumn) {
				Rect parentRect = GetElementParentRect();
				return new Rect(Math.Round(parentRect.Left), Math.Round(elementRect.Bottom + delta), PivotGrid.ActualWidth - parentRect.Left, 1);
			} else {
				return new Rect(Math.Round(elementRect.Left), Math.Round(elementRect.Bottom + delta), PivotGrid.ActualWidth - elementRect.Left, 1);
			}
		}
		Rect GetElementRect() {
			return LayoutHelper.GetRelativeElementRect(Element, PivotGrid);
		}
		Rect GetElementParentRect() {
#if SL
			if(Element == null)
				return new Rect();
#endif
			return LayoutHelper.GetRelativeElementRect((UIElement)VisualTreeHelper.GetParent(Element), PivotGrid);
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			DisposeIndicator();
		}
		#endregion
		public virtual bool NeedCheckCellsSize { get { return true; } }
	}
	public class FieldCellResizer : FieldResizer {
		bool isLastLevelItem;
		public FieldCellResizer(PivotGridWpfData data, UIElement element, PivotFieldValueItem valueItem, Orientation orientation)
			: base(data, element, valueItem, orientation) {
				isLastLevelItem = VisualItems.GetLastLevelItem(ValueItem.IsColumn, ValueItem.MinLastLevelIndex) == ValueItem;
		}
		public override double ActualSize {
			get {
				System.Drawing.Size cellSize = VisualItems.GetLastLevelItemSize(ValueItem);
				return Orientation == Orientation.Horizontal ? cellSize.Width : cellSize.Height;
			}
			set {				
				SetItemSize(ValueItem, value);
				if(isLastLevelItem && IsItemSelected(ValueItem)) 
					SetSelectionSize(ValueItem.IsColumn, value);
			}
		}
		protected void SetItemSize(PivotFieldValueItem valueItem, double value) {
			System.Drawing.Size cellSize = VisualItems.GetLastLevelItemSize(valueItem);
			int roundedValue = (int)(value + .5);
			if(Orientation == Orientation.Horizontal) 
				cellSize.Width = Math.Max(roundedValue, Convert.ToInt32(Field.MinWidth));
			else
				cellSize.Height = Math.Max(roundedValue,  Convert.ToInt32(Field.MinHeight));
			VisualItems.SetItemSize(valueItem, cellSize);
			if(valueItem.ResizingField == null) 
				Data.OnFieldSizeChanged(Data.DataField, Orientation == System.Windows.Controls.Orientation.Horizontal, Orientation != System.Windows.Controls.Orientation.Horizontal);
		}
		protected void SetSelectionSize(bool isColumn, double value) {
			System.Drawing.Rectangle selection = VisualItems.Selection;
			int start = isColumn ? selection.Left : selection.Top,
				end = isColumn ? selection.Right : selection.Bottom;
			for(int i = start; i < end; i++) {
				PivotFieldValueItem valueItem = VisualItems.GetLastLevelItem(isColumn, i);
				if(IsItemSelected(valueItem))
					SetItemSize(valueItem, value);
			}
		}
		protected bool IsItemSelected(PivotFieldValueItem valueItem) {
			int lastLevelIndex = valueItem.MinLastLevelIndex;
			if(valueItem.IsColumn)
				return VisualItems.IsWholeColumnSelected(lastLevelIndex);
			else
				return VisualItems.IsWholeRowSelected(lastLevelIndex);
		}
		public override void OnDoubleClick() {
			if(ValueItem.MinLastLevelIndex != ValueItem.MaxLastLevelIndex) return;
			Dictionary<int, int> items = new Dictionary<int, int>();
			int count;
			foreach(System.Drawing.Point p in Data.VisualItems.SelectedCells) {
				int position = IsColumn ? p.X : p.Y;
				if(!items.TryGetValue(position, out count)) {
					items.Add(position, 1);
				} else {
					items[position] = count + 1;
				}
			}
			int value;
			bool contains = items.TryGetValue(ValueItem.MinLastLevelIndex, out value);
			if(contains && value == (IsColumn ? Data.VisualItems.RowCount : Data.VisualItems.ColumnCount)) {
				foreach(KeyValuePair<int, int> pair in items) {
					if(pair.Value == (IsColumn ? Data.VisualItems.RowCount : Data.VisualItems.ColumnCount))
						Data.PivotGrid.BestFit(IsColumn, pair.Key);
				}
			} else {
				Data.PivotGrid.BestFit(IsColumn, ValueItem.MinLastLevelIndex);
			}
		}
		public override bool NeedCheckCellsSize { get { return false; } }
	}
}
