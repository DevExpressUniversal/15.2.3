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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Spreadsheet.UI;
namespace DevExpress.Xpf.Spreadsheet {
	public class SpreadsheetWrappedCommand : ICommand {
		SpreadsheetControl control;
		public SpreadsheetWrappedCommand(SpreadsheetControl control, SpreadsheetCommandId id) {
			this.control = control;
			this.Id = id;
			if (control != null)
				Command = control.CreateCommand(id);
		}
		public SpreadsheetCommandId Id { get; private set; }
		SpreadsheetCommand Command { get; set; }
		#region ICommand Members
		public bool CanExecute(object parameter) {
			return Command.CanExecute();
		}
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			Command.Execute();
		}
		#endregion
	}
	partial class SpreadsheetControl {
		#region BarManager
		public static readonly DependencyProperty BarManagerProperty = DependencyPropertyManager.Register(
				"BarManager",
				typeof(BarManager),
				typeof(SpreadsheetControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBarManagerChanged)));
#if !SL
	[DevExpressXpfSpreadsheetLocalizedDescription("SpreadsheetControlBarManager")]
#endif
		public BarManager BarManager {
			get { return (BarManager)GetValue(BarManagerProperty); }
			set { SetValue(BarManagerProperty, value); }
		}
		static void OnBarManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpreadsheetControl instance = (SpreadsheetControl)d;
			instance.OnBarManagerChanged((BarManager)e.OldValue, (BarManager)e.NewValue);
		}
		#endregion
		#region Ribbon
		public static readonly DependencyProperty RibbonProperty = DependencyPropertyManager.Register(
				"Ribbon",
				typeof(RibbonControl),
				typeof(SpreadsheetControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnRibbonChanged)));
#if !SL
	[DevExpressXpfSpreadsheetLocalizedDescription("SpreadsheetControlRibbon")]
#endif
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		static void OnRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpreadsheetControl instance = (SpreadsheetControl)d;
			instance.OnRibbonChanged((RibbonControl)e.OldValue, (RibbonControl)e.NewValue);
		}
		#endregion
		#region ChartLayoutGalleryGroups
		public static readonly DependencyProperty ChartLayoutGalleryGroupsProperty = DependencyProperty.Register("ChartLayoutGalleryGroups", typeof(ObservableCollection<ChartLayoutGalleryGroupInfo>), typeof(SpreadsheetControl));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ObservableCollection<ChartLayoutGalleryGroupInfo> ChartLayoutGalleryGroups {
			get { return (ObservableCollection<ChartLayoutGalleryGroupInfo>)GetValue(ChartLayoutGalleryGroupsProperty); }
			set { SetValue(ChartLayoutGalleryGroupsProperty, value); }
		}
		#endregion
		protected internal virtual void SubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded += OnBarManagerLoaded;
		}
		protected internal virtual void UnsubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded -= OnBarManagerLoaded;
		}
		protected internal virtual void SubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded += OnRibbonLoaded;
		}
		protected internal virtual void UnsubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded -= OnRibbonLoaded;
		}
		protected internal virtual void OnBarManagerChanged(BarManager oldValue, BarManager newValue) {
			if (oldValue != null) {
				commandManager.UnsubscribeBarItemsEvents(oldValue);
				UnsubscribeBarManagerEvents(oldValue);
			}
			if (newValue != null)
				SubscribeBarManagerEvents(newValue);
			commandManager.UpdateBarItemsDefaultValues();
		}
		protected internal virtual void OnRibbonChanged(RibbonControl oldValue, RibbonControl newValue) {
			if (oldValue != null)
				UnsubscribeRibbonEvents(oldValue);
			if (newValue != null)
				SubscribeRibbonEvents(newValue);
			commandManager.UpdateRibbonItemsDefaultValues();
		}
		void OnBarManagerLoaded(object sender, RoutedEventArgs e) {
			commandManager.UpdateBarItemsDefaultValues();
			UnsubscribeBarManagerEvents(BarManager);
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnRibbonLoaded(object sender, RoutedEventArgs e) {
			commandManager.UpdateRibbonItemsDefaultValues();
			UnsubscribeRibbonEvents(Ribbon);
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnUpdateUI(object sender, EventArgs e) {
			try {
				commandManager.UpdateBarItemsState();
				commandManager.UpdateRibbonItemsState();
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
#if !SL
		internal void InternalUpdateBarManager() {
			ForceElementNameBinding(this, BarManagerProperty);
			if (BarManager == null) return;
			foreach (BarItem item in BarManager.Items)
				ForceBarItemElementNameBinding(item);
		}
		void ForceBarItemElementNameBinding(BarItem item) {
			ForceElementNameBinding(item, BarItem.CommandProperty);
			ForceElementNameBinding(item, BarItem.CommandParameterProperty);
			ForceElementNameBinding(item, BarItem.CommandTargetProperty);
			ISpreadsheetControlDependencyPropertyOwner propertyOwner = item as ISpreadsheetControlDependencyPropertyOwner;
			if (propertyOwner != null)
				ForceElementNameBinding(item, propertyOwner.DependencyProperty);
			BarEditItem editItem = item as BarEditItem;
			if (editItem != null && editItem.EditSettings != null) {
				ISpreadsheetControlDependencyPropertyOwner settings = editItem.EditSettings as ISpreadsheetControlDependencyPropertyOwner;
				if (settings != null)
					ForceElementNameBinding(editItem.EditSettings, settings.DependencyProperty);
			}
		}
		internal void ForceElementNameBinding(DependencyObject o, DependencyProperty p) {
			BindingExpression bindingExpression = BindingOperations.GetBindingExpression(o, p);
			if (bindingExpression == null || bindingExpression.Status != BindingStatus.PathError) return;
			Binding binding = bindingExpression.ParentBinding;
			BindingOperations.ClearBinding(o, p);
			BindingOperations.SetBinding(o, p, binding);
		}
#endif
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	public interface ISpreadsheetControlDependencyPropertyOwner {
		DependencyProperty DependencyProperty { get; }
	}
}
