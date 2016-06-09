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
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
namespace DevExpress.ExpressApp.Web.Core {
	#region Obsolete 15.1
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DictionaryOnSession : IDictionary<string, object> {
		private HttpSessionState GetSession() {
				if(HttpContext.Current == null) {
					throw new InvalidOperationException("HttpContext.Current is null");
				}
				if(HttpContext.Current.Session == null) {
					throw new InvalidOperationException("HttpContext.Current.Session is null");
				}
			return HttpContext.Current.Session;
		}
		public void Add(string key, object value) {
			GetSession().Add(key, value);
		}
		public bool ContainsKey(string key) {
			throw new Exception("The method or operation is not implemented.");
		}
		public ICollection<string> Keys {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public bool Remove(string key) {
			GetSession().Remove(key);
			return true;
		}
		public bool TryGetValue(string key, out object value) {
			value = GetSession()[key];
			return true;
		}
		public ICollection<object> Values {
			get { throw new Exception("The method or operation is not implemented."); }
		}
		public object this[string key] {
			get { return GetSession()[key]; }
			set { GetSession()[key] = value; }
		}
		public void Add(KeyValuePair<string, object> item) {
			Add(item.Key, item.Value);
		}
		public void Clear() {
			throw new Exception("The method or operation is not implemented.");
		}
		public bool Contains(KeyValuePair<string, object> item) {
			throw new Exception("The method or operation is not implemented.");
		}
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
			throw new Exception("The method or operation is not implemented.");
		}
		public int Count {
			get { return GetSession().Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(KeyValuePair<string, object> item) {
			return Remove(item.Key);
		}
		public System.Collections.IEnumerator GetEnumerator() {
			return GetSession().GetEnumerator();
		}
		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
			throw new Exception("The method or operation is not implemented.");
		}
	}
	#endregion
}
