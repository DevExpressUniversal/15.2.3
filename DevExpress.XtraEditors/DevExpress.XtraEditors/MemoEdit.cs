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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ScrollHelpers;
using DevExpress.Skins;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemMemoEdit : RepositoryItemTextEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemMemoEdit Properties { get { return this; } }
		bool _wordWrap, _acceptsTab, _acceptsReturn;
		int _linesCount;
		ScrollBars _scrollBars;
		public RepositoryItemMemoEdit() {
			this._linesCount = 0;
			this._wordWrap = true;
			this._acceptsTab = false;
			this._acceptsReturn = true;
			this._scrollBars = System.Windows.Forms.ScrollBars.Vertical;
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			style.Padding = new PaddingInfo(2, 1, 0, 0);
			return style;
		}
		protected override void SetupTextBrickStyleProperties(PrintCellHelperInfo info, BrickStyle style) {
			base.SetupTextBrickStyleProperties(info, style);
			BrickStringFormat source = style.StringFormat;
			BrickStringFormat bsf = new BrickStringFormat(source.Alignment, DevExpress.XtraPrinting.Native.PSConvert.ToStringAlignment(info.Appearance.TextOptions.VAlignment),
				StringFormatFlags.LineLimit | StringFormatFlags.NoClip, source.HotkeyPrefix, source.Trimming);
			bsf.PrototypeKind = source.PrototypeKind;
			style.StringFormat = bsf;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info){
			return CreateTextBrick(info);
		}
		protected internal override bool UseMaskBox { get { return false; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemMemoEdit source = item as RepositoryItemMemoEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this._acceptsReturn = source.AcceptsReturn;
				this._acceptsTab = source.AcceptsTab;
				this._linesCount = source.LinesCount;
				this._scrollBars = source.ScrollBars;
				this._wordWrap = source.WordWrap;
			}
			finally {
				EndUpdate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.XtraEditors.Mask.MaskProperties Mask {
			get { return base.Mask; }
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "MemoEdit"; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight { get { return false; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoEditWordWrap"),
#endif
 DefaultValue(true), SmartTagProperty("WordWrap", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool WordWrap {
			get {return _wordWrap; }
			set {
				if(WordWrap == value) return;
				_wordWrap = value; 
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoEditLinesCount"),
#endif
 DefaultValue(0), SmartTagProperty("Lines Count", "")]
		public int LinesCount {
			get { return _linesCount; }
			set {
				if(value < 1) value = 0;
				if(LinesCount == value) return;
				_linesCount = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoEditAcceptsTab"),
#endif
 DefaultValue(false)]
		public bool AcceptsTab {
			get {return _acceptsTab; }
			set {
				if(AcceptsTab == value) return;
				_acceptsTab = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoEditAcceptsReturn"),
#endif
 DefaultValue(true)]
		public bool AcceptsReturn {
			get {return _acceptsReturn; }
			set {
				if(AcceptsReturn == value) return;
				_acceptsReturn = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMemoEditScrollBars"),
#endif
 DefaultValue(System.Windows.Forms.ScrollBars.Vertical), SmartTagProperty("ScrollBars", "")]
		public virtual System.Windows.Forms.ScrollBars ScrollBars {
			get {return _scrollBars; }
			set {
				if(ScrollBars == value) return;
				_scrollBars = value;
				OnPropertiesChanged();
			}
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(AcceptsReturn && key == Keys.Enter)
				return true;
			if(AcceptsTab && key == Keys.Tab)
				return true;
			if(key == Keys.Up)
				return true;
			if(key == Keys.Down)
				return true;
			return base.NeededKeysContains(key);
		}
		protected override bool IsNeededKeyCore(Keys keyData) {
			if(keyData == (Keys.Enter | Keys.Control)) return false;
			bool res = base.IsNeededKeyCore(keyData);
			if(res) return true;
			if(keyData == Keys.PageUp || keyData == Keys.PageDown) return true;
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override char PasswordChar {
			get { return base.PasswordChar; }
			set { base.PasswordChar = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool UseSystemPasswordChar {
			get { return base.UseSystemPasswordChar; }
			set { base.UseSystemPasswordChar = value; }
		}
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraEditors.Design.MemoEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows multi-line text to be edited."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), Docking(DockingBehavior.Ask), SmartTagAction(typeof(MemoEditActions), "Lines", "Edit multi-line text", SmartTagActionType.CloseAfterExecute),
	 SmartTagFilter(typeof(MemoEditFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "MemoEdit")
	]
	public class MemoEdit : TextEdit, IAutoHeightControl {
		protected internal ScrollBarEditorsAPIHelper scrollHelper;
		public MemoEdit() {
			UseOptimizedRendering = true;
			this.scrollHelper = CreateScrollHelper();
			this.scrollHelper.Init(MaskBox, this);
			this.scrollHelper.ScrollMouseLeave += new EventHandler(OnScroll_MouseLeave);
			this.scrollHelper.LookAndFeel = LookAndFeel;
		}
		protected virtual ScrollBarEditorsAPIHelper CreateScrollHelper() {
			return new ScrollBarEditorsAPIHelper();
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				this.scrollHelper.ScrollMouseLeave -= new EventHandler(OnScroll_MouseLeave);
				this.scrollHelper.Dispose();
			}
			base.Dispose(disposing);
		}
		protected internal override bool AllowSmartMouseWheel { get { return true; } }
		protected override Size CalcSizeableMinSize() { return new Size(10, CalcMinHeight()); }
		protected override bool AcceptsReturn { get { return Properties.AcceptsReturn; } }
		protected override bool AcceptsTab { get { return Properties.AcceptsTab; } }
		protected override void OnStyleControllerChanged() {
			base.OnStyleControllerChanged();
			scrollHelper.LookAndFeel.Assign(LookAndFeel); 
		}
		[Browsable(false), DefaultValue(true)]
		public bool UseOptimizedRendering { get; set; }
		int IAutoHeightControl.CalcHeight(GraphicsCache cache) {
			if(ViewInfo.IsReady) {
				IHeightAdaptable ih = ViewInfo as IHeightAdaptable;
				if(ih != null) return ih.CalcHeight(cache, Width);
			}
			return Height;
		}
		bool IAutoHeightControl.SupportsAutoHeight { get { return true; } }
		EventHandler heightChanged;
		event EventHandler IAutoHeightControl.HeightChanged {
			add { heightChanged += value; }
			remove { heightChanged -= value; }
		}
		protected void RaiseHeightChanged() {
			if(heightChanged != null) heightChanged(this, EventArgs.Empty);
		}
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			base.LayoutChanged();
			this.scrollHelper.UpdateScrollBars();
			if(!IsLayoutLocked) RaiseHeightChanged();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(!IsLayoutLocked) RaiseHeightChanged();
		}
		protected internal override bool OnMaskBoxPreWndProc(ref Message msg) {
			if(msg.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ERASEBKGND) {
				XtraForm f = FindForm() as XtraForm;
				if(f != null && f.IsSuspendRedraw) {
					msg.Result = new IntPtr(1);
					return true;
				}
			}
			return base.OnMaskBoxPreWndProc(ref msg);
		}
		protected internal override void OnMaskBoxWndProc(ref Message msg) {
			base.OnMaskBoxWndProc(ref msg);
			this.scrollHelper.WndProc(ref msg);
		}
		protected override void UpdateMaskBoxProperties(bool always) {
			base.UpdateMaskBoxProperties(always);
			if(MaskBox == null) return;
			if(always || !MaskBox.Multiline ) MaskBox.Multiline = true;
			if(always || Properties.WordWrap != MaskBox.WordWrap) MaskBox.WordWrap = Properties.WordWrap;
			if(always || Properties.AcceptsTab != MaskBox.AcceptsTab) MaskBox.AcceptsTab = Properties.AcceptsTab;
			if(always || Properties.AcceptsReturn != MaskBox.AcceptsReturn) MaskBox.AcceptsReturn = Properties.AcceptsReturn;
			if(always || Properties.ScrollBars != MaskBox.ScrollBars) MaskBox.ScrollBars = Properties.ScrollBars;
		}
		void OnScroll_MouseLeave(object sender, EventArgs e) {
			CheckMouseHere();
		}
		protected override bool IsInputKey(Keys keyData) {
			bool result = base.IsInputKey(keyData);
			if(result) return true;
			if(Properties.AcceptsReturn && keyData == Keys.Enter) return true;
			if(Properties.AcceptsTab && keyData == Keys.Tab) return true;
			return result;
		}
		protected override Size DefaultSize { get { return new Size(100, 96); } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "MemoEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MemoEditLines"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string[] Lines { 
			get { return LinesConverter.TextToLines(Text); 	}
			set { Text = LinesConverter.LinesToText(value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MemoEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemMemoEdit Properties { get { return base.Properties as RepositoryItemMemoEdit; } }
		public int CalcAutoHeight() {
			if(MaskBox == null) return -1;
			int ret = TextRenderer.MeasureText("Wg", MaskBox.Font).Height * (this.MaskBox.GetLineFromCharIndex(this.MaskBox.TextLength) + 1);
			using(GraphicsCache cache = new GraphicsCache(this.CreateGraphics())) {
				ObjectInfoArgs args = new ObjectInfoArgs();
				args.Bounds = new Rectangle(0, 0, 10, ret);
				ret = ViewInfo.BorderPainter.CalcBoundsByClientRectangle(args).Height;
			}
			return ret;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class MemoEditViewInfo : TextEditViewInfo, IHeightAdaptable {
		Rectangle memoBounds;
		static TextOptions defaultMemoTextOptions;
		static TextOptions DefaultMemoTextOptions {
			get {
				if(defaultMemoTextOptions == null) {
					defaultMemoTextOptions = new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.Wrap, Trimming.None);
				}
				return defaultMemoTextOptions;
			}
		}
		public MemoEditViewInfo(RepositoryItem item) : base(item) {
		}
		protected override bool AllowMaskBoxWhenDisabled { get { return true; } }
		public Rectangle MemoBounds { get { return memoBounds; } }
		public override bool DefaultAllowDrawFocusRect { get { return false; } }
		public new RepositoryItemMemoEdit Item { get { return base.Item as RepositoryItemMemoEdit; } }
		public override bool IsSupportIncrementalSearch { get { return false; } }
		public override void Reset() {
			this.memoBounds = Rectangle.Empty;
			base.Reset();
		}
		bool IsScrollVisible(bool horz) {
			MemoEdit edit = OwnerControl as MemoEdit;
			if(edit == null) return false;
			if(edit.scrollHelper.IsNativeVisible(horz)) return true;
			if(horz) return Item.ScrollBars == ScrollBars.Horizontal || Item.ScrollBars == ScrollBars.Both;
			return Item.ScrollBars == ScrollBars.Vertical || Item.ScrollBars == ScrollBars.Both;
		}
		protected override Padding GetTextBoxContentPadding() {
			Padding padding = base.GetTextBoxContentPadding();
			if(!IsSkinLookAndFeel) return padding;
			MemoEdit edit = OwnerControl as MemoEdit;
			if(LookAndFeel.GetTouchUI()) {
				var element = EditorsSkins.GetSkin(LookAndFeel)[EditorsSkins.SkinTextBox];
				if(element == null) return Padding.Empty;
				if(IsScrollVisible(false))
					padding.Right = element.ContentMarginsCore.ToPadding().Right;
				if(IsScrollVisible(true))
					padding.Bottom = element.ContentMarginsCore.ToPadding().Bottom;
			}
			if(IsScrollVisible(false)) {
				if(RightToLeft)
					padding.Left = 0;
				else
					padding.Right = 0;
			}
			return padding;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			this.memoBounds = this.fMaskBoxRect = CalcMaskBoxRect(ContentRect);
			if(BorderPainter is EmptyBorderPainter)
				this.memoBounds = Rectangle.Empty;
			else {
				if(!RightToLeft) {
					this.fMaskBoxRect.X += FocusRectThin;
				}
				this.fMaskBoxRect.Width -= FocusRectThin;
				this.memoBounds = fMaskBoxRect;
			}
		}
		protected override void UpdateMaxBoxRectangle(ref Rectangle rect) { }
		int IHeightAdaptable.CalcHeight(GraphicsCache cache, int width) {
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(cache);
			info.Bounds = new Rectangle(0, 0, width, 100);
			Rectangle textRect = BorderPainter.GetObjectClientRectangle(info);
			if(!(BorderPainter is EmptyBorderPainter) && !(BorderPainter is InplaceBorderPainter))
				textRect.Inflate(-1, -1);
			Rectangle empty = Rectangle.Empty;
			textRect = CalcMaskBoxRect(textRect, ref empty);
			textRect.Width -= FocusRectThin;
			string text = string.Empty;
			if(Item.LinesCount == 0) {
				text = DisplayText;
				if(text != null && text.Length > 0) {
					char lastChar = text[text.Length - 1];
					if(lastChar == 13 || lastChar == 10) text += "W";
				}
			} else {
				for(int i = 0; i < Item.LinesCount; i++)
					text += (string.IsNullOrEmpty(text) ? "" : Environment.NewLine) + "W";
			}
			int height = CalcTextSizeCore(cache, text, textRect.Width).Height + 1;
			return (height + 100 - textRect.Bottom) + 1;
		}
		protected override int GetTextToolTipWidth() { return MaskBoxRect.Width; } 
		public override TextOptions DefaultTextOptions { 
			get { 
				if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone) {
					if(Item.ScrollBars == ScrollBars.Horizontal || Item.ScrollBars == ScrollBars.Both) {
						return new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.NoWrap, Trimming.None);
					} else {
						return new TextOptions(HorzAlignment.Near, VertAlignment.Top, Item.WordWrap ? WordWrap.Wrap : WordWrap.NoWrap, Trimming.None);
					}
				}
				return DefaultMemoTextOptions; } 
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class MemoEditPainter : TextEditPainter {
		protected override void DrawTextBoxArea(ControlGraphicsInfoArgs info) {
			MemoEditViewInfo vi = info.ViewInfo as MemoEditViewInfo;
			if(!vi.FillBackground || vi.MemoBounds.IsEmpty) {
				base.DrawTextBoxArea(info);
				return;
			}
			DrawEmptyArea(info, vi.MemoBounds);
			DrawRectangle(info, vi.PaintAppearance.GetBackBrush(info.Cache), vi.MemoBounds, 1);
		}
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if(vi.AllowDrawText) {
				if(BaseEditViewInfo.ShowFieldBindings && vi.GetDataBindingText() != "") return;
				Rectangle r = vi.MaskBoxRect;
				if(!CheckDrawMatchedText(info)) {
					DrawString(info, r, vi.DisplayText, vi.PaintAppearance);
				}
			}
		}
	} 
}
