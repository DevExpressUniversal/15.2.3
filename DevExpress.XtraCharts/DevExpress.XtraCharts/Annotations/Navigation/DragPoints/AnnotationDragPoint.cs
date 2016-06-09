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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IAnnotationDragPoint : IAnnotationDragElement {
		CursorType CanDragCursorType { get; }
		CursorType DragCursorType { get; }
		string DesignerHint { get; }
		AnnotationOperation CreateOperation();
		Annotation Annotation { get; }
	}
	public abstract class AnnotationDragPoint : IAnnotationDragPoint {
		protected const int PointRadius = 4;
		readonly Annotation annotation;
		readonly DiagramPoint point;
		protected virtual Color BorderColor { get { return Color.Black; } }
		protected virtual Color BackColor { get { return Color.White; } }
		protected abstract CursorType CanDragCursorType { get; }
		protected virtual CursorType DragCursorType { get { return CanDragCursorType; } }
		protected abstract string DesignerHint { get; }
		protected DiagramPoint Point { get { return point; } }
		public Annotation Annotation { get { return annotation; } }
		public AnnotationDragPoint(Annotation annotation, DiagramPoint point) {
			this.annotation = annotation;
			this.point = point;
		}
		#region IAnnotationDragPoint Members
		CursorType IAnnotationDragPoint.CanDragCursorType { get { return CanDragCursorType; } }
		CursorType IAnnotationDragPoint.DragCursorType { get { return DragCursorType; } }
		string IAnnotationDragPoint.DesignerHint { get { return DesignerHint; } }
		void IAnnotationDragElement.DoSelect() {
			annotation.RuntimeOperationSelect = true;
		}
		#endregion
		protected void ProcessHitTesting(IRenderer renderer, DiagramPoint point) {
			Annotation annotation = Annotation;
			HitTestController hitTestController = CommonUtils.FindOwnerChart(Annotation).HitTestController;
			int doubleRadius = PointRadius * 2;
			RectangleF rect = new RectangleF((float)point.X - PointRadius, (float)point.Y - PointRadius, doubleRadius, doubleRadius);
			renderer.ProcessHitTestRegion(hitTestController, annotation, null, new HitRegion(rect), true);
			renderer.ProcessHitTestRegion(hitTestController, annotation, new ChartFocusedArea(this), true, new HitRegion(rect), true);
		}
		public virtual void Render(IRenderer renderer) {
			if (Annotation == null)
				return;
			renderer.EnableAntialiasing(true);
			renderer.FillCircle((Point)point, PointRadius, BorderColor);
			renderer.FillCircle((Point)point, PointRadius - 1, BackColor);
			renderer.RestoreAntialiasing();
			ProcessHitTesting(renderer, point);
		}
		public abstract AnnotationOperation CreateOperation();
	}
}
