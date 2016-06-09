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

using System.Windows.Media;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using System.Reflection;
namespace DevExpress.Xpf.Gauges.Native {
	public class SymbolsStateGenerator {
		const string symbolsImageFileNamePref = "DevExpress.Xpf.Gauges.Images.";
		readonly Dictionary<char, SymbolState> cache = new Dictionary<char, SymbolState>();
		readonly MatrixViewInternal viewInternal;
		Bitmap bitmap = null;
		bool ShouldCacheSymbol { get { return bitmap != null; } }
		public SymbolsStateGenerator(MatrixViewInternal viewInternal) {
			this.viewInternal = viewInternal;
			string fileName = "Symbols" + viewInternal.SymbolWidth.ToString() + "x" + viewInternal.SymbolHeight.ToString() + ".png";
			LoadSymbolsImage(fileName);
		}
		void LoadSymbolsImage(string symbolsImageFileName) {
			bitmap = ResourceImageHelper.CreateBitmapFromResources(symbolsImageFileNamePref + symbolsImageFileName, Assembly.GetExecutingAssembly());
		}
		int GetColor(int x, int y) {
			return bitmap != null ? bitmap.GetPixel(x, y).ToArgb() : 0;
		}
		SymbolState GetSymbolState(char symbol) {
			bool[] segments = new bool[viewInternal.SymbolWidth * viewInternal.SymbolHeight];
			int index = 0;
			int indexX = symbol & 0x00FF;
			int indexY = (symbol >> 8) & 0x00FF;
			for (int y = 0; y < viewInternal.SymbolHeight; y++) {
				for (int x = 0; x < viewInternal.SymbolWidth; x++) {
					int color = GetColor(indexX * viewInternal.SymbolWidth + x, indexY * viewInternal.SymbolHeight + y) & 0x00FFFFFF;
					segments[index++] = color > 0;
				}
			}
			return new SymbolState(symbol.ToString(), segments.Length, segments);
		}
		public SymbolState GetCachedSymbolState(char symbol) {
			SymbolState result;
			cache.TryGetValue(symbol, out result);
			if (result == null) {
				result = GetSymbolState(symbol);
				if (ShouldCacheSymbol)
					cache.Add(symbol, result);
			}
			return result;
		}
	}	
}
