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

using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting.Export.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Pdf.Common {
	public class PdfTextMeasuringProcessor : IDisposable {
		static bool HasUnicodeChars(string line) {
#if DEBUGTEST
			if(PdfGraphicsImpl.Test_ProcessSimpleStings)
				return true;
#endif
			for(int i = 0; i < line.Length; i++)
				if(line[i] >= '\u00FF') return true;
			return false;
		}
		float tabStopInterval;
		PdfMeasuringContext context;
		StringDirection stringProcessor;
		Font font;
		bool rtl;
		Dictionary<string, TextRun> textRunCache = new Dictionary<string, TextRun>();
		protected float TabStopInterval { get { return tabStopInterval; } }
		protected bool Rtl { get { return rtl; } }
		public PdfTextMeasuringProcessor(PdfMeasuringContext context, float tabStopInterval, Font font, bool rtl) {
			this.context = context;
			this.tabStopInterval = tabStopInterval;
			this.font = font;
			this.rtl = rtl;
		}
		public float GetTextWidth(string source) {
			string[] tabbedPieces = TextUtils.GetTabbedPieces(source);
			return this.tabStopInterval > 0 ?
				GetTextWidthWithTabs(tabbedPieces) :
				GetTextWidthWithoutTabs(tabbedPieces);
		}
		float GetTextWidthWithTabs(string[] tabbedPieces) {
			float width = 0;
			for(int i = 0; i < tabbedPieces.Length - 1; i++) {
				float pieceWidth = GetSimpleTextWidth(tabbedPieces[i]);
				int tabStopCount = (int)(pieceWidth / this.tabStopInterval) + 1;
				width += this.tabStopInterval * tabStopCount;
			}
			width += GetSimpleTextWidth(tabbedPieces[tabbedPieces.Length - 1]);
			return width;
		}
		float GetTextWidthWithoutTabs(string[] tabbedPieces) {
			float width = 0;
			foreach(string tabbedPiece in tabbedPieces)
				width += GetSimpleTextWidth(tabbedPiece);
			return width;
		}
		protected float GetSimpleTextWidth(string text) {
			return this.context.GetTextWidth(GetTextRun(text));
		}
		StringDirection GetStringProcessor() {
			if(stringProcessor == null && SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd && !PdfGraphics.EnableAzureCompatibility)
				stringProcessor = new StringDirection(font, rtl);
			return stringProcessor;
		}
		protected TextRun GetTextRun(string text) {
			TextRun run;
			if(!textRunCache.TryGetValue(text, out run)) {
				StringDirection stringProcessor = HasUnicodeChars(text) ? GetStringProcessor() : null;
				run = stringProcessor != null ? stringProcessor.ProcessString(text) : new TextRun() { Text = text };
			}
			return run;
		}
		public void Dispose() {
			if(stringProcessor != null) {
				stringProcessor.Dispose();
				stringProcessor = null;
			}
			font = null;
		}
	}
}
