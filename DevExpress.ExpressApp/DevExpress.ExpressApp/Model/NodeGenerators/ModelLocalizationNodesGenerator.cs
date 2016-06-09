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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelOptionsNodesGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) { }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class TemplatesModelNodeGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) { }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelLocalizationNodesGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			ResourceLocalizationNodeGeneratorHelper.GenerateRootGroups(node);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelLocalizationGroupGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			ResourceLocalizationNodeGeneratorHelper.GenerateGroupsAndItems(node);
		}
	}
	static class ResourceLocalizationNodeGeneratorHelper {
		private static void GenerateItems(IModelLocalizationGroup localizationNode, IXafResourceManagerParameters parameters) {
			if(localizationNode == null) {
				throw new ArgumentNullException("localizationNode");
			}
			if(parameters == null) {
				throw new ArgumentNullException("parameters");
			}
			List<CultureInfo> culturesToGenerate = new List<CultureInfo>();
			culturesToGenerate.Add(new CultureInfo(""));
			IModelApplicationServices modeApplicationServices = GetMasterLayer(localizationNode.Application);
			for(int counter = 0; counter < modeApplicationServices.AspectCount; counter++) {
				string aspect = modeApplicationServices.GetAspect(counter);
				if(aspect != CaptionHelper.DefaultLanguage) {
					CultureInfo culture = new CultureInfo(aspect);
					if(!culturesToGenerate.Contains(culture)) {
						culturesToGenerate.Add(culture);
					}
				}
			}
			foreach(CultureInfo culture in culturesToGenerate) {
				GenerateItemsForCulture(localizationNode, parameters, culture);
			}
		}
		private static ModelApplicationBase GetMasterLayer(IModelApplication node) {
			ModelApplicationBase modeApplicationServices = (ModelApplicationBase)node;
			ModelNode master = modeApplicationServices.Master == null ? modeApplicationServices : modeApplicationServices.Master;
			return (ModelApplicationBase)master;
		}
		private static void GenerateItemsForCulture(IModelLocalizationGroup localizationGroupNode, IXafResourceManagerParameters parameters, CultureInfo culture) {
			XafResourceManager resourceManager = new XafResourceManager(parameters);
			IEnumerable<DictionaryEntry> resourceSetDictionary = resourceManager.GetFilteredResourceSet(culture, true, true);
			string aspect = CaptionHelper.GetAspectByCultureInfo(culture);
			int aspectIndex = 0;
			ModelApplicationBase modeApplicationServices = GetMasterLayer(localizationGroupNode.Application); ;
			if(!modeApplicationServices.Aspects.Contains(aspect) && aspect != CaptionHelper.DefaultLanguage) {
				modeApplicationServices.AddAspect(aspect);
			}
			aspectIndex = modeApplicationServices.GetAspectIndex(aspect);
			foreach(DictionaryEntry entry in resourceSetDictionary) {
				string itemName = entry.Key.ToString();
				CultureInfo sourceCulture = GetSourceCulture(itemName, resourceManager, culture);
				if(sourceCulture != culture) {
					continue;
				}
				if(!string.IsNullOrEmpty(parameters.ResourceItemPrefix)) {
					itemName = itemName.Replace(parameters.ResourceItemPrefix, "");
				}
				if(!string.IsNullOrEmpty(itemName)) {
					string itemValue = entry.Value as string;
					IModelLocalizationItem itemNode = (IModelLocalizationItem)localizationGroupNode[itemName];
					if(itemNode == null) {
						itemNode = localizationGroupNode.AddNode<IModelLocalizationItem>(itemName);
					}
					string currentAspect = modeApplicationServices.CurrentAspect;
					modeApplicationServices.SetCurrentAspect(aspect);
					itemNode.Value = itemValue == null ? "" : itemValue;
					modeApplicationServices.SetCurrentAspect(currentAspect);
				}
			}
		}
		private static CultureInfo GetSourceCulture(string itemName, ResourceManager resourceManager, CultureInfo culture) {
			if(string.IsNullOrEmpty(culture.Name)) {
				return culture;
			}
			ResourceSet resourceSet = resourceManager.GetResourceSet(culture, true, true);
			string value = resourceSet.GetString(itemName);
			CultureInfo parentCulture = culture.Parent;
			ResourceSet parentResourceSet = resourceManager.GetResourceSet(parentCulture, true, true);
			string parentValue = parentResourceSet.GetString(itemName);
			if(value == parentValue) {
				return GetSourceCulture(itemName, resourceManager, parentCulture);
			}
			return culture;
		}
		private static void GenerateEnumsLocalizationItems(ModelNode node) {
			if(node.Id == "Enums") {
				List<Type> generatedEnums = new List<Type>();
				foreach(Type type in ((IModelSources)node.Application).BOModelTypes) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
					foreach(IMemberInfo memberInfo in typeInfo.Members) {
						if(memberInfo.IsVisible) {
							Type enumType = EnumDescriptor.GetEnumType(memberInfo.MemberType);
							if((enumType != null) && !generatedEnums.Contains(enumType)) {
								CompoundNameConvertStyle compoundNameConvertStyle = ResourceLocalizationNodeGenerator.GetCompoundNameConvertStyle(enumType);
								EnumDescriptor.GenerateDefaultCaptions(node.AddNode<IModelLocalizationGroup>(enumType.FullName), enumType, compoundNameConvertStyle);
								generatedEnums.Add(enumType);
							}
						}
					}
				}
				generatedEnums.Clear();
				generatedEnums = null;
			}
		}
		private static string GetLocalizationGroupNodePath(ModelNode node) {
			string result = null;
			ModelNode parent = node;
			while(parent.Parent != null) {
				result = result == null ? parent.Id : parent.Id + "\\" + result;
				parent = parent.Parent;
				if(parent is IModelLocalization) {
					break;
				}
			}
			return result;
		}
		private static void GenerateEnumsLocalizationGroup(ModelNode node) {
			node.AddNode<IModelLocalizationGroup>("Enums");
		}
		internal static void GenerateRootGroups(ModelNode node) {
			GenerateEnumsLocalizationGroup(node);
			List<string> createdGroups = new List<string>();
			foreach(IXafResourceLocalizer localizer in ((IModelSources)node.Application).Localizers) {
				string[] pathItems = localizer.XafResourceManagerParameters.LocalizationGroupPath;
				if(!createdGroups.Contains(pathItems[0])) {
					createdGroups.Add(pathItems[0]);
					node.Application.Localization.AddNode<IModelLocalizationGroup>(pathItems[0]);
				}
			}
			IModelLocalizationGroup securityLocalizationGroup = node.AddNode<IModelLocalizationGroup>(PermissionTargetBusinessClassListConverter.SecurityLocalizationGroupName);
		}
		internal static void GenerateGroupsAndItems(ModelNode node) {
			GenerateEnumsLocalizationItems(node);
			List<string> createdGroups = new List<string>();
			string targetNodePath = GetLocalizationGroupNodePath(node);
			foreach(IXafResourceLocalizer localizer in ((IModelSources)node.Application).Localizers) {
				string groupPath = string.Join("\\", localizer.XafResourceManagerParameters.LocalizationGroupPath);
				if(groupPath.StartsWith(targetNodePath)) {
					string addPath = groupPath.Substring(targetNodePath.Length);
					if(string.IsNullOrEmpty(addPath)) {
						IModelApplication modelApplication = localizer.XafResourceManagerParameters.ModelApplication;
						localizer.XafResourceManagerParameters.ModelApplication = null;
						GenerateItems((IModelLocalizationGroup)node, localizer.XafResourceManagerParameters);
						localizer.XafResourceManagerParameters.ModelApplication = modelApplication;
					}
					else {
						string[] pathItems = addPath.Replace('\\', '/').Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
						if(!createdGroups.Contains(pathItems[0])) {
							createdGroups.Add(pathItems[0]);
							node.AddNode<IModelLocalizationGroup>(pathItems[0]);
						}
					}
				}
			}
		}
	}
	public class ResourceLocalizationNodeGenerator { 
		internal static CompoundNameConvertStyle GetCompoundNameConvertStyle(Type enumType) {
			if(CustomizeCompoundNameConvertStyle != null) {
				CustomizeCompoundNameConvertStyleEventArgs arg = new CustomizeCompoundNameConvertStyleEventArgs(enumType);
				CustomizeCompoundNameConvertStyle(null, arg);
				return arg.CompoundNameConvertStyle;
			}
			else {
				return CompoundNameConvertStyle.SplitAndCapitalization;
			}
		}
		[Obsolete("For internal use only.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void GenerateRootGroups(ModelNode node) {
			ResourceLocalizationNodeGeneratorHelper.GenerateRootGroups(node);
		}
		[Obsolete("For internal use only.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void GenerateGroupsAndItems(ModelNode node) {
			ResourceLocalizationNodeGeneratorHelper.GenerateGroupsAndItems(node);
		}
		public static event EventHandler<CustomizeCompoundNameConvertStyleEventArgs> CustomizeCompoundNameConvertStyle; 
	}
	public class CustomizeCompoundNameConvertStyleEventArgs : EventArgs {
		private Type enumType;
		private CompoundNameConvertStyle compoundNameConvertStyle = CompoundNameConvertStyle.SplitAndCapitalization;
		public CustomizeCompoundNameConvertStyleEventArgs(Type enumType) {
			this.enumType = enumType;
		}
		public Type EnumType {
			get { return enumType; }
		}
		public CompoundNameConvertStyle CompoundNameConvertStyle {
			get { return compoundNameConvertStyle; }
			set { compoundNameConvertStyle = value; }
		}
	}
}
