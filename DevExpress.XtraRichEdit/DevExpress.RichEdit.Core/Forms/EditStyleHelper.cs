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
using System.Linq;
using System.Text;
using DevExpress.Office.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Reflection;
#if SL
using System.Windows.Media;
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public static class EditStyleHelper {
		#region IsValidFontSize
		public static bool IsValidFontSize(int fontSize) {
			return fontSize >= PredefinedFontSizeCollection.MinFontSize && fontSize <= PredefinedFontSizeCollection.MaxFontSize * 2;
		}
		#endregion
		#region IsFontSizeValid
		public static bool IsFontSizeValid(object edtValue, out string text, out int value) {
			text = String.Empty;
			bool isIntValue = OfficeFontSizeEditHelper.TryGetHalfSizeValue(edtValue, out value);
			if (isIntValue) {
				if (!IsValidFontSize(value)) {
					text = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidFontSize), PredefinedFontSizeCollection.MinFontSize, PredefinedFontSizeCollection.MaxFontSize);
					return false;
				}
			}
			else {
				text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidNumber);
				return false;
			}
			return true;
		}
		public static bool IsFontSizeValid(object edtValue, out string text) {
			int value;
			return IsFontSizeValid(edtValue, out text, out value);
		}
		#endregion
		#region IsValidStyleName
		public static bool IsValidStyleName<T>(string name, T sourceStyle, StyleCollectionBase<T> collection) where T : StyleBase<T> {
			if (sourceStyle.StyleName != name) {
			foreach (T style in collection) {
				if (style.StyleName == name)
					return false;
				}
			}
			return true;
		}
		#endregion
		#region LoadSmallImageToGlyph
		public static Image LoadSmallImageToGlyph(string name) {
			return CommandResourceImageLoader.LoadSmallImage("DevExpress.XtraRichEdit.Images", name, typeof(DevExpress.XtraRichEdit.Commands.RichEditCommand).GetAssembly());
		}
		#endregion
		#region ChangeConditionalType
		public static void ChangeConditionalType(Table table, ConditionalTableStyleFormattingTypes styleType) {
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.FirstRow))
				table.TableLook |= TableLookTypes.ApplyFirstRow;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.LastRow))
				table.TableLook |= TableLookTypes.ApplyLastRow;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.EvenRowBanding) || styleType.HasFlag(ConditionalTableStyleFormattingTypes.OddRowBanding))
				table.TableLook &= ~TableLookTypes.DoNotApplyRowBanding;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.FirstColumn))
				table.TableLook |= TableLookTypes.ApplyFirstColumn;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.LastColumn))
				table.TableLook |= TableLookTypes.ApplyLastColumn;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.TopLeftCell))
				table.TableLook |= TableLookTypes.ApplyFirstColumn | TableLookTypes.ApplyFirstRow;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.TopRightCell))
				table.TableLook |= TableLookTypes.ApplyLastColumn | TableLookTypes.ApplyFirstRow;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.BottomLeftCell))
				table.TableLook |= TableLookTypes.ApplyFirstColumn | TableLookTypes.ApplyLastRow;
			if (styleType.HasFlag(ConditionalTableStyleFormattingTypes.BottomRightCell))
				table.TableLook |= TableLookTypes.ApplyLastColumn | TableLookTypes.ApplyLastRow;
		}
#endregion
	}
#region CharacterPropertyAccessorBase
	public abstract class CharacterPropertyAccessorBase<T> {
		public T GetValue(CharacterProperties properties) {
			return GetValue(properties.Info.Info);
		}
		public abstract T GetValue(CharacterFormattingInfo properies);
		public abstract void SetValue(CharacterProperties properties, T value);
		public abstract void Reset(CharacterProperties properties);
		public abstract bool GetUseValue(CharacterProperties properties);
	}
#endregion
#region FontBoldAccessor
	internal class FontBoldAccessor : CharacterPropertyAccessorBase<bool> {
		public override bool GetValue(CharacterFormattingInfo properties) {
			return properties.FontBold;
		}
		public override void SetValue(CharacterProperties properties, bool value) {
			properties.FontBold = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.ResetUse(CharacterFormattingOptions.Mask.UseFontBold);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseFontBold;
		}
	}
