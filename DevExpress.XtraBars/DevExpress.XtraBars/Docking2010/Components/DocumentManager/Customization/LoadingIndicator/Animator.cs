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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars.Docking2010.Customization {
	internal class RingAnimator : DevExpress.Utils.Animation.ILoadingAnimator {
		RingProgressDotPainter painter = new RingProgressDotPainter();
		List<RingProgressDotInfoArgs> progressDotListCore;
		public event EventHandler Invalidate;
		public double Acceleration { get; set; }
		public double InitialSpeed { get; set; }
		public bool AnimationInProgress { get { return Timer.Enabled; } }
		public Timer Timer { get; set; }
		public RingAnimator() {
			Acceleration = 7.0;
			InitialSpeed = 6.0;
			InitProgressDotList();
			InitTimer();
		}
		protected void InitProgressDotList() {
			progressDotListCore = new List<RingProgressDotInfoArgs>();
			for(int i = 0; i < 5; i++) {
				RingProgressDotInfoArgs progressDot = new RingProgressDotInfoArgs(Acceleration, InitialSpeed, 0x19) { Delay = 5 * i };
				progressDot.CalcPeriod();
				progressDotListCore.Add(progressDot);
			}
		}
		protected void InitTimer() {
			Timer = new Timer();
			Timer.Interval = 0x26;
			Timer.Tick += new EventHandler(OnTimerTick);
		}
		public void Dispose() {
			Timer.Stop();
			Timer.Tick -= new EventHandler(OnTimerTick);
			Timer = null;
			progressDotListCore.Clear();
			progressDotListCore = null;
		}
		public void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds) {
			DrawAnimatedItem(cache, bounds, Color.Black);
		}
		public void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds, Color color) {
			if(!AnimationInProgress) {
				Timer.Start();
			}
			GraphicsState savedState = cache.Graphics.Save();
			try {
				cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				foreach(RingProgressDotInfoArgs progressDot in progressDotListCore) {
					progressDot.Calc(bounds);
					progressDot.Color = color;
					ObjectPainter.DrawObject(cache, painter, progressDot);
				}
			}
			finally {
				cache.Graphics.Restore(savedState);
			}
		}
		public Size GetMinSize() {
			return new Size(40, 40);
		}
		void OnTimerTick(object sender, EventArgs e) {
			RaiseInvalidate();
		}
		void RaiseInvalidate() {
			if(Invalidate != null) {
				Invalidate(this, EventArgs.Empty);
			}
		}
	}
	internal class RingProgressDotInfoArgs : ObjectInfoArgs {
		public double Acceleration { get; set; }
		public double AccelerationTime { get; set; }
		internal bool AllowDraw { get; set; }
		public Point Center { get; set; }
		public Color Color { get; set; }
		int CurrentFrame { get; set; }
		public int Delay { get; set; }
		public int HideDelay { get; set; }
		public double InitialSpeed { get; set; }
		double MaxSpeed { get; set; }
		public double Period { get; set; }
		public int Radius { get; set; }
		public double RotationAngle { get; set; }
		public RingProgressDotInfoArgs(double acceleration, double initialSpeed, int hideDelay) {
			Acceleration = acceleration;
			InitialSpeed = initialSpeed;
			HideDelay = hideDelay;
			Color = Color.White;
		}
		public void Calc(Rectangle bounds) {
			Radius = bounds.Height / 2;
			Point center = bounds.Location;
			center.Offset(Radius, Radius);
			Center = center;
			AllowDraw = false;
			if(CurrentFrame < -1) {
				CurrentFrame++;
			}
			else if(CurrentFrame > (2.0 * Period)) {
				CurrentFrame = -HideDelay;
			}
			else {
				double frameTime = CurrentFrame;
				frameTime = (CurrentFrame > Period) ? (CurrentFrame - (Period * ((int)(((double)CurrentFrame) / Period)))) : ((double)CurrentFrame);
				if(frameTime < AccelerationTime) {
					RotationAngle = (MaxSpeed * frameTime) - ((Acceleration * Math.Pow(frameTime, 2)) / 2.0);
				}
				if((frameTime >= AccelerationTime) && (frameTime <= (AccelerationTime + (140.0 / InitialSpeed)))) {
					RotationAngle += InitialSpeed;
				}
				if(frameTime > (AccelerationTime + (140.0 / InitialSpeed))) {
					double tempTime = frameTime - (AccelerationTime + (140.0 / InitialSpeed));
					RotationAngle = (InitialSpeed * tempTime) + ((Acceleration * Math.Pow(tempTime, 2)) / 2.0);
					RotationAngle += 250.0;
				}
				AllowDraw = true;
				CurrentFrame++;
			}
		}
		public void CalcPeriod() {
			double discriminant = Math.Pow(2.0 * InitialSpeed, 2.0) - ((4.0 * Acceleration) * -220.0);
			AccelerationTime = ((-2.0 * InitialSpeed) + Math.Sqrt(discriminant)) / (2.0 * Acceleration);
			MaxSpeed = InitialSpeed + (Acceleration * AccelerationTime);
			Period = (2.0 * AccelerationTime) + (140.0 / InitialSpeed);
			CurrentFrame -= Delay;
		}
	}
	internal class RingProgressDotPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawRingProgressDot(e.Cache, e as RingProgressDotInfoArgs);
		}
		public void DrawRingProgressDot(GraphicsCache cache, RingProgressDotInfoArgs info) {
			if(info.AllowDraw) {
				cache.Graphics.FillEllipse(cache.GetSolidBrush(info.Color),
					info.Center.X - (((float)Math.Cos(((info.RotationAngle + 250.0) * Math.PI) / 180.0)) * info.Radius),
					info.Center.Y - (((float)Math.Sin(((info.RotationAngle + 250.0) * Math.PI) / 180.0)) * info.Radius),
					(float)(info.Radius / 4), (float)(info.Radius / 4));
			}
		}
	}
}
