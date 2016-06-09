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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.Tutorials {
	[ToolboxItem(false)]
	public class ColoredTextControl : Control {
		bool viewInfoValid;
		ColoredTextControlViewInfo viewInfo;
		string lexerKind, lexemProcessorKind;
		int textPadding;
		int bestFitMaxWidth;
		bool wordWrap, hintBorderVisible;
		public ColoredTextControl() {
			this.viewInfoValid = false;
			this.viewInfo = new ColoredTextControlViewInfo(this);
			this.lexerKind = "CSharp";
			this.lexemProcessorKind = "CSharp";
			this.Text = string.Empty;
			this.textPadding = 8;
			this.wordWrap = false;
			this.hintBorderVisible = true;
			this.bestFitMaxWidth = 300;
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(!viewInfoValid) {
				viewInfo.Calculate();
				viewInfoValid = true;
			}
			viewInfo.Paint(e.Graphics);
		}
	  [Browsable(true)]
	  public bool HintBorderVisible {
		 get { return hintBorderVisible; }
		 set { 
			if(value == hintBorderVisible) return;
			hintBorderVisible = value;
			Invalidate();
		 }
	  }
	  private void DrawHintBorder(Graphics g) {
		 g.DrawLine(Pens.White, new Point(0, 0), new Point(Width - 2, 0));
		 g.DrawLine(Pens.White, new Point(0, 0), new Point(0, Height - 2));
		 g.DrawLine(Pens.Black, new Point(Width - 1, 0), new Point(Width - 1, Height));
		 g.DrawLine(Pens.Black, new Point(0, Height - 1), new Point(Width, Height - 1));
	  }
		protected override void OnTextChanged(EventArgs e) {
			CalculateAndInvalidate();
		}
		protected override void OnSizeChanged(EventArgs e) {
			CalculateAndInvalidate();
		}
		public void CalculateAndInvalidate() {
			viewInfoValid = false;
			viewInfo.Calculate();
			Invalidate();
		}
		public Size CalcBestFit(int maxWidth) {
			return viewInfo.CalcBestFit(maxWidth);
		}
		public void BestFit() {
			this.Size = CalcBestFit(bestFitMaxWidth);
		}
		[System.ComponentModel.Browsable(true)]
		public string LexerKind {
			get { return lexerKind; }
			set {
				if(lexerKind == value) return;
				lexerKind = value;
				CalculateAndInvalidate();
			}
		}
		[System.ComponentModel.Browsable(true)]
		public string LexemProcessorKind {
			get { return lexemProcessorKind; }
			set {
				if(lexemProcessorKind == value) return;
				lexemProcessorKind = value;
				CalculateAndInvalidate();
			}
		}
		[System.ComponentModel.Browsable(true), System.ComponentModel.DefaultValue(4)]
		public int TextPadding {
			get { return textPadding; }
			set {
				if(textPadding == value) return;
				textPadding = value;
				CalculateAndInvalidate();
			}
		}
		[System.ComponentModel.Browsable(true), System.ComponentModel.DefaultValue(false)]
		public bool WordWrap {
			get { return wordWrap; }
			set {
				if(wordWrap == value) return;
				wordWrap = value;
				CalculateAndInvalidate();
			}
		}
		[System.ComponentModel.Browsable(true), System.ComponentModel.DefaultValue(300)]
		public int BestFitMaxWidth {
			get { return bestFitMaxWidth; }
			set { bestFitMaxWidth = value; }
		}
		public ColoredTextControlViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class ColoredTextControlViewInfo {
		TextPortionInfoCollection textPortions;
		ColoredTextControl control;
		TextPortionPopulator populator;
		ScrollBarInfo scrollInfo;
		public ColoredTextControlViewInfo(ColoredTextControl control) {
			textPortions = new TextPortionInfoCollection();
			populator = new TextPortionPopulator(this);
			this.control = control;		 
			this.scrollInfo = new ScrollBarInfo(this);
		}
		public void Calculate() {
			populator.Update();
			populator.PopulateText();
			scrollInfo.UpdateScrollInfo();
		}
		public void Paint(Graphics g) {
			foreach(TextPortionInfo info in textPortions)
				PaintTextPortion(g, info);
		}
		protected void PaintTextPortion(Graphics g, TextPortionInfo info) {
			g.DrawString(info.Text, info.FormatInfo.TextFont, new SolidBrush(info.FormatInfo.TextColor), info.X, info.Y);
		}
		public Size CalcBestFit(int maxWidth) {
			return populator.CalcBestFit(maxWidth);
		}
		public TextPortionInfoCollection TextPortions { get { return textPortions; } } 
		public ColoredTextControl Control { get { return control; } }
		public TextPortionPopulator Populator { get { return populator; } }
		public ScrollBarInfo ScrollInfo { get { return scrollInfo; } }
	}
	public class ScrollBarInfo {
		int scrollOffsetX, scrollOffsetY;
		int prevControlWidth, prevControlHeight;
		DevExpress.XtraEditors.HScrollBar horzScrollBar;
		DevExpress.XtraEditors.VScrollBar vertScrollBar;
		bool horzScrollBarVisible, vertScrollBarVisible;
		ColoredTextControlViewInfo viewInfo;
		Rectangle realClientRect;
		Panel pnlBottomRight;
		public bool ScrollBarVisible { get { return horzScrollBarVisible || vertScrollBarVisible; } }
		public ScrollBarInfo(ColoredTextControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			InitScrollBars();
			prevControlWidth = viewInfo.Control.Bounds.Width;
			prevControlHeight = viewInfo.Control.Bounds.Height;
			horzScrollBarVisible = vertScrollBarVisible = false;
			this.realClientRect = Rectangle.Empty;
			pnlBottomRight = new Panel();
			pnlBottomRight.Visible = false;
			pnlBottomRight.Parent = viewInfo.Control;
		}
		private void InitScrollBars() {
			horzScrollBar = new DevExpress.XtraEditors.HScrollBar();
			vertScrollBar = new DevExpress.XtraEditors.VScrollBar();
			InitScrollBar(horzScrollBar);
			InitScrollBar(vertScrollBar);
		}
		private void InitScrollBar(DevExpress.XtraEditors.ScrollBarBase scrollBar) {
			scrollBar.Visible = false;
			scrollBar.ValueChanged += new EventHandler(ScrollBarPositionChanged);
			scrollBar.Parent = viewInfo.Control;
		}
		private void CheckScrollBarsVisible() {
			UpdateRealClientRect();
			HorzScrollBarVisible = (viewInfo.Populator.TotalWidth > realClientRect.Width);
			VertScrollBarVisible = (viewInfo.Populator.TotalHeight > realClientRect.Height);
		}
		private void UpdateRealClientRect() {
			bool tmpVScrollVisibile, tmpHScrollVisible;
			tmpVScrollVisibile = tmpHScrollVisible = false;
			if(TextOverflowsClientAreaHorz(false))
				tmpHScrollVisible = true;
			if(TextOverflowsClientAreaVert(false))
				tmpVScrollVisibile = true;
			if(tmpHScrollVisible && TextOverflowsClientAreaVert(true))
				tmpVScrollVisibile = true;
			if(tmpVScrollVisibile && TextOverflowsClientAreaHorz(true))
				tmpHScrollVisible = true;
			UpdateRealClientRect(tmpHScrollVisible, tmpVScrollVisibile);
		}
		private void UpdateRealClientRect(bool hScrollVisible, bool vScrollVisible) {
			realClientRect = viewInfo.Control.Bounds;
			if(hScrollVisible)
				realClientRect.Height -= SystemInformation.HorizontalScrollBarHeight;
			if(vScrollVisible)
				realClientRect.Width -= SystemInformation.VerticalScrollBarWidth;
		}
		private bool TextOverflowsClientAreaHorz(bool useScrollbar) {
			int clientWidth = viewInfo.Control.Bounds.Width;
			if(useScrollbar)
				clientWidth -= SystemInformation.VerticalScrollBarWidth;
			return (viewInfo.Populator.TotalWidth > clientWidth);
		}
		private bool TextOverflowsClientAreaVert(bool useScrollbar) {
			int clientHeight = viewInfo.Control.Bounds.Height;
			if(useScrollbar)
				clientHeight -= SystemInformation.HorizontalScrollBarHeight;
			return (viewInfo.Populator.TotalHeight > clientHeight);
		}
		private bool NeedUpdate() {
			bool result = false;
			if(prevControlWidth != viewInfo.Control.Bounds.Width) {
				prevControlWidth = viewInfo.Control.Bounds.Width;
				result = true;
			}
			if(prevControlHeight != viewInfo.Control.Bounds.Height) {
				prevControlHeight = viewInfo.Control.Bounds.Height;
				result = true;
			}
			return result;
		}
		public void UpdateScrollInfo() {
			if(!NeedUpdate()) return;
			CheckScrollBarsVisible();		 
			UpdateScrollBars();
		}
		private void UpdateScrollBars() {
			Rectangle controlClientRect = viewInfo.Control.Bounds;
			horzScrollBar.Bounds = new Rectangle(controlClientRect.Left, controlClientRect.Bottom - SystemInformation.HorizontalScrollBarHeight, 
				realClientRect.Width, SystemInformation.HorizontalScrollBarHeight);
			vertScrollBar.Bounds = new Rectangle(controlClientRect.Right - SystemInformation.VerticalScrollBarWidth, controlClientRect.Top, 
				SystemInformation.VerticalScrollBarWidth, realClientRect.Height);
			horzScrollBar.Visible = horzScrollBarVisible;
			vertScrollBar.Visible = vertScrollBarVisible;
			UpdateScrollBarValues();
			CheckBottomRightPanel();
		}
		private void UpdateScrollBarValues() {
			horzScrollBar.Maximum = viewInfo.Populator.TotalWidth; 
			horzScrollBar.LargeChange = realClientRect.Width;
			vertScrollBar.Maximum = viewInfo.Populator.TotalHeight;
			vertScrollBar.LargeChange = realClientRect.Height;
			if(horzScrollBar.Visible)
				horzScrollBar.Value = Math.Min(horzScrollBar.Value, viewInfo.Populator.TotalWidth - realClientRect.Width);
			else horzScrollBar.Value = 0;
			if(vertScrollBar.Visible)
				vertScrollBar.Value = Math.Min(vertScrollBar.Value, viewInfo.Populator.TotalHeight - realClientRect.Height);
			else vertScrollBar.Value = 0;
		}
		private void CheckBottomRightPanel() {
			Rectangle controlClientRect = viewInfo.Control.Bounds;
			if(vertScrollBarVisible && horzScrollBarVisible) {
				pnlBottomRight.Bounds = new Rectangle(controlClientRect.Right - SystemInformation.VerticalScrollBarWidth, 
					controlClientRect.Bottom - SystemInformation.HorizontalScrollBarHeight, 
					SystemInformation.VerticalScrollBarWidth, 
					SystemInformation.HorizontalScrollBarHeight);
				pnlBottomRight.Visible = true;
			} 
			else 
				pnlBottomRight.Visible = false;
		}
		protected bool HorzScrollBarVisible {
			get { return horzScrollBarVisible; }
			set {
				if(horzScrollBarVisible == value) return;
				horzScrollBarVisible = value;
				if(horzScrollBarVisible) horzScrollBar.Value = 0;
			}
		}
		protected bool VertScrollBarVisible {
			get { return vertScrollBarVisible; }
			set {
				if(vertScrollBarVisible == value) return;
				vertScrollBarVisible = value;
				if(vertScrollBarVisible) vertScrollBar.Value = 0;
			}
		}
		private void ScrollBarPositionChanged(object sender, EventArgs e) {
			scrollOffsetX = horzScrollBar.Value;
			scrollOffsetY = vertScrollBar.Value;
			viewInfo.Control.CalculateAndInvalidate();
		}
		public int ScrollOffsetX { get { return scrollOffsetX; } }
		public int ScrollOffsetY { get { return scrollOffsetY; } }
	}
	public class TextPortionPopulator {
		ColoredTextControlViewInfo viewInfo;
		double currX, currY;
		int totalWidth, totalHeight;
		int offsetX, offsetY;
		double currLineHeight;
		int wordsInCurrLine;
		bool skipLine = false;
		LexerBase lexer;
		LexemProcessorBase lexemProcessor;
		LexerFactory lexerFactory;
		LexemProcessorFactory lexemProcessorFactory;
		public TextPortionPopulator(ColoredTextControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			offsetX = offsetY = 0;
			lexerFactory = new LexerFactory();
			lexemProcessorFactory = new LexemProcessorFactory(); 
		}
		public void Update() {
			UpdateOffsets();
			ResetCoords();
			InitLexer();
			InitLexemProcessor();
		}
		private void UpdateOffsets() {
			offsetX = viewInfo.Control.TextPadding;
			offsetY = viewInfo.Control.TextPadding;
		}
		private void ResetCoords() {
			totalWidth = 0;
			totalHeight = offsetY;
			currLineHeight = 0;
			currX = offsetX;
			currY = offsetY;
			wordsInCurrLine = 0;
		}
		private void UpdateCoordsAfterLine() {
			if(skipLine) {
				skipLine = false;
				return;
			}
			if(wordsInCurrLine == 0) 
				currLineHeight = GetEmptyLineHeight();
			if(totalWidth < currX) totalWidth = (int)Math.Ceiling(currX);
			currX = offsetX;
			currY += currLineHeight;
			currLineHeight = 0;
			wordsInCurrLine = 0;
		}
		private int GetEmptyLineHeight() {
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				SizeF stringSize = ginfo.Graphics.MeasureString("Qq", lexemProcessor.DefaultFont);
				return (int)Math.Ceiling(stringSize.Height);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		private void UpdateCoordsAfterToken(TextPortionInfo info) {
			string tmpString = "w" + info.Text + "w";
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				SizeF stringSize = ginfo.Graphics.MeasureString(tmpString, info.FormatInfo.TextFont);
				SizeF wwSize = ginfo.Graphics.MeasureString("ww", info.FormatInfo.TextFont);
				if(stringSize.Height > currLineHeight) currLineHeight = stringSize.Height;
				currX += Math.Ceiling(stringSize.Width - wwSize.Width);
				wordsInCurrLine++;
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		protected TextPortionInfoCollection TextPortions { get { return viewInfo.TextPortions; } }
		protected ColoredTextControl Control { get { return viewInfo.Control; } }
		public void PopulateText() {
			PopulateText(viewInfo.Control.Bounds.Width);
		}
		public void PopulateText(int maxWidth) {
			TextPortions.Clear();
			int aMark = 0;
			while(lexer.ReadNext()) {
				if(lexer.CurrentString == "\"") aMark++;
				if(lexemProcessor.Init(lexer.CurrentString)) {
					if(ProcessNewLine(lexer.CurrentString)) {
						UpdateCoordsAfterLine();
						continue;
					}
					if(NeedWrap(lexer.CurrentString, maxWidth))
						UpdateCoordsAfterLine();
					double x = currX - viewInfo.ScrollInfo.ScrollOffsetX;
					double y = currY - viewInfo.ScrollInfo.ScrollOffsetY;
					string realString = lexemProcessor.CorrectString(lexer.CurrentString);
					TextPortionInfo info = new TextPortionInfo((float)x, (float)y, realString, lexemProcessor.GetStringFormatInfo(lexer.CurrentString, aMark));
					TextPortions.Add(info);
					UpdateCoordsAfterToken(info);
				} else skipLine = lexemProcessor.InNotesComment;
			} 
			UpdateCoordsAfterLine();
			UpdateCoordsFinally();
		}
		private bool NeedWrap(string currentString, int maxWidth) {
			if(!viewInfo.Control.WordWrap) return false;
			if(currX + GetStringWidth(currentString) + offsetX + 4 >= maxWidth) {
				if(wordsInCurrLine > 1)
					return true;
			}
			return false;
		}
		private float GetStringWidth(string s) {
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				SizeF stringSize = ginfo.Graphics.MeasureString(s, lexemProcessor.GetStringFormatInfo(s, 0).TextFont);
				return stringSize.Width;
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		private void UpdateCoordsFinally() {
			totalHeight = (int)Math.Ceiling(currY);
			totalHeight += offsetY;
			totalWidth += offsetX + 4;
		}
		private bool ProcessNewLine(string currentString) {
			if(currentString != "\n" && currentString != "\r\n") return false;
			lexemProcessor.ProcessAuxiliaryToken("\n");
			return true;
		}
		public Size CalcBestFit(int maxWidth) {
			if(!viewInfo.Control.WordWrap)
				return new Size(totalWidth, totalHeight);
			if(maxWidth > totalWidth) 
				return new Size(totalWidth + 2, totalHeight + 2);
			TextPortionPopulator tmpPopulator = new TextPortionPopulator(viewInfo);
			tmpPopulator.Update();
			tmpPopulator.PopulateText(maxWidth);
			return new Size((int)tmpPopulator.TotalWidth + 2, (int)tmpPopulator.TotalHeight + 2);
		}
		private void InitLexer() {
			System.Reflection.ConstructorInfo cInfo = lexerFactory.GetConstructorByKind(Control.LexerKind);
			if(cInfo != null)
				lexer = cInfo.Invoke(new object[] { Control.Text } ) as LexerBase;
		}
		private void InitLexemProcessor() {
			System.Reflection.ConstructorInfo cInfo = lexemProcessorFactory.GetConstructorByKind(Control.LexemProcessorKind);
			if(cInfo != null)
				lexemProcessor = cInfo.Invoke(new object[] {} ) as LexemProcessorBase;
		}
		public int TotalWidth { get { return (int)totalWidth; } }
		public int TotalHeight { get { return (int)totalHeight; } }
	}
	public class TextPortionInfo {
		float x, y;
		string text;
		TextFormatInfo formatInfo;
		public TextPortionInfo(float x, float y, string text, TextFormatInfo formatInfo) {
			this.x = x;
			this.y = y;
			this.text = text;
			this.formatInfo = formatInfo;
		}
		public string Text { get { return text; } }
		public float X { get { return x; } }
		public float Y { get { return y; } }
		public TextFormatInfo FormatInfo { get { return formatInfo; } }
	}
	public class TextPortionInfoCollection : CollectionBase {
		public void Add(TextPortionInfo info) {
			List.Add(info);
		}
		public TextPortionInfo this[int index] {
			get { return this[index] as TextPortionInfo; } 
		}
	}
	public class TextFormatInfo {
		Color textColor;
		Font textFont;
		public TextFormatInfo(Color color, Font font) {
			this.textColor = color;
			this.textFont = font;
		}
		public Color TextColor { get { return textColor; } }
		public Font TextFont { get { return textFont; } }
		public static TextFormatInfo Empty {
			get {
				return new TextFormatInfo(Color.Black, new Font("Tahoma", 8));
			}
		}
	}
	public class LexerBase {
		string stringToProcess, currentString;
		int currentPos;
		public LexerBase(string stringToProcess) {
			this.stringToProcess = stringToProcess;
			this.currentPos = 0;
			this.currentString = string.Empty;
		}
		public bool ReadNext() {
			if(currentPos >= stringToProcess.Length - 1) return false;
			if(ExtractHeadingSeparator(stringToProcess.Substring(currentPos))) return true;
			ReadNextToken();
			return true;
		}
		private bool ExtractHeadingSeparator(string s) {
			foreach(string separator in GetSeparatorStrings()) {
				if(s.StartsWith(separator)) {
					currentString = separator;
					currentPos += separator.Length;
					return true;
				}
			}
			return false;
		}
		private void ReadNextToken() {
			int tempPos = currentPos;
			while(tempPos < stringToProcess.Length) {
				if(StringStartsWithSeparator(stringToProcess.Substring(tempPos)))
					break;
				tempPos++;
			}
			currentString = stringToProcess.Substring(currentPos, tempPos - currentPos);
			currentPos = tempPos;
		}
		private bool StringStartsWithSeparator(string s) {
			foreach(string separator in GetSeparatorStrings()) {
				if(s.StartsWith(separator))
					return true;
			}
			return false;
		}
		protected virtual string[] GetSeparatorStrings() {
			return new string[] {};
		}
		public string CurrentString { get { return currentString; } }
	}
	public class LexerLines : LexerBase {
		public LexerLines(string stringToProcess) : base(stringToProcess) {}
		protected override string[] GetSeparatorStrings() {
			return new string[] {"\n", "\r\n", Environment.NewLine };
		}
	}
	public class LexerLinesSpaces : LexerBase {
		public LexerLinesSpaces(string stringToProcess) : base(stringToProcess) {}
		protected override string[] GetSeparatorStrings() {
			return new string[] {"\n", "\r\n", " ", "\t"};
		}
	}
	public class LexerCSharp : LexerBase {
		public LexerCSharp(string stringToProcess) : base(stringToProcess) {}
		protected override string[] GetSeparatorStrings() {
			return new string[] {"\n", "\r\n", " ", "(", ")", "}", "{", "[", "]", "//", "\t", ";", ","};
		}
	}
	public class LexerVB : LexerBase {
		public LexerVB(string stringToProcess) : base(stringToProcess) {}
		protected override string[] GetSeparatorStrings() {
			return new string[] {"\n", "\r\n", " ", "(", ")", "[", "]", "'", "\t", ","};
		}
	}
	public class FactoryBase {
		Hashtable registeredEntries;
		public FactoryBase() {
			registeredEntries = new Hashtable();
			RegisterEntries();
		}
		protected virtual void RegisterEntries() { }
		protected virtual Type[] GetConstructorParamTypes() {
			return new Type[] {};
		}
		protected void RegisterEntry(string kind, Type type) {
			registeredEntries.Add(kind, type);
		}
		public System.Reflection.ConstructorInfo GetConstructorByKind(string kind) {
			if(registeredEntries.ContainsKey(kind)) {
				Type type = registeredEntries[kind] as System.Type;
				return type.GetConstructor(GetConstructorParamTypes());
			}
			return null;
		}
	}
	public class LexerFactory : FactoryBase {
		protected override Type[] GetConstructorParamTypes() {
			return new Type[] { Type.GetType("System.String") };
		}
		protected override void RegisterEntries() {
			RegisterEntry("Lines", Type.GetType("DevExpress.Tutorials.LexerLines"));
			RegisterEntry("LinesSpaces", Type.GetType("DevExpress.Tutorials.LexerLinesSpaces"));
			RegisterEntry("CSharp", Type.GetType("DevExpress.Tutorials.LexerCSharp"));
			RegisterEntry("VB", Type.GetType("DevExpress.Tutorials.LexerVB"));
		}
	}
	public class LexemProcessorFactory : FactoryBase {
		protected override void RegisterEntries() {
			RegisterEntry("CSharp", Type.GetType("DevExpress.Tutorials.LexemProcessorCSharp"));
			RegisterEntry("VB", Type.GetType("DevExpress.Tutorials.LexemProcessorVB"));
			RegisterEntry("BoldIfSharp", Type.GetType("DevExpress.Tutorials.LexemProcessorBoldIfSharp"));
		}
	}
	public abstract class LexemProcessorBase {
		public abstract TextFormatInfo GetStringFormatInfo(string s, int markCount);
		public virtual bool Init(string s) { return true; }
		public virtual bool InNotesComment { get { return false; } }
		public virtual void ProcessAuxiliaryToken(string s) {
		}
		public virtual string CorrectString(string s) {
			return s;
		}
		public abstract Font DefaultFont { get; }
	}
	public class LexemProcessorBoldIfSharp : LexemProcessorBase {
		TextFormatInfo boldInfo, regularInfo;
		bool currentWordInBold;
		public LexemProcessorBoldIfSharp() {
			this.currentWordInBold = false;
			this.boldInfo = new TextFormatInfo(SystemColors.InfoText, new Font("Tahoma", 8, FontStyle.Bold));
			this.regularInfo = new TextFormatInfo(SystemColors.InfoText, new Font("Tahoma", 8));
		}
		public override TextFormatInfo GetStringFormatInfo(string s, int markCount) {
			currentWordInBold = false;
			if(s.StartsWith("#"))
				currentWordInBold = true;
			return InternalGetStringFormatInfo();
		}
		private TextFormatInfo InternalGetStringFormatInfo() {
			if(currentWordInBold)
				return boldInfo;
			return regularInfo;
		}
		public override string CorrectString(string s) {
			if(s.StartsWith("#"))
				return s.Substring(1);
			return s;
		}
		public override Font DefaultFont { get { return regularInfo.TextFont; } }
	}
	public class LexemProcessorCode : LexemProcessorBase {
		bool inComment;
		bool inNotesComment;
		TextFormatInfo commentInfo, keywordInfo, defaultInfo;
		Font font;
		public LexemProcessorCode() {
			inComment = false;
			inNotesComment = false;
			font = new Font("Courier", (float)8.25);
			commentInfo = GetCommentInfo();
			keywordInfo = GetKeywordInfo();
			defaultInfo = GetDefaultInfo();
		}
		public override bool InNotesComment { get { return inNotesComment; } }
		protected virtual TextFormatInfo GetCommentInfo() {
			return new TextFormatInfo(Color.Green, font);
		}
		protected virtual TextFormatInfo GetKeywordInfo() {
			return new TextFormatInfo(Color.Blue, font);
		}
		protected virtual TextFormatInfo GetDefaultInfo() {
			return new TextFormatInfo(Color.Black, font);
		}
		public override string CorrectString(string s) {
			if(s.StartsWith("~"))
				return s.Substring(1);
			return s;
		}
		public override Font DefaultFont { get { return defaultInfo.TextFont; } }
		public override bool Init(string s) {
			if(s.StartsWith("~") && inNotesComment) inComment = true;
			if(IsStartNotesCommentString(s)) {
				inNotesComment = true;
				return false;
			}
			if(IsEndNotesCommentString(s)) {
				inNotesComment = false;
				inComment = false;
				return false;
			}
			return true;
		}
		public override TextFormatInfo GetStringFormatInfo(string s, int markCount) {
			if(inComment)
				return commentInfo;
			if(IsStartCommentString(s)) {
				inComment = true && markCount % 2 == 0;
				return commentInfo;
			}
			if(IsKeyword(s) && !inNotesComment)
				return keywordInfo;
			return defaultInfo;
		}
		public override void ProcessAuxiliaryToken(string s) {
			if(IsNewLine(s))
				inComment = false;
		}
		private bool IsNewLine(string s) {
			if(s == "\n") return true;
			return false;
		}
		protected virtual bool IsStartCommentString(string s) {
			return false;
		}
		protected virtual bool IsStartNotesCommentString(string s) {
			return false;
		}
		protected virtual bool IsEndNotesCommentString(string s) {
			return false;
		}
		private bool IsKeyword(string s) {
			foreach(string keyword in GetKeywords()) {
				if(s == keyword)
					return true;
			}
			return false;
		}
		protected virtual string[] GetKeywords() {
			return new string[] {};
		}
	}
	public class LexemProcessorCSharp : LexemProcessorCode {
		protected override bool IsStartCommentString(string s) {
			if(s == "//") return true;
			return false;
		}
		protected override bool IsStartNotesCommentString(string s) {
			return s.Trim() == "/*";
		}
		protected override bool IsEndNotesCommentString(string s) {
			return s.Trim() == "*/";
		}
		protected override string[] GetKeywords() {
			return new string[] { "abstract", "as", "base", "bool", "break", "byte", "case", 
									"catch", "char", "checked", "class", "const", "continue", 
									"decimal", "default", "delegate", "do", "double", "else", 
									"enum", "event", "explicit", "extern", "false", "finally",
									"fixed", "float", "for", "foreach", "goto", "if", "implicit",
									"in", "int", "interface", "internal", "is", "lock", "long",
									"namespace", "new", "null", "object", "operator", "out", "override",
									"params", "private", "protected", "public", "readonly", "ref", "return", 
									"sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
									"struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
									"ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
									"volatile", "void", "while"};
		}
	}
	public class LexemProcessorVB : LexemProcessorCode {
		protected override bool IsStartCommentString(string s) {
			if(s == "'") return true;
			return false;
		}
		protected override string[] GetKeywords() {
			return new string[] { "AddHandler", "And", "Auto", "ByVal", "CBool", "CDec", "Class", "CShort", "Date",
									"Delegate", "Double", "End", "Event", "For", "GetType", "Implements", "Integer", 
									"Lib", "Me", "MustOverride", "New", "NotInheritable", "Option", "Overloads", "Preserve",
									"Public", "REM", "Select", "Short", "Stop", "SyncLock", "True", "Until", "With", "AddressOf",
									"Ansi", "Boolean", "Call", "CByte", "CDbl", "CLng", "CSng", "Decimal", "Dim", "Each", "Enum",
									"Exit", "Friend", "GoTo", "Imports", "Interface", "Like", "Mod", "MyBase", "Next", "NotOverridable",
									"Optional", "Overridable", "Private", "RaiseEvent", "RemoveHandler", "Set", "Single", 
									"String", "Then", "Try", "Variant", "WithEvents", "AndAlso", "As", "ByRef", "Case", "CChar",
									"Char", "CObj", "CStr", "Declare", "DirectCast", "Else", "Erase", "False", "Function", "Handles", 
									"In", "Is", "Long", "Module", "MyClass", "Not", "Object", "Or", "Overrides", "Property", "ReadOnly",
									"Resume", "Shadows", "Static", "Structure", "Throw", "TypeOf", "When", "WriteOnly", "Alias", 
									"Assembly", "Byte", "Catch", "CDate", "CInt", "Const", "CType", "Default", "Do", "ElseIf", "Error",
									"Finally", "Get", "If", "Inherits", "Let", "Loop", "MustInherit", "Namespace", "Nothing", "Or",
									"OrElse", "ParamArray", "Protected", "ReDim", "Return", "Shared", "Step", "Sub", 
									"To", "Unicode", "While", "Xor"};
		}
	}
	public class ColoredHint : DevExpress.Tutorials.Win.ToolTipWindow {
		DevExpress.Tutorials.ColoredTextControl control;
		public ColoredHint() {
			InitializeComponent();
		}
		private void ColoredHintCalcSize(object sender, DevExpress.Tutorials.Win.ToolTipCalcSizeEventArgs e) {
			e.Size = control.CalcBestFit(300);
		}
		private void CreateTextControl() {
		}
		private void InitializeComponent() {
			this.control = new DevExpress.Tutorials.ColoredTextControl();
			this.SuspendLayout();
			this.control.Dock = System.Windows.Forms.DockStyle.Left;
			this.control.LexemProcessorKind = "BoldIfSharp";
			this.control.LexerKind = "Lines";
			this.control.Name = "control";
			this.control.Size = new System.Drawing.Size(808, 180);
			this.control.TabIndex = 0;
			this.control.Text = string.Empty;
			this.control.WordWrap = true;
		 this.control.HintBorderVisible = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(320, 180);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.control});
			this.Name = "ColoredHint";
			this.ToolTipCalcSize += new DevExpress.Tutorials.Win.ToolTipCalcSizeEventHandler(this.ColoredHintCalcSize);
			this.ResumeLayout(false);
		}
		public void ShowAtControl(Control ctrl) {
			this.Size = new Size(10, 10);
			Point hintPos = ctrl.PointToScreen(new Point(0, ctrl.Height));
			ShowTip(hintPos, hintPos);
			control.Update();
			this.Size = control.CalcBestFit(300);
			control.Size = this.Size;
		}
		public string ColoredHintText {
			get { return control.Text; }
			set { control.Text = value; }
		}
	  protected override void OnDeactivate(EventArgs e) {
		 MessageBox.Show("Works");
		 base.OnDeactivate(e);
	  }
	}
}
namespace DevExpress.Tutorials.Win {
	public class ToolTipCustomDrawEventArgs : EventArgs { 
		PaintEventArgs paintArgs;
		bool handled;
		public ToolTipCustomDrawEventArgs(PaintEventArgs paintArgs) {
			this.handled = false;
			this.paintArgs = paintArgs;
		}
		public bool Handled { 
			get { return handled; }
			set {
				handled = value;
			}
		}
		public PaintEventArgs PaintArgs { 
			get { return paintArgs; }
		}
	}
	public class ToolTipCalcSizeEventArgs : EventArgs {
		Size size;
		Point topPosition, bottomPosition;
		public ToolTipCalcSizeEventArgs(Point bottomPosition, Point topPosition, Size size) {
			this.topPosition = topPosition;
			this.bottomPosition = bottomPosition;
			this.size = size;
		}
		public Point TopPosition { 
			get { return topPosition; }
			set { topPosition = value;
			}
		}
		public Point BottomPosition { 
			get { return bottomPosition; }
			set { bottomPosition = value;
			}
		}
		public Size Size {
			get { return size; }
			set {
				size = value;
			}
		}
	}
	public class ToolTipCanShowEventArgs : EventArgs {
		string text;
		bool show;
		Point position;
		StringAlignment windowAlignment;
		public ToolTipCanShowEventArgs(bool show, string text, Point position) : this(show, text, position, StringAlignment.Near) {
		}
		public ToolTipCanShowEventArgs(bool show, string text, Point position, StringAlignment windowAlignment) {
			this.text = text;
			this.show = show;
			this.position = position;
			this.windowAlignment = windowAlignment;
		}
		public string Text { get { return text; } set { text = value; } }
		public bool Show { get { return show; } set { show = value; } }
		public Point Position { get { return position; } set { position = value; } }
		public StringAlignment WindowAlignment { get { return windowAlignment; } set { windowAlignment = value; } }
	}
	public delegate void ToolTipCanShowEventHandler(object sender, ToolTipCanShowEventArgs e);
	public class ToolTipEx : IDisposable {
		ToolTipWindow toolWindow;
		int initialDelay, autoPopDelay, reshowDelay;
		bool allowAutoPop;
		Timer initialTimer, autoPopTimer;
		public event ToolTipCanShowEventHandler ToolTipCanShow;
		public event ToolTipCustomDrawEventHandler ToolTipCustomDraw;
		public event ToolTipCalcSizeEventHandler ToolTipCalcSize;
		object activeObject;
		static AppearanceObject defaultStyle;
		AppearanceObject style;
		static ToolTipEx() {
			CreateStyle();
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferencesChanged);
		}
		static void OnUserPreferencesChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			CreateStyle();
		}
		static void CreateStyle() {
			defaultStyle = new AppearanceObject("ToolTip");
			defaultStyle.BackColor = SystemColors.Info;
			defaultStyle.ForeColor = SystemColors.InfoText;
		}
		public ToolTipEx() {
			this.style = null;
			this.toolWindow = new ToolTipWindow();
			this.ToolWindow.ToolTipCalcSize += new ToolTipCalcSizeEventHandler(OnToolTipCalcSize);
			this.ToolWindow.ToolTipCustomDraw += new ToolTipCustomDrawEventHandler(OnToolTipCustomDraw);
			this.activeObject = null;
			this.reshowDelay = 100;
			this.initialDelay = 500;
			this.autoPopDelay = 5000;
			this.allowAutoPop = true;
			this.initialTimer = new Timer();
			this.autoPopTimer = new Timer();
			AutoPopTimer.Tick += new EventHandler(OnAutoPopTimerTick);
			InitialTimer.Tick += new EventHandler(OnInitialTimerTick);
			AutoPopTimer.Interval = AutoPopDelay;
			InitialTimer.Interval = InitialDelay;
			UpdateToolStyle();
		}
		public virtual void Dispose() {
			if(ToolWindow != null) {
				ToolWindow.Dispose();
				this.toolWindow = null;
			}
		}
		public virtual void ObjectEnter(object obj) {
			ActiveObjectCore = obj;
		}
		public virtual void ObjectLeave(object newObject) {
			ActiveObjectCore = newObject;
		}
		protected virtual void OnInitialTimerTick(object sender, EventArgs e) {
			ShowHint();
		}
		protected virtual void OnAutoPopTimerTick(object sender, EventArgs e) {
			if(ToolWindow.Visible && AllowAutoPop) {
				ToolWindow.HideTip();
			}
		}
		public virtual object ActiveObject { get { return ActiveObjectCore; } }
		protected virtual object ActiveObjectCore {
			get { return activeObject; }
			set {
				if(ActiveObjectCore == value) return;
				object prevObject = ActiveObjectCore;
				activeObject = value;
				if(activeObject != null) {
					InitialTimer.Interval = prevObject != null ? ReshowDelay : InitialDelay;
					InitialTimer.Start();
				} else {
					HideHint();
				}
			}
		}
		protected virtual Timer InitialTimer { get { return initialTimer; } }
		protected virtual Timer AutoPopTimer { get { return autoPopTimer; } }
		public virtual ToolTipWindow ToolWindow { get { return toolWindow; } }
		protected virtual void OnToolTipCustomDraw(object sender, ToolTipCustomDrawEventArgs e) {
			if(ToolTipCustomDraw != null) ToolTipCustomDraw(this, e);
		}
		protected virtual void OnToolTipCalcSize(object sender, ToolTipCalcSizeEventArgs e) {
			if(ToolTipCalcSize != null) ToolTipCalcSize(this, e);
		}
		protected virtual AppearanceObject ActiveStyle {
			get { return Style == null ? defaultStyle : Style; }
		}
		protected virtual void UpdateToolStyle() {
			ToolWindow.Font = ActiveStyle.Font;
			ToolWindow.BackColor = ActiveStyle.BackColor;
			ToolWindow.ForeColor = ActiveStyle.ForeColor;
		}
		public virtual void ShowHint() {
			ToolTipCanShowEventArgs e = new ToolTipCanShowEventArgs(false, "", Point.Empty);
			if(ToolTipCanShow != null) ToolTipCanShow(this, e);
			ShowHint(e);
		}
		public virtual void ShowHint(ToolTipCanShowEventArgs e) {
			InitialTimer.Stop();
			if(e.Show) {
				ToolWindow.ToolTipAlignment = e.WindowAlignment;
				ToolWindow.Font = ActiveStyle.Font;
				ToolWindow.BackColor = ActiveStyle.BackColor;
				ToolWindow.ForeColor = ActiveStyle.ForeColor;
				ToolWindow.ToolTip = e.Text;
				ToolWindow.ShowTip(e.Position, new Point(e.Position.X, e.Position.Y + 10));
				if(AllowAutoPop) AutoPopTimer.Start();
			} else {
				HideHint();
			}
		}
		public virtual void HideHint() {
			this.activeObject = null;
			bool prevVisible = ToolWindow.Visible;
			ToolWindow.HideTip();
			AutoPopTimer.Stop();
			InitialTimer.Stop();
		}
		public virtual AppearanceObject Style {
			get { return style; }
			set {
				if(Style == value) return;
				style = value;
				UpdateToolStyle();
			}
		}
		public bool AllowAutoPop {
			get { return allowAutoPop; }
			set { allowAutoPop = value; }
		}
		public int InitialDelay { 
			get { return initialDelay; } 
			set { 
				if(value < 1) value = 1;
				if(InitialDelay == value) return;
				initialDelay = value; 
				InitialTimer.Interval = InitialDelay;
			}
		}
		public int ReshowDelay { 
			get { return reshowDelay; } 
			set { 
				if(value < 1) value = 1;
				if(ReshowDelay == value) return;
				reshowDelay = value; 
			}
		}
		public int AutoPopDelay {
			get { return autoPopDelay; }
			set { 
				if(value < 1) value = 1;
				if(AutoPopDelay == value) return;
				autoPopDelay = value;
				AutoPopTimer.Interval = AutoPopDelay;
			}
		}
	}
	public delegate void ToolTipCalcSizeEventHandler(object sender, ToolTipCalcSizeEventArgs e);
	public delegate void ToolTipCustomDrawEventHandler(object sender, ToolTipCustomDrawEventArgs e);
	[ToolboxItem(false)]
	public class ToolTipWindow : CustomTopForm {
		string toolTip;
		StringAlignment toolTipAlignment;
		Point bottomPosition, topPosition;
		bool mouseTransparent;
		public event ToolTipCustomDrawEventHandler ToolTipCustomDraw;
		public event ToolTipCalcSizeEventHandler ToolTipCalcSize;
		public ToolTipWindow() {
			mouseTransparent = true;
			toolTipAlignment = StringAlignment.Near;
			Font = SystemInformation.MenuFont;
			toolTip = "";
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			ControlBox = false;
			BackColor = SystemColors.Info;
			ForeColor = SystemColors.InfoText;
			SetStyle(ControlStyles.Opaque, true);
		}
		public void ShowTip(Point bottomPosition, Point topPosition) {
			this.bottomPosition = bottomPosition;
			this.topPosition = topPosition;
			ToolTipChanged(true);
			Visible = true;
		}
		public void HideTip() {
			Visible = false;
		}
		public virtual bool MouseTransparent { 
			get { return mouseTransparent; }
			set {
				mouseTransparent = value;
			}
		}
		public StringAlignment ToolTipAlignment {
			get { return toolTipAlignment; }
			set {
				if(ToolTipAlignment == value || value == StringAlignment.Center) return;
				toolTipAlignment = value;
				ToolTipChanged(false);
			}
		}
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT =(-1);
			base.WndProc(ref m);
			switch(m.Msg) {
				case WM_NCHITTEST : 
					if(MouseTransparent) m.Result = new IntPtr(HTTRANSPARENT);
					break;
			}
		}
		protected virtual Size CalcTipSize() {
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				ToolTipCalcSizeEventArgs sizeArgs = new ToolTipCalcSizeEventArgs(bottomPosition, topPosition, Size.Empty);
				sizeArgs.Size = ginfo.Cache.CalcTextSize(ToolTip, Font, TextOptions.DefaultStringFormat, 0).ToSize();
				sizeArgs.Size = new Size(sizeArgs.Size.Width + 4, sizeArgs.Size.Height + 4);
				if(ToolTipCalcSize != null) {
					ToolTipCalcSize(this, sizeArgs);
					bottomPosition = sizeArgs.BottomPosition;
					topPosition = sizeArgs.TopPosition;
				}
				return sizeArgs.Size;
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		public string ToolTip {
			get { return toolTip; }
			set {
				if(ToolTip == value) return;
				toolTip = value;
				ToolTipChanged(false);
			}
		}
		protected virtual void ToolTipChanged(bool makeVisible) {
			if(!Visible && !makeVisible) return;
			Point btm = bottomPosition, top = topPosition;
			Size size = CalcTipSize();
			if(size.Width < 1 || size.Height < 1) {
				Size = size;
				HideTip();
				return;
			}
			if(ToolTipAlignment == StringAlignment.Far) {
				btm.X -= size.Width;
				top.X -= size.Width;
			}
			Point loc = DevExpress.Utils.ControlUtils.CalcLocation(btm, top, size);
			ClientSize = size;
			Location = loc;
			Invalidate();
			if(makeVisible)
				Visible = true;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(ToolTipCustomDraw != null) {
				ToolTipCustomDrawEventArgs args = new ToolTipCustomDrawEventArgs(e);
				ToolTipCustomDraw(this, args);
				if(args.Handled) return;
			}
			Brush backBrush = new SolidBrush(BackColor), foreBrush = new SolidBrush(ForeColor);
			GraphicsCache cache = new GraphicsCache(e);
			e.Graphics.FillRectangle(backBrush, ClientRectangle);
			e.Graphics.FillRectangle(Brushes.Black, new Rectangle(0, Height - 1, Width, 1));
			e.Graphics.FillRectangle(Brushes.Black, new Rectangle(Width - 1, 0, 1, Height));
			cache.DrawString(ToolTip, Font, foreBrush, new Rectangle(2, 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4), TextOptions.DefaultStringFormat);
			backBrush.Dispose();
			foreBrush.Dispose();
		}
	}
}
