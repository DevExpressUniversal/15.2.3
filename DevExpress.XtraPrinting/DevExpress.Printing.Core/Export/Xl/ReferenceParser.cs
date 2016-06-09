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
namespace DevExpress.Export.Xl {
	#region XlRangeReferenceParser
	internal class XlRangeReferenceParser {
		static XlRangeReferenceParser instance = new XlRangeReferenceParser();
		internal static XlCellRange Parse(string reference) {
			return Parse(reference, true);
		}
		internal static XlCellRange Parse(string reference, bool throwInvalidException) {
			return instance.ParseCore(reference, throwInvalidException);
		}
		XlCellRange ParseCore(string reference, bool throwInvalidException) {
			int splitIndex = reference.LastIndexOf('!');
			string sheetName = splitIndex == -1 ? String.Empty : reference.Substring(0, splitIndex);
			if (sheetName.StartsWith("'")) {
				sheetName = sheetName.Remove(0, 1);
				sheetName = sheetName.Remove(sheetName.Length - 1, 1);
				sheetName = sheetName.Replace("''", "'");
			}
			string referencePart = reference.Substring(splitIndex + 1);
			string[] positions = referencePart.Split(':');
			XlCellPosition topLeft = XlCellReferenceParser.Parse(positions[0], throwInvalidException);
			if (!topLeft.IsValid)
				return null;
			XlCellPosition bottomRight = positions.Length == 1 ? topLeft : XlCellReferenceParser.Parse(positions[1], throwInvalidException);
			if (!bottomRight.IsValid)
				return null;
			return new XlCellRange(sheetName, topLeft, bottomRight);
		}
	}
	#endregion
}
