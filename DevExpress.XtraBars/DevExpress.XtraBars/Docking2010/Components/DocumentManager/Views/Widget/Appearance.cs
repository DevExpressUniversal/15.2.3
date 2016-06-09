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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetViewAppearanceCollection : BaseViewAppearanceCollection {
		public WidgetViewAppearanceCollection(WidgetView owner)
			: base(owner) {
		}
		protected override void CreateAppearances() {
			base.CreateAppearances();
			activeDocumentCaption = CreateAppearance("ActiveDocumentCaption");
			documentCaption = CreateAppearance("DocumentCaption");
		}
		AppearanceObject activeDocumentCaption;
		AppearanceObject documentCaption;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ActiveDocumentCaption { get { return activeDocumentCaption; } }
		void ResetActiveDocumentCaption() { ActiveDocumentCaption.Reset(); }
		bool ShouldSerializeActiveDocumentCaption() { return ActiveDocumentCaption.ShouldSerialize(); }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DocumentCaption { get { return documentCaption; } }
		void ResetDocumentCaption() { DocumentCaption.Reset(); }
		bool ShouldSerializeDocumentCaption() { return DocumentCaption.ShouldSerialize(); }
	}
}
