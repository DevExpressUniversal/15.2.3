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

using DevExpress.Export.Xl;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	public class VmlInsetData {
		public int LeftMargin { get; set; }
		public int TopMargin { get; set; }
		public int RightMargin { get; set; }
		public int BottomMargin { get; set; }
		public void CopyFrom(VmlInsetData source) {
			LeftMargin = source.LeftMargin;
			TopMargin = source.TopMargin;
			RightMargin = source.RightMargin;
			BottomMargin = source.BottomMargin;
		}
	}
	public class VmlTextBox {
		string style; 
		string htmlContent;
		VmlInsetData inset = new VmlInsetData(); 
		public VmlTextBox() {
			style = "mso-direction-alt:auto";
			htmlContent = @"<div style=""text-align:left""></div>";
		}
		#region Properties
		public string Style { get { return style; } set { style = value; } }
		public string HtmlContent { get { return htmlContent; } set { htmlContent = value; } }
		public VmlInsetData Inset { get { return inset; } set { inset = value; } }
		public bool FitShapeToText {
			get {
				return Style.HasPart("mso-fit-shape-to-text:t");
			}
			set {
				if(FitShapeToText == value)
					return;
				Style = Style.ReplacePart("mso-fit-shape-to-text:", value ? "t" : string.Empty);
			}
		}
		public XlReadingOrder TextDirection {
			get {
				if(Style.HasPart("mso-direction-alt:auto"))
					return XlReadingOrder.Context;
				if(Style.HasPart("direction:RTL"))
					return XlReadingOrder.RightToLeft;
				return XlReadingOrder.LeftToRight;
			}
			set {
				XlReadingOrder oldValue = TextDirection;
				if(oldValue == value)
					return;
				string styleContent = Style.ExcludePart("mso-direction-alt:", "direction:");
				if(value == XlReadingOrder.Context)
					Style = styleContent.ReplacePart("mso-direction-alt:", "auto");
				else if(value == XlReadingOrder.RightToLeft)
					Style = styleContent.ReplacePart("direction:", "RTL");
				else
					Style = styleContent;
				if(oldValue == XlReadingOrder.RightToLeft || value == XlReadingOrder.RightToLeft)
					TextAlign = TextAlign;
			}
		}
		public XlHorizontalAlignment TextAlign {
			get {
				if(HtmlContent.StartsWith("<div")) {
					if(HtmlContent.HasPart("text-align:left"))
						return XlHorizontalAlignment.Left;
					if(HtmlContent.HasPart("text-align:right"))
						return XlHorizontalAlignment.Right;
				}
				return XlHorizontalAlignment.General;
			}
			set {
				if(HtmlContent.StartsWith("<div")) {
					if(TextDirection == XlReadingOrder.RightToLeft) {
						if(value == XlHorizontalAlignment.Left)
							HtmlContent = @"<div style=""text-align:left;direction:rtl""></div>";
						else if(value == XlHorizontalAlignment.Right)
							HtmlContent = @"<div style=""text-align:right;direction:rtl""></div>";
						else
							HtmlContent = @"<div></div>";
					}
					else {
						if(value == XlHorizontalAlignment.Left)
							HtmlContent = @"<div style=""text-align:left""></div>";
						else if(value == XlHorizontalAlignment.Right)
							HtmlContent = @"<div style=""text-align:right""></div>";
						else
							HtmlContent = @"<div></div>";
					}
				}
			}
		}
		#endregion
		public void CopyFrom(VmlTextBox source) {
			Style = source.Style;
			HtmlContent = source.HtmlContent;
			Inset.CopyFrom(Inset);
		}
	}
}
