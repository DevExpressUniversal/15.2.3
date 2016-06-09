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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.PropertyGrid.Themes {
	public class DynamicPropertyGridResourceExtension : ThemeResourceExtensionBase {
		const string assemblyName = "DevExpress.Xpf.PropertyGrid";
		const string defaultThemeName = Theme.DeepBlueName;
		const string themesPrefix = "DevExpress.Xpf.Themes";
		string themeName = null;
		protected sealed override string Namespace {
			get { return !String.IsNullOrEmpty(themeName) ? String.Format("{0}.{1}", themesPrefix, themeName) : assemblyName; }
		}
		public DynamicPropertyGridResourceExtension(string resourcePath) : base(resourcePath) { }
		protected override string GetResourcePath(IServiceProvider serviceProvider) {
			themeName = ThemeNameHelper.GetThemeName(serviceProvider);
			if (string.IsNullOrEmpty(themeName)) {
				return String.Format("Themes/{0}/{1}", Theme.DeepBlueName, ResourcePath);
			} else {
				return String.Format("{0}/{1}/{2}", assemblyName, themeName, ResourcePath);
			}
		}
	}
}
