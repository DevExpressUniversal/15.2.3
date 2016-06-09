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
using System.Windows;
using System.Windows.Controls;
#if !SILVERLIGHT
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Native;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
namespace DevExpress.Xpf.Grid {
	public class GridTotalSummary : ContentControl {
		public GridTotalSummary() {
			this.SetDefaultStyleKey(typeof(GridTotalSummary));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetValue(GridPopupMenu.GridMenuTypeProperty, GridMenuType.TotalSummary);
		}
	}
	public class FixedTotalSummaryContainerControl : ContentControl {
		public FixedTotalSummaryContainerControl() {
			this.SetDefaultStyleKey(typeof(FixedTotalSummaryContainerControl));
		}
	}
	public class FixedTotalSummaryItemsControl : ItemsControl {
		public FixedTotalSummaryItemsControl() {
			this.SetDefaultStyleKey(typeof(FixedTotalSummaryItemsControl));
		}
	}
	public class FixedTotalSummaryItemsTextBlock : ContentControl, IFixedTotalSummary {
		public static readonly DependencyProperty TotalSummariesSourceProperty = DependencyPropertyManager.Register("TotalSummariesSource", typeof(IList<GridTotalSummaryData>), typeof(FixedTotalSummaryItemsTextBlock), new FrameworkPropertyMetadata(null, (d, e) => ((FixedTotalSummaryItemsTextBlock)d).OnTotalSummariesSourceChanged((IList<GridTotalSummaryData>)e.OldValue, (IList<GridTotalSummaryData>)e.NewValue)));
		public static readonly DependencyProperty SummaryTextProperty = DependencyPropertyManager.Register("SummaryText", typeof(string), typeof(FixedTotalSummaryItemsTextBlock), new FrameworkPropertyMetadata(String.Empty));
		void OnTotalSummariesSourceChanged(IList<GridTotalSummaryData> oldList, IList<GridTotalSummaryData> newList) {
			if(oldList != null)
				UnsubscribeList(oldList);
			SibscribeList(newList);
		}
		void SibscribeList(IList<GridTotalSummaryData> newList) {
			ObservableCollection<GridTotalSummaryData> oc = newList as ObservableCollection<GridTotalSummaryData>;
			if(oc == null)
				return;
			oc.CollectionChanged += FixedTotalSummaryCollectionChanged;
			SummaryText = FixedTotalSummaryHelper.GetFixedSummariesString(newList);
		}
		void UnsubscribeList(IList<GridTotalSummaryData> oldList) {
			ObservableCollection<GridTotalSummaryData> oc = oldList as ObservableCollection<GridTotalSummaryData>;
			if(oc == null)
				return;
			oc.CollectionChanged -= FixedTotalSummaryCollectionChanged;
		}
		void FixedTotalSummaryCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			SummaryText = FixedTotalSummaryHelper.GetFixedSummariesString(TotalSummariesSource);
		}
		public FixedTotalSummaryItemsTextBlock() {
			this.SetDefaultStyleKey(typeof(FixedTotalSummaryItemsTextBlock));
		}
		public IList<GridTotalSummaryData> TotalSummariesSource {
			get { return (IList<GridTotalSummaryData>)GetValue(TotalSummariesSourceProperty); }
			set { SetValue(TotalSummariesSourceProperty, value); }
		}
		public string SummaryText {
			get { return (string)GetValue(SummaryTextProperty); }
			set { SetValue(SummaryTextProperty, value); }
		}
		#if !SL
		[Browsable(false)]
		public bool ShouldSerializeTotalSummariesSource(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		#endif
	}
	public interface IFixedTotalSummary {
		string SummaryText { get; }
	}
}
