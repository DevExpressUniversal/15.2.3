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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsChartDataSource : PivotGridOptionsChartDataSourceBase {
		public const int DefaultUpdateDelay = 300;
		bool selectionOnly;
		int updateDelay;
		public PivotGridOptionsChartDataSource(PivotGridData data) : base(data) {
			this.selectionOnly = true;
			this.updateDelay = DefaultUpdateDelay;
		}
		protected bool SelectionOnlyInternal {
			get { return selectionOnly; }
			set { selectionOnly = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceSelectionOnly"),
#endif
		DefaultValue(true), XtraSerializableProperty()
		]
		public virtual bool SelectionOnly {
			get { return selectionOnly; }
			set {
				if(selectionOnly == value) return;
				selectionOnly = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceUpdateDelay"),
#endif
		DefaultValue(DefaultUpdateDelay), XtraSerializableProperty()
		]
		public virtual int UpdateDelay {
			get { return updateDelay; }
			set {
				if(updateDelay == value) return;
				updateDelay = value < 0 ? 0 : value;
				OnOptionsChanged();
			}
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridOptionsChartDataSourceBase : PivotGridOptionsBase, IXtraSerializableLayoutEx {
		public const int DefaultMaxAllowedSeriesCount = 10;
		public const int DefaultMaxAllowedPointCountInSeries = 100;
		internal const PivotChartFieldValuesProvideMode DefaultFieldValuesProvideMode = PivotChartFieldValuesProvideMode.Default;
		internal const PivotChartDataProvideMode DefaultDataProvideMode = PivotChartDataProvideMode.ProvideLastLevelData;
		internal const PivotChartDataProvidePriority DefaultDataProvidePriority = PivotChartDataProvidePriority.Rows;
		internal const bool DefaultProvideColumnGrandTotals = false;
		internal const bool DefaultProvideColumnTotals = false;
		internal const bool DefaultProvideColumnCustomTotals = false;
		internal const bool DefaultProvideRowGrandTotals = false;
		internal const bool DefaultProvideRowTotals = false;
		internal const bool DefaultProvideRowCustomTotals = false;
		internal const bool DefaultAutoTransposeChart = false;
		PivotGridData data;
		internal bool provideDataByColumns,
			provideColumnGrandTotals, provideRowGrandTotals,
			provideColumnTotals, provideRowTotals,
			provideColumnCustomTotals, provideRowCustomTotals;
		PivotChartFieldValuesProvideMode fieldValuesProvideMode;
		PivotChartDataProvideMode dataProvideMode;
		PivotChartDataProvidePriority dataProvidePriority;
		Type provideColumnFieldValuesAsType;
		Type provideRowFieldValuesAsType;
		Type provideCellValuesAsType;
		bool provideEmptyCells;
		bool provideDataFieldsOnSeries;
		int maxAllowedSeriesCount;
		int maxAllowedPointCountInSeries;
		bool autoTransposeChart;
		protected PivotGridData Data { get { return data; } }
		public PivotGridOptionsChartDataSourceBase(PivotGridData data) {
			this.data = data;
			provideDataByColumns = true;
			provideColumnGrandTotals = DefaultProvideColumnGrandTotals;
			provideRowGrandTotals = DefaultProvideRowGrandTotals;
			provideColumnTotals = DefaultProvideColumnTotals;
			provideRowTotals = DefaultProvideRowTotals;
			provideColumnCustomTotals = DefaultProvideColumnCustomTotals;
			provideRowCustomTotals = DefaultProvideRowCustomTotals;
			fieldValuesProvideMode = DefaultFieldValuesProvideMode;
			dataProvidePriority = DefaultDataProvidePriority;
			dataProvideMode = DefaultDataProvideMode;
			provideColumnFieldValuesAsType = null;
			provideRowFieldValuesAsType = null;
			provideCellValuesAsType = null;
			provideEmptyCells = true;
			provideDataFieldsOnSeries = false;
			maxAllowedSeriesCount = DefaultMaxAllowedSeriesCount;
			maxAllowedPointCountInSeries = DefaultMaxAllowedPointCountInSeries;
			autoTransposeChart = DefaultAutoTransposeChart;
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions options) {
			try {
				options.BeginUpdate();
				base.Assign(options);
				PivotGridOptionsChartDataSourceBase opts = options as PivotGridOptionsChartDataSourceBase;
				if(opts == null)
					return;
				ProvideDataByColumns = opts.ProvideDataByColumns;
				FieldValuesProvideMode = opts.FieldValuesProvideMode;
				DataProvideMode = opts.DataProvideMode;
				DataProvidePriority = opts.DataProvidePriority;
				ProvideColumnFieldValuesAsType = opts.ProvideColumnFieldValuesAsType;
				ProvideRowFieldValuesAsType = opts.ProvideRowFieldValuesAsType;
				ProvideCellValuesAsType = opts.ProvideCellValuesAsType;
				ProvideEmptyCells = opts.ProvideEmptyCells;
				ProvideDataFieldsOnSeries = opts.ProvideDataFieldsOnSeries;
				ProvideColumnGrandTotals = opts.ProvideColumnGrandTotals;
				ProvideRowGrandTotals = opts.ProvideRowGrandTotals;
				ProvideColumnTotals = opts.ProvideColumnTotals;
				ProvideRowTotals = opts.ProvideRowTotals;
				ProvideColumnCustomTotals = opts.ProvideColumnCustomTotals;
				ProvideRowCustomTotals = opts.ProvideRowCustomTotals;
				MaxAllowedSeriesCount = opts.MaxAllowedSeriesCount;
				MaxAllowedPointCountInSeries = opts.MaxAllowedPointCountInSeries;
				AutoTransposeChart = opts.AutoTransposeChart;
			} finally {
				options.EndUpdate();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideDataByColumns"),
#endif
		DefaultValue(true), NotifyParentProperty(true), XtraSerializableProperty(-100)
		]
		public bool ProvideDataByColumns {
			get { return provideDataByColumns; }
			set {
				if(provideDataByColumns == value) return;
				provideDataByColumns = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseFieldValuesProvideMode"),
#endif
		DefaultValue(DefaultFieldValuesProvideMode), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual PivotChartFieldValuesProvideMode FieldValuesProvideMode {
			get { return fieldValuesProvideMode; }
			set {
				if(fieldValuesProvideMode == value) return;
				fieldValuesProvideMode = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseDataProvideMode"),
#endif
		DefaultValue(DefaultDataProvideMode), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual PivotChartDataProvideMode DataProvideMode {
			get { return dataProvideMode; }
			set {
				if(dataProvideMode == value)
					return;
				dataProvideMode = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseDataProvidePriority"),
#endif
		DefaultValue(DefaultDataProvidePriority), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual PivotChartDataProvidePriority DataProvidePriority {
			get { return dataProvidePriority; }
			set {
				if(dataProvidePriority == value)
					return;
				dataProvidePriority = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideColumnFieldValuesAsType"),
#endif
		DefaultValue(null), Browsable(false), XtraSerializableProperty(-100), NotifyParentProperty(true),
		]
		public virtual Type ProvideColumnFieldValuesAsType {
			get { return provideColumnFieldValuesAsType; }
			set {
				if(provideColumnFieldValuesAsType == value) return;
				provideColumnFieldValuesAsType = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideRowFieldValuesAsType"),
#endif
		DefaultValue(null), Browsable(false), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual Type ProvideRowFieldValuesAsType {
			get { return provideRowFieldValuesAsType; }
			set {
				if(provideRowFieldValuesAsType == value) return;
				provideRowFieldValuesAsType = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideCellValuesAsType"),
#endif
		DefaultValue(null), Browsable(false), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual Type ProvideCellValuesAsType {
			get { return provideCellValuesAsType; }
			set {
				if(provideCellValuesAsType == value) return;
				provideCellValuesAsType = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideEmptyCells"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual bool ProvideEmptyCells {
			get { return provideEmptyCells; }
			set {
				if(provideEmptyCells == value) return;
				provideEmptyCells = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideDataFieldsOnSeries"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual bool ProvideDataFieldsOnSeries {
			get { return provideDataFieldsOnSeries; }
			set {
				if(provideDataFieldsOnSeries == value) return;
				provideDataFieldsOnSeries = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideColumnGrandTotals"),
#endif
		DefaultValue(DefaultProvideColumnGrandTotals), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideColumnGrandTotals {
			get { return provideColumnGrandTotals; }
			set {
				if(provideColumnGrandTotals == value) return;
				provideColumnGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideRowGrandTotals"),
#endif
		DefaultValue(DefaultProvideRowGrandTotals), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideRowGrandTotals {
			get { return provideRowGrandTotals; }
			set {
				if(provideRowGrandTotals == value) return;
				provideRowGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideColumnTotals"),
#endif
		DefaultValue(DefaultProvideColumnTotals),
		XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideColumnTotals {
			get { return provideColumnTotals; }
			set {
				if(provideColumnTotals == value) return;
				provideColumnTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideRowTotals"),
#endif
		DefaultValue(DefaultProvideRowTotals), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideRowTotals {
			get { return provideRowTotals; }
			set {
				if(provideRowTotals == value) return;
				provideRowTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideColumnCustomTotals"),
#endif
		DefaultValue(DefaultProvideColumnCustomTotals), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideColumnCustomTotals {
			get { return provideColumnCustomTotals; }
			set {
				if(provideColumnCustomTotals == value) return;
				provideColumnCustomTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseProvideRowCustomTotals"),
#endif
		DefaultValue(DefaultProvideRowCustomTotals), XtraSerializableProperty(-100), NotifyParentProperty(true)
		]
		public virtual bool ProvideRowCustomTotals {
			get { return provideRowCustomTotals; }
			set {
				if(provideRowCustomTotals == value) return;
				provideRowCustomTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseMaxAllowedSeriesCount"),
#endif
		DefaultValue(DefaultMaxAllowedSeriesCount), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual int MaxAllowedSeriesCount {
			get { return maxAllowedSeriesCount; }
			set {
				if(value <= 0) value = 0;
				if(maxAllowedSeriesCount == value) return;
				maxAllowedSeriesCount = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseMaxAllowedPointCountInSeries"),
#endif
		DefaultValue(DefaultMaxAllowedPointCountInSeries), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual int MaxAllowedPointCountInSeries {
			get { return maxAllowedPointCountInSeries; }
			set {
				if(value <= 0) value = 0;
				if(maxAllowedPointCountInSeries == value) return;
				maxAllowedPointCountInSeries = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsChartDataSourceBaseAutoTransposeChart"),
#endif
		DefaultValue(DefaultAutoTransposeChart), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AutoTransposeChart {
			get { return autoTransposeChart; }
			set {
				if(autoTransposeChart == value)
					return;
				autoTransposeChart = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false)]
		public virtual bool ShouldRemoveTotals {
			get {
				return FieldValuesProvideMode == PivotChartFieldValuesProvideMode.Value || !ProvideRowTotals || !ProvideColumnTotals ||
				  !ProvideRowCustomTotals || !ProvideColumnCustomTotals || !ProvideRowGrandTotals || !ProvideColumnGrandTotals;
			}
		}
		internal bool ContainsAnyRowTotals {
			get {
				return ProvideRowTotals || ProvideRowCustomTotals || ProvideRowGrandTotals;
			}
		}
		internal bool ContainsAnyColumnTotals {
			get {
				return ProvideColumnTotals || ProvideColumnCustomTotals || ProvideColumnGrandTotals;
			}
		}
#if DEBUGTEST
		public void ProvideAllTotals(bool show) {
			BeginUpdate();
			ProvideRowTotals = show;
			ProvideColumnTotals = show;
			ProvideRowCustomTotals = show;
			ProvideColumnCustomTotals = show;
			ProvideRowGrandTotals = show;
			ProvideColumnGrandTotals = show;
			EndUpdate();
		}
#endif
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(Data.IsDeserializing)
				return true;
			return !PivotGridData.IsPropertyObsolete(propertyName);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			this.Reset();
		}
		#endregion
	}
}
