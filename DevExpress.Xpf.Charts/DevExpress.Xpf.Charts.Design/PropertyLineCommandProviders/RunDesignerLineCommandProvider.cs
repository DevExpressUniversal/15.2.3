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
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Charts.Designer;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using DevExpress.Charts.Designer.Native;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using System.Windows;
namespace DevExpress.Xpf.Charts.Design {
	public class ChartDesignTimeProvider : IDesignTimeProvider {
		#region IDesignTimeProvider implementation
		IModelItem IDesignTimeProvider.CreateBindingItem(IModelItem item, string elementName) {
			return DesignTimeObjectModelCreateService.CreateBindingItem(item, null, null, elementName);
		}
		bool? IDesignTimeProvider.ShowDialog(Window window) {
			return DesignDialogHelper.ShowDialog(window);
		}
		#endregion
	}
	public class RunDesignerPropertyLineCommandProvider : CommandActionLineProvider {
		public RunDesignerPropertyLineCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
				try {
					if (Context != null && Context.ModelItem != null && Context.ModelItem.ItemType.IsAssignableFrom(typeof(ChartControl)))
						Context.ModelItem.Context.CreateItem(typeof(ChartControl));
				}
				catch {
				}
		}
		protected override string GetCommandText() {
			return "Run Designer";
		}
		protected override void OnCommandExecute(object param) {
			if (Context != null && Context.ModelItem != null && Context.ModelItem.ItemType.IsAssignableFrom(typeof(ChartControl))) {
				ChartDesigner designer = DesignerInDesignTimeHelper.CreateDesignerForDesignTime(Context.ModelItem.GetCurrentValue() as ChartControl, Context.ModelItem, new ChartDesignTimeProvider());
				var service = XpfModelItem.ToModelItem(Context.ModelItem).Context.Services.GetService<SmartTagDesignService>();
				DesignerInDesignTimeHelper.RunDesignerInDesignTime(designer);
			}
		}
	}
	public class RunDataSourceWizardPropertyLineCommandProvider : CommandActionLineProvider {
		public RunDataSourceWizardPropertyLineCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Data Source Wizard";
		}
		protected override void OnCommandExecute(object param) {
			if (Context != null && Context.ModelItem != null)
				DevExpress.Xpf.Core.Design.DataAccess.UI.ItemsSourceWizard.Run(XpfModelItem.ToModelItem(Context.ModelItem));
		}
	}
}
