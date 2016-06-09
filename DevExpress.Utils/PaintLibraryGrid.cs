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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using DevExpress.Utils.Paint;
using DevExpress.Accessibility;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Text;
namespace DevExpress.Utils.Drawing {
	public class FooterCellInfoArgs : StyleObjectInfoArgs {
		bool visible;
		string displayText;
		object _value;
		public FooterCellInfoArgs() : this(null) { }
		public FooterCellInfoArgs(GraphicsCache cache) : base(cache) {
			this.visible = true;
			this.displayText = "";
			AllowHtmlDraw = true;
			AllowDrawBackground = true;
		}
		public bool AllowDrawBackground { get; set; }
		public bool AllowHtmlDraw { get; set; }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return this._value; } set { this._value = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
	}
	public class FooterCellPainter : StyleObjectPainter {
		ObjectPainter contentPainter;
		public FooterCellPainter(ObjectPainter contentPainter) {
			this.contentPainter = contentPainter;
		}
		public ObjectPainter ContentPainter { 
			get { return contentPainter; }
			set { contentPainter = value; }
		}
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) { return e; }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return ContentPainter.GetObjectClientRectangle(UpdateInfo(e)); }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return ContentPainter.CalcBoundsByClientRectangle(UpdateInfo(e), client); }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle b = new Rectangle(Point.Empty, CalcTextSize(e));
			b.Inflate(1, 1);
			e.Bounds = b;
			Rectangle r = CalcBoundsByClientRectangle(e);
			return r;
		}
		protected virtual void DrawBackground(ObjectInfoArgs e) {
			FooterCellInfoArgs ee = e as FooterCellInfoArgs;
			if(ContentPainter != null) {
				ContentPainter.DrawObject(UpdateInfo(e));
			}
			Rectangle r = GetObjectClientRectangle(e);
			GetStyle(e).FillRectangle(e.Cache, r);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			FooterCellInfoArgs ee = e as FooterCellInfoArgs;
			if(ee.AllowDrawBackground) DrawBackground(e);
			Rectangle r = GetObjectClientRectangle(e);
			r.Inflate(-1, 0);
			AppearanceObject appearance = GetStyle(e);
			if(ee.AllowHtmlDraw) {
				StringPainter.Default.DrawString(e.Cache, appearance, ee.DisplayText, r, GetDefaultTextOptions(ee));
			} else
				appearance.DrawString(e.Cache, ee.DisplayText, r, appearance.GetStringFormat(GetDefaultTextOptions(ee)));
		}
		protected TextOptions GetDefaultTextOptions(FooterCellInfoArgs e) {
			return TextOptions.DefaultOptionsNoWrapEx;
		}
		public virtual bool IsCaptionFit(GraphicsCache cache, ObjectInfoArgs e) {
			Rectangle caption = GetCaptionBounds(cache, e);
			if(caption.Width < 0) return false;
			e.Cache = cache;
			Size size = CalcTextSize(e);
			if(size.Width > caption.Width || size.Height > caption.Height) return false;
			return true;
		}
		protected virtual Size CalcTextSize(ObjectInfoArgs e) {
			FooterCellInfoArgs ee = e as FooterCellInfoArgs;
			string text = string.IsNullOrEmpty(ee.DisplayText) ? "Wg" : ee.DisplayText;
			Rectangle bounds = e.Bounds;
			if(ee.AllowHtmlDraw) {
				Graphics g = GraphicsInfo.Default.AddGraphics(e.Graphics);
				try {
					StringCalculateArgs sca = new StringCalculateArgs(g, GetStyle(e), GetDefaultTextOptions(ee), text, bounds, null);
					return StringPainter.Default.Calculate(sca).Bounds.Size;
				}
				finally {
					GraphicsInfo.Default.ReleaseGraphics();
				}
			}
			else {
				return GetStyle(e).CalcTextSize(e.Cache, ((FooterCellInfoArgs)e).DisplayText, 0).ToSize();
			}
		}
		protected virtual Rectangle GetCaptionBounds(GraphicsCache cache, ObjectInfoArgs e) {
			Rectangle res = GetObjectClientRectangle(cache.Graphics, this, e);
			res.Inflate(-1, 0);
			return res;
		}
	}
	public class FooterPanelInfoArgs : StyleObjectInfoArgs {
		int rowCount, cellHeight;
		public FooterPanelInfoArgs(GraphicsCache cache, int rowCount, int cellHeight) : this(cache, null, Rectangle.Empty, rowCount, cellHeight) { }
		public FooterPanelInfoArgs(GraphicsCache cache, AppearanceObject style, Rectangle bounds, int rowCount, int cellHeight) : base(cache, bounds, style, ObjectState.Normal) {
			this.cellHeight = cellHeight;
			this.rowCount = rowCount;
		}
		public int CellHeight { get { return cellHeight; } set { cellHeight = value; } }
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
	}
	public class FooterPanelPainter : StyleObjectPainter {
		ObjectPainter panelButtonPainter;
		public FooterPanelPainter(ObjectPainter panelButtonPainter) {
			this.panelButtonPainter = panelButtonPainter;
		}
		public ObjectPainter PanelButtonPainter {
			get { return panelButtonPainter; }
			set { panelButtonPainter = value; }
		}
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) { return e; } 
		protected virtual int RowIndent { get { return 1; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = PanelButtonPainter.GetObjectClientRectangle(UpdateInfo(e));
			r.Inflate(-1, -1);
			return r;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = PanelButtonPainter.CalcBoundsByClientRectangle(UpdateInfo(e), client);
			r.Inflate(1, 1);
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			FooterPanelInfoArgs ee = e as FooterPanelInfoArgs ;
			Rectangle r = new Rectangle(0, 0, 100, ee.CellHeight);
			if(ee.RowCount > 1) r.Height = (ee.CellHeight + RowIndent) * ee.RowCount;
			return CalcBoundsByClientRectangle(e, r);
		}
		public virtual Rectangle CalcCellBounds(Rectangle footerClient, Rectangle column, int cellHeight, int rowIndex, int rowCount) {
			Rectangle r = new Rectangle(column.X, footerClient.Y, column.Width, cellHeight * rowCount);
			if(r.X < footerClient.X) {
				r.Width = r.Right - footerClient.X;
				r.X = footerClient.X;
			}
			if(r.Right > footerClient.Right) {
				r.Width = footerClient.Right - r.X;
			}
			r.Inflate(-1, 0);
			if(rowIndex > 0) r.Y += ((cellHeight + RowIndent) * rowIndex);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			PanelButtonPainter.DrawObject(UpdateInfo(e));
		}
	}
	public class GridWindowsXPButtonPainter : XPHeaderPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			r.Inflate(-1, -1);
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectMinBounds(e);
			r.Inflate(1, 1);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = base.CalcBoundsByClientRectangle(e, client);
			r.Inflate(1, 1);
			return r;
		}
	}
	public class GridUltraFlatButtonPainter : SimpleButtonObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			r.X ++; r.Y ++; r.Width --; r.Height --;
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectMinBounds(e);
			r.Height ++;
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = base.CalcBoundsByClientRectangle(e, client);
			r.X --;
			r.Y --;
			r.Width ++;
			r.Height ++;
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r, newBounds;
			r = newBounds = e.Bounds;
			newBounds.Offset(1, 1); newBounds.Width --;newBounds.Height --;
			e.Bounds = newBounds;
			try {
				base.DrawObject(e);
			}
			finally {
				e.Bounds = r;
			}
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			r = e.Bounds;	r.Height = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
			r = e.Bounds; r.Width = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
		}
	}
	public class Office2003FooterPanelObjectPainter : ButtonObjectPainter {
		Color backColor = Color.Empty;
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 8, 8);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = client;
			r.Inflate(1, 1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			AppearanceDefault appearance = Office2003Colors.Default[Office2003GridAppearance.FooterPanel];
			Color back = BackColor == Color.Empty ? appearance.BackColor : BackColor;
			e.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(appearance.BorderColor), r);
			r.Inflate(-1, -1);
			e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(back), r);
		}
	}
	public class Office2003FooterCellPainter : FooterCellPainter {
		public Office2003FooterCellPainter() : base(null) {
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle r = e.Bounds;
			r.Inflate(-2, -2);
			return r; 
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			Rectangle r = client;
			r.Inflate(2, 2);
			return r; 
		}
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceDefault appearance = Office2003Colors.Default[Office2003GridAppearance.FooterCell];
			FooterCellInfoArgs ee = e as FooterCellInfoArgs;
			Rectangle r = e.Bounds;
			Color border = appearance.BorderColor;
			Color back = appearance.BackColor;
			e.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(border), r);
			r.Inflate(-1, -1);
			e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(back), r);
			r.Inflate(-1, -1);
			AppearanceObject style = GetStyle(e);
			style.DrawString(e.Cache, ee.DisplayText, r);
		}
		protected override Rectangle GetCaptionBounds(GraphicsCache cache, ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -2, -2);
		}
	}
	public class Office2003HeaderObjectPainter : HeaderObjectPainter {
		public class Office2003HeaderButtonObjectPainter : StyleObjectPainter {
			const int FadeHeight = 4;
			bool headerPainter;
			public Office2003HeaderButtonObjectPainter() : this(false) { }
			public Office2003HeaderButtonObjectPainter(bool headerPainter) {
				this.headerPainter = headerPainter;
			}
			protected bool HeaderPainter { get { return headerPainter; } }
			public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
				Rectangle r = e.Bounds;
				r.Inflate(-1, -1);
				r.Height -= (FadeHeight - 2);
				if((e.State & ObjectState.Pressed) != 0) {
					r.Offset(1, 1);
				}
				return r;
			}
			public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
				return e.Bounds;
			}
			public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
				Rectangle r = client;
				r.Inflate(1, 1);
				r.Height += FadeHeight;
				return r;
			}
			public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { 
				return new Rectangle(0, 0, 10, 8 + FadeHeight); 
			}
			public override void DrawObject(ObjectInfoArgs e) {
				if((e.State & ObjectState.Pressed) != 0) 
					DrawObjectPressed(e);
				else
					DrawObjectNormal(e);
			}
			protected virtual void DrawObjectPressed(ObjectInfoArgs e) {
				Rectangle r = e.Bounds;
				AppearanceDefault appearance = Office2003Colors.Default[Office2003GridAppearance.HeaderPressedLine];
				Color c = appearance.BackColor;
				Color border = appearance.BorderColor; 
				e.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(border), r);
				r.Inflate(-1, -1);
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(c), r);
			}
			protected virtual void DrawObjectNormal(ObjectInfoArgs e) {
				Rectangle r = e.Bounds;
				r.Height -= 4;
				AppearanceDefault appearance = Office2003Colors.Default[Office2003GridAppearance.Header];
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(appearance.BackColor), r);
				r.Y = r.Bottom;
				r.Height = 4;
				if(e.State == ObjectState.Hot) {
					AppearanceDefault hot = Office2003Colors.Default[Office2003GridAppearance.HeaderHotLine];
					e.Paint.FillGradientRectangle(e.Graphics, e.Cache.GetGradientBrush(r, appearance.BackColor, appearance.BackColor2, LinearGradientMode.Vertical), r);
					r.Inflate(-1, 0);
					e.Paint.FillGradientRectangle(e.Graphics, e.Cache.GetGradientBrush(r, hot.BackColor, hot.BackColor2, LinearGradientMode.Vertical), r);
				} else
					e.Paint.FillGradientRectangle(e.Graphics, e.Cache.GetGradientBrush(r, appearance.BackColor2, ControlPaint.Dark(appearance.BackColor2, 0.05f), LinearGradientMode.Vertical), r);
				r = new Rectangle(e.Bounds.X, e.Bounds.Y + 3, 1, e.Bounds.Height - FadeHeight - 3);
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(Color.White), r);
				r.X = e.Bounds.Right - 1;
				r.Y --;
				e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(appearance.BorderColor), r);
			}
		}
		public Office2003HeaderObjectPainter() : base(new Office2003HeaderButtonObjectPainter(true)) { }
	}
	public class UltraFlatHeaderObjectPainter : HeaderObjectPainter {
		public UltraFlatHeaderObjectPainter() : this(new SimpleButtonObjectPainter()) { }
		public UltraFlatHeaderObjectPainter(ButtonObjectPainter painter) : base(painter) {	}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			r.X ++; r.Y++; r.Width --; r.Height --;
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = base.CalcBoundsByClientRectangle(e, client);
			r.X --;
			r.Y --;
			r.Width ++;
			r.Height ++;
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			Rectangle r = e.Bounds;	r.Height = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
			r = e.Bounds; r.Width = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
		}
		protected override void DrawButtonObject(ObjectInfoArgs e) {
			Rectangle r, newBounds;
			r = newBounds = e.Bounds;
			newBounds.Offset(1, 1); newBounds.Width --;newBounds.Height --;
			e.Bounds = newBounds;
			try {
				ButtonPainter.DrawObject(e);
			}
			finally {
				e.Bounds = r;
			}
		}
	}
	public class HeaderWebButtonObjectPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle bounds = e.Bounds;
			AppearanceObject app = GetStyle(e);
			app.DrawBackground(e.Cache, bounds);
			bounds.Y = bounds.Bottom - 1;
			bounds.Height = 1;
			bounds.Inflate(-3, 0);
			Color border = app.GetBorderColor();
			if(border == Color.Empty || border == SystemColors.Control) border = SystemColors.ControlDark;
			e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(border), bounds);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle res = e.Bounds;
			res.Inflate(-6, -3);
			return res;	
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			Rectangle res = client;
			res.Inflate(6, 3);
			return res; 
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { return e.Bounds; }
	}
	public class SkinOpenCloseButtonObjectPainter : OpenCloseButtonObjectPainter {
		ISkinProvider provider;
		public SkinOpenCloseButtonObjectPainter(ISkinProvider provider) : base(SkinElementPainter.Default) { 
			this.provider = provider;
		}
		protected virtual SkinElement Element {
			get { return GridSkins.GetSkin(this.provider)[GridSkins.SkinPlusMinus]; }
		}
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			OpenCloseButtonInfoArgs ee = (OpenCloseButtonInfoArgs)e;
			SkinElementInfo info = new SkinElementInfo(Element, e.Bounds);
			info.RightToLeft = ee.RightToLeft;
			info.State = e.State;
			info.ImageIndex = (ee.Opened ? 1 : 0);
			info.BackAppearance = ee.BackAppearance;
			info.Cache = ee.Cache;
			return info;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, UpdateInfo(e));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e));
		}
	}
	public class SkinSortedShapeObjectPainter : SortedShapeObjectPainter {
		ISkinProvider provider;
		public SkinSortedShapeObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[GridSkins.SkinSortShape], e.Bounds);
			info.Cache = e.Cache;
			SortedShapeObjectInfoArgs sort = (SortedShapeObjectInfoArgs)e;
			info.ImageIndex = sort.Ascending ? 1 : 0;
			if(sort.UseAlternateShapes) info.ImageIndex += 2;
			return info;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, UpdateInfo(e));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e));
		}
	}
	public class SkinFooterPanelPainter : FooterPanelPainter {
		class SkinPainter : SkinCustomPainter {
			public SkinPainter(ISkinProvider provider) : base(provider) { }
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) { 
				return new SkinElementInfo(GridSkins.GetSkin(Provider)[GridSkins.SkinFooterPanel]);
			}
		}
		public SkinFooterPanelPainter(ISkinProvider provider) : base(new SkinPainter(provider)) { }
	}
	public class SkinFooterCellPainter : FooterCellPainter {
		class SkinPainter : SkinCustomPainter {
			public SkinPainter(ISkinProvider provider) : base(provider) { }
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) { 
				return new SkinElementInfo(GridSkins.GetSkin(Provider)[GridSkins.SkinFooterCell]);
			}
		}
		public SkinFooterCellPainter(ISkinProvider provider) : base(new SkinPainter(provider)) { }
		protected override void DrawBackground(ObjectInfoArgs e) {
			ContentPainter.DrawObject(UpdateInfo(e));
		}
	}
	public class SkinHeaderObjectPainter : HeaderObjectPainter {
		SkinElement element;
		ISkinProvider provider;
		public SkinHeaderObjectPainter(ISkinProvider provider, SkinElement element) : base(null) { 
			this.provider = provider;
			this.element = element;
		}
		public ISkinProvider Provider { get { return provider; } }
		public SkinHeaderObjectPainter(ISkinProvider provider) : this(provider, null) { }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e));
		}
		public SkinElement Element {
			get {
				if(element == null) element = Skin[GridSkins.SkinHeader];
				return element;
			} 
			set { element = value; }
		}
		protected Skin Skin {
			get {
				return GridSkins.GetSkin(Provider);
			}
		}
		protected SkinElementInfo UpdateInfo(ObjectInfoArgs e) { return UpdateInfo(e, e.Bounds); }
		public static SkinElement ElementByPosition(ISkinProvider provider, HeaderPositionKind position, bool isTopMost) {
			SkinElement element = GridSkins.GetSkin(provider)[GridSkins.SkinHeader];
			if(!isTopMost && position != HeaderPositionKind.Special) return element;
			switch(position) {
				case HeaderPositionKind.Special : element = GridSkins.GetSkin(provider)[GridSkins.SkinHeaderSpecial]; break;
				case HeaderPositionKind.Left : element = GridSkins.GetSkin(provider)[GridSkins.SkinHeaderLeft]; break;
				case HeaderPositionKind.Right : 
					if(!GridSkins.GetSkin(provider).Properties.GetBoolean(GridSkins.OptDrawRightColumnWithCenterHeader))
						element = GridSkins.GetSkin(provider)[GridSkins.SkinHeaderRight]; break;
			}
			return element;
		}
		public static int ElementImageIndexByState(ObjectState state) {
			if((state & ObjectState.Pressed) != 0) return 2;
			if((state & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e, Rectangle bounds) {
			HeaderObjectInfoArgs header = (HeaderObjectInfoArgs)e;
			SkinElementInfo info = new SkinElementInfo(Element, bounds);
			if(header.AllowColoring) {
				info.Attributes = new ImageAttributes();
				Color color = header.Appearance.BackColor;
				info.Attributes.SetColorMatrix(new ColorMatrix(new float[][] { 
					new float[]{ color.R / 255.0f, 0.0f, 0.0f, 0.0f, 0.0f},
					new float[]{ 0.0f, color.G / 255.0f, 0.0f, 0.0f, 0.0f},
					new float[] { 0.0f, 0.0f, color.B / 255.0f, 0.0f, 0.0f},
					new float[]{ 0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
					new float[]{ 0.0f, 0.0f, 0.0f, 0.0f, 1.0f},
				}));
			}
			info.State = e.State;
			info.Element = ElementByPosition(Provider, header.HeaderPosition, header.IsTopMost);
			info.ImageIndex = ElementImageIndexByState(info.State);
			info.RightToLeft = header.RightToLeft;
			return info;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, UpdateInfo(e), client);
		}
		protected override void DrawButtonObject(ObjectInfoArgs e) {
			HeaderObjectInfoArgs info = (HeaderObjectInfoArgs)e;
			Rectangle bounds = e.Bounds;
			if(info.HeaderPosition == HeaderPositionKind.Right || info.HeaderPosition == HeaderPositionKind.Center) {
				if(Skin.Properties.GetBoolean(GridSkins.OptHeaderRequireHorzOffset)) {
					if(info.RightToLeft) bounds.Width++;
					else {
						bounds.X--; bounds.Width++;
					}
				}
			}
			if(!info.IsTopMost) {
				if(Skin.Properties.GetBoolean(GridSkins.OptHeaderRequireVertOffset)) {
					bounds.Y --; bounds.Height ++;
				}
			}
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, UpdateInfo(e, bounds));
		}
	}
	public enum IndicatorKind { Header, Row, RowFooter, Detail, Band };
	public class IndicatorObjectInfoArgs : StyleObjectInfoArgs {
		IndicatorKind kind;
		int imageIndex;
		object imageCollection;
		HeaderPositionKind headerPosition;
		string displayText;
		bool isTopMost;
		public IndicatorObjectInfoArgs(object imageCollection) : this(Rectangle.Empty, null, imageCollection, -1, IndicatorKind.Row) { }
		public IndicatorObjectInfoArgs(Rectangle bounds, AppearanceObject appearance, object imageCollection, int imageIndex, IndicatorKind kind) {
			this.kind = kind;
			this.Bounds = bounds;
			this.SetAppearance(appearance);
			this.imageIndex = imageIndex;
			this.imageCollection = imageCollection;
			this.displayText= "";
			if(Kind == IndicatorKind.Header || Kind == IndicatorKind.Band)
				isTopMost = true;
			UpdateHeaderPosition();
		}
		public bool IsTopMost { get { return isTopMost; } set { isTopMost = value; } }
		public IndicatorKind Kind { 
			get { return kind; } 
			set { 
				kind = value; 
				UpdateHeaderPosition();
			} 
		}
		public HeaderPositionKind HeaderPosition { get { return headerPosition; } set { headerPosition = value; } }
		public bool IsRowIndicator { get { return Kind == IndicatorKind.Row; } }
		public int ImageIndex { 
			get { return imageIndex; }
			set { imageIndex = value; }
		}
		public object ImageCollection { get { return imageCollection; } }
		public ImageList Images { get { return ImageCollection as ImageList; } }
		public string DisplayText {
			get { return displayText; }
			set { displayText = value; }
		}
		void UpdateHeaderPosition() {
			switch(Kind) {
				case IndicatorKind.Band :
				case IndicatorKind.Header :
					HeaderPosition = HeaderPositionKind.Left;
					break;
				default:
					HeaderPosition = HeaderPositionKind.Center;
					break;
			}
		}
	}
	public class SkinIndicatorObjectPainter : IndicatorObjectPainter {
		SkinElement element;
		ISkinProvider provider;
		public SkinIndicatorObjectPainter(ISkinProvider provider, SkinElement element) : base(SkinElementPainter.Default) { 
			this.provider = provider;
			this.element = element;
		}
		public SkinIndicatorObjectPainter(ISkinProvider provider) : this(provider, null) { }
		public SkinElement Element {
			get {
				if(element == null) {
					element = Skin[GridSkins.SkinIndicator];
					if(element == null)
						element = Skin[GridSkins.SkinHeaderLeft];
				}
				return element;
			} 
			set { element = value; }
		}
		protected Skin Skin {
			get {
				return GridSkins.GetSkin(this.provider);
			}
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return ButtonPainter.GetObjectClientRectangle(UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return ButtonPainter.CalcBoundsByClientRectangle(UpdateInfo(e), client);
		}
		protected override void DrawButtonObject(ObjectInfoArgs e) {
			IndicatorObjectInfoArgs info = (IndicatorObjectInfoArgs)e;
			Rectangle bounds = e.Bounds;
			if(!info.IsTopMost) {
				if(Skin.Properties.GetBoolean(GridSkins.OptHeaderRequireVertOffset)) {
					bounds.Y --; bounds.Height ++;
				}
			}
			ButtonPainter.DrawObject(UpdateInfo(e, bounds));
		}
		protected override ObjectInfoArgs UpdateInfo(ObjectInfoArgs e, Rectangle bounds) { 
			IndicatorObjectInfoArgs ee = e as IndicatorObjectInfoArgs;
			SkinElementInfo info = new SkinElementInfo(Element, bounds);
			info.RightToLeft = ee.RightToLeft;
			info.State = e.State;
			SkinElement element = null;
			if(ee.Kind != IndicatorKind.Band && ee.Kind != IndicatorKind.Header) {
				element = Skin[GridSkins.SkinIndicator];
			}
			if(element == null) element = SkinHeaderObjectPainter.ElementByPosition(this.provider, ee.HeaderPosition, ee.IsTopMost);
			info.Element = element;
			info.ImageIndex = SkinHeaderObjectPainter.ElementImageIndexByState(info.State);
			info.Cache = e.Cache;
			return info; 
		}
		public override ImageCollection ImageList {
			get {
				SkinElement element = GridSkins.GetSkin(provider)[GridSkins.SkinIndicatorImages];
				if(element != null && element.Image != null) return element.Image.GetImages();
				return base.ImageList;
			}
		}
	}
	public class IndicatorObjectPainter : StyleObjectPainter {
		#region image collection
		[ThreadStatic]
		static ImageCollection defaultImageList = null;
		public static ImageCollection DefaultImageList {
			get {
				if(defaultImageList == null) {
					ImageCollection collection = new ImageCollection();
					defaultImageList = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.Indicator.bmp", typeof(DevExpress.Utils.Controls.ImageHelper).Assembly, new Size(7, 9), Color.Fuchsia);
				}
				return defaultImageList;
			}
		}
		#endregion
		protected const int IndicatorMinHeight = 12;
		public const int IndicatorImageWidth = 8;
		ObjectPainter buttonPainter;
		public IndicatorObjectPainter(ObjectPainter buttonPainter) {
			this.buttonPainter = buttonPainter;
		}
		public virtual ObjectPainter ButtonPainter { get { return buttonPainter; } }
		protected ObjectInfoArgs UpdateInfo(ObjectInfoArgs e) { return UpdateInfo(e, e.Bounds); }
		protected virtual ObjectInfoArgs UpdateInfo(ObjectInfoArgs e, Rectangle bounds) { return e; }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = ButtonPainter.GetObjectClientRectangle(UpdateInfo(e));
			r.Inflate(-1, 0);
			return r;
		}
		protected virtual void DrawButtonObject(ObjectInfoArgs e) {
			ButtonPainter.DrawObject(UpdateInfo(e));
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			IndicatorObjectInfoArgs info = e as IndicatorObjectInfoArgs;
			Size imageSize = ImageCollection.GetImageListSize(info.ImageCollection);
			if(imageSize.IsEmpty) imageSize = new Size(8, 8);
			Rectangle client = new Rectangle(Point.Empty, imageSize);
			Rectangle rect = CalcBoundsByClientRectangle(e, client);
			rect.Height = Math.Max(IndicatorMinHeight, rect.Height);
			return rect;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle res = ButtonPainter.CalcBoundsByClientRectangle(UpdateInfo(e), client);
			res.Inflate(1, 0);
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			IndicatorObjectInfoArgs ee = (IndicatorObjectInfoArgs)e;
			DrawButtonObject(e);
			Rectangle r = GetObjectClientRectangle(e);
			Size size = ImageCollection.GetImageListSize(ee.ImageCollection);
			if(ImageCollection.IsImageListImageExists(ee.ImageCollection, ee.ImageIndex) && r.Width >= size.Width) {
				Rectangle bounds = new Rectangle(r.X, r.Y + (r.Height - size.Height) / 2, size.Width, size.Height);
				Matrix current = null;
				if(ee.RightToLeft) {
					current = e.Cache.Graphics.Transform.Clone() as Matrix;
					e.Cache.Graphics.TranslateTransform(bounds.X + bounds.Right, 0);
					e.Cache.Graphics.ScaleTransform(-1, 1);
				}
				ImageCollection.DrawImageListImage(e.Cache, ee.ImageCollection, ee.ImageIndex,
					bounds);
				if(current != null) e.Cache.Graphics.Transform = current;
				r.X += size.Width + 1;
				r.Width -= (size.Width + 1);
			} 
			if(ee.DisplayText != null && ee.DisplayText.Length > 0 && r.Width > 0) {
				r.Inflate(-2, 0);
				ee.Appearance.DrawString(e.Cache, ee.DisplayText, r);
			}
		}
		public virtual ImageCollection ImageList {
			get {
				return DefaultImageList;
			}
		}
	}
	public class FlatIndicatorObjectPainter : IndicatorObjectPainter {
		public FlatIndicatorObjectPainter() : base(new FlatButtonObjectPainter()) { }
	}
	public class XPIndicatorObjectPainter : IndicatorObjectPainter {
		public XPIndicatorObjectPainter() : base(new XPHeaderPainter()) { }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			if(!WXPaint.WXPPainter.Default.IsVista && WXPaint.WXPPainter.Default.GetXPThemeType() != DevExpress.Utils.WXPaint.XPThemeType.Unknown)
				r.X -= 2;
			if(WXPaint.WXPPainter.Default.IsVista) r.Inflate(-2, 0);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle res = base.CalcBoundsByClientRectangle(e, client);
			if(WXPaint.WXPPainter.Default.IsVista) res.Inflate(2, 0);
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			IndicatorObjectInfoArgs ee = (IndicatorObjectInfoArgs)e;
			bool vista = ee.IsRowIndicator && WXPaint.WXPPainter.Default.IsVista;
			Rectangle r = e.Bounds;
			if(vista) e.Bounds = new Rectangle(r.X, r.Y, r.Width, r.Height + 1);
			base.DrawObject(e);
			e.Bounds = r;
		}
	}
	public class Style3DIndicatorObjectPainter : IndicatorObjectPainter {
		public Style3DIndicatorObjectPainter() : base(new Style3DButtonObjectPainter()) { }
	}
	public class Office2003IndicatorObjectPainter : IndicatorObjectPainter {
		public Office2003IndicatorObjectPainter() : base(new Office2003HeaderObjectPainter.Office2003HeaderButtonObjectPainter()) { }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle res = base.CalcBoundsByClientRectangle(e, client);
			res.Inflate(2, 0);
			return res;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle res = base.GetObjectClientRectangle(e);
			res.Inflate(-2, 0);
			return res;
		}
	}
	public class Office2003CellIndicatorObjectPainter : Office2003IndicatorObjectPainter {
		protected override void DrawButtonObject(ObjectInfoArgs e) {
			IndicatorObjectInfoArgs ee = (IndicatorObjectInfoArgs)e;
			AppearanceDefault appearance = Office2003Colors.Default[Office2003GridAppearance.Header];
			Rectangle r = e.Bounds;
			if(!ee.IsTopMost) {
				r.Y --; r.Height ++;
			}
			e.Paint.DrawRectangle(e.Graphics, e.Cache.GetPen(appearance.BorderColor), r);
			r.Inflate(-1, -1);
			e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(appearance.BackColor), r);
		}
	}
	public class UltraFlatIndicatorObjectPainter : IndicatorObjectPainter {
		public UltraFlatIndicatorObjectPainter() : base(new SimpleButtonObjectPainter()) { }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = base.GetObjectClientRectangle(e);
			r.X += 2; r.Y ++; r.Width --; r.Height --;
			return r;
		}
		protected override void DrawButtonObject(ObjectInfoArgs e) {
			Rectangle r, newBounds;
			r = newBounds = e.Bounds;
			newBounds.Offset(1, 1); newBounds.Width --;newBounds.Height --;
			e.Bounds = newBounds;
			try {
				ButtonPainter.DrawObject(e);
			}
			finally {
				e.Bounds = r;
			}
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle rect = base.CalcObjectMinBounds(e);
			rect.Width += 2;
			return rect;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			Rectangle r = e.Bounds;	r.Height = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
			r = e.Bounds; r.Width = 1;
			ee.Appearance.FillRectangle(e.Cache, r);
		}
	}
	public interface ISupportObjectInfo {
		ObjectInfoArgs ParentObject { get; set; }
	}
	public interface IParentAppearanceDependent {
		AppearanceObject ParentAppearance { get; set; }
	}
	public class HeaderObjectPainter : StyleObjectPainter {
		ObjectPainter buttonPainter;
		public HeaderObjectPainter(ObjectPainter buttonPainter) {
			this.buttonPainter = buttonPainter;
			UseInnerElementsForBestHeight = true;
		}
		public ObjectPainter ButtonPainter { get { return buttonPainter; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			ee.CaptionRect = Rectangle.Empty;
			Rectangle clRect = GetObjectClientRectangle(e);
			ee.TopLeft = clRect.Location;
			bool nullCache = e.Cache == null;
			try {
				if(nullCache) {
					GraphicsInfo.Default.AddGraphics(null);
					e.Cache = GraphicsInfo.Default.Cache;
				}
				ee.InnerElements.UpdateRightToLeft(ee.RightToLeft);
				clRect = ee.InnerElements.CalcBounds(ee, e.Cache, clRect, e.Bounds);
			}
			finally {
				if(nullCache) GraphicsInfo.Default.ReleaseGraphics();
			}
			ee.InnerElements.SetBackStyle(ee.Appearance);
			ee.InnerElements.SetAppearance(ee.Appearance);
			if(clRect.Width > 0) {
				ee.CaptionRect = clRect;
			}
			return e.Bounds;
		}
		public virtual bool IsCaptionFit(GraphicsCache cache, ObjectInfoArgs e) {
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			if(ee.Caption.Length == 0) return true;
			if(ee.CaptionRect.Width == 0) return false;
			else {
				int width = CalcCaptionTextSize(cache, ee, ee.Caption).Width;
				return width <= ee.CaptionRect.Width;
			}
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = ButtonPainter.GetObjectClientRectangle(e);
			r.Inflate(-2, 0);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle r = ButtonPainter.CalcBoundsByClientRectangle(e, client);
			r.Inflate(2, 0);
			return r;
		}
		public virtual bool UseInnerElementsForBestHeight { get; set; }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			string caption = ee.Caption;
			if(caption.Length == 0) caption = "Wg";
			Size captionSize = CalcCaptionTextSize(e.Cache, ee, caption);
			bool canDrawMore = true;
			Size res = ee.InnerElements.CalcMinSize(e.Graphics, ref canDrawMore);
			if(!canDrawMore) captionSize.Width = 0;
			if(UseInnerElementsForBestHeight)
				res.Height = Math.Max(res.Height, captionSize.Height);
			else
				res.Height = captionSize.Height;
			res.Width += captionSize.Width;
			return CalcBoundsByClientRectangle(e, new Rectangle(Point.Empty, res));
		}
		protected virtual Size CalcCaptionTextSize(GraphicsCache cache, HeaderObjectInfoArgs ee, string caption) {
			CheckAppearance(ee);
			if(ee.AutoHeight && ee.CaptionRect.IsEmpty && !ee.Bounds.IsEmpty) CalcObjectBounds(ee);
			if(ee.UseHtmlTextDraw) {
				StringInfo info = StringPainter.Default.Calculate(cache.Graphics, ee.Appearance, TextOptions.DefaultOptionsNoWrap,
					caption, ee.AutoHeight ? ee.CaptionRect.Width : 0, null, ee.HtmlContext);
				var captionSize = info.Bounds.Size;
				return captionSize;
			}
			else {
				Size captionSize = ee.Appearance.CalcTextSize(cache, ee.Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrap), caption, ee.AutoHeight ? ee.CaptionRect.Width  : 0).ToSize();
				if(!ee.AutoHeight) {
					captionSize.Height++; captionSize.Width++;
				}
				return captionSize;
			}
		}
		protected void CheckAppearance(HeaderObjectInfoArgs ee) {
			if(ee.AutoHeight) ee.Appearance.TextOptions.WordWrap = ee.Appearance.TextOptions.WordWrap == WordWrap.Default ? WordWrap.Wrap : ee.Appearance.TextOptions.WordWrap;
		}
		protected virtual void DrawCaptionText(HeaderObjectInfoArgs ee, Rectangle bounds, Brush foreBrush) {
			if(ee.UseHtmlTextDraw)
				StringPainter.Default.DrawString(ee.Cache, ee.Appearance, ee.Caption, bounds, TextOptions.DefaultOptionsNoWrapEx, ee.HtmlContext);
			else {
				CheckAppearance(ee);
				ee.Appearance.DrawString(ee.Cache, ee.Caption, bounds, foreBrush, ee.Appearance.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx));
			}
		}
		protected virtual void DrawButtonObject(ObjectInfoArgs e) {
			ButtonPainter.DrawObject(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawButtonObject(e);
			HeaderObjectInfoArgs ee = e as HeaderObjectInfoArgs;
			Rectangle r = GetObjectClientRectangle(e);
			Brush foreBrush = e.Cache.GetSolidBrush(ee.Appearance.ForeColor);
			if(ee.DesignTimeSelected) {
				ee.Cache.Paint.FillRectangle(e.Graphics, ee.Cache.GetSolidBrush(ControlPaint.Light(SystemColors.Highlight, 0)), r);
				foreBrush = SystemBrushes.HighlightText;
			}
			Point offs = new Point(r.X - ee.TopLeft.X, r.Y - ee.TopLeft.Y);
			ee.InnerElements.DrawObjects(ee, ee.Cache, offs);
			if(ee.Caption.Length == 0 || ee.CaptionRect.IsEmpty) return;
			r = ee.CaptionRect;
			r.Offset(offs);
			DrawCaptionText(ee, r, foreBrush);
		}
		const int TagSize = 9;
		protected virtual void DrawSmartTag(HeaderObjectInfoArgs e) {
			Rectangle tag, client;
			client = e.Bounds;
			client.Offset(-1, 0);
			tag = client;
			if(tag.Width < TagSize + 1 || tag.Height < TagSize + 1) return;
			tag.Width = TagSize;
			tag.X = client.Right - tag.Width;
			tag.Y = client.Bottom - TagSize;
			tag.Height = TagSize;
			e.Graphics.FillPolygon(e.Cache.GetSolidBrush(Color.FromArgb((e.State & ObjectState.Hot) == 0 ? 20 : 100, Color.Blue)), 
				new Point[] { 
					new Point(tag.X, tag.Bottom - 1),
					new Point(tag.Right - 1, tag.Y),
					new Point(tag.Right - 1, tag.Bottom - 1)
				}, FillMode.Winding);
			e.Graphics.DrawPolygon(e.Cache.GetPen(Color.FromArgb((e.State & ObjectState.Hot) == 0 ? 50 : 200, Color.Blue)), 
				new Point[] { 
					new Point(tag.X, tag.Bottom - 1),
					new Point(tag.Right - 1, tag.Y),
					new Point(tag.Right - 1, tag.Bottom - 1)
				});
		}
	}
	public enum HeaderPositionKind { Left, Center, Right, Special }
	public class HeaderObjectInfoArgs : StyleObjectInfoArgs, IAccessibleGridHeaderCell {
		DrawElementInfoCollection innerElements;
		HeaderPositionKind headerPosition;
		string caption;
		Rectangle captionRect;
		Point topLeft;
		bool useHtmlTextDraw;
		bool autoHeight = false;
		bool designTimeSelected, isTopMost;
		public HeaderObjectInfoArgs() {
			this.autoHeight = false;
			this.useHtmlTextDraw = false;
			this.isTopMost = true;
			this.headerPosition = HeaderPositionKind.Center;
			this.innerElements = new DrawElementInfoCollection();
			this.caption = "";
		}
		Rectangle IAccessibleGridHeaderCell.Bounds { get { return Bounds; } }
		string IAccessibleGridHeaderCell.GetDefaultAction() { return GetDefaultAccessibleAction(); }
		void IAccessibleGridHeaderCell.DoDefaultAction() { DoDefaultAccessibleAction(); }
		string IAccessibleGridHeaderCell.GetName() { return GetAccessibleName(); }
		AccessibleStates IAccessibleGridHeaderCell.GetState() { return AccessibleStates.Default; }
		protected virtual string GetAccessibleName() { return Caption; } 
		protected virtual void DoDefaultAccessibleAction() { }
		protected virtual string GetDefaultAccessibleAction() { return null; }
		public bool IsTopMost { get { return isTopMost; } set { isTopMost = value; } }
		public bool AllowColoring { get; set; }
		public object HtmlContext { get; set; }
		public virtual bool DesignTimeSelected { 
			get { return designTimeSelected; } 
			set { designTimeSelected = value; }
		}
		public bool AutoHeight {
			get { return autoHeight; }
			set { autoHeight = value; }
		}
		public bool UseHtmlTextDraw {
			get { return useHtmlTextDraw; }
			set { useHtmlTextDraw = value; }
		}
		public HeaderPositionKind HeaderPosition { get { return headerPosition; } set { headerPosition = value; } }
		public DrawElementInfoCollection InnerElements { get { return innerElements; } }
		public virtual string Caption { 
			get { return caption; } 
			set { 
				if(value == null) value = "";
				caption = value; 
			} 
		}
		public Rectangle CaptionRect { get { return captionRect; } set { captionRect = value; } }
		public Point TopLeft { get { return topLeft; } set { topLeft = value; } }
	}
	public class SkinGridBorderPainter : SkinBorderPainter {
		public SkinGridBorderPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(GridSkins.GetSkin(Provider)[GridSkins.SkinBorder]);
		}
	}
	#region Sort Shape
	public class SortedShapeObjectInfoArgs : StyleObjectInfoArgs {
		bool ascending, useAlternateShapes;
		public SortedShapeObjectInfoArgs() {
			this.ascending = true;
			this.useAlternateShapes = false;
		}
		public bool Ascending {
			get { return ascending; }
			set { ascending = value; }
		}
		public bool UseAlternateShapes {
			get { return useAlternateShapes; }
			set { useAlternateShapes = value; }
		}
	}
	public class SortedShapeHelper {
		public static ObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinSortedShapeObjectPainter(lookAndFeel);
			return lookAndFeel.ActiveLookAndFeel.Painter.SortedShape;
		}
	}
	public class SortedShapeObjectPainter : StyleObjectPainter {
		const int width = 9 , height =7;
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return e.Bounds;	}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) { return e.Bounds;	}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { return client; 	}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, width, height);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SortedShapeObjectInfoArgs ee = e as SortedShapeObjectInfoArgs;
			Graphics g = e.Graphics;
			AppearanceObject style = GetStyle(e);
			Rectangle r = e.Bounds;
			Point[] p;
			if(ee.Ascending) {	
				p = new Point[3] { new Point( r.Left + r.Width / 2, r.Top),
									 new Point( r.Left, r.Bottom), new Point(r.Right - 1, r.Bottom) };
			}
			else {
				p = new Point[3] { new Point( r.Left, r.Top), new Point(r.Right - 1, r.Top),
									 new Point(r.Left + r.Width / 2, r.Bottom)};
			}
			g.DrawPolygon(Pens.Black, p);
		}
	}
	public class FlatSortedShapeObjectPainter : SortedShapeObjectPainter {
		const int width = 8 , height =7;
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, width, height);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SortedShapeObjectInfoArgs ee = e as SortedShapeObjectInfoArgs;
			PPens pens = new PPens(e.Cache, ee.Appearance.BackColor);
			Graphics g = e.Graphics;
			Rectangle rect = e.Bounds;
			Rectangle r = new Rectangle( (rect.Left + rect.Right) / 2  - (width / 2),
				(rect.Top + rect.Bottom) / 2 - (height /2), width, height);
			if(ee.Ascending) {	
				Point[] p = {new Point( r.Left + (width / 2) - 1, r.Top),
								new Point( r.Left, r.Bottom - 1) };
				g.DrawLines(pens.Dark, p);
				p = new Point[2] { new Point( r.Left + ( width /2 ) - 2, r.Top),
									 new Point( r.Left, r.Bottom - 1) };
				g.DrawLines(pens.Dark, p);
				p   = new Point[3] { new Point( r.Left + 1, r.Bottom - 1), 
									   new Point( r.Left + width - 1, r.Bottom - 1),
									   new Point( r.Left + width / 2, r.Top - 1) };
				g.DrawLines(pens.LightLight, p);
				p = new Point [2] { new Point( r.Left + width / 2, r.Top + 1),
									  new Point( r.Left + width - 1, r.Bottom - 1) };
				g.DrawLines(pens.LightLight, p);
			}
			else {
				Point [] p = { new Point( r.Left + width - 1, r.Top),
								 new Point( r.Left, r.Top),
								 new Point( r.Left + width / 2 - 1, r.Top) };
				g.DrawLines(pens.Dark, p);
				p = new Point[2] { new Point( r.Left + 1, r.Top + 1),
									 new Point( r.Left + width / 2 - 1, r.Bottom - 1) };
				g.DrawLines(pens.Dark, p);
				p = new Point[2] { new Point( r.Left + width - 1, r.Top + 1),
									 new Point( r.Left + width / 2, r.Bottom) };
				g.DrawLines(pens.LightLight, p);
				p = new Point[2] { new Point( r.Left + width - 2, r.Top + 1), 
									 new Point( r.Left + width / 2, r.Bottom - 1) };
				g.DrawLines(pens.LightLight, p);	
			}
		}
	}
	#endregion
	public class GridGroupPanelPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject style = GetStyle(e);
			style.FillRectangle(e.Cache, e.Bounds);
		}
		public virtual void DrawEmptyText(ObjectInfoArgs e, string emptyText) {
			AppearanceObject style = GetStyle(e);
			Rectangle r = e.Bounds;
			r.Inflate(-5, -10);
			style.DrawString(e.Cache, emptyText, r, GetForeBrush(e));
		}
		public virtual Brush GetForeBrush(ObjectInfoArgs e) {
			return e.Cache.GetSolidBrush(GetStyle(e).ForeColor);
		}
	}
	public class Office2003GridGroupPanelPainter : GridGroupPanelPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			Color backColor = Office2003Colors.Default[Office2003GridAppearance.GroupPanel].BackColor;
			e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(backColor), e.Bounds); 
		}
		public override Brush GetForeBrush(ObjectInfoArgs e) {
			Color foreColor = Office2003Colors.Default[Office2003GridAppearance.GroupPanel].ForeColor;
			return e.Cache.GetSolidBrush(foreColor);
		}
	}
	public class SkinGridGroupPanelPainter : GridGroupPanelPainter {
		ISkinProvider provider;
		public SkinGridGroupPanelPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GridSkins.GetSkin(this.provider)[GridSkins.SkinGridGroupPanel], e.Bounds);
			info.RightToLeft = ((StyleObjectInfoArgs)e).RightToLeft;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
}
