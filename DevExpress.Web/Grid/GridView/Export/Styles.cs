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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
using System.Linq;
namespace DevExpress.Web {
	public class GridViewExportAppearance : GridExportAppearanceBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportAppearanceBorderSize"),
#endif
 NotifyParentProperty(true), DefaultValue(1)]
		public override int BorderSize { get { return base.BorderSize; } set { base.BorderSize = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportAppearanceBorderColor"),
#endif
 NotifyParentProperty(true)]
		public override Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportAppearanceBorderSides"),
#endif
 NotifyParentProperty(true), DefaultValue(BorderSide.All)]
		public override BorderSide BorderSides { get { return base.BorderSides; } set { base.BorderSides = value; } }
	}
	public abstract class GridExportAppearanceBase : GridViewStyleBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string CssClass { get { return base.CssClass; } set { } }
		public virtual int BorderSize {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderSize", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "BorderSize");
				ViewStateUtils.SetIntProperty(ViewState, "BorderSize", 1, value);
			}
		}
		public virtual new Color BorderColor { get { return Border.BorderColor; } set { Border.BorderColor = value; } }
		public virtual BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)BorderSide.All); }
			set { ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)BorderSide.All, (int)value); }
		}
		public override void CopyFrom(System.Web.UI.WebControls.Style style) {
			base.CopyFrom(style);
			if(style is GridExportAppearanceBase) {
				var exportStyle = style as GridExportAppearanceBase;
				BorderSize = exportStyle.BorderSize;
				BorderSides = exportStyle.BorderSides;
				if(!exportStyle.BorderColor.Equals(Color.Empty))
					BorderColor = exportStyle.BorderColor;
			}
		}
		public override void MergeWith(System.Web.UI.WebControls.Style style) {
			base.MergeWith(style);
			if(style is GridExportAppearanceBase) {
				var exportStyle = style as GridExportAppearanceBase;
				BorderSize = exportStyle.BorderSize;
				BorderSides = exportStyle.BorderSides;
				if(BorderColor.Equals(Color.Empty))
					BorderColor = exportStyle.BorderColor;
			}
		}
	}
	public class GridViewExportOptionalAppearance : GridExportOptionalAppearanceBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportOptionalAppearanceEnabled"),
