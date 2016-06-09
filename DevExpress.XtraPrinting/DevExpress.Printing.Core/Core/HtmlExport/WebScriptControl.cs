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
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Collections.Generic;
using DevExpress.XtraPrinting.HtmlExport;
namespace DevExpress.XtraPrinting.Export.Web {
	public class WebScriptControl : DXHtmlControl {
		private const string clientScriptStart1 = "<script type=\"text/javascript\"";
		private const string clientScriptStart2 = ">\r\n// <![CDATA[\r\n";
		private const string clientScriptEnd = "// ]]>\r\n</script>";
		protected SortedList<string, string> scriptHT = new SortedList<string, string>();
		public WebScriptControl()
			: base(DXHtmlTextWriterTag.Script) {
		}
		public virtual void ClearContent() {
			scriptHT.Clear();
		}
		protected virtual string CreateId(string script) {
			return string.Empty;
		}  
		public void RegisterClientScriptBlock(string key, string script) {
			if(script != null && script.Length > 0)
				scriptHT.Add(key, script);
		}
		protected internal override void Render(DXHtmlTextWriter writer) {
			RenderScripts(writer);
		}
		protected void RenderScripts(DXHtmlTextWriter writer) {
			if(scriptHT.Values.Count == 0)
				return;
			string script = BuildScript();
			WriteScriptStart(writer, script);
			writer.Write(script); 
			writer.WriteLine(clientScriptEnd);
		}
		void WriteScriptStart(DXHtmlTextWriter writer, string script) {
			writer.Write(clientScriptStart1);
			string id = CreateId(script);
			if(!string.IsNullOrEmpty(id))
				writer.Write(string.Concat(" id=\"", id, "\""));
			writer.WriteLine(clientScriptStart2);
		}
		string BuildScript() {
			System.Text.StringBuilder builder = new System.Text.StringBuilder(); 
			foreach(string val in scriptHT.Values) {
				builder.AppendLine(val);
			}
			return builder.ToString();
		}
	}
}
