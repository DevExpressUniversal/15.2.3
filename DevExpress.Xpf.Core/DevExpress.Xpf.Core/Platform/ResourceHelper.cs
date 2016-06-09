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

using System.Windows;
using System;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Bars.Themes;
using System.Collections.Generic;
using System.Windows.Media;
namespace DevExpress.Xpf.Core {
	public static class ResourceHelper {
		public static object FindResource(this FrameworkElement root, object key) {
			key = CorrectResourceKey(key);
#if !SILVERLIGHT
			return root.TryFindResource(key);
#else
			if (root == null || key == null)
				return null;
			object result = FindResource(root.Resources, key);
			if(result == null) {
				FrameworkElement parent = root.Parent as FrameworkElement;
				if(parent == null) parent = VisualTreeHelper.GetParent(root) as FrameworkElement;
				if(parent != null)
					result = FindResource(parent, key);
			}
			if(result == null)
				result = FindResource(key);
#if SILVERLIGHT
			if(result == null && ThemeManager.ActualApplicationTheme != null && ThemeManager.ActualApplicationTheme.Styles != null)
				result = FindResource(ThemeManager.ActualApplicationTheme.Styles, key);
			if(result == null)
				result = FindResource(AgStyleStorage.Dictionaries, key);
#endif
			return result;
#endif
		}
		static object FindResource(List<ResourceDictionary> dicList, object key) {
			object res = null;
			for(int i = dicList.Count - 1; i >= 0; i--) {
				res = FindResource(dicList[i], key);
				if(res != null)
					break;
			}
			return res;
		}
		static object FindResource(ResourceDictionary dic, object key) {
			if (dic == null || key == null)
				return null;
			if (dic.Contains(key))
				return dic[key];
			object res = null;
			for(int i = dic.MergedDictionaries.Count - 1; i >= 0; i--) {
				res = FindResource(dic.MergedDictionaries[i], key);
				if(res != null)
					break;
			}
			return res;
		}
		static object FindResource(object key) {
			if (Application.Current != null && Application.Current.Resources.Contains(key))
				return Application.Current.Resources[key];
			return null;
		}
		internal static object CorrectResourceKey(object key) {
#if !SILVERLIGHT
			return key;
#else
			if(key is Type)
				return key;
			string res = key.ToString();
			return res.Replace("Extension", "");
#endif
		}
	}
}
