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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Mvvm.Native;
namespace DevExpress.Charts.Designer.Native {
	public class PaneItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartPanePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewPane)),
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartPanePropertyGridModel();
		}
	}
	public class PanePropertyGridModelCollection : PropertyGridModelCollectionBase {
		WpfChartModel chartModel;
		public PanePropertyGridModelCollection(WpfChartModel chartModel) {
			this.chartModel = chartModel;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (WpfChartPanePropertyGridModel newPane in e.NewItems)
						if (newPane.PaneModel == null) {
							AddPaneCommandBase command;
							if (chartModel.DiagramModel.PaneOrientation == Orientation.Horizontal)
								command = new AddPaneHorizontalCommand(chartModel);
							else
								command = new AddPaneVerticalCommand(chartModel);
							((ICommand)command).Execute(null);
							break;
						}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (WpfChartPanePropertyGridModel oldPane in e.OldItems) {
						if (chartModel.DiagramModel.PanesCollectionModel.ModelCollection.Contains(oldPane.PaneModel)) {
							RemovePaneCommand command = new RemovePaneCommand(chartModel);
							((ICommand)command).Execute(oldPane.Pane);
							break;
						}
					}
					break;
				default:
					break;
			}
		}
	}
	public class WpfChartPanePropertyGridModel : PropertyGridModelBase {
		SetPanePropertyCommand setPropertyCommand;
		WpfChartScrollBarOptionsPropertyGridModel axisXScrollBarOptions;
		WpfChartScrollBarOptionsPropertyGridModel axisYScrollBarOptions;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected internal WpfChartPaneModel PaneModel { get { return ModelElement as WpfChartPaneModel; } }
		protected internal Pane Pane { get { return PaneModel.Pane; } }
		[Category(Categories.Navigation)]
		public bool? EnableAxisXNavigation {
			get { return Pane.EnableAxisXNavigation; }
			set { SetProperty("EnableAxisXNavigation", value); }
		}
		[Category(Categories.Navigation)]
		public bool? EnableAxisYNavigation {
			get { return Pane.EnableAxisYNavigation; }
			set { SetProperty("EnableAxisYNavigation", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Navigation)
		]
		public WpfChartScrollBarOptionsPropertyGridModel AxisXScrollBarOptions {
			get { return axisXScrollBarOptions; }
			set { SetProperty("AxisXScrollBarOptions", new ScrollBarOptions()); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Navigation)
		]
		public WpfChartScrollBarOptionsPropertyGridModel AxisYScrollBarOptions {
			get { return axisYScrollBarOptions; }
			set { SetProperty("AxisYScrollBarOptions", new ScrollBarOptions()); }
		}
		[Category(Categories.Brushes)]
		public Brush DomainBrush {
			get { return Pane.DomainBrush; }
			set { SetProperty("DomainBrush", value); }
		}
		[Category(Categories.Brushes)]
		public Brush DomainBorderBrush {
			get { return Pane.DomainBorderBrush; }
			set { SetProperty("DomainBorderBrush", value); }
		}
		[Category(Categories.Behavior)]
		public double MirrorHeight {
			get { return Pane.MirrorHeight; }
			set { SetProperty("MirrorHeight", value); }
		}
		public WpfChartPanePropertyGridModel() : this(null) { 
		}
		public WpfChartPanePropertyGridModel(WpfChartPaneModel paneModel) : base(paneModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (PaneModel == null || PaneModel.Pane == null)
				return;
			if (Pane.AxisXScrollBarOptions != null) {
				if (axisXScrollBarOptions != null && Pane.AxisXScrollBarOptions != axisXScrollBarOptions.ScrollBarOptions || axisXScrollBarOptions == null)
					axisXScrollBarOptions = new WpfChartScrollBarOptionsPropertyGridModel(PaneModel, Pane.AxisXScrollBarOptions, setPropertyCommand, "AxisXScrollBarOptions.");
			}
			else
				axisXScrollBarOptions = null;
			if (Pane.AxisYScrollBarOptions != null) {
				if (axisYScrollBarOptions != null && Pane.AxisYScrollBarOptions != axisYScrollBarOptions.ScrollBarOptions || axisYScrollBarOptions == null)
					axisYScrollBarOptions = new WpfChartScrollBarOptionsPropertyGridModel(PaneModel, Pane.AxisYScrollBarOptions, setPropertyCommand, "AxisYScrollBarOptions.");
			}
			else
				axisYScrollBarOptions = null;
		}
		protected override void UpdateCommands() {
			base.UpdateCommands();
			setPropertyCommand = new SetPanePropertyCommand(ChartModel);
		}
	}
}
