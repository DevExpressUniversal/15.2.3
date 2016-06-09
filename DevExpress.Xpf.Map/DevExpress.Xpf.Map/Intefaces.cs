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

using DevExpress.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Map.Native {
	public interface IViewportAnimatableElement {
		bool OriginInProgress { get; }
		bool SizeInProgress { get; }
		void ProgressChanged();
		void BeforeSizeProgressCompleting();
	}
	public interface IOwnedElement {
		object Owner { get; set; }
	}
	public interface IMapItem {
		void CalculateLayout();
		void CompleteLayout(UIElement element);		
		bool Visible { get; }
		Point Location { get; }
		Size Size { get; }
	}
	public interface IHitTestableElement {
		Object Element { get; }
		bool IsHitTestVisible { get; }
	}
	public interface IMapItemStyleProvider {
		Visibility Visibility { get; set; }
		Brush Fill { get; set; }
		Brush Stroke { get; set; }
		double StrokeThickness { get; set; }
		DoubleCollection StrokeDashArray { get; set; }
		PenLineCap StrokeDashCap { get; set; }
		double StrokeDashOffset { get; set; }
		PenLineCap StrokeEndLineCap { get; set; }
		PenLineCap StrokeStartLineCap { get; set; }
		PenLineJoin StrokeLineJoin { get; set; }
		double StrokeMiterLimit { get; set; }
		Effect Effect { get; set; }
	}
	public interface IMapView {
		double ZoomLevel { get; }
		CoordPoint CenterPoint { get; }
		Size InitialMapSize { get; }
		MapCoordinateSystem CoordinateSystem { get; }
		Rect Viewport { get; }
		Size ViewportInPixels { get; }
	}
	public interface ISupportProjection {
		ProjectionBase Projection { get; }
	}
	public interface ILegendDataProvider {
		IList<MapLegendItemBase> CreateItems(MapLegendBase legend);
	}
	public interface IInvalidKeyPanelHolder {
		Grid InvalidKeyPanel { get; }
	}
}
