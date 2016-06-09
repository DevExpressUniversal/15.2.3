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
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.Collections.Generic;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public static class FontCommandUtils {
		const string titleIndexKey = "TitleIndex";
		public static Control GetLegendFontHolder(ChartControl chartControl) {
			return chartControl.Legend;
		}
		public static IModelItem GetLegendFontHolder(IModelItem chartModelItem) {
			return chartModelItem.Properties["Legend"].Value;
		}
		static int GetTitleIndex(HistoryItem historyItem) {
			return historyItem.ExecuteCommandInfo.IndexByNameDictionary[titleIndexKey];
		}
		public static Control GetTitleFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			return chartControl.Titles[GetTitleIndex(historyItem)];
		}
		public static IModelItem GetTitleFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			return chartModelItem.Properties["Titles"].Collection[GetTitleIndex(historyItem)];
		}
		public static ElementIndexItem[] GetTitlePathIndexes(WpfChartModel model) {
			WpfChartTitleModel titleModel = (WpfChartTitleModel)model.SelectedModel;
			int index = model.Chart.Titles.IndexOf(titleModel.Title);
			return new ElementIndexItem[] { new ElementIndexItem(titleIndexKey, index) };
		}
		public static ElementIndexItem[] GetConstantLineTitlePathIndexes(Axis2D axis2D, ConstantLine constantLine, XYDiagram2D diagram ) {
			Axis2D axis;
			if (axis2D == null)
				axis = ChartDesignerPropertiesProvider.GetConstantLineOwner(constantLine);
			else
				axis = axis2D;
			List<ElementIndexItem> list = new List<ElementIndexItem>();
			list = new List<ElementIndexItem>();
			if (axis is AxisX2D)
				list.Add(new ElementIndexItem(ConstantLineCommandBase.AxisKey, -1));
			else if (axis is AxisY2D)
				list.Add(new ElementIndexItem(ConstantLineCommandBase.AxisKey, -2));
			else if (axis is SecondaryAxisX2D) {
				list.Add(new ElementIndexItem(ConstantLineCommandBase.AxisKey, -3));
				int secondaryAxisXIndex = diagram.SecondaryAxesX.IndexOf((SecondaryAxisX2D)axis);
				list.Add(new ElementIndexItem(ConstantLineCommandBase.SecondaryAxisKey, secondaryAxisXIndex));
			}
			else if (axis is SecondaryAxisY2D) {
				list.Add(new ElementIndexItem(ConstantLineCommandBase.AxisKey, -4));
				int secondaryAxisYIndex = diagram.SecondaryAxesY.IndexOf((SecondaryAxisY2D)axis);
				list.Add(new ElementIndexItem(ConstantLineCommandBase.SecondaryAxisKey, secondaryAxisYIndex));
			}
			else
				throw new ChartDesignerException("Unknown axis type for constant line.");
			int constantLineIndex;
			if (constantLine != null)
				constantLineIndex = axis.ConstantLinesInFront.IndexOf(constantLine);
			else
				constantLineIndex = -1;
			list.Add(new ElementIndexItem(ConstantLineCommandBase.ConstantLineKey, constantLineIndex));
			return list.ToArray();
		}
		public static ConstantLine GetConstantLineFontHolder(ChartControl chartControl, HistoryItem historyItem) {
			Axis2D axis = GetTargetAxisForRuntimeApply(chartControl, historyItem);
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.ConstantLineKey];
			ConstantLine targetConstantLine = axis.ConstantLinesInFront[constantLineIndex];
			return targetConstantLine;
		}
		static Axis2D GetTargetAxisForRuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.AxisKey];
			XYDiagram2D xyDiagram2D = (XYDiagram2D)chartControl.Diagram;
			Axis2D axis;
			switch (axisPseudoIndex) {
				case -1:
					axis = xyDiagram2D.AxisX;
					break;
				case -2:
					axis = xyDiagram2D.AxisY;
					break;
				case -3:
					int secondaryAxisXIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.SecondaryAxisKey];
					axis = xyDiagram2D.SecondaryAxesX[secondaryAxisXIndex];
					break;
				case -4:
					int secondaryAxisYIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.SecondaryAxisKey];
					axis = xyDiagram2D.SecondaryAxesY[secondaryAxisYIndex];
					break;
				default:
					throw new ChartDesignerException("Unknown pseudo index.");
			}
			return axis;
		}
		internal static IModelItem GetConstantLineFontHolder(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis = GetTargetAxisForDesigntimeApply(chartModelItem, historyItem);
			int constantLineIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.ConstantLineKey];
			IModelItem constantLine = axis.Properties["ConstantLinesInFront"].Collection[constantLineIndex];
			return constantLine;
		}
		static IModelItem GetTargetAxisForDesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem) {
			IModelItem xyDiagram2D = chartModelItem.Properties["Diagram"].Value;
			IModelItem axis;
			int axisPseudoIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.AxisKey];
			switch (axisPseudoIndex) {
				case -1:
					axis = xyDiagram2D.Properties["AxisX"].Value;
					break;
				case -2:
					axis = xyDiagram2D.Properties["AxisY"].Value;
					break;
				case -3:
					int secondaryAxisXIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.SecondaryAxisKey];
					axis = xyDiagram2D.Properties["SecondaryAxesX"].Collection[secondaryAxisXIndex];
					break;
				case -4:
					int secondaryAxisYIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[ConstantLineCommandBase.SecondaryAxisKey];
					axis = xyDiagram2D.Properties["SecondaryAxesY"].Collection[secondaryAxisYIndex];
					break;
				default:
					throw new ChartDesignerException("Unknown pseudo index.");
			}
			return axis;
		}
	}
	public abstract class FontCommandBase : ChartCommandBase {
		public WpfChartFontModel FontModel { get { return (WpfChartFontModel)ChartModel.SelectedModel; } }
		public ChartControl ChartControl { get { return PreviewChart; } }
		public FontCommandBase(WpfChartModel chartModel) 
			: base(chartModel) { }
		protected virtual ElementIndexItem[] GetPathIndexes() {
			return new ElementIndexItem[] { };
		}
		protected override bool CanExecute(object parameter) {
			return true;
		}
		protected abstract Control GetFontHolder(ChartControl chartControl, HistoryItem historyItem);
		protected abstract IModelItem GetFontHolder(IModelItem chartModelItem, HistoryItem historyItem);
	}
	public abstract class FontFamilyCommand : FontCommandBase {
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Font_Family); } }
		public override string ImageName { get { return null; } }
		public FontFamilyCommand(WpfChartModel chartModel) 
			: base(chartModel) { }
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontFamily = (FontFamily)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontFamily = (FontFamily)historyItem.NewValue;
			return null;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			FontFamily fontFamily = parameter as FontFamily;
			if (fontFamily == null)
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.FontHolder, FontModel.FontHolder.FontFamily, fontFamily);
			FontModel.FontHolder.FontFamily = fontFamily;
			return new CommandResult(historyItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			GetFontHolder(chartControl, historyItem).FontFamily = (FontFamily)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			GetFontHolder(chartModelItem, historyItem).Properties["FontFamily"].SetValue((FontFamily)historyItem.NewValue);
		}
	}
	public abstract class FontSizeCommand : FontCommandBase, IComboBoxItememsFiller {
		const int FontSizesCount = 40;
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Font_Size); } }
		public override string ImageName { get { return null; } }
		public FontSizeCommand(WpfChartModel chartModel) 
			: base(chartModel) { }
		#region IComoboBoxFiller implementation
		public object[] GetItems() {
			object[] fontSizes = new object[FontSizesCount];
			int fontSize = 3;
			for (int i = 0; i < fontSizes.Length; i++) {
				fontSizes[i] = fontSize;
				if (fontSize >= 80)
					fontSize += 8;
				else if (fontSize >= 40)
					fontSize += 4;
				else if (fontSize >= 24)
					fontSize += 2;
				else
					fontSize++;
			}
			return fontSizes;
		}
		#endregion
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontSize = (double)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontSize = (double)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			GetFontHolder(chartControl, historyItem).FontSize = (double)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			GetFontHolder(chartModelItem, historyItem).Properties["FontSize"].SetValue((double)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null || !(parameter is int))
				return null;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			double fontSize = (int)parameter;
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.FontHolder, FontModel.FontHolder.FontSize, fontSize);
			FontModel.FontHolder.FontSize = fontSize;
			return new CommandResult(historyItem);
		}
	}
	public interface IComboBoxItememsFiller {
		object[] GetItems();
	}
	public abstract class FontBoldCommand : FontCommandBase {
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Font_Bold); } }
		public override string ImageName { get { return GlyphUtils.BarItemImages + "FontBold"; } }
		public FontBoldCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontWeight = (FontWeight)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontWeight = (FontWeight)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			GetFontHolder(chartControl, historyItem).FontWeight = (FontWeight)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			GetFontHolder(chartModelItem, historyItem).Properties["FontWeight"].SetValue((FontWeight)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null)
				return null;
			FontWeight fontWeight = (bool)parameter ? FontWeights.Bold : FontWeights.Normal;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.FontHolder, FontModel.FontHolder.FontWeight, fontWeight);
			FontModel.FontHolder.FontWeight = fontWeight;
			return new CommandResult(historyItem);
		}
	}
	public abstract class FontItalicCommand : FontCommandBase {
		public override string Caption { get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Font_Italic); } }
		public override string ImageName { get { return GlyphUtils.BarItemImages + "FontItalic"; } }
		public FontItalicCommand(WpfChartModel chartModel) : base(chartModel) {
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontStyle = (FontStyle)historyItem.OldValue;
			return null;
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			Control target = (Control)historyItem.TargetObject;
			target.FontStyle = (FontStyle)historyItem.NewValue;
			return null;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			GetFontHolder(chartControl, historyItem).FontStyle = (FontStyle)historyItem.NewValue;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			GetFontHolder(chartModelItem, historyItem).Properties["FontStyle"].SetValue((FontStyle)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if (parameter == null)
				return null;
			FontStyle fontStyle = (bool)parameter ? FontStyles.Italic : FontStyles.Normal;
			ExecuteCommandInfo info = new ExecuteCommandInfo(parameter, GetPathIndexes());
			HistoryItem historyItem = new HistoryItem(info, this, FontModel.FontHolder, FontModel.FontHolder.FontStyle, fontStyle);
			FontModel.FontHolder.FontStyle = fontStyle;
			return new CommandResult(historyItem);
		}
	}
}
