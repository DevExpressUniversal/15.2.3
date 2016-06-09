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
using System.Collections;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class MacroInfoCollection : DictionaryBase
	{
		Hashtable _FullMacroRanges = new Hashtable();
		bool _SyncMacroRanges = true;
		void UpdateFullMacroRanges()
		{
			_SyncMacroRanges = false;
			_FullMacroRanges.Clear();
			foreach (object value in this.Values)
			{
				MacroInfo info = value as MacroInfo;
				if (info == null)
					continue;
				SourceRange fullMacroRange = info.MacroFullRange;
				if (fullMacroRange.IsEmpty || _FullMacroRanges.ContainsKey(fullMacroRange))
					continue;
				_FullMacroRanges.Add(fullMacroRange, fullMacroRange);
			}
		}
		protected override void OnClear()
		{
			base.OnClear();
			_SyncMacroRanges = true;
		}
		protected override void OnInsert(object key, object value)
		{
			base.OnInsert(key, value);
			_SyncMacroRanges = true;
		}
		protected override void OnRemove(object key, object value)
		{
			base.OnRemove(key, value);
			_SyncMacroRanges = true;
		}
		public void Add(SourceRange keyRange, MacroInfo info)
		{
			InnerHashtable.Add(keyRange, info);			
		}
		public void AddRange(IDictionary range)
		{
			IDictionaryEnumerator enumer = range.GetEnumerator();
			while (enumer.MoveNext())
				InnerHashtable.Add(enumer.Key, enumer.Value);
		}
		public bool ContainsKey(Object key)
		{
			return InnerHashtable.ContainsKey(key);
		}		
		public void Remove(SourceRange keyRange)
		{
			InnerHashtable.Remove(keyRange);
		}
		public string GetExpansion(SourceRange keyRange)
		{
				MacroInfo macroInfo = this[keyRange];
				if (macroInfo == null)
					return null;
				return macroInfo.MacroExpansion;			
		}
		public ICollection Keys
		{
			get
			{
				return InnerHashtable.Keys;
			}
		}
		public ICollection Values
		{
			get
			{
				return InnerHashtable.Values;
			}
		}
		public MacroInfo this[SourceRange keyRange]
		{
			get
			{
				if (!InnerHashtable.Contains(keyRange))
					return null;
				return InnerHashtable[keyRange] as MacroInfo;
			}
			set
			{
				InnerHashtable[keyRange] = value;
			}
		}
		public bool ContainsFullRange(SourceRange range)
		{
			return !range.IsEmpty && FullMacroRanges.Contains(range);
		}
		public Hashtable FullMacroRanges
		{
			get
			{
				if (_SyncMacroRanges)
					UpdateFullMacroRanges();
				return _FullMacroRanges;
			}
		}
	}
}
