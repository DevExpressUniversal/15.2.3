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
	public class TokenCollection : CollectionBase
#if !DXCORE
		, ITokenCollection
#endif
 {
		#region Add
		public int Add(Token token)
		{
			return List.Add(token);
		}
		#endregion
		#region IndexOf
		public int IndexOf(Token token)
		{
			return List.IndexOf(token);
		}
		#endregion
		#region Insert
		public void Insert(int index, Token token)
		{
			List.Insert(index, token);
		}
		#endregion
		#region Remove
		public void Remove(Token token)
		{
			List.Remove(token);
		}
		#endregion
		#region Contains
		public bool Contains(Token token)
		{
			return List.Contains(token);
		}
		#endregion
		#region this[int index]
		public Token this[int index]
		{
			get
			{
				return (Token)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion
#if !DXCORE
		#region ITokenCollection Members
		IToken ITokenCollection.this[int index] {
			get { return this[index]; }
		}
		#endregion
#endif
	}
}
