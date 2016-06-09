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
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates"),
	TemplateVisualState(Name = "Normal", GroupName = "CommonStates"),
	TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Pressed", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Focused", GroupName = "FocusStates")
	]
	public class AxisScrollBarThumb : Control {
		static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsDragging", 
			typeof(bool), typeof(AxisScrollBarThumb), new PropertyMetadata(false, IsDraggingPropertyChanged));
		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
		static void IsDraggingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisScrollBarThumb thumb = d as AxisScrollBarThumb;
			if (thumb != null)
				thumb.UpdateVisualState();
		}
		Point origin;
		Point previousPosition;
		bool isMouseOver;
		bool isFocused;
		public bool IsDragging { get { return (bool)GetValue(IsDraggingProperty); } }
		UIElement ParentElement { get { return Parent as UIElement; } }
		public event DragCompletedEventHandler DragCompleted;
		public event DragDeltaEventHandler DragDelta;
		public event DragStartedEventHandler DragStarted;
		public AxisScrollBarThumb() {
			DefaultStyleKey = typeof(AxisScrollBarThumb);
		}
		void RaiseDragCompleted(bool canceled) {
			if (DragCompleted != null)
				DragCompleted(this, new DragCompletedEventArgs(previousPosition.X - origin.X, previousPosition.Y - origin.Y, canceled));
		}
		void UpdateVisualState(bool useTransitions) {
			if (!IsEnabled)
				VisualStateManager.GoToState(this, "Disabled", useTransitions);
			else if (IsDragging)
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			else if (isMouseOver)
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			else
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			if (isFocused && IsEnabled)
				VisualStateManager.GoToState(this, "Focused", useTransitions);
			else
				VisualStateManager.GoToState(this, "Unfocused", useTransitions);
		}
		void UpdateVisualState() {
			UpdateVisualState(true);
		}
		void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateVisualState(false);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			UIElement parentElement = ParentElement;
			if (parentElement != null &&  IsDragging) {
				Point point = e.GetPosition(parentElement);
				if (point != previousPosition) {
					if (DragDelta != null)
						DragDelta(this, new DragDeltaEventArgs(point.X - previousPosition.X, point.Y - previousPosition.Y));
					previousPosition = point;
				}
			}
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			if (IsDragging && IsEnabled) {
				this.SetValue(IsDraggingPropertyKey, false);
				ReleaseMouseCapture();
				RaiseDragCompleted(false);
			}
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			UIElement parentElement = ParentElement;
			if (parentElement != null && !e.Handled && !IsDragging && IsEnabled) {
				e.Handled = true;
				CaptureMouse();
				this.SetValue(IsDraggingPropertyKey, true);
				origin = e.GetPosition(parentElement);
				previousPosition = origin;
				bool shouldCancelDrag = false;
				try {
					if (DragStarted != null)
						DragStarted(this, new DragStartedEventArgs(origin.X, origin.Y));
					shouldCancelDrag = true;
				}
				finally {
					if (!shouldCancelDrag && IsDragging) {
						this.SetValue(IsDraggingPropertyKey, false);
						RaiseDragCompleted(true);
					}
				}
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			UIElement parentElement = ParentElement;
			if (parentElement != null && IsDragging) {
				e.Handled = true;
				this.SetValue(IsDraggingPropertyKey, false);
				ReleaseMouseCapture();
				Point point = e.GetPosition(parentElement);
				if (DragCompleted != null)
					DragCompleted(this, new DragCompletedEventArgs(point.X - origin.X, point.Y - origin.Y, false));
			}
			base.OnMouseLeftButtonUp(e);
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			isFocused = true;
			UpdateVisualState();
		}
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			isFocused = false;
			UpdateVisualState();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			isMouseOver = true;
			UpdateVisualState();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			isMouseOver = false;
			UpdateVisualState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			UpdateVisualState(false);
		}
	}
	[
	TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates"),
	TemplateVisualState(Name = "Normal", GroupName = "CommonStates"),
	TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Pressed", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Disabled", GroupName = "CommonStates"),
	TemplateVisualState(Name = "Focused", GroupName = "FocusStates")
	]
	public class AxisScrollBarThumbResizer : AxisScrollBarThumb {
		public AxisScrollBarThumbResizer() {
			DefaultStyleKey = typeof(AxisScrollBarThumbResizer);
		}
	}
}
