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
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraExport.Helpers {
	public class ClipboardTxtExporter<TCol, TRow> : IClipboardExporter<TCol,TRow>	 
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public ClipboardTxtExporter() { }
		StringBuilder txtCore;
		List<string> headerCollection;
		List<List<string>> rowCollection;
		int[] columnSizeCollection;
		public void BeginExport() {
			txtCore = new StringBuilder();
			headerCollection = new List<string>();
			rowCollection = new List<List<string>>();
			columnSizeCollection = null;
		}
		public void EndExport() {
			if(headerCollection.Count > 0) {
				for(int i = 0; i < headerCollection.Count; i++) {
					string format = string.Format("{{0,-{0}}}\t", columnSizeCollection[i]);
					txtCore.AppendFormat(format, headerCollection.ElementAt(i));
				}
				txtCore.AppendLine();
			}
			if(rowCollection.Count > 0) {
				for(int i = 0; i < rowCollection.Count; i++) {
					List<string> row = rowCollection.ElementAt(i);
					if (row.Count>1)
						for(int n = 0; n < row.Count; n++) {
							string format = string.Format("{{0,-{0}}}\t", columnSizeCollection[n]);
							txtCore.AppendFormat(format, row.ElementAt(n), columnSizeCollection[n]);
						}
					else
						txtCore.Append(row.First());
					txtCore.AppendLine();
				}
			}
		}
		public void AddHeaders(IEnumerable<TCol> selectedColumns, IEnumerable<Export.Xl.XlCellFormatting> appearance) {
			if (columnSizeCollection==null)
				columnSizeCollection = new int[selectedColumns.Count()];
			for(int i = 0; i < selectedColumns.Count(); i++) {
				headerCollection.Add(selectedColumns.ElementAt(i).Header);
				columnSizeCollection[i] = selectedColumns.ElementAt(i).Header.Length;
			}
		}
		public void AddGroupHeader(string groupHeader, Export.Xl.XlCellFormatting appearance, int columnsCount) {
			if(columnSizeCollection == null)
				columnSizeCollection = new int[columnsCount]; 
			List<string> row = new List<string>();
			row.Add(groupHeader);
			rowCollection.Add(row);
		}
		public void AddRow(IEnumerable<ClipboardCellInfo> rowInfo) {
			if(columnSizeCollection == null)
				columnSizeCollection = new int[rowInfo.Count()]; 
			List<string> row = new List<string>();
			for(int i = 0; i < rowInfo.Count(); i++) {
				string value = new string(' ', 4 * rowInfo.ElementAt(i).Formatting.Alignment.Indent);
				value += rowInfo.ElementAt(i).DisplayValue;
				row.Add(value);
				if(value.Length > columnSizeCollection[i]) columnSizeCollection[i] = value.Length;
			}
			rowCollection.Add(row);
		}
		public void SetDataObject(DataObject data) {
			data.SetData(typeof(string), txtCore.ToString());
		}
	}
}
