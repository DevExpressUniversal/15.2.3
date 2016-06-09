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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	public class DashboardItemCaptionButtonsRepository : IDisposable {
		readonly List<DashboardItemCaptionButtonInfo> captionButtonsInfo = new List<DashboardItemCaptionButtonInfo>();
		readonly DashboardViewer dashboardViewer;
		public int Count { get { return captionButtonsInfo.Count; } }
		public DashboardItemCaptionButtonInfo this[int index] { get { return captionButtonsInfo[index]; } }
		public DashboardItemCaptionButtonsRepository(DashboardViewer dashboardViewer) {
			this.dashboardViewer = dashboardViewer;
		}
		public void FillButtonInfo(BaseButtonCollection buttonInfo, IList<DashboardItemCaptionButtonInfoCreator> creators) {
			DisposeCaptionButtonsInfo();
			foreach(DashboardItemCaptionButtonInfoCreator creator in creators) {
				DashboardItemCaptionButtonInfo info = creator.GetCaptionButtonInfo();
				if (info != null)
					captionButtonsInfo.Insert(0, info);
			}
			foreach (DashboardItemCaptionButtonInfo info in captionButtonsInfo)
				buttonInfo.Add(new GroupBoxButton() {
					Image  = info.ButtonImage,
					Style = XtraBars.Docking2010.ButtonStyle.PushButton,
					Enabled = info.ButtonState != ObjectState.Disabled,
					ToolTip = info.Tooltip,
					UseCaption = false,
					Visible = true,
				});
		}
		public void ExecuteButtonInfo(DashboardItemViewer itemViewer, int buttonIndex) {
			if(captionButtonsInfo.Count > buttonIndex)
				ExecuteButtonInfo(itemViewer, captionButtonsInfo[buttonIndex]);
		}
		public void ExecuteButtonInfo(DashboardItemViewer itemViewer, DashboardItemCaptionButtonInfo info) {
			info.Execute(dashboardViewer, itemViewer, DashboardArea.DashboardItemCaption);
		}
		public void Dispose() {
			DisposeCaptionButtonsInfo();
		}
		void DisposeCaptionButtonsInfo() {
			captionButtonsInfo.ForEach(info => info.Dispose());
			captionButtonsInfo.Clear();
		}
	}
}
