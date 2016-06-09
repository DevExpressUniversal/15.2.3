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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using DevExpress.Data;
using System.ComponentModel;
using System.Windows;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Carousel {
	public class ParameterCollection : NotificationCollectionBase {
		public ParameterCollection() { }
		public Parameter this[int index] {
			get {
				if(index >= base.Count || index < 0)
					return null;
				return (base.List[index] as Parameter);
			}
		}
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			Parameter parameter = (Parameter)e.Element;
			switch(e.Action) {
				case CollectionChangeAction.Add:
					parameter.ParameterChanged += OnParameterChanged;
					break;
				case CollectionChangeAction.Remove:
					parameter.ParameterChanged -= OnParameterChanged;
					break;
			}
			base.OnCollectionChanged(e);
		}
		public Parameter this[string parameterName] {
			get {
				foreach(Parameter item in this.List)
					if(item.Name == parameterName)
						return item;
				return null;
			}
		}
		public void Remove(Parameter parameter) {
			base.List.Remove(parameter);
		} 
		public void Add(Parameter parameter) {
			foreach(Parameter existingParameter in new ArrayList(this))
				if(existingParameter.Name == parameter.Name) {
					this.Remove(existingParameter);
				}
			if(parameter.Name == BuiltInParametersNames.OffsetX ||
				parameter.Name == BuiltInParametersNames.OffsetY)
				throw new ArgumentException("BuiltIn Parameter with same name is already exists", parameter.Name);
			base.List.Add(parameter);
		}
		void OnParameterChanged() {
			base.OnCollectionChanged(null);
		}
		public void Add(string name, FunctionBase function) {
			Add(new Parameter(name, function));
		}
	}
	public class ParameterCollectionChangedEventManager : WeakEventManager {
		public static void AddListener(ParameterCollection source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(ParameterCollection source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		static ParameterCollectionChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(ParameterCollectionChangedEventManager);
				ParameterCollectionChangedEventManager currentManager = (ParameterCollectionChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if (currentManager == null) {
					currentManager = new ParameterCollectionChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		ParameterCollectionChangedEventManager() { }
		protected override void StartListening(object source) {
			ParameterCollection changed = (ParameterCollection)source;
			changed.CollectionChanged += CollectionChanged;
		}
		protected override void StopListening(object source) {
			ParameterCollection changed = (ParameterCollection)source;
			changed.CollectionChanged -= CollectionChanged;
		}
		void CollectionChanged(object sender, CollectionChangeEventArgs e) {
			base.DeliverEvent(sender, e);
		}
	}
}
