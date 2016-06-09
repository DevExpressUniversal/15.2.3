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
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class DataMatrixGenerator : BarCode2DGenerator {
		#region fields & properties
		DataMatrixCompactionMode mode = DataMatrixCompactionMode.ASCII;
		DataMatrixPatternProcessor ecc200PatternProcessor = null;
		[
		DefaultValue(DataMatrixSize.MatrixAuto),
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.MatrixSize"),
		XtraSerializableProperty,
		NotifyParentProperty(true)
		]
		public DataMatrixSize MatrixSize {
			get {
				return MatrixPatternProcessor.MatrixSize;
			}
			set {
				MatrixPatternProcessor.MatrixSize = value;
			}
		}
		[
		DefaultValue(DataMatrixCompactionMode.ASCII),
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.CompactionMode"),
		XtraSerializableProperty,
		NotifyParentProperty(true)
		]
		public virtual DataMatrixCompactionMode CompactionMode {
			get { return mode; }
			set {
				if(value != mode) {
					mode = value;
					MatrixPatternProcessor = DataMatrixPatternProcessor.CreateInstance(mode);
					RefreshPatternProcessor();
				}
			}
		}
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.DataMatrix; }
		}
		protected override float YRatio { get { return 1; } }
		protected override IPatternProcessor PatternProcessor {
			get {
				return MatrixPatternProcessor;
			}
		}
		DataMatrixPatternProcessor MatrixPatternProcessor {
			get {
				if(ecc200PatternProcessor == null) {
					ecc200PatternProcessor = DataMatrixPatternProcessor.CreateInstance(mode);
					RefreshPatternProcessor();
				}
				return ecc200PatternProcessor;
			}
			set {
				if(ecc200PatternProcessor != null)
					((IPatternProcessor)value).Assign(ecc200PatternProcessor);
				ecc200PatternProcessor = value;
				RefreshPatternProcessor();
			}
		}
		protected override bool IsSquareBarcode {
			get { return true; }
		}
		#endregion
		public DataMatrixGenerator() {
		}
		public DataMatrixGenerator(DataMatrixGenerator source)
			: base(source) {
			Init(source);
		}
		protected override bool TextCompactionMode() {
			return !BinaryCompactionMode();
		}
		protected override bool BinaryCompactionMode() {
			return CompactionMode == DataMatrixCompactionMode.Binary;
		}
		protected void Init(DataMatrixGenerator source) {
			CompactionMode = source.CompactionMode;
			PatternProcessor.Assign(source.PatternProcessor);
			Text = source.Text;
			BinaryData = source.BinaryData;
			MatrixSize = source.MatrixSize;
			RefreshPatternProcessor();
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new DataMatrixGenerator(this);
		}
		protected override string GetValidCharSet() { return DataMatrixASCIIPatternProcessor.ValidCharSet; }
	}
}
