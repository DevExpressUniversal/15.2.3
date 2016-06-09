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
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	class FloatPanelUIView : BaseFloatFormUIView {
		public FloatPanelUIView(BaseDocument document)
			: base(document) {
		}
		protected override void RegisterListeners() {
			RegisterUIServiceListener(new BaseFloatFormUIViewNonClientDragListener());
			RegisterUIServiceListener(new FloatPanelUIViewFloatingMovingListener());
		}
		protected override object CheckLayoutHierarchyRootKey() {
			return View.ViewInfo.FloatPanelInfos[Document];
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new FloatPanelElementFactory();
		}
		protected override void SetFloatLocationCore(Point point) {
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			fForm.FloatLayout.Panel.FloatLocation = point;
		}
		protected override void SetFloatingBoundsCore(Rectangle bounds) {
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			fForm.FloatLayout.Panel.FloatLocation = bounds.Location;
			fForm.FloatLayout.Panel.FloatSize = bounds.Size;
		}
		bool inDraggingCore;
		public bool InDragging {
			get { return inDraggingCore; }
		}
		public void StartPanelDocking(Point screenPoint) {
			inDraggingCore = true;
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			Docking.DockPanel panel = fForm.FloatLayout.Panel;
			panel.InitDocking(screenPoint);
		}
		public void PanelDocking(Point screenPoint) {
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			Docking.DockPanel panel = fForm.FloatLayout.Panel;
			Point pt = panel.PointToClient(screenPoint);
			panel.MouseHandler.MouseMove(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
		}
		public void EndPanelDocking(Point screenPoint) {
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			Docking.DockPanel panel = fForm.FloatLayout.Panel;
			Point pt = panel.PointToClient(screenPoint);
			panel.MouseHandler.MouseUp(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
			inDraggingCore = false;
		}
		public void CancelPanelDocking() {
			Docking.FloatForm fForm = Form as Docking.FloatForm;
			Docking.DockPanel panel = fForm.FloatLayout.Panel;
			panel.MouseHandler.Reset();
			inDraggingCore = false;
		}
		bool suspendBehindDraggingCore;
		public void SuspendBehindDragging(bool suspend) {
			suspendBehindDraggingCore = suspend;
		}
		protected internal override bool CanSuspendBehindDragging(ILayoutElement dragItem) {
			return suspendBehindDraggingCore;
		}
	}
}
