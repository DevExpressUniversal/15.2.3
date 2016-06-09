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
using DevExpress.Office.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotAdditionalProperties  -- SXEx --
	public class XlsCommandPivotAdditionalProperties : XlsCommandBase {
		#region Fields
		private enum sxexFlaf1 {
			AcrossPageLay = 0,
			WrapPage = 1 
		}
		private enum sxexFlaf2 {
			EnableWizard = 0, 
			EnableDrilldown = 1, 
			EnableFieldDialog = 2, 
			PreserveFormatting = 3, 
			MergeLabels = 4, 
			DisplayErrorString = 5, 
			DisplayNullString = 6, 
			SubtotalHiddenPageItems = 7 
		}
		int numberSxFormat;
		int numberSxSelect;
		int numberRowPage;
		int numberColumnPage;
		BitwiseContainer flag1 = new BitwiseContainer(16, new int[] { 1, 8 });
		BitwiseContainer flag2 = new BitwiseContainer(16);
		XLUnicodeStringNoCch errorMessage = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch displayNull = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch tag = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch pageFieldStyle = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch tableStyle = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch vacateStyle = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public int NumberSxFormat {
			get { return numberSxFormat; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfSxFormat");
				numberSxFormat = value;
			}
		}
		public int NumberSxSelect {
			get { return numberSxSelect; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfSxSelect");
				numberSxSelect = value;
			}
		}
		public int NumberRowPage {
			get { return numberRowPage; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberRowInPage");
				numberRowPage = value;
			}
		}
		public int NumberColumnPage {
			get { return numberColumnPage; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberColumnInPage");
				numberColumnPage = value;
			}
		}
		public bool IsAcrossPageLay {
			get { return flag1.GetBoolValue((int)sxexFlaf1.AcrossPageLay); }
			set { flag1.SetBoolValue((int)sxexFlaf1.AcrossPageLay, value); }
		}
		public int WrapPage {
			get { return flag1.GetIntValue((int)sxexFlaf1.WrapPage); }
			set { flag1.SetIntValue((int)sxexFlaf1.WrapPage, value); }
		}
		public bool IsEnableWizard {
			get { return flag2.GetBoolValue((int)sxexFlaf2.EnableWizard); }
			set { flag2.SetBoolValue((int)sxexFlaf2.EnableWizard, value); }
		}
		public bool IsEnableDrillDown {
			get { return flag2.GetBoolValue((int)sxexFlaf2.EnableDrilldown); }
			set { flag2.SetBoolValue((int)sxexFlaf2.EnableDrilldown, value); }
		}
		public bool IsEnableFieldDialog {
			get { return flag2.GetBoolValue((int)sxexFlaf2.EnableFieldDialog); }
			set { flag2.SetBoolValue((int)sxexFlaf2.EnableFieldDialog, value); }
		}
		public bool IsPreserveFormatting {
			get { return flag2.GetBoolValue((int)sxexFlaf2.PreserveFormatting); }
			set { flag2.SetBoolValue((int)sxexFlaf2.PreserveFormatting, value); }
		}
		public bool IsMergeLabels {
			get { return flag2.GetBoolValue((int)sxexFlaf2.MergeLabels); }
			set { flag2.SetBoolValue((int)sxexFlaf2.MergeLabels, value); }
		}
		public bool IsDisplayErrorString {
			get { return flag2.GetBoolValue((int)sxexFlaf2.DisplayErrorString); }
			set { flag2.SetBoolValue((int)sxexFlaf2.DisplayErrorString, value); }
		}
		public bool IsDisplayNullString {
			get { return flag2.GetBoolValue((int)sxexFlaf2.DisplayNullString); }
			set { flag2.SetBoolValue((int)sxexFlaf2.DisplayNullString, value); }
		}
		public bool IsSubtotalHiddenPageItems {
			get { return flag2.GetBoolValue((int)sxexFlaf2.SubtotalHiddenPageItems); }
			set { flag2.SetBoolValue((int)sxexFlaf2.SubtotalHiddenPageItems, value); }
		}
		public string ErrorMessage {
			get { return errorMessage.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "ErrorMessage");
				errorMessage.Value = value;
			}
		}
		public string DisplayNull {
			get { return displayNull.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "DisplayNull");
				displayNull.Value = value;
			}
		}
		public string Tag {
			get { return tag.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "Tag");
				tag.Value = value;
			}
		}
		public string PageFieldStyle {
			get { return pageFieldStyle.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "PageFieldStyle");
				pageFieldStyle.Value = value;
			}
		}
		public string TableStyle {
			get { return tableStyle.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "TableStyle");
				tableStyle.Value = value;
			}
		}
		public string VacateStyle {
			get { return vacateStyle.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "VacateStyle");
				vacateStyle.Value = value;
			}
		}
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			NumberSxFormat = reader.ReadUInt16();
			int lenError = reader.ReadUInt16();
			int lenDisplay = reader.ReadUInt16();
			int lenTag = reader.ReadUInt16();
			NumberSxSelect = reader.ReadUInt16();
			NumberRowPage = reader.ReadUInt16();
			NumberColumnPage = reader.ReadUInt16();
			flag1.ShortContainer = (short)reader.ReadUInt16();
			flag2.ShortContainer = (short)reader.ReadUInt16();
			int lenPageFieldStyle = reader.ReadUInt16();
			int lenTableStyle = reader.ReadUInt16();
			int lenVacateStyle = reader.ReadUInt16();
			if (lenError != 0xFFFF)
				errorMessage = XLUnicodeStringNoCch.FromStream(reader, lenError);
			if (lenDisplay != 0xFFFF)
				displayNull = XLUnicodeStringNoCch.FromStream(reader, lenDisplay);
			if (lenTag != 0xFFFF)
				tag = XLUnicodeStringNoCch.FromStream(reader, lenTag);
			if (lenPageFieldStyle != 0xFFFF)
				pageFieldStyle = XLUnicodeStringNoCch.FromStream(reader, lenPageFieldStyle);
			if (lenTableStyle != 0xFFFF)
				tableStyle = XLUnicodeStringNoCch.FromStream(reader, lenTableStyle);
			if (lenVacateStyle != 0xFFFF)
				vacateStyle = XLUnicodeStringNoCch.FromStream(reader, lenVacateStyle);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			 XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.PivotTable != null) {
				builder.PivotTable.SetPageWrap(WrapPage);
				builder.PivotTable.SetPageOverThenDown(IsAcrossPageLay);
				builder.PivotTable.EnableWizard = IsEnableWizard;
				builder.PivotTable.EnableDrill = IsEnableDrillDown;
				builder.PivotTable.EnableFieldProperties = IsEnableFieldDialog;
				builder.PivotTable.PreserveFormatting = IsPreserveFormatting;
				builder.PivotTable.SetMergeItem(IsMergeLabels);
				builder.PivotTable.SetShowError(IsDisplayErrorString);
				builder.PivotTable.SetShowMissing(IsDisplayNullString);
				builder.PivotTable.SetSubtotalHiddenItems(IsSubtotalHiddenPageItems);
				builder.PivotTable.SetErrorCaptionCore(ErrorMessage);
				builder.PivotTable.SetMissingCaptionCore(DisplayNull);
				builder.PivotTable.Tag = Tag;
				builder.PivotTable.PageStyle = PageFieldStyle;
				builder.PivotTable.PivotTableStyle = TableStyle;
				builder.PivotTable.VacatedStyle = VacateStyle;
				builder.PivotTable.Location.RowPageCount = NumberRowPage;
				builder.PivotTable.Location.ColumnPageCount = NumberColumnPage;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)NumberSxFormat);
			writer.Write((ushort)(ErrorMessage.Length > 0 ? ErrorMessage.Length : 0xFFFF));
			writer.Write((ushort)(DisplayNull.Length > 0 ? DisplayNull.Length : 0xFFFF));
			writer.Write((ushort)(Tag.Length > 0 ? Tag.Length : 0xFFFF));
			writer.Write((ushort)NumberSxSelect);
			writer.Write((ushort)NumberRowPage);
			writer.Write((ushort)NumberColumnPage);
			writer.Write((ushort)flag1.ShortContainer);
			writer.Write((ushort)flag2.ShortContainer);
			writer.Write((ushort)(PageFieldStyle.Length > 0 ? PageFieldStyle.Length : 0xFFFF));
			writer.Write((ushort)(TableStyle.Length > 0 ? TableStyle.Length : 0xFFFF));
			writer.Write((ushort)(VacateStyle.Length > 0 ? VacateStyle.Length : 0xFFFF));
			if (ErrorMessage.Length > 0)
				errorMessage.Write(writer);
			if (DisplayNull.Length > 0)
				displayNull.Write(writer);
			if (Tag.Length > 0)
				tag.Write(writer);
			if (PageFieldStyle.Length > 0)
				pageFieldStyle.Write(writer);
			if (TableStyle.Length > 0)
				tableStyle.Write(writer);
			if (VacateStyle.Length > 0)
				vacateStyle.Write(writer);
		}
		protected override short GetSize() {
			int result = 24;
			result += ErrorMessage.Length > 0 ? errorMessage.Length : 0;
			result += DisplayNull.Length > 0 ? displayNull.Length : 0;
			result += Tag.Length > 0 ? tag.Length : 0;
			result += PageFieldStyle.Length > 0 ? pageFieldStyle.Length : 0;
			result += TableStyle.Length > 0 ? tableStyle.Length : 0;
			result += VacateStyle.Length > 0 ? vacateStyle.Length : 0;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#endregion
	}
	#endregion
}
