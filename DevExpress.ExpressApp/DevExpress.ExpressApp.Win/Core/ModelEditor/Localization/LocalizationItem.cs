#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface ILocalizationItem : INotifyPropertyChanged {
		string PropertyName { get; }
		string DefaultLanguageValue { get; set; }
		string TranslatedValue { get; set; }
		bool IsCalculated { get; }
		bool IsTranslated { get; set; }
		bool IsModified { get; }
		string NodePath { get; }
		[Browsable(false)]
		string FullPath { get; }
		string Description { get; }
	}
	public class LocalizationItemBind : ILocalizationItem {
		#region UsingAspectScope
		class AspectScope : IDisposable {
			private ModelNode node;
			private string backupAspect;
			public AspectScope(ModelNode node, string aspect) {
				this.node = node;
				backupAspect = CurrentAspect;
				CurrentAspect = aspect;
			}
			private string CurrentAspect {
				get { return ((IModelApplicationServices)node.Root).CurrentAspectProvider.CurrentAspect; }
				set {
					if(CurrentAspect != value) {
						((IModelApplicationServices)node.Root).CurrentAspectProvider.CurrentAspect = value;
					}
				}
			}
			void IDisposable.Dispose() {
				CurrentAspect = backupAspect;
			}
		}
		#endregion
		public const string pathSeparator = "\\";
		private ModelNode node;
		private string nodePath;
		private string propertyName;
		private bool isPropertyCalculated;
		private bool defaultLanguageHasValue;
		private bool isDefaultValueModified;
		private string defaultLanguageValue;
		private string translatedAspect;
		private bool? isTranslated;
		private void Initialize(ModelNode node, string propertyName, string translatedAspect) {
			this.node = node;
			this.nodePath = ModelEditorHelper.GetModelNodePath(node);
			this.propertyName = propertyName;
			this.isPropertyCalculated = node.NodeInfo.IsCalculated(propertyName);
			using(new AspectScope(node, string.Empty)) {
				this.defaultLanguageHasValue = node.HasValue(propertyName);
				this.isDefaultValueModified = node.IsValueModified(propertyName);
				this.defaultLanguageValue = node.GetValue<string>(propertyName);
			}
			this.translatedAspect = translatedAspect;
		}
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public LocalizationItemBind(ModelNode node, string propertyName, string translatedAspect) {
			Initialize(node, propertyName, translatedAspect);
		}
		public void UpdateDefaultLanguageValue() {
			using(new AspectScope(node, string.Empty)) {
				string currentValue = node.GetValue<string>(propertyName);
				if(DefaultLanguageValue != currentValue) {
					defaultLanguageValue = currentValue;
					OnPropertyChanged("DefaultLanguageValue");
				}
			}
		}
		public void Undo() {
			if(IsModified) {
				using(new AspectScope(node, string.Empty)) {
					isDefaultValueModified = false;
					node.ClearValue(propertyName);
					defaultLanguageValue = node.GetValue<string>(propertyName);
					OnPropertyChanged("TranslatedValue");
					OnPropertyChanged("DefaultLanguageValue");
				}
				isTranslated = null;
			}
		}
		public bool IsModified {
			get {
				if(isDefaultValueModified) {
					return true;
				}
				using(new AspectScope(node, translatedAspect)) {
					return node.IsValueModified(propertyName);
				}
			}
		}
		public string FullPath {
			get { return NodePath + pathSeparator + PropertyName; }
		}
		public string NodePath {
			get { return nodePath; }
		}
		public string PropertyName {
			get { return propertyName; }
		}
		public bool IsCalculated {
			get {
				if(!isPropertyCalculated || defaultLanguageHasValue) {
					return false;
				}
				using(new AspectScope(node, translatedAspect)) {
					return !node.HasValue(propertyName);
				}
			}
		}
		public string DefaultLanguageValue {
			get { return defaultLanguageValue; }
			set {
				if(defaultLanguageValue != value) {
					defaultLanguageValue = value;
					using(new AspectScope(node, string.Empty)) {
						node.SetValue<string>(propertyName, value);
						defaultLanguageHasValue = node.HasValue(propertyName);
						isDefaultValueModified = node.IsValueModified(propertyName);
					}
					OnPropertyChanged("DefaultLanguageValue");
				}
			}
		}
		public bool IsTranslated {
			get {
				if(!isTranslated.HasValue) {
					isTranslated = DefaultLanguageValue != TranslatedValue || ModelEditorHelper.HasValueInCurrentAspect(node, propertyName);
				}
				return isTranslated.Value;
			}
			set {
				if(value) {
					ModelEditorHelper.ForceSetValueInCurrentAspect<string>(node, propertyName, DefaultLanguageValue);
				}
				else {
					Undo();
				}
				isTranslated = null;
			}
		}
		public string TranslatedValue {
			get {
				using(new AspectScope(node, translatedAspect)) {
					try {
						return node.GetValue<string>(propertyName);
					}
					catch {
						return null;
					}
				}
			}
			set {
				if(TranslatedValue != value) {
					using(new AspectScope(node, translatedAspect)) {
						node.SetValue<string>(propertyName, value);
					}
					isTranslated = null;
					OnPropertyChanged("TranslatedValue");
				}
			}
		}
		public string Description {
			get { return ModelEditorHelper.GetPropertyDescriptionAttributeValue(node, propertyName); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public class LocalizationItemUnbind : ILocalizationItem {
		public const string pathSeparator = "\\";
		private bool isModified;
		private string nodePath;
		private string propertyName;
		private string defaultLanguageValue;
		private bool isTranslated;
		private string translatedValue;
		private string description;
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public LocalizationItemUnbind(string propertyName, string sourceAspectValue, string destinationAspectValue, string nodePath, string description, string isTranslated) {
			this.isModified = false;
			this.nodePath = nodePath;
			this.propertyName = propertyName;
			this.defaultLanguageValue = sourceAspectValue;
			this.translatedValue = destinationAspectValue;
			this.description = description;
			if(!Boolean.TryParse(isTranslated, out this.isTranslated)) {
				this.isTranslated = false;
			}
		}
		public bool IsModified {
			get { return isModified; }
		}
		public string FullPath {
			get { return NodePath + pathSeparator + PropertyName; }
		}
		public string NodePath {
			get { return nodePath; }
		}
		public string PropertyName {
			get { return propertyName; }
		}
		public bool IsCalculated {
			get { return false; }
		}
		public string DefaultLanguageValue {
			get { return defaultLanguageValue; }
			set {
				if(defaultLanguageValue != value) {
					defaultLanguageValue = value;
					isModified = true;
					OnPropertyChanged("DefaultLanguageValue");
				}
			}
		}
		public bool IsTranslated {
			get { return isTranslated || DefaultLanguageValue != TranslatedValue; }
			set { throw new NotSupportedException(); }
		}
		public string TranslatedValue {
			get { return translatedValue; }
			set {
				if(TranslatedValue != value) {
					translatedValue = value;
					isModified = true;
					OnPropertyChanged("TranslatedValue");
				}
			}
		}
		public string Description {
			get { return description; }
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
