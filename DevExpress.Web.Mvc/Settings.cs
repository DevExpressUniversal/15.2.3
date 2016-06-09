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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public abstract class SettingsBase {
		WebControl control;
		AttributeCollection attributes;
		ClientSideEventsBase clientSideEvents;
		AppearanceStyleBase controlStyle;
		ImagesBase images;
		CssStyleCollection style;
		StylesBase styles;
		public SettingsBase() {
			this.control = new WebControl(HtmlTextWriterTag.Unknown);
			this.attributes = this.control.Attributes;
			this.clientSideEvents = CreateClientSideEvents();
			this.controlStyle = CreateControlStyle();
			this.images = CreateImages();
			this.style = this.control.Style;
			this.styles = CreateStyles();
			ClientVisibleInternal = true;
			EncodeHtml = true;
			Enabled = true;
			EnableTheming = true;
			RightToLeftInternal = DefaultBoolean.Default;
		}
		public string AccessKey { get; set; }
		public AttributeCollection Attributes { get { return attributes; } }
		public AppearanceStyleBase ControlStyle { get { return controlStyle; } }
		public bool EncodeHtml { get; set; }
		public string Name { get; set; }
		public CssStyleCollection Style { get { return style; } }
		public short TabIndex { get; set; }
		public string Theme { get; set; }
		public virtual string ToolTip { get; set; }
		public bool EnableTheming { get; set; }
		public virtual bool Enabled { get; set; }
		public virtual string SkinID { get; set; }
		public virtual Unit Height { get; set; }
		public virtual Unit Width { get; set; }
		public EventHandler Init { get; set; }
		public EventHandler PreRender { get; set; }
		protected internal bool AccessibilityCompliantInternal { get; set; }
		protected internal ClientSideEventsBase ClientSideEventsInternal { get { return clientSideEvents; } }
		protected internal bool ClientVisibleInternal { get; set; }
		protected internal bool EnableClientSideAPIInternal { get; set; }
		protected internal ImagesBase ImagesInternal { get { return images; } }
		protected internal DefaultBoolean RightToLeftInternal { get; set; }
		protected internal StylesBase StylesInternal { get { return styles; } }
		protected internal EventHandler BeforeGetCallbackResultInternal { get; set; }
		protected internal ASPxClientLayoutHandler ClientLayoutInternal { get; set; }
		protected abstract ClientSideEventsBase CreateClientSideEvents();
		protected virtual AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected abstract ImagesBase CreateImages();
		protected abstract StylesBase CreateStyles();
	}
}
