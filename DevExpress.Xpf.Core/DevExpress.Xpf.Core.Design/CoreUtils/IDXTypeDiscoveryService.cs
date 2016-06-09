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

using DevExpress.Design.SmartTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Design {
	public interface IDXTypeDiscoveryService {
		IEnumerable<IDXTypeMetadata> GetTypes(Func<IDXAssemblyMetadata, bool> assemblyPredicate, Func<IDXTypeMetadata, bool> typePredicate, bool fromActiveProjectOnly);
	}
	public interface IDXAssemblyMetadata {
		string Name { get; }
		string FullName { get; }
	}
	public interface IDXTypeMetadata {
		string FullName { get; }
		string Name { get; }
		IDXAssemblyMetadata Assembly { get; }
		string Namespace { get; }
		bool IsAbstract { get; }
		bool IsArray { get; }
		bool IsEnum { get; }
		bool IsInterface { get; }
		bool IsValueType { get; }
		bool IsGenericType { get; }
		bool HasDefaultConstructor { get; }
		bool IsPocoViewModel { get; }
		IDXTypeMetadata BaseType { get; }
		Type GetRuntimeType();
		bool IsVisible { get; }
		bool ImplementsInterface(Type interfaceType);
	}
}
