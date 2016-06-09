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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public class ParameterViewInfo {
		readonly Point pointCore;
		readonly ParameterDescription parameterDescriptionCore;
		public Point Point { get { return pointCore; } }
		public ParameterDescription ParameterDescription { get { return parameterDescriptionCore; } }
		public ParameterViewInfo(Point point, ParameterDescription parameterDescription) {
			this.pointCore = point;
			this.parameterDescriptionCore = parameterDescription;
		}
	}
	public interface IShapeParametersAdorner {
		ParameterViewInfo[] Parameters { get; set; }
	}
	public interface IShapeRotationAdorner {
	}
	public class DiagramShapeController : DiagramItemController {
		readonly IFontTraits fontTraits;
		public IDiagramShape DiagramShape { get { return (IDiagramShape)Item; } }
		public ShapeGeometry Shape {
			get { return shape; }
			private set {
				if(shape == value)
					return;
				shape = value;
				OnShapeChanged();
			}
		}
		ShapeGeometry shape;
		public Rect EditorBounds { get { return DiagramShape.Shape.GetEditorBounds(DiagramShape.Size, DiagramShape.GetParameters()); } }
		public DiagramShapeController(IDiagramShape shape)
			: base(shape) {
			fontTraits = new FontTraits(Item);
		}
		IEnumerable<Point> connectionPoints;
		public override IEnumerable<Point> GetConnectionPoints() {
			if (connectionPoints == null)
				connectionPoints = DiagramShape.Shape.GetConnectionPoints(Item.ActualBounds().Size, DiagramShape.GetParameters());
			return connectionPoints;
		}
		public override IFontTraits GetFontTraits() {
			return fontTraits;
		}
		public override IEnumerable<Point> GetIntersectionPoints(Point point1, Point point2) {
			if(Shape == null)
				return base.GetIntersectionPoints(point1, point2);
			Point rotationCenter = Item.GetDiagramRotationCenter();
			Point p1 = point1.Rotate(-Item.Angle, rotationCenter);
			Point p2 = point2.Rotate(-Item.Angle, rotationCenter);
			var points = shape.GetIntersectionPoints(DiagramShape.DiagramPosition(), p1, p2).Select(p => p.Rotate(Item.Angle, rotationCenter)).ToArray();
			return points;
		}
		internal override Func<IDiagramControl, DefaultSelectionLayerHandler, IUpdatableAdorner> SingleSelectionAdornerFactory {
			get {
				return (diagram, handler) => {
					return new CompositeUpdatableAdorner(
						base.SingleSelectionAdornerFactory(diagram, handler),
						diagram.AdornerFactory().CreateShapeParametersAdorner(DiagramShape).AsUpdatableAdorner(UpdateShapeParametersAdorner)
					);
				};
			}
		}
		void UpdateShapeParametersAdorner(IShapeParametersAdorner model) {
			model.Parameters = DiagramShape.Shape.ParameterCollection.Parameters.Select(param => CreateParameterViewInfo(param)).ToArray();
		}
		ParameterViewInfo CreateParameterViewInfo(ParameterDescription parameter) {
			return new ParameterViewInfo(DiagramShape.GetParameterPoint(parameter), parameter);
		}
		protected override void OnSizeChangedCore(Size oldValue) {
			base.OnSizeChangedCore(oldValue);
			UpdateShape();
		}
		public void UpdateShape() {
			Shape = DiagramShape.Shape.GetShape(DiagramShape.ActualBounds().Size, DiagramShape.GetParameters());
			DiagramShape.NotifyInteractionChanged();
		}
		public override Rect GetInplaceEditAdornerBounds() {
			return DiagramShape.GetDiagramRect(EditorBounds);
		}
		void InvalidateConnectionPoins() {
			connectionPoints = null;
		}
		void OnShapeChanged() {
			InvalidateConnectionPoins();
		}
		public override PropertyDescriptor GetTextProperty() {
			return ExpressionHelper.GetProperty((IDiagramShape item) => item.Content);
		}
		#region Themes
		public override DiagramItemStyleId GetDefaultStyleId() {
			return DiagramShape.Shape != null ? DiagramShape.Shape.StyleId : null;
		}
		public override ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return DiagramShapeStyleId.Styles;
		}
		protected override bool GetAllowLightForeground() {
			return Shape != null ? !DiagramShape.Shape.UseBackgroundAsForeground : base.GetAllowLightForeground();
		}
		protected override bool GetAllowLightStroke() {
			return Shape.Return(x => x.GetIsFilled(), () => base.GetAllowLightStroke());
		}
		#endregion
	}
}
