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
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
namespace DevExpress.Utils.Frames {
	[ToolboxItem(false)]
	public class LabelInfoText {
		LabelInfoTextCollection collection;
		string text;
		Color color;
		bool active;
		bool hasCheckbox;
		bool checkBoxValue;
		bool bold;
		object tag;
		public LabelInfoText(LabelInfoTextCollection collection) : this(collection, "", Color.Empty, false) { }
		public LabelInfoText(LabelInfoTextCollection collection, string text, Color color, bool active) : this(collection, text, color, active, false) { }
		public LabelInfoText(LabelInfoTextCollection collection, string text, Color color, bool active, bool bold) {
			this.collection = collection;
			this.text = text;
			this.color = color;
			this.active = active;
			this.tag = null;
			this.bold = bold;
		}
		public string Text {
			get { return text; }
			set {
				if(value == null) value = "";
				if(Text == value) return;
				text = value;
				OnChanged();
			}
		}
		public Color Color {
			get { return color; }
			set {
				if(Color == value) return;
				color = value;
				OnChanged();
			}
		}
		public bool Active {
			get { return active; }
			set {
				if(Active == value) return;
				active = value;
				OnChanged();
			}
		}
		public bool Bold {
			get { return bold; }
			set {
				if(Bold == value) return;
				bold = value;
				OnChanged();
			}
		}
		public bool HasCheckBox {
			get { return hasCheckbox; }
			set {
				if(value == HasCheckBox) return;
				hasCheckbox = value;
				OnChanged();
			}
		}
		public bool CheckBoxValue {
			get { return checkBoxValue; }
			set {
				if(value == CheckBoxValue) return;
				checkBoxValue = value;
				OnChanged();
			}
		}
		public object Tag { get { return tag; } set { tag = value; } }
		protected LabelInfoTextCollection Collection { get { return collection; } }
		protected virtual void OnChanged() {
			if(Collection != null)
				Collection.OnChanged();
		}
	}
	[ListBindable(false)]
	public class LabelInfoTextCollection : CollectionBase {
		ILabelInfo label;
		public LabelInfoTextCollection(ILabelInfo label) {
			this.label = label;
		}
		public LabelInfoText Add() {
			return Add(string.Empty);
		}
		public LabelInfoText Add(string text) {
			return Add(text, Color.Empty);
		}
		public LabelInfoText Add(string text, Color color) {
			return Add(text, color, false);
		}
		public LabelInfoText Add(string text, bool active) {
			return Add(text, Color.Empty, active);
		}
		public LabelInfoText Add(string text, Color color, bool active) {
			return Add(text, color, active, false);
		}
		public LabelInfoText Add(string text, Color color, bool active, bool bold) {
			LabelInfoText item = new LabelInfoText(this, text, color, active, bold);
			List.Add(item);
			return item;
		}
		public ILabelInfo Label { get { return label; } }
		public LabelInfoText this[int index] { get { return InnerList[index] as LabelInfoText; } }
		public string Text { 
			get { 
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < Count; i ++) 
					sb.Append(this[i].Text);
				return sb.ToString();
			}
		}
		protected internal virtual void OnChanged() {
			if(Label != null)
				Label.TextInfoChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnChanged();
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			OnChanged();
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			OnChanged();
		}
	}
	public class LabelInfoTextViewInfoBase {
		LabelInfoText infoText;
		LabelInfoViewInfo viewInfo;
		int width;
		int scrollTop;
		public LabelInfoTextViewInfoBase(LabelInfoViewInfo viewInfo, LabelInfoText infoText, int width) {
			this.viewInfo = viewInfo;
			this.infoText = infoText;
			this.width = width;
			this.scrollTop = 0;
		}
		public LabelInfoText InfoText { get { return infoText; } }
		public LabelInfoViewInfo ViewInfo { get { return viewInfo; } }
		public int Index { get { return ViewInfo.IndexOf(this); } }
		public bool IsBreak { get { return InfoText.Text == LabelInfo.HalfLineBreak; } }
		public bool IsActive { get { return ViewInfo.ActiveItem == this; } }
		public bool IsBold { get { return InfoText != null && InfoText.Bold; } }
		public int Width { get { return width; } }
		public int ScrollTop { get { return scrollTop; } set { scrollTop = value; } }
		public virtual void Calculate(Graphics graphics, Font font, int fontHeight, ref int x, ref int y) {
		}
		public virtual void Draw(GraphicsCache cache, Font font, Color foreColor, StringFormat format) { }
		public virtual bool IsContains(Point pt) {
			return false;
		}
		public virtual int Bottom { get { return 0; } }
		public virtual int Top { get { return 0; } }
		public virtual int LineCount { get { return 0; } }
		public virtual int GetScrollHeight(int lineCount) {  return 0;  }
	}
	internal class LabelInfoTextLineViewInfo {
		string text;
		int x, y, width, height;
		bool isNewLine;
		public LabelInfoTextLineViewInfo(string text, int x, int y, int width, int height) {
			this.text = text;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		public string Text { get { return text; } }
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public Rectangle Bounds { get { return new Rectangle(X, Y, Width, Height); } }
		public bool IsNewLine { get { return isNewLine; } set { isNewLine = value; } }
	}
	internal class LabelInfoTextViewInfo : LabelInfoTextViewInfoBase {
		ArrayList list;
		Rectangle checkboxBounds;
		int checkBoxWidth;
		Point location;
		public LabelInfoTextViewInfo(LabelInfoViewInfo viewInfo, LabelInfoText infoText, int width, Point location) : base(viewInfo, infoText, width) {
			this.location = location;
			this.list = new ArrayList();
			this.checkboxBounds = Rectangle.Empty;
			this.checkBoxWidth = 0;
		}
		public int Count { get { return list.Count; } }
		public LabelInfoTextLineViewInfo this[int index] { get { return list[index] as LabelInfoTextLineViewInfo; } }
		public bool HasCheckBox { get { return InfoText.HasCheckBox; } }
		public bool CheckedValue { get { return InfoText.CheckBoxValue; } }
		public override int Bottom { get { return Count > 0 ? this[Count - 1].Bounds.Bottom : base.Bottom; } }
		public override int Top { get { return Count > 0 ? this[0].Bounds.Top : base.Top; } }
		public override int LineCount { 
			get { 
				if(Count == 0) return 0; 
				int lineCount = 0;
				for(int i = 0; i < Count; i ++) {
					if(this[i].IsNewLine) 
						lineCount ++;
				}
				return lineCount;
			} 
		}
		public override int GetScrollHeight(int lineCount) { 
			int scrollHeight = 0;
			for(int i = 0; i < Count; i ++) {
				if(i >= lineCount) break;
				if(this[i].IsNewLine) 
					scrollHeight +=this[i].Height;
			}
			return scrollHeight;
		}
		public virtual int GetLineHeight(Font font) {
			int res = (int)font.GetHeight();
			return res;
		}
		public override void Calculate(Graphics graphics, Font font, int fontHeight, ref int x, ref int y) {
			if(InfoText.Text == "") return;
			if(IsBreak) {
				y += (GetLineHeight(font) >> 1);
				return;
			}
			string drawText = InfoText.Text;
			bool isNewLine = x == LabelInfoViewInfo.TextIndent;
			while(drawText.Length > 0) {
				string words = GetNextWords(graphics, drawText, font, Width - x - GetCheckBoxWidth(graphics), x == LabelInfoViewInfo.TextIndent);
				if(words == "" || words[0] < ' ') {
					if(drawText.Length > 0 && drawText[0] == (char)13) {
						while(drawText.Length > 0 && drawText[0] == (char)13) {
							drawText = drawText.Remove(0, 1);
							while(drawText.Length > 0 && drawText[0] != (char)13 && drawText[0] <= ' ')
								drawText = drawText.Remove(0, 1);
							if(!isNewLine) 
								y += GetLineHeight(font);
							isNewLine = false;
						}
					}
					else {
						y += GetLineHeight(font);
						if(drawText.Length > 0 && drawText[0] != (char)13 && drawText[0] <= ' ')
							drawText = drawText.Remove(0, 1);
					}
					isNewLine = true;
					x = LabelInfoViewInfo.TextIndent;
					drawText = drawText.TrimStart();
					continue;
				}
				int wordsWidth = GetStringWidth(graphics, words, font);
				SetCheckboxBounds(graphics, ref x, y, (int)font.GetHeight());
				LabelInfoTextLineViewInfo item = new LabelInfoTextLineViewInfo(words, location.X + x, location.Y + y, wordsWidth, fontHeight);
				item.IsNewLine = isNewLine;
				list.Add(item);
				x += wordsWidth;
				isNewLine = x >= Width;
				if(x >= Width) {
					y += GetLineHeight(font);
					x = LabelInfoViewInfo.TextIndent;
				} 
				if(words == drawText)
					break;
				drawText = drawText.Substring(words.Length, drawText.Length - words.Length);
			}
		}
		FontStyle GetStyle(Font font) {
			FontStyle ret = font.Style;
			if(IsActive) ret |= FontStyle.Underline;
			if(IsBold) ret |= FontStyle.Bold;
			return ret;
		}
		public override void Draw(GraphicsCache cache, Font font, Color foreColor, StringFormat format) {
			if(IsBreak) return;
			DrawCheckBox(cache.Graphics);
			Font currentFont = IsActive || IsBold ? new Font(font, GetStyle(font)) : null;
			StringFormat sformat = format == null ? CreateStringFormat() : format;
			for(int i = 0; i < Count; i ++) {
				Rectangle bounds = this[i].Bounds;
				bounds.Y  = bounds.Y - ScrollTop;
				cache.DrawString(this[i].Text, currentFont != null ? currentFont : font, cache.GetSolidBrush(foreColor), bounds, sformat);
			}
			if(format == null) sformat.Dispose();
			if(currentFont != null)
				currentFont.Dispose();
		}
		StringFormat CreateStringFormat() {
			StringFormat format = new StringFormat(TextOptions.DefaultStringFormat);
			return format;
		}
		public override bool IsContains(Point pt) {
			pt.Y += ScrollTop;
			for(int i = 0; i < Count; i ++) {
				if(this[i].Bounds.Contains(pt))
					return true;
			}
			return this.checkboxBounds.Contains(pt);
		}
		string GetNextWords(Graphics graphics, string drawText, Font font, int wordsWidth, bool isStartLine) {
			string res = isStartLine ? GetNextWord(ref drawText) : "";
			string nextWord = "";
			while (drawText != "") {
				nextWord = GetNextWord(ref drawText);
				if(nextWord == "" || nextWord[0] < ' ') break;
				if(GetStringWidth(graphics, res + nextWord, font)> wordsWidth)
					break;
				res += nextWord;
			}
			return res;
		}
		int GetStringWidth(Graphics graphics, string drawText, Font font) {
			if(drawText.Length == 0) return 0;
			using(StringFormat format = CreateStringFormat()) {
				Font currentFont = IsActive || IsBold ? new Font(font, GetStyle(font)) : null;
				format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				int ret = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, drawText, currentFont != null ? currentFont : font, format, 0).Width);
				if(currentFont != null)
					currentFont.Dispose();
				return ret;
			}
		}
		string GetNextWord(ref string drawText) {
			if(drawText.Length == 0) return "";
			int i = 1;
			while(i < drawText.Length && !char.IsWhiteSpace(drawText[i]) && drawText[i] >= ' ')
				i ++;
			string word = drawText.Substring(0, i);
			drawText = drawText.Substring(i, drawText.Length - i);
			return word;
		}
		int GetCheckBoxWidth(Graphics graphics) {
			if(HasCheckBox && checkboxBounds.IsEmpty) {
				if(this.checkBoxWidth == 0) {
					CheckObjectInfoArgs infoArgs = new CheckObjectInfoArgs(AppearanceObject.ControlAppearance);
					CheckObjectPainter painter = CheckPainterHelper.GetPainter(ActiveLookAndFeelStyle.Flat);
					Size size = ObjectPainter.CalcObjectMinBounds(graphics, painter, infoArgs).Size;
					this.checkBoxWidth = size.Width + 1;  
				}
				return this.checkBoxWidth;
			}
			return 0;
		}
		const int CheckToTextIndent = 7;
		void SetCheckboxBounds(Graphics graphics, ref int x, int y, int height) {
			if(HasCheckBox && checkboxBounds.IsEmpty) {
				this.checkboxBounds = new Rectangle(x + 3, y, GetCheckBoxWidth(graphics), height); 
				x += this.checkboxBounds.Width + CheckToTextIndent;
			}
		}
		void DrawCheckBox(Graphics graphics) {
			if(checkboxBounds.IsEmpty) return;
			CheckObjectInfoArgs infoArgs = new CheckObjectInfoArgs(AppearanceObject.ControlAppearance);
			CheckObjectPainter painter = CheckPainterHelper.GetPainter(ActiveLookAndFeelStyle.UltraFlat);
			Rectangle scrollBounds = checkboxBounds;
			scrollBounds.Y -= ScrollTop;
			infoArgs.Bounds = scrollBounds;
			infoArgs.CheckState = CheckedValue ? CheckState.Checked : CheckState.Unchecked;
			if(IsActive) 
				infoArgs.State = ViewInfo.IsPressed ? ObjectState.Pressed : ObjectState.Hot;
			else infoArgs.State = ObjectState.Normal;
			ObjectPainter.CalcObjectBounds(graphics, painter, infoArgs);
			GraphicsCache cache = new GraphicsCache(graphics);
			ObjectPainter.DrawObject(cache, painter, infoArgs);
			cache.Dispose();
		}
	}
	public class LabelInfoViewInfo {
		public const int TextIndent = 2;
		ArrayList list;
		bool ready;
		bool isPressed;
		ILabelInfo label;
		LabelInfoTextViewInfoBase activeItem;
		bool needScrollBar;
		int topLine;
		int fontHeight;
		public LabelInfoViewInfo(ILabelInfo label) {
			this.list = new ArrayList();
			this.label = label;
			this.activeItem = null;
			this.isPressed = false;
			Clear();
		}
		public ILabelInfo Label { get { return label; } }
		public Font Font { get { return Label.Font; } }
		public LabelInfoTextCollection Texts { get { return Label.Texts; } } 
		public int Count { get { return list.Count; } }
		public int IndexOf(LabelInfoTextViewInfoBase textViewInfo) { return list.IndexOf(textViewInfo); }
		public LabelInfoTextViewInfoBase this[int index] { get { return list[index] as LabelInfoTextViewInfoBase; } }
		public bool IsReady { get { return ready; } }
		public bool IsPressed { get { return isPressed; } }
		public bool NeedScrollBar { get { return needScrollBar; } }
		public void Clear() {
			list.Clear();
			this.ready = false;
			this.needScrollBar = false;
			this.topLine = 0;
			this.fontHeight = 0;
		}
		public void Calculate(Graphics graphics) {
			if(IsReady) return;
			Calculate(graphics, Label.ClientSize.Width - 2 * TextIndent);
			if(Count > 0 && this[Count - 1].Bottom > Label.ClientSize.Height) {
				Calculate(graphics, Label.ClientSize.Width - Label.ScrollBarWidth - 2 * TextIndent);
				this.needScrollBar = true;
			}
		}
		public int FontHeight { get { return fontHeight; } }
		public int TopLine {
			get {	return topLine; }
			set {
				if(!IsReady || topLine == value) return;
				int scrollTop = 0;
				topLine = value;
				int lines = topLine;
				for(int i = 0; i < Count; i ++) {
					if(lines <= 0) break;
					scrollTop += this[i].GetScrollHeight(lines);
					lines -= this[i].LineCount;
				}
				for(int i = 0; i < Count; i ++)
					this[i].ScrollTop = scrollTop;
			}
		}
		protected void Calculate(Graphics graphics, int clientWidth) {
			Clear();
			this.ready = true;
			fontHeight = (int)XPaint.Graphics.CalcTextSize(graphics, "W", Font, new StringFormat(), int.MaxValue).Height;
			int x = TextIndent; int y = 0;
			for(int i = 0; i < Texts.Count; i ++) {
				list.Add(CreateViewInfo(Texts[GetTextIndex(i, Texts.Count)], clientWidth));
				this[i].Calculate(graphics, Font, FontHeight, ref x, ref y);
			}
		}
		protected virtual int GetTextIndex(int index, int count) {
			return index;
		}
		protected virtual LabelInfoTextViewInfoBase CreateViewInfo(LabelInfoText infoText, int width) {
			return new LabelInfoTextViewInfo(this, infoText, width, this.Label.ClientLocation);
		}
		public int LineCount { 
			get {
				if(!IsReady) return 0;
				int lineCount = 0;
				for(int i = 0; i < Count; i ++) {
					lineCount += this[i].LineCount;
				}
				return lineCount;
			}
		}
		public int Height {	get { return FontHeight * LineCount; } }
		public int VisibleLineCount {
			get {
				if(!IsReady || !NeedScrollBar) return 0;
				int bottom = this[Count - 1].Bottom;
				return (int)(Label.ClientSize.Height * LineCount / bottom);
			}
		}
		public void OnMouseMove(int x, int y) {
			if(!IsReady) return;
			Point pt = new Point(x, y);
			ActiveItem = GetItemByLocation(pt);
		}
		public void OnMouseDown(int x, int y) {
			if(!IsReady) return;
			this.isPressed = true;
		}
		public void OnMouseUp(int x, int y) {
			if(!IsReady) return;
			this.isPressed = false;
		}
		public void OnMouseLeave() {
			if(!IsReady) return;
			ActiveItem = null;
		}
		public LabelInfoTextViewInfoBase ActiveItem {
			get { return activeItem; }
			set {
				if(value != null && !value.InfoText.Active)
					value = null;
				if(value == ActiveItem) return;
				activeItem = value;
				Label.Invalidate();
				OnActiveItemChanged();
			}
		}
		protected virtual void OnActiveItemChanged() { }
		LabelInfoTextViewInfoBase GetItemByLocation(Point pt) {
			for(int i = 0; i < Count; i ++) {
				if(this[i].IsContains(pt))
					return this[i];
			}
			return null;
		}
	}
	public class LabelInfoItemClickEventArgs : EventArgs {
		LabelInfoText infoText;
		public LabelInfoItemClickEventArgs(LabelInfoText infoText) {
			this.infoText = infoText;
		}
		public LabelInfoText InfoText { get { return infoText; } }
	}
	public delegate void LabelInfoItemClickEvent(object sender, LabelInfoItemClickEventArgs e);
	public interface ILabelInfo {
		void TextInfoChanged();
		void Invalidate();
		LabelInfoTextCollection Texts { get; }
		int ScrollBarWidth { get; }
		Font Font { get ; }
		Size ClientSize { get; }
		Point ClientLocation { get; }
	}
	[ToolboxItem(false)]
	public class LabelInfo : Label, ILabelInfo {
		public const string HalfLineBreak = "<BREAK>";
		LabelInfoTextCollection texts;
		LabelInfoViewInfo viewInfo;
		DevExpress.XtraEditors.VScrollBar scrollBar;
		bool verticalScrollBar;
		bool autoHeight;
		int suspendTextChanges;
		public LabelInfo() {
			this.texts = new LabelInfoTextCollection(this);
			this.viewInfo = new LabelInfoViewInfo(this);
			this.suspendTextChanges = 0;
			this.verticalScrollBar = true;
			this.scrollBar = new DevExpress.XtraEditors.VScrollBar();
			this.scrollBar.Visible = false;
			this.scrollBar.Parent = this;
			this.scrollBar.Dock = DockStyle.Right;
			this.scrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);
			this.autoHeight = false;
		}
		static readonly object itemClick = new object();
		static readonly object itemDoubleClick = new object();
		public event LabelInfoItemClickEvent ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		public event LabelInfoItemClickEvent ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		public LabelInfoTextCollection Texts { get { return this.texts; } }
		public new string Text {
			get { return  Texts.Text; }
		}
		public void SuspendTextChanges() {
			this.suspendTextChanges ++;
		}
		public void ResumeTextChanges() {
			ResumeTextChanges(true);
		}
		public void ResumeTextChanges(bool refresh) {
			this.suspendTextChanges --;
			if(this.suspendTextChanges < 0)
				this.suspendTextChanges = 0;
			if(refresh)
				TextInfoChanged();
		}
		public bool IsTextChangesSuspend { get { return this.suspendTextChanges > 0; } }
		public override void Refresh() {
			ViewInfo.Clear();
			this.scrollBar.Visible = false;
			base.Refresh();
		}
		public void Clear() {
			texts.Clear();
			Refresh();
		}
		public bool VerticalScrollBar { get { return verticalScrollBar; } set { verticalScrollBar = value; } }
		public bool AutoHeight { 
			get { return autoHeight; } 
			set { 
				autoHeight = value;
				this.scrollBar.Value = this.scrollBar.Minimum;
				Refresh();
			}
		}
		public void TextInfoChanged() {
			if(!IsTextChangesSuspend) {
				Refresh();
				this.scrollBar.Value = this.scrollBar.Minimum;
			}
		}
		public Point ClientLocation { get { return Point.Empty; } }
		public int ScrollBarWidth {	get {	return this.scrollBar.Width; }	}
		protected virtual Color GetColor(LabelInfoText info) {
			return info.Color == Color.Empty ? Color.Black : info.Color;
		}
		protected virtual void OnItemClick(LabelInfoItemClickEventArgs e) {
			LabelInfoItemClickEvent handler = (LabelInfoItemClickEvent)this.Events[itemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnItemDoubleClick(LabelInfoItemClickEventArgs e) {
			LabelInfoItemClickEvent handler = (LabelInfoItemClickEvent)this.Events[itemDoubleClick];
			if(handler != null) handler(this, e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ViewInfo.OnMouseMove(e.X, e.Y);
			Cursor = ViewInfo.ActiveItem != null ? Cursors.Hand : Cursors.Arrow;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left) return;
			ViewInfo.OnMouseDown(e.X, e.Y);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button != MouseButtons.Left) return;
			ViewInfo.OnMouseUp(e.X, e.Y);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			if(ViewInfo.ActiveItem != null)
				OnItemClick(new LabelInfoItemClickEventArgs(ViewInfo.ActiveItem.InfoText));
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			if(ViewInfo.ActiveItem != null)
				OnItemDoubleClick(new LabelInfoItemClickEventArgs(ViewInfo.ActiveItem.InfoText));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.OnMouseLeave();
		}
		protected override void OnResize(EventArgs e) {
			ViewInfo.Clear();
			this.scrollBar.Value = this.scrollBar.Minimum;
			base.OnResize(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			ViewInfo.Calculate(e.Graphics);
			SetupScrollBar();
			ViewInfo.TopLine = this.scrollBar.Value;
			if(AutoHeight && Height != ViewInfo.Height)
				Height = ViewInfo.Height;
			using(GraphicsCache cache = new GraphicsCache(e)) {
				for(int i = 0; i < ViewInfo.Count; i ++) {
					ViewInfo[i].Draw(cache, Font, GetColor(ViewInfo[i].InfoText), null);
				}
			}
		}
		void SetupScrollBar() {
			if(!VerticalScrollBar || AutoHeight) return;
			bool oldVisible = this.scrollBar.Visible;
			if(ViewInfo.NeedScrollBar) {
				this.scrollBar.Maximum = ViewInfo.LineCount + 1;
				this.scrollBar.LargeChange = ViewInfo.VisibleLineCount;
			}
			this.scrollBar.Visible = ViewInfo.NeedScrollBar;
			if(ViewInfo.NeedScrollBar != oldVisible)
				this.Invalidate(true);
		}
		void OnScrollBarValueChanged(object sender, EventArgs e) {
			Invalidate();
		}
		LabelInfoViewInfo ViewInfo { get { return viewInfo; } }
	}
}
