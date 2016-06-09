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
using System.Globalization;
using System.Resources;
using System.Text;
using System.Xml;
using DevExpress.Utils.Localization.Internal;
using System.Threading;
namespace DevExpress.Utils.Localization {
	#region XtraLocalizer<T> (abstract class)
	public abstract class XtraLocalizer<T> where T : struct {
		#region Fields
		static ActiveLocalizerProvider<T> localizerProvider;
		Dictionary<T, string> stringTable;
		#endregion
		protected XtraLocalizer() {
			CreateStringTable();
		}
		#region Properties
		CultureInfo currentCulture;
		public virtual string Language { get { return "English"; } }
		internal Dictionary<T, string> StringTable { get { return stringTable; } }
		public static XtraLocalizer<T> Active {
			get { return localizerProvider.GetActiveLocalizer(); }
			set {
				if (value == null)
					value = localizerProvider.GetActiveLocalizer().CreateResXLocalizer();
				if (Object.ReferenceEquals(localizerProvider.GetActiveLocalizer(), value))
					return;
				localizerProvider.SetActiveLocalizer(value);
				RaiseActiveChanged();
			}
		}
		CultureInfo CurrentUICulture {
			get {
#if DXRESTRICTED || WINRT
				return CultureInfo.CurrentUICulture;
#else
				return Thread.CurrentThread.CurrentUICulture;
#endif
			}
		}
#endregion
		#region Events
		[ThreadStatic]
		static EventHandler onActiveChanged;
		public static event EventHandler ActiveChanged { add { onActiveChanged += value; } remove { onActiveChanged -= value; } }
		public static void RaiseActiveChanged() {
			if (onActiveChanged != null)
				onActiveChanged(Active, EventArgs.Empty);
		}
		#endregion
		public static void SetActiveLocalizerProvider(ActiveLocalizerProvider<T> value) {
			localizerProvider = value;
		}
		public static ActiveLocalizerProvider<T> GetActiveLocalizerProvider() {
			return localizerProvider;
		}
		protected virtual IEqualityComparer<T> CreateComparer() {
			return EqualityComparer<T>.Default;
		}
		protected internal virtual void CreateStringTable() {
			this.stringTable = new Dictionary<T, string>(CreateComparer());
			PopulateStringTable();
			currentCulture = CurrentUICulture;
		}
		protected virtual void AddString(T id, string str) {
			stringTable[id] = str;
		}
		protected virtual bool DiffersFromCurrentCulture() {
			return !object.Equals(currentCulture, CurrentUICulture);
		}
		public virtual string GetLocalizedString(T id) {
			if(DiffersFromCurrentCulture()) {
				stringTable.Clear();
				PopulateStringTable();
				currentCulture = CurrentUICulture;
			}
			string result;
			return stringTable.TryGetValue(id, out result) ? result : String.Empty;
		}
		protected internal virtual string GetEnumTypeName() {
			return typeof(T).Name;
		}
#if !SILVERLIGHT && !WINRT && !DXRESTRICTED
		public virtual void WriteToXml(string fileName) {
			XmlDocument doc = CreateXmlDocument();
			XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
			try {
				writer.Formatting = Formatting.Indented;
				doc.WriteTo(writer);
			}
			finally {
				writer.Flush();
				writer.Close();
			}
		}
		public virtual XmlDocument CreateXmlDocument() {
			string typeName = GetEnumTypeName();
			XmlDocument doc = new XmlDocument();
			XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
			doc.AppendChild(decl);
			XmlElement root = doc.CreateElement("root");
			doc.AppendChild(root);
			T[] values = (T[])Enum.GetValues(typeof(T));
			string[] names = Enum.GetNames(typeof(T));
			int count = values.Length;
			for (int i = 0; i < count; i++) {
				XmlElement dataEl = doc.CreateElement("data");
				root.AppendChild(dataEl);
				XmlAttribute nameAttr = doc.CreateAttribute("name");
				nameAttr.Value = String.Format("{0}.{1}", typeName, names[i]);
				dataEl.Attributes.Append(nameAttr);
				XmlElement valueEl = doc.CreateElement("value");
				dataEl.AppendChild(valueEl);
				XmlText valueText = doc.CreateTextNode("value");
				valueText.Value = GetLocalizedString(values[i]);
				valueEl.AppendChild(valueText);
			}
			return doc;
		}
#endif
		protected abstract void PopulateStringTable();
		public abstract XtraLocalizer<T> CreateResXLocalizer();
	}
#endregion
	#region XtraResXLocalizer<T> (abstract class)
	public abstract class XtraResXLocalizer<T> : XtraLocalizer<T> where T : struct {
		#region Fields
		readonly XtraLocalizer<T> embeddedLocalizer;
		ResourceManager manager;
		#endregion
		protected XtraResXLocalizer(XtraLocalizer<T> embeddedLocalizer) {
			Guard.ArgumentNotNull(embeddedLocalizer, "embeddedLocalizer");
			this.embeddedLocalizer = embeddedLocalizer;
			CreateResourceManager();
		}
		#region Properties
		protected internal virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		internal XtraLocalizer<T> EmbeddedLocalizer { get { return embeddedLocalizer; } }
		#endregion
		protected override void PopulateStringTable() {
		}
		protected internal virtual void CreateResourceManager() {
#if !WINRT && !DXRESTRICTED
			if (manager != null)
				this.manager.ReleaseAllResources();
#endif
			this.manager = CreateResourceManagerCore();
		}
		public override string GetLocalizedString(T id) {
			string result = base.GetLocalizedString(id);
			if (String.IsNullOrEmpty(result)) {
				lock (this) {
					result = base.GetLocalizedString(id);
					if (String.IsNullOrEmpty(result)) {
						result = GetLocalizedStringCore(id);
						AddString(id, result);
					}
				}
			}
			return result;
		}
		protected internal virtual string GetLocalizedStringCore(T id) {
			string result = GetLocalizedStringFromResources(id);
			if (result == null)
				result = embeddedLocalizer.GetLocalizedString(id);
			return result;
		}
		protected string GetLocalizedStringFromResources(T id) {
			string resStr = String.Format("{0}.{1}", GetEnumTypeName(), id.ToString());
			return Manager.GetString(resStr);
		}
#if !SILVERLIGHT && !WINRT && !DXRESTRICTED
		public override XmlDocument CreateXmlDocument() {
			return embeddedLocalizer.CreateXmlDocument();
		}
#endif
		public override XtraLocalizer<T> CreateResXLocalizer() {
			return embeddedLocalizer.CreateResXLocalizer();
		}
#if DXPORTABLE
		public static string GetLocalizedStringFromResources<TStringId, TLocalizer>(TStringId id, CultureInfo culture, Func<TLocalizer> getActiveLocalizer, Func<TStringId, string> defaultGetString)
			where TLocalizer : XtraResXLocalizer<TStringId>
			where TStringId : struct {
			TLocalizer localizer = getActiveLocalizer();
			if (localizer == null) {
				defaultGetString(id); 
				localizer = getActiveLocalizer(); 
				if (localizer == null)
					return defaultGetString(id);
			}
			string resStr = String.Format("{0}.{1}", typeof(TStringId).Name, id);
			string result = localizer.Manager.GetString(resStr, culture);
			if (!String.IsNullOrEmpty(result))
				return result;
			return defaultGetString(id);
		}
#else
#endif
		protected abstract ResourceManager CreateResourceManagerCore();
		public string GetInvariantString(T id) {
			return EmbeddedLocalizer.GetLocalizedString(id);
		}
	}
#endregion
}
namespace DevExpress.Utils.Localization.Internal {
	public static class XtraLocalizierHelper<T> where T : struct {
		public static Dictionary<T, string> GetStringTable(XtraLocalizer<T> localizer) {
			return localizer.StringTable;
		}
	}
	public abstract class ActiveLocalizerProvider<T> where T : struct {
		readonly XtraLocalizer<T> defaultLocalizer;
		protected ActiveLocalizerProvider(XtraLocalizer<T> defaultLocalizer) {
			this.defaultLocalizer = defaultLocalizer;
			SetActiveLocalizerCore(defaultLocalizer);
		}
		protected internal XtraLocalizer<T> DefaultLocalizer { get { return defaultLocalizer; } }
		public XtraLocalizer<T> GetActiveLocalizer() {
			XtraLocalizer<T> active = GetActiveLocalizerCore();
			if (active == null) {
				SetActiveLocalizerCore(DefaultLocalizer);
				return DefaultLocalizer;
			}
			else
				return active;
		}
		public void SetActiveLocalizer(XtraLocalizer<T> localizer) {
			SetActiveLocalizerCore(localizer);
		}
		protected internal abstract XtraLocalizer<T> GetActiveLocalizerCore();
		protected internal abstract void SetActiveLocalizerCore(XtraLocalizer<T> localizer);
	}
	public class DefaultActiveLocalizerProvider<T> : ActiveLocalizerProvider<T> where T : struct {
		[ThreadStatic()]
		static XtraLocalizer<T> threadLocalizer;
		public DefaultActiveLocalizerProvider(XtraLocalizer<T> defaultLocalizer)
			: base(defaultLocalizer) {
		}
		protected internal override XtraLocalizer<T> GetActiveLocalizerCore() {
			return threadLocalizer;
		}
		protected internal override void SetActiveLocalizerCore(XtraLocalizer<T> localizer) {
			threadLocalizer = localizer;
		}
	}
}
