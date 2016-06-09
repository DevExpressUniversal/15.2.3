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
namespace DevExpress.XtraSpreadsheet.Model {
	public interface ISpreadsheetFieldList {
		void RefreshTreeList();
		void CreateController(ISpreadsheetFieldList spreadsheetFieldList);
	}
	public class SpreadsheetFieldListController {
		#region Fields
		readonly ISpreadsheetFieldList spreadsheetFieldList;
		ISpreadsheetControl spreadsheetControl;
		#endregion
		#region Properites
		public ISpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				if (spreadsheetControl == value)
					return;
				UnsubscribeDocumentModelEvents();
				spreadsheetControl = value;
				SubscribeDocumentModelEvents();
			}
		}
		protected internal DocumentModel Workbook { get { return SpreadsheetControl != null && SpreadsheetControl.InnerControl != null ? SpreadsheetControl.InnerControl.DocumentModel : null; } }
		#endregion
		public SpreadsheetFieldListController(ISpreadsheetFieldList spreadsheetFieldList) {
			this.spreadsheetFieldList = spreadsheetFieldList;
		}
		void OnMailMergeDataSourceChanged(object sender, EventArgs e) {
			spreadsheetFieldList.RefreshTreeList();
		}
		void UnsubscribeDocumentModelEvents() {
			if (Workbook == null)
				return;
			Workbook.MailMergeDataSourceChanged -= OnMailMergeDataSourceChanged;
			Workbook.MailMergeQueriesChanged -= OnMailMergeDataSourceChanged;
			Workbook.MailMergeRelationsChanged -= OnMailMergeDataSourceChanged;
		}
		void SubscribeDocumentModelEvents() {
			if (Workbook == null)
				return;
			Workbook.MailMergeDataSourceChanged += OnMailMergeDataSourceChanged;
			Workbook.MailMergeQueriesChanged += OnMailMergeDataSourceChanged;
			Workbook.MailMergeRelationsChanged += OnMailMergeDataSourceChanged;
		}
	}
}
