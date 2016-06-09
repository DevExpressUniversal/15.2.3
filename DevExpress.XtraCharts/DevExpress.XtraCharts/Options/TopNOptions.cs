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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum TopNMode {
		Count,
		ThresholdValue,
		ThresholdPercent
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(TopNOptionsTypeConverter))
	]
	public class TopNOptions : ChartElement {
		internal const string SharedOptionsProperty = "PointsFilterOptions";
		const TopNMode DefaultMode = TopNMode.Count;
		const int DefaultCount = 5;
		const double DefaultThresholdValue = 100.0;
		const double DefaultThresholdPercent = 10.0;
		const bool DefaultEnabled = false;
		const bool DefaultShowOthers = true;
		bool enabled = DefaultEnabled;
		TopNMode mode = DefaultMode;
		int count = DefaultCount;
		double thresholdValue = DefaultThresholdValue;
		double thresholdPercent = DefaultThresholdPercent;
		bool showOthers = DefaultShowOthers;
		string othersArgument = String.Empty;
		SeriesBase SeriesBase { get { return (SeriesBase)Owner; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.Enabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Enabled {
			get { return enabled; }
			set {
				if (value != enabled) {
					if(value && SeriesBase != null && !SeriesBase.ShouldApplyTopNOptions)
						throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedTopNOptions));
					SendNotification(new ElementWillChangeNotification(this));
					enabled = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.Mode"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public TopNMode Mode {
			get { return mode; }
			set {
				if (value != mode) {
					SendNotification(new ElementWillChangeNotification(this));
					mode = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.Count"),
		XtraSerializableProperty
		]
		public int Count {
			get { return count; }
			set {
				if (value != count) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTopNCount));
					SendNotification(new ElementWillChangeNotification(this));
					count = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsThresholdValue"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.ThresholdValue"),
		XtraSerializableProperty
		]
		public double ThresholdValue {
			get { return thresholdValue; }
			set {
				if (value != thresholdValue) {
					if (value <= 0.0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTopNThresholdValue));
					SendNotification(new ElementWillChangeNotification(this));
					thresholdValue = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsThresholdPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.ThresholdPercent"),
		XtraSerializableProperty
		]
		public double ThresholdPercent {
			get { return thresholdPercent; }
			set {
				if (value != thresholdPercent) {
					if (value <= 0.0 || value > 100.0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTopNThresholdPercent));
					SendNotification(new ElementWillChangeNotification(this));
					thresholdPercent = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsShowOthers"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.ShowOthers"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool ShowOthers {
			get { return showOthers; }
			set {
				if (value != showOthers) {
					SendNotification(new ElementWillChangeNotification(this));
					showOthers = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TopNOptionsOthersArgument"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TopNOptions.OthersArgument"),
		XtraSerializableProperty
		]
		public string OthersArgument {
			get { return othersArgument; }
			set {
				if (value != othersArgument) {
					SendNotification(new ElementWillChangeNotification(this));
					othersArgument = value;
					RaiseControlChanged(new PropertyUpdateInfo(Owner, SharedOptionsProperty)); 
				}
			}
		}
		internal TopNOptions(SeriesBase seriesBase) : base(seriesBase) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeEnabled() {
			return enabled != DefaultEnabled;
		}
		void ResetEnabled() {
			Enabled = DefaultEnabled;
		}
		bool ShouldSerializeMode() {
			return mode != DefaultMode;
		}
		void ResetMode() {
			Mode = DefaultMode;
		}
		bool ShouldSerializeCount() {
			return count != DefaultCount;
		}
		void ResetCount() {
			Count = DefaultCount;
		}
		bool ShouldSerializeThresholdValue() {
			return thresholdValue != DefaultThresholdValue;
		}
		void ResetThresholdValue() {
			ThresholdValue = DefaultThresholdValue;
		}
		bool ShouldSerializeThresholdPercent() {
			return thresholdPercent != DefaultThresholdPercent;
		}
		void ResetThresholdPercent() {
			ThresholdPercent = DefaultThresholdPercent;
		}
		bool ShouldSerializeShowOthers() {
			return showOthers != DefaultShowOthers;
		}
		void ResetShowOthers() {
			ShowOthers = DefaultShowOthers;
		}
		bool ShouldSerializeOthersArgument() {
			return !String.IsNullOrEmpty(othersArgument);
		}
		void ResetOthersArgument() {
			OthersArgument = string.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeEnabled() || ShouldSerializeMode() ||
				ShouldSerializeCount() || ShouldSerializeThresholdValue() || ShouldSerializeThresholdPercent() ||
				ShouldSerializeShowOthers() || ShouldSerializeOthersArgument();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Enabled":
					return ShouldSerializeEnabled();
				case "Mode":
					return ShouldSerializeMode();
				case "Count":
					return ShouldSerializeCount();
				case "ThresholdValue":
					return ShouldSerializeThresholdValue();
				case "ThresholdPercent":
					return ShouldSerializeThresholdPercent();
				case "ShowOthers":
					return ShouldSerializeShowOthers();
				case "OthersArgument":
					return ShouldSerializeOthersArgument();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new TopNOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TopNOptions options = obj as TopNOptions;
			if (options != null) {
				enabled = options.enabled;
				mode = options.mode;
				count = options.count;
				thresholdValue = options.thresholdValue;
				thresholdPercent = options.thresholdPercent;
				showOthers = options.showOthers;
				othersArgument = options.othersArgument;
			}
		}
		public override bool Equals(object obj) {
			TopNOptions options = obj as TopNOptions;
			return options != null && enabled == options.enabled && mode == options.mode && count == options.count && 
				thresholdValue == options.thresholdValue && thresholdPercent == options.thresholdPercent &&
				showOthers == options.showOthers && othersArgument == options.othersArgument;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
