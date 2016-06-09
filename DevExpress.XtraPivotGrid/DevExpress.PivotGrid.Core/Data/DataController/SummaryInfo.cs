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

using DevExpress.Compatibility.System.ComponentModel;
using System;
#if !SL
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.PivotGrid {
	public class PivotCustomSummaryInfo {
		PivotSummaryItem summaryItem;
		PivotColumnInfo colColumn;
		PivotColumnInfo rowColumn;
		GroupRowInfo groupRow;
		PivotSummaryValue summaryValue;
		DataControllerGroupHelperBase helper;
		VisibleListSourceRowCollection visibleListSourceRows;
		public PivotCustomSummaryInfo(DataControllerGroupHelperBase helper, VisibleListSourceRowCollection visibleListSourceRows,
			PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, GroupRowInfo groupRow) {
			this.helper = helper;
			this.visibleListSourceRows = visibleListSourceRows;
			this.groupRow = groupRow;
			this.summaryItem = summaryItem;
			this.summaryValue = summaryValue;
			this.colColumn = null;
			this.rowColumn = null;
		}
		public PivotSummaryItem SummaryItem { get { return summaryItem; } }
		public DataColumnInfo DataColumn { get { return SummaryItem.ColumnInfo; } }
		public PivotColumnInfo ColColumn { get { return colColumn; } set { colColumn = value; } }
		public PivotColumnInfo RowColumn { get { return rowColumn; } set { rowColumn = value; } }
		public GroupRowInfo GroupRow { get { return groupRow; } }
		public PivotSummaryValue SummaryValue { get { return summaryValue; } }
		public VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		public int ChildCount {
			get {
				if(helper == null || GroupRow == null || GroupRow.Level >= helper.GroupInfo.Count - 1) return 0;
				return helper.GroupInfo.GetTotalChildrenGroupCount(groupRow);
			}
		}
		public PivotCustomSummaryInfo GetChildSummaryInfo(int index) {
			if(index >= ChildCount) return null;
			GroupRowInfo childGroupRow = helper.GroupInfo[GroupRow.Index + index + 1];
			return helper.CreateCustomSummaryInfo(summaryItem, childGroupRow);
		}
	}
	public class SummaryPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor pd;
		PivotSummaryItem summaryItem;
		public SummaryPropertyDescriptor(PropertyDescriptor pd, PivotSummaryItem summaryItem)
			: base(summaryItem.Name, null) {
			this.pd = pd;
			this.summaryItem = summaryItem;
		}
		public PivotSummaryItem SummaryItem { get { return summaryItem; } }
		public override bool CanResetValue(object component) {
			return pd.CanResetValue(component);
		}
		public override object GetValue(object component) {
			return pd.GetValue(component);
		}
		public override void SetValue(object component, object value) {
			pd.SetValue(component, value);
		}
		public override bool IsReadOnly { get { return pd.IsReadOnly; } }
		public override Type ComponentType { get { return pd.ComponentType; } }
		public override Type PropertyType { get { return pd.PropertyType; } }
		public override void ResetValue(object component) { pd.ResetValue(component); }
		public override bool ShouldSerializeValue(object component) { return pd.ShouldSerializeValue(component); }
	}
}
