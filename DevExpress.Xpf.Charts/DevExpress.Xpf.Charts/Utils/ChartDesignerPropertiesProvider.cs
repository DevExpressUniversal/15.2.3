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
using System.Windows.Controls;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Serialization;
namespace DevExpress.Xpf.Charts.Native {
	public static class ChartDesignerPropertiesProvider {
		public static Type GetSupportedDiagramType(Type seriesType) {
			Series series = (Series)Activator.CreateInstance(seriesType);
			return series.SupportedDiagramType;
		}
		public static ScaleType GetAxisScaleType(AxisBase axis) {
			return (ScaleType)axis.ScaleMap.ScaleType;
		}
		public static bool GetIsAutoSeries(Series series) {
			return (series != null) ? series.IsAutoSeries : false;
		}
		public static bool GetIsAutoPointsAdded(Series series) {
			return series != null ? series.IsAutoPointsAdded : false;
		}
		public static AxisScaleTypeMap GetAxisScaleTypeMap(AxisBase axis) {
			return axis.ScaleMap;
		}
		public static Axis2D GetConstantLineOwner(ConstantLine constantLine) {
			return constantLine.Axis;
		}
		public static bool IsAxisVertical(AxisBase axis) {
			return axis.IsVertical;
		}
		public static Axis2D GetStripOwner(Strip strip) {
			return strip.Axis;
		}
		public static bool IsValuesAxis(AxisBase axis) {
			return axis.IsValuesAxis;
		}
		public static XYSeries2D GetIndicatorOwner(Indicator indicator) {
			IOwnedElement indicatorAsOwnedElement = (IOwnedElement)indicator;
			return (XYSeries2D)indicatorAsOwnedElement.Owner;
		}
		public static bool? IsOuter(NestedDonutSeries2D series) {
			return series.IsOuter;
		}
	}
	public static class CustomPropertiesSerializationUtils {
		public static void SerializableControlProperties(object obj, CustomGetSerializablePropertiesEventArgs e) {
			Control control = obj as Control;
			if (control != null) {
				e.SetPropertySerializable(Control.FontFamilyProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.FontSizeProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.FontStyleProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.FontWeightProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.ForegroundProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.BackgroundProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.VisibilityProperty.ToString(), new DXSerializable());
				e.SetPropertySerializable(Control.BorderThicknessProperty.ToString(), new DXSerializable());
			}
		}
	}
}
