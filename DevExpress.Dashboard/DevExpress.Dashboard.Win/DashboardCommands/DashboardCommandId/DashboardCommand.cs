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
using System.Drawing;
using System.Reflection;
using DevExpress.Data.Utils;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.DashboardWin.ServiceModel;
using System.Xml.Linq;
namespace DevExpress.DashboardWin.Commands {
	public abstract class DashboardCommand : ControlCommand<DashboardDesigner, DashboardCommandId, DashboardWinStringId> {
		protected override string ImageResourcePrefix { get { return "DevExpress.DashboardWin.Images.Bars"; } }
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetExecutingAssembly(); } }
		protected override XtraLocalizer<DashboardWinStringId> Localizer { get { return DashboardWinLocalizer.Active; } }
		protected Image SmallImage {
			get {
				Image image = Image;
				if(image != null || String.IsNullOrEmpty(ImageName))
					return image;
				LoadImage();
				return Image;
			}
		}		
		protected DashboardCommand(DashboardDesigner control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			var uiLocker = Control.RequestServiceStrictly<UILocker>();
			if (!uiLocker.IsLocked) {
				try {
					RibbonControl ribbon = Control.MenuManager as RibbonControl;
					if (ribbon != null)
						ribbon.HideApplicationButtonContentControl();
					ExecuteInternal(state);
				}
				finally {
					Control.OnUpdateUI();
				}
			}
		}
		protected abstract void ExecuteInternal(ICommandUIState state);
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = state.Visible = Control.Dashboard != null;
		}
	}
	public abstract class DashboardItemCommand<T> : DashboardCommand where T : DashboardItem {
		protected T DashboardItem { get { return Control.SelectedDashboardItem as T; } }
		protected DashboardItemCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected DashboardItemViewer FindDashboardItemViewer(string componentName) {
			IDashboardLayoutAccessService layoutAccessService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutAccessService>();
			return layoutAccessService.FindDashboardItemViewer(componentName);
		}
		protected virtual void UpdateVisibleState(ICommandUIState state) {
			state.Visible = DashboardItem != null;
		}
		protected virtual void UpdateEnableState(ICommandUIState state) {
			state.Enabled = DashboardItem != null;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			UpdateVisibleState(state);
			UpdateEnableState(state);
		}
	}
	public abstract class DashboardItemInteractionCommand<T> : DashboardItemCommand<T> where T : DashboardItem {
		protected DashboardItemInteractionCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected abstract bool CheckDashboardItem(T item);
		protected abstract IHistoryItem CreateHistoryItem(T dashboardItem, bool enabled);
		protected virtual void UpdateCheckState(ICommandUIState state) {
			T item = DashboardItem;
			state.Checked = item != null ? CheckDashboardItem(item) : false;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			UpdateCheckState(state);
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			T dashboardItem = DashboardItem;
			if(dashboardItem != null) {
				IHistoryItem historyItem = CreateHistoryItem(dashboardItem, !CheckDashboardItem(dashboardItem));
				if(historyItem != null) {
					historyItem.Redo(Control);
					Control.History.Add(historyItem);
				}
			}
		}
	}
	public abstract class DashboardViewerCommand {
		readonly DashboardViewer viewer;
		protected DashboardViewer Viewer { get { return viewer; } }
		protected DashboardViewerCommand(DashboardViewer viewer) {
			this.viewer = viewer;
		}
		public abstract void Execute();
	}
	public abstract class DashboardItemViewerCommand : DashboardViewerCommand {
		readonly DashboardItemViewer itemViewer;
		protected DashboardItemViewer ItemViewer { get { return itemViewer; } }
		protected DashboardItemViewerCommand(DashboardViewer viewer, DashboardItemViewer itemViewer)
			: base(viewer) {
			this.itemViewer = itemViewer;
		}
	}
	public abstract class DashboardItemViewerButtonCommand : DashboardItemViewerCommand {
		public abstract DashboardButtonType ButtonType { get; }
		protected DashboardItemViewerButtonCommand(DashboardViewer viewer, DashboardItemViewer itemViewer)
			: base(viewer, itemViewer) {
		}
	}
}
