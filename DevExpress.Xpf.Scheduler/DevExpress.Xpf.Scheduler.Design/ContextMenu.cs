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

extern alias Platform;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::System.Collections.ObjectModel;
using Platform::DevExpress.Xpf.Core.Design;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler.Design {
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class SchedulerContextMenuProvider : ContextMenuProviderBase {
		public SchedulerContextMenuProvider() {
			SchedulerBarsMenuCreatorBase barsMenuCreator = new SchedulerBarsMenuCreator();
			barsMenuCreator.CreateMenu(Items);
			SchedulerBarsMenuCreatorBase ribbonMenuCreator = new SchedulerRibbonMenuCreator();
			ribbonMenuCreator.CreateMenu(Items);
		}
	}
	public abstract class BarInfosContainer {
		public static List<BarInfosContainer> CreateMenuItemInfos() {
			List<BarInfosContainer> result = new List<BarInfosContainer>();
			result.Add(new MenuItemCreateAllBar());
			result.Add(new BarInfosFileContainer());
			result.Add(new BarInfosHomeContainer());
			result.Add(new BarInfosViewContainer());
			result.Add(new BarInfosAppointmentContainer());
			return result;
		}
		protected BarInfosContainer() {
			Infos = new List<BarInfo>();
			Register();
		}
		public abstract string DisplayName { get; }
		public List<BarInfo> Infos { get; private set; }
		protected void RegisterBarInfo(BarInfo info) {
			Infos.Add(info);
		}
		protected abstract void Register();
		protected void CopyFrom(BarInfosContainer creationInfo) {
			Infos.AddRange(creationInfo.Infos);
		}
	}
	public class BarInfosHomeContainer : BarInfosContainer {
		public override string DisplayName { get { return "Home"; } }
		protected override void Register() {
			RegisterBarInfo(BarInfos.Appointment);
			RegisterBarInfo(BarInfos.ViewNavigator);
			RegisterBarInfo(BarInfos.ArrangeView);
			RegisterBarInfo(BarInfos.GroupSelector);
		}
	}
	public class BarInfosViewContainer : BarInfosContainer {
		public override string DisplayName { get { return "View"; } }
		protected override void Register() {
			RegisterBarInfo(BarInfos.ViewSelector);
			RegisterBarInfo(BarInfos.TimeScale);
			RegisterBarInfo(BarInfos.Layout);
		}
	}
	public class BarInfosFileContainer : BarInfosContainer {
		public override string DisplayName { get { return "File"; } }
		protected override void Register() {
			RegisterBarInfo(BarInfos.FileCommon);
		}
	}
	public class BarInfosAppointmentContainer : BarInfosContainer {
		public override string DisplayName { get { return "Appointment"; } }
		protected override void Register() {
			RegisterBarInfo(BarInfos.AppointmentActions);
			RegisterBarInfo(BarInfos.AppointmentOptions);
		}
	}
	public class MenuItemCreateAllBar : BarInfosContainer {
		public override string DisplayName { get { return "All"; } }
		protected override void Register() {
			CopyFrom(new BarInfosFileContainer());
			CopyFrom(new BarInfosHomeContainer());
			CopyFrom(new BarInfosViewContainer());
			CopyFrom(new BarInfosAppointmentContainer());
		}
	}
	public class MenuItemExecuteAdapter  {
		BarInfosContainer barCreationInfo;
		SchedulerBarsMenuCreatorBase menuCreator;
		public MenuItemExecuteAdapter(SchedulerBarsMenuCreatorBase menuCreator, BarInfosContainer barCreationInfo) {
			this.barCreationInfo = barCreationInfo;
			this.menuCreator = menuCreator;
		}
		public void Execute(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, barCreationInfo.Infos.ToArray());
		}
		void CreateBars(ModelItem primarySelection, BarInfo[] barInfos) {
			CommandBarCreator creator = this.menuCreator.CreateBarCreator();
			creator.CreateBars(primarySelection, barInfos);
		}
	}
	public abstract class SchedulerBarsMenuCreatorBase {
		protected internal abstract string SubMenuCaption { get; }
		protected internal abstract string MenuGroupId { get; }
		public void CreateMenu(System.Collections.Generic.IList<MenuBase> items) {
			MenuGroup createToolbarsMenuGroup = new MenuGroup(MenuGroupId, SubMenuCaption);
			createToolbarsMenuGroup.HasDropDown = true;
			List<BarInfosContainer> barCreationInfos = BarInfosContainer.CreateMenuItemInfos();
			foreach (BarInfosContainer infoItem in barCreationInfos) {
				MenuAction menuItem = new MenuAction(FormatMenuItemCaption(infoItem.DisplayName));
				MenuItemExecuteAdapter itemAdapter = new MenuItemExecuteAdapter(this, infoItem);
				menuItem.Execute += itemAdapter.Execute;
				createToolbarsMenuGroup.Items.Add(menuItem);
			}
			items.Add(createToolbarsMenuGroup);
		}
		protected internal abstract CommandBarCreator CreateBarCreator();
		protected internal abstract string FormatMenuItemCaption(string name);
		public static BarInfo[] GetAllBarInfos() {
			MenuItemCreateAllBar createAllBar = new MenuItemCreateAllBar();
			return createAllBar.Infos.ToArray();
		}
	}
	public class SchedulerBarsMenuCreator : SchedulerBarsMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateSchedulerBarItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Bars"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new SchedulerCommandBarCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
	public class SchedulerRibbonMenuCreator : SchedulerBarsMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateSchedulerRibbonItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Ribbon Items"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new SchedulerCommandRibbonCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
}
