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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Services;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Office.Internal;
#if SL
using Visual = System.Windows.UIElement;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.Core.Native;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndepententKeyPressEventArgs = DevExpress.Data.KeyPressEventArgs;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
#else
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerControl : IMouseWheelScrollClient {
		void InitializeMouse() {
			BehaviorCollection behaviors = Interaction.GetBehaviors(this);
			behaviors.Add(new SchedulerMouseWheelScrollBehavior());			
		}
		protected virtual PlatformIndependentMouseEventArgs CreateMouseEventArgs(System.Windows.Input.MouseEventArgs originalEventArgs, bool leftButtonPressed, bool rightButtonPressed, int clicks, int delta) {
			Point pt = originalEventArgs.GetPosition(this);
			return new PlatformIndependentMouseEventArgs(CalculateMouseButton(leftButtonPressed, rightButtonPressed), clicks, (int)pt.X, (int)pt.Y, delta);
		}
		PlatformIndependentMouseButtons CalculateMouseButton(bool leftButtonPressed, bool rightButtonPressed) {
			if (leftButtonPressed) return PlatformIndependentMouseButtons.Left;
			if (rightButtonPressed) return PlatformIndependentMouseButtons.Right;
			return PlatformIndependentMouseButtons.None;
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->MouseMove: menuOpen={0}, IsMouseCaptured={1}, LeftButtonPressed={2}", Menu.IsOpen, IsMouseCaptured, LeftButtonPressed);
			if (Menu.IsOpen)
				return;			
			ISchedulerStateService svcState = GetService<ISchedulerStateService>();
			if (svcState.IsInplaceEditorOpened)
				return;
			IMouseHandlerService svcMouseHandler = GetService<IMouseHandlerService>();
			if (svcMouseHandler != null)
				svcMouseHandler.OnMouseMove(CreateMouseEventArgs(e, LeftButtonPressed, RightButtonPressed, 0, 0));
			base.OnMouseMove(e);
		}
#if SL
		protected virtual void OnMouseButtonDownHandler(object sender, MouseButtonEventArgs e) {
			if (!(sender is SchedulerControl))
				return;			
			OnMouseLeftButtonDownCore(e);
			object focusedElement = DevExpress.Xpf.Core.WPFCompatibility.Helpers.FocusHelper.GetFocusedElement();
			if (focusedElement is ScrollViewer)
				this.Focus();
		}
		protected virtual void OnMouseButtonUpHandler(object sender, MouseButtonEventArgs e) {
			if (!(sender is SchedulerControl))
				return;
			OnMouseLeftButtonUp(e);
			if (!IsKeyboardFocusWithin)
				this.Focus();
		}
#else
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnMouseLeftButtonDown");
			OnMouseLeftButtonDownCore(e);
			base.OnMouseLeftButtonDown(e);
		}
		internal PlatformIndependentMouseEventArgs CreatePlatformIndependentMouseEventArgs(System.Windows.Input.MouseEventArgs e) {
			return CreateMouseEventArgs(e, LeftButtonPressed, RightButtonPressed, 0, 0);
		}
#endif
		private void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			if (Menu.IsOpen)
				return;
			LeftButtonPressed = true;
			CaptureMouse();
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null) {
				InplaceEditController.DoCommit();
				svc.OnMouseDown(CreateMouseEventArgs(e, true, false, 1, 0));
			}					  
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnMouseLeftButtonUp");
			if (Menu.IsOpen)
				return;			
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseUp(CreateMouseEventArgs(e, true, false, 1, 0));
			LeftButtonPressed = false;
			if (e.RightButton == MouseButtonState.Released)
				RightButtonPressed = false;
			base.OnMouseLeftButtonUp(e);
			if (!(LeftButtonPressed || RightButtonPressed))
				ReleaseMouseCapture();
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnMouseRightButtonDown");
			if (Menu.IsOpen)
				return;
			RightButtonPressed = true;
			CaptureMouse();
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseDown(CreateMouseEventArgs(e, false, true, 1, 0));
#if SL
			if (FormManager.IsFormExceptReminderOpen)
				return;
			if (Menu.IsOpen)
				Menu.ClosePopup();
			Point point = e.GetPosition(null);
			OnPopupMenu(XpfTypeConverter.FromPlatformPoint(point));
			e.Handled = true;
			if (!IsKeyboardFocusWithin)
				this.Focus();
#endif
			base.OnMouseRightButtonDown(e);
		}
		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnMouseRightButtonUp");
			if (Menu.IsOpen)
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseUp(CreateMouseEventArgs(e, false, true, 1, 0));
			base.OnMouseRightButtonUp(e);
			RightButtonPressed = false;
			if (e.LeftButton == MouseButtonState.Released)
				LeftButtonPressed = false;
			if (!(LeftButtonPressed || RightButtonPressed))
				ReleaseMouseCapture();			
		}
#if !SL
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->OnMouseDown");
			if (Menu.IsOpen)
				return;
			if (!IsKeyboardFocusWithin)
				Keyboard.Focus(this);
			base.OnMouseDown(e);
		}
#endif
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.Mouse, "->IMouseWheelScrollClient.OnMouseWheel");
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null) {
				svc.OnMouseWheel(CreatePlatformIndependentMouseWheelEventArgs(e));
			}
		}
		PlatformIndependentMouseEventArgs CreatePlatformIndependentMouseWheelEventArgs(MouseWheelEventArgs e) {
			Point pt = e.GetPosition(this);
#if !SL
			MouseWheelEventArgsEx ee = e as MouseWheelEventArgsEx;
			bool isHorizontal = ee != null;
			int delta = (ee == null) ? e.Delta : ee.DeltaX;
			OfficeMouseWheelEventArgs coreArgs = new OfficeMouseWheelEventArgs(CalculateMouseButton(e), 0, (int)pt.X, (int)pt.Y, delta / System.Windows.Forms.SystemInformation.MouseWheelScrollDelta);
			coreArgs.IsHorizontal = isHorizontal;
			return coreArgs;
#else
			return new PlatformIndependentMouseEventArgs(CalculateMouseButton(), 0, (int)pt.X, (int)pt.Y, e.Delta / DevExpress.Utils.SystemInformation.MouseWheelScrollDelta);
#endif
		}
#if !SL
		PlatformIndependentMouseButtons CalculateMouseButton(MouseEventArgs e) {
			return CalculateMouseButton(e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed);
		}
#else
		PlatformIndependentMouseButtons CalculateMouseButton() {
			return CalculateMouseButton(LeftButtonPressed, RightButtonPressed);
		}
#endif        
	}
}
