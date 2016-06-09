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
	public interface IVS2012TypeResolver {
		IVS2012AssemblyMetadata ProjectAssembly { get; }
		IEnumerable<IVS2012AssemblyMetadata> AssemblyReferences { get; }
		IVS2012TypeMetadata GetType(Type type);
		IEnumerable<IVS2012TypeMetadata> GetTypes(IVS2012AssemblyMetadata assembly);
		bool EnsureAssemblyReferenced(string simpleNameOrPath, bool includeDependencies);
	}
	public interface IVS2012MemberMetadata {
		bool IsPublic { get; }
		string Name { get; }
		string FullName { get; }
	}
	public interface IVS2012TypeMetadata : IVS2012MemberMetadata {
		IVS2012AssemblyMetadata RuntimeAssembly { get; }
		string Namespace { get; }
		bool IsAbstract { get; }
		bool IsArray { get; }
		bool IsInterface { get; }
		bool IsGenericType { get; }
		bool IsAssignableFrom(IVS2012TypeMetadata type);
		bool HasDefaultConstructor { get; }
		IVS2012TypeMetadata BaseType { get; }
		Type RuntimeType { get; }
	}
	public interface IVS2012PropertyMetadata : IVS2012MemberMetadata {
	}
	public interface IVS2012AssemblyMetadata {
		string Name { get; }
		string FullName { get; }
	}
}
