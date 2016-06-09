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
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Charts.Native {
	public class NestedDonut2DGroup {
		List<SeriesItem> items = new List<SeriesItem>();
		public NestedDonut2DGroup(SeriesItem item) {
			items.Add(item);
		}
		public SeriesItem this[int index] {
			get { return items[index]; }
		}
		public IEnumerable<SeriesLabelConnectorItem> LabelConnectorItems {
			get {
				for (int i = 0; i < items.Count; i++) {
					SeriesLabel label = items[0].Series.ActualLabel;
					for (int j = 0; j < label.Items.Count; j++) {
						var connector = label.Items[j].ConnectorItem;
						if (connector != null)
							yield return connector;
					}
				}
			}
		}
		public IEnumerable<SeriesLabelItem> LabelItems {
			get {
				for (int i = 0; i < items.Count; i++) {
					SeriesLabel label = items[i].Series.ActualLabel;
					for (int j = 0; j < label.Items.Count; j++) {
						yield return label.Items[j];
					}
				}
			}
		}
		public IEnumerable<SeriesLabel> SeriesLabels {
			get {
				for (int i = 0; i < items.Count; i++)
					yield return items[i].Series.ActualLabel;
			}
		}
		public int Count {
			get { return items.Count; }
		}
		void SetBinding(FrameworkElement target, DependencyProperty property, object source, string path) {
			Binding binding = new Binding(path);
			binding.Source = source;
			target.SetBinding(property, binding);
		}
		public void Add(SeriesItem item) {
			if (item.Series.GetType() != typeof(NestedDonutSeries2D))
				throw new ArgumentException("Expected a SeriesItem which according to a NestedDonutSeries2D.");
			items.Add(item);
		}
	}
}
