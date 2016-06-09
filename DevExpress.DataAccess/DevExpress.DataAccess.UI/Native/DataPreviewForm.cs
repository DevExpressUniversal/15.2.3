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
using DevExpress.DataAccess.UI.Localization;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.DataAccess.UI.Native {
	public partial class DataPreviewForm : OkCancelForm {
		object dataSource;
		public object DataSource
		{
			get { return this.dataSource; }
			set {
				this.dataSource = value;
				UpdatePreview();
			}
		}
		public DataPreviewForm() : this(true) { }
		public DataPreviewForm(bool isCustomSqlPreview) {
			InitializeComponent();
			LocalizeComponent(isCustomSqlPreview);
		}
		void LocalizeComponent(bool isCustomSqlPreview) {
			btnCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Close);
			Text = isCustomSqlPreview
				? DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlDataPreviewCaption_All)
				: DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryDesignControlDataPreviewCaption);
		}
		void UpdatePreview() {
			this.gridControl.BeginUpdate();
			try {
				this.gridView.Columns.Clear();
				this.gridControl.DataSource = this.dataSource;
				for(int i = 0; i < this.gridView.Columns.Count; i++) {
					GridColumn gridColumn = this.gridView.Columns[i];
					if(gridColumn.ColumnType == typeof(DateTime)) {
						gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
						gridColumn.DisplayFormat.FormatString = "MM/d/yyyy hh:mm:ss tt";
					}
				}
				this.gridView.BestFitColumns();
			} finally {
				this.gridControl.EndUpdate();
			}
		}
	}
}
