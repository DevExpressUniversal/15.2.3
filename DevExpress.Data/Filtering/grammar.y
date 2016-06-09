%{
// ATTENTION ATTENTION ATTENTION ATTENTION
// this .CS file is a tool generated file from grammar.y and lexer.l
// DO NOT CHANGE BY HAND!!!!
// YOU HAVE BEEN WARNED !!!!

namespace DevExpress.Data.Filtering.Helpers {
	using System;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	//using System.Runtime.Serialization;
	using DevExpress.Data.Filtering;
	using DevExpress.Data.Filtering.Exceptions;

	/// <summary>
	///    The C# Parser
	/// </summary>
	public class CriteriaParser {
		CriteriaOperator[] result;
		public CriteriaOperator[] Result { get { return result; } }
		List<OperandValue> resultParameters = new List<OperandValue>();
		public List<OperandValue> ResultParameters { get { return resultParameters; } }
%}

	/* YACC Declarations  Cheops grammar*/
	%token CONST
	%token AGG_EXISTS AGG_COUNT AGG_MIN AGG_MAX AGG_AVG AGG_SUM AGG_SINGLE
	%token PARAM
	%token COL
	%token '.'
	%token FN_ISNULL
	%token FUNCTION SORT_ASC SORT_DESC
	%token '[' ']'
	%token '(' ')'
	%left OR
	%left AND
	%right NOT
	%nonassoc IS NULL
	%left OP_EQ OP_NE OP_LIKE
	%left OP_GT OP_LT OP_GE OP_LE
	%nonassoc OP_IN OP_BETWEEN
	%left '|'
	%left '^'
	%left '&'
	%nonassoc '~'
	%left '-' '+'
	%left '*' '/' '%'
	%nonassoc NEG
	/* Grammar follows */
	%%
criteriaList:
	'\0'		{ result = new CriteriaOperator[0]; }
	| queryCollection '\0'	{ result = ((List<CriteriaOperator>)$1).ToArray(); }
	;

queryCollection:
	expOrSort			{ $$ = new List<CriteriaOperator>(new CriteriaOperator[] {(CriteriaOperator)$1}); }
	| queryCollection ';' expOrSort	{ $$ = $1; ((List<CriteriaOperator>)$$).Add((CriteriaOperator)$3); }
	| queryCollection ',' expOrSort	{ $$ = $1; ((List<CriteriaOperator>)$$).Add((CriteriaOperator)$3); }
	;
	
expOrSort:
	exp				{ $$ = $1; }
	| exp SORT_ASC	{ $$ = $1; }
	| exp SORT_DESC {
#if CF
					throw new NotSupportedException();
#else
					$$ = new FunctionOperator(DevExpress.Data.Helpers.ServerModeCore.OrderDescToken, (CriteriaOperator)$1);
#endif
					}
	;

type:
	COL				{ $$ = $1; }
	| type	'.'	COL	{
		OperandProperty prop1 = (OperandProperty)$1;
		OperandProperty prop3 = (OperandProperty)$3;
		$$ = new OperandProperty(prop1.PropertyName + '.' + prop3.PropertyName);
	}
	| type	'+'	COL	{
		OperandProperty prop1 = (OperandProperty)$1;
		OperandProperty prop3 = (OperandProperty)$3;
		$$ = new OperandProperty(prop1.PropertyName + '+' + prop3.PropertyName);
	}
	;

upcast:
	OP_LT type OP_GT COL	{
		OperandProperty prop2 = (OperandProperty)$2;
		OperandProperty prop4 = (OperandProperty)$4;
		$$ = new OperandProperty('<' + prop2.PropertyName + '>' + prop4.PropertyName);
	}
	;

column:
	COL			{ $$ = $1; }
	|	upcast	{ $$ = $1; }
	|	'^'		{ $$ = new OperandProperty("^"); }
	;

property:
	column		{ $$ = $1; }
	|  property '.' column	{
		OperandProperty prop1 = (OperandProperty)$1;
		OperandProperty prop3 = (OperandProperty)$3;
		prop1.PropertyName += '.' + prop3.PropertyName;
		$$ = prop1;
	}
	;

aggregate:
	property '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$3;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' exp ']' '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$6;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, (CriteriaOperator)$3, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' ']' '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$5;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' exp ']' { $$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, (CriteriaOperator)$3, Aggregate.Exists, null); }
	|  property '[' ']' { $$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, Aggregate.Exists, null); }
	|  topLevelAggregate
	;

