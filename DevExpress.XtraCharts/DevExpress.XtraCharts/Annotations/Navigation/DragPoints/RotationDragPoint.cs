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
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class RotationDragPoint : AnnotationDragPoint {
		public const int Indent = 15;
		readonly DiagramPoint point2;
		protected override Color BackColor { get { return Color.GreenYellow; } }
		protected override CursorType CanDragCursorType { get { return CursorType.Rotate; } }
		protected override CursorType DragCursorType { get { return CursorType.RotateDrag; } }
		protected override string DesignerHint { get { return ChartLocalizer.GetString(ChartStringId.MsgAnnotationRotationToolTip); } }
		public RotationDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2) : base(annotation, point1) {
			this.point2 = point2;
		}
		public override void Render(IRenderer renderer) {
			renderer.DrawLine((Point)Point, (Point)point2, Annotation.ActualBorderColor, 1);
			renderer.EnableAntialiasing(true);
			renderer.FillCircle((Point)point2, PointRadius, BorderColor);
			renderer.FillCircle((Point)point2, PointRadius - 1, BackColor);
			renderer.RestoreAntialiasing();
			ProcessHitTesting(renderer, point2);
		}
		public override AnnotationOperation CreateOperation() {
			return new RotationOperation(Annotation);
		}
	}
}
