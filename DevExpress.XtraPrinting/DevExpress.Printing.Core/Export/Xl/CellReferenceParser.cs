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

using DevExpress.Office.Utils;
using System;
namespace DevExpress.Export.Xl {
	#region XlCellReferenceParcer
	internal class XlCellReferenceParser {
		static XlCellReferenceParser instance = new XlCellReferenceParser();
		public static XlCellPosition Parse(string reference) {
			return Parse(reference, true);
		}
		public static XlCellPosition Parse(string reference, bool throwInvalidException) {
			XlCellPosition result = instance.ParseCore(reference);
			if (!result.IsValid && throwInvalidException)
				throw new ArgumentException(reference, reference);
			return result;
		}
		XlCellPosition ParseCore(string reference) {
			CellReferencePart lettersPart = new CellReferencePart(CellReferenceParserProvider.Letters, 26);
			int from = lettersPart.Parse(reference, 0);
			lettersPart.Value--;
			if (lettersPart.Value > XlCellPosition.MaxColumnCount)
				return XlCellPosition.InvalidValue;
			CellReferencePart digitsPart = new CellReferencePart(CellReferenceParserProvider.Digits, 10);
			from = digitsPart.Parse(reference, from);
			if (from < reference.Length)
				return XlCellPosition.InvalidValue;
			digitsPart.Value--;
			if (digitsPart.Value >= XlCellPosition.MaxRowCount)
				return XlCellPosition.InvalidValue;
			return new XlCellPosition(lettersPart.Value, digitsPart.Value, (XlPositionType)lettersPart.Type, (XlPositionType)digitsPart.Type);
		}
	}
	#endregion
}