#endif
 NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public override DefaultBoolean Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
	}
	public class GridExportOptionalAppearanceBase : GridViewExportAppearance {
		public virtual DefaultBoolean Enabled {
			get { return (DefaultBoolean)ViewStateUtils.GetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default, value); }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			GridViewExportOptionalAppearance exportStyle = style as GridViewExportOptionalAppearance;
			if(exportStyle == null)
				return;
			Enabled = exportStyle.Enabled;
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			GridViewExportOptionalAppearance exportStyle = style as GridViewExportOptionalAppearance;
			if(exportStyle == null)
				return;
			if(Enabled == DefaultBoolean.Default)
				Enabled = exportStyle.Enabled;
		}
	}
	public class GridViewExportStyles : PropertiesBase {
		GridViewExportAppearance header, cell, footer, groupFooter, groupRow, preview, _default, title, hyperLink, image;
		GridViewExportOptionalAppearance altCell;
		public GridViewExportStyles(IPropertiesOwner owner)
			: base(owner) {
			this._default = new GridViewExportAppearance();
			this.header = new GridViewExportAppearance();
			this.cell = new GridViewExportAppearance();
			this.footer = new GridViewExportAppearance();
			this.groupFooter = new GridViewExportAppearance();
			this.groupRow = new GridViewExportAppearance();
			this.preview = new GridViewExportAppearance();
			this.title = new GridViewExportAppearance();
			this.hyperLink = new GridViewExportAppearance();
			this.image = new GridViewExportAppearance();
			this.altCell = new GridViewExportOptionalAppearance();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesDefault"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Default { get { return _default; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Header { get { return header; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Cell { get { return cell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Footer { get { return footer; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesGroupFooter"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance GroupFooter { get { return groupFooter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesGroupRow"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance GroupRow { get { return groupRow; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesPreview"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Preview { get { return preview; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesTitle"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Title { get { return title; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesHyperLink"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance HyperLink { get { return hyperLink; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Image { get { return image; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExportStylesAlternatingRowCell"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportOptionalAppearance AlternatingRowCell { get { return altCell; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			GridViewExportStyles styles = source as GridViewExportStyles;
			if(styles != null) {
				Default.Assign(styles.Default);
				Cell.Assign(styles.Cell);
				Header.Assign(styles.Header);
				GroupRow.Assign(styles.GroupRow);
				Preview.Assign(styles.Preview);
				Footer.Assign(styles.Footer);
				GroupFooter.Assign(styles.GroupFooter);
				Title.Assign(styles.Title);
				HyperLink.Assign(styles.HyperLink);
				Image.Assign(styles.Image);
				AlternatingRowCell.Assign(styles.AlternatingRowCell);
			}
		}
	}
}
namespace DevExpress.Web.Export {
	public class GridExportStyleHelper : ExportStyleHelperBase {
		GridViewExportStyles renderStyles;
		public GridExportStyleHelper(GridViewExportStyles userStyles)
			: base(userStyles) {
		}
		GridViewExportStyles RenderStyles {
			get {
				if(renderStyles == null)
					renderStyles = new GridViewExportStyles(null);
				return renderStyles;
			}
		}
		protected override void InitStyles(PropertiesBase userStyles) {
			SetupBuiltInDefault();
			Import(RenderStyles.Default);
			SetupBuiltInStyles();
			var gridViewExportUserStyles = (GridViewExportStyles)userStyles;
			Import(gridViewExportUserStyles.Default);
			Import(gridViewExportUserStyles);
		}
		void SetupBuiltInDefault() {
			RenderStyles.Default.BackColor = Color.White;
			RenderStyles.Default.ForeColor = Color.Black;
			RenderStyles.Default.BorderColor = Color.DarkGray;
			RenderStyles.Default.Paddings.PaddingLeft = RenderStyles.Default.Paddings.PaddingRight = Unit.Pixel(2);
			RenderStyles.Default.Paddings.PaddingTop = RenderStyles.Default.Paddings.PaddingBottom = Unit.Pixel(1);
		}
		void SetupBuiltInStyles() {
			RenderStyles.Header.BackColor = Color.Gray;
			RenderStyles.Header.ForeColor = Color.White;
			RenderStyles.Header.HorizontalAlign = HorizontalAlign.Center;
			RenderStyles.GroupRow.BackColor = Color.LightGray;
			RenderStyles.Header.ForeColor = Color.White;
			RenderStyles.Preview.ForeColor = Color.Blue;
			RenderStyles.Footer.BackColor = Color.LightYellow;
			RenderStyles.GroupFooter.BackColor = Color.LightYellow;
			RenderStyles.Title.HorizontalAlign = HorizontalAlign.Center;
			RenderStyles.Title.Font.Bold = true;
			RenderStyles.HyperLink.ForeColor = Color.Blue;
			RenderStyles.HyperLink.Font.Underline = true;
			RenderStyles.HyperLink.BackColor = Color.Empty;
			RenderStyles.AlternatingRowCell.BackColor = Color.FromArgb(0xededeb);
		}
		void Import(Style commonStyle) {
			RenderStyles.Cell.CopyFrom(commonStyle);
			RenderStyles.Header.CopyFrom(commonStyle);
			RenderStyles.GroupRow.CopyFrom(commonStyle);
			RenderStyles.Preview.CopyFrom(commonStyle);
			RenderStyles.Footer.CopyFrom(commonStyle);
			RenderStyles.GroupFooter.CopyFrom(commonStyle);
			RenderStyles.Title.CopyFrom(commonStyle);
			RenderStyles.HyperLink.CopyFrom(commonStyle);
			RenderStyles.Image.CopyFrom(commonStyle);
			RenderStyles.AlternatingRowCell.CopyFrom(commonStyle);
		}
		void Import(GridViewExportStyles styles) {
			RenderStyles.Cell.CopyFrom(styles.Cell);
			RenderStyles.Header.CopyFrom(styles.Header);
			RenderStyles.GroupRow.CopyFrom(styles.GroupRow);
			RenderStyles.Preview.CopyFrom(styles.Preview);
			RenderStyles.Footer.CopyFrom(styles.Footer);
			RenderStyles.GroupFooter.CopyFrom(styles.GroupFooter);
			RenderStyles.Title.CopyFrom(styles.Title);
			RenderStyles.HyperLink.CopyFrom(styles.HyperLink);
			RenderStyles.Image.CopyFrom(styles.Image);
			RenderStyles.AlternatingRowCell.CopyFrom(styles.AlternatingRowCell);
		}
		public DefaultBoolean AlternatingRowEnabled() {
			return RenderStyles.AlternatingRowCell.Enabled;
		}
		public BrickStyle GetTitlePanelStyle(BrickGraphics graph, HorizontalAlign gridAlign) {
			object[] key = new object[] { TitleStyleCacheKey };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateTitlePanelStyle(graph, gridAlign);
			return StyleCache[key];			
		}
		BrickStyle CreateTitlePanelStyle(BrickGraphics graph, HorizontalAlign gridAlign) {
			GridViewExportAppearance style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.Title);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = gridAlign;
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Center;
			return ConvertStyle(graph, style);
		}
		public BrickStyle GetHeaderStyle(BrickGraphics graph, HorizontalAlign align) {
			object[] key = new object[] { 
				HeaderStyleCacheKey,
				align
			};
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateHeaderStyle(graph, align);
			return StyleCache[key];  
		}
		BrickStyle CreateHeaderStyle(BrickGraphics graph, HorizontalAlign align) {
			GridViewExportAppearance style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.Header);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
		public BrickStyle GetGroupRowStyle(BrickGraphics graph) {
			object[] key = new object[] { GroupRowStyleCacheKey };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateGroupRowStyle(graph);
			return StyleCache[key];
		}
		BrickStyle CreateGroupRowStyle(BrickGraphics graph) {
			return ConvertStyle(graph, RenderStyles.GroupRow);
		}
		public BrickStyle GetPreviewRowStyle(BrickGraphics graph) {
			object[] key = new object[] { PreviewRowStyleCacheKey };
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreatePreviewRowStyle(graph);
			return StyleCache[key];						
		}
		BrickStyle CreatePreviewRowStyle(BrickGraphics graph) {
			return ConvertStyle(graph, RenderStyles.Preview);
		}
		public BrickStyle GetGroupFooterStyle(BrickGraphics graph, HorizontalAlign align) {
			object[] key = new object[] { 
				GroupFooterStyleCacheKey,
				align
			};
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateGroupFooterStyle(graph, align);
			return StyleCache[key];			
		}
		BrickStyle CreateGroupFooterStyle(BrickGraphics graph, HorizontalAlign align) {
			GridViewExportAppearance style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.GroupFooter);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
		protected override BrickStyle CreateCellBrickStyle(BrickGraphics graph, IWebGridDataColumn webColumn, int rowIndex, bool alternating, HorizontalAlign align, bool isLink, bool isImage, bool useCondFormating) {
			var style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.Cell);
			var column = (GridViewColumn)webColumn;
			style.CopyFrom(column.ExportCellStyle);
			if(useCondFormating)
				style.CopyFrom(GetFormatConditionsCellStyle(webColumn, rowIndex));
			if(alternating)
				style.CopyFrom(RenderStyles.AlternatingRowCell);
			if(isLink)
				style.CopyFrom(RenderStyles.HyperLink);
			if(isImage)
				style.CopyFrom(RenderStyles.Image);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
		protected override BrickStyle CreateFooterStyle(BrickGraphics graph, HorizontalAlign align) {
			var style = new GridViewExportAppearance();
			style.CopyFrom(RenderStyles.Footer);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = align;
			return ConvertStyle(graph, style);
		}
	}
	public abstract class ExportStyleHelperBase : IDisposable {
		protected const int
			TitleStyleCacheKey = 0,
			GroupRowStyleCacheKey = 1,
			PreviewRowStyleCacheKey = 2,
			CellStyleCacheKey = 3,
			HeaderStyleCacheKey = 4,
			GroupFooterStyleCacheKey = 5,
			FooterStyleCacheKey = 6;
		internal class CacheKeyComparer : IEqualityComparer<object[]> {
			bool IEqualityComparer<object[]>.Equals(object[] x, object[] y) {
				if(x.Length != y.Length)
					return false;
				for(int i = x.Length - 1; i >= 0; i--) {
					if(!Equals(x[i], y[i]))
						return false;
				}
				return true;
			}
			int IEqualityComparer<object[]>.GetHashCode(object[] obj) {
				int result = 0;
				for(int i = obj.Length - 1; i >= 0; i--) {
					result ^= obj[i].GetHashCode();
				}
				return result;
			}
		}
		Dictionary<object[], Font> fontCache;
		Dictionary<object[], BrickStyle> styleCache;
		public ExportStyleHelperBase(PropertiesBase userStyles) {
			InitStyles(userStyles);
		}
		protected abstract void InitStyles(PropertiesBase userStyles);
		protected Dictionary<object[], Font> FontCache {
			get {
				if(fontCache == null)
					fontCache = new Dictionary<object[], Font>(new CacheKeyComparer());
				return fontCache;
			}
		}
		protected Dictionary<object[], BrickStyle> StyleCache {
			get {
				if(styleCache == null)
					styleCache = new Dictionary<object[], BrickStyle>(new CacheKeyComparer());
				return styleCache;
			}
		}
		public virtual BrickStyle GetCellStyle(BrickGraphics graph, IWebGridDataColumn column, int rowIndex, bool alternating, HorizontalAlign align, bool isLink, bool isImage = false) {
			if(DrawFormatConditionsStyles(column))
				return CreateCellBrickStyle(graph, column, rowIndex, alternating, align, isLink, isImage, true);
			object[] key = new object[] { 
				CellStyleCacheKey, 
				column,
				alternating, 
				align, 
				isLink,
				isImage
			};
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateCellBrickStyle(graph, column, rowIndex, alternating, align, isLink, isImage, false);
			return StyleCache[key];
		}
		protected abstract BrickStyle CreateCellBrickStyle(BrickGraphics graph, IWebGridDataColumn column, int rowIndex, bool alternating, HorizontalAlign align, bool isLink, bool isImage, bool useCondFormating);
		bool DrawFormatConditionsStyles(IWebGridDataColumn column) {
			var grid = GetGrid(column);
			if(grid == null)
				return false;
			var formatConditions = grid.FormatConditions;
			return formatConditions.GetActiveColumnCellConditions(column).Any() || formatConditions.GetActiveItemConditions().Any();
		}
		protected AppearanceStyle GetFormatConditionsCellStyle(IWebGridDataColumn column, int rowIndex) {
			var grid = GetGrid(column);
			if(grid == null)
				return null;
			var formatConditions = grid.FormatConditions;
			var style = grid.RenderHelper.GetConditionalFormatItemStyle(rowIndex);
			style.CopyFrom(grid.RenderHelper.GetConditionalFormatCellStyle(column, rowIndex));
			return style;
		}
		ASPxGridBase GetGrid(IWebGridDataColumn column) {
			return column != null ? column.Adapter.Grid : null;
		}
		public BrickStyle GetFooterStyle(BrickGraphics graph, HorizontalAlign align) {
			object[] key = new object[] { 
				FooterStyleCacheKey,
				align
			};
			if(!StyleCache.ContainsKey(key))
				StyleCache[key] = CreateFooterStyle(graph, align);
			return StyleCache[key];
		}
		protected abstract BrickStyle CreateFooterStyle(BrickGraphics graph, HorizontalAlign align);
		protected BrickStyle ConvertStyle(BrickGraphics graph, GridExportAppearanceBase style) {
			BrickStyle brickStyle = new BrickStyle();
			brickStyle.Font = GetFontByFontInfo(graph.DefaultFont, style.Font);
			brickStyle.BorderColor = style.BorderColor;
			brickStyle.BorderWidth = style.BorderSize;
			brickStyle.BorderStyle = BrickBorderStyle.Center;
			brickStyle.Sides = style.BorderSides;
			brickStyle.BackColor = style.BackColor.IsEmpty ? graph.BackColor : style.BackColor;
			brickStyle.ForeColor = style.ForeColor.IsEmpty ? graph.ForeColor : style.ForeColor;
			if(!style.Paddings.IsEmpty)
				brickStyle.Padding = CreatePaddingByStylePadding(style.Paddings);
			brickStyle.TextAlignment = TextAlignmentConverter.ToTextAlignment(GetBrickHorzAlignment(style.HorizontalAlign), GetBrickVertAlignment(style.VerticalAlign));
			brickStyle.StringFormat = BrickStringFormat.Create(brickStyle.TextAlignment, style.Wrap == DefaultBoolean.Default || style.Wrap == DefaultBoolean.True);
			return brickStyle;
		}
		public Font GetFontByFontInfo(Font defaultFont, FontInfo fontInfo) {
			object[] key = new object[] {
				fontInfo.Bold,
				fontInfo.Italic,
				fontInfo.Strikeout,
				fontInfo.Underline,
				GetFontSize(fontInfo.Size, defaultFont.Size),
				fontInfo.Name
			};
			if(!FontCache.ContainsKey(key))
				FontCache[key] = CreateFontByFontInfo(defaultFont, fontInfo);
			return FontCache[key];
		}
		internal static Font CreateFontByFontInfo(Font defaultFont, FontInfo fontInfo) {
			FontStyle fontStyle = FontStyle.Regular;
			if(fontInfo.Bold)
				fontStyle |= FontStyle.Bold;
			if(fontInfo.Italic)
				fontStyle |= FontStyle.Italic;
			if(fontInfo.Strikeout)
				fontStyle |= FontStyle.Strikeout;
			if(fontInfo.Underline)
				fontStyle |= FontStyle.Underline;
			float emSize = defaultFont.Size;
			string familyName = string.IsNullOrEmpty(fontInfo.Name) ? defaultFont.Name : fontInfo.Name;
			if(!fontInfo.Size.IsEmpty)
				emSize = GetFontSize(fontInfo.Size, emSize);
			return new Font(familyName, emSize, fontStyle);
		}
		protected static PaddingInfo CreatePaddingByStylePadding(Paddings paddings) {
			int left = GetPaddingValue(paddings.PaddingLeft, paddings.Padding);
			int right = GetPaddingValue(paddings.PaddingRight, paddings.Padding);
			int top = GetPaddingValue(paddings.PaddingTop, paddings.Padding);
			int bottom = GetPaddingValue(paddings.PaddingBottom, paddings.Padding);
			return new PaddingInfo(left, right, top, bottom);
		}
		static int GetPaddingValue(Unit unit, Unit commonUnit) {
			if(unit.IsEmpty)
				unit = commonUnit;
			if(unit.IsEmpty || unit.Type != UnitType.Pixel)
				return 0;
			return (int)unit.Value;
		}
		static float GetBorderWidth(Unit unit, float defaultWidth) {
			if(unit.IsEmpty || unit.Type != UnitType.Pixel)
				return defaultWidth;
			return (float)unit.Value;
		}
		static float GetFontSize(FontUnit size, float defaultSize) {
			if(size.Type == FontSize.NotSet || size.Type == FontSize.Medium)
				return defaultSize;
			if(size.Type == FontSize.AsUnit && !size.Unit.IsEmpty)
				return (float)size.Unit.Value;
			float rank = 10, defaultRank = 10;
			switch(size.Type) {
				case FontSize.Large:
					rank = 14;
					break;
				case FontSize.Larger:
					rank = 16;
					break;
				case FontSize.XLarge:
					rank = 20;
					break;
				case FontSize.XXLarge:
					rank = 24;
					break;
				case FontSize.Small:
					rank = 8;
					break;
				case FontSize.Smaller:
					rank = 6;
					break;
				case FontSize.XSmall:
					rank = 5;
					break;
				case FontSize.XXSmall:
					rank = 4;
					break;
			}
			return defaultSize * rank / defaultRank;
		}
		protected static HorzAlignment GetBrickHorzAlignment(HorizontalAlign align) {
			switch(align) {
				case HorizontalAlign.Right:
					return HorzAlignment.Far;
				case HorizontalAlign.Center:
					return HorzAlignment.Center;
			}
			return HorzAlignment.Near;
		}
		static VertAlignment GetBrickVertAlignment(VerticalAlign align) {
			switch(align) {
				case VerticalAlign.Bottom:
					return VertAlignment.Bottom;
				case VerticalAlign.Top:
					return VertAlignment.Top;
			}
			return VertAlignment.Center;
		}
		public Size CalcTextSize(BrickGraphics graph, string text, BrickStyle brickStyle, int maxWidth) {
			int horPaddings = brickStyle.Padding.Left + brickStyle.Padding.Right;
			maxWidth -= horPaddings;
			SizeF sizeF = DevExpress.XtraPrinting.Native.Measurement.MeasureString(text, brickStyle.Font, maxWidth, brickStyle.StringFormat.Value, graph.PageUnit);
			RectangleF rect = new RectangleF(PointF.Empty, sizeF);
			rect = brickStyle.InflateBorderWidth(rect, GraphicsDpi.Pixel);
			Size size = Size.Ceiling(rect.Size);
			size.Width += horPaddings;
			size.Height += brickStyle.Padding.Top + brickStyle.Padding.Bottom;
			return size;
		}
		public Size CalcImageSize(int width, int height, BrickGraphics graph, BrickStyle brickStyle, bool paddingsInside) {
			RectangleF rect = new RectangleF(PointF.Empty, new Size(width, height));
			rect = brickStyle.InflateBorderWidth(rect, GraphicsDpi.Pixel);
			Size imageSize = Size.Ceiling(rect.Size);
			int horPaddings = brickStyle.Padding.Left + brickStyle.Padding.Right;
			if(paddingsInside)
				horPaddings = -horPaddings;
			imageSize.Width += horPaddings;
			imageSize.Height += brickStyle.Padding.Top + brickStyle.Padding.Bottom;
			return imageSize;
		}
		void System.IDisposable.Dispose() {
			this.styleCache = null;
			this.fontCache = null;
		}
	}
}
