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
	public class CommentCollection : LanguageElementCollection
	{
		protected override NodeList CreateInstance()
		{
			return new CommentCollection();
		}
		#region Add
		public int Add(Comment aComment)
		{
			return base.Add(aComment);
		}
		#endregion
		#region AddRange
		public void AddRange(CommentCollection collection)
		{
			base.AddRange(collection);
		}
		#endregion
		#region IndexOf
		public int IndexOf(Comment aComment)
		{
			return base.IndexOf(aComment);
		}
		#endregion
		#region Insert
		public void Insert(int index, Comment aComment)
		{
			base.Insert(index, aComment);
		}
		#endregion
		#region Remove
		public void Remove(Comment aComment)
		{
			base.Remove(aComment);
		}
		#endregion
		#region Find
		public Comment Find(Comment aComment)
		{
			return (Comment) base.Find(aComment);
		}
		#endregion
		#region FindFirst
		public Comment FindFirst(string name)
		{
			for (int i=0; i < Count; i++)
			{
				if (this[i].Name == name)
					return this[i];
			}
			return null;
		}
		#endregion
		#region FindLast
		public Comment FindLast(string name)
		{
			for (int i = Count-1; i >= 0; i--)
			{
				if (this[i].Name == name)
					return this[i];
			}
			return null;
		}
		#endregion
		#region Contains
		public bool Contains(Comment aComment)
		{
			return base.Contains(aComment);
		}
		#endregion		
		#region this[int aIndex]
		public new Comment this[int index] 
		{
			get
			{
				return (Comment) base[index];
			}
			set
			{
				base[index] = value;
			}
		}
		#endregion		
		public Comment LastComment
		{
			get
			{
				if (Count == 0)
					return null;
				return this[Count - 1];
			}
		}
	}
}
