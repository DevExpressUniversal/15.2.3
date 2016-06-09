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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using System.Windows;
using DevExpress.Design.SmartTags;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using PivotGridControl = Platform.DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform.DevExpress.Xpf.PivotGrid.PivotGridField;
using IPivotOLAPDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotOLAPDataSource;
using PivotGridWpfData = Platform.DevExpress.Xpf.PivotGrid.Internal.PivotGridWpfData;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Policies;
using System.Windows.Input;
using System.Windows.Media;
#else
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using System.Windows.Threading;
using Microsoft.Windows.Design.Policies;
using System.Windows.Input;
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	class DataControlAdornerFeatureConnector : FeatureConnector<PivotGridControlAdornerProvider> {
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
			if(newSelection != null && newSelection.PrimarySelection != null && typeof(PivotGridControl).IsAssignableFrom(newSelection.PrimarySelection.ItemType)) {
				ModelItem gridItem = newSelection.PrimarySelection.Parent;
				if(gridItem.IsItemOfType(typeof(PivotGridControl)) && Environment.StackTrace.Contains("DesignerView.OnMouseUp")) {
					((PivotGridControl)gridItem.View.PlatformObject).Dispatcher.BeginInvoke(new Action(delegate {
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
				if(typeof(PivotGridControl).IsAssignableFrom(item.ItemType) || typeof(PivotGridField).IsAssignableFrom(item.ItemType)) {
					item = item.Parent;
				}
			}
			if(item != null) {
				yield return item;
			}
		}
	}
	public class ChangePropertyTypeInfo : DependencyObject {
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ChangePropertyTypeInfo), new UIPropertyMetadata(true));
		public ChangePropertyTypeInfo(ICommand command, Type type, ImageSource image) {
			Command = command;
			Type = type;
			Image = image;
		}
		public ICommand Command { get; private set; }
		public Type Type { get; private set; }
		public ImageSource Image { get; private set; }
		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
	}
}
