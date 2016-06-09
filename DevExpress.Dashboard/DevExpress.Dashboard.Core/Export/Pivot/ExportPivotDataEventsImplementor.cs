#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.PivotGrid.Events;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardExport {
	public class ExportPivotDataEventsImplementor : PivotGridEmptyEventsImplementorBase {
		readonly PivotGridData pivotData;
		public event EventHandler<ExportFieldValueDisplayTextEventArgsWrapper> RequestFieldValueDisplayText;
		public event EventHandler<ExportPivotCellDisplayTextEventArgsWrapper> RequestCustomCellDisplayText;
		public ExportPivotDataEventsImplementor(PivotGridData pivotData) {
			this.pivotData = pivotData;
		}
		protected override string FieldValueDisplayTextCore(PivotFieldValueItem item, string defaultText) {
			if(RequestFieldValueDisplayText != null) {
				var e = new ExportFieldValueDisplayTextEventArgsWrapper(item, pivotData.GetField(item.Field), defaultText);
				RequestFieldValueDisplayText(this, e);
				return e.DisplayText;
			}
			return base.FieldValueDisplayTextCore(item, defaultText);
		}
		protected override string CustomCellDisplayTextCore(PivotGridCellItem cellItem) {
			if(RequestCustomCellDisplayText != null) {
				var e = new ExportPivotCellDisplayTextEventArgsWrapper(pivotData.GetField(cellItem.DataField), cellItem.Value);
				RequestCustomCellDisplayText(this, e);
				return e.DisplayText;
			}
			return base.CustomCellDisplayTextCore(cellItem);
		}
	}
	public class ExportFieldValueDisplayTextEventArgsWrapper : PivotFieldDisplayTextEventArgsWrapperBase {
		readonly PivotFieldValueItem item;
		readonly PivotGridFieldBase field;
		string displayText;
		public string DisplayText { get { return displayText; } }
		public override IPivotGridField Field {
			get { return field == null ? null : new ExportPivotGridFieldWrapper(field); }
		}
		public override object Value {
			get { return item.Value; }
		}
		public override bool IsColumn {
			get { return item.IsColumn; }
		}
		public override PivotDashboardItemValueType ValueType {
			get {
				switch(item.ValueType) {
					case PivotGridValueType.CustomTotal:
						return PivotDashboardItemValueType.CustomTotal;
					case PivotGridValueType.GrandTotal:
						return PivotDashboardItemValueType.GrandTotal;
					case PivotGridValueType.Total:
						return PivotDashboardItemValueType.Total;
					case PivotGridValueType.Value:
					default:
						return PivotDashboardItemValueType.Value;
				}
			}
		}
		public ExportFieldValueDisplayTextEventArgsWrapper(PivotFieldValueItem item, PivotGridFieldBase field, string defaultText) {
			this.item = item;
			this.field = field;
			this.displayText = defaultText;
		}
		public override void SetDisplayText(string displayText) {
			this.displayText = displayText;
		}
		protected override PivotDrillDownDataSource CreateDrillDownDataSource() {
			return item.CreateDrillDownDataSource();
		}
	}
	public class ExportPivotCellDisplayTextEventArgsWrapper : PivotCellDisplayTextEventArgsBase {
		readonly PivotGridFieldBase field;
		readonly object value;
		string displayText;
		public string DisplayText { get { return displayText; } }
		public override IPivotGridField DataField {
			get { return field == null ? null : new ExportPivotGridFieldWrapper(field); }
		}
		public override object Value {
			get { return value; }
		}
		public ExportPivotCellDisplayTextEventArgsWrapper(PivotGridFieldBase field, object value) {
			this.value = value;
			this.field = field;
		}
		public override void SetDisplayText(string displayText) {
			this.displayText = displayText;
		}
	}
	public class ExportPivotCustomDrawCellEventArgs : PivotCustomDrawCellEventArgsBase {
		public ExportPivotCustomDrawCellEventArgs(PivotDrillDownDataSource drillDownDataSource, string valueFieldName, Color backColor, Color foreColor, Font font, bool isDataArea, Color defaultBackColor)
			: base(drillDownDataSource, valueFieldName, isDataArea, false, defaultBackColor) {
			StyleSettings = new PrintAppearanceStyleSettingsInfo(backColor, foreColor, font);
		}
	}
	public class PrintAppearanceStyleSettingsInfo : StyleSettingsInfo {
		Color foreColor;
		bool customForeColor;
		public PrintAppearanceStyleSettingsInfo(Color backColor, Color foreColor, Font font) {
			this.BackColor = backColor;
			this.foreColor = foreColor;
			this.Font = font;
		}
		public override Color ForeColor {
			get { return foreColor; }
			set {
				foreColor = value;
				customForeColor = true;
			}
		}
		public bool CustomForeColor { get { return customForeColor; } }
	}
}
