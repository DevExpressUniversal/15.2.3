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
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXHtmlTableRow : DXHtmlContainerControl {
		DXHtmlTableCellCollection cells;
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
		public virtual DXHtmlTableCellCollection Cells {
			get {
				if(cells == null)
					cells = new DXHtmlTableCellCollection(this);
				return cells;
			}
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
		public string VAlign {
			get {
				string valign = Attributes["valign"];
				if(valign == null)
					return string.Empty;
				return valign;
			}
			set { Attributes["valign"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public DXHtmlTableRow()
			: base(DXHtmlTextWriterTag.Tr) {
		}
		protected override DXWebControlCollection CreateControlCollection() {
			return new DXHtmlTableCellControlCollection(this);
		}
		protected class DXHtmlTableCellControlCollection : DXWebControlCollection {
			internal DXHtmlTableCellControlCollection(DXWebControlBase owner)
				: base(owner) {
			}
			public override void Add(DXWebControlBase child) {
				if(!(child is DXHtmlTableCell))
					throw new ArgumentException("Cannot_Have_Children_Of_Type");
				base.Add(child);
			}
			public override void AddAt(int index, DXWebControlBase child) {
				if(!(child is DXHtmlTableCell))
					throw new ArgumentException("Cannot_Have_Children_Of_Type");
				base.AddAt(index, child);
			}
		}
	}
}
