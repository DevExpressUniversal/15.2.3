#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Web.Controls {
	[ToolboxItem(false)]
	public class ASPxImageLabelControl : Table {
		private ASPxImage image;
		private ASPxLabel label;
		private bool showImage;
		public ASPxImageLabelControl()
			: base() {
			image = RenderHelper.CreateASPxImage();
			showImage = true;
			label = RenderHelper.CreateASPxLabel();
			label.Style["font-weight"] = "inherit"; 
			HorizontalAlign = HorizontalAlign.Left;
			CellPadding = 0;
			CellSpacing = 0;
			TableCell imageCell = new TableCell();
			TableCell labelCell = new TableCell();
			imageCell.Controls.Add(image);
			labelCell.Controls.Add(label);
			TableRow row = new TableRow();
			row.Cells.Add(imageCell);
			row.Cells.Add(labelCell);
			Rows.Add(row);
			labelCell.Width = Unit.Percentage(100);
			imageCell.Style["padding-right"] = "3px";  
			this.Width = Unit.Percentage(100);
		}
		public string Text {
			get { return label.Text; }
			set {
				image.AlternateText = value;
				label.Text = value;
			}
		}
		public ASPxImage Image {
			get { return image; }
		}
		public ASPxLabel Label {
			get { return label; }
		}
		public bool ShowImage {
			get {
				return showImage;
			}
			set {
				showImage = value;
				Rows[0].Cells[0].Visible = showImage;
			}
		}
		public override System.Drawing.Color ForeColor {
			get {
				return label.ForeColor;
			}
			set {
				label.ForeColor = value;
			}
		}
		public override FontInfo Font {
			get {
				return label.Font;
			}
		}
	}
}
