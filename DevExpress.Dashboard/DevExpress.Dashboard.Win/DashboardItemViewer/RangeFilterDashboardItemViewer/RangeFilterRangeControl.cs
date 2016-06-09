#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using System.Drawing;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class RangeFilterRangeControl : RangeControl {
		class RangeFilterRangeControlHandler : RangeControlHandler {
			readonly ToolTipController toolTipController;
			readonly RangeFilterRangeControl rangeControl;
			public RangeFilterRangeControlHandler(RangeFilterRangeControl rangeControl, ToolTipController toolTipController)
				: base(rangeControl) {
				this.rangeControl = rangeControl;
				this.toolTipController = toolTipController;
			}
			string GetToolTipText(double normalizedValue) {
				return rangeControl.Client.ValueToString(normalizedValue);
			}
			protected override void UpdateCursors() {
				base.UpdateCursors();
				RangeControlHitTest hitTest = PrevCursor == null ? ViewInfo.PressedInfo.HitTest : ViewInfo.HotInfo.HitTest;
				string text;
				switch (hitTest) {
					case RangeControlHitTest.MinRangeThumb:
						text = GetToolTipText(((BaseRangeControlViewInfo)rangeControl.ViewInfo).RangeMinimum);
						break;
					case RangeControlHitTest.MaxRangeThumb:
						text = GetToolTipText(((BaseRangeControlViewInfo)rangeControl.ViewInfo).RangeMaximum);
						break;
					default:
						text = null;
						break;
				}
				if (String.IsNullOrEmpty(text))
					toolTipController.HideHint();
				else {
					ToolTipControllerShowEventArgs args = new ToolTipControllerShowEventArgs(rangeControl, null);
					args.ToolTipType = ToolTipType.SuperTip;
					args.SuperTip.Items.Add(text);
					toolTipController.ShowHint(args);
				}
			}
		}
		public bool DrawIgnoreUpdatesState { get; set; }
		readonly ToolTipController toolTipController;
		public RangeFilterRangeControl(ToolTipController toolTipController) {
			this.toolTipController = toolTipController;
		}
		protected override RangeControlHandler CreateHandler() {
			return new RangeFilterRangeControlHandler(this, toolTipController);
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			base.OnPaint(e);
			if(DrawIgnoreUpdatesState)
				DashboardWinHelper.DrawIgnoreUpdatesState(e.Graphics, LookAndFeel, ClientRectangle);
		}
	}
}
