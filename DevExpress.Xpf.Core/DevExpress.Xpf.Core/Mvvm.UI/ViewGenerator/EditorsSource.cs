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

using DevExpress.Data.Browsing;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core;
using System.ComponentModel.DataAnnotations;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Filtering.Internal;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public interface ICommandAttributesApplier {
		void SetPropertyName(string propertyName);
		void SetCaption(string caption);
		void SetHint(string hint);
		void SetImageUri(string imageName);
		void SetLargeImageUri(string imageName);
		void SetParameter(string parameterPropertyName);
	}
	public interface ICommandGroupsGenerator {
		ICommandSubGroupsGenerator CreateGroup(string groupName);
	}
	public interface ICommandSubGroupsGenerator {
		ICommandsGenerator CreateSubGroup(string groupName);
	}
	public class SingleGroupGenerator : ICommandGroupsGenerator {
		readonly ICommandSubGroupsGenerator gen;
		public SingleGroupGenerator(ICommandsGenerator gen) {
			this.gen = new SingleSubGroupGenerator(gen);
		}
		ICommandSubGroupsGenerator ICommandGroupsGenerator.CreateGroup(string groupName) {
			return gen;
		}
	}
	public class SingleSubGroupGenerator : ICommandSubGroupsGenerator {
		readonly ICommandsGenerator gen;
		public SingleSubGroupGenerator(ICommandsGenerator gen) {
			this.gen = gen;
		}
		ICommandsGenerator ICommandSubGroupsGenerator.CreateSubGroup(string groupName) {
			return gen;
		}
	}
	public interface ICommandsGenerator {
		void GenerateCommand(IEdmPropertyInfo property);
	}
	public class ViewModelMetadataOptions {
		public static ViewModelMetadataOptions ForContextMenuRuntime() {
			return new ViewModelMetadataOptions(false, null, LayoutType.ContextMenu);
		}
		public static ViewModelMetadataOptions ForRuntime() {
			return new ViewModelMetadataOptions(false, null, LayoutType.ToolBar);
		}
		public static ViewModelMetadataOptions ForScaffolding(IAttributesProvider attributesProvider = null) {
			return new ViewModelMetadataOptions(true, attributesProvider, LayoutType.ToolBar);
		}
		public IAttributesProvider AttributesProvider { get; private set; }
		public bool Scaffolding { get; private set; }
		public LayoutType LayoutType { get; private set; }
		ViewModelMetadataOptions(bool scaffolding, IAttributesProvider attributesProvider, LayoutType layoutType) {
			this.Scaffolding = scaffolding;
			this.AttributesProvider = attributesProvider;
			this.LayoutType = layoutType;
		}
	}
	public static class ViewModelMetadataSource {
#if !SL
		internal const string PackPrefix = "pack://application:,,,";
#else
		internal const string PackPrefix = "";
#endif
		public const string ImagesImagePrefix = PackPrefix + "/" + AssemblyInfo.SRAssemblyImages + ";component/";
		const string STR_Command = "Command";
		public static IEnumerable<IEdmPropertyInfo> GetProperties(object viewModel) {
			return GetPropertiesCore(TypeDescriptor.GetProperties(viewModel), viewModel.GetType());
		}
		public static IEnumerable<IEdmPropertyInfo> GetPropertiesByType(Type type) {
			return GetPropertiesCore(TypeDescriptor.GetProperties(type), type);
		}
		static IEnumerable<IEdmPropertyInfo> GetPropertiesCore(PropertyDescriptorCollection propertyDescriptors, Type type) {
			IEntityProperties properties = new ReflectionEntityProperties(propertyDescriptors.Cast<PropertyDescriptor>(), type, true);
			return properties.AllProperties;
		}
		public static void GenerateMetadata(IEnumerable<IEdmPropertyInfo> properties, ICommandGroupsGenerator generator, ViewModelMetadataOptions options) {
			var provider = options.AttributesProvider;
			if(options.LayoutType == LayoutType.ContextMenu)
				properties = properties.Where(x => x.Attributes.IsContextMenuItem()).ToArray();
			var groupedProperties = EditorsGeneratorBase.GetFilteredAndSortedProperties(properties, options.Scaffolding, false, options.LayoutType)
				.Select(x => provider == null ? x : x.AddAttributes(provider.GetAttributes(x.Name)))
				.GroupBy(x => x.Attributes.ToolBarPageName());
			foreach(IGrouping<string, IEdmPropertyInfo> group in groupedProperties) {
				var commandsSubGroupGenerator = generator.CreateGroup(group.Key);
				foreach(IGrouping<string, IEdmPropertyInfo> subGroup in group.GroupBy(x => x.Attributes.GetToolBarPageGroupName(options.LayoutType))) {
					var commandsGenerator = commandsSubGroupGenerator.CreateSubGroup(subGroup.Key);
					foreach(IEdmPropertyInfo property in subGroup) {
						ProcessProperty(property, commandsGenerator);
					}
				}
			}
		}
		static void ProcessProperty(IEdmPropertyInfo property, ICommandsGenerator generator) {
			if(IsCommandProperty(property.PropertyType))
				generator.GenerateCommand(property);
		}
		public static bool IsCommandProperty(Type propertyType) {
			return typeof(ICommand).IsAssignableFrom(propertyType);
		}
		public static string GetCaptionFromCommandName(string commandName) {
			return SplitStringHelper.SplitPascalCaseString(GetImageNameFromCommandName(commandName));
		}
		static string GetImageNameFromCommandName(string commandName) {
			string caption = commandName;
			if(caption.EndsWith(STR_Command) && caption != STR_Command)
				caption = caption.Remove(caption.Length - STR_Command.Length, STR_Command.Length);
			return caption;
		}
		public static string GetKnownImageUri(string name, ImageSize imageSize, ImageType imageType) {
			return DXImageHelper.GetFile(name, imageSize, imageType);
		}
		public static void ApplyCommandAttributes(this IEdmPropertyInfo propertyInfo, ICommandAttributesApplier applier, ImageType defaultImageType) {
			applier.SetPropertyName(propertyInfo.Name);
			string caption = propertyInfo.HasDisplayAttribute() ? propertyInfo.DisplayName : GetCaptionFromCommandName(propertyInfo.Name);
			applier.SetCaption(propertyInfo.Attributes.ShortName ?? caption);
			if(!string.IsNullOrEmpty(propertyInfo.Attributes.Description)) {
				applier.SetHint(propertyInfo.Attributes.Description);
			}
			var imageInfo = EnumSourceHelperCore.GetImageInfo(propertyInfo.Attributes.Image(), propertyInfo.Attributes.DXImage(), GetImageNameFromCommandName(propertyInfo.Name), GetKnownImageCallback(defaultImageType));
			if(imageInfo.Item1 != null)
				applier.SetImageUri(imageInfo.Item1);
			if(imageInfo.Item2 != null)
				applier.SetLargeImageUri(imageInfo.Item2);
			if(!string.IsNullOrEmpty(propertyInfo.Attributes.CommandParameterName())) {
				applier.SetParameter(propertyInfo.Attributes.CommandParameterName());
			}
		}
		public static Func<string, bool, string> GetKnownImageCallback(ImageType defaultImageType) {
			return (image, large) => ViewModelMetadataSource.GetKnownImageUri(image, large ? ImageSize.Size32x32 : ImageSize.Size16x16, defaultImageType);
		}
	}
	public static class EditorsSource {
		public static HorizontalAlignment CurrencyValueAlignment = HorizontalAlignment.Right;
		public static double MultilineTextMinHeight = 50;
		public static string PhoneNumberMask = "(000) 000-0000";
		public static readonly Type[] NumericIntegerTypes = {
													  typeof(int), typeof(uint),
													  typeof(short), typeof(ushort),
													  typeof(byte),
													  typeof(long), typeof(ulong),
												  };
		public static readonly Type[] NumericFloatTypes = {
													  typeof(double), typeof(decimal), typeof(float),
												   };
		static readonly string[] DisplayMemberNames = {
														"fullname",
														"lastname",
														"name",
														"title",
														"caption",
														"displaytext",
														"subject",
														"description",
														"text",
														"subj",
														"desc"
												  };
		class LayoutElementFactory : ILayoutElementFactory {
			readonly IGroupGenerator groupGenerator;
			readonly GenerateEditorOptions options;
			readonly Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty;
			public LayoutElementFactory(IGroupGenerator groupGenerator, GenerateEditorOptions options, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty) {
				this.groupGenerator = groupGenerator;
				this.options = options;
				this.getForegnKeyProperty = getForegnKeyProperty;
			}
			void ILayoutElementFactory.CreateGroup(LayoutGroupInfo groupInfo) {
				IGroupGenerator nestedGroupenrator = groupGenerator.CreateNestedGroup(groupInfo.Name, groupInfo.GetView(), groupInfo.GetOrientation());
				GenerateEditorsCore(groupInfo, nestedGroupenrator, options, getForegnKeyProperty);
				nestedGroupenrator.OnAfterGenerateContent();
			}
			void ILayoutElementFactory.CreateItem(IEdmPropertyInfo property) {
				GenerateEditor(property, groupGenerator.EditorsGenerator, getForegnKeyProperty, options.CollectionProperties, options.GuessImageProperties, options.GuessDisplayMembers, options.SkipIEnumerableProperties);
			}
		}
		public static void GenerateEditors(LayoutGroupInfo rootGroupInfo, IEnumerable<IEdmPropertyInfo> properties, IGroupGenerator generator, EditorsGeneratorBase availableItemsGenerator, GenerateEditorOptions options, bool allItemsAreAvailable, bool filterAndSortProperties = true, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty = null, bool usePreFiltering = true) {
			if(filterAndSortProperties) {
				if(usePreFiltering) properties = EditorsGeneratorBase.GetFilteredAndSortedProperties(properties, options.Scaffolding, options.SortColumnsWithNegativeOrder, options.LayoutType);
				if(generator != null && generator.EditorsGenerator != null)
					properties = generator.EditorsGenerator.FilterProperties(properties);
			}
			foreach(IEdmPropertyInfo property in properties.Where(x => !IsAvailableItem(x, allItemsAreAvailable, availableItemsGenerator))) {
				LayoutGroupInfo groupInfo = rootGroupInfo.GetGroupInfo(property.Attributes.GetGroupName(options.LayoutType));
				groupInfo.Children.Add(new LayoutItemInfo(property));
			}
			if(rootGroupInfo.Children.Count > 0) {
				GenerateEditorsCore(rootGroupInfo, generator, options, getForegnKeyProperty);
			}
			GenerateEditors(properties.Where(x => IsAvailableItem(x, allItemsAreAvailable, availableItemsGenerator)), availableItemsGenerator, options);
		}
		static bool IsAvailableItem(IEdmPropertyInfo property, bool isAvailable, EditorsGeneratorBase availableItemsGenerator) {
			return (availableItemsGenerator != null) && (isAvailable || property.Attributes.Hidden());
		}
		static void GenerateEditorsCore(LayoutGroupInfo groupInfo, IGroupGenerator generator, GenerateEditorOptions options, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty) {
			generator.ApplyGroupInfo(groupInfo.Name, groupInfo.GetView(), groupInfo.GetOrientation());
			LayoutElementFactory factory = new LayoutElementFactory(generator, options, getForegnKeyProperty);
			foreach(ILayoutElementGenerator child in groupInfo.Children) {
				child.CreateElement(factory);
			}
		}
		public static void GenerateEditors(IEntityProperties typeInfo, EditorsGeneratorBase generator, GenerateEditorOptions options, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty = null) {
			GenerateEditors(typeInfo.AllProperties, generator, options, getForegnKeyProperty);
		}
		public static void GenerateEditors(IEnumerable<IEdmPropertyInfo> properties, EditorsGeneratorBase generator, GenerateEditorOptions options, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty = null) {
			properties = EditorsGeneratorBase.GetFilteredAndSortedProperties(properties, options.Scaffolding, options.SortColumnsWithNegativeOrder, options.LayoutType);
			if(generator != null) properties = generator.FilterProperties(properties);
			foreach(IEdmPropertyInfo property in properties)
				GenerateEditor(property, generator, getForegnKeyProperty, options.CollectionProperties, options.GuessImageProperties, options.GuessDisplayMembers, options.SkipIEnumerableProperties);
		}
		public static void GenerateEditor(IEdmPropertyInfo property, EditorsGeneratorBase generator, Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty, IEnumerable<TypeNamePropertyPair> collectionProperties = null, bool guessImageProperties = false, bool guessDisplayMembers = false, bool skipIEnumerableProperties = false) {
			EditorsGeneratorSelector.GenerateEditor(property, generator, getForegnKeyProperty, collectionProperties, guessImageProperties, guessDisplayMembers, skipIEnumerableProperties);
		}
		public static string GetDisplayMemberPropertyName(Type type) {
			var displayColumn = type
				.GetCustomAttributes(true)
				.OfType<DisplayColumnAttribute>()
				.FirstOrDefault()
				.With(a => a.DisplayColumn as string);
			if (displayColumn != null) {
				return displayColumn;
			}
			var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>();
			PropertyDescriptor exactProperty = DisplayMemberNames.Select(d => properties.Where(x => x.PropertyType == typeof(string)).FirstOrDefault(x => x.Name.ToLowerInvariant().Contains(d))).Where(p => p != null).FirstOrDefault();
			return (exactProperty ?? properties
				.FirstOrDefault(x => x.PropertyType == typeof(string)).Return(x => x, () => properties
				.FirstOrDefault(x => Type.GetTypeCode(x.PropertyType) != TypeCode.Object).Return(x => x, () => properties
				.FirstOrDefault())))
				.Return(x => x.Name, () => string.Empty);
		}
		public static string GetImagePropertyName(IEntityTypeInfo type) {
			return type.GetProperties().Where(p => IsImagePropertyMostLikely(p)).Select(p => p.Name).FirstOrDefault();
		}
		internal static bool IsImagePropertyName(string name) {
			string[] imageNames = new[] { "image", "photo", "avatar", "picture", "icon", "glyph" };
			name = name.ToLower();
			return imageNames.Any(x => name.Contains(x));
		}
		static bool IsImagePropertyMostLikely(IEdmPropertyInfo property) {
			PropertyDataType dataType = property.Attributes.PropertyDataType();
			if(property.PropertyType == typeof(string) && dataType == PropertyDataType.ImageUrl) return true;
			if(property.PropertyType == typeof(byte[]) && IsImagePropertyName(property.Name)) return true;
			return false;
		}
	}
	public class ForeignKeyInfo {
		public ForeignKeyInfo(string foreignKeyPropertyName, string primaryKeyPropertyName) {
			this.ForeignKeyPropertyName = foreignKeyPropertyName;
			this.PrimaryKeyPropertyName = primaryKeyPropertyName;
		}
		public string ForeignKeyPropertyName { get; private set; }
		public string PrimaryKeyPropertyName { get; private set; }
	}
}
