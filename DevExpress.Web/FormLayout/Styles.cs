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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class EmptyLayoutItemStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
	}
	public class FormLayoutStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
	}
	public class LayoutGroupBoxCaptionStyle : AppearanceStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBoxCaptionStyleOffsetY"),
#endif
		DefaultValue(null), NotifyParentProperty(true), AutoFormatEnable]
		public int? OffsetY {
			get { return (int?)ViewStateUtils.GetObjectProperty(ReadOnlyViewState, "OffsetY", null); }
			set {
				if (value.HasValue) {
					ViewStateUtils.SetObjectProperty(ViewState, "OffsetY", null, value);
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBoxCaptionStyleOffsetX"),
#endif
		DefaultValue(null), NotifyParentProperty(true), AutoFormatEnable]
		public int? OffsetX {
			get { return (int?)ViewStateUtils.GetObjectProperty(ReadOnlyViewState, "OffsetX", null); }
			set {
				if (value.HasValue) {
					ViewStateUtils.SetObjectProperty(ViewState, "OffsetX", null, value);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
#if !SL
	[DevExpressWebLocalizedDescription("LayoutGroupBoxCaptionStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				return base.IsEmpty && !OffsetX.HasValue && !OffsetY.HasValue;
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			LayoutGroupBoxCaptionStyle captionStyle = style as LayoutGroupBoxCaptionStyle;
			if (captionStyle == null) return;
			OffsetX = captionStyle.OffsetX;
			OffsetY = captionStyle.OffsetY;
		}
		public override void Reset() {
			base.Reset();
			OffsetX = null;
			OffsetY = null;
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				LayoutGroupBoxCaptionStyle layoutGroupBoxCaptionStyle = style as LayoutGroupBoxCaptionStyle;
				if (style != null) {
					if (layoutGroupBoxCaptionStyle.OffsetX.HasValue && !OffsetX.HasValue)
						OffsetX = layoutGroupBoxCaptionStyle.OffsetX;
					if (layoutGroupBoxCaptionStyle.OffsetY.HasValue && !OffsetY.HasValue)
						OffsetY = layoutGroupBoxCaptionStyle.OffsetY;
				}
			}
		}
		public override void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment, bool exceptOpacity) {
			base.AssignToControl(control, range, exceptTextDecoration, useBlockAlignment, exceptOpacity);
			if (OffsetY.HasValue)
				control.Style[HtmlTextWriterStyle.Top] = string.Format("{0}px", -OffsetY.Value);
			if (OffsetX.HasValue)
				control.Style[HtmlTextWriterStyle.Left] = string.Format("{0}px", OffsetX.Value);
		}
	}
	public class LayoutGroupCellStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
	}
	public class LayoutGroupStyle : AppearanceStyleBase {
		private LayoutGroupCellStyle cellStyle = new LayoutGroupCellStyle();
		private AppearanceStyleBase groupTableStyle = new AppearanceStyleBase();
		protected internal AppearanceStyleBase GroupTable {
			get { return groupTableStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupStyleCell"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupCellStyle Cell {
			get { return this.cellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
#if !SL
	[DevExpressWebLocalizedDescription("LayoutGroupStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				return base.IsEmpty && Cell.IsEmpty && GroupTable.IsEmpty;
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			LayoutGroupStyle groupStyle = style as LayoutGroupStyle;
			if (groupStyle == null) return;
			Cell.CopyFrom(groupStyle.Cell);
			GroupTable.CopyFrom(groupStyle.GroupTable);
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				LayoutGroupStyle layoutGroupStyle = style as LayoutGroupStyle;
				if (style != null) {
					Cell.MergeWith(layoutGroupStyle.Cell);
					GroupTable.MergeWith(layoutGroupStyle.GroupTable);
				}
			}
		}
		public override void Reset() {
			base.Reset();
			Cell.Reset();
			GroupTable.Reset();
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if (!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((LayoutGroupStyle)style).GetObject(ref ((LayoutGroupStyle)style).cellStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutGroupStyle)style).GetObject(ref ((LayoutGroupStyle)style).groupTableStyle, create); });
#pragma warning restore 197
				state = list.ToArray();
				lock (stateDelegates)
					stateDelegates[GetType()] = state;
			}
			return state;
		}
	}
	public class LayoutGroupBoxStyle : AppearanceStyleBase {
		private LayoutGroupBoxCaptionStyle captionStyle = new LayoutGroupBoxCaptionStyle();
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupBoxStyleCaption"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupBoxCaptionStyle Caption {
			get { return this.captionStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
#if !SL
	[DevExpressWebLocalizedDescription("LayoutGroupBoxStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				return base.IsEmpty && Caption.IsEmpty;
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			LayoutGroupBoxStyle groupBoxStyle = style as LayoutGroupBoxStyle;
			if (groupBoxStyle != null) {
				Caption.CopyFrom(groupBoxStyle.Caption);
			}
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				LayoutGroupBoxStyle layoutGroupBoxStyle = style as LayoutGroupBoxStyle;
				if (style != null) {
					Caption.MergeWith(layoutGroupBoxStyle.Caption);
				}
			}
		}
		public override void Reset() {
			base.Reset();
			Caption.Reset();
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if (!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((LayoutGroupBoxStyle)style).GetObject(ref ((LayoutGroupBoxStyle)style).captionStyle, create); });
#pragma warning restore 197
				state = list.ToArray();
				lock (stateDelegates)
					stateDelegates[GetType()] = state;
			}
			return state;
		}
	}
	public class LayoutItemCaptionStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
	}
	public class LayoutItemCellStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
	}
	public class LayoutItemStyle : AppearanceStyle {
		private LayoutItemCaptionStyle captionStyle = new LayoutItemCaptionStyle();
		private LayoutItemCellStyle nestedControlCellStyle = new LayoutItemCellStyle();
		private LayoutItemCellStyle captionCellStyle = new LayoutItemCellStyle();
		private HelpTextStyle helpTextStyle = new HelpTextStyle();
		private AppearanceStyleBase internalNestedControlTableStyle = new AppearanceStyle();
		private AppearanceStyleBase itemTableStyle = new AppearanceStyleBase();
		protected internal AppearanceStyleBase InternalNestedControlTable {
			get { return this.internalNestedControlTableStyle; }
		}
		protected internal AppearanceStyleBase ItemTable {
			get { return this.itemTableStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemStyleHelpText"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HelpTextStyle HelpText {
			get { return this.helpTextStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemStyleCaption"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemCaptionStyle Caption {
			get { return this.captionStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemStyleNestedControlCell"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemCellStyle NestedControlCell {
			get { return this.nestedControlCellStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemStyleCaptionCell"),
#endif
		AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemCellStyle CaptionCell {
			get { return this.captionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new virtual Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		protected override DisabledStyle CreateDisabledStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreatePressedStyle() {
			return null;
		}
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return null;
		}
#if !SL
	[DevExpressWebLocalizedDescription("LayoutItemStyleIsEmpty")]
#endif
		public override bool IsEmpty {
			get {
				return base.IsEmpty && ItemTable.IsEmpty && Caption.IsEmpty && CaptionCell.IsEmpty && NestedControlCell.IsEmpty
					&& InternalNestedControlTable.IsEmpty && HelpText.IsEmpty;
			}
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			LayoutItemStyle itemStyle = style as LayoutItemStyle;
			if (itemStyle == null) return;
			ItemTable.CopyFrom(itemStyle.ItemTable);
			Caption.CopyFrom(itemStyle.Caption);
			CaptionCell.CopyFrom(itemStyle.CaptionCell);
			NestedControlCell.CopyFrom(itemStyle.NestedControlCell);
			InternalNestedControlTable.CopyFrom(itemStyle.InternalNestedControlTable);
			HelpText.CopyFrom(itemStyle.HelpText);
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			if ((style != null) && !style.IsEmpty) {
				base.MergeWith(style);
				LayoutItemStyle layoutItemStyle = style as LayoutItemStyle;
				if (style != null) {
					ItemTable.MergeWith(layoutItemStyle.ItemTable);
					Caption.MergeWith(layoutItemStyle.Caption);
					CaptionCell.MergeWith(layoutItemStyle.CaptionCell);
					NestedControlCell.MergeWith(layoutItemStyle.NestedControlCell);
					InternalNestedControlTable.MergeWith(layoutItemStyle.InternalNestedControlTable);
					HelpText.MergeWith(layoutItemStyle.HelpText);
				}
			}
		}
		public override void Reset() {
			base.Reset();
			ItemTable.Reset();
			Caption.Reset();
			CaptionCell.Reset();
			NestedControlCell.Reset();
			InternalNestedControlTable.Reset();
			HelpText.Reset();
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if (!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).itemTableStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).captionCellStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).captionStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).nestedControlCellStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).internalNestedControlTableStyle, create); });
				list.Add(delegate(object style, bool create) { return ((LayoutItemStyle)style).GetObject(ref ((LayoutItemStyle)style).helpTextStyle, create); });
#pragma warning restore 197
				state = list.ToArray();
				lock (stateDelegates)
					stateDelegates[GetType()] = state;
			}
			return state;
		}
	}
	public class FormLayoutStyles : StylesBase {
		const string cssClassNamePrefix = "dxfl";
		const string
			EmptyLayoutItemStyleName = "EmptyLayoutItemStyle",
			LayoutGroupBoxStyleName = "LayoutGroupBoxStyle",
			LayoutItemStyleName = "LayoutItemStyle",
			LayoutGroupStyleName = "LayoutGroupStyle",
			StyleStyleName = "Style";
		protected internal const string
			HeadingLineGroupBoxSystemClassName = cssClassNamePrefix + "HeadingLineGroupBoxSys",
			GroupBoxSystemClassName = cssClassNamePrefix + "GroupBoxSys",
			ItemSystemClassName = cssClassNamePrefix + "ItemSys",
			CommandItemSystemClassName = cssClassNamePrefix + "CommandItemSys",
			SafariItemSystemClassName = cssClassNamePrefix + "SafariItemSys",
			ItemTypeSystemClassNameFormat = cssClassNamePrefix + "{0}ItemSys",
			LayoutItemCaptionCellSystemClassName = cssClassNamePrefix + "CaptionCellSys",
			FormLayoutHALeftSystemClassName = cssClassNamePrefix + "HALSys",
			FormLayoutHARightSystemClassName = cssClassNamePrefix + "HARSys",
			FormLayoutHACenterSystemClassName = cssClassNamePrefix + "HACSys",
			FormLayoutVABottomSystemClassName = cssClassNamePrefix + "VABSys",
			FormLayoutVAMiddleSystemClassName = cssClassNamePrefix + "VAMSys",
			FormLayoutVATopSystemClassName = cssClassNamePrefix + "VATSys",
			FormLayoutCaptionLocationTopSystemClassName = cssClassNamePrefix + "CLTSys",
			FormLayoutCaptionLocationBottomSystemClassName = cssClassNamePrefix + "CLBSys",
			FormLayoutCaptionLocationLeftSystemClassName = cssClassNamePrefix + "CLLSys",
			FormLayoutCaptionLocationRightSystemClassName = cssClassNamePrefix + "CLRSys",
			FormLayoutLeftHelpTextSystemClassName = cssClassNamePrefix + "LHelpTextSys",
			FormLayoutRightHelpTextSystemClassName = cssClassNamePrefix + "RHelpTextSys",
			FormLayoutTopHelpTextSystemClassName = cssClassNamePrefix + "THelpTextSys",
			FormLayoutBottomHelpTextSystemClassName = cssClassNamePrefix + "BHelpTextSys",
			LayoutGroupSystemClassName = cssClassNamePrefix + "GroupSys",
			AlignedLayoutGroupSystemClassName = cssClassNamePrefix + "AGSys",
			TabbedGroupPageControlSystemClassName = cssClassNamePrefix + "PCSys",
			ElementsContainerSystemClassName = cssClassNamePrefix + "ElConSys",
			FloatedElementsContainerSystemClassName = cssClassNamePrefix + "FloatedElConSys",
			NotFloatedElementSystemClassName = cssClassNamePrefix + "NotFloatedElSys",
			GroupChildElementInFirstRowSystemClassName = cssClassNamePrefix + "ChildInFirstRowSys",
			GroupFirstChildElementSystemClassName = cssClassNamePrefix + "FirstChildSys",
			GroupChildElementInLastRowSystemClassName = cssClassNamePrefix + "ChildInLastRowSys",
			GroupLastChildElementSystemClassName = cssClassNamePrefix + "LastChildSys",
			GroupLastChildElementInRowSystemClassName = cssClassNamePrefix + "LastChildInRowSys",
			NoDefaultPaddingsSystemClassName = cssClassNamePrefix + "NoDefaultPaddings",
			ViewFormLayoutSystemClassName = cssClassNamePrefix + "ViewFormLayoutSys";
		public FormLayoutStyles(ISkinOwner formLayout)
			: base(formLayout) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return cssClassNamePrefix;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FormLayoutStyle Style {
			get { return (FormLayoutStyle)GetStyle(StyleStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutStylesDisabled"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle Disabled {
			get { return base.DisabledInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutStylesLayoutGroupBox"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupBoxStyle LayoutGroupBox
		{
			get { return (LayoutGroupBoxStyle)GetStyle(LayoutGroupBoxStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutStylesLayoutItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemStyle LayoutItem
		{
			get { return (LayoutItemStyle)GetStyle(LayoutItemStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutStylesLayoutGroup"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupStyle LayoutGroup
		{
			get { return (LayoutGroupStyle)GetStyle(LayoutGroupStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutStylesEmptyLayoutItem"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EmptyLayoutItemStyle EmptyLayoutItem
		{
			get { return (EmptyLayoutItemStyle)GetStyle(EmptyLayoutItemStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(StyleStyleName, delegate() { return new FormLayoutStyle(); }));
			list.Add(new StyleInfo(LayoutGroupBoxStyleName, delegate() { return new LayoutGroupBoxStyle(); }));
			list.Add(new StyleInfo(LayoutItemStyleName, delegate() { return new LayoutItemStyle(); }));
			list.Add(new StyleInfo(EmptyLayoutItemStyleName, delegate() { return new EmptyLayoutItemStyle(); }));
			list.Add(new StyleInfo(LayoutGroupStyleName, delegate() { return new LayoutGroupStyle(); }));
		}
		protected AppearanceStyle GetDefaultStyle(string styleName) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(styleName));
			return style;
		}
		protected internal AppearanceStyle GetDefaultFormLayoutStyle() {
			return GetDefaultStyle("FormLayout");
		}
		protected internal LayoutGroupBoxStyle GetDefaultLayoutGroupBoxStyle() {
			LayoutGroupBoxStyle result = new LayoutGroupBoxStyle();
			result.CopyFrom(GetDefaultStyle("GroupBox"));
			result.Caption.CopyFrom(GetDefaultStyle("GroupBoxCaption"));
			return result;
		}
		protected internal LayoutItemStyle GetDefaultLayoutItemStyle() {
			LayoutItemStyle result = new LayoutItemStyle();
			result.CopyFrom(GetDefaultStyle("Item"));
			result.ItemTable.CopyFrom(GetDefaultStyle("ItemTable"));
			result.CaptionCell.CopyFrom(GetDefaultStyle("CaptionCell"));
			result.CaptionCell.CssClass = RenderUtils.CombineCssClasses(result.CaptionCell.CssClass, FormLayoutStyles.LayoutItemCaptionCellSystemClassName);
			result.NestedControlCell.CopyFrom(GetDefaultStyle("NestedControlCell"));
			result.Caption.CopyFrom(GetDefaultStyle("Caption"));
			result.InternalNestedControlTable.CopyFrom(GetDefaultStyle("InternalEditorTable"));
			result.HelpText.CopyFrom(GetDefaultStyle("HelpText"));
			return result;
		}
		protected internal AppearanceStyle GetDefaultLayoutItemRequiredMarkStyle() {
			AppearanceStyle result = new AppearanceStyle();
			result.CopyFrom(GetDefaultStyle("Required"));
			return result;
		}
		protected internal AppearanceStyle GetDefaultLayoutItemOptionalMarkStyle() {
			AppearanceStyle result = new AppearanceStyle();
			result.CopyFrom(GetDefaultStyle("Optional"));
			return result;
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			return GetDefaultStyle("FormLayout");
		}
		protected internal LayoutGroupStyle GetDefaultLayoutGroupStyle() {
			LayoutGroupStyle result = new LayoutGroupStyle();
			result.CopyFrom(GetDefaultStyle("Group"));
			result.Cell.CopyFrom(GetDefaultStyle("GroupCell"));
			result.GroupTable.CopyFrom(GetDefaultStyle("GroupTable"));
			return result;
		}
		protected internal AppearanceStyle GetDefaultEmptyLayoutItemStyle() {
			return GetDefaultStyle("EmptyItem");
		}
		internal static string GetHorizontalAlignSystemClassName(FormLayoutHorizontalAlign align) {
			switch (align) {
				case FormLayoutHorizontalAlign.Left: return FormLayoutHALeftSystemClassName;
				case FormLayoutHorizontalAlign.Right: return FormLayoutHARightSystemClassName;
				case FormLayoutHorizontalAlign.Center: return FormLayoutHACenterSystemClassName;
			}
			return string.Empty;
		}
		internal static string GetVerticalAlignSystemClassName(FormLayoutVerticalAlign align) {
			switch (align) {
				case FormLayoutVerticalAlign.Bottom: return FormLayoutVABottomSystemClassName;
				case FormLayoutVerticalAlign.Middle: return FormLayoutVAMiddleSystemClassName;
				case FormLayoutVerticalAlign.Top: return FormLayoutVATopSystemClassName;
			}
			return string.Empty;
		}
		internal static string GetHorizontalAlignSystemClassName(HelpTextHorizontalAlign align) {
			switch (align) {
				case HelpTextHorizontalAlign.Left: return FormLayoutHALeftSystemClassName;
				case HelpTextHorizontalAlign.Right: return FormLayoutHARightSystemClassName;
				case HelpTextHorizontalAlign.Center: return FormLayoutHACenterSystemClassName;
			}
			return string.Empty;
		}
		internal static string GetVerticalAlignSystemClassName(HelpTextVerticalAlign align) {
			switch (align) {
				case HelpTextVerticalAlign.Bottom: return FormLayoutVABottomSystemClassName;
				case HelpTextVerticalAlign.Middle: return FormLayoutVAMiddleSystemClassName;
				case HelpTextVerticalAlign.Top: return FormLayoutVATopSystemClassName;
			}
			return string.Empty;
		}
		internal static string GetCaptionLocationSystemClassName(LayoutItemCaptionLocation captionLocation) {
			switch (captionLocation) {
				case LayoutItemCaptionLocation.Top: return FormLayoutCaptionLocationTopSystemClassName;
				case LayoutItemCaptionLocation.Bottom: return FormLayoutCaptionLocationBottomSystemClassName;
				case LayoutItemCaptionLocation.Left: return FormLayoutCaptionLocationLeftSystemClassName;
				case LayoutItemCaptionLocation.Right: return FormLayoutCaptionLocationRightSystemClassName;
			}
			return string.Empty;
		}
		internal static string[] GetLayoutItemSystemClassNames(LayoutItemCaptionLocation captionLocation) {
			List<string> result = new List<string>();
			result.Add(GetCaptionLocationSystemClassName(captionLocation));
			result.Add(ItemSystemClassName);
			if (RenderUtils.Browser.IsSafari)
				result.Add(SafariItemSystemClassName);
			return result.ToArray();
		}
		internal static string[] GetAlignmentClassNames(FormLayoutHorizontalAlign hAlign, FormLayoutVerticalAlign vAlign) {
			return new string[] {
				GetHorizontalAlignSystemClassName(hAlign),
				GetVerticalAlignSystemClassName(vAlign),
			};
		}
		internal static string[] GetAlignmentClassNames(HelpTextHorizontalAlign hAlign, HelpTextVerticalAlign vAlign) {
			return new string[] {
				GetHorizontalAlignSystemClassName(hAlign),
				GetVerticalAlignSystemClassName(vAlign),
			};
		}
		internal static string GetHelpTextClassName(HelpTextPosition helpTextPosition) {
			if (helpTextPosition == HelpTextPosition.Left)
				return FormLayoutLeftHelpTextSystemClassName;
			if(helpTextPosition == HelpTextPosition.Right)
				return FormLayoutRightHelpTextSystemClassName;
			if (helpTextPosition == HelpTextPosition.Top)
				return FormLayoutTopHelpTextSystemClassName;
			if (helpTextPosition == HelpTextPosition.Bottom)
				return FormLayoutBottomHelpTextSystemClassName;
			return string.Empty;
		}
	}
}
