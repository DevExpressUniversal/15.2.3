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
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Data;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using DevExpress.XtraReports.Native;
using System.Xml;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using System.Security.Permissions;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI 
{
	[
	Serializable(),
	TypeConverter(typeof(DevExpress.Utils.Design.BinaryTypeConverter)),
	]
	public class SerializableString : ISerializable 
	{
		private string val = String.Empty;
		public SerializableString() {
		}
		public SerializableString(string value) {
			this.val = value;
		}
		public SerializableString(SerializationInfo info, StreamingContext context) {
			val = info.GetString("Value");
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AssemblyName = this.GetType().Assembly.GetName().Name;
			info.AddValue("Value", val);
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("SerializableStringValue"),
#endif
		XtraSerializableProperty,
		]
		public string Value { get { return val == null ? String.Empty : val; } set { val = value; } }
	}
}
namespace DevExpress.XtraReports.Serialization 
{
	internal interface IComponentSource 
	{
		void FillContainer(ComponentContainer container);
	}
	internal class DesignComponentSource : IComponentSource 
	{
		private ISite site;
		public DesignComponentSource(ISite site) {
			this.site = site;
		}
		void IComponentSource.FillContainer(ComponentContainer container) {
			foreach(IComponent c in site.Container.Components) {
				container.Add(c, c.Site.Name);
			}
		}
	}
	[Serializable]
	public class XRSerializationException : Exception {
		static string GetErrorText(System.CodeDom.Compiler.CompilerErrorCollection errors) {
			string text = "";
			foreach(System.CodeDom.Compiler.CompilerError error in errors) {
				if(error.IsWarning)
					continue;
				text += error.Line + ":  " + error.ErrorText + "\r\n";
			}
			return text;
		}
		public XRSerializationException(string message) : base(message) {
		}
		protected XRSerializationException(SerializationInfo info, StreamingContext context) : base(info, context) { 
		}
		internal XRSerializationException(System.CodeDom.Compiler.CompilerErrorCollection errors) : base(GetErrorText(errors)) {
		}
	}
	public class XRSerializerEventArgs 
	{
		XRSerializer serializer;
		System.Resources.ResourceManager resourceManager;
		public XRSerializer Serializer { get { return serializer; }
		}
		public System.Resources.ResourceManager ResourceManager {
			get { return resourceManager; }
		}
		public XRSerializerEventArgs(XRSerializer serializer) {
			this.serializer = serializer;
		}
		public XRSerializerEventArgs(System.Resources.ResourceManager resourceManager) {
			this.resourceManager = resourceManager;
		}
	} 
	public delegate void XRSerializerEventHandler(object sender, XRSerializerEventArgs e);
	internal class ComponentContainer 
	{
		private Hashtable components = new Hashtable();
		public object this[string name] { get { return components[name]; }
		}
		public ComponentContainer(IComponentSource componentSource) {
			componentSource.FillContainer(this);
		}
		public void Add(object component, string name) {
			if(component != null && name.Length > 0) 
				components.Add(name, component);
		}
		public string GetComponentName(object comp) {
			foreach(string name in components.Keys) {
				if( Comparer.Equals(comp,components[name]) )
					return name;
			}
			return "";
		}
	}
	public class XRSerializationInfo : XRSerializationInfoBase {
		SerializationInfo info;
		Hashtable serializationHT;
		protected Hashtable SerializationHT { 
			get {
				if(serializationHT == null) {
					serializationHT = new Hashtable();
					SerializationInfoEnumerator en = info.GetEnumerator();
					while(en.MoveNext())
						serializationHT.Add(en.Name, en.Value);
				}
				return serializationHT;
			}
		}
		public XRSerializationInfo(SerializationInfo info) {
			this.info = info;
		}
		protected override bool ContainsKey(string name) {
			return SerializationHT.Contains(name);
		}
		protected override object GetValue(string name) {
			return SerializationHT[name];
		}
		protected override void AddValueInternal(string name, object value) {
			info.AddValue(name, value);
		}
		public override void AddValue(string name, object value, Type type) {
			info.AddValue(name, value, type);
		}
	} 
	internal abstract class ObjectStorageBase {
		#region static
		static protected ArrayList GetXRObjects(IList objects) {
			ArrayList xrObjects = new ArrayList();
			foreach(ObjectStorage obj in objects) {
				if(obj.SerializableObject != null) {
					obj.InitControls();
					xrObjects.Add(obj.SerializableObject);
				}
			}
			return xrObjects;
		}
		#endregion
		protected ArrayList items = new ArrayList();
		protected XRSerializer serializer;
		protected IXRSerializable fSerializableObject;
		protected virtual string ItemName { get { return "Item"; }
		}
		public IXRSerializable SerializableObject { get { return fSerializableObject; } 
		}
		protected ObjectStorageBase(IXRSerializable obj) {
			System.Diagnostics.Debug.Assert(obj != null);
			this.fSerializableObject = obj;
		}
		protected ObjectStorageBase(SerializationInfo info, StreamingContext context) {
			serializer = new XRSerializer(info);
			DeserializeComponent();
		}
		protected abstract void DeserializeComponent();
	} 
	[Serializable()]
	internal  class ObjectStorage : ObjectStorageBase, ISerializable {
		#region static
		static void CopyControls(IList source, IList dest) {
			dest.Clear();
			foreach(IXRSerializable obj in source)
				dest.Add(obj);
		}
		#endregion
		public static ObjectStorage CreateInstance(IXRSerializable obj) {
			return new ObjectStorage(obj);
		}
		public ObjectStorage(IXRSerializable obj) : base(obj) {
		}
		protected ObjectStorage(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
		protected override void DeserializeComponent() {
			Type t = serializer.DeserializeType("Type", null);
			if(t == null) return;
			try {
				fSerializableObject = Activator.CreateInstance(t) as IXRSerializable;
				if(fSerializableObject != null) {
					fSerializableObject.DeserializeProperties(serializer);
					items = serializer.DeserializeItems(ItemName);
				}
			} catch {}
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			SerializeComponent(info, context);
		}
		protected virtual void SerializeComponent(SerializationInfo info, StreamingContext context) {
			if(fSerializableObject != null) {
				serializer = new XRSerializer(info);
				serializer.SerializeType("Type", fSerializableObject.GetType());
				fSerializableObject.SerializeProperties(serializer);
				serializer.SerializeItems(fSerializableObject.SerializableObjects, ItemName);
			}
		}
		public virtual void InitControls() {
			System.Diagnostics.Debug.Assert(fSerializableObject != null);
			CopyControls(GetXRObjects(items), fSerializableObject.SerializableObjects);
		}
	}
	[Serializable]
	internal class ControlStorage : ObjectStorage 
	{
		public ControlStorage(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
		public ControlStorage(XRControl control) : base(control) {
		}
		public void InitControlStructure(XRControl control) {
			((IXRSerializable)control).DeserializeProperties(serializer);
			ArrayList controls = GetXRObjects(items);
			control.Controls.CopyFrom((XRControl[])controls.ToArray(typeof(XRControl)));
		}
	}
	[Serializable()]
	internal  class ReportStorage : ObjectStorageBase, ISerializable
	{
		protected override string ItemName { get { return "Band"; }
		}
		public ReportStorage(XtraReport report) : base(report) {
		}
		public ReportStorage(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
		protected override void DeserializeComponent() {
			items = serializer.DeserializeItems(ItemName);
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			if(fSerializableObject != null) {
				serializer = new XRSerializer(info);
				((XtraReport)fSerializableObject).OnSerialize( new XRSerializerEventArgs(serializer) );
				serializer.SerializeItems(fSerializableObject.SerializableObjects, ItemName);
				fSerializableObject.SerializeProperties(serializer);
			}
		}
		public void InitReportStructure(XtraReport report) {
			((IXRSerializable)report).DeserializeProperties(serializer);
			report.OnDeserialize(new XRSerializerEventArgs(serializer));
			ArrayList bands = GetXRObjects(items);
			report.Bands.CopyFrom((Band[])bands.ToArray(typeof(Band)));
		}
	}
	internal class MySerializationBinder : SerializationBinder {
		static System.Collections.Generic.Dictionary<string, string> types = new System.Collections.Generic.Dictionary<string, string>();
		static MySerializationBinder() {
			types.Add("DevExpress.XtraPrinting.BorderSide", typeof(DevExpress.XtraPrinting.BorderSide).Assembly.FullName);
			types.Add("DevExpress.XtraPrinting.TextAlignment", typeof(DevExpress.XtraPrinting.TextAlignment).Assembly.FullName);
			types.Add("DevExpress.XtraReports.UI.LineDirection", typeof(DevExpress.XtraReports.UI.LineDirection).Assembly.FullName);
			types.Add("DevExpress.XtraPrinting.PageInfo", typeof(DevExpress.XtraPrinting.PageInfo).Assembly.FullName);
			types.Add("DevExpress.XtraPrinting.ImageSizeMode", typeof(DevExpress.XtraPrinting.ImageSizeMode).Assembly.FullName);
		}
		public override Type BindToType(string assemblyName, string typeName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			string name = assembly.GetName().Name;
			assemblyName = XRSerializer.GetPatchedAssemblyName(assemblyName);
			if(types.ContainsKey(typeName)) {
				assemblyName = types[typeName];
			}
			if(assemblyName.IndexOf(name) >= 0) {
				return typeName.IndexOf("ObjectStorage") >= 0 ? typeof(ObjectStorage) :
					typeName.IndexOf("ReportStorage") >= 0 ? typeof(ReportStorage) :
					typeName.IndexOf("ControlStorage") > 0 ? typeof(ControlStorage) :
					assembly.GetType(typeName);
			}
			Assembly refAssembly = XRSerializer.GetReferencedAssembly(XRSerializer.GetAssemblyName(assemblyName));
			Type t = refAssembly != null ? refAssembly.GetType(typeName) : Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
			if(t == null && XRSerializer.GetAssemblyName(assemblyName) == AssemblyInfo.SRAssemblyPrinting) {
				t = typeof(DevExpress.Data.ResFinder).Assembly.GetType(typeName);
				if(t == null)
					t = typeof(DevExpress.Printing.ResFinder).Assembly.GetType(typeName);
			}
			return t != null ? t : XRSerializer.LoadingAssembly != null ? XRSerializer.LoadingAssembly.GetType(typeName) : null;
		}
	}
}
