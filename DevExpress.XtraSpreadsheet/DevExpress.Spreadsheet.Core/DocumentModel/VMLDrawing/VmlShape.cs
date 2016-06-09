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
using DevExpress.Office;
using DevExpress.Utils;
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region VmlShape
	public class VmlShape : SpreadsheetUndoableIndexBasedObject<VmlShapeInfo> {
		readonly VmlShadowEffect shadow; 
		VmlShapeFillProperties fill; 
		VmlLineStrokeSettings stroke; 
		VmlShapePath path; 
		VmlTextBox textbox; 
		VmlClientData clientData; 
		VmlShapeImageData imageData; 
		public VmlShape(Worksheet sheet)
			: base(sheet) {
			shadow = new VmlShadowEffect();
			fill = new VmlShapeFillProperties();
			stroke = new VmlLineStrokeSettings();
			path = new VmlShapePath();
			textbox = new VmlTextBox();
			clientData = new VmlClientData(sheet);
			clientData.LockAspectRatio = LockAspectRatio;
			Info.Type = VmlShapeType.DefaultID;
		}
		#region Properties
		public VmlShadowEffect Shadow { get { return shadow; }  }
		public VmlShapeImageData ImageData { get { return imageData; } set { imageData = value; } }
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		public VmlShapeFillProperties Fill { get { return fill; } set { fill = value; } }
		public VmlLineStrokeSettings Stroke { get { return stroke; } set { stroke = value; } }
		public VmlShapePath Path { get { return path; } set { path = value; } }
		public VmlTextBox Textbox { get { return textbox; } set { textbox = value; } }
		public VmlClientData ClientData {
			get { return clientData; }
			set {
				clientData = value;
				if (clientData != null)
					clientData.LockAspectRatio = LockAspectRatio;
			}
		}
		public bool LockAspectRatio {
			get {
				return Info.LockAspectRatio;
			}
			set {
				ClientData.LockAspectRatio = value;
				if (LockAspectRatio == value)
					return;
				SetPropertyValue((info, newValue) => {
					info.LockAspectRatio = newValue;
					return DocumentModelChangeActions.None;
				}, value);
			}
		}
		#region Type
		public string Type {
			get { return Info.Type; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(Type, value) == 0)
					return;
				SetPropertyValue(SetTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTypeCore(VmlShapeInfo info, string value) {
			info.Type = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Style
		public string Style {
			get { return Info.Style; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(Style, value) == 0)
					return;
				SetPropertyValue(SetStyleCore, value);
			}
		}
		DocumentModelChangeActions SetStyleCore(VmlShapeInfo info, string value) {
			info.Style = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Fillcolor
		public Color Fillcolor {
			get { return Info.Fillcolor; }
			set {
				if (Fillcolor == value)
					return;
				SetPropertyValue(SetFillcolorCore, value);
			}
		}
		DocumentModelChangeActions SetFillcolorCore(VmlShapeInfo info, Color value) {
			info.Fillcolor = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Filled
		public bool? Filled {
			get { return Info.Filled; }
			set {
				if (Filled == value)
					return;
				SetPropertyValue(SetFilledCore, value);
			}
		}
		DocumentModelChangeActions SetFilledCore(VmlShapeInfo info, bool? value) {
			info.Filled = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsetMode
		public VmlInsetMode InsetMode {
			get { return Info.InsetMode; }
			set {
				if (InsetMode == value)
					return;
				SetPropertyValue(SetInsetModeCore, value);
			}
		}
		DocumentModelChangeActions SetInsetModeCore(VmlShapeInfo info, VmlInsetMode value) {
			info.InsetMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Alt
		public string Alt {
			get { return Info.Alt; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(Alt, value) == 0)
					return;
				SetPropertyValue(SetAltCore, value);
			}
		}
		DocumentModelChangeActions SetAltCore(VmlShapeInfo info, string value) {
			info.Alt = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Strokecolor
		public Color Strokecolor {
			get { return Info.Strokecolor; }
			set {
				if (Strokecolor == value)
					return;
				SetPropertyValue(SetStrokecolorCore, value);
			}
		}
		DocumentModelChangeActions SetStrokecolorCore(VmlShapeInfo info, Color value) {
			info.Strokecolor = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Strokeweight
		public int Strokeweight {
			get { return Info.Strokeweight; }
			set {
				if (Strokeweight == value)
					return;
				SetPropertyValue(SetStrokeweightCore, value);
			}
		}
		DocumentModelChangeActions SetStrokeweightCore(VmlShapeInfo info, int value) {
			info.Strokeweight = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Stroked
		public bool Stroked {
			get { return Info.Stroked; }
			set {
				if (Stroked == value)
					return;
				SetPropertyValue(SetStrokedCore, value);
			}
		}
		DocumentModelChangeActions SetStrokedCore(VmlShapeInfo info, bool value) {
			info.Stroked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IsHidden
		public bool IsHidden {
			get {
				return Style.HasPart("visibility:hidden");
			}
			set {
				if (IsHidden == value)
					return;
				Style = Style.ReplacePart("visibility:", value ? "hidden" : "visible");
				ApplyChanges(DocumentModelChangeActions.Redraw);
			}
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<VmlShapeInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.VmlShapeInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.ApplyChanges(changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void CopyFrom(VmlShape sourceItem) {
			base.CopyFrom(sourceItem);
			fill.CopyFrom(sourceItem.Fill);
			shadow.CopyFrom(sourceItem.shadow);  
			stroke.CopyFrom(sourceItem.stroke);
			path.CopyFrom(sourceItem.path);
			textbox.CopyFrom(sourceItem.textbox);
			clientData.CopyFrom(sourceItem.clientData);
		}
	}
	#endregion
	#region VmlShapeInfo
	public class VmlShapeInfo : ICloneable<VmlShapeInfo>, ISupportsCopyFrom<VmlShapeInfo>, ISupportsSizeOf {
		#region Fields
		string type;
		string style; 
		Color fillcolor; 
		bool? filled;
		VmlInsetMode insetMode; 
		string alt;
		Color strokecolor;
		int strokeweight; 
		bool stroked;
		bool lockAspectRatio;
		#endregion
		#region Properties
		public string Type { get { return type; } set { type = value; } }
		public string Style { get { return style; } set { style = value; } }
		public Color Fillcolor { get { return fillcolor; } set { fillcolor = value; } }
		public bool? Filled { get { return filled; } set { filled = value; } }
		public VmlInsetMode InsetMode { get { return insetMode; } set { insetMode = value; } }
		public string Alt { get { return alt; } set { alt = value; } }
		public Color Strokecolor { get { return strokecolor; } set { strokecolor = value; } }
		public int Strokeweight { get { return strokeweight; } set { strokeweight = value; } }
		public bool Stroked { get { return stroked; } set { stroked = value; } }
		public bool LockAspectRatio { get { return lockAspectRatio; } set { lockAspectRatio = value; } }
		#endregion
		#region ICloneable<VmlShapeInfo> Members
		public VmlShapeInfo Clone() {
			VmlShapeInfo clone = new VmlShapeInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<VmlShapeInfo> Members
		public void CopyFrom(VmlShapeInfo value) {
			this.type = value.type;
			this.style = value.style;
			this.strokeweight = value.strokeweight;
			this.strokecolor = value.strokecolor;
			this.stroked = value.stroked;
			this.insetMode = value.insetMode;
			this.fillcolor = value.fillcolor;
			this.filled = value.filled;
			this.alt = value.alt;
			this.lockAspectRatio = value.lockAspectRatio;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			VmlShapeInfo info = obj as VmlShapeInfo;
			if (info == null)
				return false;
			return this.type == info.type
				&& this.style == info.style
				&& this.fillcolor == info.fillcolor
				&& this.style == info.style
				&& this.fillcolor == info.fillcolor
				&& this.filled == info.filled
				&& this.insetMode == info.insetMode
				&& this.alt == info.alt
				&& this.strokecolor == info.strokecolor
				&& this.strokeweight == info.strokeweight
				&& this.stroked == info.stroked
				&& this.lockAspectRatio == info.lockAspectRatio;
		}
		public override int GetHashCode() {
			CombinedHashCode calculator = new CombinedHashCode();
			calculator.AddObject(type);
			calculator.AddObject(style);
			calculator.AddObject(filled);
			calculator.AddObject(insetMode);
			calculator.AddObject(alt);
			calculator.AddObject(strokecolor);
			calculator.AddObject(strokeweight);
			calculator.AddObject(stroked);
			calculator.AddObject(lockAspectRatio);
			return calculator.CombinedHash32;
		}
	}
	#endregion
	#region VmlShapeInfoCache
	public class VmlShapeInfoCache : UniqueItemsCache<VmlShapeInfo> {
		public VmlShapeInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override VmlShapeInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			VmlShapeInfo item = new VmlShapeInfo();
			item.Type = VmlShapeType.DefaultID;
			item.InsetMode = VmlInsetMode.Custom;
			item.Fillcolor = DXColor.FromArgb(0xffffe1);
			item.InsetMode = VmlInsetMode.Auto;
			item.Filled = null;
			item.Strokecolor = DXColor.Black;
			item.Strokeweight = unitConverter.PointsToModelUnits(1);
			item.Stroked = true;
			item.Style = @"position:absolute;margin-left:395.25pt;margin-top:142.5pt;width:108pt;height:59.25pt;z-index:1;visibility:visible";
			item.LockAspectRatio = false;
			return item;
		}
	}
	#endregion
	#region VmlInsetMode
	public enum VmlInsetMode {
		Auto,
		Custom
	}
	#endregion
	#region VmlShapeFillProperties
	public class VmlShapeFillProperties {
		Color color; 
		Color color2; 
		float opacity; 
		float opacity2; 
		bool recolor; 
		bool rotate; 
		VmlFillMethod method; 
		VmlFillType type; 
		string title; 
		float focus; 
		float originX; 
		float originY;
		float sizeX; 
		float sizeY;
		float positionX; 
		float positionY;
		VmlImageAspect aspect; 
		public VmlShapeFillProperties() {
			color = DXColor.White;
			color2 = DXColor.White;
			opacity = 1;
			opacity2 = 1;
			focus = 0;
			recolor = false;
			rotate = false;
			method = VmlFillMethod.Sigma;
			type = VmlFillType.Solid;
		}
		#region Properties
		public Color Color { get { return color; } set { color = value; } }
		public Color Color2 { get { return color2; } set { color2 = value; } }
		public float Opacity { get { return opacity; } set { opacity = value; } }
		public float Opacity2 { get { return opacity2; } set { opacity2 = value; } }
		public bool Recolor { get { return recolor; } set { recolor = value; } }
		public bool Rotate { get { return rotate; } set { rotate = value; } }
		public VmlFillMethod Method { get { return method; } set { method = value; } }
		public VmlFillType Type { get { return type; } set { type = value; } }
		public float Focus { get { return focus; } set { focus = value; } }
		public string Title { get { return title; } set { title = value; } }
		public float OriginX { get { return originX; } set { originX = value; } }
		public float OriginY { get { return originY; } set { originY = value; } }
		public float SizeX { get { return sizeX; } set { sizeX = value; } }
		public float SizeY { get { return sizeY; } set { sizeY = value; } }
		public float PositionX { get { return positionX; } set { positionX = value; } }
		public float PositionY { get { return positionY; } set { positionY = value; } }
		public VmlImageAspect Aspect { get { return aspect; } set { aspect = value; } }
		#endregion
		public void CopyFrom(VmlShapeFillProperties source) {
			Color = source.Color;
			Color2 = source.Color2;
			Opacity = source.Opacity;
			Opacity2 = source.Opacity2;
			Recolor = source.Recolor;
			Rotate = source.Rotate;
			Method = source.Method;
			Type = source.Type;
			Title = source.Title;
			Focus = source.Focus;
			OriginX = source.OriginX;
			OriginY = source.OriginY;
			SizeX = source.SizeX;
			SizeY = source.SizeY;
			PositionX = source.PositionX;
			PositionY = source.PositionY;
			Aspect = source.Aspect;
		}
	}
	#endregion
	#region VmlFillMethod
	public enum VmlFillMethod {
		None,
		Linear,
		Sigma,
		Any,
		LinearSigma,
	}
	#endregion
	#region VmlShadowEffect
	public class VmlShadowEffect {
		Color color; 
		bool obscured; 
		bool on; 
		bool isDefault = true;
		public VmlShadowEffect() {
			color = DXColor.Gray;
			obscured = false;
			on = true;
		}
		#region Properties
		public Color Color { get { return color; } set { color = value; } }
		public bool Obscured { get { return obscured; } set { obscured = value; } }
		public bool On { get { return on; } set { on = value; } }
		public bool IsDefault { get { return isDefault; } set { isDefault = value; } }
		#endregion
		public void CopyFrom(VmlShadowEffect source) {
			Color = source.Color;
			Obscured = source.Obscured;
			On = source.On;
			isDefault = source.isDefault;
		}
	}
	#endregion
	#region VmlImageAspect
	public enum VmlImageAspect {
		Ignore,
		AtMost,
		AtLeast,
	}
	#endregion
	#region VmlImageData
	public class VmlShapeImageData {
		#region Fields
		string althref;
		bool? bilevel;
		string blacklevel;
		Color chromakey;
		string cropbottom;
		string cropleft;
		string cropright;
		string croptop;
		bool? detectmouseclick;
		Color embosscolor;
		string gain;
		string gamma;
		bool? grayscale;
		string href;
		string id;
		float? movie;
		float? oleid;
		Color recolortarget;
		string src;
		string title;
		OfficeImage image; 
		#endregion
		#region Properties
		public string Althref { get { return althref; } set { althref = value; } }
		public bool? Bilevel { get { return bilevel; } set { bilevel = value; } }
		public string Blacklevel { get { return blacklevel; } set { blacklevel = value; } }
		public Color Chromakey { get { return chromakey; } set { chromakey = value; } }
		public string Cropbottom { get { return cropbottom; } set { cropbottom = value; } }
		public string Cropleft { get { return cropleft; } set { cropleft = value; } }
		public string Cropright { get { return cropright; } set { cropright = value; } }
		public string Croptop { get { return croptop; } set { croptop = value; } }
		public bool? Detectmouseclick { get { return detectmouseclick; } set { detectmouseclick = value; } }
		public Color Embosscolor { get { return embosscolor; } set { embosscolor = value; } }
		public string Gain { get { return gain; } set { gain = value; } }
		public string Gamma { get { return gamma; } set { gamma = value; } }
		public bool? Grayscale { get { return grayscale; } set { grayscale = value; } }
		public string Href { get { return href; } set { href = value; } }
		public string Id { get { return id; } set { id = value; } }
		public float? Movie { get { return movie; } set { movie = value; } }
		public float? Oleid { get { return oleid; } set { oleid = value; } }
		public Color Recolortarget { get { return recolortarget; } set { recolortarget = value; } }
		public string Src { get { return src; } set { src = value; } }
		public string Title { get { return title; } set { title = value; } }
		public OfficeImage Image { get { return image; } set { image = value; } }
		#endregion
	}
	#endregion
}
