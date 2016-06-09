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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.EditForm;
using DevExpress.XtraGrid.EditForm.Layout;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
namespace DevExpress.XtraGrid.EditForm.Helpers {
	public class ValuesCache {
		Dictionary<string, object> values;
		public void SaveValues(ColumnView view, int rowHandle) {
			this.values = PullValues(view, rowHandle, false);
		}
		public void RestoreValues(ColumnView view, int rowHandle) {
			PushValues(view, rowHandle, values);
		}
		public void PushValues(ColumnView view, int rowHandle, Dictionary<string, object> values) {
			if(values == null) return;
			foreach(GridColumn c in view.Columns) {
				if(c.ReadOnly || !values.ContainsKey(c.FieldName)) continue;
				object current = view.GetRowCellValue(rowHandle, c.FieldName);
				object saved = values[c.FieldName];
				try {
					if(object.Equals(current, saved)) continue;
				}
				catch {
				}
				view.SetRowCellValue(rowHandle, c.FieldName, saved);
			}
		}
		public Dictionary<string, object> PullValues(ColumnView view, int rowHandle, bool processReadOnly) {
			var res = new Dictionary<string, object>();
			foreach(GridColumn c in view.Columns) {
				if((c.ReadOnly && !processReadOnly) || string.IsNullOrEmpty(c.FieldName)) continue;
				res[c.FieldName] = view.GetRowCellValue(rowHandle, c.FieldName);
			}
			return res;
		}
		public virtual void Clear() {
			if(values != null) values.Clear();
		}
	}
}
