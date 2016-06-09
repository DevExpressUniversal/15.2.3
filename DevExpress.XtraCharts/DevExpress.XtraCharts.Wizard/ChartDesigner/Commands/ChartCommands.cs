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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AddChartTitleCommand : AddCommandBase<ChartTitle> {
		readonly ChartTitleCollection chartTitleCollection;
		protected override ChartCollectionBase ChartCollection { get { return chartTitleCollection; } }
		public AddChartTitleCommand(CommandManager commandManager, ChartTitleCollection chartTitleCollection)
			: base(commandManager) {
			this.chartTitleCollection = chartTitleCollection;
		}
		protected override ChartTitle CreateChartElement(object parameter) {
			return new ChartTitle();
		}
		protected override void AddToCollection(ChartTitle chartElement) {
			chartTitleCollection.Add(chartElement);
		}
	}
	public class DeleteChartTitleCommand : DeleteCommandBase<ChartTitle> {
		readonly ChartTitleCollection chartTitleCollection;
		protected override ChartCollectionBase ChartCollection { get { return chartTitleCollection; } }
		public DeleteChartTitleCommand(CommandManager commandManager, ChartTitleCollection chartTitleCollection)
			: base(commandManager) {
			this.chartTitleCollection = chartTitleCollection;
		}
		protected override void InsertIntoCollection(int index, ChartTitle chartElement) {
			chartTitleCollection.Insert(index, chartElement);
		}
	}
	public class AddAnnotationCommand : AddCommandBase<Annotation> {
		readonly AnnotationRepository annotationRepository;
		protected override ChartCollectionBase ChartCollection { get { return annotationRepository; } }
		public AddAnnotationCommand(CommandManager commandManager, AnnotationRepository annotationRepository)
			: base(commandManager) {
			this.annotationRepository = annotationRepository;
		}
		protected override Annotation CreateChartElement(object parameter) {
			AnnotationType type = (AnnotationType)parameter;
			switch (type) {
				case AnnotationType.Text:
					return new TextAnnotation();
				case AnnotationType.Image:
					return new ImageAnnotation();
				default:
					ChartDebug.Fail("Unknown annotation type: " + type);
					return null;
			}
		}
		protected override void AddToCollection(Annotation chartElement) {
			annotationRepository.Add(chartElement);
		}
		public override bool CanExecute(object parameter) {
			return parameter is AnnotationType;
		}
	}
	public class DeleteAnnotationCommand : DeleteCommandBase<Annotation> {
		readonly AnnotationRepository annotationRepository;
		protected override ChartCollectionBase ChartCollection { get { return annotationRepository; } }
		public DeleteAnnotationCommand(CommandManager commandManager, AnnotationRepository annotationRepository)
			: base(commandManager) {
			this.annotationRepository = annotationRepository;
		}
		protected override void InsertIntoCollection(int index, Annotation chartElement) {
			annotationRepository.Insert(index, chartElement);
		}
	}
	public class SetDataMemberCommand : ChartCommandBase {
		public SetDataMemberCommand(CommandManager commandManager)
			: base(commandManager) {
		}
		string GetDataMemberOldValue(DesignerChartElementModelBase selectedModel, string dataMemberName) {
			DesignerChartModel chartModel = selectedModel as DesignerChartModel;
			if (dataMemberName == "SeriesDataMember") {
				if (chartModel == null)
					chartModel = selectedModel.FindParent<DesignerChartModel>();
				return ((Chart)chartModel.ChartElement).DataContainer.SeriesDataMember;
			}
			else {
				DesignerSeriesModelBase seriesModel = selectedModel as DesignerSeriesModelBase;
				if (seriesModel == null)
					seriesModel = chartModel.SeriesTemplate;
				switch (dataMemberName) {
					case "ArgumentDataMember":
						return seriesModel.SeriesBase.ArgumentDataMember;
					case "ValueDataMember":
					case "Value1DataMember":
					case "LowValueDataMember":
						return seriesModel.SeriesBase.ValueDataMembers[0];
					case "Value2DataMember":
					case "WeightDataMember":
					case "HighValueDataMember":
						return seriesModel.SeriesBase.ValueDataMembers[1];
					case "OpenValueDataMember":
						return seriesModel.SeriesBase.ValueDataMembers[2];
					case "CloseValueDataMember":
						return seriesModel.SeriesBase.ValueDataMembers[3];
					case "ColorDataMember":
						return seriesModel.SeriesBase.ColorDataMember;
					case "ToolTipHintDataMember":
						return seriesModel.SeriesBase.ToolTipHintDataMember;
				}
			}
			return string.Empty;
		}
		void SetDataMember(DesignerChartElementModelBase selectedModel, string dataMemberName, string value) {
			DesignerChartModel chartModel = selectedModel as DesignerChartModel;
			if (dataMemberName == "SeriesDataMember") {
				if (chartModel == null)
					chartModel = selectedModel.FindParent<DesignerChartModel>();
				((Chart)chartModel.ChartElement).DataContainer.SeriesDataMember = value;
			}
			else {
				DesignerSeriesModelBase seriesModel = selectedModel as DesignerSeriesModelBase;
				if (seriesModel == null)
					seriesModel = chartModel.SeriesTemplate;
				switch (dataMemberName) {
					case "ArgumentDataMember":
						seriesModel.SeriesBase.ArgumentDataMember = value;
						break;
					case "ValueDataMember":
					case "Value1DataMember":
					case "LowValueDataMember":
						seriesModel.SeriesBase.ValueDataMembers[0] = value;
						break;
					case "Value2DataMember":
					case "WeightDataMember":
					case "HighValueDataMember":
						seriesModel.SeriesBase.ValueDataMembers[1] = value;
						break;
					case "OpenValueDataMember":
						seriesModel.SeriesBase.ValueDataMembers[2] = value;
						break;
					case "CloseValueDataMember":
						seriesModel.SeriesBase.ValueDataMembers[3] = value;
						break;
					case "ColorDataMember":
						try {
							seriesModel.SeriesBase.ColorDataMember = value;
							break;
						}
						finally {							
						}
					case "ToolTipHintDataMember":
						seriesModel.SeriesBase.ToolTipHintDataMember = value;
						break;
				}
			}
		}
		public override bool CanExecute(object parameter) {
			SetDataMemberCommandParameter commandParameter = parameter as SetDataMemberCommandParameter;
			if (commandParameter != null)
				return commandParameter.SelectedModel is DesignerSeriesModelBase || commandParameter.SelectedModel is DesignerChartModel;
			else
				return false;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			SetDataMemberCommandParameter commandParameter = parameter as SetDataMemberCommandParameter;
			if (commandParameter != null) {
				string oldValue = GetDataMemberOldValue(commandParameter.SelectedModel, commandParameter.DataMemberName);
				try {
					SetDataMember(commandParameter.SelectedModel, commandParameter.DataMemberName, commandParameter.Value);
				}
				catch (Exception e) {
					SetDataMember(commandParameter.SelectedModel, commandParameter.DataMemberName, oldValue);
					XtraMessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}
				HistoryItem hItem = new HistoryItem(this, oldValue, commandParameter.Value, commandParameter);
				return hItem;
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			SetDataMemberCommandParameter commandParameter = historyItem.Parameter as SetDataMemberCommandParameter;
			if (commandParameter != null)
				SetDataMember(commandParameter.SelectedModel, commandParameter.DataMemberName, (string)historyItem.OldValue);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			SetDataMemberCommandParameter commandParameter = historyItem.Parameter as SetDataMemberCommandParameter;
			if (commandParameter != null)
				SetDataMember(commandParameter.SelectedModel, commandParameter.DataMemberName, (string)historyItem.NewValue);
		}
	}
	public class SetDataMemberCommandParameter {
		readonly string dataMemberName;
		readonly string value;
		readonly DesignerChartElementModelBase selectedModel;
		public string DataMemberName { get { return dataMemberName; } }
		public string Value { get { return value; } }
		public DesignerChartElementModelBase SelectedModel { get { return selectedModel; } }
		public SetDataMemberCommandParameter(string dataMemberName, string value, DesignerChartElementModelBase selectedModel) {
			this.dataMemberName = dataMemberName;
			this.value = value;
			this.selectedModel = selectedModel;
		}
	}
}
