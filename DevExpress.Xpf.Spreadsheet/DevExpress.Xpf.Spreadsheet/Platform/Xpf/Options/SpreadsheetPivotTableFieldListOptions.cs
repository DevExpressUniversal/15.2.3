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
using System.Windows;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using InnerSpreadsheetPivotTableFieldListOptions = DevExpress.XtraSpreadsheet.SpreadsheetPivotTableFieldListOptions;
using InnerSpreadsheetPivotTableFieldListStartPosition = DevExpress.XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetPivotTableFieldListStartPosition
	public enum SpreadsheetPivotTableFieldListStartPosition {
		ManualScreen = InnerSpreadsheetPivotTableFieldListStartPosition.ManualScreen,
		ManualSpreadsheetControl = InnerSpreadsheetPivotTableFieldListStartPosition.ManualSpreadsheetControl,
		CenterScreen = InnerSpreadsheetPivotTableFieldListStartPosition.CenterScreen,
		CenterSpreadsheetControl = InnerSpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl
	}
	#endregion
	#region SpreadsheetPivotTableFieldListOptions
	public class SpreadsheetPivotTableFieldListOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty StartPositionProperty;
		public static readonly DependencyProperty StartLocationProperty;
		public static readonly DependencyProperty StartSizeProperty;
		InnerSpreadsheetPivotTableFieldListOptions source;
		#endregion
		static SpreadsheetPivotTableFieldListOptions() {
			Type ownerType = typeof(SpreadsheetProtectionBehaviorOptions);
			StartPositionProperty = DependencyProperty.Register("StartPosition", typeof(SpreadsheetPivotTableFieldListStartPosition), ownerType,
				new FrameworkPropertyMetadata(SpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl));
			StartLocationProperty = DependencyProperty.Register("StartLocation", typeof(Point), ownerType,
				new FrameworkPropertyMetadata(new Point(0, 0)));
			StartSizeProperty = DependencyProperty.Register("StartSize", typeof(Size), ownerType,
				new FrameworkPropertyMetadata(new Size(0, 0)));
		}
		#region Properties
		public SpreadsheetPivotTableFieldListStartPosition StartPosition {
			get { return (SpreadsheetPivotTableFieldListStartPosition)GetValue(StartPositionProperty); }
			set { SetValue(StartPositionProperty, value); }
		}
		public Point StartLocation {
			get { return (Point)GetValue(StartLocationProperty); }
			set { SetValue(StartLocationProperty, value); }
		}
		public Size StartSize {
			get { return (Size)GetValue(StartSizeProperty); }
			set { SetValue(StartSizeProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetPivotTableFieldListOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (StartPosition != (SpreadsheetPivotTableFieldListStartPosition)GetDefaultValue(StartPositionProperty))
				source.StartPosition = (InnerSpreadsheetPivotTableFieldListStartPosition)StartPosition;
			if (StartLocation != (Point)GetDefaultValue(StartLocationProperty))
				source.StartLocation = StartLocation.ToDrawingPoint();
			if (StartSize != (Size)GetDefaultValue(StartSizeProperty))
				source.StartSize = StartSize.ToDrawingSize();
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindStartPositionProperty();
			BindStartLocationProperty();
			BindStartSizeProperty();
		}
		void BindStartPositionProperty() {
			Binding bind = new Binding("StartPosition") { Source = source, Mode = BindingMode.TwoWay, Converter = new PivotTableFieldListStartPositionConveter() };
			BindingOperations.SetBinding(this, StartPositionProperty, bind);
		}
		void BindStartLocationProperty() {
			Binding bind = new Binding("StartLocation") { Source = source, Mode = BindingMode.TwoWay, Converter = new DrawingPointToWindowsPointConveter() };
			BindingOperations.SetBinding(this, StartLocationProperty, bind);
		}
		void BindStartSizeProperty() {
			Binding bind = new Binding("StartSize") { Source = source, Mode = BindingMode.TwoWay, Converter = new DrawingSizeToWindowsSizeConveter() };
			BindingOperations.SetBinding(this, StartSizeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
