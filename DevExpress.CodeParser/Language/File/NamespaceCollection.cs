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
	public class NamespaceCollection: LanguageElementCollectionBase
	{
		#region private fields...
		private string _FullName;
		#endregion
		#region NamespaceCollection
		public NamespaceCollection(string fullName)
		{
		_FullName = fullName;
		}
		#endregion
		#region ClearMemberReferencesToThis
		private void ClearMemberReferencesToThis() 
		{
			foreach(Namespace lNamespace in this)
				lNamespace.SetNamespaceCollection(null);
		}
		#endregion
		#region OnClear
		protected override void OnClear()
		{
			ClearMemberReferencesToThis();
			base.OnClear();
		}
		#endregion		
		protected new NamespaceCollection CreateInstance()
		{
			return new NamespaceCollection(_FullName);
		}		
		protected void CloneElements(NamespaceCollection target, ElementCloneOptions options)
		{
			if (target == null)
				return;
			for (int i = 0; i < Count; i++)
			{
				Namespace lClone = this[i].Clone(options) as Namespace;
				target.Add(lClone);				
			}
		}
		#region Add
		public int Add(Namespace aNamespace)
		{
			if (aNamespace == null)
				return -1;
			if (aNamespace.FullName != _FullName)
				throw new Exception(String.Format("Namespace name, \"{0}\", does not match name assigned to this collection (\"{1}\"). Unable to add namespace.", aNamespace.FullName, _FullName));
			aNamespace.SetNamespaceCollection(this);
			return base.Add(aNamespace);
		}
		#endregion
		#region Remove
		public void Remove(Namespace aNamespace)
		{
			if (aNamespace != null)
				aNamespace.SetNamespaceCollection(null);
			int lIndex = IndexOf(aNamespace);
			if (lIndex < 0)
			{
				return;
			}
			base.RemoveAt(lIndex);
		}
		#endregion
		#region IndexOf
		public int IndexOf(Namespace aNamespace)
		{
			for(int i = 0; i < this.Count; i++)
				if (this[i] == aNamespace)	
					return i;
			return -1;
		}
		#endregion
		#region Insert
		public void Insert(int index, Namespace aNamespace)
		{
			base.Insert(index, aNamespace);
		}
		#endregion
		#region Find
		public Namespace Find(Namespace aNamespace)
		{
			foreach(Namespace lNamespace in this)
				if (lNamespace == aNamespace)	
					return lNamespace;
			return null;	
		}
		#endregion
		#region Contains
		public bool Contains(Namespace aNamespace)
		{
			return (Find(aNamespace) != null);
		}
		#endregion
		public new NamespaceCollection DeepClone()
		{
			return DeepClone (ElementCloneOptions.Default);
		}
		public new NamespaceCollection DeepClone(ElementCloneOptions options)
		{
			NamespaceCollection lClonedCollection = CreateInstance();
			CloneElements(lClonedCollection, options);
			return lClonedCollection;
		}		
		#region this[int aIndex]
		public new Namespace this[int index] 
		{
			get
			{
				return (Namespace) base[index];
			}
			set
			{
				base[index] = value;
			}
		}
		#endregion
		#region FullName
		public string FullName
		{
			get
			{
				return _FullName;
			}
		}
		#endregion
	}
}
