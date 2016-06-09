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

namespace DevExpress.Data.Filtering.Helpers {
	using System;
	using System.Collections;
	using System.Globalization;
	using System.ComponentModel;
	using System.Reflection;
	using DevExpress.Data.Filtering.Exceptions;
	using System.IO;
	using System.Collections.Generic;
	using System.Text;
	using System.Linq;
	using DevExpress.XtraEditors.Filtering;
	using DevExpress.Utils;
	public class CriteriaValidatorError {
		string text;
		public CriteriaValidatorError(string text) {
			this.text = text;
		}
		public string Text { get { return text; } }
	}
	public enum TokenType { Property, Constant, Group, Aggregate, CompareOperator, MathOperator, Function, Predicate, Not, OpenParenthesis, CloseParenthesis, Dot, Unknown };
	public class CriteriaLexerToken {
		CriteriaOperator criteriaOperator;
		int token;
		int position;
		int length;
		public CriteriaLexerToken(CriteriaOperator criteriaOperator, int token, int pos, int len) {
			this.criteriaOperator = criteriaOperator;
			this.position = pos;
			this.token = token;
			this.length = len;
		}
		public CriteriaOperator CriteriaOperator { get { return criteriaOperator; } }
		public int Token { get { return token; } }
		public int Position { get { return position; } }
		public int Length { get { return length; } }
		public int PositionEnd { get { return Position + Length; } }
		public TokenType TokenType { get { return this.ToTokenType(); } }
		public static TokenType GetNextTokenType(TokenType previousToken) {
			switch(previousToken) {
				case TokenType.Aggregate: return TokenType.CompareOperator;
				case TokenType.CompareOperator: return TokenType.Constant;
				case TokenType.Constant: return TokenType.Group;
				case TokenType.Function:  return TokenType.Constant;
				case TokenType.Group: return TokenType.Property;
				case TokenType.MathOperator: return TokenType.Constant;
				case TokenType.Not: return TokenType.Property;
				case TokenType.Predicate: return TokenType.Group;
				case TokenType.Property: return TokenType.CompareOperator;
				case TokenType.OpenParenthesis: return TokenType.Constant;
				case TokenType.CloseParenthesis: return TokenType.Group;
			}
			return TokenType.Unknown;
		}
		protected virtual TokenType ToTokenType() {
			switch (this.Token) {
				case DevExpress.Data.Filtering.Helpers.Token.AND:
				case DevExpress.Data.Filtering.Helpers.Token.OR:
					return TokenType.Group;
				case DevExpress.Data.Filtering.Helpers.Token.AGG_AVG:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_COUNT:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_EXISTS:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_MAX:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_MIN:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_SUM:
				case DevExpress.Data.Filtering.Helpers.Token.AGG_SINGLE:
					return TokenType.Aggregate;
				case DevExpress.Data.Filtering.Helpers.Token.CONST:
					return TokenType.Constant;
				case DevExpress.Data.Filtering.Helpers.Token.OP_EQ:
				case DevExpress.Data.Filtering.Helpers.Token.OP_GE:
				case DevExpress.Data.Filtering.Helpers.Token.OP_GT:
				case DevExpress.Data.Filtering.Helpers.Token.OP_IN:
				case DevExpress.Data.Filtering.Helpers.Token.OP_LE:
				case DevExpress.Data.Filtering.Helpers.Token.OP_LIKE:
				case DevExpress.Data.Filtering.Helpers.Token.OP_LT:
				case DevExpress.Data.Filtering.Helpers.Token.OP_NE:
				case DevExpress.Data.Filtering.Helpers.Token.OP_BETWEEN:
					return TokenType.CompareOperator;
				case DevExpress.Data.Filtering.Helpers.Token.FN_ISNULL:
				case DevExpress.Data.Filtering.Helpers.Token.FUNCTION:
					return TokenType.Function;
				case DevExpress.Data.Filtering.Helpers.Token.COL:
					return TokenType.Property;
				case DevExpress.Data.Filtering.Helpers.Token.IS:
				case DevExpress.Data.Filtering.Helpers.Token.NULL:
				case DevExpress.Data.Filtering.Helpers.Token.NEG:
					return TokenType.Predicate;
				case DevExpress.Data.Filtering.Helpers.Token.NOT:
					return TokenType.Not;
				case '+':
				case '-':
				case '*':
				case '/':
				case '%':
				case '|':
				case '&':
				case '^':
				case '~':
					return TokenType.MathOperator;
				case '(':
					return TokenType.OpenParenthesis;
				case ')':
					return TokenType.CloseParenthesis;
				case '.':
					return TokenType.Dot;
			}
			return TokenType.Unknown;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ListName;
	}
	public class CriteriaLexerTokenHelper {
		readonly string data;
		readonly List<CriteriaLexerToken> tokenList;
		public CriteriaLexerTokenHelper(string data) {
			this.data = data;
			this.tokenList = Analyze();
		}
		public List<CriteriaLexerToken> TokenList { get { return tokenList; } }
		public CriteriaLexerToken FindToken(int position) {
			int index = FindTokenIndex(position);
			return index > -1 ? TokenList[index] : null;
		}
		public bool IsAggregate(int position) {
			int index = FindTokenIndex(position);
			if(index < 0) {
				index = TokenList.Count - 1;
			}
			for(int i = index; i >= 0; i--) {
				TokenType tokenType = TokenList[i].TokenType;
				if(tokenType == TokenType.Aggregate) return true;
				if(tokenType == TokenType.OpenParenthesis || tokenType == TokenType.CloseParenthesis || tokenType == TokenType.Property) continue;
				break;
			}
			return false;
		}
		public TokenType GetTokenType(int position) {
			CriteriaLexerToken token = FindToken(position);
			return token != null ? token.TokenType : TokenType.Unknown;
		}
		public string GetTokenText(int position) {
			return GetTokenText(FindToken(position));
		}
		public object GetTokenValue(int position) {
			CriteriaLexerToken token = FindToken(position);
			if(token == null) return null;
			OperandValue value = token.CriteriaOperator as OperandValue;
			return object.Equals(value, null) ? null : value.Value;
		}
		public void GetTokenPositionLength(int position, out int startPosition, out int length) {
			startPosition = 0;
			length = 0;
			CriteriaLexerToken token = FindToken(position);
			if (token == null) return;
			startPosition = token.Position;
			length = token.Length;
		}
		public CriteriaLexerToken GetNeighborToken(int position, bool leftNeighbor) {
			for(int i = 0; i < TokenList.Count; i++) {
				CriteriaLexerToken token = TokenList[i];
				if(token.Position > position && !leftNeighbor) return token;
				if(token.PositionEnd < position && leftNeighbor) {
					if(i == TokenList.Count - 1) return token;
					if(TokenList[i + 1].PositionEnd >= position) return token;
				}
			}
			return null;
		}
		public TokenType GetNextTokenType(int position) {
			CriteriaLexerToken token = GetNeighborToken(position, true);
			if(token != null) {
				if(token.TokenType == TokenType.Property) {
					CriteriaLexerToken curToken = FindToken(position);
					TokenType curTokenType = curToken != null ? curToken.TokenType : TokenType.Unknown;
					if(curTokenType == TokenType.Dot) return TokenType.Property;
					if(curToken != null && curToken.Token == (int)'[') return TokenType.Property;
				}
				if(token.TokenType == TokenType.Constant && GetTokenText(position).Trim() == ",") return TokenType.Constant;
				if(token.TokenType == TokenType.Unknown) {
					if(data.Substring(token.Position, token.Length).Trim() == ",") return TokenType.Constant;
					if(token.Token == (int)']' && GetTokenType(position) == TokenType.Dot) return TokenType.Aggregate;
					if(token.Token == (int)'[') return TokenType.Property;
				}
				if(token.TokenType == TokenType.CloseParenthesis && IsAggregate(position)) {
					return TokenType.CompareOperator;
				}
				if(token.TokenType == TokenType.OpenParenthesis && IsAggregate(position)) {
					return TokenType.Property;
				}
				if(token.TokenType == TokenType.Aggregate)  return TokenType.Property;
				return CriteriaLexerToken.GetNextTokenType(token.TokenType);
			}
			return GetTokenType(position);
		}
		public void GetNeighborTokenRange(int position, bool leftNeighbor, out int startPosition, out int length) {
			startPosition = 0;
			length = 0;
			CriteriaLexerToken token = GetNeighborToken(position, leftNeighbor);
			if(token == null) return;
			startPosition = token.Position;
			length = token.Length;
		}
		public TokenType GetNeighborTokenType(int position, bool leftNeighbor) {
			CriteriaLexerToken token = GetNeighborToken(position, leftNeighbor);
			return token != null ? token.TokenType : TokenType.Unknown;
		}
		public string ConvertProperties(ConvertPropertyDelegate convertMethod) {
			return ConvertProperties(false, convertMethod);
		}
		public string ConvertProperties(bool forceBrackets, ConvertPropertyDelegate convertMethod) {
			UpdateListProperty(forceBrackets);
			CriteriaLexerToken previousToken = null;
			StringBuilder sb = new StringBuilder();
			string lastListName = null;
			foreach(CriteriaLexerToken token in TokenList) {
				if (lastListName == null && token.ListName != null) {
					lastListName = token.ListName;
				}
				int positionEnd = previousToken != null ? previousToken.PositionEnd : 0;
				if(token.Position > positionEnd) {
					sb.Append(data.Substring(positionEnd, token.Position - positionEnd));
				}
				previousToken = token;
				sb.Append(ConvertPropertyToken(forceBrackets, convertMethod, token, token.ListName ?? lastListName));
			}
			return sb.ToString();
		}
		public string ConvertConstants(ConvertConstantDelegate convertMethod) {
			UpdateListProperty(true);
			CriteriaLexerToken previousToken = null;
			CriteriaLexerToken lastPropertyToken = null;
			StringBuilder sb = new StringBuilder();
			foreach(CriteriaLexerToken token in TokenList) {
				int positionEnd = previousToken != null ? previousToken.PositionEnd : 0;
				if(token.Position > positionEnd) {
					sb.Append(data.Substring(positionEnd, token.Position - positionEnd));
				}
				previousToken = token;
				if(token.TokenType == TokenType.Property) {
					lastPropertyToken = token;
				}
				sb.Append(ConvertConstantToken(convertMethod, lastPropertyToken, token, lastPropertyToken == null ? "" : lastPropertyToken.ListName));
			}
			return sb.ToString();
		}
		public void UpdateListProperty(bool forceBrackets) {
			CriteriaLexerToken previousToken = null;
			string listName = string.Empty;
			int listBrasketCount = 0;
			foreach (CriteriaLexerToken token in TokenList) {
				if (string.IsNullOrEmpty(listName) && previousToken != null && previousToken.TokenType == TokenType.Property) {
					if(forceBrackets) {
						listName = ((OperandProperty)previousToken.CriteriaOperator).PropertyName;
					} else {
						listName = GetTokenText(previousToken);
					}
				}
				if (token.TokenType == TokenType.Group && listBrasketCount == 0) {
					listName = string.Empty;
				}
				if (token.TokenType == TokenType.Unknown && token.Length == 1 && !string.IsNullOrEmpty(listName)) {
					if (token.Token == (int)'[') listBrasketCount++;
					if (token.Token == (int)']') listBrasketCount--;
				}
				previousToken = token;
				if (listBrasketCount > 0) {
					token.ListName = listName;
				}
			}
		}
		string ConvertPropertyToken(bool forceBrackets, ConvertPropertyDelegate convertMethod, CriteriaLexerToken token, string listName) {
			string tokenText = GetTokenText(token);
			if(token.TokenType == TokenType.Property) {
				if(forceBrackets) tokenText = ((token.CriteriaOperator) as OperandProperty).PropertyName;
				if(convertMethod != null) tokenText = convertMethod(listName, tokenText);
				if(forceBrackets) tokenText = "[" + tokenText + "]";
			}
			return tokenText;
		}
		string ConvertConstantToken(ConvertConstantDelegate convertMethod, CriteriaLexerToken propertyToken, CriteriaLexerToken token, string listName) {
			string tokenText = GetTokenText(token);
			if(token.TokenType == TokenType.Constant && propertyToken != null) {
				string propertyName = GetTokenText(propertyToken);
				propertyName = propertyName.Replace("[", string.Empty);
				propertyName = propertyName.Replace("]", string.Empty);
				if(convertMethod != null) tokenText = convertMethod(listName, propertyName, tokenText, token);
			}
			return tokenText;
		}
		protected int FindTokenIndex(int position) {
			for(int i = 0; i < TokenList.Count; i ++) {
				if(TokenList[i].Position <= position && position <= TokenList[i].PositionEnd) return i;
			}
			return -1;
		}
		string GetTokenText(CriteriaLexerToken token) {
			return token != null ? data.Substring(token.Position, token.Length) : string.Empty;
		}
		List<CriteriaLexerToken> Analyze() {
			List<CriteriaLexerToken> list = new List<CriteriaLexerToken>();
			try {
				using(StringReader reader = new StringReader(data)) {
					FilterEditorCriteriaLexer lexer = new FilterEditorCriteriaLexer(reader);
					lexer.SkipBlanks();
					int oldPosition = lexer.Position;
					while(lexer.Advance()) {
						CriteriaLexerToken currentToken = new CriteriaLexerToken(lexer.CurrentValue as CriteriaOperator,
							lexer.CurrentToken, oldPosition, lexer.Position - oldPosition);
						CriteriaLexerToken prevToken;
						if(currentToken.TokenType == TokenType.OpenParenthesis && list.Count > 0 && (prevToken = list[list.Count - 1]).TokenType == TokenType.Property) {
							list[list.Count - 1] = new CriteriaLexerToken(null, DevExpress.Data.Filtering.Helpers.Token.FUNCTION, prevToken.Position, prevToken.Length);
						}
						list.Add(currentToken);
						lexer.SkipBlanks();
						oldPosition = lexer.Position;
					}
				}
			} catch { }
			return list;
		}
	}
	class FilterEditorCriteriaLexer : CriteriaLexer {
		public FilterEditorCriteriaLexer(TextReader reader) : base(reader) { }
		public override void YYError(string message, params object[] args) { }
	}
	public delegate IBoundProperty GetBoundPropertyByFieldName(string fieldName);
	public class ErrorsEvaluatorCriteriaValidator : BoundPropertyList, IClientCriteriaVisitor {
		protected List<CriteriaValidatorError> errors;
		const string IncorrectDataAssignedToProperty = "A value with incorrect data type was assigned to property '{0}'";
		public ErrorsEvaluatorCriteriaValidator(List<IBoundProperty> properties) : base(properties) {
			this.errors = new List<CriteriaValidatorError>();
		}
		public CriteriaValidatorError this[int index] { get { return this.errors[index]; } }
		public int Count { get { return this.errors.Count; } }
		public virtual void Visit(BetweenOperator theOperator) {
			ValidateOperandType(theOperator.TestExpression, theOperator.BeginExpression);
			ValidateOperandType(theOperator.TestExpression, theOperator.EndExpression);
			Validate(theOperator.TestExpression);
			Validate(theOperator.EndExpression);
			Validate(theOperator.BeginExpression);
		}
		public virtual void Visit(BinaryOperator theOperator) {
			ValidateOperandType(theOperator.LeftOperand, theOperator.RightOperand);
			Validate(theOperator.LeftOperand);
			Validate(theOperator.RightOperand);
		}
		public virtual void Visit(UnaryOperator theOperator) {
			Validate(theOperator.Operand);
		}
		public virtual void Visit(InOperator theOperator) {
			foreach (CriteriaOperator item in theOperator.Operands) {
				ValidateOperandType(theOperator.LeftOperand, item);
			}
			Validate(theOperator.LeftOperand);
			Validate(theOperator.Operands);
		}
		public virtual void Visit(GroupOperator theOperator) {
			foreach (CriteriaOperator item in theOperator.Operands) {
				if (!verifyOperandType(item, typeof(System.Boolean))) {
					throw new Exception("A value with incorrect data type was assigned to property");
				}
			}
			Validate(theOperator.Operands);
		}
		public virtual void Visit(OperandValue theOperand) {
		}
		public virtual void Visit(FunctionOperator theOperator) {
			Validate(theOperator.Operands);
		}
		public virtual void Visit(OperandProperty theOperand) {
			if (ReferenceEquals(theOperand, null)) {
				throw new Exception("Property is null");
			}
			IBoundProperty property = GetFilterPropertyByPropertyName(theOperand);
			if (property == null || property.IsAggregate || IsListProperty(property)) {
				throw new Exception(string.Format("Invalid property '{0}'", theOperand.PropertyName));
			}
		}
		bool IsListProperty(IBoundProperty property) {
			if(property.IsList) return true;
			if(property.Parent == null || Properties == null || Properties.Contains(property)) return false;
			return IsListProperty(property.Parent);
		}
		public virtual void Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel)
				throw new InvalidOperationException("can't evaluate top level aggregate on single object, collection property expected");	
			IBoundProperty listProperty = GetFilterPropertyByPropertyName(theOperand.CollectionProperty);
			if(listProperty == null || !listProperty.IsList) {
				throw new Exception(string.Format("Invalid property '{0}'", theOperand.CollectionProperty.PropertyName));
			}
			if(!CriteriaOperator.Equals(theOperand.Condition, null)) {
				ErrorsEvaluatorCriteriaValidator validator = new ErrorsEvaluatorCriteriaValidator(listProperty.Children);
				validator.Validate(theOperand.Condition);
				this.errors.AddRange(validator.errors);
			}
		}
		public virtual void Visit(JoinOperand theOperand) {
		}
		public void Clear() {
			this.errors.Clear();
		}
		public void Validate(string filter) {
			try {
				CriteriaOperator criteria = CriteriaOperator.Parse(filter);
				Validate(criteria);
			}
			catch(Exception e) {
				this.errors.Add(new CriteriaValidatorError(e.Message));
			}
		}
		public void Validate(IList operands) {
			foreach (CriteriaOperator operand in operands)
				Validate(operand);
		}
		public void Validate(CriteriaOperator criteria) {
			try {
				if (!ReferenceEquals(criteria, null))
					criteria.Accept(this);
			} catch (Exception e) {
				this.errors.Add(new CriteriaValidatorError(e.Message));
			}
		}
		protected void ValidateOperandType(CriteriaOperator op1, CriteriaOperator op2) {
			IBoundProperty filterProperty;
			if(!verifyOperandType(op1, op2, out filterProperty)) {
				string filterPropertyName = filterProperty != null ? filterProperty.Name : string.Empty;
				throw new Exception(string.Format(IncorrectDataAssignedToProperty, filterPropertyName));
			}
		}
		protected bool verifyOperandType(CriteriaOperator op1, CriteriaOperator op2, out IBoundProperty filterProperty) {
			OperandValue value = null;
			filterProperty = null;
			if (op1 is OperandProperty && op2 is OperandValue) {
				filterProperty = GetFilterPropertyByPropertyName((OperandProperty)op1);
				value = (OperandValue)op2;
			}
			if (op1 is OperandValue && op2 is OperandProperty) {
				filterProperty = GetFilterPropertyByPropertyName((OperandProperty)op2);
				value = (OperandValue)op1;
			}
			if (filterProperty != null) {
				value.Value = GetCorrectFiltereColumnValue(filterProperty, value.Value);
				return TypeConvertionValidator.CanConvert(GetFilterColumnValueForValidation(filterProperty, value.Value), GetFilterColumnType(filterProperty));
			}
			return true;
		}
		protected virtual object GetFilterColumnValueForValidation(IBoundProperty filterProperty, object value) {
			return value;
		}
		protected virtual object GetCorrectFiltereColumnValue(IBoundProperty filterProperty, object value) {
			return value;
		}
		protected virtual Type GetFilterColumnType(IBoundProperty filterProperty) {
			return filterProperty.Type;
		}
		protected bool verifyOperandType(CriteriaOperator op, Type type) {
			if (op is OperandValue) {
				return TypeConvertionValidator.CanConvert((op as OperandValue).Value, type);
			}
			return true;
		}
		protected IBoundProperty GetFilterPropertyByPropertyName(OperandProperty property) {
			if(Equals(property, null)) return null;
			return this[property.PropertyName];
		}
	}
	public class TypeConvertionValidator {
		public static bool TryConvert(object objValue, Type destinationType, out object result) {
			result = null;
			if(objValue == null) {
				if(destinationType.IsValueType()) {
					result = Activator.CreateInstance(destinationType);
				}
				return true;
			}
			if(objValue.GetType() == destinationType) {
				result = objValue;
				return true;
			}
			bool canConvert = false;
			string value = objValue.ToString();
			switch(DXTypeExtensions.GetTypeCode(destinationType)) {
				case TypeCode.Boolean:
					bool boolResult;
					canConvert = Boolean.TryParse(value, out boolResult);
					result = boolResult;
					break;
				case TypeCode.Byte:
					Byte byteResult;
					canConvert = Byte.TryParse(value, out byteResult);
					result = byteResult;
					break;
				case TypeCode.Char:
					char charResult;
					canConvert = Char.TryParse(value, out charResult);
					result = charResult;
					break;
				case TypeCode.DateTime:
					DateTime dateTimeResult;
					canConvert = DateTime.TryParse(value, out dateTimeResult);
					result = dateTimeResult;
					break;
				case TypeCode.Decimal:
					Decimal decimalResult;
					canConvert = Decimal.TryParse(value, out decimalResult);
					result = decimalResult;
					break;
				case TypeCode.Double:
					Double doubleResult;
					canConvert = Double.TryParse(value, out doubleResult);
					result = doubleResult;
					break;
				case TypeCode.Int16:
					Int16 int16Result;
					canConvert = Int16.TryParse(value, out int16Result);
					result = int16Result;
					break;
				case TypeCode.Int32:
					if(destinationType.IsEnum()) {
						canConvert = true;
						try {
							result = Enum.Parse(destinationType, value, false);
						} catch {
							canConvert = false;
						} finally { }
					} else {
						Int32 int32Result;
						canConvert = Int32.TryParse(value, out int32Result);
						result = int32Result;
					}
					break;
				case TypeCode.Int64:
					Int64 int64Result;
					canConvert = Int64.TryParse(value, out int64Result);
					result = int64Result;
					break;
				case TypeCode.SByte:
					SByte sbyteResult;
					canConvert = SByte.TryParse(value, out sbyteResult);
					result = sbyteResult;
					break;
				case TypeCode.Single:
					Single singleResult;
					canConvert = Single.TryParse(value, out singleResult);
					result = singleResult;
					break;
				case TypeCode.String:
					result = value;
					canConvert = true;
					break;
				case TypeCode.UInt16:
					UInt16 uint16Result;
					canConvert = UInt16.TryParse(value, out uint16Result);
					result = uint16Result;
					break;
				case TypeCode.UInt32:
					UInt32 uint32Result;
					canConvert = UInt32.TryParse(value, out uint32Result);
					result = uint32Result;
					break;
				case TypeCode.UInt64:
					UInt64 uint64Result;
					canConvert = UInt64.TryParse(value, out uint64Result);
					result = uint64Result;
					break;
				case TypeCode.Object:
					result = objValue;
					canConvert = destinationType.IsAssignableFrom(objValue.GetType());
					break;
			}
			return canConvert;
		}
		public static bool CanConvert(object objValue, Type destinationType) {
			if(objValue == null) return true;
			object dummy;
			return TryConvert(objValue, destinationType, out dummy);
		}
		public static bool CanConvertType(Type sourceType, Type destinationType) {
			if(sourceType == null || destinationType == null) return false;
			if(sourceType == destinationType) return true;
			if(destinationType.IsAssignableFrom(sourceType)) return true;
			if(destinationType == typeof(string)) return true;
			TypeCode sourceTypeCode = DXTypeExtensions.GetTypeCode(sourceType);
			TypeCode destinationTypeCode = DXTypeExtensions.GetTypeCode(destinationType);
			switch(destinationTypeCode) {
				case TypeCode.DateTime:
					return sourceTypeCode == TypeCode.DateTime;
				case TypeCode.Char:
					return sourceTypeCode == TypeCode.Char;
				case TypeCode.Double:
					return sourceTypeCode == TypeCode.Byte ||
						sourceTypeCode == TypeCode.SByte ||
						sourceTypeCode == TypeCode.Int16 ||
						sourceTypeCode == TypeCode.UInt16 ||
						sourceTypeCode == TypeCode.Int32 ||
						sourceTypeCode == TypeCode.UInt32 ||
						sourceTypeCode == TypeCode.Int64 ||
						sourceTypeCode == TypeCode.UInt64 ||
						sourceTypeCode == TypeCode.Single ||
						sourceTypeCode == TypeCode.Decimal;
				case TypeCode.Single:
					return sourceTypeCode == TypeCode.Byte ||
						sourceTypeCode == TypeCode.SByte ||
						sourceTypeCode == TypeCode.Int16 ||
						sourceTypeCode == TypeCode.UInt16 ||
						sourceTypeCode == TypeCode.Int32 ||
						sourceTypeCode == TypeCode.UInt32 ||
						sourceTypeCode == TypeCode.Int64 ||
						sourceTypeCode == TypeCode.UInt64 ||
						sourceTypeCode == TypeCode.Decimal;
				case TypeCode.Decimal:
					return sourceTypeCode == TypeCode.Byte ||
						sourceTypeCode == TypeCode.SByte ||
						sourceTypeCode == TypeCode.Int16 ||
						sourceTypeCode == TypeCode.UInt16 ||
						sourceTypeCode == TypeCode.Int32 ||
						sourceTypeCode == TypeCode.UInt32 ||
						sourceTypeCode == TypeCode.Int64 ||
						sourceTypeCode == TypeCode.UInt64;
				case TypeCode.Byte:
					return sourceTypeCode == TypeCode.Byte;
				case TypeCode.SByte:
					return sourceTypeCode == TypeCode.SByte;
				case TypeCode.UInt16:
				case TypeCode.Int16:
					return sourceTypeCode == TypeCode.Byte || sourceTypeCode == TypeCode.SByte;
				case TypeCode.UInt32:
				case TypeCode.Int32:
					return sourceTypeCode == TypeCode.Byte ||
						sourceTypeCode == TypeCode.SByte ||
						sourceTypeCode == TypeCode.Int16 ||
						sourceTypeCode == TypeCode.UInt16;
				case TypeCode.UInt64:
				case TypeCode.Int64:
					return sourceTypeCode == TypeCode.Byte ||
						sourceTypeCode == TypeCode.SByte ||
						sourceTypeCode == TypeCode.Int16 ||
						sourceTypeCode == TypeCode.UInt16 ||
						sourceTypeCode == TypeCode.Int32 ||
						sourceTypeCode == TypeCode.UInt32;
			}
			return false;
		}
	}
	public delegate string ConvertPropertyDelegate(string listName, string name);
	public delegate string ConvertConstantDelegate(string listName, string propertyName, string constValue, CriteriaLexerToken constToken);
	public class BinaryOperatorsCollectorHelper: IClientCriteriaVisitor<IEnumerable<BinaryOperator>> {
		BinaryOperatorsCollectorHelper() { }
		IEnumerable<BinaryOperator> IClientCriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(AggregateOperand theOperand) {
			return null;
		}
		IEnumerable<BinaryOperator> IClientCriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(OperandProperty theOperand) {
			return null;
		}
		IEnumerable<BinaryOperator> IClientCriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(JoinOperand theOperand) {
			return null;
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression).Concat(Process(theOperator.BeginExpression)).Concat(Process(theOperator.EndExpression));
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(BinaryOperator theOperator) {
			if(theOperator.LeftOperand is OperandProperty && theOperator.RightOperand is OperandValue && !(theOperator.RightOperand is OperandParameter))
				return Enumerable.Repeat(theOperator, 1);
			else
				return Process(theOperator.LeftOperand).Concat(Process(theOperator.RightOperand));
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(InOperator theOperator) {
			return Process(theOperator.LeftOperand).Concat(Process(theOperator.Operands));
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(GroupOperator theOperator) {
			return Process(theOperator.Operands);
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(OperandValue theOperand) {
			return null;
		}
		IEnumerable<BinaryOperator> ICriteriaVisitor<IEnumerable<BinaryOperator>>.Visit(FunctionOperator theOperator) {
			return Process(theOperator.Operands);
		}
		IEnumerable<BinaryOperator> Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return Enumerable.Empty<BinaryOperator>();
			var rv = (IEnumerable<BinaryOperator>)op.Accept(this);
			return rv ?? Enumerable.Empty<BinaryOperator>();
		}
		IEnumerable<BinaryOperator> Process(IEnumerable<CriteriaOperator> ops) {
			return ops.SelectMany(x => Process(x));
		}
		public static BinaryOperator[] Collect(CriteriaOperator criteria) {
			IEnumerable<BinaryOperator> rv = new BinaryOperatorsCollectorHelper().Process(criteria);
			return rv.ToArray();
		}
	}
	public class DisplayNameVisitor : IClientCriteriaVisitor {
		Stack<string> path = new Stack<string>();
		public IEnumerable<IBoundProperty> Columns { get; set; }
		public bool FromEditor { get; set; }
		public virtual void Visit(InOperator theOperator) {
			for(int i = 0; i < theOperator.Operands.Count; ++i) {
				SafeAccept(theOperator.Operands[i]);
			}
			SafeAccept(theOperator.LeftOperand);
		}
		public virtual void Visit(BinaryOperator theOperator) {
			SafeAccept(theOperator.LeftOperand);
			SafeAccept(theOperator.RightOperand);
		}
		string GetCurrentPath() {
			string res = "";
			foreach(string s in path) {
				res = s + '.' + res;
			}
			return res;
		}
		protected IBoundProperty GetCurrentProperty(string lastPropertyName) {
			string columnName = GetCurrentPath() + lastPropertyName;
			return GetColumnByMixedPath(columnName, FromEditor);
		}
		IBoundProperty SearchColumn(IEnumerable<IBoundProperty> columns, Func<IBoundProperty, string> nameSelector, ref string columnName) {
			string localPropertyName = columnName;
			var property = columns
				.Where(p => localPropertyName.StartsWith(nameSelector(p)))
				.OrderByDescending(c => nameSelector(c).Length)
				.FirstOrDefault();
			if(property != null) {
				columnName = columnName.Substring(nameSelector(property).Length);
			}
			return property;
		}
		private IBoundProperty GetColumnByMixedPath(string columnName, bool tryDisplayNames) {
			IBoundProperty property = null;
			var columns = Columns;
			while(columnName != null && columnName.Length > 0) {
				if(columns == null)
					return null;
				if(tryDisplayNames) {
					property = SearchColumn(columns, p => p.DisplayName, ref columnName)
							?? SearchColumn(columns, p => p.Name, ref columnName);
				} else {
					property = SearchColumn(columns, p => p.Name, ref columnName);
				}
				if(property == null)
					return null;
				if(columnName.Length > 0) {
					if(columnName[0] != '.')
						return null;
					columnName = columnName.Substring(1);
				}
				columns = property.HasChildren ? property.Children : null;
			}
			return property;
		}
		protected void SafeAccept(CriteriaOperator criteria) {
			if(!ReferenceEquals(criteria, null)) {
				criteria.Accept(this);
			}
		}
		public void Visit(UnaryOperator theOperator) {
			SafeAccept(theOperator.Operand);
		}
		public void Visit(BetweenOperator theOperator) {
			SafeAccept(theOperator.BeginExpression);
			SafeAccept(theOperator.EndExpression);
			SafeAccept(theOperator.TestExpression);
		}
		public void Visit(JoinOperand theOperand) {
			SafeAccept(theOperand.Condition);
		}
		public void Visit(FunctionOperator theOperator) {
			foreach(CriteriaOperator co in theOperator.Operands) {
				co.Accept(this);
			}
		}
		public void Visit(OperandValue theOperand) {
		}
		public void Visit(GroupOperator theOperator) {
			foreach(CriteriaOperator co in theOperator.Operands) {
				SafeAccept(co);
			}
		}
		public void Visit(AggregateOperand theOperand) {
			if(!ReferenceEquals(theOperand.CollectionProperty, null)) {
				string collectionPropertyName = theOperand.CollectionProperty.PropertyName;
				theOperand.CollectionProperty.Accept(this);
				path.Push(collectionPropertyName);
				SafeAccept(theOperand.AggregatedExpression);
				SafeAccept(theOperand.Condition);
				path.Pop();
				return;
			}
			SafeAccept(theOperand.AggregatedExpression);
			SafeAccept(theOperand.Condition);
		}
		public void Visit(OperandProperty theOperand) {
			if(!ReferenceEquals(theOperand, null)) {
				var column = GetCurrentProperty(theOperand.PropertyName);
				if(column == null)
					return;
				theOperand.PropertyName = FromEditor ? column.GetFullName() : column.GetFullDisplayName();
			}
		}
	}
}
