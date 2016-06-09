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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ModelPersistentName("ViewItems")]
	[ModelNodesGenerator(typeof(ModelRegisteredViewItemsGenerator))]
	[ImageName("ModelEditor_DetailViewItems")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("EditorsIModelRegisteredViewItems")]
#endif
	public interface IModelRegisteredViewItems : IModelNode, IModelList<IModelRegisteredViewItem> {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelRegisteredViewItemsPropertyEditors")]
#endif
		IModelRegisteredPropertyEditors PropertyEditors { get; } 
	}
	[KeyProperty("Name")]
	[ModelPersistentName("ViewItem")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("EditorsIModelRegisteredViewItem")]
#endif
	public interface IModelRegisteredViewItem : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredViewItemName"),
#endif
 Category("Data")]
		string Name { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredViewItemDefaultItemType"),
#endif
 DataSourceProperty("RegisteredTypes")]
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[Required]
		[Category("Appearance")]
		Type DefaultItemType { get; set; }
		[Browsable(false)]
		List<Type> RegisteredTypes { get; }
	}
	[ReadOnly(true)]
	[ModelPersistentName("PropertyEditor")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("EditorsIModelRegisteredPropertyEditors")]
#endif
	public interface IModelRegisteredPropertyEditors : IModelRegisteredViewItem, IModelList<IModelRegisteredPropertyEditor> {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorsProtectedContentPropertyEditor"),
#endif
 Required]
		[DataSourceProperty("ProtectedContentRegisteredTypes")]
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[Category("Appearance")]
		Type ProtectedContentPropertyEditor { get; set; }
		[Browsable(false)]
		List<Type> ProtectedContentRegisteredTypes { get; }
	}
	[KeyProperty("PropertyType")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("EditorsIModelRegisteredPropertyEditor")]
#endif
	public interface IModelRegisteredPropertyEditor : IModelNode {
		[Category("Data")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorPropertyType")]
#endif
		string PropertyType { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorEditorType")]
#endif
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[Required, DataSourceProperty("RegisteredTypes")]
		[Category("Appearance")]
		Type EditorType { get; set; }
		[Browsable(false)]
		List<Type> RegisteredTypes { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorDefaultDisplayFormat"),
#endif
 Category("Format"), Localizable(true)]
		string DefaultDisplayFormat { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorDefaultEditMask"),
#endif
 Category("Format"), Localizable(true)]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.MaskModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string DefaultEditMask { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRegisteredPropertyEditorDefaultEditMaskType"),
#endif
 Category("Format")]
		[Browsable(false)]
		EditMaskType DefaultEditMaskType { get; set; }
	}
}
