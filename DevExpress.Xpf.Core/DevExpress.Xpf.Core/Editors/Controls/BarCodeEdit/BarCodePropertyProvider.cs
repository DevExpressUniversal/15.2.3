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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.XtraPrinting.BarCode;
namespace DevExpress.Xpf.Editors {
	public class BarCodePropertyProvider : ActualPropertyProvider {		
		public static readonly DependencyProperty CalcCheckSumProperty;
		public static readonly DependencyProperty StartStopPairProperty;
		public static readonly DependencyProperty WideNarrowRatioProperty;
		public static readonly DependencyProperty CharacterSetProperty;
		public static readonly DependencyProperty MSICheckSumProperty;
		public static readonly DependencyProperty FNC1SubstituteProperty;
		public static readonly DependencyProperty HumanReadableTextProperty;
		public static readonly DependencyProperty ColumnsProperty;
		public static readonly DependencyProperty PDF417CompactionModeProperty;
		public static readonly DependencyProperty DataMatrixCompactionModeProperty;
		public static readonly DependencyProperty QRCodeCompactionModeProperty;
		public static readonly DependencyProperty PDF417ErrorCorrectionLevelProperty;
		public static readonly DependencyProperty QRCodeErrorCorrectionLevelProperty;
		public static readonly DependencyProperty RowsProperty;
		public static readonly DependencyProperty TruncateSymbolProperty;
		public static readonly DependencyProperty YToXRatioProperty;
		public static readonly DependencyProperty MatrixSizeProperty;
		public static readonly DependencyProperty VersionProperty;
		public static readonly DependencyProperty SegmentsInRowProperty;
		public static readonly DependencyProperty TypeProperty;
		static BarCodePropertyProvider() {
			Type ownerType = typeof(BarCodePropertyProvider);
			CalcCheckSumProperty = DependencyProperty.Register("CalcCheckSum", typeof(bool), ownerType, new PropertyMetadata(true, PropertyChanged));
			StartStopPairProperty = DependencyProperty.Register("StartStopPair", typeof(CodabarStartStopPair), ownerType, new PropertyMetadata(CodabarStartStopPair.AT, PropertyChanged));
			WideNarrowRatioProperty = DependencyProperty.Register("WideNarrowRatio", typeof(float), ownerType, new PropertyMetadata((float)2, PropertyChanged));
			CharacterSetProperty = DependencyProperty.Register("CharacterSet", typeof(Code128Charset), ownerType, new PropertyMetadata(Code128Charset.CharsetA, PropertyChanged));
			MSICheckSumProperty = DependencyProperty.Register("MSICheckSum", typeof(MSICheckSum), ownerType, new PropertyMetadata(MSICheckSum.Modulo10, PropertyChanged));
			FNC1SubstituteProperty = DependencyProperty.Register("FNC1Substitute", typeof(string), ownerType, new PropertyMetadata("#", PropertyChanged));
			HumanReadableTextProperty = DependencyProperty.Register("HumanReadableText", typeof(bool), ownerType, new PropertyMetadata(true, PropertyChanged));
			ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), ownerType, new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinColumnsCount, PropertyChanged));
			PDF417CompactionModeProperty = DependencyProperty.Register("PDF417CompactionMode", typeof(PDF417CompactionMode), ownerType, new PropertyMetadata(PDF417CompactionMode.Text, PropertyChanged));
			DataMatrixCompactionModeProperty = DependencyProperty.Register("DataMatrixCompactionMode", typeof(DataMatrixCompactionMode), ownerType, new PropertyMetadata(DataMatrixCompactionMode.ASCII, PropertyChanged));
			QRCodeCompactionModeProperty = DependencyProperty.Register("QRCodeCompactionMode", typeof(QRCodeCompactionMode), ownerType, new PropertyMetadata(QRCodeCompactionMode.AlphaNumeric, PropertyChanged));
			PDF417ErrorCorrectionLevelProperty = DependencyProperty.Register("PDF417ErrorCorrectionLevel", typeof(ErrorCorrectionLevel), ownerType, new PropertyMetadata(ErrorCorrectionLevel.Level2, PropertyChanged));
			QRCodeErrorCorrectionLevelProperty = DependencyProperty.Register("QRCodeErrorCorrectionLevel", typeof(QRCodeErrorCorrectionLevel), ownerType, new PropertyMetadata(QRCodeErrorCorrectionLevel.L, PropertyChanged));
			RowsProperty = DependencyProperty.Register("Rows", typeof(int), ownerType, new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinRowsCount, PropertyChanged));
			TruncateSymbolProperty = DependencyProperty.Register("TruncateSymbol", typeof(bool), ownerType, new PropertyMetadata(false, PropertyChanged));
			YToXRatioProperty = DependencyProperty.Register("YToXRatio", typeof(float), ownerType, new PropertyMetadata(DevExpress.XtraPrinting.BarCode.Native.PDF417Constants.MinYToXRatio, PropertyChanged));
			MatrixSizeProperty = DependencyProperty.Register("MatrixSize", typeof(DataMatrixSize), ownerType, new PropertyMetadata(DataMatrixSize.MatrixAuto, PropertyChanged));
			VersionProperty = DependencyProperty.Register("Version", typeof(QRCodeVersion), ownerType, new PropertyMetadata(QRCodeVersion.AutoVersion, PropertyChanged));
			SegmentsInRowProperty = DependencyProperty.Register("SegmentsInRow", typeof(int), ownerType, new PropertyMetadata(20, PropertyChanged));
			TypeProperty = DependencyProperty.Register("Type", typeof(DataBarType), ownerType, new PropertyMetadata(DataBarType.Omnidirectional, PropertyChanged));
		}
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarCodePropertyProvider provider = (BarCodePropertyProvider)d;
			BarCodeEdit barCodeEdit = (BarCodeEdit)provider.Editor;
			barCodeEdit.BarCodeInvalidateVisual();
		}
		public bool CalcCheckSum {
			get { return (bool)GetValue(CalcCheckSumProperty); }
			set { SetValue(CalcCheckSumProperty, value); }
		}
		public CodabarStartStopPair StartStopPair {
			get { return (CodabarStartStopPair)GetValue(StartStopPairProperty); }
			set { SetValue(StartStopPairProperty, value); }
		}
		public float WideNarrowRatio {
			get { return (float)GetValue(WideNarrowRatioProperty); }
			set { SetValue(WideNarrowRatioProperty, value); }
		}
		public Code128Charset CharacterSet {
			get { return (Code128Charset)GetValue(CharacterSetProperty); }
			set { SetValue(CharacterSetProperty, value); }
		}
		public MSICheckSum MSICheckSum {
			get { return (MSICheckSum)GetValue(MSICheckSumProperty); }
			set { SetValue(MSICheckSumProperty, value); }
		}
		public string FNC1Substitute {
			get { return (string)GetValue(FNC1SubstituteProperty); }
			set { SetValue(FNC1SubstituteProperty, value); }
		}
		public bool HumanReadableText {
			get { return (bool)GetValue(HumanReadableTextProperty); }
			set { SetValue(HumanReadableTextProperty, value); }
		}
		public int Columns {
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		public PDF417CompactionMode PDF417CompactionMode {
			get { return (PDF417CompactionMode)GetValue(PDF417CompactionModeProperty); }
			set { SetValue(PDF417CompactionModeProperty, value); }
		}
		public DataMatrixCompactionMode DataMatrixCompactionMode {
			get { return (DataMatrixCompactionMode)GetValue(DataMatrixCompactionModeProperty); }
			set { SetValue(DataMatrixCompactionModeProperty, value); }
		}
		public QRCodeCompactionMode QRCodeCompactionMode {
			get { return (QRCodeCompactionMode)GetValue(QRCodeCompactionModeProperty); }
			set { SetValue(QRCodeCompactionModeProperty, value); }
		}
		public ErrorCorrectionLevel PDF417ErrorCorrectionLevel {
			get { return (ErrorCorrectionLevel)GetValue(PDF417ErrorCorrectionLevelProperty); }
			set { SetValue(PDF417ErrorCorrectionLevelProperty, value); }
		}
		public QRCodeErrorCorrectionLevel QRCodeErrorCorrectionLevel {
			get { return (QRCodeErrorCorrectionLevel)GetValue(QRCodeErrorCorrectionLevelProperty); }
			set { SetValue(QRCodeErrorCorrectionLevelProperty, value); }
		}
		public int Rows {
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}
		public float YToXRatio {
			get { return (float)GetValue(YToXRatioProperty); }
			set { SetValue(YToXRatioProperty, value); }
		}
		public bool TruncateSymbol {
			get { return (bool)GetValue(TruncateSymbolProperty); }
			set { SetValue(TruncateSymbolProperty, value); }
		}
		public DataMatrixSize MatrixSize {
			get { return (DataMatrixSize)GetValue(MatrixSizeProperty); }
			set { SetValue(MatrixSizeProperty, value); }
		}
		public QRCodeVersion Version {
			get { return (QRCodeVersion)GetValue(VersionProperty); }
			set { SetValue(VersionProperty, value); }
		}
		public int SegmentsInRow {
			get { return (int)GetValue(SegmentsInRowProperty); }
			set { SetValue(SegmentsInRowProperty, value); }
		}
		public DataBarType Type {
			get { return (DataBarType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		public BarCodePropertyProvider(BarCodeEdit barCodeEdit)
			: base(barCodeEdit) { }
	}
}
