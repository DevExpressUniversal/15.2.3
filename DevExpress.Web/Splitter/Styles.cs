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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class SplitterStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class SplitterPaneStyle : SplitterStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class SplitterSimpleStyle : SplitterStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
	}
	public class SplitterPaneCollapsedStyle : SplitterSimpleStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
	}
	public class SplitterSeparatorStyleBase : SplitterSimpleStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class SplitterSeparatorStyle : SplitterSeparatorStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorStyleHoverStyle"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new SplitterSeparatorSelectedStyle();
		}
	}
	public class SplitterSeparatorButtonStyle : SplitterSimpleStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorButtonStyleHoverStyle"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new SplitterSeparatorSelectedStyleBase();
		}
	}
	public class SplitterSeparatorSelectedStyleBase : AppearanceSelectedStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
	}
	public class SplitterSeparatorSelectedStyle : SplitterSeparatorSelectedStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class SplitterResizingPointerStyle : SplitterSimpleStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
	}
	public class SplitterStyles : StylesBase {
		public const string PaneStyleName = "Pane";
		public const string PaneCollapsedStyleName = "PaneCollapsed";
		public const string SeparatorStyleName = "Separator";
		public const string SeparatorHoverStyleName = "SeparatorHover";
		public const string SeparatorCollapsedStyleName = "SeparatorCollapsed";
		public const string SeparatorButtonStyleName = "SeparatorButton";
		public const string SeparatorButtonHoverStyleName = "SeparatorButtonHover";
		public const string VerticalSeparatorStyleName = "V" + SeparatorStyleName;
		public const string VerticalSeparatorHoverStyleName = "V" + SeparatorHoverStyleName;
		public const string VerticalSeparatorCollapsedStyleName = "V" + SeparatorCollapsedStyleName;
		public const string VerticalSeparatorButtonStyleName = "V" + SeparatorButtonStyleName;
		public const string VerticalSeparatorButtonHoverStyleName = "V" + SeparatorButtonHoverStyleName;
		public const string HorizontalSeparatorStyleName = "H" + SeparatorStyleName;
		public const string HorizontalSeparatorHoverStyleName = "H" + SeparatorHoverStyleName;
		public const string HorizontalSeparatorCollapsedStyleName = "H" + SeparatorCollapsedStyleName;
		public const string HorizontalSeparatorButtonStyleName = "H" + SeparatorButtonStyleName;
		public const string HorizontalSeparatorButtonHoverStyleName = "H" + SeparatorButtonHoverStyleName;
		public const string ResizingPointerStyleName = "ResizingPointer";
		public SplitterStyles(ISkinOwner splitter)
			: base(splitter) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxspl";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesPane"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterPaneStyle Pane { get { return (SplitterPaneStyle)GetStyle(PaneStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesPaneCollapsed"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterPaneCollapsedStyle PaneCollapsed { get { return (SplitterPaneCollapsedStyle)GetStyle(PaneCollapsedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesSeparator"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyle Separator { get { return (SplitterSeparatorStyle)GetStyle(SeparatorStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesSeparatorCollapsed"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyleBase SeparatorCollapsed { get { return (SplitterSeparatorStyleBase)GetStyle(SeparatorCollapsedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesSeparatorButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorButtonStyle SeparatorButton { get { return (SplitterSeparatorButtonStyle)GetStyle(SeparatorButtonStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesVerticalSeparator"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyle VerticalSeparator { get { return (SplitterSeparatorStyle)GetStyle(VerticalSeparatorStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesVerticalSeparatorCollapsed"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyleBase VerticalSeparatorCollapsed { get { return (SplitterSeparatorStyleBase)GetStyle(VerticalSeparatorCollapsedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesVerticalSeparatorButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorButtonStyle VerticalSeparatorButton { get { return (SplitterSeparatorButtonStyle)GetStyle(VerticalSeparatorButtonStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesHorizontalSeparator"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyle HorizontalSeparator { get { return (SplitterSeparatorStyle)GetStyle(HorizontalSeparatorStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesHorizontalSeparatorCollapsed"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyleBase HorizontalSeparatorCollapsed { get { return (SplitterSeparatorStyleBase)GetStyle(HorizontalSeparatorCollapsedStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesHorizontalSeparatorButton"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorButtonStyle HorizontalSeparatorButton { get { return (SplitterSeparatorButtonStyle)GetStyle(HorizontalSeparatorButtonStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterStylesResizingPointer"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterResizingPointerStyle ResizingPointer { get { return (SplitterResizingPointerStyle)GetStyle(ResizingPointerStyleName); } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(PaneStyleName, delegate() { return new SplitterPaneStyle(); }));
			list.Add(new StyleInfo(PaneCollapsedStyleName, delegate() { return new SplitterPaneCollapsedStyle(); }));
			list.Add(new StyleInfo(SeparatorStyleName, delegate() { return new SplitterSeparatorStyle(); }));
			list.Add(new StyleInfo(SeparatorCollapsedStyleName, delegate() { return new SplitterSeparatorStyleBase(); }));
			list.Add(new StyleInfo(SeparatorButtonStyleName, delegate() { return new SplitterSeparatorButtonStyle(); }));
			list.Add(new StyleInfo(VerticalSeparatorStyleName, delegate() { return new SplitterSeparatorStyle(); }));
			list.Add(new StyleInfo(VerticalSeparatorCollapsedStyleName, delegate() { return new SplitterSeparatorStyleBase(); }));
			list.Add(new StyleInfo(VerticalSeparatorButtonStyleName, delegate() { return new SplitterSeparatorButtonStyle(); }));
			list.Add(new StyleInfo(HorizontalSeparatorStyleName, delegate() { return new SplitterSeparatorStyle(); }));
			list.Add(new StyleInfo(HorizontalSeparatorCollapsedStyleName, delegate() { return new SplitterSeparatorStyleBase(); }));
			list.Add(new StyleInfo(HorizontalSeparatorButtonStyleName, delegate() { return new SplitterSeparatorButtonStyle(); }));
			list.Add(new StyleInfo(ResizingPointerStyleName, delegate() { return new SplitterResizingPointerStyle(); }));
		}
		protected internal SplitterPaneStyle GetDefaultPaneStyle() {
			return CreateStyleCopyByName<SplitterPaneStyle>(PaneStyleName);
		}
		protected internal SplitterPaneCollapsedStyle GetDefaultPaneCollapsedStyle() {
			return CreateStyleCopyByName<SplitterPaneCollapsedStyle>(PaneCollapsedStyleName);
		}
		protected internal SplitterSeparatorStyle GetDefaultSeparatorStyle(bool isVertical) {
			string name = isVertical ? VerticalSeparatorStyleName : HorizontalSeparatorStyleName;
			return CreateStyleCopyByName<SplitterSeparatorStyle>(name);
		}
		protected internal SplitterSeparatorStyle GetDefaultSeparatorHoverStyle(bool isVertical) {
			string name = isVertical ? VerticalSeparatorHoverStyleName : HorizontalSeparatorHoverStyleName;
			return CreateStyleCopyByName<SplitterSeparatorStyle>(name);
		}
		protected internal SplitterSeparatorStyleBase GetDefaultSeparatorCollapsedStyle(bool isVertical) {
			string name = isVertical ? VerticalSeparatorCollapsedStyleName : HorizontalSeparatorCollapsedStyleName;
			return CreateStyleCopyByName<SplitterSeparatorStyleBase>(name);
		}
		protected internal SplitterSeparatorButtonStyle GetDefaultSeparatorButtonStyle(bool isVertical) {
			string name = isVertical ? VerticalSeparatorButtonStyleName : HorizontalSeparatorButtonStyleName;
			return CreateStyleCopyByName<SplitterSeparatorButtonStyle>(name);
		}
		protected internal AppearanceSelectedStyle GetDefaultSeparatorButtonHoverStyle(bool isVertical) {
			string name = isVertical ? VerticalSeparatorButtonHoverStyleName : HorizontalSeparatorButtonHoverStyleName;
			return CreateStyleCopyByName<AppearanceSelectedStyle>(name);
		}
		protected internal SplitterResizingPointerStyle GetDefaultResizingPointerStyle() {
			return CreateStyleCopyByName<SplitterResizingPointerStyle>(ResizingPointerStyleName);
		}
	}
}
