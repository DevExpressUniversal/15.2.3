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
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public abstract class MatrixView : SymbolViewBase {
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public abstract class MatrixViewInternal : SymbolViewInternal {
		readonly SymbolsStateGenerator symbolsStateGenerator;
		protected internal abstract int SymbolWidth { get; }
		protected internal abstract int SymbolHeight { get; }
		public MatrixViewInternal() {
			this.symbolsStateGenerator = new SymbolsStateGenerator(this);
		}
		protected override SymbolsAnimation GetDefaultAnimation() {
			return new BlinkingAnimation();
		}
		protected override List<SymbolState> GetSymbolsStateByDisplayText(List<string> textBySymbols) {
			List<SymbolState> result = new List<SymbolState>();
			foreach (string symbolText in textBySymbols) {
				SymbolSegmentsMapping customMapping = GetCustomSegmentsMapping(symbolText[0]);
				if (customMapping != null)
					result.Add(new SymbolState(symbolText, Math.Max(customMapping.SegmentsStates.States.Length, SymbolHeight*SymbolWidth), customMapping.SegmentsStates.States));
				else
					result.Add(symbolsStateGenerator.GetCachedSymbolState(symbolText[0]));
			}
			return result;
		}
		protected internal override List<string> SeparateTextToSymbols(string text) {
			List<string> symbols = new List<string>();
			if (text != null) {
				for (int i = 0; i < text.Length; i++)
					symbols.Add(text[i].ToString());
			}
			return symbols;
		}
	}
}
