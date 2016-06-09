﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.Office.Design.Internal {
	public static class PaperKindDescriptionHelper {
		public static string CalculateDescription(PaperKind paperKind, string displayName, DocumentUnit unit, Dictionary<DocumentUnit, UnitConverter> unitConverters, DocumentModelUnitConverter documentModelUnitConverter) {
			unit = (unit == DocumentUnit.Document) ? DocumentUnit.Inch : unit;
			UnitConverter unitConverter = unitConverters[unit];
			Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(paperKind);
			UIUnit width = new UIUnit(unitConverter.FromUnits(documentModelUnitConverter.TwipsToModelUnits(paperSizeInTwips.Width)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			UIUnit height = new UIUnit(unitConverter.FromUnits(documentModelUnitConverter.TwipsToModelUnits(paperSizeInTwips.Height)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			return String.Format("{0}\r\n{1} x {2}", displayName, width.ToString(), height.ToString());
		}
	}
}
