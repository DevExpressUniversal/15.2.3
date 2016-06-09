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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class MVCxPopupWindow : PopupWindow {
		public MVCxPopupWindow()
			: base() {
		}
		public MVCxPopupWindow(string text)
			: base(text) {
		}
		public MVCxPopupWindow(string text, string name)
			: base(text, name) {
		}
		public MVCxPopupWindow(string text, string name, string headerText)
			: base(text, name, headerText) {
		}
		public MVCxPopupWindow(string text, string name, string headerText, string footerText)
			: base(text, name, headerText, footerText) {
		}
		[EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Web.UI.ControlCollection Controls {
			get {
				return base.Controls;
			}
		}
		protected internal string ContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> ContentTemplateContentMethod { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> HeaderContentTemplateContentMethod { get; set; }
		protected internal string FooterTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> FooterTemplateContentMethod { get; set; }
		protected internal string FooterContentTemplateContent { get; set; }
		protected internal Action<PopupControlTemplateContainer> FooterContentTemplateContentMethod { get; set; }
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		public void SetContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			ContentTemplateContentMethod = contentMethod;
		}
		public void SetContentTemplateContent(string content) {
			ContentTemplateContent = content;
		}
		public void SetHeaderTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			HeaderContentTemplateContentMethod = contentMethod;
		}
		public void SetHeaderContentTemplateContent(string content) {
			HeaderContentTemplateContent = content;
		}
		public void SetFooterTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			FooterTemplateContentMethod = contentMethod;
		}
		public void SetFooterTemplateContent(string content) {
			FooterTemplateContent = content;
		}
		public void SetFooterContentTemplateContent(Action<PopupControlTemplateContainer> contentMethod) {
			FooterContentTemplateContentMethod = contentMethod;
		}
		public void SetFooterContentTemplateContent(string content) {
			FooterContentTemplateContent = content;
		}
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
	}
	public class MVCxPopupWindowCollection : PopupWindowCollection {
		public new MVCxPopupWindow this[int index] {
			get { return (GetItem(index) as MVCxPopupWindow); }
		}
		public MVCxPopupWindowCollection()
			: base() {
		}
		public void Add(Action<MVCxPopupWindow> method) {
			method(Add());
		}
		public void Add(MVCxPopupWindow window) {
			base.Add(window);
		}
		public new MVCxPopupWindow Add() {
			MVCxPopupWindow window = new MVCxPopupWindow();
			Add(window);
			return window;
		}
		public new MVCxPopupWindow Add(string text) {
			return Add(text, string.Empty, string.Empty, string.Empty);
		}
		public new MVCxPopupWindow Add(string text, string name) {
			return Add(text, name, string.Empty, string.Empty);
		}
		public new MVCxPopupWindow Add(string text, string name, string headerText) {
			return Add(text, name, headerText, string.Empty);
		}
		public new MVCxPopupWindow Add(string text, string name, string headerText, string footerText) {
			MVCxPopupWindow window = new MVCxPopupWindow(text, name, headerText, footerText);
			Add(window);
			return window;
		}
		public MVCxPopupWindow GetVisibleWindow(int index) {
			return GetVisibleItem(index) as MVCxPopupWindow;
		}
		public int IndexOf(MVCxPopupWindow window) {
			return base.IndexOf(window);
		}
		public void Insert(int index, MVCxPopupWindow window) {
			base.Insert(index, window);
		}
		public void Remove(MVCxPopupWindow window) {
			base.Remove(window);
		}
		public new MVCxPopupWindow FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxPopupWindow);
		}
	}
}
