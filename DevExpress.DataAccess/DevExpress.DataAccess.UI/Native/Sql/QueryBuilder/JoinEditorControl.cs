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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	[ToolboxItem(false)]
	public class JoinEditorControl : ConditionEditorControl {
		readonly HoverImageButton addItemButton;
		readonly List<ConditionControl> items = new List<ConditionControl>();
		IDisplayNameProvider displayNameProvider;
		string leftTableName;
		Dictionary<string, List<string>> leftObjectNames;
		Dictionary<string, List<string>> rightObjectNames;
		public JoinEditorControl() {
			this.addItemButton = new HoverImageButton(this.addHover, this.addNormal);
			this.addItemButton.Click += AddCondition_Click;
			this.panelControls.Controls.Add(this.addItemButton); 
		}
		public List<ConditionControl> Items { get { return this.items; } }
		public void InitLookups(string leftTable, IEnumerable<string> leftColumns, IDictionary<string, List<string>> right) {
			this.leftTableName = leftTable;
			this.leftObjectNames = new Dictionary<string, List<string>> { { leftTable, leftColumns as List<string> ?? new List<string>(leftColumns) } };
			this.rightObjectNames = right as Dictionary<string, List<string>> ?? new Dictionary<string, List<string>>(right);
		}
		public void InitLookups(string leftTable, IEnumerable<string> leftColumns, IEnumerable<KeyValuePair<string, IEnumerable<string>>> right) {
			InitLookups(leftTable, leftColumns, right, null);
		}
		public void InitLookups(string leftTable, IEnumerable<string> leftColumns, IEnumerable<KeyValuePair<string, IEnumerable<string>>> right, IDisplayNameProvider displayNameProvider) {
			this.leftTableName = leftTable;
			this.leftObjectNames = new Dictionary<string, List<string>> { { leftTable, leftColumns as List<string> ?? new List<string>(leftColumns) } };
			this.rightObjectNames =  right.ToDictionary(pair => pair.Key, pair => pair.Value as List<string> ?? new List<string>(pair.Value));
			this.displayNameProvider = displayNameProvider;
		}
		public void AddCondition(string leftColumn, BinaryOperatorType conditionOperator, string rightTable, string rightColumn) {
			ConditionControl control = CreateEmptyCondition();
			control.LeftColumnName = leftColumn;
			control.OperatorType = conditionOperator;
			control.RightTableName = rightTable;
			control.RightColumnName = rightColumn;
		}
		public void ClearItems() {
			foreach(ConditionControl control in Items) {
				this.panelControls.Controls.Remove(control);
				control.Dispose();
			}
			Items.Clear();
			AlignItems();
		}
		protected internal override void AlignItems() {
			int posY = topMargin;
			foreach(ConditionControl item in Items) {
				item.Top = posY;
				item.Left = leftMargin;
				posY += item.Height + verticalSpacing;
			}
			this.addItemButton.Left = leftMargin;
			this.addItemButton.Top = posY;
		}
		protected override void CreateBitmaps(Color operatorColor, Color foreColor) {
			foreach(Image[] images in this.conditionOperationsImages.Values) {
				DisposeImageNotNull(ref images[0]);
				DisposeImageNotNull(ref images[1]);
			}
			this.conditionOperationsImages.Clear();
			this.conditionOperationsImages.Add(BinaryOperatorType.Equal, new Image[2]);
			this.conditionOperationsImages.Add(BinaryOperatorType.NotEqual, new Image[2]);
			this.conditionOperationsImages.Add(BinaryOperatorType.Greater, new Image[2]);
			this.conditionOperationsImages.Add(BinaryOperatorType.GreaterOrEqual, new Image[2]);
			this.conditionOperationsImages.Add(BinaryOperatorType.Less, new Image[2]);
			this.conditionOperationsImages.Add(BinaryOperatorType.LessOrEqual, new Image[2]);
			this.conditionOperationsImages[BinaryOperatorType.Equal][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("Equals"), foreColor);
			this.conditionOperationsImages[BinaryOperatorType.NotEqual][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("DoesNotEqual"), foreColor);
			this.conditionOperationsImages[BinaryOperatorType.Greater][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("Greater"), foreColor);
			this.conditionOperationsImages[BinaryOperatorType.GreaterOrEqual][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("GreaterOrEqual"), foreColor);
			this.conditionOperationsImages[BinaryOperatorType.Less][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("Less"), foreColor);
			this.conditionOperationsImages[BinaryOperatorType.LessOrEqual][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("LessOrEqual"), foreColor);
			foreach(Image[] images in this.conditionOperationsImages.Values)
				images[1] = ImageHelper.ColorBitmap(images[0], operatorColor);
		}
		void AddCondition_Click(object sender, EventArgs e) {
			CreateEmptyCondition();
			AlignItems();
			RaiseItemsChanged();
		}
		ConditionControl CreateEmptyCondition() {
			ConditionControl control = new ConditionControl(this.barManager, displayNameProvider, this.conditionOperationsImages, this.removeNormal, this.removeHover);
			control.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			control.LeftObjectNames = this.leftObjectNames;
			control.RightObjectNames = this.rightObjectNames;
			control.AllowChangeOperatorType = true;
			control.RemoveButtonClick += control_RemoveButtonClick;
			control.LeftTableName = this.leftTableName;
			this.panelControls.Controls.Add(control);
			Items.Add(control);
			return control;
		}
		void RaiseItemsChanged() {
		}
		void control_RemoveButtonClick(object sender, EventArgs e) {
			ConditionControl control = sender as ConditionControl;
			if(control != null) {
				Items.Remove(control);
				this.panelControls.Controls.Remove(control);
				control.Dispose();
				AlignItems();
			}
		}
	}
}
