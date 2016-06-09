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
using System.Text;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
#if !DXPORTABLE
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Design;
#else
using System.Windows.Media;
#endif
#endif
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Forms {
#region FontFormControllerParameters
	public class FontFormControllerParameters : FormControllerParameters {
		readonly MergedCharacterProperties sourceCharacterProperties;
		internal FontFormControllerParameters(IRichEditControl control, MergedCharacterProperties sourceCharacterProperties)
			: base(control) {
			Guard.ArgumentNotNull(sourceCharacterProperties, "sourceCharacterProperties");
			this.sourceCharacterProperties = sourceCharacterProperties;
		}
		internal MergedCharacterProperties SourceCharacterProperties { get { return sourceCharacterProperties; } }
	}
#endregion
#region FontFormController
	public class FontFormController : FormController {
		readonly MergedCharacterProperties sourceCharacterProperties;
		bool? allCaps;
		CharacterFormattingScript? script;
		StrikeoutType? fontStrikeoutType;
		string fontName;
		bool? fontBold;
		bool? fontItalic;
		int? doubleFontSize;
		Color? foreColor;
		Color? underlineColor;
		UnderlineType? fontUnderlineType;
		UnderlineType? prevFontUnderlineType;
		bool? underlineWordsOnly;
		bool? hidden;
		public FontFormController(FontFormControllerParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.sourceCharacterProperties = parameters.SourceCharacterProperties;
			InitializeController();
		}
		protected internal MergedCharacterProperties SourceCharacterProperties { get { return sourceCharacterProperties; } }
		protected CharacterFormattingOptions SourceOptions { get { return SourceCharacterProperties.Options; } }
		protected CharacterFormattingInfo SourceInfo { get { return SourceCharacterProperties.Info; } }
		public bool? AllCaps { get { return allCaps; } set { allCaps = value; } }
		public CharacterFormattingScript? Script { get { return script; } set { script = value; } }
		public StrikeoutType? FontStrikeoutType { get { return fontStrikeoutType; } set { fontStrikeoutType = value; } }
		public string FontName { get { return fontName; } set { fontName = value; } }
		public bool? FontBold { get { return fontBold; } set { fontBold = value; } }
		public bool? FontItalic { get { return fontItalic; } set { fontItalic = value; } }
		public int? DoubleFontSize { get { return doubleFontSize; } set { doubleFontSize = value; } }
		public Color? ForeColor { get { return foreColor; } set { foreColor = value; } }
		public Color? UnderlineColor { get { return underlineColor; } set { underlineColor = value; } }
		public UnderlineType? FontUnderlineType {
			get { return fontUnderlineType; }
			set {				
				fontUnderlineType = value;
				if(UnderlineWordsOnly == true && fontUnderlineType != UnderlineType.Single) {
					underlineWordsOnly = false;
					prevFontUnderlineType = fontUnderlineType;
				}
			}
		}
		public bool? UnderlineWordsOnly {
			get { return underlineWordsOnly; }
			set {				
				underlineWordsOnly = value;
				if(underlineWordsOnly == true) {
					prevFontUnderlineType = initUnderlineWordsOnly ? UnderlineType.None : fontUnderlineType;
					fontUnderlineType = UnderlineType.Single;
				}
				else if(prevFontUnderlineType != null)
					fontUnderlineType = prevFontUnderlineType;
			}
		}
		bool initUnderlineWordsOnly = true;
		public void SetFontUnderline(UnderlineType? underlineType, bool? underlineWordsOnly) {
			if(underlineType == FontUnderlineType && UnderlineWordsOnly == underlineWordsOnly)
				return;
			if(underlineType != FontUnderlineType)
				FontUnderlineType = underlineType;
			else {
				initUnderlineWordsOnly = false;
				UnderlineWordsOnly = underlineWordsOnly;
			}
		}
		public bool? Hidden { get { return hidden; } set { hidden = value; } }
		protected internal virtual void InitializeController() {
			CharacterFormattingInfo sourceInfo = SourceCharacterProperties.Info;
			CharacterFormattingOptions sourceOptions = SourceCharacterProperties.Options;
			AllCaps = ConvertToNullable(sourceOptions.UseAllCaps, sourceInfo.AllCaps);
			Script = ConvertToNullable(sourceOptions.UseScript, sourceInfo.Script);
			FontStrikeoutType = ConvertToNullable(sourceOptions.UseFontStrikeoutType, sourceInfo.FontStrikeoutType);
			FontName = (sourceOptions.UseFontName) ? sourceInfo.FontName : null;
			FontBold = ConvertToNullable(sourceOptions.UseFontBold && sourceOptions.UseFontItalic, sourceInfo.FontBold);
			FontItalic = ConvertToNullable(sourceOptions.UseFontBold && sourceOptions.UseFontItalic, sourceInfo.FontItalic);
			DoubleFontSize = ConvertToNullable(sourceOptions.UseDoubleFontSize, sourceInfo.DoubleFontSize);
			ForeColor = ConvertToNullable(sourceOptions.UseForeColor, sourceInfo.ForeColor);
			UnderlineColor = ConvertToNullable(sourceOptions.UseUnderlineColor, sourceInfo.UnderlineColor);
			FontUnderlineType = ConvertToNullable(sourceOptions.UseFontUnderlineType, sourceInfo.FontUnderlineType);
			UnderlineWordsOnly = ConvertToNullable(sourceOptions.UseUnderlineWordsOnly, sourceInfo.UnderlineWordsOnly);
			Hidden = ConvertToNullable(sourceOptions.UseHidden, SourceInfo.Hidden);
		}		
		Nullable<T> ConvertToNullable<T>(bool use, T value) where T : struct {
			if (use)
				return value;
			return null;
		}
		public override void ApplyChanges() {
			ApplyAllCaps();
			ApplyFontName();
			ApplyFontBold();
			ApplyFontItalic();
			ApplyDoubleFontSize();
			ApplyForeColor();
			ApplyFontUnderlineType();
			ApplyUnderlineColor();
			ApplyFontStrikeoutType();
			ApplyScript();
			ApplyUnderlineWordsOnly();
			ApplyHidden();
		}
		protected internal virtual void ApplyAllCaps() {
			SourceOptions.UseAllCaps = (AllCaps != null) && (SourceInfo.AllCaps != AllCaps || !SourceOptions.UseAllCaps);
			if (SourceOptions.UseAllCaps)
				SourceInfo.AllCaps = AllCaps.Value;
		}
		protected internal virtual void ApplyFontName() {
			SourceOptions.UseFontName = !String.IsNullOrEmpty(FontName) && (SourceInfo.FontName != FontName || !SourceOptions.UseFontName);
			if (SourceOptions.UseFontName)
				SourceInfo.FontName = FontName;
		}
		protected internal virtual void ApplyFontBold() {
			SourceOptions.UseFontBold = (FontBold != null) && (SourceInfo.FontBold != FontBold || !SourceOptions.UseFontBold);
			if (SourceOptions.UseFontBold)
				SourceInfo.FontBold = FontBold.Value;
		}
		protected internal virtual void ApplyFontItalic() {
			SourceOptions.UseFontItalic = (FontItalic != null) && (SourceInfo.FontItalic != FontItalic || !SourceOptions.UseFontItalic);
			if (SourceOptions.UseFontItalic)
				SourceInfo.FontItalic = FontItalic.Value;
		}
		protected internal virtual void ApplyDoubleFontSize() {
			SourceOptions.UseDoubleFontSize = (DoubleFontSize != null) && (SourceInfo.DoubleFontSize != DoubleFontSize || !SourceOptions.UseDoubleFontSize);
			if (SourceOptions.UseDoubleFontSize)
				SourceInfo.DoubleFontSize = Math.Max(1, DoubleFontSize.Value) ;
		}
		protected internal virtual void ApplyForeColor() {
			SourceOptions.UseForeColor = (ForeColor != null) && (!IsColorsEqual(SourceInfo.ForeColor, ForeColor.Value) || !SourceOptions.UseForeColor);
			if (SourceOptions.UseForeColor)
				SourceInfo.ForeColor = ForeColor.Value;
		}
		protected internal virtual void ApplyFontUnderlineType() {
			SourceOptions.UseFontUnderlineType = (FontUnderlineType != null) && (!SourceOptions.UseFontUnderlineType || SourceInfo.FontUnderlineType != FontUnderlineType);
			if (SourceOptions.UseFontUnderlineType)
				SourceInfo.FontUnderlineType = FontUnderlineType.Value;
		}
		protected internal virtual void ApplyUnderlineColor() {
			SourceOptions.UseUnderlineColor = (UnderlineColor != null) && (!IsColorsEqual(SourceInfo.UnderlineColor, UnderlineColor.Value) || !SourceOptions.UseUnderlineColor);
			if (SourceOptions.UseUnderlineColor)
				SourceInfo.UnderlineColor = UnderlineColor.Value;
		}
		protected internal virtual void ApplyFontStrikeoutType() {
			SourceOptions.UseFontStrikeoutType = (FontStrikeoutType != null) && (SourceInfo.FontStrikeoutType != FontStrikeoutType || !SourceOptions.UseFontStrikeoutType);
			if (SourceOptions.UseFontStrikeoutType)
				SourceInfo.FontStrikeoutType = FontStrikeoutType.Value;
		}
		protected internal virtual void ApplyScript() {
			SourceOptions.UseScript = (Script != null) && (SourceInfo.Script != Script || !SourceOptions.UseScript);
			if (SourceOptions.UseScript)
				SourceInfo.Script = Script.Value;
		}
		protected internal virtual void ApplyUnderlineWordsOnly() {
			SourceOptions.UseUnderlineWordsOnly = (UnderlineWordsOnly != null) && (SourceInfo.UnderlineWordsOnly != UnderlineWordsOnly || !SourceOptions.UseUnderlineWordsOnly);
			if(SourceOptions.UseUnderlineWordsOnly)
				SourceInfo.UnderlineWordsOnly = UnderlineWordsOnly.Value;
		}
		protected internal virtual void ApplyHidden() {
			SourceOptions.UseHidden = (Hidden != null) && (SourceInfo.Hidden != Hidden || !SourceOptions.UseHidden);
			if (SourceOptions.UseHidden)
				SourceInfo.Hidden = Hidden.Value;
		}
		bool IsColorsEqual(Color color1, Color color2) {
			if (color1 != color2)
				return (color1.A == color2.A) && (color1.B == color2.B) && (color1.G == color2.G) && (color1.R == color2.R);
			else
				return true;
		}
	}
#endregion
}
