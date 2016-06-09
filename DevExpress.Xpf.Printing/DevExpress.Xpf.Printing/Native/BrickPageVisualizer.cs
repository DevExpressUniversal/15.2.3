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

using System;
using System.IO;
using System.Windows;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
#if !SL
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class BrickPageVisualizer : PageVisualizer {
		readonly TextMeasurementSystem textMeasurementSystem;
		public BrickPageVisualizer(TextMeasurementSystem textMeasurementSystem) {
			this.textMeasurementSystem = textMeasurementSystem;
		}
		protected XamlCompatibility XamlCompatibility {
			get {
#if SL
				return XamlCompatibility.Silverlight;
#else
				return XamlCompatibility.WPF;
#endif
			}
		}
		public Stream SaveToStream(Page page, int pageIndex, int pageCount) {
			if(page == null)
				throw new ArgumentNullException("page");
			MemoryStream stream = new MemoryStream();			
			XamlExporter exporter = new XamlExporter();
			exporter.Export(
					stream,
					page,
					pageIndex + 1,
					pageCount,
					XamlCompatibility,
					textMeasurementSystem);
			stream.Position = 0;
			return stream;			
		}
		public override FrameworkElement Visualize(PSPage page, int pageIndex, int pageCount) {			
				Stream stream = SaveToStream(page, pageIndex, pageCount);
				return XamlReaderHelper.Load(stream);
			}
		}
	}
