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
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[Flags]
	public enum PivotGridBestFitMode { None = 0, FieldValue = 1, FieldHeader = 2, Cell = 4, Default = FieldValue | FieldHeader | Cell };
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum CopyCollapsedValuesMode { DuplicateCollapsedValues, PreserveCollapsedLevels, RemoveCollapsedLevels };
	public enum CopyMultiSelectionMode { IncludeIntermediateColumnsAndRows, DiscardIntermediateColumnsAndRows };
	public class PivotGridOptionsBehaviorBase : PivotGridOptionsBase {
		public const int DefaultLoadingPanelDelay = 300;
		bool copyToClipboardWithFieldValues;
		bool copyToClipboardRemoveEmptyLines;
		CopyMultiSelectionMode clipboardCopyMultiSelectionMode;
		CopyCollapsedValuesMode clipboardCopyCollapsedValuesMode;
		bool useAsyncMode;
		int loadingPanelDelay;
		PivotSortBySummaryOrder sortBySummaryDefaultOrder;
		PivotGridBestFitMode bestFitMode;
		public PivotGridOptionsBehaviorBase(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.copyToClipboardWithFieldValues = false;
			this.copyToClipboardRemoveEmptyLines = false;
			this.clipboardCopyMultiSelectionMode = CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows;
			this.clipboardCopyCollapsedValuesMode = CopyCollapsedValuesMode.PreserveCollapsedLevels;
			this.useAsyncMode = false;
			this.sortBySummaryDefaultOrder = PivotSortBySummaryOrder.Default;
			this.loadingPanelDelay = DefaultLoadingPanelDelay;
			this.bestFitMode = PivotGridBestFitMode.Default;
		}
		internal bool CopyToClipboardRemoveEmptyLines {
			get { return copyToClipboardRemoveEmptyLines; }
			set { copyToClipboardRemoveEmptyLines = value; }
		}
		[
		Browsable(false),
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseClipboardCopyMultiSelectionMode"),
#endif
		DefaultValue(CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows), XtraSerializableProperty()
		]
		public CopyMultiSelectionMode ClipboardCopyMultiSelectionMode {
			get { return clipboardCopyMultiSelectionMode; }
			set { clipboardCopyMultiSelectionMode = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseCopyToClipboardWithFieldValues"),
#endif
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool CopyToClipboardWithFieldValues {
			get { return copyToClipboardWithFieldValues; }
			set { copyToClipboardWithFieldValues = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseClipboardCopyCollapsedValuesMode"),
#endif
		DefaultValue(CopyCollapsedValuesMode.PreserveCollapsedLevels), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public CopyCollapsedValuesMode ClipboardCopyCollapsedValuesMode {
			get { return clipboardCopyCollapsedValuesMode; }
			set { clipboardCopyCollapsedValuesMode = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseUseAsyncMode"),
#endif
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool UseAsyncMode {
			set { useAsyncMode = value; }
			get { return useAsyncMode; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseLoadingPanelDelay"),
#endif
		DefaultValue(DefaultLoadingPanelDelay), XtraSerializableProperty()
		]
		public int LoadingPanelDelay {
			set { loadingPanelDelay = value; }
			get { return loadingPanelDelay; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseSortBySummaryDefaultOrder"),
#endif
		DefaultValue(PivotSortBySummaryOrder.Default), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public PivotSortBySummaryOrder SortBySummaryDefaultOrder {
			get { return sortBySummaryDefaultOrder; }
			set { sortBySummaryDefaultOrder = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsBehaviorBaseBestFitMode"),
#endif
#if !SL && !DXPORTABLE
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		DefaultValue(PivotGridBestFitMode.Default), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public PivotGridBestFitMode BestFitMode {
			get { return bestFitMode; }
			set {
				bestFitMode = value;
				OnOptionsChanged();
			}
		}
		protected internal bool IsBestFitEnabled(PivotGridBestFitMode mode) {
			return (BestFitMode & mode) == mode;
		}
	}
}
