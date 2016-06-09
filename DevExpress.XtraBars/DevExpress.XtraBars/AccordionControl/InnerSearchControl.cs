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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using System.Drawing.Imaging;
using DevExpress.XtraBars.Localization;
namespace DevExpress.XtraBars.Navigation {
	public partial class AccordionSearchControl : XtraUserControl, IFilterContent {
		public AccordionSearchControl(AccordionControl control) {
			AccordionControl = control;
			InitializeComponent();
			LookAndFeel.ParentLookAndFeel = AccordionControl.LookAndFeel;
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			AccordionControl.RightToLeftChanged += OnRightToLeftChanged;
			NullValuePrompt = defaultNullValuePrompt;
			this.lbSearchIcon.Appearance.Image = AccordionControlHelper.Load("AccordionControl.SearchIcon.png");
			this.lbSearchIcon.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.teSearch.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.teSearch.KeyDown += teSearch_KeyDown;
			GotFocus += AccordionSearchControl_GotFocus;
			UpdateLayoutCore();
		}
		string defaultNullValuePrompt { get { return BarLocalizer.Active.GetLocalizedString(BarString.AccordionControlSearchBoxPromptText); } }
		string nullValuePrompt;
		[Localizable(true)]
		public string NullValuePrompt {
			get { return nullValuePrompt; }
			set {
				if(nullValuePrompt == value) return;
				nullValuePrompt = value;
				if(this.teSearch == null) return;
				this.teSearch.Properties.NullValuePrompt = nullValuePrompt;
			}
		}
		bool ShouldSerializeNullValuePrompt() { return NullValuePrompt != defaultNullValuePrompt; }
		void ResetNullValuePrompt() { NullValuePrompt = defaultNullValuePrompt; }
		void teSearch_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape && GetValue() == string.Empty) {
				if(AccordionControl != null && AccordionControl.ShowFilterControl == ShowFilterControl.Auto) {
					AccordionControl.UpdateFilterControlState(false);
					e.Handled = true;
				}
			}
		}
		UserLookAndFeel IFilterContent.LookAndFeel { get { return LookAndFeel; } }
		void OnRightToLeftChanged(object sender, EventArgs e) {
			LayoutEditors();
		}
		protected AccordionControl AccordionControl { get; private set; }
		SkinElement GetSkinElement() {
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinSearchControl];
			if(elem == null)
				elem = AccordionControlSkins.GetSkin(DefaultSkinProvider.Default)[AccordionControlSkins.SkinSearchControl];
			return elem;
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			((IFilterContent)this).UpdateLayout();
		}
		private void UpdatePadding() {
			if(LookAndFeel.ActiveStyle != DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
				Padding = new Padding(DefaultGlyphToTextIndent);
			SkinElement elem = GetSkinElement();
			if(elem == null)
				Padding = new Padding(DefaultGlyphToTextIndent);
			else 
				Padding = new Padding(elem.ContentMargins.Left, elem.ContentMargins.Top, elem.ContentMargins.Right, elem.ContentMargins.Bottom);
		}
		void IFilterContent.UpdateLayout() {
			UpdateLayoutCore();
		}
		protected void UpdateLayoutCore() {
			UpdateAppearance();
			UpdatePadding();
			UpdateControlSize();
			LayoutEditors();
		}
		void IFilterContent.UpdateAppearance() {
			UpdateAppearance();
		}
		protected Color GetBackColor() {
			return ((AccordionControlViewInfo)AccordionControl.GetViewInfo()).GetBackColor();
		}
		private void UpdateAppearance() {
			if(LookAndFeel.ActiveStyle != DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				this.lbSearchIcon.Appearance.ForeColor =
					this.lbSearchIcon.AppearanceHovered.ForeColor =
					this.lbSearchIcon.AppearancePressed.ForeColor =
					this.lbSearchIcon.AppearanceDisabled.ForeColor = Color.Empty;
				this.teSearch.Properties.AppearanceFocused.ForeColor =
					this.teSearch.Properties.Appearance.ForeColor = Color.Empty;
			}
			SkinElement elem = GetSkinElement();
			if(elem != null) {
				this.lbSearchIcon.Appearance.ForeColor = elem.GetForeColor(ObjectState.Normal);
				this.lbSearchIcon.AppearanceHovered.ForeColor = elem.GetForeColor(ObjectState.Hot);
				this.lbSearchIcon.AppearancePressed.ForeColor = elem.GetForeColor(ObjectState.Pressed);
				this.lbSearchIcon.AppearanceDisabled.ForeColor = elem.GetForeColor(ObjectState.Disabled);
				this.lbSearchIcon.Appearance.Image = elem.Image.Image;
				this.Appearance.BackColor = 
				this.teSearch.Properties.Appearance.BackColor =
				this.teSearch.Properties.AppearanceFocused.BackColor = GetBackColor();
				this.teSearch.Properties.Appearance.ForeColor = 
				this.teSearch.Properties.AppearanceFocused.ForeColor = elem.GetForeColor(ObjectState.Normal);
			}
		}
		private void UpdateControlSize() {
			Size = CalcBestSize();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			LayoutEditors();
			AccordionControl.ForceRefresh();
		}
		Control IFilterContent.Control { get { return this; } }
		private void LayoutEditors() {
			this.lbSearchIcon.Size = this.lbSearchIcon.CalcBestSize();
			int height = Math.Max(this.lbSearchIcon.Height, this.teSearch.Height);
			this.lbSearchIcon.Location = new Point(Padding.Left, Padding.Top + (height - this.lbSearchIcon.Height) / 2);
			this.teSearch.Location = new Point(this.lbSearchIcon.Right + GlyphToTextIndent, Padding.Top + (height - this.teSearch.Height) / 2);
			this.teSearch.Width = Width - Padding.Right - this.teSearch.Left;
			if(AccordionControl != null && AccordionControl.IsRightToLeft) {
				this.teSearch.Bounds = CheckBounds(teSearch.Bounds);
				this.lbSearchIcon.Bounds = CheckBounds(lbSearchIcon.Bounds);
			}
		}
		Rectangle CheckBounds(Rectangle bounds) {
			return new Rectangle(Width - bounds.Right, bounds.Y, bounds.Width, bounds.Height);
		}
		const int DefaultGlyphToTextIndent = 16;
		protected int GlyphToTextIndent { 
			get {
				if(LookAndFeel.ActiveStyle != DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
					return DefaultGlyphToTextIndent;
				SkinElement elem = GetSkinElement();
				if(elem == null)
					return DefaultGlyphToTextIndent;
				int res = elem.Properties.GetInteger(AccordionControlSkins.OptGlyphToTextIndent);
				return res == 0? DefaultGlyphToTextIndent: res;
			} 
		}
		private Size CalcBestSize() {
			this.lbSearchIcon.Size = this.lbSearchIcon.CalcBestSize();
			return new Size(AccordionControl.Width, Math.Max(lbSearchIcon.Height, this.teSearch.Height + Padding.Vertical));
		}
		void teSearch_EditValueChanged(object sender, EventArgs e) {
			RaiseEditValueChanged();
		}
		private void peSearchIconClick(object sender, EventArgs e) {
			if(this.teSearch.MaskBox.Focused)
				AccordionControl.Focus();
			else
				this.teSearch.Focus();
		}
		protected void AccordionSearchControl_GotFocus(object sender, EventArgs e) {
			this.teSearch.Focus();
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if(AccordionControlHelper.IsLetterOrNumberKey(e)) {
				this.teSearch.Focus();
				this.teSearch.Text += e.KeyChar;
				this.teSearch.SelectionStart = this.teSearch.Text.Length;
				return;
			}
			base.OnKeyPress(e);
		}
		public object FilterValue {
			get { return teSearch.EditValue; }
		}
		void RaiseEditValueChanged() {
			EventHandler handler = Events[changed] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		object changed = new object();
		public event EventHandler FilterValueChanged {
			add { Events.AddHandler(changed, value); }
			remove { Events.RemoveHandler(changed, value); }
		}
		public string GetValue() {
			if(this.teSearch.EditValue == null) return string.Empty;
			return this.teSearch.EditValue.ToString();
		}
	}
	[ToolboxItem(false)]
	class AccordionLabelControl : LabelControl {
		protected override XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new AccordionLabelControlViewInfo(this);
		}
	}
	class AccordionLabelControlViewInfo : DevExpress.XtraEditors.ViewInfo.LabelControlViewInfo {
		public AccordionLabelControlViewInfo(LabelControl label)
			: base(label) {
		}
		protected override XtraEditors.Drawing.LabelControlObjectPainter GetLabelPainter() {
			return GetLabelPainterCore(OwnerControl.BorderStyle, OwnerControl.LookAndFeel);
		}
		protected XtraEditors.Drawing.LabelControlObjectPainter GetLabelPainterCore(BorderStyles style, UserLookAndFeel lookAndFill) {
			if(lookAndFill == null) {
				lookAndFill = UserLookAndFeel.Default;
			}
			switch(lookAndFill.ActiveStyle) {
				case ActiveLookAndFeelStyle.Office2003:
				case ActiveLookAndFeelStyle.WindowsXP:
					return new AccordionLabelControlWindowsXPObjectPainter();
				case ActiveLookAndFeelStyle.Skin:
					return new AccordionLabelControlSkinObjectPainter(lookAndFill);
			}
			return new AccordionLabelControlObjectPainter();
		}
	}
	class AccordionLabelControlObjectPainter : DevExpress.XtraEditors.Drawing.LabelControlObjectPainter {
		public override void DrawBackgroundImage(ObjectInfoArgs e) {
			ColorizedImagePaintHelper.DrawImage(e);
		}
	}
	class AccordionLabelControlWindowsXPObjectPainter : DevExpress.XtraEditors.Drawing.LabelControlWindowsXPObjectPainter {
		public override void DrawBackgroundImage(ObjectInfoArgs e) {
			ColorizedImagePaintHelper.DrawImage(e);
		}
	}
	class AccordionLabelControlSkinObjectPainter : DevExpress.XtraEditors.Drawing.LabelControlSkinObjectPainter {
		public AccordionLabelControlSkinObjectPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override void DrawBackgroundImage(ObjectInfoArgs e) {
			ColorizedImagePaintHelper.DrawImage(e);
		}
	}
	public class ColorizedImagePaintHelper {
		public static void DrawImage(ObjectInfoArgs e) {
			DevExpress.XtraEditors.ViewInfo.LabelInfoArgs le = e as DevExpress.XtraEditors.ViewInfo.LabelInfoArgs;
			Image img = le.Appearance.GetImage(ObjectState.Normal);
			if(img != null) {
				Rectangle srcImgBounds = new Rectangle(Point.Empty, le.Appearance.Image.Size);
				if(!le.GetAllowGlyphSkinning()) {
					e.Cache.Paint.DrawImage(e.Graphics, img, le.ImageBounds, srcImgBounds, le.DrawImageEnabled);
				}
				else {
					ImageAttributes attr = new ImageAttributes();
					attr.SetColorMatrix(CreateColorMatrix(le.Appearance.ForeColor));
					e.Cache.Paint.DrawImage(e.Graphics, img, le.ImageBounds, srcImgBounds, attr);
				}
			}
		}
		static float[][] matrix;
		public static ColorMatrix CreateColorMatrix(Color color) {
			if(matrix == null) {
				matrix = new float[][] { 
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
					new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
					};
			}
			matrix[0][0] = color.R / 255.0f;
			matrix[1][1] = color.G / 255.0f;
			matrix[2][2] = color.B / 255.0f;
			return new ColorMatrix(matrix);
		}
	}
}
