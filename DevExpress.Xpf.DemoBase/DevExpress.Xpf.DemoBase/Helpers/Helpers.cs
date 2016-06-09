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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.DemoData.Helpers;
#if !DEMO
using System.Windows.Input;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DevExpress.Mvvm;
using System.Diagnostics;
#endif
namespace DevExpress.Xpf.DemoBase.Helpers {
	class EmptyCommand : ICommand {
		bool canExecute;
		public EmptyCommand(bool canExecute) {
			this.canExecute = canExecute;
		}
		public bool CanExecute(object parameter) { return canExecute; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) { }
	}
	public class DefferableValue<T> : BindableBase {
		Func<object> getValueCallback;
		public DefferableValue(Func<object> getValueCallback) {
			this.getValueCallback = getValueCallback;
		}
		public bool LoadingInProgress {
			get { return GetProperty(() => LoadingInProgress); }
			set { SetProperty(() => LoadingInProgress, value, RaiseLoadingInProgressChanged); }
		}
		public bool ValueLoaded {
			get { return GetProperty(() => ValueLoaded); }
			set { SetProperty(() => ValueLoaded, value); }
		}
		public object Value {
			get { return GetProperty(() => Value); }
			set { SetProperty(() => Value, value); }
		}
		public event ThePropertyChangedEventHandler<bool> LoadingInProgressChanged;
		public void Load() {
			if(ValueLoaded || LoadingInProgress) return;
			LoadingInProgress = true;
			object value = null;
			DevExpress.DemoData.Helpers.BackgroundHelper.DoInBackground(() => {
				value = this.getValueCallback();
			}, () => {
				Value = value;
				ValueLoaded = true;
				LoadingInProgress = false;
			});
		}
		void RaiseLoadingInProgressChanged() {
			if(LoadingInProgressChanged != null)
				LoadingInProgressChanged(this, new ThePropertyChangedEventArgs<bool>(false, LoadingInProgress));
		}
		public override string ToString() {
			return Value == null ? "NULL" : Value.ToString();
		}
	}
	enum TextTrimmingMode { Default, None, CharacterEllipsis, WordEllipsis }
	static class TextTrimmingHelper {
		#region Dependency Properties
		public static readonly DependencyProperty TextTrimmingProperty;
		static TextTrimmingHelper() {
			Type ownerType = typeof(TextTrimmingHelper);
			TextTrimmingProperty = DependencyProperty.RegisterAttached("TextTrimming", typeof(TextTrimmingMode), ownerType, new PropertyMetadata(TextTrimmingMode.Default, RaiseTextTrimmingChanged));
		}
		#endregion
		public static void SetTextTrimming(TextBlock tb, TextTrimmingMode v) { tb.SetValue(TextTrimmingProperty, v); }
		public static TextTrimmingMode GetTextTrimming(TextBlock tb) { return (TextTrimmingMode)tb.GetValue(TextTrimmingProperty); }
		static void RaiseTextTrimmingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TextTrimmingMode newValue = (TextTrimmingMode)e.NewValue;
			if(newValue == TextTrimmingMode.Default) return;
			TextBlock tb = (TextBlock)d;
			switch(newValue) {
				case TextTrimmingMode.WordEllipsis: tb.TextTrimming = TextTrimming.WordEllipsis; break;
				case TextTrimmingMode.CharacterEllipsis: tb.TextTrimming = TextTrimming.CharacterEllipsis; break;
				default: tb.TextTrimming = TextTrimming.None; break;
			}
		}
	}
