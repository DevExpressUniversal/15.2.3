#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.XtraGrid.Views.BandedGrid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridBandModelSynchronizer : IModelSynchronizer {
		IModelBandWin bandModel;
		public GridBandModelSynchronizer(IModelBand bandModel) {
			this.bandModel = (IModelBandWin)bandModel;
		}
		IModelBandWin Model {
			get {
				return bandModel;
			}
		}
		IModelLayoutElement IModelSynchronizer.Model {
			get { return Model; }
		}
		public void ApplyModel(Component component) {
			GridBand band = component as GridBand;
			if(band != null) {
				band.Visible = !bandModel.Index.HasValue || bandModel.Index >= 0;
				band.Caption = bandModel.Caption;
				band.AutoFillDown = bandModel.AutoFillDown;
				ApplyModelCore(band);
			}
		}
		public void SynchronizeModel(Component component) {
			GridBand band = component as GridBand;
			if(band != null) {
				band.Caption = bandModel.Caption;
				bandModel.AutoFillDown = band.AutoFillDown;
				SynchronizeModelCore(band);
			}
		}
		protected virtual void ApplyModelCore(GridBand band) { }
		protected virtual void SynchronizeModelCore(GridBand band) { }
	}
}
