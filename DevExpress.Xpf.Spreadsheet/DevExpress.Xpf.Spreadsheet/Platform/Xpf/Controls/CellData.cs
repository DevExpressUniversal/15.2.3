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
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Spreadsheet;
using System.Windows.Media;
using System.ComponentModel;
using CellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
namespace DevExpress.Xpf.Spreadsheet {
	#region CellData
	public class CellData : INotifyPropertyChanged {
		#region Fields
		WeakReference control;
		CellPosition position;
		RectangleGeometry clip;
		#endregion
		internal CellData(WorksheetControl control, CellPosition position, TextSettings settings, RectangleGeometry clip)
			: this(control, position, settings, clip, null) {
		}
		internal CellData(WorksheetControl control, CellPosition position, TextSettings settings, RectangleGeometry clip, ConditionalFormattingSettings conditionalFormattingSettings) {
			this.control = new WeakReference(control);
			this.position = position;
			this.clip = clip;
			TextSettings = settings;
			ConditionalFormattingSettings = conditionalFormattingSettings;
		}
		#region Properties
		public Cell Cell { get { return GetApiCell(); } }
		public TextSettings TextSettings { get; set; }
		public ConditionalFormattingSettings ConditionalFormattingSettings { get; set; }
		public RectangleGeometry Clip {
			get { return clip; }
			set {
				if (clip == value)
					return;
				clip = value;
				RaisePropertyChanged("Clip");
			}
		}
		#endregion
		Cell GetApiCell() {
			if (control == null || !control.IsAlive)
				return null;
			WorksheetControl worksheetControl = control.Target as WorksheetControl;
			return worksheetControl.GetApiCell(position);
		}
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	#region TextSettingsBase (abstract class)
	public abstract class TextSettingsBase {
		public FontFamily FontFamily { get; set; }
		public double FontSize { get; set; }
		public FontStyle FontStyle { get; set; }
		public FontWeight FontWeight { get; set; }
		public TextWrapping TextWrapping { get; set; }
	}
	#endregion
	public class TextSettings : TextSettingsBase {
		public string Text { get; set; }
		public TextAlignment TextAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public TextTrimming TextTrimming { get; set; }
		public TextDecorationCollection TextDecorations { get; set; }
		public Color Foreground { get; set; }
		public Rect TextBounds { get; set; }
	}
	public class ConditionalFormattingSettings {
		#region Fields
		WeakReference image;
		WeakReference dataBarFillBrush;
		WeakReference dataBarBorderBrush;
		WeakReference axisBrush;
		#endregion
		public ImageSource Image {
			get { return image != null ? image.Target as ImageSource : null; }
			set { image = new WeakReference(value); }
		}
		public Brush DataBarFillBrush {
			get { return dataBarFillBrush != null ? dataBarFillBrush.Target as Brush : null; }
			set { dataBarFillBrush = new WeakReference(value); }
		}
		public Brush DataBarBorderBrush {
			get { return dataBarBorderBrush != null ? dataBarBorderBrush.Target as Brush : null; }
			set { dataBarBorderBrush = new WeakReference(value); }
		}
		public Rect DataBarBounds { get; set; }
		public Size DataBarSize { get { return DataBarBounds.Size; } }
		public Thickness DataBarMargin { get { return new Thickness(DataBarBounds.X, 0, 0, 0); } }
		public Point AxisStart { get; set; }
		public Point AxisEnd { get; set; }
		public Brush AxisBrush {
			get { return axisBrush != null ? axisBrush.Target as Brush : null; }
			set { axisBrush = new WeakReference(value); }
		}
	}
}
