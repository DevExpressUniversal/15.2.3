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
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.BarCode {
	public class QRCodeGenerator : BarCode2DGenerator {
		QRCodeCompactionMode mode = QRCodeCompactionMode.AlphaNumeric;
		QRCodePatternProcessor patternProcessor;
		[DefaultValue(QRCodeCompactionMode.AlphaNumeric),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("QRCodeGeneratorCompactionMode"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.CompactionMode"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public QRCodeCompactionMode CompactionMode {
			get { return mode; }
			set {
				if(value == mode) return;
				mode = value;
				QRCodePatternProcessor processor = QRCodePatternProcessor.CreateInstance(mode);
				if(patternProcessor != null)
					((IPatternProcessor)processor).Assign(patternProcessor);
				patternProcessor = processor;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(QRCodeErrorCorrectionLevel.L),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("QRCodeGeneratorErrorCorrectionLevel"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.ErrorCorrectionLevel"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public QRCodeErrorCorrectionLevel ErrorCorrectionLevel {
			get { return ((QRCodePatternProcessor)PatternProcessor).ErrorCorrectionLevel; }
			set {
				((QRCodePatternProcessor)PatternProcessor).ErrorCorrectionLevel = value;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(QRCodeVersion.AutoVersion),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("QRCodeGeneratorVersion"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.Version"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public QRCodeVersion Version {
			get { return ((QRCodePatternProcessor)PatternProcessor).Version; }
			set {
				((QRCodePatternProcessor)PatternProcessor).Version = value;
				RefreshPatternProcessor();
			}
		}
		protected override IPatternProcessor PatternProcessor {
			get {
				if(patternProcessor == null)
					patternProcessor = QRCodePatternProcessor.CreateInstance(mode);
				return patternProcessor;
			}
		}
		protected override bool IsSquareBarcode {
			get { return true; }
		}
		public QRCodeGenerator() {
		}
		public QRCodeGenerator(QRCodeGenerator source) {
			Init(source);
		}
		void Init(QRCodeGenerator source) {
			CompactionMode = source.CompactionMode;
			PatternProcessor.Assign(source.PatternProcessor);
			Text = source.Text;
			BinaryData = source.BinaryData;
			RefreshPatternProcessor();
		}
		protected override bool TextCompactionMode() {
			return !BinaryCompactionMode();
		}
		protected override bool BinaryCompactionMode() {
			return CompactionMode == QRCodeCompactionMode.Byte && BinaryData.Length > 0;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new QRCodeGenerator(this);
		}
		protected override string GetValidCharSet() {
			return (PatternProcessor as QRCodePatternProcessor).GetValidCharset();
		}
		protected override bool IsValidText(string text) {
			if(CompactionMode == QRCodeCompactionMode.Byte)
				return true;
			bool dataValid = (PatternProcessor as QRCodePatternProcessor).IsValidData(text);
			return  dataValid && base.IsValidText(text);
		}
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.QRCode; }
		}
	}
}
