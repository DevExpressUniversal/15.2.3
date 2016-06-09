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
using System.Text;
using DevExpress.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListExportAppearance : AppearanceStyle {
		#region Hidden styles
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new string CssClass { get { return base.CssClass; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing { get { return base.Spacing; } set { } }
		#endregion  
		[
		NotifyParentProperty(true), DefaultValue(1)]
		public new int BorderWidth {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderWidth", 1); }
			set {
				if(value < 0) value = 0;
				ViewStateUtils.SetIntProperty(ViewState, "BorderWidth", 1, value);
			}
		}
		[
		NotifyParentProperty(true)]
		public new Color BorderColor {
			get { return Border.BorderColor; }
			set { Border.BorderColor = value; }
		}
		[
		NotifyParentProperty(true), DefaultValue(BorderSide.All)]
		public BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetEnumProperty(ViewState, "BorderSides", BorderSide.All); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "BorderSides", BorderSide.All, value); }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			TreeListExportAppearance exportStyle = style as TreeListExportAppearance;
			if(exportStyle == null) return;
			BorderWidth = exportStyle.BorderWidth;
			BorderSides = exportStyle.BorderSides;
			if(!exportStyle.BorderColor.Equals(Color.Empty)) 
				BorderColor = exportStyle.BorderColor;
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			TreeListExportAppearance exportStyle = style as TreeListExportAppearance;
			if(exportStyle == null) return;
			BorderWidth = exportStyle.BorderWidth;
			BorderSides = exportStyle.BorderSides;
			if(BorderColor.Equals(Color.Empty))
				BorderColor = exportStyle.BorderColor;
		}
	}
	public class TreeListExportStyles : PropertiesBase {
		TreeListExportAppearance _default, header, cell, indent, preview, footer, groupFooter, hyperLink, image;
		public TreeListExportStyles(IPropertiesOwner owner)
			: base(owner) {
			this._default = new TreeListExportAppearance();
			this.header = new TreeListExportAppearance();
			this.cell = new TreeListExportAppearance();
			this.indent = new TreeListExportAppearance();
			this.preview = new TreeListExportAppearance();
			this.footer = new TreeListExportAppearance();
			this.groupFooter = new TreeListExportAppearance();
			this.hyperLink = new TreeListExportAppearance();
			this.image = new TreeListExportAppearance();
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Default { get { return _default; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Header { get { return header; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Cell { get { return cell; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Indent { get { return indent; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Preview { get { return preview; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Footer { get { return footer; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance GroupFooter { get { return groupFooter; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance HyperLink { get { return hyperLink; } }
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TreeListExportAppearance Image { get { return image; } }
		public void CopyFrom(TreeListExportStyles styles) {
			Default.CopyFrom(styles.Default);
			Cell.CopyFrom(styles.Cell);
			Header.CopyFrom(styles.Header);
			Indent.CopyFrom(styles.Indent);
			Preview.CopyFrom(styles.Preview);
			Footer.CopyFrom(styles.Footer);
			GroupFooter.CopyFrom(styles.GroupFooter);
			HyperLink.CopyFrom(styles.HyperLink);
			Image.CopyFrom(styles.Image);
		}
		public void MergeWithDefault() {
			Cell.MergeWith(Default);
			Header.MergeWith(Default);
			Indent.MergeWith(Default);
			Preview.MergeWith(Default);
			Footer.MergeWith(Default);
			GroupFooter.MergeWith(Default);
			HyperLink.MergeWith(Default);
			Image.MergeWith(Default);
		}
	}
	public class TreeListExportSystemStyles : TreeListExportStyles {
		public TreeListExportSystemStyles(IPropertiesOwner owner)
			: base(owner) {
			Default.BackColor = Color.White;
			Default.ForeColor = Color.Black;
			Default.BorderColor = Color.DarkGray;
			Default.Paddings.PaddingLeft = Default.Paddings.PaddingRight = Unit.Pixel(2);
			Default.Paddings.PaddingTop = Default.Paddings.PaddingBottom = Unit.Pixel(1);
			Header.BackColor = Color.FromArgb(190, 190, 190); 
			Indent.BackColor = Color.Transparent;
			Preview.BackColor = Color.FromArgb(242, 242, 242);
			Preview.ForeColor = Color.FromArgb(127, 127, 127);
			Footer.BackColor = Color.FromArgb(190, 190, 190);
			GroupFooter.BackColor = Color.FromArgb(224, 224, 224);
			HyperLink.ForeColor = Color.Blue;
			HyperLink.Font.Underline = true;
		}
	}
}
