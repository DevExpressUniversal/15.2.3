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
using DevExpress.Compatibility.System;
using DevExpress.Xpo.DB;
#if !CF && !SL && !DXPORTABLE
using System.Runtime.Serialization;
#endif
namespace DevExpress.Data.Filtering.Exceptions {
	[Serializable]
	public class CriteriaParserException : Exception {
#if !CF && !SL && !DXPORTABLE
		protected CriteriaParserException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
#endif
		int line = -1, column = -1;
		public int Line { get { return line; } set { line = value; } }
		public int Column { get { return column; } set { column = value; } }
		public CriteriaParserException(string explanation) : base(explanation) { }
		public CriteriaParserException(string explanation, int line, int column) : this(explanation) {
			this.line = line;
			this.column = column;
		}
	}
	[Serializable]
	public class InvalidPropertyPathException : Exception {
#if !CF && !SL && !DXPORTABLE
		protected InvalidPropertyPathException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public InvalidPropertyPathException(string messageText)
			: base(messageText) {
		}
	}
	public sealed class FilteringExceptionsText {
		public const string LexerInvalidInputCharacter = "Invalid input character \"{0}\".";
		public const string LexerNonClosedElement = "Malformed {0}: missing closing \"{1}\".";
		public const string LexerInvalidElement = "Invalid {0} value: \"{1}\".";
		public const string LexerElementPropertyName = "property name";
		public const string LexerElementStringLiteral = "string literal";
		public const string LexerElementDateTimeLiteral = "date/time literal";
		public const string LexerElementDateTimeOrUserTypeLiteral = "date/time or user type literal";
		public const string LexerElementGuidLiteral = "guid literal";
		public const string LexerElementNumberLiteral = "numeric literal";
		public const string GrammarCatchAllErrorMessage = "Parser error at line {0}, character {1}: {2}; (\"{3}\")";
		public const string ErrorPointer = "{FAILED HERE}";
		public const string ExpressionEvaluatorOperatorSubtypeNotImplemented = "ICriteriaProcessor.ProcessOperator({0} '{1}') not implemented";
		public const string ExpressionEvaluatorAnalyzePatternInvalidPattern = "Invalid argument '{0}'";
		public const string ExpressionEvaluatorInvalidPropertyPath = "Can't find property '{0}'";
		public const string ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType = "{0} {1} not supported for type {2}";
		public const string ExpressionEvaluatorNotACollectionPath = "'{0}' doesn't implement ITypedList";
		public const string ExpressionEvaluatorJoinOperandNotSupported = "JoinOperand not supported";
	}
}
