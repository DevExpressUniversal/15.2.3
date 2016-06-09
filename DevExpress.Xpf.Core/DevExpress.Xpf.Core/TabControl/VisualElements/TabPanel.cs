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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Native {
	public class TabPanelSelector : IValueConverter {
		ItemsPanelTemplate multiLineView, scrollView, stretchView;
		public ItemsPanelTemplate MultiLineView { get { return multiLineView; } set { multiLineView = value; multiLineView.Do(x => x.Seal()); } }
		public ItemsPanelTemplate ScrollView { get { return scrollView; } set { scrollView = value; scrollView.Do(x => x.Seal()); } }
		public ItemsPanelTemplate StretchView { get { return stretchView; } set { stretchView = value; stretchView.Do(x => x.Seal()); } }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null) return null;
			if(value is TabControlMultiLineView)
				return MultiLineView;
			if(value is TabControlScrollView)
				return ScrollView;
			if(value is TabControlStretchView)
				return StretchView;
			throw new NotImplementedException();
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[ContentProperty("Child")]
	public class TabPanelContainer : TabOrientablePanel {
		public static readonly DependencyProperty NormalMarginProperty = DependencyProperty.Register("NormalMargin", typeof(Thickness), typeof(TabPanelContainer), 
			new PropertyMetadata(new Thickness(), (d, e) => ((TabPanelContainer)d).UpdateMargings()));
		public static readonly DependencyProperty NormalPaddingProperty = DependencyProperty.Register("NormalPadding", typeof(Thickness), typeof(TabPanelContainer), 
			new PropertyMetadata(new Thickness(), (d, e) => ((TabPanelContainer)d).UpdateMargings()));
		public static readonly DependencyProperty HoldMarginProperty = DependencyProperty.Register("HoldMargin", typeof(Thickness), typeof(TabPanelContainer),
			new PropertyMetadata(new Thickness(), (d, e) => ((TabPanelContainer)d).UpdateMargings()));
		public static readonly DependencyProperty HoldPaddingProperty = DependencyProperty.Register("HoldPadding", typeof(Thickness), typeof(TabPanelContainer),
			new PropertyMetadata(new Thickness(), (d, e) => ((TabPanelContainer)d).UpdateMargings()));
		public Thickness NormalMargin { get { return (Thickness)GetValue(NormalMarginProperty); } set { SetValue(NormalMarginProperty, value); } }
		public Thickness NormalPadding { get { return (Thickness)GetValue(NormalPaddingProperty); } set { SetValue(NormalPaddingProperty, value); } }
		public Thickness HoldMargin { get { return (Thickness)GetValue(HoldMarginProperty); } set { SetValue(HoldMarginProperty, value); } }
		public Thickness HoldPadding { get { return (Thickness)GetValue(HoldPaddingProperty); } set { SetValue(HoldPaddingProperty, value); } }
		public Thickness ActualMargin { 
			get { return RotatableHelper.CorrectThicknessBasedOnOrientation(ViewInfo.If(x => x.IsHoldMode).Return(x => HoldMargin, () => NormalMargin), ViewInfo); } 
		}
		public Thickness ActualPadding { 
			get { return RotatableHelper.CorrectThickness(ViewInfo.If(x => x.IsHoldMode).Return(x => HoldPadding, () => NormalPadding), ViewInfo); } 
		}
		public double ActualLength {
			get {
				var orientation = ViewInfo.Return(x => x.Orientation, () => Orientation.Horizontal);
				if(orientation == Orientation.Horizontal)
					return Math.Max(0d, ActualChild.ActualWidth - ActualPadding.Left - ActualPadding.Right);
				else return Math.Max(0d, ActualChild.ActualHeight - ActualPadding.Top - ActualPadding.Bottom);
			}
		}
		public DXBorder ActualChild { get; private set; }
		public DXBorder ActualControlBox { get; private set; }
		public ItemsPresenter Child { get { return (ItemsPresenter)ActualChild.Child; } set { ActualChild.Child = value; } }
		public FrameworkElement ControlBox { get { return (FrameworkElement)ActualControlBox.Child; } set { ActualControlBox.Child = value; } }
		public TabPanelBase Panel { get { return LayoutTreeHelper.GetVisualChildren(Child).OfType<TabPanelBase>().FirstOrDefault(); } }
		public TabPanelContainer() {
			ActualChild = new DXBorder() { ClipToBounds = true };
			ActualControlBox = new DXBorder();
			Children.Add(ActualControlBox);
			Children.Add(ActualChild);
			Canvas.SetZIndex(ActualControlBox, 1000);
		}
		protected override void OnViewInfoChanged() {
			base.OnViewInfoChanged();
			UpdateMargings();
		}
		void UpdateMargings() {
			Margin = ActualMargin.Multiply(ScreenHelper.DpiThicknessCorrection);
			ActualChild.Padding = ActualPadding;
		}
	}
	public abstract class TabPanelBase : Panel {
		public static readonly DependencyProperty ChildMinHeightProperty = DependencyProperty.RegisterAttached("ChildMinHeight", typeof(double), typeof(TabPanelBase), new PropertyMetadata(null));
		public static double GetChildMinHeight(FrameworkElement obj) { return (double)obj.GetValue(ChildMinHeightProperty); }
		public static void SetChildMinHeight(FrameworkElement obj, double value) { obj.SetValue(ChildMinHeightProperty, value); }
		public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TabPanelBase), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty HeaderLocationProperty = DependencyProperty.Register("HeaderLocation", typeof(HeaderLocation), typeof(TabPanelBase),
			new FrameworkPropertyMetadata(HeaderLocation.Top, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((TabPanelBase)d).OnHeaderLocationChanged()));
		public int SelectedIndex { get { return (int)GetValue(SelectedIndexProperty); } set { SetValue(SelectedIndexProperty, value); } }
		public HeaderLocation HeaderLocation { get { return (HeaderLocation)GetValue(HeaderLocationProperty); } set { SetValue(HeaderLocationProperty, value); } }
		public Orientation Orientation { get { return RotatableHelper.Convert(HeaderLocation); } }
		protected TabPanelContainer PanelContainer { get { return LayoutTreeHelper.GetVisualParents(this).OfType<TabPanelContainer>().FirstOrDefault(); } }
		protected FrameworkElement ControlBox { get { return PanelContainer.With(x => x.ActualControlBox); } }
		protected SizeHelperBase OrientationHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		protected IEnumerable<FrameworkElement> VisibleChildren { get; private set; }
		protected virtual void OnHeaderLocationChanged() { }
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			UpdateVisibleChildren();
		}
		protected sealed override Size MeasureOverride(Size avSize) {
			BeforeMeasureOverride();
			avSize = CorrectSizeForControlBox(avSize, true, false);
			UpdateVisibleChildren();
			foreach(FrameworkElement child in VisibleChildren)
				SetMinHeight(child, double.NaN);
			Size res;
			if(VisibleChildren.Count() == 0)
				res = base.MeasureOverride(avSize);
			else res = CorrectSize(MeasureOverrideCore(CorrectSize(avSize)));
			return CorrectSizeForControlBox(res, false, true);
		}
		protected sealed override Size ArrangeOverride(Size avSize) {
			avSize = CorrectSizeForControlBox(avSize, true, false);
			if(VisibleChildren.Count() == 0) return base.ArrangeOverride(avSize);
			Rect rect = new Rect(new Point(), CorrectSize(avSize));
			Size res = CorrectSize(ArrangeOverrideCore(rect));
			res = CorrectSizeForControlBox(res, false, true);
			AfterArrangeOverride(res);
			return res;
		}
		protected virtual void BeforeMeasureOverride() { }
		protected virtual void AfterArrangeOverride(Size actualSize) {
			UpdateControlBoxPosition(actualSize);
		}
		protected virtual Size MeasureOverrideCore(Size avSize) { return avSize; }
		protected virtual Size ArrangeOverrideCore(Rect avRect) { return avRect.Size; }
		protected virtual void UpdateVisibleChildren() {
			VisibleChildren = FilterChildren(x => x.Visibility != Visibility.Collapsed).OrderBy(GetVisibleIndex).ToList();
		}
		protected virtual int GetVisibleIndex(FrameworkElement child) { return -1; }
		IEnumerable<FrameworkElement> FilterChildren(Func<FrameworkElement, bool> filter) {
			var res = new List<FrameworkElement>();
			foreach(FrameworkElement child in Children)
				if(child.Return(x => filter(x), () => false))
					res.Add(child);
			return res;
		}
		protected virtual Size CorrectSizeForControlBox(Size avSize, bool decreaseSize, bool increaseSize) {
			if(ControlBox == null) return avSize;
			Size res = avSize;
			if(Orientation == Orientation.Horizontal) {
				if(decreaseSize) res.Width = Math.Max(0, res.Width - ControlBox.DesiredSize.Width);
				if(increaseSize) res.Width = Math.Max(0, res.Width + ControlBox.DesiredSize.Width);
				return res;
			} else {
				if(decreaseSize) res.Height = Math.Max(0, res.Height - ControlBox.DesiredSize.Height);
				if(increaseSize) res.Height = Math.Max(0, res.Height + ControlBox.DesiredSize.Height);
				return res;
			}
		}
		protected virtual void UpdateControlBoxPosition(Size actualSize) {
			TranslateTransform transform = ControlBox.RenderTransform as TranslateTransform;
			if(transform == null) ControlBox.RenderTransform = transform = new TranslateTransform();
			transform.X = 0;
			transform.Y = 0;
			double realWidth = GetSumChildActualWidth(VisibleChildren);
			if(Orientation == Orientation.Horizontal)
				transform.X = realWidth;
			else transform.Y = realWidth;
		}
		protected internal FrameworkElement GetActiveChild() {
			foreach(FrameworkElement child in VisibleChildren) {
				if((bool)child.GetValue(Selector.IsSelectedProperty))
					return child;
			}
			return null;
		}
		protected void MeasureChild(FrameworkElement child, Size size) {
			child.Measure(CorrectSize(size));
		}
		protected void ArrangeChild(FrameworkElement child, Rect rect) {
			child.Arrange(CorrectRect(rect));
		}
		protected void SetMinHeight(FrameworkElement child, double height) {
			SetChildMinHeight(child, height);
		}
		protected Size GetSize(FrameworkElement child) {
			return CorrectSize(new Size(child.Width, child.Height));
		}
		protected Size GetMinSize(FrameworkElement child) {
			return CorrectSize(new Size(child.MinWidth, child.MinHeight));
		}
		protected Size GetMaxSize(FrameworkElement child) {
			return CorrectSize(new Size(child.MaxWidth, child.MaxHeight));
		}
		protected Size GetActualSize(FrameworkElement child) {
			return CorrectSize(new Size(child.ActualWidth, child.ActualHeight));
		}
		protected Size GetDesiredSize(FrameworkElement child) {
			return CorrectSize(child.DesiredSize);
		}
		protected double GetRenderWidth(FrameworkElement child) {
			return OrientationHelper.GetDefineSize(child.RenderSize);
		}
		Size CorrectSize(Size size) {
			return Orientation == Orientation.Horizontal ? size : new Size(size.Height, size.Width);
		}
		Rect CorrectRect(Rect rect) {
			return Orientation == Orientation.Horizontal ? rect : new Rect(rect.Y, rect.X, rect.Height, rect.Width);
		}
		protected double GetStretchCoef(double avWidth, IEnumerable<FrameworkElement> childs, FrameworkElement child) {
			double allWidth = GetSumChildDesireWidth(childs);
			if(allWidth >= avWidth) return 0d;
			double coef = avWidth - allWidth;
			int childsCount = childs.Count();
			double commonCoef = Math.Floor(coef / childsCount);
			coef = (coef - commonCoef * childsCount);
			int childIndex = childs.ToList().IndexOf(child) + 1;
			if(coef >= childIndex)
				return commonCoef + 1;
			return commonCoef;
		}
		protected double GetSumChildDesireWidth(IEnumerable<FrameworkElement> childs) {
			if(childs == null) return 0d;
			double res = 0d;
			foreach(FrameworkElement child in childs)
				res += GetDesiredSize(child).Width;
			return res;
		}
		protected double GetSumChildActualWidth(IEnumerable<FrameworkElement> childs) {
			if(childs == null) return 0d;
			double res = 0d;
			foreach(FrameworkElement child in childs)
				res += GetActualSize(child).Width;
			return res;
		}
		protected void ApplyOpacityMask(FrameworkElement child, double offset, double childWidth, double avWidth, double transparencySize) {
			child.Opacity = 1;
			child.OpacityMask = null;
			child.IsHitTestVisible = true;
			double right = offset + childWidth;
			if(offset >= -1 && right <= avWidth + 1) return;
			if(right <= 0 || offset >= avWidth) {
				child.Opacity = 0;
				child.IsHitTestVisible = false;
				return;
			}
			LinearGradientBrush opacityBrush = new LinearGradientBrush() { StartPoint = OrientationHelper.CreatePoint(0, 0.5), EndPoint = OrientationHelper.CreatePoint(1, 0.5) };
			if(offset < 0) {
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, -offset / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, (-offset + transparencySize) / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, 1));
			} else {
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, 0));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, (avWidth - offset - transparencySize) / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, (avWidth - offset) / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
			}
			child.OpacityMask = opacityBrush;
		}
		protected void UpdateOpacityMasks(IEnumerable<FrameworkElement> childs, Rect viewBox, double transparencySize) {
			double offset = viewBox.X;
			foreach(FrameworkElement child in childs) {
				double childWidth = GetActualSize(child).Width;
				ApplyOpacityMask(child, offset, childWidth, viewBox.Width, transparencySize);
				offset += childWidth;
			}
		}
	}
	public abstract class TabPanelMultiLineViewBase : TabPanelBase {
		public static readonly DependencyProperty AutoMoveActiveRowProperty = DependencyProperty.Register("AutoMoveActiveRow", typeof(bool), typeof(TabPanelMultiLineViewBase),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty AutoReverseItemsProperty = DependencyProperty.Register("AutoReverseItems", typeof(bool), typeof(TabPanelMultiLineViewBase),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty IsStretchedHorizontallyProperty = DependencyProperty.Register("IsStretchedHorizontally", typeof(bool), typeof(TabPanelMultiLineViewBase),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty IsStretchedVerticallyProperty = DependencyProperty.Register("IsStretchedVertically", typeof(bool), typeof(TabPanelMultiLineViewBase),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public bool AutoMoveActiveRow { get { return (bool)GetValue(AutoMoveActiveRowProperty); } set { SetValue(AutoMoveActiveRowProperty, value); } }
		public bool AutoReverseItems { get { return (bool)GetValue(AutoReverseItemsProperty); } set { SetValue(AutoReverseItemsProperty, value); } }
		public bool IsStretchedHorizontally { get { return (bool)GetValue(IsStretchedHorizontallyProperty); } set { SetValue(IsStretchedHorizontallyProperty, value); } }
		public bool IsStretchedVertically { get { return (bool)GetValue(IsStretchedVerticallyProperty); } set { SetValue(IsStretchedVerticallyProperty, value); } }
		RowManager rows;
		protected int RowCount { get; private set; }
		protected override void BeforeMeasureOverride() {
			rows = null;
			base.BeforeMeasureOverride();
		}
		protected override Size MeasureOverrideCore(Size avSize) {
			foreach(FrameworkElement child in Children)
				MeasureChild(child, new Size(double.PositiveInfinity, double.PositiveInfinity));
			RowCount = GetRowCount(avSize.Width);
			rows = Split(avSize.Width, RowCount);
			if(AutoReverseItems && (HeaderLocation == HeaderLocation.Top || HeaderLocation == HeaderLocation.Left))
				rows.ReverseRows();
			foreach(Row row in rows.Rows)
				foreach(FrameworkElement child in row.Children)
					SetMinHeight(child, row.Height);
			return new Size(Math.Min(rows.MaxWidth, avSize.Width), rows.Height);
		}
		protected override Size ArrangeOverrideCore(Rect avRect) {
			double yOffset = 0d;
			var activeChild = GetActiveChild();
			var activeRow = GetActiveRow();
			if(AutoMoveActiveRow)
				rows.MoveToLastPosition(activeRow);
			bool inverseArrange = HeaderLocation == HeaderLocation.Bottom || HeaderLocation == HeaderLocation.Right;
			int zIndex = 0;
			foreach(Row row in rows.Rows) {
				double xOffset = 0d;
				foreach(FrameworkElement child in row.Children) {
					Panel.SetZIndex(child, zIndex);
					double childWidth;
					double childHeight;
					if(!IsStretchedHorizontally && rows.Count == 1)
						childWidth = GetDesiredSize(child).Width;
					else childWidth = GetDesiredSize(child).Width + GetStretchCoef(avRect.Width, row.Children, child);
					if(!IsStretchedVertically) childHeight = row.Height;
					else childHeight = avRect.Size.Height - yOffset;
					ArrangeChild(child, new Rect(xOffset, inverseArrange && IsStretchedVertically ? 0d : yOffset, childWidth, childHeight));
					xOffset += childWidth;
				}
				yOffset += row.Height;
				zIndex++;
				if(row == activeRow) {
					activeChild.Do(x => Panel.SetZIndex(x, zIndex));
					zIndex++;
				}
			}
			return avRect.Size;
		}
		protected override void UpdateControlBoxPosition(Size actualSize) {
			TranslateTransform transform = ControlBox.RenderTransform as TranslateTransform;
			if(transform == null) ControlBox.RenderTransform = transform = new TranslateTransform();
			transform.X = 0;
			transform.Y = 0;
			double realWidth = RowCount != 0 && rows != null ? rows.Rows.Max(x => GetSumChildActualWidth(x.Children)) : GetSumChildActualWidth(VisibleChildren);
			if(Orientation == Orientation.Horizontal)
				transform.X = realWidth;
			else transform.Y = realWidth;
		}
		class Row {
			readonly RowManager Owner;
			public readonly IEnumerable<FrameworkElement> Children;
			public FrameworkElement this[int index] { get { return Children.ToList()[index]; } }
			public int Count { get { return Children.Count(); } }
			public double Width { get; protected set; }
			public double Height { get { return Children.Max(x => Owner.Panel.GetDesiredSize(x).Height); } }
			public double Coef { get { return Math.Max(0d, (Owner.AvSize - Width) / Count); } }
			public Row(RowManager owner) {
				Owner = owner;
				Children = new List<FrameworkElement>();
				Width = 0d;
			}
			public void Add(FrameworkElement child, int? position = null) {
				if(position == null)
					(Children as List<FrameworkElement>).Add(child);
				else (Children as List<FrameworkElement>).Insert(position.Value, child);
				Width += Owner.Panel.GetDesiredSize(child).Width;
			}
			public FrameworkElement Remove() {
				var res = (Children as List<FrameworkElement>).Last();
				(Children as List<FrameworkElement>).Remove(res);
				Width -= Owner.Panel.GetDesiredSize(res).Width;
				return res;
			}
		}
		class RowManager {
			public readonly TabPanelMultiLineViewBase Panel;
			public readonly double AvSize;
			public IEnumerable<Row> Rows { get; private set; }
			public Row this[int index] { get { return Rows.ToList()[index]; } }
			public int Count { get { return Rows.Count(); } }
			public Row Last { get { return Rows.Last(); } }
			public int RowWithMaxCoef {
				get {
					var row = Rows.OrderBy(x => x.Coef).Last();
					return Rows.ToList().IndexOf(row);
				}
			}
			public double MaxCoef { get { return Rows.Max(x => x.Coef); } }
			public double MaxWidth { get { return Rows.Max(x => x.Width); } }
			public double Height { get { return Rows.Sum(x => x.Height); } }
			public RowManager(TabPanelMultiLineViewBase panel, double avSize) {
				Panel = panel;
				AvSize = avSize;
				Rows = new List<Row>();
				AddNewRow();
			}
			public void MoveToLastPosition(Row row) {
				if(row == null || row == Rows.Last()) return;
				(Rows as List<Row>).Remove(row);
				(Rows as List<Row>).Add(row);
			}
			public void ReverseRows() {
				Rows = Rows.Reverse().ToList();
			}
			public void AddNewRow() {
				((List<Row>)Rows).Add(new Row(this));
			}
			public bool CanMoveChildToRow(int row) {
				var prevRow = this[row - 1];
				var curRow = this[row];
				var child = prevRow.Children.Last();
				return Panel.GetDesiredSize(child).Width + curRow.Width < AvSize;
			}
			public void MoveChildToRow(int row) {
				var prevRow = this[row - 1];
				var curRow = this[row];
				var child = prevRow.Remove();
				curRow.Add(child, 0);
			}
			public RowManager Clone() {
				RowManager res = new RowManager(Panel, AvSize);
				((List<Row>)res.Rows).Clear();
				foreach(Row row in Rows) {
					res.AddNewRow();
					foreach(FrameworkElement child in row.Children)
						res.Last.Add(child);
				}
				return res;
			}
		}
		Row GetActiveRow() {
			var activeChild = GetActiveChild();
			foreach(Row row in rows.Rows) {
				foreach(FrameworkElement child in row.Children)
					if(child == activeChild) return row;
			}
			return null;
		}
		RowManager Split(double avSize, int rowCount) {
			RowManager currentSolution = new RowManager(this, avSize);
			RowManager bestSolution;
			foreach(FrameworkElement child in VisibleChildren) {
				if(currentSolution.Last.Width + GetDesiredSize(child).Width > avSize && currentSolution.Last.Count > 0)
					currentSolution.AddNewRow();
				currentSolution.Last.Add(child);
			}
			if(currentSolution.Count == 1) return currentSolution;
			bestSolution = currentSolution.Clone();
			while(true) {
				if(currentSolution.RowWithMaxCoef == 0 || !currentSolution.CanMoveChildToRow(currentSolution.RowWithMaxCoef))
					break;
				currentSolution.MoveChildToRow(currentSolution.RowWithMaxCoef);
				if(currentSolution.MaxCoef < bestSolution.MaxCoef)
					bestSolution = currentSolution.Clone();
			}
			return bestSolution;
		}
		int GetRowCount(double avSize) {
			int rows = 0;
			double currentRowWidth = 0d;
			int currentRowCount = 0;
			foreach(FrameworkElement child in VisibleChildren) {
				double childWidth = GetDesiredSize(child).Width;
				if(currentRowWidth + childWidth > avSize && currentRowCount > 0) {
					currentRowWidth = childWidth;
					currentRowCount = 1;
					rows++;
				} else {
					currentRowWidth += childWidth;
					currentRowCount++;
				}
			}
			return rows;
		}
	}
	public class TabPanelMultiLineView : TabPanelMultiLineViewBase {
		public static readonly DependencyProperty IsHoldModeProperty = DependencyProperty.Register("IsHoldMode", typeof(bool), typeof(TabPanelMultiLineView), 
			new PropertyMetadata(false, (d, e) => ((TabPanelMultiLineView)d).OnIsHoldModeChanged()));
		public static readonly DependencyProperty AutoEnableHoldModeWhenManyRowsProperty = DependencyProperty.Register("AutoEnableHoldModeWhenManyRows", typeof(bool), typeof(TabPanelMultiLineView), new PropertyMetadata(false));
		public bool IsHoldMode { get { return (bool)GetValue(IsHoldModeProperty); } set { SetValue(IsHoldModeProperty, value); } }
		public bool AutoEnableHoldModeWhenManyRows { get { return (bool)GetValue(AutoEnableHoldModeWhenManyRowsProperty); } set { SetValue(AutoEnableHoldModeWhenManyRowsProperty, value); } }
		void OnIsHoldModeChanged() {
			if(IsHoldMode) {
				AutoMoveActiveRow = false;
				AutoReverseItems = false;
				IsStretchedVertically = false;
			} else {
				AutoMoveActiveRow = true;
				AutoReverseItems = true;
				IsStretchedVertically = true;
			}
		}
		protected override Size MeasureOverrideCore(Size avSize) {
			var res = base.MeasureOverrideCore(avSize);
			if(AutoEnableHoldModeWhenManyRows)
				IsHoldMode = RowCount > 0;
			return res;
		}
	}
	public abstract class TabPanelScrollViewBase : TabPanelBase {
		public static readonly DependencyProperty IsStretchedHorizontallyProperty = DependencyProperty.Register("IsStretchedHorizontally", typeof(bool), typeof(TabPanelScrollViewBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
		public bool IsStretchedHorizontally { get { return (bool)GetValue(IsStretchedHorizontallyProperty); } set { SetValue(IsStretchedHorizontallyProperty, value); } }
		protected double FullWidth { get; private set; }
		protected Size VisibleSize { get; private set; }
		protected override Size MeasureOverrideCore(Size avSize) {
			double resWidth = 0, resHeight = 0;
			foreach(FrameworkElement child in VisibleChildren) {
				MeasureChild(child, new Size(double.PositiveInfinity, avSize.Height));
				resWidth += GetDesiredSize(child).Width;
				resHeight = Math.Max(resHeight, GetDesiredSize(child).Height);
			}
			FullWidth = resWidth;
			foreach(FrameworkElement child in VisibleChildren)
				SetMinHeight(child, resHeight);
			if(!double.IsInfinity(avSize.Width))
				resWidth = IsStretchedHorizontally ? avSize.Width : Math.Min(avSize.Width, resWidth);
			return new Size(resWidth, resHeight);
		}
		protected override Size ArrangeOverrideCore(Rect rect) {
			double offset = rect.X;
			foreach(FrameworkElement child in VisibleChildren) {
				Panel.SetZIndex(child, 0);
				double childWidth = GetDesiredSize(child).Width;
				if(IsStretchedHorizontally)
					childWidth += GetStretchCoef(rect.Size.Width, VisibleChildren, child);
				ArrangeChild(child, new Rect(new Point(offset, rect.Y), new Size(childWidth, rect.Height)));
				offset += childWidth;
			}
			GetActiveChild().Do(x => Panel.SetZIndex(x, 99));
			return rect.Size;
		}
		protected override void AfterArrangeOverride(Size actualSize) {
			Size sz = CorrectSizeForControlBox(actualSize, true, false);
			VisibleSize = Orientation == Orientation.Horizontal ? sz : new Size(sz.Height, sz.Width);
			base.AfterArrangeOverride(actualSize);
		}
		protected override void UpdateControlBoxPosition(Size actualSize) {
			TranslateTransform transform = ControlBox.RenderTransform as TranslateTransform;
			if(transform == null) ControlBox.RenderTransform = transform = new TranslateTransform();
			transform.X = 0;
			transform.Y = 0;
			double realWidth = !IsStretchedHorizontally ? Math.Min(FullWidth, VisibleSize.Width) : VisibleSize.Width;
			if(Orientation == Orientation.Horizontal)
				transform.X = realWidth;
			else transform.Y = realWidth;
		}
	}
	public class TabPanelScrollView : TabPanelScrollViewBase {
		public static readonly DependencyProperty AllowAnimationProperty = DependencyProperty.Register("AllowAnimation", typeof(bool), typeof(TabPanelScrollView), new PropertyMetadata(true));
		public static readonly DependencyProperty TransparencySizeProperty = DependencyProperty.Register("TransparencySize", typeof(int), typeof(TabPanelScrollView), new PropertyMetadata(15));
		public bool AllowAnimation { get { return (bool)GetValue(AllowAnimationProperty); } set { SetValue(AllowAnimationProperty, value); } }
		public int TransparencySize { get { return (int)GetValue(TransparencySizeProperty); } set { SetValue(TransparencySizeProperty, value); } }
		DoubleAnimation scrollAnimation;
		public DoubleAnimation ScrollAnimation {
			get { return scrollAnimation; }
			set {
				if(scrollAnimation == value) return;
				scrollAnimation = value;
				UpdateAnimation();
			}
		}
		TranslateTransform Transform;
		Storyboard Storyboard;
		public TabPanelScrollView() {
			Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateAnimation();
		}
		protected override void OnHeaderLocationChanged() {
			base.OnHeaderLocationChanged();
			UpdateAnimation();
		}
		void UpdateAnimation() {
			Transform.Do(x => x.Changed -= OnTransformUpdated);
			RenderTransform = null;
			if(ScrollAnimation == null) return;
			Storyboard = new Storyboard();
			RenderTransform = Transform = new TranslateTransform();
			Transform.Changed += OnTransformUpdated;
			Storyboard.SetTarget(ScrollAnimation, this);
			Storyboard.SetTargetProperty(ScrollAnimation, new PropertyPath(Orientation == Orientation.Horizontal ? "RenderTransform.X" : "RenderTransform.Y"));
			Storyboard.Children.Add(ScrollAnimation);
		}
		protected override void AfterArrangeOverride(Size actualSize) {
			base.AfterArrangeOverride(actualSize);
			OnTransformUpdated(Transform, EventArgs.Empty);
		}
		void OnTransformUpdated(object sender, EventArgs e) {
			if(Transform == null) return;
			Rect viewBox = new Rect();
			viewBox.Size = VisibleSize;
			viewBox.X = Orientation == Orientation.Horizontal ? Transform.X : Transform.Y;
			UpdateOpacityMasks(VisibleChildren, viewBox, TransparencySize);
		}
		public bool CanScroll { get { return MaxOffset > 0; } }
		public bool CanScrollPrev { get { return CanScroll && Offset > MinOffset; } }
		public bool CanScrollNext { get { return CanScroll && Offset < MaxOffset; } }
		internal double VisibleLength { get { return VisibleSize.Width; } }
		internal double PanelLength { get { return FullWidth; } }
		internal double MinOffset { get { return 0; } }
		internal double MaxOffset { get { return Math.Floor(PanelLength - VisibleLength); } }
		double offset = 0d;
		internal double Offset {
			get { return offset; }
			set {
				if(Math.Abs(offset - value) < 0.01) return;
				var oldValue = offset;
				offset = value;
				OnOffsetChanged(oldValue);
			}
		}
		internal double RightOffset {
			get { return Offset + VisibleLength; }
			set { Offset = value - VisibleLength; }
		}
		internal bool disableAnimation = false;
		void OnOffsetChanged(double oldValue) {
			ApplyAnimation(Offset);
		}
		void ApplyAnimation(double value) {
			ScrollAnimation.To = -value;
			if(!AllowAnimation || disableAnimation) {
				Storyboard.Begin(this, true);
				Storyboard.SkipToFill(this);
			} else Storyboard.Begin(this);
		}
		public bool CanScrollTo(FrameworkElement child) {
			if(!CanScroll || !VisibleChildren.Contains(child)) return false;
			return CanScroll && (Offset > GetOffset(child) ||
				RightOffset <= GetRightOffset(child));
		}
		public void ScrollTo(FrameworkElement child, bool allowAnimation = true) {
			try {
				disableAnimation = !allowAnimation;
				if(Offset < MinOffset) {
					ScrollToBegin(allowAnimation);
					return;
				}
				if(Offset > MaxOffset) {
					ScrollToEnd(allowAnimation);
					return;
				}
				if(!CanScrollTo(child)) return;
				double headerOffset = GetOffset(child);
				bool canScrollPrevValue = CanScrollPrev;
				double visibleLengthValue = VisibleLength;
				if(Offset >= headerOffset) Offset = headerOffset;
				else RightOffset = headerOffset + GetRenderWidth(child);
			} finally {
				disableAnimation = false;
			}
		}
		public void ScrollToBegin(bool allowAnimation = true) {
			try {
				disableAnimation = !allowAnimation;
				Offset = MinOffset;
			} finally {
				disableAnimation = false;
			}
		}
		public void ScrollToEnd(bool allowAnimation = true) {
			try {
				disableAnimation = !allowAnimation;
				if(CanScroll) Offset = MaxOffset;
				else ScrollToBegin(allowAnimation);
			} finally {
				disableAnimation = false;
			}
		}
		public void ScrollPrev(bool allowAnimation = true) {
			if(!CanScrollPrev) return;
			var firstFullVisibleItem = GetFirstFullVisibleItem();
			int index = VisibleChildren.ToList().IndexOf(firstFullVisibleItem);
			index -= 1;
			if(index >= 0)
				ScrollTo(VisibleChildren.ToList()[index], allowAnimation);
			else ScrollToBegin(allowAnimation);
		}
		public void ScrollNext(bool allowAnimation = true) {
			if(!CanScrollNext) return;
			var lastFullVisibleItem = GetLastFullVisibleItem();
			int index = VisibleChildren.ToList().IndexOf(lastFullVisibleItem);
			if(index >= 0 && index < VisibleChildren.Count())
				ScrollTo(lastFullVisibleItem, allowAnimation);
			else ScrollToEnd(allowAnimation);
		}
		internal double GetOffset(FrameworkElement child) {
			if(!VisibleChildren.Contains(child)) return 0d;
			double res = 0;
			foreach(FrameworkElement el in VisibleChildren) {
				if(el == child) return res;
				res += GetRenderWidth(el);
			}
			return res;
		}
		internal double GetRightOffset(FrameworkElement child) {
			return GetOffset(child) + GetRenderWidth(child);
		}
		internal FrameworkElement GetFirstFullVisibleItem() {
			double offset = 0d;
			foreach(FrameworkElement child in VisibleChildren) {
				if(offset >= Offset) return child;
				offset += GetRenderWidth(child);
			}
			return null;
		}
		internal FrameworkElement GetLastFullVisibleItem() {
			double offset = 0d;
			foreach(FrameworkElement child in VisibleChildren) {
				offset += GetRenderWidth(child);
				if(offset > RightOffset) return child;
			}
			return null;
		}
		internal double GetHeaderOffset(int index) {
			return GetOffset(VisibleChildren.ElementAt(index));
		}
		internal int GetFirstFullVisibleItemIndex() {
			return VisibleChildren.ToList().IndexOf(GetFirstFullVisibleItem());
		}
		internal bool CanScrollTo(int index) {
			return CanScrollTo(VisibleChildren.ElementAt(index));
		}
		internal void ScrollTo(int index) {
			ScrollTo(VisibleChildren.ElementAt(index));
		}
	}
	public class TabPanelStretchViewBase : TabPanelBase {
		public static readonly DependencyProperty NormalChildSizeProperty = DependencyProperty.Register("NormalChildSize", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(140, (d,e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty NormalChildMinSizeProperty = DependencyProperty.Register("NormalChildMinSize", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(30, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty ActiveChildMinSizeProperty = DependencyProperty.Register("ActiveChildMinSize", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(50, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty PinChildLeftSizeProperty = DependencyProperty.Register("PinChildLeftSize", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(0, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty PinChildRightSizeProperty = DependencyProperty.Register("PinChildRightSize", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(0, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty PinPanelLeftIndentProperty = DependencyProperty.Register("PinPanelLeftIndent", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(8, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty PinPanelRightIndentProperty = DependencyProperty.Register("PinPanelRightIndent", typeof(int), typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(8, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		public static readonly DependencyProperty TransparencySizeProperty = DependencyProperty.Register("TransparencySize", typeof(int), typeof(TabPanelStretchViewBase), new PropertyMetadata(15));
		public int NormalChildSize { get { return (int)GetValue(NormalChildSizeProperty); } set { SetValue(NormalChildSizeProperty, value); } }
		public int NormalChildMinSize { get { return (int)GetValue(NormalChildMinSizeProperty); } set { SetValue(NormalChildMinSizeProperty, value); } }
		public int ActiveChildMinSize { get { return (int)GetValue(ActiveChildMinSizeProperty); } set { SetValue(ActiveChildMinSizeProperty, value); } }
		public int PinChildLeftSize { get { return (int)GetValue(PinChildLeftSizeProperty); } set { SetValue(PinChildLeftSizeProperty, value); } }
		public int PinChildRightSize { get { return (int)GetValue(PinChildRightSizeProperty); } set { SetValue(PinChildRightSizeProperty, value); } }
		public int PinPanelLeftIndent { get { return (int)GetValue(PinPanelLeftIndentProperty); } set { SetValue(PinPanelLeftIndentProperty, value); } }
		public int PinPanelRightIndent { get { return (int)GetValue(PinPanelRightIndentProperty); } set { SetValue(PinPanelRightIndentProperty, value); } }
		public int TransparencySize { get { return (int)GetValue(TransparencySizeProperty); } set { SetValue(TransparencySizeProperty, value); } }
		static TabPanelStretchViewBase() {
			SelectedIndexProperty.OverrideMetadata(typeof(TabPanelStretchViewBase), new FrameworkPropertyMetadata(-1, (d, e) => ((TabPanelStretchViewBase)d).ForceUpdateLayout()));
		}
		public TabPanelStretchViewBase() {
			PinnedLeftChildren = new List<FrameworkElement>();
			PinnedRightChildren = new List<FrameworkElement>();
			PinnedNoneChildren = new List<FrameworkElement>();
		}
		public void ForceUpdateLayout() {
			InvalidateMeasure();
			InvalidateArrange();
		}
		protected IEnumerable<FrameworkElement> PinnedLeftChildren { get; private set; }
		protected IEnumerable<FrameworkElement> PinnedRightChildren { get; private set; }
		protected IEnumerable<FrameworkElement> PinnedNoneChildren { get; private set; }
		protected virtual TabPinMode GetPinMode(FrameworkElement child) {
			return TabPinMode.None;
		}
		protected override void UpdateVisibleChildren() {
			base.UpdateVisibleChildren();
			PinnedLeftChildren = VisibleChildren.Where(x => GetPinMode(x) == TabPinMode.Left);
			PinnedRightChildren = VisibleChildren.Where(x => GetPinMode(x) == TabPinMode.Right).Reverse() ;
			PinnedNoneChildren = VisibleChildren.Where(x => GetPinMode(x) == TabPinMode.None);
		}
		double GetPinPanelLeftSize(bool useActualSizeInstedOfDesired = false) {
			double res;
			if(PinChildLeftSize == 0)
				res = useActualSizeInstedOfDesired ? GetSumChildActualWidth(PinnedLeftChildren) : GetSumChildDesireWidth(PinnedLeftChildren);
			else res = PinChildLeftSize * PinnedLeftChildren.Count();
			if(!res.IsZero()) res += PinPanelLeftIndent;
			return res;
		}
		double GetPinPanelRightSize(bool useActualSizeInstedOfDesired = false) {
			double res;
			if(PinChildRightSize == 0)
				res = useActualSizeInstedOfDesired ? GetSumChildActualWidth(PinnedRightChildren) : GetSumChildDesireWidth(PinnedRightChildren);
			else res = PinChildRightSize * PinnedRightChildren.Count();
			if(!res.IsZero()) res += PinPanelRightIndent;
			return res;
		}
		double GetChildWidth(FrameworkElement child, double avWidth) {
			if(PinnedNoneChildren.Count() == 1) return Math.Min(avWidth, NormalChildSize);
			double width;
			int childCount = PinnedNoneChildren.Count();
			width = double.IsInfinity(avWidth) ? NormalChildSize : avWidth / childCount;
			width = Math.Min(NormalChildSize, width);
			var activeChild = GetActiveChild();
			if(!PinnedNoneChildren.Contains(activeChild)) activeChild = null;
			if(child == activeChild && width < ActiveChildMinSize)
				return Math.Min(avWidth, ActiveChildMinSize);
			if(activeChild != null && width < ActiveChildMinSize) {
				childCount--;
				width = (avWidth - ActiveChildMinSize) / childCount;
				width = Math.Max(0d, width);
			}
			double coef = (width - Math.Floor(width)) * childCount;
			width = Math.Floor(width);
			int childIndex = PinnedNoneChildren.ToList().IndexOf(child) + 1;
			if(coef >= childIndex)
				width++;
			return Math.Max(NormalChildMinSize, width);
		}
		protected override Size MeasureOverrideCore(Size avSize) {
			double resWidth = 0d, resHeight = 0d;
			double avWidth = avSize.Width;
			foreach(var child in PinnedRightChildren) {
				double width = PinChildRightSize == 0 ? double.PositiveInfinity : PinChildRightSize;
				MeasureChild(child, new Size(width, avSize.Height));
				resHeight = Math.Max(resHeight, GetDesiredSize(child).Height);
			}
			avWidth = Math.Max(0, avWidth - GetPinPanelRightSize());
			foreach(var child in PinnedLeftChildren) {
				double width = PinChildLeftSize == 0 ? double.PositiveInfinity : PinChildLeftSize;
				MeasureChild(child, new Size(width, avSize.Height));
				resHeight = Math.Max(resHeight, GetDesiredSize(child).Height);
			}
			avWidth = Math.Max(0, avWidth - GetPinPanelLeftSize());
			avWidth = Math.Max(0, avWidth - GetDesiredSize(ControlBox).Width);
			foreach(FrameworkElement child in PinnedNoneChildren) {
				double childWidth = GetChildWidth(child, avWidth);
				MeasureChild(child, new Size(childWidth, avSize.Height));
				resWidth += GetDesiredSize(child).Width;
				resHeight = Math.Max(resHeight, GetDesiredSize(child).Height);
			}
			resWidth += GetPinPanelRightSize() + GetPinPanelLeftSize();
			resWidth += GetDesiredSize(ControlBox).Width;
			resWidth = Math.Min(avSize.Width, resWidth);
			return new Size(resWidth, resHeight);
		}
		protected override Size ArrangeOverrideCore(Rect avRect) {
			double offset = avRect.X;
			foreach(FrameworkElement child in PinnedLeftChildren) {
				Panel.SetZIndex(child, 100);
				double childWidth = PinChildLeftSize != 0 ? PinChildLeftSize : GetDesiredSize(child).Width;
				ArrangeChild(child, new Rect(new Point(offset, avRect.Y), new Size(childWidth, avRect.Height)));
				offset += childWidth;
			}
			offset = avRect.Width;
			foreach(FrameworkElement child in PinnedRightChildren) {
				Panel.SetZIndex(child, 200);
				double childWidth = PinChildRightSize != 0 ? PinChildRightSize : GetDesiredSize(child).Width;
				offset -= childWidth;
				ArrangeChild(child, new Rect(new Point(offset, avRect.Y), new Size(childWidth, avRect.Height)));
			}
			offset = avRect.X + GetPinPanelLeftSize(true);
			double avWidth = Math.Max(0, avRect.Width - GetPinPanelLeftSize(true) - GetPinPanelRightSize(true) - GetDesiredSize(ControlBox).Width);
			foreach(FrameworkElement child in PinnedNoneChildren) {
				Panel.SetZIndex(child, 0);
				double childWidth = GetChildWidth(child, avWidth);
				ArrangeChild(child, new Rect(new Point(offset, avRect.Y), new Size(childWidth, avRect.Height)));
				offset += GetActualSize(child).Width;
			}
			var activeChild = GetActiveChild();
			if(PinnedNoneChildren.Contains(activeChild))
				activeChild.Do(x => Panel.SetZIndex(x, 99));
			else activeChild.Do(x => Panel.SetZIndex(x, 300));
			return avRect.Size;
		}
		protected override Size CorrectSizeForControlBox(Size avSize, bool decreaseSize, bool increaseSize) {
			return avSize;
		}
		protected override void AfterArrangeOverride(Size actualSize) {
			base.AfterArrangeOverride(actualSize);
			Size actualSz = OrientationHelper.CreateSize(actualSize.Width, actualSize.Height);
			double pinLeft = GetPinPanelLeftSize();
			double pinRight = GetPinPanelRightSize();
			double controlBox = OrientationHelper.GetDefineSize(GetDesiredSize(ControlBox));
			Rect pinnedNoneViewBox = new Rect(new Point(), new Size(
				Math.Max(0, actualSz.Width - pinLeft - pinRight - controlBox),
				actualSz.Height));
			UpdateOpacityMasks(PinnedNoneChildren, pinnedNoneViewBox, TransparencySize);
			Rect pinnedLeftViewBox = pinnedNoneViewBox.Width.IsZero() 
				? new Rect(new Point(), new Size(
					Math.Max(0, actualSz.Width - pinRight),
					actualSz.Height))
				: new Rect(new Point(), SizeHelper.Infinite);
			UpdateOpacityMasks(PinnedLeftChildren, pinnedLeftViewBox, TransparencySize);
			Rect pinnedRightViewBox = pinnedLeftViewBox.Width.IsZero()
				? new Rect(new Point(actualSz.Width - pinRight, 0), new Size(
					Math.Max(0, actualSz.Width),
					actualSz.Height))
				: new Rect(new Point(), SizeHelper.Infinite);
			UpdateOpacityMasks(PinnedRightChildren.Reverse(), pinnedRightViewBox, TransparencySize);
		}
		protected override void UpdateControlBoxPosition(Size actualSize) {
			TranslateTransform transform = ControlBox.RenderTransform as TranslateTransform;
			if(transform == null) ControlBox.RenderTransform = transform = new TranslateTransform();
			transform.X = 0; transform.Y = 0;
			double actualLength = OrientationHelper.GetDefineSize(actualSize);
			double controlBoxLength = GetDesiredSize(ControlBox).Width;
			double pinLeft = GetPinPanelLeftSize();
			double pinRight = GetPinPanelRightSize();
			double unPin = GetSumChildActualWidth(PinnedNoneChildren);
			double minPosition = pinLeft + unPin;
			double maxPosition = actualLength - pinRight - controlBoxLength;
			ControlBox.Visibility = pinLeft + pinRight + controlBoxLength > actualLength ? Visibility.Hidden : Visibility.Visible;
			double pos = Math.Min(minPosition, maxPosition);
			if(Orientation == Orientation.Horizontal) transform.X = pos;
			else transform.Y = pos;
		}
	}
	public class TabPanelStretchView : TabPanelStretchViewBase, IDragPanelVisual {
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(TabControlStretchView), typeof(TabPanelStretchView), new PropertyMetadata(null));
		public static readonly DependencyProperty DragDropModeProperty = DependencyProperty.Register("DragDropMode", typeof(TabControlDragDropMode), typeof(TabPanelStretchView), 
			new PropertyMetadata(TabControlDragDropMode.Full, (d,e) => ((TabPanelStretchView)d).OnDragDropModeChanged()));
		public TabControlStretchView Owner { get { return (TabControlStretchView)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public TabControlDragDropMode DragDropMode { get { return (TabControlDragDropMode)GetValue(DragDropModeProperty); } set { SetValue(DragDropModeProperty, value); } }
		PinnedLeftDragPanel pinnedLeftDragPanel;
		PinnedRightDragPanel pinnedRightDragPanel;
		PinnedNoneDragPanel pinnedNoneDragPanel;
		public TabPanelStretchView() {
			pinnedLeftDragPanel = new PinnedLeftDragPanel(this);
			pinnedRightDragPanel = new PinnedRightDragPanel(this);
			pinnedNoneDragPanel = new PinnedNoneDragPanel(this);
			OnDragDropModeChanged();
		}
		protected override int GetVisibleIndex(FrameworkElement child) {
			return ((DXTabItem)child).VisibleIndex;
		}
		protected override TabPinMode GetPinMode(FrameworkElement child) {
			return TabControlStretchView.GetPinMode(child as DXTabItem);
		}
		protected override void UpdateVisibleChildren() {
			base.UpdateVisibleChildren();
			pinnedLeftDragPanel.RaiseChildrenChanged();
			pinnedRightDragPanel.RaiseChildrenChanged();
			pinnedNoneDragPanel.RaiseChildrenChanged();
		}
		void OnDragDropModeChanged() {
			pinnedLeftDragPanel.Init(DragDropMode);
			pinnedRightDragPanel.Init(DragDropMode);
			pinnedNoneDragPanel.Init(DragDropMode);
		}
		IDragPanel IDragPanelVisual.GetDragPanel(IDragPanel sourceDragPanel) {
			if(sourceDragPanel is PinnedLeftDragPanel)
				return pinnedLeftDragPanel;
			else if(sourceDragPanel is PinnedRightDragPanel)
				return pinnedRightDragPanel;
			return pinnedNoneDragPanel;
		}
		class DragPanelBase : TabControlStretchViewDragPanelBase {
			public DragPanelBase(TabPanelStretchView panel) : base(panel) { }
			protected override IEnumerable<FrameworkElement> Children { get { return Panel.Children.OfType<FrameworkElement>(); } }
			protected override DragControllerBase Controller { get { return controller; } }
			DragPanelControllerBase<TabControlStretchViewDragPanelBase, TabPanelStretchView> controller;
			public void Init(TabControlDragDropMode mode) {
				if(mode == TabControlDragDropMode.None) {
					controller.Do(x => x.Uninitialize());
					controller = null;
					return;
				}
				if(mode == TabControlDragDropMode.ReorderOnly) {
					controller.Do(x => x.Uninitialize());
					controller = new TabControlStretchViewLocalDragPanelController();
					controller.Initialize(this);
					return;
				}
				if(mode == TabControlDragDropMode.ReorderAndMove || mode == TabControlDragDropMode.Full) {
					controller.Do(x => x.Uninitialize());
					controller = new TabControlStretchViewGlobalDragPanelController();
					controller.Initialize(this);
					return;
				}
				throw new NotImplementedException();
			}
			protected override int CorrectIndexWhenMove(FrameworkElement child, int index) {
				if(Children.Count() == 0) return index;
				return Panel.VisibleChildren.ToList().IndexOf(Children.ElementAt(index));
			}
		}
		class PinnedLeftDragPanel : DragPanelBase {
			public PinnedLeftDragPanel(TabPanelStretchView panel) : base(panel) { }
			protected override IEnumerable<FrameworkElement> Children { get { return Panel.PinnedLeftChildren; } }
		}
		class PinnedRightDragPanel : DragPanelBase {
			public PinnedRightDragPanel(TabPanelStretchView panel) : base(panel) { }
			protected override IEnumerable<FrameworkElement> Children { get { return Panel.PinnedRightChildren.Reverse(); } }
		}
		class PinnedNoneDragPanel : DragPanelBase {
			public PinnedNoneDragPanel(TabPanelStretchView panel) : base(panel) { }
			protected override IEnumerable<FrameworkElement> Children { get { return Panel.PinnedNoneChildren; } }
		}
	}
}
