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

using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace WindowsUI {
		class DocumentManagerUIViewUIInteractionListener : UIInteractionServiceListener {
			public DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			public WindowsUIView WindowsUIView {
				get { return View.Manager.View as WindowsUIView; }
			}
			public override bool OnMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
					interactiveInfo.ProcessMouseDown(GetArgs(buttons, hitInfo.HitPoint));
				return false;
			}
			public override bool OnMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
					interactiveInfo.ProcessMouseUp(GetArgs(buttons, hitInfo.HitPoint));
				return false;
			}
			public override void OnMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
					interactiveInfo.ProcessMouseMove(GetArgs(buttons, hitInfo.HitPoint));
			}
			public override void OnMouseLeave(LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null) {
					IBaseElementInfo elementInfo = interactiveInfo as IBaseElementInfo;
					if(elementInfo == null || !elementInfo.IsDisposing)
						interactiveInfo.ProcessMouseLeave();
				}
			}
			public override void OnMouseWheel(MouseEventArgs ea, LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
					interactiveInfo.ProcessMouseWheel(ea);
			}
			public override bool OnFlick(System.Drawing.Point point, DevExpress.Utils.Gesture.FlickGestureArgs args, LayoutElementHitInfo hitInfo) { 
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
				   return  interactiveInfo.ProcessFlick(point, args);
				return false;
			}
			public override bool OnGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters, LayoutElementHitInfo hitInfo) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Element);
				if(interactiveInfo != null)
					return interactiveInfo.ProcessGesture(gid, args, parameters);
				return false;
			}
			public override bool OnClickAction(LayoutElementHitInfo clickInfo) {
				return false;
			}
		}
	}
}
