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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUIButtonsPanelViewInfo : BaseButtonsPanelViewInfo {
		public WindowsUIButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			if(button is BaseSeparator) return new WindowsUISeparatorInfo(button);
			return new WindowsUIButtonInfo(button);
		}
		protected override Point GetPointOffset(bool horz, Point offset, Rectangle innerContent) {
			if(horz) {
				return new Point(offset.X - Content.X + innerContent.X, offset.Y);
			}
			return offset;
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			int rowOffset = MinSize.Height;
			int columnOffset = MinSize.Width;
			Point tempOffset = offset;
			int previousRow = 0;
			int previousColumn = 0;
			foreach(BaseButtonInfo buttonInfo in buttons) {
				if(previousRow != buttonInfo.Row) {
					offset.X = tempOffset.X;
					offset.Y = tempOffset.Y + buttonInfo.Row * rowOffset + interval * buttonInfo.Row;
				}
				if(previousColumn != buttonInfo.Column) {
					offset.Y = tempOffset.Y;
					offset.X = tempOffset.X + buttonInfo.Column * columnOffset + interval * buttonInfo.Column;
				}
				if(horz) {
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(Content.Width, rowOffset)), horz);
					offset.X += (buttonInfo.Bounds.Width + interval);
					previousRow = buttonInfo.Row;
				}
				else {
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(columnOffset, Content.Height)), horz);
					offset.Y += (buttonInfo.Bounds.Height + interval);
					previousColumn = buttonInfo.Column;
				}
			}
			return offset;
		}
		protected override void SortButtonListCore(List<IBaseButton> positiveList) {
			positiveList.Sort(PositiveCompare);
		}
	}
	public class CustomHeaderButtonsPanelViewInfo : WindowsUIButtonsPanelViewInfo {
		public CustomHeaderButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			if(button is BaseSeparator) return new WindowsUISeparatorInfo(button); 
			return new CustomHeaderButtonInfo(button);
		}
	}
}
