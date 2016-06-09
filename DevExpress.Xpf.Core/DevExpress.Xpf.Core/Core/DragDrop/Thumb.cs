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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
#if SILVERLIGHT
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {
	public class DXThumb :
#if SILVERLIGHT
		DevExpress.Xpf.Core.DragAndDrop.Thumb
#else
		 System.Windows.Controls.Primitives.Thumb
#endif
	{ }
}
namespace DevExpress.Xpf.Core.DragAndDrop {
	[Browsable(false)]
	public class SLThumb : Control {
#if SILVERLIGHT
		public event MouseButtonEventHandler MouseDoubleClick;
		protected DoubleClickImplementer DoubleClickImplementer;
		public SLThumb() {
			DoubleClickImplementer = new DoubleClickImplementer(this);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			DoubleClickImplementer.OnMouseLeftButtonUpDoubleClickForce(e, new MouseButtonEventHandler(OnMouseDoubleClickCore), MouseDoubleClick);
			base.OnMouseLeftButtonUp(e);
		}
		protected virtual void OnMouseDoubleClickCore(object sender, MouseButtonEventArgs e) {
			OnMouseDoubleClick(e);
		}
		protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e) {
		}
#endif
	}
#if !SILVERLIGHT
	[DefaultEvent("DragDelta"), Localizability(LocalizationCategory.NeverLocalize)]
#else
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates"), TemplateVisualState(Name = "Normal", GroupName = "CommonStates"), TemplateVisualState(Name = "Dragging", GroupName = "DraggingState"), TemplateVisualState(Name = "NoDragging", GroupName = "DraggingState")]
#endif
	[Browsable(false)]
	public class Thumb : SLThumb {
		#region Dependency Properties
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty IsDraggingProperty =
			DependencyPropertyManager.Register("IsDragging", typeof(bool), typeof(Thumb), new PropertyMetadata(false, (d, e) => ((Thumb)d).OnIsDraggingPropertyChanged()));
		public static readonly DependencyProperty AllowDragProperty =
			DependencyProperty.Register("AllowDrag", typeof(bool), typeof(Thumb), new PropertyMetadata(true, (d, e) => ((Thumb)d).OnAllowDragPropertyChanged()));
		public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent("DragCompleted", RoutingStrategy.Direct, typeof(DragCompletedEventHandler), typeof(Thumb));
		public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent("DragDelta", RoutingStrategy.Direct, typeof(DragDeltaEventHandler), typeof(Thumb));
		public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Direct, typeof(DragStartedEventHandler), typeof(Thumb));
		#endregion Dependency Properties
		#region Events
		[Category("Behavior")]
		public event DragCompletedEventHandler DragCompleted {
			add { AddHandler(DragCompletedEvent, value); }
			remove { RemoveHandler(DragCompletedEvent, value); }
		}
		[Category("Behavior")]
		public event DragDeltaEventHandler DragDelta {
			add { AddHandler(DragDeltaEvent, value); }
			remove { RemoveHandler(DragDeltaEvent, value); }
		}
		[Category("Behavior")]
		public event DragStartedEventHandler DragStarted {
			add { AddHandler(DragStartedEvent, value); }
			remove { RemoveHandler(DragStartedEvent, value); }
		}
		protected virtual void RaiseDragCompleted(double horizontalChange, double verticalChange, bool canceled) {
			RaiseEvent(new DragCompletedEventArgs(horizontalChange, verticalChange, canceled) { RoutedEvent = DragCompletedEvent });			
		}
		protected virtual void RaiseDragDelta(double horizontalChange, double verticalChange) {
			RaiseEvent(new DragDeltaEventArgs(horizontalChange, verticalChange) { RoutedEvent = DragDeltaEvent });			
		}
		protected virtual void RaiseDragStarted(double horizontalOffset, double verticalOffset) {
			RaiseEvent(new DragStartedEventArgs(horizontalOffset, verticalOffset) { RoutedEvent = DragStartedEvent });			
		}
		#endregion Events
		public bool IsDragging {
			get { return (bool)GetValue(IsDraggingProperty); }
			protected set { SetValue(IsDraggingProperty, value); }
		}
		public bool AllowDrag {
			get { return (bool)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}
		public Point StartDragPointOnThumb { get; protected set; }
		public Point StartDragPointOnScreen { get; protected set; }
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool SuppressHandleMouseEvents { get; set; }
		public Thumb() {
			Focusable = false;
			ClearDragProperty();
		}
		public void CancelDrag() {
			if (IsDragging) {
				RaiseDragCompleted(this.previousScreenCoordPosition.X - StartDragPointOnScreen.X,
					this.previousScreenCoordPosition.Y - StartDragPointOnScreen.Y, true);
				ClearDragProperty();
			}
		}
		protected virtual void ClearDragProperty() {
			StartDragPointOnThumb = new Point();
			StartDragPointOnScreen = new Point();
			IsDragging = false;
			MouseHelper.ReleaseCapture(this);
		}
		protected virtual void StartDrag(MouseButtonEventArgs e) {
			if(e != null) {
				if(!SuppressHandleMouseEvents)
					e.Handled = true;
				StartDragPointOnThumb = e.GetPosition(this);
				StartDragPointOnScreen = e.GetPosition(null);
			}
			IsDragging = MouseHelper.Capture(this);
			this.previousScreenCoordPosition = StartDragPointOnScreen;
			RaiseDragStarted(StartDragPointOnThumb.X, StartDragPointOnThumb.Y);
		}
		protected virtual void CompleteDrag(MouseButtonEventArgs e) {
			if(!SuppressHandleMouseEvents)
				e.Handled = true;
			Point point = e.GetPosition(null);
			RaiseDragCompleted(point.X - StartDragPointOnScreen.X, point.Y - StartDragPointOnScreen.Y, false);
			ClearDragProperty();
		}
		protected virtual void MoveDrag(MouseEventArgs e) {
			Point pointOnThumb = e.GetPosition(this);
			Point pointOnScreen = e.GetPosition(null);
			if (pointOnScreen != this.previousScreenCoordPosition) {
				this.previousScreenCoordPosition = pointOnScreen;
				if(!SuppressHandleMouseEvents)
					e.SetHandled(true);
				RaiseDragDelta(pointOnThumb.X - StartDragPointOnThumb.X, pointOnThumb.Y - StartDragPointOnThumb.Y);
			}
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(!IsDragging && AllowDrag)
				StartDrag(e);
			UpdateVisualState();
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if (MouseHelper.Captured == this && IsDragging && AllowDrag)
				CompleteDrag(e);
			base.OnMouseLeftButtonUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (!IsDragging || !AllowDrag)
				return;
			if (IsDragging)
				MoveDrag(e);
			else
				ClearDragProperty();
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			if (MouseHelper.Captured == this)
				CancelDrag();
			base.OnLostMouseCapture(e);
		}
		protected virtual void OnAllowDragPropertyChanged() {
			if (IsDragging)
				CancelDrag();
			else
				ClearDragProperty();
		}
		protected virtual void OnIsDraggingPropertyChanged() {
			UpdateVisualState();
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			UpdateVisualState();
		}
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			UpdateVisualState();
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateVisualState();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateVisualState();
		}
		void GoToState(string stateName) {
			VisualStateManager.GoToState(this, stateName, false);
		}
		void UpdateVisualState() {
			if (IsDragging) {
				GoToState("Dragging");
				return;
			} else
				GoToState("NoDragging");
			if (IsMouseOver)
				GoToState("MouseOver");
			else
				GoToState("Normal");
		}
		#region Locals
		Point previousScreenCoordPosition;
		#endregion
	}
}
