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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views {
	class DocumentsHostWindowRoot : IDocumentsHostWindowRoot, IBaseDocumentInfo {
		BaseView view;
		BaseDocument document;
		IDocumentsHostWindow window;
		int windowHashCode;
		public DocumentsHostWindowRoot(IDocumentsHostWindow window) {
			this.window = window;
			this.windowHashCode = window.GetHashCode();
			this.children = new UIChildren();
			children.Add(window.DocumentManager);
			this.view = window.DocumentManager.View;
			this.document = ((IBaseViewControllerInternal)view.Controller).CreateAndInitializeDocument(window as Control);
			this.document.MarkAsDocumentsHost();
		}
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				children.Remove(window.DocumentManager);
				document.LockFormDisposing();
				Ref.Dispose(ref document);
				this.view = null;
				this.window = null;
			}
			GC.SuppressFinalize(this);
		}
		public override bool Equals(object obj) {
			if(object.ReferenceEquals(obj, this)) return true;
			return object.Equals(window, obj);
		}
		public override int GetHashCode() {
			return windowHashCode;
		}
		IDocumentsHostWindow IDocumentsHostWindowRoot.Window {
			get { return window; }
		}
		Type Views.IUIElementInfo.GetUIElementKey() {
			return typeof(IDocumentsHostWindowRoot);
		}
		BaseDocument IBaseDocumentInfo.BaseDocument {
			get { return document; }
		}
		#region IBaseElementInfo
		bool IBaseElementInfo.IsDisposing {
			get { return isDisposing; }
		}
		BaseView IBaseElementInfo.Owner {
			get { return view; }
		}
		bool IBaseElementInfo.IsVisible {
			get { return ((Form)window).Visible; }
		}
		Rectangle IBaseElementInfo.Bounds {
			get { return ((Form)window).Bounds; }
		}
		void IBaseElementInfo.Calc(Graphics g, Rectangle bounds) { }
		void IBaseElementInfo.Draw(DevExpress.Utils.Drawing.GraphicsCache cache) { }
		void IBaseElementInfo.UpdateStyle() { }
		void IBaseElementInfo.ResetStyle() { }
		#endregion IBaseElementInfo
		#region IUIElement
		IUIElement IUIElement.Scope {
			get { return null; }
		}
		UIChildren children;
		UIChildren IUIElement.Children {
			get { return children; }
		}
		#endregion IUIElement
	}
}
