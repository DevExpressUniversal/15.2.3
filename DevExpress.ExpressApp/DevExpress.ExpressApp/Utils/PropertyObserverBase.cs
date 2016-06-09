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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public class PropertyObserverBase : IDisposable, INotifyPropertyChanged {
		private string propertyName;
		private ISelectionContext selectionContext;
		private object currentObject;
		private object propertyValue;
		private void RemovePropertyChangedHandler(object obj, PropertyChangedEventHandler handler) {
			INotifyPropertyChanged supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
			if(supportNotifyPropertyChanged != null) {
				supportNotifyPropertyChanged.PropertyChanged -= handler;
			}
		}
		private void AddPropertyChangedHandler(object obj, PropertyChangedEventHandler handler) {
			INotifyPropertyChanged supportNotifyPropertyChanged = obj as INotifyPropertyChanged;
			if(supportNotifyPropertyChanged != null) {
				supportNotifyPropertyChanged.PropertyChanged += handler;
			}
		}
		private void selectionContext_CurrentObjectChanged(object sender, EventArgs e) {
			CurrentObject = selectionContext.CurrentObject;
		}
		private void currentObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(!string.IsNullOrEmpty(propertyName) && (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == propertyName))) {
				OnCurrentObjectPropertyChanged();
			}
		}
		protected void OnCurrentObjectPropertyChanged() {
			RemovePropertyChangedHandler(propertyValue, propertyValueChanged);
			PropertyValue = GetPropertyValue(CurrentObject);
			AddPropertyChangedHandler(propertyValue, propertyValueChanged);
		}
		private void propertyValueChanged(object sender, PropertyChangedEventArgs e) {
			OnPropertyValueChanged();
		}
		protected void OnPropertyValueChanged() {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public PropertyObserverBase(ISelectionContext selectionContext, string propertyName) {
			if(selectionContext != null) {
				this.selectionContext = selectionContext;
				this.propertyName = propertyName;
				CurrentObject = selectionContext.CurrentObject;
				selectionContext.CurrentObjectChanged += new EventHandler(selectionContext_CurrentObjectChanged);
			}
		}
		public virtual void Dispose() {
			RemovePropertyChangedHandler(currentObject, currentObjectPropertyChanged);
			RemovePropertyChangedHandler(propertyValue, propertyValueChanged);
			if(selectionContext != null) {
				selectionContext.CurrentObjectChanged -= new EventHandler(selectionContext_CurrentObjectChanged);
				selectionContext = null;
			}
			currentObject = null;
			propertyValue = null;
		}
		protected virtual string PropertyName {
			get { return propertyName; }
		}
		protected virtual object CurrentObject {
			get { return currentObject; }
			set {
				RemovePropertyChangedHandler(currentObject, currentObjectPropertyChanged);
				currentObject = value;
				AddPropertyChangedHandler(currentObject, currentObjectPropertyChanged);
				OnCurrentObjectPropertyChanged();
			}
		}
		public object GetPropertyValue(object theObject) {
			if(theObject != null && !string.IsNullOrEmpty(propertyName)) {
				return ReflectionHelper.GetMemberValue(theObject, propertyName);
			}
			else {
				return null;
			}
		}
		public object PropertyValue {
			get { return propertyValue; }
			set {
				propertyValue = value;
				OnPropertyValueChanged();
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
