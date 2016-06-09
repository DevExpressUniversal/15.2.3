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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorCssFile : CollectionItem {
		public HtmlEditorCssFile()
			: base() {
		}
		public HtmlEditorCssFile(string filePath)
			: base() {
			FilePath = filePath;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorCssFileFilePath"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), UrlProperty,
		Editor("DevExpress.Web.Design.CssFileNameEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FilePath {
			get { return GetStringProperty("FilePath", ""); }
			set { SetStringProperty("FilePath", "", value); }
		}
		public override void Assign(CollectionItem source) {
			if (source is HtmlEditorCssFile) {
				HtmlEditorCssFile src = source as HtmlEditorCssFile;
				FilePath = src.FilePath;
			}
			base.Assign(source);
		}
		public override string ToString() {
			return (FilePath != "") ? Path.GetFileName(FilePath) : GetType().Name;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorCssFileCollection : Collection<HtmlEditorCssFile> {
		public HtmlEditorCssFileCollection()
			: base() {
		}
		public HtmlEditorCssFileCollection(IWebControlObject owner)
			: base(owner) {
		}
		public HtmlEditorCssFile Add() {
			return Add("");
		}
		public HtmlEditorCssFile Add(string filePath) {
			HtmlEditorCssFile cssFilePath = new HtmlEditorCssFile(filePath);
			Add(cssFilePath);
			return cssFilePath;
		}
		public void AddRange(IEnumerable<string> cssFiles) {
			base.AddRange(
				new List<string>(cssFiles).ConvertAll<HtmlEditorCssFile>(delegate(string cssFilePath) {
					return new HtmlEditorCssFile(cssFilePath);
				})
			);
		}
		internal void RemoveDuplicates() {
			Hashtable hash = new Hashtable();
			int i = 0;
			while(i < Count) {
				string cssFile = this[i].FilePath;
				if(hash[cssFile] != null)
					RemoveAt(i);
				else {
					hash[cssFile] = 1;
					i++;
				}
			}
		}
		protected override void OnChanged() {
			if (Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
}
