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
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Xpf.Utils;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.Windows.Interop;
using DevExpress.Xpf.Editors.Automation;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.Automation;
#endif
#if !SL
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using System.Collections;
using DevExpress.Xpf.Editors.Native;
#else
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Native;
#endif
namespace DevExpress.Xpf.Editors {
	public class PopupSizeGrip : Control {
		const string ThumbName = "PART_Thumb";
		Point prevDragPoint;
		PostponedAction updateThumbPositionAction;
		PopupBaseEdit popupBaseEdit;
		PopupBaseEdit OwnerEdit { get { return popupBaseEdit ?? (popupBaseEdit = (PopupBaseEdit)BaseEdit.GetOwnerEdit(this)); } }
		Thumb Thumb { get; set; }
		public PopupSizeGrip() {
			updateThumbPositionAction = new PostponedAction(ShouldPostponeUpdateThumbPosition);
		}
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			Thumb = (Thumb)GetTemplateChild(ThumbName);
			SubscribeEvents();
		}
		 bool ShouldPostponeUpdateThumbPosition() {
#if SL
			return true;
#else
			return false;
#endif
		}
		void SubscribeEvents() {
			if (Thumb == null)
				return;
			Thumb.DragStarted += OnDragStarted;
			Thumb.DragCompleted += OnDragCompleted;
			Thumb.DragDelta += OnDragDelta;
		}
		void UnsubscribeEvents() {
			if (Thumb == null)
				return;
			Thumb.DragStarted -= OnDragStarted;
			Thumb.DragCompleted -= OnDragCompleted;
			Thumb.DragDelta -= OnDragDelta;
		}
		void SetStartSize() {
			if (double.IsNaN(OwnerEdit.PopupWidth))
				OwnerEdit.PopupWidth = ((FrameworkElement)OwnerEdit.Popup.Child).ActualWidth;
			if (double.IsNaN(OwnerEdit.PopupHeight))
				OwnerEdit.PopupHeight = ((FrameworkElement)OwnerEdit.Popup.Child).ActualHeight;
		}
		void OnDragStarted(object sender, DragStartedEventArgs e) {
			SetStartSize();
			Thumb_DragStarted(sender, e);
		}
		void OnDragCompleted(object sender, DragCompletedEventArgs e) {
			Thumb_DragCompleted(sender, e);
		}
		void OnDragDelta(object sender, DragDeltaEventArgs e) {
			if (OwnerEdit == null || OwnerEdit.Popup == null)
				return;
			Thumb_DragDelta(sender, e);
		}
		void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) {
#if SL
			((FrameworkElement)sender).MouseMove -= Thumb_MouseMove;
			mouseHelper = new MouseEventArgsHelper();
			ReleaseMouseCapture();
#endif
		}
		void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
#if SL
			CaptureMouse();
			mouseHelper = new MouseEventArgsHelper();
			((FrameworkElement)sender).MouseMove += Thumb_MouseMove;
#endif
			prevDragPoint = GetMousePosition(this);
		}
#if SL
		MouseEventArgsHelper mouseHelper = new MouseEventArgsHelper();
		void Thumb_MouseMove(object sender, MouseEventArgs e) {
			mouseHelper = new MouseEventArgsHelper(e);
			updateThumbPositionAction.PerformForce();
		}
#endif
		void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
			updateThumbPositionAction.PerformPostpone(() => {
#if SL
				Point delta = GetDelta(this);
#else
				Point delta = new Point(e.HorizontalChange, e.VerticalChange);
#endif
				bool changed = false;
				if (0 < Math.Abs(delta.X)) {
					OwnerEdit.PopupSettings.SetHorizontalPopupSizeChange(delta.X);
					changed = true;
				}
				if (0 < Math.Abs(delta.Y)) {
					OwnerEdit.PopupSettings.SetVerticalPopupSizeChange(delta.Y);
					changed = true;
				}
				if (changed)
					OwnerEdit.UpdateLayout();
			});
		}
		Point GetMousePosition(FrameworkElement element) {
			Point position;
#if !SL
			position = Mouse.GetPosition(element);
#else
			position = mouseHelper.GetMousePosition(element); ;
#endif
			return position;
		}
#if SL
		Point GetDelta(FrameworkElement thumb) {
			Point currentDragPosition = GetMousePosition(OwnerEdit);
			Point dragDelta = new Point(
				prevDragPoint.X == 0 ? 0 : currentDragPosition.X - prevDragPoint.X,
				prevDragPoint.Y == 0 ? 0 : currentDragPosition.Y - prevDragPoint.Y
				);
			Point relativeToThumbPosition = GetMousePosition(thumb);
			Point relativeToCenterThumbOffset = new Point(
				relativeToThumbPosition.X - GetSize(thumb).X / 2.0,
				relativeToThumbPosition.Y - GetSize(thumb).Y / 2.0
				);
			Point relativeToThumbDirection = new Point(
				Math.Sign(relativeToCenterThumbOffset.X),
				Math.Sign(relativeToCenterThumbOffset.Y)
				);
			prevDragPoint = currentDragPosition;
			return new Point(
				relativeToThumbDirection.X != Math.Sign(dragDelta.X) ? 0d : relativeToCenterThumbOffset.X,
				relativeToThumbDirection.Y != Math.Sign(dragDelta.Y) ? 0d : relativeToCenterThumbOffset.Y
				);
		}
#endif
		Point GetSize(FrameworkElement element) {
			if (element == null)
				return new Point();
			return new Point(element.ActualWidth, element.ActualHeight);
		}
	}
	class MouseEventArgsHelper {
		MouseEventArgs Args { get; set; }
		public MouseEventArgsHelper() {
		}
		public MouseEventArgsHelper(MouseEventArgs args) {
			Args = args;
		}
		public Point GetMousePosition(FrameworkElement element) {
			return Args != null ? Args.GetPosition(element) : new Point(0, 0);
		}
	}
}
