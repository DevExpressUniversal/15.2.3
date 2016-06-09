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
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISlideGroupScrollBarInfo :
		IScrollBarInfo {
	}
	class SlideGroupScrollBarInfo : ScrollBarInfo, IUIElement, ISlideGroupScrollBarInfo {
		public SlideGroupScrollBarInfo(IScrollBarInfoOwner owner)
			: base(owner) {
		}
		public Type GetUIElementKey() {
			return typeof(ISlideGroupScrollBarInfo);
		}
		public override ScrollBarType ScrollBarType {
			get { return Owner.IsHorizontal ? ScrollBarType.Horizontal : ScrollBarType.Vertical; }
		}
		public bool IsScrolling { get; set; }
		#region IUIElement Members
		IUIElement IUIElement.Scope { get { return (SlideGroupInfo)Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion
		#region IBaseElementInfo Members
		BaseView IBaseElementInfo.Owner {
			get { return (BaseView)Owner; }
		}
		public bool IsVisible {
			get { return true; }
		}
		public void Calc(Graphics g, Rectangle bounds) { }
		public void Draw(GraphicsCache cache) { }
		public void UpdateStyle() { }
		public void ResetStyle() { }
		#endregion
		#region IInteractiveElementInfo Members
		public void ProcessMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
		}
		public void ProcessMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
		}
		public void ProcessMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
		}
		public void ProcessMouseLeave() {
			Handler.OnMouseLeave();
		}
		public void ProcessMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		public bool ProcessFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) { return false; }
		public bool ProcessGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) { return false; }
		#endregion
	}
	public interface ITileContainerScrollBarInfo :
		IScrollBarInfo {
	}
	class TileContainerScrollBarInfo : ScrollBarInfo, IUIElement, ITileContainerScrollBarInfo {
		public TileContainerScrollBarInfo(IScrollBarInfoOwner owner)
			: base(owner) {
		}
		public Type GetUIElementKey() {
			return typeof(ITileContainerScrollBarInfo);
		}
		public override ScrollBarType ScrollBarType {
			get { return Owner.IsHorizontal ? ScrollBarType.Horizontal : ScrollBarType.Vertical; }
		}
		public bool IsScrolling { get; set; }
		#region IUIElement Members
		IUIElement IUIElement.Scope { get { return (TileContainerInfo)Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion
		#region IBaseElementInfo Members
		BaseView IBaseElementInfo.Owner {
			get { return (BaseView)Owner; }
		}
		public bool IsVisible {
			get { return Visible; }
		}
		public void Calc(Graphics g, Rectangle bounds) { }
		public void Draw(GraphicsCache cache) { }
		public void UpdateStyle() { }
		public void ResetStyle() { }
		#endregion
		#region IInteractiveElementInfo Members
		public void ProcessMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
		}
		public void ProcessMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
		}
		public void ProcessMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
		}
		public void ProcessMouseLeave() {
			Handler.OnMouseLeave();
		}
		public void ProcessMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
		}
		public bool ProcessFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) { return false; }
		public bool ProcessGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) { return false; }
		#endregion
	}
}
