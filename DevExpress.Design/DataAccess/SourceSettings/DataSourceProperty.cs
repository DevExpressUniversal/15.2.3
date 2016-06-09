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
namespace DevExpress.Design.DataAccess {
	sealed class DataSourceProperty : LocalizableDataAccessObject<DataSourcePropertyCodeName>, IDataSourceProperty {
		public DataSourceProperty(DataSourcePropertyCodeName codeName, IDataSourceSettingsModel settingsModel)
			: base(codeName) {
#if !DEBUGTEST
			AssertionException.IsNotNull(settingsModel);
#endif
			this.SettingsModel = settingsModel;
		}
		public IDataSourceSettingsModel SettingsModel {
			get;
			private set;
		}
		protected override int GetHash(DataSourcePropertyCodeName codeName) {
			return (int)codeName;
		}
		public bool ShowName {
			get {
				switch(((IDataSourceProperty)this).GetCodeName()) {
					case DataSourcePropertyCodeName.AreSourceRowsThreadSafe:
					case DataSourcePropertyCodeName.IsSynchronizedWithCurrentItem:
					case DataSourcePropertyCodeName.ShowCodeBehind:
					case DataSourcePropertyCodeName.ShowDesignData:
						return false;
				}
				return true;
			}
		}
	}
	sealed class CustomBindingDataSourceProperty : WpfBindableBase, ICustomBindingDataSourceProperty {
		ICustomBindingPropertyMetadata metadata;
		int hash;
		public CustomBindingDataSourceProperty(IDataSourceSettingsModel settingsModel, ICustomBindingPropertyMetadata metadata) {
			AssertionException.IsNotNull(metadata);
			this.metadata = metadata;
			this.hash = metadata.PropertyName.GetHashCode();
#if !DEBUGTEST
			AssertionException.IsNotNull(settingsModel);
#endif
			this.SettingsModel = settingsModel;
			SettingsModel.CustomBindingProperties.TryGetValue(metadata.PropertyName, out valueCore);
			ResetValueCommand = new Design.UI.WpfDelegateCommand(ResetValue);
			SettingsModel.PropertyChanged += SettingsModel_PropertyChanged;
		}
		void SettingsModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "SelectedElement") ResetValue();
		}
		public IDataSourceSettingsModel SettingsModel {
			get;
			private set;
		}
		string valueCore;
		public string Value {
			get { return valueCore; }
			set { SetProperty(ref valueCore, value, "Value", OnValueChanged); }
		}
		void OnValueChanged() {
			var storage = SettingsModel.CustomBindingProperties;
			string storageKey = metadata.PropertyName;
			if(Value == null)
				storage.Remove(storageKey);
			else {
				string storageValue;
				if(storage.TryGetValue(storageKey, out storageValue))
					storage[storageKey] = Value;
				else
					storage.Add(storageKey, Value);
			}
			RaisePropertyChanged("HasValue");
		}
		public bool HasValue {
			get { return SettingsModel.CustomBindingProperties.ContainsKey(metadata.PropertyName); }
		}
		void ResetValue() {
			Value = null;
		}
		public Design.UI.ICommand<object> ResetValueCommand {
			get;
			private set;
		}
		public string Name {
			get { return metadata.Name; }
		}
		public string Description {
			get { return metadata.Description; }
		}
		public bool ShowName {
			get { return true; }
		}
		public DataSourcePropertyCodeName GetCodeName() {
			return DataSourcePropertyCodeName.CustomBindingProperty;
		}
		public override int GetHashCode() {
			return (int)DataSourcePropertyCodeName.CustomBindingProperty ^ hash;
		}
		public sealed override bool Equals(object obj) {
			IDataSourceProperty source = obj as IDataSourceProperty;
			if(object.ReferenceEquals(source, null)) return false;
			CustomBindingDataSourceProperty customSource = obj as CustomBindingDataSourceProperty;
			if(!object.ReferenceEquals(customSource, null))
				return customSource.metadata.PropertyName == metadata.PropertyName;
			return source.GetCodeName().ToString() == metadata.PropertyName;
		}
	}
}
