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
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region GroupProcessingState
	public class GroupProcessingState : SpreadsheetMouseHandlerState {
		OutlineLevelBox startBox;
		public GroupProcessingState(SpreadsheetMouseHandler mouseHandler, OutlineLevelBox box)
			: base(mouseHandler) {
				this.startBox = box;
		}
		public override void Start() {
			base.Start();
			if (startBox.IsCollapsedButton && !Control.Options.InnerBehavior.Group.ExpandAllowed)
				return;
			if (startBox.IsExpandedButton && !Control.Options.InnerBehavior.Group.CollapseAllowed)
				return;
			startBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.Pressed;
			Control.InnerControl.Owner.Redraw();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if (hitTestResult != null && hitTestResult.GroupBox != null && hitTestResult.GroupBox.Bounds.Equals(startBox.Bounds)) {
				if (startBox.OutlineLevelBoxSelectType != OutlineLevelBoxSelectType.Pressed)
					startBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.Pressed;
				else
					return;
			}
			else startBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.None;
			Control.InnerControl.Owner.Redraw();
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (startBox.OutlineLevelBoxSelectType == OutlineLevelBoxSelectType.Pressed)
				GroupProcessingCore();
			MouseHandler.SwitchToDefaultState();
		}
		void GroupProcessingCore() {
			DocumentModel.BeginUpdate();
			try {
				if (startBox.IsHeaderButton)
					new Commands.OutlineLevelCommand(Control, startBox.Level, startBox.IsRowButton).Execute();
				else {
					foreach (GroupItem group in startBox.GetExpandingGroups())
						new Commands.ExpandGroupCommand(Control, group).Execute();
					foreach (GroupItem group in startBox.GetCollapsingGroups())
						new Commands.CollapseGroupCommand(Control, group).Execute();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			return Control.InnerControl.ActiveView.CalculateGroupHitTest(pt);
		}
	}
	#endregion
}
