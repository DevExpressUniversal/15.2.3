ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Retorna o valor absoluto de um número, um número sem sinal.",
		arguments: [
			{
				name: "núm",
				description: "é o número real cujo valor absoluto se deseja obter"
			}
		]
	},
	{
		name: "ACOS",
		description: "Retorna o arco cosseno de um número, em radianos no intervalo de 0 a Pi. O arco cosseno é o ângulo cujo cosseno é número.",
		arguments: [
			{
				name: "núm",
				description: "é o cosseno do ângulo desejado e deve estar entre -1 e 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Retorna o cosseno hiperbólico inverso de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real igual ou maior do que 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Retorna o arco cotangente de um número, em radianos no intervalo de 0 a Pi.",
		arguments: [
			{
				name: "número",
				description: "é o cotangente do ângulo desejado"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Retorna a hiperbólica inversa da cotangente de um número.",
		arguments: [
			{
				name: "número",
				description: "é a hiperbólica da cotangente do ângulo desejado"
			}
		]
	},
	{
		name: "AGORA",
		description: "Retorna a data e a hora atuais formatadas como data e hora.",
		arguments: [
		]
	},
	{
		name: "ALEATÓRIO",
		description: "Retorna um número aleatório maior ou igual a 0 e menor que 1 (modificado quando recalculado) distribuído uniformemente.",
		arguments: [
		]
	},
	{
		name: "ALEATÓRIOENTRE",
		description: "Retorna um número aleatório entre os números especificados.",
		arguments: [
			{
				name: "inferior",
				description: "é o menor inteiro que ALEATÓRIOENTRE retornará"
			},
			{
				name: "superior",
				description: "é o maior inteiro que ALEATÓRIOENTRE retornará"
			}
		]
	},
	{
		name: "ANO",
		description: "Retorna o ano de uma data, um número inteiro no intervalo de 1900 a 9999.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet"
			}
		]
	},
	{
		name: "ARÁBICO",
		description: "Converte um numeral romano em arábico.",
		arguments: [
			{
				name: "texto",
				description: "é o algarismo romano que você deseja converter"
			}
		]
	},
	{
		name: "ÁREAS",
		description: "Retorna o número de áreas em uma referência. Uma área é um intervalo de células contíguas ou uma única célula.",
		arguments: [
			{
				name: "ref",
				description: "é uma referência a uma célula ou intervalo de células e pode referir-se a várias áreas"
			}
		]
	},
	{
		name: "ARRED",
		description: "Arredonda um número até uma quantidade especificada de dígitos.",
		arguments: [
			{
				name: "núm",
				description: "é o número que se deseja arredondar"
			},
			{
				name: "núm_dígitos",
				description: "é o número de dígitos para o qual se deseja arredondar. Números negativos são arredondados para a esquerda da vírgula decimal e zero para o inteiro mais próximo"
			}
		]
	},
	{
		name: "ARREDMULTB",
		description: "Arredonda um número para baixo até o múltiplo ou a significância mais próxima.",
		arguments: [
			{
				name: "núm",
				description: "é o valor numérico que se deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual se deseja arredondar. 'Núm' e 'Significância' devem ser ambos positivos ou negativos"
			}
		]
	},
	{
		name: "ARREDMULTB.MAT",
		description: "Arredonda um número para baixo, para o inteiro mais próximo ou para o próximo múltiplo significativo.",
		arguments: [
			{
				name: "número",
				description: "é o valor que você deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual você deseja arredondar"
			},
			{
				name: "modo",
				description: "quando determinado e diferente de zero essa função arredondará em direção a zero"
			}
		]
	},
	{
		name: "ARREDMULTB.PRECISO",
		description: "Arredonda um número para baixo, em direção ao inteiro mais próximo ou ao múltiplo mais próximo de significância.",
		arguments: [
			{
				name: "número",
				description: " é o valor numérico que você deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual você quer arredondar. "
			}
		]
	},
	{
		name: "ARREDONDAR.PARA.BAIXO",
		description: "Arredonda um número para baixo até zero.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real que se deseja arredondado para baixo"
			},
			{
				name: "núm_dígitos",
				description: "é o número de dígitos para o qual se deseja arredondar. Números negativos são arredondados para a direita da vírgula decimal. Zero ou não especificado, para o inteiro mais próximo"
			}
		]
	},
	{
		name: "ARREDONDAR.PARA.CIMA",
		description: "Arredonda um número para cima afastando-o de zero.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real que se deseja arredondado para cima"
			},
			{
				name: "núm_dígitos",
				description: "é o número de dígitos para o qual se deseja arredondar. Números negativos são arredondados para a esquerda da vírgula decimal. Zero ou não especificado, para o inteiro mais próximo"
			}
		]
	},
	{
		name: "ARRUMAR",
		description: "Remove os espaços de uma cadeia de texto, com exceção dos espaços simples entre palavras.",
		arguments: [
			{
				name: "texto",
				description: "é o texto de onde você deseja que os espaços sejam removidos"
			}
		]
	},
	{
		name: "ASEN",
		description: "Retorna o arco seno de um número em radianos, no intervalo entre -Pi/2 e Pi/2.",
		arguments: [
			{
				name: "núm",
				description: "é o seno do ângulo que se deseja e que deve estar entre -1 e 1"
			}
		]
	},
	{
		name: "ASENH",
		description: "Retorna o seno hiperbólico inverso de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real igual ou maior do que 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Retorna o arco tangente de um número em radianos, no intervalo entre -Pi e p1/2.",
		arguments: [
			{
				name: "núm",
				description: "é a tangente do ângulo que se deseja"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Retorna o arco tangente das coordenadas x e y especificadas, em radianos entre -Pi e Pi, excluindo -Pi.",
		arguments: [
			{
				name: "núm_x",
				description: "é a coordenada x do ponto"
			},
			{
				name: "núm_y",
				description: "é a coordenada y do ponto"
			}
		]
	},
	{
		name: "ATANH",
		description: "Retorna a tangente hiperbólica inversa de um número.",
		arguments: [
			{
				name: "núm",
				description: "é um número real entre -1 e 1 (excluindo -1 e 1)"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Converte um número em texto (baht).",
		arguments: [
			{
				name: "número",
				description: "é um número que você deseja converter"
			}
		]
	},
	{
		name: "BASE",
		description: "Converte um número em uma representação de texto com determinado radix (base).",
		arguments: [
			{
				name: "número",
				description: "é o número que você deseja converter"
			},
			{
				name: "radix",
				description: "é o radix de base em que você deseja converter o número"
			},
			{
				name: "min_length",
				description: "é o comprimento mínimo da cadeia de caracteres retornada. Se omitido, os zeros à esquerda não serão adicionados"
			}
		]
	},
	{
		name: "BD",
		description: "Retorna a depreciação de um ativo para um determinado período utilizando o método de balanço de declínio fixo.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do ativo"
			},
			{
				name: "recuperação",
				description: "é o valor residual no final da vida útil do ativo"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o ativo está sendo depreciado (algumas vezes denominado a vida útil do ativo)"
			},
			{
				name: "período",
				description: "é o período cuja depreciação você deseja calcular. A unidade do período deve ser a mesma de 'Vida_útil'"
			},
			{
				name: "mês",
				description: "é o número de meses do primeiro ano. Se mês for omitido, presume-se que seja 12"
			}
		]
	},
	{
		name: "BDCONTAR",
		description: "Conta as células contendo números no campo (coluna) de registros no banco de dados que corresponde às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo de coluna entre aspas duplas ou um número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDCONTARA",
		description: "Conta as células não vazias no campo (coluna) de registros do banco de dados que atendam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDD",
		description: "Retorna a depreciação de um ativo para um determinado período utilizando o método do balanço de declínio duplo ou qualquer outro método especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do ativo"
			},
			{
				name: "recuperação",
				description: "é o valor residual no final da vida útil do ativo"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o ativo está sendo depreciado (algumas vezes denominado a vida útil do ativo)"
			},
			{
				name: "período",
				description: "é o período cuja depreciação você deseja calcular. A unidade do período deve ser a mesma de 'Vida_útil'"
			},
			{
				name: "fator",
				description: "é o índice de declínio do saldo. Se o fator for omitido, o fator adotado será 2 (método do saldo decrescente duplo)"
			}
		]
	},
	{
		name: "BDDESVPA",
		description: "Calcula o desvio padrão com base na população total de entradas selecionadas do banco de dados.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDEST",
		description: "Estima o desvio padrão com base em uma amostra de entradas selecionadas do banco de dados.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDEXTRAIR",
		description: "Extrai de um banco de dados um único registro que corresponde às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para uma condição"
			}
		]
	},
	{
		name: "BDMÁX",
		description: "Retorna o maior número no campo (coluna) de registros do banco de dados que atendam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou o banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDMÉDIA",
		description: "Calcula a média dos valores em uma coluna de uma lista ou um banco de dados que correspondam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou o banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDMÍN",
		description: "Retorna o menor número no campo (coluna) de registros do banco de dados que atendam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDMULTIPL",
		description: "Multiplica os valores no campo (coluna) de registros do banco de dados que atendam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDSOMA",
		description: "Soma os números no campo (coluna) de registros no banco de dados que atendam às condições especificadas.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDV",
		description: "Retorna a depreciação de um ativo para um período específico, incluindo o período parcial, utilizando o método de balanço decrescente duplo ou qualquer outro método especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do ativo"
			},
			{
				name: "recuperação",
				description: "é o valor residual no final da vida útil do ativo"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o ativo está sendo depreciado (algumas vezes denominado a vida útil do ativo)"
			},
			{
				name: "início_período",
				description: "é o período inicial cuja depreciação você deseja calcular. A unidade do período deve ser a mesma unidade de 'Vida_útil'."
			},
			{
				name: "final_período",
				description: "é o período final cuja depreciação você deseja calcular. A unidade do período deve ser a mesma unidade de 'Vida_útil'"
			},
			{
				name: "fator",
				description: "é o índice de declínio do saldo. Quando não especificado, é utilizado 2 (balanço decrescente duplo)"
			},
			{
				name: "sem_mudança",
				description: "alternar para depreciação em linha reta quando a depreciação é maior que o balanço decrescente = FALSO ou não especificado. Não alternar = VERDADEIRO"
			}
		]
	},
	{
		name: "BDVAREST",
		description: "Estima a variação com base em uma amostra das entradas selecionadas do banco de dados.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BDVARP",
		description: "Calcula a variação com base na população total de entradas selecionadas de banco de dados.",
		arguments: [
			{
				name: "banco_dados",
				description: "é o intervalo de células que constitui a lista ou banco de dados. Um banco de dados é uma lista de dados relacionados"
			},
			{
				name: "campo",
				description: "é o rótulo da coluna entre aspas ou o número que representa a posição da coluna na lista"
			},
			{
				name: "critérios",
				description: "é o intervalo de células que contém as condições especificadas. O intervalo inclui um rótulo de coluna e uma célula abaixo do rótulo para a condição"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Retorna a função Bessel modificada ln(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor com o qual a função será avaliada."
			},
			{
				name: "n",
				description: "é a ordem da função Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Retorna a função Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor com o qual a função será avaliada"
			},
			{
				name: "n",
				description: "é a ordem da função Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Retorna a função Bessel modificada Kn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor com o qual a função será avaliada."
			},
			{
				name: "n",
				description: "é a ordem da função"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Retorna a função Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "é o valor com o qual a função será avaliada."
			},
			{
				name: "n",
				description: "é a ordem da função"
			}
		]
	},
	{
		name: "BETA.ACUM.INV",
		description: "Retorna o inverso da função de densidade da probabilidade beta cumulativa (DISTBETA).",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição beta"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, devendo ser maior que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, devendo ser maior que 0"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Quando não especificado, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Quando não especificado, B = 1"
			}
		]
	},
	{
		name: "BINADEC",
		description: "Converte um número binário em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário que se deseja converter"
			}
		]
	},
	{
		name: "BINAHEX",
		description: "Converte um número binário em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "BINAOCT",
		description: "Converte um número binário em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o número binário que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "BITAND",
		description: "Retorna um bit 'E' de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que você deseja avaliar"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que você deseja avaliar"
			}
		]
	},
	{
		name: "BITOR",
		description: "Retorna um bit 'Ou' de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que você deseja avaliar"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que você deseja avaliar"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Retorna um bit 'Exclusivo Ou' de dois números.",
		arguments: [
			{
				name: "número1",
				description: "é a representação decimal do número binário que você deseja avaliar"
			},
			{
				name: "número2",
				description: "é a representação decimal do número binário que você deseja avaliar"
			}
		]
	},
	{
		name: "CAMPO",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "CARACT",
		description: "Retorna o caractere especificado pelo código numérico do conjunto de caracteres de seu computador.",
		arguments: [
			{
				name: "núm",
				description: "é um número entre 1 e 255 que especifica o caractere desejado"
			}
		]
	},
	{
		name: "CÉL",
		description: "Retorna informações sobre o formato, o local ou conteúdo da primeira célula, de acordo com o sentido de leitura da planilha, em uma referência.",
		arguments: [
			{
				name: "tipo_info",
				description: "é um valor de texto que especifica o tipo de informações da célula que você deseja."
			},
			{
				name: "ref",
				description: "é a célula sobre a qual você deseja obter informações"
			}
		]
	},
	{
		name: "CODIFURL",
		description: "Retorna uma cadeia de caracteres codificada de URL.",
		arguments: [
			{
				name: "text",
				description: "É uma cadeia de caracteres a ser codificada de URL"
			}
		]
	},
	{
		name: "CÓDIGO",
		description: "Retorna um código numérico para o primeiro caractere de uma cadeia de texto, no conjunto de caracteres usado por seu computador.",
		arguments: [
			{
				name: "texto",
				description: "é o texto cujo código do primeiro caractere você deseja obter"
			}
		]
	},
	{
		name: "COL",
		description: "Retorna o número da coluna de uma referência.",
		arguments: [
			{
				name: "ref",
				description: "é a célula ou células contíguas cujo número da coluna você deseja obter. Se omitido, a célula contendo a função COL será usada"
			}
		]
	},
	{
		name: "COLS",
		description: "Retorna o número de colunas contido em uma matriz ou referência.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz, fórmula matricial ou uma referência a um intervalo de células cujo número de colunas você deseja obter"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Retorna o número de combinações para um determinado número de itens.",
		arguments: [
			{
				name: "núm",
				description: "é o número total de itens"
			},
			{
				name: "núm_escolhido",
				description: "é o número de itens em cada combinação"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Retorna o número de combinações com repetições para um determinado número de itens.",
		arguments: [
			{
				name: "número",
				description: "é o número total de itens"
			},
			{
				name: "núm_escolhido",
				description: "é o número de itens em cada combinação"
			}
		]
	},
	{
		name: "COMPLEXO",
		description: "Converte coeficientes reais e imaginários em um número complexo.",
		arguments: [
			{
				name: "núm_real",
				description: "é o coeficiente real do número complexo"
			},
			{
				name: "i_núm",
				description: "é o coeficiente imaginário do número complexo"
			},
			{
				name: "sufixo",
				description: "é o sufixo para o componente imaginário do número complexo"
			}
		]
	},
	{
		name: "CONCATENAR",
		description: "Agrupa várias cadeias de texto em uma única sequência de texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "texto1",
				description: "de 1 a 255 cadeias de texto a serem agrupadas em uma única cadeia, podendo ser cadeias de texto, números ou referências a células únicas"
			},
			{
				name: "texto2",
				description: "de 1 a 255 cadeias de texto a serem agrupadas em uma única cadeia, podendo ser cadeias de texto, números ou referências a células únicas"
			}
		]
	},
	{
		name: "CONT.NÚM",
		description: "Calcula o número de células em um intervalo que contém números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 argumentos que podem conter ou referir-se a diversos tipos de dados, mas somente os números são contados"
			},
			{
				name: "valor2",
				description: "de 1 a 255 argumentos que podem conter ou referir-se a diversos tipos de dados, mas somente os números são contados"
			}
		]
	},
	{
		name: "CONT.SE",
		description: "Calcula o número de células não vazias em um intervalo que corresponde a uma determinada condição.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células no qual se deseja contar células que não estão em branco"
			},
			{
				name: "critérios",
				description: "é a condição, na forma de um número, expressão ou texto, que define quais células serão contadas"
			}
		]
	},
	{
		name: "CONT.SES",
		description: "Conta o número de células especificadas por um dado conjunto de condições ou critérios.",
		arguments: [
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que se deseja avaliar para a condição determinada"
			},
			{
				name: "critérios",
				description: "é a condição expressa como um número, uma expressão ou um texto que define quais células serão contadas"
			}
		]
	},
	{
		name: "CONT.VALORES",
		description: "Calcula o número de células em um intervalo que não estão vazias.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 argumentos que representam os valores e as células que deseja contar. Valores podem ser quaisquer tipos de informação"
			},
			{
				name: "valor2",
				description: "de 1 a 255 argumentos que representam os valores e as células que deseja contar. Valores podem ser quaisquer tipos de informação"
			}
		]
	},
	{
		name: "CONTAR.VAZIO",
		description: "Conta o número de células vazias em um intervalo de células especificado.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo a partir do qual se deseja contar as células vazias"
			}
		]
	},
	{
		name: "CONVERTER",
		description: "Converte um número de um sistema de medidas para outro.",
		arguments: [
			{
				name: "núm",
				description: "é o valor em de_unidades a ser convertido"
			},
			{
				name: "de_unidade",
				description: "é a unidade do núm"
			},
			{
				name: "para_unidade",
				description: "é a unidade do resultado"
			}
		]
	},
	{
		name: "CORREL",
		description: "Retorna o coeficiente de correlação entre dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é um intervalo de células de valores. Os valores devem ser números, nomes, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é um segundo intervalo de células de valores. Os valores devem ser números, nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "CORRESP",
		description: "Retorna a posição relativa de um item em uma matriz que corresponda a um valor específico em uma ordem específica.",
		arguments: [
			{
				name: "valor_procurado",
				description: "é o valor utilizado para encontrar o valor desejado na matriz, podendo ser um número, texto, um valor lógico ou um nome que faça referência a um destes valores"
			},
			{
				name: "matriz_procurada",
				description: "é um intervalo contíguo de células que contém valores possíveis de procura, uma matriz de valores ou uma referência a uma matriz"
			},
			{
				name: "tipo_correspondência",
				description: "é um número 1, 0 ou -1 indicando qual valor retornar."
			}
		]
	},
	{
		name: "COS",
		description: "Retorna o cosseno de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos para o qual você deseja obter o cosseno"
			}
		]
	},
	{
		name: "COSEC",
		description: "Retorna a cossecante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a cossecante"
			}
		]
	},
	{
		name: "COSECH",
		description: "Retorna a hiperbólica da cossecante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a hiperbólica da cossecante"
			}
		]
	},
	{
		name: "COSH",
		description: "Retorna o cosseno hiperbólico de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "COT",
		description: "Retorna a cotangente de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a cotangente"
			}
		]
	},
	{
		name: "COTH",
		description: "Retorna a hiperbólica da cotangente de um número.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a hiperbólica da cotangente"
			}
		]
	},
	{
		name: "COVAR",
		description: "Retorna a covariação, a média dos produtos dos desvios de cada par de pontos de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "COVARIAÇÃO.P",
		description: "Retorna a covariação da população, a média dos produtos dos desvios para cada par de pontos de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "COVARIAÇÃO.S",
		description: "Retorna a covariação da amostra, a média dos produtos dos desvios de cada par de pontos de dados em dois conjuntos de dados.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			},
			{
				name: "matriz2",
				description: "é o segundo intervalo de células de inteiros, devendo ser números, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "CRESCIMENTO",
		description: "Retorna números em uma tendência de crescimento exponencial que corresponda a pontos de dados conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = b*m^x, uma matriz ou intervalo de números positivos"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores de x que já deve ser conhecido na relação y = b*m^x, uma matriz ou intervalo de mesmo tamanho que 'Val_conhecidos_y'"
			},
			{
				name: "novos_valores_x",
				description: "são novos valores de x para os quais se deseja que CRESCIMENTO retorne valores y correspondentes"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante 'b' é calculada normalmente se Constante = VERDADEIRO; 'b' é definido como 1 se Constante = FALSO ou não especificado"
			}
		]
	},
	{
		name: "CRIT.BINOM",
		description: "Retorna o menor valor para o qual a distribuição binomial cumulativa é maior ou igual ao valor padrão.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas de Bernoulli"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o valor padrão, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "CUPDATAANT",
		description: "Retorna a última data do cupom antes da data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupom por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "CUPDATAPRÓX",
		description: "Retorna a próxima data do cupom depois da data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupom por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "CUPDIASINLIQ",
		description: "Retorna o número de dias entre o início do período do cupom e a data de liquidação.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupom por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "CUPNÚM",
		description: "Retorna o número de cupons a serem pagos entre a data de liquidação e a data de vencimento.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "frequência",
				description: "é o número de pagamentos de cupom por ano"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "CURT",
		description: "Retorna a curtose de um conjunto de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja curtose você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja curtose você deseja obter"
			}
		]
	},
	{
		name: "DATA",
		description: "Retorna o número que representa a data no código data-hora do Spreadsheet.",
		arguments: [
			{
				name: "ano",
				description: "é um número de 1900 a 9999 no Spreadsheet para Windows ou de 1904 a 9999 no Spreadsheet para Macintosh"
			},
			{
				name: "mês",
				description: "é um número de 1 a 12 que representa o mês do ano"
			},
			{
				name: "dia",
				description: "é um número de 1 a 31 que representa o dia do mês"
			}
		]
	},
	{
		name: "DATA.VALOR",
		description: "Converte uma data com formato de texto em um número que representa a data no código data-hora do Spreadsheet.",
		arguments: [
			{
				name: "texto_data",
				description: "é o texto que representa uma data no formato de data do Spreadsheet, entre 1/1/1900 (Windows) ou 1/1/1904 (Macintosh) e 31/12/9999."
			}
		]
	},
	{
		name: "DATADIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATAM",
		description: "Retorna o número de série da data que é o número indicado de meses antes ou depois da data inicial.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número serial de data que representa a data inicial"
			},
			{
				name: "meses",
				description: "é o número de meses antes ou depois de data_inicial"
			}
		]
	},
	{
		name: "DECABIN",
		description: "Converte um número decimal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o inteiro decimal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "DECAHEX",
		description: "Converte um número decimal em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o inteiro decimal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "DECAOCT",
		description: "Converte um número decimal em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o inteiro decimal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Converte uma representação de texto de um número em uma determinada base em um número decimal.",
		arguments: [
			{
				name: "número",
				description: "é o número que você deseja converter"
			},
			{
				name: "radix",
				description: "é o radix de base do número que você está convertendo"
			}
		]
	},
	{
		name: "DEF.NÚM.DEC",
		description: "Arredonda um número para o número de casas decimais especificado e retorna o resultado como texto com ou sem separadores de milhares.",
		arguments: [
			{
				name: "núm",
				description: "é o número que se deseja arrendondar e converter para texto"
			},
			{
				name: "decimais",
				description: "é o número de dígitos à direita da vírgula decimal. Quando não especificado, Decimais = 2"
			},
			{
				name: "sem_sep_milhar",
				description: "é um valor lógico: não exibir separadores de milhares no texto retornado = VERDADEIRO; exibir separadores de milhares no texto retornado = FALSO ou não especificado"
			}
		]
	},
	{
		name: "DEGRAU",
		description: "Testa se um número é maior que um valor limite.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser testado em comparação a passo"
			},
			{
				name: "passo",
				description: "é o valor limite"
			}
		]
	},
	{
		name: "DELTA",
		description: "Testa se dois números são iguais.",
		arguments: [
			{
				name: "núm1",
				description: "é o primeiro número"
			},
			{
				name: "núm2",
				description: "é o segundo número"
			}
		]
	},
	{
		name: "DESC",
		description: "Retorna a taxa de desconto de um título.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "pr",
				description: "é o preço do título por R$100 do valor nominal"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por R$100 do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "DESLOC",
		description: "Retorna uma referência a um intervalo que possui um número específico de linhas e colunas com base em uma referência especificada.",
		arguments: [
			{
				name: "ref",
				description: "é a referência em que se deseja basear o deslocamento, uma referência a uma célula ou intervalo de células adjacentes"
			},
			{
				name: "lins",
				description: "é o número de linhas, acima e abaixo, ao qual você deseja que a célula superior esquerda do resultado faça referência"
			},
			{
				name: "cols",
				description: "é o número de colunas, à esquerda ou à direita, ao qual você deseja que a célula superior esquerda do resultado faça referência"
			},
			{
				name: "altura",
				description: "é a altura, em número de linhas, na qual você deseja que o resultado se apresente, quando não especificada terá a mesma altura que 'Ref'"
			},
			{
				name: "largura",
				description: "é a largura, em número de colunas, na qual você deseja que o resultado se apresente, quando não especificada terá a mesma largura que 'Ref'"
			}
		]
	},
	{
		name: "DESLOCDIRBIT",
		description: "Retorna um número deslocados à direita por shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "é a representação decimal do número decimal que você deseja avaliar"
			},
			{
				name: "shift_amount",
				description: "é o número de bits pelos quais você deseja deslocar o Número à direita"
			}
		]
	},
	{
		name: "DESLOCESQBIT",
		description: "Retorna um número deslocado à esquerda por shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "é a representação decimal do número binário que você deseja avaliar"
			},
			{
				name: "shift_amount",
				description: "é o número de bits pelos quais você deseja deslocar o Número para a esquerda"
			}
		]
	},
	{
		name: "DESV.MÉDIO",
		description: "Retorna a média dos desvios absolutos dos pontos de dados a partir de sua média. Os argumentos podem ser números ou nomes, matrizes ou referências que contenham números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos cuja média dos desvios absolutos se deseja calcular"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos cuja média dos desvios absolutos se deseja calcular"
			}
		]
	},
	{
		name: "DESVPAD",
		description: "Estima o desvio padrão com base em uma amostra (ignora os valores lógicos e texto da amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números que correspondem a uma amostra de uma população, podendo ser números ou referências que contenham números"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números que correspondem a uma amostra de uma população, podendo ser números ou referências que contenham números"
			}
		]
	},
	{
		name: "DESVPAD.A",
		description: "Calcula o desvio padrão a partir de uma amostra (ignora os valores lógicos e o texto da amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números que correspondem a uma amostra de uma população, podendo ser números ou referências que contenham números"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números que correspondem a uma amostra de uma população, podendo ser números ou referências que contenham números"
			}
		]
	},
	{
		name: "DESVPAD.P",
		description: "Calcula o desvio padrão com base na população total determinada como argumento (ignora valores lógicos e texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números que correspondem a uma população, podendo ser números ou referências que contenham números"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números que correspondem a uma população, podendo ser números ou referências que contenham números"
			}
		]
	},
	{
		name: "DESVPADA",
		description: "Estima o desvio padrão com base em uma amostra, incluindo valores lógicos e texto. Texto e o valor lógico FALSO têm o valor 0, o valor lógico VERDADEIRO tem valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 valores que correspondem a uma amostra de uma população, podendo ser valores, nomes ou referências que contenham valores"
			},
			{
				name: "valor2",
				description: "de 1 a 255 valores que correspondem a uma amostra de uma população, podendo ser valores, nomes ou referências que contenham valores"
			}
		]
	},
	{
		name: "DESVPADP",
		description: "Calcula o desvio padrão com base na população total determinada como argumento (ignora valores lógicos e texto).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números que correspondem a uma população, podendo ser números ou referências que contenham números"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números que correspondem a uma população, podendo ser números ou referências que contenham números"
			}
		]
	},
	{
		name: "DESVPADPA",
		description: "Calcula o desvio padrão com base na população total, incluindo valores lógicos e texto. Texto e o valor lógico FALSO têm o valor 0, o valor lógico VERDADEIRO tem valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 valores que correspondem à população, podendo ser valores, nomes, matrizes ou referências que contenham valores"
			},
			{
				name: "valor2",
				description: "de 1 a 255 valores que correspondem à população, podendo ser valores, nomes, matrizes ou referências que contenham valores"
			}
		]
	},
	{
		name: "DESVQ",
		description: "Retorna a soma dos quadrados dos desvios dos pontos de dados da média de suas amostras.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos, ou uma matriz ou referência de matriz, da qual se deseja calcular DEVSQ"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos, ou uma matriz ou referência de matriz, da qual se deseja calcular DEVSQ"
			}
		]
	},
	{
		name: "DIA",
		description: "Retorna o dia do mês, um número de 1 a 31.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet"
			}
		]
	},
	{
		name: "DIA.DA.SEMANA",
		description: "Retorna um número entre 1 e 7 identificando o dia da semana,.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número que representa uma data"
			},
			{
				name: "retornar_tipo",
				description: "é um número: para Domingo=1 até Sábado=7, utilize 1; para Segunda=1 até Domingo=7, utilize 2; para Segunda=0 até Domingo=6, utilize 3"
			}
		]
	},
	{
		name: "DIAS",
		description: "Retorna o número de dias entre duas datas.",
		arguments: [
			{
				name: "data_final",
				description: "data_inicial e data_final são as duas datas entre as quais você deseja saber o número de dias"
			},
			{
				name: "data_inicial",
				description: "data_inicial e data_final são as duas datas entre as quais você deseja saber o número de dias"
			}
		]
	},
	{
		name: "DIAS360",
		description: "Retorna o número de dias entre duas datas com base em um ano de 360 dias (doze meses de 30 dias).",
		arguments: [
			{
				name: "data_inicial",
				description: "'Data_inicial' e 'Data_final' são as duas datas entre as quais você deseja saber o número de dias"
			},
			{
				name: "data_final",
				description: "'Data_inicial' e 'Data_final' são as duas datas entre as quais você deseja saber o número de dias"
			},
			{
				name: "método",
				description: "é um valor lógico que especifica o método de cálculo: U.S. (NASD) = FALSO ou não especificado; Europeu = VERDADEIRO."
			}
		]
	},
	{
		name: "DIATRABALHO",
		description: "Retorna o número de série da data antes ou depois de um número especificado de dias úteis.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número serial de data que representa a data inicial"
			},
			{
				name: "dias",
				description: "é o número de dias úteis antes ou depois da data inicial"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números seriais de datas para serem excluídos do calendário de trabalho, como feriados estaduais e federais e feriados móveis"
			}
		]
	},
	{
		name: "DIATRABALHO.INTL",
		description: "Retorna o número de série de datas anteriores ou posteriores a um número especificado de dias úteis com parâmetros de fim de semana personalizados.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número de série de datas que representa a data inicial"
			},
			{
				name: "dias",
				description: "é o número de dias úteis antes ou depois da data_inicial"
			},
			{
				name: "fimdesemana",
				description: "é um número ou cadeia de caracteres que especifica quando ocorrem os fins de semana"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números de série de datas a serem excluídos do calendário de trabalho, como feriados estaduais e federais e feriados móveis"
			}
		]
	},
	{
		name: "DIATRABALHOTOTAL",
		description: "Retorna o número de dias úteis entre duas datas.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número serial de data que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é o número serial de data que representa a data final"
			},
			{
				name: "feriados",
				description: "é uma matriz opcional de um ou mais números seriais de datas para serem excluídos do calendário de trabalho, como feriados estaduais e federais e feriados móveis"
			}
		]
	},
	{
		name: "DIATRABALHOTOTAL.INTL",
		description: "Retorna o número de dias úteis entre duas datas com parâmetros de fim de semana personalizados.",
		arguments: [
			{
				name: "data_inicial",
				description: "é um número de série de datas que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é um número de série de datas que representa a data final"
			},
			{
				name: "fimdesemana",
				description: "é um número ou cadeia de caracteres que especifica quando ocorrem os fins de semana"
			},
			{
				name: "feriados",
				description: "é um conjunto opcional de um ou mais números de série de datas a serem excluídos do calendário de trabalho, como feriados estaduais e federais e feriados móveis"
			}
		]
	},
	{
		name: "DIREITA",
		description: "Retorna o número de caracteres especificado do final de uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os caracteres que se deseja extrair"
			},
			{
				name: "núm_caract",
				description: "especifica o número de caracteres que se deseja extrair, 1 quando não especificado"
			}
		]
	},
	{
		name: "DIST.BETA",
		description: "Retorna a função de distribuição da probabilidade beta.",
		arguments: [
			{
				name: "x",
				description: "é o valor entre A e B no qual se avalia a função"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, devendo ser maior que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, devendo ser maior que 0"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de densidade da probabilidade, use FALSO"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Quando não especificado, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Quando não especificado, B = 1"
			}
		]
	},
	{
		name: "DIST.BIN.NEG",
		description: "Retorna a distribuição binomial negativa, a probabilidade de que ocorrerá Núm_f de falhas antes de Núm_s de sucessos, com probabilidade Probabilidade_s de um sucesso.",
		arguments: [
			{
				name: "núm_f",
				description: "é o número de falhas"
			},
			{
				name: "núm_s",
				description: "é o número a partir do qual se considera haver sucesso"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso, um número entre 0 e 1"
			}
		]
	},
	{
		name: "DIST.BIN.NEG.N",
		description: "Retorna a distribuição binomial negativa, a probabilidade de que ocorrerá Núm_f de falhas antes de Núm_s de sucessos, com probabilidade Probabilidade_s de um sucesso.",
		arguments: [
			{
				name: "núm_f",
				description: "é o número de falhas"
			},
			{
				name: "núm_s",
				description: "é o número a partir do qual se considera haver sucesso"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso, um número entre 0 e 1"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de probabilidade de massa, use FALSO"
			}
		]
	},
	{
		name: "DIST.F",
		description: "Retorna a distribuição (grau de diversidade) de probabilidade F (cauda esquerda) para dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico a ser retornado pela função: a função de distribuição cumulativa = VERDADEIRO, a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.F.CD",
		description: "Retorna a distribuição (grau de diversidade) de probabilidade F (cauda direita) para dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.GAMA",
		description: "Retorna a distribuição gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se deseja avaliar a distribuição, um número não negativo"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é o parâmetro da distribuição, um número positivo. Se beta = 1, DIST.GAMA retorna a distribuição gama padrão"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: retornar a função de distribuição cumulativa = VERDADEIRO, retornar a função de probabilidade de massa = FALSO ou não especificado"
			}
		]
	},
	{
		name: "DIST.HIPERGEOM",
		description: "Retorna a distribuição hipergeométrica.",
		arguments: [
			{
				name: "exemplo_s",
				description: "é o número de sucessos em uma amostra"
			},
			{
				name: "exemplo_núm",
				description: "é o tamanho da amostra"
			},
			{
				name: "população_s",
				description: "é o número de sucessos na população"
			},
			{
				name: "núm_população",
				description: "é o tamanho da população"
			}
		]
	},
	{
		name: "DIST.HIPERGEOM.N",
		description: "Retorna a distribuição hipergeométrica.",
		arguments: [
			{
				name: "exemplo_s",
				description: "é o número de sucessos em uma amostra"
			},
			{
				name: "exemplo_núm",
				description: "é o tamanho da amostra"
			},
			{
				name: "população_s",
				description: "é o número de sucessos na população"
			},
			{
				name: "núm_população",
				description: "é o tamanho da população"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de densidade da probabilidade, use FALSO"
			}
		]
	},
	{
		name: "DIST.LOGNORMAL",
		description: "Retorna a distribuição log-normal cumulativa de x, onde ln(x) é normalmente distribuído com parâmetros Média e Desv_padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número positivo"
			},
			{
				name: "média",
				description: "é a média do ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão do ln(x), um número positivo"
			}
		]
	},
	{
		name: "DIST.LOGNORMAL.N",
		description: "Retorna a distribuição log-normal de x, onde ln(x) é normalmente distribuído com parâmetros Média e Desv_padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número positivo"
			},
			{
				name: "média",
				description: "é a média do ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão do ln(x), um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de densidade da probabilidade, use FALSO"
			}
		]
	},
	{
		name: "DIST.NORM.N",
		description: "Retorna a distribuição normal da média e do desvio padrão especificados.",
		arguments: [
			{
				name: "x",
				description: "é o valor cuja distribuição você deseja obter"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de densidade da probabilidade, use FALSO"
			}
		]
	},
	{
		name: "DIST.NORMP.N",
		description: "Retorna a distribuição normal padrão (possui uma média zero e um desvio padrão 1).",
		arguments: [
			{
				name: "z",
				description: "é o valor cuja distribuição você deseja obter"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico a ser retornado pela função: a função de distribuição cumulativa = VERDADEIRO, a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.POISSON",
		description: "Retorna a distribuição Poisson.",
		arguments: [
			{
				name: "x",
				description: "é o número de eventos."
			},
			{
				name: "média",
				description: "é o valor numérico esperado, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a probabilidade Poisson, use VERDADEIRO; para a função de probabilidade de massa Poisson, use FALSO"
			}
		]
	},
	{
		name: "DIST.QUI",
		description: "Retorna a probabilidade de cauda direita da distribuição qui-quadrada.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se deseja avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.QUIQUA",
		description: "Retorna a probabilidade de cauda esquerda da distribuição qui-quadrada.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se deseja avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico a ser retornado pela função: a função de distribuição cumulativa = VERDADEIRO, a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DIST.QUIQUA.CD",
		description: "Retorna a probabilidade de cauda direita da distribuição qui-quadrada.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se deseja avaliar a distribuição, um número não negativo"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DIST.T",
		description: "Retorna a cauda esquerda da distribuição t de Student.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico em que se avalia a distribuição"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro indicando o número de graus de liberdade que caracteriza a distribuição"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de densidade da probabilidade, use FALSO"
			}
		]
	},
	{
		name: "DIST.T.BC",
		description: "Retorna a distribuição t de Student bicaudal.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico em que se avalia a distribuição"
			},
			{
				name: "graus_liberdade",
				description: "é um inteiro indicando o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "DIST.T.CD",
		description: "Retorna a distribuição t de Student de cauda direita.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico em que se avalia a distribuição"
			},
			{
				name: "graus_liberdade",
				description: "é um inteiro indicando o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "DIST.WEIBULL",
		description: "Retorna a distribuição Weibull.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de probabilidade de massa, use FALSO"
			}
		]
	},
	{
		name: "DISTBETA",
		description: "Retorna a função de densidade da probabilidade beta cumulativa.",
		arguments: [
			{
				name: "x",
				description: "é o valor entre A e B no qual se avalia a função"
			},
			{
				name: "alfa",
				description: "é um parâmetro da distribuição, devendo ser maior do que 0"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, devendo ser maior que 0"
			},
			{
				name: "A",
				description: "é um limite inferior opcional para o intervalo de x. Quando não especificado, A = 0"
			},
			{
				name: "B",
				description: "é um limite superior opcional para o intervalo de x. Quando não especificado, B = 1"
			}
		]
	},
	{
		name: "DISTEXPON",
		description: "Retorna a distribuição exponencial.",
		arguments: [
			{
				name: "x",
				description: "é o valor da função, um número não negativo"
			},
			{
				name: "lambda",
				description: "é o valor do parâmetro, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico a ser retornado pela função: a função de distribuição cumulativa = VERDADEIRO, a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DISTF",
		description: "Retorna a distribuição (grau de diversidade) de probabilidade F (cauda direita) para dois conjuntos de dados.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "DISTGAMA",
		description: "Retorna a distribuição gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se deseja avaliar a distribuição, um número não negativo"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo. Se beta = 1, DISTGAMA retorna a distribuição gama padrão"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: retornar a função de distribuição cumulativa = VERDADEIRO, retornar a função de probabilidade de massa = FALSO ou não especificado"
			}
		]
	},
	{
		name: "DISTNORM",
		description: "Retorna a distribuição cumulativa normal para um desvio padrão e média especificada.",
		arguments: [
			{
				name: "x",
				description: "é o valor cuja distribuição você deseja obter"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO. Para a função de densidade da probabilidade, use FALSO"
			}
		]
	},
	{
		name: "DISTNORMP",
		description: "Retorna a distribuição cumulativa normal padrão (possui uma média zero e um desvio padrão 1).",
		arguments: [
			{
				name: "z",
				description: "é o valor cuja distribuição você deseja obter"
			}
		]
	},
	{
		name: "DISTORÇÃO",
		description: "Retorna a distorção de uma distribuição: uma caracterização do grau de assimetria da distribuição em relação à média.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja distorção você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja distorção você deseja obter"
			}
		]
	},
	{
		name: "DISTORÇÃO.P",
		description: "Retorna a distorção de uma distribuição com base na população: uma caracterização do grau de assimetria da distribuição em torno da média.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "número1",
				description: " são de 1 a 254 números ou nomes, matrizes ou referências que contenham números para o qual você deseja a distorção da população"
			},
			{
				name: "número2",
				description: " são de 1 a 254 números ou nomes, matrizes ou referências que contenham números para o qual você deseja a distorção da população"
			}
		]
	},
	{
		name: "DISTR.BINOM",
		description: "Retorna a probabilidade da distribuição binomial do termo individual.",
		arguments: [
			{
				name: "núm_s",
				description: "é o número de tentativas bem sucedidas"
			},
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO. Para a função de probabilidade de massa, use FALSO"
			}
		]
	},
	{
		name: "DISTR.EXPON",
		description: "Retorna a distribuição exponencial.",
		arguments: [
			{
				name: "x",
				description: "é o valor da função, um número não negativo"
			},
			{
				name: "lambda",
				description: "é o valor do parâmetro, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico a ser retornado pela função: a função de distribuição cumulativa = VERDADEIRO, a função de densidade da probabilidade = FALSO"
			}
		]
	},
	{
		name: "DISTRBINOM",
		description: "Retorna a probabilidade da distribuição binomial do termo individual.",
		arguments: [
			{
				name: "núm_s",
				description: "é o número de tentativas bem-sucedidas"
			},
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO. Para a função de probabilidade de massa, use FALSO"
			}
		]
	},
	{
		name: "DISTT",
		description: "Retorna a distribuição t de Student.",
		arguments: [
			{
				name: "x",
				description: "é o valor numérico em que se avalia a distribuição"
			},
			{
				name: "graus_liberdade",
				description: "é um número inteiro indicando o número de graus de liberdade que caracteriza a distribuição"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas da distribuição a ser retornado: distribuição unicaudal = 1; distribuição bicaudal = 2"
			}
		]
	},
	{
		name: "DPD",
		description: "Retorna a depreciação em linha reta de um ativo durante um período.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do ativo"
			},
			{
				name: "recuperação",
				description: "é o valor residual no final da vida útil do ativo"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o ativo está sendo depreciado (algumas vezes denominado a vida útil do ativo)"
			}
		]
	},
	{
		name: "DURAÇÃOP",
		description: "Retorna o número de períodos exigido por um investimento para atingir um valor especificado.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período."
			},
			{
				name: "pv",
				description: "é o valor presente do investimento"
			},
			{
				name: "fv",
				description: "é o valor futuro desejado do investimento"
			}
		]
	},
	{
		name: "E",
		description: "Verifica se os argumentos são VERDADEIRO e retorna VERDADEIRO se todos os argumentos forem VERDADEIRO.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "lógico1",
				description: "de 1 a 255 condições a serem testadas que podem ter valor VERDADEIRO ou FALSO e podem ter valores lógicos, matrizes ou referências"
			},
			{
				name: "lógico2",
				description: "de 1 a 255 condições a serem testadas que podem ter valor VERDADEIRO ou FALSO e podem ter valores lógicos, matrizes ou referências"
			}
		]
	},
	{
		name: "É.NÃO.DISP",
		description: "Verifica se um valor é #N/D e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar. 'Valor' pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "É.NÃO.TEXTO",
		description: "Verifica se um valor não é texto (células vazias não são texto) e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar: uma célula, uma fórmula ou um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "ÉCÉL.VAZIA",
		description: "Verifica se uma referência está sendo feita a uma célula vazia e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é a célula ou um nome que faz referência à célula a ser testada"
			}
		]
	},
	{
		name: "ÉERRO",
		description: "Verifica se um valor é um erro (#VALOR!, #REF!, #DIV/0!, #NÚM!, #NOME? ou #NULO!) excluindo-se #N/D e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: " é o valor que se deseja testar. Valor pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "ÉERROS",
		description: "Verifica se um valor é um erro (#N/D, #VALOR!, #REF!, #DIV/0!, #NÚM!, #NOME? ou #NULO!) e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: " é o valor que se deseja testar. Valor pode se referir a uma célula, fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "EFETIVA",
		description: "Retorna a taxa de juros efetiva anual.",
		arguments: [
			{
				name: "taxa_nominal",
				description: "é a taxa de juros nominal"
			},
			{
				name: "núm_por_ano",
				description: "é o número de períodos compostos por ano"
			}
		]
	},
	{
		name: "ÉFÓRMULA",
		description: "Verifica se uma referência é uma célula que contém uma fórmula e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "referência",
				description: "é uma referência à célula que você deseja testar. A referência pode ser uma referência de célula, fórmula ou nome que se refere a uma célula"
			}
		]
	},
	{
		name: "ÉIMPAR",
		description: "Retorna VERDADEIRO se o número for ímpar.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser testado"
			}
		]
	},
	{
		name: "ÉLÓGICO",
		description: "Verifica se um valor é lógico (VERDADEIRO ou FALSO) e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar. 'Valor' pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "ENDEREÇO",
		description: "Cria uma referência de célula como texto, de acordo com números de linha e coluna específicos.",
		arguments: [
			{
				name: "núm_lin",
				description: "é o número da linha a ser utilizado na referência de célula: Núm_lin = 1 para linha 1"
			},
			{
				name: "núm_col",
				description: "é o número da coluna a ser utilizado na referência da célula. Por exemplo, Núm_col = 4 para coluna D"
			},
			{
				name: "núm_abs",
				description: "especifica o tipo de referência: absoluta = 1; linha absoluta/coluna relativa = 2; linha relativa/coluna absoluta = 3; relativa = 4"
			},
			{
				name: "a1",
				description: "é um valor lógico que especifica o estilo de referência: estilo A1 = 1 ou VERDADEIRO; estilo L1C1 = 0 ou FALSO"
			},
			{
				name: "texto_planilha",
				description: "é o texto que especifica o nome da planilha a ser utilizada como referência externa"
			}
		]
	},
	{
		name: "ÉNÚM",
		description: "Verifica se um valor é um número e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar. 'Valor' pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "EPADYX",
		description: "Retorna o erro padrão do valor-y previsto para cada x da regressão.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de pontos de dados dependentes, podendo ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é uma matriz ou intervalo de pontos de dados independentes, podendo ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "ÉPAR",
		description: "Retorna VERDADEIRO se o número for par.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser testado"
			}
		]
	},
	{
		name: "ÉPGTO",
		description: "Retorna os juros pagos durante um período específico do investimento.",
		arguments: [
			{
				name: "taxa",
				description: "taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "período",
				description: "período cujos juros você deseja obter"
			},
			{
				name: "nper",
				description: "número de períodos de pagamento em um investimento"
			},
			{
				name: "vp",
				description: "quantia total atual correspondente a uma série de pagamentos futuros"
			}
		]
	},
	{
		name: "ÉREF",
		description: "Verifica se um valor é uma referência e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar. 'Valor' pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "ESCOLHER",
		description: "Escolhe um valor a partir de uma lista de valores, com base em um número de índice.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_índice",
				description: "especifica qual o argumento de valor está selecionado, 'Núm_índice' deve estar entre 1 e 254 ou, uma fórmula ou uma referência a um número entre 1 e 254"
			},
			{
				name: "valor1",
				description: "de 1 a 254 números, referências de célula, nomes definidos, fórmulas, funções ou argumentos de texto a partir dos quais ESCOLHER seleciona um valor"
			},
			{
				name: "valor2",
				description: "de 1 a 254 números, referências de célula, nomes definidos, fórmulas, funções ou argumentos de texto a partir dos quais ESCOLHER seleciona um valor"
			}
		]
	},
	{
		name: "ESQUERDA",
		description: "Retorna o número especificado de caracteres do início de uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os caracteres que se deseja extrair"
			},
			{
				name: "núm_caract",
				description: "especifica quantos caracteres ESQUERDA deve extrair; retorna 1 quando não especificado"
			}
		]
	},
	{
		name: "ÉTEXTO",
		description: "Verifica se um valor é texto e retorna VERDADEIRO ou FALSO.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar. 'Valor' pode se referir a uma célula, a uma fórmula ou a um nome que faz referência a uma célula, fórmula ou valor"
			}
		]
	},
	{
		name: "EXATO",
		description: "Verifica se duas cadeias são exatamente iguais e retorna VERDADEIRO ou FALSO. EXATO diferencia maiúsculas de minúsculas.",
		arguments: [
			{
				name: "texto1",
				description: "é a primeira cadeia de texto"
			},
			{
				name: "texto2",
				description: "é a segunda cadeia de texto"
			}
		]
	},
	{
		name: "EXP",
		description: "Retorna 'e' elevado à potência de um determinado número.",
		arguments: [
			{
				name: "núm",
				description: "é o expoente aplicado à base 'e'. A constante 'e' é igual a 2,71828182845904, a base do logaritmo natural"
			}
		]
	},
	{
		name: "EXT.TEXTO",
		description: "Retorna os caracteres do meio de uma cadeia de texto, tendo a posição e o comprimento especificados.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de texto que contém os caracteres que se deseja extrair"
			},
			{
				name: "núm_inicial",
				description: "é a posição do primeiro caractere que se deseja extrair. O primeiro caractere do texto é 1"
			},
			{
				name: "núm_caract",
				description: "especifica o número de caracteres a serem retornados do texto"
			}
		]
	},
	{
		name: "FALSO",
		description: "Retorna o valor lógico FALSO.",
		arguments: [
		]
	},
	{
		name: "FATDUPLO",
		description: "Retorna o fatorial duplo de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o valor cujo fatorial duplo se deseja retornar"
			}
		]
	},
	{
		name: "FATORIAL",
		description: "Retorna o fatorial de um número, igual a 1*2*3*...*núm.",
		arguments: [
			{
				name: "núm",
				description: "é o número não negativo do qual se deseja o fatorial"
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
		name: "FIMMÊS",
		description: "Retorna o número serial do último dia do mês antes ou depois de um dado número de meses.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número serial de data que representa a data inicial"
			},
			{
				name: "meses",
				description: "é o número de meses antes ou depois da data inicial"
			}
		]
	},
	{
		name: "FISHER",
		description: "Retorna a transformação Fisher.",
		arguments: [
			{
				name: "x",
				description: "é um valor no qual se deseja a transformação, um número entre -1 e 1, excluindo -1 e 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Retorna o inverso da transformação Fisher: se y = FISHER(x), então FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "é o valor para o qual se deseja executar o inverso da transformação"
			}
		]
	},
	{
		name: "FÓRMULATEXTO",
		description: "Retorna uma fórmula como uma cadeia de caracteres.",
		arguments: [
			{
				name: "reference",
				description: "é uma referência a uma fórmula"
			}
		]
	},
	{
		name: "FRAÇÃOANO",
		description: "Retorna a fração do ano que representa o número de dias inteiros entre data_inicial e data_final.",
		arguments: [
			{
				name: "data_inicial",
				description: "é o número serial de data que representa a data inicial"
			},
			{
				name: "data_final",
				description: "é o número serial de data que representa a data final"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "FREQÜÊNCIA",
		description: "Calcula a frequência de ocorrência de valores em um intervalo de valores e retorna uma matriz vertical de números contendo um elemento a mais do que 'Matriz_bin'.",
		arguments: [
			{
				name: "matriz_dados",
				description: "é uma matriz de ou uma referência a um conjunto de valores cujas frequências você deseja contar (espaços em branco e textos são ignorados)"
			},
			{
				name: "matriz_bin",
				description: "é uma matriz de ou referência a intervalos nos quais você deseja agrupar os valores contidos em 'Matriz_dados'"
			}
		]
	},
	{
		name: "FUNERRO",
		description: "Retorna a função de erro.",
		arguments: [
			{
				name: "limite_inferior",
				description: "é o limite inferior na integração de FUNERRO"
			},
			{
				name: "limite_superior",
				description: "é o limite superior na integração de FUNERRO"
			}
		]
	},
	{
		name: "FUNERRO.PRECISO",
		description: "Retorna a função de erro.",
		arguments: [
			{
				name: "X",
				description: "é o limite inferior para a integração de FUNERRO.PRECISO"
			}
		]
	},
	{
		name: "FUNERROCOMPL",
		description: "Retorna a função de erro complementar.",
		arguments: [
			{
				name: "x",
				description: "é o limite inferior na integração de FUNERRO"
			}
		]
	},
	{
		name: "FUNERROCOMPL.PRECISO",
		description: "Retorna a função de erro complementar.",
		arguments: [
			{
				name: "x",
				description: "é o limite inferior para a integração de FUNERROCOMPL.PRECISO"
			}
		]
	},
	{
		name: "GAMA",
		description: "Retorna o valor da função Gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual você deseja calcular a Gama"
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
		name: "GRAUS",
		description: "Converte radianos em graus.",
		arguments: [
			{
				name: "ângulo",
				description: "é o ângulo em radianos que se deseja converter"
			}
		]
	},
	{
		name: "HEXABIN",
		description: "Converte um número hexadecimal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "HEXADEC",
		description: "Converte um número hexadecimal em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal que se deseja converter"
			}
		]
	},
	{
		name: "HEXAOCT",
		description: "Converte um número hexadecimal em octal.",
		arguments: [
			{
				name: "núm",
				description: "é o número hexadecimal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "HIPERLINK",
		description: "Cria um atalho ou salto que abre um documento armazenado no disco rígido, em um servidor de rede ou na Internet.",
		arguments: [
			{
				name: "local_vínculo",
				description: "é o texto que informa o caminho e nome de arquivo do documento a ser aberto, um local no disco rígido, um endereço UNC ou um caminho de URL"
			},
			{
				name: "nome_amigável",
				description: "é o texto ou um número que é exibido em uma célula. Quando não especificado, a célula exibe o texto 'Local_vínculo'"
			}
		]
	},
	{
		name: "HOJE",
		description: "Retorna a data de hoje formatada como uma data.",
		arguments: [
		]
	},
	{
		name: "HORA",
		description: "Retorna a hora como um número de 0 (12:00 AM) a 23 (11:00 PM).",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet ou um texto no formato de hora como, por exemplo, 16:48:00 ou 4:48:00 PM"
			}
		]
	},
	{
		name: "IMABS",
		description: "Retorna o valor absoluto (módulo) de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo valor absoluto se deseja obter"
			}
		]
	},
	{
		name: "IMAGINÁRIO",
		description: "Retorna o coeficiente imaginário de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo coeficiente imaginário se deseja obter"
			}
		]
	},
	{
		name: "IMARG",
		description: "Retorna o argumento q, um ângulo expresso em radianos.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo argumento se deseja obter"
			}
		]
	},
	{
		name: "IMCONJ",
		description: "Retorna o conjugado complexo de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo conjugado se deseja obter"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Retorna o cosseno de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo cosseno se deseja obter"
			}
		]
	},
	{
		name: "IMCOSEC",
		description: "Retorna a cossecante de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a cossecante"
			}
		]
	},
	{
		name: "IMCOSECH",
		description: "Retorna a hiperbólica da cossecante de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a hiperbólica da cossecante"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Retorna o cosseno hiperbólico de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo cujo cosseno hiperbólico você deseja"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Retorna a cotangente de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a cotangente"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Retorna o quociente de dois números complexos.",
		arguments: [
			{
				name: "inúm1",
				description: "é o numerador ou dividendo complexo"
			},
			{
				name: "inúm2",
				description: "é o denominador ou divisor complexo"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Retorna o exponencial de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo exponencial se deseja obter"
			}
		]
	},
	{
		name: "IMLN",
		description: "Retorna o logaritmo natural de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo logaritmo natural se deseja obter"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Retorna o logaritmo de base-10 de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo logaritmo de base-10 se deseja obter"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Retorna o logaritmo de base-2 de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo logaritmo de base-2 se deseja obter"
			}
		]
	},
	{
		name: "ÍMPAR",
		description: "Arredonda um número positivo para cima e um número negativo para baixo até o número ímpar inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser arredondado"
			}
		]
	},
	{
		name: "IMPOT",
		description: "Retorna um número complexo elevado a uma potência inteira.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo que se deseja elevar a uma potência"
			},
			{
				name: "núm",
				description: "é a potência para a qual se deseja elevar o número complexo"
			}
		]
	},
	{
		name: "IMPROD",
		description: "Retorna o produto de 1 a 255 números complexos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúm1",
				description: "Inúm1, Inúm2,... são de 1 a 255 números complexos para multiplicar."
			},
			{
				name: "inúm2",
				description: "Inúm1, Inúm2,... são de 1 a 255 números complexos para multiplicar."
			}
		]
	},
	{
		name: "IMRAIZ",
		description: "Retorna a raiz quadrada de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cuja raiz quadrada se deseja obter"
			}
		]
	},
	{
		name: "IMREAL",
		description: "Retorna o coeficiente real de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo coeficiente real se deseja obter"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Retorna a secante de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a secante"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Retorna a hiperbólica da secante de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a hiperbólica da secante"
			}
		]
	},
	{
		name: "IMSENH",
		description: "Retorna o seno hiperbólico de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo do qual você deseja obter o seno hiperbólico"
			}
		]
	},
	{
		name: "IMSENO",
		description: "Retorna o seno de um número complexo.",
		arguments: [
			{
				name: "inúm",
				description: "é um número complexo cujo seno se deseja obter"
			}
		]
	},
	{
		name: "IMSOMA",
		description: "Retorna a soma dos números complexos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inúm1",
				description: "são de 1 a 255 números complexos para adicionar"
			},
			{
				name: "inúm2",
				description: "são de 1 a 255 números complexos para adicionar"
			}
		]
	},
	{
		name: "IMSUBTR",
		description: "Retorna a diferença de dois números complexos.",
		arguments: [
			{
				name: "inúm1",
				description: "é o número complexo do qual inúm2 será subtraído"
			},
			{
				name: "inúm2",
				description: "é o número complexo a ser subtraído de inúm1"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Retorna a tangente de um número complexo.",
		arguments: [
			{
				name: "inúmero",
				description: "é um número complexo para o qual você deseja a tangente"
			}
		]
	},
	{
		name: "INCLINAÇÃO",
		description: "Retorna a inclinação da reta de regressão linear para os pontos de dados determinados.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de pontos de dados dependentes, podendo ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é uma matriz ou intervalo de pontos de dados independentes, podendo ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "ÍNDICE",
		description: "Retorna um valor ou a referência da célula na interseção de uma linha e coluna específica, em um dado intervalo.",
		arguments: [
			{
				name: "matriz",
				description: "é um intervalo de células ou uma constante de matriz"
			},
			{
				name: "núm_linha",
				description: "seleciona a linha na matriz ou referência de onde um valor será retornado. Quando não especificado, núm_coluna é necessário"
			},
			{
				name: "núm_coluna",
				description: "seleciona a coluna na matriz ou referência de onde um valor será retornado. Quando não especificado, núm_linha é necessário"
			}
		]
	},
	{
		name: "INDIRETO",
		description: "Retorna uma referência indicada por um valor de texto.",
		arguments: [
			{
				name: "texto_ref",
				description: "é uma referência a uma célula que contém uma referência de estilo A1 ou L1C1, um nome definido como uma referência ou uma referência a uma célula como uma cadeia de texto"
			},
			{
				name: "a1",
				description: "é um valor lógico que especifica o tipo de referência contido em texto_ref; estilo L1C1 = FALSO; estilo A1 = VERDADEIRO ou não especificado"
			}
		]
	},
	{
		name: "INFODADOSTABELADINÂMICA",
		description: "Extrai os dados armazenados em uma tabela dinâmica.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "campo_dados",
				description: "é o nome do campo de dados do qual se deseja extrair dados"
			},
			{
				name: "tab_din",
				description: "é uma referência a uma célula ou a um intervalo de células da tabela dinâmica que contém os dados que você deseja recuperar"
			},
			{
				name: "campo",
				description: "campo ao qual se deseja fazer referência"
			},
			{
				name: "item",
				description: "item de campo ao qual se deseja fazer referência"
			}
		]
	},
	{
		name: "INFORMAÇÃO",
		description: "Retorna informações sobre o ambiente operacional atual.",
		arguments: [
			{
				name: "tipo_texto",
				description: "é o texto que especifica qual tipo de informação você deseja que seja retornado."
			}
		]
	},
	{
		name: "INT",
		description: "Arredonda um número para baixo até o número inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o número real que se deseja arredondar para baixo até um inteiro"
			}
		]
	},
	{
		name: "INT.CONFIANÇA",
		description: "Retorna o intervalo de confiança para uma média da população.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da população para o intervalo de dados, e presume-se conhecido. Desv_padrão deve ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INT.CONFIANÇA.NORM",
		description: "Retorna o intervalo de confiança para uma média da população, usando uma distribuição normal.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da população para o intervalo de dados, e presume-se conhecido. Desv_padrão deve ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INT.CONFIANÇA.T",
		description: "Retorna o intervalo de confiança para uma média da população, usando uma distribuição T de Student.",
		arguments: [
			{
				name: "alfa",
				description: "é o nível de significância utilizado para calcular o nível de confiança, um número maior que 0 e menor que 1"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da população para o intervalo de dados, e presume-se conhecido. Desv_padrão deve ser maior que 0"
			},
			{
				name: "tamanho",
				description: "é o tamanho da amostra"
			}
		]
	},
	{
		name: "INTERCEPÇÃO",
		description: "Calcula o ponto em que uma linha interceptará o eixo y usando uma linha de regressão de melhor ajuste plotada através dos valores de x e valores de y conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto dependente de observações ou dados, podendo ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é o conjunto independente de observações ou dados, podendo ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "INTERV.DISTR.BINOM",
		description: "Retorna a probabilidade de um resultado de teste usando uma distribuição binomial.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas independentes"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa"
			},
			{
				name: "número_s",
				description: "é o número de tentativas bem-sucedidas"
			},
			{
				name: "número_s2",
				description: "se fornecido, esta função retorna a probabilidade de que o número de tentativas bem-sucedidas deverá estar entre núm_s e number_s2"
			}
		]
	},
	{
		name: "INV.BETA",
		description: "Retorna o inverso da função de densidade da probabilidade beta cumulativa (DIST.BETA).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade associada à distribuição beta"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição que deve ser maior que 0"
			},
			{
				name: "beta",
				description: "é o parâmetro da distribuição que deve ser maior que 0"
			},
			{
				name: "A",
				description: "é o limite inferior opcional para o intervalo de x. Quando não especificado, A = 0"
			},
			{
				name: "B",
				description: "é o limite superior opcional para o intervalo de x. Quando não especificado, B = 1"
			}
		]
	},
	{
		name: "INV.BINOM",
		description: "Retorna o menor valor para o qual a distribuição binomial cumulativa é maior ou igual ao valor padrão.",
		arguments: [
			{
				name: "tentativas",
				description: "é o número de tentativas de Bernoulli"
			},
			{
				name: "probabilidade_s",
				description: "é a probabilidade de sucesso em cada tentativa, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o valor padrão, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.F",
		description: "Retorna o inverso da distribuição de probabilidade F (cauda esquerda): se p = DISTF(x,...), então INV.F(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "é a probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "deg_freedom1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "deg_freedom2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.F.CD",
		description: "Retorna o inverso da distribuição de probabilidade F (cauda direita): se p = DIST.F.CD(x,...), então INV.F.CD(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.GAMA",
		description: "Retorna o inverso da distribuição cumulativa gama: se p = DIST.GAMA(x,...), então INV.GAMA(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição gama, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é o parâmetro para a distribuição, um número positivo. Se beta = 1, INV.GAMA retorna o inverso da distribuição gama padrão"
			}
		]
	},
	{
		name: "INV.LOGNORMAL",
		description: "Retorna o inverso da função de distribuição cumulativa log-normal de x, onde ln(x) é normalmente distribuído com parâmetros Média e Desv_padrão.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição log-normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média do ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão do ln(x), um número positivo"
			}
		]
	},
	{
		name: "INV.NORM",
		description: "Retorna o inverso da distribuição cumulativa normal para a média e o desvio padrão especificados.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "INV.NORM.N",
		description: "Retorna o inverso da distribuição cumulativa normal para a média e o desvio padrão especificados.",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "INV.NORMP",
		description: "Retorna o inverso da distribuição cumulativa normal padrão (possui uma média zero e um desvio padrão 1).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.NORMP.N",
		description: "Retorna o inverso da distribuição cumulativa normal padrão (possui uma média zero e um desvio padrão 1).",
		arguments: [
			{
				name: "probabilidade",
				description: "é uma probabilidade correspondente à distribuição normal, um número entre 0 e 1 inclusive"
			}
		]
	},
	{
		name: "INV.QUI",
		description: "Retorna o inverso da probabilidade de cauda direita da distribuição qui-quadrada.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição qui-quadrada, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.QUIQUA",
		description: "Retorna o inverso da probabilidade de cauda esquerda da distribuição qui-quadrada.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição qui-quadrada, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.QUIQUA.CD",
		description: "Retorna o inverso da probabilidade de cauda direita da distribuição qui-quadrada.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição qui-quadrada, um valor entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é o número de graus de liberdade, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INV.T",
		description: "Retorna o inverso de cauda esquerda da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um inteiro positivo que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "INV.T.BC",
		description: "Retorna o inverso bicaudal da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um inteiro positivo que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "INVF",
		description: "Retorna o inverso da distribuição de probabilidades F (cauda direita): se p = DISTF(x,...), então INVF(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição cumulativa F, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade1",
				description: "é o grau de liberdade do numerador, um número entre 1 e 10^10, excluindo 10^10"
			},
			{
				name: "graus_liberdade2",
				description: "é o grau de liberdade do denominador, um número entre 1 e 10^10, excluindo 10^10"
			}
		]
	},
	{
		name: "INVGAMA",
		description: "Retorna o inverso da distribuição cumulativa gama: se p = DISTGAMA(x,...), então INVGAMA(p,...) = x.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição gama, um número entre 0 e 1 inclusive"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é um parâmetro da distribuição, um número positivo. Se beta = 1, INVGAMA retorna o inverso da distribuição gama padrão"
			}
		]
	},
	{
		name: "INVLOG",
		description: "Retorna o inverso da função de distribuição log-normal cumulativa de x, onde ln(x) é normalmente distribuído com parâmetros Média e Dev_padrão.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição log-normal, um número entre 0 e 1 inclusive"
			},
			{
				name: "média",
				description: "é a média do ln(x)"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão do ln(x), um número positivo"
			}
		]
	},
	{
		name: "INVT",
		description: "Retorna o inverso bicaudal da distribuição t de Student.",
		arguments: [
			{
				name: "probabilidade",
				description: "é a probabilidade associada à distribuição t de Student bicaudal, um número entre 0 e 1 inclusive"
			},
			{
				name: "graus_liberdade",
				description: "é um inteiro positivo que indica o número de graus de liberdade que caracteriza a distribuição"
			}
		]
	},
	{
		name: "IPGTO",
		description: "Retorna o pagamento dos juros de um investimento durante um determinado período, com base nos pagamentos constantes, periódicos e na taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "período",
				description: "é o período cujos juros se deseja saber e deve estar no intervalo entre 1 e nper"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento em um investimento"
			},
			{
				name: "vp",
				description: "é o valor presente ou a quantia total atual correspondente a uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou um saldo em dinheiro que se deseja obter após o último pagamento ter sido efetuado. Quando não especificado, Vf = 0"
			},
			{
				name: "tipo",
				description: "é um valor lógico que representa o vencimento do pagamento: ao final do período = 0 ou não especificado; no início do período = 1"
			}
		]
	},
	{
		name: "ISO.TETO",
		description: "Arredonda um número para cima até o inteiro ou o múltiplo da significância mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor que você deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo opcional ao qual você deseja arredondar"
			}
		]
	},
	{
		name: "JUROSACUMV",
		description: "Retorna os juros acumulados de um título que paga juros no vencimento.",
		arguments: [
			{
				name: "emissão",
				description: "é a data de emissão do título, expressa como um número serial de data"
			},
			{
				name: "liquidação",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "taxa",
				description: "é a taxa de cupom anual do título"
			},
			{
				name: "valor_nominal",
				description: "é o valor de paridade do título"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "LIN",
		description: "Retorna o número da linha de uma referência.",
		arguments: [
			{
				name: "ref",
				description: "é a célula ou intervalo de célula cujo número da linha você deseja obter. Quando não especificado, retorna a célula que contém a função LIN"
			}
		]
	},
	{
		name: "LINS",
		description: "Retorna o número de linhas em uma referência ou matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz, uma fórmula matricial ou uma referência a um intervalo de células cujo número de linhas você deseja obter"
			}
		]
	},
	{
		name: "LN",
		description: "Retorna o logaritmo natural de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo natural você deseja obter"
			}
		]
	},
	{
		name: "LNGAMA",
		description: "Retorna o logaritmo natural da função gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor para o qual você deseja calcular LNGAMA, um número positivo"
			}
		]
	},
	{
		name: "LNGAMA.PRECISO",
		description: "Retorna o logaritmo natural da função gama.",
		arguments: [
			{
				name: "x",
				description: "é o valor pelo qual você deseja calcular LNGAMA.PRECISO, um número positivo"
			}
		]
	},
	{
		name: "LOCALIZAR",
		description: "Retorna o número do caractere no qual um caractere ou uma cadeia de texto foram localizados, sendo a leitura feita da esquerda para a direita (não distingue maiúsculas de minúsculas).",
		arguments: [
			{
				name: "texto_procurado",
				description: "é o texto que se deseja localizar. Você pode usar os caracteres curinga '?' e '*'. Use '~?' e '~*' para localizar os caracteres '?' e '*'"
			},
			{
				name: "no_texto",
				description: "é o texto no qual se deseja localizar 'Texto_procurado'"
			},
			{
				name: "núm_inicial",
				description: "é o número do caractere em 'No_texto', a partir da esquerda, no qual se deseja iniciar a pesquisa. Quando não especificado, é usado 1"
			}
		]
	},
	{
		name: "LOG",
		description: "Retorna o logaritmo de um número em uma base especificada.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo você deseja"
			},
			{
				name: "base",
				description: "é a base do logaritmo; 10 quando não especificado"
			}
		]
	},
	{
		name: "LOG10",
		description: "Retorna o logaritmo de base 10 de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número real positivo cujo logaritmo de base 10 você deseja obter"
			}
		]
	},
	{
		name: "LUCRODESC",
		description: "Retorna o rendimento anual de um título com deságio. Por exemplo, uma letra do tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "pr",
				description: "é o preço do título por R$100 do valor nominal"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por R$100 do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "MAIOR",
		description: "Retorna o maior valor k-ésimo de um conjunto de dados. Por exemplo, o quinto maior número.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados cujo maior valor k-ésimo você deseja determinar"
			},
			{
				name: "k",
				description: "é a posição (começando do maior) na matriz ou intervalo de células do valor a ser retornado"
			}
		]
	},
	{
		name: "MAIÚSCULA",
		description: "Converte a cadeia de texto em maiúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que se deseja converter em maiúsculas, uma referência ou uma cadeia de texto"
			}
		]
	},
	{
		name: "MARRED",
		description: "Retorna um número arredondado para o múltiplo desejado.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser arredondado"
			},
			{
				name: "múltiplo",
				description: "é o múltiplo para o qual se deseja arredondar núm"
			}
		]
	},
	{
		name: "MATRIZ.DETERM",
		description: "Retorna o determinante de uma matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz numérica com um mesmo número de linhas e de colunas, um intervalo de células ou uma constante de matriz"
			}
		]
	},
	{
		name: "MATRIZ.INVERSO",
		description: "Retorna a matriz inversa de uma matriz.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz numérica com um mesmo número de linhas e de colunas, um intervalo de células ou constante de matriz"
			}
		]
	},
	{
		name: "MATRIZ.MULT",
		description: "Retorna a matriz produto de duas matrizes, uma matriz com o mesmo número de linhas que a matriz1 e de colunas que a matriz2.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz de números a serem multiplicados e deve possuir o mesmo número de colunas que a Matriz2 possui de linhas"
			},
			{
				name: "matriz2",
				description: "é a primeira matriz de números a serem multiplicados e deve possuir o mesmo número de colunas que a Matriz2 possui de linhas"
			}
		]
	},
	{
		name: "MÁXIMO",
		description: "Retorna o valor máximo de um conjunto de argumentos. Valores lógicos e texto são ignorados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em forma de texto cujo valor máximo você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em forma de texto cujo valor máximo você deseja obter"
			}
		]
	},
	{
		name: "MÁXIMOA",
		description: "Retorna o valor máximo contido em um conjunto de valores. Não ignora valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em formato de texto cujo valor máximo você deseja obter"
			},
			{
				name: "valor2",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em formato de texto cujo valor máximo você deseja obter"
			}
		]
	},
	{
		name: "MDC",
		description: "Retorna o máximo divisor comum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 valores"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 valores"
			}
		]
	},
	{
		name: "MED",
		description: "Retorna a mediana, ou o número central de um determinado conjunto de números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contêm números cuja mediana você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contêm números cuja mediana você deseja obter"
			}
		]
	},
	{
		name: "MÉDIA",
		description: "Retorna a média (aritmética) dos argumentos que podem ser números ou nomes, matrizes ou referências que contêm números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 255 argumentos numéricos cuja média se deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 255 argumentos numéricos cuja média se deseja obter"
			}
		]
	},
	{
		name: "MÉDIA.GEOMÉTRICA",
		description: "Retorna a média geométrica de uma matriz ou um intervalo de dados numéricos positivos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja média você deseja calcular"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja média você deseja calcular"
			}
		]
	},
	{
		name: "MÉDIA.HARMÔNICA",
		description: "Retorna a média harmônica de um conjunto de dados de números positivos: o recíproco da média aritmética de recíprocos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja média harmônica você deseja calcular"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cuja média harmônica você deseja calcular"
			}
		]
	},
	{
		name: "MÉDIA.INTERNA",
		description: "Retorna a média da parte interior de um conjunto de valores de dados.",
		arguments: [
			{
				name: "matriz",
				description: "é o intervalo ou matriz de valores a se calcular a média desprezando os desvios"
			},
			{
				name: "porcentagem",
				description: "é o número fracionário de ponto de dados a ser excluído do início e fim do conjunto de dados"
			}
		]
	},
	{
		name: "MÉDIAA",
		description: "Retorna a média aritmética dos argumentos, avaliando texto e FALSO em argumentos como 0, VERDADEIRO é avaliado como 1. Os argumentos podem ser números, nomes, matrizes ou referências.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 argumentos cuja média você deseja calcular"
			},
			{
				name: "valor2",
				description: "de 1 a 255 argumentos cuja média você deseja calcular"
			}
		]
	},
	{
		name: "MÉDIASE",
		description: "Descobre a média aritmética das células especificadas por uma dada condição ou determinados critérios.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células que se deseja avaliar"
			},
			{
				name: "critérios",
				description: "é a condição ou os critérios expressos como um número, uma expressão ou um texto que define quais células serão usadas para calcular a média"
			},
			{
				name: "intervalo_média",
				description: "são as células que serão realmente usadas para calcular a média. Se omitido, serão usadas as células no intervalo"
			}
		]
	},
	{
		name: "MÉDIASES",
		description: "Descobre a média aritmética das células especificadas por um dado conjunto de condições ou critérios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "intervalo_média",
				description: "são as células que serão realmente usadas para descobrir a média."
			},
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que se deseja avaliar para a condição dada"
			},
			{
				name: "critérios",
				description: "é a condição ou os critérios expressos como um número, uma expressão ou um texto que define quais células serão usadas para calcular a média"
			}
		]
	},
	{
		name: "MENOR",
		description: "Retorna o menor valor k-ésimo do conjunto de dados. Por exemplo, o quinto menor número.",
		arguments: [
			{
				name: "matriz",
				description: "é uma matriz ou intervalo de dados numéricos cujo menor valor k-ésimo você deseja determinar"
			},
			{
				name: "k",
				description: "é a posição (começando do menor) na matriz ou intervalo do valor a ser retornado"
			}
		]
	},
	{
		name: "MÊS",
		description: "Retorna o mês, um número entre 1 (janeiro) e 12 (dezembro).",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet"
			}
		]
	},
	{
		name: "MÍNIMO",
		description: "Retorna o valor mínimo contido em um conjunto de valores. Texto e valores lógicos são ignorados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em forma de texto cujo valor mínimo você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em forma de texto cujo valor mínimo você deseja obter"
			}
		]
	},
	{
		name: "MÍNIMOA",
		description: "Retorna o valor mínimo contido em um conjunto de valores. Não ignora valores lógicos e texto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em formato de texto cujo valor mínimo você deseja obter"
			},
			{
				name: "valor2",
				description: "de 1 a 255 números, células vazias, valores lógicos ou números em formato de texto cujo valor mínimo você deseja obter"
			}
		]
	},
	{
		name: "MINÚSCULA",
		description: "Converte todas as letras em uma cadeia de texto em minúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que se deseja converter em minúsculas. Os caracteres do texto que não são letras não serão alterados"
			}
		]
	},
	{
		name: "MINUTO",
		description: "Retorna o minuto, um número de 0 a 59.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet ou um texto no formato de hora como, por exemplo, 16:48:00 ou 4:48:00 PM"
			}
		]
	},
	{
		name: "MMC",
		description: "Retorna o mínimo múltiplo comum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 valores cujos mínimos múltiplos comum se deseja obter"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 valores cujos mínimos múltiplos comum se deseja obter"
			}
		]
	},
	{
		name: "MOD",
		description: "Retorna o resto da divisão após um número ter sido dividido por um divisor.",
		arguments: [
			{
				name: "núm",
				description: "é o número cujo resto da divisão você deseja localizar"
			},
			{
				name: "divisor",
				description: "é o número pelo qual você deseja dividir 'Núm'"
			}
		]
	},
	{
		name: "MODO",
		description: "Retorna o valor mais repetitivo ou que ocorre com maior frequência, em uma matriz ou um intervalo de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseje obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseje obter"
			}
		]
	},
	{
		name: "MODO.MULT",
		description: "Retorna uma matriz vertical dos valores mais repetidos, ou que ocorrem com maior frequência, em uma matriz ou um intervalo de dados. Para a matriz horizontal, use =TRANSPOR(MODO.MULT(número1,número2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseja obter"
			}
		]
	},
	{
		name: "MODO.ÚNICO",
		description: "Retorna o valor mais repetido, ou que ocorre com maior frequência, em uma matriz ou um intervalo de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseja obter"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números ou nomes, matrizes ou referências que contenham números cujo modo você deseja obter"
			}
		]
	},
	{
		name: "MOEDA",
		description: "Converte um número em texto, utilizando o formato de moeda.",
		arguments: [
			{
				name: "núm",
				description: "é um número, uma referência a uma célula contendo um número, ou uma fórmula que gera um número"
			},
			{
				name: "decimais",
				description: "é o número de dígitos à direita da vírgula decimal. O número é arredondado conforme necessário; caso não seja especificado, Decimais = 2"
			}
		]
	},
	{
		name: "MOEDADEC",
		description: "Converte um preço em moeda, expresso como uma fração, em um preço em moeda, expresso como um número decimal.",
		arguments: [
			{
				name: "moeda_fracionária",
				description: "é um número expresso como uma fração"
			},
			{
				name: "fração",
				description: "é o inteiro a ser utilizado no denominador da fração"
			}
		]
	},
	{
		name: "MOEDAFRA",
		description: "Converte um preço em moeda, expresso como um número decimal, em um preço em moeda, expresso como uma fração.",
		arguments: [
			{
				name: "moeda_decimal",
				description: "é um número decimal"
			},
			{
				name: "fração",
				description: "é o inteiro a ser utilizado no denominador da fração"
			}
		]
	},
	{
		name: "MTIR",
		description: "Retorna a taxa interna de retorno para uma série de fluxos de caixa periódicos, considerando o custo de investimento e os juros de reinvestimento de caixa.",
		arguments: [
			{
				name: "valores",
				description: "é uma matriz ou referência a células que contêm números que representam uma série de pagamentos (negativa) e renda (positiva) em períodos regulares"
			},
			{
				name: "taxa_financ",
				description: "é a taxa de juros paga sobre o dinheiro utilizado em fluxos de caixa"
			},
			{
				name: "taxa_reinvest",
				description: "é a taxa de juros recebida sobre os fluxos de caixa à medida que estes forem sendo reinvestidos"
			}
		]
	},
	{
		name: "MUDAR",
		description: "Substitui parte de uma cadeia de texto por uma cadeia diferente.",
		arguments: [
			{
				name: "texto_antigo",
				description: "é o texto no qual se deseja substituir alguns caracteres"
			},
			{
				name: "núm_inicial",
				description: "é a posição do caractere em 'Texto_antigo' que se deseja substituir por 'Novo_texto'"
			},
			{
				name: "núm_caract",
				description: "é o número de caracteres em 'Texto_antigo' que se deseja substituir"
			},
			{
				name: "novo_texto",
				description: "é o texto que substituirá caracteres em 'Texto_antigo'"
			}
		]
	},
	{
		name: "MULT",
		description: "Multiplica todos os números dados como argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números, valores lógicos ou representações de número em forma de texto que você deseja multiplicar"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números, valores lógicos ou representações de número em forma de texto que você deseja multiplicar"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Retorna o multinômio de um conjunto de números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "são de 1 a 255 valores cujos multinômios se deseja obter"
			},
			{
				name: "núm2",
				description: "são de 1 a 255 valores cujos multinômios se deseja obter"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Retorna a matriz de unidade para a dimensão especificada.",
		arguments: [
			{
				name: "dimensão",
				description: "é um inteiro que especifica a dimensão da matriz da unidade que você deseja retornar"
			}
		]
	},
	{
		name: "N",
		description: "Converte um valor não numérico em um número, datas em números de série, VERDADEIRO em 1 e qualquer outro valor em 0 (zero).",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja converter"
			}
		]
	},
	{
		name: "NÃO",
		description: "Inverte FALSO para VERDADEIRO, ou VERDADEIRO para FALSO.",
		arguments: [
			{
				name: "lógico",
				description: "é o valor ou expressão que podem ser avaliados como VERDADEIRO ou FALSO"
			}
		]
	},
	{
		name: "NÃO.DISP",
		description: "Retorna o valor de erro #N/D (valor não disponível).",
		arguments: [
		]
	},
	{
		name: "NOMINAL",
		description: "Retorna a taxa de juros nominal anual.",
		arguments: [
			{
				name: "taxa_efetiva",
				description: "é a taxa de juros efetiva"
			},
			{
				name: "núm_por_ano",
				description: "é o número de períodos compostos por ano"
			}
		]
	},
	{
		name: "NPER",
		description: "Retorna o número de períodos de um investimento com base em pagamentos constantes periódicos e uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado a cada período; ele não pode ser alterado no decorrer do investimento"
			},
			{
				name: "vp",
				description: "é o valor presente ou a quantia total atual correspondente a uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro ou um saldo em dinheiro que se deseja obter após o último pagamento ter sido efetuado. Quando não especificado, é usado valor zero"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			}
		]
	},
	{
		name: "NÚM.CARACT",
		description: "Retorna o número de caracteres em uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é o texto cujo tamanho se deseja determinar. Os espaços são considerados caracteres"
			}
		]
	},
	{
		name: "NÚMSEMANA",
		description: "Retorna o número da semana no ano.",
		arguments: [
			{
				name: "núm_série",
				description: "é o código data-hora utilizado pelo Spreadsheet para cálculo de data e hora"
			},
			{
				name: "tipo_retorno",
				description: "é um número (1 ou 2) que determina o tipo do valor retornado"
			}
		]
	},
	{
		name: "NÚMSEMANAISO",
		description: "Retorna o número do número da semana ISO do ano de uma determinada data.",
		arguments: [
			{
				name: "data",
				description: "é o código de data-hora usado pelo Spreadsheet para cálculo de data e hora"
			}
		]
	},
	{
		name: "OCTABIN",
		description: "Converte um número octal em binário.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "OCTADEC",
		description: "Converte um número octal em decimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal que se deseja converter"
			}
		]
	},
	{
		name: "OCTAHEX",
		description: "Converte um número octal em hexadecimal.",
		arguments: [
			{
				name: "núm",
				description: "é o número octal que se deseja converter"
			},
			{
				name: "casas",
				description: "é o número de caracteres a ser utilizado"
			}
		]
	},
	{
		name: "ORDEM",
		description: "Retorna a posição de um número em uma lista de números: o seu tamanho em relação a outros valores da lista.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja posição se deseja localizar"
			},
			{
				name: "ref",
				description: "é uma matriz de, ou referência a, uma lista de números. Valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: posicionar na lista em ordem decrescente = 0 ou não especificado; posicionar na lista em ordem crescente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.EQ",
		description: "Retorna a posição de um número em uma lista de números: o seu tamanho em relação a outros valores da lista; se mais de um valor tiver a mesma posição, será retornada a posição mais elevada do conjunto de valores.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja posição você deseja localizar"
			},
			{
				name: "ref",
				description: "é uma matriz de, ou referência a, uma lista de números. Valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: posicionar na lista em ordem decrescente = 0 ou não especificado; posicionar na lista em ordem crescente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.MÉD",
		description: "Retorna a posição de um número em uma lista de números: o seu tamanho em relação a outros valores da lista; se mais de um valor tiver a mesma posição, será retornada uma posição média.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja posição você deseja localizar"
			},
			{
				name: "ref",
				description: "é uma matriz de, ou referência a, uma lista de números. Valores não numéricos são ignorados"
			},
			{
				name: "ordem",
				description: "é um número: posicionar na lista em ordem decrescente = 0 ou não especificado; posicionar na lista em ordem crescente = qualquer valor diferente de zero"
			}
		]
	},
	{
		name: "ORDEM.PORCENTUAL",
		description: "Retorna a posição de um valor em um conjunto de dados na forma de uma porcentagem do conjunto de dados.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de dados com valores numéricos que define uma posição relativa"
			},
			{
				name: "x",
				description: "é o valor cuja posição você deseja saber"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de dígitos significativos para a porcentagem retornada, três dígitos quando não especificado (0,xxx%)"
			}
		]
	},
	{
		name: "ORDEM.PORCENTUAL.EXC",
		description: "Retorna a posição de um valor em um conjunto de dados como uma porcentagem do conjunto de dados (0..1, exclusive).",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com valores numéricos que define uma posição relativa"
			},
			{
				name: "x",
				description: "é o valor cuja posição você deseja saber"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de dígitos significativos para a porcentagem retornada, três dígitos, quando não especificado (0.xxx%)"
			}
		]
	},
	{
		name: "ORDEM.PORCENTUAL.INC",
		description: "Retorna a posição de um valor em um conjunto de dados como uma porcentagem do conjunto de dados (0..1, inclusive).",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados com valores numéricos que define uma posição relativa"
			},
			{
				name: "x",
				description: "é o valor cuja posição você deseja saber"
			},
			{
				name: "significância",
				description: "é um valor opcional que identifica o número de dígitos significativos para a porcentagem retornada, três dígitos, quando não especificado (0,xxx%)"
			}
		]
	},
	{
		name: "OTN",
		description: "Retorna o rendimento de uma letra do tesouro equivalente ao rendimento de um título.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto da letra do tesouro"
			}
		]
	},
	{
		name: "OTNLUCRO",
		description: "Retorna o rendimento de uma letra do tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "pr",
				description: "é o preço por R$100 do valor nominal da letra do tesouro"
			}
		]
	},
	{
		name: "OTNVALOR",
		description: "Retorna o preço por R$100 do valor nominal de uma letra do tesouro.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento da letra do tesouro, expressa como um número serial de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto da letra do tesouro"
			}
		]
	},
	{
		name: "OU",
		description: "Verifica se algum argumento é VERDADEIRO e retorna VERDADEIRO ou FALSO. Retorna FALSO somente se todos os argumentos forem FALSO.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "lógico1",
				description: "de 1 a 255 condições que você deseja testar, podendo ser VERDADEIRO ou FALSO"
			},
			{
				name: "lógico2",
				description: "de 1 a 255 condições que você deseja testar, podendo ser VERDADEIRO ou FALSO"
			}
		]
	},
	{
		name: "PADRONIZAR",
		description: "Retorna um valor normalizado de uma distribuição caracterizada por uma média e um desvio padrão.",
		arguments: [
			{
				name: "x",
				description: "é o valor que se deseja normalizar"
			},
			{
				name: "média",
				description: "é a média aritmética da distribuição"
			},
			{
				name: "desv_padrão",
				description: "é o desvio padrão da distribuição, um número positivo"
			}
		]
	},
	{
		name: "PAR",
		description: "Arredonda um número positivo para cima e um número negativo para baixo até o valor inteiro mais próximo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor a ser arredondado"
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
		name: "PEARSON",
		description: "Retorna o coeficiente de correlação do momento do produto Pearson, r.",
		arguments: [
			{
				name: "matriz1",
				description: "é um conjunto de valores independentes"
			},
			{
				name: "matriz2",
				description: "é um conjunto de valores dependentes"
			}
		]
	},
	{
		name: "PERCENTIL",
		description: "Retorna o k-ésimo percentil de valores em um intervalo.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil entre 0 e 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.EXC",
		description: "Retorna o k-ésimo percentil de valores em um intervalo, em que k está no intervalo 0..1, exclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil no intervalo 0 a 1, inclusive"
			}
		]
	},
	{
		name: "PERCENTIL.INC",
		description: "Retorna o k-ésimo percentil de valores em um intervalo, em que k está no intervalo 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de dados que define a posição relativa"
			},
			{
				name: "k",
				description: "é o valor do percentil no intervalo 0 a 1, inclusive"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Retorna o número de permutações para um dado número de objetos que podem ser selecionados do total de objetos.",
		arguments: [
			{
				name: "núm",
				description: "é o número que descreve o número de objetos"
			},
			{
				name: "núm_escolhido",
				description: "é o número de objetos em cada permutação"
			}
		]
	},
	{
		name: "PERMUTAS",
		description: "Retorna o número de permutações para um determinado número de objetos (com repetições) que podem ser selecionados do total de objetos.",
		arguments: [
			{
				name: "número",
				description: "é o número total de objetos"
			},
			{
				name: "núm_escolhido",
				description: "é o número de objetos em cada permutação"
			}
		]
	},
	{
		name: "PGTO",
		description: "Calcula o pagamento de um empréstimo com base em pagamentos e em uma taxa de juros constantes.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período de um empréstimo. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "nper",
				description: "é o número total de pagamentos em um empréstimo"
			},
			{
				name: "vp",
				description: "é o valor presente: a quantia total atual de uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro, ou um saldo em dinheiro que se deseja obter após o último pagamento ter sido efetuado; 0 (zero) quando não especificado"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			}
		]
	},
	{
		name: "PGTOCAPACUM",
		description: "Retorna o capital cumulativo pago em um empréstimo entre dois períodos.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento"
			},
			{
				name: "vp",
				description: "é o valor presente"
			},
			{
				name: "início_período",
				description: "é o primeiro período do cálculo"
			},
			{
				name: "final_período",
				description: "é o último período do cálculo"
			},
			{
				name: "tipo_pgto",
				description: "indica quando o pagamento é efetuado"
			}
		]
	},
	{
		name: "PGTOJURACUM",
		description: "Retorna os juros cumulativos pagos entre dois períodos.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento"
			},
			{
				name: "vp",
				description: "é o valor presente"
			},
			{
				name: "início_período",
				description: "é o primeiro período do cálculo"
			},
			{
				name: "final_período",
				description: "é o último período do cálculo"
			},
			{
				name: "tipo_pgto",
				description: "indica quando o pagamento é efetuado"
			}
		]
	},
	{
		name: "PHI",
		description: "Retorna o valor da função de densidade para uma distribuição normal padrão.",
		arguments: [
			{
				name: "x",
				description: "é o número do qual você deseja a densidade da distribuição normal padrão"
			}
		]
	},
	{
		name: "PI",
		description: "Retorna o valor de Pi, 3,14159265358979, em 15 dígitos.",
		arguments: [
		]
	},
	{
		name: "PLAN",
		description: "Retorna o número da folha da folha de referência.",
		arguments: [
			{
				name: "valor",
				description: " é o nome de uma planilha ou uma referência que você deseja que o número da folha de. Se omitido, o número da planilha que contém a função será retornado"
			}
		]
	},
	{
		name: "PLANS",
		description: "Retorna o número de planilhas em uma referência.",
		arguments: [
			{
				name: "referência",
				description: " é uma referência para o qual você deseja saber o número de planilhas que ela contém. Se omitido, o número das planilhas na pasta de trabalho que contém a função será retornado"
			}
		]
	},
	{
		name: "POISSON",
		description: "Retorna a distribuição Poisson.",
		arguments: [
			{
				name: "x",
				description: "é o número de eventos"
			},
			{
				name: "média",
				description: "é o valor numérico esperado, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a probabilidade Poisson, use VERDADEIRO; para a função de probabilidade de massa Poisson, use FALSO"
			}
		]
	},
	{
		name: "POTÊNCIA",
		description: "Retorna o resultado de um número elevado a uma potência.",
		arguments: [
			{
				name: "núm",
				description: "é o número de base, qualquer número real"
			},
			{
				name: "potência",
				description: "é o expoente para o qual a base é elevada"
			}
		]
	},
	{
		name: "PPGTO",
		description: "Retorna o pagamento sobre o montante de um investimento com base em pagamentos e em uma taxa de juros constantes, periódicos.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "per",
				description: "especifica o período e deve estar no intervalo entre 1 e nper"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento em um investimento"
			},
			{
				name: "vp",
				description: "é o valor presente: a quantia total atual de uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro, ou um saldo em dinheiro que se deseja obter após o último pagamento ter sido efetuado"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			}
		]
	},
	{
		name: "PREÇODESC",
		description: "Retorna o preço por R$100 do valor nominal de um título com deságio.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título"
			},
			{
				name: "resgate",
				description: "é o valor de resgate do título por R$100 do valor nominal"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "PREVISÃO",
		description: "Calcula ou prevê, um valor futuro junto com uma tendência linear usando valores existentes.",
		arguments: [
			{
				name: "x",
				description: "é o ponto de dados cujo valor você deseja prever, um valor numérico"
			},
			{
				name: "val_conhecidos_y",
				description: "é o intervalo de dados numéricos ou matriz dependente"
			},
			{
				name: "val_conhecidos_x",
				description: "é o intervalo de dados ou matriz independente. A variação de 'Val_conhecidos_x' não pode ser zero"
			}
		]
	},
	{
		name: "PRI.MAIÚSCULA",
		description: "Converte uma cadeia de texto no formato apropriado; a primeira letra de cada palavra em maiúscula e as demais letras em minúsculas.",
		arguments: [
			{
				name: "texto",
				description: "é o texto entre aspas, uma fórmula que retorna o texto ou uma referência a uma célula que contenha o texto o qual você deseja colocar parcialmente em maiúscula"
			}
		]
	},
	{
		name: "PROB",
		description: "Retorna a probabilidade de que valores em um intervalo estão entre dois limites ou são iguais ao limite inferior.",
		arguments: [
			{
				name: "intervalo_x",
				description: "é o intervalo de valores numéricos de x com os quais são associadas probabilidades"
			},
			{
				name: "intervalo_prob",
				description: "é um conjunto de probabilidades associadas a valores no 'Intervalo_x', valores entre 0 e 1 excluindo 0"
			},
			{
				name: "limite_inferior",
				description: "é o limite inferior do valor cuja probabilidade você deseja obter"
			},
			{
				name: "limite_superior",
				description: "é o limite superior opcional do valor cuja probabilidade você deseja obter. Quando não especificado, PROB retorna a probabilidade de os valores de 'Intervalo_x' serem iguais ao 'Limite_inferior'"
			}
		]
	},
	{
		name: "PROC",
		description: "Procura um valor a partir de um intervalo de linha ou coluna ou de uma matriz. Fornecido para manter a compatibilidade com versões anteriores.",
		arguments: [
			{
				name: "valor_procurado",
				description: "é o valor que PROC pesquisa no Vetor_proc, podendo ser um número, texto, um valor lógico, um nome ou uma referência a um valor"
			},
			{
				name: "vetor_proc",
				description: "é o intervalo que contém somente uma linha ou uma coluna de texto, números ou valores lógicos, colocados em order crescente"
			},
			{
				name: "vetor_result",
				description: "é um intervalor que contém apneas uma linha ou coluna com o mesmo tamanho de Vetor_proc"
			}
		]
	},
	{
		name: "PROCH",
		description: "Pesquisa um valor na linha superior de uma tabela ou matriz de valores e retorna o valor na mesma coluna a partir de uma linha especificada.",
		arguments: [
			{
				name: "valor_procurado",
				description: "é o valor a ser localizado na primeira linha da tabela. Podendo ser um valor, uma referência ou uma cadeia de texto"
			},
			{
				name: "matriz_tabela",
				description: "é uma tabela de texto, números ou valores lógicos na qual são pesquisados dados. 'Matriz_tabela' pode ser uma referência a um intervalo ou um nome de intervalo"
			},
			{
				name: "núm_índice_lin",
				description: "é o número da linha em 'Matriz_tabela' de onde o valor correspondente deve ser retornado. A primeira linha de valores da tabela é linha 1"
			},
			{
				name: "procurar_intervalo",
				description: "é um valor lógico: para determinar a correspondência mais semelhante na linha superior (classificada em ordem crescente) = VERDADEIRO ou não especificada; determinar uma correspondência exata = FALSO"
			}
		]
	},
	{
		name: "PROCURAR",
		description: "Retorna a posição inicial de uma cadeia de texto encontrada em outra cadeia de texto. LOCALIZAR diferencia maiúsculas de minúsculas.",
		arguments: [
			{
				name: "texto_procurado",
				description: "é o texto que se deseja encontrar. Utilize aspas duplas (texto vazio) para corresponder ao primeiro caractere em no_texto; não é permitido o uso de caracteres curinga"
			},
			{
				name: "no_texto",
				description: "é o texto contendo o texto que se deseja encontrar"
			},
			{
				name: "núm_inicial",
				description: "especifica o caractere a partir do qual a pesquisa será iniciada. O primeiro caractere em 'No_texto' é o caractere número 1. Quando não especificado, Núm_inicial = 1"
			}
		]
	},
	{
		name: "PROCV",
		description: "Procura um valor na primeira coluna à esquerda de uma tabela e retorna um valor na mesma linha de uma coluna especificada. Como padrão, a tabela deve estar classificada em ordem crescente.",
		arguments: [
			{
				name: "valor_procurado",
				description: "é o valor a ser localizado na primeira coluna de uma tabela, podendo ser um valor, uma referência ou uma cadeia de texto"
			},
			{
				name: "matriz_tabela",
				description: "é uma tabela de texto, números ou valores lógicos cujos dados são recuperados. 'Matriz_tabela' pode ser uma referência a um intervalo ou a um nome de intervalo"
			},
			{
				name: "núm_índice_coluna",
				description: "é o número da coluna em 'Matriz_tabela' a partir do qual o valor correspondente deve ser retornado. A primeira coluna de valores na tabela é a coluna 1"
			},
			{
				name: "procurar_intervalo",
				description: "é um valor lógico: para encontrar a correspondência mais próxima na primeira coluna (classificada em ordem crescente) = VERDADEIRO ou não especificado. Para encontrar a correspondência exata = FALSO"
			}
		]
	},
	{
		name: "PROJ.LIN",
		description: "Retorna a estatística que descreve a tendência linear que corresponda aos pontos de dados, ajustando uma linha reta através do método de quadrados mínimos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = mx + b"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores x que já deve ser conhecido na relação y = mx + b"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante 'b' é calculada normalmente se Constante = VERDADEIRO ou não especificado; 'b' é definido como 0 se Constante = FALSO"
			},
			{
				name: "estatística",
				description: "é um valor lógico: retorna estatística de regressão adicional = VERDADEIRO; retorna coeficientes-m e a constante 'b' = FALSO ou não especificado"
			}
		]
	},
	{
		name: "PROJ.LOG",
		description: "Retorna a estatística que descreve uma curva exponencial que corresponda a pontos de dados conhecidos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é o conjunto de valores de y já conhecidos na relação y = b*m^x"
			},
			{
				name: "val_conhecidos_x",
				description: "é um conjunto opcional de valores x que já deve ser conhecido na relação y = b*m^x"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante 'b' é calculada normalmente se Constante = VERDADEIRO ou não especificado; 'b' é definido como 1 se Constante = FALSO"
			},
			{
				name: "estatística",
				description: "é um valor lógico: retorna estatística de regressão adicional = VERDADEIRO; retorna coeficientes-m e a constante 'b' = FALSO ou não especificado"
			}
		]
	},
	{
		name: "QUARTIL",
		description: "Retorna o quartil do conjunto de dados.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou o intervalo de células de valores numéricos cujo valor quartil você deseja obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0, primeiro quartil = 1, valor mediano = 2, terceiro quartil = 3, valor máximo = 4"
			}
		]
	},
	{
		name: "QUARTIL.EXC",
		description: "Retorna o quartil do conjunto de dados, com base nos valores de percentil de 0..1, exclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de células de valores numéricos cujo valor do quartil você deseja obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0, primeiro quartil = 1, valor mediano = 2, terceiro quartil = 3, valor máximo = 4"
			}
		]
	},
	{
		name: "QUARTIL.INC",
		description: "Retorna o quartil do conjunto de dados, com base nos valores de percentil de 0..1, inclusive.",
		arguments: [
			{
				name: "matriz",
				description: "é a matriz ou intervalo de células de valores numéricos cujo valor do quartil você deseja obter"
			},
			{
				name: "quarto",
				description: "é um número: valor mínimo = 0, primeiro quartil = 1, valor mediano = 2, terceiro quartil = 3, valor máximo = 4"
			}
		]
	},
	{
		name: "QUOCIENTE",
		description: "Retorna a parte inteira de uma divisão.",
		arguments: [
			{
				name: "numerador",
				description: "é o dividendo"
			},
			{
				name: "denominador",
				description: "é o divisor"
			}
		]
	},
	{
		name: "RADIANOS",
		description: "Converte graus em radianos.",
		arguments: [
			{
				name: "ângulo",
				description: "é um ângulo em graus que se deseja converter"
			}
		]
	},
	{
		name: "RAIZ",
		description: "Retorna a raiz quadrada de um número.",
		arguments: [
			{
				name: "núm",
				description: "é o número cuja raiz quadrada você deseja calcular"
			}
		]
	},
	{
		name: "RAIZPI",
		description: "Retorna a raiz quadrada de (núm * Pi).",
		arguments: [
			{
				name: "núm",
				description: "é o número pelo qual pi é multiplicado"
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
		name: "RECEBER",
		description: "Retorna a quantia recebida no vencimento para um título totalmente investido.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "investimento",
				description: "é a quantia investida no título"
			},
			{
				name: "desconto",
				description: "é a taxa de desconto do título"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "REPT",
		description: "Repete o texto um determinado número de vezes. Utilize REPT para preencher uma célula com um número de repetições de uma cadeia.",
		arguments: [
			{
				name: "texto",
				description: "é o texto que se deseja repetir"
			},
			{
				name: "número_vezes",
				description: "é um número positivo que especifica o número de vezes que texto deve ser repetido"
			}
		]
	},
	{
		name: "ROMANO",
		description: "Converte um algarismo arábico em romano, como texto.",
		arguments: [
			{
				name: "núm",
				description: "é o algarismo arábico que se deseja converter"
			},
			{
				name: "forma",
				description: "é o número que especifica o tipo de algarismo romano desejado."
			}
		]
	},
	{
		name: "RQUAD",
		description: "Retorna o quadrado do coeficiente de correlação do momento do produto de Pearson para os pontos de dados determinados.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é uma matriz ou intervalo de pontos de dados, podendo ser números ou nomes, matrizes ou referências que contenham números"
			},
			{
				name: "val_conhecidos_x",
				description: "é uma matriz ou intervalo de pontos de dados, podendo ser números ou nomes, matrizes ou referências que contenham números"
			}
		]
	},
	{
		name: "RTD",
		description: "Recupera dados em tempo real de um programa que oferece suporte à automação COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "é o nome da identificação da programação de um suplemento de automação COM registrado. Coloque o nome entre aspas"
			},
			{
				name: "server",
				description: "é o nome do servidor no qual o suplemento deve ser executado. Coloque o nome entre aspas. Se o suplemento for executado localmente, use uma cadeia vazia"
			},
			{
				name: "topic1",
				description: "de 1 a 38 parâmetros especificam um pedaço de dados"
			},
			{
				name: "topic2",
				description: "de 1 a 38 parâmetros especificam um pedaço de dados"
			}
		]
	},
	{
		name: "SDA",
		description: "Retorna a depreciação dos dígitos da soma dos anos de um ativo para um período especificado.",
		arguments: [
			{
				name: "custo",
				description: "é o custo inicial do ativo"
			},
			{
				name: "recuperação",
				description: "é o valor residual no final da vida útil do ativo"
			},
			{
				name: "vida_útil",
				description: "é o número de períodos durante os quais o ativo está sendo depreciado (algumas vezes denominado a vida útil do ativo)"
			},
			{
				name: "per",
				description: "é o período e deve utilizar as mesmas unidades de 'Vida_útil'"
			}
		]
	},
	{
		name: "SE",
		description: "Verifica se uma condição foi satisfeita e retorna um valor se for VERDADEIRO e retorna um outro valor se for FALSO.",
		arguments: [
			{
				name: "teste_lógico",
				description: "é qualquer valor ou expressão que pode ser avaliada como VERDADEIRO ou FALSO"
			},
			{
				name: "valor_se_verdadeiro",
				description: "é o valor retornado se 'Teste_lógico' for VERDADEIRO. Quando não especificado, é retornado VERDADEIRO. Você pode aninhar até sete funções SE"
			},
			{
				name: "valor_se_falso",
				description: "é o valor retornado se 'Teste_lógico' for FALSO. Quando não especificado, é retornado FALSO"
			}
		]
	},
	{
		name: "SEC",
		description: "Retorna a secante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a secante"
			}
		]
	},
	{
		name: "SECH",
		description: "Retorna a hiperbólica da secante de um ângulo.",
		arguments: [
			{
				name: "número",
				description: "é o ângulo em radianos para o qual você deseja a hiperbólica da secante"
			}
		]
	},
	{
		name: "SEERRO",
		description: "Retorna valor_se_erro se a expressão for um erro ; caso contrário, retorna o valor da expressão.",
		arguments: [
			{
				name: "valor",
				description: "é qualquer valor, expressão ou referência"
			},
			{
				name: "valor_se_erro",
				description: "é qualquer valor, expressão ou referência"
			}
		]
	},
	{
		name: "SEGUNDO",
		description: "Retorna o segundo, um número de 0 a 59.",
		arguments: [
			{
				name: "núm_série",
				description: "é um número no código data-hora usado pelo Spreadsheet ou um texto no formato de hora como, por exemplo, 16:48:23 ou 4:48:47 PM"
			}
		]
	},
	{
		name: "SEN",
		description: "Retorna o seno de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos cujo seno você deseja obter. Graus * PI()/180 = radianos"
			}
		]
	},
	{
		name: "SENÃODISP",
		description: "Retorna o valor que você especificar se a expressão resolver para #N/A, caso contrário, retorna o resultado da expressão.",
		arguments: [
			{
				name: "valor",
				description: "é qualquer valor ou expressão ou referência"
			},
			{
				name: "valor_se_na",
				description: "é qualquer valor ou expressão ou referência"
			}
		]
	},
	{
		name: "SENH",
		description: "Retorna o seno hiperbólico de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "SINAL",
		description: "Retorna o sinal de um número: 1 se o número for positivo, 0 se o número for zero ou -1 se o número for negativo.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "SOMA",
		description: "Soma todos os números em um intervalo de células.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números a serem somados. Valores lógicos e texto são ignorados, mesmo quando digitados como argumentos"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números a serem somados. Valores lógicos e texto são ignorados, mesmo quando digitados como argumentos"
			}
		]
	},
	{
		name: "SOMAQUAD",
		description: "Retorna a soma dos quadrados dos argumentos. Os argumentos podem ser números, matrizes, nomes ou referências a células que contenham números.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 números, matrizes, nomes ou referências a matrizes cuja soma dos quadrados você deseja calcular"
			},
			{
				name: "núm2",
				description: "de 1 a 255 números, matrizes, nomes ou referências a matrizes cuja soma dos quadrados você deseja calcular"
			}
		]
	},
	{
		name: "SOMARPRODUTO",
		description: "Retorna a soma dos produtos de intervalos ou matrizes correspondentes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matriz1",
				description: "de 2 a 255 matrizes para as quais você deseja multiplicar e somar componentes. Todas as matrizes devem ter as mesmas dimensões"
			},
			{
				name: "matriz2",
				description: "de 2 a 255 matrizes para as quais você deseja multiplicar e somar componentes. Todas as matrizes devem ter as mesmas dimensões"
			},
			{
				name: "matriz3",
				description: "de 2 a 255 matrizes para as quais você deseja multiplicar e somar componentes. Todas as matrizes devem ter as mesmas dimensões"
			}
		]
	},
	{
		name: "SOMASE",
		description: "Adiciona as células especificadas por um determinado critério ou condição.",
		arguments: [
			{
				name: "intervalo",
				description: "é o intervalo de células que se quer calculado"
			},
			{
				name: "critérios",
				description: "é o critério ou condição na forma de um número, expressão ou texto, que definem quais células serão adicionadas"
			},
			{
				name: "intervalo_soma",
				description: "são células a serem somadas. Quando não especificadas, são usadas as células do intervalo"
			}
		]
	},
	{
		name: "SOMASEQÜÊNCIA",
		description: "Retorna a soma de uma série de potência baseada na fórmula.",
		arguments: [
			{
				name: "x",
				description: "é o valor de entrada para a série de potência"
			},
			{
				name: "n",
				description: "é a potência inicial à qual se deseja elevar x"
			},
			{
				name: "m",
				description: "é a etapa pela qual se acrescenta n em cada termo na série"
			},
			{
				name: "coeficientes",
				description: "é um conjunto de coeficientes pelo qual cada potência sucessiva de x será multiplicada"
			}
		]
	},
	{
		name: "SOMASES",
		description: "Adiciona as células especificadas por um dado conjunto de condições ou critérios.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "intervalo_soma",
				description: "são as células a serem somadas."
			},
			{
				name: "intervalo_critérios",
				description: "é o intervalo de células que se deseja avaliar com a condição dada"
			},
			{
				name: "critérios",
				description: "é a condição ou os critérios expressos como um número, uma expressão ou um texto que define quais células serão adicionadas"
			}
		]
	},
	{
		name: "SOMAX2DY2",
		description: "Soma as diferenças entre dois quadrados de dois intervalos ou matrizes correspondentes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é a primeira matriz ou intervalo de números, podendo ser um número ou nome, matriz ou referência que contenha números"
			},
			{
				name: "matriz_y",
				description: "é a segunda matriz ou intervalo de números, podendo ser um número ou nome, matriz ou referência que contenha números"
			}
		]
	},
	{
		name: "SOMAX2SY2",
		description: "Retorna a soma total das somas dos quadrados dos números em dois intervalos ou matrizes correspondentes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é a primeira matriz ou intervalo de números, podendo ser um número ou nome, matriz ou referência que contenha números"
			},
			{
				name: "matriz_y",
				description: "é a segunda matriz ou intervalo de números, podendo ser um número ou nome, matriz ou referência que contenha números"
			}
		]
	},
	{
		name: "SOMAXMY2",
		description: "Soma os quadrados das diferenças em dois intervalos ou matrizes correspondentes.",
		arguments: [
			{
				name: "matriz_x",
				description: "é a primeira matriz ou intervalo de valores, podendo ser um número ou nome, matriz ou referência que contenha números"
			},
			{
				name: "matriz_y",
				description: "é a segunda matriz ou intervalo de valores, podendo ser um número ou nome, matriz ou referência que contenha números"
			}
		]
	},
	{
		name: "SUBSTITUIR",
		description: "Substitui um texto antigo por outro novo em uma cadeia de texto.",
		arguments: [
			{
				name: "texto",
				description: "é o texto ou a referência a uma célula com texto cujos caracteres você deseja substituir"
			},
			{
				name: "texto_antigo",
				description: "é o texto que se deseja substituir. SUBSTITUIR não irá substituir textos iguais que não coincidam maiúsculas e minúsculas"
			},
			{
				name: "novo_texto",
				description: "é o texto pelo qual você deseja substituir 'Texto_antigo'"
			},
			{
				name: "núm_da_ocorrência",
				description: "especifica qual a ocorrência de 'Texto_antigo' que deve ser substituída por 'Novo_texto'. Quando não especificada, toda instância de 'Texto_antigo' é substituída"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Retorna um subtotal em uma lista ou um banco de dados.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm_função",
				description: "é o número de 1 a 11 que determina a função resumo do subtotal"
			},
			{
				name: ";ref1",
				description: "de 1 a 254 intervalos ou referências cujos subtotais se deseja"
			}
		]
	},
	{
		name: "T",
		description: "Verifica se um valor é texto e retorna o texto referido em caso afirmativo ou retorna aspas duplas (texto vazio) em caso negativo.",
		arguments: [
			{
				name: "valor",
				description: "é o valor que se deseja testar"
			}
		]
	},
	{
		name: "TAN",
		description: "Retorna a tangente de um ângulo.",
		arguments: [
			{
				name: "núm",
				description: "é o ângulo em radianos cuja tangente você deseja obter. Graus * PI()/180 = radianos"
			}
		]
	},
	{
		name: "TANH",
		description: "Retorna a tangente hiperbólica de um número.",
		arguments: [
			{
				name: "núm",
				description: "é qualquer número real"
			}
		]
	},
	{
		name: "TAXA",
		description: "Retorna a taxa de juros por período em um empréstimo ou investimento. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA.",
		arguments: [
			{
				name: "nper",
				description: "é o número total de períodos de pagamento em um empréstimo ou um investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado a cada período e não pode ser alterado no decorrer do empréstimo ou investimento"
			},
			{
				name: "vp",
				description: "é o valor presente: a quantia total atual de uma série de pagamentos futuros"
			},
			{
				name: "vf",
				description: "é o valor futuro, ou um saldo em dinheiro que se deseja atingir após o último pagamento ter sido efetuado. Quando não especificado, utiliza Vf = 0"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			},
			{
				name: "estimativa",
				description: "é a estimativa do valor da taxa; quando não especificado, Estimar = 0,1 (10 por cento)"
			}
		]
	},
	{
		name: "TAXAJURO",
		description: "Retorna uma taxa de juros equivalente para o crescimento de um investimento.",
		arguments: [
			{
				name: "nper",
				description: "é o número de períodos do investimento"
			},
			{
				name: "pv",
				description: "é o valor presente do investimento"
			},
			{
				name: "fv",
				description: "é o valor futuro do investimento"
			}
		]
	},
	{
		name: "TAXAJUROS",
		description: "Retorna a taxa de juros de um título totalmente investido.",
		arguments: [
			{
				name: "liquidação",
				description: "é a data de liquidação do título, expressa como um número serial de data"
			},
			{
				name: "vencimento",
				description: "é a data de vencimento do título, expressa como um número serial de data"
			},
			{
				name: "investimento",
				description: "é a quantia investida no título"
			},
			{
				name: "resgate",
				description: "é a quantia a ser recebida no vencimento"
			},
			{
				name: "base",
				description: "é o tipo de base de contagem diária a ser utilizada"
			}
		]
	},
	{
		name: "TEMPO",
		description: "Converte horas, minutos e segundos fornecidos como números em um número de série do Spreadsheet, formatado com formato de hora.",
		arguments: [
			{
				name: "hora",
				description: "é um número de 0 a 23 representando a hora"
			},
			{
				name: "minuto",
				description: "é um número de 0 a 59 representando o minuto"
			},
			{
				name: "segundo",
				description: "é um número de 0 a 59 representando o segundo"
			}
		]
	},
	{
		name: "TENDÊNCIA",
		description: "Retorna números em uma tendência linear que corresponda a pontos de dados conhecidos através do método de quadrados mínimos.",
		arguments: [
			{
				name: "val_conhecidos_y",
				description: "é um intervalo ou matriz de valores de y conhecidos na relação y = mx + b"
			},
			{
				name: "val_conhecidos_x",
				description: "é um intervalo ou matriz de valores de x conhecidos na relação y = ms + b, uma matriz com o mesmo tamanho que 'Val_conhecidos_y'"
			},
			{
				name: "novos_valores_x",
				description: "é um intervalo ou matriz de valores de x para os quais você deseja que TENDÊNCIA forneça valores de y correspondentes"
			},
			{
				name: "constante",
				description: "é um valor lógico: a constante 'b' é calculada normalmente se Constante = VERDADEIRO ou não especificado; 'b' é definido como 0 se Constante = FALSO"
			}
		]
	},
	{
		name: "TESTE.F",
		description: "Retorna o resultado de um teste F, a probabilidade bicaudal de que as variações em Matriz1 e Matriz2 não sejam significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz ou intervalo de dados, podendo ser números ou nomes, matrizes ou referências que contenham números (os espaços em branco são ignorados)"
			},
			{
				name: "matriz2",
				description: "é a segunda matriz ou intervalo de dados, podendo ser números ou nomes, matrizes ou referências que contenham números (os espaços em branco são ignorados)"
			}
		]
	},
	{
		name: "TESTE.QUI",
		description: "Retorna o teste para independência: o valor da distribuição qui-quadrada para a estatística e o grau apropriado de liberdade.",
		arguments: [
			{
				name: "intervalo_real",
				description: "é o intervalo de dados que contém observações para serem comparadas com os valores esperados"
			},
			{
				name: "intervalo_esperado",
				description: "é o intervalo de dados que contém a proporção do produto entre os totais de linhas e os totais de colunas e o total geral"
			}
		]
	},
	{
		name: "TESTE.QUIQUA",
		description: "Retorna o teste para independência: o valor da distribuição qui-quadrada para a estatística e o grau apropriado de liberdade.",
		arguments: [
			{
				name: "intervalo_real",
				description: "é o intervalo de dados que contém observações para serem comparadas com os valores esperados"
			},
			{
				name: "intervalo_esperado",
				description: "é o intervalo de dados que contém a proporção entre o produto dos totais de linhas e dos totais de colunas e o total geral"
			}
		]
	},
	{
		name: "TESTE.T",
		description: "Retorna a probabilidade associada ao teste t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro conjunto de dados"
			},
			{
				name: "matriz2",
				description: "é o segundo conjunto de dados"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas da distribuição a ser retornado: distribuição unicaudal = 1; distribuição bicaudal = 2"
			},
			{
				name: "tipo",
				description: "é o tipo de teste t: par = 1, variação igual de duas amostras (homoscedástica) = 2, variação desigual de duas amostras = 3"
			}
		]
	},
	{
		name: "TESTE.Z",
		description: "Retorna o valor-P unicaudal do teste-z.",
		arguments: [
			{
				name: "matriz",
				description: " é a matriz ou intervalo de dados em que x será testado"
			},
			{
				name: "x",
				description: " é o valor para teste"
			},
			{
				name: "sigma",
				description: "é o desvio padrão da população (conhecida). Quando não especificado, o desvio padrão da amostra é utilizado"
			}
		]
	},
	{
		name: "TESTEF",
		description: "Retorna o resultado de um teste F, a probabilidade bicaudal de que as variações em Matriz1 e Matriz2 não sejam significativamente diferentes.",
		arguments: [
			{
				name: "matriz1",
				description: "é a primeira matriz ou intervalo de dados, podendo ser números ou nomes, matrizes ou referências que contenham números (os espaços em branco são ignorados)"
			},
			{
				name: "matriz2",
				description: "é a segunda matriz ou intervalo de dados, podendo ser números ou nomes, matrizes ou referências que contenham números (os espaços em branco são ignorados)"
			}
		]
	},
	{
		name: "TESTET",
		description: "Retorna a probabilidade associada ao teste t de Student.",
		arguments: [
			{
				name: "matriz1",
				description: "é o primeiro conjunto de dados"
			},
			{
				name: "matriz2",
				description: "é o segundo conjunto de dados"
			},
			{
				name: "caudas",
				description: "especifica o número de caudas da distribuição a ser retornado: distribuição unicaudal = 1; distribuição bicaudal = 2"
			},
			{
				name: "tipo",
				description: "é o tipo de teste t: par = 1, variação igual de duas amostras (homoscedástica) = 2, variação desigual de duas amostras = 3"
			}
		]
	},
	{
		name: "TESTEZ",
		description: "Retorna o valor-P unicaudal do teste z.",
		arguments: [
			{
				name: "matriz",
				description: " é a matriz ou o intervalo de dados em que x será testado"
			},
			{
				name: "x",
				description: " é o valor para teste"
			},
			{
				name: "sigma",
				description: "é o desvio padrão da população (conhecida). Quando não especificado, o desvio padrão da amostra é utilizado"
			}
		]
	},
	{
		name: "TETO",
		description: "Arredonda um número para cima, para o próximo múltiplo significativo.",
		arguments: [
			{
				name: "núm",
				description: "é o valor que se deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual se deseja arredondar"
			}
		]
	},
	{
		name: "TETO.MAT",
		description: "Arredonda um número para cima, para o inteiro mais próximo ou para o próximo múltiplo significativo.",
		arguments: [
			{
				name: "número",
				description: "é o valor que você deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual você deseja arredondar"
			},
			{
				name: "modo",
				description: "quando determinado e diferente de zero, essa função arredondará afastando de zero"
			}
		]
	},
	{
		name: "TETO.PRECISO",
		description: "Arredonda um número para cima, para o próximo número inteiro ou até o próximo múltiplo significativo.",
		arguments: [
			{
				name: "número",
				description: "é o valor que você deseja arredondar"
			},
			{
				name: "significância",
				description: "é o múltiplo para o qual você quer arredondar"
			}
		]
	},
	{
		name: "TEXTO",
		description: "Converte um valor em texto com um formato de número específico.",
		arguments: [
			{
				name: "valor",
				description: "é um valor numérico, uma fórmula avaliada em um valor numérico, ou uma referência a uma célula que contém um valor numérico"
			},
			{
				name: "formato_texto",
				description: "é um formato de número na forma de texto contido na caixa 'Categoria' da guia 'Número' na caixa de diálogo 'Formatar células' (não 'Geral')"
			}
		]
	},
	{
		name: "TIPO",
		description: "Retorna um número inteiro que indica o tipo de dado de um valor: número = 1, texto = 2, valor lógico = 4, valor de erro = 16, matriz = 64.",
		arguments: [
			{
				name: "valor",
				description: "pode ser qualquer valor"
			}
		]
	},
	{
		name: "TIPO.ERRO",
		description: "Retorna um número que corresponde a um valor de erro.",
		arguments: [
			{
				name: "val_erro",
				description: "é o valor de erro cujo número de identificação você deseja obter, podendo ser um valor de erro real ou uma referência a uma célula contendo um valor de erro"
			}
		]
	},
	{
		name: "TIR",
		description: "Retorna a taxa interna de retorno de uma série de fluxos de caixa.",
		arguments: [
			{
				name: "valores",
				description: "é uma matriz ou uma referência a células que contêm números cuja taxa interna de retorno se deseja calcular"
			},
			{
				name: "estimativa",
				description: "é um número que se estima ser próximo do resultado de TIR; 0,1 (10%) quando não especificado"
			}
		]
	},
	{
		name: "TIRAR",
		description: "Remove do texto todos os caracteres não imprimíveis.",
		arguments: [
			{
				name: "texto",
				description: "é qualquer informação sobre a planilha de onde você deseja remover os caracteres que não podem ser impressos"
			}
		]
	},
	{
		name: "TRANSPOR",
		description: "Converte um intervalo de células vertical em um intervalo horizontal e vice-versa.",
		arguments: [
			{
				name: "matriz",
				description: "é um intervalo de células em uma planilha ou matriz de valores que se deseja transpor"
			}
		]
	},
	{
		name: "TRUNCAR",
		description: "Trunca um número até um número inteiro removendo a parte decimal ou fracionária do número.",
		arguments: [
			{
				name: "núm",
				description: "é o número que se deseja truncar"
			},
			{
				name: "núm_dígitos",
				description: "é um número que especifica a precisão da truncagem, 0 (zero) quando não especificado"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Retorna o número (ponto de código) correspondente ao primeiro caractere do texto.",
		arguments: [
			{
				name: "texto",
				description: "é o caractere do qual você deseja o valor Unicode"
			}
		]
	},
	{
		name: "VALOR",
		description: "Converte uma cadeia de texto que representa um número em um número.",
		arguments: [
			{
				name: "texto",
				description: "é o texto entre aspas ou a referência a uma célula contendo o texto que se deseja converter"
			}
		]
	},
	{
		name: "VALOR.TEMPO",
		description: "Converte uma hora de texto em um número de série do Spreadsheet que represente hora, um número de 0 (12:00:00 AM) a 0.999988426 (11:59:59 PM). Formate o número com um formato de hora após inserir a fórmula.",
		arguments: [
			{
				name: "texto_hora",
				description: "é uma cadeia de texto que retorna uma hora em qualquer um dos formatos de hora do Spreadsheet (a informação de data na cadeia é ignorada)"
			}
		]
	},
	{
		name: "VALORNUMÉRICO",
		description: "Converte texto em número de maneira independente de localidade.",
		arguments: [
			{
				name: "texto",
				description: "é a cadeia de caracteres que representa o número que você deseja converter"
			},
			{
				name: "separador_decimal",
				description: "é o caractere usado como separador decimal na cadeia de caracteres"
			},
			{
				name: "separador_grupo",
				description: "é o caractere usado como separador de grupo na cadeia de caracteres"
			}
		]
	},
	{
		name: "VAR",
		description: "Estima a variação com base em uma amostra (ignora valores lógicos e texto da amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma amostra de uma população"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma amostra de uma população"
			}
		]
	},
	{
		name: "VAR.A",
		description: "Estima a variação com base em uma amostra (ignora valores lógicos e texto na amostra).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma amostra de uma população"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma amostra de uma população"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calcula a variação com base na população total (ignora valores lógicos e texto da população).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma população"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma população"
			}
		]
	},
	{
		name: "VARA",
		description: "Estima a variação com base em uma amostra, incluindo valores lógicos e texto. Texto e o valor lógico FALSO têm o valor 0, o valor lógico VERDADEIRO tem valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 argumentos que correspondem a uma amostra de uma população"
			},
			{
				name: "valor2",
				description: "de 1 a 255 argumentos que correspondem a uma amostra de uma população"
			}
		]
	},
	{
		name: "VARP",
		description: "Calcula a variação com base na população total (ignora valores lógicos e texto da população).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "núm1",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma população"
			},
			{
				name: "núm2",
				description: "de 1 a 255 argumentos numéricos que correspondem a uma população"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calcula a variação com base na população total, incluindo valores lógicos e texto. Texto e o valor lógico FALSO têm o valor 0, o valor lógico VERDADEIRO tem valor 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valor1",
				description: "de 1 a 255 valores que correspondem a uma população"
			},
			{
				name: "valor2",
				description: "de 1 a 255 valores que correspondem a uma população"
			}
		]
	},
	{
		name: "VERDADEIRO",
		description: "Retorna o valor lógico VERDADEIRO.",
		arguments: [
		]
	},
	{
		name: "VF",
		description: "Retorna o valor futuro de um investimento com base em pagamentos constantes e periódicos e uma taxa de juros constante.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "nper",
				description: "é o número total de períodos de pagamento em um investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado a cada período; não pode ser alterado no decorrer do investimento"
			},
			{
				name: "vp",
				description: "é o valor presente, ou a quantia total atual correspondente a uma série de pagamentos futuros. Quando não especificado, Vp = 0"
			},
			{
				name: "tipo",
				description: "é o valor que representa o vencimento do pagamento; pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			}
		]
	},
	{
		name: "VFPLANO",
		description: "Retorna o valor futuro de um capital inicial depois de ter sido aplicada uma série de taxas de juros compostos.",
		arguments: [
			{
				name: "capital",
				description: "é o valor presente"
			},
			{
				name: "plano",
				description: "é uma matriz de taxas de juros a ser aplicada"
			}
		]
	},
	{
		name: "VP",
		description: "Retorna o valor presente de um investimento: a quantia total atual de uma série de pagamentos futuros.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de juros por período. Por exemplo, use 6%/4 para pagamentos trimestrais a uma taxa de 6% TPA"
			},
			{
				name: "per",
				description: "é o número total de períodos de pagamento em um investimento"
			},
			{
				name: "pgto",
				description: "é o pagamento efetuado a cada período, não podendo ser alterado no decorrer do investimento"
			},
			{
				name: "vf",
				description: "é o valor futuro ou um saldo em dinheiro que se deseja obter após o último pagamento ter sido efetuado"
			},
			{
				name: "tipo",
				description: "é um valor lógico: pagamento no início do período = 1; pagamento ao final do período = 0 ou não especificado"
			}
		]
	},
	{
		name: "VPL",
		description: "Retorna o valor líquido atual de um investimento, com base em uma taxa de desconto e uma série de pagamentos futuros (valores negativos) e renda (valores positivos).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de desconto durante um período"
			},
			{
				name: "valor1",
				description: "de 1 a 254 pagamentos e rendas, distribuídos em espaços iguais, e que ocorrem ao final de cada período"
			},
			{
				name: "valor2",
				description: "de 1 a 254 pagamentos e rendas, distribuídos em espaços iguais, e que ocorrem ao final de cada período"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Retorna a distribuição Weibull.",
		arguments: [
			{
				name: "x",
				description: "é o valor no qual se avalia a função, um número não negativo"
			},
			{
				name: "alfa",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "beta",
				description: "é o parâmetro da distribuição, um número positivo"
			},
			{
				name: "cumulativo",
				description: "é um valor lógico: para a função de distribuição cumulativa, use VERDADEIRO; para a função de probabilidade de massa, use FALSO"
			}
		]
	},
	{
		name: "XOR",
		description: "Retorna uma lógica 'Exclusivo Ou' de todos os argumentos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "lógico1",
				description: "são de 1 a 254 condições que você deseja testar e que podem ser VERDADEIRAS ou FALSAS e podem ser valores lógicos, matrizes ou referências"
			},
			{
				name: "lógico2",
				description: "são de 1 a 254 condições que você deseja testar e que podem ser VERDADEIRAS ou FALSAS e podem ser valores lógicos, matrizes ou referências"
			}
		]
	},
	{
		name: "XTIR",
		description: "Retorna a taxa de retorno interna de um cronograma de fluxos de caixa.",
		arguments: [
			{
				name: "valores",
				description: "é uma série de fluxos de caixa que corresponde a um cronograma de pagamentos em datas"
			},
			{
				name: "datas",
				description: "é o cronograma de datas de pagamento que corresponde aos pagamentos de fluxo de caixa"
			},
			{
				name: "estimativa",
				description: "é um número que se estima ser próximo ao resultado de XTIR"
			}
		]
	},
	{
		name: "XVPL",
		description: "Retorna o valor presente líquido de um cronograma de fluxos de caixa.",
		arguments: [
			{
				name: "taxa",
				description: "é a taxa de desconto a ser aplicada nos fluxos de caixa"
			},
			{
				name: "valores",
				description: "é uma série de fluxos de caixa que corresponde a um cronograma de pagamentos em datas"
			},
			{
				name: "datas",
				description: "é o cronograma de datas de pagamento que corresponde aos pagamentos de fluxo de caixa"
			}
		]
	}
];