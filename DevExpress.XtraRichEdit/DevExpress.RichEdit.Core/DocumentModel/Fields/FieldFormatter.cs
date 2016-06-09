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
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Model {
	public abstract class FieldFormatter {
		protected internal virtual CultureInfo Culture { get { return CultureInfo.CurrentCulture; } }
		internal static void ThrowIncorrectNumericFieldFormatError() {
			ThrowException(XtraRichEditStringId.Msg_IncorrectNumericFieldFormat);
		}
		internal static void ThrowSyntaxError(string incorrectFormat) {
			ThrowException(XtraRichEditStringId.Msg_SyntaxErrorInFieldPattern, incorrectFormat);
		}
		internal static void ThrowSyntaxError(char incorrectChar) {
			ThrowSyntaxError(incorrectChar.ToString());
		}
		internal static void ThrowUnmatchedQuotesError() {
			ThrowException(XtraRichEditStringId.Msg_UnmatchedQuotesInFieldPattern);
		}
		internal static void ThrowUnknownSwitchArgumentError() {
			ThrowException(XtraRichEditStringId.Msg_UnknownSwitchArgument);
		}
		internal static void ThrowUnexpectedEndOfFormulaError() {
			ThrowException(XtraRichEditStringId.Msg_UnexpectedEndOfFormula);
		}
		internal static void ThrowMissingOperatorError() {
			ThrowException(XtraRichEditStringId.Msg_MissingOperator);
		}
		internal static void ThrowZeroDivideError() {
			ThrowException(XtraRichEditStringId.Msg_ZeroDivide);
		}
		static void ThrowException(XtraRichEditStringId messageId) {
			string message = XtraRichEditLocalizer.GetString(messageId);
			throw new ArgumentException(message);
		}
		static void ThrowException(XtraRichEditStringId messageId, string argument) {
			string message = XtraRichEditLocalizer.GetString(messageId);
			throw new ArgumentException(String.Format(message, argument));
		}
	}
	public abstract class SpecificFieldFormatter<T> : FieldFormatter {
		public string Format(object value, string format, bool hasFormat) {
			if (!(value is T))
				Exceptions.ThrowArgumentException("value", value);
			if (!hasFormat)
				return FormatByDefault((T)value);
			if (String.IsNullOrEmpty(format))
				return String.Empty;
			return Format((T)value, format);
		}
		protected abstract string FormatByDefault(T value);
		protected abstract string Format(T value, string format);
	}
}
