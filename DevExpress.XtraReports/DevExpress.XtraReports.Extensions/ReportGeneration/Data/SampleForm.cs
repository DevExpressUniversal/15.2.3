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

using System.Data;
using DevExpress.XtraEditors;
namespace DevExpress.XtraReports.Design.ReportGenerator.Data {
	public partial class SampleForm : XtraForm {
		public SampleForm(){
			InitializeComponent();
			gridControl1.DataSource = GetDataTable();
			if(gridView1.Columns.Count > 0){
				gridView1.Columns["UnitPrice"].DisplayFormat.FormatString = "c2";
			}
		}
		public XtraGrid.Views.Grid.GridView GridView{
			get { return gridView1; }
		}
		DataTable GetDataTable(){
			var dataTable1 = new DataTable();
			dataTable1.TableName = "Category";
			dataTable1.Columns.Add("OrderID", typeof(int));
			dataTable1.Columns.Add("ProductID", typeof(int));
			dataTable1.Columns.Add("UnitPrice", typeof(double));
			dataTable1.Rows.Add(new object[]{10248, 11, 14});
			dataTable1.Rows.Add(new object[]{10248, 42, 9.8});
			dataTable1.Rows.Add(new object[]{10248, 72, 34.8});
			dataTable1.Rows.Add(new object[]{10249, 14, 18.6});
			dataTable1.Rows.Add(new object[]{10249, 51, 42.4});
			return dataTable1;
		}
	}
}
