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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetViewInfo : BaseViewInfo {
		IDictionary<StackGroup, IStackGroupInfo> stackGroupInfosCore;
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		public WidgetViewInfo(WidgetView view)
			: base(view) {
			stackGroupInfosCore = new Dictionary<StackGroup, IStackGroupInfo>();
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
			SetDirty();
		}
		protected WidgetView WidgetView {
			get { return View as WidgetView; }
		}
		protected override IDockingAdornerInfo CreateEmptyViewAdornerInfo() {
			return new EmptyViewDockingAdornerInfo(WidgetView);
		}
		protected override Rectangle[] CalculateCore(Graphics g, Rectangle bounds) {
			Control host = View.Manager.GetOwnerControl();
			if(host is WidgetsHost)
				bounds.Location = (host as WidgetsHost).AutoScrollPosition;
			if(WidgetView.LayoutMode == LayoutMode.StackLayout)
				CalcGroups(g, bounds);
			else
				WidgetView.DocumentGroup.Info.Calc(g, bounds);
			return new Rectangle[] { };
		}
		public  bool IsHorizontal {
			get { return WidgetView.Orientation == System.Windows.Forms.Orientation.Horizontal; }
		}
		void CalcGroups(Graphics g, Rectangle bounds) {
			Rectangle boundsGroup = bounds;
			StackGroupLengthHelper.CalcActualGroupLength(WidgetView.Orientation == System.Windows.Forms.Orientation.Horizontal ?
				bounds.Height - (WidgetView.StackGroups.Count + 1) * WidgetView.DocumentSpacing :
				bounds.Width - (WidgetView.StackGroups.Count + 1) * WidgetView.DocumentSpacing, WidgetView.StackGroups);
			if(IsHorizontal) {
				boundsGroup.Y += WidgetView.DocumentSpacing;
			}
			else {
				boundsGroup.X += WidgetView.DocumentSpacing;
			}
			foreach(StackGroup group in WidgetView.StackGroups) {
				IStackGroupInfo groupInfo = null;
				if(!StackGroupInfos.TryGetValue(group, out groupInfo)) continue;
				int width = IsHorizontal ? 100000 : group.ActualLength;
				int height = IsHorizontal ? group.ActualLength : 100000;
				boundsGroup = new Rectangle(boundsGroup.X, boundsGroup.Y, width, height);
				groupInfo.Calc(g, boundsGroup);
				if(IsHorizontal)
					boundsGroup.Y = boundsGroup.Y + boundsGroup.Height + WidgetView.DocumentSpacing;
				else
					boundsGroup.X = boundsGroup.X + boundsGroup.Width + WidgetView.DocumentSpacing;
			}
		}
		protected override bool UseEmptyViewAdorner() {
			return true;
		}
		protected internal IDictionary<StackGroup, IStackGroupInfo> StackGroupInfos {
			get { return stackGroupInfosCore; }
		}
		protected internal IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		protected internal override Point GetFloatLocation(BaseDocument document) {
			return document.Control.Bounds.Location;
		}
		public void RegisterInfo(StackGroup group) {
			if(StackGroupInfos.Keys.Contains(group)) return;
			IStackGroupInfo info = CreateStackGroupInfo(group);
			group.SetInfo(info);
			StackGroupInfos.Add(group, info);
			LayoutHelper.Register(View, info);
		}
		IStackGroupInfo CreateStackGroupInfo(StackGroup group) {
			return new StackGroupInfo(WidgetView, group);
		}
		public void UnregisterInfo(StackGroup group) {
			IStackGroupInfo info;
			if(StackGroupInfos.TryGetValue(group, out info)) {
				StackGroupInfos.Remove(group);
				LayoutHelper.Unregister(View, info);
			}
			group.SetInfo(null);
		}
		public void UnregisterInfo(Document document) {
			IDocumentInfo info;
			if(DocumentInfos.TryGetValue(document, out info)) {
				DocumentInfos.Remove(document);
				LayoutHelper.Unregister(View, info as DragEngine.IUIElement);
			}
		}
		public void RegisterInfo(Document document) {
			if(DocumentInfos.Keys.Contains(document)) return;
			if(!(document.Info is DragEngine.IUIElement)) return;
			DocumentInfos.Add(document, document.Info);
			DragEngine.IUIElement element = document.Info as DragEngine.IUIElement;
			LayoutHelper.Register(View, element);
		}
		AdornerElementInfo draggingAdornerInfo;
		StackGroupDraggingAdornerInfoArgs adornerArgs;
		public void UpdateDragging(IStackGroupInfo stackGroupInfo) {
			Rectangle contentBounds = Rectangle.Intersect(Bounds, stackGroupInfo.Bounds);
			if(draggingAdornerInfo == null) {
				adornerArgs = new StackGroupDraggingAdornerInfoArgs(WidgetView);
				adornerArgs.Content = contentBounds;
				draggingAdornerInfo = new AdornerElementInfo(new StackGroupDraggingAdornerPainter(), adornerArgs);
				WidgetView.Manager.Adorner.Show(draggingAdornerInfo);
				WidgetView.Manager.Adorner.Invalidate();
			}
			if(draggingAdornerInfo != null && adornerArgs.Content != contentBounds) {
				adornerArgs.Content = contentBounds;
				WidgetView.Manager.Adorner.Invalidate();
			}
		}
		public void Reorder(StackGroup group, StackGroup targetGroup) {
			int dropIndex = WidgetView.StackGroups.IndexOf(group);
			int targetIndex = WidgetView.StackGroups.IndexOf(targetGroup);
			WidgetView.BeginUpdateAnimation();
			WidgetView.StackGroups.Remove(group);
			WidgetView.StackGroups.Insert(targetIndex, group);
			WidgetView.EndUpdateAnimation();
			ResetDragging();
		}
		public void ResetDragging() {
			WidgetView.Manager.Adorner.Reset(draggingAdornerInfo);
			draggingAdornerInfo = null;
		}
	}
}
