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

using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization.Internal;
using System.Windows.Markup;
using DevExpress.Utils.Localization;
using System.Resources;
using System;
namespace DevExpress.Xpf.Editors {
	public enum EditorStringId {
		#region common
		OK,
		Cancel,
		Clear,
		Apply,
		Yes,
		No,
		Copy,
		Paste,
		Cut,
		Open,
		Save,
		CaptionError,
		SetNullValue,
		#endregion
		#region validation
		OutOfRange,
		MaskIncomplete,
		#endregion
		#region ComboBoxEdit
		SelectAll,
		EmptyItem,
		#endregion
		#region Calculator
		CalculatorButtonMC,
		CalculatorButtonMR,
		CalculatorButtonMS,
		CalculatorButtonMAdd,
		CalculatorButtonMSub,
		CalculatorButtonBack,
		CalculatorButtonCancel,
		CalculatorButtonClear,
		CalculatorButtonZero,
		CalculatorButtonOne,
		CalculatorButtonTwo,
		CalculatorButtonThree,
		CalculatorButtonFour,
		CalculatorButtonFive,
		CalculatorButtonSix,
		CalculatorButtonSeven,
		CalculatorButtonEight,
		CalculatorButtonNine,
		CalculatorButtonSign,
		CalculatorButtonAdd,
		CalculatorButtonSub,
		CalculatorButtonMul,
		CalculatorButtonDiv,
		CalculatorButtonFract,
		CalculatorButtonPercent,
		CalculatorButtonSqrt,
		CalculatorButtonEqual,
		CalculatorDivisionByZeroError,
		CalculatorError,
		CalculatorInvalidInputError,
		#endregion
		#region ColorEdit
		ColorEdit_AutomaticButtonCaption,
		ColorEdit_MoreColorsButtonCaption,
		ColorEdit_NoColorButtonCaption,
		ColorEdit_RecentColorsCaption,
		ColorEdit_ColorChooserWindowTitle,
		ColorEdit_ThemeColorsCaption,
		ColorEdit_StandardColorsCaption,
		ColorEdit_DefaultColors_Black,
		ColorEdit_DefaultColors_White,
		ColorEdit_DefaultColors_DarkRed,
		ColorEdit_DefaultColors_Red,
		ColorEdit_DefaultColors_Orange,
		ColorEdit_DefaultColors_Yellow,
		ColorEdit_DefaultColors_LightGreen,
		ColorEdit_DefaultColors_Green,
		ColorEdit_DefaultColors_LightBlue,
		ColorEdit_DefaultColors_Blue,
		ColorEdit_DefaultColors_DarkBlue,
		ColorEdit_DefaultColors_Purple,
		#endregion
		#region ImageEdit
		ImageEdit_OpenFileFilter,
		ImageEdit_OpenFileFilter_SL,
		ImageEdit_SaveFileFilter,
		ImageEdit_InvalidFormatMessage,
		#endregion
		#region filter ids
		FilterCriteriaToStringFunctionIsNullOrEmpty,
		FilterCriteriaToStringFunctionStartsWith,
		FilterCriteriaToStringFunctionEndsWith,
		FilterCriteriaToStringFunctionContains,
		FilterCriteriaToStringBetween,
		FilterCriteriaToStringIn,
		FilterCriteriaToStringIsNotNull,
		FilterCriteriaToStringNotLike,
		FilterCriteriaToStringBinaryOperatorBitwiseAnd,
		FilterCriteriaToStringBinaryOperatorBitwiseOr,
		FilterCriteriaToStringBinaryOperatorBitwiseXor,
		FilterCriteriaToStringBinaryOperatorDivide,
		FilterCriteriaToStringBinaryOperatorEqual,
		FilterCriteriaToStringBinaryOperatorGreater,
		FilterCriteriaToStringBinaryOperatorGreaterOrEqual,
		FilterCriteriaToStringBinaryOperatorLess,
		FilterCriteriaToStringBinaryOperatorLessOrEqual,
		FilterCriteriaToStringBinaryOperatorLike,
		FilterCriteriaToStringBinaryOperatorMinus,
		FilterCriteriaToStringBinaryOperatorModulo,
		FilterCriteriaToStringBinaryOperatorMultiply,
		FilterCriteriaToStringBinaryOperatorNotEqual,
		FilterCriteriaToStringBinaryOperatorPlus,
		FilterCriteriaToStringUnaryOperatorBitwiseNot,
		FilterCriteriaToStringUnaryOperatorIsNull,
		FilterCriteriaToStringUnaryOperatorMinus,
		FilterCriteriaToStringUnaryOperatorNot,
		FilterCriteriaToStringUnaryOperatorPlus,
		FilterCriteriaToStringGroupOperatorAnd,
		FilterCriteriaToStringGroupOperatorOr,
		FilterCriteriaInvalidExpression,
		FilterCriteriaInvalidExpressionEx,
		#endregion
		#region expression editor
		ExpressionEditor_Functions_Text,
		ExpressionEditor_Parameters_Text,
		ExpressionEditor_Variables_Text,
		ExpressionEditor_Operators_Text,
		ExpressionEditor_Fields_Text,
		ExpressionEditor_Constants_Text,
		ExpressionEditor_FunctionType_AllItems,
		ExpressionEditor_FunctionType_AggregateItems,
		ExpressionEditor_FunctionType_DateTimeItems,
		ExpressionEditor_FunctionType_LogicalItems,
		ExpressionEditor_FunctionType_MathItems,
		ExpressionEditor_FunctionType_StringItems,
		ExpressionEditor_Abs_Description,
		ExpressionEditor_Acos_Description,
		ExpressionEditor_AddDays_Description,
		ExpressionEditor_AddHours_Description,
		ExpressionEditor_AddMilliSeconds_Description,
		ExpressionEditor_AddMinutes_Description,
		ExpressionEditor_AddMonths_Description,
		ExpressionEditor_AddSeconds_Description,
		ExpressionEditor_AddTicks_Description,
		ExpressionEditor_AddTimeSpan_Description,
		ExpressionEditor_AddYears_Description,
		ExpressionEditor_Ascii_Description,
		ExpressionEditor_Asin_Description,
		ExpressionEditor_Atn_Description,
		ExpressionEditor_Atn2_Description,
		ExpressionEditor_BigMul_Description,
		ExpressionEditor_Ceiling_Description,
		ExpressionEditor_Char_Description,
		ExpressionEditor_CharIndex_Description,
		ExpressionEditor_CharIndex3Param_Description,
		ExpressionEditor_Concat_Description,
		ExpressionEditor_Cos_Description,
		ExpressionEditor_Cosh_Description,
		ExpressionEditor_DateDiffDay_Description,
		ExpressionEditor_DateDiffHour_Description,
		ExpressionEditor_DateDiffMilliSecond_Description,
		ExpressionEditor_DateDiffMinute_Description,
		ExpressionEditor_DateDiffMonth_Description,
		ExpressionEditor_DateDiffSecond_Description,
		ExpressionEditor_DateDiffTick_Description,
		ExpressionEditor_DateDiffYear_Description,
		ExpressionEditor_Exp_Description,
		ExpressionEditor_Floor_Description,
		ExpressionEditor_GetDate_Description,
		ExpressionEditor_GetDay_Description,
		ExpressionEditor_GetDayOfWeek_Description,
		ExpressionEditor_GetDayOfYear_Description,
		ExpressionEditor_GetHour_Description,
		ExpressionEditor_GetMilliSecond_Description,
		ExpressionEditor_GetMinute_Description,
		ExpressionEditor_GetMonth_Description,
		ExpressionEditor_GetSecond_Description,
		ExpressionEditor_GetTimeOfDay_Description,
		ExpressionEditor_GetYear_Description,
		ExpressionEditor_Iif_Description,
		ExpressionEditor_Insert_Description,
		ExpressionEditor_IsNull_Description,
		ExpressionEditor_IsNullOrEmpty_Description,
		ExpressionEditor_Len_Description,
		ExpressionEditor_Log_Description,
		ExpressionEditor_Log2Param_Description,
		ExpressionEditor_Log10_Description,
		ExpressionEditor_Lower_Description,
		ExpressionEditor_Now_Description,
		ExpressionEditor_PadLeft_Description,
		ExpressionEditor_PadLeft3Param_Description,
		ExpressionEditor_PadRight_Description,
		ExpressionEditor_PadRight3Param_Description,
		ExpressionEditor_Power_Description,
		ExpressionEditor_Remove2Param_Description,
		ExpressionEditor_Remove3Param_Description,
		ExpressionEditor_Replace_Description,
		ExpressionEditor_Reverse_Description,
		ExpressionEditor_Rnd_Description,
		ExpressionEditor_Round_Description,
		ExpressionEditor_Round2Param_Description,
		ExpressionEditor_Sign_Description,
		ExpressionEditor_Sin_Description,
		ExpressionEditor_Sinh_Description,
		ExpressionEditor_Sqr_Description,
		ExpressionEditor_Substring3param_Description,
		ExpressionEditor_Substring2param_Description,
		ExpressionEditor_Tan_Description,
		ExpressionEditor_Tanh_Description,
		ExpressionEditor_Today_Description,
		ExpressionEditor_ToInt_Description,
		ExpressionEditor_ToLong_Description,
		ExpressionEditor_ToFloat_Description,
		ExpressionEditor_ToDouble_Description,
		ExpressionEditor_ToDecimal_Description,
		ExpressionEditor_ToStr_Description,
		ExpressionEditor_Trim_Description,
		ExpressionEditor_Upper_Description,
		ExpressionEditor_UtcNow_Description,
		ExpressionEditor_LocalDateTimeDayAfterTomorrow_Description,
		ExpressionEditor_LocalDateTimeLastWeek_Description,
		ExpressionEditor_LocalDateTimeNextMonth_Description,
		ExpressionEditor_LocalDateTimeNextWeek_Description,
		ExpressionEditor_LocalDateTimeNextYear_Description,
		ExpressionEditor_LocalDateTimeNow_Description,
		ExpressionEditor_LocalDateTimeThisMonth_Description,
		ExpressionEditor_LocalDateTimeThisWeek_Description,
		ExpressionEditor_LocalDateTimeThisYear_Description,
		ExpressionEditor_LocalDateTimeToday_Description,
		ExpressionEditor_LocalDateTimeTomorrow_Description,
		ExpressionEditor_LocalDateTimeTwoWeeksAway_Description,
		ExpressionEditor_LocalDateTimeYesterday_Description,
		ExpressionEditor_Plus_Description,
		ExpressionEditor_Minus_Description,
		ExpressionEditor_Multiply_Description,
		ExpressionEditor_Divide_Description,
		ExpressionEditor_Modulo_Description,
		ExpressionEditor_BitwiseOr_Description,
		ExpressionEditor_BitwiseAnd_Description,
		ExpressionEditor_BitwiseXor_Description,
		ExpressionEditor_Equal_Description,
		ExpressionEditor_NotEqual_Description,
		ExpressionEditor_Less_Description,
		ExpressionEditor_LessOrEqual_Description,
		ExpressionEditor_GreaterOrEqual_Description,
		ExpressionEditor_Greater_Description,
		ExpressionEditor_In_Description,
		ExpressionEditor_Like_Description,
		ExpressionEditor_Between_Description,
		ExpressionEditor_And_Description,
		ExpressionEditor_Or_Description,
		ExpressionEditor_Not_Description,
		ExpressionEditor_Max_Description,
		ExpressionEditor_Min_Description,
		ExpressionEditor_StartsWith_Description,
		ExpressionEditor_EndsWith_Description,
		ExpressionEditor_Contains_Description,
		ExpressionEditor_IsThisWeek_Description,
		ExpressionEditor_IsThisMonth_Description,
		ExpressionEditor_IsThisYear_Description,
		ExpressionEditor_GridFields_Description_Prefix,
		ExpressionEditor_True_Description,
		ExpressionEditor_False_Description,
		ExpressionEditor_Null_Description,
		ExpressionEditor_Title,
		ExpressionEditor_AvgAggregate_Description,
		ExpressionEditor_CountAggregate_Description,
		ExpressionEditor_ExistsAggregate_Description,
		ExpressionEditor_MaxAggregate_Description,
		ExpressionEditor_MinAggregate_Description,
		ExpressionEditor_SumAggregate_Description,
		ExpressionEditor_SingleAggregate_Description,
		#endregion
		#region filter control
		FilterGroupAnd,
		FilterGroupOr,
		FilterGroupNotAnd,
		FilterGroupNotOr,
		FilterGroupAddCondition,
		FilterGroupAddGroup,
		FilterGroupRemoveGroup,
		FilterGroupClearAll,
		FilterEmptyValueText,
		FilterClauseAnyOf,
		FilterClauseBeginsWith,
		FilterClauseBetween,
		FilterClauseBetweenAnd,
		FilterClauseContains,
		FilterClauseEndsWith,
		FilterClauseEquals,
		FilterClauseGreater,
		FilterClauseGreaterOrEqual,
		FilterClauseIsNotNull,
		FilterClauseIsNull,
		FilterClauseLess,
		FilterClauseLessOrEqual,
		FilterClauseLike,
		FilterClauseNoneOf,
		FilterClauseNotBetween,
		FilterClauseDoesNotContain,
		FilterClauseDoesNotEqual,
		FilterClauseNotLike,
		FilterClauseIsNullOrEmpty,
		FilterClauseIsNotNullOrEmpty,
		FilterClauseIsBeyondThisYear,
		FilterClauseIsLaterThisYear,
		FilterClauseIsLaterThisMonth,
		FilterClauseIsNextWeek,
		FilterClauseIsLaterThisWeek,
		FilterClauseIsTomorrow,
		FilterClauseIsToday,
		FilterClauseIsYesterday,
		FilterClauseIsEarlierThisWeek,
		FilterClauseIsLastWeek,
		FilterClauseIsEarlierThisMonth,
		FilterClauseIsEarlierThisYear,
		FilterClauseIsPriorThisYear,
		FilterClauseLocalDateTimeThisYear,
		FilterClauseLocalDateTimeThisMonth,
		FilterClauseLocalDateTimeLastWeek,
		FilterClauseLocalDateTimeThisWeek,
		FilterClauseLocalDateTimeYesterday,
		FilterClauseLocalDateTimeToday,
		FilterClauseLocalDateTimeNow,
		FilterClauseLocalDateTimeTomorrow,
		FilterClauseLocalDateTimeDayAfterTomorrow,
		FilterClauseLocalDateTimeNextWeek,
		FilterClauseLocalDateTimeTwoWeeksAway,
		FilterClauseLocalDateTimeNextMonth,
		FilterClauseLocalDateTimeNextYear,
		FilterDateTimeOperatorMenuCaption,
		FilterEditorChecked,
		FilterEditorUnchecked,
		FilterToolTipNodeAction,
		FilterToolTipNodeAdd,
		FilterToolTipNodeRemove,
		FilterToolTipValueType,
		FilterToolTipElementAdd,
		FilterToolTipKeysAdd,
		FilterToolTipKeysRemove,
		#endregion
		#region FilterPanel Control Strings
		FilterPanelEditFilter,
		FilterPanelClearFilter,
		FilterPanelEnableFilter,
		FilterPanelDisableFilter,
		#endregion
		#region PasswordBoxEdit
		PasswordBoxEditToolTipHeader,
		PasswordBoxEditToolTipContent,
		#endregion
		#region WaitIndicator
		WaitIndicatorText,
		#endregion
		#region DateNavigator
		CantModifySelectedDates,
		#endregion
		#region DataPager
		Page,
		Of,
		#endregion
		#region DisplayFormatTextControl
		DisplayFormatTextControlExample,
		DisplayFormatTextControlPrefix,
		DisplayFormatTextControlSuffix,
		DisplayFormatTextControlDisplayFormatText,
		DisplayFormatHelperWrongDisplayFormatValue,
		DataTypeStringExample,
		DataTypeCharExample,
		DisplayFormatNullValue,
		DisplayFormatGroupTypeDefault,
		DisplayFormatGroupTypeNumber,
		DisplayFormatGroupTypePercent,
		DisplayFormatGroupTypeCurrency,
		DisplayFormatGroupTypeSpecial,
		DisplayFormatGroupTypeDatetime,
		DisplayFormatGroupTypeCustom,
		#endregion
		#region LookUp
		LookUpFind,
		LookUpSearch,
		LookUpClose,
		LookUpAddNew,
		#endregion
		#region FontEdit
		ConfirmationDialogMessage,
		ConfirmationDialogCaption,
		#endregion
		#region CheckEdit
		CheckChecked,
		CheckUnchecked,
		CheckIndeterminate,
		#endregion
		InvalidValueConversion,
		Today,
		#region SparklineEdit
		SparklineViewArea,
		SparklineViewBar,
		SparklineViewLine,
		SparklineViewWinLoss,
		#endregion
		#region ColorPicker
		CMYK,
		RGB,
		HLS,
		HSB,
		ColorPickerRed,
		ColorPickerGreen,
		ColorPickerBlue,
		ColorPickerAlpha,
		ColorPickerCyan,
		ColorPickerMagenta,
		ColorPickerYellow,
		ColorPickerKeyColor,
		ColorPickerHue,
		ColorPickerSaturation,
		ColorPickerLightness,
		ColorPickerBrightness,
		#endregion
		#region Character Map
		Caption_CommonCharactersToggleButton,
		Caption_SpecialCharactersToggleButton,
		Caption_SymbolFormSearchByCode,
		Caption_SymbolFormFontName,
		Caption_SymbolFormCharacterSet,
		Caption_SymbolFormFilter,
		#endregion
		#region DateTimePicker
		DatePickerHours,
		DatePickerMinutes,
		DatePickerSeconds,
		DatePickerMilliseconds,
		#endregion
		#region BrushEdit 
		BrushEditNone,
		BrushEditSolid,
		BrushEditLinear,
		BrushEditRadial,
		BrushEditStartPoint,
		BrushEditStartPointDescription,
		BrushEditEndPoint,
		BrushEditEndPointDescription,
		BrushEditMappingMode,
		BrushEditMappingModeDescription,
		BrushEditSpreadMethod,
		BrushEditSpreadMethodDescription,
		BrushEditGradientOrigin,
		BrushEditGradientOriginDescription,
		BrushEditCenter,
		BrushEditCenterDescription,
		BrushEditRadiusX,
		BrushEditRadiusXDescription,
		BrushEditRadiusY,
		BrushEditRadiusYDescription,
		#endregion
		TokenEditorNewTokenText,
#if SL
		ListBoxSelectAllSelectionMode,
#endif
		CameraSettingsCaption,
		CameraResolutionCaption,
		CameraBrightnessCaption,
		CameraContrastCaption,
		CameraDesaturateCaption,
		CameraResetButtonCaption,
		CameraDeviceCaption,
		CameraAgainButtonCaption,
		CameraTakePictureCaption,
		CameraTakePictureToolTip,
		CameraCaptureButtonCaption,
		CameraErrorCaption,
		CameraRefreshButtonCaption,
		CameraNoDevicesErrorCaption
	}
	public class EditorLocalizer : DXLocalizer<EditorStringId> {
		public new static XtraLocalizer<EditorStringId> Active {
			get { return XtraLocalizer<EditorStringId>.Active; }
			set { XtraLocalizer<EditorStringId>.Active = value; }
		}
		static EditorLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<EditorStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			#region validation
			AddString(EditorStringId.OutOfRange, "Value is out of range");
			AddString(EditorStringId.MaskIncomplete, "Mask incomplete");
			#endregion
			#region ComboBoxEdit
			AddString(EditorStringId.SelectAll, "(Select All)");
			AddString(EditorStringId.EmptyItem, "(None)");
			#endregion
			#region common
			AddString(EditorStringId.OK, "OK");
			AddString(EditorStringId.Cancel, "Cancel");
			AddString(EditorStringId.Clear, "Clear");
			AddString(EditorStringId.Apply, "Apply");
			AddString(EditorStringId.Yes, "Yes");
			AddString(EditorStringId.No, "No");
			AddString(EditorStringId.Open, "Open");
			AddString(EditorStringId.Save, "Save");
			AddString(EditorStringId.Today, "Today");
			AddString(EditorStringId.Cut, "Cut");
			AddString(EditorStringId.Copy, "Copy");
			AddString(EditorStringId.Paste, "Paste");
			AddString(EditorStringId.CaptionError, "Error");
			AddString(EditorStringId.SetNullValue, "Clear");
			#endregion
			#region Calculator
			AddString(EditorStringId.CalculatorButtonMC, "MC");
			AddString(EditorStringId.CalculatorButtonMR, "MR");
			AddString(EditorStringId.CalculatorButtonMS, "MS");
			AddString(EditorStringId.CalculatorButtonMAdd, "M+");
			AddString(EditorStringId.CalculatorButtonMSub, "M-");
			AddString(EditorStringId.CalculatorButtonBack, "←");
			AddString(EditorStringId.CalculatorButtonCancel, "CE");
			AddString(EditorStringId.CalculatorButtonClear, "C");
			AddString(EditorStringId.CalculatorButtonZero, "0");
			AddString(EditorStringId.CalculatorButtonOne, "1");
			AddString(EditorStringId.CalculatorButtonTwo, "2");
			AddString(EditorStringId.CalculatorButtonThree, "3");
			AddString(EditorStringId.CalculatorButtonFour, "4");
			AddString(EditorStringId.CalculatorButtonFive, "5");
			AddString(EditorStringId.CalculatorButtonSix, "6");
			AddString(EditorStringId.CalculatorButtonSeven, "7");
			AddString(EditorStringId.CalculatorButtonEight, "8");
			AddString(EditorStringId.CalculatorButtonNine, "9");
			AddString(EditorStringId.CalculatorButtonSign, "±");
			AddString(EditorStringId.CalculatorButtonAdd, "+");
			AddString(EditorStringId.CalculatorButtonSub, "-");
			AddString(EditorStringId.CalculatorButtonMul, "*");
			AddString(EditorStringId.CalculatorButtonDiv, "/");
			AddString(EditorStringId.CalculatorButtonFract, "1/x");
			AddString(EditorStringId.CalculatorButtonPercent, "%");
			AddString(EditorStringId.CalculatorButtonSqrt, "√");
			AddString(EditorStringId.CalculatorButtonEqual, "=");
			AddString(EditorStringId.CalculatorDivisionByZeroError, "Cannot divide by zero");
			AddString(EditorStringId.CalculatorError, "Error");
			AddString(EditorStringId.CalculatorInvalidInputError, "Invalid input");
			#endregion
			#region Filter Strings
			AddString(EditorStringId.FilterCriteriaToStringFunctionIsNullOrEmpty, "Is null or empty");
			AddString(EditorStringId.FilterCriteriaToStringFunctionStartsWith, "Starts with");
			AddString(EditorStringId.FilterCriteriaToStringFunctionEndsWith, "Ends with");
			AddString(EditorStringId.FilterCriteriaToStringFunctionContains, "Contains");
			AddString(EditorStringId.FilterCriteriaToStringBetween, "Between");
			AddString(EditorStringId.FilterCriteriaToStringIn, "In");
			AddString(EditorStringId.FilterCriteriaToStringIsNotNull, "Is Not Null");
			AddString(EditorStringId.FilterCriteriaToStringNotLike, "Not Like");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd, "&");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseOr, "|");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseXor, "^");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorDivide, "/");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorEqual, "=");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorGreater, ">");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual, ">=");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorLess, "<");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorLessOrEqual, "<=");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorLike, "Like");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorMinus, "-");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorModulo, "%");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorMultiply, "*");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorNotEqual, "<>");
			AddString(EditorStringId.FilterCriteriaToStringBinaryOperatorPlus, "+");
			AddString(EditorStringId.FilterCriteriaToStringUnaryOperatorBitwiseNot, "~");
			AddString(EditorStringId.FilterCriteriaToStringUnaryOperatorIsNull, "Is Null");
			AddString(EditorStringId.FilterCriteriaToStringUnaryOperatorMinus, "-");
			AddString(EditorStringId.FilterCriteriaToStringUnaryOperatorNot, "Not");
			AddString(EditorStringId.FilterCriteriaToStringUnaryOperatorPlus, "+");
			AddString(EditorStringId.FilterCriteriaToStringGroupOperatorAnd, "And");
			AddString(EditorStringId.FilterCriteriaToStringGroupOperatorOr, "Or");
			AddString(EditorStringId.FilterCriteriaInvalidExpression, "The specified expression contains invalid symbols (line {0}, character {1}).");
			AddString(EditorStringId.FilterCriteriaInvalidExpressionEx, "The specified expression is invalid.");
			#endregion
			#region expression editor
			AddString(EditorStringId.ExpressionEditor_FunctionType_AllItems, "(All)");
			AddString(EditorStringId.ExpressionEditor_FunctionType_AggregateItems, "Aggregate");
			AddString(EditorStringId.ExpressionEditor_FunctionType_DateTimeItems, "Date-time");
			AddString(EditorStringId.ExpressionEditor_FunctionType_LogicalItems, "Logical");
			AddString(EditorStringId.ExpressionEditor_FunctionType_MathItems, "Math");
			AddString(EditorStringId.ExpressionEditor_FunctionType_StringItems, "String");
			AddString(EditorStringId.ExpressionEditor_Functions_Text, "Functions");
			AddString(EditorStringId.ExpressionEditor_Operators_Text, "Operators");
			AddString(EditorStringId.ExpressionEditor_Parameters_Text, "Parameters");
			AddString(EditorStringId.ExpressionEditor_Variables_Text, "Variables");
			AddString(EditorStringId.ExpressionEditor_Fields_Text, "Fields");
			AddString(EditorStringId.ExpressionEditor_Constants_Text, "Constants");
			AddString(EditorStringId.ExpressionEditor_Abs_Description, "Abs(Value)\r\nReturns the absolute, positive value of the given numeric expression.");
			AddString(EditorStringId.ExpressionEditor_Acos_Description, "Acos(Value)\r\nReturns the arccosine of a number (the angle, in radians, whose cosine is the given float expression).");
			AddString(EditorStringId.ExpressionEditor_AddDays_Description, "AddDays(DateTime, DaysCount)\r\nReturns a date-time value that is the specified number of days away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddHours_Description, "AddHours(DateTime, HoursCount)\r\nReturns a date-time value that is the specified number of hours away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddMilliSeconds_Description, "AddMilliSeconds(DateTime, MilliSecondsCount)\r\nReturns a date-time value that is the specified number of milliseconds away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddMinutes_Description, "AddMinutes(DateTime, MinutesCount)\r\nReturns a date-time value that is the specified number of minutes away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddMonths_Description, "AddMonths(DateTime, MonthsCount)\r\nReturns a date-time value that is the specified number of months away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddSeconds_Description, "AddSeconds(DateTime, SecondsCount)\r\nReturns a date-time value that is the specified number of seconds away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddTicks_Description, "AddTicks(DateTime, TicksCount)\r\nReturns a date-time value that is the specified number of ticks away from the specified DateTime.");
			AddString(EditorStringId.ExpressionEditor_AddTimeSpan_Description, "AddTimeSpan(DateTime, TimeSpan)\r\nReturns a date-time value that is away from the specified DateTime for the given TimeSpan.");
			AddString(EditorStringId.ExpressionEditor_AddYears_Description, "AddYears(DateTime, YearsCount)\r\nReturns a date-time value that is the specified number of years away from the specieid DateTime.");
			AddString(EditorStringId.ExpressionEditor_Ascii_Description, "Ascii(String)\r\nReturns the ASCII code value of the leftmost character in a character expression.");
			AddString(EditorStringId.ExpressionEditor_Asin_Description, "Asin(Value)\r\nReturns the arcsine of a number (the angle, in radians, whose sine is the given float expression).");
			AddString(EditorStringId.ExpressionEditor_Atn_Description, "Atn(Value)\r\nReturns the arctangent of a number (the angle, in radians, whose tangent is the given float expression).");
			AddString(EditorStringId.ExpressionEditor_Atn2_Description, "Atn2(Value1, Value2)\r\nReturns the angle whose tangent is the quotient of two specified numbers, in radians.");
			AddString(EditorStringId.ExpressionEditor_BigMul_Description, "BigMul(Value1, Value2)\r\nReturns an Int64 containing the full product of two specified 32-bit numbers.");
			AddString(EditorStringId.ExpressionEditor_Ceiling_Description, "Ceiling(Value)\r\nReturns the smallest integer that is greater than or equal to the given numeric expression.");
			AddString(EditorStringId.ExpressionEditor_Char_Description, "Char(Number)\r\nConverts an integerASCIICode to a character.");
			AddString(EditorStringId.ExpressionEditor_CharIndex_Description, "CharIndex(String1, String2)\r\nReturns the starting position of String1 within String2, beginning from the zero character position to the end of a string.");
			AddString(EditorStringId.ExpressionEditor_CharIndex3Param_Description, "CharIndex(String1, String2, StartLocation)\r\nReturns the starting position of String1 within String2, beginning from the StartLocation character position to the end of a string.");
			AddString(EditorStringId.ExpressionEditor_Concat_Description, "Concat(String1, ... , StringN)\r\nReturns a string value containing the concatenation of the current string with any additional strings.");
			AddString(EditorStringId.ExpressionEditor_Cos_Description, "Cos(Value)\r\nReturns the cosine of the angle defined in radians.");
			AddString(EditorStringId.ExpressionEditor_Cosh_Description, "Cosh(Value)\r\nReturns the hyperbolic cosine of the angle defined in radians.");
			AddString(EditorStringId.ExpressionEditor_DateDiffDay_Description, "DateDiffDay(startDate, endDate)\r\nReturns the number of day boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffHour_Description, "DateDiffHour(startDate, endDate)\r\nReturns the number of hour boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffMilliSecond_Description, "DateDiffMilliSecond(startDate, endDate)\r\nReturns the number of millisecond boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffMinute_Description, "DateDiffMinute(startDate, endDate)\r\nReturns the number of minute boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffMonth_Description, "DateDiffMonth(startDate, endDate)\r\nReturns the number of month boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffSecond_Description, "DateDiffSecond(startDate, endDate)\r\nReturns the number of second boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffTick_Description, "DateDiffTick(startDate, endDate)\r\nReturns the number of tick boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_DateDiffYear_Description, "DateDiffYear(startDate, endDate)\r\nReturns the number of year boundaries between two non-nullable dates.");
			AddString(EditorStringId.ExpressionEditor_Exp_Description, "Exp(Value)\r\nReturns the exponential value of the given float expression.");
			AddString(EditorStringId.ExpressionEditor_Floor_Description, "Floor(Value)\r\nReturns the largest integer less than or equal to the given numeric expression.");
			AddString(EditorStringId.ExpressionEditor_GetDate_Description, "GetDate(DateTime)\r\nExtracts a date from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetDay_Description, "GetDay(DateTime)\r\nExtracts a day from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetDayOfWeek_Description, "GetDayOfWeek(DateTime)\r\nExtracts a day of the week from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetDayOfYear_Description, "GetDayOfYear(DateTime)\r\nExtracts a day of the year from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetHour_Description, "GetHour(DateTime)\r\nExtracts an hour from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetMilliSecond_Description, "GetMilliSecond(DateTime)\r\nExtracts milliseconds from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetMinute_Description, "GetMinute(DateTime)\r\nExtracts minutes from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetMonth_Description, "GetMonth(DateTime)\r\nExtracts a month from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetSecond_Description, "GetSecond(DateTime)\r\nExtracts seconds from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_GetTimeOfDay_Description, "GetTimeOfDay(DateTime)\r\nExtracts the time of the day from the defined DateTime, in ticks.");
			AddString(EditorStringId.ExpressionEditor_GetYear_Description, "GetYear(DateTime)\r\nExtracts a year from the defined DateTime.");
			AddString(EditorStringId.ExpressionEditor_Iif_Description, "Iif(Expression, TruePart, FalsePart)\r\nReturns either TruePart or FalsePart, depending on the evaluation of the Boolean Expression.");
			AddString(EditorStringId.ExpressionEditor_Insert_Description, "Insert(String1, StartPosition, String2)\r\nInserts String2 into String1 at the position specified by StartPositon");
			AddString(EditorStringId.ExpressionEditor_IsNull_Description, "IsNull(Value)\r\nReturns True if the specified Value is NULL.");
			AddString(EditorStringId.ExpressionEditor_IsNullOrEmpty_Description, "IsNullOrEmpty(String)\r\nReturns True if the specified String object is NULL or an empty string; otherwise, False is returned.");
			AddString(EditorStringId.ExpressionEditor_Len_Description, "Len(Value)\r\nReturns an integer containing either the number of characters in a string or the nominal number of bytes required to store a variable.");
			AddString(EditorStringId.ExpressionEditor_Log_Description, "Log(Value)\r\nReturns the natural logarithm of a specified number.");
			AddString(EditorStringId.ExpressionEditor_Log2Param_Description, "Log(Value, Base)\r\nReturns the logarithm of a specified number in a specified Base.");
			AddString(EditorStringId.ExpressionEditor_Log10_Description, "Log10(Value)\r\nReturns the base 10 logarithm of a specified number.");
			AddString(EditorStringId.ExpressionEditor_Lower_Description, "Lower(String)\r\nReturns the String in lowercase.");
			AddString(EditorStringId.ExpressionEditor_Now_Description, "Now()\r\nReturns the current system date and time.");
			AddString(EditorStringId.ExpressionEditor_PadLeft_Description, "PadLeft(String, Length)\r\nLeft-aligns characters in the defined string, padding its left side with white space characters up to a specified total length.");
			AddString(EditorStringId.ExpressionEditor_PadLeft3Param_Description, "PadLeft(String, Length, Char)\r\nLeft-aligns characters in the defined string, padding its left side with the specified Char up to a specified total length.");
			AddString(EditorStringId.ExpressionEditor_PadRight_Description, "PadRight(String, Length)\r\nRight-aligns characters in the defined string, padding its left side with white space characters up to a specified total length.");
			AddString(EditorStringId.ExpressionEditor_PadRight3Param_Description, "PadRight(String, Length, Char)\r\nRight-aligns characters in the defined string, padding its left side with the specified Char up to a specified total length.");
			AddString(EditorStringId.ExpressionEditor_Power_Description, "Power(Value, Power)\r\nReturns a specified number raised to a specified power.");
			AddString(EditorStringId.ExpressionEditor_Remove2Param_Description, "Remove(String, StartPosition)\r\nDeletes all characters from this instance, beginning at a specified position.");
			AddString(EditorStringId.ExpressionEditor_Remove3Param_Description, "Remove(String, StartPosition, Length)\r\nDeletes a specified number of characters from this instance, beginning at a specified position.");
			AddString(EditorStringId.ExpressionEditor_Replace_Description, "Replace(String, SubString2, String3)\r\nReturns a copy of String1, in which SubString2 has been replaced with String3.");
			AddString(EditorStringId.ExpressionEditor_Reverse_Description, "Reverse(String)\r\nReverses the order of elements within a string.");
			AddString(EditorStringId.ExpressionEditor_Rnd_Description, "Rnd()\r\nReturns a random number that is less than 1, but greater than or equal to zero.");
			AddString(EditorStringId.ExpressionEditor_Round_Description, "Round(Value)\r\nRounds the given value to the nearest integer.");
			AddString(EditorStringId.ExpressionEditor_Round2Param_Description, "Round(Value, Precision)\r\nRounds the given value to the nearest integer, or to a specified number of decimal places.");
			AddString(EditorStringId.ExpressionEditor_Sign_Description, "Sign(Value)\r\nReturns the positive (+1), zero (0), or negative (-1) sign of the given expression.");
			AddString(EditorStringId.ExpressionEditor_Sin_Description, "Sin(Value)\r\nReturns the sine of the angle, defined in radians.");
			AddString(EditorStringId.ExpressionEditor_Sinh_Description, "Sinh(Value)\r\nReturns the hyperbolic sine of the angle defined in radians.");
			AddString(EditorStringId.ExpressionEditor_Sqr_Description, "Sqr(Value)\r\nReturns the square root of a given number.");
			AddString(EditorStringId.ExpressionEditor_Substring3param_Description, "Substring(String, StartPosition, Length)\r\nRetrieves a substring from String. The substring starts at StartPosition and has the specified Length.");
			AddString(EditorStringId.ExpressionEditor_Substring2param_Description, "Substring(String, StartPosition)\r\nRetrieves a substring from String. The substring starts at StartPosition.");
			AddString(EditorStringId.ExpressionEditor_Tan_Description, "Tan(Value)\r\nReturns the tangent of the angle defined in radians.");
			AddString(EditorStringId.ExpressionEditor_Tanh_Description, "Tanh(Value)\r\nReturns the hyperbolic tangent of the angle defined in radians.");
			AddString(EditorStringId.ExpressionEditor_Today_Description, "Today()\r\nReturns the current date. Regardless of the actual time, this function returns midnight of the current date.");
			AddString(EditorStringId.ExpressionEditor_ToInt_Description, "ToInt(Value)\r\nConverts Value to an equivalent 32-bit signed integer.");
			AddString(EditorStringId.ExpressionEditor_ToLong_Description, "ToLong(Value)\r\nConverts Value to an equivalent 64-bit signed integer.");
			AddString(EditorStringId.ExpressionEditor_ToFloat_Description, "ToFloat(Value)\r\nConverts Value to an equivalent 32-bit single-precision floating-point number.");
			AddString(EditorStringId.ExpressionEditor_ToDouble_Description, "ToDouble(Value)\r\nConverts Value to an equivalent 64-bit double-precision floating-point number.");
			AddString(EditorStringId.ExpressionEditor_ToDecimal_Description, "ToDecimal(Value)\r\nConverts Value to an equivalent decimal number.");
			AddString(EditorStringId.ExpressionEditor_ToStr_Description, "ToStr(Value)\r\nReturns a string representation of an object.");
			AddString(EditorStringId.ExpressionEditor_Trim_Description, "Trim(String)\r\nRemoves all leading and trailing SPACE characters from String.");
			AddString(EditorStringId.ExpressionEditor_Upper_Description, "Upper(String)\r\nReturns String in uppercase.");
			AddString(EditorStringId.ExpressionEditor_UtcNow_Description, "UtcNow()\r\nReturns the current system date and time, expressed as Coordinated Universal Time (UTC).");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeDayAfterTomorrow_Description, "LocalDateTimeDayAfterTomorrow()\r\nReturns a date-time value corresponding to the day after Tomorrow.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeLastWeek_Description, "LocalDateTimeLastWeek()\r\nReturns a date-time value corresponding to the first day of the previous week.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeNextMonth_Description, "LocalDateTimeNextMonth()\r\nReturns a date-time value corresponding to the first day of next month.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeNextWeek_Description, "LocalDateTimeNextWeek()\r\nReturns a date-time value corresponding to the first day of the following week.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeNextYear_Description, "LocalDateTimeNextYear()\r\nReturns a date-time value corresponding to the first day of the following year.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeNow_Description, "LocalDateTimeNow()\r\nReturns a date-time value corresponding to the current moment in time.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeThisMonth_Description, "LocalDateTimeThisMonth()\r\nReturns a date-time value corresponding to the first day of the current month.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeThisWeek_Description, "LocalDateTimeThisWeek()\r\nReturns a date-time value corresponding to the first day of the current week.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeThisYear_Description, "LocalDateTimeThisYear()\r\nReturns a date-time value corresponding to the first day of the current year.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeToday_Description, "LocalDateTimeToday()\r\nReturns a date-time value corresponding to Today.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeTomorrow_Description, "LocalDateTimeTomorrow()\r\nReturns a date-time value corresponding to Tomorrow.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeTwoWeeksAway_Description, "LocalDateTimeTwoWeeksAway()\r\nReturns a date-time value corresponding to the first day of the week that is after next week.");
			AddString(EditorStringId.ExpressionEditor_LocalDateTimeYesterday_Description, "LocalDateTimeYesterday()\r\nReturns a date-time value corresponding to Yesterday.");
			AddString(EditorStringId.ExpressionEditor_Plus_Description, "Adds the value of one numeric expression to another, or concatenates two strings.");
			AddString(EditorStringId.ExpressionEditor_Minus_Description, "Finds the difference between two numbers.");
			AddString(EditorStringId.ExpressionEditor_Multiply_Description, "Multiplies the value of two expressions.");
			AddString(EditorStringId.ExpressionEditor_Divide_Description, "Divides the first operand by the second.");
			AddString(EditorStringId.ExpressionEditor_Modulo_Description, "Returns the remainder (modulus) obtained by dividing one numeric expression into another.");
			AddString(EditorStringId.ExpressionEditor_BitwiseOr_Description, "Compares each bit of its first operand to the corresponding bit of its second operand. If either bit is 1, the corresponding result bit is set to 1. Otherwise, the corresponding result bit is set to 0.");
			AddString(EditorStringId.ExpressionEditor_BitwiseAnd_Description, "Performs a bitwise logical AND operation between two integer values.");
			AddString(EditorStringId.ExpressionEditor_BitwiseXor_Description, "Performs a logical exclusion on two Boolean expressions, or a bitwise exclusion on two numeric expressions.");
			AddString(EditorStringId.ExpressionEditor_Equal_Description, "Returns true if both operands have the same value; otherwise, it returns false.");
			AddString(EditorStringId.ExpressionEditor_NotEqual_Description, "Returns true if the operands do not have the same value; otherwise, it returns false.");
			AddString(EditorStringId.ExpressionEditor_Less_Description, "Less than operator. Used to compare expressions.");
			AddString(EditorStringId.ExpressionEditor_LessOrEqual_Description, "Less than or equal to operator. Used to compare expressions.");
			AddString(EditorStringId.ExpressionEditor_GreaterOrEqual_Description, "Greater than or equal to operator. Used to compare expressions.");
			AddString(EditorStringId.ExpressionEditor_Greater_Description, "Greater than operator. Used to compare expressions.");
			AddString(EditorStringId.ExpressionEditor_In_Description, "In (,,,)\r\nTests for the existence of a property in an object.");
			AddString(EditorStringId.ExpressionEditor_Like_Description, "Compares a string against a pattern. If the value of the string matches the pattern, result is true. If the string does not match the pattern, result is false. If both string and pattern are empty strings, the result is true.");
			AddString(EditorStringId.ExpressionEditor_Between_Description, "Between (,)\r\nSpecifies a range to test. Returns true if a value is greater than or equal to the first operand and less than or equal to the second operand.");
			AddString(EditorStringId.ExpressionEditor_And_Description, "Performs a logical conjunction on two expressions.");
			AddString(EditorStringId.ExpressionEditor_Or_Description, "Performs a logical disjunction on two Boolean expressions.");
			AddString(EditorStringId.ExpressionEditor_Not_Description, "Performs logical negation on an expression.");
			AddString(EditorStringId.ExpressionEditor_Max_Description, "Max(Value1, Value2)\r\nReturns the maximum value from the specified values.");
			AddString(EditorStringId.ExpressionEditor_Min_Description, "Min(Value1, Value2)\r\nReturns the minimum value from the specified values.");
			AddString(EditorStringId.ExpressionEditor_StartsWith_Description, "StartsWith(String, StartString)\r\nReturns True if the beginning of String matches StartString; otherwise, False is returned.");
			AddString(EditorStringId.ExpressionEditor_EndsWith_Description, "EndsWith(String, EndString)\r\nReturns True if the end of String matches EndString; otherwise, False is returned.");
			AddString(EditorStringId.ExpressionEditor_Contains_Description, "Contains(String, SubString)\r\nReturns True if SubString occurs within String; otherwise, False is returned.");
			AddString(EditorStringId.ExpressionEditor_IsThisYear_Description, "IsThisYear");
			AddString(EditorStringId.ExpressionEditor_IsThisMonth_Description, "IsThisMonth");
			AddString(EditorStringId.ExpressionEditor_IsThisWeek_Description, "IsThisWeek");
			AddString(EditorStringId.ExpressionEditor_GridFields_Description_Prefix, "Field Information\r\nCaption: {1}\r\nThe type of this field is: {2}");
			AddString(EditorStringId.ExpressionEditor_True_Description, "Represents the Boolean True value.");
			AddString(EditorStringId.ExpressionEditor_False_Description, "Represents the Boolean False value.");
			AddString(EditorStringId.ExpressionEditor_Null_Description, "Represents a null reference, one that does not refer to any object.");
			AddString(EditorStringId.ExpressionEditor_Title, "Expression Editor");
			AddString(EditorStringId.ExpressionEditor_AvgAggregate_Description, "Avg(Value)\r\nEvaluates the average of the values in the collection.");
			AddString(EditorStringId.ExpressionEditor_CountAggregate_Description, "Count()\r\nReturns the number of objects in a collection.");
			AddString(EditorStringId.ExpressionEditor_ExistsAggregate_Description, "Exists()\r\nDetermines whether the object exists in the collection.");
			AddString(EditorStringId.ExpressionEditor_MaxAggregate_Description, "Max(Value)\r\nReturns the maximum expression value in a collection.");
			AddString(EditorStringId.ExpressionEditor_MinAggregate_Description, "Min(Value)\r\nReturns the minimum expression value in a collection.");
			AddString(EditorStringId.ExpressionEditor_SumAggregate_Description, "Sum(Value)\r\nReturns the sum of all the expression values in the collection.");
			AddString(EditorStringId.ExpressionEditor_SingleAggregate_Description, "Single()\r\nReturns a single object from the collection.");
			#endregion
			#region Filter Control Strings
			AddString(EditorStringId.FilterGroupAnd, "And");
			AddString(EditorStringId.FilterGroupOr, "Or");
			AddString(EditorStringId.FilterGroupNotAnd, "NotAnd");
			AddString(EditorStringId.FilterGroupNotOr, "NotOr");
			AddString(EditorStringId.FilterGroupAddCondition, "Add Condition");
			AddString(EditorStringId.FilterGroupAddGroup, "Add Group");
			AddString(EditorStringId.FilterGroupRemoveGroup, "Remove Group");
			AddString(EditorStringId.FilterGroupClearAll, "Clear All");
			AddString(EditorStringId.FilterEmptyValueText, "<enter a value>");
			AddString(EditorStringId.FilterClauseAnyOf, "Is any of");
			AddString(EditorStringId.FilterClauseBeginsWith, "Begins with");
			AddString(EditorStringId.FilterClauseBetween, "Is between");
			AddString(EditorStringId.FilterClauseBetweenAnd, "and");
			AddString(EditorStringId.FilterClauseContains, "Contains");
			AddString(EditorStringId.FilterClauseEndsWith, "Ends with");
			AddString(EditorStringId.FilterClauseEquals, "Equals");
			AddString(EditorStringId.FilterClauseGreater, "Is greater than");
			AddString(EditorStringId.FilterClauseGreaterOrEqual, "Is greater than or equal to");
			AddString(EditorStringId.FilterClauseIsNull, "Is null");
			AddString(EditorStringId.FilterClauseIsNotNull, "Is not null");
			AddString(EditorStringId.FilterClauseLess, "Is less than");
			AddString(EditorStringId.FilterClauseLessOrEqual, "Is less than or equal to");
			AddString(EditorStringId.FilterClauseLike, "Is like");
			AddString(EditorStringId.FilterClauseNoneOf, "Is none of");
			AddString(EditorStringId.FilterClauseNotBetween, "Is not between");
			AddString(EditorStringId.FilterClauseDoesNotContain, "Does not contain");
			AddString(EditorStringId.FilterClauseDoesNotEqual, "Does not equal");
			AddString(EditorStringId.FilterClauseNotLike, "Is not like");
			AddString(EditorStringId.FilterClauseIsNullOrEmpty, "Is blank");
			AddString(EditorStringId.FilterClauseIsNotNullOrEmpty, "Is not blank");
			AddString(EditorStringId.FilterClauseIsBeyondThisYear, "Is Beyond This Year");
			AddString(EditorStringId.FilterClauseIsLaterThisYear, "Is Later This Year");
			AddString(EditorStringId.FilterClauseIsLaterThisMonth, "Is Later This Month");
			AddString(EditorStringId.FilterClauseIsNextWeek, "Is Next Week");
			AddString(EditorStringId.FilterClauseIsLaterThisWeek, "Is Later This Week");
			AddString(EditorStringId.FilterClauseIsTomorrow, "Is Tomorrow");
			AddString(EditorStringId.FilterClauseIsToday, "Is Today");
			AddString(EditorStringId.FilterClauseIsYesterday, "Is Yesterday");
			AddString(EditorStringId.FilterClauseIsEarlierThisWeek, "Is Earlier This Week");
			AddString(EditorStringId.FilterClauseIsLastWeek, "Is Last Week");
			AddString(EditorStringId.FilterClauseIsEarlierThisMonth, "Is Earlier This Month");
			AddString(EditorStringId.FilterClauseIsEarlierThisYear, "Is Earlier This Year");
			AddString(EditorStringId.FilterClauseIsPriorThisYear, "Is Prior This Year");
			AddString(EditorStringId.FilterClauseLocalDateTimeThisYear, "This year");
			AddString(EditorStringId.FilterClauseLocalDateTimeThisMonth, "This month");
			AddString(EditorStringId.FilterClauseLocalDateTimeLastWeek, "Last week");
			AddString(EditorStringId.FilterClauseLocalDateTimeThisWeek, "This week");
			AddString(EditorStringId.FilterClauseLocalDateTimeYesterday, "Yesterday");
			AddString(EditorStringId.FilterClauseLocalDateTimeToday, "Today");
			AddString(EditorStringId.FilterClauseLocalDateTimeNow, "Now");
			AddString(EditorStringId.FilterClauseLocalDateTimeTomorrow, "Tomorrow");
			AddString(EditorStringId.FilterClauseLocalDateTimeDayAfterTomorrow, "Day after tomorrow");
			AddString(EditorStringId.FilterClauseLocalDateTimeNextWeek, "Next week");
			AddString(EditorStringId.FilterClauseLocalDateTimeTwoWeeksAway, "Two weeks away");
			AddString(EditorStringId.FilterClauseLocalDateTimeNextMonth, "Next month");
			AddString(EditorStringId.FilterClauseLocalDateTimeNextYear, "Next year");
			AddString(EditorStringId.FilterDateTimeOperatorMenuCaption, "DateTime operators");
			AddString(EditorStringId.FilterEditorChecked, "Checked");
			AddString(EditorStringId.FilterEditorUnchecked, "Unchecked");
			AddString(EditorStringId.FilterToolTipNodeAction, "Actions");
			AddString(EditorStringId.FilterToolTipNodeAdd, "Adds a new condition to this group");
			AddString(EditorStringId.FilterToolTipNodeRemove, "Removes this condition");
			AddString(EditorStringId.FilterToolTipValueType, "Compare with a value / another field's value");
			AddString(EditorStringId.FilterToolTipElementAdd, "Adds a new item to the list");
			AddString(EditorStringId.FilterToolTipKeysAdd, "(Use the Insert or Add key)");
			AddString(EditorStringId.FilterToolTipKeysRemove, "(Use the Delete or Subtract key)");
			#endregion
			#region FilterPanel Control Strings
			AddString(EditorStringId.FilterPanelEditFilter, "Edit Filter");
			AddString(EditorStringId.FilterPanelClearFilter, "Clear Filter");
			AddString(EditorStringId.FilterPanelEnableFilter, "Enable Filter");
			AddString(EditorStringId.FilterPanelDisableFilter, "Disable Filter");
			#endregion
			#region ColorEdit
			AddString(EditorStringId.ColorEdit_AutomaticButtonCaption, "Automatic");
			AddString(EditorStringId.ColorEdit_NoColorButtonCaption, "No Color");
			AddString(EditorStringId.ColorEdit_MoreColorsButtonCaption, "More Colors...");
			AddString(EditorStringId.ColorEdit_RecentColorsCaption, "Recent Colors");
			AddString(EditorStringId.ColorEdit_ColorChooserWindowTitle, "Colors");
			AddString(EditorStringId.ColorEdit_ColorChooserWindowTitle, "Colors");
			AddString(EditorStringId.ColorEdit_ThemeColorsCaption, "Theme Colors");
			AddString(EditorStringId.ColorEdit_StandardColorsCaption, "Standard Colors");
			AddString(EditorStringId.ColorEdit_DefaultColors_Black, "Black");
			AddString(EditorStringId.ColorEdit_DefaultColors_White, "White");
			AddString(EditorStringId.ColorEdit_DefaultColors_DarkRed, "DarkRed");
			AddString(EditorStringId.ColorEdit_DefaultColors_Red, "Red");
			AddString(EditorStringId.ColorEdit_DefaultColors_Orange, "Orange");
			AddString(EditorStringId.ColorEdit_DefaultColors_Yellow, "Yellow");
			AddString(EditorStringId.ColorEdit_DefaultColors_LightGreen, "LightGreen");
			AddString(EditorStringId.ColorEdit_DefaultColors_Green, "Green");
			AddString(EditorStringId.ColorEdit_DefaultColors_LightBlue, "LightBlue");
			AddString(EditorStringId.ColorEdit_DefaultColors_Blue, "Blue");
			AddString(EditorStringId.ColorEdit_DefaultColors_DarkBlue, "DarkBlue");
			AddString(EditorStringId.ColorEdit_DefaultColors_Purple, "Purple");
			#endregion
			#region ImageEdit
			AddString(EditorStringId.ImageEdit_OpenFileFilter, "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*");
			AddString(EditorStringId.ImageEdit_OpenFileFilter_SL, "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG|All files (*.*)|*.*");
			AddString(EditorStringId.ImageEdit_SaveFileFilter, "PNG(*.png)|*.png|BMP(*.bmp)|*.BMP|JPG(*.jpg)|*.jpg|GIF(*.gif)|*.gif");
			AddString(EditorStringId.ImageEdit_InvalidFormatMessage, "Wrong image format");
			#endregion
			#region PasswordBoxEdit
			AddString(EditorStringId.PasswordBoxEditToolTipHeader, "Caps Lock is On");
			AddString(EditorStringId.PasswordBoxEditToolTipContent, String.Format(@"Enabling Caps Lock may result in entering password incorrectly.{0}Disable Caps Lock before entering your password.", Environment.NewLine));
			#endregion
			#region DateNavigator
			AddString(EditorStringId.CantModifySelectedDates, "Can only change SelectedDates collection in multiple selection mode. Use FocusedDate in single select mode.");
			#endregion
			#region DataPager
			AddString(EditorStringId.Page, "Page");
			AddString(EditorStringId.Of, "of {0}");
			#endregion
			#region DisplayFormatTextControl
			AddString(EditorStringId.DisplayFormatHelperWrongDisplayFormatValue, "Invalid display format text");
			AddString(EditorStringId.DisplayFormatTextControlExample, "Example: ");
			AddString(EditorStringId.DisplayFormatTextControlPrefix, "Prefix:");
			AddString(EditorStringId.DisplayFormatTextControlSuffix, "Suffix:");
			AddString(EditorStringId.DisplayFormatTextControlDisplayFormatText, "Display format text:");
			AddString(EditorStringId.DataTypeStringExample, "ABCXYZ");
			AddString(EditorStringId.DataTypeCharExample, "A");
			AddString(EditorStringId.DisplayFormatNullValue, "None");
			AddString(EditorStringId.DisplayFormatGroupTypeDefault, "Default");
			AddString(EditorStringId.DisplayFormatGroupTypeNumber, "Number");
			AddString(EditorStringId.DisplayFormatGroupTypePercent, "Percent");
			AddString(EditorStringId.DisplayFormatGroupTypeCurrency, "Currency");
			AddString(EditorStringId.DisplayFormatGroupTypeSpecial, "Special");
			AddString(EditorStringId.DisplayFormatGroupTypeDatetime, "Datetime");
			AddString(EditorStringId.DisplayFormatGroupTypeCustom, "Custom");
			#endregion
			#region FontEdit
			AddString(EditorStringId.ConfirmationDialogMessage, "The font \"{0}\" is not available on your system. Do you want to use it anyway?");
			AddString(EditorStringId.ConfirmationDialogCaption, "Confirmation");
			#endregion
			#region CheckEdit
			AddString(EditorStringId.CheckChecked, "Checked");
			AddString(EditorStringId.CheckUnchecked, "Unchecked");
			AddString(EditorStringId.CheckIndeterminate, "Indeterminate");
			#endregion
			#region ColorPicker
			AddString(EditorStringId.CMYK, "CMYK");
			AddString(EditorStringId.RGB, "RGB");
			AddString(EditorStringId.HLS, "HLS");
			AddString(EditorStringId.HSB, "HSB");
			AddString(EditorStringId.ColorPickerAlpha, "Alpha");
			AddString(EditorStringId.ColorPickerBlue, "Blue");
			AddString(EditorStringId.ColorPickerBrightness, "Brightness");
			AddString(EditorStringId.ColorPickerCyan, "Cyan");
			AddString(EditorStringId.ColorPickerGreen, "Green");
			AddString(EditorStringId.ColorPickerHue, "Hue");
			AddString(EditorStringId.ColorPickerKeyColor, "Key color");
			AddString(EditorStringId.ColorPickerLightness, "Lightness");
			AddString(EditorStringId.ColorPickerMagenta, "Magenta");
			AddString(EditorStringId.ColorPickerRed, "Red");
			AddString(EditorStringId.ColorPickerSaturation, "Saturation");
			AddString(EditorStringId.ColorPickerYellow, "Yellow");
			#endregion
			#region Character Map
			AddString(EditorStringId.Caption_CommonCharactersToggleButton, "Common Characters");
			AddString(EditorStringId.Caption_SpecialCharactersToggleButton, "Special Characters");
			AddString(EditorStringId.Caption_SymbolFormSearchByCode, "Search by code:");
			AddString(EditorStringId.Caption_SymbolFormFontName, "Font name:");
			AddString(EditorStringId.Caption_SymbolFormCharacterSet, "Character set:");
			AddString(EditorStringId.Caption_SymbolFormFilter, "Filter:");
			#endregion
			#region DateTimePicker
			AddString(EditorStringId.DatePickerHours, "hours");
			AddString(EditorStringId.DatePickerMilliseconds, "msecs");
			AddString(EditorStringId.DatePickerMinutes, "mins");
			AddString(EditorStringId.DatePickerSeconds, "secs");
			#endregion
			#region BrushEdit
			AddString(EditorStringId.BrushEditNone, "None");
			AddString(EditorStringId.BrushEditSolid, "Solid");
			AddString(EditorStringId.BrushEditLinear, "Linear");
			AddString(EditorStringId.BrushEditRadial, "Radial");
			AddString(EditorStringId.BrushEditStartPoint, "StartPoint");
			AddString(EditorStringId.BrushEditStartPointDescription, "Gets or sets the two-dimensional coordinates for the start point of the linear gradient.");
			AddString(EditorStringId.BrushEditEndPoint, "EndPoint");
			AddString(EditorStringId.BrushEditEndPointDescription, "Gets or sets the two-dimensional coordinates for the end point of the linear gradient.");
			AddString(EditorStringId.BrushEditMappingMode, "MappingMode");
			AddString(EditorStringId.BrushEditMappingModeDescription, "Gets or sets a value that specifies whether the gradient brush positioning coordinates are absolute or relative to the output area.");
			AddString(EditorStringId.BrushEditSpreadMethod, "SpreadMethod");
			AddString(EditorStringId.BrushEditSpreadMethodDescription, "Gets or sets the type of spread method that specifies how to draw a gradient that starts or ends inside the bounds of the object to be painted.");
			AddString(EditorStringId.BrushEditGradientOrigin, "GradientOrigin");
			AddString(EditorStringId.BrushEditGradientOriginDescription, "Gets or sets the two-dimensional focal point that defines the beginning of a gradient.");
			AddString(EditorStringId.BrushEditCenter, "Center");
			AddString(EditorStringId.BrushEditCenterDescription, "Gets or sets the center of the outermost circle of the radial gradient.");
			AddString(EditorStringId.BrushEditRadiusX, "RadiusX");
			AddString(EditorStringId.BrushEditRadiusXDescription, "Gets or sets the horizontal radius of the outermost circle of the radial gradient.");
			AddString(EditorStringId.BrushEditRadiusY, "RadiusY");
			AddString(EditorStringId.BrushEditRadiusYDescription, "Gets or sets the vertical radius of the outermost circle of the radial gradient.");
			#endregion
			AddString(EditorStringId.InvalidValueConversion, "Invalid value conversion");
#if SL
			AddString(EditorStringId.ListBoxSelectAllSelectionMode, "Can only call SelectAll when SelectionMode is Multiple or Extended.");
#endif
			AddString(EditorStringId.WaitIndicatorText, "Loading...");
			#region LookUp
			AddString(EditorStringId.LookUpFind, "Find");
			AddString(EditorStringId.LookUpSearch, "Search");
			AddString(EditorStringId.LookUpClose, "Close");
			AddString(EditorStringId.LookUpAddNew, "Add New");
			#endregion
			#region SparklineEdit
			AddString(EditorStringId.SparklineViewLine, "Line");
			AddString(EditorStringId.SparklineViewArea, "Area");
			AddString(EditorStringId.SparklineViewBar, "Bar");
			AddString(EditorStringId.SparklineViewWinLoss, "WinLoss");
			#endregion
			AddString(EditorStringId.TokenEditorNewTokenText, "New...");
			AddString(EditorStringId.CameraSettingsCaption, "Camera Settings");
			AddString(EditorStringId.CameraResolutionCaption, "Resolution:");
			AddString(EditorStringId.CameraBrightnessCaption, "Brightness:");
			AddString(EditorStringId.CameraContrastCaption, "Contrast:");
			AddString(EditorStringId.CameraDesaturateCaption, "Desaturate");
			AddString(EditorStringId.CameraResetButtonCaption, "Reset To Default");
			AddString(EditorStringId.CameraDeviceCaption, "Device:");
			AddString(EditorStringId.CameraAgainButtonCaption, "Try Again");
			AddString(EditorStringId.CameraTakePictureCaption, "Take Picture");
			AddString(EditorStringId.CameraTakePictureToolTip, "Take Picture From Camera");
			AddString(EditorStringId.CameraCaptureButtonCaption, "Capture");
			AddString(EditorStringId.CameraErrorCaption, "The webcam is not available. Try closing other applications that might be using it.");
			AddString(EditorStringId.CameraRefreshButtonCaption, "Refresh");
			AddString(EditorStringId.CameraNoDevicesErrorCaption, "The application couldn't find a webcam.");
		}
		#endregion
		public static XtraLocalizer<EditorStringId> CreateDefaultLocalizer() {
			return new EditorResXLocalizer();
		}
		public static string GetString(EditorStringId id) {
			return Active.GetLocalizedString(id);
		}
		public static string GetString(string id) {
			return Active.GetLocalizedString((EditorStringId)Enum.Parse(typeof(EditorStringId), id, false));
		}
		public override XtraLocalizer<EditorStringId> CreateResXLocalizer() {
			return new EditorResXLocalizer();
		}
	}
	public class EditorResXLocalizer : DXResXLocalizer<EditorStringId> {
		public const string ResxPath =
 "DevExpress.Xpf.Core.Editors.LocalizationRes";
		public EditorResXLocalizer()
			: base(new EditorLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager(ResxPath, typeof(EditorResXLocalizer).Assembly);
		}
	}
#if !SL
	public class EditorStringIdExtension : MarkupExtension {
		public EditorStringId StringId { get; set; }
		public EditorStringIdExtension(EditorStringId stringId) {
			StringId = stringId;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return EditorLocalizer.GetString(StringId);
		}
	}
#endif
}
