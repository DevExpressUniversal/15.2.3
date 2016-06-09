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
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public static class LocalaizableCriteriaToStringProcessor {
		public static string Process(CriteriaOperator op) {
			return LocalaizableCriteriaToStringProcessorCore.Process(new LocalaizableCriteriaToStringProcessorLocalizerWrapper(EditorLocalizer.Active), op);
		}
		public class LocalaizableCriteriaToStringProcessorLocalizerWrapper : ILocalaizableCriteriaToStringProcessorOpNamesSource {
			readonly XtraLocalizer<EditorStringId> localizer;
			public LocalaizableCriteriaToStringProcessorLocalizerWrapper(XtraLocalizer<EditorStringId> localizer) {
				this.localizer = localizer;
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetBetweenString() {
				return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBetween);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetInString() {
				return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringIn);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNotNullString() {
				return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringIsNotNull);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNullString() {
				return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorIsNull);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetNotLikeString() {
				return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringNotLike);
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(Aggregate opType) {
				return null;
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(FunctionOperatorType opType) {
				switch(opType) {
					case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsBeyondThisYear);
					case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsEarlierThisMonth);
					case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsEarlierThisWeek);
					case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsEarlierThisYear);
					case FunctionOperatorType.IsOutlookIntervalLastWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsLastWeek);
					case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsLaterThisMonth);
					case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsLaterThisWeek);
					case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsLaterThisYear);
					case FunctionOperatorType.IsOutlookIntervalNextWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsNextWeek);
					case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsPriorThisYear);
					case FunctionOperatorType.IsOutlookIntervalToday:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsToday);
					case FunctionOperatorType.IsOutlookIntervalTomorrow:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsTomorrow);
					case FunctionOperatorType.IsOutlookIntervalYesterday:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseIsYesterday);
					case FunctionOperatorType.IsNullOrEmpty:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringFunctionIsNullOrEmpty);
					case FunctionOperatorType.StartsWith:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringFunctionStartsWith);
					case FunctionOperatorType.EndsWith:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringFunctionEndsWith);
					case FunctionOperatorType.Contains:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringFunctionContains);
					case FunctionOperatorType.LocalDateTimeThisYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeThisYear);
					case FunctionOperatorType.LocalDateTimeThisMonth:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeThisMonth);
					case FunctionOperatorType.LocalDateTimeLastWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeLastWeek);
					case FunctionOperatorType.LocalDateTimeThisWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeThisWeek);
					case FunctionOperatorType.LocalDateTimeYesterday:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeYesterday);
					case FunctionOperatorType.LocalDateTimeToday:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeToday);
					case FunctionOperatorType.LocalDateTimeTomorrow:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeTomorrow);
					case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeDayAfterTomorrow);
					case FunctionOperatorType.LocalDateTimeNextWeek:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeNextWeek);
					case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeTwoWeeksAway);
					case FunctionOperatorType.LocalDateTimeNextMonth:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeNextMonth);
					case FunctionOperatorType.LocalDateTimeNextYear:
						return localizer.GetLocalizedString(EditorStringId.FilterClauseLocalDateTimeNextYear);
					default:
						return opType.ToString();
				}
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(BinaryOperatorType opType) {
				switch(opType) {
					case BinaryOperatorType.BitwiseAnd:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd);
					case BinaryOperatorType.BitwiseOr:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseOr);
					case BinaryOperatorType.BitwiseXor:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorBitwiseXor);
					case BinaryOperatorType.Divide:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorDivide);
					case BinaryOperatorType.Equal:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorEqual);
					case BinaryOperatorType.Greater:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorGreater);
					case BinaryOperatorType.GreaterOrEqual:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual);
					case BinaryOperatorType.Less:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorLess);
					case BinaryOperatorType.LessOrEqual:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorLessOrEqual);
#pragma warning disable 618
					case BinaryOperatorType.Like:
#pragma warning restore 618
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorLike);
					case BinaryOperatorType.Minus:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorMinus);
					case BinaryOperatorType.Modulo:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorModulo);
					case BinaryOperatorType.Multiply:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorMultiply);
					case BinaryOperatorType.NotEqual:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorNotEqual);
					case BinaryOperatorType.Plus:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringBinaryOperatorPlus);
					default:
						return opType.ToString();
				}
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(UnaryOperatorType opType) {
				switch(opType) {
					case UnaryOperatorType.BitwiseNot:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorBitwiseNot);
					case UnaryOperatorType.IsNull:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorIsNull);
					case UnaryOperatorType.Minus:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorMinus);
					case UnaryOperatorType.Not:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorNot);
					case UnaryOperatorType.Plus:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringUnaryOperatorPlus);
					default:
						return opType.ToString();
				}
			}
			string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(GroupOperatorType opType) {
				switch(opType) {
					case GroupOperatorType.And:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringGroupOperatorAnd);
					case GroupOperatorType.Or:
						return localizer.GetLocalizedString(EditorStringId.FilterCriteriaToStringGroupOperatorOr);
					default:
						return opType.ToString();
				}
			}
		}
	}
}
