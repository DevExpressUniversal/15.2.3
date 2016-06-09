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

namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
		partial class DiagramControl {
			CanvasSizeMode _CanvasSizeMode = CanvasSizeMode.AutoSize;
			public CanvasSizeMode CanvasSizeMode {
				get { return _CanvasSizeMode; }
				set {
					if(CanvasSizeMode == value) return;
					_CanvasSizeMode = value;
					this.Controller.OnCanvasSizeModeChanged();
					OnPropertiesChanged();
				}
			}
			SelectionToolsModel<IDiagramItem> _RootToolsModel = default(SelectionToolsModel<IDiagramItem>);
			[Browsable(false)]
			public SelectionToolsModel<IDiagramItem> RootToolsModel {
				get { return _RootToolsModel; }
				private set {
					if(RootToolsModel == value) return;
					_RootToolsModel = value;
				}
			}
			SelectionModel<IDiagramItem> _SelectionModel = default(SelectionModel<IDiagramItem>);
			[Browsable(false)]
			public SelectionModel<IDiagramItem> SelectionModel {
				get { return _SelectionModel; }
				private set {
					if(SelectionModel == value) return;
					_SelectionModel = value;
				}
			}
			SelectionToolsModel<IDiagramItem> _SelectionToolsModel = default(SelectionToolsModel<IDiagramItem>);
			[Browsable(false)]
			public SelectionToolsModel<IDiagramItem> SelectionToolsModel {
				get { return _SelectionToolsModel; }
				private set {
					if(SelectionToolsModel == value) return;
					_SelectionToolsModel = value;
				}
			}
			bool _CanUndo = default(bool);
			[Browsable(false)]
			public bool CanUndo {
				get { return _CanUndo; }
				private set {
					if(CanUndo == value) return;
					_CanUndo = value;
				}
			}
			bool _CanRedo = default(bool);
			[Browsable(false)]
			public bool CanRedo {
				get { return _CanRedo; }
				private set {
					if(CanRedo == value) return;
					_CanRedo = value;
				}
			}
		}
}
namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
		partial class DiagramItem {
			Point _Position = default(Point);
			[DXCategory(CategoryName.Layout)]
			[TypeConverter(typeof(PointFloatConverter))]
			public virtual Point Position {
				get { return _Position; }
				set {
					if(Position == value) return;
					var oldValue = Position;
					_Position = value;
					this.OnPositionChanged(oldValue);
					OnPropertiesChanged();
				}
			}
			Boolean _CanDelete = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanDelete {
				get { return _CanDelete; }
				set {
					if(CanDelete == value) return;
					_CanDelete = value;
					this.NotifyInteractionChanged();
					OnPropertiesChanged();
				}
			}
			Boolean _CanResize = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanResize {
				get { return _CanResize; }
				set {
					if(CanResize == value) return;
					_CanResize = value;
					this.NotifyInteractionChanged();
					OnPropertiesChanged();
				}
			}
			Boolean _CanMove = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanMove {
				get { return _CanMove; }
				set {
					if(CanMove == value) return;
					_CanMove = value;
					OnPropertiesChanged();
				}
			}
			Boolean _CanCopy = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanCopy {
				get { return _CanCopy; }
				set {
					if(CanCopy == value) return;
					_CanCopy = value;
					this.NotifyInteractionChanged();
					OnPropertiesChanged();
				}
			}
			Boolean _CanRotate = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanRotate {
				get { return _CanRotate; }
				set {
					if(CanRotate == value) return;
					_CanRotate = value;
					this.NotifyInteractionChanged();
					OnPropertiesChanged();
				}
			}
			Boolean _CanSnapToThisItem = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanSnapToThisItem {
				get { return _CanSnapToThisItem; }
				set {
					if(CanSnapToThisItem == value) return;
					_CanSnapToThisItem = value;
					OnPropertiesChanged();
				}
			}
			Boolean _CanSnapToOtherItems = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanSnapToOtherItems {
				get { return _CanSnapToOtherItems; }
				set {
					if(CanSnapToOtherItems == value) return;
					_CanSnapToOtherItems = value;
					OnPropertiesChanged();
				}
			}
			Boolean _CanSelect = true;
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean CanSelect {
				get { return _CanSelect; }
				set {
					if(CanSelect == value) return;
					_CanSelect = value;
					OnPropertiesChanged();
				}
			}
			Sides _Anchors = default(Sides);
			[DXCategory(CategoryName.Layout)]
			public virtual Sides Anchors {
				get { return _Anchors; }
				set {
					if(Anchors == value) return;
					_Anchors = value;
					OnPropertiesChanged();
				}
			}
			ISelectionLayer _SelectionLayer = DefaultSelectionLayer.Instance;
			[Browsable(false)]
			public ISelectionLayer SelectionLayer {
				get { return this.Controller.CoerceSelectionLayer(_SelectionLayer); }
				set {
					if(SelectionLayer == value) return;
					_SelectionLayer = value;
					this.OnSelectionLayerChanged();
					OnPropertiesChanged();
				}
			}
			Brush _Stroke = default(Brush);
			[Browsable(false)]
			public Brush Stroke {
				get { return _Stroke; }
				set {
					if(Stroke == value) return;
					_Stroke = value;
					this.InvalidateVisual();
					OnPropertiesChanged();
				}
			}
			Double _StrokeThickness = 1d;
			[Browsable(false)]
			public Double StrokeThickness {
				get { return _StrokeThickness; }
				set {
					if(StrokeThickness == value) return;
					_StrokeThickness = value;
					this.InvalidateVisual();
					OnPropertiesChanged();
				}
			}
			DoubleCollection _StrokeDashArray = default(DoubleCollection);
			[DefaultValue(null)]
			[Browsable(false)]
			public DoubleCollection StrokeDashArray {
				get { return _StrokeDashArray; }
				set {
					if(StrokeDashArray == value) return;
					_StrokeDashArray = value;
					this.InvalidateVisual();
					OnPropertiesChanged();
				}
			}
			DiagramItemStyleId _ThemeStyleId = DiagramItemStyleId.DefaultStyleId;
			[Browsable(false)]
			public DiagramItemStyleId ThemeStyleId {
				get { return _ThemeStyleId; }
				set {
					if(ThemeStyleId == value) return;
					_ThemeStyleId = value;
					this.Controller.UpdateCustomStyle();
					OnPropertiesChanged();
				}
			}
			Object _CustomStyleId = default(Object);
			[DefaultValue(null)]
			[Browsable(false)]
			public Object CustomStyleId {
				get { return _CustomStyleId; }
				set {
					if(CustomStyleId == value) return;
					_CustomStyleId = value;
					this.Controller.UpdateCustomStyle();
					OnPropertiesChanged();
				}
			}
			Nullable<DiagramThemeColorId> _ForegroundId = default(Nullable<DiagramThemeColorId>);
			[DefaultValue(null)]
			[Browsable(false)]
			public Nullable<DiagramThemeColorId> ForegroundId {
				get { return _ForegroundId; }
				set {
					if(ForegroundId == value) return;
					_ForegroundId = value;
					this.Controller.UpdateCustomStyle();
					OnPropertiesChanged();
				}
			}
			Nullable<DiagramThemeColorId> _BackgroundId = default(Nullable<DiagramThemeColorId>);
			[DefaultValue(null)]
			[Browsable(false)]
			public Nullable<DiagramThemeColorId> BackgroundId {
				get { return _BackgroundId; }
				set {
					if(BackgroundId == value) return;
					_BackgroundId = value;
					this.Controller.UpdateCustomStyle();
					OnPropertiesChanged();
				}
			}
			Nullable<DiagramThemeColorId> _StrokeId = default(Nullable<DiagramThemeColorId>);
			[DefaultValue(null)]
			[Browsable(false)]
			public Nullable<DiagramThemeColorId> StrokeId {
				get { return _StrokeId; }
				set {
					if(StrokeId == value) return;
					_StrokeId = value;
					this.Controller.UpdateCustomStyle();
					OnPropertiesChanged();
				}
			}
			Double _Angle = default(Double);
			[DXCategory(CategoryName.Appearance)]
			public virtual Double Angle {
				get { return this.CoerceAngle(_Angle); }
				set {
					if(Angle == value) return;
					_Angle = value;
					this.OnAngleChanged();
					OnPropertiesChanged();
				}
			}
			bool _IsSelected = default(bool);
			[Browsable(false)]
			public bool IsSelected {
				get { return _IsSelected; }
				private set {
					if(IsSelected == value) return;
					_IsSelected = value;
				}
			}
		}
}
namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
	using TextDecorationCollection = DevExpress.XtraDiagram.TextDecoration;
		partial class DiagramShape {
			ShapeDescription _Shape = BasicShapes.Rectangle;
			[DXCategory(CategoryName.Appearance)]
			public ShapeDescription Shape {
				get { return _Shape; }
				set {
					if(Shape == value) return;
					var oldValue = Shape;
					_Shape = value;
					this.OnShapeChanged(oldValue);
					OnPropertiesChanged();
				}
			}
			String _Content = default(String);
			[DXCategory(CategoryName.Appearance)]
			public String Content {
				get { return _Content; }
				set {
					if(Content == value) return;
					_Content = value;
					this.Update();
					OnPropertiesChanged();
				}
			}
			DoubleCollection _Parameters = default(DoubleCollection);
			[DefaultValue(null)]
			[Browsable(false)]
			public DoubleCollection Parameters {
				get { return _Parameters; }
				set {
					if(Parameters == value) return;
					_Parameters = value;
					this.UpdateShape();
					OnPropertiesChanged();
				}
			}
		}
}
namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
	using ConnectorPointsCollection = DevExpress.XtraDiagram.Base.DiagramConnectorPointCollection;
		partial class DiagramConnector {
			ArrowDescription _BeginArrow = default(ArrowDescription);
			[DXCategory(CategoryName.Appearance)]
			public ArrowDescription BeginArrow {
				get { return _BeginArrow; }
				set {
					if(BeginArrow == value) return;
					_BeginArrow = value;
					this.Controller().OnAppearancePropertyChanged();
					OnPropertiesChanged();
				}
			}
			ArrowDescription _EndArrow = default(ArrowDescription);
			[DXCategory(CategoryName.Appearance)]
			public ArrowDescription EndArrow {
				get { return _EndArrow; }
				set {
					if(EndArrow == value) return;
					_EndArrow = value;
					this.Controller().OnAppearancePropertyChanged();
					OnPropertiesChanged();
				}
			}
			Size _BeginArrowSize = default(Size);
			[DXCategory(CategoryName.Appearance)]
			public Size BeginArrowSize {
				get { return _BeginArrowSize; }
				set {
					if(BeginArrowSize == value) return;
					_BeginArrowSize = value;
					this.Controller().OnAppearancePropertyChanged();
					OnPropertiesChanged();
				}
			}
			Size _EndArrowSize = default(Size);
			[DXCategory(CategoryName.Appearance)]
			public Size EndArrowSize {
				get { return _EndArrowSize; }
				set {
					if(EndArrowSize == value) return;
					_EndArrowSize = value;
					this.Controller().OnAppearancePropertyChanged();
					OnPropertiesChanged();
				}
			}
			ConnectorType _Type = ConnectorType.RightAngle;
			[DXCategory(CategoryName.Layout)]
			public ConnectorType Type {
				get { return _Type; }
				set {
					if(Type == value) return;
					_Type = value;
					this.Controller().OnTypeChanged();
					OnPropertiesChanged();
				}
			}
			String _Text = default(String);
			[DXCategory(CategoryName.Appearance)]
			public String Text {
				get { return _Text; }
				set {
					if(Text == value) return;
					_Text = value;
					OnPropertiesChanged();
				}
			}
			Point _BeginPoint = default(Point);
			[DXCategory(CategoryName.Layout)]
			public Point BeginPoint {
				get { return _BeginPoint; }
				set {
					if(BeginPoint == value) return;
					_BeginPoint = value;
					this.Controller().OnPointChanged(ConnectorPointType.Begin);
					OnPropertiesChanged();
				}
			}
			Point _EndPoint = default(Point);
			[DXCategory(CategoryName.Layout)]
			public Point EndPoint {
				get { return _EndPoint; }
				set {
					if(EndPoint == value) return;
					_EndPoint = value;
					this.Controller().OnPointChanged(ConnectorPointType.End);
					OnPropertiesChanged();
				}
			}
			IDiagramItem _BeginItem = default(IDiagramItem);
			[DefaultValue(null)]
			[DXCategory(CategoryName.Layout)]
			public IDiagramItem BeginItem {
				get { return _BeginItem; }
				set {
					if(BeginItem == value) return;
					_BeginItem = value;
					this.Controller().OnItemChanged(ConnectorPointType.Begin);
					OnPropertiesChanged();
				}
			}
			IDiagramItem _EndItem = default(IDiagramItem);
			[DefaultValue(null)]
			[DXCategory(CategoryName.Layout)]
			public IDiagramItem EndItem {
				get { return _EndItem; }
				set {
					if(EndItem == value) return;
					_EndItem = value;
					this.Controller().OnItemChanged(ConnectorPointType.End);
					OnPropertiesChanged();
				}
			}
			Int32 _BeginItemPointIndex = -1;
			[DefaultValue(-1)]
			[DXCategory(CategoryName.Layout)]
			public Int32 BeginItemPointIndex {
				get { return _BeginItemPointIndex; }
				set {
					if(BeginItemPointIndex == value) return;
					_BeginItemPointIndex = value;
					this.Controller().OnItemPointIndexChanged(ConnectorPointType.Begin);
					OnPropertiesChanged();
				}
			}
			Int32 _EndItemPointIndex = -1;
			[DefaultValue(-1)]
			[DXCategory(CategoryName.Layout)]
			public Int32 EndItemPointIndex {
				get { return _EndItemPointIndex; }
				set {
					if(EndItemPointIndex == value) return;
					_EndItemPointIndex = value;
					this.Controller().OnItemPointIndexChanged(ConnectorPointType.End);
					OnPropertiesChanged();
				}
			}
		}
}
namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
		partial class DiagramContainer {
			Boolean _IsSnapScope = default(Boolean);
			[DefaultValue(false)]
			[DXCategory(CategoryName.Behavior)]
			public virtual Boolean IsSnapScope {
				get { return _IsSnapScope; }
				set {
					if(IsSnapScope == value) return;
					_IsSnapScope = value;
					OnPropertiesChanged();
				}
			}
			AdjustBoundaryBehavior _AdjustBoundsBehavior = AdjustBoundaryBehavior.AutoAdjust;
			[DXCategory(CategoryName.Behavior)]
			public virtual AdjustBoundaryBehavior AdjustBoundsBehavior {
				get { return _AdjustBoundsBehavior; }
				set {
					if(AdjustBoundsBehavior == value) return;
					_AdjustBoundsBehavior = value;
					OnPropertiesChanged();
				}
			}
		}
}
namespace DevExpress.XtraDiagram {
using System;
using System.Collections;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using System.Drawing;
using Size = System.Drawing.SizeF;
using Thickness = System.Windows.Forms.Padding;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Point = DevExpress.Utils.PointFloat;
using Brush = System.Drawing.Color;
		partial class DiagramOrgChartBehavior {
			String _ChildrenPath = default(String);
			public String ChildrenPath {
				get { return _ChildrenPath; }
				set {
					if(ChildrenPath == value) return;
					_ChildrenPath = value;
					OnPropertiesChanged();
				}
			}
			IChildrenSelector _ChildrenSelector = default(IChildrenSelector);
			public IChildrenSelector ChildrenSelector {
				get { return _ChildrenSelector; }
				set {
					if(ChildrenSelector == value) return;
					_ChildrenSelector = value;
					OnPropertiesChanged();
				}
			}
			IEnumerable _ItemsSource = default(IEnumerable);
			public IEnumerable ItemsSource {
				get { return _ItemsSource; }
				set {
					if(ItemsSource == value) return;
					_ItemsSource = value;
					this.controller.OnItemsSourceChanged();
					OnPropertiesChanged();
				}
			}
			String _KeyMember = default(String);
			public String KeyMember {
				get { return _KeyMember; }
				set {
					if(KeyMember == value) return;
					_KeyMember = value;
					OnPropertiesChanged();
				}
			}
			String _ParentMember = default(String);
			public String ParentMember {
				get { return _ParentMember; }
				set {
					if(ParentMember == value) return;
					_ParentMember = value;
					OnPropertiesChanged();
				}
			}
		}
}
