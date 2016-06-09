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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region GenerateDrawingTextParagraphPropertiesContent
		#region Static Members
		internal static Dictionary<DrawingTextAlignmentType, string> DrawingTextAlignmentTypeTable = CreateDrawingTextAlignmentTypeTable();
		static Dictionary<DrawingTextAlignmentType, string> CreateDrawingTextAlignmentTypeTable() {
			Dictionary<DrawingTextAlignmentType, string> result = new Dictionary<DrawingTextAlignmentType, string>();
			result.Add(DrawingTextAlignmentType.Center, "ctr");
			result.Add(DrawingTextAlignmentType.Distributed, "dist");
			result.Add(DrawingTextAlignmentType.Justified, "just");
			result.Add(DrawingTextAlignmentType.JustifiedLow, "justLow");
			result.Add(DrawingTextAlignmentType.Left, "l");
			result.Add(DrawingTextAlignmentType.Right, "r");
			result.Add(DrawingTextAlignmentType.ThaiDistributed, "thaiDist");
			return result;
		}
		internal static Dictionary<DrawingFontAlignmentType, string> DrawingFontAlignmentTypeTable = CreateDrawingFontAlignmentTypeTable();
		static Dictionary<DrawingFontAlignmentType, string> CreateDrawingFontAlignmentTypeTable() {
			Dictionary<DrawingFontAlignmentType, string> result = new Dictionary<DrawingFontAlignmentType, string>();
			result.Add(DrawingFontAlignmentType.Automatic, "auto");
			result.Add(DrawingFontAlignmentType.Baseline, "base");
			result.Add(DrawingFontAlignmentType.Bottom, "b");
			result.Add(DrawingFontAlignmentType.Center, "ctr");
			result.Add(DrawingFontAlignmentType.Top, "t");
			return result;
		}
		#endregion
		protected internal void GenerateDrawingTextParagraphPropertiesContent(DrawingTextParagraphProperties properties) {
			GenerateDrawingTextParagraphPropertiesContentCore("pPr", properties);
		}
		void GenerateDrawingTextParagraphPropertiesContentCore(string tagName, DrawingTextParagraphProperties properties) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				IDrawingTextParagraphPropertiesOptions options = properties.Options;
				IDrawingTextMargin margin = properties.Margin;
				WriteIntValue("marL", margin.Left, options.HasLeftMargin);
				WriteIntValue("marR", margin.Right, options.HasRightMargin);
				WriteIntValue("lvl", properties.TextIndentLevel, options.HasTextIndentLevel);
				WriteIntValue("indent", properties.Indent, options.HasIndent);
				WriteEnumValue("algn", properties.TextAlignment, DrawingTextAlignmentTypeTable, options.HasTextAlignment);
				WriteIntValue("defTabSz", properties.DefaultTabSize, options.HasDefaultTabSize);
				WriteOptionalBoolValue("rtl", properties.RightToLeft, options.HasRightToLeft);
				WriteOptionalBoolValue("eaLnBrk", properties.EastAsianLineBreak, options.HasEastAsianLineBreak);
				WriteEnumValue("fontAlgn", properties.FontAlignment, DrawingFontAlignmentTypeTable, options.HasFontAlignment);
				WriteOptionalBoolValue("latinLnBrk", properties.LatinLineBreak, options.HasLatinLineBreak);
				WriteOptionalBoolValue("hangingPunct", properties.HangingPunctuation, options.HasHangingPunctuation);
				GenerateDrawingTextSpacingsContent(properties.Spacings);
				GenerateDrawingTextBulletContent(properties.Bullets);
				GenerateDrawingTextTabStopListContent(properties.TabStopList);
				if (properties.ApplyDefaultCharacterProperties)
					GenerateCharacterPropertiesContent("defRPr", properties.DefaultCharacterProperties);
			} finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
