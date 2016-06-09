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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Helpers;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
namespace DevExpress.XtraDashboardLayout {
	public class SelectionBehaviour : BaseBehaviour {
		public SelectionBehaviour(AdornerWindowHandler handler) : base(handler) { }
		 public override bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			if(eventType == EventType.MouseDown && owner.State == AdornerWindowHandlerStates.Normal && e!=null) {
				BaseLayoutItemHitInfo hitTest = owner.Owner.CalcHitInfo(e.Location);
				if(hitTest.Item != null && owner.SelectedItem != hitTest.Item && hitTest.Item.Parent != null) {
					owner.SelectedItem = hitTest.Item;
					if(e.Button == MouseButtons.Left && sender is string && (sender as string) == "HookEvent") return true;
				}
			}
			return false;
		}
		public override void Paint(Graphics g) {
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.SelectedItem != null && !owner.SelectedItem.IsDisposing) glyphs.Add(new SimpleGlyph(owner.Owner) { Bounds = owner.SelectedItem.ViewInfo.BoundsRelativeToControl, Brush = new SolidBrush(Color.Red) });
		}
	}
}
