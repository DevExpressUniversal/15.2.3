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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public class PivotColumnViewModel {
		public string Caption { get; set; }
		public ValueFormatViewModel Format { get; set; }
		public string DataId { get; set; }
		public PivotColumnViewModel() {
		}
		public PivotColumnViewModel(string caption, ValueFormatViewModel format, string dataId) {
			Caption = caption;
			Format = format;
			DataId = dataId;
		}
	}
	public class PivotDashboardItemViewModel : DataDashboardItemViewModel {
		public IList<PivotColumnViewModel> Columns { get; set; }
		public IList<PivotColumnViewModel> Rows { get; set; }
		public IList<PivotColumnViewModel> Values { get; set; }
		public bool ShowColumnGrandTotals { get; set; }
		public bool ShowRowGrandTotals { get; set; }
		public bool ShowColumnTotals { get; set; }
		public bool ShowRowTotals { get; set; }
		public string[] MeasureIds { get { return Values != null ? Values.Select(v => v.DataId).ToArray() : new string[0]; } }
		public PivotDashboardItemViewModel()
			: base() {
		}
		public PivotDashboardItemViewModel(PivotDashboardItem dashboardItem, IList<PivotColumnViewModel> columns, IList<PivotColumnViewModel> rows, IList<PivotColumnViewModel> values)
			: base(dashboardItem) {
			Columns = columns;
			Rows = rows;
			Values = values;
		}
	}
}
