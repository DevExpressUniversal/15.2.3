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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Services;
using System.ComponentModel.Design;
using DevExpress.Utils;
using System.Drawing.Imaging;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Animation.Internal {
	public interface ISchedulerAnimationManager : IDisposable {
		bool Paint(PaintEventArgs e);
		void SetRestrictions(System.Collections.Generic.List<SchedulerControlChangeType> changeTypes);
		void SetRestrictions();
		void RemoveRestrictions(bool reset);
#if DEBUGTEST
		bool IsAnimationActive();
#endif
		void Lock();
		void Unlock();
		void CompleteAnimation();
	}
	public class EmptySchedulerAnimationManager : ISchedulerAnimationManager {
		public bool Paint(PaintEventArgs e) {
			return false;
		}
		public void SetRestrictions(System.Collections.Generic.List<SchedulerControlChangeType> changeTypes) {
		}
		public void SetRestrictions() {
		}
		public void RemoveRestrictions(bool reset) {
		}
#if DEBUGTEST
		public bool IsAnimationActive() {
			return false;
		}		
#endif
		public void Lock() {
		}
		public void Unlock() {
		}
		public void Dispose() {
		}
		public void CompleteAnimation() {
		}
	}
	public class SchedulerAnimationManager : ISchedulerAnimationManager, IDisposable {
		LastPaintInfo lastPaintInfo;
		int drawToBitmapCount = 0; 
		AnimationJob animationJob;
		SchedulerControl scheduler;
		bool enable = true;
		bool innerEnable = true;
		public SchedulerAnimationManager(SchedulerControl scheduler) {
			this.lastPaintInfo = new LastPaintInfo();
			this.scheduler = scheduler;
			UpdateEnable();
			SubscribeEvents(scheduler);
		}
		protected SchedulerControl Scheduler { get { return scheduler; } }
		internal virtual bool Enable {
			get {
				return enable && innerEnable;
			}
			private set {
				enable = value;
			}
		}
		public bool Paint(PaintEventArgs e) {
			if (!Enable)
				return false;
			if (this.drawToBitmapCount <= 0) {
				if (this.animationJob != null) {
					bool isAnimationRendered = this.animationJob.OnPaint(e);
					if (isAnimationRendered)
						return true;
				}
				if (ShouldObtainAnimationInfo()) {
					this.lastPaintInfo.ObtainCurrentAnimationInfo(Scheduler);
					IAnimationService animationService = Scheduler.GetService<IAnimationService>();
					if (animationService != null && !animationService.CanRun())
						return false;
					AnimationEffect effect = AnimationEffectCalculatorBase.Calculate(Scheduler, this.lastPaintInfo.PrevAnimationInfo, this.lastPaintInfo.CurrentAnimationInfo);
					if (effect != null) {
						StartAnimation(e.Graphics, effect);
						return true;
					}
					e.Graphics.DrawImage(lastPaintInfo.CurrentAnimationInfo.Bitmap, Point.Empty);
					return true;
				}
			}
			return false;
		}
		public void CompleteAnimation() {
			if (this.animationJob == null)
				return;
			this.animationJob.Stop();
		}
		public void Lock() {
			this.drawToBitmapCount++;
		}
		public void Unlock() {
			this.drawToBitmapCount--;
		}
		public void SetRestrictions(System.Collections.Generic.List<SchedulerControlChangeType> changeTypes) {
			if (!Enable)
				return;
			for (int i = 0; i < changeTypes.Count; i++) {
				if (changeTypes[i] == SchedulerControlChangeType.DateTimeScroll) {
					this.innerEnable = false;
					break;
				}
			}
			if (!this.innerEnable)
				this.lastPaintInfo.ObtainCurrentAnimationInfo(Scheduler);
		}
		public void SetRestrictions() {
			if (!Enable)
				return;
			this.innerEnable = false;
			this.lastPaintInfo.ObtainCurrentAnimationInfo(Scheduler);
		}
		public void RemoveRestrictions(bool reset) {
			this.innerEnable = true;
			if (reset)
				this.lastPaintInfo.Reset(Scheduler);
		}
		void StartAnimation(Graphics graphics, AnimationEffect effect) {
			this.animationJob = new AnimationJob(Scheduler, effect);
			this.animationJob.OnPaintCore(graphics, 0);
			this.animationJob.Start();
			this.animationJob.Complete += new EventHandler(OnAnimationJobComplete);
			this.animationJob.Repaint += new EventHandler(OnAnimationJobRepaint);
		}
		void OnAnimationJobRepaint(object sender, EventArgs e) {
			Scheduler.Invalidate();
			Scheduler.Update();
		}
		void OnAnimationJobComplete(object sender, EventArgs e) {
			this.animationJob.Repaint -= OnAnimationJobRepaint;
			this.animationJob.Complete -= OnAnimationJobComplete;
			this.animationJob = null;
			this.lastPaintInfo.ObtainCurrentAnimationInfo(Scheduler);
			Scheduler.Invalidate();
			Scheduler.Update();
		}
		protected internal virtual bool ShouldObtainAnimationInfo() {
			if (drawToBitmapCount != 0)
				return false;
			if (Scheduler.IsUpdateLocked)
				return false;
			if (this.animationJob != null)
				return false;
			if (Scheduler.Bounds.Width <= 0 || Scheduler.Bounds.Height <= 0)
				return false;
			return this.lastPaintInfo.ShouldObtainAnimationInfo(Scheduler);
		}		
		void SubscribeEvents(SchedulerControl scheduler) {
			scheduler.OptionsView.Changed += OnOptionsBehaviorChanged;
		}
		void UnsubscribeEvents(SchedulerControl scheduler) {
			if (scheduler != null && scheduler.OptionsView != null)
				scheduler.OptionsView.Changed -= new Utils.Controls.BaseOptionChangedEventHandler(OnOptionsBehaviorChanged);
		}
		void OnOptionsBehaviorChanged(object sender, Utils.Controls.BaseOptionChangedEventArgs e) {
			UpdateEnable();
			this.lastPaintInfo.Reset(Scheduler);
		}
		void UpdateEnable() {
			Enable = Scheduler.OptionsView.EnableAnimation;
		}
		#region IDisposable implementation
		public void Dispose() {
			if (this.scheduler != null) {
				UnsubscribeEvents(this.scheduler);
				this.scheduler = null;
			}
			if (this.animationJob != null) {
				this.animationJob.Dispose();
				this.animationJob = null;
			}
			if (this.lastPaintInfo != null) {
				this.lastPaintInfo.Dispose();
				this.lastPaintInfo = null;
			}
		}
		#endregion
#if DEBUGTEST
		public bool IsAnimationActive() {
			if (!Scheduler.OptionsView.EnableAnimation)
				return false;
			return animationJob != null;
		}		
#endif        
	}
}
