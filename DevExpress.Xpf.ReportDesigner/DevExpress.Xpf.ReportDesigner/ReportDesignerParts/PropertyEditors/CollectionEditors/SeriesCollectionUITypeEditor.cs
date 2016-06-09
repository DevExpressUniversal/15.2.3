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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraCharts;
using DevExpress.Xpf.DataAccess.Editors.CollectionUITypeEditors;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	#region Inner classes
	public class PointItem : INotifyPropertyChanged {
		public PointItem() {
			Point = new SeriesPoint() { Argument = "0" };
			this.openAnnotationCollectionEditorCommand = DelegateCommandFactory.Create(OpenAnnotationCollectionEditor);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName = null) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		readonly ICommand openAnnotationCollectionEditorCommand;
		public ICommand OpenAnnotationCollectionEditorCommand { get { return openAnnotationCollectionEditorCommand; } }
		public string Argument {
			get { return Point.Argument; }
			set {
				Point.Argument = value;
				RaisePropertyChanged("Argument");
			}
		}
		public double Value {
			get { return Point.Values[0]; }
			set {
				Point.Values[0] = value;
				RaisePropertyChanged("Value");
			}
		}
		public Color Color {
			get { return Point.Color; }
			set {
				Point.Color = value;
				RaisePropertyChanged("Color");
			}
		}
		public AnnotationCollection Annotations { get { return Point.Annotations; } }
		public SeriesPoint Point { get; set; }
		void OpenAnnotationCollectionEditor() {
		}
	}
	#endregion
	public class SeriesCollectionUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty SelectedPointProperty;
		public static readonly DependencyProperty PointsProperty;
		static SeriesCollectionUITypeEditor() {
			DependencyPropertyRegistrator<SeriesCollectionUITypeEditor>.New()
				.Register(d => d.SelectedPoint, out SelectedPointProperty, null)
				.Register(d => d.Points, out PointsProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public SeriesCollectionUITypeEditor() {
			copySeriesCommand = DelegateCommandFactory.Create(CopySeries, CanCopySeries);
			addPointCommand = DelegateCommandFactory.Create(AddPoint, CanAddPoint);
			insertPointCommand = DelegateCommandFactory.Create(InsertPoint, CanInsertPoint);
			removePointCommand = DelegateCommandFactory.Create(RemovePoint, CanRemovePoint);
			clearPointsCommand = DelegateCommandFactory.Create(ClearPoints, CanClearPoints);
			moveUpPointCommand = DelegateCommandFactory.Create(MoveUpPoint, CanMoveUpPoint);
			moveDownPointCommand = DelegateCommandFactory.Create(MoveDownPoint, CanMoveDownPoint);
			propertyChanged = (s, e) => {
				var index = ((Series)SelectedItem).Points.IndexOf(((PointItem)s).Point);
				((Series)SelectedItem).Points[index].Argument = ((PointItem)s).Argument;
				((Series)SelectedItem).Points[index].Values[0] = ((PointItem)s).Value;
				((Series)SelectedItem).Points[index].Color = ((PointItem)s).Color;
			};
		}
		readonly PropertyChangedEventHandler propertyChanged;
		public PointItem SelectedPoint {
			get { return (PointItem)GetValue(SelectedPointProperty); }
			set { SetValue(SelectedPointProperty, value); }
		}
		public ObservableCollectionWrappedBindingList<PointItem> Points {
			get { return (ObservableCollectionWrappedBindingList<PointItem>)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}
		protected override ICollectionEditorItem<object> CreateItem(object item) {
			if(item == null) {
				string name = Items.GetNewName("parameter");
				var series = new Series() { Name = name };
				return CollectionEditorItem<Series>.Create(series, name);
			}
			return CollectionEditorItem<Series>.Create((Series)item, ((Series)item).Name);
		}
		protected override void OnSelectedItemChanged() {
			base.OnSelectedItemChanged();
			if(SelectedItem != null) {
				Points = new ObservableCollectionWrappedBindingList<PointItem>(x => ((Series)SelectedItem).Points.Add(x.Point), propertyChanged);
				foreach(SeriesPoint point in ((Series)SelectedItem).Points) {
					Points.AddItemWithoutDoActions(new PointItem() {
						Argument = point.Argument,
						Value = point.Values.First(),
						Color = point.Color,
						Point = point
					});
				}
			}
		}
		#region Commands
		readonly ICommand copySeriesCommand;
		public ICommand CopySeriesCommand { get { return copySeriesCommand; } }
		void CopySeries() {
			var index = Items.IndexOf(Items.First(x => x.Item == SelectedItem));
			Series series = (Series)((Series)SelectedItem).Clone();
			series.Name = Items.GetNewName("Series ");
			AddItem(series);
			EditValue.Add(series);
			SetSelected(series);
		}
		bool CanCopySeries() {
			return SelectedItem != null;
		}
		readonly ICommand addPointCommand;
		public ICommand AddPointCommand { get { return addPointCommand; } }
		void AddPoint() {
			Points.Add(new PointItem());
		}
		bool CanAddPoint() {
			return SelectedItem != null;
		}
		readonly ICommand insertPointCommand;
		public ICommand InsertPointCommand { get { return insertPointCommand; } }
		void InsertPoint() {
			var index = Points.IndexOf(SelectedPoint);
			Points.Insert(index, new PointItem());
		}
		bool CanInsertPoint() {
			return SelectedPoint != null;
		}
		readonly ICommand removePointCommand;
		public ICommand RemovePointCommand { get { return removePointCommand; } }
		void RemovePoint() {
			if(SelectedPoint != null) {
				Points.Remove(SelectedPoint);
				((Series)SelectedItem).Points.Remove(SelectedPoint.Point);
			}
		}
		bool CanRemovePoint() {
			return SelectedPoint != null;
		}
		readonly ICommand clearPointsCommand;
		public ICommand ClearPointsCommand { get { return clearPointsCommand; } }
		void ClearPoints() {
			Points.Clear();
			((Series)SelectedItem).Points.Clear();
		}
		bool CanClearPoints() {
			return Points.Count > 0;
		}
		readonly ICommand moveUpPointCommand;
		public ICommand MoveUpPointCommand { get { return moveUpPointCommand; } }
		void MoveUpPoint() {
			var indexPoint = Points.IndexOf(SelectedPoint);
			Points.Swap(indexPoint, indexPoint - 1);
		}
		bool CanMoveUpPoint() {
			return SelectedPoint != Points.FirstOrDefault() && SelectedPoint != null;
		}
		readonly ICommand moveDownPointCommand;
		public ICommand MoveDownPointCommand { get { return moveDownPointCommand; } }
		void MoveDownPoint() {
			var indexPoint = Points.IndexOf(SelectedPoint);
			Points.Swap(indexPoint, indexPoint + 1);
		}
		bool CanMoveDownPoint() {
			return SelectedPoint != Points.LastOrDefault() && SelectedPoint != null;
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class DrawingColorToMediaColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			System.Drawing.Color color = (System.Drawing.Color)value;
			return value == null ? System.Windows.Media.Color.FromArgb(0, 0, 0, 0) : System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			System.Windows.Media.Color color = (System.Windows.Media.Color)value;
			return value == null ? System.Drawing.Color.FromArgb(0, 0, 0, 0) : System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
	public class DrawingColorToMediaColorConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new DrawingColorToMediaColorConverter();
		}
	}
}
