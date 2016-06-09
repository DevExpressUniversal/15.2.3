#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
using System.Collections.Generic;
using System;
namespace DevExpress.DashboardWin.Commands {
	public class TransposeItemCommand : DashboardItemCommand<DataDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.TransposeItem; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return TransposeOperator.MenuCaptionStringId; } }
		public override DashboardWinStringId DescriptionStringId { get { return TransposeOperator.DescriptionStringId; } }
		public override string ImageName { get { return "TransposeItem"; } }
		ITransposeOperator TransposeOperator { get { return Control.SelectedItemUIManager.TransposeOperator; } }
		public TransposeItemCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = TransposeOperator.Visible;
			state.Enabled = TransposeOperator.IsEnabled(DashboardItem);
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			TransposeOperator.Execute(DashboardItem, Control);
		}
	}
	#region SelectedItemUIManagers
	public interface ISelectedItemUIManager {
		ITransposeOperator TransposeOperator { get; }
	}
	public class EmptyDashboardItemUIManager : ISelectedItemUIManager {
		static EmptyDashboardItemUIManager instance = new EmptyDashboardItemUIManager();
		EmptyDashboardItemUIManager() {
		}
		public static EmptyDashboardItemUIManager Instance { get { return instance; } }
		public ITransposeOperator TransposeOperator { get { return DefaultTransposeOperator.Instance; } }
	}
	public class ChartDashboardItemUIManager : ISelectedItemUIManager {
		public ChartDashboardItemUIManager() {
		}
		public ITransposeOperator TransposeOperator { get { return ChartTransposeOperator.Instance; } }
	}
	public class ScatterChartDashboardItemUIManager : ISelectedItemUIManager {
		public ScatterChartDashboardItemUIManager() {
		}
		public ITransposeOperator TransposeOperator { get { return ScatterChartTransposeOperator.Instance; } }
	}
	public class PieDashboardItemUIManager : ISelectedItemUIManager {
		public PieDashboardItemUIManager() {
		}
		public ITransposeOperator TransposeOperator { get { return PieTransposeOperator.Instance; } }
	}
	public class PivotDashboardItemUIManager : ISelectedItemUIManager {
		public PivotDashboardItemUIManager() {
		}
		public ITransposeOperator TransposeOperator { get { return PivotTransposeOperator.Instance; } }
	}
	#endregion
	public static class DashboardItemUIManagerFactory {
		static Dictionary<Type, ISelectedItemUIManager> dictionary = CreateTable();
		static Dictionary<Type, ISelectedItemUIManager> CreateTable() {
			Dictionary<Type, ISelectedItemUIManager> result = new Dictionary<Type, ISelectedItemUIManager>();
			result.Add(typeof(ChartDashboardItem), new ChartDashboardItemUIManager());
			result.Add(typeof(ScatterChartDashboardItem), new ScatterChartDashboardItemUIManager());
			result.Add(typeof(PieDashboardItem), new PieDashboardItemUIManager());
			result.Add(typeof(PivotDashboardItem), new PivotDashboardItemUIManager());
			return result;
		}
		public static ISelectedItemUIManager CreateManager(DashboardItem item) {
			if(item == null)
				return EmptyDashboardItemUIManager.Instance;
			Type itemType = item.GetType();
			ISelectedItemUIManager result;
			dictionary.TryGetValue(itemType, out result);
			return result == null ? EmptyDashboardItemUIManager.Instance : result;
		}
	}
	#region UICommandOperator
	public interface UICommandOperator {
		DashboardWinStringId MenuCaptionStringId { get; }
		DashboardWinStringId DescriptionStringId { get; }
		bool Visible { get; }
		bool IsEnabled(DashboardItem selectedItem);
		void Execute(DashboardItem selectedItem, DashboardDesigner control);
	}
	#endregion
	#region ITransposeOperator
	public interface ITransposeOperator : UICommandOperator {
	}
	public class ChartTransposeOperator : ITransposeOperator {
		static ChartTransposeOperator instance = new ChartTransposeOperator();
		ChartTransposeOperator() {
		}
		public static ChartTransposeOperator Instance { get { return instance; } }
		public DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTransposeItemCaption; } }
		public DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandTransposeChartDescription; } }
		public bool Visible { get { return true; } }
		public bool IsEnabled(DashboardItem selectedItem) {
			ChartDashboardItem chart = selectedItem as ChartDashboardItem;
			return chart == null ? false : chart.Arguments.Count > 0 || chart.SeriesDimensions.Count > 0;
		}
		public void Execute(DashboardItem selectedItem, DashboardDesigner control) {
			TransposeChartHistoryItem historyItem = new TransposeChartHistoryItem((ChartDashboardItem)selectedItem);
			historyItem.Redo(control);
			control.History.Add(historyItem);
		}
	}
	public class ScatterChartTransposeOperator : ITransposeOperator {
		static ScatterChartTransposeOperator instance = new ScatterChartTransposeOperator();
		ScatterChartTransposeOperator() {
		}
		public static ScatterChartTransposeOperator Instance { get { return instance; } }
		public DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTransposeItemCaption; } }
		public DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandTransposeScatterChartDescription; } }
		public bool Visible { get { return true; } }
		public bool IsEnabled(DashboardItem selectedItem) {
			ScatterChartDashboardItem scatter = selectedItem as ScatterChartDashboardItem;
			return scatter != null && scatter.AxisXMeasure != null && scatter.AxisYMeasure != null;
		}
		public void Execute(DashboardItem selectedItem, DashboardDesigner control) {
			TransposeScatterChartHistoryItem historyItem = new TransposeScatterChartHistoryItem((ScatterChartDashboardItem)selectedItem);
			historyItem.Redo(control);
			control.History.Add(historyItem);
		}
	}
	public class PivotTransposeOperator : ITransposeOperator {
		static PivotTransposeOperator instance = new PivotTransposeOperator();
		PivotTransposeOperator() {
		}
		public static PivotTransposeOperator Instance { get { return instance; } }
		public DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTransposeItemCaption; } }
		public DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandTransposePivotDescription; } }
		public bool Visible { get { return true; } }
		public bool IsEnabled(DashboardItem selectedItem) {
			PivotDashboardItem pivot = selectedItem as PivotDashboardItem;
			return pivot == null ? false : pivot.Columns.Count > 0 || pivot.Rows.Count > 0;
		}
		public void Execute(DashboardItem selectedItem, DashboardDesigner control) {
			TransposePivotHistoryItem historyItem = new TransposePivotHistoryItem((PivotDashboardItem)selectedItem);
			historyItem.Redo(control);
			control.History.Add(historyItem);
		}
	}
	public class PieTransposeOperator : ITransposeOperator {
		static PieTransposeOperator instance = new PieTransposeOperator();
		PieTransposeOperator() {
		}
		public static PieTransposeOperator Instance { get { return instance; } }
		public DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTransposeItemCaption; } }
		public DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandTransposePieDescription; } }
		public bool Visible { get { return true; } }
		public bool IsEnabled(DashboardItem selectedItem) {
			PieDashboardItem pie = selectedItem as PieDashboardItem;
			return pie == null ? false : pie.Arguments.Count > 0 || pie.SeriesDimensions.Count > 0;
		}
		public void Execute(DashboardItem selectedItem, DashboardDesigner control) {
			TransposePieHistoryItem historyItem = new TransposePieHistoryItem((PieDashboardItem)selectedItem);
			historyItem.Redo(control);
			control.History.Add(historyItem);
		}
	}
	public class DefaultTransposeOperator : ITransposeOperator {
		static DefaultTransposeOperator instance = new DefaultTransposeOperator();
		DefaultTransposeOperator() {
		}
		public static DefaultTransposeOperator Instance { get { return instance; } }
		public DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTransposeItemCaption; } }
		public DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandDefaultTransposeItemDescription; } }
		public bool Visible { get { return false; } }
		public bool IsEnabled(DashboardItem selectedItem) {
			return false;
		}
		public void Execute(DashboardItem selectedItem, DashboardDesigner control) {
		}
	}
	#endregion
}
