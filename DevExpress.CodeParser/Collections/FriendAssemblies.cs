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
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class FriendAssemblies
	{
		const string STR_NameSeparator = ",";
		ArrayList _FriendAssebliesNames;
		IElementCollection _UnresolvedExpressions;
		#region FriendAssemblies
		public FriendAssemblies()
		{
			_FriendAssebliesNames = new ArrayList();
			_UnresolvedExpressions = new IElementCollection();
		}
		#endregion
		#region CorrectAssemblyName
		void CorrectAssemblyName(ref string friendAssembly)
		{
			if (string.IsNullOrEmpty(friendAssembly) || !friendAssembly.Contains(STR_NameSeparator))
				return;
			string[] separatedName = friendAssembly.Split(new string[] { STR_NameSeparator }, StringSplitOptions.None);
			friendAssembly = separatedName[0];
		}
		#endregion
		#region Add
		public void Add(string friendAssembly)
		{
			if (string.IsNullOrEmpty(friendAssembly))
				return;
			CorrectAssemblyName(ref friendAssembly);
			if (_FriendAssebliesNames.Contains(friendAssembly))
				return;
			_FriendAssebliesNames.Add(friendAssembly);
		}
		#endregion
		#region AddUnresolved
		public void AddUnresolved(IExpression friendAssemblyExpression)
		{
			if (_UnresolvedExpressions.Contains(friendAssemblyExpression))
				return;
			_UnresolvedExpressions.Add(friendAssemblyExpression);
		}
		#endregion
		public int Count
		{
			get
			{
				int friendAssembliesNamesCount = _FriendAssebliesNames.Count;
				int unresolvedExpressionsCount = _UnresolvedExpressions.Count;
				return Math.Max(friendAssembliesNamesCount, unresolvedExpressionsCount);
			}
		}
		#region FriendAssembliesNames
		public string[] FriendAssembliesNames
		{
			get
			{
				return (string[])_FriendAssebliesNames.ToArray(typeof(string));
			}
		}
		#endregion
		#region UnresolvedExpressions
		public IElementCollection UnresolvedExpressions
		{
			get { return _UnresolvedExpressions; }
		}
		#endregion
	}
}
