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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface ICaseStatementCollection : ICollection
	{
		int IndexOf(ICaseStatement t);
		ICaseStatement Find(string name);
		ICaseStatement this[int index] { get; }
	}
	[Serializable]
	public class LiteCaseStatementCollection : NodeList, ICaseStatementCollection, ISerializableCollection
	{
		public LiteCaseStatementCollection() : this(1)
		{ 
		}
		public LiteCaseStatementCollection(int capacity)
			: base(capacity)
		{
		}
		protected override NodeList CreateInstance()
		{
			return new LiteCaseStatementCollection();
		}
	public CollectionType CollectionType
	{
	  get
	  {
		return CollectionType.CaseStatementCollection;
	  }
	}
		int ICaseStatementCollection.IndexOf(ICaseStatement st)
		{
			return IndexOf(st);
		}
		ICaseStatement ICaseStatementCollection.Find(string name)
		{
			int lCount = Count;
			for (int i = 0; i < lCount; i++)
			{
				ICaseStatement st = this[i] as ICaseStatement;
				if (st != null && st.Name == name)
					return st;
			}
			return null;
		}
		ICaseStatement ICaseStatementCollection.this[int index]
		{
			get
			{				
				return this[index] as ICaseStatement;
			}
		}
	}
}
