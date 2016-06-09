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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
using DevExpress.Utils.Design;
using System.Globalization;
using DevExpress.XtraPrinting.Native.Lines;
namespace DevExpress.XtraReports.Native {
	public interface IEditingContext {
		string Name { get; }
		object RootComponent { get; }
	}
	public interface IRepositoryItemCreator {
		RepositoryItem CreateItem(object instance, Type dataType, IEditingContext context);
	}
	public class EditingContext : IEditingContext {
		string name;
		object rootComponent;
		public EditingContext(string name, object rootComponent) {
			this.name = name;
			this.rootComponent = rootComponent;
		}
		public string Name {
			get { return name; }
		}
		public object RootComponent {
			get { return rootComponent; }
		}
	}
	public class DataEditorService : InstanceProvider<Dictionary<Type, IRepositoryItemCreator>> {
		public const string Guid = "DataEditorExtension";
		static readonly object padlock = new object();
		static DataEditorService service = new DataEditorService();
		public static RepositoryItem GetRepositoryItem(Type type, object instance, IEditingContext context) {
			lock(padlock) {
				return service.GetItem(type, instance, context);
			}
		}
		public static RepositoryItem GetRepositoryItem(Type type) {
			lock(padlock) {
				if(type.IsEnum)
					return CreateRepositoryItem(TypeDescriptor.GetConverter(type), Enum.GetValues(type));
				if(type == typeof(DateTime) || type == typeof(DateTime?)) {
					RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit() { EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered, EditValueChangedDelay = 500 };
					repositoryItemDateEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
					return repositoryItemDateEdit;
				}
				if(type == typeof(Boolean) || type == typeof(Boolean?))
					return CreateRepositoryItem(new AlwaysLocalizedBooleanTypeConverter(), new object[] { true, false });
				return new RepositoryItemTextEdit() { EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered, EditValueChangedDelay = 500 };
			}
		}
		public static RepositoryItem GetMultiValueRepositoryItem(Type type) { 
			lock(padlock) {
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if(type == typeof(Boolean) || type == typeof(Boolean?))
					converter = new AlwaysLocalizedBooleanTypeConverter();
				return new RepositoryItemMultiValueEdit()
				{
					EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered,
					EditValueChangedDelay = 500,
					ValueType = type,
					BaseConverter = converter,
				};
			}
		}
		public static void RegisterRepositoryItemCreator(string contextName, Type dataType, IRepositoryItemCreator creator) {
			lock(padlock) {
				service.SetCreator(contextName, dataType, creator);
			}
		}
		RepositoryItem GetItem(Type type, object instance, IEditingContext context) {
			Dictionary<Type, IRepositoryItemCreator> creators = GetInstance(context.Name);
			if(creators != null) {
				IRepositoryItemCreator creator = GetCreator(creators, type);
				if(creator != null)
					return creator.CreateItem(instance, type, context);
			}
			return null;
		}
		static IRepositoryItemCreator GetCreator(Dictionary<Type, IRepositoryItemCreator> creators, Type type) {
			IRepositoryItemCreator creator;
			creators.TryGetValue(type, out creator);
			return creator;
		}
		void SetCreator(string contextName, Type dataType, IRepositoryItemCreator item) {
			Dictionary<Type, IRepositoryItemCreator> creators = GetInstance(contextName);
			if(creators == null) {
				creators = new Dictionary<Type, IRepositoryItemCreator>();
				SetInstance(contextName, creators);
			}
			creators[dataType] = item;
		}
		static RepositoryItem CreateRepositoryItem(TypeConverter typeConverter, Array values) {
			RepositoryItemImageComboBox repositoryItem = new RepositoryItemImageComboBox();
			RuntimeTypeDescriptorContext context = new RuntimeTypeDescriptorContext(null, null);
			foreach(object value in values) {
				string name = typeConverter.ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string)) as string;
				repositoryItem.Items.Add(new ImageComboBoxItem(name, value));
			}
			return repositoryItem;
		}
	}
}
