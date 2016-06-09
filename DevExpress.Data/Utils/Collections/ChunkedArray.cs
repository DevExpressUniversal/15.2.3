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
namespace DevExpress.Utils {
	#region ChunkedArray
	public class ChunkedArray<T> {
		readonly int chunkSize;
		readonly List<List<T>> chunks;
		int totalCount = 0;
		public ChunkedArray()
			: this(8192, 0) {
		}
		public ChunkedArray(int chunkSize)
			: this(chunkSize, 0) {
		}
		public ChunkedArray(int chunkSize, int capacity) {
			Guard.ArgumentPositive(chunkSize, "chunkSize");
			this.chunkSize = chunkSize;
			if(capacity > 0) {
				int chunksCapacity = capacity / chunkSize;
				if(capacity % chunkSize > 0)
					chunksCapacity++;
				this.chunks = new List<List<T>>(chunksCapacity);
			}
			else
				this.chunks = new List<List<T>>();
			AddChunk();
		}
		List<T> LastChunk { get { return chunks[chunks.Count - 1]; } }
		public int Count { get { return totalCount; } }
		public T this[int index] {
			get {
				List<T> chunk = chunks[index / chunkSize];
				return chunk[index % chunkSize];
			}
			set {
				List<T> chunk = chunks[index / chunkSize];
				chunk[index % chunkSize] = value;
			}
		}
		public void Add(T item) {
			if(LastChunk.Count == chunkSize)
				AddChunk();
			LastChunk.Add(item);
			this.totalCount++;
		}
		public void Clear() {
			this.totalCount = 0;
			this.chunks.Clear();
			AddChunk();
		}
		void AddChunk() {
			this.chunks.Add(new List<T>(chunkSize));
		}
	}
	#endregion
}
