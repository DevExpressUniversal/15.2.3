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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using DevExpress.Xpf.Grid.TreeList;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid {
	public enum TreeListIndentType { None, Line, Root, First, Last, Middle }
	public class TreeListNodeExpandButton : GridExpandButtonBase {
		public bool IsExpandButtonVisible {
			get { return (bool)GetValue(IsExpandButtonVisibleProperty); }
			set { SetValue(IsExpandButtonVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsExpandButtonVisibleProperty = DependencyPropertyManager.Register("IsExpandButtonVisible", typeof(bool), typeof(TreeListNodeExpandButton), new PropertyMetadata(true, (d, e) => ((TreeListNodeExpandButton)d).OnIsVisibleChanged()));
		void OnIsVisibleChanged() {
		}
		public TreeListNodeExpandButton() {
			this.SetDefaultStyleKey(typeof(TreeListNodeExpandButton));
			GridViewHitInfoBase.SetHitTestAcceptor(this, new TreeListNodeExpandButtonHitTestAcceptor());
		}
	}
	public class RowMarginControl : RowOffsetBase, ISupportLoadingAnimation {
		public static readonly DependencyProperty IsReadyProperty;
		public static readonly DependencyProperty TreeLineStyleProperty;
		public static readonly DependencyProperty ShowVerticalLinesProperty;
		static RowMarginControl() {
			IsReadyProperty = DependencyPropertyManager.Register("IsReady", typeof(bool), typeof(RowMarginControl), new PropertyMetadata(true, (d, e) => ((RowMarginControl)d).OnIsReadyChanged()));
			TreeLineStyleProperty = DependencyPropertyManager.Register("TreeLineStyle", typeof(TreeListLineStyle), typeof(RowMarginControl), new PropertyMetadata(TreeListLineStyle.Solid, (d, e) => ((RowMarginControl)d).UpdateContent(((RowMarginControl)d).ActualHeight)));
			ShowVerticalLinesProperty = DependencyPropertyManager.Register("ShowVerticalLines", typeof(bool), typeof(RowMarginControl), new PropertyMetadata(true, (d, e) => ((RowMarginControl)d).UpdateContent(((RowMarginControl)d).ActualHeight)));
		}
		public RowMarginControl() {
			this.SetDefaultStyleKey(typeof(RowMarginControl));
			SetBinding(IsReadyProperty, new Binding("IsReady"));
			GridViewHitInfoBase.SetHitTestAcceptor(this, new RowMarginControlHitTestAcceptor());
		}
		public TreeListLineStyle TreeLineStyle {
			get { return (TreeListLineStyle)GetValue(TreeLineStyleProperty); }
			set { SetValue(TreeLineStyleProperty, value); }
		}
		public bool ShowVerticalLines {
			get { return (bool)GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
		public bool IsReady {
			get { return (bool)GetValue(IsReadyProperty); }
			set { SetValue(IsReadyProperty, value); }
		}
		protected new TreeListRowData RowData { get { return base.RowData as TreeListRowData; } }
		protected new TreeListView View { get { return base.View as TreeListView; } }
		protected bool ShowTreeLines { get { return TreeLineStyle != TreeListLineStyle.None; } }
		protected new int RowLevel { get { return RowData.RowLevel; } }
		protected int ActualRowLevel { get { return RowLevel + View.ServiceIndentsCount; } }
		protected Path TreeLinePath { get; private set; }
		protected TreeListNodeExpandButton ExpandButton { get; private set; }
		protected CheckBox CheckBox { get; private set; }
		protected GeometryGroup TreeLineGeometry { get; private set; }
		protected Image Image { get; private set; }
		protected override void ChangeWidth() {
			if(View == null) return;
			Offset = View.RowIndent;
			Width = Offset * ActualRowLevel;
		}
		protected override void OnNextRowLevelChanged() {
			UpdateContent(ActualHeight);
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			e.Handled = CanProcessMouseDown();
			base.OnPreviewMouseDown(e);
		}
		bool CanProcessMouseDown() {
			if(View != null)
				return !View.RequestUIUpdate();
			return false;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			TreeLinePath = GetTemplateChild("PART_TreeLinePath") as Path;
			ExpandButton = GetTemplateChild("PART_ExpandButton") as TreeListNodeExpandButton;
			CheckBox = GetTemplateChild("PART_NodeCheckBox") as CheckBox;
			Image = GetTemplateChild("PART_NodeImage") as Image;
			if(View != null) {
				SetBinding(TreeLineStyleProperty, new Binding("View.TreeLineStyle"));
				SetBinding(ShowVerticalLinesProperty, new Binding("View.ShowVerticalLines"));
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			Size size = BaseMeasureOverride(constraint);
			size.Height = Math.Min(1, size.Height);
			size.Width = View.RowIndent * ActualRowLevel;
			return size;
		}
		protected override void UpdateContent(double height) {
			if(RowData == null || View == null) return;
			base.UpdateContent(height);
		}
		protected override void DrawLinesCore(double height) {
			base.DrawLinesCore(height);
			if(RowData == null || View == null) return;
			height = CorrectHeight(height);
			DrawRowLines(height);
			DrawTreeLines(height);
		}
		protected virtual double CorrectHeight(double height) {
			return height;
		}
		protected void DrawTreeLines(double height) {
			if(TreeLinePath == null) return;
			TreeLineGeometry = new GeometryGroup();
			if(RowData.Indents != null && ShowTreeLines) {
				List<TreeListIndentType> indents = RowData.Indents;
				for(int i = 0; i < indents.Count; i++)
					DrawTreeLine(i, indents[i], height);
			}
			TreeLinePath.Data = TreeLineGeometry;
		}
		protected virtual void DrawRowLines(double height) {
			DrawRowHorizontalLine(height);
			DrawRowVerticalLine(height);
		}
		protected virtual void DrawRowHorizontalLine(double height) {
			if(!View.ShowHorizontalLines) return;
			if(NextRowLevel > -1 && RowLevel > NextRowLevel)
				Group.Children.Add(CreateLine(new Point(ActualRowLevel * Offset + 1, height - 0.5), new Point((NextRowLevel + View.ServiceIndentsCount) * Offset, height - 0.5)));
		}
		protected virtual void DrawRowVerticalLine(double height) {
			if(!ShowVerticalLines) return;
			Group.Children.Add(CreateLine(new Point(Math.Abs(ActualRowLevel * Offset - 0.5), View.ShowHorizontalLines ? -1 : 0), new Point(Math.Abs(ActualRowLevel * Offset - 0.5), height)));
		}
		protected virtual void DrawTreeLine(int index, TreeListIndentType indent, double height) {
			double x1 = index * Offset;
			double x2 = x1 + Offset;
			double offset = Math.Round(Offset / 2d) - 0.5;
			double h = Math.Floor(height / 2d) + 0.5;
			switch(indent) {
				case TreeListIndentType.Root:
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, h), new Point(x2, h)));
					break;
				case TreeListIndentType.Line:
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, 0), new Point(x1 + offset, height)));
					break;
				case TreeListIndentType.Middle:
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, h), new Point(x2, h)));
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, 0), new Point(x1 + offset, height)));
					break;
				case TreeListIndentType.First:
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, h), new Point(x1 + offset, height)));
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, h), new Point(x2, h)));
					break;
				case TreeListIndentType.Last:
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, 0), new Point(x1 + offset, h)));
					TreeLineGeometry.Children.Add(CreateLine(new Point(x1 + offset, h), new Point(x2, h)));
					break;
			}
		}
		LineGeometry CreateLine(Point startPoint, Point endPoint) {
			LineGeometry line = new LineGeometry();
			line.StartPoint = startPoint;
			line.EndPoint = endPoint;
			return line;
		}
		#region ISupportLoadingAnimation
		DataViewBase ISupportLoadingAnimation.DataView {
			get { return View; }
		}
		FrameworkElement ISupportLoadingAnimation.Element {
			get { return Image; }
		}
		bool ISupportLoadingAnimation.IsGroupRow {
			get { return false; }
		}
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		void OnIsReadyChanged() {
			if(DataContext != null) LoadingAnimationHelper.ApplyAnimation();
		}
		#endregion
	}
	public class IndentsPanel : Panel {
		public static readonly DependencyProperty RowIndentProperty;
		public double RowIndent {
			get { return (double)this.GetValue(RowIndentProperty); }
			set { this.SetValue(RowIndentProperty, value); }
		}
		static IndentsPanel() {
			Type ownerType = typeof(IndentsPanel);
			RowIndentProperty = DependencyPropertyManager.Register("RowIndent", typeof(Double), ownerType, new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		protected override Size MeasureOverride(Size availableSize) {
			int visibleElementCount = 0;
			double maxHeight = 0;
			foreach(UIElement child in Children) {
				child.Measure(new Size(RowIndent, availableSize.Height));
				if(CanProcessChild(child)) visibleElementCount++;
				if(child.DesiredSize.Height > maxHeight) maxHeight = child.DesiredSize.Height;
			}
			return new Size(RowIndent * visibleElementCount, maxHeight);
		}
		protected virtual bool CanProcessChild(UIElement child) {
			return child.Visibility == System.Windows.Visibility.Visible || child.Visibility == System.Windows.Visibility.Hidden;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			int currentPosition = 0;
			foreach(UIElement child in Children) {
				if(!CanProcessChild(child) && !(child is TreeListNodeExpandButton)) continue;
				double x = currentPosition * RowIndent + (RowIndent - child.DesiredSize.Width) / 2d;
				double y = (finalSize.Height - child.DesiredSize.Height) / 2d;
				child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
				currentPosition++;
			}
			return finalSize;
		}
	}
	public class IndentScroller : Control {
		System.Windows.Controls.Grid scrollableContent;
		Queue<UIElement> freeElements = new Queue<UIElement>();
		public IndentScroller() {
			this.SetDefaultStyleKey(typeof(IndentScroller));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			scrollableContent = GetTemplateChild("PART_ScrollableContent") as System.Windows.Controls.Grid;
			UpdateScrollableContent();
		}
		void UpdateScrollableContent() {
			if(scrollableContent != null) {
				while(freeElements.Count > 0) {
					scrollableContent.Children.Add(freeElements.Dequeue());
				}
			}
		}
		internal void AddScrollableElement(UIElement element, int position) {
			if(element != null) {
				freeElements.Enqueue(element);
				System.Windows.Controls.Grid.SetColumn(element, position);
			}
		}
		internal void SetScrollOffset(Thickness offset) {
			if(scrollableContent != null)
				scrollableContent.Margin = offset;
		}
	}
}
