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

using DevExpress.Entity.Model;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Xpf.Internal.EntityFrameworkWrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Core.Design.Wizards {
	static class EditorsGeneratorCompatibility {
		public static Type GetClrType(MetadataWorkspaceRuntimeWrapper workspace, EntityTypeRuntimeWrapper owner) {
			var oSpaceOwner = workspace.GetItems(DataSpaceRuntimeWrapper.OSpace)
				.Where(x => x.BuiltInTypeKind == owner.BuiltInTypeKind)
				.Select(EntityTypeRuntimeWrapper.Wrap)
				.First(x => x.FullName == owner.FullName);
			return oSpaceOwner.ClrType;
		}
		public static PropertyDescriptor GetPropertyDescriptor(MetadataWorkspaceRuntimeWrapper workspace, EntityTypeRuntimeWrapper owner, EdmPropertyRuntimeWrapper property) {
			return TypeDescriptor.GetProperties(GetClrType(workspace, owner))[property.Name];
		}
		public static IEdmPropertyInfo CreateEdmPropertyInfo(MetadataWorkspaceRuntimeWrapper workspace, EntityTypeRuntimeWrapper owner, EdmPropertyRuntimeWrapper property) {
			var oSpaceOwner = workspace.GetItems(DataSpaceRuntimeWrapper.OSpace)
				.Where(x => x.BuiltInTypeKind == owner.BuiltInTypeKind)
				.Select(EntityTypeRuntimeWrapper.Wrap)
				.First(x => x.FullName == owner.FullName);
			var descriptor = GetPropertyDescriptor(workspace, owner, property);
			return CreateEdmPropertyInfo(oSpaceOwner.ClrType, descriptor);
		}
		public static IEdmPropertyInfo CreateEdmPropertyInfo(Type owner, PropertyDescriptor property) {
			var attributesProvider = (IDataColumnAttributesProvider)new DataColumnAttributesProvider();
			var attributes = attributesProvider.GetAtrributes(property, owner);
			return new EdmPropertyInfo(property, attributes, true, false);
		}
	}
}
