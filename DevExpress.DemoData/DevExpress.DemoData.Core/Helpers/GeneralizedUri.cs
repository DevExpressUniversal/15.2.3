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
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using DevExpress.DemoData.Utils;
using DevExpress.Internal;
using System.Drawing;
namespace DevExpress.DemoData.Helpers {
	public abstract class GeneralizedUri {
		static object kindsUrisCreatorsLock = new object();
		static Dictionary<string, GeneralizedUriKind> kinds = new Dictionary<string, GeneralizedUriKind>();
		static Dictionary<string, GeneralizedUri> uris = new Dictionary<string, GeneralizedUri>();
		static Dictionary<Type, ObjectFromGeneralizedUriCreatorBase> creators = new Dictionary<Type, ObjectFromGeneralizedUriCreatorBase>();
		object streamLoadedLock = new object();
		bool streamLoaded;
		object uriLoadedLock = new object();
		bool uriLoaded;
		Uri uri;
		Stream stream;
		object objectsLock = new object();
		Dictionary<Type, object> objects = new Dictionary<Type, object>();
		static GeneralizedUri() {
			RegisterPackScheme();
			RegisterDefaultKinds();
			RegisterDefaultCreators();
		}
		public static GeneralizedUri GetUri(string uriString) {
			lock(kindsUrisCreatorsLock) {
				GeneralizedUri uri;
				if(!uris.TryGetValue(uriString, out uri)) {
					uri = CreateUri(uriString);
					uris.Add(uriString, uri);
				}
				return uri;
			}
		}
		public static void RegisterKind(GeneralizedUriKind kind) {
			lock(kindsUrisCreatorsLock) {
				if(!kinds.ContainsKey(kind.Prefix))
					kinds.Add(kind.Prefix, kind);
			}
		}
		public static void RegisterCreator<T>(ObjectFromGeneralizedUriCreator<T> creator) {
			lock(kindsUrisCreatorsLock) {
				if(!creators.ContainsKey(typeof(T)))
					creators.Add(typeof(T), creator);
			}
		}
		public GeneralizedUri() {
			AllowUseStreamMultipleTimes = true;
		}
		public bool AllowUseStreamMultipleTimes { get; set; }
		public Stream Stream {
			get {
				LoadStream();
				return stream;
			}
			protected set { stream = value; }
		}
		public Uri Uri {
			get {
				LoadUri();
				return uri;
			}
			protected set { uri = value; }
		}
		public GeneralizedUri PrepareToCreate<T>() {
			ObjectFromGeneralizedUriCreator<T> creator = (ObjectFromGeneralizedUriCreator<T>)creators[typeof(T)];
			creator.PrepareToCreate(this);
			return this;
		}
		public T Create<T>() {
			lock(objectsLock) {
				object obj;
				if(!objects.TryGetValue(typeof(T), out obj)) {
					ObjectFromGeneralizedUriCreator<T> creator = (ObjectFromGeneralizedUriCreator<T>)creators[typeof(T)];
					obj = creator.Create(this);
					objects.Add(typeof(T), obj);
				}
				return (T)obj;
			}
		}
		protected abstract void LoadStreamCore();
		protected abstract void LoadUriCore();
		protected static string ExtractPrefix(string uriString, out string content) {
			int d = uriString.IndexOf("://", StringComparison.Ordinal);
			if(d < 0) {
				content = uriString;
				return string.Empty;
			}
			content = uriString.SafeSubstring(d + 3);
			return uriString.SafeRemove(d);
		}
		protected static string ConcatPrefix(string prefix, string content) {
			return string.Format("{0}://{1}", prefix, content);
		}
		static void RegisterPackScheme() {
			new System.Windows.Documents.FlowDocument();
		}
		static void RegisterDefaultKinds() {
			RegisterKind(new FileUri.FileUriKind());
			RegisterKind(new ResourceUri.ResourceUriKind());
			RegisterKind(new EmbeddedResourceUri.EmbeddedResourceUriKind());
			RegisterKind(new WebUri.WebUriKind());
			RegisterKind(new SimpleUri.SimpleUriKind());
		}
		static void RegisterDefaultCreators() {
			RegisterCreator<ImageSource>(new ImageSourceCreator());
			RegisterCreator<Image>(new DrawingImageCreator());
		}
		static GeneralizedUri CreateUri(string uriString) {
			if(uriString.StartsWith("pack"))
				return new ResourceUri.ResourceUriKind().CreateUri(uriString.Replace("pack://application:,,,", "resource:/").Replace("component/", "/"));
			string content;
			string prefix = ExtractPrefix(uriString, out content);
			GeneralizedUriKind kind;
			if(!kinds.TryGetValue(prefix, out kind)) throw new ArgumentOutOfRangeException();
			return kind.CreateUri(uriString);
		}
		void LoadStream() {
			lock(streamLoadedLock) {
				if(!streamLoaded) {
					LoadStreamCore();
					if(AllowUseStreamMultipleTimes)
						FixStream();
					streamLoaded = true;
				}
			}
		}
		void LoadUri() {
			lock(uriLoadedLock) {
				if(!uriLoaded) {
					LoadUriCore();
					uriLoaded = true;
				}
			}
		}
		void FixStream() {
			if(this.stream != null && !this.stream.CanSeek) {
				using(Stream oldStream = this.stream) {
					Stream = StreamHelper.CopyToMemoryStream(oldStream);
				}
			}
		}
		#region Equality
		public override int GetHashCode() {
			return ToString().GetHashCode();
		}
		public override bool Equals(object obj) {
			GeneralizedUri uri = obj as GeneralizedUri;
			if(uri == null) return false;
			return ToString() == uri.ToString();
		}
		public static bool operator ==(GeneralizedUri uri1, GeneralizedUri uri2) {
			bool uri1IsNull = (object)uri1 == null;
			bool uri2IsNull = (object)uri2 == null;
			if(uri1IsNull && uri2IsNull) return true;
			if(uri1IsNull || uri2IsNull) return false;
			return uri1.ToString() == uri2.ToString();
		}
		public static bool operator !=(GeneralizedUri uri1, GeneralizedUri uri2) {
			return !(uri1 == uri2);
		}
		#endregion
	}
	public abstract class GeneralizedUriKind {
		string prefix;
		public GeneralizedUriKind(string prefix) {
			this.prefix = prefix;
		}
		public string Prefix { get { return prefix; } }
		public abstract GeneralizedUri CreateUri(string uriString);
	}
	public abstract class ObjectFromGeneralizedUriCreatorBase { }
	public abstract class ObjectFromGeneralizedUriCreator<T> : ObjectFromGeneralizedUriCreatorBase {
		public void PrepareToCreate(GeneralizedUri uri) {
			if(CanCreateFromUri) {
				Uri u = uri.Uri;
				if(u != null) return;
			}
			if(CanCreateFromStream) {
				Stream s = uri.Stream;
			}
		}
		public T Create(GeneralizedUri uri) { return Create(uri, null); }
		public T Create(GeneralizedUri uri, object param) {
			if(CanCreateFromUri) {
				Uri u = uri.Uri;
				if(u != null)
					return CreateFromUri(u, param);
			}
			if(CanCreateFromStream) {
				Stream s = uri.Stream;
				if(s != null) {
					s.Seek(0, SeekOrigin.Begin);
					return CreateFromStream(s, param);
				}
			}
			return default(T);
		}
		protected abstract bool CanCreateFromUri { get; }
		protected abstract bool CanCreateFromStream { get; }
		protected abstract T CreateFromUri(Uri uri, object param);
		protected abstract T CreateFromStream(Stream stream, object param);
	}
	public abstract class AsyncGeneralizedUri : GeneralizedUri {
		AutoResetEvent streamLoaded;
		AutoResetEvent uriLoaded;
		protected override sealed void LoadStreamCore() {
			this.streamLoaded = new AutoResetEvent(false);
			BeginLoadStreamCore();
			this.streamLoaded.WaitOne();
		}
		protected void EndLoadStreamCore() {
			this.streamLoaded.Set();
		}
		protected abstract void BeginLoadStreamCore();
		protected override sealed void LoadUriCore() {
			this.uriLoaded = new AutoResetEvent(false);
			BeginLoadUriCore();
			this.uriLoaded.WaitOne();
		}
		protected void EndLoadUriCore() {
			this.uriLoaded.Set();
		}
		protected abstract void BeginLoadUriCore();
	}
	public class FileUri : GeneralizedUri {
		public class FileUriKind : GeneralizedUriKind {
			public FileUriKind() : base(FileUri.Prefix) { }
			public override GeneralizedUri CreateUri(string uriString) { return new FileUri(uriString); }
		}
		public const string Prefix = "file";
		string path;
		protected FileUri(string uriString) {
			string path;
			string prefix = ExtractPrefix(uriString, out path);
			this.path = path;
		}
		public static string GetUriString(string path) {
			return ConcatPrefix(Prefix, path);
		}
		public string Path { get { return path; } }
		protected override void LoadUriCore() { }
		protected override void LoadStreamCore() {
			Stream = new FileStream(Path, FileMode.Open, FileAccess.Read);
		}
		public override string ToString() { return GetUriString(Path); }
	}
	public class ResourceUri : GeneralizedUri {
		public class ResourceUriKind : GeneralizedUriKind {
			public ResourceUriKind() : base(ResourceUri.Prefix) { }
			public override GeneralizedUri CreateUri(string uriString) { return new ResourceUri(uriString); }
		}
		public const string Prefix = "resource";
		Assembly assembly;
		string path;
		bool pathIsFull;
		protected ResourceUri(string uriString) {
			string content;
			string prefix = ExtractPrefix(uriString, out content);
			int d = content.IndexOf(';');
			if(d < 0) throw new ArgumentOutOfRangeException();
			string assemblyName = content.SafeRemove(d);
			string path = content.SafeSubstring(d + 1);
			this.pathIsFull = path.Length != 0 && path[0] == '/';
			this.path = this.pathIsFull ? path.SafeSubstring(1) : path;
			this.assembly = AssemblyHelper.GetAssembly(assemblyName);
		}
		public Assembly Assembly { get { return assembly; } }
		public string Path { get { return path; } }
		public bool PathIsFull { get { return pathIsFull; } }
		public static string GetUriString(Assembly assembly, string path, bool pathIsFull) {
			string assemblyName = AssemblyHelper.GetPartialName(assembly);
			string content = string.Format("{0};{1}", assemblyName, pathIsFull ? "/" + path : path);
			return ConcatPrefix(Prefix, content);
		}
		protected override void LoadUriCore() {
			if(PathIsFull)
				Uri = AssemblyHelper.GetResourceUri(Assembly, Path);
		}
		protected override void LoadStreamCore() {
			Stream = AssemblyHelper.GetResourceStream(Assembly, Path, PathIsFull);
		}
		public override string ToString() { return GetUriString(Assembly, Path, PathIsFull); }
	}
	public class EmbeddedResourceUri : GeneralizedUri {
		public class EmbeddedResourceUriKind : GeneralizedUriKind {
			public EmbeddedResourceUriKind() : base(EmbeddedResourceUri.Prefix) { }
			public override GeneralizedUri CreateUri(string uriString) { return new EmbeddedResourceUri(uriString); }
		}
		public const string Prefix = "embedded";
		Assembly assembly;
		string path;
		bool pathIsFull;
		protected EmbeddedResourceUri(string uriString) {
			string content;
			string prefix = ExtractPrefix(uriString, out content);
			int d = content.IndexOf(';');
			if(d < 0) throw new ArgumentOutOfRangeException();
			string assemblyName = content.SafeRemove(d);
			string path = content.SafeSubstring(d + 1);
			this.pathIsFull = path.Length != 0 && path[0] == '/';
			this.path = this.pathIsFull ? path.SafeSubstring(1) : path;
			this.assembly = AssemblyHelper.GetAssembly(assemblyName);
		}
		public Assembly Assembly { get { return assembly; } }
		public string Path { get { return path; } }
		public bool PathIsFull { get { return pathIsFull; } }
		public static string GetUriString(Assembly assembly, string path, bool pathIsFull) {
			string assemblyName = AssemblyHelper.GetPartialName(assembly);
			string content = string.Format("{0};{1}", assemblyName, pathIsFull ? "/" + path : path);
			return ConcatPrefix(Prefix, content);
		}
		protected override void LoadStreamCore() {
			Stream = AssemblyHelper.GetEmbeddedResourceStream(Assembly, Path, PathIsFull);
		}
		protected override void LoadUriCore() { }
		public override string ToString() { return GetUriString(Assembly, Path, PathIsFull); }
	}
	public class WebUri : AsyncGeneralizedUri {
		public class WebUriKind : GeneralizedUriKind {
			public WebUriKind() : base(WebUri.Prefix) { }
			public override GeneralizedUri CreateUri(string uriString) { return new WebUri(uriString); }
		}
		public const string Prefix = "web";
		string url;
		protected WebUri(string uriString) {
			string url;
			string prefix = ExtractPrefix(uriString, out url);
			this.url = url;
		}
		public string Url { get { return url; } }
		public static string GetUriString(string url) {
			return ConcatPrefix(Prefix, url);
		}
		protected override void BeginLoadUriCore() { EndLoadUriCore(); }
		protected override void BeginLoadStreamCore() {
			WebRequest request = WebRequest.Create(url);
			request.BeginGetResponse(AsyncCallback, request);
		}
		void AsyncCallback(IAsyncResult ar) {
			try {
				WebRequest request = (WebRequest)ar.AsyncState;
				WebResponse response = request.EndGetResponse(ar);
				Stream = response.GetResponseStream();
			} catch { }
			EndLoadStreamCore();
		}
		public override string ToString() { return GetUriString(Url); }
	}
	public class SimpleUri : GeneralizedUri {
		public class SimpleUriKind : GeneralizedUriKind {
			public SimpleUriKind() : base(SimpleUri.Prefix) { }
			public override GeneralizedUri CreateUri(string uriString) { return new SimpleUri(uriString); }
		}
		public const string Prefix = "uri";
		string path;
		UriKind kind;
		protected SimpleUri(string uriString) {
			string content;
			string prefix = ExtractPrefix(uriString, out content);
			string kindString = string.Empty;
			int d = content.IndexOf(';');
			if(d >= 0) {
				kindString = content.SafeRemove(d);
				content = content.SafeSubstring(d + 1);
			}
			this.kind = kindString == "relative" ? UriKind.Relative : kindString == "absolute" ? UriKind.Absolute : UriKind.RelativeOrAbsolute;
			this.path = content;
		}
		public string Path { get { return path; } }
		public UriKind Kind { get { return kind; } }
		public static string GetUriString(string path, UriKind kind) {
			string kindString = kind == UriKind.Relative ? "relative;" : kind == UriKind.Absolute ? "absolute;" : string.Empty;
			return ConcatPrefix(Prefix, kindString + path);
		}
		protected override void LoadStreamCore() { }
		protected override void LoadUriCore() {
			Uri = new Uri(Path, Kind);
		}
		public override string ToString() { return GetUriString(Path, Kind); }
	}
}
