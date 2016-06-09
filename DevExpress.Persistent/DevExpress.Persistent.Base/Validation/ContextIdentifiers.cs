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
using DevExpress.Persistent.Base;
using System.Collections;
namespace DevExpress.Persistent.Validation {
	[Flags]
	public enum DefaultContexts { Save = 1, Delete = 2 }
	public struct ContextIdentifiers : IList<string> {
		public const string Separator = ";";
		private static readonly string[] separatorArray = new string[] { Separator };
		private string _identifiers;
		private List<string> _idList;
		private List<string> idList {
			get {
				if(_idList == null) {
					_idList = new List<string>();
				}
				return _idList;
			}
		}
		private string identifiers {
			get {
				if(_identifiers == null) {
					_identifiers = "";
				}
				return _identifiers;
			}
			set {
				_identifiers = value;
			}
		}
		public ContextIdentifiers(string identifiers)
			: this(
			identifiers == null ? new string[0] : identifiers.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries)) {
		}
		public ContextIdentifiers(IList<string> identifiers) {
			_idList = new List<string>();
			_idList.Capacity = identifiers.Count;
			for(int i = 0; i < identifiers.Count; i++) {
				_idList.Add(identifiers[i].Trim());
			}
			_identifiers = string.Join(Separator, _idList.ToArray());
		}
		public override string ToString() {
			return identifiers;
		}
		public override bool Equals(object obj) {
			if(obj is ContextIdentifiers) {
				return (ContextIdentifiers)obj == this;
			}
			return false;
		}
		public override int GetHashCode() {
			return identifiers.GetHashCode();
		}
		public static implicit operator ContextIdentifiers(string s) {
			return new ContextIdentifiers(s);
		}
		public static implicit operator ContextIdentifiers(DefaultContexts arg) {
			List<string> contexts = new List<string>();
			foreach(DefaultContexts defaultContext in Enum.GetValues(typeof(DefaultContexts))) {
				if((defaultContext & arg) == defaultContext) {
					contexts.Add(defaultContext.ToString());
				}
			}
			return new ContextIdentifiers(contexts);
		}
		public static implicit operator ContextIdentifiers(ContextIdentifier contextIdentifier) {
			return new ContextIdentifiers(contextIdentifier.Id);
		}
		public static bool operator ==(ContextIdentifiers c1, ContextIdentifiers c2) {
			return c1.identifiers == c2.identifiers;
		}
		public static bool operator !=(ContextIdentifiers c1, ContextIdentifiers c2) {
			return c1.identifiers != c2.identifiers;
		}
		private static ContextIdentifiers MakeUnion(ContextIdentifiers c1, ContextIdentifiers c2) {
			return new ContextIdentifiers(CollectionsHelper.MergeCollections<string>(c1.idList, c2.idList));
		}
		public static ContextIdentifiers operator +(ContextIdentifiers c1, ContextIdentifiers c2) {
			return MakeUnion(c1, c2);
		}
		public static ContextIdentifiers operator |(ContextIdentifiers c1, ContextIdentifiers c2) {
			return MakeUnion(c1, c2);
		}
		#region IList<string> Members
		int IList<string>.IndexOf(string item) {
			return idList.IndexOf(item);
		}
		string IList<string>.this[int index] {
			get { return idList[index]; }
			set { throw new Exception("The method or operation is not implemented."); }
		}
		void IList<string>.Insert(int index, string item) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList<string>.RemoveAt(int index) {
			throw new Exception("The method or operation is not implemented.");
		}
		void ICollection<string>.Add(string item) {
			throw new Exception("The method or operation is not implemented.");
		}
		void ICollection<string>.Clear() {
			throw new Exception("The method or operation is not implemented.");
		}
		bool ICollection<string>.Remove(string item) {
			throw new Exception("The method or operation is not implemented.");
		}
		void ICollection<string>.CopyTo(string[] array, int arrayIndex) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool ICollection<string>.IsReadOnly {
			get { return true; }
		}
		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			return idList.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return idList.GetEnumerator();
		}
		public bool Contains(string item) {
			return idList.Contains(item);
		}
		public string this[int index] {
			get { return idList[index]; }
		}
		public int Count {
			get { return idList.Count; }
		}
		#endregion
	}
	public struct ContextIdentifier : IComparable {
		public static readonly ContextIdentifiers Save = (ContextIdentifier)DefaultContexts.Save;
		public static readonly ContextIdentifiers Delete = (ContextIdentifier)DefaultContexts.Delete;
		private string id;
		public ContextIdentifier(string id) {
			this.id = id;
		}
		public string Id {
			get { return id; }
		}
		public override bool Equals(object obj) {
			if(obj is ContextIdentifier) {
				return ((ContextIdentifier)obj).Id == this.Id;
			}
			return false;
		}
		public override int GetHashCode() {
			return Id.GetHashCode();
		}
		public override string ToString() {
			return Id;
		}
		public static implicit operator ContextIdentifier(string s) {
			return (ContextIdentifiers)s;
		}
		public static implicit operator ContextIdentifier(DefaultContexts arg) {
			return (ContextIdentifiers)arg;
		}
		public static implicit operator ContextIdentifier(ContextIdentifiers contextIdentifiers) {
			if(contextIdentifiers.Count != 1) {
				throw new ArgumentException(string.Format("Incorrect context: '{0}'", contextIdentifiers));
			}
			return new ContextIdentifier(((IList<string>)contextIdentifiers)[0]);
		}
		public static bool operator ==(ContextIdentifier c1, ContextIdentifier c2) {
			return c1.Id == c2.Id;
		}
		public static bool operator !=(ContextIdentifier c1, ContextIdentifier c2) {
			return c1.Id != c2.Id;
		}
		public int CompareTo(object obj) {
			if(obj == null) {
				return 1;
			}
			if(!(obj is ContextIdentifier)) {
				throw new ArgumentException(obj.GetType().FullName);
			}
			return Comparer.Default.Compare(Id, ((ContextIdentifier)obj).Id);
		}
	}
}
