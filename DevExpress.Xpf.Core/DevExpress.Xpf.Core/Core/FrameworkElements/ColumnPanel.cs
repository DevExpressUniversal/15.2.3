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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core {
	public interface IItemsControl {
		void SetItemsHost(Panel itemsHost);
	}
	public interface IColumnPanel {
		int ColumnCount { get; set; }
		int[] ColumnOffsets { get; set; }
		int[] RowOffsets { get; set; }
	}
	public class ColumnPanel : Panel, IColumnPanel {
		public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(int), typeof(ColumnPanel),
			new PropertyMetadata((d, e) => ((ColumnPanel)d).PropertyChangedColumnCount()));
		public static readonly DependencyProperty ColumnOffsetsProperty = DependencyProperty.Register("ColumnOffsets", typeof(int[]), typeof(ColumnPanel),
			new PropertyMetadata((d, e) => ((ColumnPanel)d).PropertyChangedColumnOffsets()));
		public static readonly DependencyProperty RowOffsetsProperty = DependencyProperty.Register("RowOffsets", typeof(int[]), typeof(ColumnPanel),
			new PropertyMetadata((d, e) => ((ColumnPanel)d).PropertyChangedRowOffsets()));
		Size calculatedPanelSize;
		Rect[] calculatedChildBoundsArray;
		bool itemsHostChecked;
		public int ColumnCount {
			get { return (int)GetValue(ColumnCountProperty); }
			set { SetValue(ColumnCountProperty, value); }
		}
		[TypeConverter(typeof(ColumnPanelOffsetsPropertyConverter))]
		public int[] ColumnOffsets {
			get { return (int[])GetValue(ColumnOffsetsProperty); }
			set { SetValue(ColumnOffsetsProperty, value); }
		}
		[TypeConverter(typeof(ColumnPanelOffsetsPropertyConverter))]
		public int[] RowOffsets {
			get { return (int[])GetValue(RowOffsetsProperty); }
			set { SetValue(RowOffsetsProperty, value); }
		}
		protected override Size ArrangeOverride(Size finalSize) {
			int childIndex = 0;
			foreach(UIElement child in Children)
				if(child.Visibility == Visibility.Visible)
					child.Arrange(calculatedChildBoundsArray[childIndex++]);
			return calculatedPanelSize;
		}
		protected virtual void CheckItemsHost() {
			if(itemsHostChecked) return;
			IItemsControl itemsControl = LayoutHelper.FindParentObject<IItemsControl>(this) as IItemsControl;
			if(itemsControl != null)
				itemsControl.SetItemsHost(this);
			itemsHostChecked = true;
		}
		protected virtual IColumnPanelLayoutCalculator CreateLayoutCalculator() {
			return new ColumnPanelLayoutCalculator();
		}
		protected virtual int GetVisibleChildCount() {
			int result = 0;
			foreach(UIElement child in Children)
				if(child.Visibility == Visibility.Visible)
					result++;
			return result;
		}
		protected override Size MeasureOverride(Size availableSize) {
			CheckItemsHost();
			foreach(UIElement child in Children)
				child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			int visibleChildCount = GetVisibleChildCount();
			Size<int>[] childSizes = new Size<int>[visibleChildCount];
			int childIndex = 0;
			foreach(UIElement child in Children)
				if(child.Visibility == Visibility.Visible)
					childSizes[childIndex++] = new Size<int>((int)Math.Ceiling(child.DesiredSize.Width), (int)Math.Ceiling(child.DesiredSize.Height));
			Size<int> panelSize;
			Point<int>[] childPositions;
			int[] columnOffsets = (ColumnOffsets != null) ? ColumnOffsets : new int[] { 0 };
			int[] rowOffsets = (RowOffsets != null) ? RowOffsets : new int[] { 0 };
			CreateLayoutCalculator().CalcLayout(childSizes, ColumnCount, columnOffsets, rowOffsets, out panelSize, out childPositions);
			calculatedPanelSize = new Size(panelSize.Width, panelSize.Height);
			calculatedChildBoundsArray = new Rect[visibleChildCount];
			for(int i = 0; i < visibleChildCount; i++) {
				Point<int> p = childPositions[i];
				calculatedChildBoundsArray[i] = new Rect(p.X, p.Y, childSizes[i].Width, childSizes[i].Height);
			}
			return calculatedPanelSize;
		}
		protected virtual void PropertyChangedColumnCount() {
			InvalidateMeasure();
		}
		protected virtual void PropertyChangedColumnOffsets() {
			InvalidateMeasure();
		}
		protected virtual void PropertyChangedRowOffsets() {
			InvalidateMeasure();
		}
	}
	public struct Size<T> where T : IComparable<T> {
		public Size(T width, T height)
			: this() {
			Width = width;
			Height = height;
		}
		public T Height { get; set; }
		public bool IsEmpty {
			get { return Width.CompareTo(default(T)) == 0 || Height.CompareTo(default(T)) == 0; }
		}
		public T Width { get; set; }
		public static bool operator ==(Size<T> size1, Size<T> size2) {
			return size1.Width.CompareTo(size2.Width) == 0 && size1.Height.CompareTo(size2.Height) == 0;
		}
		public static bool operator !=(Size<T> size1, Size<T> size2) {
			return size1.Width.CompareTo(size2.Width) != 0 || size1.Height.CompareTo(size2.Height) != 0;
		}
		public override bool Equals(object obj) {
			if(obj is Size<T>)
				return this == (Size<T>)obj;
			else
				return false;
		}
		public override int GetHashCode() {
			throw new NotImplementedException();
		}
	}
	public struct Point<T> where T : IComparable<T> {
		public Point(T x, T y)
			: this() {
			X = x;
			Y = y;
		}
		public T X { get; set; }
		public T Y { get; set; }
		public static bool operator ==(Point<T> size1, Point<T> size2) {
			return size1.X.CompareTo(size2.X) == 0 && size1.Y.CompareTo(size2.Y) == 0;
		}
		public static bool operator !=(Point<T> size1, Point<T> size2) {
			return size1.X.CompareTo(size2.X) != 0 || size1.Y.CompareTo(size2.Y) != 0;
		}
		public override bool Equals(object obj) {
			if(obj is Point<T>)
				return this == (Point<T>)obj;
			else
				return false;
		}
		public override int GetHashCode() {
			throw new NotImplementedException();
		}
	}
	public interface IColumnPanelLayoutCalculator {
		void CalcLayout(Size<int>[] childSizes, int columnCount, int[] columnOffsets, int[] rowOffsets, out Size<int> panelSize, out Point<int>[] childPositions);
	}
	public class ColumnPanelLayoutCalculator : IColumnPanelLayoutCalculator {
		void CheckOffsetParameter(int itemCount, ref int[] itemOffsets) {
			if(itemOffsets == null || itemOffsets.Length == 0 || itemOffsets.Length != itemCount - 1 && itemOffsets.Length != 1)
				throw new Exception();
			if(itemOffsets.Length < itemCount - 1) {
				int itemOffset = itemOffsets[0];
				itemOffsets = new int[itemCount - 1];
				for(int i = 0; i < itemCount - 1; i++)
					itemOffsets[i] = itemOffset;
			}
		}
		void CheckInputParameters(Size<int>[] childSizes, int columnCount, ref int[] columnOffsets, ref int[] rowOffsets) {
			int realColumnCount, rowCount;
			GetFieldSize(childSizes, columnCount, out realColumnCount, out rowCount);
			CheckOffsetParameter(realColumnCount, ref columnOffsets);
			CheckOffsetParameter(rowCount, ref rowOffsets);
		}
		void GetFieldSize(Size<int>[] childSizes, int columnCount, out int realColumnCount, out int rowCount) {
			int visibleChildCount = GetVisibleChildCount(childSizes);
			realColumnCount = Math.Min(visibleChildCount, columnCount);
			if(realColumnCount == 0)
				rowCount = 0;
			else
				rowCount = (visibleChildCount + realColumnCount - 1) / realColumnCount;
		}
		int GetItemSequenceSize(int itemSize, int[] itemOffsets, int itemCount) {
			int result = itemSize * itemCount;
			for(int i = 0; i < itemCount - 1; i++)
				result += itemOffsets[i];
			return result;
		}
		int GetVisibleChildCount(Size<int>[] childSizes) {
			int result = 0;
			foreach(Size<int> childSize in childSizes)
				if(!childSize.IsEmpty)
					result++;
			return result;
		}
		#region IColumnPanelLayoutCalculator Members
		void IColumnPanelLayoutCalculator.CalcLayout(Size<int>[] childSizes, int columnCount, int[] columnOffsets, int[] rowOffsets,
				out Size<int> panelSize, out Point<int>[] childPositions) {
			CheckInputParameters(childSizes, columnCount, ref columnOffsets, ref rowOffsets);
			Size<int> cellSize = new Size<int>();
			for(int i = 0; i < childSizes.Length; i++) {
				cellSize.Width = Math.Max(childSizes[i].Width, cellSize.Width);
				cellSize.Height = Math.Max(childSizes[i].Height, cellSize.Height);
			}
			int realColumnCount, rowCount;
			GetFieldSize(childSizes, columnCount, out realColumnCount, out rowCount);
			panelSize = new Size<int>(
				GetItemSequenceSize(cellSize.Width, columnOffsets, realColumnCount),
				GetItemSequenceSize(cellSize.Height, rowOffsets, rowCount)
			);
			childPositions = new Point<int>[childSizes.Length];
			int x = 0, y = 0, childIndex = 0;
			for(int i = 0; i < childSizes.Length; i++) {
				if(childSizes[i].IsEmpty) {
					childPositions[i] = new Point<int>();
					continue;
				}
				childPositions[i] = new Point<int>(x + (cellSize.Width - childSizes[i].Width) / 2, y + (cellSize.Height - childSizes[i].Height) / 2);
				if(realColumnCount > 0) {
					if(childIndex % realColumnCount == realColumnCount - 1) {
						x = 0;
						y += cellSize.Height;
						if(childIndex < childSizes.Length - 1)
							y += rowOffsets[childIndex / realColumnCount];
					}
					else
						x += cellSize.Width + columnOffsets[childIndex % realColumnCount];
				}
				childIndex++;
			}
		}
		#endregion
	}
	public class ColumnPanelOffsetsPropertyConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string[] values = ((string)value).Split(new char[] { ',' });
			int[] result = new int[values.Length];
			for(int i = 0; i < values.Length; i++)
				result[i] = int.Parse(values[i]);
			return result;
		}
	}
}
