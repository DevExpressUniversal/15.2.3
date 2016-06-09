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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Customization {
	[ToolboxItem(false)]
	public abstract class AdornerLayeredWindow : DevExpress.Utils.Internal.DXLayeredWindowEx {
		Adorner adornerCore;
		protected AdornerLayeredWindow(Adorner adorner) {
			adornerCore = adorner;
			TransparentForEvents = true;
		}
		protected AdornerLayeredWindow(Adorner adorner, byte alpha)
			: this(adorner) {
			Alpha = alpha;
		}
		public Adorner Adorner {
			get { return adornerCore; }
		}
		protected override Point GetPaintOffset() {
			return Adorner.PaintOffset;
		}
		protected override IntPtr hWndInsertAfter {
			get {
				var form = Adorner.AdornedControl != null ? Adorner.AdornedControl.FindForm() : null;
				if(form != null && form.TopMost)
					return new IntPtr(-1);
				IntPtr owner = Adorner.hWndInsertAfter;
				return IntPtr.Equals(owner, IntPtr.Zero) ? base.hWndInsertAfter : owner;
			}
		}
		protected void CheckWindowRegion(IEnumerable<Rectangle> regions) {
			using(Region region = new Region()) {
				region.MakeEmpty();
				foreach(Rectangle r in regions) {
					if(r.IsEmpty) continue;
					r.Offset(GetPaintOffset());
					region.Union(r);
				}
				using(Graphics g = Graphics.FromHwndInternal(Handle)) {
					IntPtr hrgn = region.GetHrgn(g);
					BarNativeMethods.SetWindowRgn(Handle, hrgn, false);
					region.ReleaseHrgn(hrgn);
				}
			}
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public bool TransparentForEvents { get; set; }
		protected override void NCHitTest(ref Message m) {
			base.NCHitTest(ref m);
			if(!TransparentForEvents)
				m.Result = new IntPtr(0);
		}
	}
	class TransparentAdornerLayerWindow : AdornerLayeredWindow {
		public TransparentAdornerLayerWindow(Adorner adorner)
			: base(adorner, 128) {
		}
		protected override void DrawCore(GraphicsCache cache) {
			List<Rectangle> rects = new List<Rectangle>();
			cache.Graphics.Clear(Color.Transparent);
			foreach(AdornerElementInfo element in Adorner.Elements) {
				rects.AddRange(element.InfoArgs.CalculateRegions(false));
				ObjectPainter.DrawObject(cache, element.Painter, element.InfoArgs);
			}
			CheckWindowRegion(rects);
		}
	}
	class OpaqueAdornerLayerWindow : AdornerLayeredWindow {
		public OpaqueAdornerLayerWindow(Adorner adorner)
			: base(adorner) {
		}
		protected override void DrawCore(GraphicsCache cache) {
			List<Rectangle> rects = new List<Rectangle>();
			foreach(AdornerElementInfo element in Adorner.Elements) {
				if(element.OpaquePainter == null) continue;
				rects.AddRange(element.InfoArgs.CalculateRegions(true));
				ObjectPainter.DrawObject(cache, element.OpaquePainter, element.InfoArgs);
			}
			CheckWindowRegion(rects);
		}
	}
	public class Adorner : BaseObject {
		AdornerLayeredWindow transparentLayerCore;
		AdornerLayeredWindow opaqueLayerCore;
		public Adorner(Control adornedControl) {
			AssertionException.IsNotNull(adornedControl);
			adornedControlCore = adornedControl;
			elementsCore = new List<AdornerElementInfo>();
			transparentLayerCore = new TransparentAdornerLayerWindow(this);
			opaqueLayerCore = new OpaqueAdornerLayerWindow(this);
		}
		Control adornedControlCore;
		public Control AdornedControl {
			get { return adornedControlCore; }
		}
		public AdornerLayeredWindow TransparentLayer {
			get { return transparentLayerCore; }
		}
		public AdornerLayeredWindow OpaqueLayer {
			get { return opaqueLayerCore; }
		}
		IList<AdornerElementInfo> elementsCore;
		public IList<AdornerElementInfo> Elements {
			get { return elementsCore; }
		}
		protected override void OnDispose() {
			Ref.Dispose(ref transparentLayerCore);
			Ref.Dispose(ref opaqueLayerCore);
			Ref.Clear(ref elementsCore);
			this.adornedControlCore = null;
			base.OnDispose();
		}
		protected internal virtual IntPtr hWndInsertAfter { get; set; }
		bool hasOpaque; 
		bool hasTransparent;
		public void Show(AdornerElementInfo info) {
			if(info == null) return;
			if(!Elements.Contains(info)) {
				Elements.Add(info);
				UpdateFlags();
			}
			if(!AdornedControl.IsHandleCreated) return;
			EnsureLayersCreated();
			Point ownerLocation = AdornedControl.PointToScreen(Point.Empty);
			Control ownerForm = FindParentControl(AdornedControl);
			AdornerLocation = ownerForm.PointToScreen(
				new Point(IsRightToLeftLayout ? ownerForm.ClientSize.Width + AdornerPadding.Width : -AdornerPadding.Width, -AdornerPadding.Height));
			PaintOffset = new Point(ownerLocation.X - AdornerLocation.X, ownerLocation.Y - AdornerLocation.Y);
			Size size = CalcTransparentLayerSize(ownerForm);
			TransparentLayer.Size = size;
			OpaqueLayer.Size = size;
			Show();
		}
		bool? isRightToLeftLayoutCore;
		protected internal bool IsRightToLeftLayout {
			get {
				if(!isRightToLeftLayoutCore.HasValue)
					isRightToLeftLayoutCore = CalcIsRightToLeft();
				return isRightToLeftLayoutCore.Value;
			}
		}
		bool CalcIsRightToLeft() {
			var form = FindParentControl(AdornedControl);
			return (form != null) && DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeftLayout(form);
		}
		protected internal void ResetIsRightToLeft() {
			isRightToLeftLayoutCore = null;
		}
		protected static Control FindParentControl(Control control) {
			Form form = control.FindForm();
			if(form != null) return form;
			while(control != null) {
				if(control.Parent == null)
					return control;
				control = control.Parent;
			}
			return null;
		}
		public void Show(AdornerElementInfo info, Rectangle bounds) {
			Show(info, bounds, Point.Empty);
		}
		public void Show(AdornerElementInfo info, Rectangle bounds, Point paintOffset) {
			if(info == null) return;
			if(!Elements.Contains(info)) {
				Elements.Add(info);
				UpdateFlags();
			}
			if(!AdornedControl.IsHandleCreated) return;
			EnsureLayersCreated();
			PaintOffset = paintOffset;
			AdornerPadding = Size.Empty;
			AdornerLocation = bounds.Location;
			TransparentLayer.Size = bounds.Size;
			OpaqueLayer.Size = bounds.Size;
			Show();
		}
		public void Show() {
			if(hasTransparent)
				TransparentLayer.Show(AdornerLocation);
			if(hasOpaque)
				OpaqueLayer.Show(AdornerLocation);
		}
		public void Clear() {
			if(TransparentLayer.IsCreated)
				TransparentLayer.Clear();
			if(OpaqueLayer.IsCreated)
				OpaqueLayer.Clear();
		}
		public void Hide() {
			if(TransparentLayer.IsCreated)
				TransparentLayer.Hide();
			if(OpaqueLayer.IsCreated)
				OpaqueLayer.Hide();
		}
		public void Reset(AdornerElementInfo info) {
			Reset(info, false);
		}
		public void Reset(AdornerElementInfo info, bool removeSize) {
			if(info == null) return;
			Elements.Remove(info);
			UpdateFlags();
			if(Elements.Count == 0) {
				if(removeSize) {
					if(TransparentLayer.IsCreated)
						TransparentLayer.Size = Size.Empty;
					if(OpaqueLayer.IsCreated)
						OpaqueLayer.Size = Size.Empty;
				}
				Hide();
			}
			if(!hasTransparent) {
				if(TransparentLayer.IsCreated)
					TransparentLayer.Hide();
			}
			if(!hasOpaque) {
				if(OpaqueLayer.IsCreated)
					OpaqueLayer.Hide();
			}
		}
		protected void UpdateFlags() {
			hasTransparent = false;
			hasOpaque = false;
			foreach(AdornerElementInfo info in Elements) {
				hasTransparent |= (info.Painter != null);
				hasOpaque |= (info.OpaquePainter != null);
			}
		}
		public void Invalidate() {
			if(TransparentLayer.IsCreated)
				TransparentLayer.Invalidate();
			if(OpaqueLayer.IsCreated)
				OpaqueLayer.Invalidate();
		}
		public void InvalidateOpaqueLayer() {
			if(OpaqueLayer.IsCreated)
				OpaqueLayer.Invalidate();
		}
		public void InvalidateTransparentLayer() {
			if(TransparentLayer.IsCreated)
				TransparentLayer.Invalidate();
		}
		protected Point AdornerLocation;
		protected Size AdornerPadding = new Size(100, 100);
		internal Point PaintOffset;
		protected void EnsureLayersCreated() {
			if(hasTransparent && !TransparentLayer.IsCreated)
				TransparentLayer.Create(AdornedControl);
			if(hasOpaque && !OpaqueLayer.IsCreated)
				OpaqueLayer.Create(AdornedControl);
		}
		public Point PointToClient(Point screenPoint) {
			return new Point(screenPoint.X - AdornerLocation.X, screenPoint.Y - AdornerLocation.Y);
		}
		public void UpdateTransparentLayerSize() {
			if(TransparentLayer.IsCreated) {
				Control ownerForm = FindParentControl(AdornedControl);
				TransparentLayer.Size = CalcTransparentLayerSize(ownerForm); ;
			}
		}
		Size CalcTransparentLayerSize(Control ownerForm) {
			return new Size(ownerForm.ClientSize.Width + AdornerPadding.Width * 2, ownerForm.ClientSize.Height + AdornerPadding.Height * 2);
		}
	}
	public class SnapAdorner : Adorner {
		public SnapAdorner(Control adornedControl)
			: base(adornedControl) {
		}
		internal AdornerElementInfo GetSnapAdornerInfo() {
			foreach(AdornerElementInfo info in Elements) {
				if(info.InfoArgs is SnapAdornerInfoArgs) {
					return info;
				}
			}
			return null;
		}
	}
	public sealed class AdornerElementInfo : DevExpress.Utils.Animation.BaseAdornerElementInfo {
		public AdornerElementInfo(AdornerPainter painter, AdornerOpaquePainter opaquePainter, AdornerElementInfoArgs info) : base(opaquePainter, info) {
			painterCore = painter;			
		}
		public AdornerElementInfo(AdornerOpaquePainter opaquePainter, AdornerElementInfoArgs info)
			: this(null, opaquePainter, info) {
		}
		public AdornerElementInfo(AdornerPainter painter, AdornerElementInfoArgs info)
			: this(painter, null, info) {
		}
		AdornerPainter painterCore;
		public AdornerPainter Painter {
			get { return painterCore; }
		}	   
	}
	public abstract class AdornerPainter : DevExpress.Utils.Animation.AdornerPainter {
	}
	public abstract class AdornerOpaquePainter : DevExpress.Utils.Animation.AdornerOpaquePainter {
	}
	public abstract class AdornerElementInfoArgs : DevExpress.Utils.Animation.BaseAdornerElementInfoArgs {
		bool isShiftCore;	   
		protected override bool CanModifierShift() {
			if(Control.ModifierKeys == Keys.Shift && isShiftCore == false) {
				isShiftCore = true;
				return true;
			}
			if(Control.ModifierKeys != Keys.Shift && isShiftCore == true) {
				isShiftCore = false;
				return true;
			}
			return false;
		}
	}
}
