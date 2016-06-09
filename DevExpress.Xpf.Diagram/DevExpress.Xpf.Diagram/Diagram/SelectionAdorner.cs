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

namespace DevExpress.Xpf.Diagram {
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Input;
	using System.Windows.Media;
	using DevExpress.Diagram.Core;
	using DevExpress.Mvvm.Native;
	using DevExpress.Mvvm.UI.Native;
	using DevExpress.Utils;
	using DevExpress.Xpf.Diagram.Native;
	using IInputElement = DevExpress.Diagram.Core.IInputElement;
	using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
	[TemplatePart(Name = PART_ResizerTopLeft, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerTop, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerTopRight, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerLeft, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerRight, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerBottomLeft, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerBottom, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_ResizerBottomRight, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PART_MoveBorder, Type = typeof(FrameworkElement))]
	public partial class SelectionAdorner : Control<SelectionAdorner>, ISelectionAdorner {
		const string PART_ResizerTopLeft = "ResizerTopLeft";
		const string PART_ResizerTop = "ResizerTop";
		const string PART_ResizerTopRight = "ResizerTopRight";
		const string PART_ResizerLeft = "ResizerLeft";
		const string PART_ResizerRight = "ResizerRight";
		const string PART_ResizerBottomLeft = "ResizerBottomLeft";
		const string PART_ResizerBottom = "ResizerBottom";
		const string PART_ResizerBottomRight = "ResizerBottomRight";
		const string PART_MoveBorder = "MoveBorder";
		const string PART_RotationIcon = "RotationIcon";
		static SelectionAdorner() {
			 DependencyPropertyRegistrator<SelectionAdorner>.New()
				.RegisterReadOnly(x => x.IsMultipleSelection, out IsMultipleSelectionPropertyKey, out IsMultipleSelectionProperty, false)
				.Register(x => x.CanResize, out CanResizeProperty, false)
				.Register(x => x.ShowFullUI, out ShowFullUIProperty, true)
				.Register(x => x.CanRotate, out CanRotateProperty, false)
				;
		}
		readonly DefaultSelectionLayerHandler selectionHandler;
		public SelectionAdorner(DefaultSelectionLayerHandler selectionHandler, double? multipleSelectionMargin) {
			this.selectionHandler = selectionHandler;
			IsMultipleSelection = multipleSelectionMargin.HasValue;
			Padding = multipleSelectionMargin.Return(x => new Thickness(x.Value), () => default(Thickness));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetInputElement(PART_MoveBorder, () => IsMultipleSelection ?
				new MultipleSelectionAdornerInputElement(selectionHandler.Diagram) :
				selectionHandler.PrimarySelection.Controller.CreateInputElement());
			foreach(var mode in Enum.GetValues(typeof(ResizeMode)).OfType<ResizeMode>()) {
				SetInputElement("Resizer" + mode, () => ItemResizeInputElement.Create(mode, selectionHandler));
			}
			SetInputElement(PART_RotationIcon, () => RotateShapeInputElement.Create(selectionHandler));
		}
		void SetInputElement(string elementName, Func<IInputElement> createInputElement) {
			(GetTemplateChild(elementName) as FrameworkElement)
				.Do(x => DiagramInput.SetInputElementFactory(x, createInputElement));
		}
	}
	public partial class ShapeParametersAdorner : Control<ShapeParametersAdorner>, IShapeParametersAdorner {
		readonly IDiagramShape shape;
		ItemsControl itemsControl;
		static ShapeParametersAdorner() {
			DependencyPropertyRegistrator<ShapeParametersAdorner>.New()
				.Register(x => x.Parameters, out ParametersProperty, default(ParameterViewInfo[]))
			;
		}
		public ShapeParametersAdorner(IDiagramShape shape) {
			this.shape = shape;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			itemsControl = (ItemsControl)GetTemplateChild("PART_ItemsControl");
			itemsControl.Do(x => x.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged);
		}
		void ItemContainerGenerator_StatusChanged(object sender, EventArgs e) {
			if(itemsControl.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
				return;
			for(int itemIndex = 0; itemIndex < itemsControl.Items.Count; itemIndex++) {
				FrameworkElement child = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromIndex(itemIndex);
				ParameterDescription parameter = Parameters[itemIndex].ParameterDescription;
				DiagramInput.SetInputElementFactory(child, () => ChangeParameterInputElement.Create(shape, parameter));
			}
		}
	}
	public partial class SelectionPartAdornerBase : Control, ISelectionPartAdorner {
		static SelectionPartAdornerBase() {
			DependencyPropertyRegistrator<SelectionPartAdornerBase>.New()
				.RegisterReadOnly(x => x.IsPrimarySelection, out IsPrimarySelectionPropertyKey, out IsPrimarySelectionProperty, default(bool));
		}
		public SelectionPartAdornerBase(IDiagramItem item, bool isPrimarySelection) {
			IsPrimarySelection = isPrimarySelection;
		}
	}
	public class SelectionPartAdorner : SelectionPartAdornerBase {
		static SelectionPartAdorner() {
			DependencyPropertyRegistrator<SelectionPartAdorner>.New()
				.OverrideDefaultStyleKey()
				;
		}
		public SelectionPartAdorner(IDiagramItem item, bool isPrimarySelection) : base(item, isPrimarySelection) {
			DiagramInput.SetInputElementFactory(this, () => item.With(x => x.Controller.CreateInputElement()));
		}
	}
	public partial class ShapePresenter : Control<ShapePresenter> {
		#region DEBUGTEST
#if DEBUGTEST
		public Pen PenForTests { get { return pen; } }
#endif
#endregion
		public static readonly DependencyProperty StrokeProperty;
		public Brush Stroke {
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}
		public static readonly DependencyProperty StrokeThicknessProperty;
		public double StrokeThickness {
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}
		public static readonly DependencyProperty StrokeDashArrayProperty;
		public DoubleCollection StrokeDashArray {
			get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
			set { SetValue(StrokeDashArrayProperty, value); }
		}
		static ShapePresenter() {
			DependencyPropertyRegistrator<ShapePresenter>.New()
				.Register(x => x.Zoom, out ZoomProperty, 1d, FrameworkPropertyMetadataOptions.AffectsRender)
				.AddOwner(x => x.Stroke, out StrokeProperty, DiagramShape.StrokeProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.AddOwner(x => x.StrokeThickness, out StrokeThicknessProperty, DiagramShape.StrokeThicknessProperty, 1, FrameworkPropertyMetadataOptions.AffectsRender)
				.AddOwner(x => x.StrokeDashArray, out StrokeDashArrayProperty, DiagramShape.StrokeDashArrayProperty, null, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.Shape, out ShapeProperty, default(ShapeGeometry), FrameworkPropertyMetadataOptions.AffectsRender)
				;
		}
		Pen pen;
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if(Shape == null)
				return;
			pen = DiagramThemeHelper.GetPen(pen, Stroke, StrokeThickness, StrokeDashArray);
			DiagramShapeFactory.Draw(Shape.Scale(new Size(Zoom, Zoom)), drawingContext, Background, pen);
		}
	}
	public partial class ConnectorSelectionPartAdorner : SelectionPartAdornerBase, IConnectorSelectionPartAdorner {
		static ConnectorSelectionPartAdorner() {
			DependencyPropertyRegistrator<ConnectorSelectionPartAdorner>.New()
				.Register(x => x.Shape, out ShapeProperty, default(ShapeGeometry))
				.OverrideDefaultStyleKey()
				;
		}
		public ConnectorSelectionPartAdorner(IDiagramConnector item, bool isPrimarySelection) : base(item, isPrimarySelection) {
		}
	}
	public abstract partial class ConnectorPreviewAdornerBase : Control {
		static ConnectorPreviewAdornerBase() {
			DependencyPropertyRegistrator<ConnectorPreviewAdornerBase>.New()
				.Register(x => x.Shape, out ShapeProperty, default(ShapeGeometry))
				.Register(x => x.Stroke, out StrokeProperty, default(Brush))
				.Register(x => x.StrokeThickness, out StrokeThicknessProperty, 1d)
				;
		}
	}
	public class ConnectorDragPreviewAdorner : ConnectorPreviewAdornerBase {
		static ConnectorDragPreviewAdorner() {
			DependencyPropertyRegistrator<ConnectorDragPreviewAdorner>.New()
				.OverrideDefaultStyleKey()
				;
		}
	}
	public class ConnectorMovePointPreviewAdorner : ConnectorPreviewAdornerBase, IConnectorMovePointAdorner {
		static ConnectorMovePointPreviewAdorner() {
			DependencyPropertyRegistrator<ConnectorMovePointPreviewAdorner>.New()
				.OverrideDefaultStyleKey()
				;
		}
	}
	[TemplatePart(Name = BeginFreeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = EndFreeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = BeginConnectedElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = EndConnectedElementName, Type = typeof(FrameworkElement))]
	public partial class ConnectorSelectionAdorner : Control<ConnectorSelectionAdorner>, IConnectorAdorner {
		const string BeginFreeElementName = "BeginFreeElement";
		const string EndFreeElementName = "EndFreeElement";
		const string BeginConnectedElementName = "BeginConnectedElement";
		const string EndConnectedElementName = "EndConnectedElement";
		void IConnectorAdorner.SetPoints(Point[] points) {
			Points = points.Select((x, i) => new ConnectorPointViewModel(i, connector, x)).ToArray();
		}
		static ConnectorSelectionAdorner() {
			DependencyPropertyRegistrator<ConnectorSelectionAdorner>.New()
				.Register(x => x.BeginPoint, out BeginPointProperty, default(Point))
				.Register(x => x.EndPoint, out EndPointProperty, default(Point))
				.Register(x => x.IsBeginPointConnected, out IsBeginPointConnectedProperty, false)
				.Register(x => x.IsEndPointConnected, out IsEndPointConnectedProperty, false)
				.Register(x => x.IsConnectorCurved, out IsConnectorCurvedProperty, false, (o, e) => o.InvalidateVisual())
				.Register(x => x.Points, out PointsProperty, default(ConnectorPointViewModel[]), (o, e) => o.InvalidateVisual())
			;
		}
		readonly IDiagramConnector connector;
		public ConnectorSelectionAdorner(IDiagramConnector connector) {
			this.connector = connector;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetInputElement(BeginFreeElementName, ConnectorPointType.Begin);
			SetInputElement(EndFreeElementName, ConnectorPointType.End);
			SetInputElement(BeginConnectedElementName, ConnectorPointType.Begin);
			SetInputElement(EndConnectedElementName, ConnectorPointType.End);
		}
		void SetInputElement(string elementName, ConnectorPointType pointType) {
			(GetTemplateChild(elementName) as FrameworkElement)
				.Do(x => DiagramInput.SetInputElementFactory(x, () => MoveConnectorPointInputElement.CreateMoveBeginEndPointElement(connector, pointType)));
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if(!IsConnectorCurved || Points.Count() < 2)
				return;
			var brush = new Pen(Brushes.DarkGray, 1);
			double zoomValue = AdornerLayer.GetZoom(this);
			drawingContext.DrawLine(brush, BeginPoint.ScalePoint(zoomValue), Points.First().Position.ScalePoint(zoomValue));
			drawingContext.DrawLine(brush, EndPoint.ScalePoint(zoomValue), Points.Last().Position.ScalePoint(zoomValue));
		}
	}
	public class ConnectorPointViewModel : ImmutableObject {
		public ConnectorPointViewModel(int index, IDiagramConnector connector, Point position) {
			Index = index;
			Connector = connector;
			Position = position;
		}
		public int Index { get; private set; }
		public IDiagramConnector Connector { get; private set; }
		public Point Position { get; private set; }
	}
	public class ConnectorPointItemsControl : ScaleItemsControl {
		protected override DependencyObject GetContainerForItemOverride() {
			return new ConnectorPointAdorner();
		}
	}
	public partial class ConnectorPointAdorner : Control<ConnectorPointAdorner> {
		static ConnectorPointAdorner() {
			DependencyPropertyRegistrator<ConnectorPointAdorner>.New()
				.Register(x => x.PointIndex, out PointIndexProperty, -1)
				.Register(x => x.Connector, out ConnectorProperty, default(IDiagramConnector))
				;
		}
		public ConnectorPointAdorner() {
			DiagramInput.SetInputElementFactory(this, () => Connector.Type == ConnectorType.RightAngle ? null :
													MoveConnectorPointInputElement.CreateMoveMiddlePointElement(Connector, PointIndex));
		}
	}
	public class GlueToItemAdorner : Control<GlueToItemAdorner> {
	}
	public class GlueToPointAdorner : Control<GlueToPointAdorner> {
	}
	public class ConnectionPointViewModel : ImmutableObject {
		public ConnectionPointViewModel(Point position, int index) {
			Index = index;
			Position = position;
		}
		public int Index { get; private set; }
		public Point Position { get; private set; }
	}
	public partial class ConnectionPointsAdorner : Control<ConnectionPointsAdorner>, IConnectionPointsAdorner {
		static ConnectionPointsAdorner() {
			DependencyPropertyRegistrator<ConnectionPointsAdorner>.New()
				.RegisterReadOnly(x => x.Points, out PointsPropertyKey, out PointsProperty, default(ConnectionPointViewModel[]))
				;
		}
		void IConnectionPointsAdorner.SetPoints(Point[] points) {
			Points = points.Select((x, i) => new ConnectionPointViewModel(x, i)).ToArray();
		}
	}
	public class PageBackgroundControl : Control<PageBackgroundControl> {
		public DiagramControl Diagram { get; private set; }
		public PageBackgroundControl(DiagramControl diagram) {
			this.Diagram = diagram;
		}
	}
	[TemplatePart(Name = PART_Editor, Type = typeof(FrameworkElement))]
	public class ItemEditorControl : Control<ItemEditorControl> {
		public const string PART_Editor = "Editor";
		TextBox editor;
		bool editorValueChanged;
		readonly DiagramItem item;
		readonly IDiagramItem textItem;
		readonly Action onDestroy;
#if DEBUGTEST
		internal TextBox EditorForTests { get { return editor; } }
#endif
		ICommandHandlers handlers;
		PropertyDescriptor Property { get { return textItem.Controller.GetTextProperty(); } }
		DiagramControl Diagram { get { return item.GetDiagram(); } }
		DiagramCommands Commands { get { return Diagram.Commands; } }
		public ItemEditorControl(IDiagramItem item, Action onDestroy) {
			this.item = ((DiagramItem)item);
			this.textItem = ((FontTraits)this.item.Controller.GetFontTraits()).TextItem;
			this.onDestroy = onDestroy;
			Func<bool> canExecuteSelectionCommand = () => !string.IsNullOrEmpty(editor.SelectedText);
			handlers = Commands.RegisterHandlers(registrator => {
				registrator.RegisterHandler(DiagramCommands.CancelCommand, Escape);
				registrator.RegisterHandler(DiagramCommands.EditCommand, Escape);
				registrator.RegisterHandler(DiagramCommands.DeleteCommand, () => editor.SelectedText = string.Empty, canExecuteSelectionCommand);
				registrator.RegisterHandler(DiagramCommands.CutCommand, () => editor.Cut(), canExecuteSelectionCommand);
				registrator.RegisterHandler(DiagramCommands.CopyCommand, () => editor.Copy(), canExecuteSelectionCommand);
				registrator.RegisterHandler(DiagramCommands.PasteCommand, () => editor.Paste());
				registrator.RegisterHandler(DiagramCommands.UndoCommand, () => editor.Undo(), () => editor.CanUndo);
				registrator.RegisterHandler(DiagramCommands.RedoCommand, () => editor.Redo(), () => editor.CanRedo);
			}, updateCommandsOnRequerySuggested: true);
			LostFocus += OnLostFocus;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			TranslateTransform transform = new TranslateTransform();
			if(double.IsNaN(Width) && double.IsNaN(Height)) {
				transform.X = -arrangeBounds.Width / 2;
				transform.Y = -arrangeBounds.Height / 2;
			}
			RenderTransform = transform;
			return base.ArrangeOverride(arrangeBounds);
		}
		void Escape() {
			LostFocus -= OnLostFocus;
			handlers.Destroy();
			onDestroy();
			if(editorValueChanged)
				Diagram.SetMultipleItemsPropertyValuesWithSelectionRestore<IDiagramItem>(item.Yield(), Property, editor.Text.Yield(), allowMerge: false, getComponent: x => x);
		}
		void OnLostFocus(object sender, RoutedEventArgs e) {
			Escape();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			editor = (TextBox)GetTemplateChild(PART_Editor);
			if(editor != null) {
				editor.Text = Convert.ToString(Property.GetValue(item));
				editor.TextChanged += (o, e) => editorValueChanged = true;
				new[] {
					new { Target = TextBox.VerticalContentAlignmentProperty, Source = GetBinding(x => x.VerticalContentAlignment) },
					new { Target = TextBox.TextAlignmentProperty, Source = GetBinding(x => x.TextAlignment) },
					new { Target = TextBox.FontFamilyProperty, Source = GetBinding(x => x.FontFamily) },
					new { Target = TextBox.FontStretchProperty, Source = GetBinding(x => x.FontStretch) },
					new { Target = TextBox.FontWeightProperty, Source = GetBinding(x => x.FontWeight) },
					new { Target = TextBox.FontStyleProperty, Source = GetBinding(x => x.FontStyle) },
					new { Target = TextBox.TextDecorationsProperty, Source = GetBinding(x => x.TextDecorations) },
				}.ForEach(x => editor.SetBinding(x.Target, x.Source));
				editor.SetBinding(FontSizeProperty, new MultiBinding() {
					Converter = new MultiplyConverter()
				}.Do(binding => {
					binding.Bindings.Add(GetBinding(x => x.FontSize));
					binding.Bindings.Add(new Binding() { Source = Diagram, Path = new PropertyPath(DiagramControl.ZoomFactorProperty) });
				}));
				editor.SelectAll();
				Keyboard.Focus(editor);
			}
		}
		Binding GetBinding<T>(Expression<Func<IDiagramItem, T>> expression) {
			return new Binding() { Source = textItem, Path = new PropertyPath(ExpressionHelper.GetPropertyName(expression)) };
		} 
		class MultiplyConverter : IMultiValueConverter {
			object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				return values.Select(x => Convert.ToDouble(x)).Aggregate(1d, (result, x) => result * x);
			}
			object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				throw new NotImplementedException();
			}
		}
	}
}
