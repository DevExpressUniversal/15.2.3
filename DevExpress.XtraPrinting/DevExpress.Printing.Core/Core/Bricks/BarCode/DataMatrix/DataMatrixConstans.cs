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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BarCode;
namespace DevExpress.XtraPrinting.BarCode.Native {
	public class DataMatrixMatrixProperties {
		static Hashtable properties = new Hashtable();
		static DataMatrixMatrixProperties() {
			properties.Add(DataMatrixSize.Matrix10x10, new DataMatrixMatrixProperties(10, 10, 10, 10, 3, 1, 3, 5));
			properties.Add(DataMatrixSize.Matrix12x12, new DataMatrixMatrixProperties(12, 12, 12, 12, 5, 1, 5, 7));
			properties.Add(DataMatrixSize.Matrix14x14, new DataMatrixMatrixProperties(14, 14, 14, 14, 8, 1, 8, 10));
			properties.Add(DataMatrixSize.Matrix16x16, new DataMatrixMatrixProperties(16, 16, 16, 16, 12, 1, 12, 12));
			properties.Add(DataMatrixSize.Matrix12x26, new DataMatrixMatrixProperties(12, 26, 12, 26, 16, 1, 16, 14));
			properties.Add(DataMatrixSize.Matrix18x18, new DataMatrixMatrixProperties(18, 18, 18, 18, 18, 1, 18, 14));
			properties.Add(DataMatrixSize.Matrix20x20, new DataMatrixMatrixProperties(20, 20, 20, 20, 22, 1, 22, 18));
			properties.Add(DataMatrixSize.Matrix22x22, new DataMatrixMatrixProperties(22, 22, 22, 22, 30, 1, 30, 20));
			properties.Add(DataMatrixSize.Matrix24x24, new DataMatrixMatrixProperties(24, 24, 24, 24, 36, 1, 36, 24));
			properties.Add(DataMatrixSize.Matrix26x26, new DataMatrixMatrixProperties(26, 26, 26, 26, 44, 1, 44, 28));
			properties.Add(DataMatrixSize.Matrix32x32, new DataMatrixMatrixProperties(32, 32, 16, 16, 62, 1, 62, 36));
			properties.Add(DataMatrixSize.Matrix36x36, new DataMatrixMatrixProperties(36, 36, 18, 18, 86, 1, 86, 42));
			properties.Add(DataMatrixSize.Matrix40x40, new DataMatrixMatrixProperties(40, 40, 20, 20, 114, 1, 114, 48));
			properties.Add(DataMatrixSize.Matrix44x44, new DataMatrixMatrixProperties(44, 44, 22, 22, 144, 1, 144, 56));
			properties.Add(DataMatrixSize.Matrix48x48, new DataMatrixMatrixProperties(48, 48, 24, 24, 174, 1, 174, 68));
			properties.Add(DataMatrixSize.Matrix52x52, new DataMatrixMatrixProperties(52, 52, 26, 26, 204, 2, 102, 42));
			properties.Add(DataMatrixSize.Matrix64x64, new DataMatrixMatrixProperties(64, 64, 16, 16, 280, 2, 140, 56));
			properties.Add(DataMatrixSize.Matrix72x72, new DataMatrixMatrixProperties(72, 72, 18, 18, 368, 4, 92, 36));
			properties.Add(DataMatrixSize.Matrix80x80, new DataMatrixMatrixProperties(80, 80, 20, 20, 456, 4, 114, 48));
			properties.Add(DataMatrixSize.Matrix88x88, new DataMatrixMatrixProperties(88, 88, 22, 22, 576, 4, 144, 56));
			properties.Add(DataMatrixSize.Matrix96x96, new DataMatrixMatrixProperties(96, 96, 24, 24, 696, 4, 174, 68));
			properties.Add(DataMatrixSize.Matrix104x104, new DataMatrixMatrixProperties(104, 104, 26, 26, 816, 6, 136, 56));
			properties.Add(DataMatrixSize.Matrix120x120, new DataMatrixMatrixProperties(120, 120, 20, 20, 1050, 6, 175, 68));
			properties.Add(DataMatrixSize.Matrix132x132, new DataMatrixMatrixProperties(132, 132, 22, 22, 1304, 8, 163, 62));
			properties.Add(DataMatrixSize.Matrix144x144, new DataMatrixMatrixProperties(144, 144, 24, 24, 1558, 10, 156, 62)); 
			properties.Add(DataMatrixSize.Matrix8x18, new DataMatrixMatrixProperties(8, 18, 8, 18, 5, 1, 5, 7));
			properties.Add(DataMatrixSize.Matrix8x32, new DataMatrixMatrixProperties(8, 32, 8, 16, 10, 1, 10, 11));
			properties.Add(DataMatrixSize.Matrix12x36, new DataMatrixMatrixProperties(12, 36, 12, 18, 22, 1, 22, 18));
			properties.Add(DataMatrixSize.Matrix16x36, new DataMatrixMatrixProperties(16, 36, 16, 18, 32, 1, 32, 24));
			properties.Add(DataMatrixSize.Matrix16x48, new DataMatrixMatrixProperties(16, 48, 16, 24, 49, 1, 49, 28));
		}
		public static DataMatrixMatrixProperties GetProperties(DataMatrixSize matrixSize) {
			System.Diagnostics.Debug.Assert(matrixSize != DataMatrixSize.MatrixAuto, "matrix is not chosen");
			return (DataMatrixMatrixProperties)properties[matrixSize];
		}
		public static DataMatrixSize FindOptimalMatrixSize(int codewordsTotal) {
			System.Diagnostics.Debug.Assert(codewordsTotal <= GetProperties(DataMatrixSize.Matrix144x144).codewordsTotal,
				"number codewords shall not exceed codewords in 144x144 matrix");
			DataMatrixSize optimalMatrixSize = DataMatrixSize.Matrix144x144;
			foreach(DataMatrixSize matrixSize in Enum.GetValues(typeof(DataMatrixSize))) {
				if(matrixSize == DataMatrixSize.MatrixAuto) continue;
				if(codewordsTotal <= GetProperties(matrixSize).codewordsTotal &&
					GetProperties(matrixSize).codewordsTotal < GetProperties(optimalMatrixSize).codewordsTotal) {
					optimalMatrixSize = matrixSize;
				}
			}
			return optimalMatrixSize;
		}
		int symbolHeight;
		int symbolWidth;
		int regionHeight;
		int regionWidth;
		int codewordsTotal;
		int blocksTotal;
		int dataBlock;
		int rsBlock;
		public int SymbolHeight { get { return symbolHeight; } }
		public int SymbolWidth { get { return symbolWidth; } }
		public int RegionHeight { get { return regionHeight; } }
		public int RegionWidth { get { return regionWidth; } }
		public int CodewordsTotal { get { return codewordsTotal; } }
		public int BlocksTotal { get { return blocksTotal; } }
		public int DataBlock { get { return dataBlock; } }
		public int RsBlock { get { return rsBlock; } }
		public int GetDataBlockSize(int block) {
			return (symbolWidth != 144 || block < 8) ? dataBlock : dataBlock - 1;
		}
		DataMatrixMatrixProperties(int symbolHeight, int symbolWidth, int regionHeight, int regionWidth, int codewordsTotal,
				int blocksTotal, int dataBlock, int rsBlock) {
			this.symbolHeight = symbolHeight;
			this.symbolWidth = symbolWidth;
			this.regionHeight = regionHeight;
			this.regionWidth = regionWidth;
			this.codewordsTotal = codewordsTotal;
			this.blocksTotal = blocksTotal;
			this.dataBlock = dataBlock;
			this.rsBlock = rsBlock;
		}
	};
	public static class DataMatrixConstants {
		public const byte AsciiPad = 129;
		public const byte AsciiUpperShift = 235;
		public const byte AsciiLatchToC40 = 230;
		public const byte AsciiLatchToX12 = 238;
		public const byte AsciiLatchToText = 239;
		public const byte AsciiLatchToB256 = 231;
		public const byte AsciiLatchToEdifact = 240;
		public const byte C40X12TextUnlatch = 254;
		public const byte EdifactUnlatch = 95;
	}
}
