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
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
namespace DevExpress.XtraDashboardLayout {
	public class HotTrackBehaviour : BaseBehaviour {
		public HotTrackBehaviour(AdornerWindowHandler handler) : base(handler) { }
		public override bool ProcessEvent(EventType eventType, MouseEventArgs e) {
			if(eventType == EventType.MouseMove || (eventType == EventType.MouseLeave && owner.State == AdornerWindowHandlerStates.Normal)) {
				Point p;
				if(e != null) p = e.Location;
				else p = owner.Owner.PointToClient(Cursor.Position);				
				BaseLayoutItemHitInfo hitTest = owner.Owner.CalcHitInfo(p);
				if(hitTest.Item != null && owner.HotTrackedItem != hitTest.Item && !hitTest.Item.IsDisposing) {
					owner.HotTrackedItem = hitTest.Item;					
					return false;
				}
			}
			return false;
		}
		public override void Paint(Graphics g) {
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.HotTrackedItem != null && owner.HotTrackedItem.ViewInfo != null) 
				glyphs.Add(new SimpleGlyph(owner.Owner) { Bounds = owner.HotTrackedItem.ViewInfo.BoundsRelativeToControl, Brush = new SolidBrush(Color.Green) });
		}
	}
}
