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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class DataViewStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylePagerPanelSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit PagerPanelSpacing {
			get { return ImageSpacing; }
			set { ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStyleItemSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit ItemSpacing {
			get { return Spacing; }
			set { Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class DataViewItemStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get {return base.HoverStyle;}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewItemStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewItemStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class DataViewEmptyItemStyle : DataViewItemStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class DataViewContentStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get {return base.HoverStyle;}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class DataViewEmptyDataStyle : AppearanceStyle {
		private static readonly object horizontalAlignCenter = HorizontalAlign.Center;
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewEmptyDataStyleHorizontalAlign"),
#endif
DefaultValue(HorizontalAlign.Center)]
		public override HorizontalAlign HorizontalAlign {
			get { return (HorizontalAlign)ViewStateUtils.GetEnumProperty(ReadOnlyViewState, "HorizontalAlign", horizontalAlignCenter); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "HorizontalAlign", horizontalAlignCenter, value); }
		}
		#region Hiding unused properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		#endregion
	}
	public class DataViewPagerShowMoreItemsContainerStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
	}
	public class DataViewStyles : StylesBase {
		public const string ContentStyleName = "Content";
		public const string ItemStyleName = "Item";
		public const string EmptyItemStyleName = "EmptyItem";
		public const string PagerStyleName = "Pager";
		public const string PagerButtonStyleName = "PagerButton";
		public const string PagerDisabledButtonStyleName = "PagerDisabledButton";
		public const string PagerPageNumberStyleName = "PagerPageNumber";
		public const string PagerCurrentPageNumberStyleName = "PagerCurrentPageNumber";
		public const string PagerPanelStyleName = "PagerPanel";
		public const string PagerSummaryStyleName = "PagerSummary";
		public const string PagerPageSizeItemStyleName = "PagerPageSizeItem";
		public const string PagerShowMoreItemsContainerStyleName = "PagerShowMoreItemsContainer";
		public const string EmptyDataStyleName = "EmptyData";	 
		public DataViewStyles(ASPxDataViewBase dataView)
			: base(dataView) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewContentStyle Content {
			get { return (DataViewContentStyle)GetStyle(ContentStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewItemStyle Item {
			get { return (DataViewItemStyle)GetStyle(ItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesEmptyItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewEmptyItemStyle EmptyItem {
			get { return (DataViewEmptyItemStyle)GetStyle(EmptyItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPager"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerStyle Pager {
			get { return (PagerStyle)GetStyle(PagerStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerButtonStyle PagerButton {
			get { return (PagerButtonStyle)GetStyle(PagerButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerCurrentPageNumber"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerTextStyle PagerCurrentPageNumber {
			get { return (PagerTextStyle)GetStyle(PagerCurrentPageNumberStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerDisabledButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerButtonStyle PagerDisabledButton {
			get { return (PagerButtonStyle)GetStyle(PagerDisabledButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerPageNumber"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerTextStyle PagerPageNumber {
			get { return (PagerTextStyle)GetStyle(PagerPageNumberStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerSummary"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerTextStyle PagerSummary {
			get { return (PagerTextStyle)GetStyle(PagerSummaryStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerPageSizeItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public PagerPageSizeItemStyle PagerPageSizeItem
		{
			get { return (PagerPageSizeItemStyle)GetStyle(PagerPageSizeItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesPagerPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewContentStyle PagerPanel {
			get { return (DataViewContentStyle)GetStyle(PagerPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewStylesEmptyData"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewEmptyDataStyle EmptyData {
			get { return (DataViewEmptyDataStyle)GetStyle(EmptyDataStyleName); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DataViewPagerShowMoreItemsContainerStyle PagerShowMoreItemsContainer {
			get { return (DataViewPagerShowMoreItemsContainerStyle)GetStyle(PagerShowMoreItemsContainerStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxdv";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ContentStyleName, delegate() { return new DataViewContentStyle(); } ));
			list.Add(new StyleInfo(ItemStyleName, delegate() { return new DataViewItemStyle(); } ));
			list.Add(new StyleInfo(EmptyItemStyleName, delegate() { return new DataViewEmptyItemStyle(); } ));
			list.Add(new StyleInfo(PagerStyleName, delegate() { return new PagerStyle(); } ));
			list.Add(new StyleInfo(PagerButtonStyleName, delegate() { return new PagerButtonStyle(); } ));
			list.Add(new StyleInfo(PagerCurrentPageNumberStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(PagerDisabledButtonStyleName, delegate() { return new PagerButtonStyle(); } ));
			list.Add(new StyleInfo(PagerPageNumberStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(PagerSummaryStyleName, delegate() { return new PagerTextStyle(); } ));
			list.Add(new StyleInfo(PagerPageSizeItemStyleName, delegate() { return new PagerPageSizeItemStyle(); }));
			list.Add(new StyleInfo(PagerPanelStyleName, delegate() { return new DataViewContentStyle(); } ));
			list.Add(new StyleInfo(PagerShowMoreItemsContainerStyleName, delegate() { return new DataViewPagerShowMoreItemsContainerStyle(); }));
			list.Add(new StyleInfo(EmptyDataStyleName, delegate() { return new DataViewEmptyDataStyle(); }));
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			DataViewStyle style = new DataViewStyle();
			style.CopyFrom(base.GetDefaultControlStyle());
			style.Spacing = GetItemSpacing();
			style.ImageSpacing = GetPagerPanelSpacing();
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultFlowItemsContainerStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(CreateStyleByName("FlowItemsContainerStyle"));
			return style;
		}
		protected internal virtual AppearanceStyleBase GetDefaultFlowItemStyle() {
			DataViewItemStyle style = new DataViewItemStyle();
			style.CopyFrom(CreateStyleByName("FlowItemStyle"));
			style.Width = GetItemWidth();
			return style;
		}
		protected internal virtual DataViewContentStyle GetDefaultContentStyle() {
			DataViewContentStyle style = new DataViewContentStyle();
			style.CopyFrom(CreateStyleByName("ContentStyle"));
			return style;
		}
		protected internal virtual DataViewContentStyle GetDefaultPagerPanelStyle() {
			DataViewContentStyle style = new DataViewContentStyle();
			style.CopyFrom(CreateStyleByName("PagerPanelStyle"));
			return style;
		}
		protected internal virtual DataViewItemStyle GetDefaultItemStyle() {
			DataViewItemStyle style = new DataViewItemStyle();
			style.CopyFrom(CreateStyleByName("ItemStyle"));
			style.Width = GetItemWidth();
			style.VerticalAlign = VerticalAlign.Top;
			style.HorizontalAlign = SkinOwner != null && SkinOwner.IsRightToLeft() 
				? HorizontalAlign.Right 
				: HorizontalAlign.Left;
			return style;
		}
		protected internal virtual DataViewItemStyle GetDefaultEmptyItemStyle() {
			DataViewEmptyItemStyle style = new DataViewEmptyItemStyle();
			style.CopyFrom(CreateStyleByName("EmptyItemStyle"));
			style.Width = GetEmptyItemWidth();
			return style;
		}
		protected internal virtual DataViewEmptyDataStyle GetDefaultEmptyDataStyle() {
			DataViewEmptyDataStyle style = new DataViewEmptyDataStyle();
			style.CopyFrom(CreateStyleByName("EmptyDataStyle"));
			return style;
		}
		protected internal virtual DataViewPagerShowMoreItemsContainerStyle GetDefaultPagerShowMoreItemsContainerStyle() {
			DataViewPagerShowMoreItemsContainerStyle style = new DataViewPagerShowMoreItemsContainerStyle();
			style.CopyFrom(CreateStyleByName("PagerShowMoreItemsContainerStyle"));
			return style;
		}
		protected virtual Unit GetItemSpacing() {
			return 18;
		}
		protected virtual Unit GetPagerPanelSpacing() {
			return 12;
		}
		protected virtual Unit GetItemWidth() {
			return 247;
		}
		protected virtual Unit GetEmptyItemWidth() {
			return GetItemWidth();
		}
	}
}
