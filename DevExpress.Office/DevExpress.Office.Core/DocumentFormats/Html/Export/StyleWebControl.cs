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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Office.Export.Html {
	#region StyleWebControl
	[ToolboxItem(false)]
	public class StyleWebControl : DXHtmlGenericControl, IScriptContainer {
		readonly Dictionary<string, string> styles;
		public StyleWebControl()
			: base(DXHtmlTextWriterTag.Style) {
			this.styles = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.Attributes.Add("type", "text/css");
		}
		public Dictionary<string, string> Styles { get { return styles; } }
		protected override void Render(DXHtmlTextWriter writer) {
			RenderBeginTag(writer);
			RenderStyles(writer);
			RenderEndTag(writer);
		}
		protected internal virtual void RenderStyles(TextWriter writer) {
			foreach (string style in styles.Keys) {
				string name = styles[style];
				writer.WriteLine(string.Format(".{0}{{{1}}}", name, style));
			}
		}
		#region IScriptContainer Members
		public bool IsClientScriptBlockRegistered(string key) {
			return true;
		}
		public void RegisterClientScriptBlock(string key, string script) {
			throw new NotSupportedException();
		}
		public string RegisterCssClass(string style) {
			AddStyle(style, GetClassName(style));
			return styles[style];
		}
		void AddStyle(string style, string name) {
			if (!styles.ContainsKey(style))
				styles.Add(style, name);
		}
		protected internal string GetClassName(string style) {
			return String.Format("cs{0:X}", style.GetHashCode());
		}
		#endregion
	}
	#endregion
}
