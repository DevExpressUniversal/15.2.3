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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Platform;
using DevExpress.XtraDiagram.Utils;
using PlatformSize = System.Windows.Size;
using PlatformPoint = System.Windows.Point;
using PlatformDoubleCollection = System.Windows.Media.DoubleCollection;
using FontHelper = DevExpress.Diagram.Core.FontHelper;
namespace DevExpress.XtraDiagram {
	public partial class DiagramItem {
		DiagramItemController IDiagramItem.Controller { get { return Controller; } }
		bool IDiagramItem.IsSelected {
			get { return IsSelected; }
			set { IsSelected = value; }
		}
		bool IDiagramItem.IsTabStop { get { return TabStop; } set { TabStop = value; } }
		double IDiagramItem.Weight {
			get { return 0; }
			set { }
		}
		PlatformSize IDiagramItem.MinSize { get { return MinSize.ToPlatformSize(); } }
		PlatformSize IDiagramItem.ActualSize {
			get { return GetItemSize().CoerceMinSize(MinSize.ToPlatformSize()); }
		}
		IList<IDiagramItem> IDiagramItem.NestedItems {
			get { return Items; }
		}
		static readonly Thickness DefaultPadding = new Thickness();
		Thickness IDiagramItem.Padding {
			get { return GetPadding(); }
			set { }
		}
		protected Thickness GetPadding() {
			return DefaultPadding;
		}
		protected virtual bool ShouldSerializePadding() { return GetPadding() != DefaultPadding; }
		PlatformSize IDiagramItem.Size {
			get { return GetItemSize(); }
			set { SetItemSize(value); }
		}
		protected virtual PlatformSize GetItemSize() { return Size.ToPlatformSize(); }
		protected virtual void SetItemSize(PlatformSize value) { Size = value.ToWinSize(); }
		PlatformPoint IDiagramItem.Position {
			get { return GetPosition(); }
			set { SetPosition(value); }
		}
		protected virtual void SetPosition(PlatformPoint value) { Position = value.ToPointFloat(); }
		protected virtual PlatformPoint GetPosition() { return Position.ToPlatformPoint(); }
		IEnumerable<PropertyDescriptor> IDiagramItem.GetEditableProperties() {
			return GetEditableProperties();
		}
		protected virtual IEnumerable<PropertyDescriptor> GetEditableProperties() { return ReflectionUtils.GetPublicProperties(this); }
		void IDiagramItem.SetCustomStyles(IDiagramItemStyle[] styles) {
			UpdateThemeStyle();
		}
		protected virtual void UpdateThemeStyle() { }
		protected virtual void InvalidateVisual() { }
		FontWeight IDiagramItem.FontWeight {
			get { return FontHelper.IsBoldToFontWeight(Appearance.Font.Bold); }
			set { Appearance.Update(fontStyle: Appearance.Font.Style.Update(bold: FontHelper.FontWeightToIsBold(value))); }
		}
		protected virtual bool ShouldSerializeFontWeight() {
			if(IsDisposing) return false;
			return Appearance.Font.Bold;
		}
		FontFamily IDiagramItem.FontFamily {
			get { return new FontFamily(Appearance.Font.FontFamily.Name); }
			set { Appearance.Update(fontFamily: value.FamilyNames.Values.First()); }
		}
		protected virtual bool ShouldSerializeFontFamily() {
			if(IsDisposing) return false;
			return Appearance.Font.FontFamily.Name != AppearanceObject.DefaultFont.FontFamily.Name;
		}
		FontStyle IDiagramItem.FontStyle {
			get { return FontHelper.IsItalicToFontStyle(Appearance.Font.Italic); }
			set { Appearance.Update(fontStyle: Appearance.Font.Style.Update(italic:FontHelper.FontStyleToIsItalic(value))); }
		}
		protected virtual bool ShouldSerializeFontStyle() {
			if(IsDisposing) return false;
			return Appearance.Font.Italic;
		}
		Brush IDiagramItem.Foreground {
			get { return FontTraits.ColorToBrush(Appearance.ForeColor.ToPlatformColor()); }
			set { Appearance.ForeColor = FontTraits.BrushToColor(value).ToWinColor(); }
		}
		protected virtual bool ShouldSerializeForeground() { return !Appearance.ForeColor.IsDefault(); }
		Brush IDiagramItem.Background {
			get { return FontTraits.ColorToBrush(Appearance.BackColor.ToPlatformColor()); }
			set { Appearance.BackColor = FontTraits.BrushToColor(value).ToWinColor(); }
		}
		protected virtual bool ShouldSerializeBackground() {
			if(IsDisposing) return false;
			return !Appearance.BackColor.IsDefault();
		}
		Brush IDiagramItem.Stroke {
			get { return FontTraits.ColorToBrush(Appearance.BorderColor.ToPlatformColor()); }
			set { Appearance.BorderColor = FontTraits.BrushToColor(value).ToWinColor(); }
		}
		protected virtual bool ShouldSerializeStroke() {
			if(IsDisposing) return false;
			return !Appearance.BorderColor.IsDefault();
		}
		double IDiagramItem.StrokeThickness {
			get { return Appearance.BorderSize; }
			set { Appearance.BorderSize = (int)value; }
		}
		protected virtual bool ShouldSerializeStrokeThickness() {
			if(IsDisposing) return false;
			return Appearance.BorderSize != 1;
		}
		void ResetStrokeThickness() { StrokeThickness = 1; }
		PlatformDoubleCollection IDiagramItem.StrokeDashArray {
			get { return StrokeDashArray != null ? new PlatformDoubleCollection(StrokeDashArray) : null; }
			set { StrokeDashArray = value != null ? new DoubleCollection(value) : null; }
		}
		FontStretch IDiagramItem.FontStretch {
			get { return GetFontStretch(); }
			set { }
		}
		protected virtual FontStretch GetFontStretch() {
			return default(FontStretch);
		}
		protected virtual bool ShouldSerializeFontStretch() { return GetFontStretch() != FontStretches.Normal; }
		TextDecorationCollection IDiagramItem.TextDecorations {
			get { return FontHelper.FlagsToTextDecorations(Appearance.Font.Underline, Appearance.Font.Strikeout); }
			set { Appearance.Update(fontStyle: Appearance.Font.Style.Update(underlined: FontHelper.TextDecorationsToIsUnderline(value), strikeout: FontHelper.TextDecorationsToIsStrikethrough(value))); }
		}
		protected virtual bool ShouldSerializeTextDecorations() {
			if(IsDisposing) return false;
			return Appearance.Font.Underline || Appearance.Font.Strikeout;
		}
		double IDiagramItem.FontSize {
			get { return Appearance.Font.Size; }
			set { Appearance.Update(fontSize: value); }
		}
		protected virtual bool ShouldSerializeFontSize() {
			if(IsDisposing) return false;
			return Appearance.Font.Size != AppearanceObject.DefaultFont.Size;
		}
		VerticalAlignment IDiagramItem.VerticalContentAlignment {
			get { return Appearance.ToPlatformVerticalAlignment(); }
			set { Appearance.Update(vertAlignment: value); }
		}
		protected virtual bool ShouldSerializeVerticalContentAlignment() {
			if(IsDisposing) return false;
			return Appearance.TextOptions.VAlignment != VertAlignment.Default;
		}
		TextAlignment IDiagramItem.TextAlignment {
			get { return Appearance.ToPlatformTextAlignment(); }
			set { Appearance.Update(horzAlignment: value); }
		}
		protected virtual bool ShouldSerializeTextAlignment() {
			if(IsDisposing) return false;
			return Appearance.TextOptions.HAlignment != HorzAlignment.Default;
		}
		protected virtual bool ShouldSerializePosition() { return Position != PointFloat.Empty; }
		protected virtual void ResetPosition() { Position = PointFloat.Empty; }
		protected virtual bool ShouldSerializeThemeStyleId() { return ThemeStyleId != DiagramItemStyleId.DefaultStyleId; }
		protected virtual void ResetThemeStyleId() { ThemeStyleId = DiagramItemStyleId.DefaultStyleId; }
		protected virtual bool ShouldSerializeSelectionLayer() { return SelectionLayer != DefaultSelectionLayer.Instance; }
		protected virtual void ResetSelectionLayer() { SelectionLayer = DefaultSelectionLayer.Instance; }
		protected virtual bool ShouldSerializeAngle() { return MathUtils.IsNotEquals(Angle, 0d); }
		protected virtual void ResetAngle() { Angle = 0d; }
		protected virtual bool ShouldSerializeCanDelete() {
			if(CanDelete) return false;
			return true;
		}
		protected virtual void ResetCanDelete() { CanDelete = true; }
		protected virtual bool ShouldSerializeCanResize() {
			if(CanResize) return false;
			return true;
		}
		protected virtual void ResetCanResize() { CanResize = true; }
		protected virtual bool ShouldSerializeCanMove() {
			if(CanMove) return false;
			return true;
		}
		protected virtual void ResetCanMove() { CanMove = true; }
		protected virtual bool ShouldSerializeCanCopy() {
			if(CanCopy) return false;
			return true;
		}
		protected virtual void ResetCanCopy() { CanCopy = true; }
		protected virtual bool ShouldSerializeCanRotate() {
			if(CanRotate) return false;
			return true;
		}
		protected virtual void ResetCanRotate() { CanRotate = true; }
		protected virtual bool ShouldSerializeCanSnapToThisItem() {
			if(CanSnapToThisItem) return false;
			return true;
		}
		protected virtual void ResetCanSnapToThisItem() { CanSnapToThisItem = true; }
		protected virtual bool ShouldSerializeCanSnapToOtherItems() {
			if(CanSnapToOtherItems) return false;
			return true;
		}
		protected virtual bool ShouldSerializeCanSelect() {
			if(CanSelect) return false;
			return true;
		}
		protected virtual void ResetCanSnapToOtherItems() { CanSnapToOtherItems = true; }
		protected virtual bool ShouldSerializeAnchors() {
			return Anchors != Sides.None;
		}
		protected virtual void ResetAnchors() { Anchors = Sides.None; }
		protected bool ShouldSerializeWeight() { return false; }
		PropertyDescriptor IDiagramItem.GetRealProperty(PropertyDescriptor pd) {
			return GetPropertyCore(pd);
		}
		protected virtual PropertyDescriptor GetPropertyCore(PropertyDescriptor pd) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			switch(pd.Name) {
				case "IsTabStop": return properties["TabStop"];
			}
			return properties[pd.Name] ?? new DiagramItemPropertyDescriptor(pd);
		}
	}
}
