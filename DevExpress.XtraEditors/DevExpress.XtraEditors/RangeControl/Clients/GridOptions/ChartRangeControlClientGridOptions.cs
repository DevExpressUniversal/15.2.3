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
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraEditors {
	[TypeConverter(typeof(ChartRangeControlClientGridOptionsTypeConverter))]
	public abstract class ChartRangeControlClientGridOptions {
		#region Nested Classes: ChartCoreCoreGridOptions
		sealed class ChartCoreCoreGridOptions : IChartCoreClientGridOptions {
			readonly ChartRangeControlClientGridOptions gridOptions;
			public ChartCoreCoreGridOptions(ChartRangeControlClientGridOptions gridOptions) {
				this.gridOptions = gridOptions;
			}
			#region IChartCoreClientGridOptions
			bool IChartCoreClientGridOptions.Auto {
				get { return gridOptions.Auto; }
			}
			double IChartCoreClientGridOptions.PixelPerUnit {
				get { return gridOptions.PixelPerUnit; }
			}
			GridUnit IChartCoreClientGridOptions.GridUnit {
				get { return gridOptions.CoreGridUnit; }
			}
			GridUnit IChartCoreClientGridOptions.SnapUnit {
				get { return gridOptions.CoreSnapUnit; }
			}
			IChartCoreClientGridMapping IChartCoreClientGridOptions.GridMapping {
				get { return gridOptions.GridMapping; }
			}
			#endregion
		}
		#endregion
		internal delegate void GridOptionsChangedDelegate();
		const bool DefaultAuto = true;
		const bool DefaultShowGridlinesErrorMessage = true;
		const string DefaultGridErrorMesasge = null;
		protected const double DefaultGridSpacing = 1.0;
		protected const double DefaultSnapSpacing = 1.0;
		readonly ChartCoreCoreGridOptions coreGridOptions;
		bool auto;
		double gridSpacing;
		double snapSpacing;
		bool blockChanges;
		string labelFormat;
		bool showGridlinesErrorMessage;
		IFormatProvider labelFormatProvider;
		protected abstract GridUnit CoreGridUnit { get; }
		protected abstract GridUnit CoreSnapUnit { get; }
		protected abstract IChartCoreClientGridMapping GridMapping { get; }
		protected abstract IFormatProvider LabelFormatProviderInternal { get; }
		protected abstract double PixelPerUnit { get; }
		internal IChartCoreClientGridOptions CoreGridOptions {
			get { return coreGridOptions; }
		}
		internal IFormatProvider ActualLabelFormatProvider {
			get { return (labelFormatProvider == null) ? LabelFormatProviderInternal : labelFormatProvider; }
		}
		internal GridOptionsChangedDelegate Changed { get; set; }
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.Auto"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsAuto"),
#endif
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public bool Auto {
			get { return auto; }
			set {
				if (auto != value) {
					if (!value)
						DisableAuto();
					else
						auto = value;
					RaiseChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.GridSpacing"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsGridSpacing"),
#endif
		XtraSerializableProperty
		]
		public double GridSpacing {
			get { return gridSpacing; }
			set {
				if (auto) {
					DisableAuto();
					gridSpacing = value;
					RaiseChanged();
				} else if (gridSpacing != value) {
					gridSpacing = value;
					RaiseChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.SnapSpacing"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsSnapSpacing"),
#endif
		XtraSerializableProperty
		]
		public double SnapSpacing {
			get { return snapSpacing; }
			set {
				if (auto) {
					DisableAuto();
					snapSpacing = value;
					RaiseChanged();
				} else if (snapSpacing != value) {
					snapSpacing = value;
					RaiseChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.LabelFormat"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsLabelFormat"),
#endif
		XtraSerializableProperty
		]
		public string LabelFormat {
			get {
				return labelFormat;
			}
			set {
				if (labelFormat != value) {
					if (!ValidateLabelFormat(value))
						throw new ArgumentException("Invalid format specifier");
					labelFormat = value;
					RaiseChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.LabelFormatProvider"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsLabelFormatProvider"),
#endif
		XtraSerializableProperty
		]
		public IFormatProvider LabelFormatProvider {
			get {
				return labelFormatProvider;
			}
			set {
				labelFormatProvider = value;
				RaiseChanged();
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientGridOptions.ShowGridlinesErrorMessage"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientGridOptionsShowGridlinesErrorMessage"),
#endif
		XtraSerializableProperty
		]
		public bool ShowGridlinesErrorMessage {
			get { return showGridlinesErrorMessage; }
			set {
				if (showGridlinesErrorMessage != value) {
					showGridlinesErrorMessage = value;
					RaiseChanged();
				}
			}
		}
		public ChartRangeControlClientGridOptions() {
			this.coreGridOptions = new ChartCoreCoreGridOptions(this);
			this.auto = DefaultAuto;
			this.gridSpacing = DefaultGridSpacing;
			this.snapSpacing = DefaultSnapSpacing;
			this.showGridlinesErrorMessage = DefaultShowGridlinesErrorMessage;
			this.blockChanges = false;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeGridSpacing() {
			return ShouldSerializeAuto() && gridSpacing != DefaultGridSpacing;
		}
		bool ShouldSerializeSnapSpacing() {
			return ShouldSerializeAuto() && snapSpacing != DefaultSnapSpacing;
		}
		bool ShouldSerializeLabelFormat() {
			return !string.IsNullOrWhiteSpace(labelFormat);
		}
		bool ShouldSerializeLabelFormatProvider() {
			return (labelFormatProvider != null);
		}
		bool ShouldSerializeShowGridlinesErrorMessage() {
			return (showGridlinesErrorMessage != DefaultShowGridlinesErrorMessage);
		}
		protected bool ShouldSerializeAuto() {
			return auto != DefaultAuto;
		}
		void ResetLabelFormat() {
			labelFormat = string.Empty;
		}
		void ResetLabelFormatProvider() {
			labelFormatProvider = null;
		}
		void ResetGridSpacing() {
			GridSpacing = DefaultGridSpacing;
		}
		void ResetSnapSpacing() {
			SnapSpacing = DefaultSnapSpacing;
		}
		void ResetAuto() {
			Auto = DefaultAuto;
		}
		void ResetShowGridlinesErrorMessage() {
			showGridlinesErrorMessage = DefaultShowGridlinesErrorMessage;
		}
		#endregion
		protected void DisableAuto() {
			auto = false;
			blockChanges = true;
			PushAutoUnitToProperties();
			blockChanges = false;
		}
		protected void RaiseChanged() {
			if ((Changed != null) && !blockChanges)
				Changed();
		}
		protected abstract void PushAutoUnitToProperties();
		protected abstract bool ValidateLabelFormat(string format);
		protected internal abstract object GetNativeGridValue(double value);
		public override string ToString() {
			return "(" + GetType().Name + ")";
		}
	}
}
