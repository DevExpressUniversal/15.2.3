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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.Bars.Native {	
	interface IElementBinder {
		bool OnRegistratorChanged(IElementBinder binder, ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator);
		bool PropagateRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator);
		ElementRegistrator ElementRegistrator { get; }
		ElementRegistrator LinkRegistrator { get; }
		IList<IBarNameScopeSupport> BindedElements { get; }
		IList<IBarNameScopeSupport> BindedLinks { get; }
		BarNameScope Scope { get; }
		ScopeSearchSettings ElementSearchSettings { get; }
		ScopeSearchSettings LinkSearchSettings { get; }
		void Link(IBarNameScopeSupport first, IBarNameScopeSupport second);
		void Unlink(IBarNameScopeSupport first, IBarNameScopeSupport second);
		bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second);
		bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second);
	}	
	class ElementBinderWorker {
		object elementRegistratorKey, linkRegistratorKey;
		IElementBinder OwnerBinder { get; set; }
		IElementBinder SourceBinder { get; set; }
		int Level { get; set; }
		bool first_stopAfterBinding, second_stopAfterBinding = false;
		ElementRegistrator firstOwnerRegistrator;
		ElementRegistrator secondOwnerRegistrator;
		IList<IBarNameScopeSupport> firstOwnerElements;
		IList<IBarNameScopeSupport> secondOwnerElements;
		ElementRegistrator firstSourceRegistrator;
		ElementRegistrator secondSourceRegistrator;
		IList<IBarNameScopeSupport> firstSourceElements;
		IList<IBarNameScopeSupport> secondSourceElements;
		bool firstIsElement;
		ScopeSearchSettings FirstSearchSettings { get; set; }
		ScopeSearchSettings SecondSearchSettings { get; set; }		
		protected ElementBinderWorker(IElementBinder owner, IElementBinder source) {
			this.OwnerBinder = owner;
			this.SourceBinder = source;
			elementRegistratorKey = OwnerBinder.ElementRegistrator.Key;
			linkRegistratorKey = OwnerBinder.LinkRegistrator.Key;
		}
		void Prepare(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			firstIsElement = isElementRegistrator;
			Level = ScopeTree.GetLevel(OwnerBinder.Scope, SourceBinder.Scope, firstIsElement ? OwnerBinder.ElementSearchSettings : OwnerBinder.LinkSearchSettings);
			AssignRegistratorAndElements(firstIsElement, OwnerBinder, ref firstOwnerRegistrator, ref secondOwnerRegistrator, ref firstOwnerElements, ref secondOwnerElements);
			if (!Equals(OwnerBinder, SourceBinder))
				AssignRegistratorAndElements(firstIsElement, SourceBinder, ref firstSourceRegistrator, ref secondSourceRegistrator, ref firstSourceElements, ref secondSourceElements);
			first_stopAfterBinding = firstOwnerRegistrator.Unique;
			second_stopAfterBinding = secondOwnerRegistrator.Unique;   
			FirstSearchSettings = firstIsElement ? OwnerBinder.ElementSearchSettings : OwnerBinder.LinkSearchSettings;
			SecondSearchSettings = firstIsElement ? OwnerBinder.LinkSearchSettings : OwnerBinder.ElementSearchSettings;
		}
		private void AssignRegistratorAndElements(bool firstIsElement, IElementBinder binder,
			ref ElementRegistrator firstRegistrator, ref ElementRegistrator secondRegistrator,
			ref IList<IBarNameScopeSupport> firstElements, ref IList<IBarNameScopeSupport> secondElements) {
				firstRegistrator = firstIsElement ? binder.ElementRegistrator : binder.LinkRegistrator;
				secondRegistrator = firstIsElement ? binder.LinkRegistrator : binder.ElementRegistrator;
				firstElements = firstIsElement ? binder.BindedElements : binder.BindedLinks;
				secondElements = firstIsElement ? binder.BindedLinks : binder.BindedElements;
		}
		public bool OnRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			Prepare(sender, e, isElementRegistrator);
			if (Level == -1) {
				return false;
			}
			if (e.ChangeType == ElementRegistratorChangeType.ElementChanged || e.ChangeType == ElementRegistratorChangeType.ElementRemoved) {
				UnlinkByElement(e.Element);				
			}
			if (e.ChangeType == ElementRegistratorChangeType.ElementChanged || e.ChangeType == ElementRegistratorChangeType.ElementAdded) {
				if (LinkByElement(e.Element))
					return true;
			}
			return false;
		}
		bool LinkByElement(IBarNameScopeSupport firstElement) {
			Queue<Action> propagateQueue = new Queue<Action>();
			try {
				foreach (var secondElement in secondOwnerRegistrator[firstOwnerRegistrator.GetName(firstElement)]) {
					bool skip = false;
					if (first_stopAfterBinding) {
						for (int i = 0; i < secondOwnerElements.Count; i++) {
							if (!Equals(secondOwnerElements[i], secondElement))
								continue;
							var originalFirstElement = firstOwnerElements[i];
							var originalFirstElementScope = BarNameScope.GetScope(originalFirstElement as DependencyObject);
							var originalLevel = ScopeTree.GetLevel(OwnerBinder.Scope, originalFirstElementScope, FirstSearchSettings);
							if (originalLevel != -1 && originalLevel <= Level) { 
								skip = true;
								break;
							}
							if (!OwnerBinder.CanUnlink(originalFirstElement, secondElement))
								continue;
							firstOwnerElements.RemoveAt(i);
							secondOwnerElements.RemoveAt(i);
							if (secondSourceElements != null) {
								var sseI = secondSourceElements.IndexOf(secondElement);
								if (secondSourceElements.IsValidIndex(sseI)) {
									secondSourceElements.RemoveAt(i);
									firstSourceElements.RemoveAt(i);
								}
							}
							OwnerBinder.Unlink(originalFirstElement, secondElement);
							if (Equals(originalFirstElementScope, OwnerBinder.Scope)) {
								propagateQueue.Enqueue(() => OwnerBinder.PropagateRegistratorChanged(firstOwnerRegistrator, new ElementRegistratorChangedArgs(originalFirstElement, null, null, ElementRegistratorChangeType.ElementAdded), firstIsElement));
							}
						}
					}
					if (skip)
						continue;
					if (!CanLink(secondElement, firstElement))
						continue;
					firstOwnerElements.Add(firstElement);
					secondOwnerElements.Add(secondElement);
					Link(secondElement, firstElement);
					if (!Equals(OwnerBinder, SourceBinder)) {
						firstSourceElements.Add(firstElement);
						secondSourceElements.Add(secondElement);
					}
					if (second_stopAfterBinding)
						return true;
				}
			} finally {
				propagateQueue.Enqueue(null);
				Action action;
				while ((action = propagateQueue.Dequeue()) != null) {
					action();
				}
			}
			return false;
		}
		bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			return !firstIsElement ? OwnerBinder.CanLink(first, second) : OwnerBinder.CanLink(second, first);
		}
		void Link(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			if (!firstIsElement)
				OwnerBinder.Link(first, second);
			else
				OwnerBinder.Link(second, first);
		}
		bool UnlinkByElement(IBarNameScopeSupport element) {
			Queue<Action> propagateQueue = new Queue<Action>();
			for (int i = firstOwnerElements.Count - 1; i >= 0; i--) {
				if (firstOwnerElements[i] != element) continue;
				var first = firstOwnerElements[i];
				var second = secondOwnerElements[i];
				if (!OwnerBinder.CanUnlink(first, second))
					continue;
				if(!Equals(OwnerBinder, SourceBinder))
					for (int j = firstSourceElements.Count-1; j >= 0; j--) {
						if (Equals(firstSourceElements[j], first) && Equals(secondSourceElements[j], second)) {
							secondSourceElements.RemoveAt(j);
							firstSourceElements.RemoveAt(j);
						}
					}
				firstOwnerElements.RemoveAt(i);
				secondOwnerElements.RemoveAt(i);
				OwnerBinder.Unlink(first, second);
				propagateQueue.Enqueue(() => OwnerBinder.PropagateRegistratorChanged(OwnerBinder.Scope[secondOwnerRegistrator.Key], new ElementRegistratorChangedArgs(second, null, null, ElementRegistratorChangeType.ElementAdded), !firstIsElement));
			}
			propagateQueue.Enqueue(null);
			Action action;
			while ((action = propagateQueue.Dequeue()) != null) {
				action();
			}
			return false;
		}
		public static bool OnRegistratorChanged(IElementBinder binder, IElementBinder second, ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			return new ElementBinderWorker(binder, second).OnRegistratorChanged(sender, e, isElementRegistrator);
		}		
	}	
	public abstract class ElementBinder<TServiceKey> : IBarNameScopeDecorator, IElementBinder where TServiceKey : IElementBinderService {
		readonly object elementRegistratorKey;
		readonly object linkRegistratorKey;
		readonly ScopeSearchSettings elementSearchSettings;
		readonly ScopeSearchSettings linkSearchSettings;
		BarNameScope scope;
		protected ElementRegistrator ElementRegistrator { get; private set; }
		protected ElementRegistrator LinkRegistrator { get; private set; }
		protected BarNameScope Scope { get { return scope; } }
		List<IBarNameScopeSupport> BindedElements { get; set; }
		List<IBarNameScopeSupport> BindedLinks { get; set; }
		protected ElementBinder(
			object elementRegistratorKey,
			object linkRegistratorKey,
			ScopeSearchSettings elementSearchSettings = ScopeSearchSettings.Ancestors | ScopeSearchSettings.Local,
			ScopeSearchSettings linkSearchSettings = ScopeSearchSettings.Descendants | ScopeSearchSettings.Local) {
			this.elementRegistratorKey = elementRegistratorKey;
			this.linkRegistratorKey = linkRegistratorKey;
			this.elementSearchSettings = elementSearchSettings;
			this.linkSearchSettings = linkSearchSettings;
		}
		protected void OnRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			bool checkLocalRegistrators = Equals(ElementRegistrator, sender) && linkSearchSettings.HasFlag(ScopeSearchSettings.Local) ||
				Equals(LinkRegistrator, sender) && elementSearchSettings.HasFlag(ScopeSearchSettings.Local);			
			if(checkLocalRegistrators && OnRegistratorChanged(this, sender, e, isElementRegistrator))
				return;
			PropagateRegistratorChanged(sender, e, isElementRegistrator);
		}
		bool PropagateRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			foreach (var sc in scope.ScopeTree.Find(searchDirection: ScopeSearchSettings.Ancestors | ScopeSearchSettings.Descendants)) {
				if (sc.GetService<TServiceKey>().RegistratorChanged(this, sender, e, isElementRegistrator))
					return true;
			}
			return false;
		}
		bool OnRegistratorChanged(object binder, ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) {
			return ElementBinderWorker.OnRegistratorChanged(this, binder as IElementBinder, sender, e, isElementRegistrator);
		}
		bool IsOwnRegistrator(ElementRegistrator registrator) {
			return Equals(ElementRegistrator, registrator) || Equals(LinkRegistrator, registrator);
		}	 
		protected abstract bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second);
		protected abstract bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second);
		protected abstract void Link(IBarNameScopeSupport element, IBarNameScopeSupport link);
		protected abstract void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link);
		void IBarNameScopeDecorator.Attach(BarNameScope scope) {
			ElementRegistrator = scope[elementRegistratorKey];
			LinkRegistrator = scope[linkRegistratorKey];
			ElementRegistrator.ValidateNamePredicate = ElementRegistrator.ValidateNamePredicate;
			LinkRegistrator.ValidateNamePredicate = LinkRegistrator.ValidateNamePredicate;
			this.BindedElements = new List<IBarNameScopeSupport>();
			this.BindedLinks = new List<IBarNameScopeSupport>();
			ElementRegistrator.Changed += (r, e) => OnRegistratorChanged(r, e, true);
			LinkRegistrator.Changed += (r, e) => OnRegistratorChanged(r, e, false);
			this.scope = scope;
		}
		void IBarNameScopeDecorator.Detach() {
		}		
		#region IElementBinder Members
		bool IElementBinder.OnRegistratorChanged(IElementBinder binder, ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) { return OnRegistratorChanged(binder, sender, e, isElementRegistrator); }
		bool IElementBinder.PropagateRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e, bool isElementRegistrator) { return PropagateRegistratorChanged(sender, e, isElementRegistrator); }
		ElementRegistrator IElementBinder.ElementRegistrator { get { return ElementRegistrator; } }
		ElementRegistrator IElementBinder.LinkRegistrator { get { return LinkRegistrator; } }
		IList<IBarNameScopeSupport> IElementBinder.BindedElements { get { return BindedElements; } }
		IList<IBarNameScopeSupport> IElementBinder.BindedLinks { get { return BindedLinks; } }
		BarNameScope IElementBinder.Scope { get { return scope; } }
		ScopeSearchSettings IElementBinder.ElementSearchSettings { get { return elementSearchSettings; } }
		ScopeSearchSettings IElementBinder.LinkSearchSettings { get { return linkSearchSettings; } }
		bool IElementBinder.CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return CanLink(first, second); }		
		bool IElementBinder.CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return CanUnlink(first, second); }
		void IElementBinder.Link(IBarNameScopeSupport first, IBarNameScopeSupport second) { Link(first, second); }
		void IElementBinder.Unlink(IBarNameScopeSupport first, IBarNameScopeSupport second) { Unlink(first, second); }
		#endregion
	}
	public abstract class ElementBinderService : IElementBinderService {
		readonly IElementBinder binder;
		public ElementBinderService(object binder) {
			this.binder = binder as IElementBinder;
		}
		#region IRegistratorChangedListener Members
		bool IRegistratorChangedListener.RegistratorChanged(object binder, ElementRegistrator registrator, ElementRegistratorChangedArgs args, bool isElementRegistrator) {
			var bnd = binder as IElementBinder;
			return this.binder.OnRegistratorChanged(bnd, registrator, args, isElementRegistrator);
		}
		#endregion        
		#region IElementBinderService Members
		IEnumerable<IBarNameScopeSupport> IElementBinderService.GetMatches(object el) {
			var element = el as IBarNameScopeSupport;
			if (Equals(null, element))
				yield break;
			var eKey = (element as IMultipleElementRegistratorSupport).With(x => x.RegistratorKeys);
			if (eKey == null)
				yield break;
			bool er = eKey.Contains(binder.ElementRegistrator.Key);
			var fr = er ? binder.BindedElements : binder.BindedLinks;
			var sr = er ? binder.BindedLinks : binder.BindedElements;
			for (int i = 0; i < fr.Count; i++) {
				if (Equals(fr[i], element))
					yield return sr[i];
			}
			foreach(var item in GetAdditionalMatches(el))
				yield return item;
		}
		protected virtual IEnumerable<IBarNameScopeSupport> GetAdditionalMatches(object element) { yield break; }
		#endregion
	}
	public interface IItemToLinkBinderService : IElementBinderService {
		void Lock();
		void Unlock();
	}
	public class ItemToLinkBinder : ElementBinder<IItemToLinkBinderService> {
		public ItemToLinkBinder()
			: base(typeof(BarItem), typeof(BarItemLink)) {
		}
		protected override sealed bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			return (BarNameScope.IsInSameScope(first, second)
				|| IsValidLinkForItem(first as BarItem ?? second as BarItem, first as BarItemLink ?? second as BarItemLink))
				&& (!(first as BarItemLink).Return(x => x.HasStrongLinkedItem, () => false)
				&& !(second as BarItemLink).Return(x => x.HasStrongLinkedItem, () => false));
		}
		protected override sealed bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			return true;
		}
		protected override sealed void Link(IBarNameScopeSupport element, IBarNameScopeSupport link) {
			if (link == null || element == null) return;
			Link(element as BarItem ?? link as BarItem, link as BarItemLink ?? element as BarItemLink);
		}
		protected override sealed void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link) {
			if (link == null) return;
			Unlink(element as BarItem ?? link as BarItem, link as BarItemLink ?? element as BarItemLink);
		}
		protected virtual void Link(BarItem barItem, BarItemLink barItemLink) {
			barItemLink.Link(barItem);
			CheckLink(barItem, barItemLink);
		}
		protected virtual void Unlink(BarItem barItem, BarItemLink barItemLink) {
			barItemLink.Unlink(barItem);
		}
		protected internal virtual bool IsValidLinkForItem(BarItem item, BarItemLink link) {
			if (!BarManager.CheckBarItemNames) return true;
			if (link == null) return true;
			if (link.GetType() == typeof(BarItemLink) || link.GetType() == typeof(BarItemLinkSeparator)) return true;
			if (item != null)
				if (link.GetType() == item.GetLinkType()) return true;
			return false;
		}
		protected internal virtual void CheckLink(BarItem item, BarItemLink link) {
			if (IsValidLinkForItem(item, link))
				return;
			string exception = "BarItemLink refers to unsupported BarItem. Cannot assign BarItemLink with BarItem by name: " + "\"" + ((BarItemLink)link).BarItemName + "\".";
			exception += Environment.NewLine;
			exception += "To suppress this exception, set BarManager.CheckBarItemNames static property to False";
			throw new ArgumentException(exception);
		}
		public void LockLinks() {		   
			foreach (var link in LinkRegistrator.Values.OfType<BarItemLink>()) link.LockItem();
			foreach (var scope in Scope.Children) scope.GetService<IItemToLinkBinderService>().Lock();
		}
		public void UnlockLinks() {
			foreach (var link in LinkRegistrator.Values.OfType<BarItemLink>()) link.UnlockItem();
			foreach (var scope in Scope.Children) scope.GetService<IItemToLinkBinderService>().Unlock();
		}
	}
	sealed class ItemToLinkBinderService : ElementBinderService, IItemToLinkBinderService {
		ItemToLinkBinder Binder { get; set; }
		public ItemToLinkBinderService(object binder) : base(binder) {
			Binder = (ItemToLinkBinder)binder;
		}
		void IItemToLinkBinderService.Lock() { Binder.Do(x => x.LockLinks()); }
		void IItemToLinkBinderService.Unlock() { Binder.Do(x => x.UnlockLinks()); }
		protected override IEnumerable<IBarNameScopeSupport> GetAdditionalMatches(object element) { 
			var item = (element as BarItemLink).If(x=>x.HasStrongLinkedItem).With(x=>x.Item);
			if(item!=null)
				yield return item;
			var item2 = element as BarItem;
			if(item2!=null)
				foreach(BarItemLink link in item2.Links)
					if(link.HasStrongLinkedItem)
						yield return link;
		}
	}	
}
