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

using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Charts {
	public class SimpleDiagramItemsControl : ChartItemsControl {
		SimpleDiagram3D Diagram { get { return DataContext as SimpleDiagram3D; } }
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (Diagram != null) {
				if (e.Action == NotifyCollectionChangedAction.Reset)
					Diagram.VisualContainers.Clear();
				if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
					for (int i = 0; i < e.OldItems.Count && e.OldStartingIndex < Diagram.VisualContainers.Count; i++) 
						Diagram.VisualContainers.RemoveAt(e.OldStartingIndex);
			}
			base.OnItemsChanged(e);
		}
		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
		}
		protected override void OnItemBindingGroupChanged(System.Windows.Data.BindingGroup oldItemBindingGroup, System.Windows.Data.BindingGroup newItemBindingGroup) {
			base.OnItemBindingGroupChanged(oldItemBindingGroup, newItemBindingGroup);
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
	}
	public class SimpleDiagramPanel : SimpleDiagramPanelBase {
		public static readonly DependencyProperty SimpleDiagramProperty = 
			DependencyPropertyManager.Register("SimpleDiagram", typeof(ISimpleDiagram), typeof(SimpleDiagramPanel));
		[
		Category(Categories.Common)]
		public ISimpleDiagram SimpleDiagram {
			get { return (ISimpleDiagram)GetValue(SimpleDiagramProperty); }
			set { SetValue(SimpleDiagramProperty, value); }
		}
		protected override ISimpleDiagram GetDiagram() {
			return SimpleDiagram;
		}
		protected override bool ShouldShowElement(UIElement element) {
			if ((element is ContentPresenter)) {
				Series series = ((ContentPresenter)element).Content as Series;
				IRefinedSeries refinedSeries = series != null && (SimpleDiagram is Diagram) ? ((Diagram)SimpleDiagram).ViewController.GetRefinedSeries(series) : null;
				return refinedSeries != null && series.GetActualVisible();
			}
			return false;
		}
	}
}
