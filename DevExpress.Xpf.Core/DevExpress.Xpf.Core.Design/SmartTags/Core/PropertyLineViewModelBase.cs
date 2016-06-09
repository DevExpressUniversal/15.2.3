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

extern alias Platform;
using DevExpress.Design.UI;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if !SL
using DevExpress.Utils.Extensions.Helpers;
using System.Windows.Threading;
#endif
namespace DevExpress.Design.SmartTags {
	public abstract class PropertyLineViewModelBase : SmartTagLineViewModelBase {
		bool isBindingPopupOpen;
		bool actualIsBindingPopupOpen;
		object bindingPopupContent;
		bool isMouseOverBindingPopupButton;
#if !SL
		bool preventBindingPopupOpening;
		ISmartTagPopup smartTagBindingPopup;
		EventHandler<SmartTagPopupIsPopupOpenChangedEventArgs> isBindingPopupOpenChanged;
		ICommand openBindingPopupCommand;
		BindingInfo bindingInfo;
#endif
		public PropertyLineViewModelBase(IPropertyLineContext context, string propertyName, Type propertyType, Type propertyOwnerType, IPropertyLinePlatformInfo platformInfo)
			: base(context) {
			this.PropertyOwnerType = propertyOwnerType;
			this.PropertyName = propertyName;
			this.PropertyType = propertyType;
			this.PlatformInfo = platformInfo;
			this.ClearValueCommand = new WpfDelegateCommand(ClearValue, CanClearValue);
#if !SL
			smartTagBindingPopup = new SmartTagPopup(() => IsBindingPopupOpen, v => IsBindingPopupOpen = v, h => isBindingPopupOpenChanged += h, h => isBindingPopupOpenChanged -= h);
			SmartTagPopupManager.RegisterPopup(smartTagBindingPopup);
			UpdateBindingInfo();
			if(IsBindingButtonVisible)
				BindingPopupContent = new BindingPopupViewModel(this);
#endif
		}
		public string PropertyName { get; private set; }
		public IPropertyLinePlatformInfo PlatformInfo { get; private set; }
#if !SL
		public bool IsBindingPresent { get { return BindingInfo != null; } }
		public BindingInfo BindingInfo {
			get { return bindingInfo; }
			set {
				if(SetProperty(ref bindingInfo, value, () => BindingInfo))
					RaisePropertyChanged(() => IsBindingPresent);
			}
		}
#endif
		public Type PropertyOwnerType { get; private set; }
		public Type PropertyType { get; private set; }
		public object PropertyValue {
			get { return ModelPropertyHelper.GetPropertyValue(SelectedItem, PropertyName, PropertyOwnerType); }
			set { ModelPropertyHelper.SetPropertyValue(SelectedItem, PropertyName, value, PropertyOwnerType); }
		}
		public Type PropertyValueType { get { return ModelPropertyHelper.GetPropertyValueType(SelectedItem, PropertyName, PropertyOwnerType); } }
		public string PropertyValueText {
			get { return PlatformInfo.GetPropertyValueText(ModelPropertyHelper.GetPropertyByName(SelectedItem, PropertyName, PropertyOwnerType)); }
			set { PropertyValue = value; }
		}
		protected IModelProperty ModelProperty {
			get { return SelectedItem == null ? null : SelectedItem.Properties.FindProperty(PropertyName, PropertyOwnerType ?? SelectedItem.ItemType); }
		}
		protected IModelItem ModelPropertyValue {
			get { return ModelProperty == null ? null : ModelProperty.Value; }
		}
		public bool IsBindingButtonVisible {
			get {
				var descriptor = DependencyPropertyDescriptor.FromName(PropertyName, PropertyOwnerType ?? SelectedItem.ItemType, SelectedItem.ItemType);
				return descriptor != null && !descriptor.IsReadOnly;
			}
		}
		public bool IsBindingPopupOpen {
			get { return isBindingPopupOpen; }
			set { SetProperty(ref isBindingPopupOpen, value, () => IsBindingPopupOpen, OnIsBindingPopupOpenChanged); }
		}
		public bool ActualIsBindingPopupOpen {
			get { return actualIsBindingPopupOpen; }
			set { SetProperty(ref actualIsBindingPopupOpen, value, () => ActualIsBindingPopupOpen, OnActualIsBindingPopupOpenChanged); }
		}
		public bool IsMouseOverBindingPopupButton {
			get { return isMouseOverBindingPopupButton; }
			set { SetProperty(ref isMouseOverBindingPopupButton, value, () => IsMouseOverBindingPopupButton); }
		}
		public object BindingPopupContent {
			get { return bindingPopupContent; }
			private set { SetProperty(ref bindingPopupContent, value, () => bindingPopupContent); }
		}
		public ICommand ClearValueCommand { get; private set; }
		public ICommand OpenBindingPopupCommand {
			get {
#if SL
				return null;
#else
				if(openBindingPopupCommand == null)
					openBindingPopupCommand = DesignTimeDelegateCommand.Create(OpenBindingPopup, DispatcherPriority.Background);
				return openBindingPopupCommand;
#endif
			}
		}
		public Action<PropertyLineViewModelBase> OnSelectedItemPropertyChangedAction { get; set; }
#if !SL
		void UpdateBindingInfo() {
			if(IsBindingButtonVisible)
				BindingInfo = BindingInfoHelper.GetBindingInfo(SelectedItem, PropertyName);
		}
#endif
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			if(propertyName == PropertyName) {
				RaisePropertyChanged(() => PropertyValue);
				RaisePropertyChanged(() => PropertyValueText);
#if !SL
				UpdateBindingInfo();
#endif
			}
			if(OnSelectedItemPropertyChangedAction != null)
				OnSelectedItemPropertyChangedAction(this);
		}
		public virtual void ClearValue() {
			ModelProperty.ClearValue();
		}
		bool CanClearValue() {
			return ModelProperty != null && ModelProperty.IsSet;
		}
		protected virtual void OnIsBindingPopupOpenChanged() {
#if !SL
			isBindingPopupOpenChanged.SafeRaise(this, new SmartTagPopupIsPopupOpenChangedEventArgs(smartTagBindingPopup));
#endif
		}
		protected virtual void OnActualIsBindingPopupOpenChanged() {
#if !SL
			if(ActualIsBindingPopupOpen)
				preventBindingPopupOpening = false;
			else
				preventBindingPopupOpening = IsMouseOverBindingPopupButton;
#endif
		}
#if !SL
		void OpenBindingPopup() {
			if(preventBindingPopupOpening)
				preventBindingPopupOpening = false;
			else
				IsBindingPopupOpen = true;
		}
#endif
		public override string ToString() {
			return string.Format("{0} ({1})", PropertyName, GetType().Name);
		}
	}
}
