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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using DevExpress.Data.Utils;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IBindingSourceProvider {
		object Source { get; }
		event EventHandler SourceChanged;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class BindingExpression : ICloneable {
		readonly Type sourceType;
		readonly MemberInfo memberInfo;
		Lazy<AnnotationAttributes> annotationAttributes;
		Lazy<string> nameCore;
		Lazy<string> displayNameCore;
		Lazy<string> descriptionCore;
		Lazy<string> groupNameCore;
		Lazy<bool> isHiddenCore;
		protected BindingExpression(Type sourceType, MemberInfo memberInfo) {
			this.sourceType = sourceType;
			this.memberInfo = memberInfo;
			this.annotationAttributes = new Lazy<AnnotationAttributes>(GetAnnotationAttributes);
			this.nameCore = new Lazy<string>(GetName);
			this.displayNameCore = new Lazy<string>(GetDisplayName);
			this.descriptionCore = new Lazy<string>(GetDescription);
			this.groupNameCore = new Lazy<string>(GetGroupName);
			this.isHiddenCore = new Lazy<bool>(GetIsHidden);
		}
		protected internal IBindingSourceProvider Provider;
		protected Type SourceType {
			get { return sourceType; }
		}
		protected MemberInfo MemberInfo {
			get { return memberInfo; }
		}
		protected AnnotationAttributes AnnotationAttributes {
			get { return annotationAttributes.Value; }
		}
		[Browsable(false)]
		public string Name {
			get { return nameCore.Value; }
		}
		[Browsable(false)]
		public string DisplayName {
			get { return displayNameCore.Value; }
		}
		[Browsable(false)]
		public string Description {
			get { return descriptionCore.Value; }
		}
		[Browsable(false)]
		public string GroupName {
			get { return groupNameCore.Value; }
		}
		[Browsable(false)]
		public bool IsHidden {
			get { return isHiddenCore.Value; }
		}
		protected internal abstract void Bind(object source);
		protected internal abstract void Unbind();
		AnnotationAttributes GetAnnotationAttributes() {
			return new AnnotationAttributes(MemberInfoHelper.GetAttributes(MVVMTypesResolver.Instance, MemberInfo));
		}
		protected virtual bool GetIsHidden() {
			return AnnotationAttributes.HasDisplayAttribute &&
				(!AnnotationAttributes.AutoGenerateField.GetValueOrDefault(true)) ||
				(AnnotationAttributes.Order.GetValueOrDefault() < 0);
		}
		protected virtual string GetName() {
			return MemberInfo.Name;
		}
		protected virtual string GetDisplayName() {
			return AnnotationAttributes.GetColumnCaption(AnnotationAttributes);
		}
		protected virtual string GetDescription() {
			return AnnotationAttributes.GetColumnDescription(AnnotationAttributes);
		}
		protected virtual string GetGroupName() {
			return annotationAttributes.Value.GroupName;
		}
		public override string ToString() {
			string expressionName = GetExpressionName();
			string targetName = GetTargetName();
			if(string.IsNullOrEmpty(targetName))
				return expressionName;
			return expressionName + targetName;
		}
		public virtual string GetExpressionName() {
			return GetType().Name.Replace("BindingExpression", string.Empty) + "(" + MemberInfo.Name + ")";
		}
		protected virtual string GetTargetName() {
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public abstract object[] GetSerializerParameters();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static CommandBindingExpression CreateCommandBinding(Type viewModelType, string commandName, ISupportCommandBinding target) {
			return new CommandBindingExpression(viewModelType, viewModelType.GetMethod(commandName)) { Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static CancelCommandBindingExpression CreateCancelCommandBinding(Type viewModelType, string commandName, ISupportCommandBinding target) {
			return new CancelCommandBindingExpression(viewModelType, viewModelType.GetMethod(commandName)) { Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ParameterizedCommandBindingExpression CreateParameterizedCommandBinding(Type viewModelType, string commandName, string propertyName, ISupportCommandBinding target) {
			return new ParameterizedCommandBindingExpression(viewModelType, viewModelType.GetMethod(commandName)) { Target = target, ParameterMember = propertyName };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static PropertyBindingExpression CreatePropertyBinding(Type viewModelType, string propertyName, IComponent target, string bindingMember) {
			return new PropertyBindingExpression(viewModelType, viewModelType.GetProperty(propertyName)) { Target = target, BindingMember = bindingMember };
		}
		object ICloneable.Clone() {
			return Clone();
		}
		protected abstract BindingExpression Clone();
		public static bool AreEqual(BindingExpression e1, BindingExpression e2) {
			if(object.ReferenceEquals(e1, e2)) return true;
			return (e1 != null && e2 != null)
				&& (e1.MemberInfo == e2.MemberInfo)
				&& (e1.SourceType == e2.SourceType);
		}
		protected string GetComponentName(object target) {
			IComponent component = target as IComponent;
			return (component != null && component.Site != null) ? component.Site.Name : null;
		}
		protected string GetNameFromProperty(object target) {
			Type targetType = target.GetType();
			var pName = targetType.GetProperty("Name");
			return (pName != null) ? pName.GetValue(target, null) as string : null;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.CommandBindingExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class CommandBindingExpression : BindingExpression {
		Lazy<string> imageNameOrUriCore;
		internal CommandBindingExpression(Type sourceType, MethodInfo mInfo)
			: base(sourceType, mInfo) {
			this.imageNameOrUriCore = new Lazy<string>(GetImageNameOrUri);
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		public ISupportCommandBinding Target { get; set; }
		protected internal override void Bind(object source) {
			if(Target == null || source == null) return;
			var commandProperty = MemberInfoHelper.GetCommandProperty(source, MVVMTypesResolver.Instance, MemberInfo);
			if(commandProperty != null) {
				var command = GetCommand(source, commandProperty);
				if(command != null)
					commandBindingToken = BindCommand(source, command);
			}
		}
		IDisposable commandBindingToken;
		protected virtual IDisposable BindCommand(object source, object command) {
			return Target.BindCommand(command);
		}
		protected internal override void Unbind() {
			Ref.Dispose(ref commandBindingToken);
		}
		protected virtual object GetCommand(object source, PropertyInfo commandProperty) {
			return commandProperty.GetValue(source, null);
		}
		[Browsable(false)]
		public string ImageNameOrUri {
			get { return imageNameOrUriCore.Value; }
		}
		protected override string GetName() {
			return MemberInfoHelper.GetCommandName(MVVMTypesResolver.Instance, MemberInfo);
		}
		protected override string GetDisplayName() {
			return MemberInfoHelper.GetCommandDisplayName(MVVMTypesResolver.Instance, MemberInfo, AnnotationAttributes);
		}
		protected virtual string GetImageNameOrUri() {
			return MemberInfoHelper.GetCommandImageNameOrUri(MVVMTypesResolver.Instance, MemberInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override object[] GetSerializerParameters() {
			return new object[] { SourceType, MemberInfo.Name, Target };
		}
		protected override BindingExpression Clone() {
			return new CommandBindingExpression(SourceType, (MethodInfo)MemberInfo);
		}
		protected override string GetTargetName() {
			string targetName = (Target != null) ? GetComponentName(Target) ?? GetNameFromProperty(Target) : "{target}";
			return string.Format(" => {0}", targetName);
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.ParameterizedCommandBindingExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class ParameterizedCommandBindingExpression : CommandBindingExpression {
		[Browsable(false)]
		public Type ParameterType { get; private set; }
		[Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.CommandBindingParameterConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public string ParameterMember { get; set; }
		internal ParameterizedCommandBindingExpression(Type sourceType, MethodInfo mInfo) :
			base(sourceType, mInfo) {
			var parameters = mInfo.GetParameters();
			if(parameters.Length == 1) {
				this.ParameterType = parameters[0].ParameterType;
				this.ParameterMember = GuessCommandParameterMember(SourceType, ParameterType);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override object[] GetSerializerParameters() {
			return new object[] { SourceType, MemberInfo.Name, ParameterMember, Target };
		}
		protected override BindingExpression Clone() {
			return new ParameterizedCommandBindingExpression(SourceType, (MethodInfo)MemberInfo);
		}
		protected override IDisposable BindCommand(object source, object command) {
			if(string.IsNullOrEmpty(ParameterMember)) return null;
			var accessor = InterfacesProxy.GetAccessor(SourceType, ParameterMember);
			return Target.BindCommand(command, () => accessor(source));
		}
		string GuessCommandParameterMember(Type sourceType, Type parameterType) {
			string commandParameterFromAttribute = MemberInfoHelper.GetCommandParameter(MVVMTypesResolver.Instance, MemberInfo);
			if(!string.IsNullOrEmpty(commandParameterFromAttribute))
				return commandParameterFromAttribute;
			var parameterProperty = sourceType.GetProperties()
				.Where(p => p.PropertyType == parameterType).FirstOrDefault();
			return (parameterProperty != null) ? parameterProperty.Name : null;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.CancelCommandBindingExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class CancelCommandBindingExpression : CommandBindingExpression {
		internal CancelCommandBindingExpression(Type sourceType, MethodInfo mInfo) :
			base(sourceType, mInfo) {
		}
		protected override string GetName() {
			return base.GetName() + "Cancel";
		}
		protected override object GetCommand(object source, PropertyInfo commandProperty) {
			return MVVMContext.GetCancelCommandCore(base.GetCommand(source, commandProperty));
		}
		protected override BindingExpression Clone() {
			return new CancelCommandBindingExpression(SourceType, (MethodInfo)MemberInfo);
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.PropertyBindingExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class PropertyBindingExpression : BindingExpression {
		internal PropertyBindingExpression(Type sourceType, PropertyInfo pInfo)
			: base(sourceType, pInfo) {
			BindingMemberType = pInfo.PropertyType;
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		public IComponent Target { get; set; }
		[Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.BindingMemberParameterConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public string BindingMember { get; set; }
		[Browsable(false)]
		public Type BindingMemberType { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override object[] GetSerializerParameters() {
			return new object[] { SourceType, MemberInfo.Name, Target, BindingMember };
		}
		protected override BindingExpression Clone() {
			return new PropertyBindingExpression(SourceType, (PropertyInfo)MemberInfo);
		}
		IPropertyBinding bindingCore;
		protected internal override void Bind(object source) {
			Ref.Dispose(ref bindingCore);
			if(Target != null && !string.IsNullOrEmpty(BindingMember)) {
				var setBinding = InterfacesProxy.GetSetBindingMethod(Target.GetType(), BindingMemberType, BindingMember);
				bindingCore = setBinding((MVVMContext)Provider, Target, MemberInfo.Name);
			}
		}
		protected internal override void Unbind() {
			Ref.Dispose(ref bindingCore);
		}
		protected override string GetTargetName() {
			if(string.IsNullOrEmpty(BindingMember)) 
				return null;
			string targetName = (Target != null) ? GetComponentName(Target) ?? GetNameFromProperty(Target) : "{target}";
			bool twoWay = (Target != null) && (Target is INotifyPropertyChanged || MemberInfoHelper.HasChangedEvent(Target.GetType(), BindingMember));
			return string.Format(twoWay ? " <=> {0}.{1}" : " => {0}.{1}", targetName, BindingMember);
		}
	}
	[Editor("DevExpress.XtraEditors.MVVM.Design.BindingExpressionsEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class BindingExpressionCollection : CollectionBase, IEnumerable<BindingExpression>, IDisposable {
		IBindingSourceProvider provider;
		public BindingExpressionCollection(IBindingSourceProvider provider) {
			this.provider = provider;
			if(provider != null)
				provider.SourceChanged += provider_SourceChanged;
		}
		void IDisposable.Dispose() {
			if(provider != null)
				provider.SourceChanged -= provider_SourceChanged;
			RemoveBindings();
		}
		public void CreateBindings() {
			if(provider != null) {
				foreach(BindingExpression expression in InnerList)
					expression.Bind(provider.Source);
			}
		}
		public void UpdateBindings() {
			foreach(BindingExpression expression in InnerList) {
				expression.Unbind();
				if(provider != null)
					expression.Bind(provider.Source);
			}
		}
		public void RemoveBindings() {
			foreach(BindingExpression expression in InnerList)
				expression.Unbind();
		}
		void provider_SourceChanged(object sender, EventArgs e) {
			UpdateBindings();
		}
		public void Add(BindingExpression expression) {
			((IList)this).Add(expression);
		}
		public void Remove(BindingExpression expression) {
			((IList)this).Remove(expression);
		}
		public void AddRange(BindingExpression[] expressions) {
			for(int i = 0; i < expressions.Length; i++)
				((IList)this).Add(expressions[i]);
		}
		public int IndexOf(BindingExpression expression) {
			return InnerList.IndexOf(expression);
		}
		public bool Contains(BindingExpression expression) {
			return InnerList.Contains(expression);
		}
		protected override void OnInsert(int index, object value) {
			BindingExpression expression = value as BindingExpression;
			if(expression != null) {
				expression.Provider = provider;
				if(provider != null && provider.Source != null)
					expression.Bind(provider.Source);
			}
		}
		protected override void OnRemove(int index, object value) {
			BindingExpression expression = value as BindingExpression;
			if(expression != null) {
				expression.Unbind();
				expression.Provider = null;
			}
		}
		protected override void OnClear() {
			RemoveBindings();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, oldValue as BindingExpression);
			RaiseCollectionChanged(CollectionChangeAction.Add, newValue as BindingExpression);
		}
		protected override void OnInsertComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as BindingExpression);
		}
		protected override void OnRemoveComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as BindingExpression);
		}
		protected override void OnClearComplete() {
			RaiseCollectionChanged(CollectionChangeAction.Refresh, null);
		}
		public BindingExpression this[int index] {
			get { return (BindingExpression)InnerList[index]; }
		}
		public BindingExpression[] ToArray() {
			BindingExpression[] array = new BindingExpression[Count];
			InnerList.CopyTo(array, 0);
			return array;
		}
		public event CollectionChangeEventHandler CollectionChanged;
		protected void RaiseCollectionChanged(CollectionChangeAction action, BindingExpression expression) {
			var handler = CollectionChanged;
			if(handler != null) handler(this, new CollectionChangeEventArgs(action, expression));
		}
		#region IEnumerable<BindingExpression> Members
		IEnumerator<BindingExpression> IEnumerable<BindingExpression>.GetEnumerator() {
			foreach(BindingExpression expression in InnerList)
				yield return expression;
		}
		#endregion
	}
}
