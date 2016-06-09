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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class HeadlineStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStyleLineHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new virtual Unit LineHeight {
			get { return base.LineHeight; }
			set { base.LineHeight = value; }
		}
	}
	public class HeadlineContentStyle : HeadlineStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { base.HorizontalAlign = value; }
		}
	}
	public class HeadlineDateStyle : HeadlineStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit LineHeight {
			get { return base.LineHeight; }
			set { base.LineHeight = value; }
		}
	}
	public class HeadlinePanelStyle : HeadlineStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit LineHeight {
			get { return base.LineHeight; }
			set { base.LineHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlinePanelStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlinePanelStyleVerticalAlign"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class HeadlineTailStyle : HeadlineStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit LineHeight {
			get { return base.LineHeight; }
			set { base.LineHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineTailStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
	}
	public class HeadlineStyles : StylesBase {
		public const string ContentStyleName = "Content";
		public const string DateStyleName = "Date";
		public const string HeaderStyleName = "Header";
		public const string LeftPanelStyleName = "LeftPanel";
		public const string RightPanelStyleName = "RightPanel";
		public const string TailStyleName = "Tail";
		public HeadlineStyles(ASPxHeadline headline)
			: base(headline) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineContentStyle Content {
			get { return (HeadlineContentStyle)GetStyle(ContentStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesDate"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineDateStyle Date {
			get { return (HeadlineDateStyle)GetStyle(DateStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineStyle Header {
			get { return (HeadlineStyle)GetStyle(HeaderStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesLeftPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlinePanelStyle LeftPanel {
			get { return (HeadlinePanelStyle)GetStyle(LeftPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesRightPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlinePanelStyle RightPanel {
			get { return (HeadlinePanelStyle)GetStyle(RightPanelStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HeadlineStylesTail"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineTailStyle Tail {
			get { return (HeadlineTailStyle)GetStyle(TailStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxhl";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ContentStyleName, delegate() { return new HeadlineContentStyle(); } ));
			list.Add(new StyleInfo(DateStyleName, delegate() { return new HeadlineDateStyle(); } ));
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new HeadlineStyle(); } ));
			list.Add(new StyleInfo(LeftPanelStyleName, delegate() { return new HeadlinePanelStyle(); } ));
			list.Add(new StyleInfo(RightPanelStyleName, delegate() { return new HeadlinePanelStyle(); } ));
			list.Add(new StyleInfo(TailStyleName, delegate() { return new HeadlineTailStyle(); } ));
		}
		protected internal virtual HeadlineContentStyle GetDefaultContentStyle() {
			HeadlineContentStyle style = new HeadlineContentStyle();
			style.CopyFrom(CreateStyleByName("ContentStyle"));
			style.Spacing = GetContentSpacing();
			return style;
		}
		protected internal virtual HeadlineDateStyle GetDefaultDateHeaderStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(CreateStyleByName("DateHeaderStyle"));
			return style;
		}
		protected internal virtual HeadlineDateStyle GetDefaultDateLeftPanelStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(CreateStyleByName("DateLeftPanelStyle"));
			return style;
		}
		protected internal virtual HeadlineDateStyle GetDefaultDateRightPanelStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(CreateStyleByName("DateRightPanelStyle"));
			return style;
		}
		protected internal virtual HeadlineDateStyle GetDefaultDateStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(CreateStyleByName("DateStyle"));
			style.Spacing = GetDateSpacing();
			return style;
		}
		protected internal virtual HeadlineStyle GetDefaultHeaderStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(CreateStyleByName("HeaderStyle"));
			style.Spacing = GetHeaderSpacing();
			return style;
		}
		protected internal virtual HeadlinePanelStyle GetDefaultLeftPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(CreateStyleByName("LeftPanelStyle"));
			style.Spacing = GetLeftPanelSpacing();
			style.ImageSpacing = GetLeftPanelImageSpacing();
			return style;
		}
		protected internal virtual HeadlinePanelStyle GetDefaultRightPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(CreateStyleByName("RightPanelStyle"));
			style.Spacing = GetRightPanelSpacing();
			style.ImageSpacing = GetRightPanelImageSpacing();
			return style;
		}
		protected internal virtual HeadlineTailStyle GetDefaultTailDivStyle() {
			HeadlineTailStyle style = new HeadlineTailStyle();
			style.CopyFrom(CreateStyleByName("TailDivStyle"));
			style.Spacing = GetTailSpacing();
			return style;
		}
		protected internal virtual HeadlineTailStyle GetDefaultTailStyle() {
			HeadlineTailStyle style = new HeadlineTailStyle();
			style.CopyFrom(CreateStyleByName("TailStyle"));
			style.ImageSpacing = GetTailImageSpacing();
			return style;
		}
		protected virtual Unit GetDateSpacing() {
			return 4;
		}
		protected virtual Unit GetContentSpacing() {
			return 3;
		}
		protected virtual Unit GetHeaderSpacing() {
			return 4;
		}
		protected virtual Unit GetLeftPanelImageSpacing() {
			return GetImageSpacing();
		}
		protected virtual Unit GetLeftPanelSpacing() {
			return 12;
		}
		protected virtual Unit GetRightPanelImageSpacing() {
			return GetImageSpacing();
		}
		protected virtual Unit GetRightPanelSpacing() {
			return 12;
		}
		protected internal Paddings GetTailPaddings() {
			return new Paddings();
		}
		protected virtual Unit GetTailSpacing() {
			return 3;
		}
		protected virtual Unit GetTailImageSpacing() {
			return GetImageSpacing();
		}
	}
}
