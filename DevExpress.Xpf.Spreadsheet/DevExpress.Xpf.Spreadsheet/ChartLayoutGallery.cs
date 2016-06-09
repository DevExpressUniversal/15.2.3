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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Spreadsheet.UI {
	#region ChartLayoutGalleryGroupInfo
	public class ChartLayoutGalleryGroupInfo {
		SpreadsheetControl control;
		ChartPresetCategory presetCategory;
		public ChartLayoutGalleryGroupInfo(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			SubscribeControlEvents();
			this.Items = new ObservableCollection<ChartLayoutGalleryItemInfo>();
		}
		public string Caption { get; set; }
		public ObservableCollection<ChartLayoutGalleryItemInfo> Items { get; private set; }
		void OnUpdateUI(object sender, EventArgs e) {
			if (control == null || control.InnerControl == null || control.InnerControl.IsDisposed) {
				ClearControl();
				return;
			}
			PopulateItems();
		}
		void SubscribeControlEvents() {
			if (control == null)
				return;
			control.UpdateUI += OnUpdateUI;
			control.Unloaded += SpreadsheetControlUnloaded;
		}
		void SpreadsheetControlUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			ClearControl();
		}
		void UnsubscribeControlEvents() {
			if (control == null)
				return;
			control.UpdateUI -= OnUpdateUI;
			control.Unloaded -= SpreadsheetControlUnloaded;
		}
		void ClearControl() {
			UnsubscribeControlEvents();
			Items.Clear();
			control = null;
		}
		void PopulateItems() {
			ModifyChartLayoutCommand command = new ModifyChartLayoutCommand(control);
			ChartPresetCategory chartPresetCategory = command.CalculateChartPresetCategory();
			if (chartPresetCategory == presetCategory)
				return;
			this.presetCategory = chartPresetCategory;
			IList<ChartLayoutModifier> modifiers = ChartLayoutModifier.GetModifiers(this.presetCategory);
			Items.Clear();
			if (presetCategory == ChartPresetCategory.None)
				return;
			int count = modifiers.Count;
			for (int i = 0; i < count; i++)
				Items.Add(new ChartLayoutGalleryItemInfo(control, modifiers[i], i + 1));
		}
	}
	#endregion
	#region ChartLayoutGalleryItemInfo
	public class ChartLayoutGalleryItemInfo {
		#region Fields
		readonly SpreadsheetControl control;
		ChartLayoutModifier modifier;
		string caption;
		string description;
		string hint;
		ModifyChartLayoutCommand command;
		ModifyChartLayoutSpreadsheetUICommand uiCommand;
		ImageSource imageSource;
		#endregion
		public ChartLayoutGalleryItemInfo(SpreadsheetControl control, ChartLayoutModifier modifier, int index) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.modifier = modifier;
			this.command = new ModifyChartLayoutCommand(control);
			this.command.Modifier = modifier;
			this.imageSource = null;
			this.uiCommand = new ModifyChartLayoutSpreadsheetUICommand(modifier);
			this.caption = control.CommandManager.ReplaceShortcuts(command.MenuCaption);
			this.description = command.Description;
			this.hint = "Layout " + index.ToString(); 
			if (!String.IsNullOrEmpty(command.ImageName))
				this.imageSource = ImageHelper.CreatePlatformImage(command.LargeImage).Source;
		}
		#region Properties
		public ICommand Command { get { return uiCommand; } }
		public object CommandParameter { get { return control; } }
		public string Caption { get { return caption; } }
		public string Description { get { return description; } }
		public string Hint { get { return hint; } }
		public ImageSource Image { get { return imageSource; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ModifyChartLayoutSpreadsheetUICommand
	public class ModifyChartLayoutSpreadsheetUICommand : SpreadsheetUICommand {
		readonly ChartLayoutModifier modifier;
		public ModifyChartLayoutSpreadsheetUICommand(ChartLayoutModifier modifier)
			: base(SpreadsheetCommandId.ModifyChartLayout) {
			this.modifier = modifier;
		}
		protected internal override void ExecuteCommand(SpreadsheetControl control, SpreadsheetCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, modifier);
		}
	}
	#endregion
}
