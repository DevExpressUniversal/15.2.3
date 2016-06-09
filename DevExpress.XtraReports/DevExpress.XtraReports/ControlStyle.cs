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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Collections;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Localization;
using System.Security.Permissions;
using DevExpress.Data.Helpers;
using System.Security;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.Generic;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	[
	Flags,
	Serializable,
	]
	public enum XRBorderSide { None = 0, Top = 1, Bottom = 2, Left = 4, Right = 8 }
	[
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority"),
	]
	public class StylePriority : StyleFlagsBase {
		#region Properties
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseFont"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseFont"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseFont { get { return UseFontCore; } set { UseFontCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseForeColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseForeColor"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseForeColor { get { return UseForeColorCore; } set { UseForeColorCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseBackColor"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseBackColor { get { return UseBackColorCore; } set { UseBackColorCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUsePadding"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UsePadding"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UsePadding { get { return UsePaddingCore; } set { UsePaddingCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseBorderColor"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseBorderColor { get { return UseBorderColorCore; } set { UseBorderColorCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseBorders"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseBorders"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseBorders { get { return UseBordersCore; } set { UseBordersCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseBorderWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseBorderWidth"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseBorderWidth { get { return UseBorderWidthCore; } set { UseBorderWidthCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseBorderDashStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseBorderDashStyle"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseBorderDashStyle { get { return UseBorderDashStyleCore; } set { UseBorderDashStyleCore = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("StylePriorityUseTextAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.StylePriority.UseTextAlignment"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public bool UseTextAlignment { get { return UseTextAlignmentCore; } set { UseTextAlignmentCore = value; } }
		#endregion
		public StylePriority() {
			UseBackColorCore = true;
			UseBorderColorCore = true;
			UseBordersCore = true;
			UseBorderDashStyleCore = true;
			UseBorderWidthCore = true;
			UseFontCore = true;
			UseForeColorCore = true;
			UsePaddingCore = true;
			UseTextAlignmentCore = true;
		}
		#region ShouldSerializeXXX
		internal bool ShouldSerializeUseFont() {
			return UseFontCore == false;
		}
		internal bool ShouldSerializeUseForeColor() {
			return UseForeColorCore == false;
		}
		internal bool ShouldSerializeUseBackColor() {
			return UseBackColorCore == false;
		}
		internal bool ShouldSerializeUseBorderColor() {
			return UseBorderColorCore == false;
		}
		internal bool ShouldSerializeUseBorders() {
			return UseBordersCore == false;
		}
		internal bool ShouldSerializeUseBorderWidth() {
			return UseBorderWidthCore == false;
		}
		internal bool ShouldSerializeUseBorderDashStyle() {
			return UseBorderDashStyleCore == false;
		}
		internal bool ShouldSerializeUseTextAlignment() {
			return UseTextAlignmentCore == false;
		}
		internal bool ShouldSerializeUsePadding() {
			return UsePaddingCore == false;
		}
		internal bool ShouldSerialize() {
			return ShouldSerializeUseBackColor() || ShouldSerializeUseBorderColor() || ShouldSerializeUseBorders() || ShouldSerializeUseBorderWidth() ||
				ShouldSerializeUseBorderDashStyle() || ShouldSerializeUseFont() || ShouldSerializeUseForeColor() || ShouldSerializeUsePadding() || ShouldSerializeUseTextAlignment();
		}
		#endregion
		protected override StyleFlagsBase CreateInstance() {
			return new StylePriority();
		}
	}
	[
	ListBindable(BindableSupport.No),
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
	SerializationContext(typeof(ReportSerializationContextBase)),
	]
	public class XRControlStyleSheet : CollectionBase, IXRSerializable, IXtraSupportDeserializeCollectionItem, IEnumerable<XRControlStyle> {
		#region fields & properties
		string fileName = string.Empty;
		XtraReport report;
		bool loading;
		Dictionary<string, XRControlStyle> styleCache = new Dictionary<string, XRControlStyle>();
		internal void ClearCache() {
			styleCache.Clear();
		}
		internal bool TryGetStyle(string name, out XRControlStyle value) {
			if(string.IsNullOrEmpty(name)) {
				value = null;
				return false;
			}
			if(styleCache.Count == 0 && Count > 0)
				FillCache();
			return styleCache.TryGetValue(name, out value);
		}
		void FillCache() {
			foreach(XRControlStyle item in this)
				if(!string.IsNullOrEmpty(item.Name) && !styleCache.ContainsKey(item.Name))
					styleCache[item.Name] = item;
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRControlStyleSheetItem")]
#endif
		public XRControlStyle this[string name] {
			get {
				XRControlStyle value;
				return TryGetStyle(name, out value) ? value : null;
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRControlStyleSheetItem")]
#endif
		public XRControlStyle this[int index] {
			get { return (XRControlStyle)InnerList[index]; }
			set { InnerList[index] = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRControlStyleSheetFileName"),
#endif
		XtraSerializableProperty(0),
		]
		public string FileName {
			get { return fileName; }
			set {
				if(fileName != value) {
					fileName = value;
					AddStylesToContainer();
					if(!File.Exists(fileName)) return;
					LoadFromFile(fileName);
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public CollectionBase Styles { get { return this; } } 
		#endregion
		public XRControlStyleSheet() {
		}
		public XRControlStyleSheet(XtraReport report) {
			this.report = report;
		}
		internal void AddStylesToContainer() {
			if(report == null || report.Site == null) return;
			foreach(XRControlStyle style in this) {
				if(style.Site == null)
					DesignTool.ForceAddToContainer(report.Site, style, style.Name);
			}
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeString("FileName", FileName, "");
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			FileName = serializer.DeserializeString("FileName", "");
			int styleCount = serializer.DeserializeInteger("StyleCount", 0);
			for (int i = 0; i < styleCount; i++) {
				string index = Convert.ToString(i);
				string name = serializer.DeserializeString("StyleName" + index, String.Empty);
				if (String.IsNullOrEmpty(name))
					continue;
				XRControlStyle style = new XRControlStyle();
				serializer.Deserialize("Style" + index, style);
				style.Name = name;
				Add(style);
			}
		}
		IList IXRSerializable.SerializableObjects { get { return this; }
		}
		#endregion
		#region XML Serialization
		public void SaveXmlToFile(string fileName) {
			FileStream fileStream = File.Create(fileName);
			try {
				SaveXmlToStream(fileStream);
			}
			finally {
				fileStream.Close();
			}
		}
		public void SaveXmlToStream(Stream stream) {
			StyleSheetXmlSerializer serializer = new StyleSheetXmlSerializer();
			serializer.SerializeRootObject(this, stream);
		}
		public void LoadFromXml(string fileName) {
			FileStream fileStream = File.Open(fileName, FileMode.Open);
			try {
				LoadFromXml(fileStream);
			}
			finally {
				fileStream.Close();
			}
		}
		public void LoadFromXml(Stream stream) {
			this.Clear();
			stream.Position = 0;
			StyleSheetXmlSerializer serializer = new StyleSheetXmlSerializer();
			serializer.DeserializeObject(this, stream, "");
		}
		#endregion
		public void SaveToStream(Stream stream) {
			CodeGeneratorAccessorBase.Instance.GenerateCSharpCode(InnerList, stream);
		}
		public void SaveToFile(string fileName) {
			if(String.IsNullOrEmpty(fileName)) return;
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			try {
				SaveToStream(fileStream);
			}
			finally {
				fileStream.Close();
			}
		}
		public void LoadFromFile(string fileName) {
			IList styles = new XRStyleLoader().LoadFromFile(fileName);
			LoadCore(styles);
		}
		public void LoadFromStream(Stream stream) {
			IList styles = new XRStyleLoader().LoadFromStream(stream);
			LoadCore(styles);
		}
		void LoadCore(IList styles) {
			if(styles != null) {
				this.Assign(styles);
				this.ApplyStyleUsings();
			}
		}
		[Obsolete("Use Add(XRControlStyle style)")]
		public void Add(string name, XRControlStyle style) {
			Add(style);
			style.Name = name;
		}
		public void Add(XRControlStyle style) {
			if(style != null && !List.Contains(style)) {
				System.Diagnostics.Debug.Assert(style.Owner == null);
				if(report != null)
					style.SyncDpi(report.Dpi);
				List.Add(style);
			}
		}
		public void AddRange(XRControlStyle[] styles) {
			for(int i = 0; i < styles.Length; i++)
				Add(styles[i]);
		}
		public void Remove(XRControlStyle style) {
			List.Remove(style);
		}
		public void CopyTo(Array array, int arrayIndex) {
			List.CopyTo(array, arrayIndex);
		}
		public int IndexOf(XRControlStyle style) { 
			return List.IndexOf(style);
		}
		public bool Contains(XRControlStyle style) { 
			return List.Contains(style);
		}
		protected override void OnClear() {
			foreach(XRControlStyle style in this)
				UnsubscribeStyleEvent(style);
			ClearReportStyles(this.GetNames());
			ClearCache();
		}
		void ClearReportStyles(params string[] styleNames) {
			if(this.report != null && !this.report.Loading && !this.loading)
				this.report.ClearStyleNamesRecursive(styleNames);
		}
		protected override void OnInsertComplete(int index, object value) {
			SubscribeStyleEvent((XRControlStyle)value);
			ClearCache();
		}
		protected override void OnRemoveComplete(int index, object value) {
			UnsubscribeStyleEvent((XRControlStyle)value);
			ClearReportStyles(((XRControlStyle)value).Name);
			ClearCache();
		}
		internal void Assign(IList styles) {
			if(styles == null) 
				return;
			this.loading = true;
			foreach(XRControlStyle style in styles) {
				XRControlStyle prevStyle = this[style.Name];
				if(prevStyle != null) {
					Remove(prevStyle);
					if(prevStyle.Site != null)
						prevStyle.Site.Container.Remove(prevStyle);
				}
				Add(style);
			}
			this.loading = false;
		}
		void OnStyleDisposed(object sender, EventArgs e) {
			XRControlStyle style = (XRControlStyle)sender;
			Remove(style);
			UnsubscribeStyleEvent(style);
		}
		void SubscribeStyleEvent(XRControlStyle style) {
			style.Disposed += OnStyleDisposed;
			style.Owner = this;
		}
		void UnsubscribeStyleEvent(XRControlStyle style) {
			style.Disposed -= OnStyleDisposed;
			style.Owner = null;
		}
		internal void DisposeStyles() {
			for(int i = this.Count - 1; i >= 0; i--)
				this[i].Dispose();
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Styles)
				return new XRControlStyle();
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Styles)
				this.Add(e.Item.Value as XRControlStyle);
		}
		#endregion
		#region IEnumerable<XRControlStyle> Members
		IEnumerator<XRControlStyle> IEnumerable<XRControlStyle>.GetEnumerator() {
			foreach(XRControlStyle item in List)
				yield return item;
		}
		#endregion
	}
	class XRStyleLoader {
		public IList LoadFromFile(string fileName) {
			if(String.IsNullOrEmpty(fileName))
				return null;
			Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try {
				return LoadFromStream(stream);
			} finally {
				stream.Close();
			}
		}
		public IList LoadFromStream(Stream stream) {
			if(stream == null || !stream.CanRead)
				return null;
			if(FileFormatDetector.CreateSoapDetector().FormatExists(stream)) {
				if(SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter)))
					return LoadFromSoapFormat(stream);
			} else if(FileFormatDetector.CreateXmlDetector().FormatExists(stream)) {
				XRControlStyleSheet sheet = new XRControlStyleSheet();
				sheet.LoadFromXml(stream);
				return sheet;
			}
			try {
				PermissionSet permissionSet = new NamedPermissionSet("FullTrust", PermissionState.Unrestricted);
				permissionSet.Demand();
			} catch(SecurityException ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				return null;
			}
			XtraReport source = Native.ReportCompiler.Compile(stream);
			if(source == null)
				throw new ArgumentException();
			return Native.XRAccessor.GetFieldValues(source, typeof(XRControlStyle));
		}
		[SecuritySafeCritical]
		IList LoadFromSoapFormat(Stream stream) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			SoapFormatter formatter = new SoapFormatter();
			formatter.Binder = new MySerializationBinder();
			ObjectStorage objectStorage = (ObjectStorage)formatter.Deserialize(stream);
			return (XRControlStyleSheet)objectStorage.SerializableObject;
		}
	}
	public interface IXRControlStyleContainer : IEnumerable<XRControlStyle> {
		XRControlStyle this[string name] { get; }
	}
	class XRControlStyleContainer : IXRControlStyleContainer, IDisposable {
		string fileName;
		Dictionary<string, XRControlStyle> externalStyles = new Dictionary<string, XRControlStyle>();
		XRControlStyleSheet styleSheet;
		public string FileName {
			get { return fileName; }
		}
		public void SetFileName(string value, float dpi) {
			if(fileName == value)
				return;
			fileName = value;
			DisposeExternalStyles();
			if(!File.Exists(fileName))
				return;
			IList styles = new XRStyleLoader().LoadFromFile(fileName);
			foreach(XRControlStyle style in styles) {
				if(string.IsNullOrEmpty(style.Name) || externalStyles.ContainsKey(style.Name))
					continue;
				style.SyncDpi(dpi);
				externalStyles[style.Name] = style;
				style.Owner = externalStyles.Values;
			}
		}
		public XRControlStyle this[string name] {
			get {
				XRControlStyle value;
				return styleSheet.TryGetStyle(name, out value) ? value :
					externalStyles.TryGetValue(name, out value) ? value : 
					null;
			}
		}
		public XRControlStyleContainer(XRControlStyleSheet styleSheet) {
			this.styleSheet = styleSheet;
		}
		public void SyncDpi(float toDpi) {
			foreach(XRControlStyle style in this)
				style.SyncDpi(toDpi);
		}
		public virtual void Dispose() {
			fileName = null;
			DisposeExternalStyles();
			styleSheet = null;
		}
		void DisposeExternalStyles() {
			foreach(XRControlStyle item in externalStyles.Values)
				item.Dispose();
			externalStyles.Clear();
		}
		#region IEnumerable<XRControlStyle> Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<XRControlStyle> GetEnumerator() {
			foreach(XRControlStyle item in externalStyles.Values)
					yield return item;
			foreach(XRControlStyle item in styleSheet)
				yield return item;
		}
		#endregion
	}
}
