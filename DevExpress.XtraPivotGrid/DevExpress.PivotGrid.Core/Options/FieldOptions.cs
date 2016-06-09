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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridFieldOptions : PivotGridOptionsBase {
		DefaultBoolean allowSort;
		DefaultBoolean allowFilter;
		DefaultBoolean allowFilterBySummary;
		DefaultBoolean allowDrag;
		DefaultBoolean allowDragInCustomizationForm;
		DefaultBoolean allowExpand;
		DefaultBoolean allowSortBySummary;
		bool olapUseNonEmpty;
		DefaultBoolean olapFilterByUniqueName;
		bool allowRunTimeSummaryChange;
		bool showInCustomizationForm;
		bool showInPrefilter;
		bool showInExpressionEditor;
		bool showGrandTotal;
		bool showTotals;
		bool showValues;
		bool showCustomTotals;
		bool showSummaryTypeName;
		bool hideEmptyVariationItems;
		PivotGroupFilterMode? groupFilterMode;
		PivotOLAPFilterUsingWhereClause olapFilterUsingWhereClause;
		public PivotGridFieldOptions(PivotOptionsChangedEventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(null, viewBagOwner, objectPath) {
			this.OptionsChanged = optionsChanged;
			this.olapUseNonEmpty = true;
			this.olapFilterByUniqueName = DefaultBoolean.Default;
			this.allowSort = DefaultBoolean.Default;
			this.allowFilter = DefaultBoolean.Default;
			this.allowFilterBySummary = DefaultBoolean.Default;
			this.allowDrag = DefaultBoolean.Default;
			this.allowDragInCustomizationForm = DefaultBoolean.Default;
			this.allowExpand = DefaultBoolean.Default;
			this.allowSortBySummary = DefaultBoolean.Default;
			this.allowRunTimeSummaryChange = false;
			this.showInCustomizationForm = true;
			this.showInPrefilter = true;
			this.showInExpressionEditor = true;
			this.showGrandTotal = true;
			this.showTotals = true;
			this.showValues = true;
			this.showCustomTotals = true;
			this.showSummaryTypeName = false;
			this.hideEmptyVariationItems = false;
			this.groupFilterMode = null;
			this.olapFilterUsingWhereClause = PivotOLAPFilterUsingWhereClause.SingleValuesOnly;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowSort"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSort")]
		public DefaultBoolean AllowSort {
			get { return allowSort; }
			set {
				if(value == AllowSort) return;
				allowSort = value;
				OnOptionsChanged(FieldOptions.AllowSort);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowFilter"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowFilter")]
		public DefaultBoolean AllowFilter {
			get { return allowFilter; }
			set {
				if(value == AllowFilter) return;
				allowFilter = value;
				OnOptionsChanged(FieldOptions.AllowFilter);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowFilterBySummary"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowFilterBySummary")]
		public DefaultBoolean AllowFilterBySummary {
			get { return allowFilterBySummary; }
			set {
				if(value == AllowFilterBySummary) return;
				allowFilterBySummary = value;
				OnOptionsChanged(FieldOptions.AllowFilterBySummary);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowDrag"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowDrag")]
		public DefaultBoolean AllowDrag {
			get { return allowDrag; }
			set {
				if(value == AllowDrag) return;
				allowDrag = value;
				OnOptionsChanged(FieldOptions.AllowDrag);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowDragInCustomizationForm"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowDragInCustomizationForm")]
		public DefaultBoolean AllowDragInCustomizationForm {
			get { return allowDragInCustomizationForm; }
			set {
				if(value == AllowDragInCustomizationForm) return;
				allowDragInCustomizationForm = value;
				OnOptionsChanged(FieldOptions.AllowDragInCustomizationForm);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowExpand"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowExpand")]
		public DefaultBoolean AllowExpand {
			get { return allowExpand; }
			set {
				if(value == AllowExpand) return;
				allowExpand = value;
				OnOptionsChanged(FieldOptions.AllowExpand);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowSortBySummary"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowSortBySummary")]
		public DefaultBoolean AllowSortBySummary {
			get { return allowSortBySummary; }
			set {
				if(value == AllowSortBySummary) return;
				allowSortBySummary = value;
				OnOptionsChanged(FieldOptions.AllowSortBySummary);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsAllowRunTimeSummaryChange"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.AllowRunTimeSummaryChange"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool AllowRunTimeSummaryChange {
			get { return allowRunTimeSummaryChange; }
			set {
				if(value == AllowRunTimeSummaryChange) return;
				allowRunTimeSummaryChange = value;
				OnOptionsChanged(FieldOptions.AllowRunTimeSummaryChange);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowInCustomizationForm"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInCustomizationForm"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ShowInCustomizationForm {
			get { return showInCustomizationForm; }
			set {
				if(value == ShowInCustomizationForm) return;
				showInCustomizationForm = value;
				OnOptionsChanged(FieldOptions.ShowInCustomizationForm);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowInPrefilter"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInPrefilter"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ShowInPrefilter {
			get { return showInPrefilter; }
			set {
				if(value == showInPrefilter) return;
				showInPrefilter = value;
				OnOptionsChanged(FieldOptions.ShowInPrefilter);
			}
		}		
		[
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowInExpressionEditor"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ShowInExpressionEditor {
			get { return showInExpressionEditor; }
			set { showInExpressionEditor = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowGrandTotal"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowGrandTotal"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowGrandTotal {
			get { return showGrandTotal; }
			set {
				if(value == showGrandTotal) return;
				showGrandTotal = value;
				OnOptionsChanged(FieldOptions.ShowGrandTotal);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowTotals {
			get { return showTotals; }
			set {
				if(value == showTotals) return;
				showTotals = value;
				OnOptionsChanged(FieldOptions.ShowTotals);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowValues"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowValues {
			get { return showValues; }
			set {
				if(value == showValues) return;
				showValues = value;
				OnOptionsChanged(FieldOptions.ShowValues);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowCustomTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowCustomTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowCustomTotals {
			get { return showCustomTotals; }
			set {
				if(value == showCustomTotals) return;
				showCustomTotals = value;
				OnOptionsChanged(FieldOptions.ShowCustomTotals);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsShowSummaryTypeName"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.ShowSummaryTypeName"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowSummaryTypeName {
			get { return showSummaryTypeName; }
			set {
				if(value == showSummaryTypeName) return;
				showSummaryTypeName = value;
				OnOptionsChanged(FieldOptions.ShowSummaryTypeName);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsHideEmptyVariationItems"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.HideEmptyVariationItems"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HideEmptyVariationItems {
			get { return hideEmptyVariationItems; }
			set {
				if(value == hideEmptyVariationItems) return;
				hideEmptyVariationItems = value;
				OnOptionsChanged(FieldOptions.HideEmptyVariationItems);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsGroupFilterMode"),
#endif
		DefaultValue(null), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.GroupFilterMode"),
#if SL || DXPORTABLE
		TypeConverter(typeof(EnumTypeConverter))
#else
		TypeConverter(typeof(NullableConverter))
#endif
		]
		public PivotGroupFilterMode? GroupFilterMode {
			get { return groupFilterMode; }
			set {
				if(value == groupFilterMode) return;
				groupFilterMode = value;
				OnOptionsChanged(FieldOptions.GroupFilterMode);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsOLAPFilterUsingWhereClause"),
#endif
		DefaultValue(PivotOLAPFilterUsingWhereClause.SingleValuesOnly), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPFilterUsingWhereClause"),
		TypeConverter(typeof(EnumTypeConverter))
		]
		public PivotOLAPFilterUsingWhereClause OLAPFilterUsingWhereClause {
			get { return olapFilterUsingWhereClause; }
			set {
				if(value == olapFilterUsingWhereClause) return;
				olapFilterUsingWhereClause = value;
				OnOptionsChanged(FieldOptions.OLAPFilterUsingWhereClause);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsOLAPUseNonEmpty"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPUseNonEmpty"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool OLAPUseNonEmpty {
			get { return olapUseNonEmpty; }
			set {
				if(value == olapUseNonEmpty) return;
				olapUseNonEmpty = value;
				OnOptionsChanged(FieldOptions.OLAPUseNonEmpty);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldOptionsOLAPFilterByUniqueName"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldOptions.OLAPFilterByUniqueName"),
		]
		public DefaultBoolean OLAPFilterByUniqueName {
			get { return olapFilterByUniqueName; }
			set {
				if(value == olapFilterByUniqueName)
					return;
				olapFilterByUniqueName = value;
				OnOptionsChanged(FieldOptions.OLAPFilterByUniqueName);
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new event PivotOptionsChangedEventHandler OptionsChanged;
		protected void OnOptionsChanged(FieldOptions option) {
			if(OptionsChanged != null) OptionsChanged(this, new PivotOptionsChangedEventArgs(option));
		}
		protected override void OnOptionsChanged() {
			throw new ArgumentException("Please specify the option changed");
		}
	}
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotOLAPFilterUsingWhereClause {
		SingleValuesOnly,
		Always,
		Never,
		Auto
	}
	public enum FieldOptions {
		OLAPUseNonEmpty,
		OLAPFilterByUniqueName,
		AllowDrag,
		AllowDragInCustomizationForm,
		AllowExpand,
		AllowFilter,
		AllowFilterBySummary,
		AllowRunTimeSummaryChange,
		AllowSort,
		AllowSortBySummary,
		GroupFilterMode,
		HideEmptyVariationItems,
		OLAPFilterUsingWhereClause,
		ShowCustomTotals,
		ShowGrandTotal,
		ShowInCustomizationForm,
		ShowInPrefilter,
		ShowSummaryTypeName,
		ShowTotals,
		ShowValues,
		AllowEdit,
		ReadOnly,
		ShowButtonMode,
		ShowUnboundExpressionMenu,
		IsFilterRadioMode,
		AllowUnboundExpressionEditor
	}
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum UnboundExpressionMode {
		Default,
		UseSummaryValues,
		DataSource,
		UseAggregateFunctions
	};
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum TopValueMode {
		Default,
		AllValues,
		ParentFieldValues
	};
	public class PivotOptionsChangedEventArgs : EventArgs {
		FieldOptions option;		
		public PivotOptionsChangedEventArgs(FieldOptions option) {
			this.option = option;
		}
		public FieldOptions Option { get { return option; } }
	}
	public delegate void PivotOptionsChangedEventHandler(object sender, PivotOptionsChangedEventArgs e);
}