topLevelAggregate: aggregateSuffix
	;

aggregateSuffix:
	AGG_COUNT				{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
	|  AGG_EXISTS			{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
	|  AGG_COUNT '(' ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
	|  AGG_EXISTS '(' ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
	|  AGG_AVG '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Avg, null); }
	|  AGG_SUM '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Sum, null); }
	|  AGG_SINGLE '(' ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)new OperandProperty("This"), Aggregate.Single, null); }
	|  AGG_SINGLE '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Single, null); }
	|  MinStart ')'			{ $$ = $1; }
	|  MaxStart ')'			{ $$ = $1; }
	;

MinStart:
	AGG_MIN '(' exp			{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Min, null); }
	;

MaxStart:
	AGG_MAX '(' exp			{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Max, null); }
	;

exp:
	CONST				{ $$ = $1; }
	| NULL				{ $$ = new ConstantValue(null); }
	| PARAM				{
						  string paramName = (string)$1;
						  if(string.IsNullOrEmpty(paramName)) {
						    OperandValue param = new OperandValue();
						    resultParameters.Add(param);
						    $$ = param;
						  } else {
						    bool paramNotFound = true;
						    foreach(OperandValue v in resultParameters) {
						      OperandParameter p = v as OperandParameter;
						      if(ReferenceEquals(p, null))
						        continue;
						      if(p.ParameterName != paramName)
						        continue;
						      paramNotFound = false;
						      resultParameters.Add(p);
						      $$ = p;
						      break;
						    }
						    if(paramNotFound) {
						      OperandParameter param = new OperandParameter(paramName);
						      resultParameters.Add(param);
						      $$ = param;
						    }
						  }
						}
	| property			{ $$ = $1; } 
	| aggregate			{ $$ = $1; } 
	| exp  '*'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Multiply ); }
	| exp  '/'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Divide ); }
	| exp  '+'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Plus ); }
	| exp  '-'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Minus ); }
	| exp  '%'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Modulo ); }
	| exp  '|'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseOr ); }
	| exp  '&'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseAnd ); }
	| exp  '^'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseXor ); }
	| '-'  exp %prec NEG	{
								$$ = new UnaryOperator( UnaryOperatorType.Minus, (CriteriaOperator)$2 );
								try {
									if($2 is OperandValue) {
										OperandValue operand = (OperandValue)$2;
										if(operand.Value is Int32) {
											operand.Value = -(Int32)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Int64) {
											operand.Value = -(Int64)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Double) {
											operand.Value = -(Double)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Single) {
											operand.Value = -(Single)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Decimal) {
											operand.Value = -(Decimal)operand.Value;
											$$ = operand;
											break;
										}  else if(operand.Value is Int16) {
											operand.Value = -(Int16)operand.Value;
											$$ = operand;
											break;
										}  else if(operand.Value is SByte) {
											operand.Value = -(SByte)operand.Value;
											$$ = operand;
											break;
										}
									}
								} catch {}
							}
	| '+'  exp %prec NEG	{ $$ = new UnaryOperator( UnaryOperatorType.Plus, (CriteriaOperator)$2 ); }
	| '~'  exp				{ $$ = new UnaryOperator( UnaryOperatorType.BitwiseNot, (CriteriaOperator)$2 ); }
	| exp OP_EQ exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Equal); }
	| exp OP_NE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.NotEqual); }
	| exp OP_GT exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Greater); }
	| exp OP_LT exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Less); }
	| exp OP_GE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.GreaterOrEqual); }
	| exp OP_LE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.LessOrEqual); }
	| exp OP_LIKE exp		{ $$ = LikeCustomFunction.Create( (CriteriaOperator)$1, (CriteriaOperator)$3); }
	| exp NOT OP_LIKE exp %prec OP_LIKE		{ $$ = !LikeCustomFunction.Create( (CriteriaOperator)$1, (CriteriaOperator)$4); }
	| NOT exp				{ $$ = new UnaryOperator(UnaryOperatorType.Not, (CriteriaOperator)$2); }
	| exp AND exp		{ $$ = GroupOperator.And((CriteriaOperator)$1, (CriteriaOperator)$3); }
	| exp OR exp		{ $$ = GroupOperator.Or((CriteriaOperator)$1, (CriteriaOperator)$3); }
	| '(' exp ')'			{ $$ = $2; }
	| exp IS NULL	{ $$ = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$1); }
	| exp IS NOT NULL %prec NULL	{ $$ = new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$1)); }
	| exp OP_IN argumentslist { $$ = new InOperator((CriteriaOperator)$1, (IEnumerable<CriteriaOperator>)$3); }
	| exp OP_BETWEEN '(' exp ',' exp ')'	{ $$ = new BetweenOperator((CriteriaOperator)$1, (CriteriaOperator)$4, (CriteriaOperator)$6); }
	| FN_ISNULL '(' exp ')'	{ $$ = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$3); }
	| FN_ISNULL '(' exp ',' exp ')'	{ $$ = new FunctionOperator(FunctionOperatorType.IsNull, (CriteriaOperator)$3, (CriteriaOperator)$5); }	
	| FUNCTION argumentslist 	{  FunctionOperator fo = new FunctionOperator((FunctionOperatorType)$1, (IEnumerable<CriteriaOperator>)$2); lexer.CheckFunctionArgumentsCount(fo); $$ = fo; }
	| COL argumentslist			{  FunctionOperator fo = new FunctionOperator(((OperandProperty)$1).PropertyName, (IEnumerable<CriteriaOperator>)$2); lexer.CheckFunctionArgumentsCount(fo); $$ = fo; }
	| '(' ')'	{ $$ = null; }
	| MinStart ',' exp ')'		{ $$ = new FunctionOperator(FunctionOperatorType.Min, ((AggregateOperand)$1).AggregatedExpression, (CriteriaOperator)$3); }
	| MaxStart ',' exp ')'		{ $$ = new FunctionOperator(FunctionOperatorType.Max, ((AggregateOperand)$1).AggregatedExpression, (CriteriaOperator)$3); }
	;

