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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
#if NETFX_CORE
using System.Threading.Tasks;
using Windows.UI.Popups;
using IUICommand = Windows.UI.Popups.IUICommand;
using Windows.System.Profile;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif
#if SILVERLIGHT
namespace DevExpress.Mvvm.UI {
	public class MessageBoxService : ServiceBase, IMessageBoxService {
		MessageResult IMessageBoxService.Show(string messageBoxText, string caption, MessageButton button, MessageResult defaultResult) {
			return MessageBox.Show(messageBoxText, caption, button.ToMessageBoxButton()).ToMessageResult();
		}
	}
}
#elif NETFX_CORE
namespace DevExpress.Mvvm.UI {
	public class MessageBoxService : ServiceBase, IMessageBoxService {
		public async Task<UICommand> ShowAsync(string messageBoxText, string caption, IList<UICommand> commands) {
			if(AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
				return await ShowAsyncIoT(messageBoxText, caption, commands);
			MessageDialog messageDialog = new MessageDialog(messageBoxText, caption);
			uint defaultCommandIndex = 0;
			uint cancelCommandIndex = uint.MaxValue;
			uint commandIndex = 0;
			foreach(var command in commands) {
				if(command.IsCancel)
					cancelCommandIndex = commandIndex;
				if(command.IsDefault)
					defaultCommandIndex = commandIndex;
				messageDialog.Commands.Add(ConvertToUIPopupCommand(command));
				commandIndex++;
			}
			messageDialog.DefaultCommandIndex = defaultCommandIndex;
			messageDialog.CancelCommandIndex = cancelCommandIndex;
			var result = await messageDialog.ShowAsync().AsTask();
			return commands.FirstOrDefault(c => c.Id == result.Id || c == result.Id);
		}
		async Task<UICommand> ShowAsyncIoT(string messageBoxText, string caption, IList<UICommand> commands) {
			var service = new DialogService();
			var str = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
				"<ContentControl Content=\"{Binding}\"/>" +
			"</DataTemplate>";
			service.ViewTemplate = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(str);
			return await service.ShowDialogAsync(commands, caption, messageBoxText);
		}
		IUICommand ConvertToUIPopupCommand(UICommand command) {
			return new Windows.UI.Popups.UICommand((string)command.Caption, ConvertToAction(command), command.Id ?? command);
		}
		UICommandInvokedHandler ConvertToAction(UICommand command) {
			return new UICommandInvokedHandler((d) => { command.Command.If(x => x.CanExecute(command)).Do(y => y.Execute(command)); });
		}
	}
}
#else
#if !FREE
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Core {
	[TargetTypeAttribute(typeof(UserControl))]
	[TargetTypeAttribute(typeof(Window))]
	public class DXMessageBoxService : ServiceBase, IMessageBoxService {
		MessageResult IMessageBoxService.Show(string messageBoxText, string caption, MessageButton button, MessageIcon icon, MessageResult defaultResult) {
			return DXMessageBox.Show(AssociatedObject, messageBoxText, caption, button.ToMessageBoxButton(), icon.ToMessageBoxImage(), defaultResult.ToMessageBoxResult()).ToMessageResult();
		}
	}
}
#else
namespace DevExpress.Mvvm.UI {
	public class MessageBoxService : ServiceBase, IMessageBoxService {
		MessageResult IMessageBoxService.Show(string messageBoxText, string caption, MessageButton button, MessageIcon icon, MessageResult defaultResult) {
			var owner = AssociatedObject.With(x => Window.GetWindow(x));
			if(owner == null)
				return MessageBox.Show(messageBoxText, caption, button.ToMessageBoxButton(), icon.ToMessageBoxImage(), defaultResult.ToMessageBoxResult()).ToMessageResult();
			else
				return MessageBox.Show(owner, messageBoxText, caption, button.ToMessageBoxButton(), icon.ToMessageBoxImage(), defaultResult.ToMessageBoxResult()).ToMessageResult();
		}
	}
}
#endif
#endif
