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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public sealed class GlobalStringStorage
	{
		static object _InstanceLock = new object();
		static GlobalStringStorage _Instance;		
		Dictionary<string, string> _Strings = new Dictionary<string, string>();
	int _UpdatesCount = 0;
		public GlobalStringStorage()
		{
		}
	bool BeginUpdateInternal()
	{
	  lock (this)
	  {
		_UpdatesCount++;
		return (_UpdatesCount == 1);
	  }
	}
	bool EndUpdateInternal()
	{
	  lock (this)
	  {
		_UpdatesCount--;
		if (_UpdatesCount == 0)
		{
		  ClearStrings();
		  return true;
		}
		return false;
	  }
	}
	bool Updating
	{
	  get
	  {
		return _UpdatesCount > 0;
	  }
	}
		public string InternString(string s)
		{
			if (s == null)
				return null;
			lock (this)
			{
				string res;
				if (_Strings.TryGetValue(s, out res))
					return res;
				_Strings.Add(s, s);
				return s;
			}
		}
		public bool IsInternedString(string s)
		{
			if (s == null)
				return false;
			lock (this)
			{
				return _Strings.ContainsKey(s);
			}
		}
		public void ClearStrings()
		{
			lock (this)
			{
		if (Updating)
		  return;
				_Strings = new Dictionary<string, string>();
			}
		}	
		public static string Intern(string s)
		{
	  if (s == null)
		return null;
			return Instance.InternString(s);
		}
		public static bool IsInterned(string s)
		{
	  if (s == null)
		return false;
			return Instance.IsInternedString(s);
		}
		public static void Clear()
		{
	  Instance.ClearStrings();
		}
	public static void BeginUpdate()
	{
	  Instance.BeginUpdateInternal();
	}
	public static void EndUpdate()
	{
	  Instance.EndUpdateInternal();
	}
		public static GlobalStringStorage Instance
		{
			get
			{
				if (_Instance == null)
				{
					lock (_InstanceLock)
					{
						if (_Instance == null)
							_Instance = new GlobalStringStorage();
						return _Instance;
					}
				}
				return _Instance;
			}
		}
	}
}
