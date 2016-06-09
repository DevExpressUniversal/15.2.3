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

using DevExpress.Xpf.Bars.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Bars {
	public interface IBarNameScopeSupport : IInputElement { }	
	public interface IMultipleElementRegistratorSupport : IBarNameScopeSupport {
		IEnumerable<object> RegistratorKeys { get; }
		object GetName(object registratorKey);
	}
	public interface IRegistratorChangedListener {
		bool RegistratorChanged(object binder, ElementRegistrator registrator, ElementRegistratorChangedArgs args, bool isElementRegistrator);
	}
	#region services
	public interface IElementRegistratorService {
		IEnumerable<TRegistratorKey> GetElements<TRegistratorKey>(object name);
		IEnumerable<TRegistratorKey> GetElements<TRegistratorKey>(object name, ScopeSearchSettings searchMode);
		IEnumerable<TRegistratorKey> GetElements<TRegistratorKey>();
		IEnumerable<TRegistratorKey> GetElements<TRegistratorKey>(ScopeSearchSettings searchMode);
		void NameChanged(IBarNameScopeSupport element, object registratorKey, object oldName, object newName, bool skipNameEqualityCheck = false);
		void Changed(IBarNameScopeSupport element, object registratorKey);
	}
	public interface IRadioGroupService {
		bool CanUncheck(IBarCheckItem element);
		void OnChecked(IBarCheckItem element);
	}
	public interface ICommandSourceService {
		void CommandChanged(BarItem element, ICommand oldValue, ICommand newValue);
		void KeyGestureChanged(BarItem element, KeyGesture oldValue, KeyGesture newValue);
	}
	public interface IElementBinderService : IRegistratorChangedListener {
		IEnumerable<IBarNameScopeSupport> GetMatches(object element);		
	}
#if DEBUGTEST
	public interface IBarNameScopeDebugService {
		BarNameScope Scope { get; }
	}
#endif
	#endregion
	public interface IBarNameScopeDecorator {
		void Attach(BarNameScope scope);
		void Detach();
	}
}
