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
using DevExpress.Office;
using System.Collections.Generic;
namespace DevExpress.Spreadsheet {
	public interface Formatting {
		string NumberFormat { get; set; }
		Alignment Alignment { get; }
		SpreadsheetFont Font { get; }
		Borders Borders { get; }
		Fill Fill { get; }
		Protection Protection { get; }
		StyleFlags Flags { get; }
		bool Equals(object obj);
		void BeginUpdate();
		void EndUpdate();
	}
	#region Style
	public interface Style : Formatting {
		string Name { get; }
		bool IsBuiltIn { get; }
		bool Reset();
		void CopyFrom(BuiltInStyleId id);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Utils;
	using System.Collections.Generic;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	using ModelDefinedNameCollection = DevExpress.XtraSpreadsheet.Model.DefinedNameCollection;
	using ModelDefinedNameDase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
	using ModelStyleSheet = DevExpress.XtraSpreadsheet.Model.StyleSheet;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	#endregion
	#region NativeCellStyle
	partial class NativeCellStyle : NativeObjectBase, Style {
		#region Fields
		Model.CellStyleBase cellStyle;
		NativeStyleFlags styleFlags;
		NativeAlignment alignment;
		NativeFont font;
		NativeStyleBorders borders;
		NativeActualFill fill;
		NativeProtection protection;
		#endregion
		public NativeCellStyle(Model.CellStyleBase cellStyle) {
			Guard.ArgumentNotNull(cellStyle, "cellStyle");
			FormatBaseWriteAccessor formatAccessor = new FormatBaseWriteAccessor(cellStyle);
			this.cellStyle = cellStyle;
			styleFlags = new NativeStyleFlags(formatAccessor);
			this.alignment = new NativeAlignment(cellStyle, cellStyle.Alignment, cellStyle.DocumentModel.UnitConverter);
			this.font = new NativeFont(cellStyle, cellStyle.Font);
			this.borders = new NativeStyleBorders(cellStyle, cellStyle.Border);
			this.fill = new NativeActualFill(formatAccessor);
			this.protection = new NativeProtection(cellStyle.Protection);
		}
		#region Properties
		#region ModelCellStyle
		public Model.CellStyleBase ModelCellStyle {
			get {
				CheckValid();
				return cellStyle;
			}
		}
		#endregion
		public Model.CellStyleFormat StyleFormat { get { return ModelCellStyle.FormatInfo as Model.CellStyleFormat; } }
		public string Name { get { return ModelCellStyle.Name; } }
		#region Hidden
		public bool Hidden {
			get { return ModelCellStyle.IsHidden; }
			set {
				if (ModelCellStyle.IsHidden == value)
					return;
				ModelCellStyle.SetHidden(value);
			}
		}
		#endregion
		public string NumberFormat { get { return ModelCellStyle.FormatString; } set { ModelCellStyle.FormatString = value; } }
		public SpreadsheetFont Font {
			get {
				CheckValid();
				return font;
			}
		}
		public Alignment Alignment {
			get {
				CheckValid();
				return alignment;
			}
		}
		public Borders Borders {
			get {
				CheckValid(); 
				return borders;
			}
		}
		public Fill Fill {
			get {
				CheckValid(); 
				return fill;
			}
		}
		public Protection Protection {
			get {
				CheckValid(); 
				return protection;
			}
		}
		public StyleFlags Flags {
			get {
				CheckValid();
				return styleFlags;
			}
		}
		public int OutlineLevel { get { return ModelCellStyle.OutlineLevel; } }
		#endregion
		#region CopyFrom
		void CopyFrom(Model.BuiltInCellStyle builtInStyle) {
			cellStyle.CopyFrom(builtInStyle);
		}
		#endregion
		#region CopyFrom(BuiltInStyleId id)
		public void CopyFrom(BuiltInStyleId id) {
			Model.DocumentModel modelDocumentModel = ModelCellStyle.DocumentModel;
			Model.CellStyleCollection modelCellStyles = modelDocumentModel.StyleSheet.CellStyles;
			int modelId = (int)id;
			Model.BuiltInCellStyle cellStyle = modelCellStyles.TryGetBuiltInCellStyleById(modelId);
			if (cellStyle != null)
				CopyFrom(cellStyle);
			else
				CopyFrom(new Model.BuiltInCellStyle(modelDocumentModel, modelId));
		}
		#endregion
		#region IsBuiltIn
		public bool IsBuiltIn { get { return ModelCellStyle.IsBuiltIn; } }
		#endregion
		#region Reset
		public bool Reset() {
			return ModelCellStyle.Reset();
		}
		#endregion
		#region BeginUpdate
		public void BeginUpdate() {
			ModelCellStyle.BeginUpdate();
		}
		#endregion
		#region EndUpdate
		public void EndUpdate() {
			ModelCellStyle.EndUpdate();
		}
		#endregion
		public override string ToString() {
			return String.Format("Style: \"{0}\"", Name);
		}
	}
	#endregion
	#region NativeActualCellFormat
	partial class NativeActualCellFormat : Formatting {
		#region Fields
		IFormatBaseAccessor formatAccessor;
		NativeStyleFlags flags;
		NativeActualAlignment alignment;
		NativeActualFont font;
		NativeActualBorders borders;
		NativeActualFill fill;
		NativeActualProtection protection;
		#endregion
		public NativeActualCellFormat(IFormatBaseAccessor formatAccessor) {
			Guard.ArgumentNotNull(formatAccessor, "formatAccessor");
			this.formatAccessor = formatAccessor;
			this.flags = new NativeStyleFlags(formatAccessor);
			this.alignment = new NativeActualAlignment(formatAccessor);
			this.font = new NativeActualFont(formatAccessor);
			this.borders = new NativeActualBorders(formatAccessor);
			this.fill = new NativeActualFill(formatAccessor);
			this.protection = new NativeActualProtection(formatAccessor);
		}
		#region Properties
		public Model.IFormatBaseBatchUpdateable Format { get { return ReadWriteFormat; } }
		protected Model.IFormatBaseBatchUpdateable ReadWriteFormat { get { return formatAccessor.ReadWriteFormat; } }
		protected Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get { return formatAccessor.ReadOnlyFormat; } }
		public StyleFlags Flags { get { return flags; } }
		public string NumberFormat { get { return ReadOnlyFormat.ActualFormatString; } set { ReadWriteFormat.FormatString = value; } }
		public SpreadsheetFont Font { get { return font; } }
		public Alignment Alignment { get { return alignment; } }
		public Borders Borders { get { return borders; } }
		public Fill Fill { get { return fill; } }
		public Protection Protection { get { return protection; } }
		public int OutlineLevel { get { return 0; } set { } }
		#endregion
		#region BeginUpdate
		public void BeginUpdate() {
			ReadWriteFormat.BeginUpdate();
		}
		#endregion
		#region EndUpdate
		public void EndUpdate() {
			ReadWriteFormat.EndUpdate();
		}
		#endregion
	}
	#endregion
	#region IFormatBaseAccessor
	public interface IFormatBaseAccessor {
		Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get; }
		Model.IFormatBaseBatchUpdateable ReadWriteFormat { get; }
	}
	#endregion
	public class FormatBaseWriteAccessor : IFormatBaseAccessor {
		Model.IFormatBaseBatchUpdateable formatBase;
		public FormatBaseWriteAccessor(Model.IFormatBaseBatchUpdateable formatBase) {
			this.formatBase = formatBase;
		}
		#region IFormatBaseAccessor Members
		public Model.IFormatBaseBatchUpdateable ReadOnlyFormat { get { return formatBase; } }
		public Model.IFormatBaseBatchUpdateable ReadWriteFormat { get { return formatBase; } }
		#endregion
	}
}
