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

using DevExpress.Design.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm;
using System;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class BindingEditorControlBindingEditorViewModel : BindableBase {
		string header;
		IBindingEditorControlBindingSource source;
		string path;
		IEnumerable<BindingSettingDescription<BindingMode>> modes;
		IEnumerable<BindingSettingDescription<UpdateSourceTrigger>> updateSourceTriggers;
		BindingSettingDescription<BindingMode> mode;
		BindingSettingDescription<UpdateSourceTrigger> updateSourceTrigger;
		ICommand okCommand;
		ICommand cancelCommand;
		IEnumerable<BindingEditorControlConverterViewModel> converters;
		IBindingEditorControlConverter converter;
		bool updateSourceTriggerEnabled;
		public BindingEditorControlBindingEditorViewModel(BindingEditorControlMainViewModel main) {
			Main = main;
			Main.MainControl.BindingSettingsProviderChanged += OnMainControlBindingSettingsProviderChanged;
			OnMainControlBindingSettingsProviderChanged(Main.MainControl, new ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider>(null, Main.MainControl.BindingSettingsProvider));
		}
		public BindingEditorControlMainViewModel Main { get; private set; }
		public string Header {
			get { return header; }
			set { SetProperty(ref header, value, () => Header); }
		}
		public IBindingEditorControlBindingSource Source {
			get { return source; }
			protected set { SetProperty(ref source, value, () => Source, OnSourceChanged); }
		}
		protected virtual void OnSourceChanged() {
			UpdateModes();
			((System.Windows.DependencyObject)Main.MainControl).Dispatcher.BeginInvoke((Action)((WpfDelegateCommand)OKCommand).RaiseCanExecuteChanged);
		}
		public string Path {
			get { return path; }
			protected set { SetProperty(ref path, value, () => Path, UpdateModes); }
		}
		public IEnumerable<BindingSettingDescription<BindingMode>> Modes {
			get { return modes; }
			private set { SetProperty(ref modes, value, () => Modes); }
		}
		public IEnumerable<BindingSettingDescription<UpdateSourceTrigger>> UpdateSourceTriggers {
			get { return updateSourceTriggers; }
			private set { SetProperty(ref updateSourceTriggers, value, () => UpdateSourceTriggers); }
		}
		public BindingSettingDescription<BindingMode> Mode {
			get { return mode; }
			set { SetProperty(ref mode, value, () => Mode, OnModeChanged); }
		}
		public BindingSettingDescription<UpdateSourceTrigger> UpdateSourceTrigger {
			get { return updateSourceTrigger; }
			set { SetProperty(ref updateSourceTrigger, value, () => UpdateSourceTrigger); }
		}
		public bool UpdateSourceTriggerEnabled {
			get { return updateSourceTriggerEnabled; }
			private set { SetProperty(ref updateSourceTriggerEnabled, value, () => UpdateSourceTriggerEnabled, OnUpdateSourceTriggerEnabledChanged); }
		}
		public IEnumerable<BindingEditorControlConverterViewModel> Converters {
			get { return converters; }
			private set { SetProperty(ref converters, value, () => Converters); }
		}
		public IBindingEditorControlConverter Converter {
			get { return converter; }
			set { SetProperty(ref converter, value, () => Converter); }
		}
		public ICommand OKCommand {
			get {
				if(okCommand == null)
					okCommand = new WpfDelegateCommand(Done, CanDone, false);
				return okCommand;
			}
		}
		public ICommand CancelCommand {
			get {
				if(cancelCommand == null)
					cancelCommand = new WpfDelegateCommand(Close, false);
				return cancelCommand;
			}
		}
		public void Done() {
			BindingDescription bindingDescription = new BindingDescription();
			bindingDescription.Source = Source;
			bindingDescription.Mode = Mode;
			bindingDescription.Path = Path;
			bindingDescription.UpdateSourceTrigger = UpdateSourceTrigger;
			bindingDescription.Converter = Converter;
			Main.MainControl.Binding = bindingDescription;
			Close();
		}
		public bool CanDone() {
			return Source != null;
		}
		public void Close() {
			if(Main.MainControl.CloseCommand != null && Main.MainControl.CloseCommand.CanExecute(Main.MainControl.CloseCommandParameter))
				Main.MainControl.CloseCommand.Execute(Main.MainControl.CloseCommandParameter);
		}
		void OnMainControlBindingSettingsProviderChanged(object sender, ThePropertyChangedEventArgs<IBindingEditorControlBindingSettingsProvider> e) {
			IEnumerable<BindingSettingDescription<UpdateSourceTrigger>> updateSourceTriggers = Main.MainControl.BindingSettingsProvider == null ? null : Main.MainControl.BindingSettingsProvider.GetUpdateSourceTriggers();
			UpdateSourceTriggers = updateSourceTriggers == null ? null : updateSourceTriggers.ToArray();
			UpdateSourceTrigger = UpdateSourceTriggers == null ? null : UpdateSourceTriggers.FirstOrDefault();
			IEnumerable<IBindingEditorControlConverter> converters = Main.MainControl.BindingSettingsProvider == null ? null : Main.MainControl.BindingSettingsProvider.GetConverters();
			Converters = converters == null ? null : converters.Select(c => new BindingEditorControlConverterViewModel(c)).ToArray();
			UpdateModes();
		}
		void UpdateModes() {
			IEnumerable<BindingSettingDescription<BindingMode>> modes = Main.MainControl.BindingSettingsProvider == null ? null : Main.MainControl.BindingSettingsProvider.GetModes(Source, Path);
			Modes = modes == null ? null : modes.ToArray();
			Mode = Modes == null ? null : Modes.FirstOrDefault();
		}
		void OnModeChanged() {
			UpdateSourceTriggerEnabled = Mode.ActualValue == BindingMode.TwoWay || Mode.ActualValue == BindingMode.OneWayToSource;
		}
		void OnUpdateSourceTriggerEnabledChanged() {
			UpdateSourceTrigger = UpdateSourceTriggers == null ? null : UpdateSourceTriggers.FirstOrDefault();
		}
	}
}
