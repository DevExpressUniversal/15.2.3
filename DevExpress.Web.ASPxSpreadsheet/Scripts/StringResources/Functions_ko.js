ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "절대 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 절대 값을 구하려는 실수입니다."
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "만기에 이자를 지급하는 유가 증권의 경과 이자를 반환합니다.",
		arguments: [
			{
				name: "issue",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 발행일입니다."
			},
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "rate",
				description: "은(는) 유가 증권의 연간 확정 금리입니다."
			},
			{
				name: "par",
				description: "은(는) 유가 증권의 액면가입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "ACOS",
		description: "0과 Pi 범위에서 라디안의 아크코사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 각도의 코사인 값으로 -1에서 1 사이 값이어야 합니다."
			}
		]
	},
	{
		name: "ACOSH",
		description: "역 하이퍼볼릭 코사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 1보다 크거나 같은 실수입니다."
			}
		]
	},
	{
		name: "ACOT",
		description: "0부터 Pi까지의 아크탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 각도의 코탄젠트 값입니다."
			}
		]
	},
	{
		name: "ACOTH",
		description: "역 하이퍼볼릭 코탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 각도의 하이퍼볼릭 코탄젠트 값입니다."
			}
		]
	},
	{
		name: "ADDRESS",
		description: "지정된 행/열 번호를 가지고 셀 주소를 나타내는 텍스트를 만듭니다.",
		arguments: [
			{
				name: "row_num",
				description: "은(는) 참조할 셀의 행 번호입니다. 즉, 행 1에 대한 Row_number는 1입니다."
			},
			{
				name: "column_num",
				description: "은(는) 참조할 셀의 열 번호입니다. 즉, 열 D에 대한 Column_number는 4입니다."
			},
			{
				name: "abs_num",
				description: "은(는) 참조 영역의 유형을 지정합니다. 절대 참조는 1, 절대 행/상대 열 참조는 2, 상대 행/절대 열 참조는 3, 상대 참조는 4입니다."
			},
			{
				name: "a1",
				description: "은(는) A1이나 R1C1 참조 스타일을 지정하는 논리값입니다. 즉, A1 스타일은 1이거나 TRUE이고 R1C1 스타일은 0이거나 FALSE입니다."
			},
			{
				name: "sheet_text",
				description: "은(는) 외부 참조로 사용될 워크시트 이름을 지정하는 텍스트입니다."
			}
		]
	},
	{
		name: "AND",
		description: "인수가 모두 TRUE이면 TRUE를 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 255개까지 지정할 수 있으며 논리값, 배열, 참조가 될 수 있습니다."
			},
			{
				name: "logical2",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 255개까지 지정할 수 있으며 논리값, 배열, 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "ARABIC",
		description: "로마 숫자를 아라비아 숫자로 변환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 변환할 로마 숫자입니다."
			}
		]
	},
	{
		name: "AREAS",
		description: "참조 영역 내의 영역 수를 구합니다. 영역은 연속되는 셀 범위나 하나의 셀이 될 수 있습니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 셀이나 셀 범위에 대한 참조 영역으로서 여러 개의 영역을 나타낼 수 있습니다."
			}
		]
	},
	{
		name: "ASIN",
		description: "-Pi/2부터 Pi/2까지의 아크사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 각도의 사인 값으로 -1에서 1 사이 값이어야 합니다."
			}
		]
	},
	{
		name: "ASINH",
		description: "역 하이퍼볼릭 사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 1보다 크거나 같은 실수입니다."
			}
		]
	},
	{
		name: "ATAN",
		description: "-Pi/2부터 Pi/2까지의 아크탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 각도의 탄젠트 값입니다."
			}
		]
	},
	{
		name: "ATAN2",
		description: "-Pi에서 Pi까지의 라디안에서 지정된 x, y 좌표의 아크탄젠트 값을 구합니다. 단 -Pi 값은 제외됩니다.",
		arguments: [
			{
				name: "x_num",
				description: "은(는) 각을 구하려는 지점의 x 좌표입니다."
			},
			{
				name: "y_num",
				description: "은(는) 각을 구하려는 지점의 y 좌표입니다."
			}
		]
	},
	{
		name: "ATANH",
		description: "역 하이퍼볼릭 탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) -1과 1 사이의 실수입니다."
			}
		]
	},
	{
		name: "AVEDEV",
		description: "데이터 요소의 절대 편차의 평균을 구합니다. 인수는 숫자, 이름, 배열, 숫자가 들어 있는 참조가 될 수 있습니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 절대 편차의 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 절대 편차의 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "AVERAGE",
		description: "인수들의 평균을 구합니다. 인수는 숫자나 이름, 배열, 숫자가 들어 있는 참조 등이 될 수 있습니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 평균을 구하고자 하는 값들로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 평균을 구하고자 하는 값들로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "인수들의 평균(산술 평균)을 구합니다. 인수에서 텍스트와 FALSE는 0으로 취급하며 TRUE는 1로 취급합니다. 인수는 숫자, 이름, 배열, 참조가 될 수 있습니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 평균을 구할 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 평균을 구할 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "주어진 조건에 따라 지정되는 셀의 평균(산술 평균)을 구합니다.",
		arguments: [
			{
				name: "range",
				description: "은(는) 계산할 셀의 범위입니다."
			},
			{
				name: "criteria",
				description: "은(는) 숫자, 식 또는 텍스트 형식의 조건으로 평균을 구할 셀을 정의합니다."
			},
			{
				name: "average_range",
				description: "은(는) 평균을 구하는 데 사용할 실제 셀입니다. 생략하면 범위 안에 있는 셀이 사용됩니다."
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "주어진 조건에 따라 지정되는 셀의 평균(산술 평균)을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "은(는) 평균을 구하는 데 사용할 실제 셀입니다."
			},
			{
				name: "criteria_range",
				description: "은(는) 특정 조건에 따라 계산할 셀의 범위입니다."
			},
			{
				name: "criteria",
				description: "은(는) 숫자, 식 또는 텍스트 형식의 조건으로 평균을 구할 셀을 정의합니다."
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "숫자를 텍스트(baht)로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환하려는 숫자입니다."
			}
		]
	},
	{
		name: "BASE",
		description: "숫자를 지정된 기수의 텍스트 표현으로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 숫자입니다."
			},
			{
				name: "radix",
				description: "은(는) 숫자를 변환할 기수입니다."
			},
			{
				name: "min_length",
				description: "은(는) 반환된 문자열의 최소 길이입니다. 생략하면 앞에 0이 추가되지 않습니다."
			}
		]
	},
	{
		name: "BESSELI",
		description: "수정된 Bessel 함수 In(x) 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값입니다."
			},
			{
				name: "n",
				description: "은(는) Bessel 함수의 차수입니다."
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Bessel 함수 Jn(x) 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값입니다."
			},
			{
				name: "n",
				description: "은(는) Bessel 함수의 차수입니다."
			}
		]
	},
	{
		name: "BESSELK",
		description: "수정된 Bessel 함수 Kn(x) 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값입니다."
			},
			{
				name: "n",
				description: "은(는) Bessel 함수의 차수입니다."
			}
		]
	},
	{
		name: "BESSELY",
		description: "Bessel 함수 Yn(x) 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값입니다."
			},
			{
				name: "n",
				description: "은(는) Bessel 함수의 차수입니다."
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "베타 확률 분포 함수 값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산하려는 값으로서 A와 B 사이의 값입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			},
			{
				name: "A",
				description: "은(는) 옵션이며 x 구간에서의 하한값입니다. 생략하면 A가 0이 됩니다."
			},
			{
				name: "B",
				description: "은(는) 옵션이며 x 구간에서의 상한값입니다. 생략하면 B가 1이 됩니다."
			}
		]
	},
	{
		name: "BETA.INV",
		description: "역 누적 베타 확률 밀도 함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 베타 분포를 따르는 확률값입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "A",
				description: "은(는) x가 취할 수 있는 최소값입니다. 생략하면 A가 0이 됩니다."
			},
			{
				name: "B",
				description: "은(는) x가 취할 수 있는 최대값입니다. 생략하면 B가 1이 됩니다."
			}
		]
	},
	{
		name: "BETADIST",
		description: "누적 베타 확률 밀도 함수 값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산하려는 값으로서 A와 B 사이의 값입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "A",
				description: "은(는) 옵션이며 x 구간에서의 하한값입니다. 생략하면 A가 0이 됩니다."
			},
			{
				name: "B",
				description: "은(는) 옵션이며 x 구간에서의 상한값입니다. 생략하면 B가 1이 됩니다."
			}
		]
	},
	{
		name: "BETAINV",
		description: "역 누적 베타 확률 밀도 함수 (BETADIST) 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 베타 분포를 따르는 확률값입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로서 0보다 큰 값이어야 합니다."
			},
			{
				name: "A",
				description: "은(는) x가 취할 수 있는 최소값입니다. 생략하면 A가 0이 됩니다."
			},
			{
				name: "B",
				description: "은(는) x가 취할 수 있는 최대값입니다. 생략하면 B가 1이 됩니다."
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "2진수를 10진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 2진수입니다."
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "2진수를 16진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 2진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "2진수를 8진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 2진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "개별항 이항 분포 확률을 구합니다.",
		arguments: [
			{
				name: "number_s",
				description: "은(는) trials만큼의 시행 중 성공할 횟수입니다."
			},
			{
				name: "trials",
				description: "은(는) 독립적 시행 횟수입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 각 시행에서의 성공 확률입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리 값입니다. 누적 분포 함수에는 TRUE를, 확률 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "이항 분포를 사용한 시행 결과의 확률을 반환합니다.",
		arguments: [
			{
				name: "trials",
				description: "은(는) 독립적 시행 횟수입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 각 시행에서의 성공 확률입니다."
			},
			{
				name: "number_s",
				description: "은(는) 성공한 시행 횟수입니다."
			},
			{
				name: "number_s2",
				description: "이 함수는 제공될 경우 성공한 시행 횟수가 number_s와 number_s2 사이에 있을 확률을 반환합니다."
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "누적 이항 분포가 기준치 이상이 되는 값 중 최소값을 구합니다.",
		arguments: [
			{
				name: "trials",
				description: "은(는) 베르누이 시행 횟수입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 각 시행의 성공 확률입니다. 값은 경계 값을 포함하며 0에서 1까지입니다."
			},
			{
				name: "alpha",
				description: "은(는) 기준치입니다. 값은 경계 값을 포함하며 0에서 1까지입니다."
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "개별항 이항 분포 확률을 구합니다.",
		arguments: [
			{
				name: "number_s",
				description: "은(는) trials만큼의 시행 중 성공할 횟수입니다."
			},
			{
				name: "trials",
				description: "은(는) 독립적 시행 횟수입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 각 시행에서의 성공 확률입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "BITAND",
		description: "두 수의 비트 'And' 값을 구합니다.",
		arguments: [
			{
				name: "number1",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			},
			{
				name: "number2",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "shift_amount 비트만큼 왼쪽으로 이동된 숫자를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			},
			{
				name: "shift_amount",
				description: "은(는) Number를 왼쪽으로 이동할 비트 수입니다."
			}
		]
	},
	{
		name: "BITOR",
		description: "두 수의 비트 'Or' 값을 구합니다.",
		arguments: [
			{
				name: "number1",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			},
			{
				name: "number2",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "shift_amount 비트만큼 오른쪽으로 이동된 숫자를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			},
			{
				name: "shift_amount",
				description: "은(는) Number를 오른쪽으로 이동할 비트 수입니다."
			}
		]
	},
	{
		name: "BITXOR",
		description: "두 수의 비트 '배타적 Or' 값을 구합니다.",
		arguments: [
			{
				name: "number1",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			},
			{
				name: "number2",
				description: "은(는) 구하려는 이진수의 십진수 표현입니다."
			}
		]
	},
	{
		name: "CEILING",
		description: "수를 significance의 배수가 되도록 절대 값을 올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 배수의 기준이 되는 수입니다."
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "수를 가장 가까운 정수 또는 가장 가까운 significance의 배수가 되도록 올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 올림하려는 수의 배수입니다."
			},
			{
				name: "mode",
				description: "이 함수에 0이 아닌 값을 지정하면 소수점 자리가 올림됩니다."
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "수를 significance의 배수가 되도록 절대 값을 올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 배수의 기준이 되는 수입니다."
			}
		]
	},
	{
		name: "CELL",
		description: "참조 범위에서 시트를 읽는 순서에 따라 첫째 셀의 서식이나 위치, 내용에 대한 정보를 제공합니다.",
		arguments: [
			{
				name: "info_type",
				description: "은(는) 원하는 셀 정보의 유형을 지정하는 텍스트 값입니다."
			},
			{
				name: "reference",
				description: "은(는) 정보를 구하려는 셀입니다."
			}
		]
	},
	{
		name: "CHAR",
		description: "시스템의 문자 세트에 대한 코드 번호에 해당하는 문자를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 원하는 문자를 지정하는 1에서 255 사이의 수입니다."
			}
		]
	},
	{
		name: "CHIDIST",
		description: "카이 제곱 분포의 우측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "CHIINV",
		description: "카이 제곱 분포의 역 우측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 카이 제곱 분포를 따르는 확률로, 값은 0에서 1까지입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "카이 제곱 분포의 좌측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은(는) 제외됩니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "카이 제곱 분포의 우측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "카이 제곱 분포의 역 좌측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 카이 제곱 분포를 따르는 확률로, 값은 0에서 1까지입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "카이 제곱 분포의 역 우측 검정 확률을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 카이 제곱 분포를 따르는 확률로, 값은 0에서 1까지입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 1에서 10^10까지의 자유도입니다. 단, 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "독립 검증 결과를 구합니다. 통계적이고 적절한 자유도에 대한 카이 제곱 분포값을 의미합니다.",
		arguments: [
			{
				name: "actual_range",
				description: "은(는) 기대값을 검증하기 위한 관측값이 있는 데이터 범위입니다."
			},
			{
				name: "expected_range",
				description: "은(는) 각 행과 열의 합을 곱한 값들의 전체 총계에 대한 비율이 있는 데이터 범위입니다."
			}
		]
	},
	{
		name: "CHITEST",
		description: "독립 검증 결과를 구합니다. 통계적이고 적절한 자유도에 대한 카이 제곱 분포값을 의미합니다.",
		arguments: [
			{
				name: "actual_range",
				description: "은(는) 기대값을 검증하기 위한 관측값이 있는 데이터 범위입니다."
			},
			{
				name: "expected_range",
				description: "은(는) 각 행과 열의 합을 곱한 값들의 전체 총계에 대한 비율이 있는 데이터 범위입니다."
			}
		]
	},
	{
		name: "CHOOSE",
		description: "인수 목록 중에서 하나를 고릅니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "은(는) 골라낼 인수의 위치를 지정합니다. Index_num은 1부터 254까지의 수이거나 1부터 254까지의 수에 대한 참조나 수식입니다."
			},
			{
				name: "value1",
				description: "은(는) 1부터 254까지의 수, 셀 참조, 정의된 이름, 수식, 함수, 텍스트 인수입니다."
			},
			{
				name: "value2",
				description: "은(는) 1부터 254까지의 수, 셀 참조, 정의된 이름, 수식, 함수, 텍스트 인수입니다."
			}
		]
	},
	{
		name: "CLEAN",
		description: "인쇄할 수 없는 모든 문자들을 텍스트에서 제거합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 인쇄할 수 없는 문자를 제거할 데이터입니다."
			}
		]
	},
	{
		name: "CODE",
		description: "텍스트 문자열의 첫째 문자를 나타내는 코드값을 시스템에서 사용되는 문자 집합에서 구합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 첫째 문자의 코드값을 구하려는 텍스트입니다."
			}
		]
	},
	{
		name: "COLUMN",
		description: "참조 영역의 열 번호를 구합니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 열 번호를 구하려는 셀 또는 셀 범위입니다. 생략하면 COLUMN 함수가 들어 있는 셀이 사용됩니다."
			}
		]
	},
	{
		name: "COLUMNS",
		description: "참조 영역이나 배열에 있는 열 수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 열 수를 구하려는 배열이나 배열식 또는 셀 범위의 주소입니다."
			}
		]
	},
	{
		name: "COMBIN",
		description: "주어진 개체 수로 만들 수 있는 조합의 수를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 개체 수입니다."
			},
			{
				name: "number_chosen",
				description: "은(는) 각 조합의 개체 수입니다."
			}
		]
	},
	{
		name: "COMBINA",
		description: "주어진 개체 수로 만들 수 있는 조합(반복 포함)의 수를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 개체 수입니다."
			},
			{
				name: "number_chosen",
				description: "은(는) 각 조합의 개체 수입니다."
			}
		]
	},
	{
		name: "COMPLEX",
		description: "실수부와 허수부의 계수를 복소수로 변환합니다.",
		arguments: [
			{
				name: "real_num",
				description: "은(는) 복소수의 실수부 계수입니다."
			},
			{
				name: "i_num",
				description: "은(는) 복소수의 허수부 계수입니다."
			},
			{
				name: "suffix",
				description: "은(는) 복소수의 허수 표시 접미사입니다."
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "여러 텍스트를 한 텍스트로 조인시킵니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "은(는) 하나로 조인할 텍스트들로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "text2",
				description: "은(는) 하나로 조인할 텍스트들로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "정규 분포를 사용한 모집단 평균의 신뢰 구간을 나타냅니다.",
		arguments: [
			{
				name: "alpha",
				description: "은(는) 신뢰도를 계산하는 데 쓰이는 유의 수준으로 0보다 크고 1보다 작아야 합니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 모집단의 표준 편차이며 알고 있다고 가정합니다. standard_dev는 0보다 큰 값이어야 합니다."
			},
			{
				name: "size",
				description: "은(는) 표본의 크기입니다."
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "정규 분포를 사용하는 모집단 평균의 신뢰 구간을 나타냅니다.",
		arguments: [
			{
				name: "alpha",
				description: "은(는) 신뢰도를 계산하는 데 쓰이는 유의 수준으로 0보다 크고 1보다 작아야 합니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 모집단의 표준 편차이며 알고 있다고 가정합니다. standard_dev는 0보다 큰 값이어야 합니다."
			},
			{
				name: "size",
				description: "은(는) 표본의 크기입니다."
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "스튜던트 t-분포를 사용하는 모집단 평균의 신뢰 구간을 나타냅니다.",
		arguments: [
			{
				name: "alpha",
				description: "은(는) 신뢰도를 계산하는 데 쓰이는 유의 수준으로 0보다 크고 1보다 작아야 합니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 모집단의 표준 편차이며 알고 있다고 가정합니다. standard_dev는 0보다 큰 값이어야 합니다."
			},
			{
				name: "size",
				description: "은(는) 표본의 크기입니다."
			}
		]
	},
	{
		name: "CONVERT",
		description: "다른 단위 체계의 숫자로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 from_units의 값입니다."
			},
			{
				name: "from_unit",
				description: "은(는) number의 단위입니다."
			},
			{
				name: "to_unit",
				description: "은(는) 결과 단위입니다."
			}
		]
	},
	{
		name: "CORREL",
		description: "두 데이터 집합 사이의 상관 계수를 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "COS",
		description: "각도의 코사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "COSH",
		description: "하이퍼볼릭 코사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 실수입니다."
			}
		]
	},
	{
		name: "COT",
		description: "각도의 코탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "COTH",
		description: "하이퍼볼릭 코탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "COUNT",
		description: "범위에서 숫자가 포함된 셀의 개수를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 최대 255개의 인수로서 다양한 데이터 유형을 포함하거나 참조할 수 있습니다. 이 가운데서 숫자만을 셉니다."
			},
			{
				name: "value2",
				description: "은(는) 최대 255개의 인수로서 다양한 데이터 유형을 포함하거나 참조할 수 있습니다. 이 가운데서 숫자만을 셉니다."
			}
		]
	},
	{
		name: "COUNTA",
		description: "범위에서 비어 있지 않은 셀의 개수를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 최대 255개의 인수로서 개수를 구할 값과 셀입니다. 값에는 모든 정보가 포함될 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 최대 255개의 인수로서 개수를 구할 값과 셀입니다. 값에는 모든 정보가 포함될 수 있습니다."
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "범위에서 비어 있는 셀의 개수를 구합니다.",
		arguments: [
			{
				name: "range",
				description: "은(는) 빈 셀의 개수를 구하려는 셀 범위입니다."
			}
		]
	},
	{
		name: "COUNTIF",
		description: "지정한 범위 내에서 조건에 맞는 셀의 개수를 구합니다.",
		arguments: [
			{
				name: "range",
				description: "은(는) 조건에 맞는 셀의 수를 구하려는 셀 범위입니다."
			},
			{
				name: "criteria",
				description: "은(는) 숫자, 식, 텍스트 형태의 조건입니다."
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "범위 내에서 주어진 조건에 맞는 셀의 개수를 셉니다.",
		arguments: [
			{
				name: "criteria_range",
				description: "은(는) 조건을 검사할 셀 범위입니다."
			},
			{
				name: "criteria",
				description: "은(는) 숫자, 식, 텍스트 형식으로 된 조건으로 개수를 계산할 셀을 정의합니다."
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "이자 지급 기간의 시작일부터 결산일까지의 날짜 수를 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "frequency",
				description: "은(는) 연간 이자 지급 횟수입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "COUPNCD",
		description: "결산일 다음 첫 번째 이자 지급일을 나타내는 숫자를 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "frequency",
				description: "은(는) 연간 이자 지급 횟수입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "COUPNUM",
		description: "결산일과 만기일 사이의 이자 지급 횟수를 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "frequency",
				description: "은(는) 연간 이자 지급 횟수입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "COUPPCD",
		description: "결산일 바로 전 이자 지급일을 나타내는 숫자를 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "frequency",
				description: "은(는) 연간 이자 지급 횟수입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "COVAR",
		description: "두 데이터 집합 사이의 공 분산을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "두 데이터 집합 사이의 모집단 공 분산을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "두 데이터 집합 사이의 표본 집단 공 분산을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 값들의 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "누적 이항 분포가 기준치 이상이 되는 값 중 최소값을 구합니다.",
		arguments: [
			{
				name: "trials",
				description: "은(는) 베르누이 시행 횟수입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 각 시행의 성공 확률입니다. 값은 경계 값을 포함하며 0에서 1까지입니다."
			},
			{
				name: "alpha",
				description: "은(는) 기준치입니다. 값은 경계 값을 포함하며 0에서 1까지입니다."
			}
		]
	},
	{
		name: "CSC",
		description: "각도의 코시컨트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "CSCH",
		description: "각도의 하이퍼볼릭 코시컨트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "주어진 기간 중에 납입하는 대출금 이자의 누계액을 반환합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 이자율입니다."
			},
			{
				name: "nper",
				description: "은(는) 총 불입 횟수입니다."
			},
			{
				name: "pv",
				description: "은(는) 현재 가치입니다."
			},
			{
				name: "start_period",
				description: "은(는) 계산 기간의 최초 불입 회차입니다."
			},
			{
				name: "end_period",
				description: "은(는) 계산 기간의 최종 불입 회차입니다."
			},
			{
				name: "type",
				description: "은(는) 불입 시점입니다."
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "주어진 기간 중에 납입하는 대출금 원금의 누계액을 반환합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 이자율입니다."
			},
			{
				name: "nper",
				description: "은(는) 총 불입 횟수입니다."
			},
			{
				name: "pv",
				description: "은(는) 현재 가치입니다."
			},
			{
				name: "start_period",
				description: "은(는) 계산 기간의 최초 불입 회차입니다."
			},
			{
				name: "end_period",
				description: "은(는) 계산 기간의 최종 불입 회차입니다."
			},
			{
				name: "type",
				description: "은(는) 불입 시점입니다."
			}
		]
	},
	{
		name: "DATE",
		description: "Spreadsheet의 날짜-시간 코드에서 날짜를 나타내는 수를 구합니다.",
		arguments: [
			{
				name: "year",
				description: "은(는) Windows용 Spreadsheet에서는 1900부터 9999까지, Macintosh용 Spreadsheet에서는 1904부터 9999까지의 연도를 나타내는 숫자입니다."
			},
			{
				name: "month",
				description: "은(는) 1부터 12까지의 달을 나타내는 숫자입니다."
			},
			{
				name: "day",
				description: "은(는) 1부터 31까지의 날짜를 나타내는 숫자입니다."
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "Spreadsheet의 날짜-시간 코드에서 텍스트 형태의 날짜를 숫자 값으로 변환합니다.",
		arguments: [
			{
				name: "date_text",
				description: "은(는) Spreadsheet 날짜 형식으로 1990/1/1(Windows) 또는 1904/1/1(Macintosh)부터 9999/12/31까지의 날짜를 표시한 텍스트입니다."
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "지정한 조건에 맞는 데이터베이스나 목록에서 열의 평균을 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터의 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열의 위치를 나타내는 숫자나 따옴표로 묶인 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 지정한 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DAY",
		description: "주어진 달의 날짜를 1에서 31 사이의 숫자로 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수입니다."
			}
		]
	},
	{
		name: "DAYS",
		description: "두 날짜 사이의 일 수를 반환합니다.",
		arguments: [
			{
				name: "end_date",
				description: "start_date와 end_date는 날짜 수를 계산하고자 하는 시작 날짜와 끝 날짜입니다."
			},
			{
				name: "start_date",
				description: "start_date와 end_date는 날짜 수를 계산하고자 하는 시작 날짜와 끝 날짜입니다."
			}
		]
	},
	{
		name: "DAYS360",
		description: "1년을 360일(30일 기준의 12개월)로 하여, 두 날짜 사이의 날짜 수를 계산합니다.",
		arguments: [
			{
				name: "start_date",
				description: "start_date와 end_date는 날짜 수를 계산하고자 하는 시작 날짜와 끝 날짜입니다."
			},
			{
				name: "end_date",
				description: "start_date와 end_date는 날짜 수를 계산하고자 하는 시작 날짜와 끝 날짜입니다."
			},
			{
				name: "method",
				description: "은(는) 계산 방법을 지정하는 논리값입니다. FALSE로 설정하거나 생략하면 U.S.(NASD)식을 사용하며, TRUE로 설정하면 유럽식을 사용합니다."
			}
		]
	},
	{
		name: "DB",
		description: "정율법을 사용하여 지정한 기간 동안 자산의 감가 상각을 구합니다.",
		arguments: [
			{
				name: "cost",
				description: "은(는) 자산의 초기 취득가액입니다."
			},
			{
				name: "salvage",
				description: "은(는) 감가 상각 종료 시 회수되는 값입니다."
			},
			{
				name: "life",
				description: "은(는) 자산이 감가 상각되는 기간 수(자산의 수명 년수)입니다."
			},
			{
				name: "period",
				description: "은(는) 감가 상각을 계산하려는 기간으로 수명 년수와 같은 단위여야 합니다."
			},
			{
				name: "month",
				description: "은(는) 첫 해의 개월 수입니다. 생략된 경우는 12로 가정합니다."
			}
		]
	},
	{
		name: "DCOUNT",
		description: "지정한 조건에 맞는 데이터베이스의 필드에서 숫자를 포함한 셀의 수를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열의 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "조건에 맞는 데이터베이스의 필드에서 비어 있지 않은 셀의 수를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DDB",
		description: "지정한 기간 동안 이중 체감법이나 사용자가 정하는 방법으로 자산의 감가 상각액을 구합니다.",
		arguments: [
			{
				name: "cost",
				description: "은(는) 자산의 초기 취득가액입니다."
			},
			{
				name: "salvage",
				description: "은(는) 감가 상각 종료 시 회수되는 값입니다."
			},
			{
				name: "life",
				description: "은(는) 자산이 감가 상각되는 기간 수(자산의 수명 년수)입니다."
			},
			{
				name: "period",
				description: "은(는) 감가 상각을 계산하려는 기간으로 life와 같은 단위여야 합니다."
			},
			{
				name: "factor",
				description: "은(는) 잔액이 감소되는 비율로, 생략하면 이중 체감법에 따라 2로 간주됩니다."
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "10진수를 2진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 10진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "10진수를 16진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 10진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "10진수를 8진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 10진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "DECIMAL",
		description: "지정된 기수의 텍스트 표현을 십진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 숫자입니다."
			},
			{
				name: "radix",
				description: "은(는) 변환하는 숫자의 기수입니다."
			}
		]
	},
	{
		name: "DEGREES",
		description: "라디안 형태의 각도를 도 단위로 바꿉니다.",
		arguments: [
			{
				name: "angle",
				description: "은(는) 변환하려는 라디안 형태의 각도입니다."
			}
		]
	},
	{
		name: "DELTA",
		description: "두 값이 같은지 여부를 검사합니다.",
		arguments: [
			{
				name: "number1",
				description: "은(는) 첫째 수입니다."
			},
			{
				name: "number2",
				description: "은(는) 둘째 수입니다."
			}
		]
	},
	{
		name: "DEVSQ",
		description: "표본 평균으로부터 편차의 제곱의 합을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 편차 제곱의 합을 구하려는 인수이거나 배열 또는 참조 영역으로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 편차 제곱의 합을 구하려는 인수이거나 배열 또는 참조 영역으로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "DGET",
		description: "데이터베이스에서 찾을 조건에 맞는 레코드가 하나인 경우 그 레코드를 추출합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DISC",
		description: "유가 증권의 할인율을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "pr",
				description: "은(는) 액면가 $100당 유가 증권 가격입니다."
			},
			{
				name: "redemption",
				description: "은(는) 액면가 $100당 유가 증권의 상환액입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "DMAX",
		description: "지정한 조건에 맞는 데이터베이스의 필드 값 중 가장 큰 수를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 따옴표로 묶인 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 지정한 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DMIN",
		description: "지정한 조건에 맞는 데이터베이스의 필드 값 중 가장 작은 수를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 따옴표로 묶인 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 지정한 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "분수로 표시된 금액을 소수로 표시된 금액으로 변환합니다.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "은(는) 분수로 표시되는 수입니다."
			},
			{
				name: "fraction",
				description: "은(는) 분수의 분모로 사용되는 정수입니다."
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "소수로 표시된 금액을 분수로 표시된 금액으로 변환합니다.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "은(는) 소수입니다."
			},
			{
				name: "fraction",
				description: "은(는) 분수의 분모로 사용되는 정수입니다."
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "지정한 조건에 맞는 데이터베이스에서 필드 값들의 곱을 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DSTDEV",
		description: "데이터베이스 필드 값들로부터 표본 집단의 표준 편차를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "데이터베이스 필드 값들로부터 모집단의 표준 편차를 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DSUM",
		description: "지정한 조건에 맞는 데이터베이스에서 필드 값들의 합을 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DVAR",
		description: "데이터베이스 필드 값들로부터 표본 집단의 분산을 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "DVARP",
		description: "데이터베이스 필드 값들로부터 모집단의 분산을 구합니다.",
		arguments: [
			{
				name: "database",
				description: "은(는) 데이터베이스나 목록으로 지정할 셀 범위입니다. 데이터베이스는 관련 데이터 목록입니다."
			},
			{
				name: "field",
				description: "은(는) 목록에서 열 위치를 나타내는 숫자나 열 레이블입니다."
			},
			{
				name: "criteria",
				description: "은(는) 찾을 조건이 있는 셀 범위입니다. 이 범위는 열 레이블과 조건 레이블 아래의 셀을 포함합니다."
			}
		]
	},
	{
		name: "EDATE",
		description: "지정한 날짜 전이나 후의 개월 수를 나타내는 날짜의 일련 번호를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "months",
				description: "은(는) start_date 전이나 후의 개월 수입니다."
			}
		]
	},
	{
		name: "EFFECT",
		description: "연간 실질 이자율을 반환합니다.",
		arguments: [
			{
				name: "nominal_rate",
				description: "은(는) 연간 명목 이자율입니다."
			},
			{
				name: "npery",
				description: "은(는) 연간 복리 계산 횟수입니다."
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "URL로 인코딩된 문자열을 반환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) URL로 인코딩된 문자열입니다."
			}
		]
	},
	{
		name: "EOMONTH",
		description: "지정된 달 수 이전이나 이후 달의 마지막 날의 날짜 일련 번호를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "months",
				description: "은(는) start_date 전이나 후의 개월 수입니다."
			}
		]
	},
	{
		name: "ERF",
		description: "오차 함수를 구합니다.",
		arguments: [
			{
				name: "lower_limit",
				description: "은(는) ERF 적분의 하한값입니다."
			},
			{
				name: "upper_limit",
				description: "은(는) ERF 적분의 상한값입니다."
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "오차 함수를 구합니다.",
		arguments: [
			{
				name: "X",
				description: "은(는) ERF.PRECISE 적분의 하한값입니다."
			}
		]
	},
	{
		name: "ERFC",
		description: "ERF 함수의 여값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) ERF 적분의 하한값입니다."
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "ERF 함수의 여값을 반환합니다.",
		arguments: [
			{
				name: "X",
				description: "은(는) ER.PRECISEF 적분의 하한값입니다."
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "오류 유형에 해당하는 번호를 구합니다.",
		arguments: [
			{
				name: "error_val",
				description: "은(는) 해당 번호를 찾으려는 오류 값입니다. 오류 값이 들어 있는 셀 참조가 될 수도 있고 실제 오류 값이 될 수도 있습니다."
			}
		]
	},
	{
		name: "EVEN",
		description: "가장 가까운 짝수인 정수로 양수는 올림하고 음수는 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림 또는 내림할 값입니다."
			}
		]
	},
	{
		name: "EXACT",
		description: "두 텍스트 값이 같은지 검사하여 같으면 TRUE를 돌려주고, 다르면 FALSE를 돌려줍니다. 대/소문자를 구분합니다.",
		arguments: [
			{
				name: "text1",
				description: "은(는) 첫째 텍스트입니다."
			},
			{
				name: "text2",
				description: "은(는) 둘째 텍스트입니다."
			}
		]
	},
	{
		name: "EXP",
		description: "number를 지수로 한 e의 누승을 계산합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 밑 e에 적용할 지수입니다. 상수 e는 자연 로그값의 밑인 2.71828182845904와 같습니다."
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "지수 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값으로 0 또는 양수이어야 합니다."
			},
			{
				name: "lambda",
				description: "은(는) 매개 변수 값으로 양수여야 합니다."
			},
			{
				name: "cumulative",
				description: "은(는) 지수 함수의 형태를 나타내는 논리값입니다. 누적 분포 함수는 TRUE를, 확률 밀도 함수는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "지수 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수를 계산할 값으로 0 또는 양수이어야 합니다."
			},
			{
				name: "lambda",
				description: "은(는) 매개 변수 값으로 양수여야 합니다."
			},
			{
				name: "cumulative",
				description: "은(는) 지수 함수의 형태를 나타내는 논리값입니다. 누적 분포 함수는 TRUE를, 확률 밀도 함수는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "F.DIST",
		description: "두 데이터 집합에 대해 좌측 F 확률 분포값(분포도)을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포값을 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이거나 생략하면 확률 질량 함수를 구합니다."
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "두 데이터 집합에 대해 우측 F 확률 분포값(분포도)을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포값을 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "F.INV",
		description: "좌측 F 확률 분포의 역함수 값을 구합니다. p = F.DIST(x,...)이면 F.INV(p,...)=x입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) F 누적 분포를 따르는 확률입니다. 값은 경계값을 포함하며 0에서 1까지입니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "우측 F 확률 분포의 역함수 값을 구합니다. p=F.DIST.RT(x,...)이면 F.INV.RT(p,...)=x입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) F 누적 분포를 따르는 확률입니다. 값은 경계값을 포함하며 0에서 1까지입니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은 제외됩니다."
			}
		]
	},
	{
		name: "F.TEST",
		description: "array1과 array2의 분산이 크게 차이가 나지 않는 경우 양측 확률인 F-검정 결과를 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 데이터 배열 또는 범위이며 숫자 또는 이름, 배열 또는 숫자가 들어 있는 참조가 될 수 있습니다. 공백은 무시됩니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 데이터 배열 또는 범위이며 숫자 또는 이름, 배열 또는 숫자가 들어 있는 참조가 될 수 있습니다. 공백은 무시됩니다."
			}
		]
	},
	{
		name: "FACT",
		description: "number의 계승값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 계승값을 구하려는 수로서 0 또는 양수여야 합니다."
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "숫자의 이중 계승값을 반환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 반환할 이중 계승값에 대한 값입니다."
			}
		]
	},
	{
		name: "FALSE",
		description: "논리값 FALSE를 돌려줍니다.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "두 데이터 집합에 대해 (우측 검정) F 확률 분포값(분포도)을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포값을 구하려는 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "지정한 텍스트를 다른 텍스트 내에서 찾아 해당 문자의 시작 위치를 나타냅니다. 대/소문자를 구분합니다.",
		arguments: [
			{
				name: "find_text",
				description: "은(는) 찾으려는 텍스트입니다. Within_text의 첫 문자가 일치하는 경우를 찾으려면 큰따옴표를 사용하십시오. 와일드카드 문자는 사용할 수 없습니다."
			},
			{
				name: "within_text",
				description: "은(는) 찾으려는 텍스트가 포함된 텍스트입니다."
			},
			{
				name: "start_num",
				description: "은(는) 찾기 시작할 문자의 위치입니다. within_text의 첫 문자는 1이며, 생략하면 start_num을 1로 지정합니다."
			}
		]
	},
	{
		name: "FINV",
		description: "(우측 검정) F 확률 분포의 역함수 값을 구합니다. p=FDIST(x,...)이면 FINV(p,...)=x입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) F 누적 분포를 따르는 확률입니다. 값은 경계값을 포함하며 0에서 1까지입니다."
			},
			{
				name: "deg_freedom1",
				description: "은(는) 분자의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			},
			{
				name: "deg_freedom2",
				description: "은(는) 분모의 자유도(1에서 10^10까지)입니다. 10^10은(는) 제외됩니다."
			}
		]
	},
	{
		name: "FISHER",
		description: "Fisher 변환 값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 변환하려는 수입니다. 범위는 -1에서 1까지이며 -1과 1은 제외됩니다."
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Fisher 변환의 역변환 값을 구합니다. y=FISHER(x)이면 FISHERINV(y)=x입니다.",
		arguments: [
			{
				name: "y",
				description: "은(는) 역변환 값을 구하려는 값입니다."
			}
		]
	},
	{
		name: "FIXED",
		description: "수를 고정 소수점 형식의 텍스트로 바꿉니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 반올림하여 텍스트로 바꾸려는 수입니다."
			},
			{
				name: "decimals",
				description: "은(는) 소수점 이하의 자릿수입니다. 생략하면 decimals은 2가 됩니다."
			},
			{
				name: "no_commas",
				description: "은(는) 수에 쉼표를 표시할지 지정하는 논리값입니다. 쉼표가 없으면 TRUE이고 쉼표가 표시되면 FALSE입니다."
			}
		]
	},
	{
		name: "FLOOR",
		description: "수를 significance의 배수가 되도록 절대 값을 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 내림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 올림하려는 수의 배수입니다."
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "수를 가장 가까운 정수 또는 significance의 가장 가까운 배수가 되도록 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 내림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 내림하려는 수의 배수입니다."
			},
			{
				name: "mode",
				description: "이 함수에 0이 아닌 값을 지정하면 소수점 자리가 내림됩니다."
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "수를 significance의 배수가 되도록 절대 값을 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 내림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 내림하려는 수의 배수입니다."
			}
		]
	},
	{
		name: "FORECAST",
		description: "기존 값에 의거한 선형 추세에 따라 예측값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) y 값을 예측하려는 지점의 x 값입니다. 반드시 수치 데이터여야 합니다."
			},
			{
				name: "known_y's",
				description: "은(는) 종속 데이터 배열 또는 범위입니다."
			},
			{
				name: "known_x's",
				description: "은(는) 독립 데이터 배열 또는 범위입니다. known_x's의 분산은 0이 되면 안됩니다."
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "수식을 문자열로 반환합니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 수식에 대한 참조입니다."
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "도수 분포를 세로 배열의 형태로 구합니다.",
		arguments: [
			{
				name: "data_array",
				description: "은(는) 빈도수를 계산하려는 값이 있는 셀 주소 또는 배열입니다."
			},
			{
				name: "bins_array",
				description: "은(는) data_array를 분류하는 데 필요한 구간값들이 있는 셀 주소 또는 배열입니다."
			}
		]
	},
	{
		name: "FTEST",
		description: "array1과 array2의 분산이 크게 차이가 나지 않는 경우 양측 확률인 F-검정 결과를 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫째 데이터 배열 또는 범위이며 숫자 또는 이름, 배열 또는 숫자가 들어 있는 참조가 될 수 있습니다. 공백은 무시됩니다."
			},
			{
				name: "array2",
				description: "은(는) 둘째 데이터 배열 또는 범위이며 숫자 또는 이름, 배열 또는 숫자가 들어 있는 참조가 될 수 있습니다. 공백은 무시됩니다."
			}
		]
	},
	{
		name: "FV",
		description: "주기적이고 고정적인 지급액과 고정적인 이율에 의거한 투자의 미래 가치를 산출합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다. 예를 들어, 연 이율 6%에 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "nper",
				description: "은(는) 투자의 총 지급 기간 수입니다."
			},
			{
				name: "pmt",
				description: "은(는) 각 기간마다의 지급액입니다. 이것은 투자 기간 동안 변할 수 없습니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치 또는 총합계입니다. 생략하면 Pv는 0입니다."
			},
			{
				name: "type",
				description: "은(는) 지급 시기를 나타내며 1은 투자 주기 초를, 0 또는 생략 시에는 투자 주기 말을 의미합니다."
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "초기 원금에 일련의 복리 이율을 적용했을 때의 예상 금액을 반환합니다.",
		arguments: [
			{
				name: "principal",
				description: "은(는) 현재의 가치입니다."
			},
			{
				name: "schedule",
				description: "은(는) 적용할 이율로 구성된 배열입니다."
			}
		]
	},
	{
		name: "GAMMA",
		description: "감마 함수 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 감마를 구할 값입니다."
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "감마 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포값을 구하려는 위치에서의 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로 양수입니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로 양수입니다. 1이면 표준 감마 분포값이 산출됩니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이거나 생략하면 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "감마 누적 분포의 역함수를 구합니다. p = GAMMA.DIST(x,...)이면 GAMMA.INV(p,...) = x입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 감마 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 양수입니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수입니다. 1이면 표준 감마 분포의 역함수가 산출됩니다."
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "감마 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포값을 구하려는 위치에서의 값으로 0 또는 양수여야 합니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로 양수입니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로 양수입니다. 1이면 표준 감마 분포값이 산출됩니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이거나 생략하면 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "감마 누적 분포의 역함수를 구합니다. p = GAMMADIST(x,...)이면 GAMMAINV(p,...) = x입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 감마 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로서 양수입니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수입니다. 1이면 표준 감마 분포의 역함수가 산출됩니다."
			}
		]
	},
	{
		name: "GAMMALN",
		description: "감마 함수의 자연 로그값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) GAMMALN을 구하려는 양수의 값입니다."
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "감마 함수의 자연 로그값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) GAMMALN.PRECISE를 구하려는 양수의 값입니다."
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "두 개 이상의 정수의 최대 공약수를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 계산할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 계산할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "양수 데이터 범위나 배열의 기하 평균을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 기하 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 기하 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "GESTEP",
		description: "숫자가 임계값보다 큰지 여부를 검사합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) step과 비교할 값입니다."
			},
			{
				name: "step",
				description: "은(는) 임계값입니다."
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "피벗 테이블 보고서 내에 저장된 데이터를 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "은(는) 데이터를 추출하려는 데이터 필드의 이름입니다."
			},
			{
				name: "pivot_table",
				description: "은(는) 셀에 대한 참조이거나, 검색하려는 데이터가 포함된 피벗 테이블 보고서 셀의 범위입니다."
			},
			{
				name: "field",
				description: "은(는) 참조할 필드입니다."
			},
			{
				name: "item",
				description: "은(는) 참조할 필드 개체입니다."
			}
		]
	},
	{
		name: "GROWTH",
		description: "지정한 값들의 지수 추세를 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) 양수들의 집합이며 y = b*m^x 식에서 이미 알고 있는 y 값의 집합입니다."
			},
			{
				name: "known_x's",
				description: "은(는) 옵션이며 y = b*m^x 식에서 Known_y와 크기가 일치하는 이미 알고 있는 x 값의 집합입니다."
			},
			{
				name: "new_x's",
				description: "은(는) y 값의 추세를 계산하고자 하는 새로운 x 값의 집합입니다."
			},
			{
				name: "const",
				description: "은(는) TRUE로 설정하면 상수 b를 정상적으로 계산하며 FALSE로 설정하거나 생략하면 b를 1로 지정하는 논리값입니다."
			}
		]
	},
	{
		name: "HARMEAN",
		description: "양수 데이터의 조화 평균을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 조화 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 조화 평균을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "16진수를 2진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 16진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "16진수를 10진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 16진수입니다."
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "16진수를 8진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 16진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "배열의 첫 행에서 값을 검색하여, 지정한 행의 같은 열에서 데이터를 추출합니다.",
		arguments: [
			{
				name: "lookup_value",
				description: "은(는) 표의 첫 행에서 찾으려는 값입니다. 값이나 셀 주소 또는 텍스트일 수 있습니다."
			},
			{
				name: "table_array",
				description: "은(는) 데이터를 검색하고 추출하려는 표입니다. table_array는 범위에 대한 참조값이나 범위 이름이 될 수 있습니다."
			},
			{
				name: "row_index_num",
				description: "은(는) table_array 내의 행 번호로, 값을 추출할 행을 지정합니다. 표의 첫 행 값은 행 1입니다."
			},
			{
				name: "range_lookup",
				description: "은(는) 논리값으로서 비슷하게 일치하는 것을 찾으면 TRUE이고 정확하게 일치하는 것을 찾으면 FALSE입니다."
			}
		]
	},
	{
		name: "HOUR",
		description: "시간을 0(오전 12:00)부터 23(오후 11:00)까지의 수로 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수이거나 16:48:00 또는 오후 4:48:00 같은 시간 형식의 텍스트입니다."
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "하드 드라이브, 네트워크 서버, 인터넷 등에 저장된 문서로 이동할 바로 가기 키를 만듭니다.",
		arguments: [
			{
				name: "Link_location",
				description: "은(는) 하드 드라이브 위치, UNC 주소, URL 경로, 열리는 파일의 이름과 전체 경로입니다."
			},
			{
				name: "friendly_name",
				description: "은(는) 셀에 표시하려는 숫자나 텍스트입니다. 생략하면 셀에 link_location 텍스트가 표시됩니다."
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "초기하 분포값을 구합니다.",
		arguments: [
			{
				name: "sample_s",
				description: "은(는) 표본에서의 성공 횟수입니다."
			},
			{
				name: "number_sample",
				description: "은(는) 표본의 크기입니다."
			},
			{
				name: "population_s",
				description: "은(는) 모집단에서의 성공 횟수입니다."
			},
			{
				name: "number_pop",
				description: "은(는) 모집단의 크기입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "초기하 분포값을 구합니다.",
		arguments: [
			{
				name: "sample_s",
				description: "은(는) 표본에서의 성공 횟수입니다."
			},
			{
				name: "number_sample",
				description: "은(는) 표본의 크기입니다."
			},
			{
				name: "population_s",
				description: "은(는) 모집단에서의 성공 횟수입니다."
			},
			{
				name: "number_pop",
				description: "은(는) 모집단의 크기입니다."
			}
		]
	},
	{
		name: "IF",
		description: "논리 검사를 수행하여 TRUE나 FALSE에 해당하는 값을 반환합니다.",
		arguments: [
			{
				name: "logical_test",
				description: "은(는) TRUE나 FALSE로 판정될 값이나 식입니다."
			},
			{
				name: "value_if_true",
				description: "은(는) logical_test가 TRUE일 때 돌려주는 값입니다. 생략하면 TRUE를 반환합니다. IF 함수를 일곱 번 중첩해서 쓸 수 있습니다."
			},
			{
				name: "value_if_false",
				description: "은(는) logical_test가 FALSE일 때 돌려주는 값입니다. 생략하면 FALSE를 반환합니다."
			}
		]
	},
	{
		name: "IFERROR",
		description: "식이나 식 자체의 값이 오류인 경우 value_if_error를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 값, 식 또는 참조입니다."
			},
			{
				name: "value_if_error",
				description: "은(는) 값, 식 또는 참조입니다."
			}
		]
	},
	{
		name: "IFNA",
		description: "식 결과가 #N/A이면 지정한 값을 반환하고, 그렇지 않으면 식 결과를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 임의의 값이나 식 또는 참조입니다."
			},
			{
				name: "value_if_na",
				description: "은(는) 임의의 값이나 식 또는 참조입니다."
			}
		]
	},
	{
		name: "IMABS",
		description: "복소수의 절대값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 절대값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "복소수의 허수부 계수를 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 허수부 계수를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "각도가 라디안으로 표시되는 인수 q를 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 인수 q를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "복소수의 켤레 복소수를 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 켤레 복소수를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMCOS",
		description: "복소수의 코사인 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 코사인 값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMCOSH",
		description: "복소수의 하이퍼볼릭 코사인 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 하이퍼볼릭 코사인 값을 구하려는 복소수입니다."
			}
		]
	},
	{
		name: "IMCOT",
		description: "복소수의 코탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 코탄젠트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMCSC",
		description: "복소수의 코시컨트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 코시컨트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMCSCH",
		description: "복소수의 하이퍼볼릭 코시컨트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 하이퍼볼릭 코시컨트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMDIV",
		description: "두 복소수의 나눗셈 몫을 반환합니다.",
		arguments: [
			{
				name: "inumber1",
				description: "은(는) 분자 또는 피제수인 복소수입니다."
			},
			{
				name: "inumber2",
				description: "은(는) 분모 또는 제수인 복소수입니다."
			}
		]
	},
	{
		name: "IMEXP",
		description: "복소수의 지수를 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 지수를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMLN",
		description: "복소수의 자연 로그값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 자연 로그값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMLOG10",
		description: "복소수의 밑이 10인 로그값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 상용 로그값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMLOG2",
		description: "복소수의 밑이 2인 로그값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 밑이 2인 로그값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMPOWER",
		description: "복소수의 멱을 반환합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 멱을 구할 복소수입니다."
			},
			{
				name: "number",
				description: "은(는) 멱의 지수입니다."
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "복소수 1개부터 255개까지의 곱을 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,...은(는) 곱할 복소수로 1개부터 255개까지 사용할 수 있습니다."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,...은(는) 곱할 복소수로 1개부터 255개까지 사용할 수 있습니다."
			}
		]
	},
	{
		name: "IMREAL",
		description: "복소수의 실수부 계수를 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 실수부 계수를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMSEC",
		description: "복소수의 시컨트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 시컨트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMSECH",
		description: "복소수의 하이퍼볼릭 시컨트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 하이퍼볼릭 시컨트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMSIN",
		description: "복소수의 사인 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 사인 값을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMSINH",
		description: "복소수의 하이퍼볼릭 사인 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 하이퍼볼릭 사인 값을 구하려는 복소수입니다."
			}
		]
	},
	{
		name: "IMSQRT",
		description: "복소수의 제곱근을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 제곱근을 구할 복소수입니다."
			}
		]
	},
	{
		name: "IMSUB",
		description: "두 복소수의 차를 반환합니다.",
		arguments: [
			{
				name: "inumber1",
				description: "은(는) 피감수인 복소수입니다."
			},
			{
				name: "inumber2",
				description: "은(는) 감수인 복소수입니다."
			}
		]
	},
	{
		name: "IMSUM",
		description: "복소수의 합을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "은(는) 더할 복소수로 1개부터 255개까지 사용할 수 있습니다."
			},
			{
				name: "inumber2",
				description: "은(는) 더할 복소수로 1개부터 255개까지 사용할 수 있습니다."
			}
		]
	},
	{
		name: "IMTAN",
		description: "복소수의 탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "inumber",
				description: "은(는) 탄젠트를 구할 복소수입니다."
			}
		]
	},
	{
		name: "INDEX",
		description: "표나 범위 내에서 값이나 참조 영역을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 배열로 입력된 셀의 범위입니다."
			},
			{
				name: "row_num",
				description: "은(는) 값을 돌려줄 배열이나 참조의 행 번호를 지정합니다. 생략하면 column_num을 지정해야 합니다."
			},
			{
				name: "column_num",
				description: "은(는) 값을 돌려줄 배열이나 참조의 열 번호를 지정합니다. 생략하면 row_num을 지정해야 합니다."
			}
		]
	},
	{
		name: "INDIRECT",
		description: "텍스트 문자열로 지정한 셀 주소를 돌려줍니다.",
		arguments: [
			{
				name: "ref_text",
				description: "은(는) 셀의 주소로서, 주소 또는 텍스트 문자열 형태로 셀 주소로 정의된 이름인 A1 또는 R1C1 스타일의 셀 주소를 포함합니다."
			},
			{
				name: "a1",
				description: "은(는) ref_text 셀의 텍스트가 어떤 주소 형식인지 정의하는 논리값입니다. FALSE이면 R1C1 스타일이고 TRUE이거나 생략하면 A1 스타일입니다."
			}
		]
	},
	{
		name: "INFO",
		description: "현재의 운영 환경에 관한 정보를 보여 줍니다.",
		arguments: [
			{
				name: "type_text",
				description: "은(는) 정보의 종류를 지정하는 텍스트입니다."
			}
		]
	},
	{
		name: "INT",
		description: "소수점 아래를 버리고 가장 가까운 정수로 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 정수로 내림하려는 실수입니다."
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "주어진 x와 y 값에 의거한 선형 회귀선의 y 절편을 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) 관측의 종속 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "known_x's",
				description: "은(는) 관측의 독립 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "INTRATE",
		description: "완전 투자한 유가 증권의 이자율을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "investment",
				description: "은(는) 유가 증권의 투자액입니다."
			},
			{
				name: "redemption",
				description: "은(는) 만기 시 상환액입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "IPMT",
		description: "주기적이고 고정적인 지급액과 이자율에 기반한 일정 기간 동안의 투자 금액에 대한 이자 지급액을 구합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다. 예를 들어, 연 이율 6%로 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "per",
				description: "은(는) 이자를 계산할 기간으로 1에서 nper 사이여야 합니다."
			},
			{
				name: "nper",
				description: "은(는) 투자의 총 지급 기간 수입니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치의 개략적인 합계입니다."
			},
			{
				name: "fv",
				description: "은(는) 지급이 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다. 생략하면 Fv는 0이 됩니다."
			},
			{
				name: "type",
				description: "은(는) 지급 기일로서, 0 또는 생략 시에는 기간 말이며 1이면 기간 초입니다."
			}
		]
	},
	{
		name: "IRR",
		description: "일련의 현금 흐름에 대한 내부 수익률을 구합니다.",
		arguments: [
			{
				name: "values",
				description: "은(는) 내부 수익률을 계산할 수를 포함하는 셀의 참조 영역 또는 배열입니다."
			},
			{
				name: "guess",
				description: "은(는) IRR 결과의 근사값이라고 추정하는 수입니다. 생략하면 0.1(10%)이 됩니다."
			}
		]
	},
	{
		name: "ISBLANK",
		description: "비어 있는 셀이면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 셀을 참조하는 셀이나 이름입니다."
			}
		]
	},
	{
		name: "ISERR",
		description: "값이 #N/A를 제외한 오류(#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? 또는 #NULL!)인지 확인하고 TRUE 또는 FALSE를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 테스트할 값입니다. 값은 셀 또는 수식이 되거나 셀, 수식, 값을 참조하는 이름이 될 수 있습니다."
			}
		]
	},
	{
		name: "ISERROR",
		description: "값이 오류(#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? 또는 #NULL!)인지 확인하고 TRUE 또는 FALSE를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 테스트할 값입니다. 값은 셀 또는 수식이 되거나 셀, 수식, 값을 참조하는 이름이 될 수 있습니다."
			}
		]
	},
	{
		name: "ISEVEN",
		description: "숫자가 짝수이면 TRUE를 반환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 검사할 값입니다."
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "수식을 포함하는 셀에 대한 참조인지 확인하고 TRUE 또는 FALSE를 반환합니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 테스트할 셀에 대한 참조입니다. 참조는 셀을 참조하는 셀 참조, 수식 또는 이름일 수 있습니다."
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "값이 논리값이면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "ISNA",
		description: "값이 #N/A 오류 값인지 확인하여 TRUE 또는 FALSE를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "값이 텍스트가 아니면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "값이 숫자이면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "수를 significance의 배수가 되도록 절대 값을 올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림하려는 수입니다."
			},
			{
				name: "significance",
				description: "은(는) 배수의 기준이 되는 수(선택적)입니다."
			}
		]
	},
	{
		name: "ISODD",
		description: "숫자가 홀수이면 TRUE를 반환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 검사할 값입니다."
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "지정된 날짜에 따른 해당 연도의 ISO 주 번호를 반환합니다.",
		arguments: [
			{
				name: "date",
				description: "은(는) Spreadsheet에서 날짜 및 시간 계산에 사용하는 날짜-시간 코드입니다."
			}
		]
	},
	{
		name: "ISPMT",
		description: "일정 기간 동안의 투자에 대한 이자 지급액을 구합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간당 이자율입니다. 예를 들어, 연 이율 6%로 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "per",
				description: "은(는) 이자 지급액을 구하고자 하는 기간입니다."
			},
			{
				name: "nper",
				description: "은(는) 투자 기간입니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 투자액이 상응하는 현재 가치의 개략적인 합계입니다."
			}
		]
	},
	{
		name: "ISREF",
		description: "값이 셀 주소이면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "ISTEXT",
		description: "값이 텍스트이면 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다. 값은 셀, 수식이 될 수도 있고 셀, 수식, 값을 참조하는 이름이 될 수도 있습니다."
			}
		]
	},
	{
		name: "KURT",
		description: "데이터 집합의 첨도를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 첨도를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 첨도를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "LARGE",
		description: "데이터 집합에서 k번째로 큰 값을 구합니다(예: 데이터 집합에서 5번째로 큰 값).",
		arguments: [
			{
				name: "array",
				description: "은(는) k번째로 큰 값을 결정하는 데 필요한 배열 또는 데이터 범위입니다."
			},
			{
				name: "k",
				description: "은(는) 배열 또는 셀 범위에서 몇 번째로 큰 값을 구할 것인지 지정합니다."
			}
		]
	},
	{
		name: "LCM",
		description: "정수의 최소 공배수를 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최소 공배수를 구할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최소 공배수를 구할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			}
		]
	},
	{
		name: "LEFT",
		description: "텍스트 문자열의 시작 지점부터 지정한 수만큼의 문자를 반환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 추출하려는 문자가 들어 있는 텍스트 문자열입니다."
			},
			{
				name: "num_chars",
				description: "은(는) 왼쪽에서부터 추출할 문자 수를 지정합니다. 생략하면 1이 됩니다."
			}
		]
	},
	{
		name: "LEN",
		description: "텍스트 문자열 내의 문자 개수를 구합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 사용자가 찾는 개수의 문자가 들어 있는 텍스트입니다. 공백도 문자 개수에 포함됩니다."
			}
		]
	},
	{
		name: "LINEST",
		description: "최소 자승법을 이용하여 직선을 근사시킴으로써 지정한 값들의 선형 추세 계수를 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) y = mx + b 식에서 이미 알고 있는 y 값의 집합입니다."
			},
			{
				name: "known_x's",
				description: "은(는) 옵션이며 y = mx + b 식에서 이미 알고 있는 x 값의 집합입니다."
			},
			{
				name: "const",
				description: "은(는) TRUE로 설정하거나 생략하면 상수 b를 정상적으로 계산하고 FALSE로 설정하면 0으로 지정하는 논리값입니다."
			},
			{
				name: "stats",
				description: "은(는) TRUE로 설정하면 추가적인 회귀 통계 항목을 함께 구하고 FALSE로 설정하거나 생략하면 m-계수와 상수 b를 구하는 논리값입니다."
			}
		]
	},
	{
		name: "LN",
		description: "자연 로그값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 자연 로그값을 구하려는 양의 실수입니다."
			}
		]
	},
	{
		name: "LOG",
		description: "지정한 밑에 대한 로그를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 로그값을 구하려는 양의 실수입니다."
			},
			{
				name: "base",
				description: "은(는) 로그의 밑입니다. 생략하면 10이 됩니다."
			}
		]
	},
	{
		name: "LOG10",
		description: "밑이 10인 로그값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 밑이 10인 로그값을 구하려는 양의 실수입니다."
			}
		]
	},
	{
		name: "LOGEST",
		description: "지정한 값들의 지수 추세 계수를 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) y = b*m^x 식에서 이미 알고 있는 y 값의 집합입니다."
			},
			{
				name: "known_x's",
				description: "은(는) 옵션이며 y = b*m^x 식에서 이미 알고 있는 x 값의 집합입니다."
			},
			{
				name: "const",
				description: "은(는) TRUE로 설정하거나 생략하면 상수 b를 정상적으로 계산하고 FALSE로 설정하면 b를 1로 지정하는 논리값입니다."
			},
			{
				name: "stats",
				description: "은(는) TRUE로 설정하면 추가적인 회귀 통계 항목을 함께 구하고 FALSE로 설정하거나 생략하면 m-계수와 상수 b를 구하는 논리값입니다."
			}
		]
	},
	{
		name: "LOGINV",
		description: "로그 정규 분포의 역함수 값을 구합니다. ln(x)는 평균과 표준 편차를 매개 변수로 갖는 정규 분포입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 로그 정규 분포를 따르는 확률입니다. 값은 0에서 1까지입니다."
			},
			{
				name: "mean",
				description: "은(는) ln(x)의 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) ln(x)의 표준 편차로 양수입니다."
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "x에서의 로그 정규 분포값을 구합니다. ln(x)는 평균과 표준 편차를 매개 변수로 갖는 정규 분포입니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수값을 구하려는 위치에서의 값으로 양수입니다."
			},
			{
				name: "mean",
				description: "은(는) ln(x)의 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) ln(x)의 표준 편차로 양수입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "로그 정규 분포의 역함수 값을 구합니다. ln(x)는 평균과 표준 편차를 매개 변수로 갖는 정규 분포입니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 로그 정규 분포를 따르는 확률입니다. 값은 0에서 1까지입니다."
			},
			{
				name: "mean",
				description: "은(는) ln(x)의 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) ln(x)의 표준 편차로 양수입니다."
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "x에서의 로그 정규 누적 분포값을 구합니다. ln(x)는 평균과 표준 편차를 매개 변수로 갖는 정규 분포입니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 함수값을 구하려는 위치에서의 값으로 양수입니다."
			},
			{
				name: "mean",
				description: "은(는) ln(x)의 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) ln(x)의 표준 편차로 양수입니다."
			}
		]
	},
	{
		name: "LOOKUP",
		description: "배열이나 한 행 또는 한 열 범위에서 값을 찾습니다. 이전 버전과의 호환성을 위해 제공됩니다.",
		arguments: [
			{
				name: "lookup_value",
				description: "은(는) LOOKUP이 lookup_vector에서 찾는 값입니다. 값은 숫자, 텍스트, 논리값, 값에 대한 이름이나 참조가 될 수 있습니다."
			},
			{
				name: "lookup_vector",
				description: "은(는) 텍스트, 숫자, 논리값의 한 행이나 한 열로만 이루어진 범위입니다."
			},
			{
				name: "result_vector",
				description: "은(는) 하나의 행이나 열로만 이루어진 범위입니다. lookup_vector와 크기가 같습니다."
			}
		]
	},
	{
		name: "LOWER",
		description: "텍스트 문자열의 모든 문자를 소문자로 변환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 소문자로 바꾸려는 텍스트입니다. 문자가 아닌 경우는 변환되지 않습니다."
			}
		]
	},
	{
		name: "MATCH",
		description: "배열에서 지정된 순서상의 지정된 값에 일치하는 항목의 상대 위치 값을 찾습니다.",
		arguments: [
			{
				name: "lookup_value",
				description: "은(는) 배열, 숫자, 텍스트, 논리값, 참조 등에서 찾으려고 하는 값입니다."
			},
			{
				name: "lookup_array",
				description: "은(는) 참조 값, 값 배열, 배열 참조가 들어 있는 연속된 셀 범위입니다."
			},
			{
				name: "match_type",
				description: "은(는) 되돌릴 값을 표시하는 숫자로 1, 0 또는 -1입니다."
			}
		]
	},
	{
		name: "MAX",
		description: "최대값을 구합니다. 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최대값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최대값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MAXA",
		description: "인수 목록에서 최대값을 구합니다. 논리값과 텍스트도 포함된 값입니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 최대값을 구하려는 값들로서 255개까지 지정할 수 있습니다. 값은 숫자, 빈 셀, 논리값, 텍스트가 될 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 최대값을 구하려는 값들로서 255개까지 지정할 수 있습니다. 값은 숫자, 빈 셀, 논리값, 텍스트가 될 수 있습니다."
			}
		]
	},
	{
		name: "MDETERM",
		description: "배열의 행렬 식을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 행과 열의 수가 같은 정방형 배열이며 셀 영역이거나 상수 배열입니다."
			}
		]
	},
	{
		name: "MEDIAN",
		description: "주어진 수들의 중간값을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 중간값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 중간값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MID",
		description: "문자열의 지정 위치에서 문자를 지정한 개수만큼 돌려줍니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 돌려줄 문자들이 포함된 문자열입니다."
			},
			{
				name: "start_num",
				description: "은(는) 돌려줄 문자열에서 첫째 문자의 위치입니다. Text에서 첫 문자는 1이 됩니다."
			},
			{
				name: "num_chars",
				description: "은(는) Text에서 돌려줄 문자 개수를 지정합니다."
			}
		]
	},
	{
		name: "MIN",
		description: "최소값을 구합니다. 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최소값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최소값을 구하려는 수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MINA",
		description: "인수 목록에서 최소값을 구합니다. 논리값과 텍스트도 포함된 값입니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 최소값을 구하려는 값들로서 255개까지 지정할 수 있습니다. 값은 숫자, 빈 셀, 논리값, 텍스트가 될 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 최소값을 구하려는 값들로서 255개까지 지정할 수 있습니다. 값은 숫자, 빈 셀, 논리값, 텍스트가 될 수 있습니다."
			}
		]
	},
	{
		name: "MINUTE",
		description: "분을 0부터 59까지의 수로 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수이거나 16:48:00 또는 오후 4:48:00 같은 시간 형식의 텍스트입니다."
			}
		]
	},
	{
		name: "MINVERSE",
		description: "배열의 역행렬을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 행과 열의 수가 같은 정방형 배열입니다."
			}
		]
	},
	{
		name: "MIRR",
		description: "일련의 주기적 현금 흐름에 대해 투자 비용과 현금 재투자 수익을 고려한 내부 수익률을 구합니다.",
		arguments: [
			{
				name: "values",
				description: "은(는) 정기적인 지출(음)과 수입(양)을 나타내는 수가 있는 셀의 참조 영역 또는 배열입니다."
			},
			{
				name: "finance_rate",
				description: "은(는) 현금 흐름에 사용한 금액에 대해 지불할 이자율입니다."
			},
			{
				name: "reinvest_rate",
				description: "은(는) 재투자함에 따라 현금 흐름에서 받을 이자율입니다."
			}
		]
	},
	{
		name: "MMULT",
		description: "두 배열의 행렬 곱을 구합니다. 행 수는 array1과 같고 열 수는 array2와 같아야 합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 곱하려는 숫자의 첫 번째 배열이며 열 수가 array2의 행 수와 같아야 합니다."
			},
			{
				name: "array2",
				description: "은(는) 곱하려는 숫자의 첫 번째 배열이며 열 수가 array2의 행 수와 같아야 합니다."
			}
		]
	},
	{
		name: "MOD",
		description: "나눗셈의 나머지를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 나머지를 구하려는 수입니다."
			},
			{
				name: "divisor",
				description: "은(는) 나누는 수입니다."
			}
		]
	},
	{
		name: "MODE",
		description: "데이터 집합에서 가장 자주 발생하는 값(최빈수)을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "데이터 집합에서 가장 자주 발생하는 값(최빈수)의 세로 배열을 구합니다. 가로 배열의 경우 TRANSPOSE(MODE.MULT(number1,number2,...))를 사용합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "데이터 집합에서 가장 자주 발생하는 값(최빈수)을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 최빈수를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "MONTH",
		description: "1(1월)에서 12(12월) 사이의 숫자로 해당 월을 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수입니다."
			}
		]
	},
	{
		name: "MROUND",
		description: "원하는 배수로 반올림된 수를 반환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 반올림할 값입니다."
			},
			{
				name: "multiple",
				description: "은(는) 숫자를 반올림할 배수의 기준이 되는 값입니다."
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "각 계승값의 곱에 대한 합계의 계승값 비율을 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 계산할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 계산할 값이며, 1개부터 255개까지 사용할 수 있습니다."
			}
		]
	},
	{
		name: "MUNIT",
		description: "지정된 차원의 단위 행렬을 반환합니다.",
		arguments: [
			{
				name: "dimension",
				description: "은(는) 반환할 단위 행렬의 차원을 지정하는 정수입니다."
			}
		]
	},
	{
		name: "N",
		description: "숫자가 아닌 값은 숫자로, 날짜 값은 일련 번호로, TRUE 값은 1로, 그 외의 값은 0(영)으로 변환하여 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 변환하려는 값입니다."
			}
		]
	},
	{
		name: "NA",
		description: "#N/A 오류 값을 돌려줍니다.",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "음 이항 분포값을 구합니다. 성공 확률이 Probability_s이고 Number_s번째 성공하기 전에 Number_f번 실패가 있는 경우의 확률을 의미합니다.",
		arguments: [
			{
				name: "number_f",
				description: "은(는) 실패 횟수입니다."
			},
			{
				name: "number_s",
				description: "은(는) 성공 횟수의 임계치입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 성공 확률입니다. 값은 0에서 1 사이입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "음 이항 분포값을 구합니다. 성공 확률이 Probability_s이고 Number_s번째 성공하기 전에 Number_f번 실패가 있는 경우의 확률을 의미합니다.",
		arguments: [
			{
				name: "number_f",
				description: "은(는) 실패 횟수입니다."
			},
			{
				name: "number_s",
				description: "은(는) 성공 횟수의 임계치입니다."
			},
			{
				name: "probability_s",
				description: "은(는) 성공 확률입니다. 값은 0에서 1 사이입니다."
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "두 날짜 사이의 전체 작업 일수를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "end_date",
				description: "은(는) 마지막 날짜입니다."
			},
			{
				name: "holidays",
				description: "은(는) 국경일, 공휴일, 임시 휴일 등 작업 일수에서 제외되는 한 개 이상의 날짜 목록이며 선택 사항입니다."
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "사용자 지정 weekend 매개 변수를 사용하여 두 날짜 사이의 전체 작업 일수를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "end_date",
				description: "은(는) 마지막 날짜입니다."
			},
			{
				name: "weekend",
				description: "은(는) 주말을 나타내는 숫자 또는 문자열입니다."
			},
			{
				name: "holidays",
				description: "은(는) 국경일, 공휴일, 임시 휴일 등 작업 일수에서 제외되는 한 개 이상의 날짜 목록이며 선택 사항입니다."
			}
		]
	},
	{
		name: "NOMINAL",
		description: "명목상의 연이율을 반환합니다.",
		arguments: [
			{
				name: "effect_rate",
				description: "은(는) 실질적인 이율입니다."
			},
			{
				name: "npery",
				description: "은(는) 연간 복리 계산 횟수입니다."
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "지정한 평균과 표준 편차에 의거 정규 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값입니다."
			},
			{
				name: "mean",
				description: "은(는) 분포의 산술 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 분포의 표준 편차이며 양수입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이면 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "NORM.INV",
		description: "지정한 평균과 표준 편차에 의거하여 정규 누적 분포의 역함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 정규 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			},
			{
				name: "mean",
				description: "은(는) 분포의 산술 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 분포의 표준 편차로서 양수입니다."
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "표준 정규 누적 분포값을 구합니다. 평균이 0이고 표준 편차가 1인 정규 분포를 의미합니다.",
		arguments: [
			{
				name: "z",
				description: "은(는) 분포를 구하려는 값입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE를 사용합니다."
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "표준 정규 누적 분포의 역함수를 구합니다. 평균이 0이고 표준 편차가 1인 정규 분포를 의미합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 정규 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			}
		]
	},
	{
		name: "NORMDIST",
		description: "지정한 평균과 표준 편차에 의거 정규 누적 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값입니다."
			},
			{
				name: "mean",
				description: "은(는) 분포의 산술 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 분포의 표준 편차이며 양수입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이면 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "NORMINV",
		description: "지정한 평균과 표준 편차에 의거하여 정규 누적 분포의 역함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 정규 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			},
			{
				name: "mean",
				description: "은(는) 분포의 산술 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 분포의 표준 편차로서 양수입니다."
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "표준 정규 누적 분포값을 구합니다. 평균이 0이고 표준 편차가 1인 정규 분포를 의미합니다.",
		arguments: [
			{
				name: "z",
				description: "은(는) 분포를 구하려는 값입니다."
			}
		]
	},
	{
		name: "NORMSINV",
		description: "표준 정규 누적 분포의 역함수를 구합니다. 평균이 0이고 표준 편차가 1인 정규 분포를 의미합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 정규 분포를 따르는 확률입니다. 범위는 0에서 1까지입니다."
			}
		]
	},
	{
		name: "NOT",
		description: "TRUE 식에는 FALSE를, FALSE 식에는 TRUE를 돌려줍니다.",
		arguments: [
			{
				name: "logical",
				description: "은(는) TRUE나 FALSE를 판정할 수 있는 값 또는 식입니다."
			}
		]
	},
	{
		name: "NOW",
		description: "현재의 날짜와 시간을 날짜와 시간 형식으로 구합니다.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "주기적이고 고정적인 지급액과 고정적인 이율에 의거한 투자의 기간을 구합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다.예를 들어, 연 이율 6%에 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "pmt",
				description: "은(는) 각 기간마다의 지급액입니다. 이것은 투자 기간 동안 변할 수 없습니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치 또는 총합계입니다."
			},
			{
				name: "fv",
				description: "은(는) 지급이 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다. 생략하면 0으로 지정됩니다."
			},
			{
				name: "type",
				description: "은(는) 지급 시기를 나타내며 1은 투자 주기 초를, 0 또는 생략 시에는 투자 주기 말을 의미합니다."
			}
		]
	},
	{
		name: "NPV",
		description: "주기적인 현금 흐름과 할인율을 기준으로 투자의 순 현재 가치를 산출합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "은(는) 일정 기간의 할인율입니다."
			},
			{
				name: "value1",
				description: "은(는) 지급액과 수입을 나타내는 1개에서 254개까지의 인수입니다."
			},
			{
				name: "value2",
				description: "은(는) 지급액과 수입을 나타내는 1개에서 254개까지의 인수입니다."
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "로캘 독립적 방식으로 텍스트를 숫자로 변환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 변환할 숫자를 나타내는 문자열입니다."
			},
			{
				name: "decimal_separator",
				description: "은(는) 문자열에서 소수 구분 기호로 사용되는 문자입니다."
			},
			{
				name: "group_separator",
				description: "은(는) 문자열에서 그룹 구분 기호로 사용되는 문자입니다."
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "8진수를 2진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 8진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "8진수를 10진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 8진수입니다."
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "8진수를 16진수로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 8진수입니다."
			},
			{
				name: "places",
				description: "은(는) 사용할 자릿수입니다."
			}
		]
	},
	{
		name: "ODD",
		description: "주어진 수에 가장 가까운 홀수로, 양수인 경우 올림하고 음수인 경우 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림 또는 내림하려는 수입니다."
			}
		]
	},
	{
		name: "OFFSET",
		description: "주어진 참조 영역으로부터 지정한 행과 열만큼 떨어진 위치의 참조 영역을 돌려줍니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 기본 참조 영역입니다."
			},
			{
				name: "rows",
				description: "은(는) 기본 참조 영역의 첫 행과 출력할 영역의 첫 행 사이의 간격입니다."
			},
			{
				name: "cols",
				description: "은(는) 기본 참조 영역의 첫 열과 출력할 영역의 첫 열 사이의 간격입니다."
			},
			{
				name: "height",
				description: "은(는) 출력하려는 참조 영역의 높이(행 수)입니다."
			},
			{
				name: "width",
				description: "은(는) 출력하려는 참조 영역의 너비(열 수)입니다. 생략하면 기본 참조 영역과 동일하게 설정됩니다."
			}
		]
	},
	{
		name: "OR",
		description: "하나 이상의 인수가 TRUE이면 TRUE를 반환합니다. 인수가 모두 FALSE인 경우에만 FALSE를 반환합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 255개까지 지정할 수 있습니다."
			},
			{
				name: "logical2",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "투자를 통해 지정한 가치에 도달하는 데 필요한 기간을 반환합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다."
			},
			{
				name: "pv",
				description: "은(는) 투자의 현재 가치입니다."
			},
			{
				name: "fv",
				description: "은(는) 얻고자 하는 투자의 미래 가치입니다."
			}
		]
	},
	{
		name: "PEARSON",
		description: "피어슨 곱 모멘트 상관 계수 r을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 독립적인 값의 집합입니다."
			},
			{
				name: "array2",
				description: "은(는) 종속적인 값의 집합입니다."
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "범위에서 k번째 백분위수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "k",
				description: "은(는) 0에서 1까지 범위의 백분위수 값입니다."
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "범위에서 k번째 백분위수를 구합니다.이때 k는 경계값을 제외한 0에서 1 사이의 수입니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "k",
				description: "은(는) 0에서 1까지 범위의 백분위수 값입니다."
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "범위에서 k번째 백분위수를 구합니다. 이때 k는 경계값을 포함한 0에서 1 사이의 수입니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "k",
				description: "은(는) 0에서 1까지 범위의 백분위수 값입니다."
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "데이터 집합에서 백분율 순위를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "x",
				description: "은(는) 순위를 알려는 값입니다."
			},
			{
				name: "significance",
				description: "은(는) 구한 백분율 값에 대한 유효 자릿수의 개수를 나타내는 값입니다. 생략되면 세 자리로 표시됩니다(예: 0.xxx%)."
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "데이터 집합에서 경계값을 제외한 0에서 1 사이의 백분율 순위를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "x",
				description: "은(는) 순위를 알려는 값입니다."
			},
			{
				name: "significance",
				description: "은(는) 구한 백분율 값에 대한 유효 자릿수의 개수를 나타내는 값입니다. 생략되면 세 자리로 표시됩니다(예: 0.xxx%)."
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "데이터 집합에서 경계값을 포함한 0에서 1 사이의 백분율 순위를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 상대 순위를 구할 데이터의 범위 또는 배열입니다."
			},
			{
				name: "x",
				description: "은(는) 순위를 알려는 값입니다."
			},
			{
				name: "significance",
				description: "은(는) 구한 백분율 값에 대한 유효 자릿수의 개수를 나타내는 값입니다. 생략되면 세 자리로 표시됩니다(예: 0.xxx%)."
			}
		]
	},
	{
		name: "PERMUT",
		description: "개체 전체에서 선택하여 주어진 개체 수로 만들 수 있는 순열의 수를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 개체의 전체 수입니다."
			},
			{
				name: "number_chosen",
				description: "은(는) 각 순열에서의 개체 수입니다."
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "개체 전체에서 선택하여 주어진 개체 수(반복 포함)로 만들 수 있는 순열의 수를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 개체의 전체 수입니다."
			},
			{
				name: "number_chosen",
				description: "은(는) 각 순열에서의 개체 수입니다."
			}
		]
	},
	{
		name: "PHI",
		description: "표준 정규 분포값의 밀도 함수 값을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 표준 정규 분포값의 밀도를 구하려는 숫자입니다."
			}
		]
	},
	{
		name: "PI",
		description: "원주율(파이:3.14159265358979) 값을 구합니다.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "주기적이고 고정적인 지급액과 고정적인 이율에 의거한 대출 상환금을 계산합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 대출에 대한 기간별 이자율입니다. 예를 들어, 연 이율 6%에 분기 상환 시에는 6%/4를 사용합니다."
			},
			{
				name: "nper",
				description: "은(는) 대출 상환금의 총 지급 기간 수입니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치입니다."
			},
			{
				name: "fv",
				description: "은(는) 상환이 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다. 생략하면 0(영)으로 지정됩니다."
			},
			{
				name: "type",
				description: "은(는) 상환 시기를 나타내며 1은 상환 주기 초를, 0 또는 생략 시에는 상환 주기 말을 의미합니다."
			}
		]
	},
	{
		name: "POISSON",
		description: "포아송 확률 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 사건의 수입니다."
			},
			{
				name: "mean",
				description: "은(는) 기대값으로 양수입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 확률 분포의 형태를 결정하는 논리값입니다. TRUE이면 누적 포아송 확률을, FALSE이면 포아송 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "포아송 확률 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 사건의 수입니다."
			},
			{
				name: "mean",
				description: "은(는) 기대값으로 양수입니다."
			},
			{
				name: "cumulative",
				description: "은(는) 확률 분포의 형태를 결정하는 논리값입니다. TRUE이면 누적 포아송 확률을, FALSE이면 포아송 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "POWER",
		description: "밑수를 지정한 만큼 거듭제곱한 결과를 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 밑수입니다."
			},
			{
				name: "power",
				description: "은(는) 지수입니다."
			}
		]
	},
	{
		name: "PPMT",
		description: "주기적이고 고정적인 지급액과 이자율에 기반한 일정 기간 동안의 투자에 대한 원금의 지급액을 구합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다. 예를 들어, 연 이율 6%로 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "per",
				description: "은(는) 기간을 지정하며, 1에서 nper까지의 범위입니다."
			},
			{
				name: "nper",
				description: "은(는) 투자의 총 지급 기간 수입니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치의 개략적인 합계입니다."
			},
			{
				name: "fv",
				description: "은(는) 지급이 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다."
			},
			{
				name: "type",
				description: "은(는) 지급 기일로서, 0 또는 생략 시에는 기간 말이며 1이면 기간 초입니다."
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "할인된 유가 증권의 액면가 $100당 가격을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "discount",
				description: "은(는) 유가 증권의 할인율입니다."
			},
			{
				name: "redemption",
				description: "은(는) 액면가 $100당 유가 증권의 상환액입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "PROB",
		description: "영역 내의 값이 최소값을 포함한 두 한계값 사이에 있을 확률을 구합니다.",
		arguments: [
			{
				name: "x_range",
				description: "은(는) 확률을 지정할 x 값의 범위입니다."
			},
			{
				name: "prob_range",
				description: "은(는) x_range의 각 값이 발생할 확률의 집합입니다. 범위는 0에서 1까지이며 0은 제외됩니다."
			},
			{
				name: "lower_limit",
				description: "은(는) 확률을 구하고자 하는 구간의 하한값입니다."
			},
			{
				name: "upper_limit",
				description: "은(는) 확률을 구하고자 하는 구간의 상한값입니다. 생략되면 x_range의 값이 하한값과 같을 때의 확률을 구해줍니다."
			}
		]
	},
	{
		name: "PRODUCT",
		description: "인수들의 곱을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 곱하려는 수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 곱하려는 수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "PROPER",
		description: "각 단어의 첫째 문자를 대문자로 변환하고 나머지 문자는 소문자로 변환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 인용 부호로 둘러싸인 텍스트, 텍스트를 출력하는 수식 또는 텍스트가 있는 셀의 주소입니다."
			}
		]
	},
	{
		name: "PV",
		description: "투자의 현재 가치를 구합니다. 일련의 미래 투자가 상응하는 현재 가치의 총합계입니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 기간별 이자율입니다. 예를 들어, 연 이율 6%에 분기 지급 시에는 6%/4를 사용합니다."
			},
			{
				name: "nper",
				description: "은(는) 투자의 총 지급 기간입니다."
			},
			{
				name: "pmt",
				description: "은(는) 각 기간에 대한 지급액으로서 투자 기간 중에 변경될 수 없습니다."
			},
			{
				name: "fv",
				description: "은(는) 투자가 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다."
			},
			{
				name: "type",
				description: "은(는) 투자 주기 초에 지급 시에는 1로 설정하고 투자 주기 말에 지급 시에는 0으로 설정하거나 생략하는 논리값입니다."
			}
		]
	},
	{
		name: "QUARTILE",
		description: "데이터 집합에서 사분위수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 사분위수를 구하려는 수치 값의 배열 또는 셀 범위입니다."
			},
			{
				name: "quart",
				description: "은(는) 사분위수 값으로서 최소값은 0, 1분위는 1, 중간값은 2, 3분위는 3, 최대값은 4입니다."
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "데이터 집합에서 경계값을 제외한 0에서 1 사이의 사분위수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 사분위수를 구하려는 수치 값의 배열 또는 셀 범위입니다."
			},
			{
				name: "quart",
				description: "은(는) 사분위수 값으로서 최소값은 0, 1분위는 1, 중간값은 2, 3분위는 3, 최대값은 4입니다."
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "데이터 집합에서 경계값을 포함한 0에서 1 사이의 사분위수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 사분위수를 구하려는 수치 값의 배열 또는 셀 범위입니다."
			},
			{
				name: "quart",
				description: "은(는) 사분위수 값으로서 최소값은 0, 1분위는 1, 중간값은 2, 3분위는 3, 최대값은 4입니다."
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "나눗셈 몫의 정수 부분을 반환합니다.",
		arguments: [
			{
				name: "numerator",
				description: "은(는) 피제수입니다."
			},
			{
				name: "denominator",
				description: "은(는) 제수입니다."
			}
		]
	},
	{
		name: "RADIANS",
		description: "도 단위로 표시된 각도를 라디안으로 변환합니다.",
		arguments: [
			{
				name: "angle",
				description: "은(는) 라디안으로 변환할 각도입니다."
			}
		]
	},
	{
		name: "RAND",
		description: "0보다 크거나 같고 1보다 작은, 균등하게 분포된 난수를 구합니다. 재계산 시에는 바뀝니다.",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "지정한 두 수 사이의 난수를 반환합니다.",
		arguments: [
			{
				name: "bottom",
				description: "은(는) RANDBETWEEN 함수가 반환할 최소 정수값입니다."
			},
			{
				name: "top",
				description: "은(는) RANDBETWEEN 함수가 반환할 최대 정수값입니다."
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "수 목록 내에서 지정한 수의 크기 순위를 구합니다. 목록 내에서 다른 값에 대한 상대적인 크기를 말합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 순위를 구하려는 수입니다."
			},
			{
				name: "ref",
				description: "은(는) 수 목록의 배열 또는 셀 주소입니다. 수 이외의 값은 제외됩니다."
			},
			{
				name: "order",
				description: "은(는) 순위를 정할 방법을 지정하는 수입니다. 0이나 생략하면 내림차순으로, 0이 아닌 값을 지정하면 오름차순으로 순위가 정해집니다."
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "수 목록 내에서 지정한 수의 크기 순위를 구합니다. 목록 내에서 다른 값에 대한 상대적인 크기를 말합니다. 둘 이상의 값이 순위가 같으면 평균 순위가 반환됩니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 순위를 구하려는 수입니다."
			},
			{
				name: "ref",
				description: "은(는) 수 목록의 배열 또는 셀 주소입니다. 수 이외의 값은 제외됩니다."
			},
			{
				name: "order",
				description: "은(는) 순위를 정할 방법을 지정하는 수입니다. 0이나 생략하면 내림차순으로, 0이 아닌 값을 지정하면 오름차순으로 순위가 정해집니다."
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "수 목록 내에서 지정한 수의 크기 순위를 구합니다. 목록 내에서 다른 값에 대한 상대적인 크기를 말합니다. 둘 이상의 값이 순위가 같으면 해당 값 집합에서 가장 높은 순위가 반환됩니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 순위를 구하려는 수입니다."
			},
			{
				name: "ref",
				description: "은(는) 수 목록의 배열 또는 셀 주소입니다. 수 이외의 값은 제외됩니다."
			},
			{
				name: "order",
				description: "은(는) 순위를 정할 방법을 지정하는 수입니다. 0이나 생략하면 내림차순으로, 0이 아닌 값을 지정하면 오름차순으로 순위가 정해집니다."
			}
		]
	},
	{
		name: "RATE",
		description: "대출 또는 투자의 기간별 이자율을 구합니다. 예를 들어, 연 이율 6%에 분기 지급 시에는 6%/4를 사용합니다.",
		arguments: [
			{
				name: "nper",
				description: "은(는) 대출 또는 투자의 총 지급 기간 수입니다."
			},
			{
				name: "pmt",
				description: "은(는) 각 기간의 지급액으로서, 대출 또는 투자 기간 중에 변경될 수 없습니다."
			},
			{
				name: "pv",
				description: "은(는) 일련의 미래 지급액에 상응하는 현재 가치입니다."
			},
			{
				name: "fv",
				description: "은(는) 지급이 완료된 후 얻고자 하는 미래 가치 또는 현금 잔액입니다. 생략하면 Fv는 0이 됩니다."
			},
			{
				name: "type",
				description: "은(는) 지급 시기를 나타내며 1은 지급 주기 초를, 0 또는 생략 시에는 지급 주기 말을 의미합니다."
			},
			{
				name: "guess",
				description: "은(는) 이율이 얼마나 될 것인가에 대한 추정입니다. 생략하면 guess는 0.1(10%)이 됩니다."
			}
		]
	},
	{
		name: "RECEIVED",
		description: "완전 투자 유가 증권에 대해 만기 시 수령하는 금액을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "investment",
				description: "은(는) 유가 증권의 투자액입니다."
			},
			{
				name: "discount",
				description: "은(는) 유가 증권의 할인율입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "REPLACE",
		description: "텍스트의 일부를 다른 텍스트로 바꿉니다.",
		arguments: [
			{
				name: "old_text",
				description: "은(는) 일부분을 바꾸려는 텍스트입니다."
			},
			{
				name: "start_num",
				description: "은(는) old_text에서 바꾸기를 시작할 위치로서, 문자 단위로 지정합니다."
			},
			{
				name: "num_chars",
				description: "은(는) old_text에서 바꾸려는 문자의 개수입니다."
			},
			{
				name: "new_text",
				description: "은(는) old_text의 일부를 대체할 새 텍스트입니다."
			}
		]
	},
	{
		name: "REPT",
		description: "텍스트를 지정한 횟수만큼 반복합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 반복하려는 텍스트입니다."
			},
			{
				name: "number_times",
				description: "은(는) 0보다 큰 숫자로 반복 횟수를 지정합니다."
			}
		]
	},
	{
		name: "RIGHT",
		description: "텍스트 문자열의 끝 지점부터 지정한 수만큼의 문자를 반환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 추출하려는 문자가 들어 있는 텍스트 문자열입니다."
			},
			{
				name: "num_chars",
				description: "은(는) 추출할 문자 수를 지정합니다. 생략하면 1이 됩니다."
			}
		]
	},
	{
		name: "ROMAN",
		description: "아라비아 숫자를 텍스트인 로마 숫자로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 변환할 아라비아 숫자입니다."
			},
			{
				name: "form",
				description: "은(는) 원하는 로마 숫자의 형태를 나타내는 수입니다."
			}
		]
	},
	{
		name: "ROUND",
		description: "수를 지정한 자릿수로 반올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 반올림하려는 수입니다."
			},
			{
				name: "num_digits",
				description: "은(는) 소수점 아래의 자릿수를 지정합니다."
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "0에 가까워지도록 수를 내림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 내림하려는 실수입니다."
			},
			{
				name: "num_digits",
				description: "은(는) 내림하려는 자릿수입니다."
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "0에서 멀어지도록 수를 올림합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 올림하려는 실수입니다."
			},
			{
				name: "num_digits",
				description: "은(는) 올림하려는 자릿수입니다."
			}
		]
	},
	{
		name: "ROW",
		description: "참조의 행 번호를 구합니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 행 번호를 구하려는 셀 또는 셀 범위입니다. 생략하면 ROW 함수가 들어 있는 셀을 되돌립니다."
			}
		]
	},
	{
		name: "ROWS",
		description: "참조 영역이나 배열에 있는 행 수를 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 행 수를 구하려는 배열이나 배열식 또는 셀 범위의 주소입니다."
			}
		]
	},
	{
		name: "RRI",
		description: "투자 증가액의 평균 이자율을 반환합니다.",
		arguments: [
			{
				name: "nper",
				description: "은(는) 투자 기간입니다."
			},
			{
				name: "pv",
				description: "은(는) 투자의 현재 가치입니다."
			},
			{
				name: "fv",
				description: "은(는) 투자의 미래 가치입니다."
			}
		]
	},
	{
		name: "RSQ",
		description: "피어슨 곱 모멘트 상관 계수의 제곱을 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "known_x's",
				description: "은(는) 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "RTD",
		description: "COM 자동화를 지원하는 프로그램으로부터 실시간 데이터를 가져옵니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "은(는) 등록된 COM 자동화 추가 기능의 ProgID 이름입니다. 이름 앞 뒤에 인용 부호를 사용하십시오."
			},
			{
				name: "server",
				description: "은(는) 추가 기능을 실행할 서버의 이름입니다. 이름 앞 뒤에 인용 부호를 사용하십시오. 추가 기능을 로컬로 실행하는 경우에는 문자열(string)을 비워 두십시오."
			},
			{
				name: "topic1",
				description: "은(는) 데이터를 지정하는 매개 변수로서 38개까지 지정할 수 있습니다."
			},
			{
				name: "topic2",
				description: "은(는) 데이터를 지정하는 매개 변수로서 38개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "SEARCH",
		description: "왼쪽에서 오른쪽으로 검색하여 지정한 문자 또는 텍스트 스트링이 처음 발견되는 곳에서의 문자 개수를 구합니다(대/소문자 구분 안 함).",
		arguments: [
			{
				name: "find_text",
				description: "은(는) 찾으려는 텍스트입니다. ? 및 *와 같은 와일드카드를 사용할 수도 있습니다. 실제 ?와 * 문자를 찾으려면 ~?와 ~*를 사용하십시오."
			},
			{
				name: "within_text",
				description: "은(는) find_text를 찾고자 하는 곳의 텍스트입니다."
			},
			{
				name: "start_num",
				description: "은(는) within_text에서 왼쪽에서부터 검색을 시작하고자 하는 곳의 문자 개수입니다. 생략하면 1이 사용됩니다."
			}
		]
	},
	{
		name: "SEC",
		description: "각도의 시컨트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "SECH",
		description: "각도의 하이퍼볼릭 시컨트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "SECOND",
		description: "초를 0부터 59까지의 수로 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수이거나 16:48:23 또는 오후 4:48:47 같은 시간 형식의 텍스트입니다."
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "수식에 따라 멱급수의 합을 반환합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 멱급수에 대한 입력 값입니다."
			},
			{
				name: "n",
				description: "은(는) x 멱급수의 초기항 차수입니다."
			},
			{
				name: "m",
				description: "은(는) 급수에서 각 항목의 차수 n에 대한 증분입니다."
			},
			{
				name: "coefficients",
				description: "은(는) 연속된 각 x의 멱에 곱해지는 계수의 집합입니다."
			}
		]
	},
	{
		name: "SHEET",
		description: "참조된 시트의 시트 번호를 반환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 시트 번호를 구하려는 시트 또는 참조의 이름입니다. 이 인수를 생략하면 해당 함수를 포함하는 시트 번호가 반환됩니다."
			}
		]
	},
	{
		name: "SHEETS",
		description: "참조의 시트 수를 반환합니다.",
		arguments: [
			{
				name: "reference",
				description: "은(는) 포함된 시트 수를 알려는 참조입니다. 이 인수를 생략하면 해당 함수를 포함하는 통합 문서의 시트 수가 반환됩니다."
			}
		]
	},
	{
		name: "SIGN",
		description: "수의 부호값을 반환합니다. 수가 양수이면 1을, 0이면 0을, 음수이면 -1을 돌려줍니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 부호를 구할 실수입니다."
			}
		]
	},
	{
		name: "SIN",
		description: "각도의 사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "SINH",
		description: "하이퍼볼릭 사인 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 실수입니다."
			}
		]
	},
	{
		name: "SKEW",
		description: "분포의 왜곡도를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 왜곡도를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 왜곡도를 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "SKEW.P",
		description: "모집단을 기준으로 분포의 왜곡도를 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단 왜곡도를 구하려는 인수로서 1개에서 254개까지 지정할 수 있으며 숫자나 이름, 배열 또는 참조가 될 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단 왜곡도를 구하려는 인수로서 1개에서 254개까지 지정할 수 있으며 숫자나 이름, 배열 또는 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "SLN",
		description: "한 기간 동안 정액법에 의한 자산의 감가 상각액을 구합니다.",
		arguments: [
			{
				name: "cost",
				description: "은(는) 자산의 초기 취득가액입니다."
			},
			{
				name: "salvage",
				description: "은(는) 감가 상각 종료 시 회수되는 값입니다."
			},
			{
				name: "life",
				description: "은(는) 자산이 감가 상각되는 기간 수(자산의 수명 년수)입니다."
			}
		]
	},
	{
		name: "SLOPE",
		description: "선형 회귀선의 기울기를 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) 종속 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "known_x's",
				description: "은(는) 독립 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "SMALL",
		description: "데이터 집합에서 k번째로 작은 값을 구합니다(예: 데이터 집합에서 5번째로 작은 값).",
		arguments: [
			{
				name: "array",
				description: "은(는) k번째로 작은 값을 결정하는 데 필요한 배열 또는 데이터 범위입니다."
			},
			{
				name: "k",
				description: "은(는) 배열 또는 셀 범위에서 몇 번째로 작은 값을 구할 것인지 지정합니다."
			}
		]
	},
	{
		name: "SQRT",
		description: "양의 제곱근을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 제곱근을 구하려는 수입니다."
			}
		]
	},
	{
		name: "SQRTPI",
		description: "(number * Pi)의 제곱근을 반환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) p와 곱할 수입니다."
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "정규화된 값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 정규화하려는 값입니다."
			},
			{
				name: "mean",
				description: "은(는) 분포의 산술 평균입니다."
			},
			{
				name: "standard_dev",
				description: "은(는) 분포의 표준 편차로 양수입니다."
			}
		]
	},
	{
		name: "STDEV",
		description: "표본 집단의 표준 편차를 구합니다. 표본 집단에서 텍스트나 논리값은 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "STDEV.P",
		description: "모집단의 표준 편차를 구합니다. 텍스트와 논리값은 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "STDEV.S",
		description: "표본 집단의 표준 편차를 구합니다. 표본 집단에서 텍스트나 논리값은 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "STDEVA",
		description: "표본 집단의 표준 편차(논리값과 텍스트 포함)를 구합니다. 논리값 FALSE는 0 값을, TRUE는 1 값을 가집니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 모집단 인구의 표준 편차를 구할 인수로서 255개까지 지정할 수 있습니다. 인수는 값, 이름, 참조가 될 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 모집단 인구의 표준 편차를 구할 인수로서 255개까지 지정할 수 있습니다. 인수는 값, 이름, 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "STDEVP",
		description: "모집단의 표준 편차를 구합니다. 텍스트와 논리값은 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "STDEVPA",
		description: "전체 모집단의 표준 편차(논리값과 텍스트 포함)를 구합니다. FALSE는 0 값을, TRUE는 1 값을 가집니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 표준 편차를 구할 인수로서 255개까지 지정할 수 있습니다. 인수는 값, 이름, 배열, 숫자가 들어 있는 참조가 될 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 표준 편차를 구할 인수로서 255개까지 지정할 수 있습니다. 인수는 값, 이름, 배열, 숫자가 들어 있는 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "STEYX",
		description: "회귀분석에 의해 예측한 y 값의 표준 오차를 각 x 값에 대하여 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) 종속 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "known_x's",
				description: "은(는) 독립 데이터 배열 또는 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "텍스트 중의 old_text를 찾아서 new_text로 바꿉니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 찾기 및 바꾸기의 대상이 되는 텍스트입니다."
			},
			{
				name: "old_text",
				description: "은(는) 찾아서 new_text로 바꿀 텍스트입니다. 대/소문자가 정확하게 일치해야만 바꿀 수 있습니다."
			},
			{
				name: "new_text",
				description: "은(는) old_text와 바꾸려는 새 텍스트입니다."
			},
			{
				name: "instance_num",
				description: "은(는) 몇 번째의 old_text를 바꿀 것인지 지정합니다."
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "목록이나 데이터베이스의 부분합을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "은(는) 목록 내에서 부분합을 계산하는 데 어떤 함수를 사용할 것인지 지정하는 1에서 11까지의 수입니다."
			},
			{
				name: "ref1",
				description: "은(는) 부분합을 구하려는 최대 254개의 영역이나 셀의 주소입니다."
			}
		]
	},
	{
		name: "SUM",
		description: "인수들의 합을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 합계를 구하려는 값들로서 255개까지 지정할 수 있습니다. 논리값과 텍스트는 제외됩니다."
			},
			{
				name: "number2",
				description: "은(는) 합계를 구하려는 값들로서 255개까지 지정할 수 있습니다. 논리값과 텍스트는 제외됩니다."
			}
		]
	},
	{
		name: "SUMIF",
		description: "주어진 조건에 의해 지정된 셀들의 합을 구합니다.",
		arguments: [
			{
				name: "range",
				description: "은(는) 조건에 맞는지를 검사할 셀들입니다."
			},
			{
				name: "criteria",
				description: "은(는) 더할 셀의 조건을 지정하는 수, 식 또는 텍스트입니다."
			},
			{
				name: "sum_range",
				description: "은(는) 합을 구할 실제 셀들입니다. 생략하면 범위 내의 셀들이 계산됩니다."
			}
		]
	},
	{
		name: "SUMIFS",
		description: "주어진 조건에 따라 지정되는 셀을 더합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "은(는) 합을 구할 실제 셀입니다."
			},
			{
				name: "criteria_range",
				description: "은(는) 조건을 적용시킬 셀 범위입니다."
			},
			{
				name: "criteria",
				description: "은(는) 숫자, 식 또는 텍스트 형식으로 된 조건으로 더할 셀을 정의합니다."
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "배열 또는 범위의 대응되는 값끼리 곱해서 그 합을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "은(는) 계산하려는 배열로서 2개에서 255개까지 지정할 수 있습니다. 모든 배열은 같은 차원이어야 합니다."
			},
			{
				name: "array2",
				description: "은(는) 계산하려는 배열로서 2개에서 255개까지 지정할 수 있습니다. 모든 배열은 같은 차원이어야 합니다."
			},
			{
				name: "array3",
				description: "은(는) 계산하려는 배열로서 2개에서 255개까지 지정할 수 있습니다. 모든 배열은 같은 차원이어야 합니다."
			}
		]
	},
	{
		name: "SUMSQ",
		description: "인수의 제곱의 합을 구합니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 제곱의 합을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 제곱의 합을 구하려는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "두 배열에서 대응값의 제곱의 차이를 구한 다음 차이의 합을 구합니다.",
		arguments: [
			{
				name: "array_x",
				description: "은(는) 첫 번째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array_y",
				description: "은(는) 두 번째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "두 배열에서 대응값의 제곱의 합의 합을 구합니다.",
		arguments: [
			{
				name: "array_x",
				description: "은(는) 첫째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			},
			{
				name: "array_y",
				description: "은(는) 둘째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 배열이나 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "두 배열이나 범위에서 대응값의 차이를 구한 다음 차이의 제곱의 합을 구합니다.",
		arguments: [
			{
				name: "array_x",
				description: "은(는) 첫 번째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 참조나 배열이 될 수 있습니다."
			},
			{
				name: "array_y",
				description: "은(는) 두 번째 배열 또는 셀 범위입니다. 범위는 숫자, 이름, 숫자가 들어 있는 참조나 배열이 될 수 있습니다."
			}
		]
	},
	{
		name: "SYD",
		description: "지정한 기간 동안 연수 합계법에 의한 자산의 감가 상각액을 구합니다.",
		arguments: [
			{
				name: "cost",
				description: "은(는) 자산의 초기 취득가액입니다."
			},
			{
				name: "salvage",
				description: "은(는) 감가 상각 종료 시 회수되는 값입니다."
			},
			{
				name: "life",
				description: "은(는) 자산이 감가 상각되는 기간 수(자산의 수명 년수)입니다."
			},
			{
				name: "per",
				description: "은(는) 지정한 기간이며 life와 같은 단위여야 합니다."
			}
		]
	},
	{
		name: "T",
		description: "값이 텍스트이면 값을 돌려주고, 텍스트가 아니면 큰따옴표(빈 텍스트)를 돌려줍니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 검사하려는 값입니다."
			}
		]
	},
	{
		name: "T.DIST",
		description: "좌측 스튜던트 t-분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 정수입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. 누적 분포 함수에는 TRUE를, 확률 밀도 함수에는 FALSE"
			},
			{
				name: "cumulative",
				description: "를 사용합니다."
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "양측 스튜던트 t-분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 정수입니다."
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "우측 스튜던트 t-분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 정수입니다."
			}
		]
	},
	{
		name: "T.INV",
		description: "좌측 스튜던트 t-분포의 역함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 양측 스튜던트 t-분포를 따르는 0에서 1 사이의 확률입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 양의 정수입니다."
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "양측 스튜던트 t-분포의 역함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 양측 스튜던트 t-분포를 따르는 0에서 1 사이의 확률입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 양의 정수입니다."
			}
		]
	},
	{
		name: "T.TEST",
		description: "스튜던트 t-검정에 근거한 확률을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫 번째 데이터 집합입니다."
			},
			{
				name: "array2",
				description: "은(는) 두 번째 데이터 집합입니다."
			},
			{
				name: "tails",
				description: "은(는) 분포가 단측인지 양측인지를 지정하는 수입니다. 단측이면 1로, 양측이면 2로 지정합니다."
			},
			{
				name: "type",
				description: "은(는) 수행할 t-검정의 종류입니다."
			}
		]
	},
	{
		name: "TAN",
		description: "각도의 탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 라디안으로 표시된 각도입니다."
			}
		]
	},
	{
		name: "TANH",
		description: "하이퍼볼릭 탄젠트 값을 구합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 실수입니다."
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "국채에 대해 채권에 해당하는 수익률을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 만기일입니다."
			},
			{
				name: "discount",
				description: "은(는) 국채의 할인율입니다."
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "국채에 대해 액면가 $100당 가격을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 만기일입니다."
			},
			{
				name: "discount",
				description: "은(는) 국채의 할인율입니다."
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "국채의 수익률을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 국채의 만기일입니다."
			},
			{
				name: "pr",
				description: "은(는) 액면가 $100당 국채의 가격입니다."
			}
		]
	},
	{
		name: "TDIST",
		description: "스튜던트 t-분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 수입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포의 특성을 정하는 자유도를 나타내는 정수입니다."
			},
			{
				name: "tails",
				description: "은(는) 분포가 양측인지 단측인지 나타내는 수입니다. 단측이면 1을, 양측이면 2를 지정합니다."
			}
		]
	},
	{
		name: "TEXT",
		description: "수에 지정한 서식을 적용한 후 텍스트로 변환합니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 수, 숫자 값을 산출하는 수식 또는 숫자 값이 있는 셀의 주소입니다."
			},
			{
				name: "format_text",
				description: "은(는) [셀 서식] 대화 상자의 [표시 형식] 탭에서 [범주] 상자에 있는 표시 형식입니다."
			}
		]
	},
	{
		name: "TIME",
		description: "수치로 주어진 시간, 분, 초를 시간 형식의 엑셀 일련 번호로 변환합니다.",
		arguments: [
			{
				name: "hour",
				description: "은(는) 시간을 나타내는 0부터 23까지의 수입니다."
			},
			{
				name: "minute",
				description: "은(는) 분을 나타내는 0에서 59까지의 수입니다."
			},
			{
				name: "second",
				description: "은(는) 초를 나타내는 0에서 59까지의 수입니다."
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "텍스트 형태의 시간을 Spreadsheet 일련 번호로 표현한 시간으로 변환합니다. 값은 0(오전 12:00)부터 0.999988426(오후 11:59:59)까지입니다. 수식을 입력한 후 수를 시간 형식으로 지정하십시오.",
		arguments: [
			{
				name: "time_text",
				description: "은(는) Spreadsheet의 시간 형식 중 하나로 시간을 표시한 텍스트 문자열입니다. 문자열 내의 날짜 정보는 무시됩니다."
			}
		]
	},
	{
		name: "TINV",
		description: "스튜던트 t-분포의 양측 검증 역함수 값을 구합니다.",
		arguments: [
			{
				name: "probability",
				description: "은(는) 양측 스튜던트 t-분포를 따르는 0에서 1 사이의 확률입니다."
			},
			{
				name: "deg_freedom",
				description: "은(는) 분포를 결정짓는 자유도입니다."
			}
		]
	},
	{
		name: "TODAY",
		description: "현재 날짜를 날짜 서식으로 표시합니다.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "배열이나 범위의 행과 열을 바꿉니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 바꾸려는 배열이나 워크시트의 셀 범위입니다."
			}
		]
	},
	{
		name: "TREND",
		description: "최소 자승법을 이용하여 지정한 값들의 선형 추세를 구합니다.",
		arguments: [
			{
				name: "known_y's",
				description: "은(는) y = mx + b 식에서 이미 알고 있는 y 값의 집합입니다."
			},
			{
				name: "known_x's",
				description: "은(는) 옵션이며 y = mx + b 식에서 Known_y와 크기가 일치하는 이미 알고 있는 x 값의 집합입니다."
			},
			{
				name: "new_x's",
				description: "은(는) y 값의 추세를 계산하고자 하는 새로운 x 값의 집합입니다."
			},
			{
				name: "const",
				description: "은(는) TRUE로 설정하거나 생략하면 상수 b를 정상적으로 계산하며 FALSE로 설정하면 b를 0으로 지정하는 논리값입니다."
			}
		]
	},
	{
		name: "TRIM",
		description: "텍스트의 양 끝 공백을 없앱니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 공백을 제거할 텍스트입니다."
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "데이터 집합의 양 끝값을 제외한 부분의 평균을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) 양 끝값을 없애고 평균을 구하려는 값의 배열 또는 범위입니다."
			},
			{
				name: "percent",
				description: "은(는) 데이터 집합의 양 끝에서 제외시킬 데이터의 비율입니다."
			}
		]
	},
	{
		name: "TRUE",
		description: "논리값 TRUE를 돌려줍니다.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "지정한 자릿수만을 소수점 아래에 남기고 나머지 자리를 버립니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 지정한 자릿수 아래를 잘라 낼 숫자입니다."
			},
			{
				name: "num_digits",
				description: "은(는) 잘라낼 소수점 이하의 자릿수입니다."
			}
		]
	},
	{
		name: "TTEST",
		description: "스튜던트 t-검정에 근거한 확률을 구합니다.",
		arguments: [
			{
				name: "array1",
				description: "은(는) 첫 번째 데이터 집합입니다."
			},
			{
				name: "array2",
				description: "은(는) 두 번째 데이터 집합입니다."
			},
			{
				name: "tails",
				description: "은(는) 분포가 단측인지 양측인지를 지정하는 수입니다. 단측이면 1로, 양측이면 2로 지정합니다."
			},
			{
				name: "type",
				description: "은(는) 수행할 t-검정의 종류입니다."
			}
		]
	},
	{
		name: "TYPE",
		description: "값의 유형을 나타내는 수를 구합니다. 숫자는 1, 텍스트는 2, 논리값은 4, 오류 값은 16, 배열은 64입니다.",
		arguments: [
			{
				name: "value",
				description: "은(는) 수, 텍스트, 논리값 등 아무 값이나 지정할 수 있습니다."
			}
		]
	},
	{
		name: "UNICODE",
		description: "텍스트 첫 문자에 해당하는 숫자(코드 포인트)를 반환합니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 유니코드 값을 구하려는 문자입니다."
			}
		]
	},
	{
		name: "UPPER",
		description: "텍스트 문자열을 모두 대문자로 바꿉니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 대문자로 바꾸려는 텍스트입니다."
			}
		]
	},
	{
		name: "VALUE",
		description: "텍스트 문자열 인수를 숫자로 바꿉니다.",
		arguments: [
			{
				name: "text",
				description: "은(는) 바꾸려는 텍스트가 있는 셀의 주소 또는 따옴표로 묶인 텍스트입니다."
			}
		]
	},
	{
		name: "VAR",
		description: "표본 집단의 분산을 구합니다. 표본 집단에서 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VAR.P",
		description: "전체 모집단의 분산을 구합니다. 모집단에서 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VAR.S",
		description: "표본 집단의 분산을 구합니다. 표본 집단에서 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단 표본에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VARA",
		description: "표본 집단의 평방 편차(논리값과 텍스트 포함)를 구합니다. 논리값 FALSE는 0 값을, TRUE는 1 값을 가집니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 표본 집단 인구의 평방 편차를 구할 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 표본 집단 인구의 평방 편차를 구할 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VARP",
		description: "전체 모집단의 분산을 구합니다. 모집단에서 논리값과 텍스트는 제외됩니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "number2",
				description: "은(는) 모집단에 해당하는 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VARPA",
		description: "전체 모집단의 평방 편차(논리값과 텍스트 포함)를 구합니다. FALSE는 0 값을, TRUE는 1 값을 가집니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "은(는) 모집단의 평방 편차를 구할 인수로서 255개까지 지정할 수 있습니다."
			},
			{
				name: "value2",
				description: "은(는) 모집단의 평방 편차를 구할 인수로서 255개까지 지정할 수 있습니다."
			}
		]
	},
	{
		name: "VDB",
		description: "이중 체감법 또는 지정한 방법을 사용하여 일정 기간 동안 일부 기간을 포함한 자산의 감가 상각을 구합니다.",
		arguments: [
			{
				name: "cost",
				description: "은(는) 자산의 초기 취득가액입니다."
			},
			{
				name: "salvage",
				description: "은(는) 감가 상각 종료 시 회수되는 값(자산의 잔존가액)입니다."
			},
			{
				name: "life",
				description: "은(는) 자산이 감가 상각되는 기간 수(자산의 수명 년수)입니다."
			},
			{
				name: "start_period",
				description: "은(는) 감가 상각 계산이 시작되는 기간입니다. 수명 년수와 동일한 단위를 사용합니다."
			},
			{
				name: "end_period",
				description: "은(는) 감가 상각 계산이 끝나는 기간입니다. 수명 년수와 동일한 단위를 사용합니다."
			},
			{
				name: "factor",
				description: "은(는) 잔액이 감소되는 비율입니다. 생략하면 2(이중 체감 잔액율)를 적용합니다."
			},
			{
				name: "no_switch",
				description: "은(는) 감가 상각 금액이 이중 체감법 상각보다 클 때 정액법 감가 상각으로 전환할 것인지 지정하는 논리값입니다. FALSE로 정하거나 생략하면 전환하고, TRUE로 정하면 전환하지 않습니다."
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "배열의 첫 열에서 값을 검색하여, 지정한 열의 같은 행에서 데이터를 돌려줍니다. 기본적으로 오름차순으로 표가 정렬됩니다.",
		arguments: [
			{
				name: "lookup_value",
				description: "은(는) 표의 첫 열에서 찾으려는 값입니다. 값이나 셀 주소 또는 텍스트일 수 있습니다."
			},
			{
				name: "table_array",
				description: "은(는) 데이터를 검색하고 추출하려는 표입니다. table_array는 범위 참조나 범위 이름이 될 수 있습니다."
			},
			{
				name: "col_index_num",
				description: "은(는) table_array 내의 열 번호로, 값을 추출할 열을 지정합니다. 표의 첫 열 값은 열 1입니다."
			},
			{
				name: "range_lookup",
				description: "은(는) 정확하게 일치하는 것을 찾으려면 FALSE를, 비슷하게 일치하는 것을 찾으려면 TRUE(또는 생략)를 지정합니다."
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "일정 날짜의 요일을 나타내는 1에서 7까지의 수를 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) 날짜를 나타내는 수입니다."
			},
			{
				name: "return_type",
				description: "은(는) 결과값의 유형을 결정하는 수(1, 2 또는 3)입니다. 일요일(1)에서 토요일(7)까지의 유형은 1을, 월요일(1)에서 일요일(7)까지의 유형은 2를, 월요일(0)에서 일요일(6)까지의 유형은 3을 쓰십시오."
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "지정한 주가 일 년 중 몇째 주인지를 나타내는 숫자를 반환합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 날짜 및 시간 계산에 사용하는 날짜 시간 코드입니다."
			},
			{
				name: "return_type",
				description: "은(는) 반환 값 유형을 결정하는 숫자로 1 또는 2입니다."
			}
		]
	},
	{
		name: "WEIBULL",
		description: "와이블 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값으로 0 또는 양수이어야 합니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로 양수여야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로 양수이어야 합니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이거나 생략하면 확률 밀도 함수를 구합니다."
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "와이블 분포값을 구합니다.",
		arguments: [
			{
				name: "x",
				description: "은(는) 분포를 구하려는 값으로 0 또는 양수이어야 합니다."
			},
			{
				name: "alpha",
				description: "은(는) 분포의 매개 변수로 양수여야 합니다."
			},
			{
				name: "beta",
				description: "은(는) 분포의 매개 변수로 양수이어야 합니다."
			},
			{
				name: "cumulative",
				description: "은(는) 함수의 형태를 결정하는 논리값입니다. TRUE이면 누적 분포 함수, FALSE이거나 생략하면 확률 질량 함수를 구합니다."
			}
		]
	},
	{
		name: "WON",
		description: "통화 표시 형식을 사용하여 숫자를 텍스트로 변환합니다.",
		arguments: [
			{
				name: "number",
				description: "은(는) 수, 수를 포함하는 셀에 대한 참조 또는 수로 계산하는 수식입니다."
			},
			{
				name: "decimals",
				description: "은(는) 소수점 이하 자릿수입니다. 필요한 경우에는 반올림됩니다. 생략하면 Decimals은 2가 됩니다."
			}
		]
	},
	{
		name: "WORKDAY",
		description: "특정 일(시작 날짜)의 전이나 후의 날짜 수에서 주말이나 휴일을 제외한 날짜 수, 즉 평일 수를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "days",
				description: "은(는) start_date 전이나 후의 주말이나 휴일을 제외한 날짜 수입니다."
			},
			{
				name: "holidays",
				description: "은(는) 국경일, 공휴일, 임시 공휴일과 같이 작업 일수에서 제외되는 날짜 목록이며 선택 사항입니다."
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "사용자 지정 weekend 매개 변수를 사용하여 특정 일(시작 날짜)의 전이나 후의 날짜 수에서 주말이나 휴일을 제외한 날짜 수, 즉 평일 수를 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "days",
				description: "은(는) start_date 전이나 후의 주말이나 휴일을 제외한 날짜 수입니다."
			},
			{
				name: "weekend",
				description: "은(는) 주말을 나타내는 숫자 또는 문자열입니다."
			},
			{
				name: "holidays",
				description: "은(는) 국경일, 공휴일, 임시 공휴일과 같이 작업 일수에서 제외되는 날짜 목록이며 선택 사항입니다."
			}
		]
	},
	{
		name: "XIRR",
		description: "비정기적일 수도 있는 현금 흐름의 내부 회수율을 반환합니다.",
		arguments: [
			{
				name: "values",
				description: "은(는) 지불 일정에 대응하는 일련의 현금 흐름입니다."
			},
			{
				name: "dates",
				description: "은(는) 현금 흐름상의 불입금에 해당하는 지불 일정입니다."
			},
			{
				name: "guess",
				description: "은(는) XIRR의 결과값에 가깝다고 추측되는 수입니다."
			}
		]
	},
	{
		name: "XNPV",
		description: "비정기적일 수도 있는 현금 흐름의 순 현재 가치를 반환합니다.",
		arguments: [
			{
				name: "rate",
				description: "은(는) 현금 흐름에 적용할 할인율입니다."
			},
			{
				name: "values",
				description: "은(는) 지불 일정에 대응하는 일련의 현금 흐름입니다."
			},
			{
				name: "dates",
				description: "은(는) 현금 흐름상의 불입금에 대응하는 지불 일정입니다."
			}
		]
	},
	{
		name: "XOR",
		description: "모든 인수의 논리 '배타적 Or' 값을 구합니다.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 254개까지 지정할 수 있으며 논리값, 배열, 참조가 될 수 있습니다."
			},
			{
				name: "logical2",
				description: "은(는) TRUE 또는 FALSE 값을 가지는 조건으로 1개에서 254개까지 지정할 수 있으며 논리값, 배열, 참조가 될 수 있습니다."
			}
		]
	},
	{
		name: "YEAR",
		description: "1900에서 9999 사이의 정수로 일정 날짜의 연도를 구합니다.",
		arguments: [
			{
				name: "serial_number",
				description: "은(는) Spreadsheet에서 사용하는 날짜-시간 코드 형식의 수입니다."
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "start_date와 end_date 사이의 날짜 수가 일 년 중 차지하는 비율을 반환합니다.",
		arguments: [
			{
				name: "start_date",
				description: "은(는) 시작 날짜입니다."
			},
			{
				name: "end_date",
				description: "은(는) 끝 날짜입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "할인된 유가 증권의 연 수익률을 반환합니다.",
		arguments: [
			{
				name: "settlement",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 결산일입니다."
			},
			{
				name: "maturity",
				description: "은(는) 날짜 일련 번호로 표시된 유가 증권의 만기일입니다."
			},
			{
				name: "pr",
				description: "은(는) 액면가 $100당 유가 증권 가격입니다."
			},
			{
				name: "redemption",
				description: "은(는) 액면가 $100당 유가 증권의 상환액입니다."
			},
			{
				name: "basis",
				description: "은(는) 날짜 계산 기준입니다."
			}
		]
	},
	{
		name: "Z.TEST",
		description: "z-검정의 단측 P 값을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) x를 검정하기 위한 대상 데이터 배열 또는 범위입니다."
			},
			{
				name: "x",
				description: "은(는) 검정할 값입니다."
			},
			{
				name: "sigma",
				description: "은(는) 이미 알고 있는 모집단의 표준 편차입니다. 생략하면 표본 집단의 표준 편차가 사용됩니다."
			}
		]
	},
	{
		name: "ZTEST",
		description: "z-검정의 단측 P 값을 구합니다.",
		arguments: [
			{
				name: "array",
				description: "은(는) x를 검정하기 위한 대상 데이터 배열 또는 범위입니다."
			},
			{
				name: "x",
				description: "은(는) 검정할 값입니다."
			},
			{
				name: "sigma",
				description: "은(는) 이미 알고 있는 모집단의 표준 편차입니다. 생략하면 표본 집단의 표준 편차가 사용됩니다."
			}
		]
	}
];