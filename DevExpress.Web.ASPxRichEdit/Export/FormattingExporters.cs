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

using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public abstract class FormattingExporterBase<T> {
		readonly Dictionary<int, Hashtable> innerCache;
		protected FormattingExporterBase() {
			innerCache = new Dictionary<int, Hashtable>();
		}
		public void RegisterItem(int key, T value, params DictionaryEntry[] parameters) {
			if (this.innerCache.ContainsKey(key))
				return;
			var item = ConvertToHashtable(key, value);
			foreach(var parameter in parameters)
				item.Add(parameter.Key, parameter.Value);
			this.innerCache.Add(key, item);
		}
		public Dictionary<int, Hashtable> Cache { get { return innerCache; } }
		public Hashtable ConvertToHashtable(int key, T value) {
			Hashtable result = new Hashtable();
			FillHashtable(result, value);
			return result;
		}
		public Hashtable GetHashtable(T info) {
			var ht = new Hashtable();
			FillHashtable(ht, info);
			return ht;
		}
		public abstract void FillHashtable(Hashtable result, T info);
		public abstract void RestoreInfo(Hashtable source, T info);
	}
}