#endregion
#region FontItalicAccessor
	internal class FontItalicAccessor : CharacterPropertyAccessorBase<bool> {
		public override bool GetValue(CharacterFormattingInfo properties) {
			return properties.FontItalic;
		}
		public override void SetValue(CharacterProperties properties, bool value) {
			properties.FontItalic = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.ResetUse(CharacterFormattingOptions.Mask.UseFontItalic);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseFontBold;
		}
	}
#endregion
#region FontNameAccessor
	internal class FontNameAccessor : CharacterPropertyAccessorBase<string> {
		public override string GetValue(CharacterFormattingInfo properties) {
			return properties.FontName;
		}
		public override void SetValue(CharacterProperties properties, string value) {
			properties.FontName = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.ResetUse(CharacterFormattingOptions.Mask.UseFontName);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseFontName;
		}
	}
#endregion
#region ForeColorAccessor
	internal class ForeColorAccessor : CharacterPropertyAccessorBase<Color> {
		public override Color GetValue(CharacterFormattingInfo properties) {
			return properties.ForeColor;
		}
		public override void SetValue(CharacterProperties properties, Color value) {
			properties.ForeColor = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.ResetUse(CharacterFormattingOptions.Mask.UseForeColor);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseForeColor;
		}
	}
#endregion
#region FontSizeAccessor
	internal class DoubleFontSizeAccessor : CharacterPropertyAccessorBase<int> {
		public override int GetValue(CharacterFormattingInfo properties) {
			return properties.DoubleFontSize;
		}
		public override void SetValue(CharacterProperties properties, int value) {
			properties.DoubleFontSize = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.ResetUse(CharacterFormattingOptions.Mask.UseDoubleFontSize);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseDoubleFontSize;
		}
	}
#endregion
#region FontUnderlineTypeAccessor
	internal class FontUnderlineTypeAccessor : CharacterPropertyAccessorBase<UnderlineType> {
		public override UnderlineType GetValue(CharacterFormattingInfo properties) {
			return properties.FontUnderlineType;
		}
		public override void SetValue(CharacterProperties properties, UnderlineType value) {
			properties.FontUnderlineType = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.FontUnderlineType = properties.DocumentModel.DefaultCharacterProperties.FontUnderlineType;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseFontUnderlineType);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseFontUnderlineType;
		}
	}
#endregion
#region UnderlineColorAccessor
	internal class UnderlineColorAccessor : CharacterPropertyAccessorBase<Color> {
		public override Color GetValue(CharacterFormattingInfo properties) {
			return properties.UnderlineColor;
		}
		public override void SetValue(CharacterProperties properties, Color value) {
			properties.UnderlineColor = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.UnderlineColor = properties.DocumentModel.DefaultCharacterProperties.UnderlineColor;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseUnderlineColor);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseUnderlineColor;
		}
	}
#endregion
#region FontStrikeoutTypeAccessor
	internal class FontStrikeoutTypeAccessor : CharacterPropertyAccessorBase<StrikeoutType> {
		public override StrikeoutType GetValue(CharacterFormattingInfo properties) {
			return properties.FontStrikeoutType;
		}
		public override void SetValue(CharacterProperties properties, StrikeoutType value) {
			properties.FontStrikeoutType = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.FontStrikeoutType = properties.DocumentModel.DefaultCharacterProperties.FontStrikeoutType;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseFontStrikeoutType);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseFontStrikeoutType;
		}
	}
#endregion
#region UnderlineWordsOnlyAccessor
	internal class UnderlineWordsOnlyAccessor : CharacterPropertyAccessorBase<bool> {
		public override bool GetValue(CharacterFormattingInfo properties) {
			return properties.UnderlineWordsOnly;
		}
		public override void SetValue(CharacterProperties properties, bool value) {
			properties.UnderlineWordsOnly = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.UnderlineWordsOnly = properties.DocumentModel.DefaultCharacterProperties.UnderlineWordsOnly;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseUnderlineWordsOnly);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseUnderlineWordsOnly;
		}
	}
