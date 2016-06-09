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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Bars {
	public enum ElementRegistratorChangeType { ElementAdded, ElementRemoved, ElementChanged }
	public class ElementRegistratorChangedArgs : EventArgs {
		public IBarNameScopeSupport Element { get; private set; }
		public object Name { get; private set; }
		public object OldName { get; private set; }
		public ElementRegistratorChangeType ChangeType { get; private set; }
		public ElementRegistratorChangedArgs(IBarNameScopeSupport element, object name, object oldName, ElementRegistratorChangeType changeType) {
			Element = element;
			Name = name;
			OldName = oldName;
			ChangeType = changeType;
		}
	}
	public delegate void ElementRegistratorChangedEventHandler(ElementRegistrator sender, ElementRegistratorChangedArgs e);
	public class ElementRegistrator {
		readonly object key;
		protected MultiDictionary<object, IBarNameScopeSupport> Elements { get; private set; }
		public bool Unique { get; private set; }
		public static bool GlobalSkipUniquenessCheck { get; set; }
		public bool SkipUniquenessCheck { get; set; }
		bool ActualSkipUniquenessCheck { get { return GlobalSkipUniquenessCheck || SkipUniquenessCheck; } }
		public object Key { get { return key; } }
		public Func<object, bool> ValidateNamePredicate { get; set; }
		public ElementRegistrator(object key, bool uniqueElements) {
			this.key = key;
			Elements = new MultiDictionary<object, IBarNameScopeSupport>();
			Unique = uniqueElements;			
		}
		protected bool IsValidName(IBarNameScopeSupport element) {
			return IsValidName(GetName(element));
		}
		protected bool IsValidName(object name) {
			return ValidateNamePredicate(name);
		}
		public void Add(IBarNameScopeSupport element) {
			if (!IsValidName(element) || Elements.ContainsValue(element))
				return;
			var elementName = GetName(element);
			if (Unique && Elements.ContainsKey(elementName) && !ActualSkipUniquenessCheck)
				throw new InvalidOperationException("Element with the same name already exists in the scope");			
			Elements.Add(elementName, element);
			RaiseChanged(element, ElementRegistratorChangeType.ElementAdded, elementName, elementName);
		}
		public void Remove(IBarNameScopeSupport element) {
			if (!Elements.ContainsValue(element))
				return;
			if (!IsValidName(element) || !Elements.RemoveItem(GetName(element), element)) {
				List<KeyValuePair<object, IBarNameScopeSupport>> kps = new List<KeyValuePair<object, IBarNameScopeSupport>>();
				foreach (var kp in Elements) {
					if (kp.Value == element)
						kps.Add(kp);
				}
				foreach (var kp in kps) {
					Elements.RemoveItem(kp.Key, kp.Value);
				}
			}
			RaiseChanged(element, ElementRegistratorChangeType.ElementRemoved, GetName(element), GetName(element));
		}
		public void NameChanged(IBarNameScopeSupport element, object oldValue, object newValue) {			
			if (IsValidName(oldValue) && Elements.ContainsKey(oldValue))
				Elements.RemoveItem(oldValue, element);
			if (IsValidName(newValue))
				Elements.Add(newValue, element);
			RaiseChanged(element, ElementRegistratorChangeType.ElementChanged, oldValue, newValue);
		}
		public IEnumerable<IBarNameScopeSupport> this[object name] { get { return Wrap(GetValue(name)); } }
		public IEnumerable<IBarNameScopeSupport> Values { get { return Wrap(GetValues()); } }		
		IEnumerable<IBarNameScopeSupport> GetValue(object name) {
			if (name == null || !Elements.ContainsKey(name))
				yield break;
			if (Unique && ActualSkipUniquenessCheck) {
				var value = Elements[name].With(Enumerable.FirstOrDefault);
				if (value == null)
					yield break;
				yield return value;
			} else {
				foreach (var element in Elements[name])
					yield return element;
			}
		}
		IEnumerable<IBarNameScopeSupport> GetValues() {
			return Elements.Keys.SelectMany(GetValue);			
		}
		IEnumerable<IBarNameScopeSupport> Wrap(IEnumerable<IBarNameScopeSupport> source) {
			return source.ToList().AsReadOnly(); 
		}
		public event ElementRegistratorChangedEventHandler Changed;
		protected void RaiseChanged(IBarNameScopeSupport element, ElementRegistratorChangeType change, object oldName, object currentName) {
			if (Changed == null)
				return;
			Changed(this, new ElementRegistratorChangedArgs(element, currentName, oldName, change));
		}
		public object GetName(IBarNameScopeSupport element) {
			if (element is IMultipleElementRegistratorSupport)
				return ((IMultipleElementRegistratorSupport)element).GetName(key);			
			throw new ArgumentException("element");
		}
		public void Detach() {
			while (Elements.Count > 0)
				Remove(Elements.Values.First());
		}
	}
	class ElementRegistratorService : IElementRegistratorService {
		BarNameScope scope;
		internal ElementRegistratorService(BarNameScope scope) { this.scope = scope; }
		void IElementRegistratorService.Changed(IBarNameScopeSupport element, object registratorKey) {
			var name = ((IMultipleElementRegistratorSupport)element).GetName(registratorKey);
			((IElementRegistratorService)this).NameChanged(element, registratorKey, name, name, true);
		}
		void IElementRegistratorService.NameChanged(IBarNameScopeSupport element, object registratorKey, object oldName, object newName, bool skipNameEqualityCheck) {
			if (scope == null || Equals(oldName, newName) && !skipNameEqualityCheck)
				return;
			scope[registratorKey].NameChanged(element, oldName, newName);
		}	
		IEnumerable<TRegistratorKey> IElementRegistratorService.GetElements<TRegistratorKey>(object name) {
			return ((IElementRegistratorService)this).GetElements<TRegistratorKey>(name, ScopeSearchSettings.Local);
		}
		IEnumerable<TRegistratorKey> IElementRegistratorService.GetElements<TRegistratorKey>(object name, ScopeSearchSettings searchMode) {
			if (scope == null)
				return Enumerable.Empty<TRegistratorKey>();
			return scope.ScopeTree.Find(x => x[typeof(TRegistratorKey)][name].Any(), searchMode).SelectMany(x => x[typeof(TRegistratorKey)][name]).OfType<TRegistratorKey>();
		}
		IEnumerable<TRegistratorKey> IElementRegistratorService.GetElements<TRegistratorKey>() {
			return ((IElementRegistratorService)this).GetElements<TRegistratorKey>(ScopeSearchSettings.Local);
		}
		IEnumerable<TRegistratorKey> IElementRegistratorService.GetElements<TRegistratorKey>(ScopeSearchSettings searchMode) {
			if (scope == null)
				return Enumerable.Empty<TRegistratorKey>();
			return scope.ScopeTree.Find(searchDirection: searchMode).SelectMany(x => x[typeof(TRegistratorKey)].Values).OfType<TRegistratorKey>();
		} 
		public static IEnumerable<BarNameScope> ParentsAndSelf(BarNameScope scope) {
			if (scope == null) yield break;
			while (scope != null) {
				yield return scope;
				scope = scope.Parent;
			}
		}		
	}
}
