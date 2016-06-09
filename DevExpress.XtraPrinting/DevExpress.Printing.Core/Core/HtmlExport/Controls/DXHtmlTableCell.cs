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
using System.Globalization;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXHtmlTableCell : DXHtmlContainerControl {
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
		public string BorderColor {
			get {
				string bordercolor = Attributes["bordercolor"];
				if(bordercolor == null)
					return string.Empty;
				return bordercolor;
			}
			set { Attributes["bordercolor"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public int ColSpan {
			get {
				string colspan = Attributes["colspan"];
				if(colspan == null)
					return -1;
				return int.Parse(colspan, CultureInfo.InvariantCulture);
			}
			set { Attributes["colspan"] = DXHtmlControl.MapIntegerAttributeToString(value); }
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
		public bool NoWrap {
			get {
				string nowrap = Attributes["nowrap"];
				if(nowrap == null)
					return false;
				return nowrap.Equals("nowrap");
			}
			set {
				if(value)
					Attributes["nowrap"] = "nowrap";
				else
					Attributes["nowrap"] = null;
			}
		}
		public int RowSpan {
			get {
				string rowspan = Attributes["rowspan"];
				if(rowspan == null)
					return -1;
				return int.Parse(rowspan, CultureInfo.InvariantCulture);
			}
			set { Attributes["rowspan"] = DXHtmlControl.MapIntegerAttributeToString(value); }
		}
		public string VAlign {
			get {
				string valign = Attributes["valign"];
				if(valign == null)
					return string.Empty;
				return valign;
			}
			set { Attributes["valign"] = DXHtmlControl.MapStringAttributeToString(value); }
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
		public DXHtmlTableCell()
			: base(DXHtmlTextWriterTag.Td) {
		}
		public DXHtmlTableCell(DXHtmlTextWriterTag tag)
			: base(tag) {
		}
		protected override void RenderEndTag(DXHtmlTextWriter writer) {
			base.RenderEndTag(writer);
			writer.WriteLine();
		}
	}
}
