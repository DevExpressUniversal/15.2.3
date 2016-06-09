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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.ViewInfo {
	public abstract class DiagramItemInfo : IDisposable {
		DiagramItem item;
		Rectangle bounds;
		Rectangle ownerBounds;
		ObjectState state;
		DiagramAppearanceObject paintAppearance;
		DiagramItemPaintControllerBase paintController;
		public DiagramItemInfo(DiagramItem item) {
			this.item = item;
			this.paintAppearance = null;
		}
		internal void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		internal void SetOwnerBounds(Rectangle ownerBounds) {
			this.ownerBounds = ownerBounds;
		}
		internal void SetState(ObjectState state) {
			this.state = state;
		}
		internal void SetPaintController(DiagramItemPaintControllerBase paintController) {
			this.paintController = paintController;
		}
		public DiagramItem Item { get { return item; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle OwnerBounds { get { return ownerBounds; } }
		public ObjectState State { get { return state; } }
		public DiagramAppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public DiagramItemPaintControllerBase PaintController { get { return paintController; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.paintAppearance != null) this.paintAppearance.Dispose();
			}
			this.item = null;
			this.paintAppearance = null;
		}
		public bool HitInItem(Point pt) {
			if(!HitInBounds(pt)) {
				return false;
			}
			return HitInItemCore(pt);
		}
		protected virtual bool HitInBounds(Point pt) {
			return Bounds.Contains(pt);
		}
		public virtual DiagramItemInfo GetHitItem(Point pt) {
			return HitInItem(pt) ? this : null;
		}
		protected abstract bool HitInItemCore(Point pt);
	}
	public abstract class DiagramCompositeItemInfo : DiagramItemInfo {
		Hashtable children;
		public DiagramCompositeItemInfo(DiagramItem item) : base(item) {
			this.children = new Hashtable();
		}
		public virtual void CalcItems(Func<DiagramItem, DiagramItemInfo> calcItems) {
			children.Clear();
			foreach(DiagramItem item in Item.Items) {
				DiagramItemInfo itemInfo = calcItems(item);
				children.Add(item, itemInfo);
				DiagramCompositeItemInfo compositeItem = itemInfo as DiagramCompositeItemInfo;
				if(compositeItem != null)
					compositeItem.CalcItems(calcItems);
			}
		}
		public bool Contains(DiagramItem child) {
			return children.ContainsKey(child);
		}
		public DiagramItemInfo GetChildInfo(DiagramItem item) {
			return (DiagramItemInfo)children[item];
		}
		public ICollection VisibleItems { get { return children.Values; } }
	}
	public abstract class DiagramPathViewItemInfo : DiagramItemInfo {
		DiagramItemView view;
		public DiagramPathViewItemInfo(DiagramItem item) : base(item) {
			this.view = null;
		}
		public void SetView(DiagramItemView view) {
			this.view = view;
		}
		protected override bool HitInItemCore(Point pt) {
			return View.HitTest(pt);
		}
		public DiagramItemView View { get { return view; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.view != null) {
					this.view.Dispose();
				}
				this.view = null;
			}
			base.Dispose(disposing);
		}
		public new IXtraPathView Item { get { return base.Item as IXtraPathView; } }
	}
	public class DiagramContainerInfo : DiagramCompositeItemInfo {
		public DiagramContainerInfo(DiagramContainer item)
			: base(item) {
		}
		protected override bool HitInItemCore(Point pt) {
			return Bounds.Contains(pt);
		}
		public override DiagramItemInfo GetHitItem(Point pt) {
			foreach(DiagramItemInfo child in VisibleItems) {
				DiagramItemInfo target = child.GetHitItem(pt);
				if(target != null) return target;
			}
			return base.GetHitItem(pt);
		}
		public new DiagramContainer Item { get { return base.Item as DiagramContainer; } }
	}
	public class DiagramShapeInfo : DiagramPathViewItemInfo {
		public DiagramShapeInfo(DiagramShape shape)
			: base(shape) {
		}
		public DiagramShape Shape { get { return (DiagramShape)Item; } }
	}
	public class DiagramConnectorInfo : DiagramPathViewItemInfo {
		Pen outlinePen;
		Rectangle outlineBounds;
		public DiagramConnectorInfo(DiagramConnector connector, Pen outlinePen) : base(connector) {
			this.outlinePen = outlinePen;
		}
		internal void SetOutlineBounds(Rectangle bounds) {
			this.outlineBounds = bounds;
		}
		protected override bool HitInBounds(Point pt) {
			return OutlineBounds.Contains(pt);
		}
		protected override bool HitInItemCore(Point pt) {
			return View.OutlineHitTest(pt, outlinePen);
		}
		public new DiagramConnector Item { get { return (DiagramConnector)base.Item; } }
		#region IDisposable
		protected override void Dispose(bool disposing) {
			this.outlinePen = null;
			base.Dispose(disposing);
		}
		#endregion
		public Rectangle OutlineBounds { get { return outlineBounds; } }
	}
}
