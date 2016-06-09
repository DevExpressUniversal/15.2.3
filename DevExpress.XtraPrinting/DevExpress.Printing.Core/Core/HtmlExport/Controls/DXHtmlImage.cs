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
	public class DXHtmlImage : DXHtmlGenericControl {
		public DXHtmlImage()
			: base(DXHtmlTextWriterTag.Img) {
		}
		public virtual DXWebImageAlign ImageAlign {
			get {
				object imageAlign = ViewState["ImageAlign"];
				if(imageAlign != null)
					return (DXWebImageAlign)imageAlign;
				return DXWebImageAlign.NotSet;
			}
			set {
				if(value < DXWebImageAlign.NotSet || value > DXWebImageAlign.TextTop)
					throw new ArgumentOutOfRangeException("value");
				ViewState["ImageAlign"] = value;
			}
		}
		public string Alt {
			get {
				string str = Attributes["alt"];
				if(str == null)
					return string.Empty;
				return str;
			}
			set { Attributes["alt"] = DXHtmlControl.MapStringAttributeToString(value); }
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
		public string HeightStr {
			get {
				string height = Attributes["height"];
				return height ?? string.Empty;
			}
			set { Attributes["height"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string Src {
			get {
				string str = Attributes["src"];
				return str ?? string.Empty;
			}
			set { Attributes["src"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string WidthStr {
			get {
				string width = Attributes["width"];
				return width ?? string.Empty;
			}
			set { Attributes["width"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
	}
}
