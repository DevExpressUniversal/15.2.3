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
using DevExpress.XtraPrinting.BarCode;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class PDF417Generator : BarCode2DGenerator {
		const string validCharset = "\r\n\t abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789;<>@[\\]_'‘~&!,:#-.$/+“%|*=(^)?{\"}";
		#region fields & properties
		float yToXRatio = PDF417Constants.MinYToXRatio;
		PDF417PatternProcessor pDF417PatternProcessor;
		PDF417CompactionMode mode = PDF417CompactionMode.Text;
		[DefaultValue(PDF417Constants.MinColumnsCount),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorColumns"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.Columns"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public int Columns {
			get {
				return ((PDF417PatternProcessor)PatternProcessor).Columns;
			}
			set {
				((PDF417PatternProcessor)PatternProcessor).Columns = value;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(PDF417Constants.MinRowsCount),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorRows"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.Rows"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public int Rows {
			get { return ((PDF417PatternProcessor)PatternProcessor).Rows; }
			set {
				((PDF417PatternProcessor)PatternProcessor).Rows = value;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(ErrorCorrectionLevel.Level2),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorErrorCorrectionLevel"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.ErrorCorrectionLevel"),
		XtraSerializableProperty,
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All)]
		public ErrorCorrectionLevel ErrorCorrectionLevel {
			get { return ((PDF417PatternProcessor)PatternProcessor).ErrorCorrectionLevel; }
			set {
				((PDF417PatternProcessor)PatternProcessor).ErrorCorrectionLevel = value;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorTruncateSymbol"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.TruncateSymbol"),
		XtraSerializableProperty,
		NotifyParentProperty(true)]
		public bool TruncateSymbol {
			get { return ((PDF417PatternProcessor)PatternProcessor).TruncateSymbol; }
			set { 
				((PDF417PatternProcessor)PatternProcessor).TruncateSymbol = value;
				RefreshPatternProcessor();
			}
		}
		[DefaultValue(PDF417Constants.MinYToXRatio),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorYToXRatio"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.YToXRatio"),
		XtraSerializableProperty,
		NotifyParentProperty(true),]
		public virtual float YToXRatio { 
			get { return yToXRatio; } 
			set { 
				yToXRatio = Math.Max(value, 3);
				RefreshPatternProcessor();
			} 
		}
		[DefaultValue(PDF417CompactionMode.Text),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorCompactionMode"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.PDF417Generator.CompactionMode"),
		XtraSerializableProperty,
		NotifyParentProperty(true)]
		public PDF417CompactionMode CompactionMode {
			get { return mode; }
			set {
				if (value != mode) {
					mode = value;
					PDF417PatternProcessor processor = PDF417PatternProcessor.CreateInstance(mode);
					if (pDF417PatternProcessor != null)
						((IPatternProcessor)processor).Assign(pDF417PatternProcessor);
					pDF417PatternProcessor = processor;
					RefreshPatternProcessor();
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PDF417GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.PDF417; }
		}
		protected override float YRatio { get { return YToXRatio; } }
		protected override IPatternProcessor PatternProcessor {
			get {
				if(pDF417PatternProcessor == null) {
					pDF417PatternProcessor = PDF417PatternProcessor.CreateInstance(mode);
					RefreshPatternProcessor();
				}
				return pDF417PatternProcessor;
			}
		}
		protected override bool IsSquareBarcode {
			get { return false; }
		}
		#endregion
		public PDF417Generator() {
		}
		public PDF417Generator(PDF417Generator source)
			: base(source) {
			Init(source);
		}
		protected override bool TextCompactionMode() {
			return CompactionMode == PDF417CompactionMode.Text;
		}
		protected override bool BinaryCompactionMode() {
			return CompactionMode == PDF417CompactionMode.Binary;
		}
		protected void Init(PDF417Generator source) {
			CompactionMode = source.CompactionMode;
			PatternProcessor.Assign(source.PatternProcessor);
			Text = source.Text;
			BinaryData = source.BinaryData;
			Rows = source.Rows;
			Columns = source.Columns;
			TruncateSymbol = source.TruncateSymbol;
			yToXRatio = source.YToXRatio;
			ErrorCorrectionLevel = source.ErrorCorrectionLevel;
			RefreshPatternProcessor();
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new PDF417Generator(this);
		}
		protected override string GetValidCharSet() { return validCharset; }
		protected override bool IsValidTextFormat(string text) {
			return text.Length <= PDF417Constants.MaxTextLength;
		}
		protected override bool IsValidPattern(ArrayList pattern) {
			return pattern.Count > 0;
		}
	}
}
