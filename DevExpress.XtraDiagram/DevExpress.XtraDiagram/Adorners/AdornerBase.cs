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
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using Rect = System.Windows.Rect;
namespace DevExpress.XtraDiagram.Adorners {
	public abstract class DiagramAdornerBase : IAdorner {
		DiagramElementBounds bounds;
		Point rotationOrigin;
		Rect platformBounds;
		DiagramRectTranslateDelegate platformRectTranslator;
		DiagramPointTranslateDelegate platformPointTranslator;
		DiagramPointTranslateDelegate logicalPointTranslator;
		bool isDestroyed;
		double angle;
		public DiagramAdornerBase() {
			this.isDestroyed = false;
			this.platformBounds = new Rect();
			this.angle = 0;
			this.rotationOrigin = Point.Empty;
		}
		public virtual void Update() {
			UpdateBounds();
		}
		public DiagramElementBounds Bounds { get { return bounds; } }
		public Rectangle LogicalBounds { get { return Bounds.LogicalBounds; } }
		public Rectangle DisplayBounds { get { return Bounds.DisplayBounds; } }
		public Rect PlatformBounds { get { return platformBounds; } }
		public Point RotationOrigin { get { return rotationOrigin; } }
		public double Angle {
			get { return angle; }
			set {
				if(MathUtils.IsEquals(Angle, value))
					return;
				angle = value;
				OnAngleChanged();
			}
		}
		protected virtual void OnAngleChanged() {
			if(IsRotationSupports) UpdateBounds();
		}
		protected void UpdateDisplayBounds(Rectangle displayBounds) {
			this.bounds.SetDisplayRect(displayBounds);
			OnChanged(new DiagramAdornerChangedEventArgs(this, DiagramAdornedChangeKind.Property));
		}
		public int DisplayWidth { get { return DisplayBounds.Width; } }
		public int DisplayHeight { get { return DisplayBounds.Height; } }
		internal void SetPlatformRectTranslator(DiagramRectTranslateDelegate platformRectTranslator) {
			this.platformRectTranslator = platformRectTranslator;
		}
		internal void SetPlatformPointTranslator(DiagramPointTranslateDelegate platformPointTranslator) {
			this.platformPointTranslator = platformPointTranslator;
		}
		internal void SetLogicalPointTranslator(DiagramPointTranslateDelegate logicalPointTranslator) {
			this.logicalPointTranslator = logicalPointTranslator;
		}
		protected DiagramElementBounds PlatformRectToDisplayRect(Rectangle rect) {
			DiagramElementBounds bounds = this.platformRectTranslator(rect);
			if(IsRotationSupports) {
				Point rotationOrigin = CalcRotationOrigin(rect);
				bounds.OffsetDisplay(-rotationOrigin.X, -rotationOrigin.Y);
			}
			return bounds;
		}
		protected Point CalcRotationOrigin(Rectangle rect) {
			if(!IsRotationSupports) return Point.Empty;
			DiagramElementBounds bounds = this.platformRectTranslator(rect);
			return bounds.DisplayBounds.GetCenterPoint();
		}
		protected DiagramElementPoint PlatformPointToDisplayPoint(Point pt) {
			return this.platformPointTranslator(pt);
		}
		protected DiagramElementPoint LogicalPointToDisplayPoint(Point pt) {
			return this.logicalPointTranslator(pt);
		}
		public virtual bool AffectsOnInplaceEditing { get { return false; } }
		public virtual bool AffectsSelection { get { return false; } }
		public virtual bool IsRotationSupports { get { return false; } }
		public virtual bool IsPreviewAdorner { get { return false; } }
		#region IAdorner
		Rect IAdorner.Bounds {
			get { return this.platformBounds; }
			set {
				Rect newRect = AdjustRect(value);
				if(this.platformBounds == newRect)
					return;
				Rect oldRect = this.platformBounds;
				platformBounds = newRect;
				RaisePlatformBoundsChanged(oldRect, newRect);
			}
		}
		double IAdorner.Angle {
			get { return Angle; }
			set { Angle = value; }
		}
		void IAdorner.Destroy() {
			Destroy();
		}
		void IAdorner.MakeTopmost() {
		}
		#endregion
		public virtual bool AffectsDiagramView { get { return false; } }
		protected virtual void RaisePlatformBoundsChanged(Rect prevBounds, Rect nextBounds) {
			UpdateBounds();
			OnBoundsChanged(new DiagramAdornerBoundsChangedEventArgs(this, DisplayBounds));
			if(prevBounds.IsZeroRect() && nextBounds.IsNonZeroRect()) {
				OnInitialized(new DiagramAdornerInitializedEventArgs(this));
			}
			OnChanged(new DiagramAdornerChangedEventArgs(this, DiagramAdornedChangeKind.Property));
		}
		protected virtual void UpdateBounds() {
			this.rotationOrigin = CalcRotationOrigin(this.platformBounds.ToWinRect());
			this.bounds = CalcAdornerBounds();
		}
		protected virtual DiagramElementBounds CalcAdornerBounds() {
			return PlatformRectToDisplayRect(this.platformBounds.ToWinRect());
		}
		protected virtual void Destroy() {
			OnDestroyed(new DiagramAdornerDestroyedEventArgs(this));
			OnChanged(new DiagramAdornerChangedEventArgs(this, DiagramAdornedChangeKind.Destroy));
			this.isDestroyed = true;
		}
		protected virtual Rect AdjustRect(Rect rect) {
			return rect;
		}
		public bool IsDestroyed { get { return isDestroyed; } }
		protected Point AdjustLogicalPoint(Point point) {
			return AdjustPointCore(point, LogicalBounds);
		}
		protected Point AdjustDisplayPoint(Point point) {
			Point newPoint = point;
			if(IsRotationSupports) {
				newPoint.Offset(-RotationOrigin.X, -RotationOrigin.Y);
			}
			return AdjustPointCore(newPoint, DisplayBounds);
		}
		protected Point AdjustPointCore(Point point, Rectangle rect) {
			if(MathUtils.IsNotEquals(Angle, 0)) {
				Point originPt = rect.GetCenterPoint();
				return point.Rotate(originPt, Angle);
			}
			return point;
		}
		public Rectangle AdjustDisplayBounds(DiagramAdornerBase other) {
			Rectangle rect = DisplayBounds;
			if(IsRotationSupports && other.IsRotationSupports) {
				rect.Offset(RotationOrigin.X - other.RotationOrigin.X, RotationOrigin.Y - other.RotationOrigin.Y);
			}
			if(!IsRotationSupports && other.IsRotationSupports) {
				rect.Offset(-other.RotationOrigin.X, -other.RotationOrigin.Y);
			}
			if(IsRotationSupports && !other.IsRotationSupports) {
				rect.Offset(other.RotationOrigin.X, other.RotationOrigin.Y);
			}
			return rect;
		}
		#region Events
		public event DiagramAdornerInitializedEventHandler Initialized;
		protected virtual void OnInitialized(DiagramAdornerInitializedEventArgs e) {
			if(Initialized != null) Initialized(this, e);
		}
		public event DiagramAdornerChangedEventHandler Changed;
		protected virtual void OnChanged(DiagramAdornerChangedEventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		public event DiagramAdornerDestroyedEventHandler Destroyed;
		protected virtual void OnDestroyed(DiagramAdornerDestroyedEventArgs e) {
			if(Destroyed != null) Destroyed(this, e);
		}
		public event DiagramAdornerBoundsChangedEventHandler BoundsChanged;
		protected virtual void OnBoundsChanged(DiagramAdornerBoundsChangedEventArgs e) {
			if(BoundsChanged != null) BoundsChanged(this, e);
		}
		#endregion
		public abstract DiagramAdornerPainterBase GetPainter();
		public abstract DiagramAdornerObjectInfoArgsBase GetDrawArgs();
	}
	public abstract class DiagramItemAdornerBase : DiagramAdornerBase {
		readonly DiagramItem item;
		public DiagramItemAdornerBase(IDiagramItem item) {
			this.item = (DiagramItem)item;
		}
		public DiagramItem Item { get { return item; } }
	}
	public delegate DiagramElementPoint DiagramPointTranslateDelegate(Point point);
	public delegate DiagramElementBounds DiagramRectTranslateDelegate(Rectangle rect);
	public interface IDiagramSelectionSupports {
		bool IsMultiSelection { get; }
	}
	public abstract class DiagramAdornerEventArgs : EventArgs {
		readonly DiagramAdornerBase adorner;
		public DiagramAdornerEventArgs(DiagramAdornerBase adorner) {
			this.adorner = adorner;
		}
		public DiagramAdornerBase Adorner { get { return adorner; } }
	}
	public class DiagramAdornerInitializedEventArgs : DiagramAdornerEventArgs {
		public DiagramAdornerInitializedEventArgs(DiagramAdornerBase adorner)
			: base(adorner) {
		}
	}
	public delegate void DiagramAdornerInitializedEventHandler(object sender, DiagramAdornerInitializedEventArgs e);
	public enum DiagramAdornedChangeKind { Property, Destroy, AdornerElement }
	public class DiagramAdornerChangedEventArgs : DiagramAdornerEventArgs {
		DiagramAdornedChangeKind changeKind;
		public DiagramAdornerChangedEventArgs(DiagramAdornerBase adorner, DiagramAdornedChangeKind changeKind) : base(adorner) {
			this.changeKind = changeKind;
		}
		public DiagramAdornedChangeKind ChangeKind { get { return changeKind; } }
	}
	public delegate void DiagramAdornerChangedEventHandler(object sender, DiagramAdornerChangedEventArgs e);
	public class DiagramAdornerDestroyedEventArgs : DiagramAdornerEventArgs {
		public DiagramAdornerDestroyedEventArgs(DiagramAdornerBase adorner)
			: base(adorner) {
		}
	}
	public delegate void DiagramAdornerDestroyedEventHandler(object sender, DiagramAdornerDestroyedEventArgs e);
	public class DiagramAdornerBoundsChangedEventArgs : DiagramAdornerEventArgs {
		readonly Rectangle bounds;
		public DiagramAdornerBoundsChangedEventArgs(DiagramAdornerBase adorner, Rectangle bounds) : base(adorner) {
			this.bounds = bounds;
		}
		public Rectangle Bounds { get { return bounds; } }
	}
	public delegate void DiagramAdornerBoundsChangedEventHandler(object sender, DiagramAdornerBoundsChangedEventArgs e);
}
