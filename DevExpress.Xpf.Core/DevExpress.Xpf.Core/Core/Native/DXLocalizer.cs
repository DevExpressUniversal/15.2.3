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

using System.Collections.Generic;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Reflection;
namespace DevExpress.Xpf.Core {
	public abstract class DXLocalizer<T> : XtraLocalizer<T> where T : struct {
		protected override void AddString(T id, string str) {
			Dictionary<T, string> table = XtraLocalizierHelper<T>.GetStringTable(this);
			table[id] = str;
		}
	}
	public abstract class DXResXLocalizer<T> : XtraResXLocalizer<T> where T : struct {
		protected DXResXLocalizer(XtraLocalizer<T> embeddedLocalizer)
			: base(embeddedLocalizer) {
		}
	}
#if DEBUGTEST
	public static class DXResXLocalizerTextExtensions {
		public static string GetLocalizedStringFromResources<T>(this XtraResXLocalizer<T> resLocalizer, T id) where T : struct {
			return (string)typeof(XtraResXLocalizer<T>).GetMethod("GetLocalizedStringFromResources", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(resLocalizer, new object[] { id });
		}
	}
#endif
}
