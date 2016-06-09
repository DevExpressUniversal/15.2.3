﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.PropertyEditing;
namespace DevExpress.Xpf.PropertyGrid.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		protected override Assembly RuntimeAssembly { get { return typeof(PropertyGridControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabData; } }
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			builder.AddCustomAttributes(typeof(PropertyGridControl), new FeatureAttribute(typeof(PropertyGridControlInitializer)));
			builder.AddCustomAttributes(typeof(PropertyGridControl), "PropertyDefinitions", new NewItemTypesAttribute(typeof(PropertyDefinition)));
			builder.AddCustomAttributes(typeof(PropertyGridControl), "PropertyDefinitions", new NewItemTypesAttribute(typeof(CategoryDefinition)));
			builder.AddCustomAttributes(typeof(PropertyGridControl), "PropertyDefinitions", new NewItemTypesAttribute(typeof(CollectionDefinition)));
			builder.AddCustomAttributes(typeof(PropertyDefinitionBase), "PropertyDefinitions", new NewItemTypesAttribute(typeof(PropertyDefinition)));
			builder.AddCustomAttributes(typeof(PropertyDefinitionBase), "PropertyDefinitions", new NewItemTypesAttribute(typeof(CollectionDefinition)));
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new PropertyGridPropertyLineProvider());
		}
	}
}
