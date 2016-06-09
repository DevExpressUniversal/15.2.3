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
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGauges.Core.Base {
	public interface INamed {
		string Name { get;set;}
	}
	public interface INamedObject : INamed {
		bool IsNameLocked { get;}
	}
	public interface IBaseObject : IDisposable {
		bool IsDisposing { get; }
		event EventHandler Disposed;
		event EventHandler Changed;
	}
	public interface IElement<T> : INamedObject, IBaseObject, ISupportVisitor<IElement<T>>, ISupportAcceptOrder
		where T : class {
		T Self { get;}
		IComposite<T> Parent { get;}
		bool IsLeaf { get;}
		bool IsComposite { get;}
		ILeaf<T> Leaf { get;}
		IComposite<T> Composite { get;}
	}
	public interface ILeaf<T> : IElement<T>
		where T : class {
	}
	public interface IComposite<T> : IElement<T>, ISupportAccessByName<IElement<T>>
		where T : class {
		void Add(IElement<T> element);
		void AddRange(IElement<T>[] elements);
		void Remove(IElement<T> element);
		IElementsReadOnlyCollection<T> Elements { get;}
	}
	public interface ISerizalizeableElement : IXtraSerializable, INamed {
		string ParentName { get;  set; }
		string ParentCollectionName { get; set; }
		string TypeNameEx { get; set; }
		List<ISerizalizeableElement> GetChildren();
		string BoundElementName { get; set;}
	}
	public interface IReadOnlyCollection<T> : IEnumerable<T>,
		ISupportVisitor<T>
		where T : class {
		int Count { get;}
		bool Contains(T element);
	}
	public interface IChangeableCollection<T> : IReadOnlyCollection<T>, ISupportNotification<T>
		where T : class {
		void Add(T element);
		void AddRange(T[] elements);
		bool Remove(T element);
		void CopyTo(T[] array, int index);
		void Clear();
	}
	public interface IElementsReadOnlyCollection<T> : IReadOnlyCollection<IElement<T>>,
		ISupportAccessByName<IElement<T>>,
		ISupportElementNotification<T>
		where T : class {
	}
	public interface ISupportNotification<T>
		where T : class {
		event CollectionChangedHandler<T> CollectionChanged;
	}
	public interface ISupportElementNotification<T> 
		where T : class {
		event ElementChangedHandler<T> Changed;
	}
	public interface ISupportAccessByName<T>
		where T : class {
		bool Contains(string name);
		T this[string name] { get;}
	}
	public interface ISupportAcceptOrder {
		int AcceptOrder { get;set;}
	}
	public interface ISupportVisitor<T>
		where T : class {
		void Accept(IVisitor<T> visitor);
		void Accept(VisitDelegate<T> visit);
	}
	public interface ISupportLockUpdate {
		void BeginUpdate();
		void CancelUpdate();
		void EndUpdate();
	}
	public interface ISupportCaching : IDisposable {
		bool IsCachingLocked { get;}
		void LockCaching();
		void UnlockCaching();
		void CacheValue(object key, object value);
		bool HasValue(object key);
		object TryGetValue(object key);
		void ResetCache(object key);
		void ResetCache();
	}
	public interface ISupportAssign<T> {
		void Assign(T source);
		bool IsDifferFrom(T source);
	}
}
