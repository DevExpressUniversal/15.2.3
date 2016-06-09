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

using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet.Layout;
using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Collections.Generic;
using System.Windows.Media;
using DrawingPoint = System.Drawing.Point;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetGroupControl
	public class WorksheetGroupControl : Control {
		#region Fields
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty ButtonTemplateProperty;
		public static readonly DependencyProperty LineThicknessProperty;
		public static readonly DependencyProperty LineBrushProperty;
		public static readonly DependencyProperty DotThicknessProperty;
		public static readonly DependencyProperty DotBrushProperty;
		GroupPanel panel;
		#endregion
		static WorksheetGroupControl() {
			Type ownerType = typeof(WorksheetGroupControl);
			HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(ControlTemplate), ownerType);
			ButtonTemplateProperty = DependencyProperty.Register("ButtonTemplate", typeof(ControlTemplate), ownerType);
			LineThicknessProperty = DependencyProperty.Register("LineThickness", typeof(double), ownerType);
			LineBrushProperty = DependencyProperty.Register("LineBrush", typeof(Brush), ownerType);
			DotThicknessProperty = DependencyProperty.Register("DotThickness", typeof(double), ownerType);
			DotBrushProperty = DependencyProperty.Register("DotBrush", typeof(Brush), ownerType);
		}
		public WorksheetGroupControl() {
			this.DefaultStyleKey = typeof(WorksheetGroupControl);
		}
		#region Properties
		public ControlTemplate HeaderTemplate {
			get { return (ControlTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		public ControlTemplate ButtonTemplate {
			get { return (ControlTemplate)GetValue(ButtonTemplateProperty); }
			set { SetValue(ButtonTemplateProperty, value); }
		}
		public double LineThickness {
			get { return (double)GetValue(LineThicknessProperty); }
			set { SetValue(LineThicknessProperty, value); }
		}
		public Brush LineBrush {
			get { return (Brush)GetValue(LineBrushProperty); }
			set { SetValue(LineBrushProperty, value); }
		}
		public double DotThickness {
			get { return (double)GetValue(DotThicknessProperty); }
			set { SetValue(DotThicknessProperty, value); }
		}
		public Brush DotBrush {
			get { return (Brush)GetValue(DotBrushProperty); }
			set { SetValue(DotBrushProperty, value); }
		}
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			panel = LayoutHelper.FindElementByType(this, typeof(GroupPanel)) as GroupPanel;
			panel.HeaderTemplate = HeaderTemplate;
			panel.ButtonTemplate = ButtonTemplate;
			panel.LineThickness = LineThickness;
			panel.LineBrush = LineBrush;
			panel.DotThickness = DotThickness;
			panel.DotBrush = DotBrush;
			Invalidate();
		}
		internal void Invalidate() {
			if (panel != null)
				panel.InvalidateVisual();
			InvalidateVisual();
		}
	}
	#endregion
	#region GroupPanel
	public class GroupPanel : Panel {
		#region Fields
		public static readonly DependencyProperty LayoutInfoProperty;
		int childIndex = 0;
		#endregion
		static GroupPanel() {
			LayoutInfoProperty = DependencyProperty.Register("LayoutInfo", typeof(DocumentLayout), typeof(GroupPanel));
		}
		public GroupPanel() {
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
			Loaded += OnLoaded;
		}
		#region Properties
		protected internal WorksheetControl Owner { get; private set; }
		public ControlTemplate HeaderTemplate { get; set; }
		public ControlTemplate ButtonTemplate { get; set; }
		public double LineThickness { get; set; }
		public Brush LineBrush { get; set; }
		public double DotThickness { get; set; }
		public Brush DotBrush { get; set; }
		public DocumentLayout LayoutInfo {
			get { return (DocumentLayout)GetValue(LayoutInfoProperty); }
			set { SetValue(LayoutInfoProperty, value); }
		}
		#endregion
		void OnLoaded(object sender, RoutedEventArgs e) {
			Owner = LayoutHelper.FindParentObject<WorksheetControl>(this);
			InvalidateVisual();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (LayoutInfo == null) {
				HideChildren();
				return finalSize;
			}
			GroupItemsPage page = LayoutInfo.GroupItemsPage;
			List<OutlineLevelBox> buttons = page.Buttons;
			if (buttons == null) {
				HideChildren();
				return finalSize;
			}
			foreach (OutlineLevelBox button in buttons) {
				GroupItem item = GetChild();
				item.Text = button.Text;
				item.SetSelectedType(button.OutlineLevelBoxSelectType);
				item.IsExpanded = button.IsExpandedButton;
				item.IsCollapsed = button.IsCollapsedButton;
				item.Template = button.IsHeaderButton ? HeaderTemplate : ButtonTemplate;
				Rect bounds = button.Bounds.ToRect();
				if (button.IsHeaderButton)
					bounds = CalculateHeaderButtonBounds(bounds, button.IsRowButton);
				item.Measure(bounds.Size);
				item.Arrange(bounds);
			}
			HideChildren();
			return finalSize;
		}
		Rect CalculateHeaderButtonBounds(Rect originalBounds, bool isRowButton) {
			Rect result = originalBounds;
			if (isRowButton) {
				double offset = CalculateHeaderButtonOffset(Owner.HeaderHeight, originalBounds.Height);
				result.Inflate(0, offset);
				result.Offset(-1, 0);
			}
			else {
				double offset = CalculateHeaderButtonOffset(Owner.HeaderWidth, originalBounds.Width);
				result.Inflate(offset, 0);
				result.Offset(0, -1);
			}
			return result;
		}
		double CalculateHeaderButtonOffset(double headerSize, double buttonSize) {
			return (headerSize - buttonSize) / 2.0f + 1;
		}
		GroupItem GetChild() {
			if (childIndex >= Children.Count) {
				GroupItem item = new GroupItem();
				item.Focusable = false;
				Children.Add(item);
			}
			return Children[childIndex++] as GroupItem;
		}
		void HideChildren() {
			for (int i = childIndex; i < Children.Count; i++)
				Children[i].Arrange(new Rect(0, 0, 0, 0));
			childIndex = 0;
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (LayoutInfo == null)
				return;
			GroupItemsPage groupPage = LayoutInfo.GroupItemsPage;
			DrawDots(dc, groupPage.Dots);
			DrawLines(dc, groupPage.Lines);
		}
		void DrawDots(DrawingContext dc, List<DrawingPoint> dots) {
			foreach (DrawingPoint dot in dots) {
				double offset = Math.Floor(DotThickness / 2);
				Rect bounds = new Rect(dot.X - offset, dot.Y - offset, DotThickness, DotThickness);
				dc.DrawRectangle(DotBrush, null, bounds);
			}
		}
		void DrawLines(DrawingContext dc, List<OutlineLevelLine> lines) {
			foreach (OutlineLevelLine line in lines) {
				DrawLine(dc, line.LinePoint1, line.LinePoint2);
				DrawLine(dc, line.TailPoint1, line.TailPoint2);
			}
		}
		void DrawLine(DrawingContext dc, DrawingPoint point1, DrawingPoint point2) {
			Rect bounds = GetLineBounds(point1, point2);
			dc.DrawRectangle(LineBrush, null, bounds);
		}
		Rect GetLineBounds(DrawingPoint point1, DrawingPoint point2) {
			double offset = Math.Ceiling(LineThickness / 2) - 1;
			if (point1.X == point2.X)
				return new Rect(point1.X - offset, point1.Y, LineThickness, point2.Y - point1.Y);
			return new Rect(point1.X, point1.Y - offset, point2.X - point1.X, LineThickness);
		}
	}
	#endregion
	#region GroupItem
	public class GroupItem : Control {
		#region Fields
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty IsHoveredProperty;
		public static readonly DependencyProperty IsPressedProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty IsCollapsedProperty;
		#endregion
		static GroupItem() {
			Type ownerType = typeof(GroupItem);
			TextProperty = DependencyProperty.Register("Text", typeof(string), ownerType);
			IsHoveredProperty = DependencyProperty.Register("IsHovered", typeof(bool), ownerType);
			IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), ownerType);
			IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), ownerType);
			IsCollapsedProperty = DependencyProperty.Register("IsCollapsed", typeof(bool), ownerType);
		}
		#region Properties
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public bool IsHovered {
			get { return (bool)GetValue(IsHoveredProperty); }
			set { SetValue(IsHoveredProperty, value); }
		}
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
		#endregion
		internal void SetSelectedType(OutlineLevelBoxSelectType outlineLevelBoxSelectType) {
			IsHovered = outlineLevelBoxSelectType == OutlineLevelBoxSelectType.Hovered;
			IsPressed = outlineLevelBoxSelectType == OutlineLevelBoxSelectType.Pressed;
		}
	}
	#endregion
}
