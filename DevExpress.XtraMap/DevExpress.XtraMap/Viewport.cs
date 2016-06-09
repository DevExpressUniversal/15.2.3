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
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraMap {
	public class MapViewport {
		[DefaultValue(0.0)]
		public CoordPoint CenterPoint { get; set; }
		[DefaultValue(1.0)]
		public double ZoomLevel { get; set; }
	}
}
namespace DevExpress.XtraMap.Native {
	public class MapViewportInternal {
		internal static readonly MapRect DefaultViewPort = CreateDefaultViewport();
		static MapRect CreateDefaultViewport() {
			return new MapRect(0, 0, 1, 1);
		}
		MapRect animatedViewportRect;
		Size viewportInPixels;
		public MapRect AnimatedViewportRect {
			get { return animatedViewportRect; }
			set { 
				if (animatedViewportRect == value)
				return;
				animatedViewportRect = value;
				OnChanged();
			}
		}
		public Size ViewportInPixels {
			get { return viewportInPixels; }
			set {
				if(viewportInPixels == value)
					return;
				viewportInPixels = value;
				OnChanged();
			}
		}
		public MapViewportInternal() {
			Reset();
		}
		void OnChanged() {
		}
		void Reset() {
			animatedViewportRect = CreateDefaultViewport();
			viewportInPixels = Size.Empty;
		}
	}
}
