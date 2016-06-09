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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	class BandsValidator {
		protected IDesignerHost designerHost;
		public BandsValidator(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public virtual void EnsureExistence(params BandKind[] bandKinds) {
			CreateBands((XtraReport)designerHost.RootComponent, bandKinds);
		}
		protected void CreateBands(XtraReport report, BandKind[] bandKinds) {
			foreach(BandKind kind in bandKinds) {
				if(report.Bands[kind] != null)
					continue;
				Type type = BandFactory.GetBandType(kind);
				if(type != null) {
					Band band = (Band)designerHost.CreateComponent(type);
					if(band is TopMarginBand)
						SetBandHeight(band, report.Margins.Top, report.Dpi);
					if(band is BottomMarginBand)
						SetBandHeight(band, report.Margins.Bottom, report.Dpi);
					report.Bands.Add(band);
				}
			}
		}
		static void SetBandHeight(Band band, float height, float dpi) {
			band.HeightF = XRConvert.Convert(height, dpi, band.Dpi);
		}
	}
}
