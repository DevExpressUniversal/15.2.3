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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm.DataAnnotations;
namespace DevExpress.Xpf.Editors.Internal {
	public class PopupBrushValue : BindableBase {
		object brush;
		BrushType brushType;
		readonly Locker invalidateBrushLocker = new Locker();
		public event EventHandler BrushTypeChanged;
		public BrushType BrushType {
			get { return brushType; }
			set { SetProperty(ref brushType, value, () => BrushType, RaiseBrushTypeChanged); }
		}
		[Browsable(false)]
		public object Brush {
			get { return brush; }
			set { SetProperty(ref brush, value, () => Brush, InvalidateParamsInternal); }
		}
		void RaiseBrushTypeChanged() {
			if (BrushTypeChanged != null)
				BrushTypeChanged(this, EventArgs.Empty);
		}
		protected virtual void InvalidateParams() {
		}
		protected virtual void InvalidateBrush() {
		}
		protected void InvalidateBrushInternal() {
			invalidateBrushLocker.DoLockedActionIfNotLocked(InvalidateBrush);
		}
		protected void InvalidateParamsInternal() {
			invalidateBrushLocker.DoLockedActionIfNotLocked(InvalidateParams);
		}
	}
	public class PopupSolidColorBrushValue : PopupBrushValue {
		public SolidColorBrush Color {
			get { return Brush.ToSolidColorBrush(); }
			set {
				if (Equals(Brush, value))
					return;
				Brush = value;
				RaisePropertyChanged(() => Color);
			}
		}
	}
	[MetadataType(typeof(PopupGradientBrushValueMetadata))]
	public abstract class PopupGradientBrushValue : PopupBrushValue {
		BrushMappingMode mappingMode;
		GradientSpreadMethod spreadMethod;
		public BrushMappingMode MappingMode {
			get { return mappingMode; }
			set { SetProperty(ref mappingMode, value, () => MappingMode, InvalidateBrushInternal); }
		}
		public GradientSpreadMethod SpreadMethod {
			get { return spreadMethod; }
			set { SetProperty(ref spreadMethod, value, () => SpreadMethod, InvalidateBrushInternal); }
		}
		protected override void InvalidateBrush() {
			GradientBrush brush = (GradientBrush)Brush;
			if (brush == null)
				return;
			brush.MappingMode = MappingMode;
			brush.SpreadMethod = spreadMethod;
		}
		protected override void InvalidateParams() {
			base.InvalidateParams();
			GradientBrush brush = (GradientBrush)Brush;
			MappingMode = brush.MappingMode;
			SpreadMethod = brush.SpreadMethod;
		}
	}
	[MetadataType(typeof(PopupLinearGradientBrushValueMetadata))]
	public class PopupLinearGradientBrushValue : PopupGradientBrushValue {
		Point startPoint;
		Point endPoint;
		public LinearGradientBrush GradientBrush {
			get { return Brush as LinearGradientBrush; }
			set { Brush = value; }
		}
		public Point StartPoint {
			get { return startPoint; }
			set { SetProperty(ref startPoint, value, () => StartPoint, InvalidateBrushInternal); }
		}
		public Point EndPoint {
			get { return endPoint; }
			set { SetProperty(ref endPoint, value, () => EndPoint, InvalidateBrushInternal); }
		}
		protected override void InvalidateBrush() {
			base.InvalidateBrush();
			LinearGradientBrush brush = GradientBrush;
			if (brush == null)
				return;
			brush.StartPoint = StartPoint;
			brush.EndPoint = EndPoint;
		}
		protected override void InvalidateParams() {
			base.InvalidateParams();
			StartPoint = GradientBrush.StartPoint;
			EndPoint = GradientBrush.EndPoint;
		}
	}
	[MetadataType(typeof(PopupRadialGradientBrushValueMetadata))]
	public class PopupRadialGradientBrushValue : PopupGradientBrushValue {
		Point gradientOrigin;
		Point center;
		double radiusX;
		double radiusY;
		public RadialGradientBrush GradientBrush {
			get { return Brush as RadialGradientBrush; }
			set { Brush = value; }
		}
		public Point GradientOrigin {
			get { return gradientOrigin; }
			set { SetProperty(ref gradientOrigin, value, () => GradientOrigin, InvalidateBrushInternal); }
		}
		public Point Center {
			get { return center; }
			set { SetProperty(ref center, value, () => Center, InvalidateBrushInternal); }
		}
		public double RadiusX {
			get { return radiusX; }
			set { SetProperty(ref radiusX, value, () => RadiusX, InvalidateBrushInternal); }
		}
		public double RadiusY {
			get { return radiusY; }
			set { SetProperty(ref radiusY, value, () => RadiusY, InvalidateBrushInternal); }
		}
		protected override void InvalidateBrush() {
			base.InvalidateBrush();
			RadialGradientBrush brush = GradientBrush;
			if (brush == null)
				return;
			brush.GradientOrigin = GradientOrigin;
			brush.Center = Center;
			brush.RadiusX = RadiusX;
			brush.RadiusY = RadiusY;
		}
		protected override void InvalidateParams() {
			base.InvalidateParams();
			GradientOrigin = GradientBrush.GradientOrigin;
			Center = GradientBrush.Center;
			RadiusX = GradientBrush.RadiusX;
			RadiusY = GradientBrush.RadiusY;
		}
	}
	public class PopupGradientBrushValueMetadata {
		public static void BuildMetadata(MetadataBuilder<PopupGradientBrushValue> builder) {
			builder.Property(x => x.MappingMode).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditMappingMode)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditMappingModeDescription));
			builder.Property(x => x.SpreadMethod).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditSpreadMethod)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditSpreadMethodDescription));
		}
	}
	public class PopupLinearGradientBrushValueMetadata {
		public static void BuildMetadata(MetadataBuilder<PopupLinearGradientBrushValue> builder) {
			builder.Property(x => x.StartPoint).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditStartPoint)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditStartPointDescription));
			builder.Property(x => x.EndPoint).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditEndPoint)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditEndPointDescription));
		}
	}
	public class PopupRadialGradientBrushValueMetadata {
		public static void BuildMetadata(MetadataBuilder<PopupRadialGradientBrushValue> builder) {
			builder.Property(x => x.Center).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditCenter)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditCenterDescription));
			builder.Property(x => x.GradientOrigin).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditGradientOrigin)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditGradientOriginDescription));
			builder.Property(x => x.RadiusX).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditRadiusX)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditRadiusXDescription));
			builder.Property(x => x.RadiusY).DisplayName(EditorLocalizer.GetString(EditorStringId.BrushEditRadiusY)).Description(EditorLocalizer.GetString(EditorStringId.BrushEditRadiusYDescription));
		}
	}
	public enum GradientBrushType {
		Linear,
		Radial,
	}
}
