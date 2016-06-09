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
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class HybridObjectStorage
	{
	const int INT_MaxItemsCount = 10000;
		Hashtable _CaseSensitiveHash;
		Hashtable _CaseInsensitiveHash;
	bool _LimitSize;
		public HybridObjectStorage()
		{
		}
	void AddItemInternal(Hashtable table, object key, object value)
	{
	  if (table == null)
		return;
	  if (_LimitSize && table.Count > INT_MaxItemsCount)
		table.Clear();
	  table.Add(key, value);
	}
	public IEnumerable<object> GetValues()
	{
	  ICollection values = CaseSensitiveHash.Values;
	  foreach (object value in values)
		yield return value;
	}
		public virtual void Add(object key, object value)
		{
	  AddItemInternal(CaseSensitiveHash, key, value);	
			object lObj = CaseInsensitiveHash[key];
			if (lObj == null)
		AddItemInternal(CaseInsensitiveHash, key, value);
		}
	public virtual void Remove(object key)
		{
			CaseSensitiveHash.Remove(key);
			CaseInsensitiveHash.Remove(key);
		}
	public virtual void Clear()
		{
			CaseSensitiveHash.Clear();
			CaseInsensitiveHash.Clear();
		}
		public object Get(object key)
		{
			return Get(key, false);
		}
		public object Get(object key, bool ignoreCase)
		{
			if (ignoreCase)
				return CaseInsensitiveHash[key];
			else
				return CaseSensitiveHash[key];
		}
	public virtual bool Contains(object key)
		{
			return Contains(key, false);
		}
		public bool Contains(object key, bool ignoreCase)
		{
			if (ignoreCase)
				return CaseInsensitiveHash.ContainsKey(key);
			else
				return CaseSensitiveHash.ContainsKey(key);
		}
	public virtual bool ContainsKey(object key)
	{
	  return Contains(key);
	}
	public virtual bool ContainsValue(object value)
	{
	  return CaseSensitiveHash.ContainsValue(value);
	}
	public virtual IDictionaryEnumerator GetEnumerator()
	{
	  return CaseSensitiveHash.GetEnumerator();
	}
	public string[] GetAllNames(bool caseSensitive)
	{
	  ICollection coll = null;
	  if (caseSensitive)
		coll = CaseSensitiveHash.Keys;
	  else
		coll = CaseInsensitiveHash.Keys;
	  if (coll == null)
		return null;
	  int count = coll.Count;
	  string[] result = new string[count];
	  int i = 0;
	  IEnumerator enumer = coll.GetEnumerator();
	  while (enumer.MoveNext())
	  {
		string str = enumer.Current as string;
		if (str == null)
		  continue;
		result[i] = str;
		i++;
	  }
	  return result;
	}
	public Hashtable GetCaseSensitiveHash()
	{
	  return CaseSensitiveHash;
	}
	public Hashtable GetCaseInsensitiveHash()
	{
	  return CaseInsensitiveHash;
	}
	public Hashtable GetHashTable(bool isCaseSensitive)
	{
	  if (isCaseSensitive)
		return GetCaseSensitiveHash();
	  else
		return GetCaseInsensitiveHash();
	}
		Hashtable CaseSensitiveHash
		{
			get
			{
				if (_CaseSensitiveHash == null)
					_CaseSensitiveHash = new Hashtable();
				return _CaseSensitiveHash;
			}
		}
		Hashtable CaseInsensitiveHash
		{
			get
			{
				if (_CaseInsensitiveHash == null)
					_CaseInsensitiveHash = HashtableUtils.CreateHashtable(false);
				return _CaseInsensitiveHash;
			}
		}
	internal bool LimitSize
	{
	  get
	  {
		return _LimitSize;
	  }
	  set
	  {
		_LimitSize = value;
	  }
	}
	public virtual ICollection Keys
	{
	  get
	  {
		return CaseSensitiveHash.Keys;
	  }
	}
	public virtual ICollection Values
	{
	  get
	  {
		return CaseSensitiveHash.Values;
	  }
	}
	public virtual int Count
		{
			get
			{
				return CaseSensitiveHash.Count;
			}
		}
	public virtual object this[object key]
		{
			get
			{
				return Get(key);
			}
	  set
	  {
		CaseSensitiveHash[key] = value;
		CaseInsensitiveHash[key] = value;
	  }
		}
	}
}
