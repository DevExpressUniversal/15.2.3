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

using System.ComponentModel;
using System;
namespace DevExpress.Utils.UI.Localization {
	#region enum UtilsUIStringId
	public enum UtilsUIStringId {
		Cmd_AddCalculatedField,
		Cmd_EditCalculatedFields,
		Cmd_ClearCalculatedFields,
		Cmd_AddParameter,
		Cmd_EditParameters,
		Cmd_ClearParameters,
		Cmd_DeleteParameter,
		Cmd_EditExpression,
		Cmd_DeleteCalculatedField,
		Parameter_Type_String,
		Parameter_Type_DateTime,
		Parameter_Type_Int16,
		Parameter_Type_Int32,
		Parameter_Type_Int64,
		Parameter_Type_Float,
		Parameter_Type_Double,
		Parameter_Type_Decimal,
		Parameter_Type_Boolean,
		Parameter_Type_Guid,
		NonePickerNodeText,
		FSForm_Lbl_Category,
		FSForm_Tab_StandardTypes,
		FSForm_Tab_Custom,
		FSForm_GrBox_Sample,
		FSForm_Btn_Delete,
		FSForm_Btn_Ok,
		FSForm_Btn_Cancel,
		FSForm_Lbl_Suffix,
		FSForm_Lbl_Prefix,
		FSForm_Lbl_CustomGeneral,
		FSForm_Text,
		FSForm_Msg_BadSymbol,
		FSForm_Cat_Special,
		FSForm_Cat_Percent,
		FSForm_Cat_Number,
		FSForm_Cat_Int32,
		FSForm_Cat_General,
		FSForm_Cat_DateTime,
		FSForm_Cat_Currency,
		Msg_ErrorTitle,
		Msg_ContainsIllegalSymbols,
		PropGrid_TTip_Alphabetical,
		PropGrid_TTip_Categorized,
		CollectionEditor_Cancel,
		ParameterCollectionEditor_Title,
	}
	#endregion
	#region UtilsUILocalizer.AddStrings 
	public partial class UtilsUILocalizer {
		void AddStrings() {
			AddString(UtilsUIStringId.Cmd_AddCalculatedField, "Add Calculated Field");
			AddString(UtilsUIStringId.Cmd_EditCalculatedFields, "Edit Calculated Fields...");
			AddString(UtilsUIStringId.Cmd_ClearCalculatedFields, "Remove All Calculated Fields");
			AddString(UtilsUIStringId.Cmd_AddParameter, "Add Parameter");
			AddString(UtilsUIStringId.Cmd_EditParameters, "Edit Parameters...");
			AddString(UtilsUIStringId.Cmd_ClearParameters, "Remove All Parameters");
			AddString(UtilsUIStringId.Cmd_DeleteParameter, "Delete");
			AddString(UtilsUIStringId.Cmd_EditExpression, "Edit Expression...");
			AddString(UtilsUIStringId.Cmd_DeleteCalculatedField, "Delete");
			AddString(UtilsUIStringId.Parameter_Type_String, "String");
			AddString(UtilsUIStringId.Parameter_Type_DateTime, "Date");
			AddString(UtilsUIStringId.Parameter_Type_Int16, "Number (16 bit integer)");
			AddString(UtilsUIStringId.Parameter_Type_Int32, "Number (32 bit integer)");
			AddString(UtilsUIStringId.Parameter_Type_Int64, "Number (64 bit integer)");
			AddString(UtilsUIStringId.Parameter_Type_Float, "Number (floating-point)");
			AddString(UtilsUIStringId.Parameter_Type_Double, "Number (double-precision floating-point)");
			AddString(UtilsUIStringId.Parameter_Type_Decimal, "Number (decimal)");
			AddString(UtilsUIStringId.Parameter_Type_Boolean, "Boolean");
			AddString(UtilsUIStringId.Parameter_Type_Guid, "Guid");
			AddString(UtilsUIStringId.NonePickerNodeText, "None");
			AddString(UtilsUIStringId.FSForm_Lbl_Category, "Category");
			AddString(UtilsUIStringId.FSForm_Tab_StandardTypes, "Standard Types");
			AddString(UtilsUIStringId.FSForm_Tab_Custom, "Custom");
			AddString(UtilsUIStringId.FSForm_GrBox_Sample, "Sample");
			AddString(UtilsUIStringId.FSForm_Btn_Delete, "Delete");
			AddString(UtilsUIStringId.FSForm_Btn_Ok, "OK");
			AddString(UtilsUIStringId.FSForm_Btn_Cancel, "Cancel");
			AddString(UtilsUIStringId.FSForm_Lbl_Suffix, "Suffix:");
			AddString(UtilsUIStringId.FSForm_Lbl_Prefix, "Prefix:");
			AddString(UtilsUIStringId.FSForm_Lbl_CustomGeneral, "General format has no specific number format");
			AddString(UtilsUIStringId.FSForm_Text, "FormatString Editor");
			AddString(UtilsUIStringId.FSForm_Msg_BadSymbol, "Error: Illegal symbol(s)");
			AddString(UtilsUIStringId.FSForm_Cat_Special, "Special");
			AddString(UtilsUIStringId.FSForm_Cat_Percent, "Percent");
			AddString(UtilsUIStringId.FSForm_Cat_Number, "Number");
			AddString(UtilsUIStringId.FSForm_Cat_Int32, "Int32");
			AddString(UtilsUIStringId.FSForm_Cat_General, "General");
			AddString(UtilsUIStringId.FSForm_Cat_DateTime, "DateTime");
			AddString(UtilsUIStringId.FSForm_Cat_Currency, "Currency");
			AddString(UtilsUIStringId.Msg_ErrorTitle, "Error");
			AddString(UtilsUIStringId.Msg_ContainsIllegalSymbols, "Input format string contains illegal symbol(s).");
			AddString(UtilsUIStringId.PropGrid_TTip_Alphabetical, "Alphabetical");
			AddString(UtilsUIStringId.PropGrid_TTip_Categorized, "Categorized");
			AddString(UtilsUIStringId.CollectionEditor_Cancel, "Add or remove {0} objects");
			AddString(UtilsUIStringId.ParameterCollectionEditor_Title, "Parameters");
		}
	}
	 #endregion
}
