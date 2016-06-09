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
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Collections;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TitleIndexColumn : CollectionItem {
		private ColumnStyle fStyle = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBackgroundImage"),
#endif
		Category("Appearance"), NotifyParentProperty(true), DefaultValue(""),
		 PersistenceMode(PersistenceMode.InnerProperty)]
		public BackgroundImage BackgroundImage {
			get { return Style.BackgroundImage; }
			set { Style.BackgroundImage.CopyFrom(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBackColor"),
#endif
		Category("Appearance"), DefaultValue(typeof(Color), ""),
		NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter))]
		public Color BackColor {
			get { return Style.BackColor; }
			set { Style.BackColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBorder"),
#endif
		Category("Appearance"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public BorderWrapper Border {
			get { return Style.Border; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBorderBottom"),
#endif
		Category("Appearance"), DefaultValue(typeof(BorderBottom), ""), NotifyParentProperty(true),
		 PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderBottom {
			get { return Style.BorderBottom; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBorderLeft"),
#endif
		Category("Appearance"), DefaultValue(typeof(BorderLeft), ""), NotifyParentProperty(true)]
		public Border BorderLeft {
			get { return Style.BorderLeft; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBorderRight"),
#endif
		Category("Appearance"), DefaultValue(typeof(BorderRight), ""), NotifyParentProperty(true),
		 PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderRight {
			get { return Style.BorderRight; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnBorderTop"),
#endif
		Category("Appearance"), DefaultValue(typeof(BorderTop), ""), NotifyParentProperty(true),
		 PersistenceMode(PersistenceMode.InnerProperty)]
		public Border BorderTop {
			get { return Style.BorderTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnCssClass"),
#endif
		Category("Appearance"), DefaultValue(""), NotifyParentProperty(true)]
		public string CssClass {
			get { return Style.CssClass; }
			set { Style.CssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), "")]
		public Unit Width {
			get { return Style.Width; }
			set { Style.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnPaddings"),
#endif
		Category("Layout"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public Paddings Paddings {
			get { return Style.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexColumnHoverStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public ColumnHoverStyle HoverStyle {
			get { return Style.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColumnStyle Style {
			get {
				if (fStyle == null)
					fStyle = new ColumnStyle();
				return fStyle;
			}
		}
		public TitleIndexColumn()
			: base() {
		}
		public override void Assign(CollectionItem source) {
			if (source is TitleIndexColumn) {
				TitleIndexColumn src = source as TitleIndexColumn;
				Style.Assign(src.Style);
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Style };
		}
	}
	public class TitleIndexColumnCollection : Collection<TitleIndexColumn> {
		public TitleIndexColumnCollection()
			: base() {
		}
		public TitleIndexColumnCollection(ASPxWebControl owner)
			: base(owner) {
		}
		public virtual TitleIndexColumn Add() {
			return AddInternal(new TitleIndexColumn());
		}
		protected override void OnChanged() {
			if(Owner as ASPxTitleIndex != null)
				(Owner as ASPxTitleIndex).ColumnChanged();
		}
	}
}
