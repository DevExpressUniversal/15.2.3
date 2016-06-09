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
namespace DevExpress.XtraBars.Docking2010.Base {
	public interface ISupportVisitor<T>
	where T : class {
		void Accept(IVisitor<T> visitor);
		void Accept(VisitDelegate<T> visit);
	}
	public delegate void VisitDelegate<T>(T element) where T : class;
	public interface IVisitor<T>
		where T : class {
		void Visit(T element);
	}
	public interface ISupportHierarchy<T> : ISupportVisitor<T>
	where T : class {
		T Parent { get; }
		T[] Nodes { get; }
	}
	public interface IReadOnlyCollection<T> : IEnumerable<T>, ISupportVisitor<T>
	where T : class {
		int Count { get; }
		bool Contains(T element);
		void CopyTo(T[] array, int index);
	}
	public interface IBaseCollection<T> : IReadOnlyCollection<T>, ISupportNotification<T>
		where T : class {
		bool Add(T element);
		void AddRange(T[] elements);
		bool Remove(T element);
		void Clear();
	}
	public enum CollectionChangedType {
		ElementAdded,
		ElementRemoved,
		ElementUpdated,
		ElementDisposed
	}
	public interface ISupportNotification<T>
		where T : class {
		event CollectionChangedHandler<T> CollectionChanged;
	}
	public delegate void CollectionChangedHandler<T>(
		CollectionChangedEventArgs<T> ea) where T : class;
	public class CollectionChangedEventArgs<T> : EventArgs where T : class {
		public CollectionChangedEventArgs(T element, CollectionChangedType changedType, bool clear)
			: base() {
			Element = element;
			ChangedType = changedType;
			Clear = clear;
		}
		public CollectionChangedEventArgs(T element, CollectionChangedType changedType)
			: this(element, changedType, false) {
		}
		public T Element { get; private set; }
		public CollectionChangedType ChangedType { get; private set; }
		public bool Clear { get; private set; }
	}
}
