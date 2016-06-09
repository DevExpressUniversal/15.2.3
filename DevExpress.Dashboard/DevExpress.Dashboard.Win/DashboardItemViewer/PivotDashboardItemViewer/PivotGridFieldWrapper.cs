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
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardWin.Native {
	public class PivotGridFieldWrapper : IPivotGridField {
		readonly PivotGridField field;
		public PivotDashboardItemArea Area {
			get { return PivotDashboardItemViewControl.GetPivotGridArea(field.Area); }
			set { field.Area = PivotDashboardItemViewControl.GetPivotArea(value); }
		}
		public int AreaIndex {
			get { return field.AreaIndex; }
		}
		public string Caption {
			get { return field.Caption; }
		}
		public string FieldName {
			get { return field.FieldName; }
		}
		public int Width {
			get { return field.Width; }
			set { field.Width = value; }
		}
		public PivotGridFieldWrapper(PivotGridField field) {
			this.field = field;
		}
	}
	public class FieldValueDisplayTextEventArgsWrapper : PivotFieldDisplayTextEventArgsWrapperBase {
		PivotFieldDisplayTextEventArgs eventArgs;
		public override IPivotGridField Field {
			get { return eventArgs.Field == null ? null : new PivotGridFieldWrapper(eventArgs.Field); }
		}
		public override bool IsColumn { get { return eventArgs.IsColumn; } }
		public override object Value {
			get { return eventArgs.Value; }
		}
		public override PivotDashboardItemValueType ValueType {
			get {
				switch(eventArgs.ValueType) {
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
		public FieldValueDisplayTextEventArgsWrapper(PivotFieldDisplayTextEventArgs eventArgs) {
			this.eventArgs = eventArgs;
		}
		public override void SetDisplayText(string displayText) {
			eventArgs.DisplayText = displayText;
		}
		protected override PivotDrillDownDataSource CreateDrillDownDataSource() {
			return eventArgs.CreateDrillDownDataSource();
		}
	}
	public class PivotCellDisplayTextEventArgsWrapper : PivotCellDisplayTextEventArgsBase {
		PivotCellDisplayTextEventArgs eventArgs;
		public override IPivotGridField DataField {
			get { return eventArgs.DataField == null ? null : new PivotGridFieldWrapper(eventArgs.DataField); }
		}
		public override object Value {
			get { return eventArgs.Value; }
		}
		public PivotCellDisplayTextEventArgsWrapper(PivotCellDisplayTextEventArgs eventArgs) {
			this.eventArgs = eventArgs;
		}
		public override void SetDisplayText(string displayText) {
			eventArgs.DisplayText = displayText;
		}
	}
	public class PivotDashboardItemCustomDrawCellEventArgs : PivotCustomDrawCellEventArgsBase {
		public PivotDashboardItemCustomDrawCellEventArgs(AxisPoint columnAxis, AxisPoint rowAxis, string valueFieldName, AppearanceObject appearance, bool isDataArea, bool isDarkSkin, Color defaultBackColor)
			: base(columnAxis, rowAxis, valueFieldName, isDataArea, isDarkSkin, defaultBackColor) {
			StyleSettings = new AppearanceObjectStyleSettingsInfo(appearance);
		}
		public PivotDashboardItemCustomDrawCellEventArgs(PivotDrillDownDataSource drillDownDataSource, string valueFieldName, AppearanceObject appearance, bool isDataArea, bool isDarkSkin, Color defaultBackColor)
			: base(drillDownDataSource, valueFieldName, isDataArea, isDarkSkin, defaultBackColor) {
			StyleSettings = new AppearanceObjectStyleSettingsInfo(appearance);
		}
		public bool CustomBackColor { get { return ((AppearanceObjectStyleSettingsInfo)StyleSettings).CustomBackColor; } }
	}
	public class AppearanceObjectStyleSettingsInfo : StyleSettingsInfo {
		readonly AppearanceObject appearance;
		bool customBackColor;
		bool customForeColor;
		public override Color BackColor { 
			get { return appearance.BackColor; } 
			set { 
				appearance.BackColor = value;
				appearance.BackColor2 = value;
				customBackColor = true;
			}
		}
		public override Color ForeColor { 
			get { return appearance.ForeColor; } 
			set { 
				appearance.ForeColor = value;
				customForeColor = true;
			}
		}
		public override Font Font { get { return appearance.Font; } set { appearance.Font = value; } }
		public bool CustomBackColor { get { return customBackColor; } }
		public bool CustomForeColor { get { return customForeColor; } }
		public AppearanceObjectStyleSettingsInfo(AppearanceObject appearance) {
			this.appearance = appearance;
		}
	}
	public class DisplayTextHiddenEventArgs : EventArgs {
		readonly AxisPoint rowPoint;
		readonly AxisPoint columnPoint;
		readonly string valueFieldName;
		public DisplayTextHiddenEventArgs(AxisPoint columnPoint, AxisPoint rowPoint, string valueFieldName) {
			this.rowPoint = rowPoint;
			this.columnPoint = columnPoint;
			this.valueFieldName = valueFieldName;
		}
		public AxisPoint RowAxisPoint { get { return rowPoint; } }
		public AxisPoint ColumnAxisPoint { get { return columnPoint; } }
		public string ValueFieldName { get { return valueFieldName; } }
	}
}
