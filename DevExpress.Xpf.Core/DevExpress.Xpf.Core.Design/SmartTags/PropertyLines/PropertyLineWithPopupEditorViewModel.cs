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

using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class PropertyLineWithPopupEditorViewModel : ObjectPropertyLineViewModel {
		bool preventPopupOpening;
		public PropertyLineWithPopupEditorViewModel(IPropertyLineContext context, string propertyName, Type propertyOwnerType = null, IPropertyLinePlatformInfo platformInfo = null)
			: base(context, propertyName, propertyOwnerType, platformInfo) {
			Command = DesignTimeDelegateCommand.Create(OpenPopup, DispatcherPriority.Background);
			PopupContent = CreatePopup();
		}
		protected override void OnActualIsPopupOpenChanged() {
			base.OnActualIsPopupOpenChanged();
			if(ActualIsPopupOpen)
				preventPopupOpening = false;
			else
				preventPopupOpening = IsMouseOverCommandButton;
		}
		protected abstract PropertyLineWithPopupEditorPopupViewModel CreatePopup();
		void OpenPopup() {
			if(preventPopupOpening)
				preventPopupOpening = false;
			else
				IsPopupOpen = true;
		}
	}
	public abstract class PropertyLineWithPopupEditorPopupViewModel : BindableBase {
		readonly PropertyLineWithPopupEditorViewModel propertyLine;
		ICommand closePopupCommand;
		EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs> isPopupOpenChanged;
		SmartTagPopup smartTagPopup;
		public PropertyLineWithPopupEditorPopupViewModel(PropertyLineWithPopupEditorViewModel propertyLine) {
			this.propertyLine = propertyLine;
			this.propertyLine.IsPopupOpenChanged += OnPropertyLineIsPopupOpenChanged;
			smartTagPopup = new SmartTagPopup(() => this.propertyLine.IsPopupOpen, v => this.propertyLine.IsPopupOpen = v, h => isPopupOpenChanged += h, h => isPopupOpenChanged -= h);
			SmartTagPopupManager.RegisterPopup(smartTagPopup);
		}
		public void ClosePopup() {
			this.propertyLine.IsPopupOpen = false;
		}
		public ICommand ClosePopupCommand {
			get {
				if(closePopupCommand == null)
					closePopupCommand = new WpfDelegateCommand(ClosePopup, false);
				return closePopupCommand;
			}
		}
		protected PropertyLineWithPopupEditorViewModel PropertyLine { get { return propertyLine; } }
		protected virtual void OnPropertyLineIsPopupOpenChanged(object sender, EventArgs e) {
			isPopupOpenChanged.SafeRaise(this, new SmartTagPopupIsPopupOpenChangedEventArgs(smartTagPopup));
		}
	}
}