#if !DEMO
	public class TileExt : Tile, IButtonExt {
		public static readonly DependencyProperty CommandExtProperty =
			DependencyProperty.Register("CommandExt", typeof(ICommand), typeof(TileExt), new PropertyMetadata(null,
				(d, e) => ((TileExt)d).OnCommandExtChanged(e)));
		public static readonly DependencyProperty CommandExtParameterProperty =
			DependencyProperty.Register("CommandExtParameter", typeof(object), typeof(TileExt), new PropertyMetadata(null,
				(d, e) => ((TileExt)d).OnCommandExtParameterChanged(e)));
		public static readonly DependencyProperty ExecuteCommandExtOnClickProperty =
			DependencyProperty.Register("ExecuteCommandExtOnClick", typeof(bool), typeof(TileExt), new PropertyMetadata(true));
		public static readonly DependencyProperty ExecuteCommandExtOnTapProperty =
			DependencyProperty.Register("ExecuteCommandExtOnTap", typeof(bool), typeof(TileExt), new PropertyMetadata(true));
		public static readonly DependencyProperty IsMouseOverExtProperty =
			DependencyProperty.Register("IsMouseOverExt", typeof(bool), typeof(TileExt), new PropertyMetadata(false,
				(d, e) => ((TileExt)d).OnIsMouseOverExtChanged(e)));
		public static readonly DependencyProperty IsHoveredExtProperty =
			DependencyProperty.Register("IsHoveredExt", typeof(bool), typeof(TileExt), new PropertyMetadata(false));
		public static readonly DependencyProperty IsPressedByStylusProperty =
			DependencyProperty.Register("IsPressedByStylus", typeof(bool), typeof(TileExt), new PropertyMetadata(false,
				(d, e) => ((TileExt)d).OnIsPressedByStylusChanged(e)));
		bool isStylusOver = false;
		public TileExt() {
			this.SetDefaultStyleKey(typeof(TileExt));
			IsMouseOverHelper.SetEnableIsMouseOver(this, (Action<bool>)SetIsMouseOverExt);
		}
		public ICommand CommandExt { get { return (ICommand)GetValue(CommandExtProperty); } set { SetValue(CommandExtProperty, value); } }
		public object CommandExtParameter { get { return GetValue(CommandExtParameterProperty); } set { SetValue(CommandExtParameterProperty, value); } }
		public bool ExecuteCommandExtOnClick { get { return (bool)GetValue(ExecuteCommandExtOnClickProperty); } set { SetValue(ExecuteCommandExtOnClickProperty, value); } }
		public bool ExecuteCommandExtOnTap { get { return (bool)GetValue(ExecuteCommandExtOnTapProperty); } set { SetValue(ExecuteCommandExtOnTapProperty, value); } }
		public bool IsMouseOverExt { get { return (bool)GetValue(IsMouseOverExtProperty); } set { SetValue(IsMouseOverExtProperty, value); } }
		public bool IsHoveredExt { get { return (bool)GetValue(IsHoveredExtProperty); } set { SetValue(IsHoveredExtProperty, value); } }
		public bool IsPressedByStylus { get { return (bool)GetValue(IsPressedByStylusProperty); } set { SetValue(IsPressedByStylusProperty, value); } }
		public virtual void PerformClick() {
			if(ExecuteCommandExtOnClick && CommandExt != null && CommandExt.CanExecute(CommandExtParameter))
				CommandExt.Execute(CommandExtParameter);
		}
		public virtual void PerformTap() {
			if(ExecuteCommandExtOnTap && CommandExt != null && CommandExt.CanExecute(CommandExtParameter))
				CommandExt.Execute(CommandExtParameter);
		}
		protected virtual void OnCommandExtChanged(DependencyPropertyChangedEventArgs e) { }
		protected virtual void OnCommandExtParameterChanged(DependencyPropertyChangedEventArgs e) { }
		protected override void OnClick() {
			base.OnClick();
			PerformClick();
		}
		protected virtual void OnIsMouseOverExtChanged(DependencyPropertyChangedEventArgs e) {
			UpdateIsHoveredExt();
		}
		void SetIsMouseOverExt(bool isMouseOver) {
			IsMouseOverExt = isMouseOver && !isStylusOver && (Stylus.CurrentStylusDevice == null || Stylus.CurrentStylusDevice.InAir);
			isStylusOver = false;
		}
		protected virtual void OnIsPressedByStylusChanged(DependencyPropertyChangedEventArgs e) {
			isStylusOver = true;
			UpdateIsHoveredExt();
		}
		protected virtual void UpdateIsHoveredExt() {
			IsHoveredExt = IsMouseOverExt || IsPressedByStylus;
		}
	}
	public class ToggleTileExt : TileExt {
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleTileExt), new PropertyMetadata(false));
		public static readonly DependencyProperty CheckOnClickProperty =
			DependencyProperty.Register("CheckOnClick", typeof(bool), typeof(ToggleTileExt), new PropertyMetadata(true));
		public static readonly DependencyProperty CheckOnTapProperty =
			DependencyProperty.Register("CheckOnTap", typeof(bool), typeof(ToggleTileExt), new PropertyMetadata(true));
		public static readonly DependencyProperty ToggleOnMouseOverProperty =
			DependencyProperty.Register("ToggleOnMouseOver", typeof(bool), typeof(ToggleTileExt), new PropertyMetadata(false));
		public ToggleTileExt() {
			this.SetDefaultStyleKey(typeof(ToggleTileExt));
		}
		public bool IsChecked { get { return (bool)GetValue(IsCheckedProperty); } set { SetValue(IsCheckedProperty, value); } }
		public bool CheckOnClick { get { return (bool)GetValue(CheckOnClickProperty); } set { SetValue(CheckOnClickProperty, value); } }
		public bool CheckOnTap { get { return (bool)GetValue(CheckOnTapProperty); } set { SetValue(CheckOnTapProperty, value); } }
		public bool ToggleOnMouseOver { get { return (bool)GetValue(ToggleOnMouseOverProperty); } set { SetValue(ToggleOnMouseOverProperty, value); } }
		public override void PerformClick() {
			base.PerformClick();
			if(CheckOnClick)
				IsChecked = true;
		}
		public override void PerformTap() {
			base.PerformTap();
			if(CheckOnTap)
				IsChecked = true;
		}
		protected override void OnIsMouseOverExtChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsMouseOverExtChanged(e);
			bool newValue = (bool)e.NewValue;
			if(ToggleOnMouseOver)
				IsChecked = newValue;
		}
		protected override void UpdateIsHoveredExt() {
			IsHoveredExt = (!ToggleOnMouseOver && IsMouseOverExt) || IsPressedByStylus;
		}
	}
	class CustomTileLayoutInfo {
		public int Column { get; set; }
		public int Row { get; set; }
		public int RowSpan { get; set; }
		public int ColumnSpan { get; set; }
	}
	abstract class CustomTileLayoutCalculator {
		public abstract void CalculateLayout(List<CustomTileLayoutInfo> items, int rowsCount, int columnsCount);
	}
	class VerticalTileLayoutCalculator : CustomTileLayoutCalculator {
		class LayoutCell {
			public LayoutCell(int row, int column, int layoutIndex) {
				Row = row;
				Column = column;
				LayoutIndex = layoutIndex;
			}
			public int Row { get; private set; }
			public int Column { get; private set; }
			public int LayoutIndex { get; private set; }
			#region Equality
			public override int GetHashCode() {
				return LayoutIndex;
			}
			public static bool operator ==(LayoutCell cell1, LayoutCell cell2) {
				bool cell1IsNull = (object)cell1 == null;
				bool cell2IsNull = (object)cell2 == null;
				if(cell1IsNull && cell2IsNull) return true;
				if(cell1IsNull || cell2IsNull) return false;
				return cell1.LayoutIndex == cell2.LayoutIndex;
			}
			public static bool operator !=(LayoutCell cell1, LayoutCell cell2) {
				return !(cell1 == cell2);
			}
			public override bool Equals(object obj) {
				return this == obj as LayoutCell;
			}
			#endregion
		}
		class LayoutGrid {
			List<LayoutCell> occupiedCellsList = new List<LayoutCell>();
			Dictionary<LayoutCell, bool> occupiedCellsSet = new Dictionary<LayoutCell, bool>();
			public LayoutGrid(int cellsCount, int rowsCount) {
				RowsCount = rowsCount;
				ColumnsCount = (cellsCount + rowsCount - 1) / rowsCount;
			}
			public int ColumnsCount { get; private set; }
			public int RowsCount { get; private set; }
			public void AddColumn() {
				++ColumnsCount;
			}
			public LayoutCell GetCellByLayoutIndex(int layoutIndex) {
				return new LayoutCell(layoutIndex % RowsCount, layoutIndex / RowsCount, layoutIndex);
			}
			public LayoutCell GetCellByRowColumn(int row, int column) {
				return new LayoutCell(row, column, column * RowsCount + row);
			}
			public bool IsCellFree(LayoutCell cell) {
				return !occupiedCellsSet.ContainsKey(cell);
			}
			public bool IsAreaFree(LayoutCell cell, int rowSpan, int columnSpan, out bool finish) {
				int nextColumn = cell.Column + columnSpan;
				int nextRow = cell.Row + rowSpan;
				if(nextColumn > ColumnsCount) {
					finish = true;
					return false;
				}
				finish = false;
				if(nextRow > RowsCount) return false;
				for(int column = cell.Column; column < nextColumn; ++column) {
					for(int row = cell.Row; row < nextRow; ++row) {
						if(!IsCellFree(GetCellByRowColumn(row, column))) return false;
					}
				}
				return true;
			}
			public void OccupyArea(LayoutCell cell, int rowSpan, int columnSpan) {
				int nextColumn = cell.Column + columnSpan;
				int nextRow = cell.Row + rowSpan;
				for(int column = cell.Column; column < nextColumn; ++column) {
					for(int row = cell.Row; row < nextRow; ++row) {
						LayoutCell areaCell = GetCellByRowColumn(row, column);
						occupiedCellsList.Add(areaCell);
						occupiedCellsSet.Add(areaCell, true);
					}
				}
			}
		}
		public override void CalculateLayout(List<CustomTileLayoutInfo> items, int rowsCount, int columnsCount) {
			if(rowsCount < 0)
				rowsCount = 100;
			int cellsCount = 0;
			foreach(CustomTileLayoutInfo item in items) {
				cellsCount += item.ColumnSpan * item.RowSpan;
			}
			LayoutGrid grid = new LayoutGrid(cellsCount, rowsCount);
			int layoutIndex = 0;
			foreach(CustomTileLayoutInfo item in items) {
				if(item.ColumnSpan == 1 && item.RowSpan == 1) {
					++layoutIndex;
					continue;
				}
				LayoutCell itemCell = FindCell(rowsCount, ref cellsCount, grid, item.RowSpan, item.ColumnSpan, ref layoutIndex);
				grid.OccupyArea(itemCell, item.RowSpan, item.ColumnSpan);
				item.Row = itemCell.Row;
				item.Column = itemCell.Column;
			}
			layoutIndex = 0;
			foreach(CustomTileLayoutInfo item in items) {
				if(item.ColumnSpan != 1 || item.RowSpan != 1) {
					continue;
				}
				LayoutCell itemCell = FindCell(rowsCount, ref cellsCount, grid, item.RowSpan, item.ColumnSpan, ref layoutIndex);
				grid.OccupyArea(itemCell, item.RowSpan, item.ColumnSpan);
				item.Row = itemCell.Row;
				item.Column = itemCell.Column;
			}
		}
		LayoutCell FindCell(int rowsCount, ref int cellsCount, LayoutGrid grid, int rowSpan, int columnSpan, ref int startIndex) {
			int layoutIndex = startIndex;
			LayoutCell itemCell = FindCellForward(cellsCount, grid, rowSpan, columnSpan, ref layoutIndex);
			if(itemCell != null) {
				++startIndex;
				return itemCell;
			}
			layoutIndex = startIndex;
			itemCell = FindCellBackward(grid, rowSpan, columnSpan, ref layoutIndex);
			if(itemCell != null) {
				return itemCell;
			}
			while(true) {
				cellsCount += rowsCount;
				grid.AddColumn();
				itemCell = grid.GetCellByRowColumn(0, grid.ColumnsCount - columnSpan);
				bool finish;
				if(grid.IsAreaFree(itemCell, rowSpan, columnSpan, out finish)) {
					startIndex = itemCell.LayoutIndex + 1;
					return itemCell;
				}
			}
		}
		LayoutCell FindCellForward(int cellsCount, LayoutGrid grid, int rowSpan, int columnSpan, ref int itemIndex) {
			while(true) {
				LayoutCell cell = grid.GetCellByLayoutIndex(itemIndex);
				if(cell.LayoutIndex > cellsCount) break;
				bool finish;
				if(grid.IsAreaFree(cell, rowSpan, columnSpan, out finish)) return cell;
				if(finish) break;
				++itemIndex;
			}
			return null;
		}
		LayoutCell FindCellBackward(LayoutGrid grid, int rowSpan, int columnSpan, ref int itemIndex) {
			while(true) {
				--itemIndex;
				LayoutCell cell = grid.GetCellByLayoutIndex(itemIndex);
				if(cell.LayoutIndex < 0) break;
				bool finish;
				if(grid.IsAreaFree(cell, rowSpan, columnSpan, out finish)) return cell;
			}
			return null;
		}
	}
	class OrdinarTileLayoutCalculator : CustomTileLayoutCalculator {
		int SafeDiv(int n, int by) {
			return n / by + (n % by > 0 ? 1 : 0);
		}
		Tuple<int, int> GetEmptyCell(Dictionary<Tuple<int, int>, bool> grid, int rowCount, int columnCount) {
			for(int c = 0; c < columnCount; c++) {
				for(int r = 0; r < rowCount; r++) {
					if(!grid[Tuple.Create(r, c)]) {
						return Tuple.Create(r, c);
					}
				}
			}
			Debug.Assert(false);
			throw new Exception();
		}
		public override void CalculateLayout(List<CustomTileLayoutInfo> items, int rowCount, int minColumnsCount) {
			if(items.Count == 0)
				return;
			int square = items.Sum(item => item.ColumnSpan * item.RowSpan);
			int columnCount = Math.Max(SafeDiv(square, rowCount), minColumnsCount);
			rowCount = SafeDiv(square, columnCount);
			var grid = new Dictionary<Tuple<int, int>, bool>();
			for(int c = 0; c < columnCount; c++) {
				for(int r = 0; r < rowCount; r++) {
					grid[Tuple.Create(r, c)] = false;
				}
			}
			foreach(CustomTileLayoutInfo item in items) {
				Tuple<int, int> cell = GetEmptyCell(grid, rowCount, columnCount);
				item.Row = cell.Item1;
				item.Column = cell.Item2;
				for(int r = cell.Item1; r < cell.Item1 + item.RowSpan; r++) {
					for(int c = cell.Item2; c < cell.Item2 + item.ColumnSpan; c++) {
						grid[Tuple.Create(r, c)] = true;
					}
				}
			}
		}
	}
