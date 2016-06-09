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
using DevExpress.XtraSpreadsheet.Drawing;
using System.Globalization;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region UnderlineTypeTable
		internal static Dictionary<DrawingTextUnderlineType, string> UnderlineTypeTable = CreateTextUnderlineTypeTable();
		static Dictionary<DrawingTextUnderlineType, string> CreateTextUnderlineTypeTable() {
			Dictionary<DrawingTextUnderlineType, string> result = new Dictionary<DrawingTextUnderlineType, string>();
			result.Add(DrawingTextUnderlineType.None, "none");
			result.Add(DrawingTextUnderlineType.Words, "words");
			result.Add(DrawingTextUnderlineType.Single, "sng");
			result.Add(DrawingTextUnderlineType.Double, "dbl");
			result.Add(DrawingTextUnderlineType.Heavy, "heavy");
			result.Add(DrawingTextUnderlineType.Dotted, "dotted");
			result.Add(DrawingTextUnderlineType.HeavyDotted, "dottedHeavy");
			result.Add(DrawingTextUnderlineType.Dashed, "dash");
			result.Add(DrawingTextUnderlineType.HeavyDashed, "dashHeavy");
			result.Add(DrawingTextUnderlineType.LongDashed, "dashLong");
			result.Add(DrawingTextUnderlineType.HeavyLongDashed, "dashLongHeavy");
			result.Add(DrawingTextUnderlineType.DotDash, "dotDash");
			result.Add(DrawingTextUnderlineType.HeavyDotDash, "dotDashHeavy");
			result.Add(DrawingTextUnderlineType.DotDotDash, "dotDotDash");
			result.Add(DrawingTextUnderlineType.HeavyDotDotDash, "dotDotDashHeavy");
			result.Add(DrawingTextUnderlineType.Wavy, "wavy");
			result.Add(DrawingTextUnderlineType.HeavyWavy, "wavyHeavy");
			result.Add(DrawingTextUnderlineType.DoubleWavy, "wavyDbl");
			return result;
		}
		#endregion
		#region StrikeTypeTable
		internal static Dictionary<DrawingTextStrikeType, string> StrikeTypeTable = CreateStrikeTypeTable();
		static Dictionary<DrawingTextStrikeType, string> CreateStrikeTypeTable() {
			Dictionary<DrawingTextStrikeType, string> result = new Dictionary<DrawingTextStrikeType, string>();
			result.Add(DrawingTextStrikeType.None, "noStrike");
			result.Add(DrawingTextStrikeType.Single, "sngStrike");
			result.Add(DrawingTextStrikeType.Double, "dblStrike");
			return result;
		}
		#endregion
		#region CapsTypeTable
		internal static Dictionary<DrawingTextCapsType, string> CapsTypeTable = CreateCapsTypeTable();
		static Dictionary<DrawingTextCapsType, string> CreateCapsTypeTable() {
			Dictionary<DrawingTextCapsType, string> result = new Dictionary<DrawingTextCapsType, string>();
			result.Add(DrawingTextCapsType.None, "none");
			result.Add(DrawingTextCapsType.All, "all");
			result.Add(DrawingTextCapsType.Small, "small");
			return result;
		}
		#endregion
		protected internal void GenerateCharacterPropertiesContent(string tagName, DrawingTextCharacterProperties properties) {
			WriteStartElement(tagName, DrawingMLNamespace);
			DrawingTextCharacterInfo defaultInfo = DrawingTextCharacterInfo.DefaultInfo;
			try {
				ITextCharacterOptions options = properties.Options;
				WriteOptionalBoolValue("kumimoji", properties.Kumimoji, options.HasKumimoji);
				WriteStringValue("lang", properties.Language.Name, !String.IsNullOrEmpty(properties.Language.Name));
				WriteStringValue("altLang", properties.AlternateLanguage.Name, !String.IsNullOrEmpty(properties.AlternateLanguage.Name));
				WriteIntValue("sz", properties.FontSize, options.HasFontSize);
				WriteOptionalBoolValue("b", properties.Bold, options.HasBold);
				WriteOptionalBoolValue("i", properties.Italic, options.HasItalic);
				WriteEnumValue("u", properties.Underline, UnderlineTypeTable, options.HasUnderline);
				WriteEnumValue("strike", properties.Strikethrough, StrikeTypeTable, options.HasStrikethrough);
				WriteIntValue("kern", properties.Kerning, options.HasKerning);
				WriteEnumValue("cap", properties.Caps, CapsTypeTable, options.HasCaps);
				WriteIntValue("spc", properties.Spacing, options.HasSpacing);
				WriteOptionalBoolValue("normalizeH", properties.NormalizeHeight, options.HasNormalizeHeight);
				WriteIntValue("baseline", properties.Baseline, options.HasBaseline);
				WriteOptionalBoolValue("noProof", properties.NoProofing, options.HasNoProofing);
				WriteBoolValue("dirty", properties.Dirty, defaultInfo.Dirty);
				WriteBoolValue("err", properties.SpellingError, defaultInfo.SpellingError);
				WriteBoolValue("smtClean", properties.SmartTagClean, defaultInfo.SmartTagClean);
				WriteIntValue("smtId", properties.SmartTagId, defaultInfo.SmartTagId);
				WriteStringValue("bmk", properties.Bookmark, !String.IsNullOrEmpty(properties.Bookmark));
				GenerateOutlineContent(properties.Outline, "ln");
				GenerateDrawingFillContent(properties.Fill);
				ContainerEffect containerEffect = properties.Effects;
				if (containerEffect.Effects.Count != 0)
					GenerateContainerEffectContent(containerEffect);
				GenerateHighlightContent(properties.Highlight);
				GenerateStrokeUnderlineContent(properties.StrokeUnderline);
				GenerateUnderlineFillContent(properties.UnderlineFill);
				if (!properties.Latin.IsDefault)
					GenerateDrawingTextFontContent(properties.Latin, "latin");
				if (!properties.EastAsian.IsDefault)
					GenerateDrawingTextFontContent(properties.EastAsian, "ea");
				if (!properties.ComplexScript.IsDefault)
					GenerateDrawingTextFontContent(properties.ComplexScript, "cs");
				if (!properties.Symbol.IsDefault)
					GenerateDrawingTextFontContent(properties.Symbol, "sym");
			} finally {
				WriteEndElement();
			}
		}
		void GenerateHighlightContent(DrawingColor color) {
			if (color.IsEmpty)
				return;
			WriteStartElement("highlight", DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(color);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateStrokeUnderlineContent(IStrokeUnderline strokeUnderline) {
			if (strokeUnderline == DrawingStrokeUnderline.Automatic)
				return;
			if (strokeUnderline == DrawingStrokeUnderline.FollowsText)
				GenerateFollowsTextContent("uLnTx");
			else
				GenerateOutlineContent(strokeUnderline as Outline, "uLn");
		}
		void GenerateUnderlineFillContent(IUnderlineFill underlineFill) {
			if (underlineFill == DrawingUnderlineFill.FollowsText)
				GenerateFollowsTextContent("uFillTx");
			else
				GenerateUnderlineFillCore(underlineFill as IDrawingFill);
		}
		void GenerateUnderlineFillCore(IDrawingFill fill) {
			if (fill.FillType == DrawingFillType.Automatic)
				return;
			WriteStartElement("uFill", DrawingMLNamespace);
			try {
				GenerateDrawingFillContent(fill);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateFollowsTextContent(string tagName) {
			WriteStartElement(tagName, DrawingMLNamespace);
			WriteEndElement();
		}
	}
}
