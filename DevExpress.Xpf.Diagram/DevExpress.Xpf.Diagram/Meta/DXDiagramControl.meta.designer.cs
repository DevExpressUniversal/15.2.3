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
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramControl {
			public static readonly DependencyProperty MinDragDistanceProperty = 
			  DependencyProperty.Register("MinDragDistance", typeof(Double), typeof(DiagramControl), new PropertyMetadata(3d));
			public Double MinDragDistance {
				get { return (Double)GetValue(MinDragDistanceProperty); }
				set { SetValue(MinDragDistanceProperty, value); }
			}
			public static readonly DependencyProperty ActiveToolProperty = 
			  DependencyProperty.Register("ActiveTool", typeof(DiagramTool), typeof(DiagramControl), new PropertyMetadata(DiagramController.DefaultTool, (d, e) => ((DiagramControl)d).Controller.OnActiveToolChanged(), (d, o) => ((DiagramControl)d).Controller.CoerceTool((DiagramTool)o)));
			public DiagramTool ActiveTool {
				get { return (DiagramTool)GetValue(ActiveToolProperty); }
				set { SetValue(ActiveToolProperty, value); }
			}
			public static readonly DependencyProperty PageSizeProperty = 
			  DependencyProperty.Register("PageSize", typeof(Size), typeof(DiagramControl), new PropertyMetadata(new Size(800, 600), (d, e) => ((DiagramControl)d).Controller.OnExtentChanged()));
			public Size PageSize {
				get { return (Size)GetValue(PageSizeProperty); }
				set { SetValue(PageSizeProperty, value); }
			}
			public static readonly DependencyProperty ScrollMarginProperty = 
			  DependencyProperty.Register("ScrollMargin", typeof(Thickness), typeof(DiagramControl), new PropertyMetadata(new Thickness(20), (d, e) => ((DiagramControl)d).Controller.OnExtentChanged()));
			public Thickness ScrollMargin {
				get { return (Thickness)GetValue(ScrollMarginProperty); }
				set { SetValue(ScrollMarginProperty, value); }
			}
			public static readonly DependencyProperty BringIntoViewMarginProperty = 
			  DependencyProperty.Register("BringIntoViewMargin", typeof(Double), typeof(DiagramControl), new PropertyMetadata(10d));
			public Double BringIntoViewMargin {
				get { return (Double)GetValue(BringIntoViewMarginProperty); }
				set { SetValue(BringIntoViewMarginProperty, value); }
			}
			public static readonly DependencyProperty ZoomFactorProperty = 
			  DependencyProperty.Register("ZoomFactor", typeof(Double), typeof(DiagramControl), new PropertyMetadata(1d, (d, e) => ((DiagramControl)d).Controller.OnZoomFactorChanged(), (d, o) => ((DiagramControl)d).Controller.CoerceZoom((Double)o)));
			public Double ZoomFactor {
				get { return (Double)GetValue(ZoomFactorProperty); }
				set { SetValue(ZoomFactorProperty, value); }
			}
			public static readonly DependencyProperty GridSizeProperty = 
			  DependencyProperty.Register("GridSize", typeof(Nullable<Size>), typeof(DiagramControl), new PropertyMetadata(default(Nullable<Size>), (d, e) => ((DiagramControl)d).OnGridSizeChanged()));
			public Nullable<Size> GridSize {
				get { return (Nullable<Size>)GetValue(GridSizeProperty); }
				set { SetValue(GridSizeProperty, value); }
			}
			public static readonly DependencyProperty ResizingModeProperty = 
			  DependencyProperty.Register("ResizingMode", typeof(ResizingMode), typeof(DiagramControl), new PropertyMetadata(default(ResizingMode)));
			public ResizingMode ResizingMode {
				get { return (ResizingMode)GetValue(ResizingModeProperty); }
				set { SetValue(ResizingModeProperty, value); }
			}
			public static readonly DependencyProperty SnapToGridProperty = 
			  DependencyProperty.Register("SnapToGrid", typeof(Boolean), typeof(DiagramControl), new PropertyMetadata(true));
			public Boolean SnapToGrid {
				get { return (Boolean)GetValue(SnapToGridProperty); }
				set { SetValue(SnapToGridProperty, value); }
			}
			public static readonly DependencyProperty SnapToItemsProperty = 
			  DependencyProperty.Register("SnapToItems", typeof(Boolean), typeof(DiagramControl), new PropertyMetadata(true));
			public Boolean SnapToItems {
				get { return (Boolean)GetValue(SnapToItemsProperty); }
				set { SetValue(SnapToItemsProperty, value); }
			}
			public static readonly DependencyProperty SnapToItemsDistanceProperty = 
			  DependencyProperty.Register("SnapToItemsDistance", typeof(Double), typeof(DiagramControl), new PropertyMetadata(10d));
			public Double SnapToItemsDistance {
				get { return (Double)GetValue(SnapToItemsDistanceProperty); }
				set { SetValue(SnapToItemsDistanceProperty, value); }
			}
			public static readonly DependencyProperty GlueToConnectionPointDistanceProperty = 
			  DependencyProperty.Register("GlueToConnectionPointDistance", typeof(Double), typeof(DiagramControl), new PropertyMetadata(10d));
			public Double GlueToConnectionPointDistance {
				get { return (Double)GetValue(GlueToConnectionPointDistanceProperty); }
				set { SetValue(GlueToConnectionPointDistanceProperty, value); }
			}
			public static readonly DependencyProperty GlueToItemDistanceProperty = 
			  DependencyProperty.Register("GlueToItemDistance", typeof(Double), typeof(DiagramControl), new PropertyMetadata(7d));
			public Double GlueToItemDistance {
				get { return (Double)GetValue(GlueToItemDistanceProperty); }
				set { SetValue(GlueToItemDistanceProperty, value); }
			}
			public static readonly DependencyProperty MeasureUnitProperty = 
			  DependencyProperty.Register("MeasureUnit", typeof(MeasureUnit), typeof(DiagramControl), new PropertyMetadata(MeasureUnits.Pixels));
			public MeasureUnit MeasureUnit {
				get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
				set { SetValue(MeasureUnitProperty, value); }
			}
			public static readonly DependencyProperty AllowEmptySelectionProperty = 
			  DependencyProperty.Register("AllowEmptySelection", typeof(Boolean), typeof(DiagramControl), new PropertyMetadata(true, (d, e) => ((DiagramControl)d).Selection().ValidateSelection()));
			public Boolean AllowEmptySelection {
				get { return (Boolean)GetValue(AllowEmptySelectionProperty); }
				set { SetValue(AllowEmptySelectionProperty, value); }
			}
			public static readonly DependencyProperty ThemeProperty = 
			  DependencyProperty.Register("Theme", typeof(DiagramTheme), typeof(DiagramControl), new PropertyMetadata(DiagramThemes.Office, (d, e) => ((DiagramControl)d).Controller.OnThemeChanged()));
			public DiagramTheme Theme {
				get { return (DiagramTheme)GetValue(ThemeProperty); }
				set { SetValue(ThemeProperty, value); }
			}
			public static readonly DependencyProperty CanvasSizeModeProperty = 
			  DependencyProperty.Register("CanvasSizeMode", typeof(CanvasSizeMode), typeof(DiagramControl), new PropertyMetadata(CanvasSizeMode.AutoSize, (d, e) => ((DiagramControl)d).Controller.OnCanvasSizeModeChanged()));
			public CanvasSizeMode CanvasSizeMode {
				get { return (CanvasSizeMode)GetValue(CanvasSizeModeProperty); }
				set { SetValue(CanvasSizeModeProperty, value); }
			}
			static readonly DependencyPropertyKey RootToolsModelPropertyKey = 
			  DependencyProperty.RegisterReadOnly("RootToolsModel", typeof(SelectionToolsModel<IDiagramItem>), typeof(DiagramControl), new PropertyMetadata(default(SelectionToolsModel<IDiagramItem>)));
			public static readonly DependencyProperty RootToolsModelProperty = RootToolsModelPropertyKey.DependencyProperty;
			public SelectionToolsModel<IDiagramItem> RootToolsModel {
				get { return (SelectionToolsModel<IDiagramItem>)GetValue(RootToolsModelProperty); }
				private set { SetValue(RootToolsModelPropertyKey, value); }
			}
			static readonly DependencyPropertyKey SelectionModelPropertyKey = 
			  DependencyProperty.RegisterReadOnly("SelectionModel", typeof(SelectionModel<IDiagramItem>), typeof(DiagramControl), new PropertyMetadata(default(SelectionModel<IDiagramItem>)));
			public static readonly DependencyProperty SelectionModelProperty = SelectionModelPropertyKey.DependencyProperty;
			public SelectionModel<IDiagramItem> SelectionModel {
				get { return (SelectionModel<IDiagramItem>)GetValue(SelectionModelProperty); }
				private set { SetValue(SelectionModelPropertyKey, value); }
			}
			static readonly DependencyPropertyKey SelectionToolsModelPropertyKey = 
			  DependencyProperty.RegisterReadOnly("SelectionToolsModel", typeof(SelectionToolsModel<IDiagramItem>), typeof(DiagramControl), new PropertyMetadata(default(SelectionToolsModel<IDiagramItem>)));
			public static readonly DependencyProperty SelectionToolsModelProperty = SelectionToolsModelPropertyKey.DependencyProperty;
			public SelectionToolsModel<IDiagramItem> SelectionToolsModel {
				get { return (SelectionToolsModel<IDiagramItem>)GetValue(SelectionToolsModelProperty); }
				private set { SetValue(SelectionToolsModelPropertyKey, value); }
			}
			static readonly DependencyPropertyKey CanUndoPropertyKey = 
			  DependencyProperty.RegisterReadOnly("CanUndo", typeof(bool), typeof(DiagramControl), new PropertyMetadata(default(bool)));
			public static readonly DependencyProperty CanUndoProperty = CanUndoPropertyKey.DependencyProperty;
			public bool CanUndo {
				get { return (bool)GetValue(CanUndoProperty); }
				private set { SetValue(CanUndoPropertyKey, value); }
			}
			static readonly DependencyPropertyKey CanRedoPropertyKey = 
			  DependencyProperty.RegisterReadOnly("CanRedo", typeof(bool), typeof(DiagramControl), new PropertyMetadata(default(bool)));
			public static readonly DependencyProperty CanRedoProperty = CanRedoPropertyKey.DependencyProperty;
			public bool CanRedo {
				get { return (bool)GetValue(CanRedoProperty); }
				private set { SetValue(CanRedoPropertyKey, value); }
			}
			static readonly DependencyPropertyKey HasChangesPropertyKey = 
			  DependencyProperty.RegisterReadOnly("HasChanges", typeof(bool), typeof(DiagramControl), new PropertyMetadata(default(bool)));
			public static readonly DependencyProperty HasChangesProperty = HasChangesPropertyKey.DependencyProperty;
			public bool HasChanges {
				get { return (bool)GetValue(HasChangesProperty); }
				protected set { SetValue(HasChangesPropertyKey, value); }
			}
		}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramItem {
			public static readonly DependencyProperty PositionProperty = 
			  DependencyProperty.Register("Position", typeof(Point), typeof(DiagramItem), new PropertyMetadata(default(Point), (d, e) => ((DiagramItem)d).OnPositionChanged(((Point)e.OldValue))));
			public Point Position {
				get { return (Point)GetValue(PositionProperty); }
				set { SetValue(PositionProperty, value); }
			}
			public static readonly DependencyProperty WeightProperty = 
			  DependencyProperty.Register("Weight", typeof(Double), typeof(DiagramItem), new PropertyMetadata(1d, (d, e) => ((DiagramItem)d).NotifyChanged(ItemChangedKind.Bounds), (d, o) => ((DiagramItem)d).CoerceWeight((Double)o)));
			public Double Weight {
				get { return (Double)GetValue(WeightProperty); }
				set { SetValue(WeightProperty, value); }
			}
			public static readonly DependencyProperty CanDeleteProperty = 
			  DependencyProperty.Register("CanDelete", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true, (d, e) => ((DiagramItem)d).NotifyInteractionChanged()));
			public Boolean CanDelete {
				get { return (Boolean)GetValue(CanDeleteProperty); }
				set { SetValue(CanDeleteProperty, value); }
			}
			public static readonly DependencyProperty CanResizeProperty = 
			  DependencyProperty.Register("CanResize", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true, (d, e) => ((DiagramItem)d).NotifyInteractionChanged()));
			public Boolean CanResize {
				get { return (Boolean)GetValue(CanResizeProperty); }
				set { SetValue(CanResizeProperty, value); }
			}
			public static readonly DependencyProperty CanMoveProperty = 
			  DependencyProperty.Register("CanMove", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true));
			public Boolean CanMove {
				get { return (Boolean)GetValue(CanMoveProperty); }
				set { SetValue(CanMoveProperty, value); }
			}
			public static readonly DependencyProperty CanCopyProperty = 
			  DependencyProperty.Register("CanCopy", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true, (d, e) => ((DiagramItem)d).NotifyInteractionChanged()));
			public Boolean CanCopy {
				get { return (Boolean)GetValue(CanCopyProperty); }
				set { SetValue(CanCopyProperty, value); }
			}
			public static readonly DependencyProperty CanRotateProperty = 
			  DependencyProperty.Register("CanRotate", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true, (d, e) => ((DiagramItem)d).NotifyInteractionChanged()));
			public Boolean CanRotate {
				get { return (Boolean)GetValue(CanRotateProperty); }
				set { SetValue(CanRotateProperty, value); }
			}
			public static readonly DependencyProperty CanSnapToThisItemProperty = 
			  DependencyProperty.Register("CanSnapToThisItem", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true));
			public Boolean CanSnapToThisItem {
				get { return (Boolean)GetValue(CanSnapToThisItemProperty); }
				set { SetValue(CanSnapToThisItemProperty, value); }
			}
			public static readonly DependencyProperty CanSnapToOtherItemsProperty = 
			  DependencyProperty.Register("CanSnapToOtherItems", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true));
			public Boolean CanSnapToOtherItems {
				get { return (Boolean)GetValue(CanSnapToOtherItemsProperty); }
				set { SetValue(CanSnapToOtherItemsProperty, value); }
			}
			public static readonly DependencyProperty CanSelectProperty = 
			  DependencyProperty.Register("CanSelect", typeof(Boolean), typeof(DiagramItem), new PropertyMetadata(true));
			public Boolean CanSelect {
				get { return (Boolean)GetValue(CanSelectProperty); }
				set { SetValue(CanSelectProperty, value); }
			}
			public static readonly DependencyProperty AnchorsProperty = 
			  DependencyProperty.Register("Anchors", typeof(Sides), typeof(DiagramItem), new PropertyMetadata(default(Sides)));
			public Sides Anchors {
				get { return (Sides)GetValue(AnchorsProperty); }
				set { SetValue(AnchorsProperty, value); }
			}
			public static readonly DependencyProperty SelectionLayerProperty = 
			  DependencyProperty.Register("SelectionLayer", typeof(ISelectionLayer), typeof(DiagramItem), new PropertyMetadata(DefaultSelectionLayer.Instance, (d, e) => ((DiagramItem)d).OnSelectionLayerChanged(), (d, o) => ((DiagramItem)d).Controller.CoerceSelectionLayer((ISelectionLayer)o)));
			public ISelectionLayer SelectionLayer {
				get { return (ISelectionLayer)GetValue(SelectionLayerProperty); }
				set { SetValue(SelectionLayerProperty, value); }
			}
			public static readonly DependencyProperty StrokeProperty = 
			  DependencyProperty.Register("Stroke", typeof(Brush), typeof(DiagramItem), new PropertyMetadata(default(Brush), (d, e) => ((DiagramItem)d).InvalidateVisual()));
			public Brush Stroke {
				get { return (Brush)GetValue(StrokeProperty); }
				set { SetValue(StrokeProperty, value); }
			}
			public static readonly DependencyProperty StrokeThicknessProperty = 
			  DependencyProperty.Register("StrokeThickness", typeof(Double), typeof(DiagramItem), new PropertyMetadata(1d, (d, e) => ((DiagramItem)d).InvalidateVisual()));
			public Double StrokeThickness {
				get { return (Double)GetValue(StrokeThicknessProperty); }
				set { SetValue(StrokeThicknessProperty, value); }
			}
			public static readonly DependencyProperty StrokeDashArrayProperty = 
			  DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(DiagramItem), new PropertyMetadata(default(DoubleCollection), (d, e) => ((DiagramItem)d).InvalidateVisual()));
			public DoubleCollection StrokeDashArray {
				get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
				set { SetValue(StrokeDashArrayProperty, value); }
			}
			public static readonly DependencyProperty ThemeStyleIdProperty = 
			  DependencyProperty.Register("ThemeStyleId", typeof(DiagramItemStyleId), typeof(DiagramItem), new PropertyMetadata(DiagramItemStyleId.DefaultStyleId, (d, e) => ((DiagramItem)d).Controller.UpdateCustomStyle()));
			public DiagramItemStyleId ThemeStyleId {
				get { return (DiagramItemStyleId)GetValue(ThemeStyleIdProperty); }
				set { SetValue(ThemeStyleIdProperty, value); }
			}
			public static readonly DependencyProperty CustomStyleIdProperty = 
			  DependencyProperty.Register("CustomStyleId", typeof(Object), typeof(DiagramItem), new PropertyMetadata(default(Object), (d, e) => ((DiagramItem)d).Controller.UpdateCustomStyle()));
			public Object CustomStyleId {
				get { return (Object)GetValue(CustomStyleIdProperty); }
				set { SetValue(CustomStyleIdProperty, value); }
			}
			public static readonly DependencyProperty ForegroundIdProperty = 
			  DependencyProperty.Register("ForegroundId", typeof(Nullable<DiagramThemeColorId>), typeof(DiagramItem), new PropertyMetadata(default(Nullable<DiagramThemeColorId>), (d, e) => ((DiagramItem)d).Controller.UpdateCustomStyle()));
			public Nullable<DiagramThemeColorId> ForegroundId {
				get { return (Nullable<DiagramThemeColorId>)GetValue(ForegroundIdProperty); }
				set { SetValue(ForegroundIdProperty, value); }
			}
			public static readonly DependencyProperty BackgroundIdProperty = 
			  DependencyProperty.Register("BackgroundId", typeof(Nullable<DiagramThemeColorId>), typeof(DiagramItem), new PropertyMetadata(default(Nullable<DiagramThemeColorId>), (d, e) => ((DiagramItem)d).Controller.UpdateCustomStyle()));
			public Nullable<DiagramThemeColorId> BackgroundId {
				get { return (Nullable<DiagramThemeColorId>)GetValue(BackgroundIdProperty); }
				set { SetValue(BackgroundIdProperty, value); }
			}
			public static readonly DependencyProperty StrokeIdProperty = 
			  DependencyProperty.Register("StrokeId", typeof(Nullable<DiagramThemeColorId>), typeof(DiagramItem), new PropertyMetadata(default(Nullable<DiagramThemeColorId>), (d, e) => ((DiagramItem)d).Controller.UpdateCustomStyle()));
			public Nullable<DiagramThemeColorId> StrokeId {
				get { return (Nullable<DiagramThemeColorId>)GetValue(StrokeIdProperty); }
				set { SetValue(StrokeIdProperty, value); }
			}
			public static readonly DependencyProperty AngleProperty = 
			  DependencyProperty.Register("Angle", typeof(Double), typeof(DiagramItem), new PropertyMetadata(default(Double), (d, e) => ((DiagramItem)d).OnAngleChanged(), (d, o) => ((DiagramItem)d).CoerceAngle((Double)o)));
			public Double Angle {
				get { return (Double)GetValue(AngleProperty); }
				set { SetValue(AngleProperty, value); }
			}
			static readonly DependencyPropertyKey IsSelectedPropertyKey = 
			  DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(DiagramItem), new PropertyMetadata(default(bool)));
			public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
			public bool IsSelected {
				get { return (bool)GetValue(IsSelectedProperty); }
				private set { SetValue(IsSelectedPropertyKey, value); }
			}
		}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramShape {
			public static readonly DependencyProperty ShapeProperty = 
			  DependencyProperty.Register("Shape", typeof(ShapeDescription), typeof(DiagramShape), new PropertyMetadata(BasicShapes.Rectangle, (d, e) => ((DiagramShape)d).OnShapeChanged(((ShapeDescription)e.OldValue))));
			[PropertyGridEditor(TemplateKey = "propertyGridComboboxEdit")]
			public ShapeDescription Shape {
				get { return (ShapeDescription)GetValue(ShapeProperty); }
				set { SetValue(ShapeProperty, value); }
			}
			public static readonly DependencyProperty ContentProperty = 
			  DependencyProperty.Register("Content", typeof(String), typeof(DiagramShape), new PropertyMetadata(default(String), (d, e) => ((DiagramShape)d).Update()));
			public String Content {
				get { return (String)GetValue(ContentProperty); }
				set { SetValue(ContentProperty, value); }
			}
			public static readonly DependencyProperty ParametersProperty = 
			  DependencyProperty.Register("Parameters", typeof(DoubleCollection), typeof(DiagramShape), new PropertyMetadata(default(DoubleCollection), (d, e) => ((DiagramShape)d).UpdateShape()));
			public DoubleCollection Parameters {
				get { return (DoubleCollection)GetValue(ParametersProperty); }
				set { SetValue(ParametersProperty, value); }
			}
		}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramConnector {
			public static readonly DependencyProperty BeginArrowProperty = 
			  DependencyProperty.Register("BeginArrow", typeof(ArrowDescription), typeof(DiagramConnector), new PropertyMetadata(default(ArrowDescription), (d, e) => ((DiagramConnector)d).Controller().OnAppearancePropertyChanged()));
			[PropertyGridEditor(TemplateKey = "propertyGridComboboxEdit")]
			public ArrowDescription BeginArrow {
				get { return (ArrowDescription)GetValue(BeginArrowProperty); }
				set { SetValue(BeginArrowProperty, value); }
			}
			public static readonly DependencyProperty EndArrowProperty = 
			  DependencyProperty.Register("EndArrow", typeof(ArrowDescription), typeof(DiagramConnector), new PropertyMetadata(default(ArrowDescription), (d, e) => ((DiagramConnector)d).Controller().OnAppearancePropertyChanged()));
			[PropertyGridEditor(TemplateKey = "propertyGridComboboxEdit")]
			public ArrowDescription EndArrow {
				get { return (ArrowDescription)GetValue(EndArrowProperty); }
				set { SetValue(EndArrowProperty, value); }
			}
			public static readonly DependencyProperty BeginArrowSizeProperty = 
			  DependencyProperty.Register("BeginArrowSize", typeof(Size), typeof(DiagramConnector), new PropertyMetadata(default(Size), (d, e) => ((DiagramConnector)d).Controller().OnAppearancePropertyChanged()));
			public Size BeginArrowSize {
				get { return (Size)GetValue(BeginArrowSizeProperty); }
				set { SetValue(BeginArrowSizeProperty, value); }
			}
			public static readonly DependencyProperty EndArrowSizeProperty = 
			  DependencyProperty.Register("EndArrowSize", typeof(Size), typeof(DiagramConnector), new PropertyMetadata(default(Size), (d, e) => ((DiagramConnector)d).Controller().OnAppearancePropertyChanged()));
			public Size EndArrowSize {
				get { return (Size)GetValue(EndArrowSizeProperty); }
				set { SetValue(EndArrowSizeProperty, value); }
			}
			public static readonly DependencyProperty TypeProperty = 
			  DependencyProperty.Register("Type", typeof(ConnectorType), typeof(DiagramConnector), new PropertyMetadata(ConnectorType.RightAngle, (d, e) => ((DiagramConnector)d).Controller().OnTypeChanged()));
			[PropertyGridEditor(TemplateKey = "propertyGridComboboxEdit")]
			public ConnectorType Type {
				get { return (ConnectorType)GetValue(TypeProperty); }
				set { SetValue(TypeProperty, value); }
			}
			public static readonly DependencyProperty TextProperty = 
			  DependencyProperty.Register("Text", typeof(String), typeof(DiagramConnector), new PropertyMetadata(default(String)));
			public String Text {
				get { return (String)GetValue(TextProperty); }
				set { SetValue(TextProperty, value); }
			}
			public static readonly DependencyProperty BeginPointProperty = 
			  DependencyProperty.Register("BeginPoint", typeof(Point), typeof(DiagramConnector), new PropertyMetadata(default(Point), (d, e) => ((DiagramConnector)d).Controller().OnPointChanged(ConnectorPointType.Begin)));
			public Point BeginPoint {
				get { return (Point)GetValue(BeginPointProperty); }
				set { SetValue(BeginPointProperty, value); }
			}
			public static readonly DependencyProperty EndPointProperty = 
			  DependencyProperty.Register("EndPoint", typeof(Point), typeof(DiagramConnector), new PropertyMetadata(default(Point), (d, e) => ((DiagramConnector)d).Controller().OnPointChanged(ConnectorPointType.End)));
			public Point EndPoint {
				get { return (Point)GetValue(EndPointProperty); }
				set { SetValue(EndPointProperty, value); }
			}
			public static readonly DependencyProperty BeginItemProperty = 
			  DependencyProperty.Register("BeginItem", typeof(IDiagramItem), typeof(DiagramConnector), new PropertyMetadata(default(IDiagramItem), (d, e) => ((DiagramConnector)d).Controller().OnItemChanged(ConnectorPointType.Begin)));
			public IDiagramItem BeginItem {
				get { return (IDiagramItem)GetValue(BeginItemProperty); }
				set { SetValue(BeginItemProperty, value); }
			}
			public static readonly DependencyProperty EndItemProperty = 
			  DependencyProperty.Register("EndItem", typeof(IDiagramItem), typeof(DiagramConnector), new PropertyMetadata(default(IDiagramItem), (d, e) => ((DiagramConnector)d).Controller().OnItemChanged(ConnectorPointType.End)));
			public IDiagramItem EndItem {
				get { return (IDiagramItem)GetValue(EndItemProperty); }
				set { SetValue(EndItemProperty, value); }
			}
			public static readonly DependencyProperty BeginItemPointIndexProperty = 
			  DependencyProperty.Register("BeginItemPointIndex", typeof(Int32), typeof(DiagramConnector), new PropertyMetadata(-1, (d, e) => ((DiagramConnector)d).Controller().OnItemPointIndexChanged(ConnectorPointType.Begin)));
			public Int32 BeginItemPointIndex {
				get { return (Int32)GetValue(BeginItemPointIndexProperty); }
				set { SetValue(BeginItemPointIndexProperty, value); }
			}
			public static readonly DependencyProperty EndItemPointIndexProperty = 
			  DependencyProperty.Register("EndItemPointIndex", typeof(Int32), typeof(DiagramConnector), new PropertyMetadata(-1, (d, e) => ((DiagramConnector)d).Controller().OnItemPointIndexChanged(ConnectorPointType.End)));
			public Int32 EndItemPointIndex {
				get { return (Int32)GetValue(EndItemPointIndexProperty); }
				set { SetValue(EndItemPointIndexProperty, value); }
			}
			public static readonly DependencyProperty PointsProperty = 
			  DependencyProperty.Register("Points", typeof(ConnectorPointsCollection), typeof(DiagramConnector), new PropertyMetadata(default(ConnectorPointsCollection), (d, e) => ((DiagramConnector)d).Controller().OnPointsChanged()));
			public ConnectorPointsCollection Points {
				get { return (ConnectorPointsCollection)GetValue(PointsProperty); }
				set { SetValue(PointsProperty, value); }
			}
		}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramContainer {
			public static readonly DependencyProperty IsSnapScopeProperty = 
			  DependencyProperty.Register("IsSnapScope", typeof(Boolean), typeof(DiagramContainer), new PropertyMetadata(default(Boolean)));
			public Boolean IsSnapScope {
				get { return (Boolean)GetValue(IsSnapScopeProperty); }
				set { SetValue(IsSnapScopeProperty, value); }
			}
			public static readonly DependencyProperty AdjustBoundsBehaviorProperty = 
			  DependencyProperty.Register("AdjustBoundsBehavior", typeof(AdjustBoundaryBehavior), typeof(DiagramContainer), new PropertyMetadata(AdjustBoundaryBehavior.AutoAdjust));
			public AdjustBoundaryBehavior AdjustBoundsBehavior {
				get { return (AdjustBoundaryBehavior)GetValue(AdjustBoundsBehaviorProperty); }
				set { SetValue(AdjustBoundsBehaviorProperty, value); }
			}
		}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.DataAnnotations;
		partial class DiagramOrgChartBehavior {
			public static readonly DependencyProperty ChildrenPathProperty = 
			  DependencyProperty.Register("ChildrenPath", typeof(String), typeof(DiagramOrgChartBehavior), new PropertyMetadata(default(String)));
			public String ChildrenPath {
				get { return (String)GetValue(ChildrenPathProperty); }
				set { SetValue(ChildrenPathProperty, value); }
			}
			public static readonly DependencyProperty ChildrenSelectorProperty = 
			  DependencyProperty.Register("ChildrenSelector", typeof(IChildrenSelector), typeof(DiagramOrgChartBehavior), new PropertyMetadata(default(IChildrenSelector)));
			public IChildrenSelector ChildrenSelector {
				get { return (IChildrenSelector)GetValue(ChildrenSelectorProperty); }
				set { SetValue(ChildrenSelectorProperty, value); }
			}
			public static readonly DependencyProperty ItemsSourceProperty = 
			  DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DiagramOrgChartBehavior), new PropertyMetadata(default(IEnumerable), (d, e) => ((DiagramOrgChartBehavior)d).controller.OnItemsSourceChanged()));
			public IEnumerable ItemsSource {
				get { return (IEnumerable)GetValue(ItemsSourceProperty); }
				set { SetValue(ItemsSourceProperty, value); }
			}
			public static readonly DependencyProperty KeyMemberProperty = 
			  DependencyProperty.Register("KeyMember", typeof(String), typeof(DiagramOrgChartBehavior), new PropertyMetadata(default(String)));
			public String KeyMember {
				get { return (String)GetValue(KeyMemberProperty); }
				set { SetValue(KeyMemberProperty, value); }
			}
			public static readonly DependencyProperty ParentMemberProperty = 
			  DependencyProperty.Register("ParentMember", typeof(String), typeof(DiagramOrgChartBehavior), new PropertyMetadata(default(String)));
			public String ParentMember {
				get { return (String)GetValue(ParentMemberProperty); }
				set { SetValue(ParentMemberProperty, value); }
			}
		}
}