#endif
	static class TextBoxHelper {
		#region Dependency Properties
		public static readonly DependencyProperty TextProperty;
		static TextBoxHelper() {
			Type ownerType = typeof(TextBoxHelper);
			TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), ownerType, new PropertyMetadata(Guid.NewGuid().ToString(), RaiseTextChanged));
		}
		#endregion
		public static string GetText(TextBox text) { return (string)text.GetValue(TextProperty); }
		public static void SetText(TextBox text, string v) { text.SetValue(TextProperty, v); }
		static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TextBox textBox = (TextBox)d;
			textBox.TextChanged -= OnTextBoxTextChanged;
			textBox.TextChanged += OnTextBoxTextChanged;
			textBox.Text = (string)e.NewValue;
		}
		static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e) {
			TextBox textBox = (TextBox)sender;
			SetText(textBox, textBox.Text);
		}
	}
	static class DataContextHelper {
		#region Dependency Properties
		public static readonly DependencyProperty DataContextOwnerProperty;
		static DataContextHelper() {
			Type ownerType = typeof(DataContextHelper);
			DataContextOwnerProperty = DependencyProperty.RegisterAttached("DataContextOwner", typeof(DependencyObjectExt), ownerType, new PropertyMetadata(null, RaiseDataContextOwnerChanged));
		}
		#endregion
		public static object GetDataContextOwner(FrameworkElement d) { return d.GetValue(DataContextOwnerProperty); }
		public static void SetDataContextOwner(FrameworkElement d, object v) { d.SetValue(DataContextOwnerProperty, v); }
		static void RaiseDataContextOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DependencyObjectExt owner = (DependencyObjectExt)e.NewValue;
			if(owner == null) return;
			BindingOperations.SetBinding(owner, DependencyObjectExt.DataContextProperty, new Binding("DataContext") { Source = d, Mode = BindingMode.OneWay });
		}
	}
	static class ExceptionHelper {
		public static string GetMessage(Exception e) {
			string message = string.Empty;
			for(Exception exception = e; exception != null; exception = exception.InnerException) {
				message += exception.ToString() + "\n";
			}
			return message;
		}
	}
	class SimpleControl : Control {
		public SimpleControl() { }
	}
	static class EnumHelper {
		public static T[] GetValues<T>() {
			return (T[])Enum.GetValues(typeof(T));
		}
	}
	public static class ListExtensions {
		public static List<T> Convert<T>(this IList list) {
			return new List<T>(GetFilteredItems<T>(list, (t) => true));
		}
		public static T Find<T>(this IList<T> list, Predicate<T> predicate) where T : class {
			foreach(T item in list) {
				if(predicate(item)) return item;
			}
			return null;
		}
		public static List<T> Filter<T>(this IList list, Predicate<T> predicate) {
			return new List<T>(GetFilteredItems<T>(list, predicate));
		}
		public static List<T> Filter<T>(this IList<T> list, Predicate<T> predicate) {
			return new List<T>(GetFilteredItems<T>(list, predicate));
		}
		static IEnumerable<T> GetFilteredItems<T>(this IEnumerable list, Predicate<T> predicate) {
			foreach(T item in list) {
				if(predicate(item)) {
					yield return item;
				}
			}
		}
	}
}
