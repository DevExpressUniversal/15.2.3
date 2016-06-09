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
using System.Text;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
using System.Drawing.Design;
using DevExpress.XtraScheduler.Native;
using System.Collections;
namespace DevExpress.XtraScheduler {
	#region ScaleBasedRangeControlClientOptions
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class ScaleBasedRangeControlClientOptions : SchedulerNotificationOptions, IScaleBasedRangeControlClientOptions {
		internal const int DefaultMinIntervalWidth = 30;
		internal const int DefaultMaxIntervalWidth = 250;
		SchedulerOptionsErrorProvider errorProvider;
		RangeControlDataDisplayType dataDisplayType = RangeControlDataDisplayType.Auto;
		TimeScaleCollection scales;
		DateTime rangeMinimum = DateTime.MinValue;
		DateTime rangeMaximum = DateTime.MaxValue;
		bool autoFormatScaleCaptions;
		int maxSelectedIntervalCount;
		int thumbnailHeight;
		int minIntervalWidth;
		int maxIntervalWidth;
		#region Scales
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsScales"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public TimeScaleCollection Scales
		{
			get { return scales; }
		}
		internal bool ShouldSerializeScales() {
			return !Scales.HasDefaultContent();
		}
		internal void ResetScales() {
			Scales.LoadDefaults();
		}
		#endregion
		#region AutoFormatScaleCaptions
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsAutoFormatScaleCaptions"),
#endif
		DefaultValue(true)]
		public bool AutoFormatScaleCaptions
		{
			get
			{
				return autoFormatScaleCaptions;
			}
			set
			{
				if (autoFormatScaleCaptions == value)
					return;
				autoFormatScaleCaptions = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFormatScaleCaptions", !autoFormatScaleCaptions, autoFormatScaleCaptions));
			}
		}
		#endregion
		#region MinIntervalWidth
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsMinIntervalWidth"),
#endif
		DefaultValue(DefaultMinIntervalWidth)]
		public int MinIntervalWidth
		{
			get
			{
				return minIntervalWidth;
			}
			set
			{
				if (minIntervalWidth == value)
					return;
				int oldVal = minIntervalWidth;
				minIntervalWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("MinIntervalWidth", oldVal, value));
			}
		}
		#endregion
		#region MaxIntervalWidth
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsMaxIntervalWidth"),
#endif
		DefaultValue(DefaultMaxIntervalWidth)]
		public int MaxIntervalWidth
		{
			get
			{
				return maxIntervalWidth;
			}
			set
			{
				if (maxIntervalWidth == value)
					return;
				int oldVal = maxIntervalWidth;
				maxIntervalWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxIntervalWidth", oldVal, value));
			}
		}
		#endregion
		#region MaxSelectedIntervalCount
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsMaxSelectedIntervalCount"),
#endif
		DefaultValue(0)]
		public int MaxSelectedIntervalCount
		{
			get
			{
				return maxSelectedIntervalCount;
			}
			set
			{
				if (maxSelectedIntervalCount == value)
					return;
				int oldVal = maxSelectedIntervalCount;
				maxSelectedIntervalCount = value;
				OnChanged(new BaseOptionChangedEventArgs("MaxSelectedIntervalCount", oldVal, value));
			}
		}
		#endregion
		#region DataDisplayType
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsDataDisplayType"),
#endif
		DefaultValue(RangeControlDataDisplayType.Auto)]
		public RangeControlDataDisplayType DataDisplayType
		{
			get { return dataDisplayType; }
			set
			{
				if (dataDisplayType == value)
					return;
				RangeControlDataDisplayType oldVal = dataDisplayType;
				dataDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("DataDisplayType", oldVal, value));
			}
		}
		#endregion
		#region ThumbnailHeight
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsThumbnailHeight"),
#endif
		DefaultValue(0)]
		public int ThumbnailHeight
		{
			get { return thumbnailHeight; }
			set
			{
				if (thumbnailHeight == value)
					return;
				int oldVal = thumbnailHeight;
				thumbnailHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("ThumbnailHeight", oldVal, value));
			}
		}
		#endregion
		#region RangeMinimum
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsRangeMinimum")]
#endif
		public DateTime RangeMinimum {
			get { return rangeMinimum; }
			set {
				if (rangeMinimum == value)
					return;
				if (value > RangeMaximum) {
					NotifyError("RangeMinimum", String.Format(ExceptionMessages.SRWrongMinValueArgument, value, "RangeMinimum", "RangeMaximum"));
					SetRangeMaximumCore(value);
				}
				SetRangeMinimumCore(value);
			}
		}
		void SetRangeMinimumCore(DateTime value) {
			DateTime oldValue = RangeMinimum;
			rangeMinimum = value;
			OnChanged(new BaseOptionChangedEventArgs("RangeMinimum", oldValue, rangeMinimum));
		}
		internal bool ShouldSerializeRangeMinimum() {
			return RangeMinimum != ScaleBasedRangeControlClient.DefaultRangeMinimum;
		}
		#endregion
		#region RangeMaximum
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ScaleBasedRangeControlClientOptionsRangeMaximum")]
#endif
		public DateTime RangeMaximum {
			get { return rangeMaximum; }
			set {
				if (rangeMaximum == value)
					return;
				if (value < RangeMinimum) {
					NotifyError("RangeMaximum", String.Format(ExceptionMessages.SRWrongMaxValueArgument, value, "RangeMaximum", "RangeMinimum"));
					SetRangeMinimumCore(value);
				}
				SetRangeMaximumCore(value);
			}
		}
		void SetRangeMaximumCore(DateTime value) {
			DateTime oldRangeMaximum = rangeMaximum;
			rangeMaximum = value;
			OnChanged(new BaseOptionChangedEventArgs("RangeMaximum", oldRangeMaximum, RangeMaximum));
		}
		internal bool ShouldSerializeRangeMaximum() {
			return RangeMaximum != ScaleBasedRangeControlClient.DefaultRangeMaximum;
		}
		#endregion
		event BaseOptionChangedEventHandler IScaleBasedRangeControlClientOptions.Changed {
			add { Changed += value; }
			remove { Changed -= value; }
		}
		protected internal override void ResetCore() {
			if (scales == null) {
				this.scales = CreateTimeScaleCollection();
			}
			Scales.LoadDefaults();
			RangeMinimum = ScaleBasedRangeControlClient.DefaultRangeMinimum;
			RangeMaximum = ScaleBasedRangeControlClient.DefaultRangeMaximum;
			DataDisplayType = RangeControlDataDisplayType.Auto;
			ThumbnailHeight = 0;
			AutoFormatScaleCaptions = true;
			MaxSelectedIntervalCount = 0;
			MinIntervalWidth = DefaultMinIntervalWidth;
			MaxIntervalWidth = DefaultMaxIntervalWidth;
		}
		protected internal virtual TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
		protected internal virtual void AttachErrorProvider(SchedulerOptionsErrorProvider errorProvider) {
			this.errorProvider = errorProvider;
		}
		protected internal virtual void DetachErrorProvider() {
			this.errorProvider = null;
		}
		protected void NotifyError(string propName, string message) {
			if (this.errorProvider == null)
				return;
			this.errorProvider.NotifyError(propName, message);
		}
	}
	#endregion
}
