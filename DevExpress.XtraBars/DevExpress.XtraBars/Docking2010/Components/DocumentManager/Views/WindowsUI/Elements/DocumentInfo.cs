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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentInfo : IBaseDocumentInfo, IUIElement {
		Document Document { get; }
		IContentContainerInfo ParentInfo { get; }
		void SetParentInfo(IContentContainerInfo info);
		void PatchChild(Rectangle view, bool setActive);
	}
	class DocumentInfo : BaseElementInfo, IDocumentInfo {
		Document documentCore;
		public DocumentInfo(WindowsUIView owner, Document document)
			: base(owner) {
			documentCore = document;
		}
		protected override void OnDispose() {
			LayoutHelper.Unregister(this);
			documentCore = null;
			parentInfoCore = null;
			base.OnDispose();
		}
		public override System.Type GetUIElementKey() {
			return typeof(IDocumentInfo);
		}
		BaseDocument IBaseDocumentInfo.BaseDocument {
			get { return documentCore; }
		}
		public Document Document {
			get { return documentCore; }
		}
		#region IDocumentInfo Members
		IContentContainerInfo parentInfoCore;
		public IContentContainerInfo ParentInfo { 
			get { return parentInfoCore; } 
		}
		void IDocumentInfo.SetParentInfo(IContentContainerInfo parentInfo) {
			parentInfoCore = parentInfo;
		}
		void IDocumentInfo.PatchChild(Rectangle view, bool setActive) {
			Control child = Owner.Manager.GetChild(Document);
			if(child != null) {
				if(setActive) {
					PatchMaximized(child);
					child.Bounds = Client;
					child.Update();
				}
				else child.Bounds = new Rectangle(view.X - Client.Width, view.Y - Client.Height, Client.Width, Client.Height);
			}
		}
		internal static void PatchMaximized(Control child) {
			Form form = child as Form;
			if(form != null && form.WindowState != FormWindowState.Normal)
				form.WindowState = FormWindowState.Normal;
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return ParentInfo; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
		Rectangle clientCore;
		public Rectangle Client {
			get { return clientCore; }
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			clientCore = bounds;
		}
		protected override void UpdateStyleCore() {
			Document.UpdateStyle();
		}
	}
}
