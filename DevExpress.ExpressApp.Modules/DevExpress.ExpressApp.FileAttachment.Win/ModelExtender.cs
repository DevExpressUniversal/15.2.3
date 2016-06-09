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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Win {	
	public interface IModelOptionsFileAttachments : IModelNode {
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelOptionsFileAttachmentsAttachments")]
#endif
		IModelOptionsFileAttachment Attachments { get; }
	}
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("WinIModelOptionsFileAttachment")]
#endif
	public interface IModelOptionsFileAttachment : IModelNode {
		[
#if !SL
	DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelOptionsFileAttachmentDefaultDirectory"),
#endif
 Category("Data")]
		string DefaultDirectory { get; set; }
	}
	[DomainLogic(typeof(IModelOptionsFileAttachment))]
	public static class ModelOptionsFileAttachmentLogic {
		public static string Get_DefaultDirectory() {
			return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}
	}
	public class FileTypeFiltersVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			if(node is IModelClass) {
				ITypeInfo classTypeInfo = ((IModelClass)node).TypeInfo;
				if(classTypeInfo != null) {
					if(classTypeInfo.IsListType) {
						Type[] genericArguments = classTypeInfo.Type.GetGenericArguments();
						if(genericArguments.Length == 1) {
							classTypeInfo = XafTypesInfo.Instance.FindTypeInfo(genericArguments[0].FullName); ;
						}
					}
					if(classTypeInfo != null && (classTypeInfo.Implements<IFileData>() || classTypeInfo.FindAttribute<FileAttachmentAttribute>() != null)) {
						return true;
					}
				}
			}
			if((node is IModelMember) && (((IModelMember)node).MemberInfo != null)) {
				return typeof(IFileData).IsAssignableFrom(((IModelMember)node).MemberInfo.MemberType);
			}
			return false;
		}
	}
	public interface IModelCommonFileTypeFilters : IModelNode {
		[ModelBrowsableAttribute(typeof(FileTypeFiltersVisibilityCalculator))]
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelCommonFileTypeFiltersFileTypeFilters")]
#endif
		IModelFileTypeFilters FileTypeFilters { get; }
	}
	[ModelNodesGenerator(typeof(FileTypeFiltersNodesGenerator))]
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("WinIModelFileTypeFilters")]
#endif
	public interface IModelFileTypeFilters : IModelNode, IModelList<IModelFileTypeFilter> {
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelFileTypeFiltersFileTypesFilter")]
#endif
		string FileTypesFilter { get; }
	}
	[DomainLogic(typeof(IModelFileTypeFilters))]
	public static class FileTypeFiltersLogic {
		public static string Get_FileTypesFilter(IModelFileTypeFilters filters) {
			List<string> result = new List<string>(filters.Count);
			foreach(IModelFileTypeFilter filter in filters) {
				List<string> extensions = new List<string>(filter.Extension.Count);
				foreach(IModelFileTypeFilterExtension extension in filter.Extension) {
					extensions.Add(PrepareExtension(extension.Extension));
				}
				extensions.Sort();
				string extensionsList = string.Join("; ", extensions.ToArray());
				result.Add(filter.Caption + " (" + extensionsList + ")|" + extensionsList);
			}
			return string.Join("|", result.ToArray()); ;
		}
		public static string PrepareExtension(string extension) {
			string result = extension.TrimStart('*');
			result = result.TrimStart('.');
			result  = "*." + result;
			return result;
		}
	}
	[DomainLogic(typeof(IModelFileTypeFilter))]
	public static class ModelFileTypeFilterLogic {
		public static string Get_Caption(IModelFileTypeFilter node) {
			return node.Id;
		}
	}
	public class FileTypeFiltersNodesGenerator : ModelNodesGeneratorBase {
		private void AddFileTypeFilters(ModelNode node, IEnumerable<FileTypeFilterAttribute> attributes) {
			foreach(FileTypeFilterAttribute attribute in attributes) {
				IModelFileTypeFilter fileTypeFilter = node.AddNode<IModelFileTypeFilter>(attribute.FilterID);
				fileTypeFilter.Caption = attribute.FilterCaption;
				fileTypeFilter.Index = attribute.Index;
				foreach(string extension in attribute.GetExtensions()) {
					if(fileTypeFilter.Extension == null) {
						fileTypeFilter.AddNode<IModelFileTypeFilterExtensions>();
					}
					IModelFileTypeFilterExtension modelExtension = fileTypeFilter.Extension.AddNode<IModelFileTypeFilterExtension>();
					modelExtension.Extension = extension;
				}
			}
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IModelClass modelClass = node.Parent as IModelClass;
			if(modelClass != null) {
				AddFileTypeFilters(node, modelClass.TypeInfo.FindAttributes<FileTypeFilterAttribute>(false));
			}
			IModelMember modelMember = node.Parent as IModelMember;
			if(modelMember != null) {
				AddFileTypeFilters(node, modelMember.MemberInfo.FindAttributes<FileTypeFilterAttribute>());
			}
		}
	}
	[DisplayProperty("Caption")]
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("WinIModelFileTypeFilter")]
#endif
	public interface IModelFileTypeFilter : IModelNode {
		[
#if !SL
	DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelFileTypeFilterId"),
#endif
 Required]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelFileTypeFilterCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelFileTypeFilterExtension")]
#endif
		IModelFileTypeFilterExtensions Extension { get; }
	}
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("WinIModelFileTypeFilterExtensions")]
#endif
	public interface IModelFileTypeFilterExtensions : IModelNode, IModelList<IModelFileTypeFilterExtension> {
	}
	[DisplayProperty("Extension")]
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("WinIModelFileTypeFilterExtension")]
#endif
	public interface IModelFileTypeFilterExtension : IModelNode {
		[
#if !SL
	DevExpressExpressAppFileAttachmentWinLocalizedDescription("IModelFileTypeFilterExtensionExtension"),
#endif
 Category("Data")]
		string Extension { get; set; }
	}
}