#endregion
#region ScriptAccessor
	internal class ScriptAccessor : CharacterPropertyAccessorBase<CharacterFormattingScript> {
		public override CharacterFormattingScript GetValue(CharacterFormattingInfo properties) {
			return properties.Script;
		}
		public override void SetValue(CharacterProperties properties, CharacterFormattingScript value) {
			properties.Script = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.Script = properties.DocumentModel.DefaultCharacterProperties.Script;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseScript);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseScript;
		}
	}
#endregion
#region AllCapsAccessor
	internal class AllCapsAccessor : CharacterPropertyAccessorBase<bool> {
		public override bool GetValue(CharacterFormattingInfo properties) {
			return properties.AllCaps;
		}
		public override void SetValue(CharacterProperties properties, bool value) {
			properties.AllCaps = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.AllCaps = properties.DocumentModel.DefaultCharacterProperties.AllCaps;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseAllCaps);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseAllCaps;
		}
	}
#endregion
#region HiddenAccessor
	internal class HiddenAccessor : CharacterPropertyAccessorBase<bool> {
		public override bool GetValue(CharacterFormattingInfo properties) {
			return properties.Hidden;
		}
		public override void SetValue(CharacterProperties properties, bool value) {
			properties.Hidden = value;
		}
		public override void Reset(CharacterProperties properties) {
			properties.Hidden = properties.DocumentModel.DefaultCharacterProperties.Hidden;
			properties.ResetUse(CharacterFormattingOptions.Mask.UseHidden);
		}
		public override bool GetUseValue(CharacterProperties properties) {
			return properties.UseHidden;
		}
	}
#endregion
#region ParagraphPropertyAccessorBase
	public abstract class ParagraphPropertyAccessorBase<T> {
		public T GetValue(ParagraphProperties properties) {
			return GetValue(properties.Info.Info);
		}
		public abstract T GetValue(ParagraphFormattingInfo properies);
		public abstract void SetValue(ParagraphProperties properties, T value);
		public abstract void Reset(ParagraphProperties properties);
		public abstract bool GetUseValue(ParagraphProperties properties);
		public abstract bool GetOptionsUseValue(ParagraphFormattingOptions properties);
	}
#endregion
#region AlignmentAccessor
	internal class AlignmentAccessor : ParagraphPropertyAccessorBase<ParagraphAlignment> {
		public override ParagraphAlignment GetValue(ParagraphFormattingInfo properties) {
			return properties.Alignment;
		}
		public override void SetValue(ParagraphProperties properties, ParagraphAlignment value) {
			properties.Alignment = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.Alignment = properties.DocumentModel.DefaultParagraphProperties.Alignment;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseAlignment);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseAlignment;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseAlignment;
		}
	}
#endregion
#region OutlineLevelAccessor
	internal class OutlineLevelAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.OutlineLevel;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.OutlineLevel = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.OutlineLevel = properties.DocumentModel.DefaultParagraphProperties.OutlineLevel;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseOutlineLevel);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseOutlineLevel;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseOutlineLevel;
		}
	}
#endregion
#region LeftIndentAccessor
	internal class LeftIndentAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.LeftIndent;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.LeftIndent = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.LeftIndent = properties.DocumentModel.DefaultParagraphProperties.LeftIndent;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseLeftIndent);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseLeftIndent;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseLeftIndent;
		}
	}
#endregion
#region RightIndentAccessor
	internal class RightIndentAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.RightIndent;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.RightIndent = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.RightIndent = properties.DocumentModel.DefaultParagraphProperties.RightIndent;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseRightIndent);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseRightIndent;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseRightIndent;
		}
	}
#endregion
#region FirstLineIndentAccessor
	internal class FirstLineIndentAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.FirstLineIndent;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.FirstLineIndent = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.FirstLineIndent = properties.DocumentModel.DefaultParagraphProperties.FirstLineIndent;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseFirstLineIndent;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseFirstLineIndent;
		}
	}
