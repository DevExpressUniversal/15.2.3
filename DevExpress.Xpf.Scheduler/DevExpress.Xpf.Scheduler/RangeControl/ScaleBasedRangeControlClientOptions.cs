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
using DevExpress.XtraScheduler;
using System.Windows;
using DevExpress.Utils.Controls;
namespace DevExpress.Xpf.Scheduler {
#if !SL
	public class ScaleBasedRangeControlClientOptions : Freezable, IScaleBasedRangeControlClientOptions {
#else
	public class ScaleBasedRangeControlClientOptions : DependencyObject, IScaleBasedRangeControlClientOptions {
#endif
		internal const int DefaultMinIntervalWidth = 30;
		internal const int DefaultMaxIntervalWidth = 250;
		TimeScaleCollection scales;
		public ScaleBasedRangeControlClientOptions() {
			this.scales = CreateTimeScaleCollection();
			Reset();
		}
		IScaleBasedRangeControlClientOptions Instance { get { return (IScaleBasedRangeControlClientOptions)this; } }
		#region AutoFormatScaleCaptions
		bool IScaleBasedRangeControlClientOptions.AutoFormatScaleCaptions {
			get { return false; }
			set {
			}
		}
		#endregion
		#region DataDisplayType
		public RangeControlDataDisplayType DataDisplayType {
			get { return (RangeControlDataDisplayType)GetValue(DataDisplayTypeProperty); }
			set { SetValue(DataDisplayTypeProperty, value); }
		}
		public static readonly DependencyProperty DataDisplayTypeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ScaleBasedRangeControlClientOptions, RangeControlDataDisplayType>("DataDisplayType", RangeControlDataDisplayType.Auto, (d, e) => d.OnDataDisplayTypeChanged(e.OldValue, e.NewValue), null);
		void OnDataDisplayTypeChanged(RangeControlDataDisplayType oldValue, RangeControlDataDisplayType newValue) {
			RaiseChanged("DataDisplayType", oldValue, newValue);
		}
		#endregion
		#region MaxIntervalWidth
		public int MaxIntervalWidth {
			get { return (int)GetValue(MaxIntervalWidthProperty); }
			set { SetValue(MaxIntervalWidthProperty, value); }
		}
		public static readonly DependencyProperty MaxIntervalWidthProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ScaleBasedRangeControlClientOptions, int>("MaxIntervalWidth", DefaultMaxIntervalWidth, (d, e) => d.OnMaxIntervalWidthChanged(e.OldValue, e.NewValue), null);
		void OnMaxIntervalWidthChanged(int oldValue, int newValue) {
			RaiseChanged("MaxIntervalWidth", oldValue, newValue);
		}
		#endregion
		#region MinIntervalWidth
		public int MinIntervalWidth {
			get { return (int)GetValue(MinIntervalWidthProperty); }
			set { SetValue(MinIntervalWidthProperty, value); }
		}
		public static readonly DependencyProperty MinIntervalWidthProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ScaleBasedRangeControlClientOptions, int>("MinIntervalWidth", DefaultMinIntervalWidth, (d, e) => d.OnMinIntervalWidthChanged(e.OldValue, e.NewValue), null);
		void OnMinIntervalWidthChanged(int oldValue, int newValue) {
			RaiseChanged("MinIntervalWidth", oldValue, newValue);
		}
		#endregion
		#region MaxSelectedIntervalCount
		int IScaleBasedRangeControlClientOptions.MaxSelectedIntervalCount {
			get { return 0; }
			set {
			}
		}
		#endregion
		#region RangeMaximum
		public DateTime RangeMaximum {
			get { return (DateTime)GetValue(RangeMaximumProperty); }
			set { SetValue(RangeMaximumProperty, value); }
		}
		public static readonly DependencyProperty RangeMaximumProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ScaleBasedRangeControlClientOptions, DateTime>("RangeMaximum", DateTime.MinValue, (d, e) => d.OnRangeMaximumChanged(e.OldValue, e.NewValue), null);
		void OnRangeMaximumChanged(DateTime oldValue, DateTime newValue) {
			RaiseChanged("RangeMaximum", oldValue, newValue);
		}
		#endregion
		#region RangeMinimum
		public DateTime RangeMinimum {
			get { return (DateTime)GetValue(RangeMinimumProperty); }
			set { SetValue(RangeMinimumProperty, value); }
		}
		public static readonly DependencyProperty RangeMinimumProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ScaleBasedRangeControlClientOptions, DateTime>("RangeMinimum", DateTime.MaxValue, (d, e) => d.OnRangeMinimumChanged(e.OldValue, e.NewValue), null);
		void OnRangeMinimumChanged(DateTime oldValue, DateTime newValue) {
			RaiseChanged("RangeMinimum", oldValue, newValue);
		}
		#endregion
		#region ThumbnailHeight
		int IScaleBasedRangeControlClientOptions.ThumbnailHeight {
			get { return 0; }
			set { 
			}
		}
		#endregion
		#region IScaleBasedRangeControlClientOptions implementation
		#region Scales
		TimeScaleCollection IScaleBasedRangeControlClientOptions.Scales {
			get { return scales; }
		}
		#endregion
		#region Changed event
		event BaseOptionChangedEventHandler IScaleBasedRangeControlClientOptions.Changed {
			add { OptionChanged += value; }
			remove { OptionChanged -= value; }
		}
		event BaseOptionChangedEventHandler OptionChanged;
		protected virtual void RaiseChanged(string name, object oldValue, object newValue) {
			if (OptionChanged == null)
				return;
			OptionChanged(this, new DevExpress.Utils.Controls.BaseOptionChangedEventArgs(name, oldValue, newValue));
		}
		#endregion
		void IScaleBasedRangeControlClientOptions.BeginUpdate() {
		}
		void IScaleBasedRangeControlClientOptions.EndUpdate() {
		}
		#endregion
		protected virtual void Reset() {
			if (this.scales == null) {
				this.scales = CreateTimeScaleCollection();
			}
			this.scales.LoadDefaults();
			RangeMinimum = SchedulerRangeControlClient.DefaultRangeMinimum;
			RangeMaximum = SchedulerRangeControlClient.DefaultRangeMaximum;
			DataDisplayType = RangeControlDataDisplayType.Auto;
			Instance.ThumbnailHeight = 0;
			Instance.AutoFormatScaleCaptions = true;
			Instance.MaxSelectedIntervalCount = 0;
			MinIntervalWidth = DefaultMinIntervalWidth;
			MaxIntervalWidth = DefaultMaxIntervalWidth;
		}
		protected TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
#if !SL
		#region Frezable implementation
		protected override Freezable CreateInstanceCore() {
			return new ScaleBasedRangeControlClientOptions();
		}
		#endregion
#endif
	}
	public class SchedulerOptionsRangeControl : ScaleBasedRangeControlClientOptions, ISchedulerOptionsRangeControl {
		#region AllowChangeActiveView
		public bool AllowChangeActiveView {
			get { return (bool)GetValue(AllowChangeActiveViewProperty); }
			set { SetValue(AllowChangeActiveViewProperty, value); }
		}
		public static readonly DependencyProperty AllowChangeActiveViewProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerOptionsRangeControl, bool>("AllowChangeActiveView", false, (d, e) => d.OnAllowChangeActiveViewChanged(e.OldValue, e.NewValue), null);
		void OnAllowChangeActiveViewChanged(bool oldValue, bool newValue) {
			RaiseChanged("AllowChangeActiveView", oldValue, newValue);
		}
		#endregion
		#region AutoAdjustMode
		public bool AutoAdjustMode {
			get { return (bool)GetValue(AutoAdjustModeProperty); }
			set { SetValue(AutoAdjustModeProperty, value); }
		}
		public static readonly DependencyProperty AutoAdjustModeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerOptionsRangeControl, bool>("AutoAdjustMode", false, (d, e) => d.OnAutoAdjustModeChanged(e.OldValue, e.NewValue), null);
		void OnAutoAdjustModeChanged(bool oldValue, bool newValue) {
			RaiseChanged("AutoAdjustMode", oldValue, newValue);
		}
		#endregion
		protected override void Reset() {
			base.Reset();
			AllowChangeActiveView = true;
			AutoAdjustMode = true;
		}
#if !SL
		#region Frezable implementation
		protected override Freezable CreateInstanceCore() {
			return new SchedulerOptionsRangeControl();
		}
		#endregion
#endif
	}
}
