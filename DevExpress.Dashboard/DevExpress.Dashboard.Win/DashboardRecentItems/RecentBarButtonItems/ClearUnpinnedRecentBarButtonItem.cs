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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Bars.Native {
	public abstract class ClearUnpinnedRecentBarButtonItem : RecentBarButtonItem {
		protected abstract IEnumerable<DashboardMenuFileLabel> RecentLabels { get; }
		protected ClearUnpinnedRecentBarButtonItem(DashboardMenuFileLabel label)
			: base(label) {
		}
		protected override void OnClick(BarItemLink link) {
			IDashboardErrorMessageService messageService = ServiceProvider.GetService<IDashboardErrorMessageService>();
			if(messageService != null) {
				DialogResult result = messageService.ShowMessage(
					DashboardWinLocalizer.GetString(DashboardWinStringId.MessageConfirmUnpinnedItemsRemove),
					DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption),
					MessageBoxButtons.OKCancel, MessageBoxIcon.None);
				if(result == DialogResult.OK)
					RemoveUnpinnedRecentItems();
			}
		}
		void RemoveUnpinnedRecentItems() {
			List<DashboardMenuFileLabel> removeLabels = new List<DashboardMenuFileLabel>();
			foreach(DashboardMenuFileLabel recentLabel in RecentLabels)
				if(!recentLabel.Checked)
					removeLabels.Add(recentLabel);
			removeLabels.ForEach(removeLabel => Controller.RemoveDashboardMenuFileLabel(removeLabel));
			Controller.InitializeRecentItems(true);
		}
	}
}
