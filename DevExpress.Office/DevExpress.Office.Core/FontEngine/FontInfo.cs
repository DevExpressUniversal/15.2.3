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
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Drawing {
	#region FontInfo (abstract class)
	public abstract class FontInfo : IDisposable {
		#region Fields
		bool isDisposed;
		int ascent;
		int descent;
		int free;
		int lineSpacing;
		int spaceWidth;
		int pilcrowSignWidth;
		int nonBreakingSpaceWidth;
		int underscoreWidth;
		int middleDotWidth;
		int dotWidth;
		int dashWidth;
		int equalSignWidth;
		float maxDigitWidth;
		int underlineThickness;
		int underlinePosition;
		int strikeoutThickness;
		int strikeoutPosition;
		Size subscriptSize;
		Point subscriptOffset;
		Size superscriptSize;
		Point superscriptOffset;
		float sizeInPoints;
		int baseFontIndex = -1;
		int charset;
		#endregion
		protected FontInfo(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline) {
			CreateFont(measurer, fontName, doubleFontSize, fontBold, fontItalic, fontStrikeout, fontUnderline);
			Initialize(measurer);
			CalculateFontParameters(measurer);
		}
		#region Properties
		public abstract Font Font { get; }
		public bool IsDisposed { get { return isDisposed; } }
		public abstract bool Bold { get; }
		public abstract bool Italic { get; }
		public abstract bool Underline { get; }
		public abstract bool Strikeout { get; }
		public abstract float Size { get; }
		public abstract string Name { get; }
		public abstract string FontFamilyName { get; }
		public int Ascent { get { return ascent; } protected internal set { ascent = value; } }
		public int AscentAndFree { get { return ascent + free; } }
		public int Descent { get { return descent; } protected internal set { descent = value; } }
		public int Free { get { return free; } protected internal set { free = value; } }
		public int LineSpacing { get { return lineSpacing; } protected internal set { lineSpacing = value; } }
		public int SpaceWidth { get { return spaceWidth; } protected internal set { spaceWidth = value; } }
		public int PilcrowSignWidth { get { return pilcrowSignWidth; } protected internal set { pilcrowSignWidth = value; } }
		public int NonBreakingSpaceWidth { get { return nonBreakingSpaceWidth; } set { nonBreakingSpaceWidth = value; } }
		public int UnderscoreWidth { get { return underscoreWidth; } set { underscoreWidth = value; } }
		public int MiddleDotWidth { get { return middleDotWidth; } set { middleDotWidth = value; } }
		public int DotWidth { get { return dotWidth; } set { dotWidth = value; } }
		public int DashWidth { get { return dashWidth; } set { dashWidth = value; } }
		public int EqualSignWidth { get { return equalSignWidth; } protected internal set { equalSignWidth = value; } }
		public int UnderlineThickness { get { return underlineThickness; } protected internal set { underlineThickness = value; } }
		public int UnderlinePosition { get { return underlinePosition; } protected internal set { underlinePosition = value; } }
		public int StrikeoutThickness { get { return strikeoutThickness; } protected internal set { strikeoutThickness = value; } }
		public int StrikeoutPosition { get { return strikeoutPosition; } protected internal set { strikeoutPosition = value; } }
		public Size SubscriptSize { get { return subscriptSize; } protected internal set { subscriptSize = value; } }
		public Point SubscriptOffset { get { return subscriptOffset; } protected internal set { subscriptOffset = value; } }
		public Size SuperscriptSize { get { return superscriptSize; } protected internal set { superscriptSize = value; } }
		public Point SuperscriptOffset { get { return superscriptOffset; } protected internal set { superscriptOffset = value; } }
		public float SizeInPoints { get { return sizeInPoints; } protected internal set { sizeInPoints = value; } }
		internal int BaseFontIndex { get { return baseFontIndex; } set { baseFontIndex = value; } }
		public int Charset { get { return charset; } protected internal set { charset = value; } }
		public float MaxDigitWidth { get { return maxDigitWidth; } protected internal set { maxDigitWidth = value; } }
		#endregion
		protected internal abstract void Initialize(FontInfoMeasurer measurer);
		protected internal abstract void CalculateFontVerticalParameters(FontInfoMeasurer measurer);
		protected internal abstract void CalculateUnderlineAndStrikeoutParameters(FontInfoMeasurer measurer);
		protected internal abstract int CalculateFontCharset(FontInfoMeasurer measurer);
		protected internal abstract void CreateFont(FontInfoMeasurer measurer, string fontName, int doubleFontSize, bool fontBold, bool fontItalic, bool fontStrikeout, bool fontUnderline);
		protected internal abstract float CalculateFontSizeInPoints();
		protected internal virtual void CalculateFontParameters(FontInfoMeasurer measurer) {
			CalculateFontVerticalParameters(measurer);
			sizeInPoints = CalculateFontSizeInPoints();
			AdjustFontParameters();
			spaceWidth = (int)Math.Round(measurer.MeasureCharacterWidthF(' ', this));
			pilcrowSignWidth = measurer.MeasureCharacterWidth(Characters.PilcrowSign, this);
			nonBreakingSpaceWidth = measurer.MeasureCharacterWidth(Characters.NonBreakingSpace, this);
			underscoreWidth = measurer.MeasureCharacterWidth(Characters.Underscore, this);
			middleDotWidth = measurer.MeasureCharacterWidth(Characters.MiddleDot, this);
			dotWidth = measurer.MeasureCharacterWidth(Characters.Dot, this);
			dashWidth = measurer.MeasureCharacterWidth(Characters.Dash, this);
			equalSignWidth = measurer.MeasureCharacterWidth(Characters.EqualSign, this);
			charset = CalculateFontCharset(measurer);
			maxDigitWidth = CalculateMaxDigitWidth(measurer);
			CalculateUnderlineAndStrikeoutParameters(measurer);
		}
		float CalculateMaxDigitWidth(FontInfoMeasurer measurer) {
			return measurer.MeasureMaxDigitWidthF(this);
		}
		protected internal virtual void AdjustFontParameters() {
			free = lineSpacing - ascent - descent;
			if (free < 0) {
				descent += free;
				if (descent < 0) {
					ascent += descent;
					descent = 0;
				}
				free = 0;
			}
		}
		protected internal abstract void CalculateSuperscriptOffset(FontInfo baseFontInfo);
		protected internal abstract void CalculateSubscriptOffset(FontInfo baseFontInfo);
		protected internal virtual FontInfo GetBaseFontInfo(IDocumentModel documentModel) {
			return baseFontIndex >= 0 ? documentModel.FontCache[baseFontIndex] : this;
		}
		public virtual int GetBaseAscentAndFree(IDocumentModel documentModel) {
			return GetBaseFontInfo(documentModel).AscentAndFree;
		}
		public virtual int GetBaseDescent(IDocumentModel documentModel) {
			return GetBaseFontInfo(documentModel).Descent;
		}
		public FontStyle CalculateFontStyle() {
			FontStyle style = FontStyle.Regular;
			if (Bold) style |= FontStyle.Bold;
			if (Italic) style |= FontStyle.Italic;
			if (Strikeout) style |= FontStyle.Strikeout;
			if (Underline) style |= FontStyle.Underline;
			return style;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
			isDisposed = true;
		}
		public virtual void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
