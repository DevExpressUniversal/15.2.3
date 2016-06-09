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
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsCommandColumnInfo : XlsCommandContentBase {
		XlsContentColumnInfo content = new XlsContentColumnInfo();
		#region Properties
		public int FirstColumn { get { return content.FirstColumn; } set { content.FirstColumn = value; } }
		public int LastColumn { get { return content.LastColumn; } set { content.LastColumn = value; } }
		public int ColumnWidth { get { return content.ColumnWidth; } set { content.ColumnWidth = value; } }
		public int FormatIndex { get { return content.FormatIndex; } set { content.FormatIndex = value; } }
		public bool Hidden { get { return content.Hidden; } set { content.Hidden = value; } }
		public bool CustomWidth { get { return content.CustomWidth; } set { content.CustomWidth = value; } }
		public bool BestFit { get { return content.BestFit; } set { content.BestFit = value; } }
		public bool ShowPhoneticInfo { get { return content.ShowPhoneticInfo; } set { content.ShowPhoneticInfo = value; } }
		public int OutlineLevel { get { return content.OutlineLevel; } set { content.OutlineLevel = value; } }
		public bool Collapsed { get { return content.Collapsed; } set { content.Collapsed = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Worksheet sheet = contentBuilder.CurrentSheet;
			float widthInChars = (float)((double)ColumnWidth / 256);
			widthInChars = contentBuilder.DocumentModel.GetService<IColumnWidthCalculationService>().RemoveGaps(sheet, widthInChars);
			widthInChars = (float)Math.Min(255, widthInChars);
			int lastColumnIndex = Math.Min(LastColumn, XlsDefs.MaxColumnCount - 1);
			int defaultColumnWidth = contentBuilder.CurrentSheet.Properties.FormatProperties.BaseColumnWidth;
			if(defaultColumnWidth == 0)
				defaultColumnWidth = XlsDefs.DefaultColumnWidth;
			if((IsNotDefault() || widthInChars != defaultColumnWidth) && (FirstColumn <= lastColumnIndex)) {
				Column info = sheet.Columns.CreateNewColumnRange(FirstColumn, lastColumnIndex);
				info.BeginUpdate();
				try {
					info.Width = widthInChars;
					info.BestFit = BestFit;
					info.IsHidden = Hidden;
					info.IsCollapsed = Collapsed;
					info.IsCustomWidth = CustomWidth;
					info.OutlineLevel = OutlineLevel;
					if (OutlineLevel > 0 && OutlineLevel <= 7 && sheet.Properties.FormatProperties.OutlineLevelCol < OutlineLevel)
						sheet.Properties.FormatProperties.OutlineLevelCol = OutlineLevel;
				}
				finally {
					info.EndUpdate();
				}
				if (FormatIndex > contentBuilder.DocumentModel.StyleSheet.DefaultCellFormatIndex)
					info.AssignCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndex));
			}
			if(!CustomWidth && LastColumn == XlsDefs.FullRangeColumnIndex)
				sheet.Properties.FormatProperties.DefaultColumnWidth = widthInChars;
		}
		bool IsNotDefault() {
			return CustomWidth || FormatIndex != 15 || Hidden || BestFit || Collapsed;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
}
