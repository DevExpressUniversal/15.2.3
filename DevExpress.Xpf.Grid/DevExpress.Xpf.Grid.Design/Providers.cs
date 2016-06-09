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

#if SILVERLIGHT
extern alias Platform;
#endif
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Windows.Design.Services;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Grid;
#else
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Grid.Design {
	public class DataViewBaseAdornerProvider : DXAdornerProviderBase {
		public DataViewBaseAdornerProvider() {
			hookPanel.MouseLeftButtonDown += new MouseButtonEventHandler(hookPanel_MouseLeftButtonDown);
		}
		void hookPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			SelectionOperations.Select(AdornedElement.Context, AdornedElement.Parent);
		}
		protected override Control CreateHookPanel() {
			return new UserControl() { Content = new GridViewAdornerPanel(this) { Background = Brushes.Transparent } };
		}
	}
	class DataControlAdornerFeatureConnector : FeatureConnector<DataControlAdornerProvider> {
		public DataControlAdornerFeatureConnector(FeatureManager manager)
			: base(manager) {
			Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
			}
			base.Dispose(disposing);
		}
		private void OnSelectionChanged(Selection newSelection) {
			if(newSelection != null && newSelection.PrimarySelection != null && typeof(DataViewBase).IsAssignableFrom(newSelection.PrimarySelection.ItemType)) {
				ModelItem gridItem = newSelection.PrimarySelection.Parent;
				if(GridDesignTimeHelper.IsGridItem(gridItem) && Environment.StackTrace.Contains("DesignerView.OnMouseUp")) {
					((DataControlBase)gridItem.View.PlatformObject).Dispatcher.BeginInvoke(new Action(delegate {
						SelectionOperations.Select(Context, gridItem);
					})
#if !SILVERLIGHT
					, DispatcherPriority.Normal
#endif
					);
				}
			}
		}
	}
	internal class DataControlPolicy : SelectionPolicy {
		public DataControlPolicy() {
		}
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem item = selection.PrimarySelection;
			if(item != null) {
				if(typeof(BaseEditSettings).IsAssignableFrom(item.ItemType)) {
					item = item.Parent;
				}
				if(typeof(DataViewBase).IsAssignableFrom(item.ItemType) || typeof(BaseColumn).IsAssignableFrom(item.ItemType)) {
					item = DevExpress.Xpf.Core.Design.BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(item);
				}
			}
			if(item != null) {
			   yield return item;
			}
		}
	}
}
