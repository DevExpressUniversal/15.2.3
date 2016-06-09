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
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Entity.Model.Metadata {
	public enum BuiltInTypeKind {
		AssociationEndMember = 0,
		AssociationSetEnd = 1,
		AssociationSet = 2,
		AssociationType = 3,
		EntitySetBase = 4,
		EntityTypeBase = 5,
		CollectionType = 6,
		CollectionKind = 7,
		ComplexType = 8,
		Documentation = 9,
		OperationAction = 10,
		EdmType = 11,
		EntityContainer = 12,
		EntitySet = 13,
		EntityType = 14,
		EnumType = 15,
		EnumMember = 16,
		Facet = 17,
		EdmFunction = 18,
		FunctionParameter = 19,
		GlobalItem = 20,
		MetadataProperty = 21,
		NavigationProperty = 22,
		MetadataItem = 23,
		EdmMember = 24,
		ParameterMode = 25,
		PrimitiveType = 26,
		PrimitiveTypeKind = 27,
		EdmProperty = 28,
		ProviderManifest = 29,
		ReferentialConstraint = 30,
		RefType = 31,
		RelationshipEndMember = 32,
		RelationshipMultiplicity = 33,
		RelationshipSet = 34,
		RelationshipType = 35,
		RowType = 36,
		SimpleType = 37,
		StructuralType = 38,
		TypeUsage = 39,
	}
}
