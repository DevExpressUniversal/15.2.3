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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class DefineBaseCollection :NodeList
	{
		protected int Add(DefineBase element)
		{
			return base.Add(element);
		}
		protected void AddRange(DefineBaseCollection collection)
		{
			base.AddRange(collection);
		}
		protected int IndexOf(DefineBase element)
		{
			return base.IndexOf(element);
		}
		protected void Insert(int index, DefineBase element)
		{
			base.Insert(index, element);
		}
		protected void Remove(DefineBase element)
		{
			base.Remove(element);
		}
		protected DefineBase Find(DefineBase element)
		{
			int lIndex = IndexOf(element);
			return lIndex < 0 ? null : this[lIndex];
		}
		protected bool Contains(DefineBase element)
		{
			return (Find(element) != null);
		}
		protected override NodeList CreateInstance()
		{
			return new DefineBaseCollection();			
		}
		public new DefineBase this[int index]
		{
			get
			{
				return (DefineBase) base[index];
			}
			set
			{
				base[index] = value;
			}
		}		
	}
}
