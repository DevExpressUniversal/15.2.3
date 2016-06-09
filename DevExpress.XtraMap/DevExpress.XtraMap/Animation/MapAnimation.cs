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
using DevExpress.Utils.Drawing.Animation;
using System.Windows.Forms;
using System.Diagnostics;
namespace DevExpress.XtraMap.Drawing {
	public interface IMapAnimatableItem {
		bool EnableAnimation { get; }
		void FrameChanged(object sender, AnimationAction action, double progress);
	}
	public enum AnimationAction {
		FrameChanged,
		Complete
	}
	public class MapAnimation : ISupportXtraAnimation, IXtraAnimationListener, IDisposable {
		#region inner types: enum EasingMode, interface IEaseFunction, class PowerEaseFunction
		enum EasingMode { EaseIn, EaseOut }
		interface IEaseFunction {
			double Ease(double value);
		}
		class PowerEaseFunction : IEaseFunction {
			EasingMode easingMode;
			int power;
			public EasingMode EasingMode {
				get {
					return easingMode;
				}
				set {
					if(easingMode != value)
						easingMode = value;
				}
			}
			public int Power {
				get {
					return power;
				}
				set {
					if(power != value)
						power = value;
				}
			}
			public PowerEaseFunction() {
				EasingMode = EasingMode.EaseOut;
				Power = 2;
			}
			#region IEaseFunction Members
			double IEaseFunction.Ease(double value) {
				switch(EasingMode) {
					case EasingMode.EaseIn:
						return Math.Pow(value, Power);
					case EasingMode.EaseOut:
						return 1.0 - Math.Pow(1.0 - value, Power);
				}
				return value;
			}
			#endregion
		}
		#endregion
		const int DefaultTimeLine = 1200;
		readonly IMapAnimatableItem owner;
		readonly object obj = new object();
		readonly object locker = new object();
		readonly Stopwatch elapsedWatch = new Stopwatch();
		readonly IEaseFunction easeFunction = new PowerEaseFunction() { EasingMode = EasingMode.EaseOut, Power = 5 };
		int timeLine = DefaultTimeLine;
		FloatAnimationInfo floatAnimationInfo;
		bool disposed = false;
		IMapAnimatableItem Owner { get { return owner; } }
		bool Started { get; set; }
		public double Progress {
			get {
				double result;
				if(Started) {
					if(InProgress) {
						double res = (this.elapsedWatch.ElapsedMilliseconds);
						double progress = Math.Max(0.0, Math.Min(1.0, res / (double)TimeLine));
						result = easeFunction.Ease(progress);
					}
					else
						result = 1.0;
				}
				else result = 0.0;
				return result;
			}
		}	   
		public bool InProgress { get; private set; }
		public int TimeLine {
			get { return timeLine; }
			set {
				if(timeLine == value)
					return;
				timeLine = value;
			}
		}
		public double FirstFrameProgress {
			get {
				return easeFunction.Ease(0.001 * TimeLine / floatAnimationInfo.FrameCount);
			}
		}
		public MapAnimation(IMapAnimatableItem owner) {
			this.owner = owner;
			this.elapsedWatch.Restart();
			this.InProgress = false;
			this.Started = false;
		}
		~MapAnimation() {
			Dispose(false);
		}
		#region ISupportXtraAnimation Members
		public bool CanAnimate {
			get { return Owner.EnableAnimation; }
		}
		public Control OwnerControl {
			get { return null; }
		}
		#endregion
		#region IXtraAnimationListener Members
		public void OnAnimation(BaseAnimationInfo info) {
			if(!info.AnimationId.Equals(this.obj))
				return;
			lock(locker) {
				if(Owner != null) 
				Owner.FrameChanged(this, AnimationAction.FrameChanged, Progress);
			}
		}
		public void OnEndAnimation(BaseAnimationInfo info) {
			EndAnimation();
		}
		#endregion
		void EndAnimation() {
			lock(locker) {
				InProgress = false;
				Owner.FrameChanged(this, AnimationAction.Complete, 1.0);
			}
		}
		void StopAnimation() {
			XtraAnimator.RemoveObject(this, this.obj);
		}
		void StartAnimation() {
			floatAnimationInfo = new FloatAnimationInfo(this, this.obj, TimeLine, 0.0f, 1.0f, false);
			StopAnimation();
			XtraAnimator.Current.AddAnimation(floatAnimationInfo);
		}
		void Animate() {
			lock(locker) {
				StartAnimation();
				this.elapsedWatch.Restart();
			}			
		}
		protected virtual void Dispose(bool disposing) {
			if (disposed)
				return;
			if (disposing && floatAnimationInfo != null)
				floatAnimationInfo.Dispose();
		}
		public void Start() {
			if(CanAnimate) {
				Started = true;
				InProgress = true;
				Animate();
			}
		}
		public void Stop() {
			lock(locker) {
				InProgress = false;
				StopAnimation();
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
