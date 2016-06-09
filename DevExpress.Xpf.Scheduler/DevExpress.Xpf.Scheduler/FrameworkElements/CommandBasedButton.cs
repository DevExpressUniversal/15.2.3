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
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ICommandBasedButtonInfo {
		ICommand Command { get; }
		object CommandParameter { get; }
	}
	#region CommandBasedButton<TInfo> (abstract class)
	public abstract class CommandBasedButton<TInfo> : Button, ISupportCopyFrom<TInfo> where TInfo : ICommandBasedButtonInfo {
		#region ButtonInfo
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty ButtonInfoProperty = CreateButtonInfoProperty();
		static DependencyProperty CreateButtonInfoProperty() {
			return DependencyPropertyHelper.RegisterProperty<CommandBasedButton<TInfo>, TInfo>("ButtonInfo", default(TInfo), OnButtonInfoChanged);
		}
		static void OnButtonInfoChanged(CommandBasedButton<TInfo> button, DependencyPropertyChangedEventArgs<TInfo> e) {
			button.OnButtonInfoChanged(e.OldValue, e.NewValue);
		}
		public TInfo ButtonInfo { get { return (TInfo)GetValue(ButtonInfoProperty); } set { SetValue(ButtonInfoProperty, value); } }
		#endregion
		#region ISupportCopyFrom<TInfo> Members
		public void CopyFrom(TInfo source) {
			if (source != null)
				CopyFromCore(source);
		}
		#endregion
		protected virtual void OnButtonInfoChanged(TInfo oldValue, TInfo newValue) {
			CopyFrom(newValue);
		}
		protected virtual void CopyFromCore(TInfo source) {
			CommandParameter = source.CommandParameter;
			Command = source.Command;
		}
	}
	#endregion
	public class CommandButtonHelper : DependencyObject {
		#region CommandParameter
		public static readonly DependencyProperty CommandParameterProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedPropertyCore<CommandButtonHelper, object>("CommandParameter", null, FrameworkPropertyMetadataOptions.None, OnCommandParameterPropertyChanged);
		static void OnCommandParameterPropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e) {
			Button button = s as Button;
			if(button == null)
				return;
			if(button.Command == null)
				return;
			SchedulerUICommand schedulerCommand = button.Command as SchedulerUICommand;
			if(schedulerCommand == null)
				return;
			button.CommandParameter = e.NewValue;
			schedulerCommand.RaiseCanExecuteChanged();
		}
		public static object GetCommandParameter(DependencyObject d) {
			return d.GetValue(CommandParameterProperty);
		}
		public static void SetCommandParameter(DependencyObject d, object value) {
			d.SetValue(CommandParameterProperty, value);
		}
		#endregion        
	}
}
