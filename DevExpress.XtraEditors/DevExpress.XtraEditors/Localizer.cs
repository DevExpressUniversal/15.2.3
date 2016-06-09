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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using DevExpress.XtraEditors;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraEditors.Controls {
	[ToolboxItem(false)]
	public class Localizer : XtraLocalizer<StringId> {
		static Localizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<StringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<StringId> Active {
			get { return XtraLocalizer<StringId>.Active; }
			set { XtraLocalizer<StringId>.Active = value; }
		}
		public override XtraLocalizer<StringId> CreateResXLocalizer() {
			return new EditResLocalizer();
		}
		public static XtraLocalizer<StringId> CreateDefaultLocalizer() { return new EditResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(StringId.None,
				""); 
			AddString(StringId.CaptionError,
				"Error");
			AddString(StringId.InvalidValueText,
				"Invalid Value");
			AddString(StringId.CheckChecked,
				"Checked");
			AddString(StringId.CheckUnchecked,
				"Unchecked");
			AddString(StringId.CheckIndeterminate,
				"Indeterminate");
			AddString(StringId.DateEditToday,
				"Today");
			AddString(StringId.DateEditClear,
				"Clear");
			AddString(StringId.OK,
				"OK");
			AddString(StringId.Cancel,
				"Cancel");
			AddString(StringId.NavigatorFirstButtonHint,
				"First");
			AddString(StringId.NavigatorPreviousButtonHint,
				"Previous");
			AddString(StringId.NavigatorPreviousPageButtonHint,
				"Previous Page");
			AddString(StringId.NavigatorNextButtonHint,
				"Next");
			AddString(StringId.NavigatorNextPageButtonHint,
				"Next Page");
			AddString(StringId.NavigatorLastButtonHint,
				"Last");
			AddString(StringId.NavigatorAppendButtonHint,
				"Append");
			AddString(StringId.NavigatorRemoveButtonHint,
				"Delete");
			AddString(StringId.NavigatorEditButtonHint,
				"Edit");
			AddString(StringId.NavigatorEndEditButtonHint,
				"End Edit");
			AddString(StringId.NavigatorCancelEditButtonHint,
				"Cancel Edit");
			AddString(StringId.NavigatorTextStringFormat,
				"Record {0} of {1}");
			AddString(StringId.PictureEditMenuCut,
				"Cut");
			AddString(StringId.PictureEditMenuCopy,
				"Copy");
			AddString(StringId.PictureEditMenuPaste,
				"Paste");
			AddString(StringId.PictureEditMenuDelete,
				"Delete");
			AddString(StringId.PictureEditMenuLoad,
				"Load");
			AddString(StringId.PictureEditMenuSave,
				"Save");
			AddString(StringId.PictureEditOpenFileFilter,
				"Bitmap Files (*.bmp)|*.bmp|" +
"Graphics Interchange Format (*.gif)|*.gif|" +
"JPEG File Interchange Format (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
"Icon Files (*.ico)|*.ico|" +
"Portable Network Graphics Format (*.png)|*.png|" +
"All Picture Files |*.bmp;*.gif;*.jpg;*.jpeg;*.ico;*.png;*.tif|" +
"All Files |*.*");
			AddString(StringId.PictureEditSaveFileFilter,
				"Bitmap Files (*.bmp)|*.bmp|" +
"Graphics Interchange Format (*.gif)|*.gif|" +
"JPEG File Interchange Format (*.jpg)|*.jpg|" +
"Portable Network Graphics Format (*.png)|*.png");
			AddString(StringId.PictureEditOpenFileTitle,
				"Open");
			AddString(StringId.PictureEditSaveFileTitle,
				"Save As");
			AddString(StringId.PictureEditOpenFileError,
				"Wrong picture format");
			AddString(StringId.PictureEditOpenFileErrorCaption,
				"Open error");
			AddString(StringId.PictureEditCopyImageError,
				"Could not copy image");
			AddString(StringId.LookUpEditValueIsNull,
				"[EditValue is null]");
			AddString(StringId.LookUpInvalidEditValueType,
				"Invalid LookUpEdit EditValue type.");
			AddString(StringId.LookUpColumnDefaultName,
				"Name");
			AddString(StringId.MaskBoxValidateError,
				"The entered value is incomplete.  Do you want to correct it?\r\n\r\n" +
"Yes - to the editor and correct the value.\r\n" +
"No - leave the value as is.\r\n" +
"Cancel - reset to the previous value.\r\n");
			AddString(StringId.UnknownPictureFormat,
				"Unknown picture format");
			AddString(StringId.DataEmpty,
				"No image data");
			AddString(StringId.NotValidArrayLength,
				"Not valid array length.");
			AddString(StringId.ImagePopupEmpty,
				"(Empty)");
			AddString(StringId.ImagePopupPicture,
				"(Picture)");
			AddString(StringId.ColorTabCustom,
				"Custom");
			AddString(StringId.ColorTabWeb,
				"Web");
			AddString(StringId.ColorTabSystem,
				"System");
			AddString(StringId.CalcButtonMC,
				"MC");
			AddString(StringId.CalcButtonMR,
				"MR");
			AddString(StringId.CalcButtonMS,
				"MS");
			AddString(StringId.CalcButtonMx,
				"M+");
			AddString(StringId.CalcButtonSqrt,
				"sqrt");
			AddString(StringId.CalcButtonBack,
				"Back");
			AddString(StringId.CalcButtonCE,
				"CE");
			AddString(StringId.CalcButtonC,
				"C");
			AddString(StringId.CalcError,
				"Calculation Error");
			AddString(StringId.TabHeaderButtonPrev,
				"Scroll Left");
			AddString(StringId.TabHeaderButtonNext,
				"Scroll Right");
			AddString(StringId.TabHeaderButtonClose,
				"Close");
			AddString(StringId.TabHeaderButtonPin,
				"Pin");
			AddString(StringId.TabHeaderButtonUnpin,
				"Unpin");
			AddString(StringId.TabHeaderSelectorButton,
				"Show Window List");
			AddString(StringId.XtraMessageBoxOkButtonText,
				"&OK");
			AddString(StringId.XtraMessageBoxCancelButtonText,
				"&Cancel");
			AddString(StringId.XtraMessageBoxYesButtonText,
				"&Yes");
			AddString(StringId.XtraMessageBoxNoButtonText,
				"&No");
			AddString(StringId.XtraMessageBoxAbortButtonText,
				"&Abort");
			AddString(StringId.XtraMessageBoxRetryButtonText,
				"&Retry");
			AddString(StringId.XtraMessageBoxIgnoreButtonText,
				"&Ignore");
			AddString(StringId.TextEditMenuUndo,
				"&Undo");
			AddString(StringId.TextEditMenuCut,
				"Cu&t");
			AddString(StringId.TextEditMenuCopy,
				"&Copy");
			AddString(StringId.TextEditMenuPaste,
				"&Paste");
			AddString(StringId.TextEditMenuDelete,
				"&Delete");
			AddString(StringId.TextEditMenuSelectAll,
				"Select &All");
			AddString(StringId.FilterEditorTabText,
				"Text");
			AddString(StringId.FilterEditorTabVisual,
				"Visual");
			AddString(StringId.FilterShowAll,
				"(Select All)");
			AddString(StringId.FilterGroupAnd,
				"And");
			AddString(StringId.FilterGroupNotAnd,
				"Not And");
			AddString(StringId.FilterGroupNotOr,
				"Not Or");
			AddString(StringId.FilterGroupOr,
				"Or");
			AddString(StringId.FilterClauseAnyOf,
				"Is any of");
			AddString(StringId.FilterClauseBeginsWith,
				"Begins with");
			AddString(StringId.FilterClauseBetween,
				"Is between");
			AddString(StringId.FilterClauseBetweenAnd,
				"and");
			AddString(StringId.FilterClauseContains,
				"Contains");
			AddString(StringId.FilterClauseEndsWith,
				"Ends with");
			AddString(StringId.FilterClauseEquals,
				"Equals");
			AddString(StringId.FilterClauseGreater,
				"Is greater than");
			AddString(StringId.FilterClauseGreaterOrEqual,
				"Is greater than or equal to");
			AddString(StringId.FilterClauseIsNotNull,
				"Is not null");
			AddString(StringId.FilterClauseIsNull,
				"Is null");
			AddString(StringId.FilterClauseIsNotNullOrEmpty,
				"Is not blank");
			AddString(StringId.FilterClauseIsNullOrEmpty,
				"Is blank");
			AddString(StringId.FilterClauseLess,
				"Is less than");
			AddString(StringId.FilterClauseLessOrEqual,
				"Is less than or equal to");
			AddString(StringId.FilterClauseLike,
				"Is like");
			AddString(StringId.FilterClauseNoneOf,
				"Is none of");
			AddString(StringId.FilterClauseNotBetween,
				"Is not between");
			AddString(StringId.FilterClauseDoesNotContain,
				"Does not contain");
			AddString(StringId.FilterClauseDoesNotEqual,
				"Does not equal");
			AddString(StringId.FilterClauseNotLike,
				"Is not like");
			AddString(StringId.FilterEmptyEnter,
				"<enter a value>");
			AddString(StringId.FilterEmptyParameter,
				"<enter a parameter name>");
			AddString(StringId.FilterMenuAddNewParameter,
				"Add Parameter...");
			AddString(StringId.FilterEmptyValue,
				"<empty>");
			AddString(StringId.FilterMenuConditionAdd,
				"Add Condition");
			AddString(StringId.FilterMenuGroupAdd,
				"Add Group");
			AddString(StringId.FilterMenuClearAll,
				"Clear All");
			AddString(StringId.FilterMenuRowRemove,
				"Remove Group");
			AddString(StringId.FilterToolTipNodeAdd,
				"Adds a new condition to this group");
			AddString(StringId.FilterToolTipNodeRemove,
				"Removes this condition");
			AddString(StringId.FilterToolTipNodeAction,
				"Actions");
			AddString(StringId.FilterToolTipValueType,
				"Compare with a value / another field's value");
			AddString(StringId.FilterToolTipElementAdd,
				"Adds a new item to the list");
			AddString(StringId.FilterToolTipKeysAdd,
				"(Use the Insert or Add key)");
			AddString(StringId.FilterToolTipKeysRemove,
				"(Use the Delete or Subtract key)");
			AddString(StringId.ContainerAccessibleEditName,
				"Editing control");
			AddString(StringId.FilterCriteriaToStringGroupOperatorAnd,
				"And");
			AddString(StringId.FilterCriteriaToStringGroupOperatorOr,
				"Or");
			AddString(StringId.FilterCriteriaToStringUnaryOperatorBitwiseNot,
				"~");
			AddString(StringId.FilterCriteriaToStringUnaryOperatorIsNull,
				"Is Null");
			AddString(StringId.FilterCriteriaToStringUnaryOperatorMinus,
				"-");
			AddString(StringId.FilterCriteriaToStringUnaryOperatorNot,
				"Not");
			AddString(StringId.FilterCriteriaToStringUnaryOperatorPlus,
				"+");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd,
				"&");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseOr,
				"|");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorBitwiseXor,
				"^");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorDivide,
				"/");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorEqual,
				"=");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorGreater,
				">");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual,
				">=");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorLess,
				"<");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorLessOrEqual,
				"<=");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorLike,
				"Like");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorMinus,
				"-");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorModulo,
				"%");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorMultiply,
				"*");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorNotEqual,
				"<>");
			AddString(StringId.FilterCriteriaToStringBinaryOperatorPlus, 
				"+");
			AddString(StringId.FilterCriteriaToStringBetween,
				"Between");
			AddString(StringId.FilterCriteriaToStringIn,
				"In");
			AddString(StringId.FilterCriteriaToStringIsNotNull,
				"Is Not Null");
			AddString(StringId.FilterCriteriaToStringNotLike,
				"Not Like");
			AddString(StringId.FilterCriteriaToStringFunctionIif,
				"Iif");
			AddString(StringId.FilterCriteriaToStringFunctionIsNull,
				"IsNull");
			AddString(StringId.FilterCriteriaToStringFunctionLen,
				"Len");
			AddString(StringId.FilterCriteriaToStringFunctionLower,
				"Lower");
			AddString(StringId.FilterCriteriaToStringFunctionNone,
				"None");
			AddString(StringId.FilterCriteriaToStringFunctionSubstring,			 
				"Substring");
			AddString(StringId.FilterCriteriaToStringFunctionTrim,
				"Trim");
			AddString(StringId.FilterCriteriaToStringFunctionUpper,
				"Upper");
			AddString(StringId.FilterCriteriaToStringFunctionCustom,
				"Custom");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisYear,
				"This year");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisMonth,
				"This month");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeLastWeek,
				"Last week");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisWeek,
				"This week");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeYesterday,
				"Yesterday");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeToday,
				"Today");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeNow,
				"Now");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeTomorrow,
				"Tomorrow");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeDayAfterTomorrow,
				"Day after tomorrow");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextWeek,
				"Next week");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeTwoWeeksAway,
				"Two weeks away");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextMonth,
				"Next month");
			AddString(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextYear,
				"Next year");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear,
				"Is beyond this year");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear,
				"Is later this year");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth,
				"Is later this month");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek,
				"Is next week");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek,
				"Is later this week");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow,
				"Is tomorrow");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalToday,
				"Is today");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalYesterday,
				"Is yesterday");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek,
				"Is earlier this week");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek,
				"Is last week");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth,
				"Is earlier this month");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear,
				"Is earlier this year");
			AddString(StringId.FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear,
				"Is prior to this year");
			AddString(StringId.FilterCriteriaToStringFunctionCustomNonDeterministic,
				"Custom non deterministic");
			AddString(StringId.FilterCriteriaToStringFunctionIsNullOrEmpty,
				"Is null or empty");
			AddString(StringId.FilterCriteriaToStringFunctionConcat,
				"Concat");
			AddString(StringId.FilterCriteriaToStringFunctionAscii,
				"Ascii");
			AddString(StringId.FilterCriteriaToStringFunctionChar,
				"Char");
			AddString(StringId.FilterCriteriaToStringFunctionToInt,
				"To int");
			AddString(StringId.FilterCriteriaToStringFunctionToLong,
				"To long");
			AddString(StringId.FilterCriteriaToStringFunctionToFloat,
				"To float");
			AddString(StringId.FilterCriteriaToStringFunctionToDouble,
				"To double");
			AddString(StringId.FilterCriteriaToStringFunctionToDecimal,
				"To decimal");
			AddString(StringId.FilterCriteriaToStringFunctionToStr,
				"To str");
			AddString(StringId.FilterCriteriaToStringFunctionReplace,
				"Replace");
			AddString(StringId.FilterCriteriaToStringFunctionReverse,
				"Reverse");
			AddString(StringId.FilterCriteriaToStringFunctionInsert,
				"Insert");
			AddString(StringId.FilterCriteriaToStringFunctionCharIndex,
				"Char index");
			AddString(StringId.FilterCriteriaToStringFunctionRemove,
				"Remove");
			AddString(StringId.FilterCriteriaToStringFunctionAbs,
				"Abs");
			AddString(StringId.FilterCriteriaToStringFunctionSqr,
				"Sqr");
			AddString(StringId.FilterCriteriaToStringFunctionCos,
				"Cos");
			AddString(StringId.FilterCriteriaToStringFunctionSin,
				"Sin");
			AddString(StringId.FilterCriteriaToStringFunctionAtn,
				"Atn");
			AddString(StringId.FilterCriteriaToStringFunctionExp,
				"Exp");
			AddString(StringId.FilterCriteriaToStringFunctionLog,
				"Log");
			AddString(StringId.FilterCriteriaToStringFunctionRnd,
				"Rnd");
			AddString(StringId.FilterCriteriaToStringFunctionTan,
				"Tan");
			AddString(StringId.FilterCriteriaToStringFunctionPower,
				"Power");
			AddString(StringId.FilterCriteriaToStringFunctionSign,
				"Sign");
			AddString(StringId.FilterCriteriaToStringFunctionRound,
				"Round");
			AddString(StringId.FilterCriteriaToStringFunctionCeiling,
				"Ceiling");
			AddString(StringId.FilterCriteriaToStringFunctionFloor,
				"Floor");
			AddString(StringId.FilterCriteriaToStringFunctionMax,
				"Max");
			AddString(StringId.FilterCriteriaToStringFunctionMin,
				"Min");
			AddString(StringId.FilterCriteriaToStringFunctionAcos,
				"Acos");
			AddString(StringId.FilterCriteriaToStringFunctionAsin,
				"Asin");
			AddString(StringId.FilterCriteriaToStringFunctionAtn2,
				"Atn2");
			AddString(StringId.FilterCriteriaToStringFunctionBigMul,
				"Big mul");
			AddString(StringId.FilterCriteriaToStringFunctionCosh,
				"Cosh");
			AddString(StringId.FilterCriteriaToStringFunctionLog10,
				"Log10");
			AddString(StringId.FilterCriteriaToStringFunctionSinh,
				"Sinh");
			AddString(StringId.FilterCriteriaToStringFunctionTanh,
				"Tanh");
			AddString(StringId.FilterCriteriaToStringFunctionPadLeft,
				"Pad left");
			AddString(StringId.FilterCriteriaToStringFunctionPadRight,
				"Pad right");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffTick,
				"Date diff tick");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffSecond,
				"Date diff second");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffMilliSecond,
				"Date diff millisecond");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffMinute,
				"Date diff minute");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffHour,
				"Date diff hour");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffDay,
				"Date diff day");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffMonth,
				"Date diff month");
			AddString(StringId.FilterCriteriaToStringFunctionDateDiffYear,
				"Date diff year");
			AddString(StringId.FilterCriteriaToStringFunctionGetDate,
				"Get date");
			AddString(StringId.FilterCriteriaToStringFunctionGetMilliSecond,
				"Get millisecond");
			AddString(StringId.FilterCriteriaToStringFunctionGetSecond,
				"Get second");
			AddString(StringId.FilterCriteriaToStringFunctionGetMinute,
				"Get minute");
			AddString(StringId.FilterCriteriaToStringFunctionGetHour,
				"Get hour");
			AddString(StringId.FilterCriteriaToStringFunctionGetDay,
				"Get day");
			AddString(StringId.FilterCriteriaToStringFunctionGetMonth,
				"Get month");
			AddString(StringId.FilterCriteriaToStringFunctionGetYear,
				"Get year");
			AddString(StringId.FilterCriteriaToStringFunctionGetDayOfWeek,
				"Get day of week");
			AddString(StringId.FilterCriteriaToStringFunctionGetDayOfYear,
				"Get day of year");
			AddString(StringId.FilterCriteriaToStringFunctionGetTimeOfDay,
				"Get time of day");
			AddString(StringId.FilterCriteriaToStringFunctionNow,
				"Now");
			AddString(StringId.FilterCriteriaToStringFunctionUtcNow,
				"Utc now");
			AddString(StringId.FilterCriteriaToStringFunctionToday,
				"Today");
			AddString(StringId.FilterCriteriaToStringFunctionAddTimeSpan,
				"Add time span");
			AddString(StringId.FilterCriteriaToStringFunctionAddTicks,
				"Add ticks");
			AddString(StringId.FilterCriteriaToStringFunctionAddMilliSeconds,
				"Add milliseconds");
			AddString(StringId.FilterCriteriaToStringFunctionAddSeconds,
				"Add seconds");
			AddString(StringId.FilterCriteriaToStringFunctionAddMinutes,
				"Add minutes");
			AddString(StringId.FilterCriteriaToStringFunctionAddHours,
				"Add hours");
			AddString(StringId.FilterCriteriaToStringFunctionAddDays,
				"Add days");
			AddString(StringId.FilterCriteriaToStringFunctionAddMonths,
				"Add months");
			AddString(StringId.FilterCriteriaToStringFunctionAddYears,
				"Add years");
			AddString(StringId.FilterCriteriaInvalidExpression,
				"The specified expression contains invalid symbols (line {0}, character {1}).");
			AddString(StringId.FilterCriteriaInvalidExpressionEx,
				"The specified expression is invalid.");
			AddString(StringId.Apply,
				"Apply");
			AddString(StringId.PreviewPanelText,
				"Preview,");
			AddString(StringId.TransparentBackColorNotSupported,
				"This control does not support transparent background colors");
			AddString(StringId.FilterOutlookDateText,
				"Show all|Show Empty|Filter by a specific date,|Beyond this year|Later this year|Later this month|Next week|" +
					   "Later this week|Tomorrow|Today|Yesterday|Earlier this week|Last week|Earlier this month|Earlier this year|" +
					   "Prior to this year");
			AddString(StringId.FilterDateTextAlt,
				"Show all|Show Empty|Filter by a specific date,|Beyond|||Next week|Today|This week|This month|Earlier|{0,yyyy}, {0,MMMM}");
			AddString(StringId.FilterDateTimeConstantMenuCaption,
				"Date and time constants");
			AddString(StringId.FilterDateTimeOperatorMenuCaption,
				"Date and time operators");
			AddString(StringId.DefaultBooleanTrue,
				"True");
			AddString(StringId.DefaultBooleanFalse,
				"False");
			AddString(StringId.DefaultBooleanDefault,
				"Default");
			AddString(StringId.ProgressExport, "Exporting");
			AddString(StringId.ProgressPrinting, "Printing");
			AddString(StringId.ProgressCreateDocument, "Creating document");
			AddString(StringId.ProgressCancel, "Cancel");
			AddString(StringId.ProgressCancelPending, "Cancel pending");
			AddString(StringId.ProgressLoadingData, "Loading data");
			AddString(StringId.FilterAggregateAvg, "Avg");
			AddString(StringId.FilterAggregateCount, "Count");
			AddString(StringId.FilterAggregateExists, "Exists");
			AddString(StringId.FilterAggregateMax, "Max");
			AddString(StringId.FilterAggregateMin, "Min");
			AddString(StringId.FilterAggregateSum, "Sum");
			AddString(StringId.FieldListName, "Field List ({0})");
			AddString(StringId.FilterCriteriaToStringFunctionStartsWith, "Starts with");
			AddString(StringId.FilterCriteriaToStringFunctionEndsWith, "Ends with");
			AddString(StringId.FilterCriteriaToStringFunctionContains, "Contains");
			AddString(StringId.RestoreLayoutDialogFileFilter, "XML files (*.xml)|*.xml|All files|*.*");
			AddString(StringId.SaveLayoutDialogFileFilter, "XML files (*.xml)|*.xml");
			AddString(StringId.SaveLayoutDialogTitle, "Save Layout");
			AddString(StringId.RestoreLayoutDialogTitle, "Restore Layout");
			AddString(StringId.PictureEditMenuZoom, "Zoom");
			AddString(StringId.PictureEditMenuFullSize, "Full Size");
			AddString(StringId.PictureEditMenuFitImage, "Fit Image");
			AddString(StringId.PictureEditMenuZoomIn, "Zoom In");
			AddString(StringId.PictureEditMenuZoomOut, "Zoom Out");
			AddString(StringId.PictureEditMenuZoomTo, "Zoom to,");
			AddString(StringId.PictureEditMenuZoomToolTip, "{0}%");
			AddString(StringId.FilterPopupToolbarShowOnlyAvailableItems, "Show Only Available Items");
			AddString(StringId.FilterPopupToolbarShowNewValues, "Show New Field Values");
			AddString(StringId.FilterPopupToolbarIncrementalSearch, "Incremental Search");
			AddString(StringId.FilterPopupToolbarMultiSelection, "Multi-Selection");
			AddString(StringId.FilterPopupToolbarRadioMode, "Radio Mode");
			AddString(StringId.FilterPopupToolbarInvertFilter, "Invert Filter");
			AddString(StringId.ColorPickPopupAutomaticItemCaption, "Automatic");
			AddString(StringId.ColorPickPopupThemeColorsGroupCaption, "Theme Colors");
			AddString(StringId.ColorPickPopupStandardColorsGroupCaption, "Standard Colors");
			AddString(StringId.ColorPickPopupRecentColorsGroupCaption, "Recent Colors");
			AddString(StringId.ColorPickPopupMoreColorsItemCaption, "More Colors...");
			AddString(StringId.ColorPickHueAxisName, "Hue");
			AddString(StringId.ColorPickSaturationAxisName, "Saturation");
			AddString(StringId.ColorPickLuminanceAxisName, "Luminance");
			AddString(StringId.ColorPickBrightnessAxisName, "Brightness");
			AddString(StringId.ColorPickOpacityAxisName, "Opacity");
			AddString(StringId.ColorPickRedValidationMsg, "Red component should be in range 0..255");
			AddString(StringId.ColorPickGreenValidationMsg, "Green component should be in range 0..255");
			AddString(StringId.ColorPickBlueValidationMsg, "Blue component should be in range 0..255");
			AddString(StringId.ColorPickOpacityValidationMsg, "Opacity component should be in range 0..255");
			AddString(StringId.ColorPickColorHexValidationMsg, "Invalid hexadecimal value");
			AddString(StringId.ColorPickHueValidationMsg, "Hue component should be in range 0..359");
			AddString(StringId.ColorPickSaturationValidationMsg, "Saturation component should be in range 0..100");
			AddString(StringId.ColorPickBrightValidationMsg, "Brightness component should be in range 0..100");
			AddString(StringId.ColorTabWebSafeColors, "Web-Safe");
			AddString(StringId.IncorrectNumberCopies, "The number must be between 1 and 32767.");
			AddString(StringId.ChartRangeControlClientInvalidGrid, "The interval  between grid lines is too small to be represented in the range control.");
			AddString(StringId.ChartRangeControlClientNoData, "There is no data to represent in a range control.");
			AddString(StringId.DataBarBlue, "Blue Data Bar");
			AddString(StringId.DataBarLightBlue, "Light Blue Data Bar");
			AddString(StringId.DataBarGreen, "Green Data Bar");
			AddString(StringId.DataBarYellow, "Yellow Data Bar");
			AddString(StringId.DataBarOrange, "Orange Data Bar");
			AddString(StringId.DataBarMint, "Mint Data Bar");
			AddString(StringId.DataBarViolet, "Violet Data Bar");
			AddString(StringId.DataBarRaspberry, "Raspberry Data Bar");
			AddString(StringId.DataBarCoral, "Coral Data Bar");
			AddString(StringId.DataBarBlueGradient, "Blue Data Bar Gradient");
			AddString(StringId.DataBarLightBlueGradient, "Light Blue Data Bar Gradient");
			AddString(StringId.DataBarGreenGradient, "Green Data Bar Gradient");
			AddString(StringId.DataBarYellowGradient, "Yellow Data Bar Gradient");
			AddString(StringId.DataBarOrangeGradient, "Orange Data Bar Gradient");
			AddString(StringId.DataBarMintGradient, "Mint Data Bar Gradient");
			AddString(StringId.DataBarVioletGradient, "Violet Data Bar Gradient");
			AddString(StringId.DataBarRaspberryGradient, "Raspberry Data Bar Gradient");
			AddString(StringId.DataBarCoralGradient, "Coral Data Bar Gradient");
			AddString(StringId.FormatRuleMenuItemClearColumnRules, "Clear Rules from This Column");
			AddString(StringId.FormatRuleMenuItemClearAllRules, "Clear Rules from All Columns");
			AddString(StringId.FormatRuleMenuItemHighlightCellRules, "Highlight Cell Rules");
			AddString(StringId.FormatRuleMenuItemTopBottomRules, "Top/Bottom Rules");
			AddString(StringId.FormatRuleMenuItemDataBars, "Data Bars");
			AddString(StringId.FormatRuleMenuItemColorScales, "Color Scales");
			AddString(StringId.FormatRuleMenuItemIconSets, "Icon Sets");
			AddString(StringId.FormatRuleMenuItemClearRules, "Clear Rules");
			AddString(StringId.FormatRuleMenuItemManageRules, "Manage Rules...");
			AddString(StringId.FormatRuleMenuItemUniqueDuplicateRules, "Unique/Duplicate Rules");
			AddString(StringId.FormatRuleMenuItemGradientFill, "Gradient Fill");
			AddString(StringId.FormatRuleMenuItemSolidFill, "Solid Fill");
			AddString(StringId.FormatRuleMenuItemDataBarDescription, "Add a colored data bar to represent\r\nthe value in a cell. The higher the\r\nvalue, the longer the bar.");
			AddString(StringId.IconSetCategoryRatings, "Ratings");
			AddString(StringId.IconSetCategoryIndicators, "Indicators");
			AddString(StringId.IconSetCategorySymbols, "Symbols");
			AddString(StringId.IconSetCategoryShapes, "Shapes");
			AddString(StringId.IconSetCategoryDirectional, "Directional");
			AddString(StringId.IconSetCategoryPositiveNegative, "Positive/Negative");
			AddString(StringId.FormatRuleMenuItemIconSetDescription, "Use this icon set to classify column\r\nvalues into the following ranges:");
			AddString(StringId.ColorScaleGreenYellowRed, "Green - Yellow - Red");
			AddString(StringId.ColorScalePurpleWhiteAzure, "Purple - White - Azure");
			AddString(StringId.ColorScaleYellowOrangeCoral, "Yellow - Orange - Coral");
			AddString(StringId.ColorScaleGreenWhiteRed, "Green - White - Red");
			AddString(StringId.ColorScaleEmeraldAzureBlue, "Emerald - Azure - Blue");
			AddString(StringId.ColorScaleWhiteRed, "White - Red");
			AddString(StringId.ColorScaleWhiteGreen, "White - Green");
			AddString(StringId.ColorScaleWhiteAzure, "White - Azure");
			AddString(StringId.ColorScaleYellowGreen, "Yellow - Green");
			AddString(StringId.ColorScaleBlueWhiteRed, "Blue - White - Red");
			AddString(StringId.FormatRuleMenuItemColorScaleDescription, "Apply a color gradient to a range of\r\ncells in this column. The color indicates\r\nwhere each cell falls within that range.");
			AddString(StringId.FormatRuleMenuItemUnique, "Unique Values");
			AddString(StringId.FormatRuleUniqueText, "Format cells that are UNIQUE VALUES");
			AddString(StringId.FormatRuleMenuItemDuplicate, "Duplicate Values");
			AddString(StringId.FormatRuleDuplicateText, "Format cells that are DUPLICATE VALUES");
			AddString(StringId.FormatRuleApplyFormatProperty, "Apply formatting to an entire row");
			AddString(StringId.FormatRuleWith, "with");
			AddString(StringId.FormatRuleForThisColumnWith, "for this column with");
			AddString(StringId.IconSetTitleStars3, "3 Stars");
			AddString(StringId.IconSetTitleRatings4, "4 Ratings");
			AddString(StringId.IconSetTitleRatings5, "5 Ratings");
			AddString(StringId.IconSetTitleQuarters5, "5 Quarters");
			AddString(StringId.IconSetTitleBoxes5, "5 Boxes");
			AddString(StringId.IconSetTitleArrows3Colored, "3 Arrows (Colored)");
			AddString(StringId.IconSetTitleArrows3Gray, "3 Arrows (Gray)");
			AddString(StringId.IconSetTitleTriangles3, "3 Triangles");
			AddString(StringId.IconSetTitleArrows4Colored, "4 Arrows (Colored)");
			AddString(StringId.IconSetTitleArrows4Gray, "4 Arrows (Gray)");
			AddString(StringId.IconSetTitleArrows5Colored, "5 Arrows (Colored)");
			AddString(StringId.IconSetTitleArrows5Gray, "5 Arrows (Gray)");
			AddString(StringId.IconSetTitleTrafficLights3Unrimmed, "3 Traffic Lights (Unrimmed)");
			AddString(StringId.IconSetTitleTrafficLights3Rimmed, "3 Traffic Lights (Rimmed)");
			AddString(StringId.IconSetTitleSigns3, "3 Signs");
			AddString(StringId.IconSetTitleTrafficLights4, "4 Traffic Lights");
			AddString(StringId.IconSetTitleRedToBlack, "Red To Black");
			AddString(StringId.IconSetTitleSymbols3Circled, "3 Symbols (Circled)");
			AddString(StringId.IconSetTitleSymbols3Uncircled, "3 Symbols (Uncircled)");
			AddString(StringId.IconSetTitleFlags3, "3 Flags");
			AddString(StringId.IconSetTitlePositiveNegativeArrows, "Arrows (Colored)");
			AddString(StringId.IconSetTitlePositiveNegativeArrowsGray, "Arrows (Gray)");
			AddString(StringId.IconSetTitlePositiveNegativeTriangles, "Triangles");
			AddString(StringId.FormatRuleMenuItemTop10Items, "Top 10 Items");
			AddString(StringId.FormatRuleMenuItemTop10Percent, "Top 10 %");
			AddString(StringId.FormatRuleMenuItemBottom10Items, "Bottom 10 Items");
			AddString(StringId.FormatRuleMenuItemBottom10Percent, "Bottom 10 %");
			AddString(StringId.FormatRuleMenuItemAboveAverage, "Above Average");
			AddString(StringId.FormatRuleMenuItemBelowAverage, "Below Average");
			AddString(StringId.FormatRuleTopText, "Format cells that rank in the TOP");
			AddString(StringId.FormatRuleBottomText, "Format cells that rank in the BOTTOM");
			AddString(StringId.FormatRuleAboveAverageText, "Format cells that are ABOVE AVERAGE");
			AddString(StringId.FormatRuleBelowAverageText, "Format cells that are BELOW AVERAGE");
			AddString(StringId.FormatRuleMenuItemGreaterThan, "Greater Than");
			AddString(StringId.FormatRuleMenuItemLessThan, "Less Than");
			AddString(StringId.FormatRuleMenuItemBetween, "Between");
			AddString(StringId.FormatRuleMenuItemEqualTo, "Equal To");
			AddString(StringId.FormatRuleMenuItemTextThatContains, "Text that Contains");
			AddString(StringId.FormatRuleMenuItemCustomCondition, "Custom Condition");
			AddString(StringId.FormatRuleGreaterThanText, "Format cells that are GREATER THAN");
			AddString(StringId.FormatRuleLessThanText, "Format cells that are LESS THAN");
			AddString(StringId.FormatRuleBetweenText, "Format cells that are BETWEEN");
			AddString(StringId.FormatRuleEqualToText, "Format cells that are EQUAL TO");
			AddString(StringId.FormatRuleTextThatContainsText, "Format cells that contain the text");
			AddString(StringId.FormatRuleCustomConditionText, "Format cells that match the following condition");
			AddString(StringId.FormatRuleExpressionEmptyEnter, "<enter an expression>");
			AddString(StringId.FormatRuleMenuItemDateOccurring, "A Date Occurring");
			AddString(StringId.FormatRuleDateOccurring, "Format cells that contain a date matching these conditions");
			AddString(StringId.FormatPredefinedAppearanceRedFillRedText, "Red Fill with Red Text");
			AddString(StringId.FormatPredefinedAppearanceYellowFillYellowText, "Yellow Fill with Yellow Text");
			AddString(StringId.FormatPredefinedAppearanceGreenFillGreenText, "Green Fill with Green Text");
			AddString(StringId.FormatPredefinedAppearanceRedFill, "Red Fill");
			AddString(StringId.FormatPredefinedAppearanceRedText, "Red Text");
			AddString(StringId.FormatPredefinedAppearanceGreenFill, "Green Fill");
			AddString(StringId.FormatPredefinedAppearanceGreenText, "Green Text");
			AddString(StringId.FormatPredefinedAppearanceBoldText, "Bold Text");
			AddString(StringId.FormatPredefinedAppearanceItalicText, "Italic Text");
			AddString(StringId.FormatPredefinedAppearanceStrikeoutText, "Strikeout Text");
			AddString(StringId.FormatPredefinedAppearanceRedBoldText, "Red Bold Text");
			AddString(StringId.FormatPredefinedAppearanceGreenBoldText, "Green Bold Text");
			AddString(StringId.SearchForColumn, "Search for a column...");
			AddString(StringId.SearchForField, "Search for a field...");
			AddString(StringId.ManageRuleCaption, "Conditional Formatting Rules Manager");
			AddString(StringId.ManageRuleShowFormattingRules, "Show formatting rules for:");
			AddString(StringId.ManageRuleUp, "Up");
			AddString(StringId.ManageRuleDown, "Down");
			AddString(StringId.ManageRuleNewRule, "New Rule...");
			AddString(StringId.ManageRuleEditRule, "Edit Rule...");
			AddString(StringId.ManageRuleDeleteRule, "Delete Rule");
			AddString(StringId.ManageRuleGridCaptionRule, "Rule");
			AddString(StringId.ManageRuleGridCaptionFormat, "Format");
			AddString(StringId.ManageRuleGridCaptionApplyToTheRow, "Apply to the row");
			AddString(StringId.ManageRuleGridCaptionColumn, "Column");
			AddString(StringId.ManageRuleAllColumns, "(All)");
			AddString(StringId.NewFormattingRule, "New Formatting Rule");
			AddString(StringId.EditFormattingRule, "Edit Formatting Rule");
			AddString(StringId.NewEditFormattingRuleSelectARuleType, "Select a Rule Type:");
			AddString(StringId.NewEditFormattingRuleEditTheRuleDescription, "Edit the Rule Description:");
			AddString(StringId.NewEditFormattingRuleFormatAllCellsBasedOnTheirValues, "Format all cells based on their values");
			AddString(StringId.NewEditFormattingRuleFormatOnlyCellsThatContain, "Format only cells that contain");
			AddString(StringId.NewEditFormattingRuleFormatOnlyTopOrBottomRankedValues, "Format only top or bottom ranked values");
			AddString(StringId.NewEditFormattingRuleFormatOnlyValuesThatAreAboveOrBelowAverage, "Format only values that are above or below average");
			AddString(StringId.NewEditFormattingRuleFormatOnlyUniqueOrDuplicateValues, "Format only unique or duplicate values");
			AddString(StringId.NewEditFormattingRuleUseAFormulaToDetermineWhichCellsToFormat, "Use a formula to determine which cells to format");
			AddString(StringId.ManageRuleComplexRuleBaseFormatStyle, "Format Style:");
			AddString(StringId.ManageRuleColorScale2, "2-Color Scale");
			AddString(StringId.ManageRuleColorScale3, "3-Color Scale");
			AddString(StringId.ManageRuleDataBar, "Data Bar");
			AddString(StringId.ManageRuleIconSets, "Icon Sets");
			AddString(StringId.ManageRuleCommonMinimum, "Minimum");
			AddString(StringId.ManageRuleCommonMaximum, "Maximum");
			AddString(StringId.ManageRuleCommonType, "Type:");
			AddString(StringId.ManageRuleCommonPercent, "Percent");
			AddString(StringId.ManageRuleCommonNumber, "Number");
			AddString(StringId.ManageRuleCommonValue, "Value:");
			AddString(StringId.ManageRuleCommonColor, "Color:");
			AddString(StringId.ManageRuleCommonPreview, "Preview:");
			AddString(StringId.ManageRuleNoFormatSet, "No Format Set");
			AddString(StringId.ManageRuleColorScaleMidpoint, "Midpoint");
			AddString(StringId.ManageRuleDataBarBarAppearance, "Bar Appearance:");
			AddString(StringId.ManageRuleDataBarFill, "Fill:");
			AddString(StringId.ManageRuleDataBarBorder, "Border:");
			AddString(StringId.ManageRuleDataBarDrawAxis, "Draw Axis");
			AddString(StringId.ManageRuleDataBarUseNegativeBar, "Use Negative Bar");
			AddString(StringId.ManageRuleDataBarAxisColor, "Axis Color:");
			AddString(StringId.ManageRuleDataBarBarDirection, "Bar Direction:");
			AddString(StringId.ManageRuleDataBarSolidFill, "Solid Fill");
			AddString(StringId.ManageRuleDataBarGradientFill, "Gradient Fill");
			AddString(StringId.ManageRuleDataBarNoBorder, "No Border");
			AddString(StringId.ManageRuleDataBarSolidBorder, "Solid Border");
			AddString(StringId.ManageRuleDataBarContext, "Context");
			AddString(StringId.ManageRuleDataBarLTR, "Left-to-Right");
			AddString(StringId.ManageRuleDataBarRTL, "Right-to-Left");
			AddString(StringId.ManageRuleIconSetsDisplayEachIconAccordingToTheseRules, "Display each icon according to these rules:");
			AddString(StringId.ManageRuleIconSetsReverseIconOrder, "Reverse Icon Order");
			AddString(StringId.ManageRuleIconSetsWhen, "When");
			AddString(StringId.ManageRuleIconSetsValueIs, "value is");
			AddString(StringId.ManageRuleSimpleRuleBaseFormat, "Format...");
			AddString(StringId.ManageRuleAverageFormatValuesThatAre, "Format values that are:");
			AddString(StringId.ManageRuleAverageTheAverageForTheSelectedRange, "the average of column values");
			AddString(StringId.ManageRuleAverageAbove, "Above");
			AddString(StringId.ManageRuleAverageBelow, "Below");
			AddString(StringId.ManageRuleAverageEqualOrAbove, "Equal Or Above");
			AddString(StringId.ManageRuleAverageEqualOrBelow, "Equal Or Below");
			AddString(StringId.ManageRuleFormulaFormatValuesWhereThisFormulaIsTrue, "Format values where this formula is true:");
			AddString(StringId.ManageRuleRankedValuesFormatValuesThatRankInThe, "Format values that rank in the:");
			AddString(StringId.ManageRuleRankedValuesOfTheColumnsCellValues, "% of column values");
			AddString(StringId.ManageRuleRankedValuesTop, "Top");
			AddString(StringId.ManageRuleRankedValuesBottom, "Bottom");
			AddString(StringId.ManageRuleThatContainFormatOnlyCellsWith, "Format only cells with:");
			AddString(StringId.ManageRuleThatContainCellValue, "Cell Value");
			AddString(StringId.ManageRuleThatContainDatesOccurring, "Dates Occurring");
			AddString(StringId.ManageRuleThatContainSpecificText, "Specific Text");
			AddString(StringId.ManageRuleThatContainBlanks, "Blanks");
			AddString(StringId.ManageRuleThatContainNoBlanks, "No Blanks");
			AddString(StringId.ManageRuleThatContainErrors, "Errors");
			AddString(StringId.ManageRuleThatContainNoErrors, "No Errors");
			AddString(StringId.ManageRuleCellValueBetween, "Between");
			AddString(StringId.ManageRuleCellValueNotBetween, "Not Between");
			AddString(StringId.ManageRuleCellValueEqualTo, "Equal To");
			AddString(StringId.ManageRuleCellValueNotEqualTo, "Not Equal To");
			AddString(StringId.ManageRuleCellValueGreaterThan, "Greater Than");
			AddString(StringId.ManageRuleCellValueLessThan, "Less Than");
			AddString(StringId.ManageRuleCellValueGreaterThanOrEqualTo, "Greater Than Or Equal To");
			AddString(StringId.ManageRuleCellValueLessThanOrEqualTo, "Less Than Or Equal To");
			AddString(StringId.ManageRuleDatesOccurringYesterday, "Yesterday");
			AddString(StringId.ManageRuleDatesOccurringToday, "Today");
			AddString(StringId.ManageRuleDatesOccurringTomorrow, "Tomorrow");
			AddString(StringId.ManageRuleDatesOccurringLastWeek, "Last Week");
			AddString(StringId.ManageRuleDatesOccurringThisWeek, "This Week");
			AddString(StringId.ManageRuleDatesOccurringNextWeek, "Next Week");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo1, "Last Month");
			AddString(StringId.ManageRuleDatesOccurringThisMonth, "This Month");
			AddString(StringId.ManageRuleDatesOccurringMonthAfter1, "Next Month");
			AddString(StringId.ManageRuleDatesOccurringEarlierThisWeek, "This week, prior to yesterday");
			AddString(StringId.ManageRuleDatesOccurringEarlierThisMonth, "This month, prior to previous week");
			AddString(StringId.ManageRuleDatesOccurringEarlierThisYear, "This year, prior to this month");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo2, "During the month 2 months ago");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo3, "During the month 3 months ago");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo4, "During the month 4 months ago");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo5, "During the month 5 months ago");
			AddString(StringId.ManageRuleDatesOccurringMonthAgo6, "During the month 6 months ago");
			AddString(StringId.ManageRuleDatesOccurringEarlier, "Prior to the month 6 months ago");
			AddString(StringId.ManageRuleDatesOccurringPriorThisYear, "Prior to this year");
			AddString(StringId.ManageRuleDatesOccurringLaterThisWeek, "This week, beyond tomorrow");
			AddString(StringId.ManageRuleDatesOccurringLaterThisMonth, "This month, beyond next week");
			AddString(StringId.ManageRuleDatesOccurringLaterThisYear, "This year, beyond this month");
			AddString(StringId.ManageRuleDatesOccurringMonthAfter2, "During the month in 2 months time");
			AddString(StringId.ManageRuleDatesOccurringBeyond, "Following the month in 2 months time");
			AddString(StringId.ManageRuleDatesOccurringBeyondThisYear, "Following this year");
			AddString(StringId.ManageRuleSpecificTextContaining, "Containing");
			AddString(StringId.ManageRuleSpecificTextNotContaining, "Not Containing");
			AddString(StringId.ManageRuleSpecificTextBeginningWith, "Beginning With");
			AddString(StringId.ManageRuleSpecificTextEndingWith, "Ending With");
			AddString(StringId.ManageRuleUniqueOrDuplicateFormatAll, "Format all:");
			AddString(StringId.ManageRuleUniqueOrDuplicateValuesInTheSelectedRange, "column values");
			AddString(StringId.ManageRuleUniqueOrDuplicateUnique, "Unique");
			AddString(StringId.ManageRuleUniqueOrDuplicateDuplicate, "Duplicate");
			AddString(StringId.ManageRuleColorScale, "Graded Color Scale");
			AddString(StringId.ManageRuleIconSet, "Icon Set");
			AddString(StringId.ManageRuleFormula, "Formula");
			AddString(StringId.ManageRuleAboveAverage, "Above Average");
			AddString(StringId.ManageRuleBelowAverage, "Below Average");
			AddString(StringId.ManageRuleEqualOrAboveAverage, "Equal to or Above Average");
			AddString(StringId.ManageRuleEqualOrBelowAverage, "Equal to or Below Average");
			AddString(StringId.ManageRuleFormatCellsCaption, "Format Cells");
			AddString(StringId.ManageRuleFormatCellsFont, "Font");
			AddString(StringId.ManageRuleFormatCellsFill, "Fill");
			AddString(StringId.ManageRuleFormatCellsPredefinedAppearance, "Predefined Appearance");
			AddString(StringId.ManageRuleFormatCellsFontStyle, "Font style");
			AddString(StringId.ManageRuleFormatCellsFontColor, "Font color");
			AddString(StringId.ManageRuleFormatCellsEffects, "Effects:");
			AddString(StringId.ManageRuleFormatCellsUnderline, "Underline");
			AddString(StringId.ManageRuleFormatCellsStrikethrough, "Strikethrough");
			AddString(StringId.ManageRuleFormatCellsClear, "Clear");
			AddString(StringId.ManageRuleFormatCellsBackgroundColor, "Background Color:");
			AddString(StringId.ManageRuleFormatCellsNone, "None");
			AddString(StringId.ManageRuleFormatCellsRegular, "Regular");
			AddString(StringId.ManageRuleFormatCellsBold, "Bold");
			AddString(StringId.ManageRuleFormatCellsItalic, "Italic");
		}
		#endregion
		#region string GetLocalizedString(string functionCaption)
		public static string GetLocalizedString(string functionCaption) {
			if(Enum.IsDefined(typeof(StringId), functionCaption)) {
				StringId constantName = (StringId)Enum.Parse(typeof(StringId), functionCaption);
				return Active.GetLocalizedString(constantName);
			}
			return functionCaption;
		}
		#endregion
	}
	public class EditResLocalizer : XtraResXLocalizer<StringId> {
		public EditResLocalizer() : base(new Localizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraEditors.LocalizationRes", typeof(EditResLocalizer).Assembly);
		}
	}
	#region enum StringId
	public enum StringId {
		None,
		CaptionError,
		InvalidValueText,
		CheckChecked,
		CheckUnchecked,
		CheckIndeterminate,
		DateEditToday,
		DateEditClear,
		OK,
		Cancel,
		NavigatorFirstButtonHint,
		NavigatorPreviousButtonHint,
		NavigatorPreviousPageButtonHint,
		NavigatorNextButtonHint,
		NavigatorNextPageButtonHint,
		NavigatorLastButtonHint,
		NavigatorAppendButtonHint,
		NavigatorRemoveButtonHint,
		NavigatorEditButtonHint,
		NavigatorEndEditButtonHint,
		NavigatorCancelEditButtonHint,
		NavigatorTextStringFormat,
		PictureEditMenuCut,
		PictureEditMenuCopy,
		PictureEditMenuPaste,
		PictureEditMenuDelete,
		PictureEditMenuLoad,
		PictureEditMenuSave,
		PictureEditOpenFileFilter,
		PictureEditSaveFileFilter,
		PictureEditOpenFileTitle,
		PictureEditSaveFileTitle,
		PictureEditOpenFileError,
		PictureEditOpenFileErrorCaption,
		PictureEditCopyImageError,
		LookUpEditValueIsNull,
		LookUpInvalidEditValueType,
		LookUpColumnDefaultName,
		MaskBoxValidateError,
		UnknownPictureFormat,
		DataEmpty,
		NotValidArrayLength,
		ImagePopupEmpty,
		ImagePopupPicture,
		ColorTabCustom,
		ColorTabWeb,
		ColorTabSystem,
		CalcButtonMC,
		CalcButtonMR,
		CalcButtonMS,
		CalcButtonMx,
		CalcButtonSqrt,
		CalcButtonBack,
		CalcButtonCE,
		CalcButtonC,
		CalcError,
		TabHeaderButtonPrev,
		TabHeaderButtonNext,
		TabHeaderButtonClose,
		TabHeaderButtonPin,
		TabHeaderButtonUnpin,
		TabHeaderSelectorButton,
		XtraMessageBoxOkButtonText,
		XtraMessageBoxCancelButtonText,
		XtraMessageBoxYesButtonText,
		XtraMessageBoxNoButtonText,
		XtraMessageBoxAbortButtonText,
		XtraMessageBoxRetryButtonText,
		XtraMessageBoxIgnoreButtonText,
		TextEditMenuUndo,
		TextEditMenuCut,
		TextEditMenuCopy,
		TextEditMenuPaste,
		TextEditMenuDelete,
		TextEditMenuSelectAll,
		FilterEditorTabText,
		FilterEditorTabVisual,
		FilterShowAll,
		FilterGroupAnd,
		FilterGroupNotAnd,
		FilterGroupNotOr,
		FilterGroupOr,
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
		FilterClauseIsNotNullOrEmpty,
		FilterClauseIsNullOrEmpty,
		FilterClauseLess,
		FilterClauseLessOrEqual,
		FilterClauseLike,
		FilterClauseNoneOf,
		FilterClauseNotBetween,
		FilterClauseDoesNotContain,
		FilterClauseDoesNotEqual,
		FilterClauseNotLike,
		FilterEmptyEnter,
		FilterEmptyParameter,
		FilterMenuAddNewParameter,
		FilterEmptyValue,
		FilterMenuConditionAdd,
		FilterMenuGroupAdd,
		FilterMenuClearAll,
		FilterMenuRowRemove,
		FilterToolTipNodeAdd,
		FilterToolTipNodeRemove,
		FilterToolTipNodeAction,
		FilterToolTipValueType,
		FilterToolTipElementAdd,
		FilterToolTipKeysAdd,
		FilterToolTipKeysRemove,
		ContainerAccessibleEditName,
		FilterCriteriaToStringGroupOperatorAnd,
		FilterCriteriaToStringGroupOperatorOr,
		FilterCriteriaToStringUnaryOperatorBitwiseNot,
		FilterCriteriaToStringUnaryOperatorIsNull,
		FilterCriteriaToStringUnaryOperatorMinus,
		FilterCriteriaToStringUnaryOperatorNot,
		FilterCriteriaToStringUnaryOperatorPlus,
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
		FilterCriteriaToStringBetween,
		FilterCriteriaToStringIn,
		FilterCriteriaToStringIsNotNull,
		FilterCriteriaToStringNotLike,
		FilterCriteriaToStringFunctionIif,
		FilterCriteriaToStringFunctionIsNull,
		FilterCriteriaToStringFunctionLen,
		FilterCriteriaToStringFunctionLower,
		FilterCriteriaToStringFunctionNone,
		FilterCriteriaToStringFunctionSubstring,
		FilterCriteriaToStringFunctionTrim,
		FilterCriteriaToStringFunctionUpper,
		FilterCriteriaToStringFunctionIsThisYear,
		FilterCriteriaToStringFunctionIsThisMonth,
		FilterCriteriaToStringFunctionIsThisWeek,
		FilterCriteriaToStringFunctionLocalDateTimeThisYear,
		FilterCriteriaToStringFunctionLocalDateTimeThisMonth,
		FilterCriteriaToStringFunctionLocalDateTimeLastWeek,
		FilterCriteriaToStringFunctionLocalDateTimeThisWeek,
		FilterCriteriaToStringFunctionLocalDateTimeYesterday,
		FilterCriteriaToStringFunctionLocalDateTimeToday,
		FilterCriteriaToStringFunctionLocalDateTimeNow,
		FilterCriteriaToStringFunctionLocalDateTimeTomorrow,
		FilterCriteriaToStringFunctionLocalDateTimeDayAfterTomorrow,
		FilterCriteriaToStringFunctionLocalDateTimeNextWeek,
		FilterCriteriaToStringFunctionLocalDateTimeTwoWeeksAway,
		FilterCriteriaToStringFunctionLocalDateTimeNextMonth,
		FilterCriteriaToStringFunctionLocalDateTimeNextYear,
		FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear,
		FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear,
		FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth,
		FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek,
		FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek,
		FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow,
		FilterCriteriaToStringFunctionIsOutlookIntervalToday,
		FilterCriteriaToStringFunctionIsOutlookIntervalYesterday,
		FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek,
		FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek,
		FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth,
		FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear,
		FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear,
		FilterCriteriaToStringFunctionCustom,
		FilterCriteriaToStringFunctionCustomNonDeterministic,
		FilterCriteriaToStringFunctionIsNullOrEmpty,
		FilterCriteriaToStringFunctionConcat,
		FilterCriteriaToStringFunctionAscii,
		FilterCriteriaToStringFunctionChar,
		FilterCriteriaToStringFunctionToInt,
		FilterCriteriaToStringFunctionToLong,
		FilterCriteriaToStringFunctionToFloat,
		FilterCriteriaToStringFunctionToDouble,
		FilterCriteriaToStringFunctionToDecimal,
		FilterCriteriaToStringFunctionToStr,
		FilterCriteriaToStringFunctionReplace,
		FilterCriteriaToStringFunctionReverse,
		FilterCriteriaToStringFunctionInsert,
		FilterCriteriaToStringFunctionCharIndex,
		FilterCriteriaToStringFunctionRemove,
		FilterCriteriaToStringFunctionAbs,
		FilterCriteriaToStringFunctionSqr,
		FilterCriteriaToStringFunctionCos,
		FilterCriteriaToStringFunctionSin,
		FilterCriteriaToStringFunctionAtn,
		FilterCriteriaToStringFunctionExp,
		FilterCriteriaToStringFunctionLog,
		FilterCriteriaToStringFunctionRnd,
		FilterCriteriaToStringFunctionTan,
		FilterCriteriaToStringFunctionPower,
		FilterCriteriaToStringFunctionSign,
		FilterCriteriaToStringFunctionRound,
		FilterCriteriaToStringFunctionCeiling,
		FilterCriteriaToStringFunctionFloor,
		FilterCriteriaToStringFunctionMax,
		FilterCriteriaToStringFunctionMin,
		FilterCriteriaToStringFunctionAcos,
		FilterCriteriaToStringFunctionAsin,
		FilterCriteriaToStringFunctionAtn2,
		FilterCriteriaToStringFunctionBigMul,
		FilterCriteriaToStringFunctionCosh,
		FilterCriteriaToStringFunctionLog10,
		FilterCriteriaToStringFunctionSinh,
		FilterCriteriaToStringFunctionTanh,
		FilterCriteriaToStringFunctionPadLeft,
		FilterCriteriaToStringFunctionPadRight,
		FilterCriteriaToStringFunctionDateDiffTick,
		FilterCriteriaToStringFunctionDateDiffSecond,
		FilterCriteriaToStringFunctionDateDiffMilliSecond,
		FilterCriteriaToStringFunctionDateDiffMinute,
		FilterCriteriaToStringFunctionDateDiffHour,
		FilterCriteriaToStringFunctionDateDiffDay,
		FilterCriteriaToStringFunctionDateDiffMonth,
		FilterCriteriaToStringFunctionDateDiffYear,
		FilterCriteriaToStringFunctionGetDate,
		FilterCriteriaToStringFunctionGetMilliSecond,
		FilterCriteriaToStringFunctionGetSecond,
		FilterCriteriaToStringFunctionGetMinute,
		FilterCriteriaToStringFunctionGetHour,
		FilterCriteriaToStringFunctionGetDay,
		FilterCriteriaToStringFunctionGetMonth,
		FilterCriteriaToStringFunctionGetYear,
		FilterCriteriaToStringFunctionGetDayOfWeek,
		FilterCriteriaToStringFunctionGetDayOfYear,
		FilterCriteriaToStringFunctionGetTimeOfDay,
		FilterCriteriaToStringFunctionNow,
		FilterCriteriaToStringFunctionUtcNow,
		FilterCriteriaToStringFunctionToday,
		FilterCriteriaToStringFunctionAddTimeSpan,
		FilterCriteriaToStringFunctionAddTicks,
		FilterCriteriaToStringFunctionAddMilliSeconds,
		FilterCriteriaToStringFunctionAddSeconds,
		FilterCriteriaToStringFunctionAddMinutes,
		FilterCriteriaToStringFunctionAddHours,
		FilterCriteriaToStringFunctionAddDays,
		FilterCriteriaToStringFunctionAddMonths,
		FilterCriteriaToStringFunctionAddYears,
		FilterCriteriaToStringFunctionStartsWith,
		FilterCriteriaToStringFunctionEndsWith,
		FilterCriteriaToStringFunctionContains,
		FilterCriteriaInvalidExpression,
		FilterCriteriaInvalidExpressionEx,
		Apply,
		PreviewPanelText,
		TransparentBackColorNotSupported,
		FilterOutlookDateText,
		FilterDateTimeConstantMenuCaption,
		FilterDateTimeOperatorMenuCaption,
		FilterDateTextAlt,
		FilterFunctionsMenuCaption,
		DefaultBooleanTrue,
		DefaultBooleanFalse,
		DefaultBooleanDefault,
		ProgressExport,
		ProgressPrinting,
		ProgressCreateDocument,
		ProgressCancel,
		ProgressCancelPending,
		ProgressLoadingData,
		FilterAggregateAvg,
		FilterAggregateCount,
		FilterAggregateExists,
		FilterAggregateMax,
		FilterAggregateMin,
		FilterAggregateSum,
		FieldListName,
		RestoreLayoutDialogFileFilter,
		SaveLayoutDialogFileFilter,
		RestoreLayoutDialogTitle,
		SaveLayoutDialogTitle,
		PictureEditMenuZoom,
		PictureEditMenuFullSize,
		PictureEditMenuFitImage,
		PictureEditMenuZoomIn,
		PictureEditMenuZoomOut,
		PictureEditMenuZoomTo,
		PictureEditMenuZoomToolTip,
		FilterPopupToolbarShowOnlyAvailableItems,
		FilterPopupToolbarShowNewValues,
		FilterPopupToolbarIncrementalSearch,
		FilterPopupToolbarMultiSelection,
		FilterPopupToolbarRadioMode,
		FilterPopupToolbarInvertFilter,
		ColorPickPopupAutomaticItemCaption,
		ColorPickPopupThemeColorsGroupCaption,
		ColorPickPopupStandardColorsGroupCaption,
		ColorPickPopupRecentColorsGroupCaption,
		ColorPickPopupMoreColorsItemCaption,
		ColorPickHueAxisName,
		ColorPickSaturationAxisName,
		ColorPickLuminanceAxisName,
		ColorPickBrightnessAxisName,
		ColorPickOpacityAxisName,
		ColorPickRedValidationMsg,
		ColorPickGreenValidationMsg,
		ColorPickBlueValidationMsg,
		ColorPickOpacityValidationMsg,
		ColorPickColorHexValidationMsg,
		ColorPickHueValidationMsg,
		ColorPickSaturationValidationMsg,
		ColorPickBrightValidationMsg,
		ColorTabWebSafeColors,
		Days,
		Hours,
		Mins,
		Secs,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPaused,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewError,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPendingDeletion,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPaperJam,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPaperOut,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewManualFeed,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPaperProblem,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewOffline,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewIOActive,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewBusy,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPrinting,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewOutputBinFull,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewNotAvaible,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewWaiting,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewProcessing,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewInitializing,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewWarmingUp,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewTonerLow,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewNoToner,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPagePunt,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewUserIntervention,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewOutOfMemory,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewDoorOpen,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewServerUnknown,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewPowerSave,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewReady,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewServerOffline,
		[Obsolete("Use DevExpress.XtraPrinting.Localization")]
		PreviewDriverUpdateNeeded,
		IncorrectNumberCopies,
		ChartRangeControlClientInvalidGrid,
		ChartRangeControlClientNoData,
		DataBarBlue,
		DataBarLightBlue,
		DataBarGreen,
		DataBarYellow,
		DataBarOrange,
		DataBarMint,
		DataBarViolet,
		DataBarRaspberry,
		DataBarCoral,
		DataBarBlueGradient,
		DataBarLightBlueGradient,
		DataBarGreenGradient,
		DataBarYellowGradient,
		DataBarOrangeGradient,
		DataBarMintGradient,
		DataBarVioletGradient,
		DataBarRaspberryGradient,
		DataBarCoralGradient,
		FormatRuleMenuItemClearColumnRules,
		FormatRuleMenuItemClearAllRules,
		FormatRuleMenuItemHighlightCellRules,
		FormatRuleMenuItemTopBottomRules,
		FormatRuleMenuItemDataBars,
		FormatRuleMenuItemColorScales,
		FormatRuleMenuItemIconSets,
		FormatRuleMenuItemClearRules,
		FormatRuleMenuItemManageRules,
		FormatRuleMenuItemUniqueDuplicateRules,
		FormatRuleMenuItemGradientFill,
		FormatRuleMenuItemSolidFill,
		FormatRuleMenuItemDataBarDescription,
		IconSetCategoryRatings,
		IconSetCategoryIndicators,
		IconSetCategorySymbols,
		IconSetCategoryShapes,
		IconSetCategoryDirectional,
		IconSetCategoryPositiveNegative,
		FormatRuleMenuItemIconSetDescription,
		ColorScaleGreenYellowRed,
		ColorScalePurpleWhiteAzure,
		ColorScaleYellowOrangeCoral,
		ColorScaleGreenWhiteRed,
		ColorScaleEmeraldAzureBlue,
		ColorScaleWhiteRed,
		ColorScaleWhiteGreen,
		ColorScaleWhiteAzure,
		ColorScaleYellowGreen,
		ColorScaleBlueWhiteRed,
		FormatRuleMenuItemColorScaleDescription,
		FormatRuleMenuItemUnique,
		FormatRuleUniqueText,
		FormatRuleMenuItemDuplicate,
		FormatRuleDuplicateText,
		FormatRuleApplyFormatProperty,
		FormatRuleWith,
		FormatRuleForThisColumnWith,
		IconSetTitleStars3,
		IconSetTitleRatings4,
		IconSetTitleRatings5,
		IconSetTitleQuarters5,
		IconSetTitleBoxes5,
		IconSetTitleArrows3Colored,
		IconSetTitleArrows3Gray,
		IconSetTitleTriangles3,
		IconSetTitleArrows4Colored,
		IconSetTitleArrows4Gray,
		IconSetTitleArrows5Colored,
		IconSetTitleArrows5Gray,
		IconSetTitleTrafficLights3Unrimmed,
		IconSetTitleTrafficLights3Rimmed,
		IconSetTitleSigns3,
		IconSetTitleTrafficLights4,
		IconSetTitleRedToBlack,
		IconSetTitleSymbols3Circled,
		IconSetTitleSymbols3Uncircled,
		IconSetTitleFlags3,
		IconSetTitlePositiveNegativeArrows,
		IconSetTitlePositiveNegativeArrowsGray,
		IconSetTitlePositiveNegativeTriangles,
		FormatRuleMenuItemTop10Items,
		FormatRuleMenuItemTop10Percent,
		FormatRuleMenuItemBottom10Items,
		FormatRuleMenuItemBottom10Percent,
		FormatRuleMenuItemAboveAverage,
		FormatRuleMenuItemBelowAverage,
		FormatRuleTopText,
		FormatRuleBottomText,
		FormatRuleAboveAverageText,
		FormatRuleBelowAverageText,
		FormatRuleMenuItemGreaterThan,
		FormatRuleMenuItemLessThan,
		FormatRuleMenuItemBetween,
		FormatRuleMenuItemEqualTo,
		FormatRuleMenuItemTextThatContains,
		FormatRuleMenuItemCustomCondition,
		FormatRuleGreaterThanText,
		FormatRuleLessThanText,
		FormatRuleBetweenText,
		FormatRuleEqualToText,
		FormatRuleTextThatContainsText,
		FormatRuleCustomConditionText,
		FormatRuleExpressionEmptyEnter,
		FormatRuleMenuItemDateOccurring,
		FormatRuleDateOccurring,
		FormatPredefinedAppearanceRedFillRedText,
		FormatPredefinedAppearanceYellowFillYellowText,
		FormatPredefinedAppearanceGreenFillGreenText,
		FormatPredefinedAppearanceRedFill,
		FormatPredefinedAppearanceRedText,
		FormatPredefinedAppearanceGreenFill,
		FormatPredefinedAppearanceGreenText,
		FormatPredefinedAppearanceBoldText,
		FormatPredefinedAppearanceItalicText,
		FormatPredefinedAppearanceStrikeoutText,
		FormatPredefinedAppearanceRedBoldText,
		FormatPredefinedAppearanceGreenBoldText,
		SearchForColumn,
		SearchForField,
		ManageRuleCaption,
		ManageRuleShowFormattingRules,
		ManageRuleUp,
		ManageRuleDown,
		ManageRuleNewRule,
		ManageRuleEditRule,
		ManageRuleDeleteRule,
		ManageRuleGridCaptionRule,
		ManageRuleGridCaptionFormat,
		ManageRuleGridCaptionApplyToTheRow,
		ManageRuleGridCaptionColumn,
		ManageRuleAllColumns,
		NewFormattingRule,
		EditFormattingRule,
		NewEditFormattingRuleSelectARuleType,
		NewEditFormattingRuleEditTheRuleDescription,
		NewEditFormattingRuleFormatAllCellsBasedOnTheirValues,
		NewEditFormattingRuleFormatOnlyCellsThatContain,
		NewEditFormattingRuleFormatOnlyTopOrBottomRankedValues,
		NewEditFormattingRuleFormatOnlyValuesThatAreAboveOrBelowAverage,
		NewEditFormattingRuleFormatOnlyUniqueOrDuplicateValues,
		NewEditFormattingRuleUseAFormulaToDetermineWhichCellsToFormat,
		ManageRuleComplexRuleBaseFormatStyle,
		ManageRuleColorScale2,
		ManageRuleColorScale3,
		ManageRuleDataBar,
		ManageRuleIconSets,
		ManageRuleCommonMinimum,
		ManageRuleCommonMaximum,
		ManageRuleCommonType,
		ManageRuleCommonPercent,
		ManageRuleCommonNumber,
		ManageRuleCommonValue,
		ManageRuleCommonColor,
		ManageRuleCommonPreview,
		ManageRuleNoFormatSet,
		ManageRuleColorScaleMidpoint,
		ManageRuleDataBarBarAppearance,
		ManageRuleDataBarFill,
		ManageRuleDataBarBorder,
		ManageRuleDataBarDrawAxis,
		ManageRuleDataBarUseNegativeBar,
		ManageRuleDataBarAxisColor,
		ManageRuleDataBarBarDirection,
		ManageRuleDataBarSolidFill,
		ManageRuleDataBarGradientFill,
		ManageRuleDataBarNoBorder,
		ManageRuleDataBarSolidBorder,
		ManageRuleDataBarContext,
		ManageRuleDataBarLTR,
		ManageRuleDataBarRTL,
		ManageRuleIconSetsDisplayEachIconAccordingToTheseRules,
		ManageRuleIconSetsReverseIconOrder,
		ManageRuleIconSetsWhen,
		ManageRuleIconSetsValueIs,
		ManageRuleSimpleRuleBaseFormat,
		ManageRuleAverageFormatValuesThatAre,
		ManageRuleAverageTheAverageForTheSelectedRange,
		ManageRuleAverageAbove,
		ManageRuleAverageBelow,
		ManageRuleAverageEqualOrAbove,
		ManageRuleAverageEqualOrBelow,
		ManageRuleFormulaFormatValuesWhereThisFormulaIsTrue,
		ManageRuleRankedValuesFormatValuesThatRankInThe,
		ManageRuleRankedValuesOfTheColumnsCellValues,
		ManageRuleRankedValuesTop,
		ManageRuleRankedValuesBottom,
		ManageRuleThatContainFormatOnlyCellsWith,
		ManageRuleThatContainCellValue,
		ManageRuleThatContainDatesOccurring,
		ManageRuleThatContainSpecificText,
		ManageRuleThatContainBlanks,
		ManageRuleThatContainNoBlanks,
		ManageRuleThatContainErrors,
		ManageRuleThatContainNoErrors,
		ManageRuleCellValueBetween,
		ManageRuleCellValueNotBetween,
		ManageRuleCellValueEqualTo,
		ManageRuleCellValueNotEqualTo,
		ManageRuleCellValueGreaterThan,
		ManageRuleCellValueLessThan,
		ManageRuleCellValueGreaterThanOrEqualTo,
		ManageRuleCellValueLessThanOrEqualTo,
		ManageRuleDatesOccurringBeyond,
		ManageRuleDatesOccurringBeyondThisYear,
		ManageRuleDatesOccurringEarlier,
		ManageRuleDatesOccurringEarlierThisMonth,
		ManageRuleDatesOccurringEarlierThisWeek,
		ManageRuleDatesOccurringEarlierThisYear,
		ManageRuleDatesOccurringLastWeek,
		ManageRuleDatesOccurringLaterThisMonth,
		ManageRuleDatesOccurringLaterThisWeek,
		ManageRuleDatesOccurringLaterThisYear,
		ManageRuleDatesOccurringMonthAfter1,
		ManageRuleDatesOccurringMonthAfter2,
		ManageRuleDatesOccurringMonthAgo1,
		ManageRuleDatesOccurringMonthAgo2,
		ManageRuleDatesOccurringMonthAgo3,
		ManageRuleDatesOccurringMonthAgo4,
		ManageRuleDatesOccurringMonthAgo5,
		ManageRuleDatesOccurringMonthAgo6,
		ManageRuleDatesOccurringNextWeek,
		ManageRuleDatesOccurringPriorThisYear,
		ManageRuleDatesOccurringThisMonth,
		ManageRuleDatesOccurringThisWeek,
		ManageRuleDatesOccurringTomorrow,
		ManageRuleDatesOccurringToday,
		ManageRuleDatesOccurringYesterday,
		ManageRuleSpecificTextContaining,
		ManageRuleSpecificTextNotContaining,
		ManageRuleSpecificTextBeginningWith,
		ManageRuleSpecificTextEndingWith,
		ManageRuleUniqueOrDuplicateFormatAll,
		ManageRuleUniqueOrDuplicateValuesInTheSelectedRange,
		ManageRuleUniqueOrDuplicateUnique,
		ManageRuleUniqueOrDuplicateDuplicate,
		ManageRuleColorScale,
		ManageRuleIconSet,
		ManageRuleFormula,
		ManageRuleAboveAverage,
		ManageRuleBelowAverage,
		ManageRuleEqualOrAboveAverage,
		ManageRuleEqualOrBelowAverage,
		ManageRuleFormatCellsCaption,
		ManageRuleFormatCellsFont,
		ManageRuleFormatCellsFill,
		ManageRuleFormatCellsPredefinedAppearance,
		ManageRuleFormatCellsFontStyle,
		ManageRuleFormatCellsFontColor,
		ManageRuleFormatCellsEffects,
		ManageRuleFormatCellsUnderline,
		ManageRuleFormatCellsStrikethrough,
		ManageRuleFormatCellsClear,
		ManageRuleFormatCellsBackgroundColor,
		ManageRuleFormatCellsNone,
		ManageRuleFormatCellsRegular,
		ManageRuleFormatCellsBold,
		ManageRuleFormatCellsItalic,
		TakePictureDialogTitle,
		TakePictureMenuItem,
		TakePictureDialogCapture,
		TakePictureDialogTryAgain,
		TakePictureDialogSave,
		CameraSettingsActiveDevice,
		CameraSettingsBrightness,
		CameraSettingsContrast,
		CameraSettingsDesaturate,
		CameraSettingsDefaults,
		CameraSettingsCaption,
		CameraSettingsResolution,
		CameraDeviceNotFound,
		CameraDeviceIsBusy,
		CameraDesignTimeInfo
	}
	#endregion
}
