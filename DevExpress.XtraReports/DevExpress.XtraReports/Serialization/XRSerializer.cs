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
using System.Text;
using System.Collections;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
#if !SL
using System.Data;
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraReports.Serialization {
	public class XRSerializer {
#if !SL
		#region static
		private static ColorConverter colorConverter = new ColorConverter();
		private static TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
		private static IComponentSource componentSource = new MyComponentSource();
		private static ReportStack reportStack = new ReportStack();
		private static Hashtable asmCache = new Hashtable();
		private static Hashtable fontCache = new Hashtable();
		private static Assembly loadingAssembly;
		private static ReportSerializationInfo nullInfo = new NullSerializationInfo();
		static Hashtable patchedAssemblyNames = new Hashtable();
		static XRSerializer() {
			patchedAssemblyNames["DevExpress.Data,"] = String.Format("{0},", AssemblyInfo.SRAssemblyData);
			patchedAssemblyNames["DevExpress.Utils,"] = String.Format("{0},", AssemblyInfo.SRAssemblyUtils);
			patchedAssemblyNames["DevExpress.XtraPrinting,"] = String.Format("{0},", AssemblyInfo.SRAssemblyPrinting);
			patchedAssemblyNames["DevExpress.XtraReports,"] = String.Format("{0},", AssemblyInfo.SRAssemblyReports);
			patchedAssemblyNames["DevExpress.Data3,"] = String.Format("{0},", AssemblyInfo.SRAssemblyData);
			patchedAssemblyNames["DevExpress.Utils3,"] = String.Format("{0},", AssemblyInfo.SRAssemblyUtils);
			patchedAssemblyNames["DevExpress.XtraPrinting3,"] = String.Format("{0},", AssemblyInfo.SRAssemblyPrinting);
			patchedAssemblyNames["DevExpress.XtraReports3,"] = String.Format("{0},", AssemblyInfo.SRAssemblyReports);
		}
		public static string GetPatchedAssemblyName(string assemblyName) {
			foreach (string name in patchedAssemblyNames.Keys) {
				if (assemblyName.IndexOf(name) >= 0) {
					return assemblyName.Replace(name, (string)patchedAssemblyNames[name]);
				}
			}
			return assemblyName;
		}
		internal static IComponentSource ComponentSource {
			get { return componentSource; }
			set { componentSource = value; }
		}
		internal static XtraReport SerializedReport {
			get { return SerializedReportInfo.Report; }
		}
		private static ReportSerializationInfo SerializedReportInfo {
			get { return reportStack.Peek(); }
		}
		internal static void Begin(XtraReport report, bool serialize) {
			if (serialize && reportStack.Count == 0) {
				asmCache.Clear();
			}
			reportStack.Push(report);
		}
		internal static void End(XtraReport report) {
			ReportSerializationInfo info = reportStack.Pop();
			info.UpdateReferences();
			info.Clear();
			if (reportStack.Count == 0)
				fontCache.Clear();
		}
		public static string GetShortAssemblyName(string asmName) {
			asmName = XRSerializer.GetPatchedAssemblyName(asmName);
			int commaIndex = asmName.IndexOf(',');
			return (commaIndex < 0) ? asmName : asmName.Substring(0, commaIndex);
		}
		static object GetDataSource(DataTable dataTable) {
			return dataTable.DataSet != null ? (object)dataTable.DataSet : (object)dataTable;
		}
		static void AddEventHandler(object target, string method, string name) {
			if (method.Length > 0) {
				try {
					System.Reflection.EventInfo evInfo = target.GetType().GetEvent(name);
					Delegate handler = Delegate.CreateDelegate(evInfo.EventHandlerType, SerializedReport, method);
					evInfo.AddEventHandler(target, handler);
				} catch { }
			}
		}
		static string ToAsmName(string name) {
			return name + separator + "AssemblyName";
		}
		static string ToTypeName(string name) {
			return name + separator + "TypeName";
		}
		static object DeserializeObject(string text, TypeConverter conv) {
			try {
				return conv.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, text);
			} catch {
				return conv.ConvertFromString(text);
			}
		}
		#endregion
		# region inner classes
		private class MyComponentSource : IComponentSource {
			public MyComponentSource() {
			}
			void IComponentSource.FillContainer(ComponentContainer container) {
				object source = SerializedReport;
				FieldInfo[] fields = XRAccessor.GetFields(source, typeof(IComponent));
				foreach (FieldInfo field in fields)
					container.Add(field.GetValue(source), field.Name);
				FieldInfo dataSourceField = typeof(XtraReportBase).GetField("dataSource", BindingFlags.Instance | BindingFlags.NonPublic);
				if (dataSourceField != null) {
					object dataSource = dataSourceField.GetValue(source);
					container.Add(dataSource, dataSourceField.Name);
				}
			}
		}
		private class XRControlRefPersistence {
			object obj;
			string propName;
			public XRControlRefPersistence(object obj, string propName) {
				this.obj = obj;
				this.propName = propName;
			}
			public void SetReference(XRControl val) {
				PropInfoAccessor.SetPropertyValue(obj, propName, val);
			}
		}
		class ReportSerializationInfo {
			protected XtraReport report;
			ArrayList refCache;
			public virtual XtraReport Report { get { return report; } }
			public ArrayList RefCache { get { return refCache; } }
			public ReportSerializationInfo(XtraReport report) {
				this.report = report;
				refCache = new ArrayList();
			}
			protected ReportSerializationInfo() {
			}
			public void Clear() {
				refCache.Clear();
			}
			public virtual void UpdateReferences() {
				NestedComponentEnumerator en = new NestedComponentEnumerator(report.Bands);
				while (en.MoveNext())
					foreach (Pair<string, XRControlRefPersistence> item in refCache)
						if (item.First == en.Current.Name) {
							XRControlRefPersistence refPersistece = item.Second;
							refPersistece.SetReference(en.Current);
						}
			}
		}
		class NullSerializationInfo : ReportSerializationInfo {
			public override XtraReport Report { 
				get {
					if(report == null)
						report = new FakedReport();
					return report;
				}
			}
			public NullSerializationInfo()  {
			}
			public override void UpdateReferences() {
			}
		}
		private class ReportStack {
			Stack stack;
			public int Count {
				get { return stack.Count; }
			}
			public ReportStack() {
				stack = new Stack();
			}
			public void Push(XtraReport report) {
				stack.Push(new ReportSerializationInfo(report));
			}
			public ReportSerializationInfo Pop() {
				if (Count > 0)
					return (ReportSerializationInfo)stack.Pop();
				return nullInfo;
			}
			public ReportSerializationInfo Peek() {
				if (Count > 0)
					return (ReportSerializationInfo)stack.Peek();
				return nullInfo;
			}
		}
		#endregion
		private const string separator = "_X_";
		private XRSerializationInfoBase info;
		private string baseName = "";
		private Stack nameStack;
		private ComponentContainer container;
		private ComponentContainer Container {
			get {
				if (container == null) {
					container = new ComponentContainer(componentSource);
				}
				return container;
			}
		}
		internal bool HasSoapInfo { get { return info is XRSerializationInfo; } }
		public XRSerializer(System.Runtime.Serialization.SerializationInfo info)
			: this(new XRSerializationInfo(info)) {
		}
		public XRSerializer(XRSerializationInfoBase info) {
			this.info = info;
			nameStack = new Stack();
		}
		#region serialization
		internal string FullNameOf(string name) {
			return baseName + name;
		}
		private string ShortNameOf(string fullName) {
			return fullName.Substring(baseName.Length);
		}
		private void UpdateBaseName(string name) {
			nameStack.Push(baseName);
			baseName += name + separator;
		}
		private void RestoreBaseName() {
			baseName = (string)nameStack.Pop();
		}
		public void Serialize(string name, IXRSerializable val) {
			UpdateBaseName(name);
			val.SerializeProperties(this);
			RestoreBaseName();
		}
		public void SerializeValue(string name, object val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeValue(string name, object val, Type type) {
			info.AddValue(FullNameOf(name), val, type);
		}
		public void SerializeMargins(string name, Margins val) {
			info.AddValue(FullNameOf(name), new Rectangle(val.Left, val.Top, val.Right, val.Bottom), typeof(Rectangle));
		}
		public void SerializeEnum(string name, Enum val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeInteger(string name, int val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeSingle(string name, float val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeSize(string name, Size val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeRectangle(string name, Rectangle val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeString(string name, string val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeString(string name, string val, string defaultValue) {
			if (val != defaultValue)
				SerializeString(name, val);
		}
		public void SerializeBoolean(string name, bool val) {
			info.AddValue(FullNameOf(name), val);
		}
		public void SerializeColor(string name, Color val) {
			SerializeObject(name, val, colorConverter);
		}
		public void SerializeFont(string name, Font val) {
			SerializeObject(name, val, fontConverter);
		}
		private void SerializeObject(string name, object val, TypeConverter conv) {
			string s = conv.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, val);
			info.AddValue(FullNameOf(name), s);
		}
		internal void SerializeDataSourceReference(string name, object dataSource) {
			info.AddValue(FullNameOf(name), DataSourceToString(dataSource));
		}
		private string DataSourceToString(object dataSource) {
			if (dataSource is DataTable) {
				DataTable dataTable = (DataTable)dataSource;
				string name = Container.GetComponentName(GetDataSource(dataTable));
				return name != null && name.Length > 0 ?
					String.Format("{0}.{1}", name, dataTable.TableName) :
					dataTable.TableName;
			}
			return Container.GetComponentName(dataSource);
		}
		internal void SerializeDataSource(string name, object dataSource, object defaultValue) {
			if (dataSource != defaultValue)
				SerializeDataSourceReference(name, dataSource);
		}
		internal void SerializeReference(string name, object val) {
			string refName = Container.GetComponentName(val);
			if (refName.Length > 0) SerializeString(name, refName);
		}
		internal void SerializeXRControlReference(string name, XRControl val) {
			if (val != null && name.Length > 0)
				SerializeString(name, val.Name);
		}
		internal void SerializeType(string name, Type val) {
			SerializeString(ToAsmName(name), val.Assembly.FullName);
			SerializeString(ToTypeName(name), val.FullName);
		}
		#endregion
		#region deserialization
		internal Type DeserializeType(string name, Type defaultValue) {
			try {
				string asmName = GetPatchedAssemblyName(DeserializeString(ToAsmName(name), ""));
				Assembly asm = asmCache[asmName] as Assembly;
				if (asm == null) {
					try {
						asm = Assembly.Load(asmName);
					} catch {
						asm = DevExpress.Data.Utils.AssemblyCache.LoadWithPartialName(GetShortAssemblyName(asmName));
					}
					if (asm != null)
						asmCache[asmName] = asm;
				}
				string typeName = DeserializeString(ToTypeName(name), "");
				return asm.GetType(typeName);
			} catch {
				return defaultValue;
			}
		}
		internal object DeserializeReference(string name, object defaultValue) {
			try {
				string refName = DeserializeString(name, "");
				return (refName.Length > 0) ? Container[refName] : null;
			} catch {
				return defaultValue;
			}
		}
		internal void DeserializeXRControlReference(object obj, string name) {
			ReportSerializationInfo info = SerializedReportInfo;
			if (!nullInfo.Equals(info)) {
				string refName = DeserializeString(name, "");
				if (refName.Length > 0)
					info.RefCache.Add(new Pair<string, XRControlRefPersistence>(refName, new XRControlRefPersistence(obj, name)));
			}
		}
		internal void DeserializeEventHandlers(string[] names, object target) {
			UpdateNames(names);
			for (int i = 0; i < names.Length; i++) {
				string method = info.GetString(names[i], null);
				if (method != null)
					AddEventHandler(target, method, ShortNameOf(names[i]));
			}
		}
		private void UpdateNames(IList names) {
			for (int i = 0; i < names.Count; i++) {
				names[i] = FullNameOf((string)names[i]);
			}
		}
		internal object DeserializeDataSourceReference(string name, object defaultValue) {
			string s = info.GetString(FullNameOf(name), null);
			return s != null ? DataSourceFromString(s) : defaultValue;
		}
		internal object DeserializeDataSource(string name, object defaultValue) {
			DataSet dataSet = (DataSet)DeserializeValue("DataSet", typeof(DataSet), null);
			if (dataSet != null) {
				string dataTableName = DeserializeString("DataTableName", String.Empty);
				if (!String.IsNullOrEmpty(dataTableName)) {
					return dataSet.Tables[dataTableName];
				} else
					return dataSet;
			} else
				return DeserializeDataSourceReference(name, defaultValue);
		}
		private object DataSourceFromString(string val) {
			val = val.Trim('.');
			string[] items = val.Split('.');
			if (items.Length == 0)
				return null;
			object dataSource = Container[items[0]];
			if (items.Length == 2 && dataSource is DataSet) {
				DataTable dataTable = ((DataSet)dataSource).Tables[items[1]];
				if (dataTable != null) dataSource = dataTable;
			}
			return dataSource;
		}
		public void Deserialize(string name, IXRSerializable val) {
			UpdateBaseName(name);
			val.DeserializeProperties(this);
			RestoreBaseName();
		}
		public object DeserializeValue(string name, Type type, object defaultValue) {
			return info.GetValue(FullNameOf(name), type, defaultValue);
		}
		public Margins DeserializeMargins(string name, Margins defaultValue) {
			object obj = info.GetValue(FullNameOf(name), typeof(Rectangle), null);
			if (obj is Rectangle) {
				Rectangle r = (Rectangle)obj;
				try {
					return new Margins(r.Left, r.Width, r.Top, r.Height);
				} catch { }
			}
			return defaultValue;
		}
		public Enum DeserializeEnum(string name, Type t, Enum defaultValue) {
			return (Enum)info.GetValue(FullNameOf(name), t, defaultValue);
		}
		public int DeserializeInteger(string name, int defaultValue) {
			return info.GetInt32(FullNameOf(name), defaultValue);
		}
		public float DeserializeSingle(string name, float defaultValue) {
			return info.GetSingle(FullNameOf(name), defaultValue);
		}
		public Size DeserializeSize(string name, Size defaultValue) {
			return (Size)info.GetValue(FullNameOf(name), typeof(Size), defaultValue);
		}
		public Rectangle DeserializeRectangle(string name, Rectangle defaultValue) {
			return (Rectangle)info.GetValue(FullNameOf(name), typeof(Rectangle), defaultValue);
		}
		public string DeserializeString(string name, string defaultValue) {
			return info.GetString(FullNameOf(name), defaultValue);
		}
		public bool DeserializeBoolean(string name, bool defaultValue) {
			return info.GetBoolean(FullNameOf(name), defaultValue);
		}
		public Color DeserializeColor(string name, Color defaultValue) {
			string s = info.GetString(FullNameOf(name), null);
			if (s != null) {
				try {
					return (Color)DeserializeObject(s, colorConverter);
				} catch { }
			}
			return defaultValue;
		}
		public Font DeserializeFont(string name, Font defaultValue) {
			string s = info.GetString(FullNameOf(name), null);
			if (s != null) {
				try {
					if (!fontCache.ContainsKey(s))
						fontCache.Add(s, DeserializeObject(s, fontConverter));
					return (Font)((Font)fontCache[s]).Clone();
				} catch { }
			}
			return defaultValue;
		}
		#endregion
		public static void FillReferencedAssemblyLocations(IList<string> references) {
			AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			foreach(AssemblyName name in names) {
				if(name.Name.StartsWith("DevExpress", StringComparison.OrdinalIgnoreCase))
					try {
						references.Add(Assembly.Load(name).Location);
					} catch { }
			}
		}
		public static void FillDXAssemblyLocations(IList<string> references, params string[] assemblyNames) { 
			foreach(string assemblyName in assemblyNames) {
				Assembly asm = DevExpress.Data.Utils.AssemblyCache.LoadDXAssembly(assemblyName);
				if(asm != null)
					references.Add(asm.Location);
			} 
		}
		internal static string GetXtraReportsAssemblyLocation() {
			try {
				return Assembly.GetExecutingAssembly().Location;
			} catch {
				return string.Empty;
			}
		}
		internal static string GetAssemblyName(string assemblyName) {
			return assemblyName.Substring(0, assemblyName.IndexOf(","));
		}
		internal static Assembly GetReferencedAssembly(string name) {
			AssemblyName[] names = Assembly.GetCallingAssembly().GetReferencedAssemblies();
			for (int i = 0; i < names.Length; i++) {
				if (names[i].Name == name) {
					return Assembly.Load(names[i]);
				}
			}
			return null;
		}
		internal static Type GetReportType(Stream stream) {
			return GetReportType(stream, Assembly.GetEntryAssembly());
		}
		internal static Type GetReportType(Stream stream, Assembly entryAssembly) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			XRTypeInfo info = XRTypeInfo.Deserialize(new StreamReader(stream).ReadToEnd());
			if (!info.IsValid)
				return null;
			if (info.TypeName == typeof(XtraReport).FullName)
				return typeof(XtraReport);
			Assembly asm = GetAssemblyFromDomain(Path.GetFileNameWithoutExtension(info.AssemblyLocation));
			Type type = GetType(asm, info.TypeName);
			if (type != null)
				return type;
			type = GetType(LoadAssembly(info.AssemblyLocation), info.TypeName);
			if (type != null)
				return type;
			type = GetType(entryAssembly, info.TypeName);
			if (type != null)
				return type;
			if (entryAssembly != null)
				type = GetTypeFromReferencedAssemblies(entryAssembly, info.TypeName);
			if (type != null)
				return type;
			type = GetTypeFromCurrentAppDomain(info.TypeName);
			if (type != null)
				return type;
			return typeof(XtraReport);
		}
		static Type GetType(Assembly asm, string typeName) {
			try {
				return asm != null ? asm.GetType(typeName) : null;
			} catch {
				return null;
			}
		}
		static Assembly GetAssemblyFromDomain(string asmName) {
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					string name = assembly.GetName().Name;
					if (string.Equals(name, asmName, StringComparison.OrdinalIgnoreCase))
						return assembly;
				} catch { }
			}
			return null;
		}
		public static Type GetTypeFromReferencedAssemblies(Assembly assembly, string typeName) {
			AssemblyName[] asemblyNames = assembly.GetReferencedAssemblies();
			foreach (AssemblyName assemblyName in asemblyNames) {
				try {
					Type type = GetType(Assembly.Load(assemblyName), typeName);
					if (type != null)
						return type;
				} catch { }
			}
			return null;
		}
		static Type GetTypeFromCurrentAppDomain(string typeName) {
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				Type type = GetType(assembly, typeName);
				if (type != null)
					return type;
			}
			return null;
		}
		static Assembly LoadAssembly(string asmLocation) {
			try {
				if (System.IO.File.Exists(asmLocation))
					return Assembly.LoadFrom(asmLocation);
			} catch { }
			return null;
		}
		internal static Assembly LoadingAssembly {
			get { return loadingAssembly; }
			set { loadingAssembly = value; }
		}
		internal void SerializeItems(IList items, string itemName) {
			SerializeInteger(itemName + "Count", items.Count);
			for (int i = 0; i < items.Count; i++) {
				ObjectStorage objStorage = ObjectStorage.CreateInstance((IXRSerializable)items[i]);
				SerializeValue(itemName + i, objStorage, objStorage.GetType());
			}
		}
		internal ArrayList DeserializeItems(string itemName) {
			ArrayList items = new ArrayList();
			int itemCount = DeserializeInteger(itemName + "Count", 0);
			for (int i = 0; i < itemCount; i++) {
				object item = DeserializeValue(itemName + i, typeof(ObjectStorage), null);
				items.Add(item);
			}
			return items;
		}
#endif
	} 
}
