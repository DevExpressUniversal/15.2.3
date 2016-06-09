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
using System.Linq;
namespace DevExpress.XtraMap.Native {
	public abstract class MultiScaleTileBase : MapDisposableObject {
		int x;
		int y;
		bool invalid;
		MapPoint location;
		TileIndex index;
		MultiScaleViewport viewport;
		public int X { get {return x; } }
		public int Y { get { return y; } }
		public bool IsInvalid {  get { return invalid; } set { invalid = value; }  }
		public MapPoint Location { get { return location; } }
		public TileIndex Index {
			get {
				return index;
			}
			set {
				if (index != value) {
					index = value;
					OnIndexChanged();
				}
			}
		}
		public MultiScaleViewport Viewport { get { return viewport; } }
		protected MultiScaleTileBase(MultiScaleViewport viewport, int x, int y) {
			this.viewport = viewport;
			this.x = x;
			this.y = y;
			this.invalid = true;
			this.index = TileIndex.Invalid;
			this.location = new MapPoint(x * viewport.TileSourceWidth, y * viewport.TileSourceHeight);
		}
		protected virtual void OnIndexChanged() {
			invalid = true;
		}
		protected void RedrawVisualContent() {
			viewport.RedrawVisualContent();
		}
		protected abstract void OnUpdate();
		internal abstract void UpdateRenderParams(MapPoint scaleFactor);
		public void Update() {
			OnUpdate();
		}
		public void Invalidate() {
			IsInvalid = true;
		}
	}
}
