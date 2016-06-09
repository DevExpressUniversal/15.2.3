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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	public class CommentAuthorCollectionOld : SimpleUniqueItemCollection<string>, ISupportsCopyFrom<CommentAuthorCollectionOld> {
		public void CopyFrom(CommentAuthorCollectionOld source) {
			this.Clear();
			this.InnerList.AddRange(source.InnerList);
		}
	}
	public sealed class CommentAuthorCollection {
		CommentAuthorCollectionOld inner = new CommentAuthorCollectionOld();
		int Add( string author) {
			int elementIndex = Count;
			inner.Add(author);
			return elementIndex;
		}
		public int AddIfNotPresent(string author) {
			if (Contains(author))
				return inner.IndexOf(author);
			return Add(author);
		}
		public string this[int index] {
			get {
				return inner[index];
			}
		}
		public int GetAuthorID(string name) {
			return inner.IndexOf(name);
		}
		bool Contains(string name) {
			return inner.Contains(name);
		}
		public int Count { get { return inner.Count; } }
		public void ForEach(Action<String> action) {
			inner.ForEach(action);
		}
		public void CopyFrom(CommentAuthorCollection source) {
			inner.Clear();
			inner.InnerList.AddRange(source.inner.InnerList);
		}
		public void Clear() {
			inner.Clear();
		}
	}
}
