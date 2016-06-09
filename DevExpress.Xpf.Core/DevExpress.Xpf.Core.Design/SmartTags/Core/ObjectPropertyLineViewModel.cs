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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Platform::DevExpress.Utils;
using Platform::DevExpress.XtraEditors.DXErrorProvider;
using Platform::DevExpress.Utils.Extensions.Helpers;
#if SL
using DevExpress.Utils.Design;
#else
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Design.SmartTags {
	public class ObjectPropertyLineViewModel : PropertyLineViewModelBase {
		bool hasCommand, isReadOnly;
		ICommand command;
		object commandParameter;
		IInputElement commandTarget;
		IEnumerable<object> itemsSource;
		Tuple<string, Platform::DevExpress.Xpf.Editors.MaskType> mask;
		bool hasPopup;
		object popupContent;
		bool isPopupOpen;
		bool actualIsPopupOpen;
		bool isMouseOverCommandButton;
#if !SL
		ICommand validateCommand;
		bool typeConverterLoaded;
		TypeConverter typeConverter;
#endif
		public ObjectPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type propertyOwnerType = null, IPropertyLinePlatformInfo platformInfo = null)
			: base(context, propertyName, typeof(object), propertyOwnerType, platformInfo ?? context.PlatformInfoFactory.ForStandardProperty("string")) {
		}
		public ICommand Command {
			get { return command; }
			set { SetProperty(ref command, value, () => Command, OnCommandChanged); }
		}
		public object CommandParameter {
			get { return commandParameter; }
			set { SetProperty(ref commandParameter, value, () => CommandParameter); }
		}
		public IInputElement CommandTarget {
			get { return commandTarget; }
			set { SetProperty(ref commandTarget, value, () => CommandTarget); }
		}
		public bool HasCommand {
			get { return hasCommand; }
			private set { SetProperty(ref hasCommand, value, () => HasCommand); }
		}
		public IEnumerable<object> ItemsSource {
			get { return itemsSource; }
			set { SetProperty(ref itemsSource, value, () => ItemsSource); }
		}
		public Tuple<string, Platform::DevExpress.Xpf.Editors.MaskType> Mask {
			get { return mask; }
			set { SetProperty(ref mask, value, () => Mask); }
		}
		public bool IsReadOnly {
			get { return isReadOnly; }
			set { SetProperty(ref isReadOnly, value, () => IsReadOnly); }
		}
		public object PopupContent {
			get { return popupContent; }
			set { SetProperty(ref popupContent, value, () => PopupContent); }
		}
		public bool HasPopup {
			get { return hasPopup; }
			set { SetProperty(ref hasPopup, value, () => HasPopup); }
		}
		public bool IsPopupOpen {
			get { return isPopupOpen; }
			set { SetProperty(ref isPopupOpen, value, () => IsPopupOpen, OnIsPopupOpenChanged); }
		}
		public bool ActualIsPopupOpen {
			get { return actualIsPopupOpen; }
			set { SetProperty(ref actualIsPopupOpen, value, () => ActualIsPopupOpen, OnActualIsPopupOpenChanged); }
		}
		public bool IsMouseOverCommandButton {
			get { return isMouseOverCommandButton; }
			set { SetProperty(ref isMouseOverCommandButton, value, () => IsMouseOverCommandButton); }
		}
		public event EventHandler IsPopupOpenChanged;
#if !SL
		public ICommand ValidateCommand {
			get {
				if(validateCommand == null)
					validateCommand = (ICommand)DelegateCommandFactory.Create<IValidationInfo>(Validate, false);
				return validateCommand;
			}
		}
		public virtual void Validate(IValidationInfo validationInfo) {
			string text = validationInfo.Value as string;
			if(string.IsNullOrEmpty(text) || text[0] == '{') return;
			try {
				if(TypeConverter != null) {
					IModelItem modelItem = Context.ModelItem;
					TypeConverter.ConvertFromInvariantString(new EmptyDescriptorContext(modelItem == null ? null : modelItem.Context.Services), text);
				}
			} catch(Exception e) {
				validationInfo.SetError(e.Message, ErrorType.Warning);
			}
		}
		TypeConverter TypeConverter {
			get {
				if(!typeConverterLoaded) {
					typeConverterLoaded = true;
					Type propertyType = ModelPropertyHelper.GetPropertyByName(SelectedItem, PropertyName, PropertyOwnerType).PropertyType;
					typeConverter = propertyType == typeof(object) ? null : TypeDescriptor.GetConverter(propertyType);
				}
				return typeConverter;
			}
		}
#endif
		protected virtual void OnIsPopupOpenChanged() {
			IsPopupOpenChanged.SafeRaise(this, EventArgs.Empty);
		}
		protected virtual void OnActualIsPopupOpenChanged() { }
		protected virtual void OnCommandChanged() {
			HasCommand = Command != null;
		}
		protected virtual void OnPopupContentChanged() {
			HasPopup = PopupContent != null;
		}
	}
#if !SL
	public interface IValidationInfo {
		object Value { get; }
		void SetError(object errorContent, ErrorType errorType);
	}
#endif
}
