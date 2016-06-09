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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorPlaceholderCollection : Collection<HtmlEditorPlaceholderItem> {
		public HtmlEditorPlaceholderCollection(ASPxHtmlEditor editor)
			: base(editor) { }
		public HtmlEditorPlaceholderItem Add(string value) {
			HtmlEditorPlaceholderItem sc = new HtmlEditorPlaceholderItem(value);
			Add(sc);
			return sc;
		}
		protected internal IEnumerable<string> GetValuesArray() {
			foreach(HtmlEditorPlaceholderItem item in this)
				yield return item.Value;
		}
	}
	public class HtmlEditorPlaceholderItem : CollectionItem {
		public HtmlEditorPlaceholderItem() { }
		public HtmlEditorPlaceholderItem(string value) {
			Value = value;
		}
		[DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string Value {
			get { return GetStringProperty("Value", ""); }
			set { SetStringProperty("Value", string.Empty, value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HtmlEditorPlaceholderItem src = source as HtmlEditorPlaceholderItem;
			if(src != null) {
				Value = src.Value;
			}
		}
	}
}
