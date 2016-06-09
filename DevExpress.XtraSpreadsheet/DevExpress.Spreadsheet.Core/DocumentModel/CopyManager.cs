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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public enum ModelPasteSpecialFlags {
		Formulas = 1, 
		Values = 2, 
		NumberFormats = 4, 
		Borders = 8, 
		OtherFormats = 0x10, 
		Style = 0x20,
		FormatAndStyle = 0x3C,
		Comments = 0x40,
		ColumnWidths = 0x80,
		RowHeight = 0x100,
		Tables = 0x200,   
		All = 0xFF7F, 
		AllExceptBorders = 0xFF77,  
		FormulasAndNumberFormats = 5, 
		ValuesAndNumberFormats = 6 
	}
	public class ModelPasteSpecialOptions {
		uint packedValues;
		bool shouldSkipEmptyCellsInSourceRange;
		public ModelPasteSpecialOptions()
			: this(ModelPasteSpecialFlags.All) {
		}
		public ModelPasteSpecialOptions(ModelPasteSpecialFlags options) {
			this.packedValues = (uint)options;
		}
		public bool ShouldCopyFormulas { get { return GetBooleanVal(ModelPasteSpecialFlags.Formulas); } }
		public bool ShouldCopyValues { get { return GetBooleanVal(ModelPasteSpecialFlags.Values); } }
		public bool ShouldCopyFormatAndStyle { get { return GetBooleanVal(ModelPasteSpecialFlags.FormatAndStyle); } }
		public bool ShouldCopyStyle { get { return GetBooleanVal(ModelPasteSpecialFlags.Style); } }
		public bool ShouldCopyOtherFormats { get { return GetBooleanVal(ModelPasteSpecialFlags.OtherFormats); } }
		public bool ShouldCopyNumberFormat { get { return GetBooleanVal(ModelPasteSpecialFlags.NumberFormats); } }
		public bool ShouldCopyBorders { get { return GetBooleanVal(ModelPasteSpecialFlags.Borders); } }
		public bool ShouldCopyComments { get { return GetBooleanVal(ModelPasteSpecialFlags.Comments); } }
		public bool ShouldCopyColumnWidths { get { return GetBooleanVal(ModelPasteSpecialFlags.ColumnWidths); } }
		public bool ShouldCopyRowInfoAndHeightInfo { get { return GetBooleanVal(ModelPasteSpecialFlags.RowHeight); } }
		public bool ShouldCopyTables { get { return GetBooleanVal(ModelPasteSpecialFlags.Tables); } }
		public bool ShouldCopyAll { get { return GetBooleanVal(ModelPasteSpecialFlags.All); } }
		public bool ShouldCopyAllExceptBorders { get { return GetBooleanVal(ModelPasteSpecialFlags.AllExceptBorders); } }
		public ModelPasteSpecialFlags InnerFlags { get { return (ModelPasteSpecialFlags)packedValues; } set { this.packedValues = (uint)value; } }
		#region GetBooleanVal helper
		bool GetBooleanVal(ModelPasteSpecialFlags mask) {
			uint maskCasted = (uint)mask;
			return (packedValues & maskCasted) == maskCasted;
		}
		#endregion
		public bool ShouldSkipEmptyCellsInSourceRange { get { return shouldSkipEmptyCellsInSourceRange; } set { shouldSkipEmptyCellsInSourceRange = value; } }
	}
}
