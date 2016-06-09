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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public class DialogServiceEditorBehavior : DialogServiceEditorBehaviorBase {
		public static readonly DependencyProperty CommandsProperty;
		static DialogServiceEditorBehavior() {
			CommandsProperty = DependencyPropertyRegistrator.Register<DialogServiceEditorBehavior, UICommandContainerCollection>(owner => owner.Commands, null,
				(settings, value, newValue) => settings.CommandsChanged(value, newValue));
		}
		public UICommandContainerCollection Commands {
			get { return (UICommandContainerCollection)GetValue(CommandsProperty); }
			set { SetValue(CommandsProperty, value); }
		}
		public DialogServiceEditorBehavior() {
			SetCurrentValue(CommandsProperty, new UICommandContainerCollection());
		}
		void CommandsChanged(UICommandContainerCollection oldValue, UICommandContainerCollection newValue) {
			var logicalOwner = GetLogicalOwner();
			if (logicalOwner != null)
				oldValue.Do(x => x.ForEach(child => logicalOwner.RemoveLogicalChild(child)));
			oldValue.Do(x => x.CollectionChanged -= CommandsCollectionChanged);
			if (logicalOwner != null)
				newValue.Do(x => x.ForEach(child => logicalOwner.AddLogicalChild(child)));
			newValue.Do(x => x.CollectionChanged += CommandsCollectionChanged);
		}
		void CommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (!IsAttached)
				return;
			var logicalOwner = GetLogicalOwner();
			if (logicalOwner == null)
				return;
			logicalOwner.ProcessChildrenChanged(e);
		}
		ILogicalChildrenContainer2 GetLogicalOwner() {
			var logicalOwnerProvider = AssociatedObject as ILogicalChildrenContainerProvider;
			var logicalOwner = logicalOwnerProvider.With(x => x.LogicalChildrenContainer);
			return logicalOwner;
		}
		protected override void OnAttached() {
			base.OnAttached();
			var logicalOwner = GetLogicalOwner();
			if (logicalOwner == null)
				return;
			Commands.ForEach(x => logicalOwner.AddLogicalChild(x));
		}
		protected override void OnDetaching() {
			var logicalOwner = GetLogicalOwner();
			if (logicalOwner == null)
				return;
			Commands.ForEach(x => logicalOwner.RemoveLogicalChild(x));
			base.OnDetaching();
		}
		protected internal virtual IEnumerable<UICommand> CreateUICommands() {
			return Commands.Return(command => command.Select(x => new UICommand(x.Id, x.Caption, x.Command, x.IsDefault, x.IsCancel, x.Tag)).ToList(), Enumerable.Empty<UICommand>);
		}
		protected override void ProcessClickForFrameworkElementInternal(FrameworkElement owner, object editValue) {
			var dialogService = DialogServiceTemplate.Return(TemplateHelper.LoadFromTemplate<DialogService>, CreateDefaultDialogService);
			if (dialogService != null) {
				AssignableServiceHelper2<DependencyObject, IDialogService>.DoServiceAction(owner, dialogService, service => {
					var value = GetEditableValue(owner, editValue);
					var dialogResult = service.ShowDialog(CreateUICommands(), Title, value);
					var result = RaiseDialogClosed(value, dialogResult);
					bool shouldPost = value.IsModified || (result.PostValue.HasValue && result.PostValue.Value);
					if (!shouldPost)
						return;
					PostEditValue(owner, result.Value.Value);
				});
				return;
			}
		}
		DialogService CreateDefaultDialogService() {
			DialogService service = new DialogService();
			service.SetWindowOwner = true;
			service.DialogWindowStartupLocation = WindowStartupLocation.CenterScreen;
			service.Title = Title;
			return service;
		}
	}
}
