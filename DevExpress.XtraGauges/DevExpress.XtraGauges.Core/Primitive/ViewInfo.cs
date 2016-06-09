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
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Core.Base;
using System.Collections;
using DevExpress.XtraGauges.Core.Drawing;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Primitive {
	public class BasePrimitiveInfo : BaseObject, IViewInfo {
		IRenderableElement ownerCore;
		RectangleF boundBoxCore;
		RectangleF relativeBoundBoxCore;
		PointF[] points;
		PrimitiveState stateCore;
		Matrix local;
		bool isReadyCore;
		public BasePrimitiveInfo(IRenderableElement owner)
			: base() {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {
			this.isReadyCore = false;
			this.stateCore = PrimitiveState.Normal;
			this.boundBoxCore = RectangleF.Empty;
			this.relativeBoundBoxCore = RectangleF.Empty;
		}
		protected override void OnDispose() {
			ownerCore = null;
			points = null;
		}
		public void SetDirty() {
			this.isReadyCore = false;
		}
		public Matrix LocalTransform {
			get { return local; }
		}
		public bool IsReady {
			get { return isReadyCore; }
		}
		public IRenderableElement Owner {
			get { return ownerCore; }
		}
		public void CalcInfo(Matrix local) {
			if(IsReady || IsDisposing) return;
			ClearInfo();
			CalcViewInfoCore(local);
			this.isReadyCore = true;
		}
		protected virtual void CalcViewInfoCore(Matrix local) {
			this.stateCore = CalcPrimitiveState();
			this.local = local;
		}
		protected virtual PrimitiveState CalcPrimitiveState() {
			return Owner.Enabled ? PrimitiveState.Normal : PrimitiveState.Disabled;
		}
		protected virtual RectangleF CalcBoundBox() {
			return (Points.Length == 0) ? RectangleF.Empty : MathHelper.CalcBoundBox(Points);
		}
		protected virtual PointF[] CalcPoints() {
			List<PointF> list = new List<PointF>(64);
			foreach(BaseShape shape in Owner.Shapes) 
				list.AddRange(ShapeHelper.GetPoints(shape));
			return list.ToArray();
		}
		protected virtual RectangleF CalcRelativeBoundBox(Matrix local) {
			return (Points.Length == 0) ? MathHelper.CalcRelativeBoundBox(BoundBox, local) : MathHelper.CalcRelativeBoundBox(MathHelper.CalcBoundBox(Points), local);
		}
		protected virtual void ClearInfo() {
			this.stateCore = PrimitiveState.Normal;
			this.points = null;
			this.local = null;
			this.boundBoxCore = RectangleF.Empty;
			this.relativeBoundBoxCore = RectangleF.Empty;
		}
		public PrimitiveState State {
			get { return stateCore; }
		}
		public PointF[] Points {
			get {
				if(points == null)
					points = CalcPoints();
				return points;
			}
		}
		public RectangleF BoundBox {
			get {
				if(boundBoxCore.IsEmpty)
					boundBoxCore = CalcBoundBox();
				return boundBoxCore;
			}
		}
		public RectangleF RelativeBoundBox {
			get {
				if(relativeBoundBoxCore.IsEmpty)
					relativeBoundBoxCore = CalcRelativeBoundBox(local);
				return relativeBoundBoxCore;
			}
		}
		public bool HitTest(Point pt) {
			bool result = RelativeBoundBox.Contains(pt.X, pt.Y);
			if(result) {
				bool shapeResult = false;
				foreach(BaseShape shape in Owner.Shapes) {
					shape.Accept(
						delegate(BaseShape s) {
							if(shapeResult) return;
							string key = CacheKeys.ShapePathCacheName(shape, s);
							object cachedPath = Owner.TryGetValue(key);
							if(cachedPath == null) {
								if(BaseShape.IsEmptyPath(s.Path))
									return;
								cachedPath = s.Path.Clone();
								GraphicsPath path = (GraphicsPath)cachedPath;
								if(local != null) {
									path.Transform(local);
									Owner.CacheValue(key, path);
								}
							}
							shapeResult |= ((GraphicsPath)cachedPath).IsVisible(pt);
						}
					);
					if(shapeResult) return true;
				}
			}
			return false;
		}
	}
}
