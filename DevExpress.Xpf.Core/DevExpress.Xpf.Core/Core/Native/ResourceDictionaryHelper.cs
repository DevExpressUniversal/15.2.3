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
using System.Windows;
using System.Collections;
namespace DevExpress.Xpf.Utils.Native {
	public class ResourceDictionaryHelper {
		public static ResourceDictionary CloneResourceDictionary(ResourceDictionary dict) {
			if(IsEmptyDictionary(dict))
				return new ResourceDictionary();
			ResourceDictionary clone = new ResourceDictionary();
			return CloneResourceDictionary(clone, dict);
		}
		public static ResourceDictionary CloneResourceDictionary(ResourceDictionary clone,  ResourceDictionary dict) {
			foreach(DictionaryEntry entry in dict)
				clone.Add(entry.Key, entry.Value);
			foreach(ResourceDictionary merged in dict.MergedDictionaries)
				clone.MergedDictionaries.Add(CloneResourceDictionary(merged));
			return clone;
		}
		public static bool IsEmptyDictionary(ResourceDictionary resources) {
			if(resources == null)
				return true;
			if(resources.Count > 0)
				return false;
			if(resources.MergedDictionaries.Count > 0) {
				foreach(ResourceDictionary dict in resources.MergedDictionaries) {
					if(!IsEmptyDictionary(dict))
						return false;
				}
			}
			return true;
		}
		public static ResourceDictionary GetResources(DependencyObject d) {
			if(d is FrameworkElement)
				return ((FrameworkElement)d).Resources;
#if !SL
			else if(d is FrameworkContentElement)
				return ((FrameworkContentElement)d).Resources;
#endif
			throw new ArgumentException("element");
		}
	}
}
