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
using System.Drawing;
#if !SILVERLIGHT
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.Utils {
	#region AutoScroller (abstract class)
	public abstract class AutoScroller : IDisposable {
		#region Fields
		protected internal static readonly int AutoScrollTimerInterval = 100;
		bool isDisposed;
		Timer timer;
		bool isActive;
		MouseHandler mouseHandler;
		AutoScrollerHotZoneCollection hotZones;
		AutoScrollerHotZone activeHotZone;
		#endregion
		protected AutoScroller(MouseHandler mouseHandler) {
			if(mouseHandler == null)
				throw new ArgumentNullException();
			this.mouseHandler = mouseHandler;
			this.hotZones = new AutoScrollerHotZoneCollection();
			timer = CreateTimer();
#if !SILVERLIGHT
			timer.Interval = AutoScrollTimerInterval;
#else
			timer.Interval = new TimeSpan(0, 0, 0, 0, AutoScrollTimerInterval);
#endif
		}
		#region Properties
		public bool IsDisposed { get { return isDisposed; } }
		public MouseHandler MouseHandler { get { return mouseHandler; } }
		protected internal Timer Timer { get { return timer; } }
		protected internal bool IsActive { get { return isActive; } set { isActive = value; } }
		protected internal AutoScrollerHotZoneCollection HotZones { get { return hotZones; } }
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			if (isDisposed)
				return;
			if(timer != null) {
				Deactivate();
				StopTimer();
				timer.Dispose();
				timer = null;
			}
			hotZones.Clear();
			activeHotZone = null;
			isDisposed = true;
		}
		#endregion
		protected internal virtual Timer CreateTimer() {
			return new Timer();
		}
		#region SubscribeTimerEvents
		protected internal virtual void SubscribeTimerEvents() {
			timer.Tick += OnTimerTick;
		}
		#endregion
		#region UnsubscribeTimerEvents
		protected internal virtual void UnsubscribeTimerEvents() {
			timer.Tick -= OnTimerTick;
		}
		#endregion
		public virtual void Activate(Point mousePosition) {
			isActive = false;
			hotZones.Clear();
			PopulateHotZones();
			int count = hotZones.Count;
			for(int i = 0; i < count; i++)
				isActive |= hotZones[i].Initialize(mousePosition);
		}
		public virtual void Deactivate() {
			isActive = false;
			hotZones.Clear();
			activeHotZone = null;
		}
		public virtual void OnMouseMove(Point pt) {
			if(!isActive)
				return;
			AutoScrollerHotZone newActiveHotZone = CalculateActiveHotZone(pt);
			if(newActiveHotZone != activeHotZone) {
				activeHotZone = newActiveHotZone;
				if(activeHotZone != null)
					StartTimer();
				else
					StopTimer();
			}
		}
		protected internal virtual void StartTimer() {
			StopTimer();
			SubscribeTimerEvents();
			timer.Start();
		}
		protected internal virtual void StopTimer() {
			timer.Stop();
			UnsubscribeTimerEvents();
		}
		protected internal virtual AutoScrollerHotZone CalculateActiveHotZone(Point pt) {
			int count = hotZones.Count;
			for(int i = 0; i < count; i++) {
				if(hotZones[i].CanActivate(pt))
					return hotZones[i];
			}
			return null;
		}
		protected internal virtual void OnTimerTick(object sender, EventArgs e) {
			if(activeHotZone != null)
				activeHotZone.PerformAutoScroll();
		}
		public void Suspend() {
			StopTimer();
		}
		public void Resume() {
			StartTimer();
		}
		protected abstract void PopulateHotZones();
	}
	#endregion
	#region AutoScrollerHotZoneCollection
	public class AutoScrollerHotZoneCollection : List<AutoScrollerHotZone> {
	}
	#endregion
	#region AutoScrollerHotZone
	public abstract class AutoScrollerHotZone {
		Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual bool Initialize(Point mousePosition) {
			this.bounds = CalculateHotZoneBounds();
			if(bounds.Width <= 0 || bounds.Height <= 0)
				return false;
			this.bounds = AdjustHotZoneBounds(bounds, mousePosition);
			return true;
		}
		public abstract bool CanActivate(Point mousePosition);
		public abstract void PerformAutoScroll();
		protected abstract Rectangle CalculateHotZoneBounds();
		protected abstract Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition);
	}
	#endregion
	#region EmptyAutoScroller
	public class EmptyAutoScroller : AutoScroller {
		public EmptyAutoScroller(MouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override void PopulateHotZones() {
		}
	}
	#endregion
}
