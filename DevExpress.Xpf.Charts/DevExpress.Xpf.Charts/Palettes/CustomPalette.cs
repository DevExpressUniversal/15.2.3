﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Utils.Serializing;
namespace DevExpress.Xpf.Charts {
	public class CustomPalette : Palette {
		readonly ColorCollection colors;
		public CustomPalette() {
			colors = new ColorCollection();
			colors.CollectionChanged += new NotifyCollectionChangedEventHandler(ColorsChanged);
		}
		protected internal override ColorCollection ActualColors { get { return Colors; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomPaletteColors")]
#endif
		public ColorCollection Colors { get { return colors; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public ColorCollection ColorsSerializable {
			get { return Colors; }
			set {
				Colors.Clear();
				if (value != null) {
					foreach (Color color in value)
						Colors.Add(color);
				}
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomPalettePaletteName")]
#endif
		public override string PaletteName { get { return "Custom"; } }
		void ColorsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			NotifyPropertyChanged("Colors");
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomPalette();
		}
	}
}
