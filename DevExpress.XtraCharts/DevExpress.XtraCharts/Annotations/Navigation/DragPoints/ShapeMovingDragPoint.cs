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

using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class ShapeMovingDragPoint : AnnotationDragPoint {
		readonly HitRegion hitRegion;
		protected override CursorType CanDragCursorType { get { return CursorType.SizeAll; } }
		protected override CursorType DragCursorType { get { return CursorType.SizeAll; } }
		protected override string DesignerHint { get { return ChartLocalizer.GetString(ChartStringId.MsgAnnotationMovingToolTip); } }
		public ShapeMovingDragPoint(Annotation annotation, HitRegion hitRegion): base(annotation, DiagramPoint.Zero) {
			this.hitRegion = hitRegion;
		}
		public override void Render(IRenderer renderer) {
			renderer.ProcessHitTestRegion(CommonUtils.FindOwnerChart(Annotation).HitTestController, Annotation, new ChartFocusedArea(this), true, hitRegion, true);
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionShapeMoving(Annotation);
			return new FreePositionShapeMoving(Annotation);
		}
	}
}
