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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Win.Templates.Bars;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates {
	public class XafComponentResourceManager : System.ComponentModel.ComponentResourceManager {
		private XafResourceManager xafResourceManager;
		public XafComponentResourceManager(Type controlType) : base(controlType) { 
			FrameTemplateLocalizer localizer = FrameTemplateLocalizer.GetActiveLocalizer(controlType);
			if(localizer != null) {
				xafResourceManager = new XafResourceManager(localizer.XafResourceManagerParameters);
			}
		}
		public override System.Resources.ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents) {
			if(xafResourceManager != null) {
				return xafResourceManager.GetResourceSet(culture, createIfNotExists, tryParents);
			} else {
				return base.GetResourceSet(culture, createIfNotExists, tryParents);
			}
		}
	}
	[DisplayName("MainForm Template")]
	public class MainFormTemplateLocalizer : FrameTemplateLocalizer<MainForm> { }
	[DisplayName("DetailViewForm Template")]
	public class DetailViewFormTemplateLocalizer : FrameTemplateLocalizer<DetailViewForm> { }
	[DisplayName("MainFormV2 Template")]
	public class MainFormV2TemplateLocalizer : FrameTemplateLocalizer<MainFormV2> { }
	[DisplayName("DetailFormV2 Template")]
	public class DetailFormV2TemplateLocalizer : FrameTemplateLocalizer<DetailFormV2> { }
	[DisplayName("MainRibbonFormV2 Template")]
	public class MainRibbonFormV2TemplateLocalizer : FrameTemplateLocalizer<MainRibbonFormV2> { }
	[DisplayName("DetailRibbonFormV2 Template")]
	public class DetailRibbonFormV2TemplateLocalizer : FrameTemplateLocalizer<DetailRibbonFormV2> { }
	[DisplayName("NestedFrameTemplate Template")]
	public class NestedFrameTemplateLocalizer : FrameTemplateLocalizer<NestedFrameTemplate> { }
	[DisplayName("NestedFrameTemplateV2 Template")]
	public class NestedFrameTemplateV2Localizer : FrameTemplateLocalizer<NestedFrameTemplateV2> { }
	[DisplayName("LookupControlTemplate Template")]
	public class LookupControlTemplateLocalizer : FrameTemplateLocalizer<LookupControlTemplate> { }
	[DisplayName("PopupForm Template")]
	public class PopupFormTemplateLocalizer : FrameTemplateLocalizer<PopupForm> { }
	public abstract class FrameTemplateLocalizer<T> : FrameTemplateLocalizer {
		public const string FrameTemplatesLocalizationGroupName = "FrameTemplates";
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public override void Activate() {
			InitLocalizer(typeof(T), this);
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			if (xafResourceManagerParameters == null) {
				xafResourceManagerParameters = new XafResourceManagerParameters(
					new string[] { FrameTemplatesLocalizationGroupName, typeof(T).FullName },
					typeof(T).FullName,
					"",
					typeof(T).Assembly,
					ResourceItemNameFilter);
			}
			return xafResourceManagerParameters;
		}
		protected bool ResourceItemNameFilter(string name) {
			return !name.StartsWith("$");
		}
	}
	public abstract class FrameTemplateLocalizer : XafResourceLocalizer {
		private static Dictionary<Type, FrameTemplateLocalizer> frameTemplateLocalizers = new Dictionary<Type,FrameTemplateLocalizer>();
		protected void InitLocalizer(Type frameTemplateType, FrameTemplateLocalizer localizer) {
			frameTemplateLocalizers[frameTemplateType] = localizer;
		}
		public static FrameTemplateLocalizer GetActiveLocalizer(Type frameTemplateType) {
			FrameTemplateLocalizer result = null;
			frameTemplateLocalizers.TryGetValue(frameTemplateType, out result);
			return result;
		}
	}
	public class TemplateLocalizer : IDisposable {
		private Stack<Type> currentlyProcessingTypes = new Stack<Type>();
		private List<string> excludedPropertyNames = DefaultExcludedPropertyNames;
		private static Dictionary<Type, IList<string>> cachedPaths = new Dictionary<Type, IList<string>>();
		private object container;
		private LocalizableMemberInfoList localizableMemberInfos;
		protected IList<string> GetLocalizablePaths(Type containerType) {
			IList<string> result;
			if(!cachedPaths.TryGetValue(containerType, out result)) {
				List<string> paths = new List<string>();
				CollectLocalizablePathsCore(containerType, "", paths);
				result = paths;
				cachedPaths.Add(containerType, paths);
			}
			return result;
		}
		protected virtual void CollectLocalizablePathsCore(Type type, string currentPath, List<string> paths) {
			if((!typeof(Control).IsAssignableFrom(type) && !typeof(Component).IsAssignableFrom(type)) || currentlyProcessingTypes.Contains(type)) return;
			currentlyProcessingTypes.Push(type);
			if(currentlyProcessingTypes.Count <= 2) {
				BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
				if(currentlyProcessingTypes.Count < 2) {
					flags |= BindingFlags.NonPublic;
				}
				FieldInfo[] fields = type.GetFields(flags);
				foreach(FieldInfo fieldInfo in fields) {
					string newCurrentPath = GetPath(currentPath, fieldInfo.Name);
					CollectLocalizablePathsCore(fieldInfo.FieldType, newCurrentPath, paths);
				}
			}
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach(PropertyInfo propertyInfo in properties) {
				string newCurrentPath = GetPath(currentPath, propertyInfo.Name);
				object[] attrs = propertyInfo.GetCustomAttributes(typeof(LocalizableAttribute), true);
				if(attrs.Length > 0 && propertyInfo.PropertyType == typeof(string) && !excludedPropertyNames.Contains(propertyInfo.Name)) {
					if(!paths.Contains(newCurrentPath) && newCurrentPath.IndexOf('.') != -1) {
						paths.Add(newCurrentPath);
					}
				}
			}
			if(type.BaseType != null) {
				CollectLocalizablePathsCore(type.BaseType, currentPath, paths);
			}
			currentlyProcessingTypes.Pop();
		}
		protected internal LocalizableMemberInfoList GetValuesByPaths(object control, IList<string> paths) {
			LocalizableMemberInfoList result = new LocalizableMemberInfoList();
			foreach(string path in paths) {
				result.Add(new LocalizableMemberInfo(path, GetValueByPath(control, path)));
			}
			return result;
		}
		protected internal string GetValueByPath(string path) {
			return GetValueByPath(container, path);
		}
		protected internal string GetValueByPath(object control, string path) {
			object targetControl = control;
			MemberInfo member = GetMember(control, path, out targetControl);
			if(member != null && targetControl != null) {
				return (string)((PropertyInfo)member).GetValue(targetControl, null);
			}
			return string.Empty;
		}
		protected internal MemberInfo GetMember(object control, string path, out object targetControl) {
			targetControl = control;
			MemberInfo member = GetMemberByPath(path, control.GetType());
			if(member != null) {
				string[] splitedPath = path.Split('.');
				if(splitedPath.Length > 1) {
					MemberInfo info = GetMemberByPath(splitedPath[0], control.GetType());
					if(info is FieldInfo) {
						targetControl = ((FieldInfo)info).GetValue(control);
					}
				}
			}
			return member;
		}
		protected internal MemberInfo GetMemberByPath(string path, Type type) {
			Type currentType = type;
			MemberInfo member = null;
			foreach(string pathItem in path.Split('.')) {
				member = GetMemberByName(pathItem, currentType);
				if(member != null) {
					if(member.MemberType == MemberTypes.Field) {
						currentType = ((FieldInfo)member).FieldType;
					}
					if(member.MemberType == MemberTypes.Property) {
						currentType = ((PropertyInfo)member).PropertyType;
					}
				}
			}
			return member;
		}
		protected internal MemberInfo GetMemberByName(string pathItem, Type currentType) {
			MemberInfo[] infos = currentType.GetMember(pathItem, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if(infos.Length > 0) {
				return infos[0];
			}
			return null;
		}
		protected internal string GetPath(string currentPath, string fieldName) {
			return currentPath + (string.IsNullOrEmpty(currentPath) ? "" : ".") + fieldName;
		}
		public void SetValueByPath(string path, string value) {
			object targetControl = container;
			MemberInfo member = GetMember(container, path, out targetControl);
			if(member != null && targetControl != null) {
				((PropertyInfo)member).SetValue(targetControl, value, null);
			}
		}
		public List<string> ExcludedPropertyNames {
			get { return excludedPropertyNames; }
			set {
				if(excludedPropertyNames != value) {
					excludedPropertyNames = value;
				}
			}
		}
		public void Init() {
			IList<string> paths = GetLocalizablePaths(container.GetType());
			localizableMemberInfos = GetValuesByPaths(container, paths);
		}
		public TemplateLocalizer(object container) {
			this.container = container;
		}
		public void Dispose() {
			if(localizableMemberInfos != null) {
				localizableMemberInfos.Clear();
				localizableMemberInfos = null;
			}
			if(currentlyProcessingTypes != null) {
				currentlyProcessingTypes.Clear();
				currentlyProcessingTypes = null;
			}
			if(excludedPropertyNames != null) {
				excludedPropertyNames.Clear();
				excludedPropertyNames = null;
			}
			if(container != null) {
				container = null;
			}
		}
		public LocalizableMemberInfoList LocalizableMemberInfos {
			get { return localizableMemberInfos; }
		}
		public static List<string> DefaultExcludedPropertyNames {
			get { return new List<string>(new string[] { "AccessibleName", "AccessibleDescription", "ImageKey", "Hint", "Description", "TabText" }); }
		}
#if DebugTest
		public static void DebugTest_ClearCache() {
			cachedPaths.Clear();
		}
#endif
	}
	public class LocalizableMemberInfo {
		private string memberPath;
		private string defaultValue;
		public LocalizableMemberInfo(string memberPath, string defaultValue) {
			this.memberPath = memberPath;
			this.defaultValue = defaultValue;
		}
		public string MemberPath {
			get { return memberPath; }
			set { memberPath = value; }
		}
		public string DefaultValue {
			get { return defaultValue; }
			set { defaultValue = value; }
		}
	}
	public class LocalizableMemberInfoList : List<LocalizableMemberInfo> {
		public LocalizableMemberInfoList() : base() { }
		public LocalizableMemberInfo this[string memberPath] {
			get {
				foreach(LocalizableMemberInfo info in this) {
					if(info.MemberPath == memberPath) {
						return info;
					}
				}
				return null;
			}
		}
	}
}
