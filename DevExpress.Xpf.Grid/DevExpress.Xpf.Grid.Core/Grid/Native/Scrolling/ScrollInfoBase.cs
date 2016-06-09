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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using System.ComponentModel;
using System.Windows.Threading;
namespace DevExpress.Xpf.Grid.Native {
	public enum DataControlScrollMode { Pixel, Item, ItemPixel, RowPixel }
	public abstract class ScrollInfoBase {
		static TimeSpan defaultUpdateInterval = new TimeSpan(0, 0, 0, 0, 50);
		IScrollInfoOwner scrollOwner;
		SizeHelperBase sizeHelper;
		double extent = 0;
		double viewport = 0;
		protected double fOffset = 0;
		bool isValid = false;
		public ScrollInfoBase(IScrollInfoOwner scrollOwner, SizeHelperBase sizeHelper) {
			this.scrollOwner = scrollOwner;
			this.sizeHelper = sizeHelper;
#if SL  //B213508
			if(ScrollOwner is DependencyObject && DesignerProperties.GetIsInDesignMode((DependencyObject)ScrollOwner))
				this.extent = 1;
#endif
		}
		protected virtual IScrollInfoOwner ScrollOwner { get { return scrollOwner; } }
		protected SizeHelperBase SizeHelper { get { return sizeHelper; } }
		public double Viewport { get { return viewport; } }
		public double Extent { get { return extent; } }
		public virtual double Offset { get { return fOffset; } }
		public virtual void LineDown() { LinesDown(1); }
		public virtual void LineUp() { LinesUp(1); }
		public void PageDown() { SetOffsetForce(Offset + GetScrollableViewportSize()); }
		public void PageUp() { SetOffsetForce(Offset - GetScrollableViewportSize()); }
		public void MouseWheelUp() { if(IsScrollingPerPage()) PageUp(); else LinesUp(scrollOwner.WheelScrollLines); }
		public void MouseWheelDown() { if(IsScrollingPerPage()) PageDown(); else LinesDown(scrollOwner.WheelScrollLines); }
		internal double GetScrollableViewportSize() { return Viewport; }
		protected bool IsScrollingPerPage() {
			return scrollOwner.WheelScrollLines == DataPresenterBase.WheelScrollLinesPerPage;
		}
#if DEBUGTEST
		internal static bool AllowTimer = true;
#endif
		public void SetOffset(double value) {
			if(ScrollOwner.IsDeferredScrolling)
				SetDeferredOffset(value);
			else
				SetOffsetForce(value);
		}
		public virtual void SetOffsetForce(double value) {
			SetOffsetForce(value, true);
			NeedMeasure();
		}
		void SetOffsetForce(double value, bool onChanged) {
			value = ValidateOffset(value);
			if(fOffset == value)
				return;
			if(OnBeforeChangeOffset()) {
				this.fOffset = value;
				if(onChanged) OnScrollInfoChanged();
			}
		}
		DispatcherTimer timer;
		double prevOffset;
		void SetDeferredOffset(double value) {
			if(double.IsNaN(prevOffset))
				prevOffset = fOffset;
			SetOffsetForce(value, false);
#if DEBUGTEST
			if(!AllowTimer) return;
#endif
			if(timer == null) {
#if !SILVERLIGHT
				timer = new DispatcherTimer(DispatcherPriority.Render);
#else
				timer = new DispatcherTimer();
#endif
				timer.Interval = defaultUpdateInterval;
				timer.Tick += new EventHandler(timer_Tick);
				timer.Start();
			}
		}
		void timer_Tick(object sender, EventArgs e) {
			timer.Stop();
			timer = null;
			OnTimerTick();
		}
		internal void OnTimerTick() {
			if(!OnBeforeChangeOffset())
				fOffset = prevOffset;
			OnScrollInfoChanged();
			NeedMeasure();
			prevOffset = double.NaN;
		}
		protected virtual double ValidateOffset(double value) {
			return ValidateOffsetCore(Math.Ceiling(value));
		}
		protected virtual void ValidateScrollInfo() {
		}
		protected abstract bool OnBeforeChangeOffset();
		public void Invalidate() {
			isValid = false;
		}
		public void UpdateScrollInfo(double viewport, double extent) {
			bool changed = false;
			if(Extent != extent) {
				changed = true;
				this.extent = extent;
			}
			if(Viewport != viewport) {
				this.viewport = viewport;
				changed = true;
			}
			if(changed || !isValid) {
				isValid = true;
				OnScrollInfoChanged();
			}
		}
		protected abstract double ValidateOffsetCore(double value);
		protected virtual void NeedMeasure() {
			ScrollOwner.InvalidateMeasure();
		}
		protected abstract void OnScrollInfoChanged();
		protected void LinesDown(double lineSize) { SetOffsetForce(Offset + lineSize); }
		protected void LinesUp(double lineSize) { SetOffsetForce(Offset - lineSize); }
	}
}
