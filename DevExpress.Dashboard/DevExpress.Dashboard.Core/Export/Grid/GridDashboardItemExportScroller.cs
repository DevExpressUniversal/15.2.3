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

using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using System;
using System.Collections.Generic;
namespace DevExpress.DashboardExport {
	public class GridDashboardItemExportScroller {
		readonly PivotGridThinClientData data;
		public PivotGridThinClientData ScrolledData { get; private set; }
		public GridDashboardItemExportScroller(PivotGridThinClientData data) {
			this.data = data;
		}
		public void ScrollTo(int pathRowIndex, int pathColumnIndex) {
			if(pathRowIndex == 0 && pathColumnIndex == 0) {
				ScrolledData = data;
				return;
			}
			IList<ThinClientFieldValueItem> rows = new List<ThinClientFieldValueItem>();
			for(int i = pathRowIndex; i < data.RowFieldValues.Count; i++)
				rows.Add(data.RowFieldValues[i]);
			PivotGridThinClientData res = new PivotGridThinClientData(null, rows);
			foreach(ThinClientFieldValueItem rowFieldValue in res.RowFieldValues) {
				int dataIndex = 0;
				int columnIndex = pathColumnIndex;
				ThinClientValueItem cell;
				while(data.TryGetCell(null, rowFieldValue, columnIndex++, out cell))
					res.AddCell(null, rowFieldValue, dataIndex++, cell);
			}
			ScrolledData = res;
		}
	}
}
