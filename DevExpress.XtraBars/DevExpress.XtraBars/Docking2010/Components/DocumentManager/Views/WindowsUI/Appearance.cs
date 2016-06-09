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
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Views {
	public class WindowsUIViewAppearanceCollection : BaseViewAppearanceCollection {
		public WindowsUIViewAppearanceCollection(WindowsUIView owner)
			: base(owner) {
		}
		protected override void CreateAppearances() {
			base.CreateAppearances();
			caption = CreateAppearance("Caption");
			splashScreen = CreateAppearance("SplashScreen");
			actionsBar = CreateAppearance("ActionsBar");
			actionsBarButton = CreateAppearance("ActionsBarButton");
			searchPanel = CreateAppearance("SearchPanel");
		}
		AppearanceObject caption;
		AppearanceObject splashScreen;
		AppearanceObject actionsBar;
		AppearanceObject actionsBarButton;
		AppearanceObject searchPanel;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Caption { get { return caption; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ActionsBar { get { return actionsBar; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ActionsBarButton { get { return actionsBarButton; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SplashScreen { get { return splashScreen; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject SearchPanel { get { return searchPanel; } }
		void ResetCaption() { Caption.Reset(); }
		void ResetSplashScreen() { SplashScreen.Reset(); }
		void ResetActionsBar() { ActionsBar.Reset(); }
		void ResetActionsBarButton() { ActionsBarButton.Reset(); }
		void ResetSearchPanel() { SearchPanel.Reset(); }
		bool ShouldSerializeCaption() { return Caption.ShouldSerialize(); }
		bool ShouldSerializeSplashScreen() { return SplashScreen.ShouldSerialize(); }
		bool ShouldSerializeActionsBar() { return ActionsBar.ShouldSerialize(); }
		bool ShouldSerializeActionsBarButton() { return ActionsBarButton.ShouldSerialize(); }
		bool ShouldSerializeSearchPanel() { return SearchPanel.ShouldSerialize(); }
	}
} 
