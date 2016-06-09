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
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Bars;
using System.Windows;
using Platform::System.Windows.Controls;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Design;
#if SILVERLIGHT
using Platform::System.Windows.Media;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Core.Design {
	class BarManagerInitializer : DefaultInitializer {
		const string MainMenuName = "mainMenuBar";
		const string StatusBarName = "statusBar";
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			using(ModelEditingScope batchedChange = item.BeginEdit("Creates an BarManager")) {
				item.ResetLayout();
				item.Properties["Width"].SetValue(200d);
				item.Properties["Height"].SetValue(200d);
				ModelService service = context.Services.GetRequiredService<ModelService>();
				if(service.FromName(item.Root, MainMenuName) == null) {
					ModelItem newMainMenu = ModelFactory.CreateItem(context, typeof(Bar), CreateOptions.None);
					newMainMenu = BarManagerDesignTimeHelper.ConvertToMainMenuBar(newMainMenu);
					newMainMenu.Properties["Name"].SetValue(MainMenuName);
					newMainMenu.Properties["Caption"].SetValue("Main Menu");
					item.Properties["Bars"].Collection.Add(newMainMenu);
				}
				ModelItem newBar = ModelFactory.CreateItem(context, typeof(Bar), CreateOptions.InitializeDefaults);
				newBar = BarManagerDesignTimeHelper.ConvertToSimpleBar(newBar);
				newBar.ResetLayout();
				newBar.Properties["Caption"].SetValue(BarManagerDesignTimeHelper.GetUniqueName(newBar));
				item.Properties["Bars"].Collection.Add(newBar);
				if(service.FromName(item.Root, StatusBarName) == null) {
					ModelItem newStatusBar = ModelFactory.CreateItem(context, typeof(Bar), CreateOptions.None);
					newStatusBar = BarManagerDesignTimeHelper.ConvertToStatusBar(newStatusBar);
					newStatusBar.Properties["Name"].SetValue(StatusBarName);
					newStatusBar.Properties["Caption"].SetValue("Status Bar");
					item.Properties["Bars"].Collection.Add(newStatusBar);
				}
				ModelItem grid = ModelFactory.CreateItem(context, typeof(Grid), CreateOptions.InitializeDefaults);
#if SILVERLIGHT
				grid.Properties["Background"].SetValue(new SolidColorBrush(Colors.Transparent));
#else
				grid.Properties["Background"].SetValue(Brushes.Transparent);
#endif
				grid.ResetLayout();
				item.Content.SetValue(grid);
				batchedChange.Complete();
			}
			InitializerHelper.Initialize(item);
		}
	}
	class BarItemInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			using(ModelEditingScope batchedChange = item.BeginEdit("Creates an BarManager")) {
				try {
#if SL
					item.Properties["VerticalAlignment"].ClearValue();
					item.Properties["HorizontalAlignment"].ClearValue();
#endif
					string content = string.IsNullOrEmpty(item.Name) ? item.ItemType.Name : item.Name;
					item.Properties["Content"].SetValue(content);
				} catch { }
				batchedChange.Complete();
			}
		}
	}
}
