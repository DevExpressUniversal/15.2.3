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

using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraCharts.Designer.Native {
	[UserRepositoryItem("Register")]
	public class RepositoryItemImagePopupContainerEdit : RepositoryItemPopupContainerEdit {
		internal const string EditorName = "ImagePopupContainerEdit";
		static RepositoryItemImagePopupContainerEdit() {
			Register();
		}
		public static void Register() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ImagePopupContainerEdit),
				typeof(RepositoryItemImagePopupContainerEdit), typeof(ImageButtonEditViewInfo),
				new ButtonEditPainter(), true, null, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public RepositoryItemImagePopupContainerEdit() {
		}
	}
	public class ImagePopupContainerEdit : PopupContainerEdit {
		static ImagePopupContainerEdit() {
			RepositoryItemImagePopupContainerEdit.Register();
		}
		public override string EditorTypeName {
			get { return RepositoryItemImagePopupContainerEdit.EditorName; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemImagePopupContainerEdit Properties {
			get { return base.Properties as RepositoryItemImagePopupContainerEdit; }
		}
		public ImagePopupContainerEdit() {
		}
	}
}
