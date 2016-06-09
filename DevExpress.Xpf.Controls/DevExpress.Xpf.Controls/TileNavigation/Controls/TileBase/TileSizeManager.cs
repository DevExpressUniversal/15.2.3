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
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.WindowsUI.Base;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Navigation.Internal {
	public enum TileSize { Default, Auto, Small, Medium, Wide }
	public interface ITileSizeManagerOwner : ITileSizeProvider {
		void OnSizeChanged();
	}
	public interface ITileSizeProvider {
		Size GetSize(TileSize tileSize);
	}
	public sealed class TileSizeManager : Freezable {
		public ITileSizeProvider TileSizeProvider { get; internal set; }
		#region static
		internal static double DefaultSize = 150;
		internal const double LargeSize = 310;
		internal const double SmallSize = 70;
		internal const double WideWidth = 310;
		internal const double WideHeight = 150;
		public static readonly DependencyProperty WidthProperty;
		public static readonly DependencyProperty HeightProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualWidthProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualHeightProperty;
		static TileSizeManager() {
			var dProp = new DependencyPropertyRegistrator<TileSizeManager>();
			dProp.Register("Width", ref WidthProperty, double.NaN,
				(dObj, e) => ((TileSizeManager)dObj).OnSizeChanged());
			dProp.Register("Height", ref HeightProperty, double.NaN,
				(dObj, e) => ((TileSizeManager)dObj).OnSizeChanged());
			dProp.Register("ActualWidth", ref ActualWidthProperty, DefaultSize);
			dProp.Register("ActualHeight", ref ActualHeightProperty, DefaultSize);
		}
		#endregion static
		#region Properties
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public double ActualWidth {
			get { return (double)GetValue(ActualWidthProperty); }
		}
		public double ActualHeight {
			get { return (double)GetValue(ActualHeightProperty); }
		}
		#endregion Properties
		public WeakReference OwnerRef;
		void OnSizeChanged() {
			if(OwnerRef == null) return;
			ITileSizeManagerOwner owner = OwnerRef.Target as ITileSizeManagerOwner;
			if(owner != null)
				owner.OnSizeChanged();
			else OwnerRef = null;
		}
		public void UpdateActualSize(bool isLarge, bool hasWidth, int rowCount, double rowIndent, bool hasHeight) {
			SetValue(ActualWidthProperty, hasWidth ? Width : isLarge ? DefaultSize * 2 + rowIndent : DefaultSize);
			SetValue(ActualHeightProperty, hasHeight ? Height : DefaultSize * (double)rowCount + (double)(rowCount - 1) * rowIndent);
		}
		public void UpdateActualSize(TileSize tileSize, bool isLarge, bool hasWidth, int rowCount, double rowIndent, bool hasHeight) {
			Size DefaultSize = GetTileSize(isLarge ? TileSize.Default : tileSize);
			double width = hasWidth ? Width : isLarge ? DefaultSize.Width * 2 + rowIndent : DefaultSize.Width;
			double height = hasHeight ? Height : DefaultSize.Height * (double)rowCount + (double)(rowCount - 1) * rowIndent;
			SetValue(ActualWidthProperty, width);
			SetValue(ActualHeightProperty, height);
		}
		Size GetTileSize(TileSize tileSize) {
			ITileSizeManagerOwner owner = OwnerRef.Target as ITileSizeManagerOwner;
			return owner!= null ? owner.GetSize(tileSize): new Size();
		}
		protected override Freezable CreateInstanceCore() {
			return new TileSizeManager();
		}
	}
}
