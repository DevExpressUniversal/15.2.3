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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Data;
using System.Drawing.Design;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BarCode {
	[TypeConverter(typeof(BarCodeGeneratorTypeConverter)),
	Editor("DevExpress.XtraPrinting.BarCode.Design.BarCodeGeneratorEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(UITypeEditor))]
	public abstract class BarCodeGeneratorBase : ICloneable {
		#region static
		static protected string charSetDigits = "0123456789";
		static protected string charSetUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		static protected string charSetLowerCase = "abcdefghijklmnopqrstuvwxyz";
		static protected string charSetAll = GenerateComplement("", 128);
		internal static int Char2Int(char ch) {
			if(charSetDigits.IndexOf(ch) > -1)
				return (int)ch - (int)'0';
			return 0;
		}
		static string GenerateComplement(string chars, int upperBound) {
			StringBuilder sb = new StringBuilder(upperBound);
			for(int i = 1; i < upperBound; i++) {
				char ch = (char)i;
				if(chars.IndexOf(ch) < 0)
					sb.Append(ch);
			}
			return sb.ToString();
		}
		protected static RectangleF AlignVerticalBarcodeBound(RectangleF barcodeBounds, float height, TextAlignment align) {
			switch(align) {
				case TextAlignment.MiddleLeft:
				case TextAlignment.MiddleCenter:
				case TextAlignment.MiddleRight:
				case TextAlignment.MiddleJustify:
					barcodeBounds.Y += (barcodeBounds.Height - height) / 2;
					barcodeBounds.Height = height;
					break;
				case TextAlignment.BottomLeft:
				case TextAlignment.BottomCenter:
				case TextAlignment.BottomRight:
				case TextAlignment.BottomJustify:
					barcodeBounds.Y = barcodeBounds.Bottom - height;
					barcodeBounds.Height = height;
					break;
			}
			return barcodeBounds;
		}
		static bool IsHorzOrientation(DevExpress.XtraPrinting.BarCode.BarCodeOrientation orientation) {
			return orientation == DevExpress.XtraPrinting.BarCode.BarCodeOrientation.Normal || orientation == DevExpress.XtraPrinting.BarCode.BarCodeOrientation.UpsideDown;
		}
		static PaddingInfo GetOrientedPaddingSide(DevExpress.XtraPrinting.BarCode.BarCodeOrientation orientation, PaddingInfo padding) {
			PaddingInfo orientedPadding = new PaddingInfo(padding.Left, padding.Right, padding.Top, padding.Bottom, padding.Dpi);
			switch(orientation) {
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateRight:
					orientedPadding.RotatePaddingCounterclockwise(3);
					break;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.UpsideDown:
					orientedPadding.RotatePaddingCounterclockwise(2);
					break;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateLeft:
					orientedPadding.RotatePaddingCounterclockwise(1);
					break;
			}
			return orientedPadding;
		}
		static void AdjustBounds(ref RectangleF textBounds, ref RectangleF barBounds, float textHeight, StringFormat sf) {
			if(sf.LineAlignment == StringAlignment.Near) {
				textBounds.Height = textHeight;
				barBounds.Y += textHeight;
				barBounds.Height -= textHeight;
			} else {
				barBounds.Height -= textHeight;
				textBounds.Y = barBounds.Bottom;
				textBounds.Height = textHeight;
			}
		}
		static void DrawErrorString(IGraphicsBase gr, RectangleF bounds, IBarCodeData data, string errorString) {
			StringFormat sf = (StringFormat)data.Style.StringFormat.Value.Clone();
			try {
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;
				gr.DrawString(errorString, data.Style.Font, gr.GetBrush(data.Style.ForeColor), bounds, sf);
			} finally {
				sf.Dispose();
			}
		}
		protected static SizeF MeasureTextSize(string text, float width, IBarCodeData data, IGraphicsBase gr) {
			SizeF size = width > 0 ?
				gr.MeasureString(text, data.Style.Font, width, data.Style.StringFormat.Value, gr.PageUnit) :
				gr.MeasureString(text, data.Style.Font, gr.PageUnit);
			size.Height += GraphicsUnitConverter.Convert(2f, GraphicsUnit.Pixel, gr.PageUnit);
			return size;
		}
		protected static float MeasureTextHeight(string text, float width, IBarCodeData data, IGraphicsBase gr) {
			SizeF size = width > 0 ?
				gr.MeasureString(text, data.Style.Font, width, data.Style.StringFormat.Value, gr.PageUnit) :
				gr.MeasureString(text, data.Style.Font, gr.PageUnit);
			return size.Height + GraphicsUnitConverter.Convert(2f, GraphicsUnit.Pixel, gr.PageUnit);
		}
		static RectangleF CalcClientBounds(RectangleF bounds, IBarCodeData data, GraphicsUnit pageUnit) {
			PaddingInfo paddingInPageUnit = new PaddingInfo(data.Style.Padding, GraphicsDpi.UnitToDpi(pageUnit));
			PaddingInfo orientedPadding = GetOrientedPaddingSide(data.Orientation, paddingInPageUnit);
			RectangleF clientBounds = bounds;
			if(IsHorzOrientation(data.Orientation)) {
				clientBounds.X += orientedPadding.Left;
				clientBounds.Width -= orientedPadding.Left + orientedPadding.Right;
				clientBounds.Y += orientedPadding.Top;
				clientBounds.Height -= orientedPadding.Top + orientedPadding.Bottom;
			} else {
				clientBounds.X += orientedPadding.Left;
				clientBounds.Y += orientedPadding.Top;
				float width = clientBounds.Width;
				clientBounds.Width = clientBounds.Height;
				clientBounds.Height = width;
				clientBounds.Height -= orientedPadding.Top + orientedPadding.Bottom;
				clientBounds.Width -= orientedPadding.Left + orientedPadding.Right;
			}
			return clientBounds;
		}
		#endregion
		#region Fields & Properties
		bool calcCheckSum = true;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeGeneratorBaseCalcCheckSum"),
#endif
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.BarCode.BarCodeGeneratorBase.CalcCheckSum"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public virtual bool CalcCheckSum {
			get { return calcCheckSum; }
			set { calcCheckSum = value; }
		}
		[Browsable(false),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeGeneratorBaseName"),
#endif
		XtraSerializableProperty(-1)]
		public string Name { get { return SymbologyCode.ToString(); } }
		protected virtual float YRatio { get { return 1; } }
		internal bool ForceEnoughSpace { get; set; }
		internal bool DoNotClip { get; set; }
		#endregion
		protected BarCodeGeneratorBase() {
		}
		protected BarCodeGeneratorBase(BarCodeGeneratorBase source) {
			calcCheckSum = source.calcCheckSum;
		}
		object ICloneable.Clone() {
			return CloneGenerator();
		}
		protected abstract BarCodeGeneratorBase CloneGenerator();
		protected abstract char[] PrepareText(string text);
		protected abstract Hashtable GetPatternTable();
		protected abstract string GetValidCharSet();
		[ Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public abstract BarCodeSymbology SymbologyCode { get; }
		protected virtual float GetPatternWidth(char pattern) {
			return Char2Int(pattern);
		}
		protected virtual string FormatText(string text) {
			return text;
		}
		protected virtual bool IsValidText(string text) {
			string charSet = GetValidCharSet();
			int length = text.Length;
			for(int i = 0; i < length; i++)
				if(charSet.IndexOf(text[i]) < 0)
					return false;
			return true;
		}
		protected virtual bool IsValidPattern(ArrayList pattern) {
			return true;
		}
		protected virtual bool IsValidTextFormat(string text) {
			return true;
		}
		protected virtual ArrayList GetWidthPattern(string pattern) {
			int count = pattern.Length;
			StringBuilder source = new StringBuilder(pattern);
			ArrayList target = new ArrayList();
			for(int i = 0; i < count; i++)
				target.Add(GetPatternWidth(source[i]));
			return target;
		}
		protected virtual ArrayList MakeBarCodePattern(string text) {
			char[] chars = PrepareText(text);
			int charsCount = chars.Length;
			Hashtable charPattern = GetPatternTable();
			ArrayList result = new ArrayList();
			for(int i = 0; i < charsCount; i++) {
				string pattern = charPattern[chars[i]] as string;
				if(pattern == null)
					continue;
				result.AddRange(GetWidthPattern(pattern));
			}
			return result;
		}
		protected virtual float CalcBarCodeWidth(ArrayList pattern, double module) {
			float totalPatternWidth = 0;
			int count = pattern.Count;
			for(int i = 0; i < count; i++)
				totalPatternWidth += (float)pattern[i];
			return (float)((double)totalPatternWidth * module);
		}
		protected virtual float CalcBarCodeHeight(ArrayList pattern, double module) {
			return (float)module;
		}
		protected virtual float GetLeftSpacing(IBarCodeData data, IGraphicsBase gr) {
			return 0.0f;
		}
		protected virtual float GetRightSpacing(IBarCodeData data, IGraphicsBase measurer) {
			return 0.0f;
		}
		protected virtual string MakeDisplayText(string text) {
			return text;
		}
		protected virtual void DrawText(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			gr.DrawString(DisplayText, data.Style.Font, gr.GetBrush(data.Style.ForeColor), bounds, data.Style.StringFormat.Value);
		}
		protected virtual void DrawBarCode(IGraphicsBase gr, RectangleF barBounds, RectangleF textBounds, IBarCodeData data, float xModule, float yModule) {
			float x = barBounds.Left;
			for(int i = 0; i < Pattern.Count; i++) {
				float w = xModule * (float)pattern[i];
				if(i % 2 == 0)
					gr.FillRectangle(gr.GetBrush(data.Style.ForeColor), x, barBounds.Top, w, barBounds.Height);
				x += w;
			}
			if(data.ShowText)
				DrawText(gr, textBounds, data);
		}
		protected virtual RectangleF AlignBarcodeBounds(RectangleF barcodeBounds, float width, float height, TextAlignment align) {
			StringAlignment sa = GraphicsConvertHelper.ToHorzStringAlignment(align);
			switch(sa) {
				case StringAlignment.Center:
					barcodeBounds.Offset((barcodeBounds.Width - width) / 2, 0);
					barcodeBounds.Width = width;
					return barcodeBounds;
				case StringAlignment.Far:
					barcodeBounds.Offset(barcodeBounds.Width - width, 0);
					barcodeBounds.Width = width;
					return barcodeBounds;
				default:
					barcodeBounds.Width = width;
					return barcodeBounds;
			}
		}
		IBarCodeData barCodeData;
		string finalText = null;
		string displayText = null;
		ArrayList pattern = null;
		protected IBarCodeData BarCodeData {
			get { return barCodeData; }
			set {
				barCodeData = value;
				finalText = null;
				displayText = null;
				pattern = null;
			}
		}
		protected string FinalText {
			get {
				if(finalText == null)
					finalText = IsValidText(barCodeData.Text) ? FormatText(barCodeData.Text) : barCodeData.Text;
				return finalText;
			}
		}
		protected string DisplayText {
			get {
				if(displayText == null)
					displayText = MakeDisplayText(FinalText);
				return displayText;
			}
		}
		protected ArrayList Pattern {
			get {
				if(pattern == null) 
					pattern = IsValidText(barCodeData.Text) ? MakeBarCodePattern(FinalText) : new ArrayList();
				return pattern;
			}
		}
		public string GetFinalText(IBarCodeData data) {
			IBarCodeData saved = this.BarCodeData;
			this.BarCodeData = data;
			try {
				return FinalText;
			}
			finally {
				this.BarCodeData = saved;
			}
		}
#if DEBUGTEST
		public string Test_MakeDisplayText(string text) {
			return MakeDisplayText(text);
		}
		public ArrayList Test_Pattern(IBarCodeData barCodeData) {
			BarCodeData = barCodeData;
			try {
				return Pattern;
			} finally {
				BarCodeData = null;
			}
		}
		public string Test_DisplayText(IBarCodeData barCodeData) {
			BarCodeData = barCodeData;
			try {
				return DisplayText;
			} finally {
				BarCodeData = null; 
			}
		}
		public string Test_GetValidCharSet() {
			return GetValidCharSet();
		}
		public ArrayList Test_MakeBarCodePattern(string text) {
			return MakeBarCodePattern(text);
		}
		public string Test_FormatText(string text) {
			return FormatText(text);
		}
		public bool Test_IsValidText(string text) {
			return IsValidText(text);
		}
		public bool Test_IsValidTextFormat(string text) {
			return IsValidTextFormat(text);
		}
		public RectangleF Test_AlignTextBounds(IBarCodeData data, RectangleF barBounds, RectangleF textBounds) {
			return AlignTextBounds(data, barBounds, textBounds);
		}
		public char[] Test_PrepareText(string text) {
			return PrepareText(text);
		}
#endif
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void DrawContent(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			this.BarCodeData = data;
			try {
				DrawContentCore(gr, bounds, data);
			} finally {
				this.BarCodeData = null;
			}
		}
		internal bool CalculateDrawingViewInfo(BarCodeDrawingViewInfo viewInfo, IGraphicsBase gr) {
			this.BarCodeData = viewInfo.Data;
			try {
				return CalculateDrawingViewInfoCore(viewInfo, gr);
			}
			finally {
				this.BarCodeData = null;
			}
		}
		bool CalculateDrawingViewInfoCore(BarCodeDrawingViewInfo viewInfo, IGraphicsBase gr) {
			RectangleF clientBounds = CalcClientBounds(viewInfo.Bounds, viewInfo.Data, gr.PageUnit);
			IBarCodeData data = viewInfo.Data;
			if (clientBounds.Width < 0) {
				viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_CantFitBarcodeToControlBounds);
				return false;
			}
			if (!IsValidText(FinalText)) {
				viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_InvalidBarcodeText);
				return false;
			}
			if (!IsValidTextFormat(FinalText)) {
				viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_InvalidBarcodeTextFormat);
				return false;
			}
			if (!IsValidPattern(Pattern)) {
				viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_InvalidBarcodeData);
				return false;
			}
			float xModule = (float)(data.AutoModule ? CalcAutoModuleX(data, clientBounds, gr) : data.Module);
			float yModule = (float)(data.AutoModule ? CalcAutoModuleY(data, clientBounds, gr) : this.YRatio * xModule);
			float barCodeWidth = CalcBarCodeWidth(Pattern, xModule);
			float barCodeHeight = CalcBarCodeHeight(Pattern, yModule);
			RectangleF barBounds = clientBounds;
			RectangleF textBounds = clientBounds;
			RectangleF boundingBox = new RectangleF(clientBounds.Location, new SizeF(barCodeWidth, barCodeHeight));
			if (data.ShowText) {
				SizeF textSize = MeasureTextSize(DisplayText, clientBounds.Width, data, gr);
				AdjustBounds(ref textBounds, ref barBounds, textSize.Height, data.Style.StringFormat.Value);
				textBounds.Width = textSize.Width;
				if (barBounds.Height < 0 || textBounds.Height < 0) {
					viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_CantFitBarcodeToControlBounds);
					return false;
				}
				float spacing = GetLeftSpacing(data, gr);
				barBounds.X += spacing;
				barBounds.Width -= spacing;
				textBounds.X += spacing;
				boundingBox.Width += spacing;
				spacing = GetRightSpacing(data, gr);
				barBounds.Width -= spacing;
				boundingBox.Width += spacing;
			}
			JustifyBarcodeBounds(data, ref barCodeWidth, ref barCodeHeight, ref barBounds);
			if (!ForceEnoughSpace && (FloatsComparer.Default.FirstGreaterSecond(barCodeWidth, barBounds.Width) || FloatsComparer.Default.FirstGreaterSecond(barCodeHeight, barBounds.Height))) {
				viewInfo.ErrorText = PreviewLocalizer.GetString(PreviewStringId.Msg_CantFitBarcodeToControlBounds);
				return false;
			}
			viewInfo.BarBounds = AlignBarcodeBounds(barBounds, barCodeWidth, barCodeHeight, data.Alignment);
			viewInfo.TextBounds = AlignTextBounds(data, viewInfo.BarBounds, textBounds);
			float textBoundsWidth = data.ShowText ? textBounds.Width : 0;
			viewInfo.BestSize = new SizeF(Math.Max(boundingBox.Size.Width, textBoundsWidth), boundingBox.Size.Height + viewInfo.TextBounds.Height);
			viewInfo.XModule = xModule;
			viewInfo.YModule = yModule;
			return true;
		}
		void DrawContentCore(IGraphicsBase gr, RectangleF bounds, IBarCodeData data) {
			BarCodeDrawingViewInfo viewInfo = new BarCodeDrawingViewInfo(bounds, data);
			if (!CalculateDrawingViewInfoCore(viewInfo, gr)) {
				DrawErrorString(gr, bounds, data, viewInfo.ErrorText);
				return;
			}
			RectangleF oldClip = gr.ClipBounds;
			gr.SaveTransformState();
			try {
				if (!DoNotClip)
					gr.ClipBounds = RectangleF.Intersect(gr.ClipBounds, bounds);
				gr.ResetTransform();
				SizeF offset = GetTransformTranslateOffset(bounds, data);
				gr.TranslateTransform(offset.Width, offset.Height, MatrixOrder.Append);
				gr.RotateTransform(GetTransformRotateAngle(data));
				gr.ApplyTransformState(MatrixOrder.Append, false);
				DrawBarCode(gr, viewInfo.BarBounds, viewInfo.TextBounds, data, viewInfo.XModule, viewInfo.YModule);
			} finally {
				if (!DoNotClip)
					gr.ClipBounds = oldClip;
				gr.ResetTransform();
				gr.ApplyTransformState(MatrixOrder.Append, true);
			}
		}
		protected virtual RectangleF AlignTextBounds(IBarCodeData data, RectangleF barBounds, RectangleF textBounds) {
			textBounds.X = barBounds.X;
			textBounds.Width = barBounds.Width;
			textBounds.Height = (data.ShowText) ? textBounds.Height : 0;
			return textBounds;
		}
		protected virtual void JustifyBarcodeBounds(IBarCodeData data, ref float barCodeWidth, ref float barCodeHeight, ref RectangleF barBounds) {
			if(data.AutoModule) {
				barCodeWidth = CorrectBarcodeWidth(barCodeWidth, barBounds.Width);
				barCodeHeight = CorrectBarcodeHeigth(barCodeHeight, barBounds.Height);
			}
		}
		protected float CorrectBarcodeWidth(float barCodeWidth, float brickWidth) {
			return brickWidth;
		}
		protected float CorrectBarcodeHeigth(float barCodeHeight, float brickHeight) {
			return brickHeight;
		}
		protected virtual double CalcAutoModuleX(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			float barCodeWidth = data.ShowText ? clientBounds.Width - GetLeftSpacing(data, gr) - GetRightSpacing(data, gr) : clientBounds.Width;
			return (double)barCodeWidth / (double)CalcBarCodeWidth(Pattern, 1);
		}
		protected virtual double CalcAutoModuleY(IBarCodeData data, RectangleF clientBounds, IGraphicsBase gr) {
			float barCodeHeight = data.ShowText ? clientBounds.Height - MeasureTextHeight(DisplayText, clientBounds.Width, data, gr) : clientBounds.Height;
			return (double)barCodeHeight / (double)CalcBarCodeHeight(Pattern, 1);
		}
		static float GetTransformRotateAngle(IBarCodeData data) {
			switch(data.Orientation) {
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.Normal:
					return 0;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.UpsideDown:
					return 180;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateLeft:
					return 270;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateRight:
					return 90;
				default:
					return 0;
			}
		}
		static SizeF GetTransformTranslateOffset(RectangleF bounds, IBarCodeData data) {
			switch(data.Orientation) {
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.Normal:
					return SizeF.Empty;
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.UpsideDown:
					return new SizeF(2 * bounds.X + bounds.Width, 2 * bounds.Y + bounds.Height);
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateLeft:
					return new SizeF(bounds.X - bounds.Y, bounds.X + bounds.Y + bounds.Height);
				case DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateRight:
					return new SizeF(bounds.X + bounds.Y + bounds.Width, bounds.Y - bounds.X);
				default:
					return SizeF.Empty;
			}
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	public class BarCodeDrawingViewInfo {
		public BarCodeDrawingViewInfo(RectangleF bounds, IBarCodeData data) {
			this.Data = data;
			this.Bounds = bounds;
		}
		public IBarCodeData Data { get; private set; }
		public RectangleF Bounds { get; private set; }
		public RectangleF BarBounds { get; set; }
		public RectangleF TextBounds { get; set; }
		public float XModule { get; set; }
		public float YModule { get; set; }
		public string ErrorText { get; set; }
		public SizeF BestSize { get; set; }
		public bool Calculate(BarCodeGeneratorBase generator, IGraphicsBase gr) {
			return generator.CalculateDrawingViewInfo(this, gr);
		}
		public static void SetConvertSpacingUnits(BarCodeGeneratorBase generator, bool value) {
			EAN13Generator ean13 = generator as EAN13Generator;
			if (ean13 != null)
				ean13.ConvertSpacingUnits = value;
		}
		public static void SetForceEnoughSpace(BarCodeGeneratorBase generator, bool value) {
			generator.ForceEnoughSpace = value;
		}
		public static void SetDoNotClip(BarCodeGeneratorBase generator, bool value) {
			generator.DoNotClip = value;
		}
	}
}
