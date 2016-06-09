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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplateVisualState(Name = "Visible", GroupName = "VisibleStates"),
	TemplateVisualState(Name = "Invisible", GroupName = "VisibleStates"),
	NonCategorized
	]
	public class ToolTipControl : ChartElementBase {
		public static readonly DependencyProperty ToolTipItemProperty = DependencyPropertyManager.Register("ToolTipItem",
			typeof(ToolTipItem), typeof(ToolTipControl), new FrameworkPropertyMetadata(null, ToolTipItemPropertyChanged));
		public ToolTipItem ToolTipItem {
			get { return (ToolTipItem)GetValue(ToolTipItemProperty); }
			set { SetValue(ToolTipItemProperty, value); }
		}
		static void ToolTipItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ToolTipControl tooltip = d as ToolTipControl;
			if (tooltip != null) {
				if (e.NewValue != null)
					tooltip.ShowToolTip();
				else
					tooltip.HideToolTip();
				tooltip.InvalidateLayout();
			}
		}
		FrameworkElement toolTipPanel;
		FrameworkElement toolTipContent;
		ToolTipLayoutCalculator layoutCalculator;
		FrameworkElement ToolTipPanel { get { return toolTipPanel; } }
		ChartControl Chart { get { return DataContext as ChartControl; } }
		public ToolTipControl() {
			DefaultStyleKey = typeof(ToolTipControl);
			layoutCalculator = new ToolTipLayoutCalculator(this);
		}
		void ShowToolTip() {
			this.Visibility = System.Windows.Visibility.Visible;
			VisualStateManager.GoToState(this, "Visible", true);
		}
		void HideToolTip() {
			this.Visibility = System.Windows.Visibility.Collapsed;
			VisualStateManager.GoToState(this, "Invisible", true);
		}
		internal Size GetToolTipContentSize() {
			return toolTipContent != null ? toolTipContent.DesiredSize : new Size(0, 0);
		}
		internal Point CalculateToolTipLocation(Size boundsSize) {
			ToolTipLayout layout = null;
			if (ToolTipItem != null) {
				if (ToolTipItem.ToolTipPosition is ToolTipMousePosition)
					layout = layoutCalculator.CalculatePointLayout(boundsSize);
				if (ToolTipItem.ToolTipPosition is ToolTipFreePosition) {
					IDockTarget dockTarget = ((ToolTipFreePosition)ToolTipItem.ToolTipPosition).DockTarget;
					if (dockTarget != null)
						layout = layoutCalculator.CalculateFreeLayout(dockTarget.GetBounds(), boundsSize);
					else
						layout = layoutCalculator.CalculateFreeLayout(new Rect(new Point(0,0), boundsSize), boundsSize);
				}
				if (ToolTipItem.ToolTipPosition is ToolTipRelativePosition)
					layout = layoutCalculator.CalculatePointLayout(boundsSize);
				ToolTipItem.Layout = layout;
				ToolTipItem.UpdateLayoutProperties();
			}
			return layout != null ? layout.Position : new Point();
		}
		internal void InvalidateLayout() {
			if (toolTipPanel != null)
				toolTipPanel.InvalidateMeasure();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			toolTipPanel = LayoutHelper.FindElementByName(this, "PART_ToolTipPanel");
			Popup popup = LayoutHelper.FindElementByName(this, "PART_Popup") as Popup;
			if (popup != null)
				toolTipContent = popup.Child as FrameworkElement;
		}
	}
	public class ToolTipPanel : Panel {
		ToolTipControl ToolTipControl { get { return DataContext as ToolTipControl; } }
		Size CalculateBoundsSize() {
			Size boundsSize = RenderSize;
			Rect popupScreenRect = Rect.Empty;
			if (ToolTipControl.ToolTipItem != null && !(ToolTipControl.ToolTipItem.ToolTipPosition is ToolTipFreePosition)) {
				Point anchorPosition = ToolTipControl.ToolTipItem.Position;
				Point screenOffset = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenPoint(ToolTipControl);
				Point mousePosition = System.Windows.Input.Mouse.GetPosition(ToolTipControl);
				Point mouseScreenAnchorPoint = new Point(screenOffset.X + mousePosition.X, screenOffset.Y + mousePosition.Y);
				Rect mouseScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(mouseScreenAnchorPoint);
				Point popupScreenPosition = new Point(screenOffset.X + anchorPosition.X, screenOffset.Y + anchorPosition.Y);
				if (popupScreenPosition.X < mouseScreenRect.Left)
					popupScreenPosition.X = mouseScreenRect.Left;
				if (popupScreenPosition.X > mouseScreenRect.Right)
					popupScreenPosition.X = mouseScreenRect.Right;
				if (popupScreenPosition.Y > mouseScreenRect.Bottom)
					popupScreenPosition.Y = mouseScreenRect.Bottom;
				popupScreenRect = DevExpress.Xpf.Core.Native.ScreenHelper.GetScreenRect(popupScreenPosition);
			}
			if (!popupScreenRect.IsEmpty)
				boundsSize = popupScreenRect.Size;
			return boundsSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement element in Children){
				Popup popup = element as Popup;
				if (popup != null)
					popup.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}
			return base.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Size boundsSize = CalculateBoundsSize();
			Point location = ToolTipControl.CalculateToolTipLocation(boundsSize);
			foreach (UIElement element in Children){
				Popup popup = element as Popup;
				if (popup != null) {
					AnnotationPanel panel = popup.Child as AnnotationPanel;
					popup.Child.InvalidateArrange();
					popup.Child.Arrange(new Rect(new Point(0, 0), popup.Child.DesiredSize));
					Point offset = panel.CalculatePopupOffset();
					popup.HorizontalOffset = location.X + offset.X;
					popup.VerticalOffset = location.Y + offset.Y;
				}
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
