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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class TitlesLayoutControl : ChartItemsControl {
		public static readonly DependencyProperty MasterElementProperty = DependencyPropertyManager.Register("MasterElement", 
			typeof(FrameworkElement), typeof(TitlesLayoutControl), new PropertyMetadata(MasterElementChanged));
		public static readonly DependencyProperty TitlesProperty = DependencyPropertyManager.Register("Titles", 
			typeof(TitleCollection), typeof(TitlesLayoutControl), new PropertyMetadata(TitlesChanged));
		static void MasterElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TitlesLayoutControl titlesLayoutControl = d as TitlesLayoutControl;
			if (titlesLayoutControl != null)
				titlesLayoutControl.Update();
		}
		static void TitlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TitlesLayoutControl titlesLayoutControl = d as TitlesLayoutControl;
			if (titlesLayoutControl != null) {
				TitleCollection oldCollection = e.OldValue as TitleCollection;
				if (oldCollection != null)
					oldCollection.CollectionChanged -= titlesLayoutControl.TitlesCollectionChanged;
				TitleCollection newCollection = e.NewValue as TitleCollection;
				if (newCollection != null)
					newCollection.CollectionChanged += titlesLayoutControl.TitlesCollectionChanged;
			}
		}
		[
		Category(Categories.Common)]
		public FrameworkElement MasterElement {
			get { return (FrameworkElement)GetValue(MasterElementProperty); }
			set { SetValue(MasterElementProperty, value); }
		}
		[
		Category(Categories.Common)]
		public TitleCollection Titles {
			get { return (TitleCollection)GetValue(TitlesProperty); }
			set { SetValue(TitlesProperty, value); }
		}
		public TitlesLayoutControl() {
			DefaultStyleKey = typeof(TitlesLayoutControl);
		}
		void TitlesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			TitleCollection titleCollection = sender as TitleCollection;
			if (titleCollection != null)
				Update();
		}
		void Update() {
			ObservableCollection<object> layoutItems = new ObservableCollection<object>();
			if (Titles != null)
				foreach (Title title in Titles) 
					layoutItems.Add(title);
			if (MasterElement != null)
				layoutItems.Add(MasterElement);
			ItemsSource = layoutItems;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			Title title = element as Title;
			if (title != null) {
				Binding dockBinding = new Binding("Dock");
				dockBinding.Source = title;
				title.SetBinding(DockPanel.DockProperty, dockBinding);
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			Size result = base.MeasureOverride(constraint);
			return (Double.IsInfinity(constraint.Width) || Double.IsInfinity(constraint.Height)) ? result : constraint;
		}
	}
}
