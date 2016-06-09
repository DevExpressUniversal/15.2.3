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
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class SourceLineCollection : CollectionBase
	{
		#region Add
		public int Add(SourceLine aSourceLine)
		{
			return List.Add(aSourceLine);
		}
		#endregion
		#region IndexOf
		public int IndexOf(SourceLine aSourceLine)
		{
			for(int i = 0; i < List.Count; i++)
				if (this[i] == aSourceLine)	
					return i;
			return -1;
		}
		#endregion
		#region Insert
		public void Insert(int index, SourceLine aSourceLine)
		{
			List.Insert(index, aSourceLine);
		}
		#endregion
		#region Remove
		public void Remove(SourceLine aSourceLine)
		{
			List.Remove(aSourceLine);
		}
		#endregion
		#region Find
		public SourceLine Find(SourceLine aSourceLine)
		{
			foreach(SourceLine lSourceLine in this)
				if (lSourceLine == aSourceLine)	
					return lSourceLine;
			return null;	
		}
		#endregion
		#region Contains
		public bool Contains(SourceLine aSourceLine)
		{
			return (Find(aSourceLine) != null);
		}
		#endregion
		#region this[int aIndex]
		public SourceLine this[int index] 
		{
			get
			{
				return (SourceLine) List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion
	}
}
