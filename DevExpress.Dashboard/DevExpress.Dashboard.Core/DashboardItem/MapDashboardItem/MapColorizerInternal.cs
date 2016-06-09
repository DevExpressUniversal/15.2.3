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

using System;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class MapColorizerInternal {
		readonly IChangeService changeService;
		MapPalette palette;
		MapScale scale;
		public MapPalette Palette {
			get { return palette; }
			set {
				if(palette != value) {
					RemovePaletteChangeService();
					palette = value;
					SetPaletteChangeService();
					RaiseChanged();
				}
			}
		}
		public MapScale Scale {
			get { return scale; }
			set {
				if(scale != value) {
					SetScale(value);
					RaiseChanged();
				}
			}
		}
		public event EventHandler OnChanged;
		public MapColorizerInternal(IChangeService changeService) {
			this.changeService = changeService;
			SetScale(new UniformScale());
		}
		void RemovePaletteChangeService() {
			if(palette != null)
				palette.ChangeService = null;
		}
		void SetPaletteChangeService() {
			if(palette != null)
				palette.ChangeService = changeService;
		}
		void SetScale(MapScale value) {
			RemoveScaleChangeService();
			scale = value;
			SetScaleChangeService();
		}
		void RemoveScaleChangeService() {
			if(scale != null)
				scale.ChangeService = null;
		}
		void SetScaleChangeService() {
			if(scale != null)
				scale.ChangeService = changeService;
		}
		void RaiseChanged() {
			if(OnChanged != null)
				OnChanged(this, EventArgs.Empty);
		}
		public void SaveToXml(XElement element) {
			if(scale != null) {
				DevExpress.DataAccess.Native.XmlSerializer<MapScale> serializer = XmlRepository.MapScaleRepository.GetSerializer(scale);
				if(serializer != null)
					element.Add(serializer.SaveToXml(scale));
			}
			if(palette != null) {
				DevExpress.DataAccess.Native.XmlSerializer<MapPalette> serializer = XmlRepository.MapPaletteRepository.GetSerializer(palette);
				if(serializer != null)
					element.Add(serializer.SaveToXml(palette));
			}
		}
		public void LoadFromXml(XElement rootElement) {
			foreach(XElement element in rootElement.Elements()) {
				string elementName = element.Name.LocalName;
				DevExpress.DataAccess.Native.XmlSerializer<MapPalette> paletteSerializer = XmlRepository.MapPaletteRepository.GetSerializer(elementName);
				if(paletteSerializer != null) {
					palette = paletteSerializer.LoadFromXml(element);
					continue;
				}
				DevExpress.DataAccess.Native.XmlSerializer<MapScale> scaleSerializer = XmlRepository.MapScaleRepository.GetSerializer(elementName);
				if(scaleSerializer != null) {
					scale = scaleSerializer.LoadFromXml(element);
					continue;
				}
			}
		}
		public void OnEndLoading() {
			SetPaletteChangeService();
			SetScaleChangeService();
		}
	}
}
