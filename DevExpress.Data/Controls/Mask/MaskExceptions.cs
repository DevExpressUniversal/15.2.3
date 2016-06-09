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
using System.Collections;
using System.Globalization;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
namespace DevExpress.Data.Mask {
	public class MaskExceptionsTexts {
		public const string IncorrectMaskNonclosedQuota = "Incorrect mask: closing quote expected";
		public const string IncorrectMaskBackslashBeforeEndOfMask = "Incorrect mask: character expected after '\\'";
		public const string IncorrectMaskUnknownNamedMask = "IncorrectMask: unknown named mask '{0}'";
		public const string IncorrectNumericMaskSignedMaskNotMatchMaxDigitsBeforeDecimalSeparator = "Incorrect mask: the max number of digits before the decimal separator in the positive and negative patterns must match";
		public const string IncorrectNumericMaskSignedMaskNotMatchMaxDigitsAfterDecimalSeparator = "Incorrect mask: the max number of digits after the decimal separator in the positive and negative patterns must match";
		public const string IncorrectNumericMaskSignedMaskNotMatchMinDigitsBeforeDecimalSeparator = "Incorrect mask: the min number of digits before the decimal separator in the positive and negative patterns must match";
		public const string IncorrectNumericMaskSignedMaskNotMatchMinDigitsAfterDecimalSeparator = "Incorrect mask: the min number of digits after the decimal separator in the positive and negative patterns must match";
		public const string IncorrectNumericMaskSignedMaskNotMatchIs100Multiplied = "Incorrect mask: the percent type (% or %%) in the positive and negative patterns must match";
		public const string IncorrectMaskInvalidUnicodeCategory = "Incorrect mask: unsupported unicode category '{0}'";
		public const string IncorrectMaskCurveBracketAfterPpExpected = "Incorrect mask: '{' expected after '\\p' or '\\P'";
		public const string IncorrectMaskClosingBracketAfterPpExpected = "Incorrect mask: '}' expected after '\\p{unicode_category_name' or '\\P{unicode_category_name'";
		public const string IncorrectMaskBackslashRBeforeEndOfMask = "Incorrect mask: character expected after '\\R'";
		public const string IncorrectMaskInvalidCharAfterBackslashR = "Incorrect mask: only '.', ':', '/' and '{' are allowed after '\\R'";
		public const string IncorrectMaskClosingBracketAfterRExpected = "Incorrect mask: '}' expected after '\\R{pattern_name'";
		public const string IncorrectMaskClosingSquareBracketExpected = "Incorrect mask: ']' expected after '['";
		public const string IncorrectMaskClosingCurveBracketExpected = "Incorrect mask: '}' expected after '{'";
		public const string IncorrectMaskInvalidQuantifierFormat = "Incorrect mask: invalid quantifier format";
		public const string CreateManagerReturnsNull = "The manager cannot be created for the mask {1} of {0} type. Specify another type of mask or implement the TextEdit.CreateManager method.";
		public const string InternalErrorNonCoveredCase = "Internal error: non-covered case '{0}'";
		public const string InternalErrorGetSampleCharForEmpty = "Internal error: GetSampleChar() called for empty transition";
		public const string InternalErrorNonSpecific = "Internal error";
	}
}
