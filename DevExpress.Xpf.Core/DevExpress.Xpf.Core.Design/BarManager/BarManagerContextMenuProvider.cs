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
using Platform::DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Core.Design {
	enum BarType {
		Bar,
		MainMenuBar,
		StatusBar
	}
	class BarManagerContextMenuProvider : PrimarySelectionContextMenuProvider {
		MenuGroup AddBarGroup { get; set; }
		public BarManagerContextMenuProvider() {
			AddBarGroup = new MenuGroup("Add Bar") { HasDropDown = true };
			AddBarGroup.Items.Add(new AddBarMenuAction("Bar", BarType.Bar));
			AddBarGroup.Items.Add(new AddBarMenuAction("Main Menu", BarType.MainMenuBar));
			AddBarGroup.Items.Add(new AddBarMenuAction("Status Bar", BarType.StatusBar));
			Items.Add(AddBarGroup);
		}
	}
	class AddBarMenuAction : MenuAction {
		BarType Type { get; set; }
		public AddBarMenuAction(string displayName, BarType barType)
			: base(displayName) {
			Type = barType;
			Execute += OnExecute;
		}
		void OnExecute(object sender, MenuActionEventArgs e) {
			ModelItem barManager = e.Selection.PrimarySelection;
			ModelItem newBar = ModelFactory.CreateItem(e.Context, typeof(Bar), CreateOptions.InitializeDefaults);
			using(ModelEditingScope scope = barManager.BeginEdit("Add Bar")) {
				newBar.Name = BarManagerDesignTimeHelper.GetUniqueName(newBar);
				newBar.Properties["Caption"].SetValue(newBar.Name);
				switch(Type) {
					case BarType.Bar:
						newBar = BarManagerDesignTimeHelper.ConvertToSimpleBar(newBar);
						break;
					case BarType.MainMenuBar:
						newBar = BarManagerDesignTimeHelper.ConvertToMainMenuBar(newBar);
						break;
					case BarType.StatusBar:
						newBar = BarManagerDesignTimeHelper.ConvertToStatusBar(newBar);
						break;
				}
				BarManagerDesignTimeHelper.AddBar(barManager, newBar);
				newBar.ResetLayout();
				scope.Complete();
			}
			SelectionHelper.SetSelection(e.Context, newBar);
		}
	}
}
