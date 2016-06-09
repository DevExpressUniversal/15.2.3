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

extern alias Platform;
using System.Linq;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Policies;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
#if SILVERLIGHT
using DependencyObject = Platform::System.Windows.DependencyObject;
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Xpf.Design {
	public static class DesignHelper {
#if SILVERLIGHT
		public static string GetPropertyName(Platform::System.Windows.DependencyProperty property) {
			return Platform::DevExpress.Xpf.Utils.DependencyPropertyExtensions.GetName(property);
		}
		public static Platform::System.Windows.Point ConvertToPlatformPoint(Point p) {
			return new Platform::System.Windows.Point(p.X, p.Y);
		}
		public static Point ConvertFromPlatformPoint(Platform::System.Windows.Point p) {
			return new Point(p.X, p.Y);
		}
#else
		public static string GetPropertyName(DependencyProperty property) {
			return property.Name;
		}
		public static Point ConvertToPlatformPoint(Point p) {
			return p;
		}
		public static Point ConvertFromPlatformPoint(Point p) {
			return p;
		}
#endif
	}
	public class InitializerHelper {
		static object addItemLocker = new Object();
		public static void Initialize(ModelItem item) {
			try {
				InitializeCore(item);
			}
			catch {
			}
		}
#if SL
		static void InitializeCore(ModelItem item) { }
#else
		static void InitializeCore(ModelItem item) {
		}
		public static void AddVBLicensedItem(IModelItem modelItem) {
			try {
				int majorVersion = Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
				if(majorVersion != 11 && majorVersion != 12) return;
				XpfModelItem runtimeModelItem = modelItem as XpfModelItem;
				if (runtimeModelItem == null) return;
				ModelItem item = runtimeModelItem.Value;
				if(item == null) return;
				if (item.ItemType.GetCustomAttributes(typeof(LicenseProviderAttribute), false).Count() == 0) return;
				if(item.GetType().FullName != "Microsoft.Expression.DesignSurface.ViewModel.Extensibility.SceneNodeModelItem") return;
				Type defaultTypeInstantiator = item.GetType().Assembly.GetType("Microsoft.Expression.DesignSurface.Tools.DefaultTypeInstantiator");
				if(defaultTypeInstantiator == null) return;
				object licenseManagerLock = GetFieldValue<object>(defaultTypeInstantiator, "licenseManagerLock");
				object rootNode = GetPropertyValue<object>(item, "SceneNode");
				if(rootNode == null) return;
				object viewModel = GetPropertyValue<object>(rootNode, "ViewModel");
				object projectContext = GetPropertyValue<object>(rootNode, "ProjectContext");
				if(!IsVBProject(projectContext)) return;
				if(licenseManagerLock == null)
					licenseManagerLock = addItemLocker;
				LicenseManager.LockContext(licenseManagerLock);
				try {
					AddVBLicensedItemCore(projectContext, item.ItemType.FullName, item.ItemType.Assembly.FullName);
				} finally {
					LicenseManager.UnlockContext(licenseManagerLock);
				}
			} catch { }
		}
		static void AddItem(object projectContext, string targetFolder, string sourcePath) {
			Type creationInfoType = GetHostDocumentCreationInfoType();
			object creationInfo = Activator.CreateInstance(creationInfoType);
			SetPropertyValue<string>(creationInfo, "SourcePath", sourcePath);
			SetPropertyValue<string>(creationInfo, "TargetFolder", targetFolder);
			InvokeMethod<object>(projectContext, "AddItem", creationInfo);
		}
		static Type TryGetType(string typeName, string assemblyStrongName) {
			Type resolvedType = null;
			try {
				resolvedType = Type.GetType(typeName + ", " + assemblyStrongName);
			} catch(Exception) { }
			return resolvedType;
		}
		static bool IsVBProject(object projectContext) {
			return GetPropertyValue<bool>(projectContext, "LanguageIsCaseInsensitive");
		}
		static T InvokeMethod<T>(object source, string name, params object[] parameters) {
			MethodInfo methodInfo = GetMethod(source, name);
			return (T)methodInfo.Invoke(source, parameters);
		}
		static MethodInfo GetMethod(object source, string name) {
			if(source == null) return null;
			return source.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		}
		static T GetPropertyValue<T>(object source, string name) {
			if(source == null) return default(T);
			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(pi == null) return default(T);
			return (T)pi.GetValue(source, null);
		}
		static void SetPropertyValue<T>(object source, string name, T value) {
			if(source == null) return;
			PropertyInfo pi = source.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(pi == null) return;
			pi.SetValue(source, value, null);
		}
		static T GetFieldValue<T>(object source, string name) {
			if(source == null) return default(T);
			Type type = null;
			if(source is Type)
				type = (Type)source;
			else
				type = source.GetType();
			FieldInfo pi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			if(pi == null) return default(T);
			return (T)pi.GetValue(source);
		}
		static bool IsLicensed(Type type) {
			object[] array = null;
			try {
				array = type.GetCustomAttributes(true);
			}
			catch {
			}
			return array != null && array.OfType<LicenseProviderAttribute>().Any<LicenseProviderAttribute>();
		}
		static void AddVBLicensedItemCore(object projectContext, string typeName, string assemblyName) {
			List<string> list = new List<string>();
			string text = typeName + ", " + assemblyName;
			string text2 = System.IO.Path.Combine(GetProjectRoot(projectContext), "My Project\\");
			string text3 = System.IO.Path.Combine(text2, "Licenses.licx");
			try {
				Type accessHelperType = GetAccessHelper();
				object accessService = accessHelperType.GetProperty("AccessService").GetValue(null, null);
				bool fileExist = InvokeMethod<bool>(accessService, "FileExists", text3);
				if(!fileExist) {
					if(!InvokeMethod<bool>(accessService, "DirectoryExists", text2)) {
						InvokeMethod<bool>(accessService, "DirectoryCreateDirectory", text2);
					}
				} else {
					using(StreamReader streamReader = InvokeMethod<StreamReader>(accessService, "FileOpenText", text3)) {
						while(!streamReader.EndOfStream) {
							list.Add(streamReader.ReadLine());
						}
					}
				}
				if(!list.Contains(text)) {
					MethodInfo fileOpenInfo = accessService.GetType().GetMethod("FileOpen", new Type[] { typeof(string), typeof(FileMode), typeof(FileAccess) });
					using(FileStream fileStream = (FileStream)fileOpenInfo.Invoke(accessService, new object[] { text3,  FileMode.OpenOrCreate, FileAccess.ReadWrite})) {
						MethodInfo findItemMethod = projectContext.GetType().GetMethod("FindItem", new Type[] { typeof(string) });
						var findedItem = findItemMethod.Invoke(projectContext, new object[] { text3 });
						if(findedItem == null) {
							AddItem(projectContext, text2, text3);
						}
						fileStream.Seek(0L, SeekOrigin.End);
						using(StreamWriter streamWriter = new StreamWriter(fileStream)) {
							streamWriter.WriteLine(text);
						}
					}
				}
			} catch(IOException) {
			} catch(UnauthorizedAccessException) {
			}
		}
		static Type GetAccessHelper() {
			Type type = Type.GetType(string.Format("Microsoft.Expression.Utility.IO.AccessHelper, Microsoft.Expression.Utility, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", GetVSMajorVersion()));
			return type;
		}
		static Type GetHostDocumentCreationInfoType() {
			Type type = Type.GetType(string.Format("Microsoft.Expression.DesignHost.HostDocumentCreationInfo, Microsoft.Expression.DesignHost, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", GetVSMajorVersion()));
			return type;
		}
		static int GetVSMajorVersion() {
			return Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
		}
		static string GetProjectRoot(object projectContext) {
			object projectRoot = GetPropertyValue<object>(projectContext, "ProjectRoot");
			if(projectRoot is string)
				return (string)projectRoot;
			if(projectRoot.GetType().FullName == "Microsoft.Expression.Utility.IO.ResolvedPath")
				return GetPropertyValue<string>(projectRoot, "Path");
			return null;
		}		
#endif
	}
}
