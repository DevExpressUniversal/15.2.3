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
using System.ComponentModel;
using System.IO;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Native {
	public class WinMapLoaderFactory : MapLoaderFactoryCore<MapItem> {
		public override MapItem CreateDot(CoordPoint point) {
			return new MapDot() { Location = point };
		}
		public override MapItem CreateDot(CoordPoint point, double size) {
			return new MapDot() { Location = point, Size = size };
		}
		public override MapItem CreateImageAndText(CoordPoint point, ImageTextInfoCore info) {
			MapCustomElement element = new MapCustomElement() {
				Location = point,
				Text = info.Text,
				ImageTransform = info.ImageTransform,
				ImageUri = info.ImageUri
			};
			if (info.ImageUri == null) {
				element.Image = MapUtils.ElementPushpinImage;
				element.RenderOrigin = MapUtils.ElementPushpinRenderOrigin;
			}
			return element;
		}
		public override MapItem CreateLine(CoordPoint point1, CoordPoint point2) {
			return new MapLine() { Point1 = point1, Point2 = point2 };
		}
		public override MapItem CreatePolyline() {
			return new MapPolyline();
		}
		public override MapItem CreatePolygon() {
			return new MapPolygon();
		}
		public override MapItem CreatePath() {
			return new MapPath();
		}
		public override MapItem CreateRectangle(CoordPoint location, double width, double height) {
			return new MapRectangle() { Location = location, Width = width, Height = height };
		}
		public override MapItem CreateEllipse(CoordPoint location, double width, double height) {
			return new MapEllipse() { Location = location, Width = width, Height = height };
		}
	}
}
