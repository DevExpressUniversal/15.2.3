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
using System.Reflection;
using System.Windows;
#if !NETFX_CORE
using DevExpress.Mvvm.POCO;
#else
using Windows.UI.Xaml;
using DevExpress.Mvvm.Native;
using Windows.ApplicationModel;
#endif
namespace DevExpress.Mvvm.UI {
	public class ViewLocator : ViewLocatorBase {
		static Assembly entryAssembly;
		protected static Assembly EntryAssembly {
			get {
				if(entryAssembly == null) {
#if SILVERLIGHT
					entryAssembly = Application.Current == null ? null : Application.Current.GetType().Assembly;
#elif NETFX_CORE
					entryAssembly = Application.Current == null ? null : Application.Current.GetType().GetAssembly();
#else
					entryAssembly = Assembly.GetEntryAssembly();
#endif
				}
				return entryAssembly;
			}
			set { entryAssembly = value; }
		}
		public static IViewLocator Default { get { return _default ?? Instance; } set { _default = value; } }
		static IViewLocator _default = null;
		internal static readonly IViewLocator Instance = new ViewLocator(Application.Current);
		readonly IEnumerable<Assembly> assemblies;
		public ViewLocator(Application application)
#if !NETFX_CORE
			: this(EntryAssembly != null && !EntryAssembly.IsInDesignMode() ? new[] { EntryAssembly } : new Assembly[0]) {
#else
			: this(EntryAssembly != null && !DesignMode.DesignModeEnabled ? new[] { EntryAssembly } : new Assembly[0]) {
#endif
		}
		public ViewLocator(IEnumerable<Assembly> assemblies) {
			this.assemblies = assemblies;
		}
		public ViewLocator(params Assembly[] assemblies)
			: this((IEnumerable<Assembly>)assemblies) {
		}
		protected override IEnumerable<Assembly> Assemblies {
			get {
				return assemblies;
			}
		}
	}
}
