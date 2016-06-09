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

using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Bars.Native {
	public class RegistratorFactoryDecorator : IBarNameScopeDecorator {
		BarNameScope scope;
		Func<BarNameScope, object, bool, ElementRegistrator> addRegistratorCallback;
		static List<Tuple<object, bool, Func<object, bool>, Action<ElementRegistrator>>> registeredRegistrators;
		static RegistratorFactoryDecorator() {
			registeredRegistrators = new List<Tuple<object, bool, Func<object, bool>, Action<ElementRegistrator>>>();
			AddRegistrator<BarItem>(true, x => (x is string) ? !string.IsNullOrEmpty((string)x) : x.ReturnSuccess(), x => x.SkipUniquenessCheck = !BarManager.CheckBarItemNames);
			AddRegistrator<BarItemLink>(false, x => (x is string) ? !string.IsNullOrEmpty((string)x) : x.ReturnSuccess());
			AddRegistrator<IMergingSupport>(false, x => !string.IsNullOrEmpty(x as string));
			AddRegistrator<IFrameworkInputElement>(false, name => !Equals(null, name));
			AddRegistrator<ILinksHolder>(false, name => !Equals(null, name));
			AddRegistrator(BarRegistratorKeys.BarNameKey, false, name => !string.IsNullOrEmpty(name as string));
			AddRegistrator(BarRegistratorKeys.ContainerNameKey, false, name => !string.IsNullOrEmpty(name as string));
			AddRegistrator(BarRegistratorKeys.BarTypeKey, false, name => (BarContainerType)name != BarContainerType.None);
			AddRegistrator(BarRegistratorKeys.ContainerTypeKey, false, name => (BarContainerType)name != BarContainerType.None && (BarContainerType)name != BarContainerType.Floating);
			AddRegistrator<IEventListenerClient>(false, name => !Equals(null, name));
		}
		public RegistratorFactoryDecorator(Func<BarNameScope, object, bool, ElementRegistrator> addRegistratorCallback) { this.addRegistratorCallback = addRegistratorCallback; }
		public static void AddRegistrator<T>(bool uniqueElements, Func<object, bool> validateElementNamePredicate, Action<ElementRegistrator> setProperties = null) {
			AddRegistrator(typeof(T), uniqueElements, validateElementNamePredicate, setProperties);
		}
		public static void RemoveRegistrator<T>() {
			RemoveRegistrator(typeof(T));
		}
		public static void AddRegistrator(object key, bool uniqueElements, Func<object, bool> validateElementNamePredicate, Action<ElementRegistrator> setProperties = null) {
			Guard.ArgumentNotNull(key, "key");
			Guard.ArgumentNotNull(validateElementNamePredicate, "validateElementNamePredicate");
			registeredRegistrators.RemoveAll(x => Equals(x.Item1, key));
			registeredRegistrators.Add(new Tuple<object, bool, Func<object, bool>, Action<ElementRegistrator>>(key, uniqueElements, validateElementNamePredicate, setProperties));
		}		
		public static void RemoveRegistrator(object key) {
			Guard.ArgumentNotNull(key, "key");
			registeredRegistrators.RemoveAll(x => Equals(x.Item1, key));
		}
		void IBarNameScopeDecorator.Attach(BarNameScope scope) {
			this.scope = scope;
			foreach (var value in registeredRegistrators) {
				var registrator = addRegistratorCallback(scope, value.Item1, value.Item2);
				if (!Equals(null, value.Item3))
					registrator.ValidateNamePredicate = value.Item3;
				if (!Equals(null, value.Item4))
					value.Item4(registrator);
			}
		}		
		void IBarNameScopeDecorator.Detach() {
			scope = null;
		}
	}
}
