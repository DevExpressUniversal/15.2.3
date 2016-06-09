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
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Drawing;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Paint;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraEditors.Filtering {
	public interface ILabelInfoEx : ILabelInfo  {
		Node Owner { get; }
	}
	public class FilterControlLabelInfo : ILabelInfoEx {
		Node node;
		LabelInfoTextCollection texts;
		FilterLabelInfoViewInfo viewInfo;
		int suspendTextChanges;
		int textWidth = 0;
		Rectangle textBounds, nodeBounds;
		public int NodeWidth { get; set; } 
		public FilterControlLabelInfo(Node node) {
			this.node = node;
			this.texts = new LabelInfoTextCollection(this);
			this.viewInfo = new FilterLabelInfoViewInfo(this);
			this.suspendTextChanges = 0;			
			Top = FilterControlViewInfo.TopIndent;
			Left = FilterControlViewInfo.LeftIndent;
		}
		protected FilterControl OwnerControl { get { return Model.Control; } }
		protected WinFilterTreeNodeModel Model { get { return (WinFilterTreeNodeModel)Owner.Model; } }
		public int Top { get; set; }
		public int Left { get; set; }		
		public int Height {
			get {
				int childrenHeight = 0;
				foreach (Node subNode in GetChildren()) {
					childrenHeight += Model[subNode].Height;
				}
				return ControlViewInfo != null ? ControlViewInfo.NodeHeight + childrenHeight : childrenHeight;
			}
		}
		public int Width {
			get {
				int maxChildWidth = TextWidth;
				foreach (Node subNode in GetChildren()) {
					int subNodeWidth = Model[subNode].Width;
					if (subNodeWidth > maxChildWidth)
						maxChildWidth = subNodeWidth;
				}
				return maxChildWidth + ControlViewInfo.LevelIndent;
			}
		}
		public string Text {
			get { return texts.Text; }
		}
		int TextWidth {
			get { return textWidth; }
			set { textWidth = value; }
		}
		IList<INode> GetChildren() { return node.GetChildren(); }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle NodeBounds { get { return nodeBounds; } }
		public Size TextSize { get { return textBounds.Size; } }
		public Point TextLocation { get { return textBounds.Location; } }
		public LabelInfoTextCollection Texts { get { return this.texts; } }
		internal FilterControlViewInfo ControlViewInfo {
			get {
				if (node.Model == null) return null;
				return ((WinFilterTreeNodeModel)node.Model).Control.FilterViewInfo;
			}
		}
		public void CalcTextBounds(ControlGraphicsInfoArgs info) {
			foreach (Node subNode in GetChildren()) Model[subNode].CalcTextBounds(info);
			CalcTextBoundsCore(info);
			NodeWidth = Width;
		}
		public void ClearViewInfo() {
			ClearViewInfoCore();
			foreach (Node node in GetChildren()) {
				Model[node].ClearViewInfo();
			}
		}
		void ClearViewInfoCore() {
			textWidth = 0;
			NodeWidth = Width;
		}
		void CalcTextBoundsCore(ControlGraphicsInfoArgs info) {
			if (TextWidth == 0) {
				TextWidth = LabelInfoHelper.GetFullWidth(node, info, Model[node], info.ViewInfo.Appearance.Font, info.ViewInfo.Appearance.GetStringFormat()) + 6;
				CalcRects(info);
			}
		}
		bool IsRightToLeft { get { return viewInfo.OwnerControl.FilterViewInfo.RightToLeft; } }
		protected virtual int GetLeft(ControlGraphicsInfoArgs info) {
			if(IsRightToLeft) return info.ViewInfo.ContentRect.Right - Left - TextWidth;
			return Left;
		}
		void CalcRects(ControlGraphicsInfoArgs info) {
			textBounds = new Rectangle(GetLeft(info), Top + ControlViewInfo.textIndent, TextWidth, ControlViewInfo.SingleLineHeight);
			nodeBounds = new Rectangle(GetLeft(info), Top, TextWidth, ControlViewInfo.NodeHeight - ControlViewInfo.NodeSeparatorHeight);
			Debug.Assert(Model[node] != null);
			Model[node].ViewInfo.Clear();
		}
		public void SuspendTextChanges() {
			this.suspendTextChanges++;
		}
		public void ResumeTextChanges() {
			ResumeTextChanges(true);
		}
		public void ResumeTextChanges(bool refresh) {
			this.suspendTextChanges--;
			if(this.suspendTextChanges < 0)
				this.suspendTextChanges = 0;
			if(refresh)
				TextInfoChanged();
		}
		public bool IsTextChangesSuspend { get { return this.suspendTextChanges > 0; } }
		public virtual void Refresh() {
			ViewInfo.Clear();
		}
		public void UpdateBounds() {
			int currentTop = Top + ControlViewInfo.NodeHeight;
			foreach (Node subNode in GetChildren()) {
				Model[subNode].Top = currentTop;
				Model[subNode].Left = Left + ControlViewInfo.LevelIndent;
				Model[subNode].UpdateBounds();
				currentTop += Model[subNode].Height;
			}
		}
		public void Clear() {
			texts.Clear();
			Refresh();
		}
		#region Painting
		public FilterLabelInfoViewInfo ViewInfo { get { return viewInfo; } }
		public virtual void Paint(ControlGraphicsInfoArgs info) {
			ViewInfo.Calculate(info.Graphics);
			ViewInfo.TopLine = 0;
			for(int i = 0; i < ViewInfo.Count; i++)
				ViewInfo[i].Draw(info.Cache, info.ViewInfo.Appearance.GetFont(), ViewInfo[i].InfoText.Color, info.ViewInfo.Appearance.GetStringFormat());
		}
		#endregion
		#region LabelCreator
		public void CreateLabelInfoTexts() {
			SuspendTextChanges();
			try {
				Texts.Clear();
				foreach(NodeEditableElement element in Owner.Elements) {
					AddLabelInfo(element);
				}
			} finally {
				ResumeTextChanges();
			}
			Invalidate();
		}
		protected void AddLabelInfo(NodeEditableElement element) {
			AddTextLabelInfo(element, element.TextBefore);
			AddLabelInfoText(element);
			AddTextLabelInfo(element, element.TextAfter);
		}
		protected void AddTextLabelInfo(NodeEditableElement element, string text) {
			if(string.IsNullOrEmpty(text)) return;
			AddLabelInfoText(text, element, ElementType.None);
		}
		protected Color GetActiveColor(NodeEditableElement element) {
			FilterControlViewInfo viewInfo = OwnerControl.FilterViewInfo;
			if(!OwnerControl.Enabled) return viewInfo.DisabledColor;
			switch(element.ElementType) {
				case ElementType.Property: return viewInfo.FieldNameColor;
				case ElementType.ItemCollection:
				case ElementType.Value:
					return element.IsEmpty ? viewInfo.EmptyValueColor : viewInfo.ValueColor;
				case ElementType.AggregateOperation:
				case ElementType.Operation: return viewInfo.OperatorColor;
				case ElementType.Group: return viewInfo.GroupOperatorColor;
				case ElementType.AdditionalOperandParameter:
				case ElementType.AdditionalOperandProperty:
				case ElementType.AggregateProperty:
					return element.IsEmpty ? viewInfo.EmptyValueColor : viewInfo.FieldNameColor;
			}
			return viewInfo.PaintAppearance.ForeColor;
		}
		void AddLabelInfoText(NodeEditableElement element) {
			AddLabelInfoText(element.Text, element, element.ElementType);
		}
		void AddLabelInfoText(string text, NodeEditableElement element, ElementType type) {
			if(type != ElementType.None) {
				LabelInfoText info = Texts.Add(text, GetActiveColor(element), true);
				info.Tag = element;
			} else {
				Texts.Add(text, GetActiveColor(element), false);
			}
		}
		#endregion
		#region ILabelInfo Members
		public Node Owner { get { return node; } }
		public Size ClientSize {
			get { return TextSize; }
		}
		public Point ClientLocation {
			get { return TextLocation; }
		}
		public Font Font {
			get { return OwnerControl.Appearance.Font; }
		}
		public void Invalidate() {
			OwnerControl.Invalidate();
		}
		public int ScrollBarWidth {
			get { return 0; }
		}
		public void TextInfoChanged() {
			if(!IsTextChangesSuspend) {
				Refresh();
			}
		}
		#endregion                    
	}
	public class FilterLabelInfoViewInfo : LabelInfoViewInfo {
		public FilterLabelInfoViewInfo(ILabelInfo label)
			: base(label) {
		}
		protected override LabelInfoTextViewInfoBase CreateViewInfo(LabelInfoText infoText, int width) {
			return new FilterLabelInfoTextViewInfo(this, infoText, width, this.Label.ClientLocation);
		}
		public Node Owner {
			get {
				ILabelInfoEx e = this.Label as ILabelInfoEx;
				if(e == null) return null;
				return e.Owner;
			}
		}
		public FilterControl OwnerControl { get { return ((WinFilterTreeNodeModel)Owner.Model).Control; } }
		public ElementType ActiveItemType {
			get {
				if(ActiveItem == null) return ElementType.None;
				NodeEditableElement e = ActiveItem.InfoText.Tag as NodeEditableElement;
				if(e == null) return ElementType.None;
				return e.ElementType;
			}
		}
		bool IsRightToLeft { get { return OwnerControl.FilterViewInfo.RightToLeft; } }
		protected override int GetTextIndex(int index, int count) {
			if(IsRightToLeft) return count - index - 1;
			return base.GetTextIndex(index, count);
		}
	}
	public class FilterLabelInfoTextLineViewInfo {
		string text;
		int x, y, width, height;
		public FilterLabelInfoTextLineViewInfo(string text, int x, int y, int width, int height) {
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
	}
	public class LabelInfoHelper {
		public static bool ViewInfoEquals(FilterLabelInfoTextViewInfo vi1, FilterLabelInfoTextViewInfo vi2) {
			if(vi1 == null || vi2 == null) return ReferenceEquals(vi1, vi2);
			return ReferenceEquals(vi1.InfoText, vi2.InfoText);
		}
		public static bool EditorItem(FilterLabelInfoTextViewInfo info) {
			return EditorItem(info.OwnerControl, info.InfoText);
		}
		public static bool EditorItem(FilterControl control, LabelInfoText text) {
			if(!control.ShowEditors
				&& control.ActiveEditor != null
				&& control.FocusedItem != null
				&& (control.FocusedItemType == ElementType.Value || control.FocusedItemType == ElementType.AdditionalOperandParameter || control.FocusedItemType == ElementType.ItemCollection)
				&& control.FocusedItem.InfoText.Equals(text))
				return true;
			if(control.ShowEditors
				&& text.Tag != null
				&& (((NodeEditableElement)text.Tag).ElementType == ElementType.Value || ((NodeEditableElement)text.Tag).ElementType == ElementType.AdditionalOperandParameter || ((NodeEditableElement)text.Tag).ElementType == ElementType.ItemCollection))
				return true;
			if(control.IsHaveToShowPopupEditor()
				&& control.ActiveEditor != null && control.FocusedItem != null 
				&& control.FocusedItem.InfoText.Equals(text)) return true;
			return false;
		}
		public static Rectangle GetEditorBoundsByElement(FilterLabelInfoTextViewInfo element) {
			FilterControl control = element.FilterViewInfo.OwnerControl;
			Rectangle bounds = element.TextElement.Bounds;
			bounds.Y -= (control.FilterViewInfo.RowHeight - bounds.Height) / 2;
			bounds.Height = control.FilterViewInfo.RowHeight;
			bounds.Width = GetElementWidth(true, bounds.Width);
			return bounds;
		}
		static string specialSymbols = "*+-#";
		public static bool IsActionElement(string text) {
			if(text.Length != 2) return false;
			return (text[0] == '@' && specialSymbols.IndexOf(text[1]) > -1);
		}
		[Obsolete("Use ActionElementWidth"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static int ActionElementWidht {
			get {
				return ActionElementWidth;
			}
		}
		public static int ActionElementWidth {
			get {
				return FilterControl.NodeImages.ImageSize.Width + 4;
			}
		}
		public static int GetElementWidth(bool editor, int width) {
			if(editor) return  Math.Max(FilterControlViewInfo.EditorDefaultWidth, width);
			return width;
		}
		public static int GetElementWidth(bool editor, string text, GraphicsCache cache, Font font, StringFormat format) {
			int width = 0;
			if(LabelInfoHelper.IsActionElement(text))
				width = LabelInfoHelper.ActionElementWidth;
			else {
				if(!(cache.Paint is XPaintMixed))
				cache.Paint = new XPaintMixed(); 
				width = (int)cache.CalcTextSize(text, font, format, 0).Width;
			}
			return GetElementWidth(editor, width);
		}
		public static int GetFullWidth(Node node, ControlGraphicsInfoArgs info, FilterControlLabelInfo labelInfo, Font font, StringFormat format) {
			int ret = 0;
			for(int i = 0; i < labelInfo.Texts.Count; i++) {
				ret += GetElementWidth(LabelInfoHelper.EditorItem(((WinFilterTreeNodeModel)node.Model).Control, labelInfo.Texts[i]), labelInfo.Texts[i].Text, info.Cache, font, format);
			}
			return ret;
		}
	}
	public class FilterLabelInfoTextViewInfo : LabelInfoTextViewInfoBase {
		ArrayList list;
		Point location;
		public FilterLabelInfoTextViewInfo(FilterLabelInfoViewInfo viewInfo, LabelInfoText infoText, int width, Point location)
			: base(viewInfo, infoText, width) {
			this.location = location;
			this.list = new ArrayList();
		}
		public FilterControl OwnerControl { get { return FilterViewInfo.OwnerControl; } }
		public int Count { get { return list.Count; } }
		public FilterLabelInfoTextLineViewInfo TextElement { 
			get {
				if(this.Count > 0)
					return this[0];
				else return new FilterLabelInfoTextLineViewInfo("", 0, 0, 10, 10);
			} 
		}
		FilterLabelInfoTextLineViewInfo this[int index] { get { return list[index] as FilterLabelInfoTextLineViewInfo; } }
		public override int Bottom { get { return Count > 0 ? this[Count - 1].Bounds.Bottom : base.Bottom; } }
		public override int Top { get { return Count > 0 ? TextElement.Bounds.Top : base.Top; } }
		public override int LineCount { get { return 0; } }
		public Rectangle ItemBounds { get { return TextElement.Bounds; } }
		public override int GetScrollHeight(int lineCount) { return 0; }
		public virtual int GetLineHeight(Font font) { return (int)font.GetHeight(); }
		public override void Calculate(Graphics graphics, Font font, int fontHeight, ref int x, ref int y) {
			if(InfoText.Text == "") return;
			if(IsBreak) {
				y += (GetLineHeight(font) >> 1);
				return;
			}
			string drawText = InfoText.Text;
			while(drawText.Length > 0) {
				string words = GetNextWords(graphics, drawText, font, Width - x, x == LabelInfoViewInfo.TextIndent);
				if(words == string.Empty) return;
				int wordsWidth = 0;
				if(LabelInfoHelper.IsActionElement(this.InfoText.Text)) 
					wordsWidth = LabelInfoHelper.ActionElementWidth;
				else {
					wordsWidth = LabelInfoHelper.GetElementWidth(LabelInfoHelper.EditorItem(this), GetStringWidth(graphics, words, font));
				}
				FilterLabelInfoTextLineViewInfo item = new FilterLabelInfoTextLineViewInfo(words, location.X + x, location.Y + y, wordsWidth, fontHeight);
				list.Add(item);
				x += wordsWidth;
				if(x >= Width) {
					y += GetLineHeight(font);
					x = LabelInfoViewInfo.TextIndent;
				}
				if(words == drawText)
					break;
				drawText = drawText.Substring(words.Length, drawText.Length - words.Length);
			}
		}
		void DrawButton(GraphicsCache cache) {
			int imageIndex = -1;
			switch(this.InfoText.Text[1]) { 
				case '-':
					imageIndex = IsActive ? 1 : 0;
					break;
				case '*':
					imageIndex = IsActive ? 3 : 2;
					break;
				case '+':
					imageIndex = IsActive ? 5 : 4;
					break;
				case '#':
					NodeEditableElement element = this.InfoText.Tag as NodeEditableElement;
					ClauseNode node = (ClauseNode)element.Node;
					object obj = element.AdditionalOperand;
					imageIndex = (IsActive ? 7 : 6) + (obj is OperandValue ? 2 : 0) + (obj is OperandParameter ? 2 : 0);
					break;
			}
			if(imageIndex == -1) return;
			int y = this.TextElement.Y + (this.TextElement.Height - FilterControl.NodeImages.ImageSize.Height) / 2;
			int x = this.TextElement.X + (this.TextElement.Width - FilterControl.NodeImages.ImageSize.Width) / 2;
			x = Math.Max(x, this.TextElement.X);
			cache.Paint.DrawImage(cache.Graphics, FilterControl.NodeImages.Images[imageIndex], new Point(x, y)); 
		}
		void DrawTestEditor(GraphicsCache cache, string text) {
			if(FilterViewInfo.OwnerControl.FocusedItem == this 
				&& FilterViewInfo.OwnerControl.ActiveEditor != null) 
				return;
			ClauseNode node = FilterViewInfo.Owner as ClauseNode;
			if(node == null) return;
			RepositoryItem ri = FilterViewInfo.OwnerControl.GetRepositoryItem(node).Clone() as RepositoryItem;
			ri.BorderStyle = FilterViewInfo.OwnerControl.FocusedItem == this ? DevExpress.XtraEditors.Controls.BorderStyles.Default : DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			BaseEditPainter p = ri.CreatePainter();
			BaseEditViewInfo vi = ri.CreateViewInfo();
			vi.EditValue = this.TextElement.Text;
			vi.PaintAppearance.Assign(FilterViewInfo.OwnerControl.FilterViewInfo.PaintAppearance);
			vi.Bounds = LabelInfoHelper.GetEditorBoundsByElement(this);
			vi.CalcViewInfo(cache.Graphics);
			p.Draw(new ControlGraphicsInfoArgs(vi, cache, vi.Bounds));
		}
		public override void Draw(GraphicsCache cache, Font font, Color foreColor, StringFormat format) {
			if(IsBreak) return;
			if(LabelInfoHelper.IsActionElement(this.InfoText.Text)) {
				DrawButton(cache);
				return;
			}
			if(LabelInfoHelper.EditorItem(this)) {
				DrawTestEditor(cache, this.InfoText.Text);
				return;
			}
			Font activeFont = IsActive ? new Font(font, font.Style | FontStyle.Underline) : null;
			StringFormat sformat = format == null ? CreateStringFormat() : format;
			for(int i = 0; i < Count; i++) {
				Rectangle bounds = this[i].Bounds;
				bounds.Y = bounds.Y - ScrollTop;
				cache.DrawString(this[i].Text, IsActive ? activeFont : font, cache.GetSolidBrush(foreColor), bounds, sformat);
			}
			if(format == null) sformat.Dispose();
			if(activeFont != null)
				activeFont.Dispose();
		}
		StringFormat CreateStringFormat() {
			StringFormat format = new StringFormat(TextOptions.DefaultStringFormat);
			return format;
		}
		public override bool IsContains(Point pt) {
			pt.Y += ScrollTop;
			for(int i = 0; i < Count; i++) {
				if(this[i].Bounds.Contains(pt))
					return true;
			}
			return false;
		}
		string GetNextWords(Graphics graphics, string drawText, Font font, int wordsWidth, bool isStartLine) {
			string res = isStartLine ? GetNextWord(ref drawText) : "";
			string nextWord = "";
			while(drawText != "") {
				nextWord = GetNextWord(ref drawText);
				if(nextWord == "" || nextWord[0] < ' ') break;
				if(GetStringWidth(graphics, res + nextWord, font) > wordsWidth)
					break;
				res += nextWord;
			}
			return GetRTLWord(IsRightToLeft, res);
		}
		static string GetRTLWord(bool isRightToLeft, string word) {
			if(isRightToLeft) {
				if(word == ")") return "(";
				if(word == "(") return ")";
			}
			return word;
		}
		int GetStringWidth(Graphics graphics, string drawText, Font font) {
			if(drawText.Length == 0) return 0;
			using(GraphicsCache cache = new GraphicsCache(graphics)) {
				using(StringFormat format = CreateStringFormat()) {
					format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
					return LabelInfoHelper.GetElementWidth(LabelInfoHelper.EditorItem(this), drawText, cache, font, format);
				}
			}
		}
		string GetNextWord(ref string drawText) {
			if(drawText.Length == 0) return "";
			int i = 1;
			while(i < drawText.Length && !char.IsWhiteSpace(drawText[i]) && drawText[i] >= ' ')
				i++;
			string word = drawText.Substring(0, i);
			drawText = drawText.Substring(i, drawText.Length - i);
			return word;
		}
		bool IsRightToLeft {
			get {
				if(FilterViewInfo != null && FilterViewInfo.OwnerControl != null) return FilterViewInfo.OwnerControl.IsRightToLeft;
				return false;
			}
		}
		internal FilterLabelInfoViewInfo FilterViewInfo {
			get { return this.ViewInfo as FilterLabelInfoViewInfo;  }
		}
	}
}
