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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using System.Diagnostics;
namespace DevExpress.Xpf.Diagram {
	[TemplatePart(Name = PART_Text, Type = typeof(TextBlock))]
	public partial class DiagramShape : DiagramItem, IDiagramShape {
		#region static
		const string PART_Path = "Path";
		const string PART_Text = "Text";
		static DiagramShape() {
			DependencyPropertyRegistrator<DiagramShape>.New()
				.OverrideMetadata(BackgroundProperty, x => x.Update())
				.OverrideMetadata(VerticalContentAlignmentProperty, VerticalAlignment.Center, x => x.Update())
				.OverrideMetadata(HorizontalContentAlignmentProperty, HorizontalAlignment.Center, x => x.Update())
				.OverrideDefaultStyleKey()
			;
		}
		#endregion
		#region IDiagramShape
		DoubleCollection IDiagramShape.Parameters {
			get { return Parameters; }
			set { Parameters = value != null ? new DoubleCollection(value) : null; }
		}
		#endregion
		TextBlock text;
		Pen pen;
#if DEBUGTEST
		internal TextBlock TextBlockForTests { get { return text; } }
		internal int RenderCountForTests = 0;
		internal Pen PenForTests { get { return pen; } }
#endif
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			text = (TextBlock)GetTemplateChild(PART_Text);
			Update();
		}
		void Update() {
			InvalidateVisual();
			if(text != null) {
				text.Visibility = string.IsNullOrEmpty(Content) ? Visibility.Collapsed : Visibility.Visible;
				text.Margin = GetEditorMargin();
				text.Text = Content;
				text.HorizontalAlignment = HorizontalContentAlignment;
				text.VerticalAlignment = VerticalContentAlignment;
			}
		}
		Thickness GetEditorMargin() {
			Rect editorBounds = this.Controller().EditorBounds;
			if(editorBounds.IsNotaRect() || Bounds.IsNotaRect())
				return new Thickness();
			return new Thickness(editorBounds.Left, editorBounds.Top, Bounds.Width - editorBounds.Right, Bounds.Height - editorBounds.Bottom);
		}
		protected override void OnRender(DrawingContext drawingContext) {
#if DEBUGTEST
			RenderCountForTests++;
#endif
			pen = DiagramThemeHelper.GetPen(pen, Stroke, StrokeThickness, StrokeDashArray);
			DiagramShapeFactory.Draw(this.Controller().Shape, drawingContext, Background, pen);
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			var ownerType = typeof(DiagramShape);
			return base.GetEditablePropertiesCore().Concat(new[] {
				DependencyPropertyDescriptor.FromProperty(StrokeThicknessProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(ContentProperty, ownerType),
			}).Concat(GetNonBrowsablePropertiesWrappers(new[] {
				DependencyPropertyDescriptor.FromProperty(ShapeProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(ParametersProperty, ownerType),
				DependencyPropertyDescriptor.FromProperty(StrokeProperty, ownerType),
			}));
		}
		protected override void OnActualSizeChanged() {
			base.OnActualSizeChanged();
			UpdateRotateTransform();
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateShapeController();
		}
		protected virtual DiagramShapeController CreateShapeController() {
			return new DiagramShapeController(this);
		}
		void OnShapeChanged(ShapeDescription oldShape) {
			UpdateShape();
			Controller.UpdateCustomStyle();
		}
		void UpdateShape() {
			this.Controller().UpdateShape();
			Update();
		}
	}
}
