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
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid.HitTest;
#if !SILVERLIGHT
using DevExpress.Xpf.Grid.Themes;
#endif
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using SLControl = System.Windows.Controls.Control;
using DevExpress.Xpf.Data;
#endif
namespace DevExpress.Xpf.Grid {
	public class RowIndicatorViewInfo : DependencyObject {
		public static readonly DependencyProperty AnimationProgressProperty;
		public static readonly DependencyProperty VisibleSizeProperty;
		static RowIndicatorViewInfo() {
			AnimationProgressProperty = DependencyPropertyManager.RegisterAttached("AnimationProgress", typeof(double), typeof(RowIndicatorViewInfo), new FrameworkPropertyMetadata(0d, OnPropertyChanged));
			VisibleSizeProperty = DependencyPropertyManager.RegisterAttached("VisibleSize", typeof(Size), typeof(RowIndicatorViewInfo), new FrameworkPropertyMetadata(Size.Empty, OnPropertyChanged));
		}
		public static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = sender as FrameworkElement;
			if(element == null)
				return;
			RowData rowData = element.DataContext as RowData;
			if(rowData == null)
				return;
			rowData.SetValue(e.Property, e.NewValue);
		}
		public static void SetAnimationProgress(DependencyObject d, double progress) {
			d.SetValue(AnimationProgressProperty, progress);
		}
		public static double GetAnimationProgress(DependencyObject d) {
			return (double)d.GetValue(AnimationProgressProperty);
		}
		public static void SetVisibleSize(DependencyObject d, double size) {
			d.SetValue(VisibleSizeProperty, size);
		}
		public static Size GetVisibleSize(DependencyObject d) {
			return (Size)d.GetValue(VisibleSizeProperty);
		}
	}
	public class RowIndicatorBase : SLControl {
		public RowIndicatorBase() {
			GridViewHitInfoBase.SetHitTestAcceptor(this, new RowIndicatorTableViewHitTestAcceptor());
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			Focus();
		}
	}
	public class RowIndicatorControl : RowIndicatorBase {
		public RowIndicatorControl() {
			DefaultStyleKey = typeof(RowIndicatorControl);
		}
	}
	public class DetailRowIndicatorControl : RowIndicatorControl {
		public DetailRowIndicatorControl() {
			GridViewHitInfoBase.SetHitTestAcceptor(this, null);
		}
	}
	public class RowOffsetBase : Control {
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty RowLevelProperty;
		public static readonly DependencyProperty NextRowLevelProperty;
		static RowOffsetBase() {
			OffsetProperty = DependencyPropertyManager.Register("Offset", typeof(double), typeof(RowOffsetBase));
			RowLevelProperty = DependencyPropertyManager.Register("RowLevel", typeof(int), typeof(RowOffsetBase), new FrameworkPropertyMetadata((d, e) => ((RowOffsetBase)d).UpdateContent(((RowOffsetBase)d).ActualHeight)));
			NextRowLevelProperty = DependencyPropertyManager.Register("NextRowLevel", typeof(int), typeof(RowOffsetBase), new FrameworkPropertyMetadata((d, e) => ((RowOffsetBase)d).OnNextRowLevelChanged()));
		}
		public RowOffsetBase() {
			OldHeight = double.MinValue;
		}
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public int RowLevel {
			get { return (int)GetValue(RowLevelProperty); }
			set { SetValue(RowLevelProperty, value); }
		}
		public int NextRowLevel {
			get { return (int)GetValue(NextRowLevelProperty); }
			set { SetValue(NextRowLevelProperty, value); }
		}
		protected Path OffsetPath { get; set; }
		double OldHeight { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OffsetPath = GetTemplateChild("PART_OffsetPath") as Path;
			Loaded += new RoutedEventHandler(RowOffsetBase_Loaded);
			this.SizeChanged += new SizeChangedEventHandler(RowOffsetBase_SizeChanged);
		}
		void RowOffsetBase_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(OldHeight != double.MinValue && OldHeight != ActualHeight) {
				DrawLines(ActualHeight);
			}
		}
		void RowOffsetBase_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			UpdateContent(ActualHeight);
		}
		protected RowData RowData { get { return DataContext as RowData; } }
		protected DataViewBase View { get { return RowData != null ? RowData.View : null; } }
		protected GeometryGroup Group { get; set; }
		protected virtual void OnNextRowLevelChanged() {
			DrawLines(ActualHeight);
		}
		protected void DrawLines(double height) {
			if(OffsetPath == null)
				return;
			OldHeight = height;
			Group = new GeometryGroup();
			DrawLinesCore(height);
			if(OffsetPath != null) {
				OffsetPath.Data = Group;
				OffsetPath.Height = height;
			}
		}
		protected virtual void DrawLinesCore(double height) { }
		protected virtual void ChangeWidth() {
			Width = Offset * RowLevel;
		}
		protected virtual void UpdateContent(double height) {
			ChangeWidth();
			DrawLines(height);
		}
		protected override Size MeasureOverride(Size constraint) {
			return new Size(0, 0);
		}
		protected Size BaseMeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
	}
	public class RowOffset : RowOffsetBase {
		public static readonly DependencyProperty AllowVerticalLinesProperty;
		public static readonly DependencyProperty AllowHorizontalLinesProperty;
		public static readonly DependencyProperty ShowRowBreakProperty;
		static RowOffset() {
			AllowVerticalLinesProperty = DependencyPropertyManager.Register("AllowVerticalLines", typeof(bool), typeof(RowOffset));
			AllowHorizontalLinesProperty = DependencyPropertyManager.Register("AllowHorizontalLines", typeof(bool), typeof(RowOffset), new FrameworkPropertyMetadata(true, (d, e) => ((RowOffset)d).UpdateContent(((RowOffset)d).ActualHeight)));
			ShowRowBreakProperty = DependencyProperty.Register("ShowRowBreak", typeof(bool), typeof(RowOffset), new PropertyMetadata(false, (d, e) => ((RowOffset)d).OnShowRowBreakChanged()));
		}
		protected bool ShowGroupSummaryFooter { get { return View != null ? View.ShowGroupSummaryFooter : false; } }
		public bool AllowVerticalLines {
			get { return (bool)GetValue(AllowVerticalLinesProperty); }
			set { SetValue(AllowVerticalLinesProperty, value); }
		}
		public bool AllowHorizontalLines {
			get { return (bool)GetValue(AllowHorizontalLinesProperty); }
			set { SetValue(AllowHorizontalLinesProperty, value); }
		}
		public bool ShowRowBreak {
			get { return (bool)GetValue(ShowRowBreakProperty); }
			set { SetValue(ShowRowBreakProperty, value); }
		}
		void OnShowRowBreakChanged() {
			DrawLines(ActualHeight);
		}
		protected override void DrawLinesCore(double height) {
			if(AllowHorizontalLines && !ShowRowBreak) {
				DrawHorizontalLines(height);
			}
			if(AllowVerticalLines) {
				DrawVerticalLines(height);
			}
		}
#if DEBUGTEST
		internal int horizontalLinesDrawingCount = 0;
		internal Path GetOffsetPath() {
			return OffsetPath;
		}
#endif
		protected virtual void DrawHorizontalLines(double height) {
			int horizontalLinesCount = RowLevel - NextRowLevel;
			if(ShowGroupSummaryFooter && horizontalLinesCount > 0)
				horizontalLinesCount = 1;
			int position = RowLevel;
#if DEBUGTEST
			horizontalLinesDrawingCount++;
#endif
			double lineY = height - 0.5d;
			while(horizontalLinesCount > 0) {
				Group.Children.Add(GetLine(new Point(position * Offset, lineY), new Point((position - 1) * Offset, lineY)));
				horizontalLinesCount--;
				position--;
			}
		}
		protected virtual void DrawVerticalLines(double height) {
			for(int i = 1; i <= RowLevel; i++) {
				Group.Children.Add(GetLine(new Point(i * Offset - 0.5, 0), new Point(i * Offset - 0.5, height)));
			}
		}
		LineGeometry GetLine(Point startPoint, Point endPoint) {
			LineGeometry line = new LineGeometry();
			line.StartPoint = startPoint;
			line.EndPoint = endPoint;
			return line;
		}
	}
	public class GroupRowOffset : RowOffset {
		public static readonly DependencyProperty IsExpandedProperty;
		static GroupRowOffset() {
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(GroupRowOffset), new FrameworkPropertyMetadata((d, e) => ((GroupRowOffset)d).DrawLines(((GroupRowOffset)d).ActualHeight)));
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		protected override void DrawHorizontalLines(double height) {
			if(!IsExpanded && !ShowGroupSummaryFooter)
				base.DrawHorizontalLines(height);
		}
	}
	public class RowOffsetPresenter : ContentControl {
		public RowOffsetPresenter() {
			DefaultStyleKey = typeof(RowOffsetPresenter);
		}
	}
	public class GroupRowOffsetPresenter : ContentControl {
		public GroupRowOffsetPresenter() {
			DefaultStyleKey = typeof(GroupRowOffsetPresenter);
		}
	}
}
