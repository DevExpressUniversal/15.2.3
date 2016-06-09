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
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Design.SmartTags;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if !SL
using DevExpress.Design.SmartTags;
#else
using Platform::DevExpress.Design.SmartTags;
#endif
using System;
namespace DevExpress.Xpf.Scheduler.Design {
	public sealed class SchedulerControlPropertyLinesProvider : PropertyLinesProviderBase {
		public SchedulerControlPropertyLinesProvider() : base(typeof(Platform::DevExpress.Xpf.Scheduler.SchedulerControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			GenerateBarsLineProvider generateBarsLineProvider = new GenerateBarsLineProvider(viewModel);
			GenerateRibbonLineProvider generateRibbonLineProvider = new GenerateRibbonLineProvider(viewModel);
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(generateBarsLineProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(generateRibbonLineProvider));
			return lines;
		}
	}
	public abstract class GenerateBarsLineProviderBase : CommandActionLineProvider {
		protected GenerateBarsLineProviderBase(IPropertyLineContext context)
			: base(context) {
		}
		protected override void OnCommandExecute(object param) {
			MenuItemCreateAllBar allBar = new MenuItemCreateAllBar();
			CreateBars(Context.ModelItem, allBar.Infos.ToArray());
		}
		void CreateBars(IModelItem modelItem, BarInfo[] barInfos) {
			ModelItem modelItemInstance = XpfModelItem.ToModelItem(modelItem);
			if(modelItemInstance == null)
				return;
			CommandBarCreator creator = CreateBarCreator();
			creator.CreateBars(modelItemInstance, barInfos);
		}
		protected abstract CommandBarCreator CreateBarCreator();
	}
	public class GenerateBarsLineProvider : GenerateBarsLineProviderBase {
		public GenerateBarsLineProvider(IPropertyLineContext context)
			: base(context) {
		}
		protected override string GetCommandText() {
			return "Create Scheduler Bars";
		}
		protected override CommandBarCreator CreateBarCreator() {
			return new SchedulerCommandBarCreator();
		}
	}
	public class GenerateRibbonLineProvider : GenerateBarsLineProviderBase {
		public GenerateRibbonLineProvider(IPropertyLineContext context)
			: base(context) {
		}
		protected override string GetCommandText() {
			return "Create Scheduler Ribbon";
		}
		protected override CommandBarCreator CreateBarCreator() {
			return new SchedulerCommandRibbonCreator();
		}
	}
}
