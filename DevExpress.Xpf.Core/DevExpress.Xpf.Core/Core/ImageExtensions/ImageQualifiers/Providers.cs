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

using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Core {
	public interface IBaseUriQualifier {
		string Name { get; }
		bool IsValidValue(string value);
		int GetAltitude(DependencyObject context, string value, IEnumerable<string> values, out int maxAltitude);
	}
	public interface IUriQualifier : IBaseUriQualifier {
		event EventHandler ActiveValueChanged;
	}
	public interface IBindableUriQualifier : IBaseUriQualifier {
		Binding GetBinding(DependencyObject source);
	}	
	public static class UriQualifierHelper {
		static void RegisterDefaultQualifiers() {
			RegisterQualifier(new InputQualifier());
			RegisterQualifier(new ThemeQualifier());
			RegisterQualifier(new ContrastQualifier());
		}
		#region helpers
		delegate object GetResourceManagerWrapper(object container, Uri uri, out string partName, out bool isContentFile);
		static readonly object ResourceContainer;
		static readonly GetResourceManagerWrapper getResourceManagerWrapper;
		static volatile Func<object, ResourceManager> getResourceManager;
		static readonly object olock = new object();
		static readonly Func<Uri, Package> getResourcePackage;
		public static readonly Dictionary<string, IBaseUriQualifier> registeredQualifiers;
		static readonly Dictionary<object, UriInfoMap> resourceSetDatas;
		static Func<object, object> get_DesignerContext;
		static Func<object, object> get_DocumentViewContext;
		static Func<object, object> get_ActiveView;
		static Func<object, object> get_DocumentPath;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty QualifierInfoProperty;		
		static UriQualifierHelper() {
			registeredQualifiers = new Dictionary<string, IBaseUriQualifier>();
			resourceSetDatas = new Dictionary<object, UriInfoMap>();
			getResourcePackage = ReflectionHelper.CreateInstanceMethodHandler<Func<Uri, Package>>(null, "GetResourcePackage", BindingFlags.Static | BindingFlags.NonPublic, typeof(Application));
			ResourceContainer = getResourcePackage(new Uri("application://"));
			getResourceManagerWrapper = ReflectionHelper.CreateInstanceMethodHandler<GetResourceManagerWrapper>(ResourceContainer, "GetResourceManagerWrapper", BindingFlags.Instance | BindingFlags.NonPublic, ResourceContainer.GetType());
			QualifierInfoProperty = DependencyPropertyManager.RegisterAttached("QualifierInfo", typeof(object), typeof(UriQualifierHelper), new FrameworkPropertyMetadata(null));
			RegisterDefaultQualifiers();
		}		
		internal static object GetQualifierInfo(DependencyObject obj) { return (object)obj.GetValue(QualifierInfoProperty); }
		internal static void SetQualifierInfo(DependencyObject obj, object value) { obj.SetValue(QualifierInfoProperty, value); }
		static void CheckInitializeGetResourceManager(object wrapper) {
			if (getResourceManager == null) {
				lock (olock)
				{
					if (getResourceManager == null) {
						getResourceManager = ReflectionHelper.CreateInstanceMethodHandler<Func<object, ResourceManager>>(wrapper, "get_ResourceManager", BindingFlags.Instance | BindingFlags.NonPublic, wrapper.GetType());
					}
				}
			}
		}  
		static UriInfoMap GetUriMap(Uri baseUri, Uri absoluteUri) {
			if (absoluteUri.Scheme == Uri.UriSchemeFile)
				return GetUriMapForFileScheme(baseUri, absoluteUri);
			else
				return GetUriMapForPackScheme(baseUri, absoluteUri);			
		}
		static UriInfoMap GetUriMapForPackScheme(Uri baseUri, Uri absoluteUri) {
			var manager = GetResourceManager(absoluteUri);
			if (manager == null)
				return null;
			var set = manager.GetResourceSet(CultureInfo.InvariantCulture, false, true);
			if (set == null)
				return null;
			UriInfoMap result;
			if (!resourceSetDatas.TryGetValue(set, out result)) {
				result = new UriInfoMap(new PackResourceCollection(set), absoluteUri);
				resourceSetDatas.Add(set, result);
			}
			return result;
		}
		static UriInfoMap GetUriMapForFileScheme(Uri baseUri, Uri absoluteUri) {
			UriInfoMap result;
			if (!resourceSetDatas.TryGetValue(baseUri, out result)) {
				result = new UriInfoMap(new FileResourceCollection(baseUri, absoluteUri), absoluteUri);
				resourceSetDatas.Add(baseUri, result);
			}
			return result;
		}
		static ResourceManager GetResourceManager(Uri uri) {
			string partName;
			bool isContentFile;
			var wrapper = getResourceManagerWrapper(ResourceContainer, new Uri(uri.AbsolutePath, UriKind.Relative), out partName, out isContentFile);
			CheckInitializeGetResourceManager(wrapper);
			return getResourceManager(wrapper);
		}  
		static Uri GetBaseUri(IServiceProvider serviceProvider) {
			var iuc = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;
			if (iuc != null)
				return iuc.BaseUri;
			var dObj = GetTarget(serviceProvider);
			if (dObj == null)
				return null;
			var result = System.Windows.Navigation.BaseUriHelper.GetBaseUri(dObj);
			result = ValidateBaseUriInDesignMode(dObj, result);
			return result;
		}
		static DependencyObject GetTarget(IServiceProvider serviceProvider) {
			var ipwt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (ipwt == null)
				return null;
			return ipwt.TargetObject as DependencyObject;
		}
		static Uri ValidateBaseUriInDesignMode(DependencyObject dObj, Uri result) {
			if (!DesignerProperties.GetIsInDesignMode(dObj))
				return result;
			if (result.Scheme == Uri.UriSchemeFile)
				return result;
			return TryGetBaseUriInDesignMode(dObj);
		}
		static Uri TryGetBaseUriInDesignMode(DependencyObject node) {
			var docPath = GetDocumentPathInDesignMode(node);
			if (String.IsNullOrEmpty(docPath))
				return null;
			return new Uri(System.IO.Path.GetDirectoryName(docPath) + System.IO.Path.DirectorySeparatorChar.ToString(), UriKind.Absolute);
		}
		static string GetDocumentPathInDesignMode(DependencyObject node) {
			try {
				object current = TreeHelper.GetParent(node, x => Convert.ToString(x).Contains("XamlSceneScrollViewer"));
				if (current == null)
					return null;
				if (get_DesignerContext == null)
					if (!CreatePublicGetter(current, "get_DesignerContext", out get_DesignerContext))
						return null;
				current = get_DesignerContext(current);
				if (current == null)
					return null;
				if (get_DocumentViewContext == null)
					if (!CreatePublicGetter(current, "get_DocumentViewContext", out get_DocumentViewContext))
						return null;
				current = get_DocumentViewContext(current);
				if (current == null)
					return null;
				if (get_ActiveView == null)
					if (!CreatePublicGetter(current, "get_ActiveView", out get_ActiveView))
						return null;
				current = get_ActiveView(current);
				if (current == null)
					return null;
				if (get_DocumentPath == null)
					if (!CreatePublicGetter(current, "get_DocumentPath", out get_DocumentPath))
						return null;
				current = get_DocumentPath(current);
				return Convert.ToString(current);
			} catch {
				return null;
			}
		}
		static bool CreatePublicGetter(object instance, string getterName, out Func<object, object> result) {
			result = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object>>(instance, getterName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, instance.GetType());
			return result != null;
		}
		static object ProvideBindingValue(DependencyObject dObj, IServiceProvider serviceProvider, Uri uri, UriInfoMap data) {
			var uriCandidates = data.GetValues(uri);
			if (uriCandidates.All(x => x.BindableQualifier))
				return ProvideSimpleExpression(dObj, serviceProvider, uri, ()=>data.GetValues(uri));
			return ProvideComplexExpression(serviceProvider, uri, () => data.GetValues(uri));
		}
		static BindingExpressionBase ProvideComplexExpression(IServiceProvider serviceProvider, Uri uri, Func<ICollection<UriInfo>> uriCandidates) {
			return QualifierListener.CreateBinding(serviceProvider, uri, uriCandidates).ProvideValue(serviceProvider) as BindingExpressionBase;
		}
		static BindingExpressionBase ProvideSimpleExpression(DependencyObject dObj, IServiceProvider serviceProvider, Uri uri, Func<ICollection<UriInfo>> uriCandidates) {
			var qualifier = uriCandidates().SelectMany(x => x.Qualifiers).Select(x=>x.Qualifier).OfType<IBindableUriQualifier>().Distinct().If(x => x.Count() == 1).SingleOrDefault();			
			if (qualifier == null)
				return ProvideComplexExpression(serviceProvider, uri, uriCandidates);
			return qualifier.GetBinding(((IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))).TargetObject as DependencyObject)
				.Do(x => x.Converter = new ComplexUriQualifierConverter(dObj, uriCandidates, uri)).ProvideValue(serviceProvider) as BindingExpressionBase;
		}
		static object ProvideStaticValue(IServiceProvider serviceProvider, Uri uri) {
			return UriToTypeHelper.GetObject(uri, (((IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))).TargetProperty as DependencyProperty).PropertyType);
		}
		#endregion
		public static object ProvideValueOrExpression(IServiceProvider serviceProvider, string uriString) {
			return ProvideValueOrExpression(serviceProvider, new Uri(uriString, UriKind.RelativeOrAbsolute));
		}
		public static object ProvideValueOrExpression(IServiceProvider serviceProvider, Uri relativeUri) {
			Uri absoluteUri;
			Uri baseUri = GetBaseUri(serviceProvider);
			if (baseUri == null) {
				return GetDesignTimeHook(serviceProvider, relativeUri);
			}
			if (!relativeUri.IsAbsoluteUri) {
				absoluteUri = new Uri(baseUri, relativeUri);
			} else {
				absoluteUri = relativeUri;
			}
			var data = GetUriMap(baseUri, absoluteUri);
			var ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			var dObj = ipvt.TargetObject as DependencyObject;
			var dProp = ipvt.TargetProperty as DependencyProperty;
			bool bindingUnavailiable = dObj == null || dProp == null;
			if (bindingUnavailiable || data == null || !data.GetValues(absoluteUri).Any())
				return ProvideStaticValue(serviceProvider, absoluteUri);
			return ProvideBindingValue(dObj, serviceProvider, absoluteUri, data);
		}
		static object GetDesignTimeHook(IServiceProvider serviceProvider, Uri relativeUri) {
			var dObj = GetTarget(serviceProvider);
			if (dObj != null && DesignerProperties.GetIsInDesignMode(dObj)) {
				dObj.Dispatcher.BeginInvoke(new Action(() => {
					var dp = ((IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))).TargetProperty as DependencyProperty;
					if (dp == null)
						return;
					dObj.SetValue(dp, ProvideValueOrExpression(serviceProvider, relativeUri));
				}));
			}
			return DependencyProperty.UnsetValue;
		}
		public static void RegisterQualifier(IBaseUriQualifier qualifier, bool throwOnError = true) {			
			lock (olock)
			{
				if (registeredQualifiers.ContainsKey(qualifier.Name))
					throw new ArgumentException("duplicate qualifier name, unregister first");
				registeredQualifiers.Add(qualifier.Name, qualifier);
				QualifierListener.ResetInitialization();
			}
		}
		public static IBaseUriQualifier UnregisterQualifier(string qualifierName) {
			lock (olock)
			{
				IBaseUriQualifier result = null;
				if (registeredQualifiers.TryGetValue(qualifierName, out result)) {
					registeredQualifiers.Remove(qualifierName);
				}
				return result;
			}
		}
	}	
	public class QualifiedImageExtension : MarkupExtension {
		public string Uri { get; set; }
		public QualifiedImageExtension() { }
		public QualifiedImageExtension(string uri) {
			this.Uri = uri;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return UriQualifierHelper.ProvideValueOrExpression(serviceProvider, Uri);
		}
	}
}
