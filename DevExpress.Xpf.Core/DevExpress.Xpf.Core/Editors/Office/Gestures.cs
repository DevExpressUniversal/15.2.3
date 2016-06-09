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
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Utils;
namespace DevExpress.Xpf.Office.Internal {
	public interface IGestureClient {
		void OnManipulationStarting(ManipulationStartingEventArgs e);
		int OnVerticalPan(ManipulationDeltaEventArgs e);
		int OnHorizontalPan(ManipulationDeltaEventArgs e);
		void OnZoom(ManipulationDeltaEventArgs e, double deltaZoom);
	}
	public class GestureHelper {
		readonly IGestureClient client;
		bool prevIsManipulationEnabled;
		UIElement element;
		GestureHelperState state;
		public GestureHelper(IGestureClient client) {
			Guard.ArgumentNotNull(client, "client");
			this.client = client;
		}
		public GestureHelperState State { get { return state; } }
		public IGestureClient Client { get { return client; } }
		public void Start(UIElement element) {
			Stop();
			this.element = element;
			if (element != null) {
				this.prevIsManipulationEnabled = element.IsManipulationEnabled;
				element.IsManipulationEnabled = true;
				SubscribeEvents();
			}
			SwitchToDefaultState();
		}
		public void Stop() {
			if (this.element != null) {
				UnsubscribeEvents();
				this.element.IsManipulationEnabled = prevIsManipulationEnabled;
			}
		}
		internal void SwitchToDefaultState() {
			this.SwitchState(new InitialGestureHelperState(this));
		}
		public void SwitchState(GestureHelperState newState) {
			if (state != null)
				state.Finish();
			this.state = newState;
			state.Start();
		}
		protected internal virtual void SubscribeEvents() {
			element.TouchDown += OnTouchDown;
			element.TouchUp += OnTouchUp;
			element.ManipulationStarting += OnManipulationStarting;
			element.ManipulationInertiaStarting += OnManipulationInertiaStarting;
			element.ManipulationStarted += OnManipulationStarted;
			element.ManipulationDelta += OnManipulationDelta;
			element.ManipulationCompleted += OnManipulationCompleted;
		}
		protected internal virtual void UnsubscribeEvents() {
			element.TouchDown -= OnTouchDown;
			element.TouchUp -= OnTouchUp;
			element.ManipulationStarting -= OnManipulationStarting;
			element.ManipulationInertiaStarting -= OnManipulationInertiaStarting;
			element.ManipulationDelta -= OnManipulationDelta;
			element.ManipulationCompleted -= OnManipulationCompleted;
		}
		void OnTouchDown(object sender, TouchEventArgs e) {
			State.OnTouchDown(e);
		}
		void OnTouchUp(object sender, TouchEventArgs e) {
			State.OnTouchUp(e);
		}
		void OnManipulationStarting(object sender, ManipulationStartingEventArgs e) {
			State.OnManipulationStarting(e);
		}
		void OnManipulationStarted(object sender, ManipulationStartedEventArgs e) {
			State.OnManipulationStarted(e);
		}
		void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
			State.OnManipulationDelta(e);
		}
		void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e) {
			State.OnManipulationInertiaStarting(e);
		}
		void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {
			SwitchToDefaultState();
		}
	}
	public abstract class GestureHelperState {
		readonly GestureHelper helper;
		protected GestureHelperState(GestureHelper helper) {
			Guard.ArgumentNotNull(helper, "helper");
			this.helper = helper;
		}
		protected GestureHelper Helper { get { return helper; } }
		public IGestureClient Client { get { return Helper.Client; } }
		public virtual void OnTouchDown(TouchEventArgs e) {
		}
		public virtual void OnTouchUp(TouchEventArgs e) {
		}
		public virtual void OnManipulationStarting(ManipulationStartingEventArgs e) {
		}
		public virtual void OnManipulationStarted(ManipulationStartedEventArgs e) {
		}
		public virtual void OnManipulationDelta(ManipulationDeltaEventArgs e) {
		}
		public virtual void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
		}
		public virtual void Start() {
		}
		public virtual void Finish() {
		}
	}
	public class InitialGestureHelperState : GestureHelperState {
		bool disableManipulations;
		ManipulationModes allowedManipulations;
		DispatcherTimer timer;
		int touchDownCount;
		int timerElapsedCount;
		public InitialGestureHelperState(GestureHelper helper)
			: base(helper) {
		}
		bool IsZoomingAllowed { get { return (allowedManipulations & ManipulationModes.Scale) != 0; } }
		bool IsHorizontalPanningAllowed { get { return (allowedManipulations & ManipulationModes.TranslateX) != 0; } }
		bool IsVerticalPanningAllowed { get { return (allowedManipulations & ManipulationModes.TranslateY) != 0; } }
		public override void Start() {
		}
		public override void Finish() {
			StopTimer();
		}
		public override void OnTouchDown(TouchEventArgs e) {
			StopTimer();
			this.touchDownCount++;
			this.disableManipulations = false;
			this.timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(500);
			timer.Tick += OnTimerTick;
			timer.Start();
		}
		void StopTimer() {
			if (timer != null) {
				timer.Stop();
				timer.Tick -= OnTimerTick;
				timer = null;
			}
		}
		void OnTimerTick(object sender, EventArgs e) {
			this.disableManipulations = true; 
			StopTimer();
			timerElapsedCount++;
		}
		public override void OnTouchUp(TouchEventArgs e) {
			this.disableManipulations = true;
			StopTimer();
		}
		public override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			if (disableManipulations) {
				e.Cancel();
				e.Handled = true;
			}
			Client.OnManipulationStarting(e);
			allowedManipulations = e.Mode;
		}
		public override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			if (disableManipulations) {
				e.Cancel();
				e.Handled = true;
			}
			if (IsZoomingAllowed && touchDownCount == 2) {
				Helper.SwitchState(new ZoomGestureHelperState(Helper));
				Helper.State.OnManipulationDelta(e);
				return;
			}
			Vector translation = e.DeltaManipulation.Translation;
			double deltaX = translation.X;
			double deltaY = translation.Y;
			if (deltaX != 0.0 && Math.Abs(deltaX) > Math.Abs(deltaY)) {
				if (IsHorizontalPanningAllowed) {
					Helper.SwitchState(new HorizontalPanGestureHelperState(Helper));
					Helper.State.OnManipulationDelta(e);
					return;
				}
				else {
					e.Cancel();
					e.Handled = true;
					this.disableManipulations = true;
				}
			}
			if (IsVerticalPanningAllowed && deltaY != 0.0) {
				Helper.SwitchState(new VerticalPanGestureHelperState(Helper));
				Helper.State.OnManipulationDelta(e);
				return;
			}
		}
		public override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.Cancel();
			e.Handled = true;
		}
	}
	public abstract class PanGestureHelperStateBase : GestureHelperState {
		static readonly TimeSpan boundaryFeedBackDuration = TimeSpan.FromMilliseconds(250);
		DateTime boundaryFeedBackStartTime;
		protected PanGestureHelperStateBase(GestureHelper helper)
			: base(helper) {
		}
		public override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			boundaryFeedBackStartTime = DateTime.MinValue;
		}
		public override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.TranslationBehavior.DesiredDeceleration = 0.001; 
			e.Handled = true;
		}
		protected void ReportBoundaryFeekback(int overPanX, int overPanY, ManipulationDeltaEventArgs e) {
			if (overPanX != 0.0 || overPanY != 0.0) {
				if (boundaryFeedBackStartTime == DateTime.MinValue)
					boundaryFeedBackStartTime = DateTime.Now;
				e.ReportBoundaryFeedback(new ManipulationDelta(new Vector(overPanX, overPanY), 0, new Vector(), new Vector()));
				if ( (DateTime.Now - boundaryFeedBackStartTime) > boundaryFeedBackDuration)
					e.Complete();
			}
		}
	}
	public class HorizontalPanGestureHelperState : PanGestureHelperStateBase {
		public HorizontalPanGestureHelperState(GestureHelper helper)
			: base(helper) {
		}
		public override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			if (e.DeltaManipulation.Translation.X != 0.0) {
				int overPan = Client.OnHorizontalPan(e);
				ReportBoundaryFeekback(overPan, 0, e);
			}
		}
	}
	public class VerticalPanGestureHelperState : PanGestureHelperStateBase {
		public VerticalPanGestureHelperState(GestureHelper helper)
			: base(helper) {
		}
		public override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			if (e.DeltaManipulation.Translation.Y != 0.0) {
				int overPan = Client.OnVerticalPan(e);
				ReportBoundaryFeekback(0, overPan, e);
			}
		}
	}
	public class ZoomGestureHelperState : GestureHelperState {
		public ZoomGestureHelperState(GestureHelper helper)
			: base(helper) {
		}
		public override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			if (e.IsInertial) {
				e.Complete();
				e.Handled = true;
				Helper.SwitchToDefaultState();
			}
			double deltaZoom = e.DeltaManipulation.Scale.X;
			if (deltaZoom != 1.0 && !e.IsInertial) {
				deltaZoom = Math.Min(1.15, Math.Max(0.85, deltaZoom));
				Client.OnZoom(e, deltaZoom);
			}
		}
		public override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			e.Handled = true;
			e.Cancel();
			Helper.SwitchToDefaultState();
		}
	}
}
