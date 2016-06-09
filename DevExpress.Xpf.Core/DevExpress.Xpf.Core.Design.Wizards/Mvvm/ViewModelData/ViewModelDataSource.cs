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
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Utils.Design;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Entity.Model;
using DevExpress.Images;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class TypePropertyPair {
		public TypePropertyPair(Type type, string propertyName) {
			Type = type;
			PropertyName = propertyName;
		}
		public string PropertyName { get; private set; }
		public Type Type { get; private set; }
		public TypeNamePropertyPair ToTypeNamePropertyPair() {
			return new TypeNamePropertyPair(Type.FullName, PropertyName);
		}
	}
	public static class ViewModelDataSource {
		class ReadOnlyCollectionViewModel {
			public ICommand RefreshCommand { get; set; }
		}
		class CollectionViewModel : ReadOnlyCollectionViewModel {
			public ICommand NewCommand { get; set; }
			[CommandParameter(SelectedEntityPropertyName)]
			public ICommand EditCommand { get; set; }
			[CommandParameter(SelectedEntityPropertyName)]
			public ICommand DeleteCommand { get; set; }
		}
		class EntityViewModel {
			public ICommand SaveCommand { get; set; }
			public ICommand SaveAndCloseCommand { get; set; }
			public ICommand SaveAndNewCommand { get; set; }
			[Display(Name = "Reset Changes")]
			public ICommand ResetCommand { get; set; }
			public ICommand DeleteCommand { get; set; }
			public ICommand CloseCommand { get; set; }
			[DXImage(DXImages.Save)]
			[Display(Name = "Save Layout")]
			public ICommand SaveLayoutCommand { get; set; }
			[DXImage(DXImages.Reset)]
			[Display(Name = "Reset Layout")]
			public ICommand ResetLayoutCommand { get; set; }
		}
		public const string SelectedEntityPropertyName = "SelectedEntity";
		public const string EntityPropertyName = "Entity";
		public const string FilterExpressionPropertyName = "FilterExpression";
		const bool GeneratedViewModelUsesProxyFactory = true;
		public static EntityViewModelData GetGeneratedEntityViewModelData(string assemblyName,
			string name, string nameSpace, Type entityType,
			IEntityTypeInfo typeInfo, IEnumerable<TypeNamePropertyPair> collectionProperties, IEnumerable<LookUpCollectionViewModelData> lookUpCollections, Func<IEdmPropertyInfo, ForeignKeyInfo> getForeignKeyProperty) {
			Func<ImageType, CommandInfo[]> createCommands = imageType => {
				return GetCommandInfo(typeof(EntityViewModel), imageType, null);
			};
			return new EntityViewModelData(GeneratedViewModelUsesProxyFactory,
					   assemblyName,
					   true,
					   name,
					   nameSpace,
					   true,
					   true,
					   EntityPropertyName,
					   createCommands,
					   collectionProperties,
					   true, 
					   lookUpCollections, 
					   null, 
					   typeInfo, 
					   getForeignKeyProperty);
		}
		public static CollectionViewModelData GetGeneratedCollectionViewModelData(string assemblyName, string name, string nameSpace, Type entityType, bool readOnly, IEnumerable<IEdmPropertyInfo> scaffoldingProperties, IEnumerable<TypeNamePropertyPair> collectionProperties, Func<IEdmPropertyInfo, ForeignKeyInfo> getForeignKeyProperty, IEnumerable<TypeNamePropertyPair> collectionPropertiesForWin) {
			Func<ImageType, CommandInfo[]> createCommands = imageType => {
				return GetCommandInfo(readOnly ? typeof(ReadOnlyCollectionViewModel) : typeof(CollectionViewModel), imageType, null);
			};
			return new CollectionViewModelData(GeneratedViewModelUsesProxyFactory, assemblyName, true, name, nameSpace, true, true, entityType.Name, entityType, SelectedEntityPropertyName, "Entities", createCommands, scaffoldingProperties, collectionProperties, true, null, true, getForeignKeyProperty, collectionPropertiesForWin);
		}
		public static LookUpCollectionViewModelData GetGeneratedLookUpCollectionViewModelData(string assemblyName, string name, string nameSpace, Type entityType, bool readOnly, IEnumerable<IEdmPropertyInfo> scaffoldingProperties, IEnumerable<TypeNamePropertyPair> collectionProperties, string lookUpCollectionPropertyAssociationName) {
			Func<ImageType, CommandInfo[]> createCommands = imageType => {
				return GetCommandInfo(readOnly ? typeof(ReadOnlyCollectionViewModel) : typeof(CollectionViewModel), imageType, null);
			};
			return new LookUpCollectionViewModelData(GeneratedViewModelUsesProxyFactory, assemblyName, true, name, nameSpace, true, true, entityType, SelectedEntityPropertyName, "Entities", createCommands, scaffoldingProperties, collectionProperties, true, lookUpCollectionPropertyAssociationName, null, null);
		}
		public static EntityViewModelData GetEntityViewModelData(Type type, bool isSolution, bool isLocal) {
			string typeName = type.Name;
			string nameSpace = type.Namespace;
			bool useProxyFactory;
			type = GetActualType(type, out useProxyFactory);
			if(!IsValidViewModelType(type))
				return null;
			IEnumerable<PropertyDescriptor> properties = GetProperties(type);
			IList<TypePropertyPair> genericEnumerableTypeParameters = GetGenegicEnumerableProperties(properties);
			PropertyDescriptor entityProperty = properties.FirstOrDefault(x => IsValidEntityProperty(x.PropertyType, genericEnumerableTypeParameters, false));
			if(entityProperty != null) {
				Dictionary<LookUpCollectionViewModelData, Type[]> lookUpTablesCandidates = CollectLookUpTablesCandidates(entityProperty, properties, genericEnumerableTypeParameters);
				return new EntityViewModelData(useProxyFactory,
										   GetAssemblyName(type),
										   isSolution,
										   typeName,
										   nameSpace,
										   isLocal,
										   SupportServices(type),
										   entityProperty.Name,
										   imageType => GetCommandInfo(type, null, imageType),
										   genericEnumerableTypeParameters.Select(x => x.ToTypeNamePropertyPair()), 
										   false, 
										   lookUpTablesCandidates.Select(x => x.Key).ToArray(),
										   XamlNamespaceDeclaration.Create(type),
										   GetEntityTypeInfo(entityProperty.PropertyType),
										   null);
			}
			return null;
		}
		static Dictionary<LookUpCollectionViewModelData, Type[]> CollectLookUpTablesCandidates(PropertyDescriptor entityProperty, IEnumerable<PropertyDescriptor> properties, IList<TypePropertyPair> genericEnumerableTypeParameters) {
			Dictionary<LookUpCollectionViewModelData, Type[]> lookUpTablesCandidates = new Dictionary<LookUpCollectionViewModelData, Type[]>();
			foreach(PropertyDescriptor descriptor in properties) {
				Type type = descriptor.PropertyType;
				if(!type.IsGenericType)
					continue;
				Type[] genericArgs = type.GetGenericArguments();
				int argsCount = genericArgs.Length;
				if(argsCount != 3 && argsCount != 2)
					continue;
				string entityTypeName = entityProperty.PropertyType.Name;
				string lookUpCollectionPropertySuffix = "Details";
				if(!descriptor.Name.StartsWith(entityTypeName) || !descriptor.Name.EndsWith(lookUpCollectionPropertySuffix))
					throw new InvalidOperationException();
				string lookUpCollectionPropertyAssociationName = descriptor.Name.Substring(entityTypeName.Length, descriptor.Name.Length - entityTypeName.Length - lookUpCollectionPropertySuffix.Length);
				LookUpCollectionViewModelData lookUpViewModelData = GetLookUpCollectionViewModelData(type, descriptor.Name, lookUpCollectionPropertyAssociationName, true, true);
				if(lookUpViewModelData == null)
					continue;
				lookUpTablesCandidates.Add(lookUpViewModelData, genericArgs);
			}
			return lookUpTablesCandidates;
		}
		static LookUpCollectionViewModelData GetLookUpCollectionViewModelData(Type type, string lookUpCollectionPropertyName, string lookUpCollectionPropertyAssociationName, bool isSolution, bool isLocal) {
			string typeName = type.Name;
			string nameSpace = type.Namespace;
			Type[] genericArgs = type.GetGenericArguments();
			if(genericArgs == null || genericArgs.Length<2)
				return null;
			bool useProxyFactory;
			type = GetActualType(type, out useProxyFactory);
			if(!IsValidLookUpCollectionViewModelType(type))
				return null;
			IEnumerable<PropertyDescriptor> properties = GetProperties(type);
			IEnumerable<TypePropertyPair> genericEnumerableTypeParameters = GetGenegicEnumerableProperties(properties);
			PropertyDescriptor entityProperty = properties.FirstOrDefault(x => genericArgs[0] == x.PropertyType &&  IsValidEntityProperty(x.PropertyType, genericEnumerableTypeParameters, true));
			if(entityProperty == null && !genericEnumerableTypeParameters.Any())
				return null;
			string entityPropertyName = entityProperty.With(x => x.Name);
			TypePropertyPair collectionPropertyPair = genericEnumerableTypeParameters.First(y => entityProperty == null || y.Type == entityProperty.PropertyType);
			string collectionPopertyName = collectionPropertyPair.PropertyName;
			return new LookUpCollectionViewModelData(useProxyFactory, GetAssemblyName(type), isSolution, typeName, nameSpace, isLocal, SupportServices(type), entityProperty.With(x => x.PropertyType),
				entityPropertyName, collectionPopertyName, imageType => GetCommandInfo(type, entityProperty, imageType), GetEmdProperties(collectionPropertyPair.Type), genericEnumerableTypeParameters.Select(x => x.ToTypeNamePropertyPair()), false, lookUpCollectionPropertyAssociationName, lookUpCollectionPropertyName, XamlNamespaceDeclaration.Create(type));
		}
		internal static bool IsValidLookUpCollectionViewModelType(Type type) {
			if(type.IsAbstract || !type.IsPublic || type.IsValueType || !typeof(INotifyPropertyChanged).IsAssignableFrom(type))
				return false;
			if(type.IsGenericTypeDefinition || type.IsGenericType)
				return false;
			if(typeof(DependencyObject).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type))
				return false;
			return true;
		}
		public static CollectionViewModelData GetCollectionViewModelData(Type type, bool isSolution, bool isLocal) {
			string typeName = type.Name;
			string nameSpace = type.Namespace;
			bool useProxyFactory;
			type = GetActualType(type, out useProxyFactory);
			if(!IsValidViewModelType(type))
				return null;
			IEnumerable<PropertyDescriptor> properties = GetProperties(type);
			IEnumerable<TypePropertyPair> genericEnumerableTypeParameters = GetGenegicEnumerableProperties(properties);
			PropertyDescriptor entityProperty = properties.FirstOrDefault(x => IsValidEntityProperty(x.PropertyType, genericEnumerableTypeParameters, true));
			if(entityProperty == null && !genericEnumerableTypeParameters.Any())
				return null;
			string entityPropertyName = entityProperty.With(x => x.Name);
			TypePropertyPair collectionPropertyPair = genericEnumerableTypeParameters.First(y => entityProperty == null || y.Type == entityProperty.PropertyType);
			string collectionPopertyName = collectionPropertyPair.PropertyName;
			Type entityType = entityProperty.With(x => x.PropertyType);
			string defaultViewFolderName = entityType != null ? entityType.Name : collectionPropertyPair.Type.Name;
			return new CollectionViewModelData(useProxyFactory, GetAssemblyName(type), isSolution, typeName, nameSpace, isLocal, SupportServices(type), defaultViewFolderName, entityType, entityPropertyName, collectionPopertyName, imageType => GetCommandInfo(type, entityProperty, imageType), GetEmdProperties(collectionPropertyPair.Type), genericEnumerableTypeParameters.Select(x => x.ToTypeNamePropertyPair()), false, XamlNamespaceDeclaration.Create(type), false, null,null);
		}
		static Type GetActualType(Type type, out bool useProxyFactory) {
			useProxyFactory = false;
			if(type.GetCustomAttributes(typeof(POCOViewModelAttribute), true).Any()) {
				try {
					type = ViewModelSourceHelper.GetProxyType(type);
				} catch(Exception) {
					return type;
				}
				useProxyFactory = true;
			}
			return type;
		}
		static IEnumerable<IEdmPropertyInfo> GetEmdProperties(Type entityType) {
			IEntityTypeInfo typeInfo = GetEntityTypeInfo(entityType);
			return typeInfo.AllProperties;
		}
		static ReflectionEntityTypeInfo GetEntityTypeInfo(Type entityType) {
			return new ReflectionEntityTypeInfo(entityType);
		}
		static bool SupportServices(Type type) {
			return typeof(ISupportServices).IsAssignableFrom(type);
		}
		static string GetAssemblyName(Type type) {
			return GetUnderlyingType(type).Assembly.GetName().Name;
		}
		static Type GetUnderlyingType(Type type) {
			if(typeof(IPOCOViewModel).IsAssignableFrom(type))
				type = type.BaseType;
			return type;
		}
		public static IList<TypePropertyPair> GetGenegicEnumerableProperties(IEnumerable<PropertyDescriptor> properties) {
			return properties.Select(x => GetGenericEnumerableTypeParameter(x)).Where(x => x != null && !x.Type.IsNested && !x.PropertyName.StartsWith("LookUp")).ToArray();
		}
		public static bool IsValidEntityProperty(Type propertyType, IEnumerable<TypePropertyPair> genericEnumerableTypeParameters = null, bool shouldBeInList = false) {
			if(propertyType.IsValueType || !GetProperties(propertyType).Any() || propertyType.IsNested || propertyType.GetGenericArguments().Length > 1)
				return false;
			if(typeof(ICommand).IsAssignableFrom(propertyType) || typeof(IEnumerable).IsAssignableFrom(propertyType))
				return false;
			if(typeof(System.Linq.Expressions.Expression).IsAssignableFrom(propertyType))
				return false;
			if(genericEnumerableTypeParameters != null) {
				bool isInList = genericEnumerableTypeParameters.Any(x => x.Type == propertyType);
				if(shouldBeInList)
					isInList = !isInList;
				if(isInList)
					return false;
			}
			return true;
		}
		internal static bool IsValidViewModelType(Type type) {
			if(type.IsAbstract || !type.IsPublic || type.IsValueType || !typeof(INotifyPropertyChanged).IsAssignableFrom(type))
				return false;
			if(!type.GetConstructors().Any(x => !x.GetParameters().Any()) && ViewModelSourceHelper.FindConstructorWithAllOptionalParameters(GetUnderlyingType(type)) == null)
				return false;
			if(type.IsGenericTypeDefinition || type.IsGenericType)
				return false;
			if(typeof(DependencyObject).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type))
				return false;
			return true;
		}
		static TypePropertyPair GetGenericEnumerableTypeParameter(PropertyDescriptor property) {
			Type type = property.PropertyType;
			IEnumerable<Type> interfaces = type.GetInterfaces();
			if(type.IsInterface)
				interfaces = interfaces.Union(new Type[] { type });
			foreach(Type i in interfaces) {
				if(!i.IsGenericType)
					continue;
				Type genericInterfaceDefinition = i.GetGenericTypeDefinition();
				if(genericInterfaceDefinition == typeof(IEnumerable<>))
					return new TypePropertyPair(i.GetGenericArguments().First(), property.Name);
			}
			return null;
		}
		static IEnumerable<PropertyDescriptor> GetProperties(Type type) {
			return TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>();
		}
		class CommandInfoGenerator : ICommandsGenerator {
			class CommandAttributesApplier : ICommandAttributesApplier {
				public CommandInfo CommandInfo = new CommandInfo();
				void ICommandAttributesApplier.SetCaption(string caption) {
					CommandInfo.Caption = caption;
				}
				void ICommandAttributesApplier.SetHint(string hint) {
				}
				void ICommandAttributesApplier.SetImageUri(string imageName) {
					CommandInfo.SetSmallGlyphUri(imageName);
				}
				void ICommandAttributesApplier.SetLargeImageUri(string imageName) {
					CommandInfo.SetLargeGlyphUri(imageName);
				}
				void ICommandAttributesApplier.SetParameter(string parameterPropertyName) {
					CommandInfo.ParameterPropertyName = parameterPropertyName;
				}
				void ICommandAttributesApplier.SetPropertyName(string propertyName) {
					CommandInfo.CommandPropertyName = propertyName;
				}
			}
			readonly ImageType imageType;
			List<CommandInfo> commands = new List<CommandInfo>();
			public CommandInfoGenerator(ImageType imageType) {
				this.imageType = imageType;
			}
			void ICommandsGenerator.GenerateCommand(IEdmPropertyInfo property) {
				var applier = new CommandAttributesApplier();
				property.ApplyCommandAttributes(applier, imageType);
				commands.Add(applier.CommandInfo);
			}
			public CommandInfo[] GetCommands() {
				return commands.ToArray();
			}
		}
		class AttributesProvider : IAttributesProvider {
			string[] commandsWithParameter;
			string parameterName;
			public AttributesProvider(string[] commandsWithParameter, string parameterName) {
				this.commandsWithParameter = commandsWithParameter;
				this.parameterName = parameterName;
			}
			IEnumerable<Attribute> IAttributesProvider.GetAttributes(string propertyName) {
				if(commandsWithParameter.Contains(propertyName))
					yield return new CommandParameterAttribute(parameterName);
			}
		}
		internal static CommandInfo[] GetCommandInfo(Type type, PropertyDescriptor commandParameterProperty, ImageType imageType) {
			string[] commandsWithParameter = GetProperties(type)
				.Where(x => ViewModelMetadataSource.IsCommandProperty(x.PropertyType))
				.Where(x => commandParameterProperty != null && x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().First() == commandParameterProperty.PropertyType)
				.Select(x => x.Name)
				.ToArray();
			return GetCommandInfo(type, imageType, new AttributesProvider(commandsWithParameter, commandParameterProperty.With(x => x.Name)));
		}
		static CommandInfo[] GetCommandInfo(Type type, ImageType imageType, AttributesProvider attributesProvider) {
			CommandInfoGenerator generator = new CommandInfoGenerator(imageType);
			ViewModelMetadataSource.GenerateMetadata(ViewModelMetadataSource.GetPropertiesByType(type), new SingleGroupGenerator(generator), ViewModelMetadataOptions.ForScaffolding(attributesProvider));
			return generator.GetCommands();
		}
	}
}
