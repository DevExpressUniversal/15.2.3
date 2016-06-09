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
	public interface ISourceFileCollection : ICollection
	{
		int IndexOf(ISourceFile f);
	int IndexOf(string fileName);
		ISourceFile this[int index] { get; }
	}
	[Serializable]
	public class LiteSourceFileCollection : NodeList, ISourceFileCollection, ISerializableCollection
	{
		public LiteSourceFileCollection() : this(1)
		{ 
		}
		public LiteSourceFileCollection(int capacity)
			: base(capacity)
		{
		}
		protected override NodeList CreateInstance()
		{
			return new LiteSourceFileCollection();
		}
	public CollectionType CollectionType
	{
	  get
	  {
		return CollectionType.SourceFileCollection;
	  }
	}
	int ISourceFileCollection.IndexOf(ISourceFile f)
		{
			return IndexOf(f);
		}
	int ISourceFileCollection.IndexOf(string fileName)
	{
	  if (String.IsNullOrEmpty(fileName))
		return -1;
	  for (int i = 0; i < Count; i++)
	  {
		ISourceFile sourceFile = this[i] as ISourceFile;
		if (sourceFile == null)
		  return -1;
		if (String.Compare(sourceFile.Name, fileName, StringComparison.OrdinalIgnoreCase) == 0)
		  return i;
	  }
	  return -1;
	}
		ISourceFile ISourceFileCollection.this[int index]
		{
			get
			{
				return this[index] as ISourceFile;
			}
		}
	}
}
