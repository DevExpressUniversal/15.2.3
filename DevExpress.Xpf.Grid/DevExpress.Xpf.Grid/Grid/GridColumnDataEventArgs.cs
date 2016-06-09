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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Data;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Data;
namespace DevExpress.Xpf.Grid {
	public class GridColumnDataEventArgs : ColumnDataEventArgsBase {
		readonly GridControl gridControl;
		readonly int listSourceRow;
		protected internal GridColumnDataEventArgs(GridControl gridControl, GridColumn column, int listSourceRow, object _value, bool isGetAction) 
			: base(column, _value, isGetAction) {
			this.gridControl = gridControl;
			this.listSourceRow = listSourceRow;
		}
		public int ListSourceRowIndex { get { return listSourceRow; } }
		public object GetListSourceFieldValue(string fieldName) {
			return GetListSourceFieldValue(ListSourceRowIndex, fieldName);
		}
		public object GetListSourceFieldValue(int listSourceRowIndex, string fieldName) {
			return ((GridDataProvider)gridControl.DataProviderBase).GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		public new GridColumn Column { get { return (GridColumn)base.Column; } }
		public GridControl Source { get { return gridControl; } }
	}
	public delegate void GridColumnDataEventHandler(object sender, GridColumnDataEventArgs e);
}
