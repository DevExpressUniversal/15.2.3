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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ShapePropertiesInfo
	public class ShapePropertiesInfo : ICloneable<ShapePropertiesInfo>, ISupportsCopyFrom<ShapePropertiesInfo>, ISupportsSizeOf {
		#region Fields
		const uint maskBlackWhiteMode = 0x0000000F; 
		const uint maskShapeType = 0x00000FF0;
		const int offsetShapeType = 4;
		uint packedValues;
		#endregion
		#region Properties
		public OpenXmlBlackWhiteMode BlackAndWhiteMode {
			get { return (OpenXmlBlackWhiteMode)PackedValues.GetIntBitValue(this.packedValues, maskBlackWhiteMode); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskBlackWhiteMode, (int)value); }
		}
		public ShapeType ShapeType {
			get { return (ShapeType)PackedValues.GetIntBitValue(this.packedValues, maskShapeType, offsetShapeType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskShapeType, offsetShapeType, (int)value); }
		}
		#endregion
		#region ICloneable<ShapePropertiesInfo> Members
		public ShapePropertiesInfo Clone() {
			ShapePropertiesInfo result = new ShapePropertiesInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ShapePropertiesInfo> Members
		public void CopyFrom(ShapePropertiesInfo value) {
			Guard.ArgumentNotNull(value, "ShapePropertiesInfo");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			ShapePropertiesInfo other = obj as ShapePropertiesInfo;
			if (other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)(packedValues);
		}
	}
	#endregion
	#region ShapePropertiesInfoCache
	public class ShapePropertiesInfoCache : UniqueItemsCache<ShapePropertiesInfo> {
		public ShapePropertiesInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override ShapePropertiesInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ShapePropertiesInfo info = new ShapePropertiesInfo();
			info.ShapeType = ShapeType.Rect;
			return info;
		}
	}
	#endregion
	#region ShapePropertiesBase
	public abstract class ShapePropertiesBase : SpreadsheetUndoableIndexBasedObject<ShapePropertiesInfo>, ISupportsCopyFrom<ShapePropertiesBase>, IFillOwner, ISupportsInvalidateNotify {
		#region Fields
		readonly DrawingEffectStyle effectStyle;
		readonly InvalidateProxy innerParent;
		IDrawingFill fill;
		#endregion
		protected ShapePropertiesBase(DocumentModel documentModel)
			: base(documentModel) {
			this.innerParent = new InvalidateProxy();
			this.innerParent.NotifyTarget = this;
			this.fill = GetDefaultFill();
			this.effectStyle = new DrawingEffectStyle(DocumentModel);
		}
		protected virtual IDrawingFill GetDefaultFill() {
			return DrawingFill.Automatic;
		}
		protected internal event EventHandler Changed;
		#region Properties
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		protected ISupportsInvalidate InnerParent { get { return innerParent; } }
		public DrawingEffectStyle EffectStyle { get { return effectStyle; } }
		#region Fill
		public IDrawingFill Fill {
			get { return fill; }
			set {
				if (value == null)
					value = GetDefaultFill();
				if (fill.Equals(value))
					return;
				DrawingFillPropertyChangedHistoryItem historyItem = new DrawingFillPropertyChangedHistoryItem(DocumentModel, this, fill, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void SetDrawingFillCore(IDrawingFill value) {
			RaiseSetFill(value);
			this.fill.Parent = null;
			this.fill = value;
			this.fill.Parent = this.innerParent;
			this.innerParent.Invalidate();
		}
		#endregion
		#region BlackAndWhiteMode
		public OpenXmlBlackWhiteMode BlackAndWhiteMode {
			get { return Info.BlackAndWhiteMode; }
			set {
				if (BlackAndWhiteMode == value)
					return;
				SetPropertyValue(SetBlackAndWhiteModeCore, value);
			}
		}
		DocumentModelChangeActions SetBlackAndWhiteModeCore(ShapePropertiesInfo info, OpenXmlBlackWhiteMode value) {
			info.BlackAndWhiteMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ShapePropertiesInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ShapePropertiesInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		#endregion
		protected internal void ResetToStyle() {
			ResetToStyle(false);
		}
		protected internal void ResetToStyle(bool keepOutline) {
			DocumentModel.BeginUpdate();
			try {
				ResetToStyleCore(keepOutline);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual void ResetToStyleCore(bool keepOutline) {
			Fill = DrawingFill.Automatic;
		}
		#region SetFill
		SetFillEventHandler onSetFill;
		internal event SetFillEventHandler SetFill { add { onSetFill += value; } remove { onSetFill -= value; } }
		void RaiseSetFill(IDrawingFill value) {
			if (onSetFill != null) {
				SetFillEventArgs args = new SetFillEventArgs(value);
				onSetFill(this, args);
			}
		}
		#endregion
		#region ISupportsInvalidateNotify Members
		public void InvalidateNotify() {
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
		#endregion
		#region ISupportsCopyFrom<GroupShapeProperties> Members
		public virtual void CopyFrom(ShapePropertiesBase value) {
			Guard.ArgumentNotNull(value, "ShapePropertiesBase");
			base.CopyFrom(value);
			Fill = value.Fill.CloneTo(DocumentModel);
			effectStyle.CopyFrom(value.effectStyle);
		}
		#endregion
	}
	#endregion
	#region GroupShapeProperties
	public class GroupShapeProperties : ShapePropertiesBase, ICloneable<GroupShapeProperties>, ISupportsCopyFrom<GroupShapeProperties> {
		#region Fields
		readonly GroupTransform2D transform2D;
		#endregion
		public GroupShapeProperties(DocumentModel documentModel)
			: base(documentModel) {
			this.transform2D = new GroupTransform2D(DocumentModel);
		}
		public GroupTransform2D Transform2D { get { return transform2D; } }
		#region ICloneable<GroupShapeProperties> Members
		public GroupShapeProperties Clone() {
			GroupShapeProperties result = new GroupShapeProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<GroupShapeProperties> Members
		public void CopyFrom(GroupShapeProperties value) {
			Guard.ArgumentNotNull(value, "GroupShapeProperties");
			base.CopyFrom(value);
			transform2D.CopyFrom(value.transform2D);
		}
		#endregion
	}
	#endregion
	#region ShapeProperties
	public class ShapeProperties : ShapePropertiesBase, ICloneable<ShapeProperties>, ISupportsCopyFrom<ShapeProperties> {
		#region Fields
		readonly Outline outline;
		readonly ModelShapeCustomGeometry customGeometry;
		readonly ModelShapeGuideList presetAdjustList;
		Transform2D transform2D;
		#endregion
		public ShapeProperties(DocumentModel documentModel)
			: base(documentModel) {
			this.outline = new Outline(documentModel);
			this.outline.SetDrawingFillCore(GetDefaultFill());
			this.outline.Parent = InnerParent;
			this.transform2D = new Transform2D(DocumentModel);
			this.customGeometry = new ModelShapeCustomGeometry(DocumentModel);
			this.presetAdjustList = new ModelShapeGuideList(DocumentModel);
		}
		#region Properties
		public ModelShapeCustomGeometry CustomGeometry { get { return customGeometry; } }
		public Outline Outline { get { return outline; } }
		public bool IsAutomatic {
			get {
				return
					Fill.FillType == DrawingFillType.Automatic &&
					Outline.Fill.FillType == DrawingFillType.Automatic &&
					EffectStyle.IsDefault;
			}
		}
		#endregion
		public Transform2D Transform2D { get { return transform2D; } }
		#region ShapeType
		public ShapeType ShapeType {
			get { return Info.ShapeType; }
			set {
				if (ShapeType == value)
					return;
				SetPropertyValue(SetShapeTypeCore, value);
			}
		}
		DocumentModelChangeActions SetShapeTypeCore(ShapePropertiesInfo info, ShapeType value) {
			info.ShapeType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OutlineType
		public OutlineType OutlineType {
			get {
				if (outline.Fill.FillType == DrawingFillType.Solid)
					return Model.OutlineType.Solid;
				if (outline.Fill.FillType == DrawingFillType.Pattern)
					return Model.OutlineType.Pattern;
				if (outline.Fill.FillType == DrawingFillType.Gradient)
					return Model.OutlineType.Gradient;
				return Model.OutlineType.None;
			}
			set {
				if (OutlineType == value && outline.Fill.FillType != DrawingFillType.Automatic)
					return;
				DocumentModel.History.BeginTransaction();
				try {
					if (value == Model.OutlineType.Solid) {
						DrawingSolidFill fill = new DrawingSolidFill(DocumentModel);
						fill.Color.OriginalColor.Scheme = SchemeColorValues.Text1;
						outline.Fill = fill;
					}
					else if (value == Model.OutlineType.Pattern) {
						DrawingPatternFill fill = new DrawingPatternFill(DocumentModel);
						fill.PatternType = DrawingPatternType.Percent5;
						fill.ForegroundColor.OriginalColor.Scheme = SchemeColorValues.Accent1;
						fill.BackgroundColor.OriginalColor.Scheme = SchemeColorValues.Background1;
						outline.Fill = fill;
					}
					else if (value == Model.OutlineType.Gradient) {
						DrawingGradientFill fill = new DrawingGradientFill(DocumentModel);
						fill.GradientStops.Add(CreateGradientStop(0, SchemeColorValues.Accent1, 66000, 160000));
						fill.GradientStops.Add(CreateGradientStop(50000, SchemeColorValues.Accent1, 44500, 160000));
						fill.GradientStops.Add(CreateGradientStop(100000, SchemeColorValues.Accent1, 23500, 160000));
						fill.Angle = DocumentModel.UnitConverter.ModelUnitsToAdjAngle(5400000);
						outline.Fill = fill;
					}
					else
						outline.Fill = DrawingFill.None;
				}
				finally {
					DocumentModel.History.EndTransaction();
				}
			}
		}
		DrawingGradientStop CreateGradientStop(int position, SchemeColorValues schemeColor, int tint, int saturationModulation) {
			DrawingGradientStop stop = new DrawingGradientStop(DocumentModel);
			stop.Position = position;
			stop.Color.BeginUpdate();
			try {
				stop.Color.OriginalColor.Scheme = schemeColor;
				stop.Color.Transforms.Add(new TintColorTransform(tint));
				stop.Color.Transforms.Add(new SaturationModulationColorTransform(saturationModulation));
			}
			finally {
				stop.Color.EndUpdate();
			}
			return stop;
		}
		public DrawingColor OutlineColor {
			get {
				if (outline.Fill.FillType == DrawingFillType.Solid)
					return ((DrawingSolidFill)outline.Fill).Color;
				if (outline.Fill.FillType == DrawingFillType.Pattern)
					return ((DrawingPatternFill)outline.Fill).ForegroundColor;
				if (outline.Fill.FillType == DrawingFillType.Gradient) {
					DrawingGradientFill fill = (DrawingGradientFill)outline.Fill;
					if (fill.GradientStops.Count > 0)
						return fill.GradientStops[0].Color;
				}
				return DrawingColor.Create(DocumentModel, DrawingColorModelInfo.CreateARGB(Color.FromArgb(0, 255, 255, 255)));
			}
			set {
				Guard.ArgumentNotNull(value, "OutlineColor");
				if (outline.Fill.FillType == DrawingFillType.Solid && OutlineColor.Equals(value))
					return;
				DocumentModel.History.BeginTransaction();
				try {
					if (outline.Fill.FillType != DrawingFillType.Solid) {
						DrawingSolidFill fill = new DrawingSolidFill(DocumentModel);
						outline.Fill = fill;
					}
					((DrawingSolidFill)outline.Fill).Color.CopyFrom(value);
				}
				finally {
					DocumentModel.History.EndTransaction();
				}
			}
		}
		public ModelShapeGuideList PresetAdjustList { get { return presetAdjustList; } }
		#endregion
		public ShapeProperties Clone() {
			ShapeProperties result = new ShapeProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ShapeProperties value) {
			Guard.ArgumentNotNull(value, "ShapeProperties");
			base.CopyFrom(value);
			Outline.CopyFrom(value.Outline);
			transform2D.CopyFrom(value.transform2D);
		}
		public void RotateCore(int newValue) {
			Transform2D.Rotation = newValue;
		}
		public void SetupShapeAdjustList(int?[] values) {
			ModelShapeCustomGeometry geometry = ShapesPresetGeometry.GetCustomGeometryByPreset(ShapeType);
			for (int i = 0; i < Math.Min(values.Length, geometry.AdjustValues.Count); i++) {
				if (!values[i].HasValue)
					continue;
				PresetAdjustList.Add(new ModelShapeGuide(geometry.AdjustValues[i].Name, "val " + values[i].Value));
			}
		}
		protected override void ResetToStyleCore(bool keepOutline) {
			base.ResetToStyleCore(keepOutline);
			if (!keepOutline)
				Outline.ResetToStyle();
		}
	}
	#endregion
}
