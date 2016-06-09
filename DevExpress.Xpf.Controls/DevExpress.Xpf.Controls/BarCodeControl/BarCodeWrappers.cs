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

#if !WINRT && !WP
#pragma warning disable 0618
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Controls.Internal;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
#else
using Windows.UI.Xaml;
using DevExpress.UI.Xaml.Controls.Native;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Core.Native;
#endif
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
#if !WINRT && !WP
namespace DevExpress.Xpf.Controls {	
#else
namespace DevExpress.UI.Xaml.Controls {
	public class DXFrameworkElement : FrameworkElement { }
#endif
	public abstract class SymbologyBase : DXFrameworkElement {
		public SymbologyBase() {			
			Visibility = Visibility.Collapsed;
		}
		public abstract BarCodeGeneratorBase GeneratorBase { get; }
		internal BarCodeControl BarCodeControl { get; set; }
		public static void BarCodeControlInvalidateVisual(DependencyObject d) {
			if(((SymbologyBase)d).BarCodeControl != null) ((SymbologyBase)d).BarCodeControl.BarCodeInvalidateVisual();
		}
	}
	public abstract class SymbologyBase<T> : SymbologyBase where T : BarCodeGeneratorBase, new() {
		T barCodeGeneratorBase;
		public SymbologyBase() {
			barCodeGeneratorBase = new T();
		}
		public override BarCodeGeneratorBase GeneratorBase { get { return Generator; } }
		protected T Generator { get { return barCodeGeneratorBase; } }
	}
	public abstract class SymbologyCheckSumBase<T> : SymbologyBase<T> where T : BarCodeGeneratorBase, new() {
		public bool CalcCheckSum {
			get { return (bool)GetValue(CalcCheckSumProperty); }
			set { SetValue(CalcCheckSumProperty, value); }
		}
		public static readonly DependencyProperty CalcCheckSumProperty =
			DependencyProperty.Register("CalcCheckSum", typeof(bool), typeof(SymbologyCheckSumBase<T>), new PropertyMetadata(true, CalcCheckSumPropertyChanged));		
		static void CalcCheckSumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SymbologyCheckSumBase<T>)d).CalcCheckSum = (bool)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class CodabarSymbology : SymbologyBase<CodabarGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<CodabarStartStopPair>))]
		public CodabarStartStopPair StartStopPair {
			get { return (CodabarStartStopPair)GetValue(StartStopPairProperty); }
			set { SetValue(StartStopPairProperty, value); }
		}
		public static readonly DependencyProperty StartStopPairProperty =
			DependencyProperty.Register("StartStopPair", typeof(CodabarStartStopPair), typeof(CodabarSymbology), new PropertyMetadata(CodabarStartStopPair.AT, StartStopPairPropertyChanged));
		static void StartStopPairPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodabarSymbology)d).Generator.StartStopPair = (CodabarStartStopPair)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(CodabarSymbology), new PropertyMetadata((float)2, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodabarSymbology)d).Generator.WideNarrowRatio = (float)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class Industrial2of5SymbologyBase<T> : SymbologyCheckSumBase<T> where T : Industrial2of5Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Industrial2of5SymbologyBase<T>), new PropertyMetadata(2.5f, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Industrial2of5SymbologyBase<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class Industrial2of5Symbology : Industrial2of5SymbologyBase<Industrial2of5Generator> { }
	public abstract class Interleaved2of5SymbologyBase<T> : SymbologyBase<T> where T : Interleaved2of5Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Interleaved2of5SymbologyBase<T>), new PropertyMetadata((float)3, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Interleaved2of5SymbologyBase<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class Interleaved2of5Symbology : Interleaved2of5SymbologyBase<Interleaved2of5Generator> { }
	public abstract class Code39SymbologyBase<T> : SymbologyCheckSumBase<T> where T : Code39Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Code39SymbologyBase<T>), new PropertyMetadata((float)3, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Code39SymbologyBase<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class Code39Symbology : Code39SymbologyBase<Code39Generator> { }
	public class Code39ExtendedSymbology : Code39SymbologyBase<Code39ExtendedGenerator> { }
	public class Code93Symbology : SymbologyCheckSumBase<Code93Generator> { }
	public class Code93ExtendedSymbology : SymbologyCheckSumBase<Code93ExtendedGenerator> { }
	public abstract class Code128SymbologyBase<T> : SymbologyBase<T> where T : Code128Generator, new() {
		[TypeConverter(typeof(XamlEnumTypeConverter<Code128Charset>))]
		public Code128Charset CharacterSet {
			get { return (Code128Charset)GetValue(CharacterSetProperty); }
			set { SetValue(CharacterSetProperty, value); }
		}
		public static readonly DependencyProperty CharacterSetProperty =
			DependencyProperty.Register("CharacterSet", typeof(Code128Charset), typeof(Code128SymbologyBase<T>), new PropertyMetadata(Code128Charset.CharsetA, CharacterSetPropertyChanged));
		static void CharacterSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Code128SymbologyBase<T>)d).Generator.CharacterSet = (Code128Charset)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class Code128Symbology : Code128SymbologyBase<Code128Generator> { }
	public class Code11Symbology : SymbologyBase<Code11Generator> { }
	public class CodeMSISymbology : SymbologyBase<CodeMSIGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<MSICheckSum>))]
		public MSICheckSum MSICheckSum {
			get { return (MSICheckSum)GetValue(MSICheckSumProperty); }
			set { SetValue(MSICheckSumProperty, value); }
		}
		public static readonly DependencyProperty MSICheckSumProperty =
			DependencyProperty.Register("MSICheckSum", typeof(MSICheckSum), typeof(CodeMSISymbology), new PropertyMetadata(MSICheckSum.Modulo10, MSICheckSumPropertyChanged));
		static void MSICheckSumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodeMSISymbology)d).Generator.MSICheckSum = (MSICheckSum)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class PostNetSymbology : SymbologyBase<PostNetGenerator> { }
	public class EAN13Symbology : SymbologyBase<EAN13Generator> { }
	public class UPCASymbology : SymbologyBase<UPCAGenerator> { }
	public class EAN8Symbology : SymbologyBase<EAN8Generator> { }
	public class EAN128Symbology : Code128SymbologyBase<EAN128Generator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(EAN128Symbology), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EAN128Symbology)d).Generator.FNC1Substitute = (string)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public bool HumanReadableText {
			get { return (bool)GetValue(HumanReadableTextProperty); }
			set { SetValue(HumanReadableTextProperty, value); }
		}
		public static readonly DependencyProperty HumanReadableTextProperty =
			DependencyProperty.Register("HumanReadableText", typeof(bool), typeof(EAN128Symbology), new PropertyMetadata(true, HumanReadableTextPropertyChanged));
		static void HumanReadableTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EAN128Symbology)d).Generator.HumanReadableText = (bool)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class UPCSupplemental2Symbology : SymbologyBase<UPCSupplemental2Generator> { }
	public class UPCSupplemental5Symbology : SymbologyBase<UPCSupplemental5Generator> { }
	public class UPCE0Symbology : SymbologyBase<UPCE0Generator> { }
	public class UPCE1Symbology : SymbologyBase<UPCE1Generator> { }
	public class Matrix2of5Symbology : Industrial2of5SymbologyBase<Matrix2of5Generator> { }
	public class PDF417Symbology : SymbologyBase<PDF417Generator> {
		public int Columns {
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		public static readonly DependencyProperty ColumnsProperty =
			DependencyProperty.Register("Columns", typeof(int), typeof(PDF417Symbology), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinColumnsCount, ColumnsPropertyChanged));
		static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.Columns = (int)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<PDF417CompactionMode>))]
		public PDF417CompactionMode CompactionMode {
			get { return (PDF417CompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(PDF417CompactionMode), typeof(PDF417Symbology), new PropertyMetadata(PDF417CompactionMode.Text, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.CompactionMode = (PDF417CompactionMode)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<ErrorCorrectionLevel>))]
		public ErrorCorrectionLevel ErrorCorrectionLevel {
			get { return (ErrorCorrectionLevel)GetValue(ErrorCorrectionLevelProperty); }
			set { SetValue(ErrorCorrectionLevelProperty, value); }
		}
		public static readonly DependencyProperty ErrorCorrectionLevelProperty =
			DependencyProperty.Register("ErrorCorrectionLevel", typeof(ErrorCorrectionLevel), typeof(PDF417Symbology), new PropertyMetadata(ErrorCorrectionLevel.Level2, ErrorCorrectionLevelPropertyChanged));
		static void ErrorCorrectionLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.ErrorCorrectionLevel = (ErrorCorrectionLevel)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public int Rows {
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}
		public static readonly DependencyProperty RowsProperty =
			DependencyProperty.Register("Rows", typeof(int), typeof(PDF417Symbology), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinRowsCount, RowsPropertyChanged));
		static void RowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.Rows = (int)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public bool TruncateSymbol {
			get { return (bool)GetValue(TruncateSymbolProperty); }
			set { SetValue(TruncateSymbolProperty, value); }
		}
		public static readonly DependencyProperty TruncateSymbolProperty =
			DependencyProperty.Register("TruncateSymbol", typeof(bool), typeof(PDF417Symbology), new PropertyMetadata(false, TruncateSymbolPropertyChanged));
		static void TruncateSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.TruncateSymbol = (bool)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public float YToXRatio {
			get { return (float)GetValue(YToXRatioProperty); }
			set { SetValue(YToXRatioProperty, value); }
		}
		public static readonly DependencyProperty YToXRatioProperty =
			DependencyProperty.Register("YToXRatio", typeof(float), typeof(PDF417Symbology), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinYToXRatio, YToXRatioPropertyChanged));
		static void YToXRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417Symbology)d).Generator.YToXRatio = (float)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class DataMatrixSymbology : SymbologyBase<DataMatrixGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixCompactionMode>))]
		public DataMatrixCompactionMode CompactionMode {
			get { return (DataMatrixCompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(DataMatrixCompactionMode), typeof(DataMatrixSymbology), new PropertyMetadata(DataMatrixCompactionMode.ASCII, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixSymbology)d).Generator.CompactionMode = (DataMatrixCompactionMode)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixSize>))]
		public DataMatrixSize MatrixSize {
			get { return (DataMatrixSize)GetValue(MatrixSizeProperty); }
			set { SetValue(MatrixSizeProperty, value); }
		}
		public static readonly DependencyProperty MatrixSizeProperty =
			DependencyProperty.Register("MatrixSize", typeof(DataMatrixSize), typeof(DataMatrixSymbology), new PropertyMetadata(DataMatrixSize.MatrixAuto, MatrixSizePropertyChanged));
		static void MatrixSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixSymbology)d).Generator.MatrixSize = (DataMatrixSize)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class DataMatrixGS1Symbology : SymbologyBase<DataMatrixGS1Generator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(DataMatrixGS1Symbology), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1Symbology)d).Generator.FNC1Substitute = (string)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public bool HumanReadableText {
			get { return (bool)GetValue(HumanReadableTextProperty); }
			set { SetValue(HumanReadableTextProperty, value); }
		}
		public static readonly DependencyProperty HumanReadableTextProperty =
			DependencyProperty.Register("HumanReadableText", typeof(bool), typeof(DataMatrixGS1Symbology), new PropertyMetadata(true, HumanReadableTextPropertyChanged));
		static void HumanReadableTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1Symbology)d).Generator.HumanReadableText = (bool)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixSize>))]
		public DataMatrixSize MatrixSize {
			get { return (DataMatrixSize)GetValue(MatrixSizeProperty); }
			set { SetValue(MatrixSizeProperty, value); }
		}
		public static readonly DependencyProperty MatrixSizeProperty =
			DependencyProperty.Register("MatrixSize", typeof(DataMatrixSize), typeof(DataMatrixGS1Symbology), new PropertyMetadata(DataMatrixSize.MatrixAuto, MatrixSizePropertyChanged));
		static void MatrixSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1Symbology)d).Generator.MatrixSize = (DataMatrixSize)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class QRCodeSymbology : SymbologyBase<QRCodeGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeCompactionMode>))]
		public QRCodeCompactionMode CompactionMode {
			get { return (QRCodeCompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(QRCodeCompactionMode), typeof(QRCodeSymbology), new PropertyMetadata(QRCodeCompactionMode.AlphaNumeric, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeSymbology)d).Generator.CompactionMode = (QRCodeCompactionMode)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeErrorCorrectionLevel>))]
		public QRCodeErrorCorrectionLevel ErrorCorrectionLevel {
			get { return (QRCodeErrorCorrectionLevel)GetValue(ErrorCorrectionLevelProperty); }
			set { SetValue(ErrorCorrectionLevelProperty, value); }
		}
		public static readonly DependencyProperty ErrorCorrectionLevelProperty =
			DependencyProperty.Register("ErrorCorrectionLevel", typeof(QRCodeErrorCorrectionLevel), typeof(QRCodeSymbology), new PropertyMetadata(QRCodeErrorCorrectionLevel.L, ErrorCorrectionLevelPropertyChanged));
		static void ErrorCorrectionLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeSymbology)d).Generator.ErrorCorrectionLevel = (QRCodeErrorCorrectionLevel)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeVersion>))]
		public QRCodeVersion Version {
			get { return (QRCodeVersion)GetValue(VersionProperty); }
			set { SetValue(VersionProperty, value); }
		}
		public static readonly DependencyProperty VersionProperty =
			DependencyProperty.Register("Version", typeof(QRCodeVersion), typeof(QRCodeSymbology), new PropertyMetadata(QRCodeVersion.AutoVersion, VersionPropertyChanged));
		static void VersionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeSymbology)d).Generator.Version = (QRCodeVersion)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public class IntelligentMailSymbology : SymbologyBase<IntelligentMailGenerator> { }
	public class ITF14Symbology : Interleaved2of5SymbologyBase<ITF14Generator> { }
	public class DataBarSymbology : SymbologyBase<DataBarGenerator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(DataBarSymbology), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarSymbology)d).Generator.FNC1Substitute = (string)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		public int SegmentsInRow {
			get { return (int)GetValue(SegmentsInRowProperty); }
			set { SetValue(SegmentsInRowProperty, value); }
		}
		public static readonly DependencyProperty SegmentsInRowProperty =
			DependencyProperty.Register("SegmentsInRow", typeof(int), typeof(DataBarSymbology), new PropertyMetadata(20, SegmentsInRowPropertyChanged));
		static void SegmentsInRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarSymbology)d).Generator.SegmentsInRow = (int)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataBarType>))]
		public DataBarType Type {
			get { return (DataBarType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(DataBarType), typeof(DataBarSymbology), new PropertyMetadata(DataBarType.Omnidirectional, TypePropertyChanged));
		static void TypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarSymbology)d).Generator.Type = (DataBarType)e.NewValue;
			SymbologyBase.BarCodeControlInvalidateVisual(d);
		}
	}
	public static class BarCodeSymbologyStorage {
		static Dictionary<BarCodeSymbology, Type> storage = new Dictionary<BarCodeSymbology, Type>();
		static BarCodeSymbologyStorage() {
			storage[BarCodeSymbology.Codabar] = typeof(CodabarSymbology);
			storage[BarCodeSymbology.Industrial2of5] = typeof(Industrial2of5Symbology);
			storage[BarCodeSymbology.Interleaved2of5] = typeof(Interleaved2of5Symbology);
			storage[BarCodeSymbology.Code39] = typeof(Code39Symbology);
			storage[BarCodeSymbology.Code39Extended] = typeof(Code39ExtendedSymbology);
			storage[BarCodeSymbology.Code93] = typeof(Code93Symbology);
			storage[BarCodeSymbology.Code93Extended] = typeof(Code93ExtendedSymbology);
			storage[BarCodeSymbology.Code128] = typeof(Code128Symbology);
			storage[BarCodeSymbology.Code11] = typeof(Code11Symbology);
			storage[BarCodeSymbology.CodeMSI] = typeof(CodeMSISymbology);
			storage[BarCodeSymbology.PostNet] = typeof(PostNetSymbology);
			storage[BarCodeSymbology.EAN13] = typeof(EAN13Symbology);
			storage[BarCodeSymbology.UPCA] = typeof(UPCASymbology);
			storage[BarCodeSymbology.EAN8] = typeof(EAN8Symbology);
			storage[BarCodeSymbology.EAN128] = typeof(EAN128Symbology);
			storage[BarCodeSymbology.UPCSupplemental2] = typeof(UPCSupplemental2Symbology);
			storage[BarCodeSymbology.UPCSupplemental5] = typeof(UPCSupplemental5Symbology);
			storage[BarCodeSymbology.UPCE0] = typeof(UPCE0Symbology);
			storage[BarCodeSymbology.UPCE1] = typeof(UPCE1Symbology);
			storage[BarCodeSymbology.Matrix2of5] = typeof(Matrix2of5Symbology);
			storage[BarCodeSymbology.PDF417] = typeof(PDF417Symbology);
			storage[BarCodeSymbology.DataMatrix] = typeof(DataMatrixSymbology);
			storage[BarCodeSymbology.DataMatrixGS1] = typeof(DataMatrixGS1Symbology);
			storage[BarCodeSymbology.QRCode] = typeof(QRCodeSymbology);
			storage[BarCodeSymbology.IntelligentMail] = typeof(IntelligentMailSymbology);
			storage[BarCodeSymbology.ITF14] = typeof(ITF14Symbology);
			storage[BarCodeSymbology.DataBar] = typeof(DataBarSymbology);
		}
		public static SymbologyBase Create(BarCodeSymbology symbologyCode) {
			Type type = storage[symbologyCode];
			if(type == null)
				throw new ArgumentException();
			return (SymbologyBase)Activator.CreateInstance(type);
		}
		public static IEnumerable<Type> GetSymbologyTypes() {
			return storage.Values.OrderBy(s => s.Name);
		}
	}
}
#if !WINRT && !WP
#pragma warning restore  0618
#endif
