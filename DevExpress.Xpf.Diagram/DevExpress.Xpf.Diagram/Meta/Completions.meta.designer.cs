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
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
	partial class DiagramOrgChartBehavior {
		public static readonly DependencyProperty ItemTemplateProperty;
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public static readonly DependencyProperty ItemContainerStyleProperty;
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty MarginProperty;
		public double Margin {
			get { return (double)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
	}
}
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
	partial class SelectionAdorner {
		public static readonly DependencyProperty IsMultipleSelectionProperty;
		static readonly DependencyPropertyKey IsMultipleSelectionPropertyKey;
		public bool IsMultipleSelection {
			get { return (bool)GetValue(IsMultipleSelectionProperty); }
			private set { SetValue(IsMultipleSelectionPropertyKey, value); }
		}
		public static readonly DependencyProperty CanResizeProperty;
		public bool CanResize {
			get { return (bool)GetValue(CanResizeProperty); }
			set { SetValue(CanResizeProperty, value); }
		}
		public static readonly DependencyProperty ShowFullUIProperty;
		public bool ShowFullUI {
			get { return (bool)GetValue(ShowFullUIProperty); }
			set { SetValue(ShowFullUIProperty, value); }
		}
		public static readonly DependencyProperty CanRotateProperty;
		public bool CanRotate {
			get { return (bool)GetValue(CanRotateProperty); }
			set { SetValue(CanRotateProperty, value); }
		}
	}
}
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
	partial class ShapeParametersAdorner {
		public static readonly DependencyProperty ParametersProperty;
		public ParameterViewInfo[] Parameters {
			get { return (ParameterViewInfo[])GetValue(ParametersProperty); }
			set { SetValue(ParametersProperty, value); }
		}
	}
}
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
	partial class SelectionPartAdornerBase {
		public static readonly DependencyProperty IsPrimarySelectionProperty;
		static readonly DependencyPropertyKey IsPrimarySelectionPropertyKey;
		public bool IsPrimarySelection {
			get { return (bool)GetValue(IsPrimarySelectionProperty); }
			private set { SetValue(IsPrimarySelectionPropertyKey, value); }
		}
	}
}
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
	partial class ShapePresenter {
		public static readonly DependencyProperty ZoomProperty;
		public double Zoom {
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		public static readonly DependencyProperty ShapeProperty;
		public ShapeGeometry Shape {
			get { return (ShapeGeometry)GetValue(ShapeProperty); }
			set { SetValue(ShapeProperty, value); }
		}
	}
}
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
	partial class ConnectorSelectionPartAdorner {
		public static readonly DependencyProperty ShapeProperty;
		public ShapeGeometry Shape {
			get { return (ShapeGeometry)GetValue(ShapeProperty); }
			set { SetValue(ShapeProperty, value); }
		}
	}
}
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
	partial class ConnectorPreviewAdornerBase {
		public static readonly DependencyProperty ShapeProperty;
		public ShapeGeometry Shape {
			get { return (ShapeGeometry)GetValue(ShapeProperty); }
			set { SetValue(ShapeProperty, value); }
		}
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
	}
}
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
	partial class ConnectorSelectionAdorner {
		public static readonly DependencyProperty BeginPointProperty;
		public Point BeginPoint {
			get { return (Point)GetValue(BeginPointProperty); }
			set { SetValue(BeginPointProperty, value); }
		}
		public static readonly DependencyProperty EndPointProperty;
		public Point EndPoint {
			get { return (Point)GetValue(EndPointProperty); }
			set { SetValue(EndPointProperty, value); }
		}
		public static readonly DependencyProperty IsBeginPointConnectedProperty;
		public bool IsBeginPointConnected {
			get { return (bool)GetValue(IsBeginPointConnectedProperty); }
			set { SetValue(IsBeginPointConnectedProperty, value); }
		}
		public static readonly DependencyProperty IsEndPointConnectedProperty;
		public bool IsEndPointConnected {
			get { return (bool)GetValue(IsEndPointConnectedProperty); }
			set { SetValue(IsEndPointConnectedProperty, value); }
		}
		public static readonly DependencyProperty IsConnectorCurvedProperty;
		public bool IsConnectorCurved {
			get { return (bool)GetValue(IsConnectorCurvedProperty); }
			set { SetValue(IsConnectorCurvedProperty, value); }
		}
		public static readonly DependencyProperty PointsProperty;
		public ConnectorPointViewModel[] Points {
			get { return (ConnectorPointViewModel[])GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}
	}
}
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
	partial class ConnectorPointAdorner {
		public static readonly DependencyProperty PointIndexProperty;
		public int PointIndex {
			get { return (int)GetValue(PointIndexProperty); }
			set { SetValue(PointIndexProperty, value); }
		}
		public static readonly DependencyProperty ConnectorProperty;
		public IDiagramConnector Connector {
			get { return (IDiagramConnector)GetValue(ConnectorProperty); }
			set { SetValue(ConnectorProperty, value); }
		}
	}
}
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
	partial class ConnectionPointsAdorner {
		public static readonly DependencyProperty PointsProperty;
		static readonly DependencyPropertyKey PointsPropertyKey;
		public ConnectionPointViewModel[] Points {
			get { return (ConnectionPointViewModel[])GetValue(PointsProperty); }
			private set { SetValue(PointsPropertyKey, value); }
		}
	}
}
namespace DevExpress.Xpf.Diagram {
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Internal;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Utils.Serializing;
using System.IO;
using DevExpress.Diagram.Core.TypeConverters;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
	partial class DiagramList {
		public static readonly DependencyProperty OrientationProperty;
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
}
