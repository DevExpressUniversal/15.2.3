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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;
using DevExpress.Xpf.Ribbon;
using System.Linq;
namespace DevExpress.Xpf.DocumentViewer {
	public class BarManagerActionAttachedBehavior : Behavior<BarManager> {
		public static readonly DependencyProperty ActionsProperty =
			DependencyPropertyManager.Register("Actions", typeof(ObservableCollection<IBarManagerControllerAction>), typeof(BarManagerActionAttachedBehavior),
			new PropertyMetadata(null, (obj, args) => ((BarManagerActionAttachedBehavior)obj).OnActionsChanged((ObservableCollection<IBarManagerControllerAction>)args.OldValue, (ObservableCollection<IBarManagerControllerAction>)args.NewValue)));
		public ObservableCollection<IBarManagerControllerAction> Actions {
			get { return (ObservableCollection<IBarManagerControllerAction>)GetValue(ActionsProperty); }
			set { SetValue(ActionsProperty, value); }
		}
		BarManagerController controller;
		public BarManagerActionAttachedBehavior() {
			controller = new BarManagerController();
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Controllers.Add(controller);
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.Controllers.Remove(controller);
		}
		void OnActionsChanged(ObservableCollection<IBarManagerControllerAction> oldValue, ObservableCollection<IBarManagerControllerAction> newValue) {
			oldValue.Do(x => x.CollectionChanged -= OnCollectionChanged);
			if (newValue == null)
				return;
			controller.ActionContainer.Actions.Clear();
			foreach (IBarManagerControllerAction action in newValue)
				controller.ActionContainer.Actions.Add(action);
			newValue.CollectionChanged += OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null) {
				foreach (IBarManagerControllerAction action in e.OldItems)
					controller.ActionContainer.Actions.Remove(action);
			}
			if (e.NewItems != null) {
				foreach (IBarManagerControllerAction action in e.NewItems) {
					controller.ActionContainer.Actions.Add(action);
				}
				controller.Execute();
			}
		}
	}
	public class DocumentViewerRibbonController : RibbonController {
		public override void Execute() {
			base.Execute();
		}
	}
	public class RibbonControlActionAttachedBehavior : Behavior<RibbonControl> {
		public static readonly DependencyProperty ActionsProperty =
			DependencyPropertyManager.Register("Actions", typeof(ObservableCollection<IControllerAction>), typeof(RibbonControlActionAttachedBehavior),
			new PropertyMetadata(null, (obj, args) => ((RibbonControlActionAttachedBehavior)obj).OnActionsChanged((ObservableCollection<IControllerAction>)args.OldValue, (ObservableCollection<IControllerAction>)args.NewValue)));
		public ObservableCollection<IBarManagerControllerAction> Actions {
			get { return (ObservableCollection<IBarManagerControllerAction>)GetValue(ActionsProperty); }
			set { SetValue(ActionsProperty, value); }
		}
		DocumentViewerRibbonController controller;
		public RibbonControlActionAttachedBehavior() {
			controller = new DocumentViewerRibbonController();
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Controllers.Add(controller);
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.Controllers.Remove(controller);
		}
		void OnActionsChanged(ObservableCollection<IControllerAction> oldValue, ObservableCollection<IControllerAction> newValue) {
			oldValue.Do(x => x.CollectionChanged -= OnCollectionChanged);
			if (newValue == null)
				return;
			controller.ActionContainer.Actions.Clear();
			foreach (IControllerAction cAction in newValue) {
				if (!(cAction is IBarManagerControllerAction))
					continue;
				controller.ActionContainer.Actions.Add((IBarManagerControllerAction)cAction);
			}				
			newValue.CollectionChanged += OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null) {
				foreach (var action in e.OldItems.OfType<IBarManagerControllerAction>())
					controller.ActionContainer.Actions.Remove(action);
			}
			if (e.NewItems != null) {
				foreach (var action in e.NewItems.OfType<IBarManagerControllerAction>()) {
					controller.ActionContainer.Actions.Add(action);
				}
				controller.Execute();
			}
		}
	}
}
