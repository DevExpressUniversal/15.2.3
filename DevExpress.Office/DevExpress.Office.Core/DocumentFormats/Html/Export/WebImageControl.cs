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
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.Office.Export.Html {
#if !SL
	[ToolboxItem(false)]
	public class WebImageControl : DXHtmlGenericControl {
		bool urlResolved;
		public WebImageControl()
			: base(DXHtmlTextWriterTag.Img) {
		}
		#region Properties
		[Localizable(true), Bindable(true), DefaultValue("")]
		public virtual string AlternateText {
			get {
				string str = (string)ViewState["alt"];
				if (str != null) {
					return str;
				}
				return string.Empty;
			}
			set {
				this.ViewState["alt"] = value;
			}
		}
		[DefaultValue("")]
		public virtual string DescriptionUrl {
			get {
				string str = (string)ViewState["longdesc"];
				if (str != null)
					return str;
				else
					return string.Empty;
			}
			set {
				ViewState["longdesc"] = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DXWebFontInfo Font { get { return base.Font; } }
		[DefaultValue(false)]
		public virtual bool GenerateEmptyAlternateText {
			get {
				object val = ViewState["alt"];
				return val as string != null;
			}
			set {
				if (value) {
					if (ViewState["alt"] as string == null)
						ViewState["alt"] = string.Empty;
				}
				else
					ViewState.Remove("alt");
			}
		}
		[DefaultValue(0)]
		public virtual DXWebImageAlign ImageAlign {
			get {
				object val = ViewState["align"];
				if (val != null)
					return (DXWebImageAlign)val;
				else
					return DXWebImageAlign.NotSet;
			}
			set {
				if (value < DXWebImageAlign.NotSet || value > DXWebImageAlign.TextTop)
					throw new ArgumentOutOfRangeException("value");
				ViewState["align"] = value;
			}
		}
		[DefaultValue(""), Bindable(true)]
		public virtual string ImageUrl {
			get {
				string str = (string)ViewState["src"];
				if (str != null)
					return str;
				else
					return string.Empty;
			}
			set {
				ViewState["src"] = value;
			}
		}
		internal bool UrlResolved { get { return urlResolved; } set { urlResolved = value; } }
		#endregion
		protected override void AddAttributesToRender(DXHtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			string imageUrl = ImageUrl;
			if (!UrlResolved)
				imageUrl = base.ResolveClientUrl(imageUrl);
			if (imageUrl.Length > 0) {
				if (imageUrl.StartsWithInvariantCultureIgnoreCase("file://"))
					writer.AddAttribute("src", imageUrl, false);
				else
					writer.AddAttribute(DXHtmlTextWriterAttribute.Src, imageUrl);
			}
			imageUrl = DescriptionUrl;
			if (imageUrl.Length != 0)
				writer.AddAttribute(DXHtmlTextWriterAttribute.Longdesc, base.ResolveClientUrl(imageUrl));
			imageUrl = AlternateText;
			if (imageUrl.Length > 0 || GenerateEmptyAlternateText)
				writer.AddAttribute(DXHtmlTextWriterAttribute.Alt, imageUrl);
			string alignString = GetAlignValueString();
			if (!String.IsNullOrEmpty(alignString))
				writer.AddAttribute(DXHtmlTextWriterAttribute.Align, alignString);
			if (BorderWidth.IsEmpty) {
				writer.AddStyleAttribute(DXHtmlTextWriterStyle.BorderWidth, "0px");
			}
		}
		protected internal string GetAlignValueString() {
			switch (ImageAlign) {
				case DXWebImageAlign.Left:
					return "left";
				case DXWebImageAlign.Right:
					return "right";
				case DXWebImageAlign.Baseline:
					return "baseline";
				case DXWebImageAlign.Top:
					return "top";
				case DXWebImageAlign.Middle:
					return "middle";
				case DXWebImageAlign.Bottom:
					return "bottom";
				case DXWebImageAlign.AbsBottom:
					return "absbottom";
				case DXWebImageAlign.AbsMiddle:
					return "absmiddle";
				case DXWebImageAlign.NotSet:
					return String.Empty;
				default:
					return "texttop";
			}
		}
	}
#else
	public class WebImageControl : DXHtmlGenericControl {
		string imageUrl = String.Empty;
		bool generateEmptyAlternateText;
		public WebImageControl()
			: base(DXHtmlTextWriterTag.Img) {
		}
		public string ImageUrl { get { return imageUrl; } set { imageUrl = value ?? string.Empty; } }
		public bool GenerateEmptyAlternateText { get { return generateEmptyAlternateText; } set { generateEmptyAlternateText = value; } }
		protected override void AddAttributesToRender(DXHtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			writer.AddAttribute(DXHtmlTextWriterAttribute.Src, imageUrl);
		}
	}
#endif
}
