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
namespace DevExpress.Spreadsheet {
	public interface TableStyle {
		bool BuiltIn { get; }
		bool IsTableStyle { get; set; }
		bool IsPivotStyle { get; set; }
		TableStyle Duplicate();
		TableStyle Duplicate(string name);
		string Name { get; set; }
		TableStyleElements TableStyleElements { get; }
		void BeginUpdate();
		void EndUpdate();
	}
	public interface TableStyleElements {
		[Obsolete("Use the Enum.GetValues(typeof(TableStyleElementType)).Length property instead.", true)]
		int Count { get; }
		TableStyleElement this[TableStyleElementType index] { get; }
	}
	public interface TableStyleElement {
		Borders Borders { get; }
		SpreadsheetFont Font { get; }
		Fill Fill { get; }
		void Clear();
		int StripeSize { get; set; }
	}
	public enum TableStyleElementType {
		WholeTable = DevExpress.XtraSpreadsheet.Model.TableStyle.WholeTableIndex,
		HeaderRow = DevExpress.XtraSpreadsheet.Model.TableStyle.HeaderRowIndex,
		TotalRow = DevExpress.XtraSpreadsheet.Model.TableStyle.TotalRowIndex,
		FirstColumn = DevExpress.XtraSpreadsheet.Model.TableStyle.FirstColumnIndex,
		LastColumn = DevExpress.XtraSpreadsheet.Model.TableStyle.LastColumnIndex,
		FirstRowStripe = DevExpress.XtraSpreadsheet.Model.TableStyle.FirstRowStripeIndex,
		SecondRowStripe = DevExpress.XtraSpreadsheet.Model.TableStyle.SecondRowStripeIndex,
		FirstColumnStripe = DevExpress.XtraSpreadsheet.Model.TableStyle.FirstColumnStripeIndex,
		SecondColumnStripe = DevExpress.XtraSpreadsheet.Model.TableStyle.SecondColumnStripeIndex,
		FirstHeaderCell = DevExpress.XtraSpreadsheet.Model.TableStyle.FirstHeaderCellIndex,
		LastHeaderCell = DevExpress.XtraSpreadsheet.Model.TableStyle.LastHeaderCellIndex,
		FirstTotalCell = DevExpress.XtraSpreadsheet.Model.TableStyle.FirstTotalCellIndex,
		LastTotalCell = DevExpress.XtraSpreadsheet.Model.TableStyle.LastTotalCellIndex,
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	#endregion
	partial class NativeTableStyle : TableStyle {
		Model.TableStyle modelTableStyle;
		NativeWorkbook wb;
		NativeTableStyleElements tableStyleElements;
		bool isValid;
		public NativeTableStyle(NativeWorkbook wb, Model.TableStyle modelTableStyle) {
			this.wb = wb;
			this.modelTableStyle = modelTableStyle;
			tableStyleElements = null;
		}
		public Model.TableStyle ModelTableStyle { get { return modelTableStyle; } }
		protected NativeWorkbook NativeWorkbook { get { return wb; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		void CheckDuplicateStyleName(string name) {
			if (modelTableStyle.DocumentModel.StyleSheet.TableStyles.ContainsStyleName(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDuplicateStyleName);
		}
		#region TableStyle Members
		public bool BuiltIn {
			get { return ModelTableStyle.IsPredefined; }
		}
		public TableStyle Duplicate() {
			return DuplicateCore(modelTableStyle.Duplicate());
		}
		public TableStyle Duplicate(string name) {
			CheckDuplicateStyleName(name);
			return DuplicateCore(modelTableStyle.Duplicate(name));
		}
		TableStyle DuplicateCore(Model.TableStyle modelStyle) {
			NativeWorkbook.DocumentModel.StyleSheet.TableStyles.Add(modelStyle);
			return NativeWorkbook.TableStyles[modelStyle.Name.Name];
		}
		public string Name {
			get { return modelTableStyle.Name.Name; }
			set {
				if (BuiltIn)
					throw new InvalidOperationException();
				CheckDuplicateStyleName(value);
				modelTableStyle.Name = Model.TableStyleName.CreateCustom(value); 
			}
		}
		public TableStyleElements TableStyleElements {
			get {
				if (tableStyleElements == null)
					tableStyleElements = new NativeTableStyleElements(this);
				return tableStyleElements;
			}
		}
		public void BeginUpdate() {
			modelTableStyle.BeginUpdate();
		}
		public void EndUpdate() {
			modelTableStyle.EndUpdate();
		}
		#endregion
		public override bool Equals(object obj) {
			NativeTableStyle otherTableStyle = obj as NativeTableStyle;
			if (otherTableStyle == null)
				return false;
			return ModelTableStyle.Equals(otherTableStyle.ModelTableStyle);
		}
		public override int GetHashCode() {
			return ModelTableStyle.GetHashCode();
		}
		public override string ToString() {
			return base.ToString();
		}
		public bool IsTableStyle {
			get {
				return modelTableStyle.IsTableStyle || modelTableStyle.IsGeneralStyle;
			}
			set {
				if (!value && !IsPivotStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorStyleTypeMustBeTableOrPivot);
				Model.TableStyleElementIndexTableType tableType;
				if (value)
					tableType = IsPivotStyle ? Model.TableStyleElementIndexTableType.General : Model.TableStyleElementIndexTableType.Table;
				else
					tableType = Model.TableStyleElementIndexTableType.Pivot;
				modelTableStyle.TableType = tableType;
			}
		}
		public bool IsPivotStyle {
			get {
				return modelTableStyle.IsPivotStyle || modelTableStyle.IsGeneralStyle;
			}
			set {
				if (!value && !IsTableStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorStyleTypeMustBeTableOrPivot);
				Model.TableStyleElementIndexTableType tableType;
				if (value)
					tableType = IsTableStyle ? Model.TableStyleElementIndexTableType.General : Model.TableStyleElementIndexTableType.Pivot;
				else
					tableType = Model.TableStyleElementIndexTableType.Table;
				modelTableStyle.TableType = tableType;
			}
		}
	}
	partial class NativeTableStyleElements : TableStyleElements {
		readonly NativeTableStyle nativeTableStyle;
		public NativeTableStyleElements(NativeTableStyle nativeTableStyle) {
			Guard.ArgumentNotNull(nativeTableStyle, "nativeTableStyle");
			this.nativeTableStyle = nativeTableStyle;
		}
		protected Model.TableStyle ModelTableStyle { get { return nativeTableStyle.ModelTableStyle; } }
		protected NativeTableStyle NativeTableStyle { get { return nativeTableStyle; } }
		public int Count { get { return Model.TableStyle.TableStyleIndexAccessors.Length; } }
		public TableStyleElement this[TableStyleElementType index] {
			get { return new NativeTableStyleElement(nativeTableStyle, index); }
		}
	}
	partial class NativeTableStyleElement : NativeTableStyleElementBase, TableStyleElement {
		public NativeTableStyleElement(NativeTableStyle nativeTableStyle, TableStyleElementType type)
			: base(nativeTableStyle, type) {
		}
		#region TableStyleElement Members
		public Borders Borders { get { return new NativeTableStyleBorders(NativeTableStyle, TableStyleElementType); } }
		public SpreadsheetFont Font { get { return new NativeTableStyleFont(NativeTableStyle, TableStyleElementType); } }
		public Fill Fill { get { return new NativeTableStyleFill(NativeTableStyle, TableStyleElementType); } }
		public int StripeSize {
			get { return ModelTableStyleElementFormat.StripeSize; }
			set {
				CheckValidOperation();
				ModelTableStyleElementFormat.StripeSize = value;
			}
		}
		public void Clear() {
			base.CheckValidOperation();
			ModelTableStyle.ClearElementFormat(ModelElementIndex);
		}
		#endregion
		protected override void CheckValidOperation() {
			base.CheckValidOperation();
			if (!IsValidModifyStripeSizeOperation())
				throw new InvalidOperationException();
		}
		bool IsValidModifyStripeSizeOperation() {	
			return
				TableStyleElementType == TableStyleElementType.FirstColumnStripe ||
				TableStyleElementType == TableStyleElementType.SecondColumnStripe ||
				TableStyleElementType == TableStyleElementType.FirstRowStripe ||
				TableStyleElementType == TableStyleElementType.SecondRowStripe;
		}
	}
}
