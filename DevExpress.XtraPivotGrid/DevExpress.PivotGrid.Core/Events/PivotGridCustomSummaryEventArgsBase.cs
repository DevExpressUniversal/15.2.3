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
using DevExpress.Data.PivotGrid;
using System.ComponentModel;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridCustomSummaryEventArgsBase<T> : EventArgs where T : PivotGridFieldBase {
		PivotGridFieldBase field;
		PivotCustomSummaryInfo customSummaryInfo;
		PivotGridData data;
		PivotDrillDownDataSource dataSource;
		public PivotGridCustomSummaryEventArgsBase(PivotGridData data, PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			this.data = data;
			this.field = field;
			this.customSummaryInfo = customSummaryInfo;
			this.dataSource = null;
		}
		protected PivotCustomSummaryInfo CustomSummaryInfo { get { return customSummaryInfo; } }
		public object CustomValue { get { return SummaryValue.CustomValue; } set { SummaryValue.CustomValue = value; } }
		public PivotSummaryValue SummaryValue { get { return CustomSummaryInfo.SummaryValue; } }
		public PivotGridFieldBase DataField { get { return field; } }
		public string FieldName { 
			get {
				if(field != null)
					return field.FieldName;
				return customSummaryInfo.DataColumn.Name;
			} 
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return DataSource;
		}
		public T ColumnField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.ColColumn) as T; }
		}
		public T RowField {
			get { return data.GetFieldByPivotColumnInfo(CustomSummaryInfo.RowColumn) as T; }
		}
		public object ColumnFieldValue { get { return GetFieldValue(CustomSummaryInfo.ColColumn); } }
		public object RowFieldValue { get { return GetFieldValue(CustomSummaryInfo.RowColumn); } }
		protected PivotDrillDownDataSource DataSource {
			get {
				if(this.dataSource == null)
					this.dataSource = data.GetDrillDownDataSource(CustomSummaryInfo.GroupRow, CustomSummaryInfo.VisibleListSourceRows);
				return this.dataSource;
			}
		}
		protected object GetFieldValue(PivotColumnInfo columnInfo) {
			if(columnInfo == null) return null;
			return DataSource[0][columnInfo.ColumnInfo.Name];
		}
	}	
}
