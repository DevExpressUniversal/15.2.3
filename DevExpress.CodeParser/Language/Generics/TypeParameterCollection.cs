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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class TypeParameterCollection : LanguageElementCollectionBase, ITypeParameterCollection
	{
		public int Add(TypeParameter value)
		{
			return base.Add(value);
		}
		public void Add(TypeParameterCollection collection)
		{
			AddRange(collection);
		}
		public void AddRange(TypeParameterCollection collection)
		{
			base.AddRange(collection);
		}
		public bool Contains(TypeParameter value)
		{
			return base.Contains(value);
		}
		public void CopyTo(TypeParameter[] array, int index)
		{
			base.CopyTo(array, index);
		}
		public int IndexOf(TypeParameter value)
		{
			return base.IndexOf(value);
		}
		public void Insert(int index, TypeParameter value)
		{
			base.Insert(index, value);
		}
		public void Remove(TypeParameter value)
		{
			base.Remove(value);
		}
		public TypeParameter Find(string name)
		{
			int lCount = Count;
			for (int i = 0; i < lCount; i++)
			{
				if (this[i].Name == name)
					return this[i];
			}
			return null;
		}
		public override string ToString()
		{
			StringBuilder lBuilder = new StringBuilder();
			for (int i = 0; i < Count; i++)
			{
				lBuilder.Append(this[i].Name);
				if (i < Count - 1)
					lBuilder.Append(", ");
			}
			return lBuilder.ToString();
		}		
		public new TypeParameter this[int index] 
		{
			get
			{
				return (TypeParameter) base[index];
			}
			set
			{
				base[index] = value;
			}
		}		
		#region ITypeParameterCollection Members
		int ITypeParameterCollection.IndexOf(ITypeParameter t)
		{			
			return IndexOf(t);
		}
		ITypeParameter ITypeParameterCollection.Find(string name)
		{
			return Find(name);
		}
		ITypeParameter ITypeParameterCollection.this[int index]
		{
			get
			{				
				return this[index] as ITypeParameter;
			}
		}
		#endregion
	}
}
