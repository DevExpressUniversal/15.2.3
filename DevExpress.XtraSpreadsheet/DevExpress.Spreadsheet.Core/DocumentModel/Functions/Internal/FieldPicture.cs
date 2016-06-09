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
using System.Drawing;
using System.IO;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public class FunctionFieldPicture : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "FIELDPICTURE"; } }
		public override int Code { get { return 0x4102; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		public override bool IsVolatile { get { return false; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			MailMergeProcessor mailMergeProcessor = context.Workbook.MailMergeProcessor;
			VariantValue argumentName = string.Empty;
			if (arguments.Count != 0)
				argumentName = arguments[0];
			if (argumentName.IsError)
				return argumentName;
			string text = argumentName.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
			if (mailMergeProcessor == null)
				return "[" + text + "]";
			VariantValue argumentMode = string.Empty;
			if (arguments.Count >= 1)
				argumentMode = arguments[1];
			if (argumentMode.IsError)
				return argumentMode;
			string mode = argumentMode.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
			if (string.IsNullOrEmpty(mode))
				return "[" + text + "]";
			mode = mode.ToLowerInvariant();
			VariantValue argumentRange = string.Empty;
			if (arguments.Count >= 2)
				argumentRange = arguments[2];
			if (argumentRange.IsError)
				return argumentRange;
			if (!argumentRange.IsCellRange)
				return "[" + text + "]";
			CellRange range = argumentRange.CellRangeValue.GetFirstInnerCellRange();
			switch (mode) {
				case "range":
					return GetPictureRange(text, range, arguments, context);
				case "topleft":
					return GetPictureTopLeftAnchor(text, range, arguments, context);
			}
			return "[" + text + "]";
		}
		float GetModelUnitsValue(VariantValue argument, WorkbookDataContext context, float dpi, float defaultValue) {
			float result = GetModelUnitsValueCore(argument, context, dpi);
			return result > 0 ? result : defaultValue;
		}
		float GetModelUnitsValueCore(VariantValue argument, WorkbookDataContext context, float dpi) {
			Worksheet sheet = (Worksheet)context.CurrentWorksheet;
			int pixelsValue = (int)argument.ToNumeric(context).NumericValue;
			float result = sheet.Workbook.UnitConverter.PixelsToModelUnits(pixelsValue, dpi);
			return result;
		}
		OfficeImage GetOfficeImage(object imageValue, DocumentModel documentModel) {
			byte[] bytes = imageValue as byte[];
			if(bytes != null) {
				OfficeImage officeImage = SpreadsheetImageSource.FromStream(new MemoryStream(bytes)).CreateImage(documentModel);
				return officeImage;
			}
#if !SL
			Image image = imageValue as Image;
			if(image != null) {
				OfficeImage officeImage = SpreadsheetImageSource.FromImage(image).CreateImage(documentModel);
				return officeImage;
			}
#endif
			return null;
		}
		VariantValue GetPictureTopLeftAnchor(string text, CellRange range, IList<VariantValue> arguments, WorkbookDataContext context) {
			MailMergeProcessor mailMergeProcessor = context.Workbook.MailMergeProcessor;
			object image = mailMergeProcessor.GetValueByName(text);
			if(image == null || image is DBNull)
				return "[" + text + "]";
			Worksheet sheet = (Worksheet)context.CurrentWorksheet;
			OfficeImage officeImage = GetOfficeImage(image, sheet.Workbook);
			if (officeImage == null)
				return "[" + text + "]";
			CellKey topLeft = new CellKey(sheet.SheetId, range.TopLeft.Column, range.TopLeft.Row);
			Picture picture = sheet.InsertPicture(officeImage, topLeft);
			picture.Locks.NoChangeAspect = false;
			if(arguments.Count >= 4) {
				if(arguments[3].IsError)
					return arguments[3];
				picture.Width = GetModelUnitsValue(arguments[3], context, DocumentModel.DpiX, picture.Width);
				if(arguments.Count > 4) {
					if(arguments[4].IsError)
						return arguments[4];
					picture.Height = GetModelUnitsValue(arguments[4], context, DocumentModel.DpiY, picture.Height);
				}
			}
			return string.Empty;
		}
		VariantValue GetPictureRange(string text, CellRange range, IList<VariantValue> arguments, WorkbookDataContext context) {
			MailMergeProcessor mailMergeProcessor = context.Workbook.MailMergeProcessor;
			object image = mailMergeProcessor.GetValueByName(text);
			if(image == null || image is DBNull)
				return "[" + text + "]";
			Worksheet sheet = (Worksheet) context.CurrentWorksheet;
			OfficeImage officeImage = GetOfficeImage(image, sheet.Workbook);
			if(officeImage == null)
				return "[" + text + "]";
			bool stretch = false;
			float offsetX = 0;
			float offsetY = 0;
			if(arguments.Count >= 4) {
				if(arguments[3].IsError)
					return arguments[3];
				stretch = arguments[3].ToBoolean(context).BooleanValue;
				if(arguments.Count >= 5) {
					if(arguments[4].IsError)
						return arguments[4];
					offsetX = GetModelUnitsValueCore(arguments[4], context, DocumentModel.DpiX);
					if(arguments.Count >= 6) {
						if(arguments[5].IsError)
							return arguments[5];
						offsetY = GetModelUnitsValueCore(arguments[5], context, DocumentModel.DpiY);
					}
				}
			}
			CellKey topLeft = new CellKey(sheet.SheetId, range.TopLeft.Column, range.TopLeft.Row);
			CellKey bottomRight = new CellKey(sheet.SheetId, range.BottomRight.Column + 1, range.BottomRight.Row + 1);
			Picture picture = sheet.InsertPicture(officeImage, topLeft, bottomRight, !stretch);
			picture.Move(offsetY, offsetX);
			return string.Empty;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.Default));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.Default));
			collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.Default));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
}
