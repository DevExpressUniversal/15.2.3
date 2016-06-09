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
using System.Text;
using Platform::DevExpress.XtraRichEdit;
using Platform::DevExpress.Xpf.RichEdit;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Design.SmartTags;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;
using System.Windows.Input;
using System;
namespace DevExpress.Xpf.RichEdit.Design {
	public class RichEditCommandBarCreator : CommandBarCreator {
		public override System.Type CommandsType { get { return typeof(RichEditUICommand); } }
		public override System.Type StringIdConverter { get { return typeof(RichEditStringIdConverter); } }
		public override bool GenerateCommandParameter { get { return false; } }
		protected override BarInfoItems GetBarInfoItems(BarInfo barInfo) {
			CompositeBarInfo compositeBarInfo = barInfo as CompositeBarInfo;
			if (compositeBarInfo != null)
				return compositeBarInfo.BarItems;
			return base.GetBarInfoItems(barInfo);
		}
		protected override void BindBarManagerToMasterControl(ModelItem barManager, ModelItem masterControl) {
#if !SL
			base.BindBarManagerToMasterControl(barManager, masterControl);
#else
			CreatorHelper.BindItemToControl(barManager, masterControl, "BarManager");
#endif
		}
	}
	public class RequestActionEventArgs : EventArgs {
		public RequestActionEventArgs(ModelItem modelItem) {
			ModelItem = modelItem;
		}
		public ModelItem ModelItem { get; private set; }
		public Action Action { get; internal set; }
	}
	public class RichEditActionListMenuItemInfo : MenuItemInfo {
		EventHandler<RequestActionEventArgs> onRequestAction;
		internal event EventHandler<RequestActionEventArgs> RequestAction { add { onRequestAction += value; } remove { onRequestAction -= value; } }
		public Action GetAction(ModelItem modelItem) {
			EventHandler<RequestActionEventArgs> handler = this.onRequestAction;
			if (handler != null) {
				RequestActionEventArgs args = new RequestActionEventArgs(modelItem);
				handler(this, args);
				return args.Action;
			}
			return null;
		}
	}
	public abstract class RichEditActionListCreator : RichEditBarsMenuCreatorBase<MenuItemInfo> {
		ICommand command;
		public void CreateMenu(List<MenuItemInfo> items, ICommand command) {
			this.command = command;
			PopulateItems(items);
			this.command = null;
		}
		protected override MenuItemInfo CreateMenuItem(string caption, BarInfo[] barInfos) {
			RichEditActionListMenuItemInfo result = new RichEditActionListMenuItemInfo();
			result.Caption = caption;
			result.Command = this.command;
			result.RequestAction += (sender, e) => e.Action = () => CreateBars(e.ModelItem, barInfos);
			return result;
		}
	}
	public class RichEditBarsActionListCreator : RichEditActionListCreator {
		protected internal override CommandBarCreator CreateBarCreator() {
			return new RichEditCommandBarCreator();
		}
	}
	public class RichEditRibbonActionListCreator : RichEditActionListCreator {
		protected internal override CommandBarCreator CreateBarCreator() {
			return new RichEditCommandRibbonCreator();
		}
	}
	public abstract class RichEditActionListPropertyLineContext : ActionListPropertyLineContext {
		protected RichEditActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) {
		}
		protected override void InitializeItems() {
			List<MenuItemInfo> items = new List<MenuItemInfo>();
			RichEditActionListCreator creator = CreateActionListCreator();
			creator.CreateMenu(items, SetSelectedItemCommand);
			Items = items;
			SelectedItem = items.Count > 0 ? items[0] : null;
		}
		protected override void OnSelectedItemChanged(MenuItemInfo oldValue) {
			foreach (MenuItemInfo info in Items)
				info.IsChecked = false;
			SelectedItem.IsChecked = true;
		}
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			ModelItem targetItem = XpfModelItem.ToModelItem(Context.ModelItem);
			Action action = ((RichEditActionListMenuItemInfo)SelectedItem).GetAction(targetItem);
			if (action != null)
				action();
		}
		protected abstract RichEditActionListCreator CreateActionListCreator();
	}
	public class RichEditBarsActionListPropertyLineContext : RichEditActionListPropertyLineContext {
		public RichEditBarsActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) {
		}
		protected override RichEditActionListCreator CreateActionListCreator() {
			return new RichEditBarsActionListCreator();
		}
	}
	public class RichEditRibbonActionListPropertyLineContext : RichEditActionListPropertyLineContext {
		public RichEditRibbonActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) {
		}
		protected override RichEditActionListCreator CreateActionListCreator() {
			return new RichEditRibbonActionListCreator();
		}
	}
	public class RichEditPropertyLinesProvider : PropertyLinesProviderBase {
		public RichEditPropertyLinesProvider()
			: base(typeof(RichEditControl)) {
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ActionListPropertyLineViewModel(new RichEditBarsActionListPropertyLineContext(viewModel)) { Text = "Create Bars:" });
			lines.Add(() => new ActionListPropertyLineViewModel(new RichEditRibbonActionListPropertyLineContext(viewModel)) { Text = "Create Ribbon Items:" });
			return lines;
		}
	}
}
