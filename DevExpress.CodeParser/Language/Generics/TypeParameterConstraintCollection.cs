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
	public class TypeParameterConstraintCollection : LanguageElementCollectionBase, ITypeParameterConstraintCollection
	{
		public int Add(TypeParameterConstraint value)
		{
			return base.Add(value);
		}
		public void Add(TypeParameterConstraintCollection collection)
		{
			AddRange(collection);
		}
		public void AddRange(TypeParameterConstraintCollection collection)
		{
			base.AddRange(collection);
		}
		public bool Contains(TypeParameterConstraint value)
		{
			return base.Contains(value);
		}
		public void CopyTo(TypeParameterConstraint[] array, int index)
		{
			base.CopyTo(array, index);
		}
		public int IndexOf(TypeParameterConstraint value)
		{
			return base.IndexOf(value);
		}
		public void Insert(int index, TypeParameterConstraint value)
		{
			base.Insert(index, value);
		}
		public void Remove(TypeParameterConstraint value)
		{
			base.Remove(value);
		}
		public new TypeParameterConstraint this[int index] 
		{
			get
			{
				return (TypeParameterConstraint) base[index];
			}
			set
			{
				base[index] = value;
			}
		}		
		#region ITypeParameterConstraintCollection Members
		int ITypeParameterConstraintCollection.IndexOf(ITypeParameterConstraint t)
		{
			return IndexOf(t);
		}
		ITypeParameterConstraint ITypeParameterConstraintCollection.this[int index]
		{
			get
			{
				return this[index] as ITypeParameterConstraint;
			}
		}
		#endregion
	}
}
