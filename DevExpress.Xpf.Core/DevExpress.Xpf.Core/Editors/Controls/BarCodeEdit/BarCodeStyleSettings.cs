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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors {
	public abstract class BarCodeStyleSettings : BaseEditStyleSettings {
		public BarCodeStyleSettings() {			
			Visibility = Visibility.Collapsed;
		}
		public abstract BarCodeGeneratorBase GeneratorBase { get; }
		internal BarCodeEdit BarCodeEdit { get; set; }
		public static void BarCodeControlInvalidateVisual(DependencyObject d) {
			if(((BarCodeStyleSettings)d).BarCodeEdit != null) ((BarCodeStyleSettings)d).BarCodeEdit.BarCodeInvalidateVisual();
		}
	}
	public abstract class BarCodeStyleSettings<T> : BarCodeStyleSettings where T : BarCodeGeneratorBase, new() {
		T barCodeGeneratorBase;
		public BarCodeStyleSettings() {
			barCodeGeneratorBase = new T();
		}
		public override BarCodeGeneratorBase GeneratorBase { get { return Generator; } }
		protected T Generator { get { return barCodeGeneratorBase; } }
	}
	public abstract class CheckSumStyleSettingsBase<T> : BarCodeStyleSettings<T> where T : BarCodeGeneratorBase, new() {
		public bool CalcCheckSum {
			get { return (bool)GetValue(CalcCheckSumProperty); }
			set { SetValue(CalcCheckSumProperty, value); }
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty CalcCheckSumProperty =
			DependencyProperty.Register("CalcCheckSum", typeof(bool), typeof(CheckSumStyleSettingsBase<T>), new PropertyMetadata(true, CalcCheckSumPropertyChanged));		
		static void CalcCheckSumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CheckSumStyleSettingsBase<T>)d).CalcCheckSum = (bool)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
	}
	public class CodabarStyleSettings : BarCodeStyleSettings<CodabarGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<CodabarStartStopPair>))]
		public CodabarStartStopPair StartStopPair {
			get { return (CodabarStartStopPair)GetValue(StartStopPairProperty); }
			set { SetValue(StartStopPairProperty, value); }
		}
		public static readonly DependencyProperty StartStopPairProperty =
			DependencyProperty.Register("StartStopPair", typeof(CodabarStartStopPair), typeof(CodabarStyleSettings), new PropertyMetadata(CodabarStartStopPair.AT, StartStopPairPropertyChanged));
		static void StartStopPairPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodabarStyleSettings)d).Generator.StartStopPair = (CodabarStartStopPair)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(CodabarStyleSettings), new PropertyMetadata((float)2, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodabarStyleSettings)d).Generator.WideNarrowRatio = (float)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.StartStopPair = this.StartStopPair;
			provider.WideNarrowRatio = this.WideNarrowRatio;
		}
	}
	public class Industrial2of5BarCodeStyleSettings<T> : CheckSumStyleSettingsBase<T> where T : Industrial2of5Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Industrial2of5BarCodeStyleSettings<T>), new PropertyMetadata(2.5f, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Industrial2of5BarCodeStyleSettings<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.WideNarrowRatio = this.WideNarrowRatio;
		}
	}
	public class Industrial2of5StyleSettings : Industrial2of5BarCodeStyleSettings<Industrial2of5Generator> { }
	public abstract class Interleaved2of5BarCodeStyleSettings<T> : BarCodeStyleSettings<T> where T : Interleaved2of5Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Interleaved2of5BarCodeStyleSettings<T>), new PropertyMetadata((float)3, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Interleaved2of5BarCodeStyleSettings<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.WideNarrowRatio = this.WideNarrowRatio;
		}
	}
	public class Interleaved2of5StyleSettings : Interleaved2of5BarCodeStyleSettings<Interleaved2of5Generator> { }
	public abstract class Code39BarCodeStyleSettings<T> : CheckSumStyleSettingsBase<T> where T : Code39Generator, new() {
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty WideNarrowRatioProperty =
			DependencyProperty.Register("WideNarrowRatio", typeof(float), typeof(Code39BarCodeStyleSettings<T>), new PropertyMetadata((float)3, WideNarrowRatioPropertyChanged));
		static void WideNarrowRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Code39BarCodeStyleSettings<T>)d).Generator.WideNarrowRatio = (float)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.WideNarrowRatio = this.WideNarrowRatio;
		}
	}
	public class Code39StyleSettings : Code39BarCodeStyleSettings<Code39Generator> { }
	public class Code39ExtendedStyleSettings : Code39BarCodeStyleSettings<Code39ExtendedGenerator> { }
	public class Code93StyleSettings : CheckSumStyleSettingsBase<Code93Generator> { }
	public class Code93ExtendedStyleSettings : CheckSumStyleSettingsBase<Code93ExtendedGenerator> { }
	public abstract class Code128BarCodeStyleSettings<T> : BarCodeStyleSettings<T> where T : Code128Generator, new() {
		[TypeConverter(typeof(XamlEnumTypeConverter<Code128Charset>))]
		public Code128Charset CharacterSet {
			get { return (Code128Charset)GetValue(CharacterSetProperty); }
			set { SetValue(CharacterSetProperty, value); }
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty CharacterSetProperty =
			DependencyProperty.Register("CharacterSet", typeof(Code128Charset), typeof(Code128BarCodeStyleSettings<T>), new PropertyMetadata(Code128Charset.CharsetA, CharacterSetPropertyChanged));
		static void CharacterSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Code128BarCodeStyleSettings<T>)d).Generator.CharacterSet = (Code128Charset)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.CharacterSet = this.CharacterSet;
		}
	}
	public class Code128StyleSettings : Code128BarCodeStyleSettings<Code128Generator> { }
	public class Code11StyleSettings : BarCodeStyleSettings<Code11Generator> { }
	public class CodeMSIStyleSettings : BarCodeStyleSettings<CodeMSIGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<MSICheckSum>))]
		public MSICheckSum MSICheckSum {
			get { return (MSICheckSum)GetValue(MSICheckSumProperty); }
			set { SetValue(MSICheckSumProperty, value); }
		}
		public static readonly DependencyProperty MSICheckSumProperty =
			DependencyProperty.Register("MSICheckSum", typeof(MSICheckSum), typeof(CodeMSIStyleSettings), new PropertyMetadata(MSICheckSum.Modulo10, MSICheckSumPropertyChanged));
		static void MSICheckSumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CodeMSIStyleSettings)d).Generator.MSICheckSum = (MSICheckSum)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.MSICheckSum = this.MSICheckSum;
		}
	}
	public class PostNetStyleSettings : BarCodeStyleSettings<PostNetGenerator> { }
	public class EAN13StyleSettings : BarCodeStyleSettings<EAN13Generator> { }
	public class UPCAStyleSettings : BarCodeStyleSettings<UPCAGenerator> { }
	public class EAN8StyleSettings : BarCodeStyleSettings<EAN8Generator> { }
	public class EAN128StyleSettings : Code128BarCodeStyleSettings<EAN128Generator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(EAN128StyleSettings), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EAN128StyleSettings)d).Generator.FNC1Substitute = (string)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public bool HumanReadableText {
			get { return (bool)GetValue(HumanReadableTextProperty); }
			set { SetValue(HumanReadableTextProperty, value); }
		}
		public static readonly DependencyProperty HumanReadableTextProperty =
			DependencyProperty.Register("HumanReadableText", typeof(bool), typeof(EAN128StyleSettings), new PropertyMetadata(true, HumanReadableTextPropertyChanged));
		static void HumanReadableTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((EAN128StyleSettings)d).Generator.HumanReadableText = (bool)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.FNC1Substitute = this.FNC1Substitute;
			provider.HumanReadableText = this.HumanReadableText;
		}
	}
	public class UPCSupplemental2StyleSettings : BarCodeStyleSettings<UPCSupplemental2Generator> { }
	public class UPCSupplemental5StyleSettings : BarCodeStyleSettings<UPCSupplemental5Generator> { }
	public class UPCE0StyleSettings : BarCodeStyleSettings<UPCE0Generator> { }
	public class UPCE1StyleSettings : BarCodeStyleSettings<UPCE1Generator> { }
	public class Matrix2of5StyleSettings : Industrial2of5BarCodeStyleSettings<Matrix2of5Generator> { }
	public class PDF417StyleSettings : BarCodeStyleSettings<PDF417Generator> {
		public int Columns {
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		public static readonly DependencyProperty ColumnsProperty =
			DependencyProperty.Register("Columns", typeof(int), typeof(PDF417StyleSettings), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinColumnsCount, ColumnsPropertyChanged));
		static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.Columns = (int)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<PDF417CompactionMode>))]
		public PDF417CompactionMode CompactionMode {
			get { return (PDF417CompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(PDF417CompactionMode), typeof(PDF417StyleSettings), new PropertyMetadata(PDF417CompactionMode.Text, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.CompactionMode = (PDF417CompactionMode)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<ErrorCorrectionLevel>))]
		public ErrorCorrectionLevel ErrorCorrectionLevel {
			get { return (ErrorCorrectionLevel)GetValue(ErrorCorrectionLevelProperty); }
			set { SetValue(ErrorCorrectionLevelProperty, value); }
		}
		public static readonly DependencyProperty ErrorCorrectionLevelProperty =
			DependencyProperty.Register("ErrorCorrectionLevel", typeof(ErrorCorrectionLevel), typeof(PDF417StyleSettings), new PropertyMetadata(ErrorCorrectionLevel.Level2, ErrorCorrectionLevelPropertyChanged));
		static void ErrorCorrectionLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.ErrorCorrectionLevel = (ErrorCorrectionLevel)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public int Rows {
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}
		public static readonly DependencyProperty RowsProperty =
			DependencyProperty.Register("Rows", typeof(int), typeof(PDF417StyleSettings), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinRowsCount, RowsPropertyChanged));
		static void RowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.Rows = (int)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public bool TruncateSymbol {
			get { return (bool)GetValue(TruncateSymbolProperty); }
			set { SetValue(TruncateSymbolProperty, value); }
		}
		public static readonly DependencyProperty TruncateSymbolProperty =
			DependencyProperty.Register("TruncateSymbol", typeof(bool), typeof(PDF417StyleSettings), new PropertyMetadata(false, TruncateSymbolPropertyChanged));
		static void TruncateSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.TruncateSymbol = (bool)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public float YToXRatio {
			get { return (float)GetValue(YToXRatioProperty); }
			set { SetValue(YToXRatioProperty, value); }
		}
		public static readonly DependencyProperty YToXRatioProperty =
			DependencyProperty.Register("YToXRatio", typeof(float), typeof(PDF417StyleSettings), new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinYToXRatio, YToXRatioPropertyChanged));
		static void YToXRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PDF417StyleSettings)d).Generator.YToXRatio = (float)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.Columns = this.Columns;
			provider.PDF417CompactionMode = this.CompactionMode;
			provider.PDF417ErrorCorrectionLevel = this.ErrorCorrectionLevel;
			provider.Rows = this.Rows;
			provider.TruncateSymbol = this.TruncateSymbol;
			provider.YToXRatio = this.YToXRatio;
		}
	}
	public class DataMatrixStyleSettings : BarCodeStyleSettings<DataMatrixGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixCompactionMode>))]
		public DataMatrixCompactionMode CompactionMode {
			get { return (DataMatrixCompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(DataMatrixCompactionMode), typeof(DataMatrixStyleSettings), new PropertyMetadata(DataMatrixCompactionMode.ASCII, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixStyleSettings)d).Generator.CompactionMode = (DataMatrixCompactionMode)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixSize>))]
		public DataMatrixSize MatrixSize {
			get { return (DataMatrixSize)GetValue(MatrixSizeProperty); }
			set { SetValue(MatrixSizeProperty, value); }
		}
		public static readonly DependencyProperty MatrixSizeProperty =
			DependencyProperty.Register("MatrixSize", typeof(DataMatrixSize), typeof(DataMatrixStyleSettings), new PropertyMetadata(DataMatrixSize.MatrixAuto, MatrixSizePropertyChanged));
		static void MatrixSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixStyleSettings)d).Generator.MatrixSize = (DataMatrixSize)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.DataMatrixCompactionMode = this.CompactionMode;
			provider.MatrixSize = this.MatrixSize;
		}
	}
	public class DataMatrixGS1StyleSettings : BarCodeStyleSettings<DataMatrixGS1Generator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(DataMatrixGS1StyleSettings), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1StyleSettings)d).Generator.FNC1Substitute = (string)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public bool HumanReadableText {
			get { return (bool)GetValue(HumanReadableTextProperty); }
			set { SetValue(HumanReadableTextProperty, value); }
		}
		public static readonly DependencyProperty HumanReadableTextProperty =
			DependencyProperty.Register("HumanReadableText", typeof(bool), typeof(DataMatrixGS1StyleSettings), new PropertyMetadata(true, HumanReadableTextPropertyChanged));
		static void HumanReadableTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1StyleSettings)d).Generator.HumanReadableText = (bool)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataMatrixSize>))]
		public DataMatrixSize MatrixSize {
			get { return (DataMatrixSize)GetValue(MatrixSizeProperty); }
			set { SetValue(MatrixSizeProperty, value); }
		}
		public static readonly DependencyProperty MatrixSizeProperty =
			DependencyProperty.Register("MatrixSize", typeof(DataMatrixSize), typeof(DataMatrixGS1StyleSettings), new PropertyMetadata(DataMatrixSize.MatrixAuto, MatrixSizePropertyChanged));
		static void MatrixSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataMatrixGS1StyleSettings)d).Generator.MatrixSize = (DataMatrixSize)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.FNC1Substitute = this.FNC1Substitute;
			provider.HumanReadableText = this.HumanReadableText;
			provider.MatrixSize = this.MatrixSize;
		}
	}
	public class QRCodeStyleSettings : BarCodeStyleSettings<QRCodeGenerator> {
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeCompactionMode>))]
		public QRCodeCompactionMode CompactionMode {
			get { return (QRCodeCompactionMode)GetValue(CompactionModeProperty); }
			set { SetValue(CompactionModeProperty, value); }
		}
		public static readonly DependencyProperty CompactionModeProperty =
			DependencyProperty.Register("CompactionMode", typeof(QRCodeCompactionMode), typeof(QRCodeStyleSettings), new PropertyMetadata(QRCodeCompactionMode.AlphaNumeric, CompactionModePropertyChanged));
		static void CompactionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeStyleSettings)d).Generator.CompactionMode = (QRCodeCompactionMode)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeErrorCorrectionLevel>))]
		public QRCodeErrorCorrectionLevel ErrorCorrectionLevel {
			get { return (QRCodeErrorCorrectionLevel)GetValue(ErrorCorrectionLevelProperty); }
			set { SetValue(ErrorCorrectionLevelProperty, value); }
		}
		public static readonly DependencyProperty ErrorCorrectionLevelProperty =
			DependencyProperty.Register("ErrorCorrectionLevel", typeof(QRCodeErrorCorrectionLevel), typeof(QRCodeStyleSettings), new PropertyMetadata(QRCodeErrorCorrectionLevel.L, ErrorCorrectionLevelPropertyChanged));
		static void ErrorCorrectionLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeStyleSettings)d).Generator.ErrorCorrectionLevel = (QRCodeErrorCorrectionLevel)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<QRCodeVersion>))]
		public QRCodeVersion Version {
			get { return (QRCodeVersion)GetValue(VersionProperty); }
			set { SetValue(VersionProperty, value); }
		}
		public static readonly DependencyProperty VersionProperty =
			DependencyProperty.Register("Version", typeof(QRCodeVersion), typeof(QRCodeStyleSettings), new PropertyMetadata(QRCodeVersion.AutoVersion, VersionPropertyChanged));
		static void VersionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((QRCodeStyleSettings)d).Generator.Version = (QRCodeVersion)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.QRCodeCompactionMode = this.CompactionMode;
			provider.QRCodeErrorCorrectionLevel = this.ErrorCorrectionLevel;
			provider.Version = this.Version;
		}
	}
	public class IntelligentMailStyleSettings : BarCodeStyleSettings<IntelligentMailGenerator> { }
	public class ITF14StyleSettings : Interleaved2of5BarCodeStyleSettings<ITF14Generator> { }
	public class DataBarStyleSettings : BarCodeStyleSettings<DataBarGenerator> {
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public static readonly DependencyProperty FNC1SubstituteProperty =
			DependencyProperty.Register("FNC1Substitute", typeof(string), typeof(DataBarStyleSettings), new PropertyMetadata("#", FNC1SubstitutePropertyChanged));
		static void FNC1SubstitutePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarStyleSettings)d).Generator.FNC1Substitute = (string)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public int SegmentsInRow {
			get { return (int)GetValue(SegmentsInRowProperty); }
			set { SetValue(SegmentsInRowProperty, value); }
		}
		public static readonly DependencyProperty SegmentsInRowProperty =
			DependencyProperty.Register("SegmentsInRow", typeof(int), typeof(DataBarStyleSettings), new PropertyMetadata(20, SegmentsInRowPropertyChanged));
		static void SegmentsInRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarStyleSettings)d).Generator.SegmentsInRow = (int)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		[TypeConverter(typeof(XamlEnumTypeConverter<DataBarType>))]
		public DataBarType Type {
			get { return (DataBarType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(DataBarType), typeof(DataBarStyleSettings), new PropertyMetadata(DataBarType.Omnidirectional, TypePropertyChanged));
		static void TypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataBarStyleSettings)d).Generator.Type = (DataBarType)e.NewValue;
			BarCodeStyleSettings.BarCodeControlInvalidateVisual(d);
		}
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			BarCodePropertyProvider provider = (BarCodePropertyProvider)editor.PropertyProvider;
			provider.FNC1Substitute = this.FNC1Substitute;
			provider.SegmentsInRow = this.SegmentsInRow;
			provider.Type = this.Type;
		}
	}
	public static class BarCodeStyleSettingsStorage {
		static Dictionary<BarCodeSymbology, Type> storage = new Dictionary<BarCodeSymbology, Type>();
		static BarCodeStyleSettingsStorage() {
			storage[BarCodeSymbology.Codabar] = typeof(CodabarStyleSettings);
			storage[BarCodeSymbology.Industrial2of5] = typeof(Industrial2of5StyleSettings);
			storage[BarCodeSymbology.Interleaved2of5] = typeof(Interleaved2of5StyleSettings);
			storage[BarCodeSymbology.Code39] = typeof(Code39StyleSettings);
			storage[BarCodeSymbology.Code39Extended] = typeof(Code39ExtendedStyleSettings);
			storage[BarCodeSymbology.Code93] = typeof(Code93StyleSettings);
			storage[BarCodeSymbology.Code93Extended] = typeof(Code93ExtendedStyleSettings);
			storage[BarCodeSymbology.Code128] = typeof(Code128StyleSettings);
			storage[BarCodeSymbology.Code11] = typeof(Code11StyleSettings);
			storage[BarCodeSymbology.CodeMSI] = typeof(CodeMSIStyleSettings);
			storage[BarCodeSymbology.PostNet] = typeof(PostNetStyleSettings);
			storage[BarCodeSymbology.EAN13] = typeof(EAN13StyleSettings);
			storage[BarCodeSymbology.UPCA] = typeof(UPCAStyleSettings);
			storage[BarCodeSymbology.EAN8] = typeof(EAN8StyleSettings);
			storage[BarCodeSymbology.EAN128] = typeof(EAN128StyleSettings);
			storage[BarCodeSymbology.UPCSupplemental2] = typeof(UPCSupplemental2StyleSettings);
			storage[BarCodeSymbology.UPCSupplemental5] = typeof(UPCSupplemental5StyleSettings);
			storage[BarCodeSymbology.UPCE0] = typeof(UPCE0StyleSettings);
			storage[BarCodeSymbology.UPCE1] = typeof(UPCE1StyleSettings);
			storage[BarCodeSymbology.Matrix2of5] = typeof(Matrix2of5StyleSettings);
			storage[BarCodeSymbology.PDF417] = typeof(PDF417StyleSettings);
			storage[BarCodeSymbology.DataMatrix] = typeof(DataMatrixStyleSettings);
			storage[BarCodeSymbology.DataMatrixGS1] = typeof(DataMatrixGS1StyleSettings);
			storage[BarCodeSymbology.QRCode] = typeof(QRCodeStyleSettings);
			storage[BarCodeSymbology.IntelligentMail] = typeof(IntelligentMailStyleSettings);
			storage[BarCodeSymbology.ITF14] = typeof(ITF14StyleSettings);
			storage[BarCodeSymbology.DataBar] = typeof(DataBarStyleSettings);
		}
		public static BarCodeStyleSettings Create(BarCodeSymbology symbologyCode) {
			Type type = storage[symbologyCode];
			if(type == null)
				throw new ArgumentException();
			return (BarCodeStyleSettings)Activator.CreateInstance(type);
		}
		public static IEnumerable<Type> GetSymbologyTypes() {
			return storage.Values.OrderBy(s => s.Name);
		}
	}
}
