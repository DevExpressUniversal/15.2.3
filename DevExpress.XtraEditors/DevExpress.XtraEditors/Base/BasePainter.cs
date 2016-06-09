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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Skins;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraEditors.Drawing {
	public interface IEditorBackgroundProvider {
		bool DrawBackground(Control ownerEdit, GraphicsCache cache);
	}
	public class InplaceBorderPainter : BorderPainter {
		BaseEditViewInfo viewInfo;
		ObjectPainter originalBorder;
		const int LeftIndent = 1, RightIndent = 0;
		public InplaceBorderPainter(BaseEditViewInfo viewInfo, ObjectPainter originalBorder) {
			this.viewInfo = viewInfo;
			this.originalBorder = originalBorder;
		}
		public ObjectPainter OriginalBorder { get { return originalBorder; } set { originalBorder = value; } }
		public BaseEditViewInfo ViewInfo { get { return viewInfo; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = OriginalBorder.GetObjectClientRectangle(e);
			r.X += LeftIndent;
			r.Width = r.Width - (LeftIndent + RightIndent);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = OriginalBorder.CalcBoundsByClientRectangle(e, client);
			r.X -= LeftIndent;
			r.Width += LeftIndent + RightIndent;
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(ViewInfo.FillBackground) {
				Brush brush = ViewInfo.PaintAppearance.GetBackBrush(e.Cache, e.Bounds);
				Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y, LeftIndent, e.Bounds.Height);
				e.Cache.Paint.FillRectangle(e.Graphics, brush, r);
				r = new Rectangle(e.Bounds.Right - RightIndent, e.Bounds.Y, RightIndent, e.Bounds.Height);
				e.Cache.Paint.FillRectangle(e.Graphics, brush, r);
			}
			OriginalBorder.DrawObject(e);
		}
	}
	public class BaseEditPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			if(info.IsDrawOnGlass && NativeVista.IsVista && vi.OwnerEdit != null) {
				info.Graphics.Clear(Color.Transparent);
			}
			base.Draw(info);
		}
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			vi.BorderPainter.DrawObject(new BorderObjectInfoArgs(info.Cache, vi.PaintAppearance, vi.BorderRect, vi.CalcBorderState()));
		}
		protected virtual void DrawFocusRectCore(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			Brush brush = vi.PaintAppearance.GetBackBrush(info.Cache);
			Rectangle r = vi.FocusRect;
			if(vi.DrawFocusRect) {
				r.Inflate(-1, -1);
				info.Paint.DrawFocusRectangle(info.Graphics, r,
					info.ViewInfo.PaintAppearance.ForeColor, Color.Empty);
				r.Inflate(1, 1);
				DrawRectangle(info, brush, r, BaseEditViewInfo.FocusRectThin - 1);
			}
			else {
				if(vi.UseParentBackground || (vi.AllowDrawContent && vi.FillBackground)) return;
				DrawRectangle(info, brush, r, BaseEditViewInfo.FocusRectThin);
			}
		}
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			if(vi.AllowDrawFocusRect && !vi.FocusRect.IsEmpty) {
				DrawFocusRectCore(info);
			}
		}
		protected virtual void DrawRectangle(ControlGraphicsInfoArgs info, Brush brush, Rectangle bounds, int thin) {
			if(thin == 0) return;
			Rectangle r = bounds;
			r.Height = thin;
			info.Cache.Paint.FillRectangle(info.Graphics, brush, r);
			r.Y = bounds.Bottom - thin; 
			info.Cache.Paint.FillRectangle(info.Graphics, brush, r);
			r = bounds;
			r.Width = thin;
			info.Cache.Paint.FillRectangle(info.Graphics, brush, r);
			r.X = bounds.Right - thin;
			info.Cache.Paint.FillRectangle(info.Graphics, brush, r);
		}
		protected override void DrawAdornments(ControlGraphicsInfoArgs info) {
			base.DrawAdornments(info);
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			if(vi.FillBackground) {
				if(!vi.ErrorIconBounds.IsEmpty) {
					if(vi.InplaceType != InplaceType.Standalone || vi.OwnerEdit == null ||
					  !vi.OwnerEdit.PaintErrorIconBackground(info.Graphics, vi.ErrorIconBounds)) {
						info.Cache.Paint.FillRectangle(info.Graphics, vi.PaintAppearance.GetBackBrush(info.Cache), vi.ErrorIconBounds);
					}
				}
			}
			DrawErrorIcon(info);
		}
		public static bool DrawParentBackground(BaseEditViewInfo vi, GraphicsCache cache) {
			if(!vi.AllowDrawParentBackground || !vi.UseParentBackground || vi.OwnerEdit == null) return false;
			IEditorBackgroundProvider iprov = vi.OwnerEdit.Parent as IEditorBackgroundProvider;
			if (iprov == null) return false;
			return iprov.DrawBackground(vi.OwnerEdit, cache);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			bool drawParent = DrawParentBackground(vi, info.Cache);
			if(!drawParent && vi.FillBackground) {
				info.Cache.Paint.FillRectangle(info.Graphics, vi.PaintAppearance.GetBackBrush(info.Cache), vi.ClientRect);
			}
		}
		protected virtual void DrawErrorIcon(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = (BaseEditViewInfo)info.ViewInfo;
			if(vi.ErrorIconBounds.IsEmpty) return;
			Rectangle r = vi.ErrorIconBounds;
			r.Size = vi.GetErrorIconSize();
			r.X += (vi.ErrorIconBounds.Width - r.Width) / 2;
			r.Y += (vi.ErrorIconBounds.Height - r.Height) / 2;
			info.Cache.Graphics.DrawImage(vi.GetErrorIcon(), r);
		}
		protected internal static bool CheckRectangle(Rectangle maxBounds, Rectangle r) {
			return UpdateRectangle(maxBounds, r) == r;
		}
		protected internal static Rectangle UpdateRectangle(Rectangle maxBounds, Rectangle r) {
			if(r.X < maxBounds.X) {
				r.Width = r.Width - (maxBounds.X - r.X);
				r.X = maxBounds.X;
			}
			if(r.Y < maxBounds.Y) {
				r.Height = r.Height - (maxBounds.Y - r.Y);
				r.Y = maxBounds.Y;
			}
			if(r.Right > maxBounds.Right) {
				r.Width -= r.Right - maxBounds.Right;
			}
			if(r.Bottom > maxBounds.Bottom) {
				r.Height -= r.Bottom - maxBounds.Bottom;
			}
			if(r.Width < 1 || r.Height < 1) r = Rectangle.Empty;
			return r;
		}
	}
	public class BaseEditorGroupRowPainter : StyleObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			EditorGroupRowArgs ee = (EditorGroupRowArgs)e;
			if(ee.AllowHtmlDraw) {
				return new Rectangle(0, 0, 10, StringPainter.Default.Calculate(GetStringArgs(ee, ee.Appearance, ee.Text, Rectangle.Empty)).Bounds.Height);
			}
			return new Rectangle(0, 0, 10, ee.Appearance.CalcDefaultTextSize(ee.Graphics).Height + 2);
		}
		protected StringCalculateArgs GetStringArgs(EditorGroupRowArgs ee, AppearanceObject appearance, string text, Rectangle bounds) {
			return new StringCalculateArgs(ee.Graphics, appearance, ee.Appearance.GetTextOptions(),
					text, bounds, null, ee.HtmlContext);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			EditorGroupRowArgs ee = (EditorGroupRowArgs)e;
			string groupText = GetGroupRowText(ee);
			if(!string.IsNullOrEmpty(ee.MatchedText)) {
				if(CheckDrawMatchedText(ee, groupText)) return;
			}
			if(ee.AllowHtmlDraw) {
				AppearanceObject appearance = (AppearanceObject)ee.Appearance.Clone();
				appearance.TextOptions.RightToLeft = ee.RightToLeft;
				appearance.ForeColor = ee.ForeColor;
				StringInfo si = StringPainter.Default.Calculate(GetStringArgs(ee, appearance, groupText, ee.Bounds));
				StringPainter.Default.DrawString(e.Cache, si);
				return;
			}
			ee.Appearance.DrawString(e.Cache, groupText, ee.Bounds, ee.Cache.GetSolidBrush(ee.ForeColor), ee.Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrap));
		}
		protected virtual bool CheckDrawMatchedText(EditorGroupRowArgs ee, string groupText) {
			int containsIndex, length;
			string noFormat = groupText;
			if(ee.AllowHtmlDraw) {
				if(groupText != StringPainter.Default.RemoveFormat(groupText))
					return CheckDrawMatchedTextFormatted(ee, groupText);
			}
			string groupValue = ee.GroupValueText;
			int index = groupText.IndexOf(groupValue);
			if(index == -1) return false;
			if(!TextEditPainter.IsTextMatch(groupValue, ee.MatchedText, true, false, out containsIndex, out length)) return false;
			containsIndex += index;
			AppearanceObject appearance = (AppearanceObject)ee.Appearance.Clone();
			appearance.ForeColor = ee.ForeColor;
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(ee.Properties.LookAndFeel, !ee.UseHighlightSearchAppearance);
			ee.Cache.Paint.DrawMultiColorString(ee.Cache, ee.Bounds, groupText, ee.MatchedText, appearance, appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrap),
				highlight.ForeColor, highlight.BackColor, false, containsIndex);
			return true;
		}
		bool CheckDrawMatchedTextFormatted(EditorGroupRowArgs ee, string groupText) {
			int containsIndex, length;
			string groupValue = ee.GroupValueText;
			int index = groupText.IndexOf(groupValue);
			if(index == -1) return false;
			if(!TextEditPainter.IsTextMatch(groupValue, ee.MatchedText, true, false, out containsIndex, out length)) return false;
			containsIndex += index;
			AppearanceObject appearance = (AppearanceObject)ee.Appearance.Clone();
			appearance.ForeColor = ee.ForeColor;
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(ee.Properties.LookAndFeel, !ee.UseHighlightSearchAppearance);
			groupText = groupText.Insert(containsIndex + length, "</color></backcolor>");
			groupText = groupText.Insert(containsIndex, string.Format("<color=#{0:x}><backcolor=#{1:x}>", highlight.ForeColor.ToArgb(), highlight.BackColor.ToArgb()));
			StringInfo si = StringPainter.Default.Calculate(GetStringArgs(ee, appearance, groupText, ee.Bounds));
			StringPainter.Default.DrawString(ee.Cache, si);
			return true;
		}
		public const string ImageText = "[#image]";
		protected virtual string GetGroupRowText(EditorGroupRowArgs e) {
			return e.Text.Replace(ImageText, string.Empty).Replace('\n', ' ');
		}
	}
	public class EditorGroupRowArgs : StyleObjectInfoArgs {
		RepositoryItem properties;
		string text, groupValueText;
		string matchedText;
		object editValue;
		Color foreColor;
		bool useHighlightSearchAppearance = false;
		bool allowHtmlDraw;
		public EditorGroupRowArgs(Rectangle bounds, AppearanceObject appearance, RepositoryItem properties, object editValue, string text) : base(null, bounds, appearance) {
			this.properties = properties;
			this.groupValueText = this.text = text;
			this.matchedText = string.Empty;
			this.editValue = editValue;
			this.foreColor = Appearance.GetForeColor();
		}
		public Object HtmlContext { get; set; }
		public bool AllowHtmlDraw { get { return allowHtmlDraw; } set { allowHtmlDraw = value; } }
		public Color ForeColor { 
			get { return foreColor; } 
			set { foreColor = value; } }
		public RepositoryItem Properties { get { return properties; } }
		public string Text { 
			get { return text; } 
			set {
				if(Text == value) return;
				text = value;
			} 
		}
		public string GroupValueText { get { return groupValueText; } set { groupValueText = value; } }
		public string GroupValueTextWithoutHtml {
			get {
				if(!AllowHtmlDraw) return GroupValueText;
				return StringPainter.Default.RemoveFormat(GroupValueText);
			}
		}
		public string MatchedText { get { return matchedText; } set { matchedText = value; } }
		public bool UseHighlightSearchAppearance { get { return useHighlightSearchAppearance; } set { useHighlightSearchAppearance = value; } }
		public object EditValue { get { return editValue; } set { editValue = value; } }
	}
}
