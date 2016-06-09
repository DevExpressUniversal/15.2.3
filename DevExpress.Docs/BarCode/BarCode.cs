#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.IO;
using DevExpress.Utils;
using DevExpress.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraPrinting.BarCode;
using InternalBarCode = DevExpress.XtraPrinting.BarCode;
using DevExpress.BarCodes.Internal;
using System.ComponentModel;
#if SL
using System.Windows.Media;
#else
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Drawing2D;
using DevExpress.Office.Utils;
#endif
namespace DevExpress.BarCodes {
	#region Symbology
	public enum Symbology {
		Codabar = BarCodeSymbology.Codabar,		 
		Industrial2of5 = BarCodeSymbology.Industrial2of5,  
		Interleaved2of5 = BarCodeSymbology.Interleaved2of5,
		Code39 = BarCodeSymbology.Code39,		  
		Code39Extended = BarCodeSymbology.Code39Extended,
		Code93 = BarCodeSymbology.Code93,		  
		Code93Extended = BarCodeSymbology.Code93Extended,
		Code128 = BarCodeSymbology.Code128,		 
		Code11 = BarCodeSymbology.Code11,		  
		CodeMSI = BarCodeSymbology.CodeMSI,
		PostNet = BarCodeSymbology.PostNet,
		EAN13 = BarCodeSymbology.EAN13,
		UPCA = BarCodeSymbology.UPCA,
		EAN8 = BarCodeSymbology.EAN8,
		EAN128 = BarCodeSymbology.EAN128,
		UPCSupplemental2 = BarCodeSymbology.UPCSupplemental2,
		UPCSupplemental5 = BarCodeSymbology.UPCSupplemental5,
		UPCE0 = BarCodeSymbology.UPCE0,
		UPCE1 = BarCodeSymbology.UPCE1,
		Matrix2of5 = BarCodeSymbology.Matrix2of5,
		PDF417 = BarCodeSymbology.PDF417,
		DataMatrix = BarCodeSymbology.DataMatrix,
		QRCode = BarCodeSymbology.QRCode,
		IntelligentMail = BarCodeSymbology.IntelligentMail, 
		DataMatrixGS1 = BarCodeSymbology.DataMatrixGS1,
		DataBar = BarCodeSymbology.DataBar,
		ITF14 = BarCodeSymbology.ITF14
	}
	#endregion
	#region DataBarType
	public enum DataBarType {
		Expanded = InternalBarCode.DataBarType.Expanded,
		ExpandedStacked = InternalBarCode.DataBarType.ExpandedStacked,
		Limited = InternalBarCode.DataBarType.Limited,
		Omnidirectional = InternalBarCode.DataBarType.Omnidirectional,
		Stacked = InternalBarCode.DataBarType.Stacked,
		StackedOmnidirectional = InternalBarCode.DataBarType.StackedOmnidirectional,
		Truncated = InternalBarCode.DataBarType.Truncated
	}
	#endregion
	#region DataMatrixCompactionMode
	public enum DataMatrixCompactionMode {
		ASCII = InternalBarCode.DataMatrixCompactionMode.ASCII,
		C40 = InternalBarCode.DataMatrixCompactionMode.C40,
		Text = InternalBarCode.DataMatrixCompactionMode.Text,
		X12 = InternalBarCode.DataMatrixCompactionMode.X12,
		Edifact = InternalBarCode.DataMatrixCompactionMode.Edifact,
		Binary = InternalBarCode.DataMatrixCompactionMode.Binary
	}
	#endregion
	#region DataMatrixSize
	public enum DataMatrixSize {
		MatrixAuto = InternalBarCode.DataMatrixSize.MatrixAuto,
		Matrix10x10 = InternalBarCode.DataMatrixSize.Matrix10x10,
		Matrix12x12 = InternalBarCode.DataMatrixSize.Matrix12x12,
		Matrix14x14 = InternalBarCode.DataMatrixSize.Matrix14x14,
		Matrix16x16 = InternalBarCode.DataMatrixSize.Matrix16x16,
		Matrix18x18 = InternalBarCode.DataMatrixSize.Matrix18x18,
		Matrix20x20 = InternalBarCode.DataMatrixSize.Matrix20x20,
		Matrix22x22 = InternalBarCode.DataMatrixSize.Matrix22x22,
		Matrix24x24 = InternalBarCode.DataMatrixSize.Matrix24x24,
		Matrix26x26 = InternalBarCode.DataMatrixSize.Matrix26x26,
		Matrix32x32 = InternalBarCode.DataMatrixSize.Matrix32x32,
		Matrix36x36 = InternalBarCode.DataMatrixSize.Matrix36x36,
		Matrix40x40 = InternalBarCode.DataMatrixSize.Matrix40x40,
		Matrix44x44 = InternalBarCode.DataMatrixSize.Matrix44x44,
		Matrix48x48 = InternalBarCode.DataMatrixSize.Matrix48x48,
		Matrix52x52 = InternalBarCode.DataMatrixSize.Matrix52x52,
		Matrix64x64 = InternalBarCode.DataMatrixSize.Matrix64x64,
		Matrix72x72 = InternalBarCode.DataMatrixSize.Matrix72x72,
		Matrix80x80 = InternalBarCode.DataMatrixSize.Matrix80x80,
		Matrix88x88 = InternalBarCode.DataMatrixSize.Matrix88x88,
		Matrix96x96 = InternalBarCode.DataMatrixSize.Matrix96x96,
		Matrix104x104 = InternalBarCode.DataMatrixSize.Matrix104x104,
		Matrix120x120 = InternalBarCode.DataMatrixSize.Matrix120x120,
		Matrix132x132 = InternalBarCode.DataMatrixSize.Matrix132x132,
		Matrix144x144 = InternalBarCode.DataMatrixSize.Matrix144x144,
		Matrix8x18 = InternalBarCode.DataMatrixSize.Matrix8x18,
		Matrix8x32 = InternalBarCode.DataMatrixSize.Matrix8x32,
		Matrix12x26 = InternalBarCode.DataMatrixSize.Matrix12x26,
		Matrix12x36 = InternalBarCode.DataMatrixSize.Matrix12x36,
		Matrix16x36 = InternalBarCode.DataMatrixSize.Matrix16x36,
		Matrix16x48 = InternalBarCode.DataMatrixSize.Matrix16x48,
	}
	#endregion
	#region QRCodeCompactionMode
	public enum QRCodeCompactionMode {
		Numeric = InternalBarCode.QRCodeCompactionMode.Numeric,
		AlphaNumeric = InternalBarCode.QRCodeCompactionMode.AlphaNumeric,
		Byte = InternalBarCode.QRCodeCompactionMode.Byte
	}
	#endregion
	#region PDF417CompactionMode
	public enum PDF417CompactionMode {
		Binary = InternalBarCode.PDF417CompactionMode.Binary,
		Text = InternalBarCode.PDF417CompactionMode.Text
	}
	#endregion
	#region Code128CharacterSet
	public enum Code128CharacterSet {
		CharsetA = Code128Charset.CharsetA,
		CharsetB = Code128Charset.CharsetB,
		CharsetC = Code128Charset.CharsetC,
		CharsetAuto = Code128Charset.CharsetAuto
	}
	#endregion
	#region CodabarStartStopPair
	public enum CodabarStartStopPair {
		None = InternalBarCode.CodabarStartStopPair.None,
		AT = InternalBarCode.CodabarStartStopPair.AT,
		BN = InternalBarCode.CodabarStartStopPair.BN,
		CStar = InternalBarCode.CodabarStartStopPair.CStar,
		DE = InternalBarCode.CodabarStartStopPair.DE
	}
	#endregion
	#region PDF417ErrorLevel
	public enum PDF417ErrorLevel {
		Level0 = ErrorCorrectionLevel.Level0,
		Level1 = ErrorCorrectionLevel.Level1,
		Level2 = ErrorCorrectionLevel.Level2,
		Level3 = ErrorCorrectionLevel.Level3,
		Level4 = ErrorCorrectionLevel.Level4,
		Level5 = ErrorCorrectionLevel.Level5,
		Level6 = ErrorCorrectionLevel.Level6,
		Level7 = ErrorCorrectionLevel.Level7,
		Level8 = ErrorCorrectionLevel.Level8
	}
	#endregion
	#region QRCodeErrorLevel
	public enum QRCodeErrorLevel {
		M = QRCodeErrorCorrectionLevel.M,
		L = QRCodeErrorCorrectionLevel.L,
		H = QRCodeErrorCorrectionLevel.H,
		Q = QRCodeErrorCorrectionLevel.Q
	}
	#endregion
	#region BarCodePadding
	public class BarCodePadding {
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodePaddingLeft")]
#endif
		public float Left { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodePaddingRight")]
#endif
		public float Right { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodePaddingTop")]
#endif
		public float Top { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodePaddingBottom")]
#endif
		public float Bottom { get; set; }
	}
	#endregion
	#region BarCodeMargins
	public class BarCodeMargins {
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeMarginsLeft")]
#endif
		public float Left { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeMarginsRight")]
#endif
		public float Right { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeMarginsTop")]
#endif
		public float Top { get; set; }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeMarginsBottom")]
#endif
		public float Bottom { get; set; }
	}
	#endregion
	#region BarCodeCaption
	public class BarCodeCaption {
		#region Fields
		readonly BarCode barCode;
		string text = String.Empty;
		StringAlignment horizontalAlignment;
		Color foreColor;
		Font font;
		StringFormat stringFormat;
		BarCodePadding paddings;
		#endregion
		internal BarCodeCaption(BarCode barCode) {
			Guard.ArgumentNotNull(barCode, "barCode");
			this.barCode = barCode;
			this.font = barCode.CodeTextFont;
			this.paddings = new BarCodePadding();
		}
		#region Properties
		#region Text
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCaptionText")]
#endif
		public string Text {
			get { return text; }
			set {
				if (Text == value)
					return;
				text = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region Font
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCaptionFont")]
#endif
		public Font Font {
			get { return font; }
			set {
				if (Font == value)
					return;
				font = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region HorizontalAlignment
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCaptionHorizontalAlignment")]
#endif
		public StringAlignment HorizontalAlignment {
			get { return horizontalAlignment; }
			set {
				if (horizontalAlignment == value)
					return;
				horizontalAlignment = value;
				this.stringFormat = null;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ForeColor
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCaptionForeColor")]
#endif
		public Color ForeColor {
			get { return DXColor.IsEmpty(foreColor) ? barCode.ForeColor : foreColor; }
			set {
				if (ForeColor == value)
					return;
				foreColor = value;
				ResetBarCodeImage();
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCaptionPaddings")]
#endif
		public BarCodePadding Paddings { get { return paddings; } }
		#endregion
		internal StringFormat StringFormat {
			get {
				if (stringFormat == null) {
					stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
					stringFormat.Alignment = HorizontalAlignment;
					stringFormat.FormatFlags &= ~(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
				}
				return stringFormat;
			}
		}
		#endregion
		void ResetBarCodeImage() {
			barCode.ResetBarCodeImage();
		}
	}
	#endregion
	#region BarCodeOptions
	public class BarCodeOptions {
		#region Fields
		BarCodeGeneratorBase generator;
		IntelligentMailOptions intelligentMail = new IntelligentMailOptions();
		DataMatrixGS1Options dataMatrixGS1 = new DataMatrixGS1Options();
		DataMatrixOptions dataMatrix = new DataMatrixOptions();
		QRCodeOptions qRCode = new QRCodeOptions();
		PDF417Options pdf417 = new PDF417Options();
		Code128Options code128 = new Code128Options();
		EAN128Options eAN128 = new EAN128Options();
		CodabarOptions codabar = new CodabarOptions();
		Code39ExtendedOptions code39Extended = new Code39ExtendedOptions();
		Code39Options code39 = new Code39Options();
		Industrial2of5Options industrial2of5 = new Industrial2of5Options();
		Interleaved2of5Options interleaved2of5 = new Interleaved2of5Options();
		Matrix2of5Options matrix2of5 = new Matrix2of5Options();
		Code11Options code11 = new Code11Options();
		Code93Options code93 = new Code93Options();
		Code93ExtendedOptions code93Extended = new Code93ExtendedOptions();
		CodeMSIOptions codeMSI = new CodeMSIOptions();
		EAN13Options ean13 = new EAN13Options();
		EAN8Options ean8 = new EAN8Options();
		PostNetOptions postNet = new PostNetOptions();
		UPCAOptions upcA = new UPCAOptions();
		UPCE0Options upcE0 = new UPCE0Options();
		UPCE1Options upcE1 = new UPCE1Options();
		UPCSupplemental2Options upcSupplemental2 = new UPCSupplemental2Options();
		UPCSupplemental5Options upcSupplemental5 = new UPCSupplemental5Options();
		DataBarOptions dataBar = new DataBarOptions();
		#endregion
		internal void SetGenerator(BarCodeGeneratorBase generator) {
			this.generator = generator;
		}
		#region Properties
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsIntelligentMail")]
#endif
		public IntelligentMailOptions IntelligentMail { get { return intelligentMail; } set { intelligentMail = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsDataMatrixGS1")]
#endif
		public DataMatrixGS1Options DataMatrixGS1 { get { return dataMatrixGS1; } set { dataMatrixGS1 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsDataMatrix")]
#endif
		public DataMatrixOptions DataMatrix { get { return dataMatrix; } set { dataMatrix = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsQRCode")]
#endif
		public QRCodeOptions QRCode { get { return qRCode; } set { qRCode = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsPDF417")]
#endif
		public PDF417Options PDF417 { get { pdf417.SetGenerator(generator); return pdf417; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode128")]
#endif
		public Code128Options Code128 { get { return code128; } set { code128 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsEAN128")]
#endif
		public EAN128Options EAN128 { get { return eAN128; } set { eAN128 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCodabar")]
#endif
		public CodabarOptions Codabar { get { return codabar; } set { codabar = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode39Extended")]
#endif
		public Code39ExtendedOptions Code39Extended { get { return code39Extended; } set { code39Extended = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode39")]
#endif
		public Code39Options Code39 { get { return code39; } set { code39 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsIndustrial2of5")]
#endif
		public Industrial2of5Options Industrial2of5 { get { return industrial2of5; } set { industrial2of5 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsInterleaved2of5")]
#endif
		public Interleaved2of5Options Interleaved2of5 { get { return interleaved2of5; } set { interleaved2of5 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsMatrix2of5")]
#endif
		public Matrix2of5Options Matrix2of5 { get { return matrix2of5; } set { matrix2of5 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode11")]
#endif
		public Code11Options Code11 { get { return code11; } set { code11 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode93")]
#endif
		public Code93Options Code93 { get { return code93; } set { code93 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCode93Extended")]
#endif
		public Code93ExtendedOptions Code93Extended { get { return code93Extended; } set { code93Extended = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsCodeMSI")]
#endif
		public CodeMSIOptions CodeMSI { get { return codeMSI; } set { codeMSI = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsEAN13")]
#endif
		public EAN13Options EAN13 { get { return ean13; } set { ean13 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsEAN8")]
#endif
		public EAN8Options EAN8 { get { return ean8; } set { ean8 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsPostNet")]
#endif
		public PostNetOptions PostNet { get { return postNet; } set { postNet = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsUPCA")]
#endif
		public UPCAOptions UPCA { get { return upcA; } set { upcA = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsUPCE0")]
#endif
		public UPCE0Options UPCE0 { get { return upcE0; } set { upcE0 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsUPCE1")]
#endif
		public UPCE1Options UPCE1 { get { return upcE1; } set { upcE1 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsUPCSupplemental2")]
#endif
		public UPCSupplemental2Options UPCSupplemental2 { get { return upcSupplemental2; } set { upcSupplemental2 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsUPCSupplemental5")]
#endif
		public UPCSupplemental5Options UPCSupplemental5 { get { return upcSupplemental5; } set { upcSupplemental5 = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsDataBar")]
#endif
		public DataBarOptions DataBar { get { return dataBar; } set { dataBar = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptionsITF14")]
#endif
		public Interleaved2of5Options ITF14 { get { return interleaved2of5; } set { interleaved2of5 = value; } }
		#endregion
	}
	public abstract class BarCodeGeneratorOptions {
		bool showCodeText = true;
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeGeneratorOptionsShowCodeText")]
#endif
		public bool ShowCodeText { get { return showCodeText; } set { showCodeText = value; } }
	}
	public class Code11Options : BarCodeGeneratorOptions {
	}
	public class Code93Options : BarCodeGeneratorOptions {
		bool calcCheckSum = true;
#if !SL
	[DevExpressDocsLocalizedDescription("Code93OptionsCalcCheckSum")]
#endif
		public bool CalcCheckSum { get { return calcCheckSum; } set { calcCheckSum = value; } }
	}
	public class Code93ExtendedOptions : Code93Options {
	}
	public class CodeMSIOptions : BarCodeGeneratorOptions {
	}
	public class EAN13Options : BarCodeGeneratorOptions {
	}
	public class EAN8Options : BarCodeGeneratorOptions {
	}
	public class PostNetOptions : BarCodeGeneratorOptions {
	}
	public class UPCAOptions : EAN13Options {
	}
	public class UPCE0Options : UPCAOptions {
	}
	public class UPCE1Options : UPCAOptions {
	}
	public class UPCSupplemental2Options : BarCodeGeneratorOptions {
	}
	public class UPCSupplemental5Options : BarCodeGeneratorOptions {
	}
	public class Interleaved2of5Options : BarCodeGeneratorOptions {
		float wideNarrowRatio = 3f;
		bool calcCheckSum = true;
#if !SL
	[DevExpressDocsLocalizedDescription("Interleaved2of5OptionsWideNarrowRatio")]
#endif
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.5f)
					value = 2.5f;
				wideNarrowRatio = value;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("Interleaved2of5OptionsCalcCheckSum")]
#endif
		public bool CalcCheckSum { get { return calcCheckSum; } set { calcCheckSum = value; } }
	}
	public class Matrix2of5Options : Industrial2of5Options {
	}	
	public class Industrial2of5Options : BarCodeGeneratorOptions {
		float wideNarrowRatio = 2.5f;
		bool calcCheckSum = true;
#if !SL
	[DevExpressDocsLocalizedDescription("Industrial2of5OptionsWideNarrowRatio")]
#endif
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.5f)
					value = 2.5f;
				wideNarrowRatio = value;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("Industrial2of5OptionsCalcCheckSum")]
#endif
		public bool CalcCheckSum { get { return calcCheckSum; } set { calcCheckSum = value; } }
	}
	public class Code39Options : BarCodeGeneratorOptions {
		float wideNarrowRatio = 3f;
		bool calcCheckSum = true;
#if !SL
	[DevExpressDocsLocalizedDescription("Code39OptionsWideNarrowRatio")]
#endif
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.2f)
					value = 2.2f;
				if (value > 3.0f)
					value = 3.0f;
				wideNarrowRatio = value;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("Code39OptionsCalcCheckSum")]
#endif
		public bool CalcCheckSum { get { return calcCheckSum; } set { calcCheckSum = value; } }
	}
	public class Code39ExtendedOptions : Code39Options {
	}
	public class CodabarOptions : BarCodeGeneratorOptions {
		CodabarStartStopPair startStopPair = CodabarStartStopPair.AT;
		float wideNarrowRatio = 2f;
#if !SL
	[DevExpressDocsLocalizedDescription("CodabarOptionsStartStopPair")]
#endif
		public CodabarStartStopPair StartStopPair { get { return startStopPair; } set { startStopPair = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("CodabarOptionsWideNarrowRatio")]
#endif
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.0f)
					value = 2.0f;
				if (value > 3.0f)
					value = 3.0f;
				wideNarrowRatio = value;
			}
		}
	}
	public class EAN128Options : BarCodeGeneratorOptions {
		Code128CharacterSet charset = Code128CharacterSet.CharsetAuto;
		string fNC1Substitut = "#";
#if !SL
	[DevExpressDocsLocalizedDescription("EAN128OptionsCharset")]
#endif
		public Code128CharacterSet Charset { get { return charset; } set { charset = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("EAN128OptionsFNC1Substitut")]
#endif
		public string FNC1Substitut { get { return fNC1Substitut; } set { fNC1Substitut = value; } }
	}
	public class Code128Options : BarCodeGeneratorOptions {
		Code128CharacterSet charset = Code128CharacterSet.CharsetAuto;
#if !SL
	[DevExpressDocsLocalizedDescription("Code128OptionsCharset")]
#endif
		public Code128CharacterSet Charset { get { return charset; } set { charset = value; } }
	}
	public class IntelligentMailOptions : BarCodeGeneratorOptions {
		public IntelligentMailOptions() {
			ShowCodeText = false;
		}
	}
	public class DataMatrixGS1Options : DataMatrixOptions {
		string fNC1Substitut = "#";
#if !SL
	[DevExpressDocsLocalizedDescription("DataMatrixGS1OptionsFNC1Substitut")]
#endif
		public string FNC1Substitut { get { return fNC1Substitut; } set { fNC1Substitut = value; } }
	}
	public class DataMatrixOptions : BarCodeGeneratorOptions {
		DataMatrixCompactionMode compactionMode = DataMatrixCompactionMode.ASCII;
		DataMatrixSize matrixSize = DataMatrixSize.MatrixAuto;
		public DataMatrixOptions() {
			ShowCodeText = false;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("DataMatrixOptionsCompactionMode")]
#endif
		public DataMatrixCompactionMode CompactionMode { get { return compactionMode; } set { compactionMode = value; }  }
#if !SL
	[DevExpressDocsLocalizedDescription("DataMatrixOptionsMatrixSize")]
#endif
		public DataMatrixSize MatrixSize { get { return matrixSize; } set { matrixSize = value; } }
	}
	public class QRCodeOptions : BarCodeGeneratorOptions {
		QRCodeCompactionMode compactionMode = QRCodeCompactionMode.AlphaNumeric;
		QRCodeErrorLevel errorLevel = QRCodeErrorLevel.L;
		public QRCodeOptions() {
			ShowCodeText = false;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("QRCodeOptionsCompactionMode")]
#endif
		public QRCodeCompactionMode CompactionMode { get { return compactionMode; } set { compactionMode = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("QRCodeOptionsErrorLevel")]
#endif
		public QRCodeErrorLevel ErrorLevel { get { return errorLevel; } set { errorLevel = value; } }
	}
	public class PDF417Options : BarCodeGeneratorOptions {
		BarCodeGeneratorBase generator;
		PDF417CompactionMode compactionMode = PDF417CompactionMode.Text;
		PDF417ErrorLevel errorLevel = PDF417ErrorLevel.Level2;
		bool truncateSymbol = false;
		int columns = 1;
		int rows = 3;
		public PDF417Options() {
			ShowCodeText = false;
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PDF417OptionsCompactionMode")]
#endif
		public PDF417CompactionMode CompactionMode { get { return compactionMode; } set { compactionMode = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("PDF417OptionsErrorLevel")]
#endif
		public PDF417ErrorLevel ErrorLevel { get { return errorLevel; } set { errorLevel = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("PDF417OptionsTruncateSymbol")]
#endif
		public bool TruncateSymbol { get { return truncateSymbol; } set { truncateSymbol = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("PDF417OptionsColumns")]
#endif
		public int Columns { 
			get {
				if (generator is PDF417Generator)
					columns = ((PDF417Generator)generator).Columns;
				return columns; 
			} 
			set {
				if (value < 1)
					return;
				columns = value;
				if (generator is PDF417Generator)
					((PDF417Generator)generator).Columns = columns;
			} 
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PDF417OptionsRows")]
#endif
		public int Rows { 
			get {
				if (generator is PDF417Generator)
					rows = ((PDF417Generator)generator).Rows;
				return rows; 
			}
			set {
				if (value < 1)
					return;
				rows = value;
				if (generator is PDF417Generator)
					((PDF417Generator)generator).Rows = rows;
			} 
		}
		internal void SetGenerator(BarCodeGeneratorBase generator) {
			this.generator = generator;
		}
	}
	public class DataBarOptions : BarCodeGeneratorOptions {
		string fNC1Substitute = "#";
		DataBarType type = DataBarType.ExpandedStacked;
		int segmentsInRow = 4;
#if !SL
	[DevExpressDocsLocalizedDescription("DataBarOptionsType")]
#endif
		public DataBarType Type { get { return type; } set { type = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("DataBarOptionsFNC1Substitute")]
#endif
		public string FNC1Substitute { get { return fNC1Substitute; } set { fNC1Substitute = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("DataBarOptionsSegmentsInRow")]
#endif
		public int SegmentsInRow { get { return segmentsInRow; } set { segmentsInRow = value; } }
	}
	#endregion
	public partial class BarCode : IDisposable {
		#region Fields
		const int defaultDpi = 300;
		Symbology symbology = Symbology.EAN13;
		BarCodeGeneratorBase generator;
		SizeF imageSize = new SizeF(75, 25);
		Color foreColor = DXColor.Black;
		Color backColor = DXColor.White;
		Color borderColor = DXColor.Black;
		float rotationAngle;
		float dpiX = defaultDpi;
		float dpiY = defaultDpi;
		GraphicsUnit unit = GraphicsUnit.Millimeter;
		TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias;
		bool autoSize = true;
		float barHeight = 25;
		float borderWidth;
		BarCodePadding padding = new BarCodePadding();
		BarCodeMargins margins = new BarCodeMargins();
		string codeText = "0"; 
		Font codeTextFont;
		StringAlignment codeTextHorizontalAlignment = StringAlignment.Center;
		StringAlignment codeTextVerticalAlignment = StringAlignment.Far;
		BarCodeCaption topCaption;
		BarCodeCaption bottomCaption;
		Bitmap barCodeImage;
		byte[] codeBinaryData;
		BarCodeOptions options = new BarCodeOptions();
		double module = 1;
		#endregion
		public BarCode() {
			this.codeTextFont = new Font("Times New Roman", 24);
			this.topCaption = new BarCodeCaption(this);
			this.bottomCaption = new BarCodeCaption(this);
			UpdateGenerator();
		}
		#region Properties
		#region Symbology
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeSymbology")]
#endif
		public Symbology Symbology {
			get { return symbology; }
			set {
				if (symbology == value)
					return;
				this.symbology = value;
				this.generator = null;
				UpdateGenerator();
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ImageSize
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeImageSize")]
#endif
		public SizeF ImageSize {
			get { return imageSize; }
			set {
				if (ImageSize == value)
					return;
				imageSize = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ImageWidth
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeImageWidth")]
#endif
		public float ImageWidth {
			get { return imageSize.Width; }
			set {
				if (ImageWidth == value)
					return;
				imageSize.Width = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ImageHeight
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeImageHeight")]
#endif
		public float ImageHeight {
			get { return imageSize.Height; }
			set {
				if (ImageHeight == value)
					return;
				imageSize.Height = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region BarHeight
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBarHeight")]
#endif
		public float BarHeight {
			get { return barHeight; }
			set {
				if (BarHeight == value)
					return;
				barHeight = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ForeColor
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeForeColor")]
#endif
		public Color ForeColor {
			get { return foreColor; }
			set {
				if (ForeColor == value)
					return;
				foreColor = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region BackColor
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBackColor")]
#endif
		public Color BackColor {
			get { return backColor; }
			set {
				if (BackColor == value)
					return;
				backColor = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region BorderColor
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBorderColor")]
#endif
		public Color BorderColor {
			get { return borderColor; }
			set {
				if (BorderColor == value)
					return;
				borderColor = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region BorderWidth
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBorderWidth")]
#endif
		public float BorderWidth {
			get { return borderWidth; }
			set {
				if (BorderWidth == value)
					return;
				borderWidth = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region CodeText
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCodeText")]
#endif
		public string CodeText {
			get { return codeText; }
			set {
				if (CodeText == value)
					return;
				codeText = value;
				SetTextCompactionMode();
				ResetBarCodeImage();
			}
		}
		#endregion
		#region CodeTextFont
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCodeTextFont")]
#endif
		public Font CodeTextFont {
			get {
				return codeTextFont;
			}
			set {
				if (CodeTextFont == value)
					return;
				codeTextFont = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region CodeTextHorizontalAlignment
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCodeTextHorizontalAlignment")]
#endif
		public StringAlignment CodeTextHorizontalAlignment {
			get { return codeTextHorizontalAlignment; }
			set {
				if (CodeTextHorizontalAlignment == value)
					return;
				codeTextHorizontalAlignment = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region CodeTextVerticalAlignment
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCodeTextVerticalAlignment")]
#endif
		public StringAlignment CodeTextVerticalAlignment {
			get { return codeTextVerticalAlignment; }
			set {
				if (CodeTextVerticalAlignment == value)
					return;
				codeTextVerticalAlignment = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region RotationAngle
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeRotationAngle")]
#endif
		public float RotationAngle {
			get { return rotationAngle; }
			set {
				if (RotationAngle == value)
					return;
				rotationAngle = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region Dpi
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeDpi")]
#endif
		public float Dpi {
			get { return dpiX; }
			set {
				if (Dpi == value)
					return;
				dpiX = value;
				dpiY = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region DpiX
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeDpiX")]
#endif
		public float DpiX {
			get { return dpiX; }
			set {
				if (DpiX == value)
					return;
				dpiX = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region DpiY
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeDpiY")]
#endif
		public float DpiY {
			get { return dpiY; }
			set {
				if (DpiY == value)
					return;
				dpiY = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region Unit
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeUnit")]
#endif
		public GraphicsUnit Unit {
			get { return unit; }
			set {
				if (Unit == value)
					return;
				unit = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region TextRenderingHint
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeTextRenderingHint")]
#endif
		public TextRenderingHint TextRenderingHint {
			get { return textRenderingHint; }
			set {
				if (TextRenderingHint == value)
					return;
				textRenderingHint = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region AutoSize
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeAutoSize")]
#endif
		public bool AutoSize {
			get { return autoSize; }
			set {
				if (AutoSize == value)
					return;
				autoSize = value;
				ResetBarCodeImage();
			}
		}
		#endregion
		#region BarCodeImage
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBarCodeImage")]
#endif
		public Bitmap BarCodeImage {
			get {
				return CreateImage();
			}
		}
		internal void ResetBarCodeImage() {
			if (barCodeImage != null) {
				barCodeImage.Dispose();
				barCodeImage = null;
			}
		}
		#endregion
		#region CodeBinaryData
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeCodeBinaryData")]
#endif
		public byte[] CodeBinaryData {
			get { return codeBinaryData; }
			set {
				if (CodeBinaryData == value)
					return;
				codeBinaryData = value;
				UpdateGenerator();
				SetBinaryCompactionMode();
				ResetBarCodeImage();
			}
		}
		#endregion
		#region ShowText
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeShowText")]
#endif
		public bool ShowText {
			get {
				switch (symbology) {
					case BarCodes.Symbology.DataMatrix :
						return options.DataMatrix.ShowCodeText;
					case BarCodes.Symbology.DataMatrixGS1 :
						return options.DataMatrixGS1.ShowCodeText;
					case BarCodes.Symbology.Codabar :
						return options.Codabar.ShowCodeText;
					case BarCodes.Symbology.Code11 :
						return options.Code11.ShowCodeText;
					case BarCodes.Symbology.Code128 :
						return options.Code128.ShowCodeText;
					case BarCodes.Symbology.Code39 :
						return options.Code39.ShowCodeText;
					case BarCodes.Symbology.Code39Extended :
						return options.Code39Extended.ShowCodeText;
					case BarCodes.Symbology.Code93 :
						return options.Code93.ShowCodeText;
					case BarCodes.Symbology.Code93Extended :
						return options.Code93Extended.ShowCodeText;
					case BarCodes.Symbology.CodeMSI :
						return options.CodeMSI.ShowCodeText;
					case BarCodes.Symbology.EAN128 :
						return options.EAN128.ShowCodeText;
					case BarCodes.Symbology.EAN13 :
						return options.EAN13.ShowCodeText;
					case BarCodes.Symbology.EAN8 :
						return options.EAN8.ShowCodeText;
					case BarCodes.Symbology.Industrial2of5 :
						return options.Industrial2of5.ShowCodeText;
					case BarCodes.Symbology.IntelligentMail :
						return options.IntelligentMail.ShowCodeText;
					case BarCodes.Symbology.Interleaved2of5 :
						return options.Interleaved2of5.ShowCodeText;
					case BarCodes.Symbology.Matrix2of5 :
						return options.Matrix2of5.ShowCodeText;
					case BarCodes.Symbology.PDF417 :
						return options.PDF417.ShowCodeText;
					case BarCodes.Symbology.PostNet :
						return options.PostNet.ShowCodeText;
					case BarCodes.Symbology.QRCode :
						return options.QRCode.ShowCodeText;
					case BarCodes.Symbology.UPCA :
						return options.UPCA.ShowCodeText;
					case BarCodes.Symbology.UPCE0 :
						return options.UPCE0.ShowCodeText;
					case BarCodes.Symbology.UPCE1 :
						return options.UPCE1.ShowCodeText;
					case BarCodes.Symbology.UPCSupplemental2 :
						return options.UPCSupplemental2.ShowCodeText;
					case BarCodes.Symbology.UPCSupplemental5 :
						return options.UPCSupplemental5.ShowCodeText;
					case BarCodes.Symbology.DataBar:
						return options.DataBar.ShowCodeText;
					case BarCodes.Symbology.ITF14:
						return options.ITF14.ShowCodeText;
					default :
						return true;
				}
			}
		}
		#endregion
		#region Module
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeModule")]
#endif
		public double Module { 
			get { return module; } 
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException();
				module = value; 
			} 
		}
		#endregion
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeOptions")]
#endif
		public BarCodeOptions Options { 
			get {
				options.SetGenerator(generator);
				return options; 
			} 
		}
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeTopCaption")]
#endif
		public BarCodeCaption TopCaption { get { return topCaption; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeBottomCaption")]
#endif
		public BarCodeCaption BottomCaption { get { return bottomCaption; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodePaddings")]
#endif
		public BarCodePadding Paddings { get { return padding; } }
#if !SL
	[DevExpressDocsLocalizedDescription("BarCodeMargins")]
#endif
		public BarCodeMargins Margins { get { return margins; } }
		public string Code {
			get {
				UpdateGeneratorOptions();
				BarCodeViewInfo viewInfo = new BarCodeViewInfo(this);
				return generator.GetFinalText(viewInfo);
			}
		}
		protected internal BarCodeGeneratorBase Generator { get { return generator; } }
		#endregion
		#region UpdateGeneratorOptions
		internal void UpdateGeneratorOptions() {
			EAN128Generator generatorEAN128 = generator as EAN128Generator;
			if (generatorEAN128 != null) {
				generatorEAN128.CharacterSet = (Code128Charset)options.EAN128.Charset;
				generatorEAN128.FNC1Substitute = options.EAN128.FNC1Substitut;
				return;
			}
			Code128Generator generatorCode128 = generator as Code128Generator;
			if (generatorCode128 != null) {
				generatorCode128.CharacterSet = (Code128Charset)options.Code128.Charset;
				return;
			}
			CodabarGenerator generatorCodabar = generator as CodabarGenerator;
			if (generatorCodabar != null) {
				CodabarOptions codabarOptions = options.Codabar;
				generatorCodabar.StartStopPair = (InternalBarCode.CodabarStartStopPair)codabarOptions.StartStopPair;
				generatorCodabar.WideNarrowRatio = codabarOptions.WideNarrowRatio;
				return;
			}
			Code39ExtendedGenerator generatorCode39Extended = generator as Code39ExtendedGenerator;
			if (generatorCode39Extended != null) {
				generatorCode39Extended.WideNarrowRatio = options.Code39Extended.WideNarrowRatio;
				generatorCode39Extended.CalcCheckSum = options.Code39Extended.CalcCheckSum;
				return;
			}
			Code39Generator generatorCode39 = generator as Code39Generator;
			if (generatorCode39 != null) {
				generatorCode39.WideNarrowRatio = options.Code39.WideNarrowRatio;
				generatorCode39.CalcCheckSum = options.Code39.CalcCheckSum;
				return;
			}
			Code93ExtendedGenerator generatorCode93Extended = generator as Code93ExtendedGenerator;
			if (generatorCode93Extended != null) {
				generatorCode93Extended.CalcCheckSum = options.Code93Extended.CalcCheckSum;
				return;
			}
			Code93Generator generatorCode93 = generator as Code93Generator;
			if (generatorCode93 != null) {
				generatorCode93.CalcCheckSum = options.Code93.CalcCheckSum;
				return;
			}
			Matrix2of5Generator generatorMatrix2of5 = generator as Matrix2of5Generator;
			if (generatorMatrix2of5 != null) {
				generatorMatrix2of5.WideNarrowRatio = options.Matrix2of5.WideNarrowRatio;
				generatorMatrix2of5.CalcCheckSum = options.Matrix2of5.CalcCheckSum;
				return;
			}
			Industrial2of5Generator generatorIndustrial2of5 = generator as Industrial2of5Generator;
			if (generatorIndustrial2of5 != null) {
				generatorIndustrial2of5.WideNarrowRatio = options.Industrial2of5.WideNarrowRatio;
				generatorIndustrial2of5.CalcCheckSum = options.Industrial2of5.CalcCheckSum;
				return;
			}
			Interleaved2of5Generator generatorInterleaved2of5 = generator as Interleaved2of5Generator;
			if (generatorInterleaved2of5 != null) {
				generatorInterleaved2of5.WideNarrowRatio = options.Interleaved2of5.WideNarrowRatio;
				generatorInterleaved2of5.CalcCheckSum = options.Interleaved2of5.CalcCheckSum;
				return;
			}
			DataBarGenerator generatorDataBar = generator as DataBarGenerator;
			if(generatorDataBar != null) {
				DataBarOptions optionsDataBar = options.DataBar;
				generatorDataBar.FNC1Substitute = optionsDataBar.FNC1Substitute;
				generatorDataBar.SegmentsInRow = optionsDataBar.SegmentsInRow;
				generatorDataBar.Type = (InternalBarCode.DataBarType)optionsDataBar.Type;
				return;
			}
			ITF14Generator generatorITF14 = generator as ITF14Generator;
			if(generatorITF14 != null) {
				Interleaved2of5Options optionsITF14 = options.Interleaved2of5;
				generatorITF14.CalcCheckSum = optionsITF14.CalcCheckSum;
				generatorITF14.WideNarrowRatio = optionsITF14.WideNarrowRatio;
				return;
			}
			BarCode2DGenerator barCode2DGenerator = generator as BarCode2DGenerator;
			if (barCode2DGenerator == null)
				return;
			barCode2DGenerator.Update(CodeText, CodeBinaryData);
			DataMatrixGS1Generator generatorDMGS1 = generator as DataMatrixGS1Generator;
			if (generatorDMGS1 != null) {
				DataMatrixGS1Options optionsDMGS1 = options.DataMatrixGS1;
				if (optionsDMGS1.CompactionMode == DataMatrixCompactionMode.Binary && CodeBinaryData == null)
					throw new ArgumentNullException("CodeBinaryData");
				generatorDMGS1.CompactionMode = (InternalBarCode.DataMatrixCompactionMode)optionsDMGS1.CompactionMode;
				generatorDMGS1.MatrixSize = (InternalBarCode.DataMatrixSize)optionsDMGS1.MatrixSize;
				generatorDMGS1.FNC1Substitute = optionsDMGS1.FNC1Substitut;
				return;
			}
			DataMatrixGenerator generatorDM = generator as DataMatrixGenerator;
			if (generatorDM != null) {
				DataMatrixCompactionMode compactionMode = options.DataMatrix.CompactionMode;
				if (compactionMode == DataMatrixCompactionMode.Binary && CodeBinaryData == null)
					throw new ArgumentNullException("CodeBinaryData");
				generatorDM.CompactionMode = (InternalBarCode.DataMatrixCompactionMode)compactionMode;
				generatorDM.MatrixSize = (InternalBarCode.DataMatrixSize)options.DataMatrix.MatrixSize;
				return;
			}
			QRCodeGenerator generatorQRCode = generator as QRCodeGenerator;
			if (generatorQRCode != null) {
				QRCodeOptions optionsQRCode = options.QRCode;
				if (optionsQRCode.CompactionMode == QRCodeCompactionMode.Byte && CodeBinaryData == null)
					throw new ArgumentNullException("CodeBinaryData");
				generatorQRCode.CompactionMode = (InternalBarCode.QRCodeCompactionMode)optionsQRCode.CompactionMode;
				generatorQRCode.ErrorCorrectionLevel = (QRCodeErrorCorrectionLevel)optionsQRCode.ErrorLevel;
				return;
			}
			PDF417Generator generatorPDF417 = generator as PDF417Generator;
			if (generatorPDF417 != null) {
				PDF417Options optionsPDF417 = options.PDF417;
				if (optionsPDF417.CompactionMode == PDF417CompactionMode.Binary && CodeBinaryData == null)
					throw new ArgumentNullException("CodeBinaryData");
				generatorPDF417.CompactionMode = (InternalBarCode.PDF417CompactionMode)optionsPDF417.CompactionMode;
				generatorPDF417.ErrorCorrectionLevel = (ErrorCorrectionLevel)optionsPDF417.ErrorLevel;
				generatorPDF417.TruncateSymbol = optionsPDF417.TruncateSymbol;
				generatorPDF417.Columns = optionsPDF417.Columns;
				generatorPDF417.Rows = optionsPDF417.Rows;
				return;
			}
		}
		#endregion
		#region UpdateGenerator
		void UpdateGenerator() {
			if (generator == null) {
				generator = BarCodeGeneratorFactory.Create((BarCodeSymbology)this.Symbology);
				BarCodeDrawingViewInfo.SetConvertSpacingUnits(generator, true);
				BarCodeDrawingViewInfo.SetForceEnoughSpace(generator, true);
			}
		}
		#endregion
		#region SetTextCompactionMode
		void SetTextCompactionMode() {
			DataMatrixGenerator generatorDM = generator as DataMatrixGenerator;
			if (generatorDM != null) {
				options.DataMatrix.CompactionMode = DataMatrixCompactionMode.ASCII;
				return;
			}
			QRCodeGenerator generatorQRCode = generator as QRCodeGenerator;
			if (generatorQRCode != null) {
				options.QRCode.CompactionMode = QRCodeCompactionMode.AlphaNumeric;
				return;
			}
			PDF417Generator generatorPDF417 = generator as PDF417Generator;
			if (generatorPDF417 != null) {
				options.PDF417.CompactionMode = PDF417CompactionMode.Text;
			}
		}
		#endregion
		#region SetBinaryCompactionMode
		void SetBinaryCompactionMode() {
			DataMatrixGenerator generatorDM = generator as DataMatrixGenerator;
			if (generatorDM != null) {
				options.DataMatrix.CompactionMode = DataMatrixCompactionMode.Binary;
				return;
			}
			QRCodeGenerator generatorQRCode = generator as QRCodeGenerator;
			if (generatorQRCode != null) {
				options.QRCode.CompactionMode = QRCodeCompactionMode.Byte;
				return;
			}
			PDF417Generator generatorPDF417 = generator as PDF417Generator;
			if (generatorPDF417 != null) {
				options.PDF417.CompactionMode = PDF417CompactionMode.Binary;
			}
		}
		#endregion
		#region Layout Calculation
		SizeF CalculateBestSize(BarCodeLayoutInfo layoutInfo) {
			BarCodeViewInfo viewInfo = new BarCodeViewInfo(this);
			BarCodeDrawingViewInfo drawingViewInfo = new BarCodeDrawingViewInfo(new RectangleF(0, 0, float.MaxValue, float.MaxValue), viewInfo);
			using (Bitmap bitmap = CreateBitmap(10, 10)) {
				using (Graphics graphics = CreateGraphics(bitmap)) {
					GdiGraphicsWrapperBase wrapper = new GdiGraphicsWrapperBase(graphics);
					if (!drawingViewInfo.Calculate(generator, wrapper))
						throw new Exception(drawingViewInfo.ErrorText);
					if (!String.IsNullOrEmpty(TopCaption.Text))
						layoutInfo.TopCaptionBounds = new RectangleF(PointF.Empty, CalculateCaptionBounds(graphics, TopCaption, drawingViewInfo.BestSize.Width));
					if (!String.IsNullOrEmpty(BottomCaption.Text))
						layoutInfo.BottomCaptionBounds = new RectangleF(PointF.Empty, CalculateCaptionBounds(graphics, BottomCaption, drawingViewInfo.BestSize.Width));
				}
			}
			float lineBarHeight = (generator is BarCode2DGenerator) ? 0 : BarHeight;
			return new SizeF(drawingViewInfo.BestSize.Width, drawingViewInfo.BestSize.Height + lineBarHeight);
		}
		SizeF CalculateCaptionBounds(Graphics graphics, BarCodeCaption caption, float availableWidth) {
			availableWidth -= caption.Paddings.Left + caption.Paddings.Right;
			availableWidth = Math.Max(1, availableWidth);
			SizeF size = graphics.MeasureString(caption.Text, caption.Font, new SizeF(availableWidth, float.MaxValue), caption.StringFormat);
			size.Height += caption.Paddings.Top + caption.Paddings.Bottom;
			size.Width = availableWidth;
			return size;
		}
		void CalculateAutoSizeLayout(BarCodeLayoutInfo result) {
			SizeF bestSize = CalculateBestSize(result);
			RectangleF bounds = new RectangleF(PointF.Empty, bestSize);
			RectangleF borderBounds = new RectangleF(0, 0, bestSize.Width + Paddings.Left + Paddings.Right + 2 * BorderWidth, bestSize.Height + result.TopCaptionBounds.Height + result.BottomCaptionBounds.Height + Paddings.Top + Paddings.Bottom + 2 * BorderWidth);
			RectangleF boundingBox;
			if (result.ActualRotationAngle != 0) {
				Matrix rotationMatrix = new Matrix();
				rotationMatrix.RotateAt(result.ActualRotationAngle, RectangleUtils.CenterPoint(bounds));
				boundingBox = RectangleUtils.BoundingRectangle(borderBounds, rotationMatrix);
			}
			else {
				boundingBox = borderBounds;
			}
			borderBounds.X += (boundingBox.Width - borderBounds.Width) / 2;
			borderBounds.Y += (boundingBox.Height - borderBounds.Height) / 2;
			bounds.X = borderBounds.X + Paddings.Left + BorderWidth;
			bounds.Y = borderBounds.Y + Paddings.Top + BorderWidth + result.TopCaptionBounds.Height;
			result.ImageSize = new SizeF(boundingBox.Width + Margins.Left + Margins.Right, boundingBox.Height + Margins.Top + Margins.Bottom);
			result.BorderBounds = borderBounds;
			result.BarCodeBounds = bounds;
			result.TopCaptionBounds = new RectangleF(bounds.X, bounds.Y - result.TopCaptionBounds.Height, result.TopCaptionBounds.Width, result.TopCaptionBounds.Height);
			result.BottomCaptionBounds = new RectangleF(bounds.X, bounds.Bottom, result.BottomCaptionBounds.Width, result.BottomCaptionBounds.Height);
		}
		void CalculateFixedSizeLayout(BarCodeLayoutInfo result) {
			result.ImageSize = new SizeF(ImageWidth, ImageHeight);
			SizeF bestSize = CalculateBestSize(result);
			RectangleF borderBounds = new RectangleF(0, 0, bestSize.Width + Paddings.Left + Paddings.Right + 2 * BorderWidth, bestSize.Height + result.TopCaptionBounds.Height + result.BottomCaptionBounds.Height + Paddings.Top + Paddings.Bottom + 2 * BorderWidth);
			borderBounds.X += (ImageWidth - borderBounds.Width) / 2;
			borderBounds.Y += (ImageHeight - borderBounds.Height) / 2;
			RectangleF bounds = new RectangleF(new PointF(borderBounds.X + Paddings.Left + BorderWidth, borderBounds.Y + Paddings.Top + result.TopCaptionBounds.Height + BorderWidth), bestSize);
			result.BorderBounds = borderBounds;
			result.BarCodeBounds = bounds;
			result.TopCaptionBounds = new RectangleF(bounds.X, bounds.Y - result.TopCaptionBounds.Height, result.TopCaptionBounds.Width, result.TopCaptionBounds.Height);
			result.BottomCaptionBounds = new RectangleF(bounds.X, bounds.Bottom, result.BottomCaptionBounds.Width, result.BottomCaptionBounds.Height);
		}
		internal BarCodeLayoutInfo CalculateLayout() {
			BarCodeLayoutInfo result = new BarCodeLayoutInfo();
			result.ActualRotationAngle = RotationAngle % 360.0f;
			if (AutoSize)
				CalculateAutoSizeLayout(result);
			else
				CalculateFixedSizeLayout(result);
			return result;
		}
		#endregion
		Bitmap CreateBitmap(int pixelWidth, int pixelHeight) {
			Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight);
			bitmap.SetResolution(DpiX, DpiY);
			return bitmap;
		}
		Graphics CreateGraphics(Image image) {
			Graphics graphics = Graphics.FromImage(image);
			graphics.PageUnit = unit;
			graphics.TextRenderingHint = TextRenderingHint;
			if ((RotationAngle % 90) != 0)
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			return graphics;
		}
		Bitmap CreateImage() {
			UpdateGeneratorOptions();
			BarCodeLayoutInfo layoutInfo = CalculateLayout();
			int pixelWidth;
			int pixelHeight;
			if (Unit == GraphicsUnit.Pixel) {
				pixelWidth = (int)Math.Ceiling(layoutInfo.ImageSize.Width);
				pixelHeight = (int)Math.Ceiling(layoutInfo.ImageSize.Height);
			}
			else {
				pixelWidth = (int)Math.Ceiling(GraphicsUnitConverter.Convert(layoutInfo.ImageSize.Width, Unit, GraphicsUnit.Inch) * DpiX);
				pixelHeight = (int)Math.Ceiling(GraphicsUnitConverter.Convert(layoutInfo.ImageSize.Height, Unit, GraphicsUnit.Inch) * DpiY);
			}
			Bitmap bitmap = CreateBitmap(pixelWidth, pixelHeight);
			using (Graphics graphics = CreateGraphics(bitmap)) {
				BarCodeDrawingViewInfo.SetDoNotClip(generator, true);
				try {
					ApplyTransform(graphics, layoutInfo);
					DrawBackground(graphics, layoutInfo.BorderBounds);
					DrawCaption(graphics, TopCaption, layoutInfo.TopCaptionBounds);
					DrawCaption(graphics, BottomCaption, layoutInfo.BottomCaptionBounds);
					DrawBorder(graphics, layoutInfo.BorderBounds);
					DrawBarCode(graphics, layoutInfo.BarCodeBounds);
				}
				finally {
					BarCodeDrawingViewInfo.SetDoNotClip(generator, false);
				}
			}
			return bitmap;
		}
		void DrawCaption(Graphics graphics, BarCodeCaption caption, RectangleF bounds) {
			if (bounds.Height <= 0)
				return;
			using (SolidBrush brush = new SolidBrush(caption.ForeColor)) {
				BarCodePadding padding = caption.Paddings;
				bounds.X += padding.Left;
				bounds.Y += padding.Top;
				bounds.Width -= padding.Left + padding.Right;
				bounds.Height -= padding.Bottom + padding.Top;
				graphics.DrawString(caption.Text, caption.Font, brush, bounds, caption.StringFormat);
			}
		}
		void ApplyTransform(Graphics graphics, BarCodeLayoutInfo layoutInfo) {
			if (AutoSize)
				graphics.TranslateTransform(Margins.Left, Margins.Top);
			RectangleF borderBounds = layoutInfo.BorderBounds;
			if (layoutInfo.ActualRotationAngle != 0) {
				graphics.TranslateTransform((borderBounds.Right + borderBounds.Left) / 2, (borderBounds.Bottom + borderBounds.Top) / 2);
				graphics.RotateTransform(layoutInfo.ActualRotationAngle);
				graphics.TranslateTransform(-(borderBounds.Right + borderBounds.Left) / 2, -(borderBounds.Bottom + borderBounds.Top) / 2);
			}
		}
		void DrawBackground(Graphics graphics, RectangleF bounds) {
			using (SolidBrush brush = new SolidBrush(BackColor)) {
				RectangleF fillBounds = bounds;
				fillBounds.Inflate(-BorderWidth / 2, -BorderWidth / 2);
				graphics.FillRectangle(brush, fillBounds);
			}
		}
		void DrawBorder(Graphics graphics, RectangleF borderBounds) {
			if (BorderWidth > 0 && !DXColor.IsTransparentOrEmpty(BorderColor)) {
				using (Pen pen = new Pen(BorderColor, BorderWidth)) {
					pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
					graphics.DrawRectangle(pen, borderBounds.Left, borderBounds.Top, borderBounds.Width, borderBounds.Height);
				}
			}
		}
		void DrawBarCode(Graphics graphics, RectangleF bounds) {
#if !SL
			BarCodeViewInfo viewInfo = new BarCodeViewInfo(this);
			GdiGraphicsWrapperBase wrapper = new GdiGraphicsWrapperBase(graphics);
			generator.DrawContent(wrapper, bounds, viewInfo);
#endif
		}
		public void Save(string fileName, ImageFormat format) {
			BarCodeImage.Save(fileName, format);
		}
		public void Save(Stream stream, ImageFormat format) {
			BarCodeImage.Save(stream, format);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				ResetBarCodeImage();
			}
		}
	}
}
namespace DevExpress.BarCodes.Internal {
	public class BarCodeViewInfo : IBarCodeData {
		readonly BarCode barCode;
		BrickStyle style;
		public BarCodeViewInfo(BarCode barCode) {
			Guard.ArgumentNotNull(barCode, "barCode");
			this.barCode = barCode;
			this.style = new BrickStyle();
			this.style.ForeColor = barCode.ForeColor;
			this.style.BackColor = barCode.BackColor;
			this.style.SetAlignment(HorzAlignment.Default, VertAlignment.Default);
			this.style.Padding = new PaddingInfo(0, 0, 0, 0, barCode.Unit);
			this.style.BorderWidth = 5;
			this.style.BorderColor = DXColor.Red;
			this.style.BorderDashStyle = BorderDashStyle.Solid;
			this.style.StringFormat = new BrickStringFormat(StringFormatFlags.NoWrap, barCode.CodeTextHorizontalAlignment, barCode.CodeTextVerticalAlignment);
			this.style.Font = barCode.CodeTextFont;
		}
		public TextAlignment Alignment { get { return TextAlignment.TopLeft; } }
		public bool AutoModule { get { return false; } }
		public double Module {
			get {
				return barCode.Module;
			}
		}
		public BarCodeOrientation Orientation {
			get {
				return BarCodeOrientation.Normal;
			}
		}
		public bool ShowText {
			get {
				return barCode.ShowText;
			}
		}
		public BrickStyle Style { get { return style; } }
		public string Text { get { return barCode.CodeText; } }
		public byte[] BinaryData { get { return barCode.CodeBinaryData; } }
	}
	public class BarCodeLayoutInfo {
		public RectangleF BorderBounds { get; set; }
		public RectangleF BarCodeBounds { get; set; }
		public SizeF ImageSize { get; set; }
		public float ActualRotationAngle { get; set; }
		public RectangleF TopCaptionBounds { get; set; }
		public RectangleF BottomCaptionBounds { get; set; }
	}
}
