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

using DevExpress.Data.Helpers;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.LookUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid.Native {
	public class SmartModelGridColumnsGenerator : ModelGridColumnsGeneratorBase {
		public SmartModelGridColumnsGenerator(ColumnCreatorBase creator, bool applyOnlyForSmartColumns, bool skipXamlGenerationProperties)
			: base(creator, applyOnlyForSmartColumns, skipXamlGenerationProperties) {
		}
		protected override Type GetLookUpEditType() { return typeof(LookUpEditSettings); }
		public override void DateTime(IEdmPropertyInfo property) {
			GenerateEditor(property, Mode == EditorsGeneratorMode.Edit ? typeof(DateEdit) : typeof(DateEditSettings), DateTimeInitializer(property, null, true));
		}
		public override void LookUp(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			Type editType = null;
			if(string.IsNullOrEmpty(itemsSource)) editType = Mode == EditorsGeneratorMode.Edit ? typeof(TextEdit) : null;
			else editType = GetLookUpEditType();
			GenerateEditor(property, null, editType != null ? Context.CreateItem(editType) : null,
				LookUpInitializer(property, itemsSource, displayMember, foreignKeyInfo),
				string.IsNullOrEmpty(itemsSource) ? displayMember : null, true);
		}
		public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
			var template = GetResourceTemplate((FrameworkElement)Context.GetRoot().GetCurrentValue(), resourceKey);
			var content = GetResourceContent<ColumnBase, BaseEditSettings, BaseEdit>(template);
			var contentModel = new RuntimeEditingContext(content).GetRoot();
			if(content is ColumnBase) {
				var editSettingsModel = ((ColumnBase)content).EditSettings != null ? 
					new RuntimeEditingContext(((ColumnBase)content).EditSettings).GetRoot() : null;
				GenerateEditor(property, contentModel, editSettingsModel, initializer, null, true);
				return;
			}
			if(content is BaseEditSettings) {
				GenerateEditor(property, null, contentModel, initializer, null, true);
				return;
			}
			Initializer resInitializer = initializer + new Initializer(null, (container) => container.SetValueIfNotSet(ColumnBase.CellTemplateProperty, template));
			resInitializer = new Initializer(null, resInitializer.SetContainerProperties);
			GenerateEditor(property, null, null, resInitializer, null, true);
		}
	}
	public class ScaffolingSmartModelGridColumnsGenerator : SmartModelGridColumnsGenerator {
		class ColumnCreator : ColumnCreatorBase {
			public ColumnCreator(IEditingContext context, IModelItemCollection columns, AllColumnsInfo columnsInfo)
				: base(context, columns, columnsInfo) {
			}
			protected override Type ColumnType { get { return typeof(GridColumn); } }
		}
		public ScaffolingSmartModelGridColumnsGenerator(IEditingContext context, IModelItemCollection columns, AllColumnsInfo columnsInfo)
			: base(new ColumnCreator(context, columns, columnsInfo), false, false) {
		}
		public override void Text(IEdmPropertyInfo property, bool multiline) {
			if(!multiline) {
				base.Text(property, multiline);
				return;
			}
			var maxLength = GetMaxLength(property);
			if(maxLength > 0) this.GenerateEditor(property, null, null, Initializer.Default, null, true);
			else base.GenerateEditor(property, null, null, Initializer.Default, null, true);
		}
		public override void LookUp(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			if(string.IsNullOrEmpty(itemsSource)) base.GenerateEditor(property, null, null, 
				LookUpInitializer(property, itemsSource, displayMember, foreignKeyInfo), displayMember, true);
			else base.GenerateEditor(property, null, Context.CreateItem(typeof(LookUpEditSettings)), 
				LookUpInitializer(property, itemsSource, displayMember, foreignKeyInfo), null, true);
		}
		protected override Initializer LookUpInitializer(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
			if(string.IsNullOrEmpty(itemsSource))
				return new Initializer(null, (container) => SetColumnPropertyValue(container, ColumnBase.ReadOnlyProperty, true));
			return new Initializer((container, edit) => {
				SetSettingsPropertyValue(container, edit, LookUpEditSettingsBase.ItemsSourceProperty, new Binding(itemsSource));
				SetSettingsPropertyValue(container, edit, LookUpEditSettingsBase.DisplayMemberProperty, displayMember);
			});
		}
		public override void Image(IEdmPropertyInfo property, bool readOnly) { }
		protected override Initializer ImageInitializer(IEdmPropertyInfo property, bool readOnly) {
			return Initializer.Default;
		}
		protected override void GenerateEditor(IEdmPropertyInfo property, IModelItem column, IModelItem editSettings, Initializer initializer, string displayMember, bool setFieldName) {
			if(column == null) column = creator.CreateColumn(property);
			if(column == null) return;
			column.SetValue(ColumnBase.IsSmartProperty, true);
			AttributesApplier.ApplyBaseAttributes(propertyInfo: property,
					setDisplayMember: x => {
						string path = x + (string.IsNullOrEmpty(displayMember) ? null : "." + displayMember);
						if(setFieldName) column.SetValueIfNotSet(ColumnBase.FieldNameProperty, path);
						if(path != x) {
							string header = SplitStringHelper.SplitPascalCaseString(x);
							SetColumnPropertyValue(column, ColumnBase.HeaderProperty, header, SkipXamlGenerationProperties, false);
						}
					},
					setDescription: null,
					setDisplayName: null,
					setDisplayShortName: null,
					setReadOnly: null,
					setEditable: null,
					setInvisible: () => SetColumnPropertyValue(column, ColumnBase.VisibleProperty, false, SkipXamlGenerationProperties, false),
					setHidden: () => SetColumnPropertyValue(column, ColumnBase.VisibleProperty, false, SkipXamlGenerationProperties, false),
					setRequired: null);
			initializer.SetContainerProperties(column);
			creator.AddColumn(column);
		}
		protected override bool IsEditSettingsSet(IModelItem column) {
			return column.Properties["EditSettings"].IsSet;
		}
	}
	public class ScaffoldingBandsGenerator : BandsGenerator {
		public ScaffoldingBandsGenerator(IModelItem grid)
			: base(grid, grid, typeof(GridControlBand), (dataControl, columns, columnsInfo) => new ScaffolingSmartModelGridColumnsGenerator(dataControl.Context, columns, columnsInfo), false, true, null) {
		}
	}
}
