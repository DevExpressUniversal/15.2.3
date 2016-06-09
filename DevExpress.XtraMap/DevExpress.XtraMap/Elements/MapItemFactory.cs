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
using System.Text;
namespace DevExpress.XtraMap {
	public enum MapItemType {
		Unknown = 0,
		Dot = 1,
		Ellipse = 2, 
		Line = 3, 
		Path = 4,
		Polygon = 5,
		Polyline = 6, 
		Rectangle = 7,
		Pushpin = 8,
		Custom = 9,
		Callout = 10,
		Bubble = 11,
		Pie = 12
	}
	public class DefaultMapItemFactory : IMapItemFactory {
		static readonly IMapItemFactory instance;
		static DefaultMapItemFactory() {
			instance = new DefaultMapItemFactory();
		}
		public static IMapItemFactory Instance { get { return instance; } }
		MapItem IMapItemFactory.CreateMapItem(MapItemType type, object obj) {
			return CreateMapItem(type, obj);
		}
		protected MapItem CreateMapItem(MapItemType type, object obj) {
			MapItem item = CreateItemInstance(type, obj);
			InitializeItem(item, obj);
			return item;
		}
		protected virtual MapItem CreateItemInstance(MapItemType type, object obj) {
			switch(type) {
				case MapItemType.Bubble:
					return new MapBubble();
				case MapItemType.Callout:
					return new MapCallout();
				case MapItemType.Custom:
					return new MapCustomElement();
				case MapItemType.Dot:
					return new MapDot();
				case MapItemType.Ellipse:
					return new MapEllipse();
				case MapItemType.Line:
					return new MapLine();
				case MapItemType.Path:
					return new MapPath();
				case MapItemType.Pie:
					return new MapPie();
				case MapItemType.Polygon:
					return new MapPolygon();
				case MapItemType.Polyline:
					return new MapPolyline();
				case MapItemType.Pushpin:
					return new MapPushpin();
				case MapItemType.Rectangle:
					return new MapRectangle();
			}
			return null;
		}
		protected virtual void InitializeItem(MapItem item, object obj) {
		}
	}
}
namespace DevExpress.XtraMap.Native {
	[Flags]
	public enum MapItemUpdateType {
		None = 0,
		Location = 1,
		Style = 2,
		Layout = 4,
		All = Location | Style | Layout
	};
}
