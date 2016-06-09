#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardCommon.DataProcessing {
	public interface IPool<T> {
		bool IsInPool(T poolObject);
		T GetAvailableObject();
		void ReturnObjectToPool(T poolObject);
	}
	public abstract class Pool<T> : IPool<T>, IDisposable where T : class {
		Dictionary<T, bool> objectsList = new Dictionary<T, bool>();
		bool disposed = false;
		T FindAvailableObject() {
			foreach (KeyValuePair<T, bool> pair in objectsList)
				if (pair.Value)
					return pair.Key;
			return null;
		}
		public abstract T CreateInstance();
		public T GetAvailableObject() {
			T item = FindAvailableObject();
			if (item == null) {
				item = CreateInstance();
				objectsList.Add(item, false);
			}
			else
				objectsList[item] = false;
			return item;
		}
		public void ReturnObjectToPool(T poolObject) {
			objectsList[poolObject] = true;
		}
		public bool IsInPool(T poolObject) {
			return objectsList[poolObject];
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposed)
				return;
			if (disposing) {
			}
			disposed = true;
		}
		~Pool() {
			Dispose(false);
		}
	}
	public class CircularBufferPool<T> : Pool<CircularBuffer<T>> {
		int length = 100;
		public override CircularBuffer<T> CreateInstance() {
			return new CircularBuffer<T>(length);
		}
	}
	public class ByteBufferPool : Pool<ByteBuffer> {
		int length = 100;
		public ByteBufferPool(int length) {
			this.length = length;
		}
		public override ByteBuffer CreateInstance() {
			return new ByteBuffer(length);
		}
	}
}
