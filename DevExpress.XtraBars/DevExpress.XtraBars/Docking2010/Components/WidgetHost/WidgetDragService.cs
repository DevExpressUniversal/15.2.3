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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IWidgetsHostDragService : IDisposable {
		void OnBegin(Document document);
		void OnDragging(Document document, MouseEventArgs e);
		void OnDrop(Document document);
	}
	public interface IWidgetsHostSizeServise : IDisposable {
		void OnSizing(DocumentContainer container, ref Rectangle position);
		void OnEndSizing(Document document);
	}
	public class StackLayoutDragService : IWidgetsHostDragService {
		WidgetsHost hostCore;
		public WidgetsHost Host { get { return hostCore; } }
		public DocumentManager Manager { get { return (DocumentManager)hostCore.Owner; } }
		public WidgetView View { get { return Manager.View as WidgetView; } }
		public StackLayoutDragService(WidgetsHost host) {
			hostCore = host;
		}
		public void Dispose() { }
		StackGroup savedParent;
		int savedIndex;
		public virtual void OnBegin(Document document) {
			if(document.Parent != null) {
				lastGroupInfo = document.Parent.Info as StackGroupInfo;
				Point point = document.Info.Bounds.Location;
				savedParent = document.Parent;
				savedIndex = savedParent.Items.IndexOf(document);
				document.Parent.Remove(document);
				lastGroupInfo.OnDragging(point, document);
			}
		}
		StackGroupInfo lastGroupInfo;
		public virtual void OnDragging(Document document, MouseEventArgs e) {
			BaseViewHitInfo info = Manager.CalcHitInfo(e.Location);
			var stackGroupInfo = (info.HitElement as StackGroupInfo);
			if(stackGroupInfo == null && info.Document != null && (info.Document as Document).Parent != null)
				stackGroupInfo = (info.Document as Document).Parent.Info as StackGroupInfo;
			if(stackGroupInfo != null) {
				stackGroupInfo.OnDragging(e.Location, document);
			}
			if(lastGroupInfo != stackGroupInfo) {
				if(lastGroupInfo != null)
					lastGroupInfo.ResetDragging();
				lastGroupInfo = stackGroupInfo;
			}
			Host.Invalidate();
			Application.DoEvents();
		}
		public virtual void OnDrop(Document document) {
			if(lastGroupInfo != null && lastGroupInfo.CanDock(Point.Empty)) {
				lastGroupInfo.EndDragging(document);
				Host.LayoutChanged();
			}
			else {
				if(savedParent != null) {
					savedParent.Items.Insert(savedIndex, document);
					savedParent.Info.AddAnimation();
				}
				Host.UnlockUpdateLayout();
			}
			savedParent = null;
			savedIndex = 0;
		}
	}
	public class TableLayoutDragService : IWidgetsHostDragService {
		WidgetsHost hostCore;
		public WidgetsHost Host { get { return hostCore; } }
		public DocumentManager Manager { get { return (DocumentManager)hostCore.Owner; } }
		public WidgetView View { get { return Manager.View as WidgetView; } }
		public TableGroup Group { get { return View.TableGroup; } }
		public TableLayoutDragService(WidgetsHost host) {
			hostCore = host;
		}
		public void Dispose() { }
		WidgetsWobbleAnimationInfo info;
		public virtual void OnBegin(Document document) {
			if(Group.Items.Contains(document)) {
				Group.Items.Remove(document);
				Group.Info.OnBeginDragging(document);
			}
			StartWobbleAnimation();
			Host.UnlockUpdateLayout();
		}
		void StartWobbleAnimation() {
			info = new WidgetsWobbleAnimationInfo(Host, Host, this, 800000, 50000);
			if(View.AllowDragDropWobbleAnimation != DevExpress.Utils.DefaultBoolean.False) {
				XtraAnimator.Current.AddAnimation(info);
			}
		}
		void StopWobbleAnimation() {
			WidgetsWobbleAnimationInfo existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(Host, this) as WidgetsWobbleAnimationInfo;
			if(existAnimation != null) {
				XtraAnimator.RemoveObject(Host, this);
				existAnimation.Dispose();
				if(info != null) {
					info.Dispose();
					info = null;
				}
			}
			else if(existAnimation == null && info != null) {
				info.Dispose();
				info = null;
			}
			XtraAnimator.RemoveObject(Host);
		}
		public virtual void OnDragging(Document document, MouseEventArgs e) {
			if(!View.Bounds.Contains(e.Location)) return;
			Group.Info.OnDragging(e.Location, document);
			Application.DoEvents();
		}
		public virtual void OnDrop(Document document) {
			StopWobbleAnimation();
			Group.Info.OnEndDragging(document);
			Host.UnlockUpdateLayout();
		}
	}
	public class FlowLayoutDragService : IWidgetsHostDragService {
		WidgetsHost hostCore;
		public WidgetsHost Host { get { return hostCore; } }
		public DocumentManager Manager { get { return (DocumentManager)hostCore.Owner; } }
		public WidgetView View { get { return Manager.View as WidgetView; } }
		public FlowLayoutGroup Group { get { return View.FlowLayoutGroup; } }
		public FlowLayoutDragService(WidgetsHost host) {
			hostCore = host;
		}
		public void Dispose() { }
		public virtual void OnBegin(Document document) {
			if(Group.Items.Contains(document)) {
				Group.Info.OnBeginDragging(document);
				if(Group.ItemDragMode == ItemDragMode.Swap) { 
					StartWobbleAnimation();
				}
			}
		}
		public virtual void OnDragging(Document document, MouseEventArgs e) {
			if(!View.Bounds.Contains(e.Location)) return;
			Group.Info.OnDragging(e.Location, document);
			Application.DoEvents();
			Host.Invalidate();
		}
		public virtual void OnDrop(Document document) {
			if(Group.ItemDragMode == ItemDragMode.Swap) StopWobbleAnimation();
			View.BeginUpdateAnimation();
			Group.Info.OnEndDragging(Cursor.Position, document);
			View.EndUpdateAnimation();
		}
		WidgetsWobbleAnimationInfo info;
		void StartWobbleAnimation() {
			info = new FlowLayoutWidgetsWobbleAnimationInfo(Host, Host, this, 800000, 50000);
			if(View.AllowDragDropWobbleAnimation != DevExpress.Utils.DefaultBoolean.False) {
				XtraAnimator.Current.AddAnimation(info);
			}
		}
		void StopWobbleAnimation() {
			WidgetsWobbleAnimationInfo existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(Host, this) as WidgetsWobbleAnimationInfo;
			if(existAnimation != null) {
				XtraAnimator.RemoveObject(Host, this);
				existAnimation.Dispose();
				if(info != null) {
					info.Dispose();
					info = null;
				}
			}
			else if(existAnimation == null && info != null) {
				info.Dispose();
				info = null;
			}
			XtraAnimator.RemoveObject(Host);
		}
	}
	public class TableLayoutSizeService : IWidgetsHostSizeServise {
		WidgetsHost hostCore;
		public WidgetsHost Host { get { return hostCore; } }
		public DocumentManager Manager { get { return (DocumentManager)hostCore.Owner; } }
		public WidgetView View { get { return Manager.View as WidgetView; } }
		public TableGroup Group { get { return View.TableGroup; } }
		public TableLayoutSizeService(WidgetsHost host) {
			hostCore = host;
		}
		public void Dispose() { }
		public void OnSizing(DocumentContainer container,ref Rectangle position) {
			Group.Info.CheckItemBounds(container, ref position);
		}
		public void OnEndSizing(Document document) {
			Group.Info.OnEndSizing(document);
		}
	}
}
