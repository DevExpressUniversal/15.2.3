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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public sealed class VB80KeyWords
	{
		private static StringCollection _KeyWords = null;
		private VB80KeyWords(){}
		static void CreateKeywords()
		{
			lock (typeof(VB80KeyWords))
			{
				_KeyWords = new StringCollection();
				string[] lWords = new string[VB71KeyWords.Collection.Count];
				VB71KeyWords.Collection.CopyTo(lWords, 0);
				_KeyWords.AddRange(lWords);
				_KeyWords.Add("continue");
				_KeyWords.Add("using");
				_KeyWords.Add("trycast");
				_KeyWords.Add("operator");
				_KeyWords.Add("istrue");
				_KeyWords.Add("isfalse");
				_KeyWords.Add("widening");
				_KeyWords.Add("narrowing");
				_KeyWords.Add("of");
				_KeyWords.Add("global");
				_KeyWords.Add("uinteger");
				_KeyWords.Add("ushort");
				_KeyWords.Add("ulong");
				_KeyWords.Add("csbyte");
				_KeyWords.Add("cuint");
				_KeyWords.Add("culng");
				_KeyWords.Add("cushort");
				_KeyWords.Add("partial");
				_KeyWords.Add("sbyte");
			}
		}
		public static StringCollection Collection
		{
			get
			{
				if (_KeyWords == null)
					CreateKeywords();
				return _KeyWords;
			}
		}
	}
}
