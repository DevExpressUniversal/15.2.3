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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class NavigationAdornerElementInfoArgs : AdornerElementInfoArgs, INavigationAdorner, IStringImageProvider {
		WindowsUIView ownerCore;
		IContentContainerActionsBarInfo navigationActionBarInfo;
		IContentContainerActionsBarInfo contextActionBarInfo;
		public WindowsUIView View {
			get { return ownerCore; }
		}
		public NavigationAdornerElementInfoArgs(WindowsUIView owner) {
			this.ownerCore = owner;
			navigationActionBarInfo = new NavigationActionsBarInfo(View);
			contextActionBarInfo = new ContextActionsBarInfo(View);
		}
		public IContentContainerActionsBarInfo TopActionsBarInfo {
			get { return navigationActionBarInfo; }
		}
		public IContentContainerActionsBarInfo BottomActionsBarInfo {
			get { return contextActionBarInfo; }
		}
		public bool IsOwnControl(Control control) {
			Control parent = View.Manager.GetContainer();
			return DocumentsHostContext.IsChild(control, parent);
		}
		public void RaiseShown() {
			if(View != null)
				View.RaiseNavigationBarsShown();
		}
		public void RaiseHidden() {
			if(View != null)
				View.RaiseNavigationBarsHidden();
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(Point pt) {
			return GetArgs(Control.MouseButtons, pt);
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return new DevExpress.Utils.DXMouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
		}
		MouseEventArgs hitArgs;
		IContentContainerActionsBarInfo hitInfo;
		public AdornerHitTest HitTest(Point screenPoint) {
			Point clientPoint = View.Manager.ScreenToClient(screenPoint);
			IUIElementInfo info = HitTestCore(clientPoint);
			if(info == null) return AdornerHitTest.None;
			hitInfo = info as IContentContainerActionsBarInfo;
			hitArgs = GetArgs(clientPoint);
			return hitInfo != null ? AdornerHitTest.Adorner : AdornerHitTest.Control;
		}
		public void OnMouseLeave() {
			TopActionsBarInfo.ProcessMouseLeave();
			BottomActionsBarInfo.ProcessMouseLeave();
		}
		public void OnMouseDown(Point screenPoint) {
			if(hitInfo != null)
				hitInfo.ProcessMouseDown(hitArgs);
		}
		public void OnMouseMove(Point screenPoint) {
			if(hitInfo != null)
				hitInfo.ProcessMouseMove(hitArgs);
		}
		public void OnMouseUp(Point screenPoint) {
			if(hitInfo != null)
				hitInfo.ProcessMouseUp(GetArgs(hitArgs.Button | MouseButtons.Left, hitArgs.Location));
		}
		IUIElementInfo HitTestCore(Point clientPoint) {
			IUIElementInfo hitInfo = TopActionsBarInfo.HitTest(clientPoint);
			if(hitInfo == null)
				hitInfo = BottomActionsBarInfo.HitTest(clientPoint);
			return hitInfo;
		}
		Rectangle topBarRect, bottomBarRect;
		public void Update() {
			View.SetCursor(null);
			if(View.contentContainerContextActionBarActivating <= 0)
				TopActionsBarInfo.Update();
			BottomActionsBarInfo.Update();
			SetDirty();
			CalcActionsBarsRect(View.contentContainerContextActionBarActivating > 0);
			View.Manager.Adorner.Invalidate();
		}
		public bool Show() {
			View.SetCursor(null);
			Bounds = View.Manager.GetDockingRect();
			bool contextOnly = (View.contentContainerContextActionBarActivating > 0);
			IContentContainer activeContainer = View.ActiveContentContainer;
			if(activeContainer == null)
				return false;
			IEnumerable<IContentContainerAction> actualActions = activeContainer.Actions;
			IContentContainerInternal container = activeContainer as IContentContainerInternal;
			if(container != null)
				actualActions = container.GetActualActions();
			if(!contextOnly)
				TopActionsBarInfo.AttachToContainer(activeContainer, actualActions);
			BottomActionsBarInfo.AttachToContainer(activeContainer, actualActions);
			CalcActionsBarsRect(contextOnly);
			if(topBarRect.Height > 0 || bottomBarRect.Height > 0) {
				View.CancelSearchPanelAdorner();
				View.Manager.Adorner.hWndInsertAfter = new System.IntPtr(-2);
				View.Manager.Adorner.Show(View.navigationBarInfo);
				SetDirty();
				View.Manager.Adorner.Invalidate();				
				return true;
			}
			if(!contextOnly)
				TopActionsBarInfo.DetachFromContainer(activeContainer);
			BottomActionsBarInfo.DetachFromContainer(activeContainer);
			return false;
		}
		public void Hide() {
			HideCore();
		}
		public void Cancel() {
			HideCore();
		}
		protected void HideCore() {
			if(View == null || View.Manager == null) return;
			View.SetCursor(null);
			TopActionsBarInfo.DetachFromContainer(View.ActiveContentContainer);
			BottomActionsBarInfo.DetachFromContainer(View.ActiveContentContainer);
			if(View.Manager.Adorner != null) {
				View.Manager.Adorner.hWndInsertAfter = System.IntPtr.Zero;
				View.Manager.Adorner.Reset(View.navigationBarInfo);
				View.Manager.Adorner.Clear();
				if(View.Manager.Adorner.Elements.Count > 0)
					View.Manager.Adorner.Invalidate();
			}
		}
		public Image GetImage(string id) {
			if(View != null && View.Manager != null && View.Manager.Images != null)
				return ImageCollection.GetImageListImage(View.Manager.Images, id);
			return null;
		}
		protected override int CalcCore() {
			using(IMeasureContext context = View.BeginMeasure()) {
				TopActionsBarInfo.Calc(context.Graphics, topBarRect);
				BottomActionsBarInfo.Calc(context.Graphics, bottomBarRect);
			}
			return -1;
		}
		void CalcActionsBarsRect(bool contextOnly) {
			using(IMeasureContext context = View.BeginMeasure()) {
				Size topBarSize = contextOnly ? Size.Empty : TopActionsBarInfo.CalcMinSize(context.Graphics);
				Size bottomBarSize = BottomActionsBarInfo.CalcMinSize(context.Graphics);
				if((topBarSize.Height + bottomBarSize.Height) >= Bounds.Height) {
					bottomBarSize.Height = 0;
					if(topBarSize.Height >= Bounds.Height)
						topBarSize.Height = 0;
				}
				topBarRect = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, topBarSize.Height);
				bottomBarRect = new Rectangle(Bounds.Left, Bounds.Bottom - bottomBarSize.Height, Bounds.Width, bottomBarSize.Height);
			}
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return new Rectangle[] { topBarRect, bottomBarRect };
		}
		public static NavigationAdornerElementInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, WindowsUIView owner) {
			NavigationAdornerElementInfoArgs args;
			if(target == null) {
				args = new NavigationAdornerElementInfoArgs(owner);
				target = new AdornerElementInfo(new NavigationAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as NavigationAdornerElementInfoArgs;
			args.SetDirty();
			return args;
		}
	}
	public class NavigationAdornerOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationAdornerElementInfoArgs info = e as NavigationAdornerElementInfoArgs;
			DrawContentContainerActionsBar(e.Cache, info.BottomActionsBarInfo);
			DrawContentContainerActionsBar(e.Cache, info.TopActionsBarInfo);
		}
		protected void DrawContentContainerActionsBar(GraphicsCache cache, IContentContainerActionsBarInfo info) {
			Rectangle bar = new Rectangle(0, 0, info.Bounds.Width, info.Bounds.Height);
			if(bar.Height == 0 || bar.Width == 0) return;
			using(Bitmap barImage = new Bitmap(bar.Width, bar.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				Point clientOffset = new Point(info.Bounds.X, info.Bounds.Y);
				using(Graphics g = Graphics.FromImage(barImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, bar)) {
						bg.Graphics.TranslateTransform(-info.Bounds.X, -info.Bounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							info.Draw(bufferedCache);
						}
						bg.Render();
					}
				}
				cache.Graphics.DrawImageUnscaled(barImage, clientOffset);
			}
		}
	}
}
