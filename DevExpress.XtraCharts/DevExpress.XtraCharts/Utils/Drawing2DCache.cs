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

using System.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class Drawing2DCache {
		readonly Dictionary<int, GraphicsCommand> commandCache = new Dictionary<int, GraphicsCommand>();
		readonly Dictionary<int, CoordinatesConversionCache> coordinatesConversionCache = new Dictionary<int, CoordinatesConversionCache>();
		int lastImageKey;
		Bitmap lastImage;
		public GraphicsCommand this[int key] { get { return commandCache.ContainsKey(key) ? commandCache[key] : null; } }
		public Bitmap LastImage { get { return lastImage; } }
		public Bitmap GetLastImage(int key) {
			return lastImage != null && lastImageKey == key ? lastImage : null;
		}
		public CoordinatesConversionCache GetCoordinatesConversionCache(int key) {
			return coordinatesConversionCache.ContainsKey(key) ? coordinatesConversionCache[key] : null;
		}
		public void Add(int key, GraphicsCommand command, Bitmap lastImage, CoordinatesConversionCache cache) {
			commandCache[key] = command;
			coordinatesConversionCache[key] = cache;
			if (this.lastImage != null)
				this.lastImage.Dispose();
			this.lastImage = lastImage;
			lastImageKey = key;
		}
		public void Clear() {
			foreach (GraphicsCommand command in commandCache.Values)
				command.Dispose();
			commandCache.Clear();
			coordinatesConversionCache.Clear();
			if (lastImage != null) {
				lastImage.Dispose();
				lastImage = null;
			}
			lastImageKey = 0;
		}
	}
	public abstract class CoordinatesConversionCache { }
	public class XYDiagramCoordinatesConversionCache : CoordinatesConversionCache {
		readonly Dictionary<XYDiagramPaneBase, PaneCoordinatesConversionCache> paneCaches = new Dictionary<XYDiagramPaneBase, PaneCoordinatesConversionCache>();
		public PaneCoordinatesConversionCache GetPaneCoordinatesConversionCache(XYDiagramPaneBase pane) {
			PaneCoordinatesConversionCache paneCache;
			return paneCaches.TryGetValue(pane, out paneCache) ? paneCache : null;
		}
		public void Register(XYDiagramPaneBase pane, XYDiagramMappingList mappingList, Rectangle? lastMappingBounds) {
			PaneCoordinatesConversionCache paneCache;
			if (paneCaches.TryGetValue(pane, out paneCache))
				paneCache.SetValues(mappingList, lastMappingBounds);
			else 
				paneCaches.Add(pane, new PaneCoordinatesConversionCache(mappingList, lastMappingBounds));
		}
	}
	public class PaneCoordinatesConversionCache {
		XYDiagramMappingList mappingList;
		Rectangle? mappingBounds;
		public XYDiagramMappingList MappingList { get { return mappingList; } }
		public Rectangle? MappingBounds { get { return mappingBounds; } }
		public PaneCoordinatesConversionCache(XYDiagramMappingList mappingList, Rectangle? mappingBounds) {
			this.mappingList = mappingList;
			this.mappingBounds = mappingBounds;
		}
		public void SetValues(XYDiagramMappingList mappingList, Rectangle? mappingBounds) {
			this.mappingList = mappingList;
			this.mappingBounds = mappingBounds;
		}
	}
	public class RadarCoordinatesConversionCache : CoordinatesConversionCache {
		RadarDiagramMapping diagramMapping ;
		public RadarDiagramMapping DiagramMapping { get { return diagramMapping; } }
		public RadarCoordinatesConversionCache(RadarDiagramMapping diagramMapping) {
			this.diagramMapping = diagramMapping;
		}
	}
}
