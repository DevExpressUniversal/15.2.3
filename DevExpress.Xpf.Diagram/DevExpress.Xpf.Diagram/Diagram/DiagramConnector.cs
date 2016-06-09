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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Mvvm.UI.Native;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics;
namespace DevExpress.Xpf.Diagram {
	[TemplatePart(Name = PART_ScaleCanvas, Type = typeof(ScaleCanvas))]
	[TemplatePart(Name = PART_Text, Type = typeof(TextBlock))]
	public partial class DiagramConnector : DiagramItem, IDiagramConnector {
		const string PART_ScaleCanvas = "PART_ScaleCanvas";
		const string PART_Text = "Text";
		#region DEBUGTEST
#if DEBUGTEST
		public ShapeGeometry ShapeForTests { get { return shape; } }
		public Pen PenForTests { get { return pen; } }
		public static Pen SelectionAreaPenForTests { get { return selectionAreaPen; } }
#endif
		#endregion
		#region static
		static readonly Pen selectionAreaPen = DiagramThemeHelper.GetPen(selectionAreaPen, Brushes.Transparent, DiagramConnectorController.SelectionStrokeThickness, null, PenLineCap.Round);
		public static readonly DependencyProperty TextPositionProperty;
		static readonly DependencyPropertyKey TextPositionPropertyKey;
		static DiagramConnector() {
			DependencyPropertyRegistrator<DiagramConnector>.New()
				.OverrideMetadata(TextAlignmentProperty, x => x.Controller().OnAppearancePropertyChanged())
				.RegisterReadOnly(x => x.TextPosition, out TextPositionPropertyKey, out TextPositionProperty, default(Point))
				.FixPropertyValue(CanSnapToThisItemProperty, false)
				.FixPropertyValue(CanSnapToOtherItemsProperty, false)
				.FixPropertyValue(AngleProperty, 0d)
				.FixPropertyValue(CanRotateProperty, false)
				.OverrideDefaultStyleKey();
		}
		#endregion
		ShapeGeometry shape;
		Pen pen;
		public Point TextPosition {
			get { return (Point)GetValue(TextPositionProperty); }
			private set { SetValue(TextPositionPropertyKey, value); }
		}
		public DiagramConnector() { }
		public void UpdateRoute() {
			this.Controller().UpdateRoute();
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			var ownerType = GetType();
			return base.GetEditablePropertiesCore().Select(pd => CoerceEditableProperty(pd)).Concat(new[] {
				DependencyPropertyDescriptor.FromProperty(BeginArrowProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(BeginArrowSizeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(EndArrowProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(EndArrowSizeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(BeginPointProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(EndPointProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(StrokeThicknessProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(TypeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(TextProperty, ownerType),
			})
			.Concat(GetNonBrowsablePropertiesWrappers(new[] {
				DependencyPropertyDescriptor.FromProperty(PointsProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(StrokeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(BeginItemPointIndexProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(EndItemPointIndexProperty, ownerType),
			}));
		}
		PropertyDescriptor CoerceEditableProperty(PropertyDescriptor pd) {
			PropertyDescriptor[] nonBrowsableProperties = new[] {
				DependencyPropertyDescriptor.FromProperty(AngleProperty, GetType()),
				DependencyPropertyDescriptor.FromProperty(PositionProperty, GetType()),
				ExpressionHelper.GetProperty((ItemTraits x) => x.Size)
			};
			if(nonBrowsableProperties.Where(x => x.Name == pd.Name).Any())
				return pd.AddAttributes(new BrowsableAttribute(false));
			return pd;
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateConnectorController();
		}
		protected virtual DiagramConnectorController CreateConnectorController() {
			return new DiagramConnectorController(this);
		}
		protected override void OnRender(DrawingContext drawingContext) {
			shape = this.Controller().GetShape();
			TextPosition = this.Controller().GetTextPosition();
			var connectorGeometry = DiagramShapeFactory.Create(shape);
			pen = DiagramThemeHelper.GetPen(pen, Stroke, StrokeThickness, StrokeDashArray, PenLineCap.Round);
			drawingContext.DrawGeometry(Stroke, pen, connectorGeometry);
			drawingContext.DrawGeometry(Brushes.Transparent, selectionAreaPen, connectorGeometry);
			base.OnRender(drawingContext);
		}
		#region IConnector
		void IDiagramConnector.InvalidateAppearance() {
			InvalidateVisual();
		}
		#endregion
	}
}
