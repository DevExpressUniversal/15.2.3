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

using DevExpress.Data;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DevExpress.XtraEditors {
	public class Caption {
		AppearanceObject appearance, paintAppearance;
		AppearanceDefault defaultAppearance;
		ContentAlignment alignment = ContentAlignment.BottomCenter;
		string text = "";
		Padding contentPadding;
		Point offset;
		bool visible = true;
		static Point defaultOffset = new Point(10, 10);
		static Padding defaultContentPadding = new Padding(3);
		public Caption() {
			this.appearance = new AppearanceObject();
			this.appearance.Changed += OnAppearanceChanged;
			this.defaultAppearance = CreateDefault();
			this.contentPadding = defaultContentPadding;
			this.offset = defaultOffset;
		}
		public virtual void Assign(Caption source) {
			this.text = source.Text;
			this.visible = source.Visible;
			this.contentPadding = source.ContentPadding;
			this.offset = source.Offset;
			this.alignment = source.Alignment;
			this.appearance.Changed -= OnAppearanceChanged;
			this.appearance.Assign(source.Appearance);
			this.appearance.Changed += OnAppearanceChanged;
			OnChanged();
		}
		public override string ToString() {
			if(Text == "") return "";
			return Text;
		}
		protected virtual AppearanceDefault CreateDefault() {
			return new AppearanceDefault() { BackColor = Color.FromArgb(150, Color.LightGray), ForeColor = Color.White };
		}
		protected internal AppearanceDefault AppearanceDefault { get { return defaultAppearance; } }
		protected AppearanceObject PaintAppearance { 
			get {
				if(paintAppearance == null) this.paintAppearance = CalcAppearance();
				return paintAppearance; 
			} 
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			this.paintAppearance = null;
			OnChanged();
		}
		protected internal virtual void UpdateViewInfo(CaptionViewInfo info, string forcedText) {
			info.SetAppearanceCore(PaintAppearance);
			if(forcedText == null)
				info.Text = GetDisplayText();
			else
				info.Text = forcedText;
		}
		public CaptionViewInfo CreateViewInfo() {
			var res = new CaptionViewInfo(this);
			UpdateViewInfo(res, null);
			return res;
		}
		protected virtual AppearanceObject CalcAppearance() {
			var paintAppearance = new AppearanceObject();
			AppearanceHelper.Combine(paintAppearance, new AppearanceObject[] { Appearance }, AppearanceDefault);
			return paintAppearance;
		}
		public virtual string GetDisplayText() { return Visible ? Text : ""; }
		protected virtual void OnChanged() {
			CalcAppearance();
		}
		protected virtual void OnTextChanged() {
			OnChanged();
		}
		[DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnChanged();
			}
		}
		[DefaultValue("")]
		public string Text {
			get { return text; }
			set {
				if(value == null) value = "";
				if(Text == value) return;
				text = value;
				OnTextChanged();
			}
		}
		void ResetOffset() { Offset = defaultOffset; }
		bool ShouldSerializeOffset() { return Offset != defaultOffset; }
		public Point Offset {
			get { return offset; }
			set {
				offset = value;
				OnChanged();
			}
		}
		void ResetContentPadding() { ContentPadding = defaultContentPadding; }
		bool ShouldSerializeContentPadding() { return ContentPadding != defaultContentPadding; }
		public Padding ContentPadding {
			get { return contentPadding; }
			set {
				contentPadding = value;
				OnChanged();
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		public AppearanceObject Appearance { get { return appearance; } }
		[DefaultValue(ContentAlignment.BottomCenter)]
		public ContentAlignment Alignment {
			get { return alignment; }
			set {
				if(Alignment == value) return;
				alignment = value;
				OnChanged();
			}
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class DXBindingParser {
		static DXBindingParser _default;
		public static DXBindingParser Default {
			get {
				if(_default == null) _default = new DXBindingParser();
				return _default;
			}
		}
		static Regex bindingRegEx;
		static Regex BindingRegEx {
			get {
				if(bindingRegEx == null)
					bindingRegEx = new Regex(@"(?<!{){(?<Field>[a-zA-Z][\w\.]*?)}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
				return bindingRegEx;
			}
		}
		public void ExtractFields(string binding, Hashtable hash) {
			if(binding == null) return;
			BindingRegEx.Replace(binding, (m) => {
				string field = m.Groups["Field"].Value;
				if(!string.IsNullOrEmpty(field)) {
					hash[field] = true;
				}
				return "1";
			});
		}
		public virtual bool ContainsBindings(string source) {
			if(string.IsNullOrEmpty(source)) return false;
			return source.Contains('{');
		}
		public virtual string ParseBindings(DataController controller, int n, string binding) {
			string text = BindingRegEx.Replace(binding, (m) => {
				string field = m.Groups["Field"].Value;
				if(!string.IsNullOrEmpty(field)) {
					object v = controller.GetRowValue(n, field);
					if(v == null || v == DBNull.Value) return "";
					return v.ToString();
				}
				return "";
			});
			return text;
		}
	}
	public class CaptionPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			CaptionViewInfo vi = (CaptionViewInfo)e;
			if(!vi.IsReady || vi.Bounds.Size.IsEmpty || vi.StringInfo == null) return;
			vi.Appearance.FillRectangle(vi.Cache, vi.Bounds);
			StringPainter.Default.DrawString(vi.Cache, vi.StringInfo);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			CaptionViewInfo vi = (CaptionViewInfo)e;
			if(vi.IsReady) return vi.Bounds;
			Size size = CalcObjectMinBounds(vi).Size;
			if(size.IsEmpty) {
				vi.Bounds = Rectangle.Empty;
				vi.IsReady = true;
				return vi.Bounds;
			}
			Rectangle captionBounds = new Rectangle(vi.TargetBounds.Location, size);
			if(captionBounds.Width > vi.TargetBounds.Width) captionBounds.Width = vi.TargetBounds.Width;
			if(captionBounds.Height > vi.TargetBounds.Height) captionBounds.Height = vi.TargetBounds.Height;
			captionBounds = UpdateByContentAlignment(vi.TargetBounds, captionBounds, vi.Caption.Alignment);
			captionBounds.X += vi.Caption.Offset.X;
			captionBounds.Y += vi.Caption.Offset.Y;
			captionBounds.Width -= vi.Caption.Offset.X * 2;
			captionBounds.Height -= vi.Caption.Offset.Y * 2;
			vi.TextBounds = RectangleHelper.Deflate(captionBounds, vi.Caption.ContentPadding);
			var stringArgs = GetTextArgs(vi, vi.TextBounds);
			vi.StringInfo = StringPainter.Default.Calculate(stringArgs);
			vi.Bounds = captionBounds;
			vi.IsReady = true;
			return vi.Bounds;
		}
		Rectangle UpdateByContentAlignment(Rectangle sourceBounds, Rectangle bounds, ContentAlignment alignment) {
			if(bounds.Width > sourceBounds.Width || bounds.Height > sourceBounds.Height) return bounds;
			switch(alignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					bounds.X += (sourceBounds.Width - bounds.Width) / 2;
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					bounds.X = sourceBounds.Right - bounds.Width;
					break;
			}
			switch(alignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					bounds.Y += (sourceBounds.Height - bounds.Height) / 2;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					bounds.Y = sourceBounds.Bottom - bounds.Height;
					break;
			}
			return bounds;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CaptionViewInfo vi = (CaptionViewInfo)e;
			if(string.IsNullOrEmpty(vi.Text)) return Rectangle.Empty;
			Rectangle bounds = vi.TargetBounds;
			if(bounds.Width < 1) bounds.Size = new Size(1000, 0);
			bounds = RectangleHelper.Deflate(bounds, vi.Caption.ContentPadding);
			if(vi.Caption.Offset.X > 0) bounds.Width -= (vi.Caption.Offset.X * 2);
			if(bounds.Width < 1) return Rectangle.Empty;
			var size = StringPainter.Default.Calculate(GetTextArgs(vi, bounds)).Bounds.Size;
			size.Width += vi.Caption.Offset.X * 2 + vi.Caption.ContentPadding.Horizontal;
			size.Height += vi.Caption.Offset.Y * 2 + vi.Caption.ContentPadding.Vertical;
			return new Rectangle(Point.Empty, size);
		}
		StringCalculateArgs GetTextArgs(CaptionViewInfo vi, Rectangle bounds) {
			StringCalculateArgs res = new StringCalculateArgs(vi.Graphics, vi.Appearance, TextOptions.DefaultOptionsMultiLine, vi.Text, bounds, null, vi.Caption);
			return res;
		}
	}
	public class CaptionViewInfo : StyleObjectInfoArgs {
		Caption caption;
		string text;
		Rectangle targetBounds;
		public CaptionViewInfo(Caption caption) {
			this.caption = caption;
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			TargetBounds = Offset(TargetBounds, x, y);
			TextBounds = Offset(TextBounds, x, y);
			if(StringInfo != null) StringInfo.Offset(x, y);
		}
		Rectangle Offset(Rectangle source, int x, int y) {
			if(source.IsEmpty) return source;
			source.Offset(x, y);
			return source;
		}
		public StringInfo StringInfo { get; set; }
		public Rectangle TargetBounds {
			get { return targetBounds; }
			set { targetBounds = value; }
		}
		public Caption Caption { get { return caption; } }
		public Rectangle TextBounds { get; set; }
		public string Text {
			get { return text; }
			set {
				if(Text == value) return;
				text = value;
				IsReady = false;
			}
		}
		internal void SetAppearanceCore(AppearanceObject appearance) {
			if(appearance.IsEqual(Appearance)) return;
			SetAppearance(appearance);
			IsReady = false;
		}
		internal void SetTargetBounds(Rectangle value) {
			if(TargetBounds == value) return;
			targetBounds = value;
			IsReady = false;
		}
		public bool IsReady { get; set; }
	}
}
