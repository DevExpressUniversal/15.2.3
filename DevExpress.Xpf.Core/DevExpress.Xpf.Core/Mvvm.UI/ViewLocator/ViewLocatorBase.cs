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
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Mvvm.UI {
	public abstract class ViewLocatorBase : IViewLocator {
		protected abstract IEnumerable<Assembly> Assemblies { get; }
		Dictionary<string, Type> types = new Dictionary<string, Type>();
		IEnumerator<Type> enumerator;
		object IViewLocator.ResolveView(string viewName) {
			Type viewType = ((IViewLocator)this).ResolveViewType(viewName);
			if(viewType != null)
				return Activator.CreateInstance(viewType);
			return CreateFallbackView(viewName);
		}
		Type IViewLocator.ResolveViewType(string viewName) {
			if(string.IsNullOrEmpty(viewName))
				return null;
			Type typeFromDictioanry;
			if(types.TryGetValue(viewName, out typeFromDictioanry))
				return typeFromDictioanry;
			if(enumerator == null)
				enumerator = GetTypes();
			while(enumerator.MoveNext()) {
				if(!types.ContainsKey(enumerator.Current.Name)) {
					types[enumerator.Current.Name] = enumerator.Current;
				}
				if(enumerator.Current.Name == viewName)
					return enumerator.Current;
			}
			return null;
		}
		protected virtual object CreateFallbackView(string documentType) {
			return ViewLocatorExtensions.CreateFallbackView(GetErrorMessage(documentType));
		}
		protected string GetErrorMessage(string documentType) {
			return ViewLocatorExtensions.GetErrorMessage_CannotResolveViewType(documentType);
		}
		protected virtual IEnumerator<Type> GetTypes() {
			foreach(Assembly asm in Assemblies) {
				Type[] types;
				try {
#if !NETFX_CORE
					types = asm.GetTypes();
#else
					types = Mvvm.Native.TypeExtensions.GetExportedTypes(asm);
#endif
				} catch(ReflectionTypeLoadException e) {
					types = e.Types;
				}
				foreach(Type type in types) {
					yield return type;
				}
			}
		}
	}
}
