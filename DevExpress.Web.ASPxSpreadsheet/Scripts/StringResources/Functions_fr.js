ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Renvoie la valeur absolue d'un nombre, un nombre sans son signe.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre réel dont vous voulez obtenir la valeur absolue"
			}
		]
	},
	{
		name: "ACOS",
		description: "Renvoie l'arccosinus d'un nombre exprimé en radians, de 0 à pi. L'arccosinus est l'angle dont le cosinus est ce nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est le cosinus de l'angle que vous voulez obtenir et doit être compris entre -1 et 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Renvoie le cosinus hyperbolique inverse d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel supérieur ou égal à 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Renvoie l'arccotangente d'un nombre, en radians, dans la plage 0 à Pi.",
		arguments: [
			{
				name: "nombre",
				description: "est la cotangente de l'angle recherché"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Renvoie la cotangente hyperbolique inverse d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est la cotangente hyperbolique de l'angle recherché"
			}
		]
	},
	{
		name: "ADRESSE",
		description: "Crée une référence de cellule sous forme de texte, en fonction des numéros de lignes et colonnes spécifiées.",
		arguments: [
			{
				name: "no_lig",
				description: "est le numéro de la ligne à utiliser dans la référence de cellule: numéro_cellule = 1 pour la cellule 1"
			},
			{
				name: "no_col",
				description: "est le numéro de la colonne à utiliser dans la référence de cellule. Par exemple, numéro_colonne = 4 pour la colonne D"
			},
			{
				name: "no_abs",
				description: "spécifie le type de référence à renvoyer: absolue = 1; ligne absolue/colonne relative = 2; ligne relative/colonne absolue = 3; relative = 4"
			},
			{
				name: "a1",
				description: "est une valeur logique qui spécifie le style de référence: style A1 = 1 ou VRAI; style L1C1 = 0 ou FAUX"
			},
			{
				name: "feuille_texte",
				description: "est le texte donnant le nom de la feuille de calcul à utiliser comme référence externe"
			}
		]
	},
	{
		name: "ALEA",
		description: "Renvoie un nombre aléatoire de distribution normale supérieur ou égal à 0 et inférieur à 1 (différent à chaque calcul).",
		arguments: [
		]
	},
	{
		name: "ALEA.ENTRE.BORNES",
		description: "Renvoie un nombre aléatoire entre les nombres que vous spécifiez.",
		arguments: [
			{
				name: "min",
				description: "est la valeur minimale que peut donner la fonction ALEA.ENTRE.BORNES"
			},
			{
				name: "max",
				description: "est la valeur maximale que peut donner la fonction ALEA.ENTRE.BORNES"
			}
		]
	},
	{
		name: "AMORLIN",
		description: "Calcule l'amortissement linéaire d'un bien pour une période donnée.",
		arguments: [
			{
				name: "coût",
				description: "est le coût initial du bien"
			},
			{
				name: "valeur_rés",
				description: "est la valeur résiduelle du bien ou valeur du bien à la fin de sa vie"
			},
			{
				name: "durée",
				description: "est la durée de vie utile du bien ou le nombre de périodes au cours desquelles le bien est amorti"
			}
		]
	},
	{
		name: "ANNEE",
		description: "Renvoie l’année, un entier entre 1900 et 9999.",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet"
			}
		]
	},
	{
		name: "ARRONDI",
		description: "Arrondit un nombre au nombre de chiffres indiqué.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à arrondir"
			},
			{
				name: "no_chiffres",
				description: "est le nombre de chiffres auquel vous voulez arrondir l'argument nombre. Arrondis négatifs à la gauche de la décimale; de zéro à l'entier le plus proche"
			}
		]
	},
	{
		name: "ARRONDI.AU.MULTIPLE",
		description: "Donne l'arrondi d'un nombre au multiple spécifié.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur à arrondir"
			},
			{
				name: "multiple",
				description: "est le multiple auquel le nombre doit être arrondi"
			}
		]
	},
	{
		name: "ARRONDI.INF",
		description: "Arrondit un nombre en tendant vers zéro.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel que vous voulez arrondir"
			},
			{
				name: "no_chiffres",
				description: "est le nombre de chiffres auxquels vous voulez arrondir. Arrondir négativement à la gauche de la décimale; zéro ou omis, arrondis au nombre entier le plus proche"
			}
		]
	},
	{
		name: "ARRONDI.SUP",
		description: "Arrondit un nombre en s'éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel que vous voulez arrondir"
			},
			{
				name: "no_chiffres",
				description: "est le nombre de chiffres auxquels vous voulez arrondir. Arrondir négativement à la gauche de la décimale; zéro ou omis, arrondis au nombre entier le plus proche"
			}
		]
	},
	{
		name: "ASIN",
		description: "Renvoie l'arcsinus d'un nombre en radians, dans la plage -Pi/2 à +Pi/2.",
		arguments: [
			{
				name: "nombre",
				description: "est le sinus de l'angle souhaité et doit être compris entre -1 et 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Renvoie le sinus hyperbolique inverse d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel supérieur ou égal à 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Renvoie l'arctangente d'un nombre en radians, dans la plage -Pi/2 à Pi/2.",
		arguments: [
			{
				name: "nombre",
				description: "est la tangente de l'angle voulu"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Renvoie l'arctangente des coordonnées x et y, en radians entre -Pi et Pi, -Pi étant exclu.",
		arguments: [
			{
				name: "no_x",
				description: "est la coordonnée x du point"
			},
			{
				name: "no_y",
				description: "est la coordonnée y du point"
			}
		]
	},
	{
		name: "ATANH",
		description: "Renvoie la tangente hyperbolique inverse d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel compris entre -1 et 1 (-1 et 1 exclus)"
			}
		]
	},
	{
		name: "AUJOURDHUI",
		description: "Renvoie la date du jour au format de date.",
		arguments: [
		]
	},
	{
		name: "AVERAGEA",
		description: "Renvoie la moyenne (moyenne arithmétique) de ses arguments, en considérant que le texte et la valeur logique FAUX dans les arguments = 0 ; VRAI = 1. Les arguments peuvent être des nombres, des noms, des matrices ou des références.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 arguments dont vous voulez obtenir la moyenne"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 arguments dont vous voulez obtenir la moyenne"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Convertit un nombre en texte (baht).",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre que vous voulez convertir"
			}
		]
	},
	{
		name: "BASE",
		description: "Convertit un nombre en représentation textuelle avec la base donnée.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "base",
				description: "est la base dans laquelle vous voulez convertir le nombre"
			},
			{
				name: "longueur_mini",
				description: "est la longueur minimale de la chaîne renvoyée. Sinon, des zéros non significatifs ne sont pas ajoutés"
			}
		]
	},
	{
		name: "BDECARTYPE",
		description: "Évalue l'écart-type à partir d'un échantillon de population représenté par les entrées de base de données sélectionnées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDECARTYPEP",
		description: "Calcule l'écart-type à partir de la population entière représentée par les entrées de base de données sélectionnées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions que vous avez spécifiées. Cette plage inclut une étiquette de colonne et l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDLIRE",
		description: "Extrait d'une base de données l'enregistrement qui correspond aux conditions spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDMAX",
		description: "Donne la valeur la plus élevée dans le champ (colonne) des enregistrements de la base de données correspondant aux conditions que vous avez spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDMIN",
		description: "Donne la valeur la moins élevée du champ (colonne) d'enregistrements de la base de données correspondant aux conditions définies.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la base de données ou la liste. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions que vous avez spécifiées. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDMOYENNE",
		description: "Donne la moyenne des valeurs dans la colonne d'une liste ou une base de données qui correspondent aux conditions que vous avez spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDNB",
		description: "Compte le nombre de cellules contenant des valeurs numériques satisfaisant les critères spécifiés pour la base de données précisée.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDNBVAL",
		description: "Compte le nombre de cellules non vides dans le champ (colonne) d'enregistrements dans la base de données correspondant aux conditions spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDPRODUIT",
		description: "Multiplie les valeurs dans le champ (colonne) d'enregistrements de la base de données correspondant aux conditions que vous avez spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDSOMME",
		description: "Additionne les nombres se trouvant dans le champ (colonne) d'enregistrements de la base de données correspondant aux conditions que vous avez spécifiées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDVAR",
		description: "Évalue la variance à partir d'un échantillon de population représenté par des entrées de base de données sélectionnées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BDVARP",
		description: "Calcule la variance à partir de la population entière représentée par les entrées de base de données sélectionnées.",
		arguments: [
			{
				name: "base_de_données",
				description: "est la plage de cellules qui constitue la liste ou la base de données. Une base de données est une liste de données connexes"
			},
			{
				name: "champ",
				description: "indique soit l'étiquette de la colonne entre guillemets, soit un nombre représentant la position de la colonne dans la liste"
			},
			{
				name: "critères",
				description: "est la plage de cellules qui contient les conditions. Cette plage inclut une étiquette de colonne et une cellule en dessous de l'étiquette de la condition"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Renvoie la fonction de Bessel modifiée In(x).",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction est évaluée"
			},
			{
				name: "n",
				description: "est l'ordre de la fonction de Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Renvoie la fonction de Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction est évaluée"
			},
			{
				name: "n",
				description: "est l'ordre de la fonction de Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Renvoie la fonction de Bessel modifiée Kn(x).",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction est évaluée"
			},
			{
				name: "n",
				description: "est l'ordre de la fonction"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Renvoie la fonction de Bessel modifiée Yn(x).",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction est évaluée"
			},
			{
				name: "n",
				description: "est l'ordre de la fonction"
			}
		]
	},
	{
		name: "BETA.INVERSE",
		description: "Renvoie l’inverse de la fonction de densité de distribution de la probabilité suivant une loi bêta cumulée (LOI.BÊTA).",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité associée à la distribution Bêta"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "A",
				description: "est une limite inférieure facultative de l’intervalle comprenant x. Si omis, A = 0"
			},
			{
				name: "B",
				description: "est une limite supérieure facultative de l’intervalle comprenant x. Si omis, B = 1"
			}
		]
	},
	{
		name: "BETA.INVERSE.N",
		description: "Renvoie l’inverse de la fonction de densité de distribution de la probabilité suivant une loi bêta cumulée (LOI.BETA.N).",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité associée à la distribution Bêta"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "A",
				description: "est une limite inférieure facultative de l’intervalle comprenant x. Si omis, A = 0"
			},
			{
				name: "B",
				description: "est une limite supérieure facultative de l’intervalle comprenant x. Si omis, B = 1"
			}
		]
	},
	{
		name: "BINDEC",
		description: "Convertit un nombre binaire en nombre décimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre binaire à convertir"
			}
		]
	},
	{
		name: "BINHEX",
		description: "Convertit un nombre binaire en nombre hexadécimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre binaire à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "BINOCT",
		description: "Convertit un nombre binaire en nombre octal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre binaire à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "BITDECALD",
		description: "Renvoie un nombre décalé vers la droite de total_décalage bits.",
		arguments: [
			{
				name: "nombre",
				description: "est la représentation décimale du nombre binaire à calculer"
			},
			{
				name: "total_décalé",
				description: "est le nombre de bits par lequel vous voulez décaler le nombre vers la droite"
			}
		]
	},
	{
		name: "BITDECALG",
		description: "Renvoie un nombre décalé vers la gauche de total_décalage bits.",
		arguments: [
			{
				name: "nombre",
				description: "est la représentation décimale du nombre binaire à calculer"
			},
			{
				name: "total_décalé",
				description: "est le nombre de bits par lequel vous voulez décaler le nombre vers la gauche"
			}
		]
	},
	{
		name: "BITET",
		description: "Renvoie le résultat binaire « Et » de deux nombres.",
		arguments: [
			{
				name: "nombre1",
				description: "est la représentation décimale du nombre binaire à calculer"
			},
			{
				name: "nombre2",
				description: "est la représentation décimale du nombre binaire à calculer"
			}
		]
	},
	{
		name: "BITOU",
		description: "Renvoie le résultat binaire « Ou » de deux nombres.",
		arguments: [
			{
				name: "nombre1",
				description: "est la représentation décimale du nombre binaire à calculer"
			},
			{
				name: "nombre2",
				description: "est la représentation décimale du nombre binaire à calculer"
			}
		]
	},
	{
		name: "BITOUEXCLUSIF",
		description: "Renvoie Renvoie le résultat binaire « Ou exclusif » de deux nombres.",
		arguments: [
			{
				name: "nombre1",
				description: "est la représentation décimale du nombre binaire à calculer"
			},
			{
				name: "nombre2",
				description: "est la représentation décimale du nombre binaire à calculer"
			}
		]
	},
	{
		name: "CAR",
		description: "Renvoie le caractère spécifié par le code numérique du jeu de caractères de votre ordinateur.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre compris entre 1 et 255, indiquant le caractère que vous voulez obtenir"
			}
		]
	},
	{
		name: "CELLULE",
		description: "Renvoie des informations sur la mise en forme, l'emplacement ou le contenu de la première cellule, selon l'ordre de lecture de la feuille, d'une référence.",
		arguments: [
			{
				name: "type_info",
				description: "est une valeur de texte qui indique le type d'informations de cellule voulu."
			},
			{
				name: "référence",
				description: "est la cellule sur laquelle vous souhaitez obtenir des informations"
			}
		]
	},
	{
		name: "CENTILE",
		description: "Renvoie le k-ième centile des valeurs d’une plage.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données définissant l’étendue relative"
			},
			{
				name: "k",
				description: "représente le centile ; celui-ci doit être compris entre 0 et 1 inclus"
			}
		]
	},
	{
		name: "CENTILE.EXCLURE",
		description: "Renvoie le k-ième centile des valeurs d’une plage, où k se trouve dans la plage de 0 à 1, non compris.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données définissant l’étendue relative"
			},
			{
				name: "k",
				description: "représente le centile qui doit être compris entre 0 et 1 inclus"
			}
		]
	},
	{
		name: "CENTILE.INCLURE",
		description: "Renvoie le k-ième centile des valeurs d’une plage, où k se trouve dans la plage de 0 à 1, compris.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données définissant l’étendue relative"
			},
			{
				name: "k",
				description: "représente le centile qui doit être compris entre 0 et 1 inclus"
			}
		]
	},
	{
		name: "CENTREE.REDUITE",
		description: "Renvoie une valeur centrée réduite, depuis une distribution caractérisée par une moyenne et un écart-type.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à centrer et à réduire"
			},
			{
				name: "espérance",
				description: "représente l'espérance mathématique de la distribution"
			},
			{
				name: "écart_type",
				description: "représente l'écart-type de la distribution, un nombre positif"
			}
		]
	},
	{
		name: "CHERCHE",
		description: "Renvoie le numéro du caractère au niveau duquel est trouvé un caractère ou le début d'une chaîne de caractères, en lisant de la gauche à droite (pas de distinction entre majuscules et minuscules).",
		arguments: [
			{
				name: "texte_cherché",
				description: "est le texte à trouver. Vous pouvez utiliser les caractères génériques ? et *; utilisez ~? et ~* pour trouver les caractères ? et *."
			},
			{
				name: "texte",
				description: "est le texte comprenant la chaîne de texte à trouver"
			},
			{
				name: "no_départ",
				description: "indique le numéro du caractère dans l'argument texte à partir duquel la recherche doit débuter (en comptant à partir de la gauche). 1 est utilisé par défaut"
			}
		]
	},
	{
		name: "CHIFFRE.ARABE",
		description: "Convertit un chiffre romain en un chiffre arabe.",
		arguments: [
			{
				name: "texte",
				description: "est le chiffre romain à convertir"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Renvoie le test d’indépendance : la valeur de la distribution Khi-deux pour la statistique et les degrés de liberté appropriés.",
		arguments: [
			{
				name: "plage_réelle",
				description: "représente la plage de données contenant les observations à tester par rapport aux valeurs attendues"
			},
			{
				name: "plage_prévue",
				description: "représente la plage de données contenant le rapport du produit des totaux de ligne et colonne avec le total général"
			}
		]
	},
	{
		name: "CHOISIR",
		description: "Choisit une valeur ou une action à réaliser dans une liste de valeurs, en fonction d'un numéro d'index.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "no_index",
				description: "spécifie l'argument valeur sélectionné. No_index doit être compris entre 1 et 254, ou bien être une formule ou une référence à un numéro entre 1 et 254"
			},
			{
				name: "valeur1",
				description: "représentent de 1 à 254 nombres, références de cellules, noms définis, formules, fonctions, ou arguments sous forme de texte parmi lesquels la fonction CHOISIR sélectionne"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 254 nombres, références de cellules, noms définis, formules, fonctions, ou arguments sous forme de texte parmi lesquels la fonction CHOISIR sélectionne"
			}
		]
	},
	{
		name: "CNUM",
		description: "Convertit une chaîne textuelle représentant un nombre en un nombre.",
		arguments: [
			{
				name: "texte",
				description: "est le texte placé entre guillemets ou la référence à une cellule contenant le texte que vous voulez convertir"
			}
		]
	},
	{
		name: "CODE",
		description: "Renvoie le numéro de code du premier caractère du texte, dans le jeu de caractères utilisé par votre ordinateur.",
		arguments: [
			{
				name: "texte",
				description: "est le texte dont vous voulez obtenir le code du premier caractère"
			}
		]
	},
	{
		name: "COEFFICIENT.ASYMETRIE",
		description: "Renvoie l'asymétrie d'une distribution : la caractérisation du degré d'asymétrie d'une distribution par rapport à sa moyenne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent les nombres dont vous voulez calculer l'asymétrie"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent les nombres dont vous voulez calculer l'asymétrie"
			}
		]
	},
	{
		name: "COEFFICIENT.ASYMETRIE.P",
		description: "Renvoie l'asymétrie d'une distribution basée sur une population : la caractérisation du degré d'asymétrie d'une distribution par rapport à sa moyenne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 254 nombres, noms, matrices ou références qui contiennent les nombres dont vous voulez calculer l'asymétrie de la population"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 254 nombres, noms, matrices ou références qui contiennent les nombres dont vous voulez calculer l'asymétrie de la population"
			}
		]
	},
	{
		name: "COEFFICIENT.CORRELATION",
		description: "Renvoie le coefficient de corrélation entre deux séries de données.",
		arguments: [
			{
				name: "matrice1",
				description: "représente une plage de cellules de valeurs. Les valeurs doivent être un nombre, un nom, une matrice ou une référence qui contient des nombres"
			},
			{
				name: "matrice2",
				description: "représente une seconde plage de cellules de valeurs. Les valeurs doivent être un nombre, un nom, une matrice ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "COEFFICIENT.DETERMINATION",
		description: "Renvoie la valeur du coefficient de détermination R^2 d'une régression linéaire.",
		arguments: [
			{
				name: "y_connus",
				description: "représente une matrice ou une plage d'observations et peut être des nombres, des noms, des matrices, ou des références contenant des nombres"
			},
			{
				name: "x_connus",
				description: "représente une matrice ou une plage d'observations et peut être des nombres, des noms, des matrices, ou des références contenant des nombres"
			}
		]
	},
	{
		name: "COLONNE",
		description: "Renvoie le numéro de colonne d'une référence.",
		arguments: [
			{
				name: "référence",
				description: "est la cellule ou la plage de cellules dont vous voulez obtenir le numéro de colonne. Sinon, la cellule contenant la fonction COLONNE est utilisée"
			}
		]
	},
	{
		name: "COLONNES",
		description: "Renvoie le nombre de colonnes d'une matrice ou d'une référence.",
		arguments: [
			{
				name: "tableau",
				description: "est un tableau, une formule matricielle ou la référence d'une plage de cellules dont vous voulez obtenir le nombre de colonnes"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Renvoie le nombre de combinaisons que l'on peut former avec un nombre donné d'éléments. Consultez l'aide pour l'équation utilisée.",
		arguments: [
			{
				name: "nombre_éléments",
				description: "représente le nombre total d'éléments"
			},
			{
				name: "nb_éléments_choisis",
				description: "représente le nombre d'éléments de chaque combinaison"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Renvoie le nombre de combinaisons avec répétitions que l'on peut former avec un nombre donné d'éléments.",
		arguments: [
			{
				name: "nombre",
				description: "représente le nombre total d'éléments"
			},
			{
				name: "nombre_choisi",
				description: "représente le nombre d'éléments de chaque combinaison"
			}
		]
	},
	{
		name: "COMPLEXE",
		description: "Renvoie un nombre complexe construit à partir de ses parties réelle et imaginaire.",
		arguments: [
			{
				name: "partie_réelle",
				description: "est la partie réelle du nombre complexe"
			},
			{
				name: "partie_imaginaire",
				description: "est la partie imaginaire du nombre complexe"
			},
			{
				name: "suffixe",
				description: "est le symbole utilisé pour désigner le vecteur imaginaire"
			}
		]
	},
	{
		name: "COMPLEXE.ARGUMENT",
		description: "Renvoie l'argument thêta, un angle exprimé en radians.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer l'argument"
			}
		]
	},
	{
		name: "COMPLEXE.CONJUGUE",
		description: "Renvoie le conjugué d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer le conjugué"
			}
		]
	},
	{
		name: "COMPLEXE.COS",
		description: "Renvoie le cosinus d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer le cosinus"
			}
		]
	},
	{
		name: "COMPLEXE.COSH",
		description: "Renvoie le cosinus hyperbolique d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir le cosinus hyperbolique"
			}
		]
	},
	{
		name: "COMPLEXE.COT",
		description: "Renvoie la cotangente d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la cotangente"
			}
		]
	},
	{
		name: "COMPLEXE.CSC",
		description: "Renvoie la cosécante d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la cosécante"
			}
		]
	},
	{
		name: "COMPLEXE.CSCH",
		description: "Renvoie la cosécante hyperbolique d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la cosécante hyperbolique"
			}
		]
	},
	{
		name: "COMPLEXE.DIFFERENCE",
		description: "Calcule la différence entre deux nombres complexes.",
		arguments: [
			{
				name: "nombre_complexe1",
				description: "est le nombre complexe duquel on veut soustraire le nombre_complexe2"
			},
			{
				name: "nombre_complexe2",
				description: "est le nombre complexe à soustraire au nombre_complexe1"
			}
		]
	},
	{
		name: "COMPLEXE.DIV",
		description: "Renvoie le quotient de deux nombres complexes.",
		arguments: [
			{
				name: "nombre_complexe1",
				description: "est le numérateur complexe"
			},
			{
				name: "nombre_complexe2",
				description: "est le diviseur complexe"
			}
		]
	},
	{
		name: "COMPLEXE.EXP",
		description: "Donne l'exposant d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: " est un nombre complexe dont vous voulez calculer l'exposant"
			}
		]
	},
	{
		name: "COMPLEXE.IMAGINAIRE",
		description: "Renvoie la partie imaginaire d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous désirez extraire la partie imaginaire"
			}
		]
	},
	{
		name: "COMPLEXE.LN",
		description: "Donne le logarithme népérien d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir le logarithme népérien"
			}
		]
	},
	{
		name: "COMPLEXE.LOG10",
		description: "Calcule le logarithme en base 10 d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous souhaitez obtenir le logarithme en base 10"
			}
		]
	},
	{
		name: "COMPLEXE.LOG2",
		description: "Calcule le logarithme en base 2 d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer le logarithme en base 2"
			}
		]
	},
	{
		name: "COMPLEXE.MODULE",
		description: "Renvoie le module d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer le module"
			}
		]
	},
	{
		name: "COMPLEXE.PRODUIT",
		description: "Renvoie le produit de 1 à 255 nombres complexes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nb_comp1",
				description: "nb_comp1, nb_comp2,... représentent de 1 à 255 nombres complexes à multiplier."
			},
			{
				name: "nb_comp2",
				description: "nb_comp1, nb_comp2,... représentent de 1 à 255 nombres complexes à multiplier."
			}
		]
	},
	{
		name: "COMPLEXE.PUISSANCE",
		description: "Renvoie la valeur du nombre complexe élevé à une puissance entière.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe à élever à la puissance donnée"
			},
			{
				name: "nombre",
				description: "est la puissance à laquelle le nombre complexe doit être élevé"
			}
		]
	},
	{
		name: "COMPLEXE.RACINE",
		description: "Extrait la racine carrée d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez extraire la racine carrée"
			}
		]
	},
	{
		name: "COMPLEXE.REEL",
		description: "Renvoie la partie réelle d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la partie réelle"
			}
		]
	},
	{
		name: "COMPLEXE.SEC",
		description: "Renvoie la sécante d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la sécante"
			}
		]
	},
	{
		name: "COMPLEXE.SECH",
		description: "Renvoie la sécante hyperbolique d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la sécante hyperbolique"
			}
		]
	},
	{
		name: "COMPLEXE.SIN",
		description: "Renvoie le sinus d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez calculer le sinus"
			}
		]
	},
	{
		name: "COMPLEXE.SINH",
		description: "Renvoie le sinus hyperbolique d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir le sinus hyperbolique"
			}
		]
	},
	{
		name: "COMPLEXE.SOMME",
		description: "Renvoie la somme de nombres complexes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nb_comp1",
				description: "représentent de 1 à 255 nombres complexes à ajouter"
			},
			{
				name: "nb_comp2",
				description: "représentent de 1 à 255 nombres complexes à ajouter"
			}
		]
	},
	{
		name: "COMPLEXE.TAN",
		description: "Renvoie la tangente d'un nombre complexe.",
		arguments: [
			{
				name: "nombre_complexe",
				description: "est le nombre complexe dont vous voulez obtenir la tangente"
			}
		]
	},
	{
		name: "CONCATENER",
		description: "Assemble plusieurs chaînes de caractères de façon à n'en former qu'une.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "texte1",
				description: "sont de 1 à 255 chaînes de caractères à concaténer. Il peut s'agir de chaînes, nombres ou références à des cellules uniques"
			},
			{
				name: "texte2",
				description: "sont de 1 à 255 chaînes de caractères à concaténer. Il peut s'agir de chaînes, nombres ou références à des cellules uniques"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Convertit un nombre d'une unité à une autre unité.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "de_unité",
				description: "est l'unité du nombre à convertir"
			},
			{
				name: "à_unité",
				description: "est l'unité du résultat"
			}
		]
	},
	{
		name: "COS",
		description: "Renvoie le cosinus d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous cherchez le cosinus"
			}
		]
	},
	{
		name: "COSH",
		description: "Renvoie le cosinus hyperbolique d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel"
			}
		]
	},
	{
		name: "COT",
		description: "Renvoie la cotangente d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la cotangente"
			}
		]
	},
	{
		name: "COTH",
		description: "Renvoie la cotangente hyperbolique d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la cotangente hyperbolique"
			}
		]
	},
	{
		name: "COVARIANCE",
		description: "Renvoie la covariance, moyenne du produit des écarts à la moyenne de chaque paire de points de deux séries.",
		arguments: [
			{
				name: "matrice1",
				description: "est la première plage de cellules contenant des entiers et doit être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "matrice2",
				description: "est la seconde plage de cellules contenant des entiers et doit être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "COVARIANCE.PEARSON",
		description: "Renvoie la covariance de population, moyenne du produit des écarts à la moyenne de chaque paire de points de deux séries.",
		arguments: [
			{
				name: "matrice1",
				description: "est la première plage de cellules contenant des entiers et doit être un nombre, une matrice ou une référence qui contient des nombres"
			},
			{
				name: "matrice2",
				description: "est la seconde plage de cellules contenant des entiers et doit être un nombre, une matrice ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "COVARIANCE.STANDARD",
		description: "Renvoie la covariance d’échantillon, moyenne du produit des écarts à la moyenne de chaque paire de points de deux séries.",
		arguments: [
			{
				name: "matrice1",
				description: "est la première plage de cellules contenant des entiers et doit être un nombre, une matrice ou une référence qui contient des nombres"
			},
			{
				name: "matrice2",
				description: "est la seconde plage de cellules contenant des entiers et doit être un nombre, une matrice ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "CRITERE.LOI.BINOMIALE",
		description: "Renvoie la plus petite valeur pour laquelle la distribution binomiale cumulée est supérieure ou égale à une valeur critère.",
		arguments: [
			{
				name: "tirages",
				description: "représente le nombre de tirages de Bernoulli"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité de succès à chaque tirage, un nombre entre 0 et 1 compris"
			},
			{
				name: "alpha",
				description: "représente la valeur critère, un nombre entre 0 et 1 compris"
			}
		]
	},
	{
		name: "CROISSANCE",
		description: "Calcule les valeurs de la tendance géométrique exponentielle à partir de valeurs connues.",
		arguments: [
			{
				name: "y_connus",
				description: "est la série des valeurs y déjà connues par la relation y = b*m ^ x, une matrice ou plage de nombres positifs"
			},
			{
				name: "x_connus",
				description: "est une série de valeurs x facultatives, éventuellement déjà données par la relation y = b*m ^ x, une matrice ou plage de même taille qu'Y_connus"
			},
			{
				name: "x_nouveaux",
				description: "est la nouvelle série de variables x dont vous voulez que CROISSANCE vous donne les valeurs y correspondantes"
			},
			{
				name: "constante",
				description: "est une valeur logique: la constante b est calculée normalement si Constante = VRAI; b prend la valeur 1 si Constante = FAUX ou omis"
			}
		]
	},
	{
		name: "CSC",
		description: "Renvoie la cosécante d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la cosécante"
			}
		]
	},
	{
		name: "CSCH",
		description: "Renvoie la cosécante hyperbolique d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la cosécante hyperbolique"
			}
		]
	},
	{
		name: "CTXT",
		description: "Arrondit un nombre au nombre de décimales spécifié et renvoie le résultat sous forme de texte avec ou sans virgule.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à arrondir et convertir en texte"
			},
			{
				name: "décimales",
				description: "est le nombre de chiffres à droite de la virgule. Si omis, 2 décimales"
			},
			{
				name: "no_séparateur",
				description: "est une valeur logique: ne pas afficher de virgule dans le texte renvoyé = VRAI; afficher des virgules dans le texte renvoyé = FAUX ou omis"
			}
		]
	},
	{
		name: "CUMUL.INTER",
		description: "Donne le montant cumulé des intérêts payés entre deux périodes données.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt"
			},
			{
				name: "npm",
				description: "est le nombre total de périodes de paiement"
			},
			{
				name: "va",
				description: "est la valeur actuelle"
			},
			{
				name: "période_début",
				description: "est la première période de paiement à utiliser pour le calcul"
			},
			{
				name: "période_fin",
				description: "est la dernière période du paiement à utiliser pour le calcul"
			},
			{
				name: "type",
				description: "est le nombre 0 si les remboursements sont échus à la fin de la période ou le nombre 1 s'ils sont échus au début de la période"
			}
		]
	},
	{
		name: "CUMUL.PRINCPER",
		description: "Donne le montant cumulé du remboursement du capital entre deux périodes données.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt"
			},
			{
				name: "npm",
				description: "est le nombre total de périodes de paiement"
			},
			{
				name: "va",
				description: "est la valeur actuelle"
			},
			{
				name: "période_début",
				description: "est la première période de paiement à utiliser pour le calcul"
			},
			{
				name: "période_fin",
				description: "est la dernière période du paiement à utiliser pour le calcul"
			},
			{
				name: "type",
				description: "est le nombre 0 si les remboursements sont échus à la fin de la période ou le nombre 1 s'ils sont échus au début de la période"
			}
		]
	},
	{
		name: "DATE",
		description: "Renvoie un numéro qui représente la date dans le code de date et d’heure Spreadsheet.",
		arguments: [
			{
				name: "année",
				description: "représente un nombre entre 1900 et 9999 dans Spreadsheet pour Windows ou entre 1904 et 9999 dans Spreadsheet pour le Macintosh"
			},
			{
				name: "mois",
				description: "est un nombre entre 1 et 12 représentant le mois de l’année"
			},
			{
				name: "jour",
				description: "est un nombre entre 1 et 31 représentant le jour du mois"
			}
		]
	},
	{
		name: "DATE.COUPON.PREC",
		description: "Calcule la date du coupon précédant la date de liquidation.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "fréquence",
				description: "est le nombre de coupons payés par an"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "DATE.COUPON.SUIV",
		description: "Détermine la date du coupon suivant la date de liquidation.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "fréquence",
				description: "est le nombre de coupons payés par an"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
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
		name: "DATEVAL",
		description: "Convertit une date donnée sous forme de texte en un numéro représentant la date dans le code de date et d’heure Spreadsheet.",
		arguments: [
			{
				name: "date_texte",
				description: "est du texte représentant une date dans un format de date de Spreadsheet, entre le 01/01/1900 (pour Windows), ou le 01/01/1904 (pour Macintosh), et le 31/12/9999"
			}
		]
	},
	{
		name: "DB",
		description: "Renvoie l'amortissement d'un bien durant une période spécifiée en utilisant la méthode de l'amortissement dégressif à taux fixe.",
		arguments: [
			{
				name: "coût",
				description: "est le coût initial du bien"
			},
			{
				name: "valeur_rés",
				description: "est la valeur résiduelle du bien ou valeur du bien à la fin de sa vie"
			},
			{
				name: "durée",
				description: "est la durée de vie utile du bien ou le nombre de périodes au cours desquelles le bien est amorti "
			},
			{
				name: "période",
				description: "est la période pour laquelle vous souhaitez calculer l'amortissement. Elle doit être exprimée dans la même unité que la durée de vie du bien"
			},
			{
				name: "mois",
				description: "est le nombre de mois de la première année. Si le mois est omis, la valeur par défaut est 12"
			}
		]
	},
	{
		name: "DDB",
		description: "Renvoie l'amortissement d'un bien durant une période spécifiée suivant la méthode de l'amortissement dégressif à taux double ou selon un coefficient à spécifier.",
		arguments: [
			{
				name: "coût",
				description: "est le coût initial du bien"
			},
			{
				name: "valeur_rés",
				description: "est la valeur résiduelle du bien ou valeur du bien à la fin de sa vie"
			},
			{
				name: "durée",
				description: "est la vie utile du bien ou le nombre de périodes au cours desquelles le bien est amorti (appelé aussi durée de vie utile du bien)"
			},
			{
				name: "période",
				description: "est la période pour laquelle vous souhaitez calculer l'amortissement. Elle doit être exprimée dans la même unité que la durée de vie du bien"
			},
			{
				name: "facteur",
				description: "est le taux auquel le solde à amortir décroît. Si cet argument est omis, la valeur par défaut est 2 (amortissement à taux double)"
			}
		]
	},
	{
		name: "DECALER",
		description: "Donne une référence à une plage dont le nombre de colonnes et de lignes est spécifié dans une cellule ou une plage de cellules.",
		arguments: [
			{
				name: "réf",
				description: "est la référence par rapport à laquelle le décalage doit être opéré, une référence à une cellule ou une plage de cellules adjacentes"
			},
			{
				name: "lignes",
				description: "est le nombre de lignes vers le haut ou vers le bas dont la cellule supérieure gauche de la référence renvoyée doit être décalée"
			},
			{
				name: "colonnes",
				description: "est le nombre de colonnes vers la droite ou vers la gauche dont la cellule supérieure gauche de la référence renvoyée doit être décalée"
			},
			{
				name: "hauteur",
				description: "est la hauteur, en nombre de lignes, attendue pour le résultat. Celle-ci est égale à Réf si omise"
			},
			{
				name: "largeur",
				description: "est la largeur, en nombre de colonnes, attendue pour le résultat. Celle-ci est égale à Réf si omise"
			}
		]
	},
	{
		name: "DECBIN",
		description: "Convertit un nombre décimal en nombre binaire.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "DECHEX",
		description: "Convertit un nombre décimal en nombre hexadécimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Convertit la représentation textuelle d'un nombre dans une base donnée en un nombre décimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "base",
				description: "est la base du nombre que vous convertissez"
			}
		]
	},
	{
		name: "DECOCT",
		description: "Convertit un nombre décimal en nombre octal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "DEGRES",
		description: "Convertit des radians en degrés.",
		arguments: [
			{
				name: "angle",
				description: "représente l'angle en radians à convertir"
			}
		]
	},
	{
		name: "DELTA",
		description: "Teste l'égalité de deux nombres.",
		arguments: [
			{
				name: "nombre1",
				description: "est le premier nombre"
			},
			{
				name: "nombre2",
				description: "est le second nombre"
			}
		]
	},
	{
		name: "DETERMAT",
		description: "Renvoie le déterminant d'une matrice.",
		arguments: [
			{
				name: "matrice",
				description: "est une matrice carrée numérique, soit une plage de cellules, soit une constante de matrice"
			}
		]
	},
	{
		name: "DEVISE",
		description: "Convertit un nombre en texte en utilisant le format monétaire.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre, une formule faisant référence à une cellule contenant un nombre ou une formule renvoyant un nombre"
			},
			{
				name: "décimales",
				description: "indique le nombre de chiffres à droite de la virgule. Le nombre est arrondi si nécessaire; sinon, les décimales = 2"
			}
		]
	},
	{
		name: "DROITE",
		description: "Extrait les derniers caractères de la fin d'une chaîne de texte.",
		arguments: [
			{
				name: "texte",
				description: "est la chaîne de texte contenant les caractères à extraire"
			},
			{
				name: "no_car",
				description: "indique le nombre de caractères à extraire, 1 par défaut"
			}
		]
	},
	{
		name: "DROITEREG",
		description: "Renvoie une matrice qui décrit une droite de corrélation pour vos données, calculée avec la méthode des moindres carrés.",
		arguments: [
			{
				name: "y_connus",
				description: "est la série des valeurs y déjà déterminées par la relation y = m x + b"
			},
			{
				name: "x_connus",
				description: "est une série de valeurs x facultatives, déjà déterminées dans la relation y = m x + b"
			},
			{
				name: "constante",
				description: "est une valeur logique: la constante b est calculée normalement si Constante = VRAI ou omise; b est égale à 0 si Constante = FAUX"
			},
			{
				name: "statistiques",
				description: "est une valeur logique: renvoyer les statistiques de régression complémentaires = VRAI; renvoyer les coefficients m ou la constante b = FAUX ou omis"
			}
		]
	},
	{
		name: "ECART.MOYEN",
		description: "Renvoie la moyenne des écarts absolus des points de données par rapport à leur moyenne arithmétique. Les arguments peuvent être des nombres, des noms, des matrices ou des références qui contiennent des nombres.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments pour lesquels vous voulez calculer la moyenne des écarts absolus"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments pour lesquels vous voulez calculer la moyenne des écarts absolus"
			}
		]
	},
	{
		name: "ECARTYPE",
		description: "Évalue l’écart-type d’une population en se basant sur un échantillon (ignore les valeurs logiques et le texte de l’échantillon).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres correspondant à un échantillon de population, et peuvent être des nombres ou des références qui contiennent des nombres"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres correspondant à un échantillon de population, et peuvent être des nombres ou des références qui contiennent des nombres"
			}
		]
	},
	{
		name: "ECARTYPE.PEARSON",
		description: "Calcule l’écart-type d’une population entière sous forme d’arguments (ignore les valeurs logiques et le texte).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres correspondant à une population et peuvent être des nombres ou des références contenant des nombres"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres correspondant à une population et peuvent être des nombres ou des références contenant des nombres"
			}
		]
	},
	{
		name: "ECARTYPE.STANDARD",
		description: "Évalue l’écart-type d’une population en se basant sur un échantillon (ignore les valeurs logiques et le texte de l’échantillon).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres correspondant à un échantillon de population et peuvent être des nombres ou des références qui contiennent des nombres"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres correspondant à un échantillon de population et peuvent être des nombres ou des références qui contiennent des nombres"
			}
		]
	},
	{
		name: "ECARTYPEP",
		description: "Calcule l’écart-type d’une population entière sous forme d’arguments (en ignorant les valeurs logiques et le texte).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres correspondant à une population, et peuvent être des nombres ou des références contenant des nombres"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres correspondant à une population, et peuvent être des nombres ou des références contenant des nombres"
			}
		]
	},
	{
		name: "ENT",
		description: "Arrondit un nombre à l'entier immédiatement inférieur.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre réel que vous voulez arrondir à l'entier immédiatement inférieur"
			}
		]
	},
	{
		name: "EPURAGE",
		description: "Supprime tous les caractères de contrôle du texte.",
		arguments: [
			{
				name: "texte",
				description: "représente toute information de feuille de calcul dont vous voulez supprimer les caractères qui ne doivent pas apparaître à l'impression"
			}
		]
	},
	{
		name: "EQUATION.RANG",
		description: "Renvoie le rang d’un nombre dans une liste d’arguments : sa taille est relative aux autres valeurs de la liste ; si plusieurs valeurs sont associées au même rang, renvoie le rang supérieur de ce jeu de valeurs.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre dont vous voulez connaître le rang"
			},
			{
				name: "référence",
				description: "est une matrice, ou une référence à une liste de nombres. Les valeurs non numériques sont ignorées dans la référence"
			},
			{
				name: "ordre",
				description: "est un numéro : le rang de l’argument dans la liste triée par ordre décroissant = 0 ou omis ; son rang dans la liste triée par ordre croissant = toute valeur différente de zéro"
			}
		]
	},
	{
		name: "EQUIV",
		description: "Renvoie la position relative d'un élément dans une matrice qui correspond à une valeur spécifique dans un ordre spécifique.",
		arguments: [
			{
				name: "valeur_cherchée",
				description: "est la valeur utilisée pour trouver la valeur voulue dans la matrice, un nombre, du texte, une valeur logique ou la référence à une de ces valeurs"
			},
			{
				name: "tableau_recherche",
				description: "est une plage de cellules adjacentes contenant les valeurs d'équivalence possibles, une matrice de valeurs ou la référence à une matrice"
			},
			{
				name: "type",
				description: "représente le nombre 1, 0 ou -1 indiquant la valeur à renvoyer."
			}
		]
	},
	{
		name: "ERF",
		description: "Renvoie la fonction d'erreur.",
		arguments: [
			{
				name: "limite_inf",
				description: "est la borne d'intégration inférieure"
			},
			{
				name: "limite_sup",
				description: "est la borne d'intégration supérieure"
			}
		]
	},
	{
		name: "ERF.PRECIS",
		description: "Renvoie la fonction d’erreur.",
		arguments: [
			{
				name: "X",
				description: "est la borne d’intégration de ERF.PRECIS inférieure"
			}
		]
	},
	{
		name: "ERFC",
		description: "Renvoie la fonction d'erreur complémentaire.",
		arguments: [
			{
				name: "x",
				description: "est la borne d'intégration inférieure"
			}
		]
	},
	{
		name: "ERFC.PRECIS",
		description: "Renvoie la fonction d’erreur complémentaire.",
		arguments: [
			{
				name: "X",
				description: "est la borne d’intégration de ERFC.PRECIS inférieure"
			}
		]
	},
	{
		name: "ERREUR.TYPE.XY",
		description: "Renvoie l'erreur type de la valeur y prévue pour chaque x de la régression.",
		arguments: [
			{
				name: "y_connus",
				description: "représente une matrice ou une plage d'observations dépendantes et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "x_connus",
				description: "représente une matrice ou une plage d'observations indépendantes, et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "EST.IMPAIR",
		description: "Renvoie VRAI si le nombre est impair.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur dont vous voulez déterminer la parité"
			}
		]
	},
	{
		name: "EST.PAIR",
		description: "Renvoie VRAI si le nombre est pair.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur dont vous voulez déterminer la parité"
			}
		]
	},
	{
		name: "ESTERR",
		description: "Vérifie si une valeur fait référence à une erreur (#VALEUR!, #REF!, #DIV/0!, #NUM!, #NOM? ou #NUL!), sauf #N/A, et renvoie VRAI ou FAUX.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut faire référence à une cellule, une formule ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTERREUR",
		description: "Vérifie si une valeur est une erreur (#N/A, #VALEUR!, #REF!, #DIV/0!, #NUM!, #NOM? ou #NUL!) et renvoie VRAI ou FAUX.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut faire référence à une cellule, une formule ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTFORMULE",
		description: "Vérifie si une référence renvoie à une cellule contenant une formule, et renvoie TRUE ou FALSE.",
		arguments: [
			{
				name: "référence",
				description: "est une référence à la cellule à tester. La référence peut être une référence de cellule, une formule ou un nom qui fait référence à une cellule"
			}
		]
	},
	{
		name: "ESTLOGIQUE",
		description: "Renvoie VRAI si l'argument valeur fait référence à une valeur logique, que ce soit VRAI ou FAUX.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut renvoyer à une cellule, une formule, ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTNA",
		description: "Renvoie VRAI si l'argument valeur fait référence à la valeur d'erreur #N/A (valeur non disponible).",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut renvoyer à une cellule, une formule, ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTNONTEXTE",
		description: "Renvoie VRAI si l'argument valeur fait référence à autre chose que du texte (les cellules vides ne sont pas du texte).",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur que vous voulez tester : une cellule, une formule, ou un nom faisant référence à une cellule, une formule, ou une valeur"
			}
		]
	},
	{
		name: "ESTNUM",
		description: "Contrôle si la valeur est un nombre et renvoie VRAI ou FAUX.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut renvoyer à une cellule, une formule, ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTREF",
		description: "Renvoie VRAI ou FAUX si l'argument valeur est une référence.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut renvoyer à une cellule, une formule, ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTTEXTE",
		description: "Contrôle si une valeur fait référence à du texte et renvoie VRAI ou FAUX .",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester. Elle peut renvoyer à une cellule, une formule, ou un nom qui fait référence à une cellule, une formule ou une valeur"
			}
		]
	},
	{
		name: "ESTVIDE",
		description: "Contrôle si une référence renvoie à une cellule vide et renvoie VRAI ou FAUX.",
		arguments: [
			{
				name: "valeur",
				description: "est une cellule ou un nom qui fait référence à la cellule que vous voulez tester"
			}
		]
	},
	{
		name: "ET",
		description: "Vérifie si tous les arguments sont VRAI et renvoie VRAI si tous les arguments sont VRAI.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur_logique1",
				description: "représentent de 1 à 255 conditions à tester et qui peuvent être soit VRAI, soit FAUX et représenter aussi bien des valeurs logiques que des matrices ou des références"
			},
			{
				name: "valeur_logique2",
				description: "représentent de 1 à 255 conditions à tester et qui peuvent être soit VRAI, soit FAUX et représenter aussi bien des valeurs logiques que des matrices ou des références"
			}
		]
	},
	{
		name: "EXACT",
		description: "Compare deux chaînes textuelles et renvoie VRAI si elles sont parfaitement identiques et FAUX si elles sont différentes (distinction majuscules/minuscules).",
		arguments: [
			{
				name: "texte1",
				description: "est la première chaîne de caractères"
			},
			{
				name: "texte2",
				description: "est la deuxième chaîne de caractères"
			}
		]
	},
	{
		name: "EXP",
		description: "Donne e (2,718) élevé à la puissance spécifiée.",
		arguments: [
			{
				name: "nombre",
				description: "est l'exposant de la base e. La constante e égale 2,71828182845904, la base du logarithme népérien"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Renvoie le résultat d’un test F, la probabilité bilatérale que les variances des matrice1 et matrice2 ne sont pas très différentes.",
		arguments: [
			{
				name: "matrice1",
				description: "est la première matrice ou plage de données et peut être des numéros, noms, matrices ou références qui contiennent des nombres (les cellules vides sont ignorées)"
			},
			{
				name: "matrice2",
				description: "est la seconde matrice ou plage de données et peut être des numéros, noms, matrices ou références qui contiennent des nombres (les cellules vides sont ignorées)"
			}
		]
	},
	{
		name: "FACT",
		description: "Renvoie la factorielle d'un nombre, égale à 1*2*3*...*nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre positif dont vous voulez obtenir la factorielle"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Renvoie la factorielle double d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre dont vous voulez obtenir la factorielle double"
			}
		]
	},
	{
		name: "FAUX",
		description: "Renvoie la valeur logique FAUX.",
		arguments: [
		]
	},
	{
		name: "FEUILLE",
		description: "Renvoie le numéro de la feuille référencée.",
		arguments: [
			{
				name: "valeur",
				description: "est le nom d'une feuille ou d'une référence dont vous voulez obtenir le numéro de feuille. Sinon, le numéro de la feuille contenant la fonction est renvoyé"
			}
		]
	},
	{
		name: "FEUILLES",
		description: "Renvoie le nombre de feuilles dans une référence.",
		arguments: [
			{
				name: "référence",
				description: "est la référence pour laquelle vous voulez obtenir le nombre de feuilles qu'elle contient. Sinon, le nombre de feuilles dans le classeur contenant la fonction est renvoyé"
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
		name: "FIN.MOIS",
		description: "Donne le numéro de série du dernier jour du mois situé dans un intervalle exprimé en nombre de mois dans le futur ou dans le passé.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "mois",
				description: "est le nombre de mois avant ou après la date de départ"
			}
		]
	},
	{
		name: "FISHER",
		description: "Renvoie la transformation de Fisher. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "est une valeur numérique pour laquelle vous voulez obtenir la transformation, un nombre entre -1 et 1, -1 et 1 exclus"
			}
		]
	},
	{
		name: "FISHER.INVERSE",
		description: "Renvoie la transformation de Fisher inverse : si y = FISHER(x), alors FISHER.INVERSE(y) = x. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "y",
				description: "est la valeur pour laquelle vous voulez effectuer la transformation inverse de Fisher"
			}
		]
	},
	{
		name: "FORMULETEXTE",
		description: "Renvoie une formule en tant que chaîne.",
		arguments: [
			{
				name: "référence",
				description: "est une référence à une formule"
			}
		]
	},
	{
		name: "FRACTION.ANNEE",
		description: "Renvoie une fraction correspondant au nombre de jours séparant date_début de date_fin par rapport à une année complète.",
		arguments: [
			{
				name: "date_début",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "date_fin",
				description: "est la date de fin, exprimée sous forme de numéro de série"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "FREQUENCE",
		description: "Calcule la fréquence à laquelle les valeurs apparaissent dans une plage de valeurs, puis renvoie une matrice verticale de nombres ayant un élément de plus que l'argument matrice_intervalles.",
		arguments: [
			{
				name: "tableau_données",
				description: "est un tableau ou une référence comprenant les valeurs parmi lesquelles vous recherchez une fréquence (les cellules vides et le texte sont ignorés)"
			},
			{
				name: "matrice_intervalles",
				description: "est une matrice ou une référence correspondant aux intervalles permettant de grouper les valeurs de l'argument tableau_données"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Renvoie la valeur de la fonction Gamma.",
		arguments: [
			{
				name: "x",
				description: "est la valeur pour laquelle vous voulez calculer Gamma"
			}
		]
	},
	{
		name: "GAUCHE",
		description: "Extrait le(s) premier(s) caractère(s) à l'extrême gauche d'une chaîne de texte.",
		arguments: [
			{
				name: "texte",
				description: "est la chaîne de texte contenant les caractères à extraire"
			},
			{
				name: "no_car",
				description: "indique le nombre de caractères que la fonction GAUCHE doit renvoyer; 1 si omis"
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
		name: "GRANDE.VALEUR",
		description: "Renvoie la k-ième plus grande valeur d'une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données dans laquelle vous recherchez la k-ième plus grande valeur"
			},
			{
				name: "k",
				description: "représente, dans la matrice ou la plage de cellules, le rang de la donnée à renvoyer, déterminé à partir de la valeur la plus grande"
			}
		]
	},
	{
		name: "HEURE",
		description: "Renvoie l’heure, un nombre entier entre 0 (12:00 A.M.) et 23 (11:00 P.M.).",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet, ou du texte au format horaire, par exemple 16:48:00 ou 4:48:00 P.M"
			}
		]
	},
	{
		name: "HEXBIN",
		description: "Convertit un nombre hexadécimal en nombre binaire.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "HEXDEC",
		description: "Convertit un nombre hexadécimal en nombre décimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à convertir"
			}
		]
	},
	{
		name: "HEXOCT",
		description: "Convertit un nombre hexadécimal en nombre octal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre hexadécimal à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "IMPAIR",
		description: "Arrondit un nombre au nombre entier impair de valeur absolue immédiatement supérieure.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur à arrondir"
			}
		]
	},
	{
		name: "INDEX",
		description: "Renvoie une valeur ou la référence de la cellule à l'intersection d'une ligne et d'une colonne particulières, dans une plage données.",
		arguments: [
			{
				name: "matrice",
				description: "est une plage de cellules ou une constante de matrice."
			},
			{
				name: "no_lig",
				description: "sélectionne la ligne de la matrice ou de la référence à partir de laquelle la valeur doit être renvoyée. Si cet argument est omis, no_col est requis"
			},
			{
				name: "no_col",
				description: "sélectionne la colonne de la matrice ou de la référence à partir de laquelle une référence doit être renvoyée. Si cet argument est omis, no_ligne est requis"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Donne la référence spécifiée par une chaîne de caractères.",
		arguments: [
			{
				name: "réf_texte",
				description: "est une référence à une cellule qui contient une référence de type A1 ou L1C1, un nom défini comme référence, ou une référence à une cellule équivalente à une chaîne de caractères"
			},
			{
				name: "a1",
				description: "est une valeur logique qui indique le type de référence contenu dans la cellule de l'argument réf_texte: style L1C1 = FAUX; style A1 = VRAI ou omis"
			}
		]
	},
	{
		name: "INFORMATIONS",
		description: "Retourne des informations sur l'environnement d'exploitation en cours.",
		arguments: [
			{
				name: "type",
				description: "est le texte spécifiant le type d'informations qui doit être renvoyé."
			}
		]
	},
	{
		name: "INTERET.ACC.MAT",
		description: "Renvoie l'intérêt couru non échu d'un titre dont l'intérêt est perçu à l'échéance.",
		arguments: [
			{
				name: "émission",
				description: "est la date d'émission, exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "taux",
				description: "est le taux annuel du coupon"
			},
			{
				name: "val_nominale",
				description: "est la valeur nominale du titre"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "INTERVALLE.CONFIANCE",
		description: "Renvoie l’intervalle de confiance pour la moyenne d’une population à l’aide d’une distribution normale. Consultez l’aide sur l’équation utilisée.",
		arguments: [
			{
				name: "alpha",
				description: "représente le seuil de probabilité, un nombre supérieur à 0 et inférieur à 1"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la population ; cette valeur est supposée connue. L’écart-type doit être supérieur à 0"
			},
			{
				name: "taille",
				description: "est la taille de l’échantillon"
			}
		]
	},
	{
		name: "INTERVALLE.CONFIANCE.NORMAL",
		description: "Renvoie l’intervalle de confiance pour la moyenne d’une population, à l’aide d’une loi normale.",
		arguments: [
			{
				name: "alpha",
				description: "représente le degré de précision utilisé pour calculer le niveau de confiance, un nombre supérieur à 0 et inférieur à 1"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la population pour la plage de données ; cette valeur est supposée connue. L’écart-type doit être supérieur à 0"
			},
			{
				name: "taille",
				description: "est la taille de l’échantillon"
			}
		]
	},
	{
		name: "INTERVALLE.CONFIANCE.STUDENT",
		description: "Renvoie l’intervalle de confiance pour la moyenne d’une population, à l’aide de la probabilité d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "alpha",
				description: "représente le degré de précision utilisé pour calculer le niveau de confiance, un nombre supérieur à 0 et inférieur à 1"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la population pour la plage de données ; cette valeur est supposée connue. L’écart-type doit être supérieur à 0"
			},
			{
				name: "taille",
				description: "est la taille de l’échantillon"
			}
		]
	},
	{
		name: "INTPER",
		description: "Calcule le montant des intérêts d'un investissement pour une période donnée, fondé sur des paiements périodiques et constants, et un taux d'intérêt stable.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période"
			},
			{
				name: "pér",
				description: "est la période pour laquelle vous voulez calculer les intérêts, ce nombre doit être compris entre 1 et le nombre total de périodes (Npm)"
			},
			{
				name: "npm",
				description: "est le nombre total de remboursements durant l'opération"
			},
			{
				name: "va",
				description: "est la valeur actuelle, ou la somme que représente aujourd'hui une série de paiements futurs"
			},
			{
				name: "vc",
				description: "est la valeur future, ou montant à obtenir après le dernier paiement. Si omise, Vc = 0"
			},
			{
				name: "type",
				description: "est une valeur logique représentant l'échéancement du paiement : à la fin de la période = 0 ou omis, au début de la période = 1"
			}
		]
	},
	{
		name: "INVERSE.LOI.F",
		description: "Renvoie l’inverse de la distribution de probabilité (unilatérale à droite) suivant une loi F : si p = LOI.F (x,...), alors INVERSE.LOI.F (p,...) = x.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la distribution cumulée F, un nombre entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "INVERSE.LOI.F.DROITE",
		description: "Renvoie l’inverse de la distribution de probabilité (unilatérale à droite) suivant une loi F : si p = LOI.F.DROITE (x,...), alors INVERSE.LOI.F.DROITE (p,...) = x.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la distribution cumulée F, un nombre entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "INVERSE.LOI.F.N",
		description: "Renvoie l’inverse de la distribution de probabilité (unilatérale à gauche) suivant une loi F : si p = LOI.F (x,...), alors INVERSE.LOI.F.N (p,...) = x.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la distribution cumulée F, un nombre entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "INVERSEMAT",
		description: "Renvoie la matrice inversée de la matrice enregistrée dans un tableau.",
		arguments: [
			{
				name: "matrice",
				description: "est une matrice carrée numérique, soit une plage de cellules, soit une constante de matrice"
			}
		]
	},
	{
		name: "ISO.PLAFOND",
		description: "Arrondit un nombre à l’entier ou au multiple le plus proche de l’argument précision en s’éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur que vous voulez arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple facultatif auquel vous voulez arrondir"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Renvoie les intérêts dus pour un emprunt à remboursement linéaire.",
		arguments: [
			{
				name: "taux",
				description: "taux d'intérêt par période"
			},
			{
				name: "pér",
				description: "période pour laquelle vous recherchez le montant des intérêts"
			},
			{
				name: "npm",
				description: "nombres de périodes de paiement sur la durée totale de l'opération"
			},
			{
				name: "va",
				description: "correspond à la valeur actuelle du total des paiements futurs"
			}
		]
	},
	{
		name: "JOUR",
		description: "Donne le jour du mois (un nombre entre 1 et 31).",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet"
			}
		]
	},
	{
		name: "JOURS",
		description: "Calcule le nombre de jours entre les deux dates.",
		arguments: [
			{
				name: "date_fin",
				description: "date_début et date_fin sont les deux dates délimitant la période dont vous recherchez l'étendue en jours"
			},
			{
				name: "date_début",
				description: "date_début et date_fin sont les deux dates délimitant la période dont vous recherchez l'étendue en jours"
			}
		]
	},
	{
		name: "JOURS360",
		description: "Calcule le nombre de jours entre deux dates sur la base d'une année de 360 jours (12 mois de 30 jours).",
		arguments: [
			{
				name: "date_début",
				description: "date_début et date_fin sont les deux dates délimitant la période dont vous recherchez l'étendue en jours"
			},
			{
				name: "date_fin",
				description: "date_début et date_fin sont les deux dates délimitant la période dont vous recherchez l'étendue en jours"
			},
			{
				name: "méthode",
				description: "est une valeur logique qui détermine la méthode de calcul: États-Unis (NASD) = FALSE ou omis ; Européen = TRUE."
			}
		]
	},
	{
		name: "JOURSEM",
		description: "Renvoie un chiffre entre 1 et 7 désignant le jour de la semaine.",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre représentant une date"
			},
			{
				name: "type_retour",
				description: "est un nombre : si dimanche=1 et samedi=7, utilisez 1 ; si lundi=1 et dimanche=7, utilisez 2 ; si lundi=0 et dimanche=6, utilisez 3"
			}
		]
	},
	{
		name: "KHIDEUX.INVERSE",
		description: "Renvoie, pour une probabilité unilatérale à droite donnée, la valeur d’une variable aléatoire suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité associée à la distribution du Khi-deux, une valeur entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "KURTOSIS",
		description: "Renvoie le kurtosis d'une série de données. Consultez l'aide sur l'équation utilisée.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 255 nombres, noms, matrices, ou références qui contiennent des nombres, dont vous voulez calculer le kurtosis"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 255 nombres, noms, matrices, ou références qui contiennent des nombres, dont vous voulez calculer le kurtosis"
			}
		]
	},
	{
		name: "LIEN_HYPERTEXTE",
		description: "Crée un raccourci pour ouvrir un document enregistré sur votre disque dur, un serveur de réseau, ou sur Internet.",
		arguments: [
			{
				name: "emplacement_lien",
				description: "représente le chemin et le nom du document à ouvrir, un emplacement de disque dur, une adresse UNC ou encore un chemin URL"
			},
			{
				name: "nom_convivial",
				description: "représente le nombre, le texte ou la fonction qui s'affiche dans la cellule. Si omis, la cellule affiche le texte de l'argument Emplacement_lien"
			}
		]
	},
	{
		name: "LIGNE",
		description: "Donne le numéro de ligne d'une référence.",
		arguments: [
			{
				name: "référence",
				description: "est la cellule ou la plage de cellules dont vous voulez obtenir le numéro de ligne; si omis, renvoie la cellule contenant la fonction LIGNE"
			}
		]
	},
	{
		name: "LIGNES",
		description: "Renvoie le nombre de lignes d'une référence ou d'une matrice.",
		arguments: [
			{
				name: "tableau",
				description: "est un tableau, une formule matricielle ou la référence d'une plage de cellules dont vous voulez obtenir le nombre de lignes"
			}
		]
	},
	{
		name: "LIREDONNEESTABCROISDYNAMIQUE",
		description: "Extrait des données d'un tableau croisé dynamique.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "champ_données",
				description: "est le nom du champ de données d'où extraire les données"
			},
			{
				name: "tableau_croisé_dyn",
				description: "est une référence à une cellule ou à une plage de cellules du tableau croisé dynamique contenant les données à extraire"
			},
			{
				name: "champ",
				description: "champ de référence"
			},
			{
				name: "élément",
				description: "élément du champ de référence"
			}
		]
	},
	{
		name: "LN",
		description: "Donne le logarithme népérien d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre réel positif dont vous voulez obtenir le logarithme népérien"
			}
		]
	},
	{
		name: "LNGAMMA",
		description: "Renvoie le logarithme népérien de la fonction Gamma. Consultez l'aide pour obtenir des informations sur l'équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur pour laquelle vous voulez calculer LNGAMMA, un nombre positif"
			}
		]
	},
	{
		name: "LNGAMMA.PRECIS",
		description: "Renvoie le logarithme népérien de la fonction Gamma.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur pour laquelle vous voulez calculer LNGAMMA.PRECIS, un nombre positif"
			}
		]
	},
	{
		name: "LOG",
		description: "Donne le logarithme d'un nombre dans la base spécifiée.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre réel positif dont vous voulez obtenir le logarithme"
			},
			{
				name: "base",
				description: "est la base du logarithme; 10 si omis"
			}
		]
	},
	{
		name: "LOG10",
		description: "Calcule le logarithme en base 10 d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre réel positif dont vous voulez obtenir le logarithme en base 10"
			}
		]
	},
	{
		name: "LOGREG",
		description: "Renvoie des statistiques qui décrivent une courbe de corrélation exponentielle à partir de valeurs connues.",
		arguments: [
			{
				name: "y_connus",
				description: "est la série des valeurs y déjà connues par la relation y = b*m ^ x"
			},
			{
				name: "x_connus",
				description: "est une série de valeurs x facultatives, éventuellement déjà données par la relation y = b*m ^ x"
			},
			{
				name: "constante",
				description: "est une valeur logique : la constante b est calculée normalement si Constante = VRAI ou omis ; b est égal à 1 si Constante = FAUX"
			},
			{
				name: "statistiques",
				description: "est une valeur logique : renvoyer les statistiques de régression complémentaires = VRAI ; renvoyer les coefficients m ou la constante b = FAUX ou omis"
			}
		]
	},
	{
		name: "LOI.BETA",
		description: "Renvoie la probabilité d’une variable aléatoire continue suivant une loi de probabilité Bêta.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle la fonction doit être évaluée sur l’intervalle [A, B] comprenant x"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "A",
				description: "est une limite inférieure facultative de l’intervalle comprenant x. Si omis, A = 0"
			},
			{
				name: "B",
				description: "est une limite supérieure facultative de l’intervalle comprenant x. Si omis, B = 1"
			}
		]
	},
	{
		name: "LOI.BETA.N",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi de probabilité Bêta.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle la fonction doit être évaluée sur l’intervalle [A, B] comprenant x"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "bêta",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction densité de la probabilité, utilisez FAUX"
			},
			{
				name: "cumulative",
				description: "représente un paramètre de la distribution et doit être supérieur à 0"
			},
			{
				name: "A",
				description: "est une limite inférieure facultative de l’intervalle comprenant x. Si omis, A = 0"
			},
			{
				name: "B",
				description: "est une limite supérieure facultative de l’intervalle comprenant x. Si omis, B = 1"
			}
		]
	},
	{
		name: "LOI.BINOMIALE",
		description: "Renvoie la probabilité d’une variable aléatoire discrète suivant la loi binomiale.",
		arguments: [
			{
				name: "nombre_succès",
				description: "représente le nombre de succès obtenus lors des tirages"
			},
			{
				name: "tirages",
				description: "représente le nombre de tirages indépendants"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité d’obtenir un succès à chaque tirage"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de probabilité de masse, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.BINOMIALE.INVERSE",
		description: "Renvoie la plus petite valeur pour laquelle la distribution binomiale cumulée est supérieure ou égale à une valeur critère.",
		arguments: [
			{
				name: "tirages",
				description: "représente le nombre de tirages de Bernoulli"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité de succès à chaque tirage, un nombre entre 0 et 1 compris"
			},
			{
				name: "alpha",
				description: "représente la valeur critère, un nombre entre 0 et 1 compris"
			}
		]
	},
	{
		name: "LOI.BINOMIALE.N",
		description: "Renvoie la probabilité d’une variable aléatoire discrète suivant la loi binomiale.",
		arguments: [
			{
				name: "nombre_succès",
				description: "représente le nombre de succès obtenus lors des tirages"
			},
			{
				name: "tirages",
				description: "représente le nombre de tirages indépendants"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité d’obtenir un succès à chaque tirage"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de probabilité de masse, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.BINOMIALE.NEG",
		description: "Renvoie la distribution négative binomiale, probabilité d’obtenir un nombre d’échecs égal à Nombre_échecs avant le succès numéro Nombre_succès, avec une probabilité égale à Probabilité_succès.",
		arguments: [
			{
				name: "nombre_échecs",
				description: "représente le nombre d’échecs"
			},
			{
				name: "nombre_succès",
				description: "représente le nombre de succès à obtenir"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité d’obtenir un succès; un nombre entre 0 et 1"
			}
		]
	},
	{
		name: "LOI.BINOMIALE.NEG.N",
		description: "Renvoie la distribution négative binomiale, probabilité d’obtenir un nombre d’échecs égal à Nombre_échecs avant le succès numéro Nombre_succès, avec une probabilité égale à Probabilité_succès.",
		arguments: [
			{
				name: "nombre_échecs",
				description: "représente le nombre d’échecs"
			},
			{
				name: "nombre_succès",
				description: "représente le nombre de succès à obtenir"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité d’obtenir un succès ; un nombre entre 0 et 1"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction densité de probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.BINOMIALE.SERIE",
		description: "Renvoie la probabilité d'un résultat de tirage en suivant une distribution binomiale.",
		arguments: [
			{
				name: "tirages",
				description: "représente le nombre de tirages indépendants"
			},
			{
				name: "probabilité_succès",
				description: "représente la probabilité d'obtenir un succès à chaque tirage"
			},
			{
				name: "nombre_succès",
				description: "représente le nombres de succès obtenus lors des tirages"
			},
			{
				name: "nombre_succès2",
				description: "Si cette fonction est fournie, elle renvoie la probabilité que le nombre de tirages réussis se trouve entre nombre_succès et nombre_succès2"
			}
		]
	},
	{
		name: "LOI.EXPONENTIELLE",
		description: "Renvoie la probabilité d’une variable aléatoire continue suivant une loi exponentielle. Consultez l’aide sur l’équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur de la fonction"
			},
			{
				name: "lambda",
				description: "représente la valeur du paramètre, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique à renvoyer par la fonction : fonction distribution cumulée = VRAI ; fonction densité de la probabilité = FAUX"
			}
		]
	},
	{
		name: "LOI.EXPONENTIELLE.N",
		description: "Renvoie la probabilité d’une variable aléatoire continue suivant une loi exponentielle. Consultez l’aide sur l’équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur de la fonction"
			},
			{
				name: "lambda",
				description: "représente la valeur du paramètre, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique à renvoyer par la fonction : fonction distribution cumulée = VRAI ; fonction densité de la probabilité = FAUX"
			}
		]
	},
	{
		name: "LOI.F",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi F pour deux séries de données.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.F.DROITE",
		description: "Renvoie la probabilité (unilatérale à droite) d’une variable aléatoire suivant une loi F pour deux séries de données.",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.F.N",
		description: "Renvoie la probabilité (unilatérale à gauche) d’une variable aléatoire suivant une loi F pour deux séries de données.",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "degrés_liberté1",
				description: "représente le nombre de degrés de liberté du numérateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "degrés_liberté2",
				description: "représente le nombre de degrés de liberté du dénominateur, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "cumulée",
				description: "représente une valeur logique à renvoyer par la fonction : fonction distribution cumulée = VRAI ; fonction densité de la probabilité = FAUX"
			}
		]
	},
	{
		name: "LOI.GAMMA",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi Gamma. Consultez l’aide sur l’équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle vous voulez évaluer la distribution, un nombre positif"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution. Si bêta vaut 1, cette fonction renvoie la probabilité d’une variable aléatoire suivant une loi Gamma centrée réduite"
			},
			{
				name: "cumulative",
				description: "est une valeur logique : renvoie la fonction de distribution cumulée = VRAI ; ou fonction de probabilité de masse = FAUX ou omis"
			}
		]
	},
	{
		name: "LOI.GAMMA.INVERSE",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi Gamma : si p = LOI.GAMMA(x,...), alors LOI.GAMMA.INVERSE(p,...) = x.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la loi Gamma, un nombre entre 0 et 1 compris"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution, un nombre positif. Si bêta = 1, LOI.GAMMA.INVERSE renvoie l’inverse de la valeur d’une variable aléatoire suivant une loi Gamma centrée réduite"
			}
		]
	},
	{
		name: "LOI.GAMMA.INVERSE.N",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi Gamma: si p = LOI.GAMMA.N(x,...), alors LOI.GAMMA.INVERSE.N(p,...) = x.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la loi Gamma, un nombre entre 0 et 1 compris"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution, un nombre positif. Si bêta = 1, LOI.GAMMA.INVERSE.N renvoie l’inverse de la valeur d’une variable aléatoire suivant une loi Gamma centrée réduite"
			}
		]
	},
	{
		name: "LOI.GAMMA.N",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi Gamma.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle vous voulez évaluer la distribution, un nombre positif"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: " Si bêta vaut 1, LOI.GAMMA.N renvoie la probabilité d’une variable aléatoire suivant une loi Gamma centrée réduite"
			},
			{
				name: "cumulative",
				description: "est une valeur logique : renvoie la fonction de distribution cumulée = VRAI; ou fonction de probabilité de masse = FAUX ou omis"
			}
		]
	},
	{
		name: "LOI.HYPERGEOMETRIQUE",
		description: "Renvoie la probabilité d’une variable aléatoire discrète suivant une loi hypergéométrique.",
		arguments: [
			{
				name: "succès_échantillon",
				description: "est le nombre de succès contenu dans l’échantillon"
			},
			{
				name: "nombre_échantillon",
				description: "est la taille de l’échantillon"
			},
			{
				name: "succès_population",
				description: "est le nombre de succès dans la population"
			},
			{
				name: "nombre_population",
				description: "est la taille de la population"
			}
		]
	},
	{
		name: "LOI.HYPERGEOMETRIQUE.N",
		description: "Renvoie la probabilité d’une variable aléatoire discrète suivant une loi hypergéométrique.",
		arguments: [
			{
				name: "succès_échantillon",
				description: "est le nombre de succès contenu dans l’échantillon"
			},
			{
				name: "nombre_échantillon",
				description: "est la taille de l’échantillon"
			},
			{
				name: "succès_population",
				description: "est le nombre de succès dans la population"
			},
			{
				name: "nombre_population",
				description: "est la taille de la population"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de densité de probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.KHIDEUX",
		description: "Renvoie la probabilité unilatérale à droite d’une variable aléatoire continue suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle vous voulez évaluer la distribution, un nombre positif"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.KHIDEUX.DROITE",
		description: "Renvoie la probabilité unilatérale à droite d’une variable aléatoire continue suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle vous voulez évaluer la distribution, un nombre non négatif"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.KHIDEUX.INVERSE",
		description: "Renvoie, pour une probabilité unilatérale à gauche donnée, la valeur d’une variable aléatoire suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité associée à la distribution du Khi-deux, une valeur entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.KHIDEUX.INVERSE.DROITE",
		description: "Renvoie, pour une probabilité unilatérale à droite donnée, la valeur d’une variable aléatoire suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité associée à la distribution du Khi-deux, une valeur entre 0 et 1 compris"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			}
		]
	},
	{
		name: "LOI.KHIDEUX.N",
		description: "Renvoie la probabilité unilatérale à gauche d’une variable aléatoire continue suivant une loi du Khi-deux.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle vous voulez évaluer la distribution, un nombre positif"
			},
			{
				name: "degrés_liberté",
				description: "représente le nombre de degrés de liberté, un nombre entre 1 et 10^10, 10^10 exclus"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique à renvoyer par la fonction : fonction distribution cumulée = VRAI ; fonction densité de la probabilité = FAUX"
			}
		]
	},
	{
		name: "LOI.LOGNORMALE",
		description: "Renvoie la distribution de x suivant une loi lognormale cumulée, où ln(x) est normalement distribué avec les paramètres Espérance et Écart_type.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de ln(x)"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de ln(x), un nombre positif"
			}
		]
	},
	{
		name: "LOI.LOGNORMALE.INVERSE",
		description: "Renvoie l’inverse de la fonction de distribution de x suivant une loi lognormale cumulée, où In(x) est normalement distribué avec les paramètres Espérance et Écart_type.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la distribution, un nombre entre 0 et 1 compris"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de ln(x)"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de ln(x), un nombre positif"
			}
		]
	},
	{
		name: "LOI.LOGNORMALE.INVERSE.N",
		description: "Renvoie l’inverse de la fonction de distribution de x suivant une loi lognormale cumulée, où In(x) est normalement distribué avec les paramètres Espérance et Écart_type.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la distribution, un nombre entre 0 et 1 compris"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de ln(x)"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de ln(x), un nombre positif"
			}
		]
	},
	{
		name: "LOI.LOGNORMALE.N",
		description: "Renvoie la fonction de distribution de x suivant une loi lognormale, où In(x) est normalement distribué avec les paramètres Espérance et Écart_type.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de ln(x)"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de ln(x), un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction densité de probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.NORMALE",
		description: "Renvoie la probabilité d’une variable aléatoire continue suivant une loi normale pour l’espérance arithmétique et l’écart-type spécifiés.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur dont vous recherchez la distribution"
			},
			{
				name: "espérance",
				description: "représente l’espérance arithmétique de la distribution"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la distribution, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de densité de distribution de la probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.NORMALE.INVERSE",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi normale pour la moyenne et l’écart-type spécifiés.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité correspondant à la distribution normale, un nombre entre 0 et 1 compris"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de la distribution"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la distribution, un nombre positif"
			}
		]
	},
	{
		name: "LOI.NORMALE.INVERSE.N",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi normale pour la moyenne et l’écart-type spécifiés.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité correspondant à la distribution normale, un nombre entre 0 et 1 compris"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique de la distribution"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la distribution, un nombre positif"
			}
		]
	},
	{
		name: "LOI.NORMALE.N",
		description: "Renvoie la probabilité d’une variable aléatoire continue suivant une loi normale pour l’espérance arithmétique et l’écart-type spécifiés.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur dont vous recherchez la distribution"
			},
			{
				name: "espérance",
				description: "représente l’espérance arithmétique de la distribution"
			},
			{
				name: "écart_type",
				description: "représente l’écart-type de la distribution, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI  pour la fonction de probabilité de masse, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.NORMALE.STANDARD",
		description: "Renvoie la distribution cumulée suivant une loi normale centrée réduite (d’espérance nulle et d’écart-type égal à 1).",
		arguments: [
			{
				name: "z",
				description: "représente la valeur dont vous recherchez la distribution"
			}
		]
	},
	{
		name: "LOI.NORMALE.STANDARD.INVERSE",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi normale standard (ou centrée réduite), c’est-à-dire ayant une moyenne de zéro et un écart-type de 1.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité correspondant à la distribution normale, un nombre entre 0 et 1 compris"
			}
		]
	},
	{
		name: "LOI.NORMALE.STANDARD.INVERSE.N",
		description: "Renvoie, pour une probabilité donnée, la valeur d’une variable aléatoire suivant une loi normale standard (ou centrée réduite), c’est-à-dire ayant une moyenne de zéro et un écart-type de 1.",
		arguments: [
			{
				name: "probabilité",
				description: "représente une probabilité correspondant à la distribution normale, un nombre entre 0 et 1 compris"
			}
		]
	},
	{
		name: "LOI.NORMALE.STANDARD.N",
		description: "Renvoie la distribution suivant une loi normale centrée réduite (d’espérance nulle et d’écart-type égal à 1).",
		arguments: [
			{
				name: "z",
				description: "représente la valeur dont vous recherchez la distribution"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction densité de probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.POISSON",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi de Poisson.",
		arguments: [
			{
				name: "x",
				description: "représente le nombre d’événements"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la probabilité cumulative de Poisson, utilisez VRAI ; pour la fonction de probabilité de masse en série de Poisson, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.POISSON.N",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi de Poisson.",
		arguments: [
			{
				name: "x",
				description: "représente le nombre d’événements"
			},
			{
				name: "espérance",
				description: "représente l’espérance mathématique, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la probabilité cumulative de Poisson, utilisez VRAI ; pour la fonction de probabilité de masse en série de Poisson, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.STUDENT",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur numérique à laquelle la distribution doit être évaluée"
			},
			{
				name: "degrés_liberté",
				description: "représente un nombre entier indiquant le nombre de degrés de liberté qui caractérisent la distribution"
			},
			{
				name: "uni/bilatéral",
				description: "indique le type de distribution à renvoyer : unilatérale = 1; bilatérale = 2"
			}
		]
	},
	{
		name: "LOI.STUDENT.BILATERALE",
		description: "Renvoie la probabilité bilatérale d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur numérique à laquelle la distribution doit être évaluée"
			},
			{
				name: "degrés_liberté",
				description: "représente un nombre entier indiquant le nombre de degrés de liberté qui caractérisent la distribution"
			}
		]
	},
	{
		name: "LOI.STUDENT.DROITE",
		description: "Renvoie la probabilité unilatérale à droite d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur numérique à laquelle la distribution doit être évaluée"
			},
			{
				name: "degrés_liberté",
				description: "représente un nombre entier indiquant le nombre de degrés de liberté qui caractérisent la distribution"
			}
		]
	},
	{
		name: "LOI.STUDENT.INVERSE",
		description: "Renvoie, pour une probabilité donnée, la valeur inverse bilatérale d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la loi bilatérale T de Student, c’est-à-dire un nombre entre 0 et 1 inclus"
			},
			{
				name: "degrés_liberté",
				description: "est un entier positif indiquant le nombre de degrés de liberté caractérisant la distribution"
			}
		]
	},
	{
		name: "LOI.STUDENT.INVERSE.BILATERALE",
		description: "Renvoie, pour une probabilité donnée, la valeur inverse bilatérale d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la loi bilatérale T de Student, c’est-à-dire un nombre entre 0 et 1 inclus"
			},
			{
				name: "degrés_liberté",
				description: "est un entier positif indiquant le nombre de degrés de liberté caractérisant la distribution"
			}
		]
	},
	{
		name: "LOI.STUDENT.INVERSE.N",
		description: "Renvoie, pour une probabilité donnée, la valeur inverse unilatérale à gauche d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "probabilité",
				description: "représente la probabilité associée à la loi unilatérale à gauche T de Student, c’est-à-dire un nombre entre 0 et 1 inclus"
			},
			{
				name: "degrés_liberté",
				description: "est un entier positif indiquant le nombre de degrés de liberté caractérisant la distribution"
			}
		]
	},
	{
		name: "LOI.STUDENT.N",
		description: "Renvoie la probabilité unilatérale à gauche d’une variable aléatoire suivant une loi T de Student.",
		arguments: [
			{
				name: "x",
				description: "représente la valeur numérique à laquelle la distribution doit être évaluée"
			},
			{
				name: "degrés_liberté",
				description: "représente un nombre entier indiquant le nombre de degrés de liberté qui caractérisent la distribution"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction densité de la probabilité, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.WEIBULL",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi de Weibull. Consultez l’aide sur l’équation utilisée.",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de probabilité de masse, utilisez FAUX"
			}
		]
	},
	{
		name: "LOI.WEIBULL.N",
		description: "Renvoie la probabilité d’une variable aléatoire suivant une loi de Weibull.",
		arguments: [
			{
				name: "x",
				description: "est la valeur à laquelle la fonction doit être évaluée, un nombre positif"
			},
			{
				name: "alpha",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "bêta",
				description: "représente un paramètre de la distribution, un nombre positif"
			},
			{
				name: "cumulative",
				description: "représente une valeur logique : pour la fonction distribution cumulative, utilisez VRAI ; pour la fonction de probabilité de masse, utilisez FAUX"
			}
		]
	},
	{
		name: "MAINTENANT",
		description: "Renvoie la date du jour et de l'heure du jour, sous la forme d'une date et d'une heure.",
		arguments: [
		]
	},
	{
		name: "MAJUSCULE",
		description: "Convertit une chaîne de caractères en majuscules.",
		arguments: [
			{
				name: "texte",
				description: "est le texte que vous voulez convertir en caractères majuscules, une référence ou une chaîne de caractères"
			}
		]
	},
	{
		name: "MATRICE.UNITAIRE",
		description: "Renvoie la matrice d'unités pour la dimension spécifiée.",
		arguments: [
			{
				name: "dimension",
				description: "est un nombre entier spécifiant la dimension de la matrice d'unités que vous voulez renvoyer"
			}
		]
	},
	{
		name: "MAX",
		description: "Donne la valeur la plus grande parmi une liste de valeurs. Ignore les valeurs logiques et le texte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus grande"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus grande"
			}
		]
	},
	{
		name: "MAXA",
		description: "Renvoie le plus grand nombre d'un ensemble de valeurs. Prend en compte les valeurs logiques et le texte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus grande"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus grande"
			}
		]
	},
	{
		name: "MEDIANE",
		description: "Renvoie la valeur médiane ou le nombre qui se trouve au milieu d'une liste de nombres fournie.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, matrices ou références qui contiennent des nombres, dont vous voulez obtenir la médiane"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, matrices ou références qui contiennent des nombres, dont vous voulez obtenir la médiane"
			}
		]
	},
	{
		name: "MIN",
		description: "Donne la valeur la plus petite parmi une liste de valeurs. Ignore les valeurs logiques et le texte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus petite"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres au format texte parmi lesquels vous voulez trouver la valeur la plus petite"
			}
		]
	},
	{
		name: "MINA",
		description: "Renvoie la plus petite valeur d'un ensemble de données. Prend en compte des valeurs logiques et le texte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres sous forme de texte parmi lesquels vous voulez trouver la valeur la plus petite"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 nombres, cellules vides, valeurs logiques ou nombres sous forme de texte parmi lesquels vous voulez trouver la valeur la plus petite"
			}
		]
	},
	{
		name: "MINUSCULE",
		description: "Convertit toutes les lettres majuscules en une chaîne de caractères en minuscules.",
		arguments: [
			{
				name: "texte",
				description: "est le texte que vous voulez convertir en caractères minuscules. Les caractères du texte qui ne sont pas des lettres ne sont pas modifiés"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Renvoie les minutes, un entier entre 0 et 59.",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet, ou du texte au format horaire, par exemple 16:48:00 ou 4:48:00 P.M"
			}
		]
	},
	{
		name: "MOD",
		description: "Renvoie le reste d'une division.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre dont vous voulez obtenir le reste de la division"
			},
			{
				name: "diviseur",
				description: "est le nombre par lequel vous voulez diviser l'argument nombre"
			}
		]
	},
	{
		name: "MODE",
		description: "Renvoie la valeur la plus fréquente d’une série de données.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			}
		]
	},
	{
		name: "MODE.MULTIPLE",
		description: "Renvoie une matrice verticale des valeurs les plus fréquentes d’une matrice ou série de données. Pour une matrice horizontale, utilisez =TRANSPOSE(MODE.MULTIPLE(nombre1,nombre2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			}
		]
	},
	{
		name: "MODE.SIMPLE",
		description: "Renvoie la valeur la plus fréquente d’une série de données.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			},
			{
				name: "nombre2",
				description: "représentent les 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez le mode"
			}
		]
	},
	{
		name: "MOIS",
		description: "Donne le mois, un nombre de 1 (janvier) à 12 (décembre).",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet"
			}
		]
	},
	{
		name: "MOIS.DECALER",
		description: "Renvoie le numéro de série de la date située un nombre spécifié de mois dans le passé ou le futur par rapport à une date indiquée.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "mois",
				description: "est le nombre de mois avant ou après la date de départ"
			}
		]
	},
	{
		name: "MOYENNE",
		description: "Renvoie la moyenne (espérance arithmétique) des arguments, qui peuvent être des nombres, des noms, des matrices, ou des références contenant des nombres.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments numériques dont vous souhaitez obtenir la moyenne"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments numériques dont vous souhaitez obtenir la moyenne"
			}
		]
	},
	{
		name: "MOYENNE.GEOMETRIQUE",
		description: "Renvoie la moyenne géométrique d'une matrice ou d'une plage de données numériques positives.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, noms, matrices ou références contenant des nombres dont vous recherchez la moyenne géométrique"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, noms, matrices ou références contenant des nombres dont vous recherchez la moyenne géométrique"
			}
		]
	},
	{
		name: "MOYENNE.HARMONIQUE",
		description: "Renvoie la moyenne harmonique d'une série de données en nombres positifs : la réciproque de la moyenne arithmétique des réciproques.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez la moyenne harmonique"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, noms, matrices ou références qui contiennent des nombres dont vous recherchez la moyenne harmonique"
			}
		]
	},
	{
		name: "MOYENNE.RANG",
		description: "Renvoie le rang d’un nombre dans une liste d’arguments : sa taille est relative aux autres valeurs de la liste ; si plusieurs valeurs sont associées au même rang, renvoie le rang supérieur de ce jeu de valeurs.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre dont vous voulez connaître le rang"
			},
			{
				name: "référence",
				description: "est une matrice, ou une référence à une liste de nombres. Les valeurs non numériques sont ignorées dans la référence"
			},
			{
				name: "ordre",
				description: "est un numéro : le rang de l’argument dans la liste triée par ordre décroissant = 0 ou omis ; son rang dans la liste triée par ordre croissant = toute valeur différente de zéro"
			}
		]
	},
	{
		name: "MOYENNE.REDUITE",
		description: "Renvoie la moyenne de la partie intérieure d'une série de valeurs données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de valeurs à réduire et sur laquelle calculer la moyenne"
			},
			{
				name: "pourcentage",
				description: "représente le nombre fractionnaire d'observations à exclure de la série de données"
			}
		]
	},
	{
		name: "MOYENNE.SI",
		description: "Détermine la moyenne (espérance arithmétique) des cellules satisfaisant une condition ou des critères particuliers.",
		arguments: [
			{
				name: "plage",
				description: "représente les cellules servant de base à l'évaluation"
			},
			{
				name: "critères",
				description: "représente la condition ou les critères sous la forme d'un nombre, d'une expression ou de texte définissant quelles cellules sont à utiliser pour déterminer l'espérance arithmétique"
			},
			{
				name: "plage_moyenne",
				description: "représente les cellules servant de base au calcul de la moyenne. Si omis, les cellules de la plage sont alors utilisées"
			}
		]
	},
	{
		name: "MOYENNE.SI.ENS",
		description: "Détermine la moyenne (espérance arithmétique) des cellules spécifiées par un ensemble de conditions ou de critères.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "plage_moyenne",
				description: "représente les cellules qui seront effectivement utilisées pour effectuer le calcul de la moyenne."
			},
			{
				name: "plage_critères",
				description: "représente la plage de cellules à évaluer d'après une condition particulière"
			},
			{
				name: "critères",
				description: "représente la condition ou les critères sous la forme d'un nombre, d'une expression ou de texte qui seront utilisés pour déterminer la moyenne"
			}
		]
	},
	{
		name: "MULTINOMIALE",
		description: "Renvoie le polynôme à plusieurs variables d'un ensemble de nombres.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 valeurs dont vous voulez obtenir le polynôme à plusieurs variables"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 valeurs dont vous voulez obtenir le polynôme à plusieurs variables"
			}
		]
	},
	{
		name: "N",
		description: "Renvoie une valeur convertie en nombre. Les nombres sont convertis en nombres, les dates en numéros de série, les VRAI en 1, et tout le reste en 0 (zéro).",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur que vous voulez convertir"
			}
		]
	},
	{
		name: "NA",
		description: "Renvoie la valeur d'erreur #N/A (valeur non disponible).",
		arguments: [
		]
	},
	{
		name: "NB",
		description: "Détermine le nombre de cellules d'une plage contenant des nombres.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentant de 1 à 255 arguments qui peuvent contenir ou faire référence à différents types de données, mais dont seuls les nombres sont comptés"
			},
			{
				name: "valeur2",
				description: "représentant de 1 à 255 arguments qui peuvent contenir ou faire référence à différents types de données, mais dont seuls les nombres sont comptés"
			}
		]
	},
	{
		name: "NB.COUPONS",
		description: "Calcule le nombre de coupons entre la date de liquidation et la date d'échéance.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "fréquence",
				description: "est le nombre de coupons payés par an"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "NB.JOURS.COUPON.PREC",
		description: "Calcule le nombre de jours entre le début de la période de coupon et la date de liquidation.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "fréquence",
				description: "est le nombre de coupons payés par an"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "NB.JOURS.OUVRES",
		description: "Renvoie le nombre de jours ouvrés compris entre deux dates.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "date_fin",
				description: "est la date de fin, exprimée sous forme de numéro de série"
			},
			{
				name: "jours_fériés",
				description: "est une matrice contenant un ou des numéro de série représentant des dates à compter comme jours non ouvrés"
			}
		]
	},
	{
		name: "NB.JOURS.OUVRES.INTL",
		description: "Renvoie le nombre de jours ouvrés compris entre deux dates avec des paramètres de week-end personnalisés.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "date_fin",
				description: "est la date de fin, exprimée sous forme de numéro de série"
			},
			{
				name: "week-end",
				description: "est un numéro ou une chaîne indiquant quand survient le week-end"
			},
			{
				name: "jours_fériés",
				description: "est une matrice facultative contenant un ou plusieurs numéros de série de date à exclure du calendrier des jours ouvrés (jours de fêtes nationales, etc.)"
			}
		]
	},
	{
		name: "NB.SI",
		description: "Détermine le nombre de cellules non vides répondant à la condition à l'intérieur d'une plage.",
		arguments: [
			{
				name: "plage",
				description: "est la plage de cellules dans laquelle compter les cellules non vides."
			},
			{
				name: "critère",
				description: "est la condition, exprimée sous forme de nombre, d'expression ou de texte qui détermine quelles cellules seront comptées"
			}
		]
	},
	{
		name: "NB.SI.ENS",
		description: "Compte le nombre de cellules spécifiées par un ensemble de conditions ou de critères.",
		arguments: [
			{
				name: "plage_critères",
				description: "représente la plage de cellules à évaluer d'après une condition particulière"
			},
			{
				name: "critères",
				description: "représente la condition sous la forme d'un nombre, d'une expression ou de texte définissant quelles cellules doivent être comptées"
			}
		]
	},
	{
		name: "NB.VIDE",
		description: "Compte le nombre de cellules vides à l'intérieur d'une plage spécifique.",
		arguments: [
			{
				name: "plage",
				description: "est la plage dans laquelle compter les cellules vides"
			}
		]
	},
	{
		name: "NBCAR",
		description: "Renvoie le nombre de caractères contenus dans une chaîne de texte.",
		arguments: [
			{
				name: "texte",
				description: "est le texte dont vous voulez connaître la longueur. Les espaces sont considérés comme des caractères"
			}
		]
	},
	{
		name: "NBVAL",
		description: "Détermine le nombre de cellules d'une plage qui ne sont pas vides.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentant de 1 à 255 arguments et correspondant aux valeurs et cellules à compter. Les valeurs peuvent être de n'importe quel type"
			},
			{
				name: "valeur2",
				description: "représentant de 1 à 255 arguments et correspondant aux valeurs et cellules à compter. Les valeurs peuvent être de n'importe quel type"
			}
		]
	},
	{
		name: "NO.SEMAINE",
		description: "Calcule le numéro de semaine d'une date exprimée dans le format d'heure et de date utilisé par Spreadsheet.",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est le code d'heure et de date utilisé par Spreadsheet"
			},
			{
				name: "méthode",
				description: "est un nombre (1 ou 2) qui détermine le type de la valeur renvoyée"
			}
		]
	},
	{
		name: "NO.SEMAINE.ISO",
		description: "Renvoie le numéro ISO de la semaine de l'année correspondant à une date donnée.",
		arguments: [
			{
				name: "date",
				description: "est le code date-heure utilisé par Spreadsheet pour le calcul de la date et de l'heure"
			}
		]
	},
	{
		name: "NOMPROPRE",
		description: "Met en majuscule la première lettre de chaque mot dans une chaîne de texte et met toutes les autres lettres en minuscules.",
		arguments: [
			{
				name: "texte",
				description: "est un texte entre guillemets, une formule qui renvoie du texte ou une référence à une cellule contenant un texte dont vous voulez que certaines lettres soient en majuscules"
			}
		]
	},
	{
		name: "NON",
		description: "Inverse la valeur logique de l'argument: renvoie FAUX pour un argument VRAI et VRAI pour un argument FAUX.",
		arguments: [
			{
				name: "valeur_logique",
				description: "est une valeur ou une expression dont l'évaluation peut être VRAI ou FAUX"
			}
		]
	},
	{
		name: "NPM",
		description: "Renvoie le nombre de paiements d'un investissement à versements réguliers et taux d'intérêt constant.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période. Par exemple, utilisez 6%/4 pour des paiements trimestriels à 6% APR"
			},
			{
				name: "vpm",
				description: "est le montant du remboursement pour chaque période; ce montant est inchangé durant toute la durée de l'opération"
			},
			{
				name: "va",
				description: "est la valeur actuelle, ou la somme que représente aujourd'hui une série de paiements futurs"
			},
			{
				name: "vc",
				description: "est la valeur future ou capitalisée; c'est-à-dire un montant que vous voulez atteindre après le dernier paiement. Si omis, zéro est utilisé"
			},
			{
				name: "type",
				description: "est une valeur logique: paiement au début de la période = 1; paiement à la fin de la période = 0 ou omis"
			}
		]
	},
	{
		name: "OCTBIN",
		description: "Convertit un nombre octal en nombre binaire.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre octal à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "OCTDEC",
		description: "Convertit un nombre octal en nombre décimal.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre octal à convertir"
			}
		]
	},
	{
		name: "OCTHEX",
		description: "Convertit un nombre octal en nombre hexadécimal.",
		arguments: [
			{
				name: "nombre",
				description: "représente le nombre octal à convertir"
			},
			{
				name: "nb_car",
				description: "représente le nombre de caractères à utiliser"
			}
		]
	},
	{
		name: "ORDONNEE.ORIGINE",
		description: "Calcule le point auquel une droite va croiser l'axe des y en traçant une droite de régression linéaire d'après les valeurs connues de x et de y.",
		arguments: [
			{
				name: "y_connus",
				description: "représente la série dépendante d'observations ou de données et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "x_connus",
				description: "représente la série indépendante d'observations ou de données et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "OU",
		description: "Vérifie si un argument est VRAI et renvoie VRAI ou FAUX. Renvoie FAUX uniquement si tous les arguments sont FAUX.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur_logique1",
				description: "représentent de 1 à 255 conditions que vous voulez tester, et qui peuvent être soit VRAI, soit FAUX"
			},
			{
				name: "valeur_logique2",
				description: "représentent de 1 à 255 conditions que vous voulez tester, et qui peuvent être soit VRAI, soit FAUX"
			}
		]
	},
	{
		name: "OUX",
		description: "Renvoie une valeur logique « Ou exclusif » de tous les arguments.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur_logique1",
				description: "représentent de 1 à 254 conditions à tester qui peuvent avoir la valeur VRAI ou FAUX et être des valeurs logiques, tableaux ou références"
			},
			{
				name: "valeur_logique2",
				description: "représentent de 1 à 254 conditions à tester qui peuvent avoir la valeur VRAI ou FAUX et être des valeurs logiques, tableaux ou références"
			}
		]
	},
	{
		name: "PAIR",
		description: "Arrondit un nombre au nombre entier pair le plus proche en s'éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur à arrondir"
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
		name: "PDUREE",
		description: "Renvoie le nombre de périodes requises par un investissement pour atteindre une valeur spécifiée.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période."
			},
			{
				name: "va",
				description: "est la valeur actuelle de l'investissement"
			},
			{
				name: "vc",
				description: "est la valeur future souhaitée de l'investissement"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Renvoie le coefficient de corrélation d'échantillonnage de Pearson, r. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "matrice1",
				description: "représente une série de valeurs indépendantes"
			},
			{
				name: "matrice2",
				description: "représente une série de valeurs dépendantes"
			}
		]
	},
	{
		name: "PENTE",
		description: "Renvoie la pente d'une droite de régression linéaire.",
		arguments: [
			{
				name: "y_connus",
				description: "représente une matrice ou une plage de cellules d'observations numériques dépendantes, et peut être des nombres, des noms, des matrices, ou des références qui contiennent des nombres"
			},
			{
				name: "x_connus",
				description: "représente la série d'observations indépendantes, et peut être des nombres, des noms, des matrices, ou des références qui contiennent des nombres"
			}
		]
	},
	{
		name: "PERMUTATION",
		description: "Renvoie le nombre de permutations pour un nombre donné d'objets.",
		arguments: [
			{
				name: "nombre",
				description: "représente un nombre entier correspondant au nombre d'objets"
			},
			{
				name: "nombre_choisi",
				description: "représente le nombre d'objets contenus dans chaque permutation"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Renvoie le nombre de permutations pour un nombre donné d'objets (avec répétitions).",
		arguments: [
			{
				name: "nombre",
				description: "représente un nombre entier correspondant au nombre d'objets"
			},
			{
				name: "nombre_choisi",
				description: "représente le nombre d'objets contenus dans chaque permutation"
			}
		]
	},
	{
		name: "PETITE.VALEUR",
		description: "Renvoie la k-ième plus petite valeur d'une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente une matrice ou une plage de données numériques dans laquelle vous recherchez la k-ième plus petite valeur"
			},
			{
				name: "k",
				description: "représente dans la matrice ou la plage, le rang de la donnée à renvoyer, déterminé à partir de la valeur la plus petite"
			}
		]
	},
	{
		name: "PGCD",
		description: "Renvoie le plus grand dénominateur commun.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 valeurs"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 valeurs"
			}
		]
	},
	{
		name: "PHI",
		description: "Renvoie la valeur de la fonction de densité pour une distribution suivant une loi normale centrée réduite.",
		arguments: [
			{
				name: "x",
				description: "représente le nombre pour lequel vous recherchez la densité de la distribution suivant une loi normale centrée réduite"
			}
		]
	},
	{
		name: "PI",
		description: "Renvoie la valeur de pi, 3,14159265358979 avec une précision de 15 chiffres.",
		arguments: [
		]
	},
	{
		name: "PLAFOND",
		description: "Arrondit un nombre au multiple le plus proche de l’argument précision en s’éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur que vous voulez arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir"
			}
		]
	},
	{
		name: "PLAFOND.MATH",
		description: "Arrondit un nombre à l'entier ou au multiple le plus proche de l'argument précision en s'éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur que vous voulez arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir"
			},
			{
				name: "mode",
				description: "lorsque cette fonction est fournie et différente de zéro, elle arrondira en s'éloignant de zéro"
			}
		]
	},
	{
		name: "PLAFOND.PRECIS",
		description: "Arrondit un nombre à l’entier ou au multiple le plus proche de l’argument précision en s’éloignant de zéro.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur que vous voulez arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir"
			}
		]
	},
	{
		name: "PLANCHER",
		description: "Arrondit un nombre à l’entier ou au multiple le plus proche de l’argument précision.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur à arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir. Le nombre et le degré de précision doivent être soit positifs tous les deux, soit tous les deux négatifs"
			}
		]
	},
	{
		name: "PLANCHER.MATH",
		description: "Arrondit un nombre à l'entier ou au multiple le plus proche de l'argument précision en tendant vers zéro.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur que vous voulez arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir"
			},
			{
				name: "mode",
				description: "lorsque cette fonction est fournie et différente de zéro, elle arrondira en tendant vers zéro"
			}
		]
	},
	{
		name: "PLANCHER.PRECIS",
		description: "Arrondit un nombre à l’entier ou au multiple le plus proche de l’argument précision en tendant vers zéro.",
		arguments: [
			{
				name: "nombre",
				description: "est la valeur à arrondir"
			},
			{
				name: "précision",
				description: "représente le multiple auquel vous voulez arrondir. Le nombre et le degré de précision doivent être soit positifs tous les deux, soit tous les deux négatifs"
			}
		]
	},
	{
		name: "PPCM",
		description: "Renvoie le plus petit dénominateur commun.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 valeurs dont vous voulez obtenir le plus petit dénominateur commun"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 valeurs dont vous voulez obtenir le plus petit dénominateur commun"
			}
		]
	},
	{
		name: "PREVISION",
		description: "Calcule, ou prédit, une valeur future suivant une tendance linéaire, en utilisant les valeurs existantes.",
		arguments: [
			{
				name: "x",
				description: "représente l'observation dont vous voulez prévoir la valeur et doit être une valeur numérique"
			},
			{
				name: "y_connus",
				description: "représente la matrice ou la plage de données numériques dépendante"
			},
			{
				name: "x_connus",
				description: "représente la matrice ou la plage de données numériques indépendante. La variance de x_connu doit être différente de zéro"
			}
		]
	},
	{
		name: "PRINCPER",
		description: "Calcule la part de remboursement du principal d'un emprunt, fondée sur des remboursements et un taux d'intérêt constants.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période"
			},
			{
				name: "pér",
				description: "indique la période et doit être compris entre 1 et le nombre total de périodes de paiement durant l'opération"
			},
			{
				name: "npm",
				description: "est le nombre total de remboursements durant l'opération"
			},
			{
				name: "va",
				description: "est la valeur actuelle, c'est-à-dire la valeur présente du total des remboursements futurs"
			},
			{
				name: "vc",
				description: "est la valeur future, c'est à dire la valeur résiduelle que vous voulez obtenir après le dernier remboursement"
			},
			{
				name: "type",
				description: "est une valeur logique : paiement au début de la période = 1 ; paiement à la fin de la période = 0 ou omis"
			}
		]
	},
	{
		name: "PRIX.BON.TRESOR",
		description: "Renvoie le prix d'un bon du trésor d'une valeur nominale de 100 €.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "taux_escompte",
				description: "est le taux du bon du Trésor"
			}
		]
	},
	{
		name: "PRIX.DEC",
		description: "Convertit la valeur des cotations boursières de la forme fractionnaire à la forme décimale.",
		arguments: [
			{
				name: "prix_fraction",
				description: "est un nombre exprimé sous la forme fractionnaire"
			},
			{
				name: "fraction",
				description: "est un nombre entier utilisé en dénominateur de la fraction"
			}
		]
	},
	{
		name: "PRIX.FRAC",
		description: "Convertit la valeur des cotations boursières de la forme décimale à la forme fractionnaire.",
		arguments: [
			{
				name: "prix_décimal",
				description: "est la valeur décimale à convertir"
			},
			{
				name: "fraction",
				description: "est un nombre entier utilisé en dénominateur de la fraction"
			}
		]
	},
	{
		name: "PROBABILITE",
		description: "Renvoie la probabilité pour les valeurs d'une plage d'être comprises entre deux limites ou égales à une limite inférieure.",
		arguments: [
			{
				name: "plage_x",
				description: "représente la plage des valeurs numériques de x auxquelles sont associées des probabilités"
			},
			{
				name: "plage_probabilité",
				description: "représente une série de probabilités associée aux valeurs de Plage_x, valeurs entre 0 et 1, 0 étant exclu"
			},
			{
				name: "limite_inf",
				description: "représente la limite inférieure de la valeur pour laquelle vous recherchez une probabilité"
			},
			{
				name: "limite_sup",
				description: "représente la limite supérieure facultative de la valeur. Si omise, PROBABILITE renvoie la probabilité que les valeurs de Plage_x soient égales à Limite_inf"
			}
		]
	},
	{
		name: "PRODUIT",
		description: "Donne le produit de la multiplication de tous les nombres donnés comme arguments.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, valeurs logiques, ou transcriptions textuelles des nombres que vous voulez multiplier"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, valeurs logiques, ou transcriptions textuelles des nombres que vous voulez multiplier"
			}
		]
	},
	{
		name: "PRODUITMAT",
		description: "Calcule le produit de deux matrices, sous forme d'une matrice avec le même nombre de ligne que la matrice1 et de colonnes que la matrice2.",
		arguments: [
			{
				name: "matrice1",
				description: "est la première matrice de nombres à multiplier, et doit avoir autant de colonnes que Matrice2 a de lignes"
			},
			{
				name: "matrice2",
				description: "est la première matrice de nombres à multiplier, et doit avoir autant de colonnes que Matrice2 a de lignes"
			}
		]
	},
	{
		name: "PUISSANCE",
		description: "Renvoie la valeur du nombre élevé à une puissance.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à élever à la puissance, n'importe quel nombre réel"
			},
			{
				name: "puissance",
				description: "est la puissance à laquelle le nombre est élevé"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Renvoie le quartile d’une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de cellules de valeurs numériques pour laquelle vous recherchez la valeur du quartile"
			},
			{
				name: "quart",
				description: "indique quelle valeur à renvoyer: valeur minimale = 0; 1er quartile = 1; quartile moyen = 2; 3e quartile = 3; valeur maximale = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXCLURE",
		description: "Renvoie le quartile d’une série de données, d’après des valeurs de centile comprises entre 0 et 1 non compris.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de cellules de valeurs numériques pour laquelle vous recherchez la valeur du quartile"
			},
			{
				name: "quart",
				description: "indique quelle valeur renvoyer: valeur minimale = 0 ; 1er quartile = 1 ; quartile moyen = 2 ; 3e quartile = 3 ; valeur maximale = 4"
			}
		]
	},
	{
		name: "QUARTILE.INCLURE",
		description: "Renvoie le quartile d’une série de données, d’après des valeurs de centile comprises entre 0 et 1 inclus.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de cellules de valeurs numériques pour laquelle vous recherchez la valeur du quartile"
			},
			{
				name: "quart",
				description: "indique quelle valeur renvoyer: valeur minimale = 0 ; 1er quartile = 1 ; quartile moyen = 2 ; 3e quartile = 3 ; valeur maximale = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Renvoie la partie entière du résultat d'une division.",
		arguments: [
			{
				name: "numérateur",
				description: "est le dividende"
			},
			{
				name: "dénominateur",
				description: "est le diviseur"
			}
		]
	},
	{
		name: "RACINE",
		description: "Donne la racine carrée d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre dont vous voulez obtenir la racine carrée"
			}
		]
	},
	{
		name: "RACINE.PI",
		description: "Donne la racine carrée du produit (nombre * pi).",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre auquel vous voulez multiplier PI"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Convertit des degrés en radians.",
		arguments: [
			{
				name: "angle",
				description: "désigne l'angle en degrés que vous voulez convertir"
			}
		]
	},
	{
		name: "RANG",
		description: "Renvoie le rang d’un nombre dans une liste d’arguments : sa taille est relative aux autres valeurs de la liste.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre dont vous voulez connaître le rang"
			},
			{
				name: "référence",
				description: "est une matrice, ou une référence à une liste de nombres. Les valeurs non numériques sont ignorées dans la référence"
			},
			{
				name: "ordre",
				description: "est un numéro : le rang de l’argument dans la liste triée par ordre décroissant = 0 ou omis ; son rang dans la liste triée par ordre croissant = toute valeur différente de zéro"
			}
		]
	},
	{
		name: "RANG.POURCENTAGE",
		description: "Renvoie le rang en pourcentage d’une valeur d’une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données de valeurs numériques définissant l’étendue relative"
			},
			{
				name: "x",
				description: "représente la valeur dont vous voulez connaître le rang"
			},
			{
				name: "précision",
				description: "représente une valeur facultative indiquant le nombre de décimales du pourcentage renvoyé, 3 chiffres après la décimale si omise (0,xxx %)"
			}
		]
	},
	{
		name: "RANG.POURCENTAGE.EXCLURE",
		description: "Renvoie le rang en pourcentage (0..1, non compris) d’une valeur d’une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données de valeurs numériques définissant l’étendue relative"
			},
			{
				name: "x",
				description: "représente la valeur dont vous voulez connaître le rang"
			},
			{
				name: "précision",
				description: "représente une valeur facultative indiquant le nombre de décimales du pourcentage renvoyé, 3 chiffres après la décimale si omise (0,xxx %)"
			}
		]
	},
	{
		name: "RANG.POURCENTAGE.INCLURE",
		description: "Renvoie le rang en pourcentage (0..1, inclus) d’une valeur d’une série de données.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données de valeurs numériques définissant l’étendue relative"
			},
			{
				name: "x",
				description: "représente la valeur dont vous voulez connaître le rang"
			},
			{
				name: "précision",
				description: "représente une valeur facultative indiquant le nombre de décimales du pourcentage renvoyé, 3 chiffres après la décimale si omise (0,xxx %)"
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
		name: "RECHERCHE",
		description: "Renvoie une valeur soit à partir d'une plage d'une ligne ou d'une colonne, soit à partir d'une matrice. Fournie pour la compatibilité ascendante.",
		arguments: [
			{
				name: "valeur_cherchée",
				description: "est la valeur que la fonction cherche dans un tableau ou un vecteur; cela peut être un nombre, du texte, une valeur logique, un nom ou une référence à une valeur"
			},
			{
				name: "vecteur_recherche",
				description: "représente une plage d'une seule ligne ou colonne de texte, de nombres ou de valeurs logiques rangés en ordre croissant"
			},
			{
				name: "vecteur_résultat",
				description: "est une plage qui contient une seule ligne ou colonne, de la même taille que vecteur_cherché"
			}
		]
	},
	{
		name: "RECHERCHEH",
		description: "Cherche une valeur dans la première ligne d'une matrice de valeurs ou d'un tableau et renvoie la valeur de la même colonne à partir d'une ligne spécifiée.",
		arguments: [
			{
				name: "valeur_cherchée",
				description: "représente la valeur recherchée dans le premier rang du tableau. Il peut s'agir d'une valeur, d'une référence ou d'un texte"
			},
			{
				name: "tableau",
				description: "est un tableau de données (texte, nombres, valeurs logiques) dans lequel est exécutée la recherche de la valeur. L'argument table_matrice peut être une référence à une plage ou à un nom de plage de données"
			},
			{
				name: "no_index_lig",
				description: "est le numéro de la ligne de l'argument table_matrice dont la valeur correspondante est renvoyée. La première ligne des valeurs dans la table est la ligne 1"
			},
			{
				name: "valeur_proche",
				description: "est une valeur logique: pour trouver la valeur la plus proche dans la ligne du haut (tri par ordre croissant) = VRAI ou omis; pour trouver une valeur exactement identique = FAUX"
			}
		]
	},
	{
		name: "RECHERCHEV",
		description: "Cherche une valeur dans la première colonne à gauche d'un tableau, puis renvoie une valeur dans la même ligne à partir d'une colonne spécifiée. Par défaut, le tableau doit être trié par ordre croissant.",
		arguments: [
			{
				name: "valeur_cherchée",
				description: "est la valeur à trouver dans la première colonne du tableau, et peut être une valeur, une référence, ou une chaîne textuelle"
			},
			{
				name: "table_matrice",
				description: "est un tableau de texte, nombres, valeurs logiques, à partir duquel les données sont récupérées. L'argument table_matrice peut être une plage de cellules ou le nom d'une plage"
			},
			{
				name: "no_index_col",
				description: "est le numéro de la colonne de l'argument table_matrice dont la valeur correspondante est renvoyée. La première colonne de valeurs dans le tableau est la colonne 1"
			},
			{
				name: "valeur_proche",
				description: "est une valeur logique: pour trouver la valeur la plus proche dans la première colonne (triée par ordre croissant) = VRAI ou omis; pour trouver la correspondance exacte = FAUX"
			}
		]
	},
	{
		name: "REMPLACER",
		description: "Remplace une chaîne de caractères par une autre.",
		arguments: [
			{
				name: "ancien_texte",
				description: "est le texte dans lequel vous voulez remplacer des caractères"
			},
			{
				name: "no_départ",
				description: "est la position du caractère dans ancien_texte que vous voulez remplacer par nouveau_texte"
			},
			{
				name: "no_car",
				description: "est le nombre de caractères que vous voulez remplacer dans l'ancien texte"
			},
			{
				name: "nouveau_texte",
				description: "est le texte qui va remplacer des caractères de ancien_texte"
			}
		]
	},
	{
		name: "RENDEMENT.BON.TRESOR",
		description: "Calcule le taux de rendement d'un bon du trésor.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "valeur_nominale",
				description: "est la valeur acquise pour un emprunt de 100 €"
			}
		]
	},
	{
		name: "RENDEMENT.SIMPLE",
		description: "Calcule le taux de rendement d'un titre escompté, tel qu'un bon du trésor.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "valeur_nominale",
				description: "est la somme empruntée"
			},
			{
				name: "valeur_rachat",
				description: "est la valeur de rachat pour 100 € de valeur nominale"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "REPT",
		description: "Répète un texte un certain nombre de fois. Utilisez REPT pour remplir une cellule avec un certain nombre d'occurrences d'une chaîne de caractères.",
		arguments: [
			{
				name: "texte",
				description: "est le texte que vous voulez répéter"
			},
			{
				name: "no_fois",
				description: "est un nombre positif indiquant le nombre de fois que texte doit être répété"
			}
		]
	},
	{
		name: "ROMAIN",
		description: "Convertit un chiffre arabe en chiffre romain sous forme de texte.",
		arguments: [
			{
				name: "nombre",
				description: "est le chiffre arabe que vous voulez convertir"
			},
			{
				name: "type",
				description: "est le numéro qui détermine le type de chiffre romain que doit renvoyer la fonction."
			}
		]
	},
	{
		name: "RTD",
		description: "Récupère les données en temps réel à partir d'un programme prenant en charge l'automation COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "est le nom du ProgID d'un complément d'automation COM enregistré. Le nom doit être placé entre guillemets"
			},
			{
				name: "serveur",
				description: "est le nom du serveur dans lequel le complément doit être exécuté. Le nom doit être placé entre guillemets. Si le complément est exécuté en local, utilisez une chaîne vide"
			},
			{
				name: "rubrique1",
				description: "1 à 38 paramètres spécifiant des données"
			},
			{
				name: "rubrique2",
				description: "1 à 38 paramètres spécifiant des données"
			}
		]
	},
	{
		name: "SEC",
		description: "Renvoie la sécante d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la sécante"
			}
		]
	},
	{
		name: "SECH",
		description: "Renvoie la sécante hyperbolique d'un angle.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians dont vous voulez obtenir la sécante hyperbolique"
			}
		]
	},
	{
		name: "SECONDE",
		description: "Donne les secondes, un entier entre 0 et 59. .",
		arguments: [
			{
				name: "numéro_de_série",
				description: "est un nombre dans le code date-heure utilisé par Spreadsheet, ou du texte au format horaire, par exemple 16:48:23 ou 4:48:47 P.M"
			}
		]
	},
	{
		name: "SERIE.JOUR.OUVRE",
		description: "Renvoie le numéro de série d'une date située un nombre de jours ouvrés avant ou après une date donnée.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "nb_jours",
				description: "représente le nombre de jours hors week-ends et hors jours fériés avant ou après la date de départ"
			},
			{
				name: "jours_fériés",
				description: "est une matrice optionnelle contenant un ou plusieurs numéros de série de date à exclure du calendrier des jours ouvrés (jours de fêtes nationales, etc.)"
			}
		]
	},
	{
		name: "SERIE.JOUR.OUVRE.INTL",
		description: "Renvoie le numéro de série d’une date située un nombre de jours ouvrés avant ou après une date donnée avec des paramètres de week-end personnalisés.",
		arguments: [
			{
				name: "date_départ",
				description: "est la date de départ, exprimée sous forme de numéro de série"
			},
			{
				name: "nb_jours",
				description: "représente le nombre de jours hors week-ends et hors jours fériés avant ou après la date de départ"
			},
			{
				name: "nb_jours_week-end",
				description: "représente un nombre ou une chaîne qui spécifie quand les weekends ont lieu"
			},
			{
				name: "jours_fériés",
				description: "est une matrice facultative contenant un ou plusieurs numéros de série de date à exclure du calendrier des jours ouvrés (jours de fêtes nationales, etc.)"
			}
		]
	},
	{
		name: "SI",
		description: "Vérifie si la condition est respectée et renvoie une valeur si le résultat d'une condition que vous avez spécifiée est VRAI, et une autre valeur si le résultat est FAUX.",
		arguments: [
			{
				name: "test_logique",
				description: "est toute valeur ou expression dont le résultat peut être VRAI ou FAUX"
			},
			{
				name: "valeur_si_vrai",
				description: "représente la valeur renvoyée si test_logique est VRAI. Si omis, VRAI est renvoyé. Vous pouvez utiliser jusqu'à sept fonctions SI"
			},
			{
				name: "valeur_si_faux",
				description: "représente la valeur renvoyée si test logique est FAUX. Si omis, FAUX est renvoyé"
			}
		]
	},
	{
		name: "SI.NON.DISP",
		description: "Renvoie la valeur que vous avez spécifiée si le résultat de l'expression est #N/A, sinon renvoie le résultat de l'expression.",
		arguments: [
			{
				name: "valeur",
				description: "représente n'importe quelle valeur, expression ou référence"
			},
			{
				name: "valeur_si_na",
				description: "représente n'importe quelle valeur, expression ou référence"
			}
		]
	},
	{
		name: "SIERREUR",
		description: "Renvoie « valeur_si_erreur » si l'expression est une erreur et la valeur de l'expression dans le cas contaire.",
		arguments: [
			{
				name: "valeur",
				description: "représente n'importe quelle valeur, expression ou référence"
			},
			{
				name: "valeur_si_erreur",
				description: "représente n'importe quelle valeur, expression ou référence"
			}
		]
	},
	{
		name: "SIGNE",
		description: "Donne le signe d'un nombre: 1 si le nombre est zéro, ou -1 si le nombre est négatif.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel"
			}
		]
	},
	{
		name: "SIN",
		description: "Renvoie le sinus d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians duquel vous voulez connaître le sinus. Degrés * pi()/180 = radians"
			}
		]
	},
	{
		name: "SINH",
		description: "Renvoie le sinus hyperbolique d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel"
			}
		]
	},
	{
		name: "SOMME",
		description: "Calcule la somme des nombres dans une plage de cellules.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments dont vous voulez calculer la somme. Les valeurs logiques et le texte sont ignorés dans les cellules, même s'ils sont tapés en tant qu'arguments"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments dont vous voulez calculer la somme. Les valeurs logiques et le texte sont ignorés dans les cellules, même s'ils sont tapés en tant qu'arguments"
			}
		]
	},
	{
		name: "SOMME.CARRES",
		description: "Renvoie la somme des carrés des arguments. Les arguments peuvent être des nombres, des matrices, des noms ou des références à des cellules qui contiennent des nombres.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 nombres, matrices, noms, ou références à des matrices dont vous recherchez la somme des carrés"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 nombres, matrices, noms, ou références à des matrices dont vous recherchez la somme des carrés"
			}
		]
	},
	{
		name: "SOMME.CARRES.ECARTS",
		description: "Renvoie la somme des carrés des écarts entre les points de données et leur moyenne échantillonnée.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments, une matrice ou une référence à une matrice, auxquels appliquer une SOMME.CARRES.ECARTS"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments, une matrice ou une référence à une matrice, auxquels appliquer une SOMME.CARRES.ECARTS"
			}
		]
	},
	{
		name: "SOMME.SERIES",
		description: "Renvoie la somme d'une série géométrique s'appuyant sur la formule.",
		arguments: [
			{
				name: "x",
				description: "est la variable de la série"
			},
			{
				name: "n",
				description: "est la puissance de départ de la variable x de la série"
			},
			{
				name: "m",
				description: "est le pas géométrique de la série"
			},
			{
				name: "coefficients",
				description: "est la liste des coefficients de la série"
			}
		]
	},
	{
		name: "SOMME.SI",
		description: "Additionne des cellules spécifiées selon un certain critère.",
		arguments: [
			{
				name: "plage",
				description: "représente la plage des cellules sur lesquelles vous voulez appliquer la fonction"
			},
			{
				name: "critère",
				description: "représente la condition ou le critère, sous forme de nombre, d'expression ou de texte, définissant les cellules à additionner"
			},
			{
				name: "somme_plage",
				description: "représente les cellules qui seront effectivement additionnées. Par défaut, les cellules dans la plage seront utilisées"
			}
		]
	},
	{
		name: "SOMME.SI.ENS",
		description: "Additionne les cellules indiquées par un ensemble de conditions ou de critères donné.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "plage_somme",
				description: "cellules à additionner."
			},
			{
				name: "plage_critères",
				description: "représente la plage de cellules servant de base à l'évaluation"
			},
			{
				name: "critères",
				description: "représente la condition ou les critères sous la forme d'un nombre, d'une expression ou de texte définissant quelles cellules doivent être additionnées"
			}
		]
	},
	{
		name: "SOMME.X2MY2",
		description: "Calcule la différence entre les carrés des nombres correspondants dans deux plages ou matrices, puis renvoie la somme des différences. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "matrice_x",
				description: "est la première plage ou matrice de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "matrice_y",
				description: "est la seconde plage ou matrice de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient un nombre"
			}
		]
	},
	{
		name: "SOMME.X2PY2",
		description: "Calcule la somme des carrés des nombres correspondants dans deux plages ou matrices, puis renvoie le total de l'addition des sommes. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "matrice_x",
				description: "est la première plage ou matrice de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "matrice_y",
				description: "est la seconde plage ou matrice de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient un nombre"
			}
		]
	},
	{
		name: "SOMME.XMY2",
		description: "Renvoie la somme des carrés des différences entre les valeurs correspondantes de deux matrices. Consultez l'aide sur l'équation utilisée.",
		arguments: [
			{
				name: "matrice_x",
				description: "est la première matrice ou plage de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			},
			{
				name: "matrice_y",
				description: "est la seconde matrice ou plage de valeurs et peut être un nombre, un nom, une matrice, ou une référence qui contient des nombres"
			}
		]
	},
	{
		name: "SOMMEPROD",
		description: "Donne la somme des produits des plages ou matrices correspondantes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrice1",
				description: "représentent de 2 à 255 matrices dont vous voulez multiplier puis ajouter les composants. Toutes les matrices doivent être de mêmes dimensions"
			},
			{
				name: "matrice2",
				description: "représentent de 2 à 255 matrices dont vous voulez multiplier puis ajouter les composants. Toutes les matrices doivent être de mêmes dimensions"
			},
			{
				name: "matrice3",
				description: "représentent de 2 à 255 matrices dont vous voulez multiplier puis ajouter les composants. Toutes les matrices doivent être de mêmes dimensions"
			}
		]
	},
	{
		name: "SOUS.TOTAL",
		description: "Renvoie un sous-total dans une liste ou une base de données.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "no_fonction",
				description: "représente un nombre de 1 à 11 déterminant quelle fonction de synthèse utiliser pour calculer le sous-total."
			},
			{
				name: "réf1",
				description: "représentent les 1 à 254 plages ou références pour lesquelles vous voulez calculer le sous-total"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Évalue l'écart-type d'une population en se basant sur un échantillon, incluant les valeurs logiques et le texte. Le texte et les valeurs logiques évalués en tant que FAUX = 0 ; les valeurs logiques évaluées en tant que VRAI = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 valeurs correspondant à un échantillon de population et peuvent être des valeurs, des noms, ou des références à des valeurs"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 valeurs correspondant à un échantillon de population et peuvent être des valeurs, des noms, ou des références à des valeurs"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Calcule l'écart-type d'une population entière, en incluant les valeurs logiques et le texte. Le texte et les valeurs logiques évalués en tant que FAUX = 0 ; les valeurs logiques évaluées en tant que VRAI = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent les 1 à 255 valeurs correspondant à une population et peuvent être des valeurs, des noms, des matrices ou des références qui contiennent des valeurs"
			},
			{
				name: "valeur2",
				description: "représentent les 1 à 255 valeurs correspondant à une population et peuvent être des valeurs, des noms, des matrices ou des références qui contiennent des valeurs"
			}
		]
	},
	{
		name: "STXT",
		description: "Renvoie un nombre déterminé de caractères d'une chaîne de texte à partir de la position que vous indiquez.",
		arguments: [
			{
				name: "texte",
				description: "est la chaîne de texte qui contient les caractères que vous voulez extraire"
			},
			{
				name: "no_départ",
				description: "est la position du premier caractère à extraire. Le premier caractère du texte porte le numéro 1"
			},
			{
				name: "no_car",
				description: "indique le nombre de caractères à extraire du texte"
			}
		]
	},
	{
		name: "SUBSTITUE",
		description: "Remplace des caractères dans un texte.",
		arguments: [
			{
				name: "texte",
				description: "est la chaîne de texte entre guillemets ou la référence de la cellule contenant la chaîne de texte dont vous voulez remplacer des caractères"
			},
			{
				name: "ancien_texte",
				description: "est le texte qui doit être remplacé. Si la casse de l'ancien texte ne correspond pas à la casse du nouveau, le remplacement sera annulé"
			},
			{
				name: "nouveau_texte",
				description: "est le texte qui doit remplacer l'ancien texte"
			},
			{
				name: "no_position",
				description: "indique l'occurrence de l'ancien texte que vous voulez remplacer. Par défaut, toutes les occurrence de l'ancien texte seront remplacées"
			}
		]
	},
	{
		name: "SUP.SEUIL",
		description: "Vérifie si un nombre dépasse une valeur seuil.",
		arguments: [
			{
				name: "nombre",
				description: "représente la valeur à comparer au seuil"
			},
			{
				name: "seuil",
				description: "représente la valeur seuil"
			}
		]
	},
	{
		name: "SUPPRESPACE",
		description: "Supprime tous les espaces d'une chaîne de caractères, sauf les espaces simples entre les mots.",
		arguments: [
			{
				name: "texte",
				description: "est le texte dont vous voulez supprimer les espaces inutiles"
			}
		]
	},
	{
		name: "SYD",
		description: "Calcule l'amortissement d'un bien pour une période donnée sur la base de la méthode américaine Sum-of-Years Digits.",
		arguments: [
			{
				name: "coût",
				description: "est le coût initial du bien"
			},
			{
				name: "valeur_rés",
				description: "est la valeur résiduelle du bien ou valeur du bien à la fin de sa vie"
			},
			{
				name: "durée",
				description: "est la durée de vie utile du bien ou le nombre de périodes au cours desquelles le bien est amorti "
			},
			{
				name: "période",
				description: "est la période et doit être exprimée dans la même unité que la durée de vie"
			}
		]
	},
	{
		name: "T",
		description: "Contrôle si une valeur fait référence à du texte et renvoie le texte le cas échéant, ou renvoie des guillemets (texte vide) dans le cas contraire.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur à tester"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Renvoie la probabilité associée à un test T de Student.",
		arguments: [
			{
				name: "matrice1",
				description: "représente la première série de données"
			},
			{
				name: "matrice2",
				description: "représente la deuxième série de données"
			},
			{
				name: "uni/bilatéral",
				description: "spécifie le nombre de points de distribution à renvoyer : distribution unilatérale = 1; distribution bilatérale = 2"
			},
			{
				name: "type",
				description: "représente le type de test T : observations = 1, variance égale sur deux échantillons (homoscédastique) = 2, variance inégale sur deux échantillons = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Renvoie la tangente d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est l'angle exprimé en radians duquel vous voulez connaître la tangente. Degrés * pi()/180 = radians"
			}
		]
	},
	{
		name: "TANH",
		description: "Renvoie la tangente hyperbolique d'un nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est un nombre réel. Consultez l'aide pour l'équation utilisée"
			}
		]
	},
	{
		name: "TAUX",
		description: "Calcule le taux d'intérêt par période d'un prêt ou d'un investissement. Par exemple, utilisez 6%/4 pour des paiements trimestriels à 6% APR.",
		arguments: [
			{
				name: "npm",
				description: "est le nombre total de périodes de remboursement durant l'opération"
			},
			{
				name: "vpm",
				description: "est le montant de chaque remboursement périodique et ce montant ne peut pas être changé durant toute la durée de l'opération"
			},
			{
				name: "va",
				description: "est la valeur actuelle, c'est-à-dire la valeur présente du total des remboursements futurs"
			},
			{
				name: "vc",
				description: "est la valeur future, c'est-à-dire le montant de la trésorerie attendu après le dernier remboursement. Si omise, utilise VC = 0"
			},
			{
				name: "type",
				description: "est une valeur logique: paiement au début de la période = 1; paiement à la fin de la période = 0 ou omis"
			},
			{
				name: "estimation",
				description: "est votre estimation du taux; si omise, Estimation = 0,1 (10 %)"
			}
		]
	},
	{
		name: "TAUX.EFFECTIF",
		description: "Calcule le taux effectif à partir du taux nominal et du nombre de périodes.",
		arguments: [
			{
				name: "taux_nominal",
				description: "est le taux d'intérêt nominal"
			},
			{
				name: "nb_périodes",
				description: "est le nombre de périodes par an"
			}
		]
	},
	{
		name: "TAUX.ESCOMPTE",
		description: "Calcule le taux d'escompte d'un titre.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "valeur_nominale",
				description: "est la somme empruntée"
			},
			{
				name: "valeur_échéance",
				description: "est la valeur de rachat pour 100 € de valeur nominale"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "TAUX.ESCOMPTE.R",
		description: "Renvoie le taux d'escompte rationnel d'un bon du trésor.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance de l'emprunt, exprimée sous forme de numéro de série"
			},
			{
				name: "taux_escompte",
				description: "est le taux du bon du Trésor"
			}
		]
	},
	{
		name: "TAUX.INT.EQUIV",
		description: "Renvoie un taux d'intérêt équivalent pour la croissance d'un investissement.",
		arguments: [
			{
				name: "npm",
				description: "est le nombre de périodes pour l'investissement"
			},
			{
				name: "va",
				description: "est la valeur actuelle de l'investissement"
			},
			{
				name: "vc",
				description: "est la valeur future de l'investissement"
			}
		]
	},
	{
		name: "TAUX.INTERET",
		description: "Affiche le taux d'intérêt d'un titre totalement investi.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "investissement",
				description: "est la valeur investie dans le titre"
			},
			{
				name: "valeur_échéance",
				description: "est la somme d'argent reçue à échéance"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "TAUX.NOMINAL",
		description: "Calcule le taux d'intérêt nominal à partir du taux effectif et du nombre de périodes.",
		arguments: [
			{
				name: "taux_effectif",
				description: "est le taux d'intérêt effectif (composite)"
			},
			{
				name: "nb_périodes",
				description: "est le nombre de périodes par an"
			}
		]
	},
	{
		name: "TEMPS",
		description: "Convertit les heures, minutes et secondes données sous forme de nombres en un numéro de série Spreadsheet , selon un format d'heure.",
		arguments: [
			{
				name: "heure",
				description: "est un nombre compris entre 0 et 23 qui représente l'heure"
			},
			{
				name: "minute",
				description: "est un nombre compris entre 0 et 59 qui représente les minutes"
			},
			{
				name: "seconde",
				description: "est un nombre compris entre 0 et 59 qui représente les secondes"
			}
		]
	},
	{
		name: "TEMPSVAL",
		description: "Convertit une heure donnée sous forme de texte en un numéro de série (temps_texte).",
		arguments: [
			{
				name: "heure_texte",
				description: "est une chaîne de texte entre guillemets qui indique une heure dans un des formats d’heure de Spreadsheet (l’information sur la date dans la chaîne de caractères est ignorée)"
			}
		]
	},
	{
		name: "TENDANCE",
		description: "Calcule les valeurs de la courbe de tendance linéaire par la méthode des moindres carrés, appliquée aux valeurs connues.",
		arguments: [
			{
				name: "y_connus",
				description: "est une plage ou une matrice de la série des valeurs y déjà connues par la relation y = mx + b"
			},
			{
				name: "x_connus",
				description: "représente une plage facultative ou table de valeurs x connues par la relation y=mx+b, une matrice de même dimension que y_connus"
			},
			{
				name: "x_nouveaux",
				description: "est la plage ou la matrice de la nouvelle série de variables x dont vous voulez que TENDANCE vous donne les valeurs y correspondantes"
			},
			{
				name: "constante",
				description: "est une valeur logique: la constante b est calculée normalement si Const = VRAI ou omise; b est égale à 0 si Const = FAUX"
			}
		]
	},
	{
		name: "TEST.F",
		description: "Renvoie le résultat d’un test F, c’est-à-dire la probabilité d’une variable aléatoire continue que les variances dans Matrice1 et Matrice2 ne soient pas différentes de manière significative.",
		arguments: [
			{
				name: "matrice1",
				description: "représente la première matrice ou plage de données et peut être des nombres, des noms, des matrices, ou des références qui contiennent des nombres (les cellules vides sont ignorées)"
			},
			{
				name: "matrice2",
				description: "représente la seconde matrice ou plage de données et peut être des nombres, des noms, des matrices ou des références qui contiennent des nombres (les cellules vides sont ignorées)"
			}
		]
	},
	{
		name: "TEST.KHIDEUX",
		description: "Renvoie le test d’indépendance : la valeur pour la statistique suivant la loi du Khi-deux et les degrés de liberté appropriés.",
		arguments: [
			{
				name: "plage_réelle",
				description: "représente la plage de données contenant les observations à comparer aux valeurs attendues"
			},
			{
				name: "plage_attendue",
				description: "représente la plage de données contenant le rapport du produit des totaux de lignes et des totaux de colonnes sur le total général"
			}
		]
	},
	{
		name: "TEST.STUDENT",
		description: "Renvoie la probabilité associée à un test T de Student.",
		arguments: [
			{
				name: "matrice1",
				description: "représente la première série de données"
			},
			{
				name: "matrice2",
				description: "représente la seconde série de données"
			},
			{
				name: "uni/bilatéral",
				description: "indique le type de distribution à renvoyer : unilatérale = 1; bilatérale = 2"
			},
			{
				name: "type",
				description: "représente le type de test t: par paires = 1, 2 exemples, variance égale (homoscédastique) = 2, variance inégale à 2 exemples = 3"
			}
		]
	},
	{
		name: "TEST.Z",
		description: "Renvoie la valeur unilatérale P du test Z.",
		arguments: [
			{
				name: "matrice",
				description: "représente la matrice ou la plage de données par rapport à laquelle tester x"
			},
			{
				name: "x",
				description: "représente la valeur à tester"
			},
			{
				name: "sigma",
				description: "représente l’écart-type (connu) de la population. Si omis, l’écart-type de l’échantillon est utilisé"
			}
		]
	},
	{
		name: "TEXTE",
		description: "Convertit un nombre en texte.",
		arguments: [
			{
				name: "valeur",
				description: "est un nombre, une formule dont le résultat est une valeur numérique, ou une référence à une cellule contenant une valeur numérique"
			},
			{
				name: "format_texte",
				description: "est un format de nombres en format texte de la boîte de dialogue Format de cellule, onglet Nombres, case Catégorie"
			}
		]
	},
	{
		name: "TRANSPOSE",
		description: "Change une plage de cellules verticale en plage horizontale, et vice-versa.",
		arguments: [
			{
				name: "tableau",
				description: "est un tableau dans une feuille de calcul ou une feuille macro que vous voulez transposer"
			}
		]
	},
	{
		name: "TRI",
		description: "Calcule le taux de rentabilité interne d'un investissement pour une succession de trésoreries.",
		arguments: [
			{
				name: "valeurs",
				description: "est une matrice ou une référence à des cellules qui contient des nombres dont vous voulez calculer le taux de rentabilité interne"
			},
			{
				name: "estimation",
				description: "est le taux que vous estimez être le plus proche du résultat de TRI; 0,1 (10 pourcent) si omis"
			}
		]
	},
	{
		name: "TRI.PAIEMENTS",
		description: "Calcule le taux de rentabilité interne d'un ensemble de paiements.",
		arguments: [
			{
				name: "valeurs",
				description: "est la série des paiements selon un calendrier"
			},
			{
				name: "dates",
				description: "est le calendrier de dates des paiements"
			},
			{
				name: "estimation",
				description: "est le taux que vous estimez être le plus proche du résultat de TRI.PAIEMENTS"
			}
		]
	},
	{
		name: "TRIM",
		description: "Calcule le taux de rentabilité interne pour une série de flux de trésorerie en fonction du coût de l'investissement et de l'intérêt sur le réinvestissement des liquidités.",
		arguments: [
			{
				name: "valeurs",
				description: "est une matrice ou une référence à des cellules qui contiennent des nombres représentant une série de débits (en négatif) et de crédits (en positif) à des dates régulières"
			},
			{
				name: "taux_emprunt",
				description: "est le taux d'intérêt payé pour le financement des besoins de trésorerie"
			},
			{
				name: "taux_placement",
				description: "est le taux d'intérêt perçu en cas de placement des excédents de trésorerie"
			}
		]
	},
	{
		name: "TRONQUE",
		description: "Renvoie la partie entière d'un nombre en enlevant la partie décimale ou fractionnaire du nombre.",
		arguments: [
			{
				name: "nombre",
				description: "est le nombre à tronquer"
			},
			{
				name: "no_chiffres",
				description: "est un nombre qui indique la précision de la troncature, 0 (zéro) par défaut"
			}
		]
	},
	{
		name: "TROUVE",
		description: "Renvoie la position de départ d'une chaîne de texte à l'intérieur d'une autre chaîne de texte. RECHERCHER distingue les majuscules des minuscules.",
		arguments: [
			{
				name: "texte_cherché",
				description: "est le texte que vous recherchez. Utilisez des guillemets (sans texte) pour trouver le premier caractère dans Texte; les caractères génériques ne sont pas autorisés"
			},
			{
				name: "texte",
				description: "est le texte contenant le texte recherché"
			},
			{
				name: "no_départ",
				description: "indique le caractère à partir duquel commencer la recherche. Le premier caractère dans l'argument texte est le chiffre 1. S'il est omis, No_départ = 1"
			}
		]
	},
	{
		name: "TYPE",
		description: "Renvoie un nombre indiquant le type de données d'une valeur: nombre = 1; texte = 2; valeur logique = 4; valeur d'erreur = 16; matrice = 64.",
		arguments: [
			{
				name: "valeur",
				description: "peut être n'importe quelle valeur"
			}
		]
	},
	{
		name: "TYPE.ERREUR",
		description: "Renvoie un numéro qui correspond à une valeur d'erreur.",
		arguments: [
			{
				name: "valeur",
				description: "est la valeur d'erreur dont vous voulez trouver le numéro d'identification ; il peut s'agir d'une valeur d'erreur véritable ou d'une référence à une cellule contenant une valeur d'erreur"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Renvoie le nombre (point de code) correspondant au premier caractère du texte.",
		arguments: [
			{
				name: "texte",
				description: "représente le caractère dont vous voulez obtenir la valeur Unicode"
			}
		]
	},
	{
		name: "URLENCODAGE",
		description: "Renvoie une chaîne encodée URL.",
		arguments: [
			{
				name: "texte",
				description: "représente une chaîne à encoder URL"
			}
		]
	},
	{
		name: "VA",
		description: "Calcule la valeur actuelle d'un investissement: la valeur actuelle du montant total d'une série de remboursements futurs.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période. Par exemple, utiliser 6%/4 pour des paiements trimestriels à 6%APR"
			},
			{
				name: "npm",
				description: "est le nombre total de remboursements durant l'opération"
			},
			{
				name: "vpm",
				description: "est le montant du remboursement pour chaque période et ce montant ne peut pas être changé durant toute la durée de l'opération"
			},
			{
				name: "vc",
				description: "est la valeur future, c'est-à-dire le montant que vous voulez obtenir après le dernier remboursement"
			},
			{
				name: "type",
				description: "est une valeur logique: paiement au début de la période = 1; paiement à la fin de la période = 0 ou omis"
			}
		]
	},
	{
		name: "VALEUR.ENCAISSEMENT",
		description: "Renvoie la valeur d'encaissement d'un escompte commercial, pour une valeur nominale de 100 €.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "taux",
				description: "est le taux d'escompte"
			},
			{
				name: "valeur_échéance",
				description: "est la valeur nominale"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "VALEUR.NOMINALE",
		description: "Renvoie la valeur nominale à l'échéance d'un titre entièrement investi.",
		arguments: [
			{
				name: "liquidation",
				description: "est la date de l'escompte exprimée sous forme de numéro de série"
			},
			{
				name: "échéance",
				description: "est la date d'échéance, exprimée sous forme de numéro de série"
			},
			{
				name: "investissement",
				description: "est la valeur investie dans le titre"
			},
			{
				name: "taux",
				description: "est le taux d'escompte"
			},
			{
				name: "base",
				description: "est la base annuelle utilisée pour le calcul"
			}
		]
	},
	{
		name: "VALEURNOMBRE",
		description: "Convertit le texte en nombre quels que soient les paramètres régionaux.",
		arguments: [
			{
				name: "texte",
				description: "est la chaîne représentant le nombre à convertir"
			},
			{
				name: "séparateur_décimal",
				description: "est le caractère servant de séparateur décimal dans la chaîne"
			},
			{
				name: "séparateur_groupe",
				description: "est le caractère servant de séparateur de groupes dans la chaîne"
			}
		]
	},
	{
		name: "VAN",
		description: "Calcule la valeur actuelle nette d'un investissement s'appuyant sur un taux d'escompte et une série de débits futurs (valeurs négatives) et de crédits (valeurs positives).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "taux",
				description: "est le taux d'actualisation pour une période"
			},
			{
				name: "valeur1",
				description: "sont de 1 à 254 encaissements et décaissements, répartis de façon égale dans le temps et se produisant à la fin de chaque période"
			},
			{
				name: "valeur2",
				description: "sont de 1 à 254 encaissements et décaissements, répartis de façon égale dans le temps et se produisant à la fin de chaque période"
			}
		]
	},
	{
		name: "VAN.PAIEMENTS",
		description: "Donne la valeur actuelle nette d'un ensemble de paiements planifiés.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt à appliquer"
			},
			{
				name: "valeurs",
				description: "est la série des paiements selon un calendrier"
			},
			{
				name: "dates",
				description: "est le calendrier de dates des paiements"
			}
		]
	},
	{
		name: "VAR",
		description: "Calcule la variance en se basant sur un échantillon (en ignorant les valeurs logiques et le texte de l’échantillon).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments numériques correspondant à un échantillon de population"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments numériques correspondant à un échantillon de population"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Calcule la variance d’une population entière (en ignorant les valeurs logiques et le texte de la population).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments numériques qui correspondent à la population"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments numériques qui correspondent à la population"
			}
		]
	},
	{
		name: "VAR.P.N",
		description: "Calcule la variance d’une population entière (ignore les valeurs logiques et le texte de la population).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments numériques qui correspondent à la population"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments numériques qui correspondent à la population"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Estime la variance en se basant sur un échantillon (ignore les valeurs logiques et le texte de l’échantillon).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nombre1",
				description: "représentent de 1 à 255 arguments numériques correspondant à un échantillon de population"
			},
			{
				name: "nombre2",
				description: "représentent de 1 à 255 arguments numériques correspondant à un échantillon de population"
			}
		]
	},
	{
		name: "VARA",
		description: "Estime la variance d'une population en se basant sur un échantillon, texte et valeurs logiques inclus. Le texte et la valeur logique évalués FAUX = 0 ; la valeur logique VRAI = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 arguments des valeurs correspondant à un échantillon de population"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 arguments des valeurs correspondant à un échantillon de population"
			}
		]
	},
	{
		name: "VARPA",
		description: "Calcule la variance d'une population en se basant sur une population entière, en incluant le texte et les valeurs logiques. Le texte et les valeurs logiques FAUX ont la valeur 0 ; la valeur logique VRAI = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "valeur1",
				description: "représentent de 1 à 255 arguments numériques correspondant à une population"
			},
			{
				name: "valeur2",
				description: "représentent de 1 à 255 arguments numériques correspondant à une population"
			}
		]
	},
	{
		name: "VC",
		description: "Calcule la valeur future d'un investissement fondé sur des paiements réguliers et constants, et un taux d'intérêt stable.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt par période.Par exemple, utilisez 6%/4 pour des paiements trimestriels à 6% APR"
			},
			{
				name: "npm",
				description: "est le nombre total de remboursements durant l'opération"
			},
			{
				name: "vpm",
				description: "est le montant du remboursement pour chaque période; ce montant est inchangé durant toute la durée de l'opération"
			},
			{
				name: "va",
				description: "est la valeur actuelle, ou la somme que représente aujourd'hui une série de paiements futurs. Si omis, Va = 0"
			},
			{
				name: "type",
				description: "est une valeur représentant l'échéancier du paiement: paiement au début de la période = 1; paiement à la fin de la période = 0 ou omis"
			}
		]
	},
	{
		name: "VC.PAIEMENTS",
		description: "Calcule la valeur future d'un investissement en appliquant une série de taux d'intérêt composites.",
		arguments: [
			{
				name: "va",
				description: "est la valeur actuelle"
			},
			{
				name: "taux",
				description: "est la matrice des taux d'intérêt à appliquer"
			}
		]
	},
	{
		name: "VDB",
		description: "Calcule l'amortissement d'un bien pour toute période que vous spécifiez, même partielle, en utilisant la méthode américaine Double-declining Balance ou toute autre méthode que vous spécifierez.",
		arguments: [
			{
				name: "coût",
				description: "est le coût initial du bien"
			},
			{
				name: "valeur_rés",
				description: "est la valeur résiduelle du bien ou valeur du bien à la fin de sa vie"
			},
			{
				name: "durée",
				description: "est la durée de vie utile du bien ou le nombre de périodes au cours desquelles le bien est amorti "
			},
			{
				name: "période_début",
				description: "est le début de la période de calcul de l'amortissement, exprimée dans la même unité que la durée de vie"
			},
			{
				name: "période_fin",
				description: "est le terme de la période de calcul de l'amortissement, exprimée dans la même unité que la durée de vie"
			},
			{
				name: "facteur",
				description: "est le taux auquel l'amortissement décroît, 2 par défaut"
			},
			{
				name: "valeur_log",
				description: "basculer vers la méthode de l'amortissement linéaire quand l'amortissement linéaire est supérieur à celui obtenu par la méthode de l'amortissement dégressif = FAUX ou omis; ne pas basculer = VRAI"
			}
		]
	},
	{
		name: "VPM",
		description: "Calcule le montant total de chaque remboursement périodique d'un investissement à remboursements et taux d'intérêt constants.",
		arguments: [
			{
				name: "taux",
				description: "est le taux d'intérêt du prêt par période.Par exemple, utilisez 6%/4 pour des paiements trimestriels à 6% APR"
			},
			{
				name: "npm",
				description: "est le nombre total de versements pour rembourser le prêt"
			},
			{
				name: "va",
				description: "est la valeur actuelle, c'est-à-dire la valeur présente du total des remboursements futurs"
			},
			{
				name: "vc",
				description: "est la valeur future ou valeur capitalisée, c'est-à-dire un montant que vous voulez obtenir après le dernier paiement, 0 (zéro) si omis"
			},
			{
				name: "type",
				description: "est une valeur logique: paiement au début de la période = 1; paiement à la fin de la période = 0 ou omis"
			}
		]
	},
	{
		name: "VRAI",
		description: "Renvoie la valeur logique VRAI.",
		arguments: [
		]
	},
	{
		name: "Z.TEST",
		description: "Renvoie la valeur P unilatérale d’un test Z.",
		arguments: [
			{
				name: "tableau",
				description: "représente la matrice ou la plage de données par rapport à laquelle tester X"
			},
			{
				name: "x",
				description: "représente la valeur à tester"
			},
			{
				name: "sigma",
				description: "représente l’écart-type (connu) de population. Si vous ne spécifiez pas, l’écart-type de l’échantillon est utilisé"
			}
		]
	},
	{
		name: "ZONES",
		description: "Renvoie le nombre de zones dans une référence. Une zone est une plage de cellules contiguës ou une seule cellule.",
		arguments: [
			{
				name: "référence",
				description: "est une référence à une cellule ou à une plage de cellules et peut faire référence à plusieurs zones"
			}
		]
	}
];