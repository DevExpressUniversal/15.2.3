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
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office;
using System.Runtime.InteropServices;
using DevExpress.Office.History;
using System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class Table : ICellStylePropertyChanger {
		#region ICellStylePropertyChanger Members
		public CellStyleBase GetCellStyle(int elementIndex) {
			return GetCellFormat(elementIndex).Style;
		}
		public void SetCellStyle(int elementIndex, CellStyleBase value) {
			if (value == DocumentModel.StyleSheet.CellStyles[0]) {
				SetCellStyleCore(elementIndex, value, false);
				return;
			}
			if (GetCellFormat(elementIndex).Style == value)
				return;
			SetCellStyleCore(elementIndex, value, true);
		}
		void SetCellStyleCore(int elementIndex, CellStyleBase value, bool performSetProperty) {
			DocumentModel.BeginUpdate();
			try {
				SetApplyCellStyle(elementIndex, value != null);
				if (performSetProperty)
					SetPropertyValueCore(GetTableCellFormatIndexAccessor(elementIndex), SetStyleIndex, value);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		DocumentModelChangeActions SetStyleIndex(FormatBase info, CellStyleBase value) {
			((CellFormat)info).ApplyStyle(value);
			return DocumentModelChangeActions.None;
		}
		public bool GetApplyCellStyle(int elementIndex) {
			return ApplyFlagsInfo.GetApplyCellStyle(elementIndex);
		}
		void SetApplyCellStyle(int elementIndex, bool value) {
			if (GetApplyCellStyle(elementIndex) == value)
				return;
			SetApplyFlagsInfo(elementIndex, SetApplyCellStyle, value);
		}
		DocumentModelChangeActions SetApplyCellStyle(ref TableCellStyleApplyFlagsInfo info, int elementIndex, bool value) {
			info.SetApplyCellStyle(elementIndex, value);
			return DocumentModelChangeActions.None;
		}
		#endregion
	}
}
