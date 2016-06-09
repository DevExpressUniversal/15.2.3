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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {
	public interface ILogicalOwnerProvider {
		ILogicalOwner GetLogicalOwner();
	}
	public class ColumnChooserControlBase : Control, ILogicalOwnerProvider {
		public static readonly DependencyProperty OwnerProperty;
		static ColumnChooserControlBase() {
			Type ownerType = typeof(ColumnChooserControlBase);
			OwnerProperty = DependencyPropertyManager.Register("Owner", typeof(ILogicalOwner), ownerType,
				new PropertyMetadata(null,
					(d, e) => ((ColumnChooserControlBase)d).OnOwnerChanged((ILogicalOwner)e.OldValue, (ILogicalOwner)e.NewValue)));
		}
#if !SL
		public ColumnChooserControlBase() {
			CommandManager.AddCanExecuteHandler(this, new CanExecuteRoutedEventHandler(CanExecuteRoutedEventHandler));
			CommandManager.AddExecutedHandler(this, new ExecutedRoutedEventHandler(ExecutedRoutedEventHandler));
		}
		void CanExecuteRoutedEventHandler(object sender, CanExecuteRoutedEventArgs e) {
			RoutedCommand command = e.Command as RoutedCommand;
			if(command != null)
				e.CanExecute = command.CanExecute(e.Parameter, Owner);
		}
		protected virtual void ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e) {
			RoutedCommand command = e.Command as RoutedCommand;
			if(command != null)
				command.Execute(e.Parameter, Owner);
		}
#endif
		[Browsable(false)]
		public ILogicalOwner Owner {
			get { return (ILogicalOwner)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		protected virtual void OnOwnerChanged(ILogicalOwner oldView, ILogicalOwner newView) {
			if(oldView != null)
				oldView.RemoveChild(this);
#if !SL
			if(LogicalTreeHelper.GetParent(this) == null && newView != null)
				newView.AddChild(this);
#else
			if(newView != null)
				newView.AddChild(this);
#endif
		}
		#region ILogicalOwnerProvider Members
		ILogicalOwner ILogicalOwnerProvider.GetLogicalOwner() {
			return GetLogicalOwnerCore();
		}
		protected virtual ILogicalOwner GetLogicalOwnerCore() {
			return Owner;
		}
		#endregion
	}
}
