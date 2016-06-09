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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ParametrizedActionItemControlFactory : IDisposable {
		public const string GoButtonID = "goButton";
		private static HashSet<Type> validValueTypes;
		private ParametrizedAction action;
		static ParametrizedActionItemControlFactory() {
			validValueTypes = new HashSet<Type>();
			validValueTypes.Add(typeof(int));
			validValueTypes.Add(typeof(string));
			validValueTypes.Add(typeof(DateTime));
		}
		internal static EditorButton GetGoButton(RepositoryItemButtonEdit repositoryItem) {
			return GetButtonById(GoButtonID, repositoryItem);
		}
		public static EditorButton GetButtonById(string id, RepositoryItemButtonEdit item) {
			Guard.ArgumentNotNullOrEmpty(id, "id");
			foreach(EditorButton button in item.Buttons) {
				if(id.Equals(button.Tag)) {
					return button;
				}
			}
			return null;
		}
		private static ButtonEdit CreateControlCore(Type valueType, bool canClear) {
			ButtonEdit edit = null;
			if(valueType == typeof(int)) {
				SpinEdit spinEdit = new SpinEdit();
				spinEdit.Properties.IsFloatValue = false;
				edit = spinEdit;
			}
			if(valueType == typeof(string)) {
				if(canClear) {
					edit = new ButtonEditWithClearButton();
				}
				else {
					edit = new ButtonEdit();
					edit.Properties.Buttons.Clear();
				}
			}
			if(valueType == typeof(DateTime)) {
				edit = new DateEdit();
			}
			return edit;
		}
		private static RepositoryItemButtonEdit CreateRepositoryItemCore(Type valueType, bool canClear) {
			RepositoryItemButtonEdit repositoryItem = null;
			if(valueType == typeof(int)) {
				RepositoryItemSpinEdit repositoryItemSpinEdit = new RepositoryItemSpinEdit();
				repositoryItemSpinEdit.IsFloatValue = false;
				repositoryItem = repositoryItemSpinEdit;
			}
			if(valueType == typeof(string)) {
				if(canClear) {
					repositoryItem = new RepositoryItemButtonEditWithClearButton();
				}
				else {
					repositoryItem = new RepositoryItemButtonEdit();
					repositoryItem.Buttons.Clear();
				}
			}
			if(valueType == typeof(DateTime)) {
				repositoryItem = new RepositoryItemDateEdit();
			}
			return repositoryItem;
		}
		public static RepositoryItem CreateRepositoryItem(Type valueType, bool canClear) {
			if(!validValueTypes.Contains(valueType)) {
				string message = string.Format("Cannot create the RepositoryItem because the '{0}' value type for Parameterized Action Control is not supported.", valueType.GetType());
				throw new InvalidOperationException(message);
			}
			return CreateRepositoryItemCore(valueType, canClear);
		}
		private void CheckValueType(Type valueType) {
			if(!validValueTypes.Contains(valueType)) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.InvalidParametrizedActionValueType, action.Caption));
			}
		}
		private void SetupRepositoryItem(RepositoryItemButtonEdit item, bool showExecuteButton) {
			item.NullValuePrompt = action.NullValuePrompt;
			item.NullValuePromptShowForEmptyValue = true;
			if(showExecuteButton) {
				EditorButton goButton = CreateGoButton();
				item.Buttons.Add(goButton);
			}
		}
		private EditorButton CreateGoButton() {
			EditorButton goButton = CreateButton(GoButtonID);
			UpdateGoButtonAppearance(goButton);
			return goButton;
		}
		private EditorButton CreateButton(string buttonId) {
			EditorButton goButton = new EditorButton();
			goButton.Kind = ButtonPredefines.Glyph;
			goButton.Tag = buttonId;
			goButton.ImageLocation = ImageLocation.TopLeft;
			return goButton;
		}
		internal void UpdateGoButtonAppearance(RepositoryItemButtonEdit repositoryItem) {
			EditorButton goButton = GetGoButton(repositoryItem);
			if(goButton != null) {
				UpdateGoButtonAppearance(goButton);
			}
		}
		public ParametrizedActionItemControlFactory(ParametrizedAction action) {
			this.action = action;
		}
		public ButtonEdit CreateControl(bool showExecuteButton, bool canClear) {
			CheckValueType(action.ValueType);
			ButtonEdit result = CreateControlCore(action.ValueType, canClear);
			result.AutoSizeInLayoutControl = false;
			result.MinimumSize = new Size(150, 20);
			SetupRepositoryItem(result.Properties, showExecuteButton);
			return result;
		}
		public RepositoryItemButtonEdit CreateRepositoryItem(bool showExecuteButton, bool canClear) {
			CheckValueType(action.ValueType);
			RepositoryItemButtonEdit result = CreateRepositoryItemCore(action.ValueType, canClear);
			SetupRepositoryItem(result, showExecuteButton);
			return result;
		}
		public void UpdateGoButtonAppearance(EditorButton goButton) {
			Guard.ArgumentNotNull(goButton, "goButton");
			ImageInfo imageInfo = ImageInfo.Empty;
			if(!string.IsNullOrEmpty(action.ImageName) && action.PaintStyle != ActionItemPaintStyle.Caption) {
				imageInfo = ImageLoader.Instance.GetImageInfo(action.ImageName);
			}
			if(!imageInfo.IsEmpty) {
				goButton.Image = imageInfo.Image;
				goButton.ImageLocation = ImageLocation.MiddleCenter;
				goButton.Caption = "";
			}
			else {
				goButton.Image = null;
				goButton.Caption = action.ShortCaption;
			}
		}
		public void Dispose() {
			action = null;
		}
	}
}
