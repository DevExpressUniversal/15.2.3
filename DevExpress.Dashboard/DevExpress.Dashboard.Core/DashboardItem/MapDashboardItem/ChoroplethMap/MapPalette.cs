#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class PaletteColorCollection : NotificationCollection<Color> {
	}
	public abstract class MapPalette {
		internal IChangeService ChangeService { get; set; }
		protected void OnChanged() {
			if(ChangeService != null)
				ChangeService.OnChanged(new ChangedEventArgs(ChangeReason.View, this, null));
		}
		internal abstract void SaveToXml(XElement element);
		internal abstract void LoadFromXml(XElement element);
	}
	public class GradientPalette : MapPalette {
		const string xmlStartColor = "StartColor";
		const string xmlEndColor = "EndColor";
		Color startColor, endColor;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GradientPaletteStartColor")
#else
	Description("")
#endif
		]
		public Color StartColor {
			get { return startColor; }
			set {
				if(startColor != value) {
					startColor = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GradientPaletteEndColor")
#else
	Description("")
#endif
		]
		public Color EndColor {
			get { return endColor; }
			set {
				if(endColor != value) {
					endColor = value;
					OnChanged();
				}
			}
		}
		internal override void SaveToXml(XElement element) {
			element.Add(new XAttribute(xmlStartColor, startColor.ToArgb()));
			element.Add(new XAttribute(xmlEndColor, endColor.ToArgb()));
		}
		internal override void LoadFromXml(XElement element) {
			string colorString = element.GetAttributeValue(xmlStartColor);
			startColor = XmlHelper.ColorFromString(colorString);
			colorString = element.GetAttributeValue(xmlEndColor);
			endColor = XmlHelper.ColorFromString(colorString);
		}
	}
	public class CustomPalette : MapPalette {
		readonly PaletteColorCollection colors = new PaletteColorCollection();
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CustomPaletteColors"),
#endif
		Editor(TypeNames.DefaultCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public PaletteColorCollection Colors { get { return colors; } }
		public CustomPalette() {
			colors.CollectionChanged += (sender, e) => OnChanged();
		}
		internal override void SaveToXml(XElement element) {
			foreach (Color color in colors)
				element.Add(color.SaveToXml());
		}
		internal override void LoadFromXml(XElement element) {
			foreach(XElement colorElement in element.Elements(XmlHelper.xmlColor))
				if(colorElement.Value != null)
					colors.Add(XmlHelper.ColorFromString(colorElement.Value));
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class MapPaletteEqualityComparer : IEqualityComparer<MapPalette> {
		public bool Equals(MapPalette palette1, MapPalette palette2) {
			GradientPalette gradient1 = palette1 as GradientPalette;
			GradientPalette gradient2 = palette2 as GradientPalette;
			CustomPalette custom1 = palette1 as CustomPalette;
			CustomPalette custom2 = palette2 as CustomPalette;
			if ((palette1 == null && palette2 == null))
				return true;
			if ((gradient1 != null && gradient2 == null) ||
				(custom1 != null && custom2 == null) ||
				(gradient1 == null && gradient2 != null) ||
				(custom1 == null && custom2 != null))
				return false;
			if (custom1 != null) {
				if (custom1.Colors.Count== custom2.Colors.Count) {			 
					for( int i = 0; i< custom1.Colors.Count; i++ ){
						if(!custom1.Colors[i].Equals(custom2.Colors[i]))
							return false;
					}
					return true;
				}
			}
			if (gradient1 != null) {				
				return gradient1.StartColor.Equals(gradient2.StartColor) && gradient1.EndColor.Equals(gradient2.EndColor);
			}
			return false;
		}
		public int GetHashCode(MapPalette palette) {		  
			GradientPalette gradient = palette as GradientPalette;
			CustomPalette custom = palette as CustomPalette;   
			if(gradient!=null) 
				return gradient.StartColor.GetHashCode() | gradient.EndColor.GetHashCode();	  
			else
				 return custom.Colors.Count.GetHashCode();
		}		
	}
}
