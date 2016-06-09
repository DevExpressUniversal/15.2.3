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

using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraDashboardLayout {
   public class CrosshairBehaviour : BaseBehaviour {
		public CrosshairBehaviour(AdornerWindowHandler handler, ResizeBehaviour resizeBehaviour) : base(handler) {
			ResizeBehaviour = resizeBehaviour;
		}
		public ResizeBehaviour ResizeBehaviour { get; set; }
		public override bool ProcessEvent(EventType eventType, MouseEventArgs e) {
			if(owner.State == AdornerWindowHandlerStates.Normal && e != null) {
				CrosshairGlyph g = GetGlyphAtPoint(e.Location) as CrosshairGlyph;
				if(g == null) return false;
				if(eventType == EventType.MouseDown && e.Button == MouseButtons.Left) {
					owner.Owner.RaiseDashboardLayoutChanging();
					g.Crosshair.CrosshairGroupingType = ChangeCrosshairGroupingType(g.Crosshair.CrosshairGroupingType == CrosshairGroupingTypes.NotSet ? g.Crosshair.realCrosshairTypeCore : g.Crosshair.CrosshairGroupingType);
					owner.Owner.Root.ResetResizer();
					owner.Owner.Refresh();
					owner.Owner.RaiseDashboardLayoutChanged();
				}
				owner.SetCursor(Cursors.Default);
				return true;
			}
			return false;
		}
		protected virtual CrosshairGroupingTypes ChangeCrosshairGroupingType(CrosshairGroupingTypes crosshairGroupingTypes) {
			switch(crosshairGroupingTypes) {
				case CrosshairGroupingTypes.GroupVertical:
					return CrosshairGroupingTypes.GroupHorizontal;
				default:
					return CrosshairGroupingTypes.GroupVertical;
			}
		}
		public override void Paint(Graphics g) {
			var p = owner.Owner.PointToClient(Cursor.Position);
			var res1 = GetGlyphAtPoint(p);
			var res2 = ResizeBehaviour.GetGlyphAtPoint(p);
			bool crosshairVisible = false;
			if(res1 != null || res2 != null) crosshairVisible = true;
			foreach(Glyph glyph in glyphs) {
				var crosshairGlyph = glyph as CrosshairGlyph;
				crosshairGlyph.Visible = crosshairVisible;
			}
			owner.InvalidateOnTimer = crosshairVisible;
			base.Paint(g);
		}
	   static Size crosshairImageSize = new Size(42, 40);
	   static Size crosshairGlyphSize = new Size(20, 20);
		public override void Invalidate() {
			base.Invalidate();
			if(owner.Owner.Disposing || owner.Owner.IsDisposed) return;
			if(owner.State != AdornerWindowHandlerStates.Normal) return;
			var crosshairCollection = owner.Owner.Root.GetFlatCrosshairs();
			foreach(Crosshair cross in crosshairCollection) {
				if(cross.ContainsLockedItems()) continue;
				var bounds = cross.LeftTopItem.ViewInfo.BoundsRelativeToControl;
				Point imageLocation = new Point(bounds.X + bounds.Width - crosshairImageSize.Width / 2, bounds.Y + bounds.Height - crosshairImageSize.Height / 2);
				Point glyphLocation = new Point(bounds.X + bounds.Width - crosshairGlyphSize.Width / 2, bounds.Y + bounds.Height - crosshairGlyphSize.Height / 2);
				Rectangle glyphBounds = new Rectangle(glyphLocation, crosshairGlyphSize);
				glyphs.Add(new CrosshairGlyph(owner.Owner, cross, imageLocation, glyphBounds));
			}
		}
	}
}
