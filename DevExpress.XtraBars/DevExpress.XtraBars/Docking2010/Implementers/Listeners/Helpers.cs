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
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Dragging.Tabbed;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	class LocationHelper {
		Point startLocation, startPoint;
		public LocationHelper(IUIView view, ILayoutElement element) {
			element.EnsureBounds();
			startLocation = view.ClientToScreen(element.Location);
			startPoint = view.Adapter.DragService.DragOrigin;
		}
		public Point CalcLocation(Point screenPoint) {
			int dx = screenPoint.X - startPoint.X;
			int dy = screenPoint.Y - startPoint.Y;
			return new Point(startLocation.X + dx, startLocation.Y + dy);
		}
	}
	class SplitHelper {
		readonly IUIView View;
		bool IsHorizontal;
		int Length1, Length2, Constraint1, Constraint2;
		Point startLocation, startPoint;
		public SplitHelper(IUIView view, Point clientPoint, IBaseSplitterInfo splitter) {
			View = view;
			IsHorizontal = splitter.IsHorizontal;
			Length1 = splitter.SplitLength1;
			Length2 = splitter.SplitLength2;
			Constraint1 = splitter.SplitConstraint1;
			Constraint2 = splitter.SplitConstraint2;
			this.startLocation = view.ClientToScreen(clientPoint);
			this.startPoint = view.Adapter.DragService.DragOrigin;
		}
		public int CalcChange(Point clientPoint) {
			Point screenPoint = View.ClientToScreen(clientPoint);
			int dx = screenPoint.X - startPoint.X;
			int dy = screenPoint.Y - startPoint.Y;
			return IsHorizontal ? dx : dy;
		}
		public int CalcSplitChange(Point clientPoint) {
			int change = CalcChange(clientPoint);
			if(change >= 0)
				return Math.Min(change, Length2 - Constraint2);
			return -Math.Min(-change, Length1 - Constraint1);
		}
		public bool CanSplit(Point clientPoint) {
			int change = CalcChange(clientPoint);
			return Length1 + change > Constraint1 && Length2 - change > Constraint2;
		}
	}
	class FloatingHelper {
		BaseView ownerCore;
		BaseDocument Document;
		DocumentManagerUIView View;
		public FloatingHelper(DocumentManagerUIView view, ILayoutElement element) {
			View = view;
			IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
			if(info != null) {
				ownerCore = info.Owner;
				Document = info.BaseDocument;
			}
		}
		internal BaseView Owner { get { return ownerCore; } }
		internal IDisposable LockNestedDocumentManagerUpdate() {
			return new NestedDocumentManagerUpdateContext((Document != null) ? Document.Control : null);
		}
		public virtual IUIView GetFloatingView() {
			if(ownerCore != null && Document != null) {
				DocumentManager manager = View.Manager;
				try {
					manager.BeginFloating();
					if(ownerCore.Controller.Float(Document))
						return View.Adapter.GetView(Document.Form);
				}
				finally { manager.EndFloating(); }
			}
			return null;
		}
		public virtual void InitFloatingView(IUIView floatingView, Rectangle screenRect) {
			BaseFloatFormUIView view = (BaseFloatFormUIView)floatingView;
			if((Document != null) && Document.FloatSize.HasValue)
				screenRect.Size = Document.FloatSize.Value;
			view.Form.Bounds = Check(screenRect, view.Adapter.DragService.DragOrigin);
			if(ownerCore != null && Document != null)
				ownerCore.OnFloating(Document);
			view.Form.Visible = true;
			View = null;
			ownerCore = null;
			Document = null;
		}
		static Rectangle Check(Rectangle screenRect, Point startPoint) {
			if(!screenRect.Contains(startPoint))
				screenRect.Offset(startPoint.X - screenRect.X, startPoint.Y - screenRect.Y);
			return screenRect;
		}
	}
	class DockingHelper {
		DocumentManagerUIView View;
		public DockingHelper(DocumentManagerUIView view) {
			View = view;
		}
		IDockingAdornerInfo lastAdornerInfo;
		public void UpdateDockingAdorner(Point point, ILayoutElement element) {
			LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, point);
			if(!ShouldUpdateDockingAdorner(hitInfo)) {
				ResetDockingAdorner();
				return;
			}
			IDockingAdornerInfo adornerInfo = GetDockingAdornerInfo(hitInfo.Element);
			if(adornerInfo != lastAdornerInfo) {
				ResetDockingAdorner(ref lastAdornerInfo);
				lastAdornerInfo = adornerInfo;
			}
			UpdateDockingAdorner(adornerInfo, point, element);
		}
		protected virtual bool ShouldUpdateDockingAdorner(LayoutElementHitInfo hitInfo) {
			return !(hitInfo.Element is DocumentManagerElement || hitInfo.Element is SplitterInfoElement) && !hitInfo.IsEmpty;
		}
		protected virtual IDockingAdornerInfo GetDockingAdornerInfo(ILayoutElement element) {
			return InfoHelper.GetDockingAdornerInfo(element);
		}
		public void ResetDockingAdorner() {
			ResetDockingAdorner(ref lastAdornerInfo);
		}
		public bool CanDrop(Point point, ILayoutElement element) {
			BaseDocument document = GetDocument(element);
			if(document == null || !document.Properties.CanDock) return false;
			LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, point);
			if(hitInfo.IsEmpty) return false;
			IDockingAdornerInfo info = GetDockingAdornerInfo(hitInfo.Element);
			return (info != null) && info.CanDock(point);
		}
		public void Drop(Point point, ILayoutElement element) {
			BaseDocument document = GetDocument(element);
			LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, point);
			if(hitInfo.IsEmpty) return;
			IDockingAdornerInfo info = GetDockingAdornerInfo(hitInfo.Element);
			if(info != null)
				info.Dock(point, document);
		}
		void ResetDockingAdorner(ref IDockingAdornerInfo info) {
			if(info != null) {
				info.ResetDocking(View.Manager.Adorner);
				info = null;
			}
		}
		void UpdateDockingAdorner(IDockingAdornerInfo info, Point point, ILayoutElement element) {
			if(info != null) {
				BaseDocument document = GetDocument(element);
				bool canUpdate = true;
				if(document.IsDockPanel) {
					Docking.DockPanel panel = document.GetDockPanel();
					canUpdate = (panel.DockManager == View.Manager.DockManager);
				}
				if(canUpdate)
					info.UpdateDocking(View.Manager.Adorner, point, document);
			}
		}
		static BaseDocument GetDocument(ILayoutElement element) {
			return BaseDocument.GetDocument(((IDocumentLayoutElement)element).GetElementInfo());
		}
	}
	interface ISnapHelperOwnerView : IUIView {
		DocumentManager Manager { get; }
		Form Form { get; }
		bool InExternalDragging { get; }
		bool CanEmulateSnapping { get; }
		bool IsFloatLocationLocked { get; }
		void SetFloatingBounds(Rectangle bounds);
		void SetFloatLocation(Point point);
		void EndFloating(EndFloatingReason endFloatingReason);
	}
	class SnapHelper {
		ISnapHelperOwnerView Owner;
		Adorner Adorner;
		Rectangle DockingRect;
		bool firstUpdate;
		readonly bool allowSnappingEmulation;
		public SnapHelper(ISnapHelperOwnerView owner) {
			firstUpdate = true;
			Owner = owner;
			Adorner = Owner.Manager.SnapAdorner;
			Rectangle r = Owner.Manager.GetDockingRect();
			DockingRect = new Rectangle(Owner.Manager.ClientToScreen(r.Location), r.Size);
			allowSnappingEmulation = Owner.Manager.View.CanUseSnappingEmulation();
		}
		AdornerElementInfo AdornerInfo;
		DockHint prevHotHint;
		bool CanEmulateSnapping() {
			return Owner.CanEmulateSnapping && allowSnappingEmulation;
		}
		public void UpdateSnapping(Point point, ILayoutElement element) {
			if(!CanEmulateSnapping()) return;
			Screen screen = Screen.FromPoint(point);
			Rectangle screenBounds = screen.WorkingArea;
			Rectangle screenCenterArea = Rectangle.Inflate(screenBounds, -150, -150);
			SnapAdornerInfoArgs args = SnapAdornerInfoArgs.EnsureInfoArgs(ref AdornerInfo, Adorner, Owner.Manager.View);
			bool firstShow = !Adorner.Elements.Contains(AdornerInfo);
			args.Screen = screen.DeviceName;
			args.Bounds = screenBounds;
			args.DockingRect = DockingRect;
			args.MousePosition = point;
			args.DragItem = GetDocument(element);
			if(args.Calc() || firstShow) {
				if(args.HotHint != DockHint.None) {
					Adorner.Show(AdornerInfo, screenBounds);
					if(firstUpdate)
						Adorner.Hide();
					prevHotHint = args.HotHint;
				}
				else ClearAborner();
				Adorner.Invalidate();
				firstUpdate = false;
			}
		}
		protected void ClearAborner() {
			if(prevHotHint != DockHint.None)
				Adorner.Reset(AdornerInfo, true);
			else
				Adorner.Hide();
			prevHotHint = DockHint.None;
		}
		public void ResetSnapping() {
			firstUpdate = true;
			Adorner.Reset(AdornerInfo, true);
			AdornerInfo = null;
		}
		public static bool CanSnap(Point point, SnapAdornerInfoArgs args) {
			DockHint hint = DockHint.None;
			return args.IsOverDockHint(point, out hint);
		}
		public bool CanSnap(Point point, ILayoutElement element) {
			if(!CanEmulateSnapping()) return false;
			if(Owner.Manager.DockManager != null)
				if(Owner.Manager.DockManager.ContainsHotHint()) return false;
			if(AdornerInfo != null) {
				SnapAdornerInfoArgs args = AdornerInfo.InfoArgs as SnapAdornerInfoArgs;
				return CanSnap(point, args);
			}
			return false;
		}
		public static void Snap(Point point, ILayoutElement element, SnapAdornerInfoArgs args, ISnapHelperOwnerView owner) {
			DockHint hint = DockHint.None;
			if(args.IsOverDockHint(point, out hint)) {
				switch(hint) {
					case DockHint.SnapScreen:
						DocumentsHostContext.QueueScreenSnapping(owner.Manager, owner.Form);
						break;
					case DockHint.SnapLeft:
					case DockHint.SnapRight:
					case DockHint.SnapBottom:
						Rectangle dock = new Rectangle(
							args.Bounds.X + args.DockZone.X,
							args.Bounds.Y + args.DockZone.Y,
							args.DockZone.Width, args.DockZone.Height);
						if(!DocumentsHostContext.QueueSnapping(owner.Manager, owner.Form, hint, point, dock)) {
							BaseDocument document = GetDocument(element);
							snaps.Add(document.GetID(), new SnapInfo(point, document.GetRestoreBounds()));
							owner.SetFloatingBounds(dock);
						}
						break;
				}
			}
		}
		public void Snap(Point point, ILayoutElement element) {
			if(!CanEmulateSnapping()) return;
			if(AdornerInfo != null) {
				SnapAdornerInfoArgs args = AdornerInfo.InfoArgs as SnapAdornerInfoArgs;
				Snap(point, element, args, Owner);
				ResetSnapping();
			}
		}
		public static void TryRestoreBounds(ILayoutElement element, Point screenPoint) {
			if(snaps.Count == 0) return;
			BaseDocument document = GetDocument(element);
			if(document != null) {
				SnapInfo snap;
				if(snaps.TryGetValue(document.GetID(), out snap)) {
					document.RestoreBounds(snap.GetRestoreBounds(screenPoint));
					snaps.Remove(document.GetID());
				}
			}
		}
		static IDictionary<object, SnapInfo> snaps = new Dictionary<object, SnapInfo>();
		static BaseDocument GetDocument(ILayoutElement element) {
			return BaseDocument.GetDocument(((IDocumentLayoutElement)element).GetElementInfo());
		}
		internal static void RemoveSnap(IntPtr handle) {
			snaps.Remove(handle);
		}
		internal static void AddSnap(IntPtr handle, Point point, Rectangle restoreBounds) {
			snaps.Add(handle, new SnapInfo(point, restoreBounds));
		}
		class SnapInfo {
			Point SnapPoint;
			Rectangle RestoreBounds;
			public SnapInfo(Point point, Rectangle bounds) {
				this.RestoreBounds = bounds;
				this.SnapPoint = point;
			}
			public Rectangle GetRestoreBounds(Point point) {
				int dx = point.X - SnapPoint.X;
				int dy = point.Y - SnapPoint.Y;
				RestoreBounds.Offset(dx, dy);
				return RestoreBounds;
			}
		}
	}
	static class SnapSystemParameters {
		public static bool GetAllowDockMoving() {
			if(!IsWin7OrAbove()) return true;
			return GetSysParameter(SPI_GETWINARRANGING) && GetSysParameter(SPI_GETDOCKMOVING);
		}
		static Version win7version = new Version(6, 1);
		static bool IsWin7OrAbove() {
			var osVersion = System.Environment.OSVersion;
			return osVersion.Platform == PlatformID.Win32NT && (osVersion.Version >= win7version);
		}
		const uint SPI_GETWINARRANGING = 0x0082;
		const uint SPI_GETDOCKMOVING = 0x0090;
		static bool GetSysParameter(uint param) {
			int val = 0;
			BarNativeMethods.SystemParametersInfo(param, 0, ref val, 0);
			return val != 0;
		}
	}
	class ZOrderHelper {
		public static IntPtr GetRoot(IntPtr hWnd) {
			return BarNativeMethods.GetAncestor(hWnd, GA_ROOT);
		}
		public static void Sort<T>(KeyValuePair<IntPtr, T>[] pairs, T[] items, IComparer<T> comparer) {
			IntPtr top = BarNativeMethods.GetTopWindow(IntPtr.Zero);
			HashSet<IntPtr> hash = new HashSet<IntPtr>();
			for(int i = 0; i < pairs.Length; i++)
				hash.Add(pairs[i].Key);
			IntPtr[] order = new IntPtr[pairs.Length];
			int index = 0;
			while(top != null && top != IntPtr.Zero && hash.Count > 0) {
				if(hash.Contains(top)) {
					order[index++] = top;
					hash.Remove(top);
				}
				top = BarNativeMethods.GetWindow(top, GW_HWNDNEXT);
			}
			new SortHelper<T>(order, pairs, items, comparer);
		}
		class SortHelper<T> : IComparer<KeyValuePair<IntPtr, T>> {
			IntPtr[] order;
			IComparer<T> comparer;
			public SortHelper(IntPtr[] order, KeyValuePair<IntPtr, T>[] pairs, T[] items, IComparer<T> comparer) {
				this.order = order;
				this.comparer = comparer;
				Array.Sort(pairs, items, this);
			}
			int IComparer<KeyValuePair<IntPtr, T>>.Compare(KeyValuePair<IntPtr, T> p1, KeyValuePair<IntPtr, T> p2) {
				if(object.Equals(p1.Key, p2.Key))
					return comparer.Compare(p1.Value, p2.Value);
				int order1 = Array.IndexOf(order, p1.Key);
				int order2 = Array.IndexOf(order, p2.Key);
				return order1.CompareTo(order2);
			}
		}
		const uint GW_HWNDFIRST = 0;
		const uint GW_HWNDLAST = 1;
		const uint GW_HWNDNEXT = 2;
		const uint GW_HWNDPREV = 3;
		const uint GA_PARENT = 1;
		const uint GA_ROOT = 2;
	}
}
namespace DevExpress.XtraBars.Docking2010 {
	class DockHelper {
		BaseView ownerCore;
		protected BaseView Owner {
			get { return ownerCore; }
		}
		public DockHelper(BaseView owner) {
			ownerCore = owner;
		}
		public void DockSide(BaseDocument document, Docking.FloatForm fForm, Customization.DockHint hint) {
			if(fForm != null)
				DockTo(document, fForm, hint, (dockingStyle) =>
				{
					CheckDockWithDenyFloating(document.GetDockPanel(), fForm, (panel, dockManager) =>
						panel.DockLayout.DockTo(dockManager.LayoutManager, dockingStyle, 0));
				});
		}
		public void DockCenterSide(BaseDocument document, Docking.FloatForm fForm, Customization.DockHint hint) {
			Docking.DockPanel panel = (fForm != null) ? fForm.FloatLayout.Panel : null;
			if(panel != null)
				DockTo(document, fForm, hint, (dockingStyle) =>
				{
					CheckDockWithDenyFloating(panel, fForm, (p, dm) =>
						panel.DockTo(dockingStyle));
				});
		}
		void CheckDockWithDenyFloating(Docking.DockPanel panel, Docking.FloatForm floatForm, Action<Docking.DockPanel, Docking.DockManager> dockToOperation) {
			var dockManager = Owner.Manager.DockManager;
			bool allowFloating = panel.Options.AllowFloating;
			if(!allowFloating)
				panel.Parent = dockManager.Form;
			dockToOperation(panel, dockManager);
			if(!allowFloating)
				floatForm.Dispose();
		}
		void DockTo(BaseDocument document, Docking.FloatForm fForm, Customization.DockHint hint, Action<Docking.DockingStyle> dockToOperation) {
			if(!document.IsFloating)
				Owner.Manager.RemoveDocumentFromHost(document);
			Array.ForEach(fForm.OwnedForms, ResetFloatDocumentFormOwner);
			if(IsRTLAware())
				hint = GetRTLDockHint(hint);
			dockToOperation(Customization.DockHintExtension.GetDockingStyle(hint));
			((IBaseViewControllerInternal)Owner.Controller).UnregisterDockPanel(fForm);
		}
		static void ResetFloatDocumentFormOwner(Form form) {
			if(form is BaseFloatDocumentForm)
				form.Owner = null;
		}
		bool IsRTLAware() {
			return Owner.Manager.IsRightToLeftLayout();
		}
		Customization.DockHint GetRTLDockHint(Customization.DockHint hint) {
			switch(hint) {
				case DockHint.SideLeft: return DockHint.SideRight;
				case DockHint.SideRight: return DockHint.SideLeft;
				case DockHint.CenterSideLeft: return DockHint.CenterSideRight;
				case DockHint.CenterSideRight: return DockHint.CenterSideLeft;
			}
			return hint;
		}
	}
}
