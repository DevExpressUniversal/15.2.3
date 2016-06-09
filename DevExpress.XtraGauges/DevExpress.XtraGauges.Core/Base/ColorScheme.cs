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
using System.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Base {
	[Flags]
	public enum TargetElement { Empty = 0, RangeBar = 1, ImageIndicator = 2, Label = 4 }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColorScheme {
		TargetElement targetElementsColorCore;
		Color colorCore;
		public event PropertyChangedEventHandler PropertyChanged;
		[XtraSerializableProperty, 
		Category("Appearance"), DefaultValue(TargetElement.Empty)]
#if !DXPORTABLE
		[System.ComponentModel.Editor("DevExpress.Utils.Editors.AttributesEditor," + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public TargetElement TargetElements {
			get { return targetElementsColorCore; }
			set {
				targetElementsColorCore = value;
				OnPropertyChanged("TargetElements");
			}
		}
		[XtraSerializableProperty, Description("")]
		public Color Color {
			get { return colorCore; }
			set {
				if(colorCore == value) return;
				colorCore = value;
				OnPropertyChanged("Color");
			}
		}
		internal bool ShouldSerializeColor(){
			return Color != Color.Empty;
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		public bool ShouldSerialize() {
			return (TargetElements != TargetElement.Empty ||
					Color != Color.Empty);
		}
		public void Reset() {
			TargetElements = TargetElement.Empty;
			Color = Color.Empty;
		}
		void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
	}
}
