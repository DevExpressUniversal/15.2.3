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
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public abstract class DXHtmlControl : DXWebControlBase {
		internal static string MapIntegerAttributeToString(int n) {
			if(n == -1)
				return null;
			return n.ToString(NumberFormatInfo.InvariantInfo);
		}
		internal static string MapStringAttributeToString(string s) {
			if(s != null && s.Length == 0)
				return null;
			return s;
		}
		DXWebAttributeCollection attributes;
		DXSimpleBitVector32 webControlFlags;
		DXWebStyle controlStyle;
		internal DXHtmlTextWriterTag tagKey;
		public DXWebAttributeCollection Attributes {
			get {
				if(attributes == null)
					attributes = new DXWebAttributeCollection(ViewState);
				return attributes;
			}
		}
		public virtual DXWebUnit BorderWidth {
			get {
				if(!ControlStyleCreated)
					return DXWebUnit.Empty;
				return ControlStyle.BorderWidth;
			}
			set { ControlStyle.BorderWidth = value; }
		}
		internal virtual bool RequiresLegacyRendering {
			get { return false; }
		}
		public bool Disabled {
			get {
				string str = Attributes["disabled"];
				if(str == null)
					return false;
				return str.Equals("disabled");
			}
			set {
				if(value)
					Attributes["disabled"] = "disabled";
				else
					Attributes["disabled"] = null;
			}
		}
		[Browsable(false)]
		public DXCssStyleCollection Style {
			get { return Attributes.CssStyle; }
		}
		public virtual DXWebUnit Width {
			get {
				if(!ControlStyleCreated)
					return DXWebUnit.Empty;
				return ControlStyle.Width;
			}
			set { ControlStyle.Width = value; }
		}
		public virtual DXWebUnit Height {
			get {
				if(!ControlStyleCreated)
					return DXWebUnit.Empty;
				return ControlStyle.Height;
			}
			set { ControlStyle.Height = value; }
		}
		public virtual DXWebFontInfo Font {
			get { return ControlStyle.Font; }
		}
		public virtual string CssClass {
			get {
				if(!ControlStyleCreated)
					return string.Empty;
				return ControlStyle.CssClass;
			}
			set { ControlStyle.CssClass = value; }
		}
		public bool ControlStyleCreated {
			get { return controlStyle != null; }
		}
		public DXWebStyle ControlStyle {
			get {
				if(controlStyle == null) {
					controlStyle = CreateControlStyle();
					if(IsTrackingViewState)
						controlStyle.TrackViewState();
				}
				return controlStyle;
			}
		}
		public virtual DXWebBorderStyle BorderStyle {
			get {
				if(!ControlStyleCreated)
					return DXWebBorderStyle.NotSet;
				return ControlStyle.BorderStyle;
			}
			set { ControlStyle.BorderStyle = value; }
		}
		public virtual bool Enabled {
			get { return !flags[0x80000]; }
			set {
				bool flag = !flags[0x80000];
				if(flag != value) {
					if(!value)
						flags.Set(0x80000);
					else
						flags.Clear(0x80000);
					if(IsTrackingViewState)
						webControlFlags.Set(2);
				}
			}
		}
		public virtual string ToolTip {
			get {
				if(webControlFlags[8]) {
					string str = ViewState["ToolTip"] as string;
					if(str != null)
						return str;
				}
				return string.Empty;
			}
			set {
				ViewState["ToolTip"] = value;
				webControlFlags.Set(8);
			}
		}
		public string TagName {
			get { return tagKey.ToString().ToLowerInvariant(); }
		}
		public DXHtmlTextWriterTag TagKey {
			get { return tagKey; }
		}
		protected DXHtmlControl()
			: this(DXHtmlTextWriterTag.Span) {
		}
		protected DXHtmlControl(DXHtmlTextWriterTag tagKey) {
			this.tagKey = tagKey;
		}
		public override string ToString() {
			return string.Format("{0} <{1}>", GetType().Name, TagKey.ToString().ToLowerInvariant());
		}
		internal void AddDisplayInlineBlockIfNeeded(DXHtmlTextWriter writer) {
			if(!RequiresLegacyRendering && ((BorderStyle != DXWebBorderStyle.NotSet || !BorderWidth.IsEmpty || !Height.IsEmpty || !Width.IsEmpty)))
				writer.AddStyleAttribute(DXHtmlTextWriterStyle.Display, "inline-block");
		}
		protected internal override void Render(DXHtmlTextWriter writer) {
			RenderBeginTag(writer);
		}
		protected override DXWebControlCollection CreateControlCollection() {
			return new DXWebEmptyControlCollection(this);
		}
		protected virtual string GetAttribute(string name) {
			return Attributes[name];
		}
		protected virtual void RenderBeginTag(DXHtmlTextWriter writer) {
			AddAttributesToRender(writer);
			writer.RenderBeginTag(TagKey);
		}
		protected virtual void AddAttributesToRender(DXHtmlTextWriter writer) {
			if(ID != null)
				writer.AddAttribute(DXHtmlTextWriterAttribute.Id, ClientID);
			if(!Enabled)
				writer.AddAttribute(DXHtmlTextWriterAttribute.Disabled, "disabled");
			if(webControlFlags[8]) {
				string toolTip = ToolTip;
				if(toolTip.Length > 0)
					writer.AddAttribute(DXHtmlTextWriterAttribute.Title, toolTip);
			}
			if(TagKey == DXHtmlTextWriterTag.Span || TagKey == DXHtmlTextWriterTag.A)
				AddDisplayInlineBlockIfNeeded(writer);
			if(ControlStyleCreated && !ControlStyle.IsEmpty)
				ControlStyle.AddAttributesToRender(writer, this);
			foreach(string attributeName in Attributes.Keys)
				if(StringExtensions.CompareInvariantCultureIgnoreCase(attributeName, "style") != 0)
					writer.AddAttribute(attributeName, Attributes[attributeName]);
			foreach(string key in Style.Keys)
				writer.AddStyleAttribute(key, Style[key]);
		}
		protected virtual void SetAttribute(string name, string value) {
			Attributes[name] = value;
		}
		protected virtual DXWebStyle CreateControlStyle() {
			return new DXWebStyle(ViewState);
		}
	}
}
