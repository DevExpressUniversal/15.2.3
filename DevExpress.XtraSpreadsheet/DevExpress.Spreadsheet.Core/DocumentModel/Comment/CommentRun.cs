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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region CommentRun
	public class CommentRun : SpreadsheetUndoableIndexBasedObject<RunFontInfo>, IRunFontInfo {
		string text;
		public CommentRun(Worksheet sheet)
			: base(sheet) {
		}
		public CommentRun(Worksheet sheet, string text)
			: this(sheet) {
			this.text = text;
		}
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		public IRunFontInfo FontInfo { get { return this; } }
		#region Text
		public string Text {
			get { return text; }
			set {
				if (string.Compare(text, value) == 0)
					return;
				DocumentHistory history = DocumentModel.History;
				CommentChangeRunTextHistoryItem historyItem = new CommentChangeRunTextHistoryItem(this, text, value);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		#endregion
		#region SetTextCore
		public void SetTextCore(string value) {
			text = value;
		}
		#endregion
		#region GetBatchUpdateChangeActions
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region GetCache
		protected override UniqueItemsCache<RunFontInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.FontInfoCache;
		}
		#endregion
		#region ApplyChanges
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Sheet.ApplyChanges(changeActions);
		}
		#endregion
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return Info.Name; }
			set {
				SetPropertyValue(SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(RunFontInfo info, string value) {
			info.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get {
				DocumentModel Workbook = Sheet.Workbook;
				return Workbook.Cache.ColorModelInfoCache[Info.ColorIndex].ToRgb(Workbook.StyleSheet.Palette, Workbook.OfficeTheme.Colors);
			}
			set {
				if ((this as IRunFontInfo).Color == value)
					return;
				SetPropertyValue(SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(RunFontInfo info, Color value) {
			DocumentModel Workbook = Sheet.Workbook;
			ColorModelInfo colorInfo = new ColorModelInfo();
			colorInfo.Rgb = value;
			info.ColorIndex = Workbook.Cache.ColorModelInfoCache.AddItem(colorInfo);
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return Info.Bold; }
			set {
				if ((this as IRunFontInfo).Bold == value)
					return;
				SetPropertyValue(SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(RunFontInfo info, bool value) {
			info.Bold = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return Info.Condense; }
			set {
				if ((this as IRunFontInfo).Condense == value)
					return;
				SetPropertyValue(SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(RunFontInfo info, bool value) {
			info.Condense = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return Info.Extend; }
			set {
				if ((this as IRunFontInfo).Extend == value)
					return;
				SetPropertyValue(SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(RunFontInfo info, bool value) {
			info.Extend = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return Info.Italic; }
			set {
				if ((this as IRunFontInfo).Italic == value)
					return;
				SetPropertyValue(SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(RunFontInfo info, bool value) {
			info.Italic = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return Info.Outline; }
			set {
				if ((this as IRunFontInfo).Outline == value)
					return;
				SetPropertyValue(SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(RunFontInfo info, bool value) {
			info.Outline = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return Info.Shadow; }
			set {
				if ((this as IRunFontInfo).Shadow == value)
					return;
				SetPropertyValue(SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(RunFontInfo info, bool value) {
			info.Shadow = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return Info.StrikeThrough; }
			set {
				if ((this as IRunFontInfo).StrikeThrough == value)
					return;
				SetPropertyValue(SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(RunFontInfo info, bool value) {
			info.StrikeThrough = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return Info.Charset; }
			set {
				if ((this as IRunFontInfo).Charset == value)
					return;
				SetPropertyValue(SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(RunFontInfo info, int value) {
			info.Charset = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return Info.FontFamily; }
			set {
				if ((this as IRunFontInfo).FontFamily == value)
					return;
				SetPropertyValue(SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(RunFontInfo info, int value) {
			info.FontFamily = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return Info.Size; }
			set {
				if ((this as IRunFontInfo).Size == value)
					return;
				SetPropertyValue(SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(RunFontInfo info, double value) {
			info.Size = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return Info.SchemeStyle; }
			set {
				if ((this as IRunFontInfo).SchemeStyle == value)
					return;
				SetPropertyValue(SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(RunFontInfo info, XlFontSchemeStyles value) {
			info.SchemeStyle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return Info.Script; }
			set {
				if ((this as IRunFontInfo).Script == value)
					return;
				SetPropertyValue(SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(RunFontInfo info, XlScriptType value) {
			info.Script = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return Info.Underline; }
			set {
				if ((this as IRunFontInfo).Underline == value)
					return;
				SetPropertyValue(SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(RunFontInfo info, XlUnderlineType value) {
			info.Underline = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public void CopyFrom(CommentRun source) {
			base.CopyFrom(source);
			Text = source.Text;
		}
		public Font GetFont() {
			return Info.GetFontInfo(DocumentModel.FontCache).Font;
		}
	}
	#endregion
}
