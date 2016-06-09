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
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IDocumentInfo {
		Document Document { get; }
		Rectangle Bounds { get; set; }
		int Length { get; set; }
		Size MinSize { get; }
		int RowIndex { get; }
		int ColumnIndex { get; }
		int RowSpan { get; }
		int ColumnSpan { get; }
	}
	public class DocumentInfo : IDocumentInfo, IBaseDocumentInfo, IUIElement {
		Document documentCore;
		public DocumentInfo(WidgetView owner, Document document) {
			documentCore = document;
		}
		bool isDisposing = false;
		public void Dispose() {
			if(!isDisposing) {
				documentCore = null;
				isDisposing = true;
			}
		}
		public int RowIndex { get { return Document.RowIndex; } }
		public int ColumnIndex { get { return Document.ColumnIndex; } }
		public int RowSpan { get { return Document.RowSpan; } }
		public int ColumnSpan { get { return Document.ColumnSpan; } }
		Rectangle boundsCore;
		public Rectangle Bounds {
			get { return boundsCore; }
			set { boundsCore = value; }
		}
		public int Length {
			get { return IsHorizontal ? Document.Width : Document.Height; }
			set {
				if(IsHorizontal)
					Document.Width = value;
				else
					Document.Height = value;
				UpdateStackGroup();
			}
		}
		void UpdateStackGroup() {
			Document.Manager.LayoutChanged();
		}
		public Document Document {
			get { return documentCore; }
		}
		public int CalcMinSize(Graphics g) { return 0; }
		public bool IsHorizontal {
			get { return Document.Parent != null ? Document.Parent.IsHorizontal : false; }
		}
		public Size MinSize {
			get {
				Control child = (Document != null) ? Document.Control : null;
				Size documentMinSize = (child != null) ? child.MinimumSize : Size.Empty;
				return documentMinSize;
			}
		}
		#region IBaseElementInfo Members
		BaseView IBaseElementInfo.Owner { get { return Document.Manager.View; } }
		bool IBaseElementInfo.IsVisible { get { return Document.IsVisible; } }
		bool IBaseElementInfo.IsDisposing { get { return isDisposing; } }
		Rectangle IBaseElementInfo.Bounds { get { return Bounds; } }
		void IBaseElementInfo.Calc(Graphics g, Rectangle bounds) { }
		void IBaseElementInfo.Draw(DevExpress.Utils.Drawing.GraphicsCache cache) { }
		void IBaseElementInfo.UpdateStyle() { }
		void IBaseElementInfo.ResetStyle() { }
		#endregion
		#region IUIElementInfo Members
		Type IUIElementInfo.GetUIElementKey() {
			return typeof(IDocumentInfo);
		}
		#endregion
		#region IUIElement Members
		IUIElement IUIElement.Scope { get { return Document.Manager.View; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion
		#region IBaseDocumentInfo Members
		BaseDocument IBaseDocumentInfo.BaseDocument { get { return Document; } }
		#endregion
	}
}
