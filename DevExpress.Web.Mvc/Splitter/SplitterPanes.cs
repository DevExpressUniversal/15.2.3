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
	public class MVCxSplitterPane : SplitterPane {
		public MVCxSplitterPane()
			: base() {
		}
		public MVCxSplitterPane(string name)
			: base(name) {
		}
		public new MVCxSplitterPaneCollection Panes {
			get { return (MVCxSplitterPaneCollection)base.Panes; }
		}
		[EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Web.UI.ControlCollection Controls {
			get {
				return base.Controls;
			}
		}
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		protected override SplitterPaneCollection CreatePanesCollection() {
			return new MVCxSplitterPaneCollection(this);
		}
	}
	public class MVCxSplitterPaneCollection : SplitterPaneCollection {
		public new MVCxSplitterPane this[int index] {
			get { return (GetItem(index) as MVCxSplitterPane); }
		}
		public MVCxSplitterPaneCollection()
			: base() {
		}
		public MVCxSplitterPaneCollection(SplitterPane owner)
			: base(owner) {
		}
		public void Add(Action<MVCxSplitterPane> method) {
			method(Add());
		}
		public void Add(MVCxSplitterPane pane) {
			base.Add(pane);
		}
		public new MVCxSplitterPane Add() {
			return Add("");
		}
		public new MVCxSplitterPane Add(string name) {
			MVCxSplitterPane pane = new MVCxSplitterPane(name);
			Add(pane);
			return pane;
		}
		public new MVCxSplitterPane GetVisiblePane(int index) {
			return GetVisibleItem(index) as MVCxSplitterPane;
		}
		public int IndexOf(MVCxSplitterPane pane) {
			return base.IndexOf(pane);
		}
		public void Insert(int index, MVCxSplitterPane pane) {
			base.Insert(index, pane);
		}
		public void Remove(MVCxSplitterPane pane) {
			base.Remove(pane);
		}
		public new MVCxSplitterPane FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxSplitterPane);
		}
	}
}
