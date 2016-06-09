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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Ribbon.Design.SmartTagPanels.ViewModels;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Ribbon.Design {
	static class RibbonPropertyLineRegistrator {
		public static void RegisterPropertyLines() {
			BarsActionListPropertyLineContext.BarItemAdding += BarsActionListPropertyLineContext_BarItemAdding;
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new DXRibbonWindowPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RibbonPropertyLineProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RibbonPageCategoryPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RibbonPagePropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RibbonPageGroupPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new RibbonStatusBarPropertyLinesProvider());
		}
		static void BarsActionListPropertyLineContext_BarItemAdding(object sender, LinksHolderItemTypeEventArgs e) {
			bool isRibbonItem = e.ItemType.IsAssignableFrom(typeof(BarButtonGroup)) || e.ItemType.IsAssignableFrom(typeof(RibbonGalleryBarItem));
			if(isRibbonItem) {
				if(e.Context.ModelItem.ItemType.IsAssignableFrom(typeof(RibbonControl))) {
					e.SkipItem = true;
				} else {
					ModelItem ribbon = RibbonDesignTimeHelper.FindRibbonCotnrol(XpfModelItem.ToModelItem(e.Context.ModelItem));
					e.SkipItem = ribbon == null;
				}
			} else 
				e.SkipItem = false;
		}
	}
}
