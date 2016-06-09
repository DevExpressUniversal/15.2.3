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
	public class DXHtmlLiteralControl : DXWebControlBase {
		internal string text;
		public DXHtmlLiteralControl() {
			PreventAutoID();
		}
		public DXHtmlLiteralControl(string text)
			: this() {
			this.text = text ?? string.Empty;
		}
		protected override DXWebControlCollection CreateControlCollection() {
			return new DXWebEmptyControlCollection(this);
		}
		internal override void InitRecursive(DXWebControlBase namingContainer) {
			OnInit(EventArgs.Empty);
		}
		internal override void LoadRecursive() {
			OnLoad(EventArgs.Empty);
		}
		internal override void PreRenderRecursiveInternal() {
			OnPreRender(EventArgs.Empty);
		}
		protected internal override void Render(DXHtmlTextWriter output) {
			output.Write(text);
		}
		internal override void UnloadRecursive(bool dispose) {
			OnUnload(EventArgs.Empty);
			if(dispose)
				Dispose();
		}
		public virtual string Text {
			get { return text; }
			set { text = value ?? string.Empty; }
		}
	}
}
