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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region BoxMeasurerBase (abstract class)
	public abstract class BoxMeasurer : IObjectMeasurer, IDisposable {
		#region Fields
		TextViewInfoCache textViewInfoCache;
		readonly DocumentModel documentModel;
		PieceTable pieceTable;
		bool isDisposed;
		#endregion
		protected BoxMeasurer(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.pieceTable = documentModel.MainPieceTable;
			this.textViewInfoCache = new TextViewInfoCache();
		}
		#region Properties
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		protected internal PieceTable PieceTable {
			get { return pieceTable; }
			set {
				Debug.Assert(Object.ReferenceEquals(value.DocumentModel, documentModel));
				pieceTable = value;
			}
		}
		 protected internal TextViewInfoCache TextViewInfoCache { get { return textViewInfoCache; } }
		public bool IsDisposed { get { return isDisposed; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (textViewInfoCache != null) {
				textViewInfoCache.Dispose();
				textViewInfoCache = null;
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal virtual void OnLayoutUnitChanged() {
			this.textViewInfoCache.Dispose();
			this.textViewInfoCache = new TextViewInfoCache();
		}
		protected internal virtual FontInfo GetFontInfo(FormatterPosition pos) {
			TextRunBase run = PieceTable.Runs[pos.RunIndex];
			int fontIndex = run.FontCacheIndex;
			return DocumentModel.FontCache[fontIndex];
		}
		public void MeasureText(BoxInfo boxInfo) {
			TextRunBase run = PieceTable.Runs[boxInfo.StartPos.RunIndex];
			run.Measure(boxInfo, this);
		}
		#region IObjectMeasurer implementation
		public virtual void MeasureText(BoxInfo boxInfo, string text, FontInfo fontInfo) {
#if DEBUGTEST
			Debug.Assert(boxInfo.StartPos.RunIndex == boxInfo.EndPos.RunIndex);
#endif
			boxInfo.Size = MeasureText(text, fontInfo);
		}
		public virtual Size MeasureText(string text, FontInfo fontInfo) {
			TextViewInfo textViewInfo = textViewInfoCache.TryGetTextViewInfo(text, fontInfo);
			if (textViewInfo == null) {
				textViewInfo = CreateTextViewInfo(null , text, fontInfo);
				textViewInfoCache.AddTextViewInfo(text, fontInfo, textViewInfo);
			}
			return textViewInfo.Size;
		}
		public virtual Size MeasureInlinePicture(InlinePictureRun run) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			Size actualSize = run.ActualSize;
			int width = Math.Max(1, unitConverter.ToLayoutUnits(actualSize.Width));
			int height = Math.Max(1, unitConverter.ToLayoutUnits(actualSize.Height));
			return new Size(width, height);
		}
		public Size MeasureFloatingObject(FloatingObjectAnchorRun run) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			Size actualSize = run.FloatingObjectProperties.ActualSize;
			int width = Math.Max(1, unitConverter.ToLayoutUnits(actualSize.Width));
			int height = Math.Max(1, unitConverter.ToLayoutUnits(actualSize.Height));
			return new Size(width, height);
		}
		#endregion
		public virtual void MeasureSpaces(BoxInfo boxInfo) {
#if DEBUGTEST
			Debug.Assert(boxInfo.StartPos.RunIndex == boxInfo.EndPos.RunIndex);
#endif
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(fontInfo.SpaceWidth * (boxInfo.EndPos.Offset - boxInfo.StartPos.Offset + 1), fontInfo.LineSpacing);
		}
		public virtual void MeasureSingleSpace(BoxInfo boxInfo) {
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(fontInfo.SpaceWidth, fontInfo.LineSpacing);
		}
		public virtual void MeasureTab(BoxInfo boxInfo) {
#if DEBUGTEST
			Debug.Assert(boxInfo.StartPos.RunIndex == boxInfo.EndPos.RunIndex);
#endif
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(0, fontInfo.LineSpacing);
		}
		public virtual void MeasureParagraphMark(BoxInfo boxInfo) {
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(fontInfo.PilcrowSignWidth, fontInfo.LineSpacing);
		}
		public virtual void MeasureSectionMark(BoxInfo boxInfo) {
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(fontInfo.PilcrowSignWidth, fontInfo.LineSpacing);
		}
		public virtual void MeasureLineBreakMark(BoxInfo boxInfo) {
			FontInfo fontInfo = boxInfo.GetFontInfo(PieceTable);
			boxInfo.Size = new Size(fontInfo.PilcrowSignWidth, fontInfo.LineSpacing);
		}
		public virtual void MeasurePageBreakMark(BoxInfo boxInfo) {
			boxInfo.Size = new Size(1, boxInfo.GetFontInfo(PieceTable).LineSpacing);
		}
		public virtual void MeasureColumnBreakMark(BoxInfo boxInfo) {
			boxInfo.Size = new Size(1, boxInfo.GetFontInfo(PieceTable).LineSpacing);
		}
		public virtual Size MeasureHyphen(FormatterPosition prevCharacterPos, BoxInfo hyphenBoxInfo) {
			FontInfo fontInfo = GetFontInfo(prevCharacterPos);
			TextViewInfo textViewInfo = CreateTextViewInfo(hyphenBoxInfo, HyphenBox.HyphenString, fontInfo);
			return textViewInfo.Size;
		}
		public virtual void BeginTextMeasure() {
		}
		public virtual void EndTextMeasure() {
		}
		protected internal abstract TextViewInfo CreateTextViewInfo(BoxInfo boxInfo, string text, FontInfo fontInfo);
		public abstract Rectangle[] MeasureCharactersBounds(string text, FontInfo fontInfo, Rectangle bounds);
		protected internal virtual bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth) {
			return false;
		}
		public virtual bool TryAdjustEndPositionToFit(BoxInfo boxInfo, string text, FontInfo fontInfo, int maxWidth) {
			return false;
		}
	}
	#endregion
}
