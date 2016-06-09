#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Text.RegularExpressions;
#if MVVM
namespace DevExpress.Mvvm.Native {
	static class SplitStringHelper {
#else
namespace DevExpress.Data.Helpers {
	public static class SplitStringHelper {
#endif
#if SILVERLIGHT || NETFX_CORE || DOTNET
			static Regex reg1 = new Regex("(\\p{Ll})(\\p{Lu})");
			static Regex reg2 = new Regex("(\\p{Lu}{2})(\\p{Lu}\\p{Ll}{2})");
#else
		static Regex reg1 = new Regex("(\\p{Ll})(\\p{Lu})", RegexOptions.Compiled);
		static Regex reg2 = new Regex("(\\p{Lu}{2})(\\p{Lu}\\p{Ll}{2})", RegexOptions.Compiled);
#endif
		public static string SplitPascalCaseString(string value) {
			if(value == null)
				return string.Empty;
			value = reg1.Replace(value, "$1 $2");
			value = reg2.Replace(value, "$1 $2");
			return value;
		}
	}
}
