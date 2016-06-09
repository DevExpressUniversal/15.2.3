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
using System.Drawing;
namespace DevExpress.XtraMap.Drawing {
	public struct RenderContext : IRenderContext {
		readonly static IRenderContext empty = new RenderContext() { 
			ZoomLevel = 1.0f,
			CenterPoint = new GeoPoint(), 
			Bounds = Rectangle.Empty,
			ContentBounds = Rectangle.Empty,
			ClipBounds = Rectangle.Empty,
			BackColor = Color.Empty,
			IsExport = false
		};
		public static IRenderContext Empty { get { return empty; } }
		public Rectangle Bounds { get; set; }
		public Rectangle ContentBounds { get; set; }
		public Rectangle ClipBounds { get; set; }
		public CoordPoint CenterPoint { get; set; }
		public double ZoomLevel { get; set; }
		public Color BackColor { get; set; }
		public bool IsExport { get; set; }
		public BorderedElementStyle BorderedElementStyle { get; set; }
	}
	public class RenderItemResourceHolder : MapDisposableObject, IRenderItemResourceHolder {
		static readonly IRenderItemResourceHolder empty = new RenderItemResourceHolder();
		public static IRenderItemResourceHolder Empty { get { return empty; } }
		void IRenderItemResourceHolder.Update() { }
	}
	public class RenderOverlay : MapDisposableObject, IRenderOverlay {
		const bool DefaultStoringInPool = true;
		bool storingInPool = DefaultStoringInPool;
		#region IRenderOverlay Members
		public Image OverlayImage { get; set; }
		public Size OverlayImageSize { get; set; }
		public Point ScreenPosition { get; set; }
		public bool StoringInPool { get { return storingInPool; } set { storingInPool = value; } }
		public bool Printable { get; set; }
		public bool ShowInDesign { get; set; }
		#endregion
		protected override void DisposeOverride() {
			if(OverlayImage != null) {
				OverlayImage.Dispose();
				OverlayImage = null;
			}
			base.DisposeOverride();
		}
	}
}
