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
using System.Windows;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Bar3DSection : ModelBase {
		Model3D model;
		protected abstract bool IsInternalModel { get; }
		protected internal abstract string ActualSource { get; }
		protected internal abstract bool ActualFixedHeight { get; }
		protected internal abstract bool ActualAlignByX { get; }
		protected internal abstract bool ActualAlignByY { get; }
		protected internal abstract bool ActualAlignByZ { get; }
		protected internal abstract bool ActualUseViewColor { get; }
		internal Model3D GetModel(bool loadFromResources) {
			if(model == null)
				model = Model3DExtractor.Extract(LoadObject(ActualSource, loadFromResources, IsInternalModel));
			return model;
		}
		protected internal override void ClearCache() {
			model = null;
		}
	}
	public class PredefinedBar3DSection : Bar3DSection {
		readonly string source;
		readonly bool fixedHeight;
		readonly bool alignByX;
		readonly bool alignByY;
		readonly bool alignByZ;
		readonly bool useViewColor;
		protected override bool IsInternalModel { get { return true; } }
		protected internal override string ActualSource { get { return source; } }
		protected internal override bool ActualFixedHeight { get { return fixedHeight; } }
		protected internal override bool ActualAlignByX { get { return alignByX; } }
		protected internal override bool ActualAlignByY { get { return alignByY; } }
		protected internal override bool ActualAlignByZ { get { return alignByZ; } }
		protected internal override bool ActualUseViewColor { get { return useViewColor; } }
		public PredefinedBar3DSection(string source, bool fixedHeight, bool alignByX, bool alignByY, bool alignByZ, bool useViewColor) {
			this.source = source;
			this.fixedHeight = fixedHeight;
			this.alignByX = alignByX;
			this.alignByY = alignByY;
			this.alignByZ = alignByZ;
			this.useViewColor = useViewColor;
		}
	}
	public class CustomBar3DSection : Bar3DSection {
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty FixedHeightProperty;
		public static readonly DependencyProperty UseViewColorProperty;
		public static readonly DependencyProperty AlignByXProperty;
		public static readonly DependencyProperty AlignByYProperty;
		public static readonly DependencyProperty AlignByZProperty;
		[
		Category(Categories.Common)
		]
		public string Source {
			get { return (string)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool FixedHeight {
			get { return (bool)GetValue(FixedHeightProperty); }
			set { SetValue(FixedHeightProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool AlignByX {
			get { return (bool)GetValue(AlignByXProperty); }
			set { SetValue(AlignByXProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool AlignByY {
			get { return (bool)GetValue(AlignByYProperty); }
			set { SetValue(AlignByYProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool AlignByZ {
			get { return (bool)GetValue(AlignByZProperty); }
			set { SetValue(AlignByZProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public bool UseViewColor {
			get { return (bool)GetValue(UseViewColorProperty); }
			set { SetValue(UseViewColorProperty, value); }
		}
		static CustomBar3DSection() {
			Type ownerType = typeof(CustomBar3DSection);
			SourceProperty = DependencyProperty.Register("Source", typeof(string), ownerType,
				new FrameworkPropertyMetadata(PropertyChanged));
			FixedHeightProperty = DependencyProperty.Register("FixedHeight", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
			AlignByXProperty = DependencyProperty.Register("AlignByX", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
			AlignByYProperty = DependencyProperty.Register("AlignByY", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, ChartElementHelper.UpdateWithClearDiagramCache));
			AlignByZProperty = DependencyProperty.Register("AlignByZ", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
			UseViewColorProperty = DependencyProperty.Register("UseViewColor", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, ChartElementHelper.UpdateWithClearDiagramCache));
		}
		protected override bool IsInternalModel { get { return false; } }
		protected internal override string ActualSource { get { return Source; } }
		protected internal override bool ActualFixedHeight { get { return FixedHeight; } }
		protected internal override bool ActualAlignByX { get { return AlignByX; } }
		protected internal override bool ActualAlignByY { get { return AlignByY; } }
		protected internal override bool ActualAlignByZ { get { return AlignByZ; } }
		protected internal override bool ActualUseViewColor { get { return UseViewColor; } }
		internal void Assign(CustomBar3DSection customBar3DSection) {
			if (customBar3DSection != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, SourceProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, FixedHeightProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, UseViewColorProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, AlignByXProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, AlignByYProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DSection, AlignByZProperty);
			}
		}
	}
	public class CustomBar3DSectionCollection : ChartElementCollection<CustomBar3DSection> {
		protected override ChartElementChange Change { get { return ChartElementChange.ClearDiagramCache; } }
	}
}
