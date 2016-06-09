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
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class FillStyleBase : ChartElement, IXtraSupportCreateContentPropertyValue, IXtraSupportAfterDeserialize {
		public class OptionsCache {
			List<FillOptionsBase> list = new List<FillOptionsBase>();
			public int Count { get { return list.Count; } }
			public FillOptionsBase this[int index] { get { return (FillOptionsBase)list[index]; } }
			public OptionsCache() {
			}
			public FillOptionsBase GetInstance(Type optionsType) {
				for(int i = 0; i < list.Count; i++)
					if(list[i].GetType().Equals(optionsType))
						return (FillOptionsBase)list[i];				
				return null;
			}
			public void AddOptions(FillOptionsBase options, FillStyleBase style) {
				options.SetStyle(style);
				list.Add(options);
			}
			public void Replace(FillOptionsBase options) {
				if(options == null)
					throw new ArgumentNullException("options");
				FillOptionsBase oldOptions = GetInstance(options.GetType());
				if(oldOptions != null) {
					list.Remove(oldOptions);
					oldOptions.Dispose();
				}
				list.Add(options);
			}
			public void Clear() {
				for(int i = 0; i < Count; i++)
					this[i].Dispose();
				list.Clear();
			}
		}
		protected class XmlKeys {
			public const string FillMode = "FillMode";
			public const string Options = "Options";
		}
		FillOptionsBase options;
		AspxSerializerWrapper<FillOptionsBase> optionsSerializerWrapper;
		OptionsCache optionsCache = new OptionsCache();
		protected OptionsCache FillOptionsCache { get { return this.optionsCache; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FillStyleBaseOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FillStyleBase.Options"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public FillOptionsBase Options {
			get { return this.options; }
			set {
				if(!Loading)
					throw new MemberAccessException(ChartLocalizer.GetString(ChartStringId.MsgDesignTimeOnlySetting));
				SendNotification(new ElementWillChangeNotification(this));
				this.options = value;
				if(this.options != null) {
					this.options.SetStyle(this);
					this.optionsCache.Replace(this.options);
				}
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty
		]
		public IList OptionsSerializable { get { return optionsSerializerWrapper; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")
		]
		public string FillOptionsTypeName {
			get { return String.Empty; }
			set {}
		}
		protected FillStyleBase(ChartElement owner) : this(owner, Color.Empty) {
		}
		protected FillStyleBase(ChartElement owner, Color color2) : base(owner) {
			optionsSerializerWrapper = new AspxSerializerWrapper<FillOptionsBase>(delegate() { return Options; }, 
				delegate(FillOptionsBase value) { Options = value; });
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Options" ? ShouldSerializeOptionsCore() : base.XtraShouldSerialize(propertyName);
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return e.Item.Name == "Options" ? XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "Options")
				Options = (FillOptionsBase)e.Item.Value;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeFillOptionsTypeName() {
			return false;
		}
		bool ShouldSerializeOptionsCore() {
			return options != null && options.ShouldSerialize();
		}
		bool ShouldSerializeOptions() {
			return ShouldSerializeOptionsCore() && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		void ResetOptions() {
			Options = null;
		}
		bool ShouldSerializeOptionsSerializable() {
			return ShouldSerializeOptionsCore() && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeOptions() || ShouldSerializeOptionsSerializable();
		}
		#endregion
		bool FillOptionsEquals(FillOptionsBase options) {
			return this.options == null ? options == null : this.options.Equals(options);
		}
		protected void SetFillOptions(FillOptionsBase options) {
			this.options = options;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed)
				this.optionsCache.Clear();
			base.Dispose(disposing);
		}
		protected internal virtual void ReadFromXml(XmlReader xmlReader) {
		}
		public override string ToString() {
			return "(FillStyle)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FillStyleBase style = obj as FillStyleBase;
			if(style == null)
				return;
			for(int i = 0; i < style.optionsCache.Count; i++) {
				FillOptionsBase styleOptions = style.optionsCache[i];
				if(styleOptions == null) 
					continue;
				FillOptionsBase options = this.optionsCache.GetInstance(styleOptions.GetType());
				if(options == null) 
					continue;
				options.Assign(styleOptions);
			}
		}
		public override bool Equals(object obj) {
			FillStyleBase style = obj as FillStyleBase;
			return 
				style != null &&
				FillOptionsEquals(style.Options);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum FillMode {
		Empty,
		Solid,
		Gradient,
		Hatch
	}	
	public abstract class FillStyle2D : FillStyleBase {
		FillMode defaultFillMode;
		FillMode fillMode;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FillStyle2DFillMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FillStyle2D.FillMode"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public FillMode FillMode { 
			get { return this.fillMode; } 
			set {
				if(fillMode == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				SetFillMode(value);
				RaiseControlChanged();
			}
		}
		protected FillStyle2D(ChartElement owner) : this(owner, Color.Empty) {
		}
		protected FillStyle2D(ChartElement owner, Color color2) : this(owner, color2, FillMode.Empty) {
		}
		protected FillStyle2D(ChartElement owner, Color color2, FillMode defaultFillMode) : base(owner) {
			this.fillMode = defaultFillMode;
			this.defaultFillMode = defaultFillMode;
			FillOptionsCache.AddOptions(new HatchFillOptions(color2), this);
			FillOptionsCache.AddOptions(new SolidFillOptions(), this);
			GradientFillOptionsBase gradientFillOptions = (GradientFillOptionsBase)Activator.CreateInstance(GetGradientFillOptionsType());
			gradientFillOptions.SetColor2AndInitDefaultColor2(color2);
			FillOptionsCache.AddOptions(gradientFillOptions, this);
			SetFillOptions(FillOptionsCache.GetInstance(FillMode2Type(this.fillMode)));
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "FillMode")
				return ShouldSerializeFillMode();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeFillMode() {
			return this.fillMode != this.defaultFillMode;
		}
		void ResetFillMode() {
			FillMode = this.defaultFillMode;
		}
		protected internal override bool ShouldSerialize() {
			return 
				base.ShouldSerialize() || 
				ShouldSerializeFillMode();
		}
		#endregion
		Type FillMode2Type(FillMode mode) {
			switch(mode) {
				case FillMode.Empty:
					return null;
				case FillMode.Solid:
					return typeof(SolidFillOptions);
				case FillMode.Hatch:
					return typeof(HatchFillOptions);
				case FillMode.Gradient:
					return GetGradientFillOptionsType();
				default:
					throw new DefaultSwitchException();
			}
		}
		void SetFillMode(FillMode mode) {
			this.fillMode = mode;
			SetFillOptions(FillOptionsCache.GetInstance(FillMode2Type(this.fillMode)));
		}
		protected abstract Type GetGradientFillOptionsType();
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			SetFillMode((FillMode)XmlUtils.ReadEnum(xmlReader, XmlKeys.FillMode, typeof(FillMode)));
			if(Options != null) {
				xmlReader.ReadStartElement(XmlKeys.Options);
				Options.ReadFromXml(xmlReader);
				xmlReader.ReadEndElement();
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FillStyle2D style = obj as FillStyle2D;
			if (style == null)
				return;
			SetFillMode(style.FillMode);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			FillStyle2D style = obj as FillStyle2D;
			return
				style != null &&
				this.fillMode == style.fillMode;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RectangleFillStyle : FillStyle2D {
		static internal RectangleFillStyle Empty { get { return new RectangleFillStyle(null); } }
		internal RectangleFillStyle(ChartElement owner) : base(owner) {
		}
		internal RectangleFillStyle(ChartElement owner, Color color2) : base(owner, color2) {
		}
		internal RectangleFillStyle(ChartElement owner, Color color2, FillMode defaultFillMode)  : base(owner, color2, defaultFillMode) {
		}
		protected override Type GetGradientFillOptionsType() {
			return typeof(RectangleGradientFillOptions);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RectangleFillStyle(null, Color.Empty);
		}
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			if (FillMode == FillMode.Empty || rect.Width == 0 || rect.Height == 0 || gradientRect.Width == 0 || gradientRect.Height == 0)
				return null;
			else
				return Options.CreateRectangleGraphicsCommand(rect, gradientRect, color, color2);
		}
		internal void Render(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			if (FillMode == FillMode.Empty || rect.Width == 0 || rect.Height == 0 || gradientRect.Width == 0 || gradientRect.Height == 0)
				return;
			Options.RenderRectangle(renderer, rect, gradientRect, color, color2);
		}
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect, Color color, Color color2) {
			return CreateGraphicsCommand(rect, rect, color, color2);
		}
		internal void Render(IRenderer renderer, RectangleF rect, Color color, Color color2) {
			Render(renderer, rect, rect, color, color2);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			RectangleFillStyle style = obj as RectangleFillStyle;
			return style != null && base.Equals(obj);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PolygonFillStyle : FillStyle2D {
		internal PolygonFillStyle(ChartElement owner) : base(owner) {
		}
		internal PolygonFillStyle(ChartElement owner, Color color2) : base(owner, color2) {
		}
		internal PolygonFillStyle(ChartElement owner, Color color2, FillMode defaultFillMode) : base(owner, color2, defaultFillMode) {
		}
		protected override Type GetGradientFillOptionsType() {
			return typeof(PolygonGradientFillOptions);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PolygonFillStyle(null, Color.Empty);
		}
		internal void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			if(FillMode != FillMode.Empty)
				Options.FillPolygon(gr, polygon, color, color2);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			PolygonFillStyle style = obj as PolygonFillStyle;
			return 
				style != null && 
				base.Equals(obj);
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum FillMode3D {
		Empty,
		Solid,
		Gradient
	}
	public abstract class FillStyle3D : FillStyleBase {
		FillMode3D defaultFillMode;
		FillMode3D fillMode;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FillStyle3DFillMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FillStyle3D.FillMode"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public FillMode3D FillMode { 
			get { return fillMode; } 
			set {
				SendNotification(new ElementWillChangeNotification(this));
				SetFillMode(value);
				RaiseControlChanged();
			}
		}
		protected FillStyle3D(ChartElement owner) : this(owner, Color.Empty) {
		}
		protected FillStyle3D(ChartElement owner, Color color2) : this(owner, color2, FillMode3D.Empty) {
		}
		protected FillStyle3D(ChartElement owner, Color color2, FillMode3D defaultFillMode) : base(owner) {
			this.fillMode = defaultFillMode;
			this.defaultFillMode = defaultFillMode;
			FillOptionsCache.AddOptions(new SolidFillOptions(), this);
			GradientFillOptionsBase gradientFillOptions = (GradientFillOptionsBase)Activator.CreateInstance(GetGradientFillOptionsType());
			gradientFillOptions.SetColor2AndInitDefaultColor2(color2);
			FillOptionsCache.AddOptions(gradientFillOptions, this);
			SetFillOptions(FillOptionsCache.GetInstance(FillMode2Type(this.fillMode)));
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "FillMode")
				return ShouldSerializeFillMode();
			else
				return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeFillMode() {
			return this.fillMode != this.defaultFillMode;
		}
		void ResetFillMode() {
			FillMode = this.defaultFillMode;
		}
		protected internal override bool ShouldSerialize() {
			return 
				base.ShouldSerialize() || 
				ShouldSerializeFillMode();
		}
		#endregion
		Type FillMode2Type(FillMode3D mode) {
			switch(mode) {
				case FillMode3D.Empty:
					return null;
				case FillMode3D.Solid:
					return typeof(SolidFillOptions);
				case FillMode3D.Gradient:
					return GetGradientFillOptionsType();
				default:
					throw new DefaultSwitchException();
			}
		}
		void SetFillMode(FillMode3D mode) {
			this.fillMode = mode;
			SetFillOptions(FillOptionsCache.GetInstance(FillMode2Type(this.fillMode)));
		}
		protected abstract Type GetGradientFillOptionsType();
		protected internal override void ReadFromXml(XmlReader xmlReader) {
			SetFillMode((FillMode3D)XmlUtils.ReadEnum(xmlReader, XmlKeys.FillMode, typeof(FillMode3D)));
			if(Options != null) {
				xmlReader.ReadStartElement(XmlKeys.Options);
				Options.ReadFromXml(xmlReader);
				xmlReader.ReadEndElement();
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FillStyle3D style = obj as FillStyle3D;
			if (style == null)
				return;
			SetFillMode(style.FillMode);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			FillStyle3D style = obj as FillStyle3D;
			return style != null && fillMode == style.fillMode;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RectangleFillStyle3D : FillStyle3D {
		internal RectangleFillStyle3D(ChartElement owner) : base(owner) {
		}
		internal RectangleFillStyle3D(ChartElement owner, Color color2) : base(owner, color2) {
		}
		internal RectangleFillStyle3D(ChartElement owner, Color color2, FillMode3D defaultFillMode)  : base(owner, color2, defaultFillMode) {
		}
		protected override Type GetGradientFillOptionsType() {
			return typeof(RectangleGradientFillOptions);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RectangleFillStyle3D(null, Color.Empty);
		}
		internal PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			return (FillMode == FillMode3D.Empty) ? new PlanePolygon[] { polygon } :
				Options.FillPlanePolygon(polygon, gradientRect, color, color2);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj.GetType() == typeof(RectangleFillStyle3D);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PolygonFillStyle3D : FillStyle3D {
		internal PolygonFillStyle3D(ChartElement owner)
			: base(owner) {
		}
		internal PolygonFillStyle3D(ChartElement owner, Color color2)
			: base(owner, color2) {
		}
		internal PolygonFillStyle3D(ChartElement owner, Color color2, FillMode3D defaultFillMode)
			: base(owner, color2, defaultFillMode) {
		}
		protected override Type GetGradientFillOptionsType() {
			return typeof(PolygonGradientFillOptions);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PolygonFillStyle3D(null, Color.Empty);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj.GetType() == typeof(PolygonFillStyle3D);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
