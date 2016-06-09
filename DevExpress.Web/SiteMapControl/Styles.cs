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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public enum NodeBulletStyle { NotSet, None, Auto, Disc, Circle, Square };
	public class ColumnHoverStyle : AppearanceSelectedStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return Color.Empty; }
			set { }
		}
	}
	public class ColumnStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ColumnStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColumnStyleHoverStyle"),
#endif
		Category("Layout"), AutoFormatEnable, Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new virtual ColumnHoverStyle HoverStyle {
			get { return (ColumnHoverStyle)base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override string Cursor {
			get { return string.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return Color.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override HorizontalAlign HorizontalAlign {
			get { return HorizontalAlign.NotSet; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override VerticalAlign VerticalAlign {
			get { return VerticalAlign.NotSet; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override DefaultBoolean Wrap {
			get { return DefaultBoolean.Default; }
			set { }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new ColumnHoverStyle();
		}
	}
	public class ColumnSeparatorStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("ColumnSeparatorStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColumnSeparatorStyleWidth"),
#endif
		Category("Layout"), AutoFormatEnable, Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(typeof(Unit), ""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class SiteMapStyles : StylesBase {
		public const string ColumnSeparatorStyleName = "ColumnSeparator";
		public const string ColumnStyleName = "Column";
		private DefaultLevelProperties defaultLevel = new DefaultLevelProperties();
		public SiteMapStyles(ASPxSiteMapControlBase siteMapControl)
			: base(siteMapControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SiteMapStylesColumnSeparator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ColumnSeparatorStyle ColumnSeparator {
			get { return (ColumnSeparatorStyle)GetStyle(ColumnSeparatorStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SiteMapStylesColumn"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ColumnStyle Column {
			get { return (ColumnStyle)GetStyle(ColumnStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SiteMapStylesDefaultLevel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DefaultLevelProperties DefaultLevel {
			get { return defaultLevel; }
		}
		protected override bool MakeLinkStyleAttributesImportant {
			get { return true; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxsm";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ColumnSeparatorStyleName, delegate() { return new ColumnSeparatorStyle(); } ));
			list.Add(new StyleInfo(ColumnStyleName, delegate() { return new ColumnStyle(); } ));
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle style = base.GetDefaultControlStyle();
			style.Paddings.Assign(GetControlPaddings());
			return style;
		}
		protected internal virtual ColumnStyle GetDefaultColumnStyle() {
			ColumnStyle style = new ColumnStyle();
			style.Paddings.Assign(new Paddings(Unit.Pixel(1)));
			return style;
		}
		protected internal virtual ColumnSeparatorStyle GetDefaultColumnSeparatorStyle() {
			ColumnSeparatorStyle style = new ColumnSeparatorStyle();
			style.Paddings.Assign(GetColumnSeparatorPaddings());
			style.Width = Unit.Pixel(1);
			return style;
		}
		public virtual AppearanceStyleBase GetLevelDefaultStyle(int level, bool isCategorized, 
			bool isHorizontalDirection, bool isFlowLayoutLevel) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(GetLevelDefaultProperties(level, isCategorized, isHorizontalDirection, isFlowLayoutLevel).Style);
			style.CopyFrom(CreateStyleByName(GetLevelCssStyleName(level, isCategorized, isFlowLayoutLevel)));
			return style;
		}
		protected string GetLevelCssStyleName(int level, bool isCategorized, bool isFlowLayoutLevel) {
			string cssStyleName = "";
			int curActualPropertyLevel = level;
			if(isCategorized && (level == 0)) {
				cssStyleName = "CategoryLevelStyle";
			} else {
				if(isCategorized && (level > 0))
					curActualPropertyLevel = level - 1;
				cssStyleName = curActualPropertyLevel < 5 ? "LevelStyle" + curActualPropertyLevel.ToString() : "LevelStyleOther";
				cssStyleName += isCategorized && (level == 1) ? "Categorized" : "";
			}
			cssStyleName += isFlowLayoutLevel ? "Flow" : "";
			return cssStyleName;
		}
		protected internal virtual LevelProperties GetLevelDefaultProperties(int level, bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties ret = new LevelProperties();
			if(isCategorized && (level == 0)) {
				ret = GetCategoryLevelProperties(isFlowLayoutLevel);
			} else {
				if(isCategorized)
					level--;
				if(level > -1) {
					switch(level) {
						case 0:
							ret = GetFirstLevelProperties(isCategorized, isHorizontalDirection, isFlowLayoutLevel);
							break;
						case 1:
							ret = GetSecondLevelProperties(isCategorized, isHorizontalDirection, isFlowLayoutLevel);
							break;
						case 2:
							ret = GetThirdLevelProperties(isCategorized, isHorizontalDirection, isFlowLayoutLevel);
							break;
						case 3:
							ret = GetFoursLevelProperties(isCategorized, isHorizontalDirection, isFlowLayoutLevel);
							break;
						default:
							ret = GetFifthLevelProperties(isCategorized, isHorizontalDirection, isFlowLayoutLevel);
							break;
					}
				}
			}
			ret.ImageSpacing = GetNodeImageSpacing();
			return ret;
		}
		public virtual Paddings GetColumnSeparatorPaddings() {
			return new Paddings(Unit.Pixel(16), 0, Unit.Pixel(16), 0);
		}
		protected Paddings GetControlPaddings() {
			return new Paddings(Unit.Pixel(16));
		}
		protected virtual Unit GetNodeImageSpacing() {
			return GetImageSpacing();
		}
		public override Unit GetBulletIndent() {
			if(Browser.IsOpera) {
				if(Browser.MajorVersion == 8)
					return new Unit(12, UnitType.Pixel);
				else
					return new Unit(-29, UnitType.Pixel);
			} else
				if(Browser.Family.IsNetscape)
					return new Unit(-27, UnitType.Pixel);
				else
					if(Browser.Family.IsWebKit || Browser.IsEdge)
						return new Unit(-25, UnitType.Pixel);
					else
						return new Unit(16, UnitType.Pixel);
		}
		public Unit GetFlowLayoutLineTextHeight(int level) {
			if(level <= 0) return Unit.Empty;
			switch(level) {
				case 1:
					return Unit.Percentage(140);
				case 2:
					return Unit.Percentage(120);
				default:
					return Unit.Percentage(100);
			}
		}
		protected virtual LevelProperties GetCategoryLevelProperties(bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.NodeSpacing = 20;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingTop = 9;
			return properties;
		}
		protected virtual LevelProperties GetFirstLevelProperties(bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.NodeSpacing = isHorizontalDirection ? 20 : 11;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingBottom = 14;
			properties.ChildNodesPaddings.PaddingTop = isCategorized ? 5 : 9;
			return properties;
		}
		protected virtual LevelProperties GetSecondLevelProperties(bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.BulletStyle = NodeBulletStyle.Square;
			properties.NodeSpacing = 2;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingBottom = 9;
			properties.ChildNodesPaddings.PaddingTop = 4;
			properties.ChildNodesPaddings.PaddingLeft = 36;
			return properties;
		}
		protected virtual LevelProperties GetThirdLevelProperties(bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.NodeSpacing = 1;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingBottom = 3;
			properties.ChildNodesPaddings.PaddingTop = 3;
			properties.ChildNodesPaddings.PaddingLeft = 20;
			return properties;
		}
		protected virtual LevelProperties GetFoursLevelProperties(bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.NodeSpacing = 1;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingBottom = 3;
			properties.ChildNodesPaddings.PaddingTop = 3;
			properties.ChildNodesPaddings.PaddingLeft = 20;
			return properties;
		}
		protected virtual LevelProperties GetFifthLevelProperties(bool isCategorized, bool isHorizontalDirection,
			bool isFlowLayoutLevel) {
			LevelProperties properties = new LevelProperties();
			properties.NodeSpacing = 1;
			properties.ChildNodesPaddings.Padding = 0;
			properties.ChildNodesPaddings.PaddingBottom = 3;
			properties.ChildNodesPaddings.PaddingTop = 3;
			properties.ChildNodesPaddings.PaddingLeft = 20;
			return properties;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DefaultLevel });
		}
	}
}
