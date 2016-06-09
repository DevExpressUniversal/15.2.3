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
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
namespace DevExpress.DocumentView {
	public enum PageBorderVisibility {
		None,
		AllWithoutSelection,
		All
	}
}
namespace DevExpress.DocumentView {
	public static class PrintingPaintStyles {
		static Dictionary<string, PrintingPaintStyle> PaintStyle;
		static PrintingPaintStyles() {
			PaintStyle = new Dictionary<string, PrintingPaintStyle>();
			PaintStyle.Add(PrintingPaintStylesNames.Flat, new PrintingPaintStyleFlat());
			PaintStyle.Add(PrintingPaintStylesNames.Skin, new PrintingPaintStyleSkin());
		}
		static public PrintingPaintStyle GetPaintStyle(UserLookAndFeel lookAndFeel) {
			string paintStyleName;
			PrintingPaintStyle pps = null;
			switch (lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin:
					paintStyleName = PrintingPaintStylesNames.Skin;
					break;
				case ActiveLookAndFeelStyle.WindowsXP:
				case ActiveLookAndFeelStyle.Office2003:
				case ActiveLookAndFeelStyle.Flat:
				default:
					paintStyleName = PrintingPaintStylesNames.Flat;
					break;
			}
			PaintStyle.TryGetValue(paintStyleName, out pps);
			return pps;
		}
	}
	public static class PrintingPaintStylesNames {
		public const string
							Flat = "Flat",
							Skin = "Skin";
	}
	public abstract class PrintingPaintStyle {
		public abstract string Name { get; }
		public abstract BackgroundPreviewPainter CreateBackgroundPreviewPainter(UserLookAndFeel lookAndFeel);
		public abstract PageBorderPainter CreatePageBorderPainter(UserLookAndFeel lookAndFeel);
	}
	public class PrintingPaintStyleFlat : PrintingPaintStyle {
		public override string Name { get { return PrintingPaintStylesNames.Flat; } }
		public override BackgroundPreviewPainter CreateBackgroundPreviewPainter(UserLookAndFeel lookAndFeel) {
			return new BackgroundPreviewPainterFlat();
		}
		public override PageBorderPainter CreatePageBorderPainter(UserLookAndFeel lookAndFeel) {
			return new PageBorderPainterFlat();
		}
	}
	public class PrintingPaintStyleSkin : PrintingPaintStyle {
		public override string Name { get { return PrintingPaintStylesNames.Skin; } }
		public override BackgroundPreviewPainter CreateBackgroundPreviewPainter(UserLookAndFeel lookAndFeel) {
			return new BackgroundPreviewPainterSkin(lookAndFeel);
		}
		public override PageBorderPainter CreatePageBorderPainter(UserLookAndFeel lookAndFeel) {
			return new PageBorderPainterSkin(lookAndFeel);
		}
	}
	public class BackgroundPreviewPainter : ObjectPainter {
		protected Color fBackColor;
		protected Color fForeColor;
		public Color ForeColor {
			get { return fForeColor; }
		}
		public override void DrawObject(ObjectInfoArgs e) {
			PrintControlInfo viewInfo = (PrintControlInfo)e;
			UpdateColors(viewInfo);
			DrawBackground(viewInfo);
			DrawForeground(viewInfo);
		}
		public void DrawString(PrintControlInfo viewInfo) {
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			using(Brush brush = new SolidBrush(fForeColor))
				viewInfo.Graphics.DrawString(viewInfo.BackgroundText,viewInfo.BackgroundFont, brush,viewInfo.Bounds, sf);
			sf.Dispose();
		}
		protected virtual void UpdateColors(PrintControlInfo viewInfo) {
			fForeColor = viewInfo.ForeColor == Color.Empty ? PrintControlInfo.DefaultForeColor : viewInfo.ForeColor;
			fBackColor = viewInfo.BackColor == Color.Empty ? PrintControlInfo.DefaultBackColor : viewInfo.BackColor;
		}
		protected virtual void DrawBackground(PrintControlInfo viewInfo) {
			viewInfo.Graphics.FillRectangle(new SolidBrush(fBackColor), viewInfo.Bounds);
		}
		protected virtual void DrawForeground(PrintControlInfo viewInfo) {
			if(viewInfo.BackgroundText.Length != 0) {
				DrawString(viewInfo);
			}
		}
	}
	public class BackgroundPreviewPainterFlat : BackgroundPreviewPainter { }
	public class BackgroundPreviewPainterSkin : BackgroundPreviewPainter {
		UserLookAndFeel lookAndFeel;
		public BackgroundPreviewPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		protected override void DrawBackground(PrintControlInfo viewInfo) {
			if(UseBackColor(viewInfo))
				base.DrawBackground(viewInfo);
			else
				SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, PrintingSkins.SkinBackgroundPreview, viewInfo.Bounds);
		}
		protected override void UpdateColors(PrintControlInfo viewInfo) {
			base.UpdateColors(viewInfo);
			if(viewInfo.ForeColor == Color.Empty)
				fForeColor = SkinPaintHelper.GetColor(lookAndFeel, PrintingSkins.OptForeColor);
		}
		bool UseBackColor(PrintControlInfo viewInfo) {
			return viewInfo.BackColor != Color.Empty;
		}
	}
	public class PageBorderPainter : ObjectPainter {
		SkinPaddingEdges pageBorderEdges;
		public SkinPaddingEdges PageBorderEdges {
			get { return pageBorderEdges; }
			set { pageBorderEdges = value; }
		}
		public PageBorderPainter() {
			PageBorderEdges = new SkinPaddingEdges(0);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			PageBorderInfo viewInfo = (PageBorderInfo)e;
			Rectangle temporaryBounds = viewInfo.Bounds;
			Pen pen = new Pen(viewInfo.PageBorderColor, viewInfo.PageBorderWidth);
			pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
				temporaryBounds.Inflate(viewInfo.PageBorderWidth, viewInfo.PageBorderWidth);
			viewInfo.Bounds = temporaryBounds;
			viewInfo.Graphics.DrawRectangle(pen, viewInfo.Bounds);
		}
		public virtual void UpdatePageBorderEdges(int Left, int Right, int Top, int Bottom) {
			pageBorderEdges.Left = Left;
			pageBorderEdges.Right = Right;
			pageBorderEdges.Top = Top;
			pageBorderEdges.Bottom = Bottom;
		}
		public virtual float GetPageVerticalIndent() {
			return 50;
		}
		public virtual float GetPageHorizontalIndent() {
			return 50;
		}
	}
	public class PageBorderPainterFlat : PageBorderPainter { }
	public class PageBorderPainterSkin : PageBorderPainter {
		UserLookAndFeel lookAndFeel;
		public PageBorderPainterSkin(UserLookAndFeel lookAndFeel):base() {
			this.lookAndFeel = lookAndFeel;
			PageBorderEdges = SkinPaintHelper.GetSkinEdges(lookAndFeel, PrintingSkins.SkinBorderPage);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			PageBorderInfo viewInfo = (PageBorderInfo)e;
			SkinPaddingEdges spe = SkinPaintHelper.GetSkinEdges(lookAndFeel, PrintingSkins.SkinBorderPage);
			viewInfo.Bounds =  spe.Inflate(viewInfo.Bounds);
			if(viewInfo.State == ObjectState.Selected)
				SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, PrintingSkins.SkinBorderPage, viewInfo.Bounds, 0);
			else
				SkinPaintHelper.DrawSkinElement(lookAndFeel, viewInfo.Cache, PrintingSkins.SkinBorderPage, viewInfo.Bounds, 1);
		}
		public override void UpdatePageBorderEdges(int Left, int Right, int Top, int Bottom) {			
		}
		public override float GetPageVerticalIndent() {
			return SkinPaintHelper.GetInteger(lookAndFeel, PrintingSkins.OptPageVerticalIndent);
		}
		public override float GetPageHorizontalIndent() {
			return SkinPaintHelper.GetInteger(lookAndFeel, PrintingSkins.OptPageHorizontalIndent);
		}
	}
	public static class SkinPaintHelper {
		static SkinHelperBase helper = new SkinHelperBase(SkinProductId.Printing);
		public static SkinElement GetSkinElement(ISkinProvider provider, string elementName) {
			return helper.GetSkinElement(provider, elementName);
		}
		public static SkinElementInfo GetSkinElementInfo(ISkinProvider skinProvider, string elementName, Rectangle bounds) {
			return helper.GetSkinElementInfo(skinProvider, elementName, bounds);
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds) {
			helper.DrawSkinElement(lookAndFeel, cache, elementName, bounds);
		}
		public static void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex) {
			helper.DrawSkinElement(lookAndFeel, cache, elementName, bounds, imageIndex);
		}
		public static SkinPaddingEdges GetSkinEdges(UserLookAndFeel lookAndFeel, string skinElementName) {
			return helper.GetSkinEdges(lookAndFeel, skinElementName);
		}
		public static Color GetColor(UserLookAndFeel lookAndFeel, string colorName) {
			return helper.GetColor(lookAndFeel, colorName);
		}
		public static Int32 GetInteger(UserLookAndFeel lookAndFeel, string integerName) {
			return helper.GetInteger(lookAndFeel, integerName);
		}
		public static void SetForeColors(ISkinProvider skinProvider, params DevExpress.Utils.AppearanceObject[] appearances) {
			Color foreColor = GetSystemColor(skinProvider, SystemColors.ControlText);
			foreach(DevExpress.Utils.AppearanceObject appearance in appearances)
				appearance.ForeColor = foreColor;
		}
		static Color GetSystemColor(ISkinProvider skinProvider, Color color) {
			DevExpress.Skins.Skin skin = DevExpress.Skins.CommonSkins.GetSkin(skinProvider);
			return skin != null ? skin.GetSystemColor(color) : color;
		}
	}
	class PageBorderInfo : ObjectInfoArgs {
		PrintControlInfo printControlInfo;
		public Color PageBorderColor {
			get { return State == ObjectState.Selected ? printControlInfo.SelectedPageBorderColor : printControlInfo.PageBorderColor; }
		}
		public int PageBorderWidth {
			get { return State == ObjectState.Selected ? printControlInfo.SelectedPageBorderWidth : printControlInfo.PageBorderWidth; }
		}
		public PageBorderInfo(PrintControlInfo printControlInfo)
			: base() {
			State = ObjectState.Normal;
			this.printControlInfo = printControlInfo;
			Bounds = printControlInfo.PageBounds;
		}
	}
	public class PrintControlInfo : ObjectInfoArgs, IDisposable {
		public const PageBorderVisibility DefaultPageBorderVisibility = PageBorderVisibility.All;
		public static readonly Color DefaultBackColor = SystemColors.AppWorkspace;
		public static readonly Color DefaultForeColor = Color.White;
		public const int DefaultPageBorderWidth = 1;
		public static readonly Color DefaultPageBorderColor = SystemColors.WindowFrame;
		public const int DefaultSelectedPageBorderWidth = 2;
		public static readonly Color DefaultSelectedPageBorderColor = SystemColors.ActiveCaption;
		public const int NotSetWidth = -1;
		#region fields & properties
		Color backColor;
		Color foreColor;
		string backgroundText;
		Rectangle pageBounds;
		int pageBorderWidth;
		Color pageBorderColor;
		int selectedPageBorderWidth;
		Color selectedPageBorderColor;
		PageBorderVisibility pageBorderVisibility;
		System.Drawing.Font textFont;
		public Color BackColor {
			get { return backColor; }
			set {
				backColor = value;
				if(backColor == Color.Transparent)
					backColor = Color.Empty;
				if(backColor != Color.Empty)
					backColor = Color.FromArgb(255, value);
			}
		}
		public Color ForeColor {
			get { return foreColor; }
			set { foreColor = value; }
		}
		public float PageHorizontalIndent {
			get { return pagePainter.GetPageHorizontalIndent(); }
		}
		public float PageVerticalIndent {
			get { return pagePainter.GetPageVerticalIndent(); }
		}
		public string BackgroundText {
			get { return backgroundText; }
			set { backgroundText = value; }
		}
		public Font BackgroundFont {
			get { return textFont; }
		}
		public Rectangle PageBounds {
			get { return pageBounds; }
			set { pageBounds = value; }
		}
		public SkinPaddingEdges PagePaddingEdges {
			get {
				pagePainter.UpdatePageBorderEdges(Math.Max(PageBorderWidth, SelectedPageBorderWidth), Math.Max(PageBorderWidth, SelectedPageBorderWidth), Math.Max(PageBorderWidth, SelectedPageBorderWidth), Math.Max(PageBorderWidth, SelectedPageBorderWidth));
				return pagePainter.PageBorderEdges;
			}
		}
		public int PageBorderWidth {
			get { return pageBorderWidth == NotSetWidth ? DefaultPageBorderWidth : pageBorderWidth; }
			set { pageBorderWidth = value > 0 ? value : NotSetWidth; }
		}
		public Color PageBorderColor {
			get { return pageBorderColor == Color.Empty ? DefaultPageBorderColor : pageBorderColor; }
			set { pageBorderColor = value; }
		}
		public int SelectedPageBorderWidth {
			get { return selectedPageBorderWidth == NotSetWidth ? DefaultSelectedPageBorderWidth : selectedPageBorderWidth; }
			set { selectedPageBorderWidth = value > 0 ? value : NotSetWidth; }
		}
		public Color SelectedPageBorderColor {
			get { return selectedPageBorderColor == Color.Empty ? DefaultSelectedPageBorderColor : selectedPageBorderColor; }
			set { selectedPageBorderColor = value; }
		}
		public PageBorderVisibility PageBorderVisibility {
			get { return pageBorderVisibility; }
			set { pageBorderVisibility = value; }
		}
		public System.Drawing.Font TextFont {
			get { return textFont; }
		}
		public Color BackgroundForeColor {
			get { return backgroundPainter.ForeColor; }
		}
		#endregion // fields & properties
		PageBorderPainter pagePainter;
		BackgroundPreviewPainter backgroundPainter;
		public void DrawPreviewBackground(GraphicsCache gr) {
			ObjectPainter.DrawObject(gr, backgroundPainter, this);
		}
		public void DrawSelectedPageBorder(GraphicsCache gr) {
			if(pageBorderVisibility == PageBorderVisibility.All) {
				PageBorderInfo pageInfo = new PageBorderInfo(this);
				pageInfo.State = ObjectState.Selected;
				ObjectPainter.DrawObject(gr, pagePainter, pageInfo);
			} else
				if(pageBorderVisibility == PageBorderVisibility.AllWithoutSelection)
					DrawPageBorder(gr);
		}
		public void DrawPageBorder(GraphicsCache gr) {
			if(pageBorderVisibility == PageBorderVisibility.AllWithoutSelection || pageBorderVisibility == PageBorderVisibility.All) {
				PageBorderInfo pageInfo = new PageBorderInfo(this);
				pageInfo.State = ObjectState.Normal;
				ObjectPainter.DrawObject(gr, pagePainter, pageInfo);
			}
		}
		public PrintControlInfo(BackgroundPreviewPainter backgroundPainter, PageBorderPainter pagePainter) {
			backColor = Color.Empty;
			foreColor = Color.Empty;
			pageBorderColor = Color.Empty;
			pageBorderWidth = NotSetWidth;
			selectedPageBorderColor = Color.Empty;
			SelectedPageBorderWidth = NotSetWidth;
			textFont = new Font(System.Windows.Forms.Control.DefaultFont, FontStyle.Bold);
			UpdatePainters(backgroundPainter, pagePainter);
		}
		public void UpdatePainters(BackgroundPreviewPainter backgroundPainter, PageBorderPainter pagePainter) {
			this.pagePainter = pagePainter;
			this.backgroundPainter = backgroundPainter;
		}
		#region IDisposable Members
		public void Dispose() {
			textFont.Dispose();
		}
		#endregion
	}
}
