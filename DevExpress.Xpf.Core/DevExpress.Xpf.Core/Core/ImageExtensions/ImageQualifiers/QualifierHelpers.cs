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
using System.Collections.Specialized;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
namespace DevExpress.Xpf.Core.Native {			
	class UriQualifierValue {
		public UriQualifierValue(IBaseUriQualifier qualifier, string value) {
			Qualifier = qualifier;
			Value = value;
		}
		public IBaseUriQualifier Qualifier { get; set; }
		public string Value { get; set; }
	}
	class UriInfo {
		public UriInfo(Uri uri, IEnumerable<UriQualifierValue> qualifiers) {
			Uri = uri;
			Qualifiers = qualifiers.ToList().AsReadOnly();
			Guard.ArgumentPositive(Qualifiers.Count, "uri should have at least one qualifier");
		}
		public Uri Uri { get; set; }
		public ReadOnlyCollection<UriQualifierValue> Qualifiers { get; set; }
		public bool MultipleQualifiers { get { return Qualifiers.Count > 1; } }
		public bool BindableQualifier { get { return !MultipleQualifiers && Qualifiers[0].Qualifier is IBindableUriQualifier; } }
	}
	abstract class ResourceCollection : IEnumerable<string>, INotifyCollectionChanged {
		NotifyCollectionChangedEventHandler changedHandler;
		protected abstract IEnumerable<string> EnumerateResourceKeys();
		protected void RaiseChanged() {
			if (changedHandler == null)
				return;
			changedHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}		
		IEnumerator<string> IEnumerable<string>.GetEnumerator() { return EnumerateResourceKeys().GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<string>)this).GetEnumerator(); }
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { changedHandler += value; }
			remove { changedHandler -= value; }
		}
	}
	class PackResourceCollection : ResourceCollection {
		ResourceSet set;
		public PackResourceCollection(ResourceSet set) {
			this.set = set;
		}
		protected override IEnumerable<string> EnumerateResourceKeys() {
			var en = set.GetEnumerator();
			while (en.MoveNext()) {
				yield return (string)((DictionaryEntry)en.Current).Key;
			}
		}
	}
	class FileResourceCollection : ResourceCollection {
		Uri baseuri;
		string location;
		Dictionary<string, FileSystemWatcher> filesAndWatchers;
		List<string> elements;
		public FileResourceCollection(Uri baseuri, Uri targetUri) {
			this.baseuri = baseuri;
			if (baseuri.Scheme != Uri.UriSchemeFile)
				throw new ArgumentException("only uris with the file scheme are supported");
			this.location = baseuri.LocalPath;
			filesAndWatchers = new Dictionary<string, FileSystemWatcher>();
			var projectFiles_ = Directory.EnumerateFiles(location, "*.csproj").Concat(Directory.EnumerateFiles(location, "*.vbproj"));
			foreach (var element in projectFiles_) {
				FileSystemWatcher elementWatcher = new FileSystemWatcher(Path.GetDirectoryName(element)) { Filter = Path.GetFileName(element), NotifyFilter = NotifyFilters.LastWrite };
				filesAndWatchers.Add(element, elementWatcher);
				elementWatcher.Changed += OnElementChanged;
				elementWatcher.EnableRaisingEvents = true;
			}
		}
		void OnElementChanged(object sender, FileSystemEventArgs e) {
			DropCache();
		}
		void DropCache() {
			elements = null;
			RaiseChanged();			
		}
		static XName pName = XName.Get("Resource", "http://schemas.microsoft.com/developer/msbuild/2003");
		static XName includeName = XName.Get("Include");
		protected override IEnumerable<string> EnumerateResourceKeys() {
			if (elements == null) {
				elements = new List<string>();
				try {
					foreach (var project in filesAndWatchers) {
						using (var xmlReader = XmlReader.Create(project.Key)) {
							var xElement = XElement.Load(xmlReader);
							foreach (var resource in xElement.Descendants(pName)) {
								var include = resource.Attribute(includeName);
								if (include == null)
									continue;
								elements.Add(include.Value);
							}
						}
					}
				} catch {
					Dispatcher.CurrentDispatcher.BeginInvoke(new Action(DropCache));
				}				
			}
			return elements;
		}
	}
	class UriInfoMap {
		readonly List<Uri> undefinedResources;
		readonly MultiDictionary<Uri, UriInfo> definedResources;
		IEnumerable<string> set;
		Uri baseUri;
		Uri CreateUri(string baseUriPrefix, string relativeUri) {			
			return new Uri(string.Concat(baseUriPrefix, relativeUri));
		}
		public UriInfoMap(IEnumerable<string> set, Uri baseUri) {
			this.set = set;
			this.baseUri = baseUri;
			var incc = set as INotifyCollectionChanged;
			if (incc != null) {
				incc.CollectionChanged += OnSetChanged;
			}
			undefinedResources = new List<Uri>();
			definedResources = new MultiDictionary<Uri, UriInfo>();
			string baseUriPrefix = GetBaseUriPrefix(set, baseUri);
			foreach (var uriString in set) {
				undefinedResources.Add(CreateUri(baseUriPrefix, uriString));
			}
		}
		private static string GetBaseUriPrefix(IEnumerable<string> set, Uri baseUri) {
			var baseUriPostfix = set.Where(x => baseUri.AbsolutePath.EndsWith(x)).OrderBy(x => x.Length).FirstOrDefault();
			string baseUriPrefix = baseUri.AbsoluteUri;
			if (!string.IsNullOrEmpty(baseUriPostfix))
				baseUriPrefix = new string(baseUriPrefix.Take(baseUriPrefix.LastIndexOf(baseUriPostfix)).ToArray());
			return baseUriPrefix;
		}
		void OnSetChanged(object sender, NotifyCollectionChangedEventArgs e) {
			undefinedResources.Clear();
			definedResources.Clear();
			string baseUriPrefix = GetBaseUriPrefix(set, baseUri);
			foreach (var uriString in set) {
				undefinedResources.Add(CreateUri(baseUriPrefix, uriString));
			}
			QualifierListener.ResetListeners();
		}
		public ICollection<UriInfo> GetValues(Uri uri) {
			ICollection<UriInfo> result;
			if (!definedResources.ContainsKey(uri)) {
				result = new List<UriInfo>();
				for (int i = undefinedResources.Count - 1; i >= 0; i--) {
					var key = undefinedResources[i];
					IEnumerable<UriQualifierValue> qualifiers;
					if (IsQualifiedResourceKey(uri, key, out qualifiers)) {
						result.Add(new UriInfo(key, qualifiers));
						undefinedResources.RemoveAt(i);
					}
				}
				definedResources.AddRange(uri, result);
				return result;
			}
			return definedResources[uri];
		}
		bool IsQualifiedResourceKey(Uri originalUri, Uri candidateUri, out IEnumerable<UriQualifierValue> qualifiers) {
			qualifiers = Enumerable.Empty<UriQualifierValue>();
			if (candidateUri.Segments.Length < originalUri.Segments.Length)
				return false;
			string originalFileName, originalExt, originalQualifiers;
			string candidateFileName, candidateExt, candidateQualifiers;
			SplitName(originalUri.Segments[originalUri.Segments.Length - 1], false, out originalFileName, out originalQualifiers, out originalExt);
			SplitName(candidateUri.Segments[candidateUri.Segments.Length - 1], true, out candidateFileName, out candidateQualifiers, out candidateExt);
			if (!string.Equals(originalFileName, candidateFileName, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(originalExt, candidateExt, StringComparison.InvariantCultureIgnoreCase))
				return false;
			var splittedFileNameQualifierStrings = SplitString(candidateQualifiers, '_');
			IEnumerable<UriQualifierValue> fileNameQualifiers;
			if (TryGetQualifiers(splittedFileNameQualifierStrings, out fileNameQualifiers)) {
				if (candidateUri.Segments.Length == originalUri.Segments.Length) {
					qualifiers = fileNameQualifiers;
				}
			}		
			StringBuilder folderQualifierStringsBuilder = new StringBuilder();
			for (int i = originalUri.Segments.Length - 1; i < candidateUri.Segments.Length - 1; i++) {
				folderQualifierStringsBuilder.Append(candidateUri.Segments[i]);
			}
			var splittedFolderQualifierStrings = SplitString(folderQualifierStringsBuilder.ToString(), '_', '/');
			IEnumerable<UriQualifierValue> folderQualifiers;
			if (TryGetQualifiers(splittedFolderQualifierStrings, out folderQualifiers)) {
				if (fileNameQualifiers.Select(x => x.Qualifier).Intersect(folderQualifiers.Select(x => x.Qualifier)).Any()) {
					throw new AmbiguousMatchException("incorrect element path, repeating qualifiers found");
				}
			}			
			qualifiers = fileNameQualifiers.Concat(folderQualifiers);
			return qualifiers.Any();
		}
		static bool TryGetQualifiers(string[] splittedFileNameQualifierStrings, out IEnumerable<UriQualifierValue> fileNameQualifiers) {			
			List<UriQualifierValue> result = new List<UriQualifierValue>();
			fileNameQualifiers = result;
			if (splittedFileNameQualifierStrings.Length == 0)
				return false;
			foreach (var qualifierString in splittedFileNameQualifierStrings) {
				if (String.IsNullOrEmpty(qualifierString))
					continue;
				var splittedString = SplitString(qualifierString, '-');
				if (splittedString.Length != 2)
					return false;
				IBaseUriQualifier qualifier;
				if (!UriQualifierHelper.registeredQualifiers.TryGetValue(splittedString[0], out qualifier)) {
					return false;
				}
				if (!qualifier.IsValidValue(splittedString[1]))
					return false;
				result.Add(new UriQualifierValue(qualifier, splittedString[1]));
			}
			return true;
		}
		static string[] SplitString(string candidateQualifiers, params char[] separator) {
			if (String.IsNullOrEmpty(candidateQualifiers))
				return new string[] { };
			return candidateQualifiers.ToLower().Split(separator);
		}
		static void SplitName(string nameQualifiersExt, bool hasQualifiers, out string fileName, out string qualifiers, out string ext) {
			var splitted = nameQualifiersExt.Split('.');
			if (splitted.Length == 1) {
				qualifiers = null;
				ext = null;
				fileName = nameQualifiersExt;
				return;
			}
			ext = splitted[splitted.Length - 1];
			int lastIndex = splitted.Length - 2;
			if (splitted.Length == 2) {
				qualifiers = null;
				fileName = splitted[0];
				return;
			}
			if (hasQualifiers) {
				qualifiers = splitted[lastIndex];
				lastIndex--;
			} else {
				qualifiers = null;
			}
			StringBuilder fileNameBuilder = new StringBuilder();
			for (int index = 0; index <= lastIndex; index++) {
				fileNameBuilder.Append(splitted[index]);
			}
			fileName = fileNameBuilder.ToString();
		}
	}   
	class UriQualifierObjectWrapper {
		QualifierListener instance;
		public QualifierListener Instance { get { return instance; } }
		public UriQualifierObjectWrapper(QualifierListener instance) {
			this.instance = instance;
		}
	}
	class ComplexUriQualifierConverter : IValueConverter {
		Uri uri;
		Func<ICollection<UriInfo>> uriCandidates;
		DependencyObject instance;
		public ComplexUriQualifierConverter(DependencyObject instance, Func<ICollection<UriInfo>> uriCandidates, Uri uri) {
			this.uriCandidates = uriCandidates;
			this.uri = uri;
			this.instance = instance;
		}		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			DependencyObject target = instance ?? (value as QualifierListener).With(x => x.Target);
			if (target==null)
				return uri;
			var ucv = uriCandidates();
			if (!ucv.Any())
				return Binding.DoNothing;
			Dictionary<IBaseUriQualifier, List<string>> qValues = new Dictionary<IBaseUriQualifier, List<string>>();
			foreach (var uriInfo in ucv) {
				foreach (var qualifier in uriInfo.Qualifiers) {
					if (!qValues.ContainsKey(qualifier.Qualifier))
						qValues.Add(qualifier.Qualifier, new List<string>());
					qValues[qualifier.Qualifier].Add(qualifier.Value);
				}
			}
			return UriToTypeHelper.GetObject(ucv.Select(x => new Tuple<double, Uri>(GetScore(target, x, qValues), x.Uri)).Where(x => x.Item1 >= 0).OrderByDescending(x => x.Item1).FirstOrDefault().With(x => x.Item2) ?? uri, targetType);
		}	 
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		double GetScore(DependencyObject dObj, UriInfo qualifierInfo, Dictionary<IBaseUriQualifier, List<string>> qualifiersAndValues) {
			List<Tuple<int, int>> qms = new List<Tuple<int, int>>();
			foreach(var qv in qualifiersAndValues) {
				var qualifier = qualifierInfo.Qualifiers.FirstOrDefault(x => x.Qualifier == qv.Key);
				if (qualifier == null)
					continue;
				int maxAltitude = 0;
				int currentAltitude = 0;
				currentAltitude = qv.Key.GetAltitude(dObj, qualifier.Value, qv.Value, out maxAltitude);
				if (currentAltitude == -1)
					return -1;
				qms.Add(new Tuple<int, int>(currentAltitude, maxAltitude));
			}
			var totalAltitude = (double)qms.Max(x => x.Item2);
			var result = qms.Sum(x => (totalAltitude / x.Item2) * x.Item1);
			return result;
		}
	}
	class QualifierListener : DependencyObject {
		DependencyObject target;
		public DependencyObject Target { get { return target; } }
		PostponedAction resetAction;
		QualifierListener(DependencyObject target) {
			this.target = target;
			resetAction = new PostponedAction(() => true);
		}		
		public static Binding CreateBinding(IServiceProvider serviceProvider, Uri uri, Func<ICollection<UriInfo>> uriCandidates) {
			var instance = CreateInstance(serviceProvider, uri, uriCandidates);
			return new Binding() { RelativeSource = RelativeSource.Self, Converter = new ComplexUriQualifierConverter(null, uriCandidates, uri), Path = new PropertyPath("(0).Instance", UriQualifierHelper.QualifierInfoProperty) };
		}
		static QualifierListener CreateInstance(IServiceProvider serviceProvider, Uri uri, Func<ICollection<UriInfo>> uriCandidates) {
			InitializeProperties();
			var result = new QualifierListener(((IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))).TargetObject as DependencyObject);
			result.SubscribeNotifications(uriCandidates().SelectMany(x => x.Qualifiers).Distinct());
			result.ResetForce();
			return result;
		}
		void InvokeReset() {
			target.Dispatcher.Invoke(new Action(ResetPostponed), DispatcherPriority.Send);
			target.Dispatcher.BeginInvoke(new Action(resetAction.PerformForce), DispatcherPriority.Normal);
		}
		void ResetPostponed() {
			resetAction.PerformPostpone(ResetForce);
		}
		void ResetForce() { target.SetValue(UriQualifierHelper.QualifierInfoProperty, new UriQualifierObjectWrapper(this)); }
		static volatile bool initialized;
		static object olock = new object();
		static List<IBaseUriQualifier> initializedQualifiers;
		static void InitializeProperties() {
			if (!initialized) {
				lock (olock)
				{
					if (!initialized) {
						LockedInitialize();
						initialized = true;
					}
				}
			}
		}
		static void LockedInitialize() {			
			dProps = dProps ?? new Dictionary<string, DependencyProperty>();
			initializedQualifiers = initializedQualifiers ?? new List<IBaseUriQualifier>();
			foreach (var qualifier in UriQualifierHelper.registeredQualifiers.Values) {
				if (initializedQualifiers.Contains(qualifier))
					continue;
				initializedQualifiers.Add(qualifier);
				IUriQualifier eventQualifier = qualifier as IUriQualifier;
				if (eventQualifier != null) {
					InitializeEventNotification(eventQualifier);
					continue;
				}
				IBindableUriQualifier bindingQualifier = qualifier as IBindableUriQualifier;
				if (bindingQualifier != null) {
					InitializeDPropNotification(bindingQualifier);
					continue;
				}
				throw new ArgumentException(string.Format("Only {0} or {1} qualifiers supported", typeof(IUriQualifier).Name, typeof(IBindableUriQualifier).Name));
			}
		}
		static Dictionary<string, DependencyProperty> dProps;
		static void InitializeDPropNotification(IBindableUriQualifier qualifier) {
			dProps.Add(qualifier.Name, DependencyProperty.Register(qualifier.Name, typeof(object), typeof(QualifierListener)));
		}
		static void InitializeEventNotification(IUriQualifier qualifier) {
			qualifier.ActiveValueChanged += OnEventQualifierActiveValueChanged;
		}
		static void OnEventQualifierActiveValueChanged(object sender, EventArgs eventArgs) {
			foreach (var qO in EventListeners.ToArray()) {
				if (sender == null || qO.target == sender) {
					qO.InvokeReset();
				}					
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			InvokeReset(); 
		}
		static WeakList<QualifierListener> eventListeners;
		static WeakList<QualifierListener> EventListeners { get { return eventListeners ?? (eventListeners = new WeakList<QualifierListener>()); } }
		void SubscribeNotifications(IEnumerable<UriQualifierValue> enumerable) {
			EventListeners.Add(this);
			foreach (var element in enumerable) {
				var bindable = element.Qualifier as IBindableUriQualifier;
				if (bindable == null)
					continue;
				BindingOperations.SetBinding(this, dProps[bindable.Name], bindable.GetBinding(target));
			}
		}
		internal static void ResetInitialization() {
			if (initialized) {
				lock (olock)
				{
					if (initialized)
						initialized = false;
				}
			}
			InitializeProperties();
		}
		internal static void ResetListeners() {
			OnEventQualifierActiveValueChanged(null, null);
		}
	}
	static class UriToTypeHelper {
		public static object GetObject(Uri uri, Type targetType) {
			if (targetType.IsAssignableFrom(typeof(ImageSource))) {
				return new BitmapImage(uri);
			}
			return uri;
		}
	}
}
