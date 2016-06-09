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
	public class TextStringCollection : LanguageElementCollection
  {
		Hashtable _RangesHash;
		public TextStringCollection()
		{
			_RangesHash = new Hashtable();
		}
		protected override NodeList CreateInstance()
		{
			return new TextStringCollection();
		}
  	public void Add(TextString s)
  	{
			if (s == null)
				return;
			if (!_RangesHash.ContainsKey(s.Range))
				_RangesHash.Add(s.Range, s);
  		base.Add(s);
  	}
		public void AddUnique(TextString s)
		{
			if (s == null)
				return;
			if (!_RangesHash.ContainsKey(s.Range))
			{
				_RangesHash.Add(s.Range, s);
				base.Add(s);
			}
		}
  	public void Remove(TextString s)
  	{
			if (s == null)
				return;
			if (_RangesHash.ContainsKey(s.Range))
				_RangesHash.Remove(s.Range);
  		base.Remove(s);
  	}
		public TextString Find(string s)
		{
			foreach(TextString textString in this)
			{
				if (textString.Name == s)	
					return textString;
			}
			return null;	
		}
  	public TextString Find(TextString s)
  	{
  		return (TextString)base.Find(s);
  	}
  	public bool Contains(TextString s)
  	{
  		return base.Contains(s);
  	}
		public new TextString this[int index]
  	{
  		get
  		{
  			return (TextString)base[index];
  		}
  	}
  }
}
