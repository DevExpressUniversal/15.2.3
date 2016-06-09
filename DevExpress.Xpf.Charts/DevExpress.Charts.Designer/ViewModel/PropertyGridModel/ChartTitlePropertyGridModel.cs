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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Charts.Designer.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.PropertyGrid;
using DevExpress.Mvvm.Native;
namespace DevExpress.Charts.Designer.Native {
	public class ChartTitleItemInitializer : IInstanceInitializer {
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartTitlePropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewTitle))
				};
			}
		}
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return Activator.CreateInstance(type.Type);
		}
	}
	public class ChartTitlePropertyGridModelCollection : ObservableCollection<WpfChartTitlePropertyGridModel> {
		WpfChartModel chartModel;
		public ChartTitlePropertyGridModelCollection(WpfChartModel chartModel) {
			this.chartModel = chartModel;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (WpfChartTitlePropertyGridModel newChartTitle in e.NewItems)
						if (newChartTitle.TitleModel == null) {
							AddChartTitleCommand command = new AddChartTitleCommand(chartModel, newChartTitle.Dock, newChartTitle.HorizontalAlignment, newChartTitle.VerticalAlignment);
							((ICommand)command).Execute(null);
							break;
						}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (WpfChartTitlePropertyGridModel oldChartTitle in e.OldItems) {
						if (chartModel.TitleCollectionModel.ModelCollection.Contains(oldChartTitle.TitleModel)) {
							RemoveChartTitleCommand command = new RemoveChartTitleCommand(chartModel);
							((ICommand)command).Execute(oldChartTitle.Title);
							break;
						}
					}
					break;
				default:
					break;
			}
		}
	}
	public abstract class WpfChartTitleBasePropertyGridModel : NestedElementPropertyGridModelBase {
		readonly TitleBase title;
		protected internal virtual TitleBase Title { get { return title; } }
		[Category(Categories.Common)]
		public string Content {
			get { return Title.Content.ToString(); }
			set { SetProperty("Content", value); }
		}
		[Category(Categories.Behavior)]
		public bool? Visible {
			get { return Title.Visible; }
			set { SetProperty("Visible", value); }
		}
		[Category(Categories.Layout)]
		public HorizontalAlignment HorizontalAlignment {
			get { return Title != null ? Title.HorizontalAlignment : HorizontalAlignment.Center; }
			set { SetProperty("HorizontalAlignment", value); }
		}
		[Category(Categories.Layout)]
		public VerticalAlignment VerticalAlignment {
			get { return Title != null ? Title.VerticalAlignment : VerticalAlignment.Top; }
			set { SetProperty("VerticalAlignment", value); }
		}
		public WpfChartTitleBasePropertyGridModel(ChartModelElement modelElement, TitleBase title, string propertyPath)
			: base(modelElement, propertyPath) {
			this.title = title;
		}
	}
	public class WpfChartTitlePropertyGridModel : WpfChartTitleBasePropertyGridModel {
		SetChartTitlePropertyCommand setPropertyCommand;
		Title ChartTitle { get { return Title as Title; } }
		internal WpfChartTitleModel TitleModel { get { return ModelElement as WpfChartTitleModel; } }
		protected internal override TitleBase Title { get { return TitleModel != null ? TitleModel.Title as Title : null; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		[Category(Categories.Layout)]
		public Dock Dock {
			get { return ChartTitle != null ? ChartTitle.Dock : Dock.Top; }
			set { SetProperty("Dock", value); }
		}
		public WpfChartTitlePropertyGridModel()
			: this(null) {
		}
		public WpfChartTitlePropertyGridModel(WpfChartTitleModel titleModel)
			: base(titleModel, null, String.Empty) {
		}
		protected override void UpdateCommands() {
			base.UpdateCommands();
			setPropertyCommand = new SetChartTitlePropertyCommand(ChartModel);
		}
	}
}
