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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using System.Windows.Data;
#if SL
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class GroupHeader : FieldHeader {
		const string TemplatePartPanel = "PART_Panel";
		#region static
		public static readonly DependencyProperty GroupProperty;
		static GroupHeader() {
			Type ownerType = typeof(GroupHeader);
			GroupProperty = DependencyPropertyManager.Register("Group", typeof(PivotGridGroup),
				ownerType, new PropertyMetadata(null, OnGroupPropertyChanged));
		}
		static void OnGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GroupHeader)d).OnGroupChanged((PivotGridGroup)e.OldValue, (PivotGridGroup)e.NewValue);
		}
		#endregion
#if SL
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty DataContextProxyProperty = DependencyPropertyManager.Register("DataContextProxy", typeof(object), typeof(GroupHeader), new PropertyMetadata(null, (d, e) => ((GroupHeader)d).OnDataContextChanged()));
#else
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == DataContextProperty)
				OnDataContextChanged();
		}
#endif
		public GroupHeader() {
#if SL
			SetBinding(DataContextProxyProperty, new System.Windows.Data.Binding("DataContext") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
#endif
		}
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(GroupHeader));
		}
		public PivotGridGroup Group {
			get { return (PivotGridGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public Grid Panel { get; private set; }
		protected override void BindCore(PivotGridField field, int index) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(field.Group.IndexOf(field) != 0)
				throw new ArgumentException("field");
			DataContext = field.Group;
			OnDataContextChanged();
			SetActualAreaIndex(this, index);
		}
		protected override void OnContentChanged() {
			if(Content != null)
				SetBinding(DisplayTextProperty, new Binding("Group.DisplayText") { Source = Content });
			else
				SetValue(DisplayTextProperty, string.Empty);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Panel = GetTemplateChild(TemplatePartPanel) as Grid;
			UpdateItems();
		}
		protected virtual void OnGroupChanged(PivotGridGroup oldValue, PivotGridGroup newValue) {
			if(oldValue != null)
				oldValue.Changed -= OnGroupChanged; 
			UpdateItems();
			if(newValue != null)
				newValue.Changed += OnGroupChanged;
		}
		void OnGroupChanged(object sender, RoutedEventArgs e) {
			UpdateItems();
		}
		protected void UpdateItems() {
			if(Panel == null) return;
			Panel.Children.Clear();
			Panel.ColumnDefinitions.Clear();
			if(Group == null) return;
			int visibleCount = Group.VisibleCount;
			for(int i = 0; i < visibleCount; i++) {
				PivotGridField field = Group[i];				
				FieldHeader header = CreateHeader();
				header.HeaderPosition = GetHeaderPosition(i, visibleCount);
				Panel.ColumnDefinitions.Add(new ColumnDefinition());
				header.SetValue(Grid.ColumnProperty, Panel.ColumnDefinitions.Count - 1);
				header.DataContext = null;
				Panel.Children.Add(header);
				header.Bind(field, i, FieldHeadersBase.GetFieldListArea(this));
				if(i < Group.Count - 1) {
					GroupCollapseButton button = CreateGroupCollapseButton();
					button.IsCollapseButton = field.ExpandedInFieldsGroup;
					button.Field = field;
					Panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
					button.SetValue(Grid.ColumnProperty, Panel.ColumnDefinitions.Count - 1);
					Panel.Children.Add(button);
				}
			}
		}
		protected virtual GroupCollapseButton CreateGroupCollapseButton() {
			return new GroupCollapseButton();
		}
		HeaderPosition GetHeaderPosition(int i, int count) {
			if(i == 0) return count > 1 ? HeaderPosition.Left : HeaderPosition.Single;
			if(i == count - 1) return HeaderPosition.Right;
			return HeaderPosition.Middle;
		}
		protected virtual FieldHeader CreateHeader() {
			return new InnerGroupHeader();
		}
		protected override IDragElement CreateDragElementCore(Point offset) {
			return new FieldHeaderDragElement(this, Group, offset);
		}
		protected override bool IsGroupHeader {
			get { return true; }
		}
		protected virtual void OnDataContextChanged() {
			Group = DataContext as PivotGridGroup;
			Field = Group == null ? null : Group.FirstField;
		}
		protected override object GetWidthSource() {
			return Group;
		}
	}
	[TemplateVisualState(Name = CollapseButtonState, GroupName = ButtonsGroup)]
	[TemplateVisualState(Name = ExpadButtonState, GroupName = ButtonsGroup)]
	public class GroupCollapseButton :
#if !SL
		Control
#else
		ToggleButton
#endif
		{
		const string CollapseButtonState = "CollapseButton", ExpadButtonState = "ExpandButton", ButtonsGroup = "Buttons";
		public static readonly DependencyProperty IsCollapseButtonProperty;
		public static readonly DependencyProperty FieldProperty;
		static GroupCollapseButton() {
			Type ownerType = typeof(GroupCollapseButton);
			IsCollapseButtonProperty = DependencyPropertyManager.Register("IsCollapseButton", typeof(bool), ownerType,
				new PropertyMetadata(true, (d, e) => ((GroupCollapseButton)d).OnIsCollapseButtonChanged() ));
			FieldProperty = DependencyPropertyManager.Register("Field", typeof(PivotGridField), ownerType,
				new PropertyMetadata(null));
		}
		public GroupCollapseButton() {
			Loaded += new RoutedEventHandler(OnLoaded);
#if !SL
			MouseLeftButtonDown += OnMouseClick;
#endif
			SetDefaultStyleKey();
		}
		protected virtual void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(GroupCollapseButton));
		}	  
		public bool IsCollapseButton {
			get { return (bool)GetValue(IsCollapseButtonProperty); }
			set { SetValue(IsCollapseButtonProperty, value); }
		}
		public PivotGridField Field {
			get { return (PivotGridField)GetValue(FieldProperty); }
			set { SetValue(FieldProperty, value); }
		}
		void OnIsCollapseButtonChanged() {
			VisualStateManager.GoToState(this, IsCollapseButton ? CollapseButtonState : ExpadButtonState, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OnIsCollapseButtonChanged();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			OnIsCollapseButtonChanged();
		}
#if !SL
		void OnMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			DoClick();
		}
#else
		protected override void OnClick() {
			base.OnClick();
			DoClick();
		}
#endif
		public void DoClick() {
			if(Field == null) return;
			if(Field.Data == null) {
				Field.ExpandedInFieldsGroup = !IsCollapseButton;
			} else {
				Field.Data.ChangeFieldExpandedInFieldsGroupAsync(Field.InternalField, !IsCollapseButton, false);
			}
		}
	}
}
