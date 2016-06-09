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
using System.Text;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class LanguageElementCollectionBase : NodeList, IEnumerable<LanguageElement>
	{
	IEnumerator<LanguageElement> IEnumerable<LanguageElement>.GetEnumerator()
	{
	  IEnumerator enumerator = base.GetEnumerator();
	  while (enumerator.MoveNext())
	  {
		LanguageElement element = enumerator.Current as LanguageElement;
		if (element != null)
		  yield return element;
	  }
	}
		protected int Add(LanguageElement element)
		{
			return base.Add(element);
		}
		protected void AddRange(LanguageElementCollectionBase collection)
		{
			base.AddRange(collection);
		}
		protected int IndexOf(LanguageElement element)
		{
			return base.IndexOf(element);
		}
		protected void Insert(int index, LanguageElement element)
		{
			base.Insert(index, element);
		}
		protected void Remove(LanguageElement element)
		{
			base.Remove(element);
		}
		protected LanguageElement Find(LanguageElement element)
		{
			int lIndex = IndexOf(element);
			return lIndex < 0 ? null : this[lIndex];
		}
		protected bool Contains(LanguageElement element)
		{
			return (Find(element) != null);
		}
		protected override NodeList CreateInstance()
		{
			return new LanguageElementCollectionBase();			
		}
		public override string ToString()
		{
			const string STR_Comma = ", ";
			StringBuilder lResult = new StringBuilder(String.Empty);
			string lComma = String.Empty;
			for (int i = 0; i < this.Count; i++)
			{
				lResult.AppendFormat("{0}{1}", lComma, this[i]);
				lComma = STR_Comma;
			}
			return lResult.ToString();
		}
		public SourceRange GetRange()
		{
			SourceRange range = SourceRange.Empty;
			int count = Count;
			for (int i = 0; i < count; i++)
				range = SourceRange.Union(range, this[i].Range);
			return range;
		}
		protected new LanguageElement this[int index]
		{
			get
			{
				return (LanguageElement) base[index];
			}
			set
			{
				base[index] = value;
			}
		}
  }
}