#endregion
#region SpacingBeforeAccessor
	internal class SpacingBeforeAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.SpacingBefore;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.SpacingBefore = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.SpacingBefore = properties.DocumentModel.DefaultParagraphProperties.SpacingBefore;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseSpacingBefore);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseSpacingBefore;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseSpacingBefore;
		}
	}
#endregion
#region SpacingAfterAccessor
	internal class SpacingAfterAccessor : ParagraphPropertyAccessorBase<int> {
		public override int GetValue(ParagraphFormattingInfo properties) {
			return properties.SpacingAfter;
		}
		public override void SetValue(ParagraphProperties properties, int value) {
			properties.SpacingAfter = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.SpacingAfter = properties.DocumentModel.DefaultParagraphProperties.SpacingAfter;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseSpacingAfter);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseSpacingAfter;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseSpacingAfter;
		}
	}
#endregion
#region LineSpacingAccessor
	internal class LineSpacingAccessor : ParagraphPropertyAccessorBase<float> {
		public override float GetValue(ParagraphFormattingInfo properties) {
			return properties.LineSpacing;
		}
		public override void SetValue(ParagraphProperties properties, float value) {
			properties.LineSpacing = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.LineSpacing = properties.DocumentModel.DefaultParagraphProperties.LineSpacing;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseLineSpacing);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseLineSpacing;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseLineSpacing;
		}
	}
#endregion
#region BeforeAutoSpacingAccessor
	internal class BeforeAutoSpacingAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.BeforeAutoSpacing;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.BeforeAutoSpacing = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.BeforeAutoSpacing = properties.DocumentModel.DefaultParagraphProperties.BeforeAutoSpacing;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseBeforeAutoSpacing);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseBeforeAutoSpacing;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseBeforeAutoSpacing;
		}
	}
#endregion
#region AfterAutoSpacingAccessor
	internal class AfterAutoSpacingAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.AfterAutoSpacing;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.AfterAutoSpacing = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.AfterAutoSpacing = properties.DocumentModel.DefaultParagraphProperties.AfterAutoSpacing;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseAfterAutoSpacing);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseAfterAutoSpacing;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseAfterAutoSpacing;
		}
	}
#endregion
#region ContextualSpacingAccessor
	internal class ContextualSpacingAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.ContextualSpacing;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.ContextualSpacing = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.ContextualSpacing = properties.DocumentModel.DefaultParagraphProperties.ContextualSpacing;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseContextualSpacing);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseContextualSpacing;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseContextualSpacing;
		}
	}
#endregion
#region KeepLinesTogetherAccessor
	internal class KeepLinesTogetherAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.KeepLinesTogether;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.KeepLinesTogether = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.KeepLinesTogether = properties.DocumentModel.DefaultParagraphProperties.KeepLinesTogether;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseKeepLinesTogether);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseKeepLinesTogether;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseKeepLinesTogether;
		}
	}
#endregion
#region KeepWithNextAccessor
	internal class KeepWithNextAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.KeepWithNext;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.KeepWithNext = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.KeepWithNext = properties.DocumentModel.DefaultParagraphProperties.KeepWithNext;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseKeepWithNext);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseKeepWithNext;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseKeepWithNext;
		}
	}
#endregion
#region PageBreakBeforeAccessor
	internal class PageBreakBeforeAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.PageBreakBefore;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.PageBreakBefore = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.PageBreakBefore = properties.DocumentModel.DefaultParagraphProperties.PageBreakBefore;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UsePageBreakBefore);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UsePageBreakBefore;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UsePageBreakBefore;
		}
	}
#endregion
#region SuppressLineNumbersAccessor
	internal class SuppressLineNumbersAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.SuppressLineNumbers;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.SuppressLineNumbers = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.SuppressLineNumbers = properties.DocumentModel.DefaultParagraphProperties.SuppressLineNumbers;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseSuppressLineNumbers);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseSuppressLineNumbers;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseSuppressLineNumbers;
		}
	}
