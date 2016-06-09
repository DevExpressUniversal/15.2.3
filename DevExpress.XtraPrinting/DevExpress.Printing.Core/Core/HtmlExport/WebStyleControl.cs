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
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
namespace DevExpress.XtraPrinting.Export.Web {
	public class WebStyleControl : DXHtmlContainerControl {
		private SortedList styles;
		public SortedList Styles { get { return styles; } }
		private SortedList tagStyles;
		public WebStyleControl()
			: base(DXHtmlTextWriterTag.Style) {
			this.Attributes.Add("type", "text/css");
			styles = new SortedList(StringComparer.InvariantCultureIgnoreCase);
			tagStyles = new SortedList();
		}
		public void ClearContent() {
			styles.Clear();
			tagStyles.Clear();
		}
		protected internal override void Render(DXHtmlTextWriter writer) {
			if(styles.Count > 0 || tagStyles.Count > 0) {
				RenderBeginTag(writer);
				RenderContent(writer);
				RenderEndTag(writer);
			}
		}
		void RenderContent(DXHtmlTextWriter writer) {
			for(int i = 0; i < styles.Count; i++) {
				string name = GetName(styles, i);
				string format = (name.IndexOf(".") < 0) ? ".{0} {{{1}}}" : "{0} {{{1}}}";
				writer.WriteLine(format, name, GetValue(styles, i));
			}
			for(int i = 0; i < tagStyles.Count; i++)
				writer.WriteLine("{0} {{{1}}}", GetName(tagStyles, i), GetValue(tagStyles, i));
		}
		string GetName(SortedList list, int index) {
			return (string)list.GetByIndex(index);
		}
		string GetValue(SortedList list, int index) {
			return (string)list.GetKey(index);
		}
		public void AddStyle(string style, string name) {
			if(!styles.ContainsKey(style))
				styles.Add(style, name);
		}
		public void AddTagStyle(string style, string tagName) {
			if(!tagStyles.ContainsKey(style))
				tagStyles.Add(style, tagName);
		}
		public string RegisterStyle(string style) {
			AddStyle(style, GetClassName(style));
			return (string)styles[style];
		}
		string GetClassName(string style) {
			return String.Format("cs{0:X}", style.GetHashCode());
		}
	}
}
