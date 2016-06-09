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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraCharts.Native {
	public class XtraPaletteWrapper : IXtraSupportDeserializeCollectionItem {
		Palette palette;
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public Palette Palette { get { return palette; } }
		[XtraSerializableProperty]
		public string Name {
			get { return palette.NameSerializable; }
			set { palette.NameSerializable = value; }
		}
		[XtraSerializableProperty]
		public PaletteScaleMode ScaleMode {
			get { return palette.ScaleModeSerializable; }
			set { palette.ScaleModeSerializable = value; }
		}
		public string DisplayName { get { return palette.DisplayName; } }
		public XtraPaletteWrapper() {
			palette = new Palette(String.Empty);
		}
		public XtraPaletteWrapper(Palette palette) {
			this.palette = palette;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			palette.Add((PaletteEntry)e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return new PaletteEntry();
		}
	}
	public class XtraPaletteWrapperCollection : CollectionBase {
		PaletteRepository repository;
		public XtraPaletteWrapper this[int index] { get { return (XtraPaletteWrapper)List[index]; } }
		public XtraPaletteWrapperCollection(PaletteRepository repository) {
			this.repository = repository;
			foreach(string name in repository.GetPaletteNames())
				List.Add(new XtraPaletteWrapper(repository[name]));
		}
		public int Add(XtraPaletteWrapper paletteWrapper) {
			return List.Add(paletteWrapper);
		}
		public void OnEndLoaing() {
			foreach(XtraPaletteWrapper paletteWrapper in List)
				repository.Add(paletteWrapper.Name, paletteWrapper.Palette);
		}
	}
}
