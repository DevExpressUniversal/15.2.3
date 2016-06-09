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
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace WindowsUI {
		public class DocumentManagerUIViewResizingListener : ResizingListener {
			SplitHelper helper;
			public DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			public override void OnBegin(Point point, ILayoutElement element) {
				Views.WindowsUI.ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
				info.BeginSplit(View.Manager.Adorner);
				helper = new SplitHelper(View, point, info);
			}
			public override void OnDragging(Point point, ILayoutElement element) {
				int change = helper.CalcChange(point);
				Views.WindowsUI.ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
				info.UpdateSplit(View.Manager.Adorner, change);
			}
			public override void OnDrop(Point point, ILayoutElement element) {
				Views.WindowsUI.ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
				info.ResetSplit(View.Manager.Adorner);
				info.MoveSplitter(helper.CalcSplitChange(point));
			}
			public override void OnCancel() {
				Views.WindowsUI.ISplitterInfo info = InfoHelper.GetSplitterInfo(View.Adapter.DragService.DragItem);
				info.ResetSplit(View.Manager.Adorner);
			}
			public override bool CanDrag(Point point, ILayoutElement element) {
				return helper.CanSplit(point);
			}
			public override bool CanDrop(Point point, ILayoutElement element) {
				Views.WindowsUI.ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
				if(info != null && info.Owner != null)
					return info.Owner.RaiseEndSizing(helper.CalcChange(point), info);
				return true; 
			}
		}
		static class InfoHelper {
			public static Views.WindowsUI.ISplitterInfo GetSplitterInfo(ILayoutElement element) {
				return ((IDocumentLayoutElement)element).GetElementInfo() as Views.WindowsUI.ISplitterInfo;
			}
			public static Views.WindowsUI.ITileInfo GetTileInfo(ILayoutElement element) {
				return ((IDocumentLayoutElement)element).GetElementInfo() as Views.WindowsUI.ITileInfo;
			}
			public static Views.WindowsUI.IDocumentInfo GetDocumentInfo(ILayoutElement element) {
				return ((IDocumentLayoutElement)element).GetElementInfo() as Views.WindowsUI.IDocumentInfo;
			}
		}
		public class DocumentManagerUIViewScrollingListener : ScrollingListener {
			public DocumentManagerUIView View {
				get { return ServiceProvider as DocumentManagerUIView; }
			}
			Point? startPoint;
			public override void OnBegin(Point point, ILayoutElement element) {
				var info = ((IDocumentLayoutElement)element).GetElementInfo();
				IScrollBarInfo scrollInfo = info as IScrollBarInfo;
				if(scrollInfo != null)
					scrollInfo.IsScrolling = true;
				var scrollableInfo = info as Views.IScrollableElementInfo;
				if(scrollableInfo != null) {
					startPoint = point;
					scrollableInfo.OnStartScroll();
				}
			}
			public override void OnDragging(Point point, ILayoutElement element) {
				var info = ((IDocumentLayoutElement)element).GetElementInfo();
				IScrollBarInfo scrollInfo = info as IScrollBarInfo;
				if(scrollInfo != null)
					scrollInfo.ProcessMouseMove(GetArgs(point));
				var scrollableInfo = info as Views.IScrollableElementInfo;
				if(scrollableInfo != null) {
					int delta = scrollableInfo.IsHorizontal ?
						startPoint.Value.X - point.X : startPoint.Value.Y - point.Y;
					scrollableInfo.OnScroll(delta);
				}
			}
			public override void OnDrop(Point point, ILayoutElement element) {
				var info = ((IDocumentLayoutElement)element).GetElementInfo();
				IScrollBarInfo scrollInfo = info as IScrollBarInfo;
				if(scrollInfo != null) {
					scrollInfo.ProcessMouseUp(GetArgs(point));
					scrollInfo.IsScrolling = false;
				}
				var scrollableInfo = info as Views.IScrollableElementInfo;
				if(scrollableInfo != null) {
					scrollableInfo.OnEndScroll();
					startPoint = null;
				}
			}
			public override bool CanDrag(Point point, ILayoutElement element) {
				return true;
			}
			public override bool CanDrop(Point point, ILayoutElement element) {
				return true;
			}
			static DevExpress.Utils.DXMouseEventArgs GetArgs(Point pt) {
				return new DevExpress.Utils.DXMouseEventArgs(Control.MouseButtons, 0, pt.X, pt.Y, 0);
			}
		}
	}
}
