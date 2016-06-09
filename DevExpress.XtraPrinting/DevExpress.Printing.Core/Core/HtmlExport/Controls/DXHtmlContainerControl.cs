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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public abstract class DXHtmlContainerControl : DXHtmlControl {
		[Browsable(false)]
		public virtual string InnerHtml {
			get {
				if(IsLiteralContent())
					return ((DXHtmlLiteralControl)Controls[0]).Text;
				if(Controls.Count != 0)
					throw new Exception("Inner_Content_not_literal");
				return string.Empty;
			}
			set {
				Controls.Clear();
				Controls.Add(new DXHtmlLiteralControl(value));
				ViewState["innerhtml"] = value;
			}
		}
		[Browsable(false)]
		public virtual string InnerText {
			get { return DXHttpUtility.HtmlDecode(InnerHtml); }
			set { InnerHtml = DXHttpUtility.HtmlEncode(value); }
		}
		public bool HasChildren {
			get { return Controls.Count == 0; }
		}
		protected DXHtmlContainerControl()
			: this(DXHtmlTextWriterTag.Span) {
		}
		public DXHtmlContainerControl(DXHtmlTextWriterTag tag)
			: base(tag) {
		}
		protected override DXWebControlCollection CreateControlCollection() {
			return new DXWebControlCollection(this);
		}
		protected internal override void Render(DXHtmlTextWriter writer) {
			RenderBeginTag(writer);
			RenderChildren(writer);
			RenderEndTag(writer);
		}
		protected internal virtual void RenderContents(DXHtmlTextWriter writer) {
			RenderChildren(writer);
		}
		protected override void AddAttributesToRender(DXHtmlTextWriter writer) {
			ViewState.Remove("innerhtml");
			base.AddAttributesToRender(writer);
		}
		protected virtual void RenderEndTag(DXHtmlTextWriter writer) {
			writer.RenderEndTag();
		}
	}
}
