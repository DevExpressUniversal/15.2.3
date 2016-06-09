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
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotShowButtonModeEnum {
		Default,
		ShowAlways,
		ShowForFocusedCell,
		ShowOnlyInEditor
	}
	public class PivotGridOptionsView : PivotGridOptionsViewBase {
		bool allowGlyphSkinning;
		int filterSeparatorBarPadding;
		PivotGridOptionsSeparator columnValueSeparator;
		PivotGridOptionsSeparator rowValueSeparator;
		FilterButtonShowMode headerFilterButtonShowMode;
		PivotShowButtonModeEnum showButtonMode;
		public PivotGridOptionsView(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.allowGlyphSkinning = false;
			this.headerFilterButtonShowMode = FilterButtonShowMode.Default;
			this.filterSeparatorBarPadding = 1;
			this.columnValueSeparator = new PivotGridOptionsSeparator(optionsChanged);
			this.rowValueSeparator = new PivotGridOptionsSeparator(optionsChanged);
			this.showButtonMode = PivotShowButtonModeEnum.Default;
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsViewHeaderFilterButtonShowMode")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.HeaderFilterButtonShowMode")]
		[DefaultValue(FilterButtonShowMode.Default), XtraSerializableProperty()]
		public virtual FilterButtonShowMode HeaderFilterButtonShowMode {
			get { return headerFilterButtonShowMode; }
			set {
				if(HeaderFilterButtonShowMode == value) return;
				FilterButtonShowMode prevValue = HeaderFilterButtonShowMode;
				headerFilterButtonShowMode = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsViewShowButtonMode"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.ShowButtonMode"),
		DefaultValue(PivotShowButtonModeEnum.Default), XtraSerializableProperty()
		]
		public virtual PivotShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(showButtonMode == value) return;
				showButtonMode = value;
				OnOptionsChanged();
			}
		}
		protected internal virtual FilterButtonShowMode GetHeaderFilterButtonShowMode() {
			return HeaderFilterButtonShowMode == FilterButtonShowMode.Default ? FilterButtonShowMode.SmartTag : HeaderFilterButtonShowMode;
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsViewFilterSeparatorBarPadding")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.FilterSeparatorBarPadding")]
		[DefaultValue(1), XtraSerializableProperty()]
		public int FilterSeparatorBarPadding {
			get { return filterSeparatorBarPadding; }
			set {
				if(value == FilterSeparatorBarPadding) return;
				filterSeparatorBarPadding = value;
				OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsViewAllowGlyphSkinning")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.AllowGlyphSkinning")]
		[DefaultValue(false), XtraSerializableProperty()]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(value == allowGlyphSkinning) return;
				allowGlyphSkinning = value;
				OnOptionsChanged();
			}
		}
		internal PivotGridOptionsSeparator ColumnValueSeparator {
			get { return columnValueSeparator; }
		}
		internal PivotGridOptionsSeparator RowValueSeparator {
			get { return rowValueSeparator; }
		}
	}
}
