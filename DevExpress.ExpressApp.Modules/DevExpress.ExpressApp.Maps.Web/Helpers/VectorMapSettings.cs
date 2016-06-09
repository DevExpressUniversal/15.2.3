#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Maps.Web.Helpers {
	public class VectorMapSettings {
		public IList<IAreaInfo> Areas { get; private set; }
		public IList<IVectorMapsMarker> Markers { get; private set; }
		public IList<IVectorMapsPieMarker> PieMarkers { get; private set; }
		public VectorMapPalette Palette { get; set; }
		public VectorMapType Type { get; set; }
		public MapPoint Center { get; set; }
		public BoundsSettings Bounds { get; private set; }
		public LegendSettings Legend { get; private set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public bool IsControlsEnabled { get; set; }
		public bool IsAreasTitlesEnabled { get; set; }
		public bool IsMarkersTitlesEnabled { get; set; }
		public double ZoomFactor { get; set; }
		public string MapResourceName { get; set; }
		public float DefaultAreaValue { get; set; }
		public Color BackgroundColor { get; set; }
		public int BubbleMarkerMinSize { get; set; }
		public int BubbleMarkerMaxSize { get; set; }
		public int PieMarkerSize { get; set; }
		public VectorMapSettings() {
			Areas = new List<IAreaInfo>();
			Markers = new List<IVectorMapsMarker>();
			PieMarkers = new List<IVectorMapsPieMarker>();
			Bounds = new BoundsSettings();
			Legend = new LegendSettings();
		}
	}
	public class BoundsSettings {
		public double MinLongitude { get; set; }
		public double MaxLatitude { get; set; } 
		public double MaxLongitude { get; set; }
		public double MinLatitude { get; set; }
	}
	public class LegendSettings {
		public bool Enabled { get; set; }
		public IList<LegendItem> Items { get; private set; }
		public VectorMapLegendType Type { get; set; }
		public LegendSettings() {
			Items = new List<LegendItem>();
		}
	}
	public class LegendItem {
		public float Value { get; set; }
		public string Title { get; set; }
	}
}
