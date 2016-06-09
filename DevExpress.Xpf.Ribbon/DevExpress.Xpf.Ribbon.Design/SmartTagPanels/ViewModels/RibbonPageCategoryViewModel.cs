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

using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Ribbon.Design.SmartTagPanels.ViewModels {
	sealed class RibbonPageCategoryPropertyLinesProvider : RibbonItemPropertyLinesProvider {
		public RibbonPageCategoryPropertyLinesProvider() : base(typeof(RibbonPageCategory)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ColorPropertyLineViewModel(viewModel, RibbonPageCategory.ColorProperty.Name));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new RibbonPageCategoryActionPropertyLineCommandProvider(viewModel)));
			return lines;
		}
	}
	class RibbonPageCategoryActionPropertyLineCommandProvider : CommandActionLineProvider {
		public RibbonPageCategoryActionPropertyLineCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Add RibbonPage";
		}
		protected override void OnCommandExecute(object param) {
			XpfModelItem runTimeModelItem = Context.ModelItem as XpfModelItem;
			if(runTimeModelItem == null) return;
			RibbonDesignTimeHelper.AddRibbonPage(runTimeModelItem.Value);
		}
	}
}