#endregion
#region SuppressHyphenationAccessor
	internal class SuppressHyphenationAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.SuppressHyphenation;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.SuppressHyphenation = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.SuppressHyphenation = properties.DocumentModel.DefaultParagraphProperties.SuppressHyphenation;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseSuppressHyphenation);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseSuppressHyphenation;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseSuppressHyphenation;
		}
	}
#endregion
#region WidowOrphanControlAccessor
	internal class WidowOrphanControlAccessor : ParagraphPropertyAccessorBase<bool> {
		public override bool GetValue(ParagraphFormattingInfo properties) {
			return properties.WidowOrphanControl;
		}
		public override void SetValue(ParagraphProperties properties, bool value) {
			properties.WidowOrphanControl = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.WidowOrphanControl = properties.DocumentModel.DefaultParagraphProperties.WidowOrphanControl;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseWidowOrphanControl);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseWidowOrphanControl;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseWidowOrphanControl;
		}
	}
#endregion
#region BackColorAccessor
	internal class BackColorAccessor : ParagraphPropertyAccessorBase<Color> {
		public override Color GetValue(ParagraphFormattingInfo properties) {
			return properties.BackColor;
		}
		public override void SetValue(ParagraphProperties properties, Color value) {
			properties.BackColor = value;
		}
		public override void Reset(ParagraphProperties properties) {
			properties.BackColor = properties.DocumentModel.DefaultParagraphProperties.BackColor;
			properties.ResetUse(ParagraphFormattingOptions.Mask.UseBackColor);
		}
		public override bool GetUseValue(ParagraphProperties properties) {
			return properties.UseBackColor;
		}
		public override bool GetOptionsUseValue(ParagraphFormattingOptions properties) {
			return properties.UseBackColor;
		}
	}
#endregion
#region TableCellPropertyAccessorBase
	public abstract class TableCellPropertyAccessorBase<T> {
		public abstract T GetValue(TableCellGeneralSettingsInfo properies);
		public abstract void SetValue(TableCellProperties properties, T value);
		public abstract void Reset(TableCellProperties properties);
		public abstract bool GetUseValue(TableCellProperties properties);
		public abstract bool GetOptionsUseValue(TableCellPropertiesOptions properties);
	}
#endregion
#region BackgroundColorAccessor
	internal class BackgroundColorAccessor : TableCellPropertyAccessorBase<Color> {
		public override Color GetValue(TableCellGeneralSettingsInfo properties) {
			return properties.BackgroundColor;
		}
		public override void SetValue(TableCellProperties properties, Color value) {
			properties.BackgroundColor = value;
			if (value == DXColor.Empty)
				Reset(properties);
		}
		public override void Reset(TableCellProperties properties) {
			properties.ResetUse(TableCellPropertiesOptions.Mask.UseBackgroundColor);
		}
		public override bool GetUseValue(TableCellProperties properties) {
			return properties.UseBackgroundColor;
		}
		public override bool GetOptionsUseValue(TableCellPropertiesOptions properties) {
			return properties.UseBackgroundColor;
		}
	}
#endregion
#region VerticalAlignmentAccessor
	internal class VerticalAlignmentAccessor : TableCellPropertyAccessorBase<VerticalAlignment> {
		public override VerticalAlignment GetValue(TableCellGeneralSettingsInfo properties) {
			return properties.VerticalAlignment;
		}
		public override void SetValue(TableCellProperties properties, VerticalAlignment value) {
			properties.VerticalAlignment = value;
		}
		public override void Reset(TableCellProperties properties) {
			properties.VerticalAlignment = properties.DocumentModel.DefaultTableCellProperties.VerticalAlignment;
			properties.ResetUse(TableCellPropertiesOptions.Mask.UseVerticalAlignment);
		}
		public override bool GetUseValue(TableCellProperties properties) {
			return properties.UseVerticalAlignment;
		}
		public override bool GetOptionsUseValue(TableCellPropertiesOptions properties) {
			return properties.UseVerticalAlignment;
		}
	}
#endregion
}
