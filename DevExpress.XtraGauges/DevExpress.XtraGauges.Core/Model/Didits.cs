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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Text;
using DevExpress.Compatibility.System.Drawing.Imaging;
namespace DevExpress.XtraGauges.Core.Model {
	public abstract class BaseDigit : BaseLeafPrimitive {
		byte[] segmentsCore;
		BaseShapeAppearance onAppearanceCore;
		BaseShapeAppearance offAppearanceCore;
		BaseShape digitShapeCore;
		protected BaseDigit(BaseShapeAppearance on, BaseShapeAppearance off) {
			this.onAppearanceCore = on;
			this.offAppearanceCore = off;
		}
		protected BaseShape DigitShape {
			get {
				if(digitShapeCore == null) digitShapeCore = CreateDigitShape();
				return digitShapeCore;
			}
		}
		protected sealed override bool DependsOnValueBounds {
			get { return false; }
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			Shapes.Add(DigitShape.Clone());
			rootShape = null;
		}
		public byte[] Segments {
			get { return segmentsCore; }
			set {
				segmentsCore = value;
				UpdateSegments(GetRootShape());
			}
		}
		protected BaseShapeAppearance CalculateSegmentAppearance(byte segmentStatus) {
			return (segmentStatus == 0) ? offAppearanceCore : onAppearanceCore;
		}
		protected void UpdateSegmentShape(string shapeName, int segmentIndex, ComplexShape rootShape) {
			BaseShapeAppearance appearance = CalculateSegmentAppearance(segmentsCore[segmentIndex]);
			BaseShape shape = rootShape.Collection[shapeName];
			if(shape.Appearance.IsDifferFrom(appearance)) shape.Appearance.Assign(appearance);
		}
		protected abstract BaseShape CreateDigitShape();
		protected abstract void UpdateSegments(ComplexShape rootShape);
		protected ComplexShape GetRootShape() { return Shapes["IndicatorLEDs"] as ComplexShape; }
		ComplexShape rootShape = null;
		public ComplexShape GetRoot() {
			if(rootShape == null) rootShape = GetRootShape();
			return rootShape;
		}
	}
	public class Digit_S7 : BaseDigit {
		public Digit_S7(BaseShapeAppearance on, BaseShapeAppearance off)
			: base(on, off) {
		}
		protected override BaseShape CreateDigitShape() {
			return DigitShapesFactory.GetDefaultDigitShape(DigitShapeType.Segment7);
		}
		protected override void UpdateSegments(ComplexShape rootShape) {
			UpdateSegmentShape("Path_0", 3, rootShape);
			UpdateSegmentShape("Path_1", 0, rootShape);
			UpdateSegmentShape("Path_2", 4, rootShape);
			UpdateSegmentShape("Path_3", 6, rootShape);
			UpdateSegmentShape("Path_4", 5, rootShape);
			UpdateSegmentShape("Path_5", 1, rootShape);
			UpdateSegmentShape("Path_6", 2, rootShape);
			UpdateSegmentShape("Path_7", 7, rootShape);
			UpdateSegmentShape("Path_8", 8, rootShape);
			UpdateSegmentShape("Path_9", 9, rootShape);
			UpdateSegmentShape("Path_10", 10, rootShape);
			UpdateSegmentShape("Path_11", 11, rootShape);
			UpdateSegmentShape("Path_12", 12, rootShape);
		}
	}
	public class Digit_S14 : BaseDigit {
		public Digit_S14(BaseShapeAppearance on, BaseShapeAppearance off)
			: base(on, off) {
		}
		protected override BaseShape CreateDigitShape() {
			return DigitShapesFactory.GetDefaultDigitShape(DigitShapeType.Segment14);
		}
		protected override void UpdateSegments(ComplexShape rootShape) {
			UpdateSegmentShape("Path_0", 9, rootShape);
			UpdateSegmentShape("Path_1", 10, rootShape);
			UpdateSegmentShape("Path_2", 11, rootShape);
			UpdateSegmentShape("Path_3", 15, rootShape);
			UpdateSegmentShape("Path_4", 14, rootShape);
			UpdateSegmentShape("Path_5", 13, rootShape);
			UpdateSegmentShape("Path_6", 12, rootShape);
			UpdateSegmentShape("Path_7", 8, rootShape);
			UpdateSegmentShape("Path_8", 5, rootShape);
			UpdateSegmentShape("Path_9", 6, rootShape);
			UpdateSegmentShape("Path_10", 7, rootShape);
			UpdateSegmentShape("Path_11", 2, rootShape);
			UpdateSegmentShape("Path_12", 1, rootShape);
			UpdateSegmentShape("Path_13", 0, rootShape);
			UpdateSegmentShape("Path_14", 3, rootShape);
			UpdateSegmentShape("Path_15", 4, rootShape);
			UpdateSegmentShape("Path_16", 16, rootShape);
			UpdateSegmentShape("Path_17", 17, rootShape);
			UpdateSegmentShape("Path_18", 18, rootShape);
			UpdateSegmentShape("Path_19", 19, rootShape);
		}
	}
	public abstract class Digit_DotMatrix : BaseDigit {
		protected Digit_DotMatrix(BaseShapeAppearance on, BaseShapeAppearance off)
			: base(on, off) {
		}
		protected abstract int GetMatrixWidth();
		protected abstract int GetMatrixHeight();
		protected override void UpdateSegments(ComplexShape rootShape) {
			int index = 0;
			for(int row = 0; row < GetMatrixHeight(); row++) {
				for(int col = 0; col < GetMatrixWidth(); col++) {
					UpdateSegmentShape("Dot_" + row.ToString() + "_" + col.ToString(), index++, rootShape);
				}
			}
		}
	}
	public class Digit_M5x8 : Digit_DotMatrix {
		public Digit_M5x8(BaseShapeAppearance on, BaseShapeAppearance off)
			: base(on, off) {
		}
		protected override BaseShape CreateDigitShape() {
			return DigitShapesFactory.GetDefaultDigitShape(DigitShapeType.Matrix5x8);
		}
		protected override int GetMatrixWidth() { return 5; }
		protected override int GetMatrixHeight() { return 8; }
	}
	public class Digit_M8x14 : Digit_DotMatrix {
		public Digit_M8x14(BaseShapeAppearance on, BaseShapeAppearance off)
			: base(on, off) {
		}
		protected override BaseShape CreateDigitShape() {
			return DigitShapesFactory.GetDefaultDigitShape(DigitShapeType.Matrix8x14);
		}
		protected override int GetMatrixWidth() { return 8; }
		protected override int GetMatrixHeight() { return 14; }
	}
	public abstract class BaseSegmentsCalculator {
		public const byte Active = 0x01;
		public static bool IsColon(char c) {
			return c == ':' || c == ';';
		}
		public static bool IsPoint(char c) {
			return IsTopPoint(c) || IsBottomPoint(c);
		}
		public static bool IsPointOrColon(char c) {
			return IsTopPoint(c) || IsBottomPoint(c) || IsColon(c);
		}
		public static bool IsBottomPoint(char c) {
			return c == '.' || c == ',';
		}
		public static bool IsTopPoint(char c) {
			return c == '\'';
		}
		public static bool IsMinus(char c) {
			return c == '-';
		}
		public static bool IsUnderline(char c) {
			return c == '_';
		}
		protected bool IsNotZeroLengthSymbol(char prev, char current) {
			if(IsTopPoint(prev) && IsTopPoint(current)) return true;
			if(IsBottomPoint(prev) && IsBottomPoint(current)) return true;
			if(IsBottomPoint(prev) && IsTopPoint(current)) return true;
			if(IsTopPoint(prev) && IsBottomPoint(current)) return true;
			if(IsColon(prev) && IsColon(current)) return true;
			if(IsColon(prev) && IsBottomPoint(current)) return true;
			if(IsTopPoint(prev) && IsColon(current)) return true;
			return false;
		}
		public abstract byte[] CheckColon(bool prev);
		public abstract byte[] CalcChar(char prev, char current, char next);
		public abstract bool AcceptChar(char prev, char current, char next);
	}
	public class SegmentsCalculator_S7 : BaseSegmentsCalculator {
		static byte[] n0 = new byte[] { 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n1 = new byte[] { 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
		static byte[] n2 = new byte[] { 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n3 = new byte[] { 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n4 = new byte[] { 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
		static byte[] n5 = new byte[] { 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n6 = new byte[] { 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n7 = new byte[] { 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
		static byte[] n8 = new byte[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] n9 = new byte[] { 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 };
		static byte[] Empty = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		public sealed override byte[] CheckColon(bool prev) {
			byte[] result = new byte[Empty.Length];
			if(prev) {
				result[9] = Active;
				result[11] = Active;
			}
			else {
				result[10] = Active;
				result[12] = Active;
			}
			return result;
		}
		public sealed override bool AcceptChar(char prev, char current, char next) {
			if(IsNotZeroLengthSymbol(prev, current)) return true;
			if(current == '-' || current == '_') return true;
			return char.IsDigit(current) || current == ' ';
		}
		public sealed override byte[] CalcChar(char prev, char current, char next) {
			byte[] result = new byte[Empty.Length];
			if(char.IsDigit(current)) {
				switch(current) {
					case '0': n0.CopyTo(result, 0); break;
					case '1': n1.CopyTo(result, 0); break;
					case '2': n2.CopyTo(result, 0); break;
					case '3': n3.CopyTo(result, 0); break;
					case '4': n4.CopyTo(result, 0); break;
					case '5': n5.CopyTo(result, 0); break;
					case '6': n6.CopyTo(result, 0); break;
					case '7': n7.CopyTo(result, 0); break;
					case '8': n8.CopyTo(result, 0); break;
					case '9': n9.CopyTo(result, 0); break;
				}
			}
			if(IsTopPoint(current))
				result[7] = Active;
			if(IsMinus(current))
				result[3] = Active;
			if(IsUnderline(current))
				result[6] = Active;
			if(IsBottomPoint(current))
				result[8] = Active;
			if(IsColon(current)) {
				result[10] = Active;
				result[12] = Active;
			}
			if(!IsColon(current)) {
				if(IsBottomPoint(next))
					result[8] = Active;
			}
			if(IsTopPoint(prev))
				result[7] = Active;
			if(!IsTopPoint(current)) {
				if(IsColon(next)) {
					result[10] = Active;
					result[12] = Active;
				}
			}
			if(IsColon(prev)) {
				result[9] = Active;
				result[11] = Active;
			}
			return result;
		}
	}
	public class SegmentsCalculator_S14 : BaseSegmentsCalculator {
		static Hashtable symbols;
		static SegmentsCalculator_S14() {
			symbols = new Hashtable();
			symbols.Add('+', 0x5A00);
			symbols.Add('-', 0x4200);
			symbols.Add('_', 0x0080);
			symbols.Add('/', 0x2400);
			symbols.Add('0', 0x24E7);
			symbols.Add('1', 0x0006);
			symbols.Add('2', 0x42C3);
			symbols.Add('3', 0x4287);
			symbols.Add('4', 0x4226);
			symbols.Add('5', 0x42A5);
			symbols.Add('6', 0x42E5);
			symbols.Add('7', 0x0007);
			symbols.Add('8', 0x42E7);
			symbols.Add('9', 0x42A7);
			symbols.Add(' ', 0x0000);
			symbols.Add('u', 0x00C4);
			symbols.Add('o', 0x42A7);
			symbols.Add('A', 0x4267);
			symbols.Add('B', 0x5887);
			symbols.Add('C', 0x00E1);
			symbols.Add('D', 0x1887);
			symbols.Add('E', 0x42E1);
			symbols.Add('F', 0x4261);
			symbols.Add('G', 0x40E5);
			symbols.Add('H', 0x4266);
			symbols.Add('I', 0x1881);
			symbols.Add('J', 0x00C6);
			symbols.Add('K', 0xA260);
			symbols.Add('L', 0x00E0);
			symbols.Add('M', 0x2166);
			symbols.Add('N', 0x8166);
			symbols.Add('O', 0x00E7);
			symbols.Add('P', 0x4263);
			symbols.Add('Q', 0x80E7);
			symbols.Add('R', 0xC263);
			symbols.Add('S', 0x42A5);
			symbols.Add('T', 0x1801);
			symbols.Add('U', 0x00E6);
			symbols.Add('V', 0x2460);
			symbols.Add('W', 0x8466);
			symbols.Add('X', 0xA500);
			symbols.Add('Y', 0x4A22);
			symbols.Add('Z', 0x2481);
		}
		int GetPointSymbol(char current) {
			if(IsBottomPoint(current)) return 0x08;
			if(IsTopPoint(current)) return 0x10;
			throw new NotSupportedException();
		}
		public sealed override byte[] CheckColon(bool prev) {
			byte[] result = new byte[20];
			if(prev) {
				result[18] = Active;
				result[19] = Active;
			}
			else {
				result[16] = Active;
				result[17] = Active;
			}
			return result;
		}
		public sealed override bool AcceptChar(char prev, char current, char next) {
			if(IsNotZeroLengthSymbol(prev, current)) return true;
			return symbols.Contains(char.ToUpper(current));
		}
		public sealed override byte[] CalcChar(char prev, char current, char next) {
			byte[] result = new byte[] {0, 0, 0, 0, 
										0, 0, 0, 0, 
										0, 0, 0, 0, 
										0, 0, 0, 0,
										0, 0, 0, 0 };
			if(AcceptChar(prev, current, next)) {
				current = char.ToUpper(current);
				if(!IsColon(current)) {
					int charMap = IsNotZeroLengthSymbol(prev, current) ? GetPointSymbol(current) : (int)symbols[current];
					for(int i = 0; i < 16; i++) {
						result[i] = (byte)(charMap % 2);
						charMap /= 2;
					}
				}
				else {
					result[16] = Active;
					result[17] = Active;
				}
				if(!IsColon(current)) {
					if(IsBottomPoint(next))
						result[3] = Active;
				}
				if(IsTopPoint(prev))
					result[4] = Active;
				if(!IsTopPoint(current)) {
					if(IsColon(next)) {
						result[16] = Active;
						result[17] = Active;
					}
				}
				if(IsColon(prev)) {
					result[18] = Active;
					result[19] = Active;
				}
			}
			return result;
		}
	}
	public abstract class SegmentsCalculator_DotMatrix : BaseSegmentsCalculator {
		CharacterMapGenerator generatorCore;
		public SegmentsCalculator_DotMatrix() {
			generatorCore = CreateCharacterGenerator();
		}
		protected abstract CharacterMapGenerator CreateCharacterGenerator();
		protected CharacterMapGenerator Generator {
			get { return generatorCore; }
		}
		public override byte[] CheckColon(bool prev) {
			return new byte[0];
		}
		public sealed override bool AcceptChar(char prev, char current, char next) { return true; }
		public sealed override byte[] CalcChar(char prev, char current, char next) {
			byte[] result = new byte[Generator.CharWidth * Generator.CharHeight];
			byte[] code = Generator.GetCharacterCode(current);
			int count = 0;
			for(int h = 0; h < Generator.CharHeight; h++) {
				for(int w = 0; w < Generator.CharWidth; w++) {
					result[count++] = (byte)(code[h] & (1 << w));
				}
			}
			return result;
		}
	}
	public class SegmentsCalculator_M5x8 : SegmentsCalculator_DotMatrix {
		protected override CharacterMapGenerator CreateCharacterGenerator() {
			return new CharacterMapGenerator(null, 8, FontStyle.Regular, 5, 8);
		}
	}
	public class SegmentsCalculator_M8x14 : SegmentsCalculator_DotMatrix {
		protected override CharacterMapGenerator CreateCharacterGenerator() {
			return new CharacterMapGenerator(null, 14, FontStyle.Bold, 8, 14, 1);
		}
	}
	public abstract class BaseDigitalGaugeModel : BaseGaugeModel {
		BaseSegmentsCalculator calculatorCore;
		BaseDigit[] digitsCore;
		string[] ZeroLengthLetters = { ",", ".", "`", ":" };
		SizeF contentSizeCore;
		protected override void OnCreate() {
			base.OnCreate();
			this.calculatorCore = CreateSegmentsCalculator();
		}
		protected override void OnDispose() {
			digitsCore = null;
			base.OnDispose();
		}
		protected BaseDigitalGaugeModel(IGauge gauge)
			: base(gauge) {
		}
		public override SizeF ContentSize {
			get { return contentSizeCore; }
		}
		public BaseDigit[] Digits {
			get { return digitsCore; }
		}
		protected BaseSegmentsCalculator Calculator {
			get { return calculatorCore; }
		}
		protected int GetAutoTextLength(string text) {
			if(string.IsNullOrEmpty(text)) return -1;
			foreach(string c in ZeroLengthLetters) {
				text = text.Replace(c, String.Empty);
			}
			return text.Length;
		}
		protected bool AllowCreateDigitsArray(int digitCount) {
			if(digitsCore == null) return true;
			if(digitsCore.Length == digitCount) return false;
			return true;
		}
		public override void Calc(IGauge owner, RectangleF bounds) {
			int ownerDigitCount = GetDigitCount();
			string ownerText = GetText();
			TextSpacing ownerPadding = GetPadding();
			float ownerLetterSpacing = GetLetterSpacing();
			if(ownerDigitCount <= 0) ownerDigitCount = GetAutoTextLength(ownerText);
			if(ownerDigitCount > 0) {
				if(AllowCreateDigitsArray(ownerDigitCount)) {
					if(Digits != null) {
						for(int i = 0; i < Digits.Length; i++) {
							if(digitsCore[i] != null) Composite.Remove(Digits[i]);
						}
					}
					this.digitsCore = new BaseDigit[ownerDigitCount];
				}
				if(Owner != null && ownerText != null) {
					float offset = ownerPadding.Left;
					char prev, current, next;
					OnShapesChanged();
					for(int i = 0; i < ownerDigitCount; i++) {
						if(Digits[i] == null) Digits[i] = CreateDigit();
						Digits[i].Segments = Calculator.CalcChar(' ', ' ', ' ');
						Digits[i].Location = new PointF(offset, ownerPadding.Top);
						offset += Digits[i].GetRoot().BoundingBox.Width + ownerLetterSpacing;
						string name = "digit" + i.ToString();
						Digits[i].Name = name;
						Digits[i].ZOrder = -1;
						Composite.Add(Digits[i]);
					}
					PointF localOffset = Digits[0].GetRoot().BoundingBox.Location;
					Location = new PointF(Location.X - localOffset.X, Location.Y - localOffset.Y);
					int count = ownerText.Length - 1;
					int digitsCount = ownerDigitCount - 1;
					int strDigitsCount = ownerText.Length - 1;
					for(int i = count; i >= 0; i--) {
						if(i == 0) prev = ' '; else prev = ownerText[strDigitsCount - 1];
						if(i == count) next = ' '; else next = ownerText[strDigitsCount + 1];
						current = ownerText[strDigitsCount];
						if(Calculator.AcceptChar(prev, current, next)) {
							byte[] segments = Calculator.CalcChar(prev, current, next);
							CheckColon(ownerText, prev, next, strDigitsCount, segments);
							Digits[digitsCount--].Segments = segments;
						}
						strDigitsCount--;
						if(digitsCount < 0) break;
					}
					SizeF digitsSize = new SizeF(offset, Digits[0].GetRoot().BoundingBox.Height);
					this.contentSizeCore = new SizeF(offset + ownerPadding.Right, Digits[0].GetRoot().BoundingBox.Height + ownerPadding.Height);
					PointF2D start = new PointF2D(ownerPadding.Left, 0);
					PointF2D end = new PointF2D(digitsSize.Width, digitsSize.Height + ownerPadding.Height);
					OnBackgroundBoundsCalculated(start, end);
				}
			}
			else {
				if (Digits != null) {
					int count = Digits.Length;
					for (int i = 0; i < count; i++)
						Composite.Remove(Digits[i]);
				}
				digitsCore = new BaseDigit[0];
				OnShapesChanged();
			}
			base.Calc(owner, bounds);
		}
		void CheckColon(string ownerText, char prev, char next, int pos, byte[] segments) {
			if(pos + 2 < ownerText.Length) {
				char nextNext = ownerText[pos + 2];
				if(BaseSegmentsCalculator.IsPointOrColon(next) && BaseSegmentsCalculator.IsColon(nextNext)) {
					if(BaseSegmentsCalculator.IsTopPoint(next)) return;
					byte[] colonBytes = Calculator.CheckColon(false);
					for(int b = 0; b < colonBytes.Length; b++) {
						if(colonBytes[b] == BaseSegmentsCalculator.Active)
							segments[b] = BaseSegmentsCalculator.Active;
					}
				}
			}
			if(pos - 2 >= 0) {
				char prevPrev = ownerText[pos - 2];
				if(BaseSegmentsCalculator.IsPointOrColon(prev) && BaseSegmentsCalculator.IsColon(prevPrev)) {
					if(BaseSegmentsCalculator.IsBottomPoint(prev)) return;
					byte[] colonBytes = Calculator.CheckColon(true);
					for(int b = 0; b < colonBytes.Length; b++) {
						if(colonBytes[b] == BaseSegmentsCalculator.Active)
							segments[b] = BaseSegmentsCalculator.Active;
					}
				}
			}
		}
		protected abstract BaseDigit CreateDigit();
		protected abstract BaseSegmentsCalculator CreateSegmentsCalculator();
		protected abstract string GetText();
		protected abstract int GetDigitCount();
		protected abstract TextSpacing GetPadding();
		protected abstract float GetLetterSpacing();
		protected abstract void OnBackgroundBoundsCalculated(PointF2D start, PointF2D end);
	}
	public static class UserCharacterMap {
		static Dictionary<char, byte[]> userMap=null;
		static bool userCharacterMapLoadedCore = false;
		internal static void SynchronizeCache(Dictionary<char, byte[]> cache, int charHeight) {
			if(!UserCharacterMapLoaded) return;
			if(cache != null) {
				cache.Clear();
				foreach(KeyValuePair<char, byte[]> pair in userMap) {
					if(pair.Value.Length == charHeight) cache.Add(pair.Key, pair.Value);
				}
				userCharacterMapLoadedCore = false;
			}
		}
		static bool UserCharacterMapLoaded {
			get { return userCharacterMapLoadedCore; }
		}
		public static void Load(Dictionary<char, byte[]> map) {
			if(userMap == null) userMap = new Dictionary<char, byte[]>();
			userMap.Clear();
			if(map != null) {
				foreach(KeyValuePair<char, byte[]> pair in map) userMap.Add(pair.Key, pair.Value);
				userCharacterMapLoadedCore = true;
			}
		}
	}
	public class CharacterMapGenerator {
		FontFamily fontFamilyCore;
		FontStyle fontStyleCore;
		Font fontCore;
		StringFormat formatCore;
		readonly float fontSizeCore;
		readonly int charWidthCore;
		readonly int charHeightCore;
		readonly float charOffsetCore;
		Dictionary<char, byte[]> codeCacheCore;
		public CharacterMapGenerator(string family, float size, FontStyle style, int charWidth, int charHeight)
			: this(family, size, style, charWidth, charHeight, 0f) {
		}
		public CharacterMapGenerator(string family, float size, FontStyle style, int charWidth, int charHeight, float charOffset) {
			this.codeCacheCore = new Dictionary<char, byte[]>();
			this.fontStyleCore = style;
			this.fontSizeCore = size;
			this.charOffsetCore = charOffset;
			try {
				this.fontFamilyCore = new FontFamily(string.IsNullOrEmpty(family) ? "Lucida Console" : family);
			}
			catch {
				this.fontFamilyCore = FontFamily.GenericMonospace;
			}
			this.charWidthCore = Math.Max(5, Math.Min(charWidth, sizeof(byte) * 8));
			this.charHeightCore = Math.Max(8, Math.Min(charHeight, 16));
		}
		protected void CheckUserLoadedCharacterMap() {
			UserCharacterMap.SynchronizeCache(CodeCache, CharHeight);
		}
		public FontFamily FontFamily {
			get { return fontFamilyCore; }
		}
		public float FontSize {
			get { return fontSizeCore; }
		}
		public FontStyle FontStyle {
			get { return fontStyleCore; }
		}
		public int CharWidth {
			get { return charWidthCore; }
		}
		public int CharHeight {
			get { return charHeightCore; }
		}
		public float CharOffset {
			get { return charOffsetCore; }
		}
		protected Font Font {
			get {
				if(fontCore == null) {
					if(FontFamily.IsStyleAvailable(FontStyle))
						fontCore = new Font(FontFamily, FontSize, FontStyle, GraphicsUnit.Pixel);
					else fontCore = new Font(FontFamily, FontSize, GraphicsUnit.Pixel);
				}
				return fontCore;
			}
		}
		protected StringFormat Format {
			get {
				if(formatCore == null) {
					formatCore = new StringFormat();
					formatCore.Alignment = StringAlignment.Center;
					formatCore.LineAlignment = StringAlignment.Center;
					formatCore.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
					formatCore.Trimming = StringTrimming.None;
				}
				return formatCore;
			}
		}
		protected Dictionary<char, byte[]> CodeCache {
			get { return codeCacheCore; }
		}
		public byte[] GetCharacterCode(char c) {
			CheckUserLoadedCharacterMap();
			byte[] result;
			CodeCache.TryGetValue(c, out result);
			if(result == null) {
				result = GetCharacterCodeCore(c);
				CodeCache.Add(c, result);
			}
			return result;
		}
		[System.Security.SecuritySafeCritical]
		protected byte[] GetCharacterCodeCore(char c) {
			byte[] result = GetEmptyCharacterCode();
#if DXPORTABLE
			return result;
#else
			using(Bitmap bmp = new Bitmap(CharWidth, CharHeight, PixelFormat.Format32bppArgb)) {
				using(Graphics g = Graphics.FromImage(bmp)) {
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
					g.SmoothingMode = SmoothingMode.None;
					g.Clear(Color.Black);
					float h = Font.GetHeight(g);
					RectangleF characterPlace = new RectangleF(0, CharOffset + ((float)CharHeight - h) * 0.5f,
							CharWidth, ((float)CharHeight + h) * 0.5f);
					g.DrawString(new string(c, 1), Font, Brushes.White, characterPlace, Format);
				}
				Rectangle cell = new Rectangle(0, 0, CharWidth, CharHeight);
				BitmapData data = bmp.LockBits(cell, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				int size = data.Width * data.Height;
				Int32[] colorData = new Int32[size];
				Marshal.Copy(data.Scan0, colorData, 0, size);
				for(int i = 0; i < colorData.Length; i++) {
					int row = i / data.Width;
					int column = i % data.Width;
					byte mask = (byte)(((UInt32)colorData[i] & 0x00000001U) << column);
					result[row] |= mask;
				}
				bmp.UnlockBits(data);
			}
			return result;
#endif
		}
		protected byte[] GetEmptyCharacterCode() {
			return new byte[CharHeight];
		}
	}
}