argumentslist:
	'(' commadelimitedlist ')'	{ $$ = $2; }
	| '(' ')'					{ $$ = new List<CriteriaOperator>(); }
	;

commadelimitedlist:
	exp					{
							List<CriteriaOperator> lst = new List<CriteriaOperator>();
							lst.Add((CriteriaOperator)$1);
							$$ = lst;
						}
	| commadelimitedlist ',' exp	{
							List<CriteriaOperator> lst = (List<CriteriaOperator>)$1;
							lst.Add((CriteriaOperator)$3);
							$$ = lst;
						}
	;
%%

	CriteriaLexer lexer;

	public void yyerror (string message) {
		yyerror(message, null);
	}

	public void yyerror (string message, string[] expected) {
		string buf = message;
		if ((expected != null) && (expected.Length  > 0)) {
			buf += message;
			buf += ", expecting\n";
			for (int n = 0; n < expected.Length; ++ n)
				buf += (" "+expected[n]);
			buf += "\n";
		}
		throw new CriteriaParserException(buf);
	}
	void Parse(String query) {
	    Parse(query, false);
	}
	void Parse(String query, bool allowSort) {
		StringReader sr = new System.IO.StringReader(query);
		lexer = new CriteriaLexer(sr);
		lexer.RecogniseSortings = allowSort;
		try {
			yyparse(lexer);
		} catch(CriteriaParserException e) {
			string malformedQuery = query;
			if(lexer.Line == 0) {
				try {
					malformedQuery = malformedQuery.Substring(0, lexer.Col) + FilteringExceptionsText.ErrorPointer + malformedQuery.Substring(lexer.Col);
				} catch { }
			}
			throw new CriteriaParserException(String.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.GrammarCatchAllErrorMessage, lexer.Line, lexer.Col, e.Message, malformedQuery), lexer.Line, lexer.Col);
		}
	}
	public static CriteriaOperator[] ParseList(string criteriaList, out OperandValue[] criteriaParametersList, bool allowSorting) {
		CriteriaParser parser = new CriteriaParser();
		parser.Parse(criteriaList, allowSorting);
		criteriaParametersList = parser.ResultParameters.ToArray();
		return parser.Result;
	}
	public static CriteriaOperator Parse(string stringCriteria, out OperandValue[] criteriaParametersList) {
		if(stringCriteria == null) {
			criteriaParametersList = null;
			return null;
		}
		CriteriaParser parser = new CriteriaParser();
		parser.Parse(stringCriteria);
		criteriaParametersList = parser.ResultParameters.ToArray();
		switch(parser.Result.Length) {
			case 0:
				return null;
			case 1:
				return parser.Result[0];
			default:
				throw new ArgumentException("single criterion expected", "stringCriteria");	//TODO message
		}
	}
}