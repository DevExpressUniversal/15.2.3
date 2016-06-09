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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraLayout.Customization.Behaviours {
	class ConstraintsLayoutBehaviour :LayoutBaseBehaviour {
		public ConstraintsLayoutBehaviour(LayoutAdornerWindowHandler handler)
			: base(handler) {
			visualizer = new ConstraintsVisualizer();
		}
		ConstraintsVisualizer visualizer;
		public override void Paint(GraphicsCache graphicsCache) {
			base.Paint(graphicsCache);
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.CastedOwner.Disposing) return;
			if(!((ILayoutControl)owner.CastedOwner).DesignMode && !((ILayoutControl)owner.CastedOwner).EnableCustomizationMode) return;
			if(!owner.CastedOwner.Root.Selected || owner.CastedOwner.Root.SelectedItems.Count != 0) return;
			IEnumerable<BaseLayoutItem> listGroup = owner.CastedOwner.Items.Where(q => q is LayoutControlItem);
			foreach(LayoutControlItem lci in listGroup) {
				if(!lci.Visible) continue;
				Image image = visualizer.GetItemStateImageCore(lci);
				if(image != null) glyphs.Add(new ConstraintsLayoutGlyph(lci.ViewInfo.BoundsRelativeToControl, image));
			}
		}
	}
	public class ConstraintsLayoutGlyph :LayoutGlyph {
		public ConstraintsLayoutGlyph(Rectangle rect, Image image)
			: base(null) {
			this.imageCore = image;
			this.boundsRTC = rect;
			this.pointCore = new Point(rect.X + (rect.Width - imageCore.Width) / 2, rect.Y);
		}
		Image imageCore;
		Point pointCore;
		Rectangle boundsRTC;
		public override void Paint(GraphicsCache cache) {
			if(imageCore == null) return;
			GraphicsState state = cache.Graphics.Save();
			cache.Graphics.SetClip(boundsRTC);
			cache.Graphics.DrawImageUnscaled(imageCore, pointCore);
			cache.Graphics.Restore(state);
		}
	}
}
