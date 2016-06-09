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

using System.Windows.Media;
using DevExpress.Mvvm.Native;
using System.Windows;
using System;
namespace DevExpress.Xpf.Core.Native {
	public class RenderBorderContext : RenderDecoratorContext {
		Brush background;
		Brush borderBrush;
		Brush frozenBackground;
		Brush frozenBorderBrush;
		Thickness? borderThickness;
		Thickness? padding;
		CornerRadius? cornerRadius;
		protected internal Geometry Clip { get; set; }
		public Brush Background {
			get { return frozenBackground ?? (frozenBackground = GetAsFrozen(background)); }
			set { SetProperty(ref background, value, FREInvalidateOptions.None, parametrizedAction: OnBrushChanged); }
		}
		public Brush BorderBrush {
			get { return frozenBorderBrush ?? (frozenBorderBrush = GetAsFrozen(borderBrush)); }
			set { SetProperty(ref borderBrush, value, FREInvalidateOptions.None, parametrizedAction: OnBrushChanged); }
		}		
		public Thickness? BorderThickness {
			get { return borderThickness; }
			set { SetProperty(ref borderThickness, value); }
		}
		public Thickness? Padding { 
			get { return padding; }
			set { SetProperty(ref padding, value); }
		}
		public CornerRadius? CornerRadius {
			get { return cornerRadius; }
			set { SetProperty(ref cornerRadius, value); }
		}
		public Pen LeftPenCache { get; internal set; }
		public Pen RightPenCache { get; internal set; }
		public Pen TopPenCache { get; internal set; }
		public Pen BottomPenCache { get; internal set; }
		public bool UseComplexRenderCodePath { get; internal set; }
		public StreamGeometry BackgroundGeometryCache { get; internal set; }
		public StreamGeometry BorderGeometryCache { get; internal set; }
		public RenderBorderContext(RenderBorder factory)
			: base(factory) {
		}
		protected override void RenderSizeChanged() {
			Clip = null;
		}
		protected override void ResetRenderCachesInternal() {
			base.ResetRenderCachesInternal();
			LeftPenCache = null;
			RightPenCache = null;
			TopPenCache = null;
			BottomPenCache = null;
		}
		protected internal override void ResetSizeSpecificCaches() {
			base.ResetSizeSpecificCaches();
			BackgroundGeometryCache = null;
			BorderGeometryCache = null;
		}
		protected void OnBrushChanged(Brush oldValue, Brush newValue) {
			if (oldValue != null && !oldValue.IsFrozen)
				oldValue.Changed -= OnBrushAnimated;
			if (newValue != null && !newValue.IsFrozen) {
				newValue.Changed += OnBrushAnimated;
			}
			OnBrushAnimated(null, null);
			if (ElementHost != null)
				UpdateLayout(FREInvalidateOptions.UpdateVisual);
		}		
		void OnBrushAnimated(object sender, EventArgs e) {
			frozenBackground = null;
			frozenBorderBrush = null;
			if (ElementHost != null)
				UpdateLayout(FREInvalidateOptions.UpdateVisual);
		}
		Brush GetAsFrozen(Brush value) { return value.If(x => x.CanFreeze).Return(x => x.GetCurrentValueAsFrozen() as Brush, () => value); }
		public override void Release() {
			base.Release();
			OnBrushChanged(borderBrush, null);
			OnBrushChanged(background, null);
		}
	}
}
