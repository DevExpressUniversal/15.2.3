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
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Styles {
	public class Setter {
		object valueCore;
		string pathCore;
		public Setter(string property)
			: this(property, null) {
		}
		public Setter(string property, object value) {
			pathCore = property;
			valueCore = value;
		}
		[XtraSerializableProperty]
		public string Path {
			get { return pathCore; }
		}
		[XtraSerializableProperty]
		public object Value {
			get { return valueCore; }
			set { valueCore = value; }
		}
		public override string ToString() {
			return string.Format("{0},{1}", pathCore, valueCore);
		}
	}
	public class SetterCollection : IEnumerable<Setter>, IXtraSerializable {
		IDictionary<string, Setter> settersCore;
		internal readonly PropertyDescriptorCollection Properties;
		internal SetterCollection(Type targetType) {
			settersCore = new Dictionary<string, Setter>();
			Properties = (targetType != null) ? GetProperties(targetType) : null;
		}
		internal SetterCollection()
			: this(null) {
		}
		protected virtual PropertyDescriptorCollection GetProperties(Type targetType) {
			return TypeDescriptor.GetProperties(targetType);
		}
		protected IDictionary<string, Setter> Setters {
			get { return settersCore; }
		}
		public int Count {
			get { return Setters.Count; }
		}
		public IEnumerator<Setter> GetEnumerator() {
			return Setters.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Setters.Values.GetEnumerator();
		}
		protected virtual bool AllowProperty(string path) {
			if(Properties == null) return true;
			return AllowPropertyCore(path, Properties);
		}
		bool AllowPropertyCore(string path, PropertyDescriptorCollection properties) {
			if(string.IsNullOrEmpty(path)) return false;
			if(allowedProperties != null) {
				if(Array.IndexOf(allowedProperties, path) != -1)
					return true;
			}
			return StyleHelper.GetProperty(path, properties) != null;
		}
		public void Add(string path, object value) {
			if(!AllowProperty(path)) return;
			Setter setter;
			if(!Setters.TryGetValue(path, out setter)) {
				setter = CreateSetter(path);
				Setters.Add(path, setter);
			}
			setter.Value = value;
		}
		public void Add(Setter item) {
			if(item != null && !AllowProperty(item.Path)) return;
			Setter value;
			if(Setters.TryGetValue(item.Path, out value)) {
				value.Value = item.Value;
			}
			else Setters.Add(item.Path, item);
		}
		public bool TryGetValue(string property, out object value) {
			bool result = false;
			Setter setter;
			if(Setters.TryGetValue(property, out setter)) {
				value = setter.Value;
				result = true;
			}
			else value = null;
			return result;
		}
		public bool Remove(string property) {
			return Setters.Remove(property);
		}
		public void Clear() {
			Setters.Clear();
		}
		string[] allowedProperties;
		public void AllowProperties(string[] properties) {
			allowedProperties = properties;
		}
		#region IXtraSerializable Members
		IEnumerable<Setter> itemsCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false)]
		public IEnumerable<Setter> Items {
			get { return itemsCore; }
			set { itemsCore = value; }
		}
		void IXtraSerializable.OnStartSerializing() {
			itemsCore = new List<Setter>(Setters.Values);
		}
		void IXtraSerializable.OnEndSerializing() {
			itemsCore = null;
		}
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			itemsCore = new List<Setter>(Setters.Values);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			itemsCore = null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraFindItemsItem(XtraItemEventArgs e) {
			string property = (string)e.Item.ChildProperties["Path"].Value;
			Setter value = null;
			return Setters.TryGetValue(property, out value) ? value : null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateItemsItem(XtraItemEventArgs e) {
			string property = (string)e.Item.ChildProperties["Path"].Value;
			if(!AllowProperty(property)) return null;
			Setter setter = CreateSetter(property);
			Setters[property] = setter;
			return setter;
		}
		#endregion
		protected virtual Setter CreateSetter(string property) {
			return new Setter(property);
		}
	}
	public class Style {
		Type targetTypeCore;
		SetterCollection settersCore;
		public Style(Type targetType) {
			if(targetType == null)
				throw new ArgumentNullException("targetType");
			targetTypeCore = targetType;
			settersCore = CreateSetters();
		}
		protected virtual SetterCollection CreateSetters() {
			return new SetterCollection(TargetType);
		}
		public Type TargetType {
			get { return targetTypeCore; }
		}
		[XtraSerializableProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string TargetTypeName {
			get { return TargetType.FullName; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public SetterCollection Setters {
			get { return settersCore; }
		}
		public override int GetHashCode() {
			return targetTypeCore.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(!(obj is Style)) return false;
			Style style = (Style)obj;
			return style.TargetType == targetTypeCore;
		}
		public bool IsCompatible(object target) {
			return target != null && TargetType.IsAssignableFrom(target.GetType());
		}
		public void Apply(object target) {
			Apply(target, false);
		}
		public void Apply(object target, bool force) {
			if(!force && !IsCompatible(target)) return;
			using(new SettersBatch(target)) {
				foreach(Setter setter in Setters) {
					StyleHelper.SetValue(target, setter.Path, setter.Value);
				}
			}
		}
		#region internal classes
		class SettersBatch : IDisposable {
			ISupportLockUpdate supportUpdate;
			public SettersBatch(object target) {
				supportUpdate = target as ISupportLockUpdate;
				if(supportUpdate != null)
					supportUpdate.BeginUpdate();
			}
			public void Dispose() {
				if(supportUpdate != null) {
					supportUpdate.EndUpdate();
					supportUpdate = null;
				}
			}
		}
		#endregion internal classes
	}
	public class StyleCollection : IEnumerable<Style>, IXtraSerializable {
		IDictionary<Type, Style> stylesCore;
		public StyleCollection()
			: this(null) {
		}
		public StyleCollection(StyleCollectionKey key) {
			stylesCore = new Dictionary<Type, Style>();
			keyCore = key;
		}
		protected IDictionary<Type, Style> Styles {
			get { return stylesCore; }
		}
		public int Count {
			get { return Styles.Count; }
		}
		public IEnumerator<Style> GetEnumerator() {
			return Styles.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return Styles.Values.GetEnumerator();
		}
		public void Add(Style style) {
			Styles.Add(style.TargetType, style);
		}
		public bool Remove(Style style) {
			return Styles.Remove(style.TargetType);
		}
		public void Clear() {
			Styles.Clear();
		}
		public Style this[Type type] {
			get { return Styles[type]; }
		}
		public bool Contains(Type type) {
			return Styles.ContainsKey(type);
		}
		public bool TryGetStyle(Type type, out Style style) {
			return Styles.TryGetValue(type, out style);
		}
		#region IXtraSerializable Members
		IEnumerable<Style> itemsCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public IEnumerable<Style> Items {
			get { return itemsCore; }
			set { itemsCore = value; }
		}
		void IXtraSerializable.OnStartSerializing() {
			itemsCore = new List<Style>(Styles.Values);
		}
		void IXtraSerializable.OnEndSerializing() {
			itemsCore = null;
		}
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			itemsCore = new List<Style>(Styles.Values);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			itemsCore = null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateItemsItem(XtraItemEventArgs e) {
			string targetTypeName = (string)e.Item.ChildProperties["TargetTypeName"].Value;
			Type targetType = StyleHelper.ResolveType(targetTypeName);
			Style style = CreateStyle(targetType);
			Styles[targetType] = style;
			return style;
		}
		#endregion
		protected virtual Style CreateStyle(Type targetType) {
			return new Style(targetType);
		}
		public void Apply(object target) {
			if(target == null) return;
			ISupportStyles styleTarget = target as ISupportStyles;
			if(styleTarget != null)
				styleTarget.ApplyStyles(this);
			else {
				Style style = FindStyle(target.GetType());
				if(style != null)
					style.Apply(target);
			}
		}
		public Style FindStyle(Type targetType) {
			Type resultType = null;
			foreach(Style style in Styles.Values) {
				Type type = style.TargetType;
				if(type == targetType) {
					resultType = type;
					break;
				}
				if(!type.IsAssignableFrom(targetType))
					continue;
				if(resultType == null) {
					resultType = type;
					continue;
				}
				if(resultType.IsAssignableFrom(type))
					resultType = type;
			}
			return (resultType != null) ? Styles[resultType] : null;
		}
		StyleCollectionKey keyCore;
		public StyleCollectionKey Key {
			get { return keyCore; }
		}
		public void SaveLayoutToXml(string path) {
			SaveLayoutCore(new XmlXtraSerializer(), path);
		}
		public void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(new XmlXtraSerializer(), path);
		}
		public void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new BinaryXtraSerializer(), stream);
		}
		public void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new BinaryXtraSerializer(), stream);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.SerializeObject(this, stream, this.GetType().Name);
			else
				serializer.SerializeObject(this, path.ToString(), this.GetType().Name);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name);
		}
	}
	public sealed class StyleCollectionKey {
		string scopeCore;
		string nameCore;
		string tagCore;
		string valueCore;
		public StyleCollectionKey(string scope) {
			scopeCore = scope;
			valueCore = scope;
		}
		void UpdateValue() {
			StringBuilder builder = new StringBuilder(Scope);
			if(!string.IsNullOrEmpty(Name))
				builder.AppendFormat(".{0}", Name);
			if(!string.IsNullOrEmpty(Tag))
				builder.AppendFormat(".{0}", Tag);
			valueCore = builder.ToString();
		}
		public string Scope {
			get { return scopeCore; }
		}
		public string Name {
			get { return nameCore; }
			set {
				if(Name == value) return;
				nameCore = value;
				UpdateValue();
			}
		}
		public string Tag {
			get { return tagCore; }
			set {
				if(Tag == value) return;
				tagCore = value;
				UpdateValue();
			}
		}
		public string Value {
			get { return valueCore; }
		}
		public override string ToString() {
			return valueCore;
		}
		public override bool Equals(object obj) {
			if(!(obj is StyleCollectionKey)) return false;
			StyleCollectionKey key = obj as StyleCollectionKey;
			return key.valueCore == valueCore;
		}
		public override int GetHashCode() {
			return valueCore.GetHashCode();
		}
		static string[] scopes = new string[] { "Circular", "Linear", "Digital" };
		public static StyleCollectionKey ExtractKey(string path, IServiceProvider serviceProvider) {
			var resourcesProvider = GetService<IStyleResourceProvider>(serviceProvider);
			path = path.Replace(resourcesProvider.GetPathPrefix(), string.Empty);
			path = path.Replace(resourcesProvider.GetPathSuffix(), string.Empty);
			return ExtractKeyCore(path, (name) => CheckThemeName(name, serviceProvider));
		}
		public static string CheckThemeName(string name, IServiceProvider serviceProvider) {
			var themeNameResolver = GetService<IThemeNameResolutionService>(serviceProvider);
			return themeNameResolver.Resolve(name);
		}
		static TService GetService<TService>(IServiceProvider serviceProvider) {
			return (TService)serviceProvider.GetService(typeof(TService));
		}
		static StyleCollectionKey ExtractKeyCore(string path, Func<string, string> themeNameResolve) {
			StyleCollectionKey key = null;
			for(int i = 0; i < scopes.Length; i++) {
				if(path.StartsWith(scopes[i])) {
					key = new StyleCollectionKey(scopes[i]);
					path = path.Substring(scopes[i].Length);
					break;
				}
			}
			if(key != null) {
				key.Tag = ExtractFirstWord(ref path);
				key.Name = themeNameResolve(ExtractFirstWord(ref path));
				key.Tag += path;
			}
			return key;
		}
		static string ExtractFirstWord(ref string path) {
			int pos = path.IndexOf('.');
			string result = path;
			if(pos == 0) {
				path = path.Substring(1);
				return string.Empty;
			}
			if(pos > 0) {
				result = path.Substring(0, pos);
				path = path.Substring(pos + 1);
			}
			else path = string.Empty;
			return result;
		}
	}
}
