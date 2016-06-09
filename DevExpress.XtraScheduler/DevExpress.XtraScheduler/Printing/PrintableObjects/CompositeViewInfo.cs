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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class CompositeViewInfo : IPrintableObjectViewInfo, ISupportClear {
		DXCollectionBase<IPrintableObjectViewInfo> viewInfoCollection;
		Rectangle bounds;
		GraphicsInfoArgs graphicsInfoArgs;
		public CompositeViewInfo() {
			viewInfoCollection = new DXCollectionBase<IPrintableObjectViewInfo>();
		}
		public Rectangle Bounds { get { return bounds; } }
		internal DXCollectionBase<IPrintableObjectViewInfo> ViewInfoCollection { get { return viewInfoCollection; } }
		public void AddChild(IPrintableObjectViewInfo viewInfo) {
			if (viewInfoCollection.Count == 0)
				bounds = viewInfo.Bounds;
			else {
				int left = Math.Min(viewInfo.Bounds.Left, bounds.Left);
				int right = Math.Max(viewInfo.Bounds.Right, bounds.Right);
				int top = Math.Min(viewInfo.Bounds.Top, bounds.Top);
				int bottom = Math.Max(viewInfo.Bounds.Bottom, bounds.Bottom);
				bounds = Rectangle.FromLTRB(left, top, right, bottom);
			}
			viewInfoCollection.Add(viewInfo);
		}
		public void Print(GraphicsInfoArgs graphicsInfoArgs) {
			this.graphicsInfoArgs = graphicsInfoArgs;
			GraphicsClip clipInfo = graphicsInfoArgs.Cache.ClipInfo;
			Rectangle oldMaxBounds = clipInfo.MaximumBounds;
			clipInfo.MaximumBounds = bounds;
			GraphicsClipState oldClipping = clipInfo.SaveAndSetClip(bounds);
			viewInfoCollection.ForEach(DrawItem);
			clipInfo.RestoreClipRelease(oldClipping);
			clipInfo.MaximumBounds = oldMaxBounds;
		}
		public void Clear() {
			viewInfoCollection.ForEach(ClearItem);
		}
		void DrawItem(IPrintableObjectViewInfo item) {
			item.Print(graphicsInfoArgs);
		}
		void ClearItem(IPrintableObjectViewInfo item) {
			ISupportClear itemToClear = item as ISupportClear;
			if (itemToClear != null)
				itemToClear.Clear();
		}
	}
}
