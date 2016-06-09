#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
namespace DevExpress.ExpressApp.DC {
	public interface ITypesInfo {
		void RegisterEntity(String name, Type interfaceType);
		void RegisterEntity(String name, Type interfaceType, Type baseClass);
		void RegisterSharedPart(Type interfaceType);
		void RegisterDomainLogic(Type interfaceType, Type logicType);
		void UnregisterDomainLogic(Type interfaceType, Type logicType);
		void GenerateEntities();
		void GenerateEntities(String generatedAssemblyFile);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		void RegisterEntity(Type entityType);
		ITypeInfo FindTypeInfo(Type type);
		ITypeInfo FindTypeInfo(String typeName);
		Boolean CanInstantiate(Type type);
		void RefreshInfo(ITypeInfo info);
		void RefreshInfo(Type type);
		IAssemblyInfo FindAssemblyInfo(Type ofType);
		IAssemblyInfo FindAssemblyInfo(System.Reflection.Assembly assembly);
		void LoadTypes(System.Reflection.Assembly assembly);
		IMemberInfo CreatePath(IMemberInfo first, IMemberInfo second);
		IEnumerable<ITypeInfo> PersistentTypes { get; }
	}
}
