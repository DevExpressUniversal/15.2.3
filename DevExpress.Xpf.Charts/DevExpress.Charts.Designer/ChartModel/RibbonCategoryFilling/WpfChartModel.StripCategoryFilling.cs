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
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		RibbonPageCategoryViewModel CreateStripOptionsCategory() {
			RibbonPageCategoryViewModel stripOptions = new RibbonPageCategoryViewModel(this, typeof(WpfChartStripModel));
			stripOptions.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_CategoryCaption);
			RibbonPageViewModel page = new RibbonPageViewModel(this, RibbonPagesNames.StripOptionsPage);
			page.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_PageCaption);
			stripOptions.Pages.Add(page);		  
			RibbonPageGroupViewModel generalGroup = new RibbonPageGroupViewModel();
			generalGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_GeneralGroupCaption);
			page.Groups.Add(generalGroup);
			generalGroup.BarItems.Add(CreateTextEditor_MaxLimit());
			generalGroup.BarItems.Add(CreateTextEditor_MinLimit());
			generalGroup.BarItems.Add(CreateColorEditor_Brush());
			RibbonPageGroupViewModel textGroup = new RibbonPageGroupViewModel();
			textGroup.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Strip_TextGroupCaption);
			page.Groups.Add(textGroup);
			textGroup.BarItems.Add(CreateTextEditor_AxisLabelText());
			textGroup.BarItems.Add(CreateTextEditor_LegendText());
			return stripOptions;
		}
		RibbonItemViewModelBase CreateTextEditor_MaxLimit() {
			ChangeStripMaxLimitCommand command = new ChangeStripMaxLimitCommand(this);
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new StripMaxLimitConverter());
			command.EditorModel = editor;
			return editor;
		}
		RibbonItemViewModelBase CreateTextEditor_MinLimit() {
			ChangeStripMinLimitCommand command = new ChangeStripMinLimitCommand(this);
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new StripMinLimitConverter());
			command.EditorModel = editor;
			return editor;
		}
		RibbonItemViewModelBase CreateColorEditor_Brush() {
			ChangeStripBrushCommand command = new ChangeStripBrushCommand(this);
			BarColorEditViewModel colorEditor = new BarColorEditViewModel(command, this, BindingPathToSelectedModel, new StripBrushConverter());
			colorEditor.RibbonStyle = RibbonItemStyles.Large;
			return colorEditor;
		}
		RibbonItemViewModelBase CreateTextEditor_LegendText() {
			ChangeStripLegendTextCommand command = new ChangeStripLegendTextCommand(this);
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new StripLegendTextConverter());
			return editor;
		}
		RibbonItemViewModelBase CreateTextEditor_AxisLabelText() {
			ChangeStripAxisLabelTextCommand command = new ChangeStripAxisLabelTextCommand(this);
			BarEditValueItemViewModel editor = new BarEditValueItemViewModel(command, this, BindingPathToSelectedModel, new StripAxisLabelTextConverter());
			return editor;
		}
	}
}
