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
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum DataFieldUnboundExpressionMode {
		Default,
		UseSummaryValues
	};
	public class PivotGridOptionsData : PivotGridOptionsBase {
		PivotGridData data;
		bool allowCrossGroupVariation;
		bool caseSensitive;
		bool filterByVisibleFieldsOnly;
		int fDrillDownMaxRowCount;
		DefaultBoolean autoExpandGroups = DefaultBoolean.Default;
		DataFieldUnboundExpressionMode dataFieldUnboundExpressionMode;
		ICustomObjectConverter customObjectConverter;
		public PivotGridOptionsData(PivotGridData data, EventHandler optionsChanged)
			: base(optionsChanged) {
			this.data = data;
			this.allowCrossGroupVariation = true;
			if(data != null) {   
				this.caseSensitive = data.CaseSensitive;
				this.autoExpandGroups = GetDefaultBooleanValueWithDefault(data.AutoExpandGroups, DefaultAutoExpandGroupsValue);
			}
			this.fDrillDownMaxRowCount = PivotDrillDownDataSource.AllRows;
		}
		static DefaultBoolean GetDefaultBooleanValue(bool value) {
			if(value)
				return DefaultBoolean.True;
			return DefaultBoolean.False;
		}
		static DefaultBoolean GetDefaultBooleanValueWithDefault(bool value, bool defaultValue) {
			if(value == defaultValue)
				return DefaultBoolean.Default;
			return GetDefaultBooleanValue(value);
		}
		static bool GetBooleanValueFromDefaultBooleanValue(DefaultBoolean value, bool defaultValue) {
			if(value == DefaultBoolean.Default)
				return defaultValue;
			return value == DefaultBoolean.True;
		}
		PivotGridData Data { get { return data; } set { data = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataCaseSensitive"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsData.CaseSensitive"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true),
		XtraSerializableProperty(),
		AutoFormatEnable(),
		NotifyParentProperty(true)
		]
		public bool CaseSensitive {
			get { return Data.CaseSensitive; }
			set {
				caseSensitive = value;
				Data.CaseSensitive = value;
			}
		}
		internal bool CaseSensitiveCore {
			get { return caseSensitive; }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ICustomObjectConverter CustomObjectConverter {
			get { return customObjectConverter; }
			set {
				if(value != customObjectConverter) {
					customObjectConverter = value;
					Data.CustomObjectConverterChanged();
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataAllowCrossGroupVariation"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsData.AllowCrossGroupVariation"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public bool AllowCrossGroupVariation {
			get { return allowCrossGroupVariation; }
			set {
				if(AllowCrossGroupVariation == value) return;
				allowCrossGroupVariation = value;
				Data.ReloadData();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataDrillDownMaxRowCount"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
			DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridOptionsData.DrillDownMaxRowCount"),
		DefaultValue(PivotDrillDownDataSource.AllRows), XtraSerializableProperty(),
		AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public int DrillDownMaxRowCount {
			get { return fDrillDownMaxRowCount; }
			set {
				if(fDrillDownMaxRowCount == value) return;
				if(value != PivotDrillDownDataSource.AllRows && value <= 0)
					value = PivotDrillDownDataSource.AllRows;
				fDrillDownMaxRowCount = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataDataFieldUnboundExpressionMode"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile,
		"DevExpress.XtraPivotGrid.PivotGridOptionsData.DataFieldUnboundExpressionMode"),
		DefaultValue(DataFieldUnboundExpressionMode.Default),
		XtraSerializableProperty(), AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public DataFieldUnboundExpressionMode DataFieldUnboundExpressionMode {
			get { return dataFieldUnboundExpressionMode; }
			set {
				dataFieldUnboundExpressionMode = value;
				Data.ReloadData();
			}
		}
		internal bool IsProcessExpressionOnSummaryLevel {
			get { return dataFieldUnboundExpressionMode == DataFieldUnboundExpressionMode.UseSummaryValues; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFilterByVisibleFieldsOnly"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile,
		"DevExpress.XtraPivotGrid.PivotGridOptionsData.FilterByVisibleFieldsOnly"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(false),
		XtraSerializableProperty(), AutoFormatDisable(),
		NotifyParentProperty(true)
		]
		public bool FilterByVisibleFieldsOnly {
			get { return filterByVisibleFieldsOnly; }
			set {
				if(filterByVisibleFieldsOnly == value) return;
				filterByVisibleFieldsOnly = value;
				Data.DoRefresh();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataAutoExpandGroups"),
#endif
		DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
		"DevExpress.XtraPivotGrid.PivotGridOptionsData.AutoExpandGroups"),
		TypeConverter(typeof(DefaultBooleanConverter)), AutoFormatEnable()
		]
		public DefaultBoolean AutoExpandGroups {
			get { return autoExpandGroups; }
			set {
				autoExpandGroups = value;
				Data.AutoExpandGroups = GetBooleanValueFromDefaultBooleanValue(value, DefaultAutoExpandGroupsValue);
			}
		}
		bool DefaultAutoExpandGroupsValue {
			get { return !Data.IsOLAP; }
		}
		internal bool AutoExpandGroupsInternal { get { return GetBooleanValueFromDefaultBooleanValue(AutoExpandGroups, DefaultAutoExpandGroupsValue); } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public void SetAutoExpandGroups(DefaultBoolean value) {
			autoExpandGroups = value;
		}
	}
}
