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
using System.Globalization;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXHtmlTable : DXHtmlContainerControl {
		DXHtmlTableRowCollection rows;
		public string Align {
			get {
				string align = Attributes["align"];
				if(align == null)
					return string.Empty;
				return align;
			}
			set { Attributes["align"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string BgColor {
			get {
				string bgcolor = Attributes["bgcolor"];
				if(bgcolor == null)
					return string.Empty;
				return bgcolor;
			}
			set { Attributes["bgcolor"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public int Border {
			get {
				string border = Attributes["border"];
				if(border == null)
					return -1;
				return int.Parse(border, CultureInfo.InvariantCulture);
			}
			set { Attributes["border"] = DXHtmlControl.MapIntegerAttributeToString(value); }
		}
		public string BorderColor {
			get {
				string bordercolor = Attributes["bordercolor"];
				if(bordercolor == null)
					return string.Empty;
				return bordercolor;
			}
			set { Attributes["bordercolor"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public int CellPadding {
			get {
				string cellpadding = Attributes["cellpadding"];
				if(cellpadding == null)
					return -1;
				return int.Parse(cellpadding, CultureInfo.InvariantCulture);
			}
			set { Attributes["cellpadding"] = DXHtmlControl.MapIntegerAttributeToString(value); }
		}
		public int CellSpacing {
			get {
				string cellspacing = Attributes["cellspacing"];
				if(cellspacing == null)
					return -1;
				return int.Parse(cellspacing, CultureInfo.InvariantCulture);
			}
			set { Attributes["cellspacing"] = DXHtmlControl.MapIntegerAttributeToString(value); }
		}
		public string HeightStr {
			get {
				string height = Attributes["height"];
				if(height == null)
					return string.Empty;
				return height;
			}
			set { Attributes["height"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public override string InnerHtml {
			get {
				throw new NotSupportedException("InnerHtml_not_supported");
			}
			set {
				throw new NotSupportedException("InnerHtml_not_supported");
			}
		}
		public override string InnerText {
			get {
				throw new NotSupportedException("InnerText_not_supported");
			}
			set {
				throw new NotSupportedException("InnerText_not_supported");
			}
		}
		public virtual DXHtmlTableRowCollection Rows {
			get {
				if(rows == null)
					rows = new DXHtmlTableRowCollection(this);
				return rows;
			}
		}
		public string WidthStr {
			get {
				string width = Attributes["width"];
				if(width == null)
					return string.Empty;
				return width;
			}
			set { Attributes["width"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public DXHtmlTable()
			: base(DXHtmlTextWriterTag.Table) {
		}
		protected override DXWebControlCollection CreateControlCollection() {
			return new DXHtmlTableRowControlCollection(this);
		}
		protected class DXHtmlTableRowControlCollection : DXWebControlCollection {
			internal DXHtmlTableRowControlCollection(DXWebControlBase owner)
				: base(owner) {
			}
			bool IsValidChild(DXWebControlBase child) {
				if (child is DXHtmlTableRow)
					return true;
				DXHtmlControl control = child as DXHtmlControl;
				if (control == null)
					return false;
				if (control.TagKey == DXHtmlTextWriterTag.Colgroup || control.TagKey == DXHtmlTextWriterTag.Col)
					return true;
				return false;
			}
			public override void Add(DXWebControlBase child) {
				if (!IsValidChild(child))
					throw new ArgumentException("Cannot_Have_Children_Of_Type");
				base.Add(child);
			}
			public override void AddAt(int index, DXWebControlBase child) {
				if (!IsValidChild(child))
					throw new ArgumentException("Cannot_Have_Children_Of_Type");
				base.AddAt(index, child);
			}
		}
	}
}
