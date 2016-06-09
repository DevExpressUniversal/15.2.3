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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Map.Native {
	public interface IOverlayInfo {
		MapOverlayLayout Layout { get; }
		HorizontalAlignment HorizontalAlignment { get; }
		VerticalAlignment VerticalAlignment { get; }
		Control GetPresentationControl();
		void OnAlignmentUpdated();
	}
	public class MapOverlayLayout {
		public Point Location { get; set; }
		public Size Size { get; set; }
		public double Width { get { return Size.Width; } }
		public double Height { get { return Size.Height; } }
		public Rect Bounds { get { return new Rect(Location, Size); } }
	}
	public static class OverlayLayoutUpdater {
		static readonly List<string> supportedLayoutProperties = new List<string> { "VerticalAlignment", "HorizontalAlignment", "Margin" };
		public static void LayotPropertyChanged(DependencyProperty property, IOverlayInfo overlayInfo) {
			if (supportedLayoutProperties.Contains(property.Name))
				overlayInfo.OnAlignmentUpdated();
		}
	}
	public class OverlayInfoCollection : ObservableCollection<IOverlayInfo> {
	}
	public abstract class OverlayInfoBase : INotifyPropertyChanged, IOverlayInfo {
		readonly MapControl map;
		readonly MapOverlayLayout layout = new MapOverlayLayout();
		NavigationElementOptions options;
		public NavigationElementOptions Options {
			get { return options; }
			set {
				if (options != value) {
					options = value;
					RaisePropertyChanged("Options");
				}
			}
		}
		public MapControl Map { get { return map; } }
		protected OverlayInfoBase(MapControl map) {
			this.map = map;
		}
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region IOverlayInfo implementation
		HorizontalAlignment IOverlayInfo.HorizontalAlignment { get { return ConvertToHorizontalAlignment(options.HorizontalAlignment); } }
		VerticalAlignment IOverlayInfo.VerticalAlignment { get { return ConvertToVerticalAlignment(options.VerticalAlignment); } }
		MapOverlayLayout IOverlayInfo.Layout { get { return layout; } }
		Control IOverlayInfo.GetPresentationControl() {
			return CreatePresentationControl();
		}
		void IOverlayInfo.OnAlignmentUpdated() {
		}
		#endregion
		HorizontalAlignment ConvertToHorizontalAlignment(NavigationElementHorizontalAlignment alignment) {
			switch(alignment) {
				case NavigationElementHorizontalAlignment.Center: return HorizontalAlignment.Center;
				case NavigationElementHorizontalAlignment.Right: return HorizontalAlignment.Right;
			}
			return HorizontalAlignment.Left;
		}
		VerticalAlignment ConvertToVerticalAlignment(NavigationElementVerticalAlignment alignment) {
			switch(alignment) {
				case NavigationElementVerticalAlignment.Center: return VerticalAlignment.Center;
				case NavigationElementVerticalAlignment.Bottom: return VerticalAlignment.Bottom;
			}
			return VerticalAlignment.Top;
		}
		protected internal abstract Control CreatePresentationControl();
	}
}
