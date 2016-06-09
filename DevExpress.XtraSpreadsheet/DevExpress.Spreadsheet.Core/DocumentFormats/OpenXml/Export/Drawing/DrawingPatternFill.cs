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
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static readonly Dictionary<DrawingPatternType, string> DrawingPatternTypeTable = CreateDrawingPatternTypeTable();
		static Dictionary<DrawingPatternType, string> CreateDrawingPatternTypeTable() {
			Dictionary<DrawingPatternType, string> result = new Dictionary<DrawingPatternType, string>();
			result.Add(DrawingPatternType.Cross, "cross");
			result.Add(DrawingPatternType.DashedDownwardDiagonal, "dashDnDiag");
			result.Add(DrawingPatternType.DashedHorizontal, "dashHorz");
			result.Add(DrawingPatternType.DashedUpwardDiagonal, "dashUpDiag");
			result.Add(DrawingPatternType.DashedVertical, "dashVert");
			result.Add(DrawingPatternType.DiagonalBrick, "diagBrick");
			result.Add(DrawingPatternType.DiagonalCross, "diagCross");
			result.Add(DrawingPatternType.Divot, "divot");
			result.Add(DrawingPatternType.DarkDownwardDiagonal, "dkDnDiag");
			result.Add(DrawingPatternType.DarkHorizontal, "dkHorz");
			result.Add(DrawingPatternType.DarkUpwardDiagonal, "dkUpDiag");
			result.Add(DrawingPatternType.DarkVertical, "dkVert");
			result.Add(DrawingPatternType.DownwardDiagonal, "dnDiag");
			result.Add(DrawingPatternType.DottedDiamond, "dotDmnd");
			result.Add(DrawingPatternType.DottedGrid, "dotGrid");
			result.Add(DrawingPatternType.Horizontal, "horz");
			result.Add(DrawingPatternType.HorizontalBrick, "horzBrick");
			result.Add(DrawingPatternType.LargeCheckerBoard, "lgCheck");
			result.Add(DrawingPatternType.LargeConfetti, "lgConfetti");
			result.Add(DrawingPatternType.LargeGrid, "lgGrid");
			result.Add(DrawingPatternType.LightDownwardDiagonal, "ltDnDiag");
			result.Add(DrawingPatternType.LightHorizontal, "ltHorz");
			result.Add(DrawingPatternType.LightUpwardDiagonal, "ltUpDiag");
			result.Add(DrawingPatternType.LightVertical, "ltVert");
			result.Add(DrawingPatternType.NarrowHorizontal, "narHorz");
			result.Add(DrawingPatternType.NarrowVertical, "narVert");
			result.Add(DrawingPatternType.OpenDiamond, "openDmnd");
			result.Add(DrawingPatternType.Percent10, "pct10");
			result.Add(DrawingPatternType.Percent20, "pct20");
			result.Add(DrawingPatternType.Percent25, "pct25");
			result.Add(DrawingPatternType.Percent30, "pct30");
			result.Add(DrawingPatternType.Percent40, "pct40");
			result.Add(DrawingPatternType.Percent5, "pct5");
			result.Add(DrawingPatternType.Percent50, "pct50");
			result.Add(DrawingPatternType.Percent60, "pct60");
			result.Add(DrawingPatternType.Percent70, "pct70");
			result.Add(DrawingPatternType.Percent75, "pct75");
			result.Add(DrawingPatternType.Percent80, "pct80");
			result.Add(DrawingPatternType.Percent90, "pct90");
			result.Add(DrawingPatternType.Plaid, "plaid");
			result.Add(DrawingPatternType.Shingle, "shingle");
			result.Add(DrawingPatternType.SmallCheckerBoard, "smCheck");
			result.Add(DrawingPatternType.SmallConfetti, "smConfetti");
			result.Add(DrawingPatternType.SmallGrid, "smGrid");
			result.Add(DrawingPatternType.SolidDiamond, "solidDmnd");
			result.Add(DrawingPatternType.Sphere, "sphere");
			result.Add(DrawingPatternType.Trellis, "trellis");
			result.Add(DrawingPatternType.UpwardDiagonal, "upDiag");
			result.Add(DrawingPatternType.Vertical, "vert");
			result.Add(DrawingPatternType.Wave, "wave");
			result.Add(DrawingPatternType.WideDownwardDiagonal, "wdDnDiag");
			result.Add(DrawingPatternType.WideUpwardDiagonal, "wdUpDiag");
			result.Add(DrawingPatternType.Weave, "weave");
			result.Add(DrawingPatternType.ZigZag, "zigZag");
			return result;
		}
		#endregion
		internal void GenerateDrawingPatternFillContent(DrawingPatternFill fill) {
			WriteStartElement("pattFill", DrawingMLNamespace);
			try {
				WriteStringValue("prst", DrawingPatternTypeTable[fill.PatternType]);
				GenerateDrawingColorTag("fgClr", fill.ForegroundColor);
				GenerateDrawingColorTag("bgClr", fill.BackgroundColor);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
