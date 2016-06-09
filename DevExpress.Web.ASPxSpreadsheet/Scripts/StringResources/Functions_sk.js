ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Vráti absolútnu hodnotu čísla, t.j. číslo bez znamienka + alebo -.",
		arguments: [
			{
				name: "číslo",
				description: "je reálne číslo, absolútnu hodnotu ktorého chcete zistiť"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Vráti akumulovaný úrok cenného papiera, za ktorý sa platí úrok k dátumu splatnosti.",
		arguments: [
			{
				name: "emisia",
				description: "je dátum emisie cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "sadzba",
				description: "je ročná úroková miera cenného papiera"
			},
			{
				name: "por",
				description: "je nominálna hodnota cenného papiera"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "ACOS",
		description: "Vráti hodnotu arkus kosínusu čísla v radiánoch v intervale O až pí. Arkus kosínus je uhol, ktorého kosínus je argument Číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota kosínusu daného uhla, ktorá musí byť v intervale -1 až 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Vráti hodnotu hyperbolického arkus kosínusu čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné číslo, ktoré sa rovná alebo je väčšie ako 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Vráti hodnotu arkuskotangensu čísla, v radiánoch v intervale od 0 do pí.",
		arguments: [
			{
				name: "number",
				description: "je kotangens požadovaného uhla"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Vráti inverzný hyperbolický kotangens čísla.",
		arguments: [
			{
				name: "number",
				description: "je hyperbolický kotangens požadovaného uhla"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Po zadaní čísla riadka a stĺpca vytvorí textový odkaz na bunku.",
		arguments: [
			{
				name: "číslo_riadka",
				description: "je číslo riadka použité v odkaze na bunku: Číslo_riadka = 1 pre riadok číslo 1"
			},
			{
				name: "číslo_stĺpca",
				description: "je číslo stĺpca použité v odkaze na bunku: Číslo_stĺpca = 4 pre stĺpec D"
			},
			{
				name: "abs_číslo",
				description: "určuje typ odkazu: absolútny = 1; absolútny riadok/relatívny stĺpec = 2; relatívny riadok/absolútny stĺpec = 3; relatívny = 4"
			},
			{
				name: "a1",
				description: "je logická hodnota určujúca typ odkazu: typ A1 = 1 alebo TRUE; typ R1C1 = 0 alebo FALSE"
			},
			{
				name: "text_hárka",
				description: "je text špecifikujúci hárok (jeho názov), ktorý sa má použiť ako externý odkaz"
			}
		]
	},
	{
		name: "AND",
		description: "Skontroluje, či všetky argumenty majú hodnotu TRUE, a v prípade, že to tak je, vráti hodnotu TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logická_hodnota1",
				description: "je 1 až 255 testovaných podmienok, ktoré môžu mať hodnotu TRUE alebo FALSE, pričom to môžu byť logické hodnoty, polia alebo odkazy"
			},
			{
				name: "logická_hodnota2",
				description: "je 1 až 255 testovaných podmienok, ktoré môžu mať hodnotu TRUE alebo FALSE, pričom to môžu byť logické hodnoty, polia alebo odkazy"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Konvertuje rímske číslice na arabské.",
		arguments: [
			{
				name: "text",
				description: "je rímska číslica, ktorú chcete konvertovať"
			}
		]
	},
	{
		name: "AREAS",
		description: "Vráti počet oblastí v odkaze. Oblasť môže byť jedna bunka alebo súvislý rozsah buniek.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na bunku alebo rozsah buniek a môže sa vzťahovať na viacero oblastí zároveň"
			}
		]
	},
	{
		name: "ASIN",
		description: "Vráti arkus sínus čísla v radiánoch v intervale -pí/2 až pí/2.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota sínusu daného uhla, ktorá musí byť v intervale -1 až 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Vráti hodnotu hyperbolického arkus sínusu čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné číslo, ktoré sa rovná alebo je väčšie ako 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Vráti arkus tangens čísla v radiánoch v intervale -pí/2 až pí/2.",
		arguments: [
			{
				name: "číslo",
				description: "je tangens daného čísla"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Vráti hodnotu arkus tangens danej x-ovej a y-ovej súradnice. Výsledok je v radiánoch v rozsahu od -pí do pí okrem hodnoty -pí.",
		arguments: [
			{
				name: "x_číslo",
				description: "je x-ová súradnica bodu"
			},
			{
				name: "y_číslo",
				description: "je y-ová súradnica bodu"
			}
		]
	},
	{
		name: "ATANH",
		description: "Vráti hodnotu hyperbolického arkustangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné číslo v intervale -1 až 1, s výnimkou čísel -1 a 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Vráti priemernú hodnotu absolútnych odchýlok údajových bodov od ich priemeru. Argumenty môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 argumentov, pre ktoré chcete zistiť priemernú hodnotu absolútnych odchýlok"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 argumentov, pre ktoré chcete zistiť priemernú hodnotu absolútnych odchýlok"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Vráti priemernú hodnotu argumentov (aritmetický priemer), pričom to môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentov, ktorých priemernú hodnotu chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentov, ktorých priemernú hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Vráti priemernú hodnotu (aritmetický priemer) argumentov. Text a logická hodnota FALSE = 0; TRUE = 1. Argumenty môžu byť čísla, názvy, polia alebo odkazy.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentov, ktorých priemer chcete zistiť"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentov, ktorých priemer chcete zistiť"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Vyhľadá priemer (aritmetický priemer) buniek stanovených podľa zadanej podmienky alebo kritéria.",
		arguments: [
			{
				name: "rozsah",
				description: "je rozsah buniek, ktoré chcete hodnotiť"
			},
			{
				name: "kritériá",
				description: "je podmienka alebo kritérium vo forme čísla, výrazu alebo textu definujúceho bunky, ktoré sa použijú na vyhľadanie priemeru"
			},
			{
				name: "priemerný_rozsah",
				description: "sú skutočné bunky, ktoré sa použijú na vyhľadanie priemeru. Ak sa táto hodnota vynechá, použijú sa bunky v rozsahu"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Vyhľadá priemer (aritmetický priemer) buniek, stanovených zadanou množinou podmienok alebo kritérií.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "priemerný_rozsah",
				description: "sú skutočné bunky, ktoré sa použijú na vyhľadanie priemeru."
			},
			{
				name: "rozsah_kritérií",
				description: "je rozsah buniek, ktoré chcete zhodnotiť podľa konkrétnej podmienky"
			},
			{
				name: "kritériá",
				description: "je podmienka alebo kritérium vo forme čísla, výrazu alebo textu definujúceho bunky, ktoré sa použijú na vyhľadanie priemeru"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Konvertuje číslo na text (baht).",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktoré chcete konvertovať"
			}
		]
	},
	{
		name: "BASE",
		description: "Konvertuje číslo na textové vyjadrenie s daným základom sústavy (základ).",
		arguments: [
			{
				name: "number",
				description: "je číslo, ktoré chcete konvertovať"
			},
			{
				name: "radix",
				description: "základ sústavy, na ktorý chcete skonvertovať číslo"
			},
			{
				name: "min_length",
				description: "je minimálna dĺžka vráteného reťazca. Ak sa vynechá, počiatočné nuly sa nepridajú"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Vráti upravenú Besselovu funkciu In(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa funkcia vyhodnotí"
			},
			{
				name: "n",
				description: "je poradie Besselovej funkcie"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Vráti Besselovu funkciu Jn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa funkcia hodnotí"
			},
			{
				name: "n",
				description: "je poradie Besselovej funkcie"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Vráti upravenú Besselovu funkciu Kn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa funkcia vyhodnotí"
			},
			{
				name: "n",
				description: "je poradie funkcie"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Vráti Besselovu funkciu Yn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa funkcia vyhodnotí"
			},
			{
				name: "n",
				description: "je poradie funkcie"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Vráti funkciu rozdelenia pravdepodobnosti beta.",
		arguments: [
			{
				name: "x",
				description: "je hodnota medzi hodnotami A a B, pri ktorej sa má funkcia vyhodnocovať"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia a musí byť väčší ako 0"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia a musí byť väčší ako 0"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak ide o funkciu kumulatívneho rozdelenia, použite hodnotu TRUE a ak ide o funkciu hustoty pravdepodobnosti, použite hodnotu FALSE"
			},
			{
				name: "A",
				description: "je voliteľná dolná hranica intervalu x. Ak sa vynechá, A = 0"
			},
			{
				name: "B",
				description: "je voliteľná horná hranica intervalu x. Ak sa vynechá, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Vráti inverznú hodnotu funkcie kumulatívnej hustoty pravdepodobnosti beta (BETA.DIST).",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená rozdeleniu beta"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia a musí byť väčší ako 0"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia a musí byť väčší ako 0"
			},
			{
				name: "A",
				description: "je voliteľná dolná hranica intervalu x. Ak sa vynechá, A = 0"
			},
			{
				name: "B",
				description: "je voliteľná horná hranica intervalu x. Ak sa vynechá, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Vráti hodnotu distribučnej funkcie rozdelenia pravdepodobnosti beta.",
		arguments: [
			{
				name: "x",
				description: "je hodnota medzi hodnotami argumentov A a B, pre ktorú chcete zistiť hodnotu funkcie"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť väčší ako 0"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť väčší ako 0"
			},
			{
				name: "A",
				description: "je voliteľná dolná hranica pre hodnoty x. Ak argument A nezadáte, jeho hodnota bude 0"
			},
			{
				name: "B",
				description: "je voliteľná horná hranica pre hodnoty x. Ak argument B nezadáte, jeho hodnota bude 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Vráti inverznú hodnotu súčtovej hustoty rozdelenia pravdepodobnosti beta (inverzná funkcia k funkcii BETADIST).",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť rozdelenia beta"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť väčší ako 0"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť väčší ako 0"
			},
			{
				name: "A",
				description: "je voliteľná dolná hranica pre hodnoty x. Ak argument A nezadáte, jeho hodnota bude 0"
			},
			{
				name: "B",
				description: "je voliteľná horná hranica pre hodnoty x. Ak argument B nezadáte, jeho hodnota bude 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Skonvertuje binárne číslo na desiatkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je binárne číslo, ktoré chcete skonvertovať"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Skonvertuje binárne číslo na šestnástkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je binárne číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Skonvertuje binárne číslo na osmičkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je binárne číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Vráti hodnotu binomického rozdelenia pravdepodobnosti jednotlivých veličín.",
		arguments: [
			{
				name: "číslo_s",
				description: "je počet úspešných pokusov"
			},
			{
				name: "pokusy",
				description: "je počet nezávislých pokusov"
			},
			{
				name: "pravdepodobnosť_s",
				description: "je pravdepodobnosť úspešného pokusu"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia = TRUE; funkcia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Vráti pravdepodobnosť skúšobného výsledku pomocou binomického rozdelenia.",
		arguments: [
			{
				name: "trials",
				description: "je počet nezávislých pokusov"
			},
			{
				name: "probability_s",
				description: "je pravdepodobnosť úspešnosti jednotlivých pokusov"
			},
			{
				name: "number_s",
				description: "je počet úspešných pokusov"
			},
			{
				name: "number_s2",
				description: "ak je zadaný, vráti pravdepodobnosť, že počet úspešných pokusov sa bude nachádzať medzi hodnotou argumentov number_s a number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Vráti najmenšiu hodnotu, pre ktorú má distribučná funkcia binomického rozdelenia hodnotu väčšiu alebo rovnajúcu sa hodnote kritéria.",
		arguments: [
			{
				name: "pokusy",
				description: "je počet Bernoulliho pokusov"
			},
			{
				name: "pravdepodobnosť_s",
				description: "je pravdepodobnosť úspešného pokusu. Argument je číslo od 0 do 1 vrátane oboch hodnôt"
			},
			{
				name: "alfa",
				description: "je hodnota kritéria. Je to číslo od 0 do 1 vrátane oboch hodnôt"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Vráti hodnotu binomického rozdelenia pravdepodobnosti jednotlivých veličín.",
		arguments: [
			{
				name: "počet_s",
				description: "je počet úspešných pokusov"
			},
			{
				name: "pokusy",
				description: "je počet nezávislých pokusov"
			},
			{
				name: "pravdepodobnosť_s",
				description: "je pravdepodobnosť úspešného pokusu"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia = TRUE; funkcia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Vráti bitový operátor AND dvoch čísel.",
		arguments: [
			{
				name: "number1",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			},
			{
				name: "number2",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Vráti číslo posunuté doľava o počet bitov určený argumentom shift_amount.",
		arguments: [
			{
				name: "number",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			},
			{
				name: "shift_amount",
				description: "je počet bitov, o ktoré chcete posunúť číslo doľava"
			}
		]
	},
	{
		name: "BITOR",
		description: "Vráti bitový operátor OR dvoch čísel.",
		arguments: [
			{
				name: "number1",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			},
			{
				name: "number2",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Vráti číslo posunuté doprava o počet bitov určený argumentom shift_amount.",
		arguments: [
			{
				name: "number",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			},
			{
				name: "shift_amount",
				description: "je počet bitov, o ktoré chcete posunúť číslo doprava"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Vráti bitový operátor Exclusive Or dvoch čísel.",
		arguments: [
			{
				name: "number1",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			},
			{
				name: "number2",
				description: "je desiatkovým zápisom binárneho čísla, ktoré chcete vyhodnotiť"
			}
		]
	},
	{
		name: "CEILING",
		description: "Zaokrúhli číslo smerom nahor na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "významnosť",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Zaokrúhli číslo nahor na najbližšie celé číslo alebo na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "number",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "significance",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť"
			},
			{
				name: "mode",
				description: "keď je zadaná nenulová hodnota, táto funkcia zaokrúhli číslo smerom od nuly"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Zaokrúhli číslo smerom nahor na najbližšie celé číslo alebo na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "významnosť",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť"
			}
		]
	},
	{
		name: "CELL",
		description: "Vráti v odkaze informácie o formátovaní, umiestnení alebo obsahu prvej bunky podľa poradia čítania hárka.",
		arguments: [
			{
				name: "typ_informácií",
				description: "je textová hodnota určujúca požadovaný typ informácií o bunke."
			},
			{
				name: "odkaz",
				description: "je bunka, o ktorej chcete získať informácie"
			}
		]
	},
	{
		name: "CHAR",
		description: "Vráti znak určený číslom kódu z tabuľky znakov používanej v danom počítači.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo v intervale 1 až 255 určujúce požadovaný znak"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Vráti sprava ohraničenú pravdepodobnosť pre rozdelenie chí-kvadrát.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť pravdepodobnosť. Musí to byť nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, ktorým môže byť číslo medzi 1 a 10^10 okrem čísla 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Vráti hodnotu funkcie inverznej k sprava ohraničenej pravdepodobnosti rozdelenia chí-kvadrát.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť rozdelenia chí-kvadrát. Argument je hodnota medzi 0 a 1 vrátane oboch hodnôt"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, ktorým môže byť číslo medzi 1 a 10^10 okrem čísla 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Vráti zľava ohraničenú pravdepodobnosť rozdelenia chí-kvadrát.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej chcete hodnotiť rozdelenie, a ide o nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, číslo od 1 do 10^10 a menšie než 10^10"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota pre funkciu, ktorá sa má vrátiť: ak ide o funkciu kumulatívneho rozdelenia, táto hodnota = TRUE a ak ide o funkciu hustoty pravdepodobnosti, táto hodnota = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Vráti sprava ohraničenú pravdepodobnosť rozdelenia chí-kvadrát.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej chcete hodnotiť rozdelenie, a ide o nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, číslo od 1 do 10^10 a menšie než 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Vráti inverznú hodnotu zľava ohraničenej pravdepodobnosti rozdelenia chí-kvadrát.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená k rozdeleniu chí-kvadrát, čo je hodnota od 0 do 1 vrátane"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, číslo od 1 do 10^10 a menšie než 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Vráti inverznú hodnotu sprava ohraničenej pravdepodobnosti rozdelenia chí-kvadrát.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená k rozdeleniu chí-kvadrát, čo je hodnota od 0 do 1 vrátane"
			},
			{
				name: "stupeň_voľnosti",
				description: "je počet stupňov voľnosti, číslo od 1 do 10^10 a menšie než 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Počíta test nezávislosti: hodnotu rozdelenia chí-kvadrát pre štatistiku a príslušný počet stupňov voľnosti.",
		arguments: [
			{
				name: "skutočný_rozsah",
				description: "je rozsah údajov obsahujúcich pozorovania, ktoré chcete testovať a porovnávať s predpokladanými hodnotami"
			},
			{
				name: "očakávaný_rozsah",
				description: "je rozsah údajov obsahujúcich podiel súčinu počtu riadkov a stĺpcov a celkového počtu"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Počíta test nezávislosti: hodnotu rozdelenia chí-kvadrát pre štatistiku a príslušný počet stupňov voľnosti.",
		arguments: [
			{
				name: "skutočný_rozsah",
				description: "je rozsah údajov obsahujúcich pozorovania, ktoré chcete testovať a porovnávať s predpokladanými hodnotami"
			},
			{
				name: "očakávaný_rozsah",
				description: "je rozsah údajov obsahujúcich podiel súčinu počtu riadkov a stĺpcov a celkového počtu"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Zo zoznamu hodnôt zvolí na základe zadaného čísla hodnotu alebo akciu, ktorá sa má vykonať.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_číslo",
				description: "určuje, ktorá hodnota argumentu bola vybratá. Hodnota argumentu Index_číslo musí byť medzi 1 a 254, alebo vzorec či odkaz na číslo medzi 1 a 254"
			},
			{
				name: "hodnota1",
				description: "je 1 až 254 hodnôt, odkazov na bunky, definovaných názvov, vzorcov, funkcií alebo textových argumentov, ktoré sú hodnotami funkcie CHOOSE"
			},
			{
				name: "hodnota2",
				description: "je 1 až 254 hodnôt, odkazov na bunky, definovaných názvov, vzorcov, funkcií alebo textových argumentov, ktoré sú hodnotami funkcie CHOOSE"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Odstráni z textu všetky znaky, ktoré nemožno vytlačiť.",
		arguments: [
			{
				name: "text",
				description: "je ľubovoľná informácia z hárka, z ktorej chcete odstrániť všetky znaky, ktoré nemožno vytlačiť"
			}
		]
	},
	{
		name: "CODE",
		description: "Vráti číselný kód prvého znaku textového reťazca z tabuľky znakov na danom počítači.",
		arguments: [
			{
				name: "text",
				description: "je textový reťazec, pre ktorý chcete nájsť kód prvého znaku"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Vráti číslo stĺpca odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je bunka alebo súvislý rozsah buniek, ktorej alebo ktorého číslo stĺpca chcete zistiť. Ak tento argument nezadáte, použije sa bunka obsahujúca funkciu COLUMN"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Vráti počet stĺpcov v poli alebo odkaze.",
		arguments: [
			{
				name: "pole",
				description: "je pole, vzorec poľa alebo odkaz na rozsah buniek, pre ktorý chcete zistiť počet stĺpcov"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Vráti počet kombinácií pre zadaný počet položiek.",
		arguments: [
			{
				name: "počet",
				description: "je celkový počet položiek"
			},
			{
				name: "vybratý_počet",
				description: "je počet položiek v každej kombinácii"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Vráti počet kombinácií s opakovaniami pre daný počet položiek.",
		arguments: [
			{
				name: "number",
				description: "je celkový počet položiek"
			},
			{
				name: "number_chosen",
				description: "je počet položiek v jednotlivých kombináciách"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Skonvertuje reálny a imaginárny koeficient na komplexné číslo.",
		arguments: [
			{
				name: "reálne_číslo",
				description: "je reálny koeficient komplexného čísla"
			},
			{
				name: "i_číslo",
				description: "je imaginárny koeficient komplexného čísla"
			},
			{
				name: "prípona",
				description: "je prípona imaginárnej súčasti komplexného čísla"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Zlúči niekoľko textových reťazcov do jedného.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "je 1 až 255 textových reťazcov, ktoré chcete zlúčiť do jedného reťazca. Môžu to byť textové reťazce, čísla alebo odkazy na jednu bunku"
			},
			{
				name: "text2",
				description: "je 1 až 255 textových reťazcov, ktoré chcete zlúčiť do jedného reťazca. Môžu to byť textové reťazce, čísla alebo odkazy na jednu bunku"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Vráti interval spoľahlivosti pre strednú hodnotu základného súboru s použitím normálneho rozloženia.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti, pomocou ktorej sa vypočíta hladina spoľahlivosti. Argument je číslo väčšie ako 0 a menšie ako 1"
			},
			{
				name: "smerodajná_odch",
				description: "je známa smerodajná odchýlka základného súboru pre rozsah údajov. Hodnota argumentu Smerodajná_odch musí byť väčšia ako 0"
			},
			{
				name: "veľkosť",
				description: "je veľkosť vzorky"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Vráti interval spoľahlivosti pre strednú hodnotu súboru s použitím normálneho rozloženia.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti použitá na výpočet úrovne spoľahlivosti, číslo väčšie ako 0 a menšie ako 1"
			},
			{
				name: "smerodajná_odch",
				description: "je štandardná odchýlka súboru pre rozsah údajov a predpokladá sa, že je známa. Hodnota Smerodajná_odch musí byť väčšia než 0"
			},
			{
				name: "veľkosť",
				description: "je veľkosť vzorky"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Vráti interval spoľahlivosti pre strednú hodnotu súboru s použitím Studentovho T-rozdelenia.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti použitá na výpočet úrovne spoľahlivosti, číslo väčšie ako 0 a menšie ako 1"
			},
			{
				name: "smerodajná_odch",
				description: "je štandardná odchýlka súboru pre rozsah údajov a predpokladá sa, že je známa. Hodnota Smerodajná_odch musí byť väčšia než 0"
			},
			{
				name: "veľkosť",
				description: "je veľkosť vzorky"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Skonvertuje číslo z jedného systému merania na iný.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota z_jednotiek, ktorá sa má konvertovať"
			},
			{
				name: "z_jednotky",
				description: "je jednotka pre číslo"
			},
			{
				name: "na_jednotku",
				description: "je jednotka pre výsledok"
			}
		]
	},
	{
		name: "CORREL",
		description: "Vráti korelačný koeficient pre dva súbory údajov.",
		arguments: [
			{
				name: "pole1",
				description: "je rozsah buniek s hodnotami. Hodnoty môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "pole2",
				description: "je druhý rozsah buniek s hodnotami. Hodnoty môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "COS",
		description: "Vráti kosínus uhla.",
		arguments: [
			{
				name: "číslo",
				description: "je uhol v radiánoch, ktorého kosínus chcete zistiť"
			}
		]
	},
	{
		name: "COSH",
		description: "Vráti hyperbolický kosínus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné reálne číslo"
			}
		]
	},
	{
		name: "COT",
		description: "Vráti kotangens uhla.",
		arguments: [
			{
				name: "number",
				description: "je uhol v radiánoch, ktorého kosínus chcete zistiť"
			}
		]
	},
	{
		name: "COTH",
		description: "Vráti hyperbolický kotangens čísla.",
		arguments: [
			{
				name: "nuber",
				description: "je uhol v radiánoch, ktorého hyperbolický kotangens chcete zistiť"
			}
		]
	},
	{
		name: "COUNT",
		description: "Spočíta počet buniek v rozsahu, ktorý obsahuje čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentov, ktoré môžu obsahovať alebo odkazovať na rôzne typy údajov, počítajú sa však iba čísla"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentov, ktoré môžu obsahovať alebo odkazovať na rôzne typy údajov, počítajú sa však iba čísla"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Spočíta bunky v rozsahu, ktoré nie sú prázdne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentov predstavujúcich hodnoty a bunky, ktoré chcete spočítať. Hodnoty môžu predstavovať ľubovoľný typ informácií"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentov predstavujúcich hodnoty a bunky, ktoré chcete spočítať. Hodnoty môžu predstavovať ľubovoľný typ informácií"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Vráti počet prázdnych buniek v danom rozsahu.",
		arguments: [
			{
				name: "rozsah",
				description: "je rozsah buniek, v ktorom chcete spočítať prázdne bunky"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Spočíta bunky v danom rozsahu, ktoré spĺňajú zadanú podmienku.",
		arguments: [
			{
				name: "rozsah",
				description: "je rozsah buniek, v ktorom chcete spočítať bunky, ktoré nie sú prázdne"
			},
			{
				name: "kritériá",
				description: "je podmienka vo forme čísla, výrazu alebo textu, ktorá presne definuje bunky, ktoré sa spočítajú"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Spočíta počet buniek podľa zadanej množiny podmienok alebo kritérií.",
		arguments: [
			{
				name: "rozsah_kritérií",
				description: "je rozsah buniek, ktoré chcete hodnotiť podľa konkrétnych podmienok"
			},
			{
				name: "kritériá",
				description: "je podmienka vo forme čísla, výrazu alebo textu definujúceho bunky, ktoré sa budú počítať"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Vráti počet dní od začiatku úrokového obdobia až po dátum vyrovnania.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "frekvencia",
				description: "je počet splátok úroku za rok"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Vráti ďalší dátum splatnosti úroku po dátume vyrovnania.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "frekvencia",
				description: "je počet splátok úroku za rok"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Vráti počet splátok úroku medzi dátumom vyrovnania a dátumom splatnosti.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "frekvencia",
				description: "je počet splátok úroku za rok"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Vráti dátum splatnosti úroku, ktorý predchádza dátumu vyrovnania.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "frekvencia",
				description: "je počet splátok úroku za rok"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "COVAR",
		description: "Vráti hodnotu kovariancie, priemernú hodnotu súčinu odchýlok pre každú dvojicu údajových bodov v dvoch množinách údajov.",
		arguments: [
			{
				name: "pole1",
				description: "je prvý rozsah buniek obsahujúcich celé čísla. Hodnoty argumentu musia byť čísla, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "pole2",
				description: "je druhý rozsah buniek obsahujúcich celé čísla. Hodnoty argumentu musia byť čísla, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Vráti kovarianciu súboru, priemer súčinov odchýlok pre každý pár údajových bodov v dvoch množinách údajov.",
		arguments: [
			{
				name: "pole1",
				description: "je prvý rozsah buniek celých čísel a musia to byť čísla, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "pole2",
				description: "je druhý rozsah buniek celých čísel a musia to byť čísla, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Vráti kovarianciu vzorky, priemer súčinov odchýlok pre každý pár údajových bodov v dvoch množinách údajov.",
		arguments: [
			{
				name: "pole1",
				description: "je prvý rozsah buniek celých čísel a musia to byť čísla, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "pole2",
				description: "je druhý rozsah buniek celých čísel a musia to byť čísla, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Vráti najmenšiu hodnotu, pre ktorú má distribučná funkcia binomického rozdelenia hodnotu väčšiu alebo rovnajúcu sa hodnote kritéria.",
		arguments: [
			{
				name: "pokusy",
				description: "je počet Bernoulliho pokusov"
			},
			{
				name: "pravdepodobnosť_s",
				description: "je pravdepodobnosť úspešného pokusu. Argument je číslo od 0 do 1 vrátane oboch hodnôt"
			},
			{
				name: "alfa",
				description: "je hodnota kritéria. Je to číslo od 0 do 1 vrátane oboch hodnôt"
			}
		]
	},
	{
		name: "CSC",
		description: "Vráti kosekans uhla.",
		arguments: [
			{
				name: "number",
				description: "je uhol v radiánoch, ktorého kosekans chcete zistiť"
			}
		]
	},
	{
		name: "CSCH",
		description: "Vráti hyperbolický kosekans uhla.",
		arguments: [
			{
				name: "number",
				description: "je uhol v radiánoch, ktorého hyperbolický kosekans chcete zistiť"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Vráti kumulatívny vyplatený úrok medzi dvomi obdobiami.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba"
			},
			{
				name: "pobd",
				description: "je celkový počet období splátok"
			},
			{
				name: "sh",
				description: "je súčasná hodnota"
			},
			{
				name: "počiatočné_obdobie",
				description: "je prvé obdobie výpočtu"
			},
			{
				name: "koncové_obdobie",
				description: "je posledné obdobie výpočtu"
			},
			{
				name: "typ",
				description: "je načasovanie splátky"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Vráti kumulovanú vyplatenú istinu z úveru medzi dvomi obdobiami.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková miera"
			},
			{
				name: "pobd",
				description: "je celkový počet období splátok"
			},
			{
				name: "sh",
				description: "je súčasná hodnota"
			},
			{
				name: "počiatočné_obdobie",
				description: "je prvé obdobie výpočtu"
			},
			{
				name: "koncové_obdobie",
				description: "je posledné obdobie výpočtu"
			},
			{
				name: "typ",
				description: "je načasovanie splátky"
			}
		]
	},
	{
		name: "DATE",
		description: "Vráti číslo, ktoré v kóde programu Spreadsheet pre dátum a čas predstavuje dátum.",
		arguments: [
			{
				name: "rok",
				description: "je číslo od 1900 do 9999 v programe Spreadsheet pre systém Windows alebo číslo od 1904 do 9999 v programe Spreadsheet pre počítače Macintosh"
			},
			{
				name: "mesiac",
				description: "je číslo od 1 do 12 označujúce mesiac v roku"
			},
			{
				name: "deň",
				description: "je číslo od 1 do 31 označujúce deň v mesiaci"
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
		description: "Konvertuje dátum v textovom formáte na číslo, ktoré v kóde programu Spreadsheet pre dátum a čas predstavuje dátum.",
		arguments: [
			{
				name: "text_dátumu",
				description: "je text predstavujúci dátum vo formáte programu Spreadsheet pre dátum, ktorý musí byť v rozsahu od 1.1.1900 (Windows) resp. 1.1.1904 (Macintosh) do 31.12.9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Vráti priemer hodnôt v stĺpci zoznamu alebo v databáze, ktoré spĺňajú zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DAY",
		description: "Vráti deň v mesiaci, číslo od 1 do 31.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas"
			}
		]
	},
	{
		name: "DAYS",
		description: "Vráti počet dní medzi dvomi dátumami.",
		arguments: [
			{
				name: "end_date",
				description: "start_date a end_date sú dva dátumy, medzi ktorými chcete spočítať počet dní"
			},
			{
				name: "start_date",
				description: "start_date a end_date sú dva dátumy, medzi ktorými chcete spočítať počet dní"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Vráti počet dní medzi dvoma dátumami na základe roka s 360 dňami (12 mesiacov po 30 dní).",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "Argumenty počiatočný_dátum a koncový_dátum sú dátumy ohraničujúce interval, ktorého dĺžku (v dňoch) chcete určiť"
			},
			{
				name: "koncový_dátum",
				description: "Argumenty počiatočný_dátum a koncový_dátum sú dátumy ohraničujúce interval, ktorého dĺžku (v dňoch) chcete určiť"
			},
			{
				name: "metóda",
				description: "je logická hodnota udávajúca metódu výpočtu: americká (NASD) = FALSE alebo nie je zadaná; európska = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Vypočíta odpis majetku za zadané obdobie podľa metódy klesajúceho zostatku s pevným koeficientom.",
		arguments: [
			{
				name: "cena",
				description: "je vstupná cena majetku"
			},
			{
				name: "zostatok",
				description: "je zostatková cena na konci životnosti majetku"
			},
			{
				name: "životnosť",
				description: "je počet období, v ktorých sa majetok odpisuje (tzv. životnosť majetku)"
			},
			{
				name: "obdobie",
				description: "je obdobie, za ktoré chcete vypočítať odpis. Argument Obdobie musí byť zadaný v rovnakých jednotkách ako argument Životnosť"
			},
			{
				name: "mesiac",
				description: "je počet mesiacov v prvom roku odpisu. Ak tento argument nezadáte, použije sa hodnota 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Spočíta bunky obsahujúce čísla v poli (stĺpci) záznamov databázy, ktoré spĺňajú zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Spočíta bunky, ktoré nie sú prázdne, v poli (stĺpci) záznamov databázy, ktoré spĺňajú zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DDB",
		description: "Vypočíta odpis majetku za zadané obdobie podľa metódy dvojnásobného odpisovania z klesajúceho zostatku alebo inej zadanej metódy.",
		arguments: [
			{
				name: "cena",
				description: "je vstupná cena majetku"
			},
			{
				name: "zostatok",
				description: "je zostatková cena na konci životnosti majetku"
			},
			{
				name: "životnosť",
				description: "je počet období, v ktorých sa majetok odpisuje (tzv. životnosť majetku)"
			},
			{
				name: "obdobie",
				description: "je obdobie, za ktoré chcete vypočítať odpis. Argument Obdobie musí byť zadaný v rovnakých jednotkách ako argument Životnosť"
			},
			{
				name: "faktor",
				description: "je miera poklesu zostatku. Ak tento argument nezadáte, použije sa hodnota 2 (metóda dvojnásobného odpisovania z klesajúceho zostatku)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Skonvertuje desiatkové číslo na binárne číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je desiatkové celé číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Skonvertuje desiatkové číslo na šestnástkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je desiatkové celé číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Skonvertuje desiatkové číslo na osmičkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je desiatkové celé číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Konvertuje textové vyjadrenie čísla v danom základe na desatinné číslo.",
		arguments: [
			{
				name: "number",
				description: "je číslo, ktoré chcete skonvertovať"
			},
			{
				name: "radix",
				description: "je základ sústavy čísla, ktoré konvertujete"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Konvertuje radiány na stupne.",
		arguments: [
			{
				name: "uhol",
				description: "je uhol v radiánoch, ktorý chcete konvertovať"
			}
		]
	},
	{
		name: "DELTA",
		description: "Testuje zhodu dvoch čísel.",
		arguments: [
			{
				name: "číslo1",
				description: "je prvé číslo"
			},
			{
				name: "číslo2",
				description: "je druhé číslo"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Vráti súčet druhých mocnín odchýlok údajových bodov od strednej hodnoty vzorky.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 argumentov alebo pole či odkaz na pole, pre ktoré chcete vypočítať výsledok funkcie DEVSQ"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 argumentov alebo pole či odkaz na pole, pre ktoré chcete vypočítať výsledok funkcie DEVSQ"
			}
		]
	},
	{
		name: "DGET",
		description: "Vyberie z databázy jeden záznam, ktorý spĺňa zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DISC",
		description: "Vráti diskontnú sadzbu cenného papiera.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "ss",
				description: "je cena cenného papiera za každých 100 USD nominálnej hodnoty"
			},
			{
				name: "vyplatenie",
				description: "je hodnota vyplatenia cenného papiera za každých 100 USD nominálnej hodnoty"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "DMAX",
		description: "Vráti maximálnu hodnotu v poli (stĺpci) záznamov databázy, ktorá spĺňa zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DMIN",
		description: "Vráti minimálnu hodnotu v poli (stĺpci) záznamov databázy, ktorá spĺňa zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Konvertuje číslo na text vo formáte meny.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, odkaz na bunku s číslom alebo vzorec, ktorého výsledkom je číslo"
			},
			{
				name: "desatinné_miesta",
				description: "je počet desatinných miest. Číslo je podľa potreby zaokrúhlené. Ak argument desatinné_miesta nezadáte, bude jeho hodnota 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Skonvertuje cenu dolára, vyjadrenú zlomkom na cenu dolára, vyjadrenú desatinným číslom.",
		arguments: [
			{
				name: "zlomkový_dolár",
				description: "je číslo vyjadrené ako zlomok"
			},
			{
				name: "zlomok",
				description: "je celé číslo, ktoré sa použije v menovateli zlomku"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Skonvertuje cenu dolára, vyjadrenú desatinným číslom na cenu dolára, vyjadrenú zlomkom.",
		arguments: [
			{
				name: "desatinný_dolár",
				description: "je desatinné číslo"
			},
			{
				name: "zlomok",
				description: "je celé číslo, ktoré sa použije v menovateli zlomku"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Vynásobí hodnoty v poli (stĺpci) záznamov databázy, ktoré spĺňajú zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Odhadne smerodajnú odchýlku podľa vzorky vybratých položiek databázy.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Vypočíta smerodajnú odchýlku podľa celého súboru vybratých položiek databázy.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DSUM",
		description: "Spočíta čísla v poli (stĺpci) záznamov databázy, ktoré spĺňajú zadané podmienky.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DVAR",
		description: "Odhadne rozptyl vzorky vybratých položiek databázy.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "DVARP",
		description: "Vypočíta rozptyl podľa celého súboru vybratých položiek databázy.",
		arguments: [
			{
				name: "databáza",
				description: "je rozsah buniek tvoriacich zoznam alebo databázu. Databáza je zoznam vzájomne prepojených údajov"
			},
			{
				name: "pole",
				description: "je menovka stĺpca v úvodzovkách alebo číslo označujúce poradie stĺpca v zozname"
			},
			{
				name: "kritériá",
				description: "je rozsah buniek spĺňajúci zadané podmienky. Rozsah zahŕňa menovku stĺpca a jednu bunku pod menovkou pre podmienku"
			}
		]
	},
	{
		name: "EDATE",
		description: "Vráti poradové číslo dátumu, ktoré predstavuje vyznačený počet mesiacov pred alebo po počiatočnom dátume.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "mesiace",
				description: "je počet mesiacov pred alebo po počiatočnom dátume"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Vráti platnú ročnú úrokovú mieru.",
		arguments: [
			{
				name: "nominálna_sadzba",
				description: "je nominálna úroková miera"
			},
			{
				name: "pzobd",
				description: "je počet zlučovaných období za rok"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Vráti reťazec zakódovaného URL.",
		arguments: [
			{
				name: "text",
				description: "je reťazec, ktorý sa má zakódovať URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Vráti poradové číslo posledného dňa mesiaca pred alebo po zadanom počte mesiacov.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "mesiace",
				description: "je počet mesiacov pred alebo po počiatočnom dátume"
			}
		]
	},
	{
		name: "ERF",
		description: "Vráti chybovú funkciu.",
		arguments: [
			{
				name: "dolný_limit",
				description: "je dolné pásmo integrácie ERF"
			},
			{
				name: "horný_limit",
				description: "je horné pásmo integrácie ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Vráti chybovú funkciu.",
		arguments: [
			{
				name: "X",
				description: "je dolné pásmo integrácie ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Vráti funkciu dodatkovej chyby.",
		arguments: [
			{
				name: "x",
				description: "je dolné pásmo integrácie ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Vráti doplnkovú chybovú funkciu.",
		arguments: [
			{
				name: "X",
				description: "je dolné pásmo integrácie ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Vráti číslo zodpovedajúce chybovej hodnote.",
		arguments: [
			{
				name: "hodnota_chyby",
				description: "je chybová hodnota, ktorej identifikačné číslo chcete zistiť. Môže to byť skutočná chybová hodnota alebo odkaz na bunku obsahujúcu chybovú hodnotu"
			}
		]
	},
	{
		name: "EVEN",
		description: "Zaokrúhli kladné číslo nahor a záporné číslo smerom nadol na najbližšie párne celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			}
		]
	},
	{
		name: "EXACT",
		description: "Skontroluje, či sú dva textové reťazce identické a vráti hodnotu TRUE alebo FALSE. Funkcia EXACT rozlišuje malé a veľké písmená.",
		arguments: [
			{
				name: "text1",
				description: "je prvý textový reťazec"
			},
			{
				name: "text2",
				description: "je druhý textový reťazec"
			}
		]
	},
	{
		name: "EXP",
		description: "Vráti e (základ prirodzeného logaritmu) umocnené na zadané číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je exponent použitý na základ e. Konštanta e je základ prirodzeného logaritmu a má hodnotu 2,71828182845904"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty exponenciálneho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je hodnota funkcie. Musí to byť nezáporné číslo"
			},
			{
				name: "lambda",
				description: "je hodnota parametra. Musí to byť kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota pre funkciu, ktorú chcete vrátiť: distribučná funkcia = TRUE; hustota rozdelenia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty exponenciálneho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je hodnota funkcie. Musí to byť nezáporné číslo"
			},
			{
				name: "lambda",
				description: "je hodnota parametra. Musí to byť kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota pre funkciu, ktorú chcete vrátiť: distribučná funkcia = TRUE; hustota rozdelenia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Vráti (zľava ohraničené) F rozdelenie pravdepodobnosti (miera rozličnosti) pre dve množiny údajov.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa má funkcia hodnotiť, a predstavuje nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je čitateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10, a menšie než 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je menovateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10 a menšie než 10^10"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota pre funkciu, ktorá sa má vrátiť: ak ide o funkciu kumulatívneho rozdelenia, hodnota je TRUE a ak ide o funkciu hustoty pravdepodobnosti, hodnota je FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Vráti (sprava ohraničené) F rozdelenie pravdepodobnosti (miera rozličnosti) pre dve množiny údajov.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa má funkcia hodnotiť, a predstavuje nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je čitateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10, a menšie než 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je menovateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10 a menšie než 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Vráti inverznú hodnotu (ľavostranného) rozdelenia pravdepodobnosti F: ak p = F.DIST(x,...), potom F.INV(p,...) = x.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť kumulatívneho rozdelenia F, číslo od 0 do 1 (vrátane)"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je počet stupňov voľnosti v čitateli, číslo od 1 do 10^10 okrem čísla 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je počet stupňov voľnosti v menovateli, číslo od 1 do 10^10 okrem čísla 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Vráti inverznú hodnotu (sprava ohraničeného) F rozdelenia pravdepodobnosti: ak p = F.DIST.RT(x,...), potom F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená ku kumulatívnemu F rozdeleniu, čo je číslo od 0 do 1 vrátane"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je čitateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10, a menšie než 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je menovateľ počtu stupňov voľnosti, čiže číslo od 1 do 10^10 a menšie než 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Vráti výsledok F-testu, dvojstrannú pravdepodobnosť, že rozptyly v argumentoch Pole1 a Pole2 nie sú výrazne odlišné.",
		arguments: [
			{
				name: "pole1",
				description: "je prvé pole alebo rozsah údajov. Hodnoty argumentu môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla (prázdne bunky sa neberú do úvahy)"
			},
			{
				name: "pole2",
				description: "je druhé pole alebo rozsah údajov. Hodnoty argumentu môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla (prázdne bunky sa neberú do úvahy)"
			}
		]
	},
	{
		name: "FACT",
		description: "Vráti faktoriál čísla. Výsledok sa rovná hodnote 1*2*3*...* Číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je nezáporné číslo, ktorého faktoriál chcete vypočítať"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Vráti dvojitý faktoriál čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorej dvojitý faktoriál chcete získať"
			}
		]
	},
	{
		name: "FALSE",
		description: "Vráti logickú hodnotu FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Vráti hodnotu (sprava ohraničeného) rozdelenia pravdepodobnosti F (stupeň odlišnosti) pre dve množiny údajov.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete vyhodnotiť funkciu. Musí to byť nezáporné číslo"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je počet stupňov voľnosti čitateľa, ktorým môže byť číslo medzi 1 a 10^10 okrem čísla 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je počet stupňov voľnosti menovateľa, ktorým je číslo medzi 1 a 10^10 okrem čísla 10^10"
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
		description: "Vráti počiatočnú pozíciu textového reťazca v rámci iného textového reťazca. Táto funkcia rozlišuje malé a veľké písmená.",
		arguments: [
			{
				name: "nájsť_text",
				description: "je text, ktorý hľadáte. Použite úvodzovky (bez textu) na hľadanie prvého zodpovedajúceho znaku argumentu V_texte. Zástupné znaky nie sú povolené"
			},
			{
				name: "v_texte",
				description: "je text obsahujúci hľadaný text"
			},
			{
				name: "počiatočné_číslo",
				description: "určuje znak, od ktorého začne hľadanie. Prvý znak argumentu V_texte je číslo znaku 1. Ak argument Počiatočné_číslo nezadáte, bude jeho hodnota 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Vráti hodnotu inverznej funkcie k (sprava ohraničenej) distribučnej funkcii rozdelenia pravdepodobnosti F: ak p = FDIST(x,...), potom FINV(p,...) = x.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je hodnota distribučnej funkcie rozdelenia F, ktorou je číslo medzi 0 a 1 vrátane oboch hodnôt"
			},
			{
				name: "stupeň_voľnosti1",
				description: "je počet stupňov voľnosti čitateľa, ktorým je byť číslo medzi 1 a 10^10 okrem čísla 10^10"
			},
			{
				name: "stupeň_voľnosti2",
				description: "je počet stupňov voľnosti menovateľa, ktorým je číslo medzi 1 a 10^10 okrem čísla 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Vráti hodnotu Fisherovej transformácie.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu transformácie, ktorou je číslo medzi -1 a 1 okrem čísel -1 a 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Vráti hodnotu inverznej funkcie k Fisherovej transformácii: ak y = FISHER(x), potom FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu inverznej transformácie"
			}
		]
	},
	{
		name: "FIXED",
		description: "Zaokrúhli číslo na daný počet desatinných miest a vráti výsledok vo forme textu s alebo bez čiarok.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktoré chcete zaokrúhliť a zobraziť vo forme textu"
			},
			{
				name: "desatinné_miesta",
				description: "je počet desatinných miest. Ak argument desatinné_miesta nezadáte, bude jeho hodnota 2"
			},
			{
				name: "bez_čiarok",
				description: "je logická hodnota: nezobrazovať čiarky vo vrátenom texte = TRUE; zobrazovať čiarky vo vrátenom texte = FALSE alebo nie je zadaná"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Zaokrúhli číslo nadol na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "číslo",
				description: "je číselná hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "významnosť",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť. Argumenty Číslo a Významnosť musia byť oba buď kladné, alebo záporné čísla"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Zaokrúhli číslo nadol na najbližšie celé číslo alebo na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "number",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "significance",
				description: "je násobok, na ktorý chcete hodnotu zaokrúhliť"
			},
			{
				name: "mode",
				description: "pri zadaní nenulovej hodnoty bude táto funkcia zaokrúhľovať smerom k nule"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Zaokrúhli číslo smerom nadol na najbližšie celé číslo alebo na najbližší násobok zadanej hodnoty.",
		arguments: [
			{
				name: "číslo",
				description: "je číselná hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "významnosť",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Vypočíta alebo odhadne budúcu hodnotu lineárneho trendu pomocou existujúcich hodnôt.",
		arguments: [
			{
				name: "x",
				description: "je údajový bod, pre ktorý chcete odhadnúť hodnotu. Musí to byť číselná hodnota."
			},
			{
				name: "známe_y",
				description: "je závislé pole alebo rozsah číselných údajov"
			},
			{
				name: "známe_x",
				description: "je nezávislé pole alebo rozsah číselných údajov. Rozptyl argumentu x sa nesmie rovnať nule"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Vráti vzorec ako reťazec.",
		arguments: [
			{
				name: "reference",
				description: "je odkaz na vzorec"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Vytvorí frekvenčnú tabuľku, čiže zvislé pole čísel s počtami výskytov hodnôt v jednotlivých rozsahoch.",
		arguments: [
			{
				name: "údajové_pole",
				description: "je pole alebo odkaz na množinu hodnôt, pre ktoré chcete zistiť počet výskytov (ignoruje prázdne bunky a text)"
			},
			{
				name: "binárne_pole",
				description: "je pole alebo odkaz na intervaly, do ktorých chcete zoskupiť hodnoty argumentu údajové_pole"
			}
		]
	},
	{
		name: "FTEST",
		description: "Vráti výsledok F-testu, dvojstrannú pravdepodobnosť, že rozptyly v argumentoch Pole1 a Pole2 nie sú výrazne odlišné.",
		arguments: [
			{
				name: "pole1",
				description: "je prvé pole alebo rozsah údajov. Hodnoty argumentu môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla (prázdne bunky sa neberú do úvahy)"
			},
			{
				name: "pole2",
				description: "je druhé pole alebo rozsah údajov. Hodnoty argumentu môžu byť čísla, názvy, polia alebo odkazy obsahujúce čísla (prázdne bunky sa neberú do úvahy)"
			}
		]
	},
	{
		name: "FV",
		description: "Vypočíta budúcu hodnotu investície pri pravidelných a konštantných platbách a konštantnej úrokovej sadzbe.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "pobd",
				description: "je celkový počet platobných období investície"
			},
			{
				name: "plt",
				description: "je platba uskutočnená v jednotlivých obdobiach, ktorá sa nemení po celú dobu životnosti investície"
			},
			{
				name: "sh",
				description: "je súčasná hodnota alebo celková čiastka určujúca súčasnú hodnotu budúcich platieb. Ak tento argument nezadáte, použije sa hodnota 0"
			},
			{
				name: "typ",
				description: "je hodnota predstavujúca termín platby: platba na začiatku obdobia = 1; platba na konci obdobia = 0 alebo nie je zadaná"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Vráti budúcu hodnotu úvodnej istiny po použití série zložených úrokových mier.",
		arguments: [
			{
				name: "istina",
				description: "je súčasná hodnota"
			},
			{
				name: "plán",
				description: "je pole úrokových mier, ktoré chcete použiť"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Vráti hodnotu funkcie Gamma.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete vypočítať funkciu Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Vráti rozdelenie gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej chcete hodnotiť rozdelenie, je to nezáporné číslo"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia a je to kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia a je to kladné číslo. Ak hodnota beta = 1, funkcia GAMMA.DIST vráti štandardné rozdelenie gama"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak vráti funkciu kumulatívneho rozdelenia, hodnota = TRUE, a ak vráti funkciu hromadnej pravdepodobnosti, hodnota = FALSE alebo sa vynechá"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Vráti inverznú hodnotu kumulatívneho rozdelenia gama: ak p = GAMMA.DIST(x,...), potom GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená k rozdeleniu gama a je to číslo v rozsahu od 0 do 1 vrátane"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia a je to kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia a je to kladné číslo. Ak hodnota beta = 1, funkcia GAMMA.INV vráti inverznú hodnotu štandardného rozdelenia gama"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty rozdelenia gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu rozdelenia. Musí to byť nezáporné číslo"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo. Ak beta = 1, funkcia GAMMADIST vráti hodnotu štandardného rozdelenia gama"
			},
			{
				name: "kumulatívne",
				description: "je logická funkcia: vráti distribučnú funkciu = TRUE; vráti hustotu rozdelenia pravdepodobnosti = FALSE, alebo nie je zadaná"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Vráti hodnotu inverznej funkcie k distribučnej funkcii gama rozdelenia: ak p = GAMMADIST(x,...), potom GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je hodnota pravdepodobnosti gama rozdelenia, ktorou je číslo medzi 0 a 1 vrátane obidvoch hodnôt"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo. Ak beta = 1, GAMMAINV vráti inverznú funkciu k štandardnému gama rozdeleniu"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Vráti prirodzený logaritmus funkcie gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete vypočítať hodnotu funkcie GAMMALN. Argument musí byť kladné číslo"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Vráti prirodzený logaritmus funkcie gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete vypočítať hodnotu funkcie GAMMALN.PRECISE. Argument musí byť kladné číslo"
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
		description: "Vráti najväčší spoločný deliteľ.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 hodnôt"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 hodnôt"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Vráti geometrický priemer poľa alebo rozsahu kladných číselných údajov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých geometrický priemer chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých geometrický priemer chcete zistiť"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Testuje, či je číslo väčšie ako prahová hodnota.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorá sa testuje v krokoch"
			},
			{
				name: "krok",
				description: "je prahová hodnota"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Extrahuje údaje uložené v kontingenčnej tabuľke.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "údajové_pole",
				description: "je názov údajového poľa, z ktorého sa majú extrahovať údaje"
			},
			{
				name: "kontingenčná_tabuľka",
				description: "je odkaz na bunku alebo rozsah buniek v kontingenčnej tabuľke obsahujúcej požadované údaje"
			},
			{
				name: "pole",
				description: "pole, na ktoré sa odkazuje"
			},
			{
				name: "položka",
				description: "položka poľa, na ktorú sa odkazuje"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Vráti hodnoty trendu exponenciálneho rastu, ktorý zodpovedá známym údajovým bodom.",
		arguments: [
			{
				name: "známe_y",
				description: "je množina známych hodnôt v rovnici y = b*m^x. Môže to byť pole alebo rozsah kladných čísel"
			},
			{
				name: "známe_x",
				description: "je voliteľná množina známych hodnôt x v rovnici y = b*m^x. Môže to byť pole alebo rozsah rovnakej veľkosti ako argument známe_y"
			},
			{
				name: "nové_x",
				description: "sú nové hodnoty x, pre ktoré má funkcia GROWTH vrátiť zodpovedajúce hodnoty y"
			},
			{
				name: "konštanta",
				description: "je logická hodnota: konštanta b sa vypočíta, ak argument b = TRUE; konštanta b sa rovná 1, ak argument b = FALSE alebo nie je zadaný"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Vráti harmonický priemer množiny kladných číselných údajov: prevrátená hodnota aritmetického priemeru prevrátených hodnôt.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých harmonický priemer chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých harmonický priemer chcete zistiť"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Skonvertuje šestnástkové číslo na binárne číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je šestnástkové číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Skonvertuje šestnástkové číslo na desiatkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je šestnástkové číslo, ktoré chcete skonvertovať"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Skonvertuje šestnástkové číslo na osmičkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je šestnástkové číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Prehľadá horný riadok tabuľky alebo pole hodnôt a vráti hodnotu zo zadaného riadka obsiahnutú v rovnakom stĺpci.",
		arguments: [
			{
				name: "vyhľadávaná_hodnota",
				description: "je hodnota, ktorú chcete vyhľadať v prvom riadku tabuľky. Môže to byť hodnota, odkaz alebo textový reťazec"
			},
			{
				name: "pole_tabuľky",
				description: "je prehľadávaná tabuľka obsahujúca text, čísla alebo logické hodnoty. Argument Pole_tabuľky môže byť odkaz na rozsah alebo názov rozsahu"
			},
			{
				name: "číslo_indexu_riadka",
				description: "je číslo riadka v argumente Pole_tabuľky, z ktorého sa vráti zodpovedajúca hodnota. Prvý riadok hodnôt tabuľky je riadok číslo 1"
			},
			{
				name: "vyhľadávanie_rozsah",
				description: "je logická hodnota: nájsť najbližšiu zodpovedajúcu hodnotu v hornom riadku (hodnoty sú zoradené vzostupne) = TRUE alebo nie je zadaná; nájsť presne zodpovedajúcu hodnotu = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Vráti hodinu, číslo od 0 (12:00 dop.) do 23 (11:00 odp.).",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas alebo text vo formáte času, napríklad 16:48:00 alebo 4:48:00 odp."
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Vytvorí odkaz alebo skok, ktorý otvorí dokument uložený na pevnom disku, sieťovom serveri alebo na Internete.",
		arguments: [
			{
				name: "umiestnenie_prepojenia",
				description: "je text určujúci cestu a názov súboru s dokumentom, ktorý chcete otvoriť, umiestnenie na pevnom disku, adresa UNC alebo cesta URL"
			},
			{
				name: "priateľský_názov",
				description: "je text alebo číslo, ktoré sa zobrazí v bunke. Ak ho nezadáte, v bunke sa zobrazí text prepojenia"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Vráti hypergeometrické rozdelenie.",
		arguments: [
			{
				name: "vzorka_s",
				description: "je počet úspechov v rámci vzorky"
			},
			{
				name: "veľkosť_vzorky",
				description: "je veľkosť vzorky"
			},
			{
				name: "populácia_s",
				description: "je počet úspechov v rámci súboru"
			},
			{
				name: "počet_pop",
				description: "je veľkosť súboru"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak ide o funkciu kumulatívneho rozdelenia, použije sa hodnota TRUE a ak ide o funkciu hustoty rozdelenia, použije sa hodnota FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Vráti hodnotu hypergeometrického rozdelenia.",
		arguments: [
			{
				name: "vzorka_s",
				description: "je počet úspešných pokusov vo výbere"
			},
			{
				name: "počet_vzorka",
				description: "je veľkosť výberu "
			},
			{
				name: "populácia_s",
				description: "je počet úspešných pokusov v základnom súbore"
			},
			{
				name: "počet_pop",
				description: "je veľkosť základného súboru"
			}
		]
	},
	{
		name: "IF",
		description: "Skontroluje, či je podmienka splnená a vráti jednu hodnotu, ak je výsledkom TRUE, a inú hodnotu, ak je výsledkom FALSE.",
		arguments: [
			{
				name: "logický_test",
				description: "je ľubovoľná hodnota alebo výraz, ktorému môže byť priradená logická hodnota TRUE alebo FALSE"
			},
			{
				name: "hodnota_ak_pravda",
				description: "je hodnota, ktorá bude vrátená, ak je hodnota argumentu logický_test TRUE. Ak argument vynecháte, vráti sa hodnota TRUE. Môžete navrstviť až sedem funkcií IF"
			},
			{
				name: "hodnota_ak_nepravda",
				description: "je hodnota, ktorá bude vrátená, ak je hodnota argumentu logický_test FALSE. Ak argument vynecháte, vráti sa hodnota FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Vráti hodnotu argumentu hodnota_ak_chyba, ak je zadaný výraz chybou; v opačnom prípade vráti výraz.",
		arguments: [
			{
				name: "hodnota",
				description: "je akákoľvek hodnota, výraz alebo odkaz"
			},
			{
				name: "hodnota_ak_chyba",
				description: "je akákoľvek hodnota, výraz alebo odkaz"
			}
		]
	},
	{
		name: "IFNA",
		description: "Vráti zadanú hodnotu, ak je výsledkom výrazu hodnota #N/A, v opačnom prípade vráti výsledok výrazu.",
		arguments: [
			{
				name: "value",
				description: "je ľubovoľná hodnota alebo výraz alebo odkaz"
			},
			{
				name: "value_if_na",
				description: "je ľubovoľná hodnota alebo výraz alebo odkaz"
			}
		]
	},
	{
		name: "IMABS",
		description: "Vráti absolútnu hodnotu (modul) komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého absolútnu hodnotu chcete získať"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Vráti imaginárny koeficient komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého imaginárny koeficient chcete získať"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Vráti argument q, čiže uhol vyjadrený v radiánoch.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého argument chcete získať"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Vráti komplexnú konjugáciu komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého konjugáciu chcete získať"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Vráti kosínus komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého kosínus chcete získať"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Vráti hyperbolický kosínus komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého hyperbolický kosínus chcete zistiť"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Vráti kotangens komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého kotangens chcete zistiť"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Vráti kosekans komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého kosekans chcete zistiť"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Vráti hyperbolický kosekans komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého hyperbolický kosekans chcete zistiť"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Vráti kvocient dvoch komplexných čísel.",
		arguments: [
			{
				name: "ičíslo1",
				description: "je komplexný čitateľ alebo delenec"
			},
			{
				name: "ičíslo2",
				description: "je komplexný menovateľ alebo deliteľ"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Vráti exponenciálu komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého exponenciálu chcete získať"
			}
		]
	},
	{
		name: "IMLN",
		description: "Vráti prirodzený logaritmus komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého prirodzený logaritmus chcete získať"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Vráti dekadický logaritmus komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého dekadický logaritmus chcete získať"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Vráti binárny logaritmus komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého binárny logaritmus chcete získať"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Vráti komplexné číslo umocnené na celočíselnú mocninu.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktoré chcete umocniť"
			},
			{
				name: "číslo",
				description: "je mocnina, na ktorú chcete umocniť komplexné číslo"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Vráti súčin 1 až 255 komplexných čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ičíslo1",
				description: "Ičíslo1, Ičíslo2,... je 1 až 255 komplexných čísel, ktoré možno vynásobiť."
			},
			{
				name: "ičíslo2",
				description: "Ičíslo1, Ičíslo2,... je 1 až 255 komplexných čísel, ktoré možno vynásobiť."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Vráti reálny koeficient komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého reálny koeficient chcete získať"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Vráti sekans komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého sekans chcete zistiť"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Vráti hyperbolický sekans komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého hyperbolický sekans chcete zistiť"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Vráti sínus komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého sínus chcete získať"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Vráti hyperbolický sínus komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého hyperbolický sínus chcete zistiť"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Vráti druhú odmocninu komplexného čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexné číslo, ktorého druhú odmocninu chcete získať"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Vráti rozdiel dvoch komplexných čísel.",
		arguments: [
			{
				name: "ičíslo1",
				description: "je komplexné číslo, od ktorého chcete odrátať komplexné číslo2"
			},
			{
				name: "ičíslo2",
				description: "je komplexné číslo, ktoré chcete odrátať od komplexného čísla1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Vráti súčet komplexných čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ičíslo1",
				description: "je 1 až 255 komplexných čísel, ktoré možno pridať"
			},
			{
				name: "ičíslo2",
				description: "je 1 až 255 komplexných čísel, ktoré možno pridať"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Vráti tangens komplexného čísla.",
		arguments: [
			{
				name: "inumber",
				description: "je komplexné číslo, ktorého tangens chcete zistiť"
			}
		]
	},
	{
		name: "INDEX",
		description: "Vráti hodnotu alebo odkaz na bunku v určitom riadku a stĺpci v danom rozsahu.",
		arguments: [
			{
				name: "pole",
				description: "je rozsah buniek alebo konštanta poľa."
			},
			{
				name: "číslo_riadka",
				description: "vyberie riadok v argumente Pole alebo Odkaz, z ktorého sa vráti hodnota. Ak tento argument vynecháte, je nutné zadať argument Číslo_stĺpca"
			},
			{
				name: "číslo_stĺpca",
				description: "vyberie stĺpec v argumente Pole alebo Odkaz, z ktorého sa vráti hodnota. Ak tento argument vynecháte, je nutné zadať argument Číslo_riadka"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Vráti odkaz určený textovým reťazcom.",
		arguments: [
			{
				name: "text_odkazu",
				description: "je odkaz na bunku obsahujúcu odkaz na štýl A1 alebo R1C1, názov definovaný ako odkaz alebo odkaz na bunku ako textový reťazec"
			},
			{
				name: "a1",
				description: "je logická hodnota určujúca typ odkazu v argumente Text_odkazu: štýl R1C1 = FALSE; štýl A1 = TRUE alebo nie je zadaná"
			}
		]
	},
	{
		name: "INFO",
		description: "Vráti informácie o aktuálnom pracovnom prostredí.",
		arguments: [
			{
				name: "text_typu",
				description: "je text určujúci typ požadovaných informácií."
			}
		]
	},
	{
		name: "INT",
		description: "Zaokrúhli číslo nadol na najbližšie celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je reálne číslo, ktoré chcete zaokrúhliť nadol na celé číslo"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Vypočíta súradnice bodu, v ktorom čiara pretína os y. Výpočet sa robí preložením najlepšej regresnej čiary známymi hodnotami x a y.",
		arguments: [
			{
				name: "známe_y",
				description: "je závislý súbor pozorovaní alebo údajov. Hodnoty argumentu môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "známe_x",
				description: "je nezávislý súbor pozorovaní alebo údajov. Hodnoty argumentu môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Vráti úrokovú mieru za úplne investovaný cenný papier.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splátka",
				description: "je suma investovaná do cenného papiera"
			},
			{
				name: "vyplatenie",
				description: "je suma, ktorá sa má prijať k dátumu splatnosti"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "IPMT",
		description: "Vypočíta výšku platby úroku v určitom úrokovom období pri pravidelných konštantných splátkach a konštantnej úrokovej sadzbe.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "obd",
				description: "je obdobie, pre ktoré chcete vypočítať úrok. Jeho hodnota musí byť v intervale od 1 do hodnoty argumentu Pobd"
			},
			{
				name: "pobd",
				description: "je celkový počet platobných období investície"
			},
			{
				name: "sh",
				description: "je súčasná hodnota alebo celková čiastka určujúca súčasnú hodnotu budúcich platieb"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej platby. Ak tento argument nezadáte, použije sa hodnota 0"
			},
			{
				name: "typ",
				description: "je logická hodnota predstavujúca termín platby: platba na konci obdobia = 0 alebo nie je zadaná, platba na začiatku obdobia = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Vypočíta vnútornú mieru návratnosti pre sériu hotovostných tokov.",
		arguments: [
			{
				name: "hodnoty",
				description: "je pole alebo odkaz na bunky obsahujúce čísla, ktorých vnútornú mieru návratnosti chcete zistiť"
			},
			{
				name: "odhad",
				description: "je číslo, ktoré je vaším odhadom výsledku funkcie IRR. Ak tento argument nezadáte, použije sa hodnota 0,1 (10 percent)"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Skontroluje, či je argument odkazom na prázdnu bunku a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je testovaná bunka alebo názov označujúci bunku, ktorú chcete testovať"
			}
		]
	},
	{
		name: "ISERR",
		description: "Skontroluje, či je argument chybovou hodnotou (#HODNOTA!, #ODKAZ!, #DELENIE NULOU!, #ČÍSLO!, #NÁZOV? alebo #NEPLATNÝ!) okrem hodnoty #NEDOSTUPNÝ a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Skontroluje, či ide o chybovú hodnotu (#NEDOSTUPNÝ, #HODNOTA!, #ODKAZ!, #DELENIE NULOU!, #ČÍSLO!, #NÁZOV? alebo #NEPLATNÝ!), a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota môže odkazovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Vráti hodnotu TRUE, ak je číslo párne.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete testovať"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Overí, či odkaz smeruje na bunku obsahujúcu vzorec a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "reference",
				description: "je odkaz na bunku, ktorú chcete testovať. Odkazom môže byť odkaz na bunku, vzorec alebo názov, ktorý odkazuje na bunku"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Skontroluje, či je argument logickou hodnotou (TRUE alebo FALSE) a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISNA",
		description: "Skontroluje, či hodnota je #NEDOSTUPNÝ a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Skontroluje, či argument nie je textovou hodnotou (prázdne bunky nie sú text) a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať: bunka; vzorec; alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Skontroluje, či je argument číselnou hodnotou a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Zaokrúhľuje číslo nahor na najbližšie celé číslo alebo na najbližší násobok významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "významnosť",
				description: "je voliteľný násobok, na ktorý chcete zaokrúhľovať"
			}
		]
	},
	{
		name: "ISODD",
		description: "Vráti hodnotu TRUE, ak je číslo nepárne.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete testovať"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Vráti číslo týždňa v roku pre daný dátum podľa normy ISO.",
		arguments: [
			{
				name: "date",
				description: "je kód dátumu a času, ktorý sa v Spreadsheeti používa vo výpočtoch dátumu a času"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Vypočíta úrok zaplatený v zadanom období investície.",
		arguments: [
			{
				name: "sadzba",
				description: "úroková sadzba za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "obd",
				description: "je obdobie, pre ktoré chcete zistiť výšku úroku"
			},
			{
				name: "pobd",
				description: "počet platobných období investície"
			},
			{
				name: "sh",
				description: "celková čiastka určujúca súčasnú hodnotu série budúcich platieb"
			}
		]
	},
	{
		name: "ISREF",
		description: "Skontroluje, či je argument odkazom a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Skontroluje, či je argument textovou hodnotou a vráti hodnotu TRUE alebo FALSE.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete testovať. Hodnota sa môže vzťahovať na bunku, vzorec alebo názov odkazujúci na bunku, vzorec alebo hodnotu"
			}
		]
	},
	{
		name: "KURT",
		description: "Vráti špicatosť množiny údajov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete vypočítať špicatosť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete vypočítať špicatosť"
			}
		]
	},
	{
		name: "LARGE",
		description: "Vráti k-tu najväčšiu hodnotu v množine údajov, napríklad piate najväčšie číslo.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, pre ktoré chcete určiť k-tu najväčšiu hodnotu"
			},
			{
				name: "k",
				description: "je pozícia hľadanej hodnoty (počítaná od najväčšej) v poli alebo rozsahu buniek"
			}
		]
	},
	{
		name: "LCM",
		description: "Vráti najmenší spoločný násobok.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 hodnôt, za ktoré chcete zistiť najmenší spoločný násobok"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 hodnôt, za ktoré chcete zistiť najmenší spoločný násobok"
			}
		]
	},
	{
		name: "LEFT",
		description: "Vráti zadaný počet znakov od začiatku textového reťazca.",
		arguments: [
			{
				name: "text",
				description: "je textový reťazec obsahujúci znaky, ktoré chcete extrahovať"
			},
			{
				name: "počet_znakov",
				description: "určuje, koľko znakov má funkcia LEFT extrahovať. Ak tento argument nezadáte, bude jeho hodnota 1"
			}
		]
	},
	{
		name: "LEN",
		description: "Vráti počet znakov textového reťazca.",
		arguments: [
			{
				name: "text",
				description: "je text, ktorého dĺžku chcete zistiť. Medzery sú považované za znaky"
			}
		]
	},
	{
		name: "LINEST",
		description: "Vráti štatistickú hodnotu popisujúcu lineárny trend zodpovedajúci známym údajovým bodom aproximáciou priamky metódou najmenších štvorcov.",
		arguments: [
			{
				name: "známe_y",
				description: "je množina známych hodnôt y v rovnici y = mx + b"
			},
			{
				name: "známe_x",
				description: "je voliteľná množina známych hodnôt x v rovnici y = mx + b"
			},
			{
				name: "konštanta",
				description: "je logická hodnota: konštanta b sa vypočíta štandardne, ak argument b = TRUE alebo nie je zadaný; b sa rovná nule, ak argument b = FALSE"
			},
			{
				name: "štatistika",
				description: "je logická hodnota: vráti dodatočnú regresnú štatistiku = TRUE; vráti koeficienty m a konštantu b = FALSE alebo nie je zadaná"
			}
		]
	},
	{
		name: "LN",
		description: "Vráti prirodzený logaritmus daného čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálne číslo, ktorého prirodzený logaritmus chcete zistiť"
			}
		]
	},
	{
		name: "LOG",
		description: "Vráti logaritmus čísla pri určenom základe.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálne číslo, ktorého logaritmus chcete zistiť"
			},
			{
				name: "základ",
				description: "je základ logaritmu. Ak tento argument nezadáte, bude jeho hodnota 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Vráti dekadický logaritmus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálne číslo, ktorého dekadický logaritmus chcete zistiť"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Vráti štatistiku popisujúcu exponenciálnu krivku, ktorá zodpovedá známym údajovým bodom.",
		arguments: [
			{
				name: "známe_y",
				description: "je množina hodnôt y známych vo vzťahu y = b*m^x"
			},
			{
				name: "známe_x",
				description: "je voliteľná množina hodnôt x známych vo vzťahu y = b*m^x"
			},
			{
				name: "konštanta",
				description: "je logická hodnota: konštanta b sa vypočíta, ak argument b = TRUE alebo nie je zadaný; konštanta b sa rovná 1, ak argument b = FALSE"
			},
			{
				name: "štatistika",
				description: "je logická hodnota: vráti dodatočnú regresnú štatistiku = TRUE; vráti koeficienty m a konštantu b = FALSE alebo nie je zadaná"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Vráti inverznú funkciu k distribučnej funkcii lognormálneho rozdelenia hodnôt x, kde funkcia ln(x) má normálne rozdelenie s parametrami Stred a Smerodajná_odch.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť lognormálneho rozdelenia, ktorá je číslo medzi 0 a 1 vrátane obidvoch hodnôt"
			},
			{
				name: "stred",
				description: "je stredná hodnota funkcie ln(x)"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka funkcie ln(x), ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Vráti rozdelenie prirodzeného logaritmu x, kde ln(x) je normálne rozdelený s parametrami Stred a Smerodajná_odch.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pri ktorej sa má hodnotiť funkcia, a je to kladné číslo"
			},
			{
				name: "stred",
				description: "je stredná hodnota ln(x)"
			},
			{
				name: "smerodajná_odch",
				description: "je štandardná odchýlka ln(x) a je to kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak ide o funkciu kumulatívneho rozdelenia, použite hodnotu TRUE a ak ide o funkciu hustoty pravdepodobnosti, použite hodnotu FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Vráti inverznú funkciu k distribučnej funkcii lognormálneho rozdelenia hodnôt x, kde funkcia ln(x) má normálne rozdelenie s parametrami Stred a Smerodajná_odch.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť lognormálneho rozdelenia, ktorá je číslo medzi 0 a 1 vrátane obidvoch hodnôt"
			},
			{
				name: "stred",
				description: "je stredná hodnota funkcie ln(x)"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka funkcie ln(x), ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Vráti hodnotu distribučnej funkcie lognormálneho rozdelenia hodnôt x, kde funkcia ln(x) má normálne rozdelenie s parametrami Stred a Smerodajná_odch.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu rozdelenia. Argument musí byť kladné číslo."
			},
			{
				name: "stred",
				description: "je stredná hodnota funkcie ln(x)"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka funkcie ln(x), ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Vyhľadá hodnotu z poľa alebo z rozsahu jedného riadka alebo jedného stĺpca. Funkcia je poskytovaná za účelom spätnej kompatibility.",
		arguments: [
			{
				name: "vyhľadávaná_hodnota",
				description: "je hodnota vyhľadávaná v argumente vektor_vyhľadávania. Môže to byť číslo, text, logická hodnota, názov alebo odkaz na hodnotu"
			},
			{
				name: "vektor_vyhľadávania",
				description: "je rozsah pozostávajúci z jedného riadka alebo stĺpca, v ktorom je vzostupne usporiadaný text, čísla alebo logické hodnoty"
			},
			{
				name: "vektor_výsledkov",
				description: "je rozsah pozostávajúci z jedného riadka alebo stĺpca, ktorý má rovnakú veľkosť ako veľkosť argumentu vektor_vyhľadávania"
			}
		]
	},
	{
		name: "LOWER",
		description: "Konvertuje všetky písmená v textovom reťazci na malé.",
		arguments: [
			{
				name: "text",
				description: "je text, ktorý chcete konvertovať na malé písmená. Znaky v argumente Text, ktoré nie sú písmená, sa nemenia"
			}
		]
	},
	{
		name: "MATCH",
		description: "Vráti relatívnu pozíciu položky poľa, ktorá zodpovedá danej hodnote v danom poradí.",
		arguments: [
			{
				name: "vyhľadávaná_hodnota",
				description: "je hodnota, ktorá sa použije na vyhľadanie požadovanej hodnoty v poli. Môže to byť číslo, text, logická hodnota, názov alebo odkaz na hodnotu"
			},
			{
				name: "pole_vyhľadávania",
				description: "je súvislý rozsah buniek obsahujúci hľadané hodnoty, pole hodnôt alebo odkaz na pole"
			},
			{
				name: "typ_zhody",
				description: "je číslo 1, 0 alebo -1, ktoré určuje, aká hodnota bude vrátená."
			}
		]
	},
	{
		name: "MAX",
		description: "Vráti najvyššiu hodnotu z množiny hodnôt. Ignoruje logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých maximálnu hodnotu chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých maximálnu hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "MAXA",
		description: "Vráti najväčšiu hodnotu v množine hodnôt. Neignoruje logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých maximálnu hodnotu chcete zistiť"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých maximálnu hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Vráti determinant matice poľa.",
		arguments: [
			{
				name: "pole",
				description: "je číselné pole s rovnakým počtom riadkov a stĺpcov predstavujúce rozsah bunky alebo konštantu poľa"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Vráti medián, čiže hodnotu v strede množiny daných čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla , ktorých medián chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla , ktorých medián chcete zistiť"
			}
		]
	},
	{
		name: "MID",
		description: "Vráti znaky z textového reťazca, ak je zadaná počiatočná pozícia a dĺžka.",
		arguments: [
			{
				name: "text",
				description: "je textový reťazec, z ktorého chcete znaky extrahovať"
			},
			{
				name: "počiatočná_pozícia",
				description: "je pozícia prvého znaku, ktorý chcete extrahovať. Prvý znak v texte je 1"
			},
			{
				name: "počet_znakov",
				description: "určí počet znakov textu, ktoré sa majú vrátiť"
			}
		]
	},
	{
		name: "MIN",
		description: "Vráti najnižšie číslo z množiny hodnôt. Ignoruje logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých minimálnu hodnotu chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých minimálnu hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "MINA",
		description: "Vráti najmenšiu hodnotu v množine hodnôt. Neignoruje logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých minimálnu hodnotu chcete zistiť"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 čísel, prázdnych buniek, logických hodnôt alebo čísel v textovom formáte, ktorých minimálnu hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Vráti minútu, číslo od 0 do 59.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas alebo text vo formáte času, napríklad 16:48:00 alebo 4:48:00 odp."
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Vráti inverznú maticu matice uloženej v poli.",
		arguments: [
			{
				name: "pole",
				description: "je číselné pole s rovnakým počtom riadkov a stĺpcov predstavujúce rozsah bunky alebo konštantu poľa"
			}
		]
	},
	{
		name: "MIRR",
		description: "Vypočíta vnútornú mieru návratnosti pre sériu pravidelných hotovostných tokov. Zohľadňuje pritom náklady na investície, ako aj úrok z opätovnej investície získaných prostriedkov.",
		arguments: [
			{
				name: "hodnoty",
				description: "je pole alebo odkaz na bunky obsahujúce čísla, ktoré predstavujú splátky (záporná hodnota) a príjmy (kladná hodnota) v pravidelných obdobiach"
			},
			{
				name: "finančná_sadzba",
				description: "je úroková sadzba platená za používané peniaze"
			},
			{
				name: "reinvestičná_sadzba",
				description: "je úroková sadzba získaná z reinvestovaných peňazí"
			}
		]
	},
	{
		name: "MMULT",
		description: "Vráti maticu so súčinom dvoch polí, ktorá obsahuje rovnaký počet riadkov ako pole1 a rovnaký počet stĺpcov ako pole2.",
		arguments: [
			{
				name: "pole1",
				description: "je prvé pole čísel, ktoré chcete násobiť. Počet stĺpcov musí byť rovnaký ako počet riadkov Poľa2"
			},
			{
				name: "pole2",
				description: "je prvé pole čísel, ktoré chcete násobiť. Počet stĺpcov musí byť rovnaký ako počet riadkov Poľa2"
			}
		]
	},
	{
		name: "MOD",
		description: "Vráti zvyšok po delení čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, pre ktoré chcete nájsť zvyšok po delení"
			},
			{
				name: "deliteľ",
				description: "je číslo, ktorým chcete deliť argument Číslo"
			}
		]
	},
	{
		name: "MODE",
		description: "Vráti modus, čiže hodnotu, ktorá sa v poli alebo rozsahu údajov vyskytuje alebo opakuje najčastejšie.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Vráti zvislé pole najčastejšie sa vyskytujúcich (alebo opakujúcich sa) hodnôt v poli alebo rozsahu údajov. V prípade vodorovného poľa použite funkciu =TRANSPOSE(MODE.MULT(číslo1,číslo2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Vráti modus, čiže hodnotu, ktorá sa v poli alebo rozsahu údajov vyskytuje alebo opakuje najčastejšie.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, ktorých modus chcete zistiť"
			}
		]
	},
	{
		name: "MONTH",
		description: "Vráti mesiac, číslo od 1 (január) do 12 (december).",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas"
			}
		]
	},
	{
		name: "MROUND",
		description: "Vráti hodnotu zaokrúhlenú na požadovaný násobok.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			},
			{
				name: "násobok",
				description: "je násobok, na ktorý chcete číslo zaokrúhliť"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Vráti polynomickú množinu čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 hodnôt, ktorých polynomickú hodnotu chcete zistiť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 hodnôt, ktorých polynomickú hodnotu chcete zistiť"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Vráti maticu jednotky pre zadanú dimenziu.",
		arguments: [
			{
				name: "dimension",
				description: "je celé číslo určujúce dimenziu matice jednotky, ktorú má funkcia vrátiť"
			}
		]
	},
	{
		name: "N",
		description: "Konvertuje nečíselnú hodnotu na číslo, dátumy na poradové čísla, hodnotu TRUE na číslo 1, všetky ostatné výrazy na číslo 0 (nula).",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete konvertovať"
			}
		]
	},
	{
		name: "NA",
		description: "Vráti chybovú hodnotu #NEDOSTUPNÝ (hodnota nie je dostupná).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Záporné binomické rozdelenie, pravdepodobnosť počet_f zlyhaní pred počet_s-tym úspechom pri pravdepodobnosti Pravdepodobnosť_s.",
		arguments: [
			{
				name: "počet_f",
				description: "=počet zlyhaní"
			},
			{
				name: "počet_s",
				description: "=prahový počet úspechov"
			},
			{
				name: "pravdepodobnosť_s",
				description: "=pravdepodobnosť úspechu (0 až 1)"
			},
			{
				name: "kumulatívne",
				description: "=logická hodnota: kumulatívne rozdelenie = TRUE, hromadná pravdepodobnosť = FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: " Vráti hodnotu záporného binomického rozdelenia (pravdepodobnosť Počet_f neúspešných pokusov po Počet_s úspešných pri pravdepodobnosti Pravdepodobnosť_s.",
		arguments: [
			{
				name: "počet_f",
				description: "=neúspešné pokusy"
			},
			{
				name: "počet_s",
				description: "=úspešné pokusy"
			},
			{
				name: "pravdepodobnosť_s",
				description: "=pravdepodobnosť úspechu (od 0 do 1)"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Vráti počet celých pracovných dní medzi dvomi dátumami.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "koncový_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje koncový dátum"
			},
			{
				name: "sviatky",
				description: "je voliteľná množina poradových čísel dátumu, ktoré možno vyňať z pracovného kalendára, napríklad štátne sviatky a pohyblivé dni voľna"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Vráti počet celých pracovných dní medzi dvoma dátumami s vlastnými parametrami víkendov.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "koncový_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje koncový dátum"
			},
			{
				name: "víkend",
				description: "je číslo alebo reťazec označujúci, kedy sa vyskytnú víkendy"
			},
			{
				name: "sviatky",
				description: "je voliteľná množina poradových čísel dátumov, ktoré sa majú vylúčiť z pracovného kalendára, ako sú napríklad štátne sviatky a pohyblivé sviatky"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Vráti ročnú nominálnu úrokovú mieru.",
		arguments: [
			{
				name: "účinná_sadzba",
				description: "je platná úroková miera"
			},
			{
				name: "pzobd",
				description: "je počet zlučovaných období za rok"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Vráti normálne rozdelenie pre zadanú strednú hodnotu a štandardnú odchýlku.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, ktorej rozdelenie chcete zistiť"
			},
			{
				name: "stred",
				description: "je aritmetická stredná hodnota rozdelenia"
			},
			{
				name: "smerodajná_odch",
				description: "je štandardná odchýlka rozdelenia a je to kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak ide o funkciu kumulatívnej distribúcie, použije sa hodnota TRUE a v prípade funkcie hustoty pravdepodobnosti sa použije hodnota FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Vráti inverznú funkciu k distribučnej funkcii normálneho rozdelenia pre zadanú strednú hodnotu a smerodajnú odchýlku.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť normálneho rozdelenia, ktorej hodnota musí byť číslo medzi 0 a 1 vrátane oboch krajných hodnôt"
			},
			{
				name: "stred",
				description: "je aritmetická stredná hodnota rozdelenia"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka rozdelenia, ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Vráti štandardné normálne rozdelenie (so strednou hodnotou nula a štandardnou odchýlkou jeden).",
		arguments: [
			{
				name: "z",
				description: "je hodnota, pre ktorú chcete rozdelenie"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota pre funkciu, ktorá sa má vrátiť: funkcia kumulatívneho rozdelenia = TRUE; funkcia hustoty pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Vráti inverznú funkciu k distribučnej funkcii štandardného normálneho rozdelenia. Toto rozdelenie má strednú hodnotu 0 a smerodajnú odchýlku 1.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť normálneho rozdelenia, ktorej hodnota musí byť číslo medzi 0 a 1 vrátane oboch krajných hodnôt"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty normálneho rozdelenia pre zadanú strednú hodnotu a smerodajnú odchýlku.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu rozdelenia"
			},
			{
				name: "stred",
				description: "je aritmetická stredná hodnota rozdelenia"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka rozdelenia, ktorá musí byť kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia = TRUE; hustota rozdelenia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Vráti inverznú funkciu k distribučnej funkcii normálneho rozdelenia pre zadanú strednú hodnotu a smerodajnú odchýlku.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť normálneho rozdelenia, ktorej hodnota musí byť číslo medzi 0 a 1 vrátane oboch krajných hodnôt"
			},
			{
				name: "stred",
				description: "je aritmetická stredná hodnota rozdelenia"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka rozdelenia, ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Vráti hodnotu distribučnej funkcie štandardného normálneho rozdelenia. Toto rozdelenie má strednú hodnotu 0 a smerodajnú odchýlku 1.",
		arguments: [
			{
				name: "z",
				description: "je hodnota, ktorej rozdelenie chcete zistiť"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Vráti inverznú funkciu k distribučnej funkcii štandardného normálneho rozdelenia. Toto rozdelenie má strednú hodnotu 0 a smerodajnú odchýlku 1.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť normálneho rozdelenia, ktorej hodnota musí byť číslo medzi 0 a 1 vrátane oboch krajných hodnôt"
			}
		]
	},
	{
		name: "NOT",
		description: "Zmení hodnotu FALSE na TRUE alebo hodnotu TRUE na FALSE.",
		arguments: [
			{
				name: "logická_hodnota",
				description: "je hodnota alebo výraz, ktorý môže byť TRUE alebo FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Vráti aktuálny dátum a čas vo formáte dátumu a času.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Vypočíta počet období pre investíciu pri pravidelných konštantných platbách a konštantnej úrokovej sadzbe.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "plt",
				description: "je platba uskutočnená v jednotlivých obdobiach, ktorá sa nemení po celú dobu životnosti investície"
			},
			{
				name: "sh",
				description: "je súčasná hodnota alebo celková čiastka určujúca súčasnú hodnotu série budúcich platieb"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej platby. Ak tento argument nezadáte, použije sa hodnota 0"
			},
			{
				name: "typ",
				description: "je logická hodnota: platba na začiatku obdobia = 1; platba na konci obdobia = 0 alebo nie je zadaná"
			}
		]
	},
	{
		name: "NPV",
		description: "Vráti čistú súčasnú hodnotu investície vypočítanú na základe diskontnej sadzby, série budúcich splátok (záporná hodnota) a príjmov (kladná hodnota).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sadzba",
				description: "je diskontná sadzba za jedno obdobie"
			},
			{
				name: "hodnota1",
				description: "je 1 až 254 platieb a príjmov rovnomerne rozložených v čase a vyskytujúcich sa na konci každého obdobia"
			},
			{
				name: "hodnota2",
				description: "je 1 až 254 platieb a príjmov rovnomerne rozložených v čase a vyskytujúcich sa na konci každého obdobia"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Konvertuje text na číslo spôsobom, ktorý nie je závislý od miestnych nastavení.",
		arguments: [
			{
				name: "text",
				description: "je reťazec predstavujúci číslo, ktoré chcete skonvertovať"
			},
			{
				name: "decimal_separator",
				description: "je znak, ktorý sa v reťazci používa ako oddeľovač desatinných miest"
			},
			{
				name: "group_separator",
				description: "je znak, ktorý sa v reťazci používa ako oddeľovač skupín"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Skonvertuje osmičkové číslo na binárne číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Skonvertuje osmičkové číslo na binárne číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, ktoré chcete skonvertovať"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Skonvertuje osmičkové číslo na šestnástkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, ktoré chcete skonvertovať"
			},
			{
				name: "miesta",
				description: "je počet znakov, ktoré sa majú použiť"
			}
		]
	},
	{
		name: "ODD",
		description: "Zaokrúhli kladné číslo nahor a záporné číslo nadol na najbližšie nepárne celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, ktorú chcete zaokrúhliť"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Vráti odkaz na rozsah, ktorý predstavuje určený počet riadkov a stĺpcov z daného odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na bunku alebo súvislý rozsah buniek, od ktorej chcete zmeniť odsadenie"
			},
			{
				name: "riadky",
				description: "je počet riadkov nahor alebo nadol, na ktoré má ľavá horná bunka výsledku odkazovať"
			},
			{
				name: "stĺpce",
				description: "je počet stĺpcov vpravo alebo vľavo, na ktoré má ľavá horná bunka výsledku odkazovať"
			},
			{
				name: "výška",
				description: "je požadovaná výška výsledku vyjadrená počtom riadkov. Ak tento argument nezadáte, bude jeho hodnota rovnaká ako hodnota argumentu Odkaz"
			},
			{
				name: "šírka",
				description: "je požadovaná šírka výsledku vyjadrená počtom stĺpcov. Ak tento argument nezadáte, bude jeho hodnota rovnaká ako hodnota argumentu Odkaz"
			}
		]
	},
	{
		name: "OR",
		description: "Skontroluje, či existujú argumenty s hodnotou TRUE a vráti hodnotu TRUE alebo FALSE. Vráti hodnotu FALSE len v prípade, že všetky argumenty majú hodnotu FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logická_hodnota1",
				description: "je 1 až 255 testovaných podmienok, ktoré môžu mať hodnotu TRUE alebo FALSE"
			},
			{
				name: "logická_hodnota2",
				description: "je 1 až 255 testovaných podmienok, ktoré môžu mať hodnotu TRUE alebo FALSE"
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
		description: "Vráti počet období, ktoré sú potrebné na dosiahnutie zadanej hodnoty investície.",
		arguments: [
			{
				name: "rate",
				description: "je úroková sadzba za dané obdobie"
			},
			{
				name: "pv",
				description: "je súčasná hodnota investície"
			},
			{
				name: "fv",
				description: "je požadovaná budúca hodnota investície"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Vráti Pearsonov koeficient korelácie r.",
		arguments: [
			{
				name: "pole1",
				description: "je množina nezávislých hodnôt"
			},
			{
				name: "pole2",
				description: "je množina závislých hodnôt"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Vráti k-ty percentil hodnôt v rozsahu.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, ktoré definuje vzájomnú polohu"
			},
			{
				name: "k",
				description: "je hodnota percentilu, ktorá leží medzi 0 a 1 vrátane"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Vráti k-ty percentil hodnôt v rozsahu, kde k je z rozsahu čísel väčších ako 0 a menších ako 1.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, ktoré definujú relatívne postavenie"
			},
			{
				name: "k",
				description: "je hodnota percentilu z rozsahu od 0 do 1 vrátane"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Vráti k-ty percentil hodnôt v rozsahu, kde k je z rozsahu čísel od 0 do 1 vrátane.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, ktoré definujú relatívne postavenie"
			},
			{
				name: "k",
				description: "je hodnota percentilu z rozsahu od 0 do 1 vrátane"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Vráti poradie hodnoty v množine údajov vyjadrené percentuálnou časťou množiny údajov.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov s číselnými hodnotami, ktoré definuje vzájomnú polohu"
			},
			{
				name: "x",
				description: "je hodnota, ktorej poradie chcete zistiť"
			},
			{
				name: "významnosť",
				description: "je voliteľná hodnota, ktorá určuje počet platných číslic výslednej percentuálnej hodnoty. Ak sa nezadá, použijú sa tri číslice (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Vráti pozíciu hodnoty v množine údajov ako percentuálnu hodnotu (hodnotu väčšiu ako 0 a menšiu ako 1) množiny údajov.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov s číselnými hodnotami, ktoré definujú relatívne postavenie"
			},
			{
				name: "x",
				description: "je hodnota, ktorej pozíciu chcete zistiť"
			},
			{
				name: "významnosť",
				description: "je voliteľná hodnota, ktorá určuje počet platných číslic pre vrátenú percentuálnu hodnotu, ak sa vynechá, použijú sa tri číslice (0,xxx %)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Vráti pozíciu hodnoty v množine údajov ako percentuálnu hodnotu množiny údajov od 0 do 1 vrátane.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov s číselnými hodnotami, ktoré definujú relatívne postavenie"
			},
			{
				name: "x",
				description: "je hodnota, ktorej pozíciu chcete zistiť"
			},
			{
				name: "významnosť",
				description: "je voliteľná hodnota, ktorá určuje počet platných číslic pre vrátenú percentuálnu hodnotu, ak sa vynechá, použijú sa tri číslice (0,xxx %)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Vracia počet permutácií pre daný počet objektov, ktoré možno vybrať z celkového počtu objektov.",
		arguments: [
			{
				name: "počet",
				description: "je celkový počet objektov"
			},
			{
				name: "vybratý_počet",
				description: "je počet objektov v každej permutácii"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Vráti počet permutácií pre daný počet objektov (s opakovaniami), ktoré môžu byť vybraté z celkového počtu objektov.",
		arguments: [
			{
				name: "number",
				description: "je celkový počet objektov"
			},
			{
				name: "number_chosen",
				description: "je počet objektov v každej permutácii"
			}
		]
	},
	{
		name: "PHI",
		description: "Vráti hodnotu funkcie hustoty pre štandardné normálne rozdelenie.",
		arguments: [
			{
				name: "x",
				description: "je číslo, pre ktoré chcete vypočítať hustotu štandardného normálneho rozdelenia"
			}
		]
	},
	{
		name: "PI",
		description: "Vráti hodnotu pí s presnosťou na 15 číslic. Výsledkom je hodnota 3,14159265358979.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Vypočíta splátku pôžičky pri konštantných platbách a konštantnej úrokovej sadzbe.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba pre pôžičku za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "pobd",
				description: "je celkový počet splátok pôžičky"
			},
			{
				name: "sh",
				description: "je súčasná hodnota: súčasná celková hodnota série budúcich platieb"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej splátky. Ak tento argument nezadáte, použije sa hodnota 0"
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na začiatku obdobia = 1; splátka na konci obdobia = 0 alebo nie je zadaná"
			}
		]
	},
	{
		name: "POISSON",
		description: "Vráti hodnoty Poissonovho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je počet udalostí"
			},
			{
				name: "stred",
				description: "je očakávaná číselná hodnota, ktorou je kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia Poissonovho rozdelenia = TRUE, rozdelenie pravdepodobnosti Poissonovho rozdelenia = FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Vráti hodnoty Poissonovho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je počet udalostí"
			},
			{
				name: "stred",
				description: "je očakávaná číselná hodnota, ktorou je kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia Poissonovho rozdelenia = TRUE, rozdelenie pravdepodobnosti Poissonovho rozdelenia = FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Umocní číslo na zadanú mocninu.",
		arguments: [
			{
				name: "číslo",
				description: "je základ mocniny, ľubovoľné reálne číslo"
			},
			{
				name: "mocnina",
				description: "je exponent, na ktorý chcete základ umocniť"
			}
		]
	},
	{
		name: "PPMT",
		description: "Vypočíta hodnotu splátky istiny pre zadanú investíciu pri pravidelných konštantných platbách a konštantnej úrokovej sadzbe.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba na obdobie. Ak chcete napríklad zadať štvrťročné splátky pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "obd",
				description: "určuje obdobie a hodnota musí byť v rozsahu od 1 do hodnoty argumentu Pobd"
			},
			{
				name: "pobd",
				description: "je celkový počet platobných období investície"
			},
			{
				name: "sh",
				description: "je súčasná hodnota: celková čiastka určujúca súčasnú hodnotu série budúcich platieb"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej splátky"
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na začiatku obdobia = 1; splátka na konci obdobia = 0 alebo nie je zadaná"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Vráti cenu za každých 100 USD diskontovaného cenného papiera.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "zľava",
				description: "je diskontná sadzba cenného papiera"
			},
			{
				name: "vyplatenie",
				description: "je hodnota vyplatenia cenného papiera za každých 100 USD nominálnej hodnoty"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "PROB",
		description: "Vráti pravdepodobnosť, že hodnoty v rozsahu sú medzi dvoma hranicami, alebo sa rovnajú dolnej hranici.",
		arguments: [
			{
				name: "x_rozsah",
				description: "je rozsah číselných hodnôt x, ku ktorým existujú zodpovedajúce pravdepodobnosti"
			},
			{
				name: "rozsah_pravdepodobnosti",
				description: "je množina pravdepodobností zodpovedajúcich hodnotám v argumente X_rozsah, hodnotám medzi 0 a 1 vynímajúc 0"
			},
			{
				name: "dolný_limit",
				description: "je dolná hranica hodnôt, pre ktoré chcete zistiť pravdepodobnosť"
			},
			{
				name: "horný_limit",
				description: "je voliteľná horná hranica hodnôt. Ak sa vynechá, funkcia PROB vráti pravdepodobnosť, že sa hodnoty argumentu X_rozsah rovnajú argumentu Dolný_limit"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Vynásobí všetky čísla zadané ako argumenty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 čísel, logických hodnôt alebo čísel v textovom formáte, ktoré chcete vynásobiť"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 čísel, logických hodnôt alebo čísel v textovom formáte, ktoré chcete vynásobiť"
			}
		]
	},
	{
		name: "PROPER",
		description: "Písmená v textovom reťazci konvertuje do riadneho formátu, prvé písmeno každého slova na veľké a všetky ostatné písmená na malé.",
		arguments: [
			{
				name: "text",
				description: "je text v úvodzovkách, vzorec, ktorého výsledkom je text, alebo odkaz na bunku obsahujúcu text, v ktorom chcete zmeniť prvé písmená slov na veľké"
			}
		]
	},
	{
		name: "PV",
		description: "Vypočíta súčasnú hodnotu investície: súčasnú celkovú hodnotu série budúcich platieb.",
		arguments: [
			{
				name: "sadzba",
				description: "je úroková sadzba za obdobie. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4"
			},
			{
				name: "pobd",
				description: "je celkový počet platobných období investície"
			},
			{
				name: "plt",
				description: "je platba uskutočnená v jednotlivých obdobiach, ktorá sa nemení po celú dobu životnosti investície"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej platby"
			},
			{
				name: "typ",
				description: "je logická hodnota: platba na začiatku obdobia = 1; platba na konci obdobia = 0 alebo nie je zadaná"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Vráti kvartil množiny údajov.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah buniek s číselnými hodnotami, pre ktoré chcete vypočítať hodnotu kvartilu"
			},
			{
				name: "kvart",
				description: "je číslo: najmenšia hodnota = 0, prvý kvartil = 1, medián = 2, tretí kvartil = 3, najväčšia hodnota = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Vráti kvartil množiny údajov na základe hodnôt percentilov, ktoré sú väčšie ako 0 a menšie ako 1.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah buniek s číselnými hodnotami, pre ktoré chcete zistiť hodnotu kvartilu"
			},
			{
				name: "kvart",
				description: "je číslo: minimálna hodnota = 0; 1. kvartil = 1; hodnota mediánu = 2; 3. kvartil = 3; maximálna hodnota = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Vráti kvartil množiny údajov na základe hodnôt percentilov od 0 do 1 vrátane.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah buniek s číselnými hodnotami, pre ktoré chcete zistiť hodnotu kvartilu"
			},
			{
				name: "kvart",
				description: "je číslo: minimálna hodnota = 0; 1. kvartil = 1; hodnota mediánu = 2; 3. kvartil = 3; maximálna hodnota = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Vráti celočíselnú časť delenia.",
		arguments: [
			{
				name: "čitateľ",
				description: "je delenec"
			},
			{
				name: "menovateľ",
				description: "je deliteľ"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Konvertuje stupne na radiány.",
		arguments: [
			{
				name: "uhol",
				description: "je uhol v stupňoch, ktorý chcete konvertovať"
			}
		]
	},
	{
		name: "RAND",
		description: "Vráti náhodné číslo, ktoré je väčšie alebo rovné 0 a menšie než 1 (zmení sa pri každom prepočítaní).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Vráti náhodne vybraté číslo medzi zadanými číslami.",
		arguments: [
			{
				name: "najnižšie",
				description: "je najmenšie celé číslo, ktoré vráti funkcia RANDBETWEEN"
			},
			{
				name: "najvyššie",
				description: "je najväčšie celé číslo, ktoré vráti funkcia RANDBETWEEN"
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
		description: "Vráti relatívnu veľkosť čísla v zozname čísel vzhľadom na hodnoty v zozname.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktorého relatívnu veľkosť chcete zistiť"
			},
			{
				name: "odkaz",
				description: "je pole alebo odkaz na zoznam čísel. Ignoruje nečíselné hodnoty"
			},
			{
				name: "poradie",
				description: "je číslo: poradie v zozname usporiadanom zostupne = 0 alebo nie je zadané; poradie v zozname usporiadanom vzostupne = každá hodnota okrem hodnoty 0"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Vráti pozíciu čísla v zozname čísel: jeho veľkosť vo vzťahu k ostatným hodnotám v zozname; ak má viacero hodnôt rovnakú pozíciu, vráti sa priemerná pozícia.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktorého pozíciu chcete zistiť"
			},
			{
				name: "odkaz",
				description: "je pole zoznamu čísel alebo odkaz na zoznam čísel. Hodnoty, ktoré nie sú číselné, sa ignorujú"
			},
			{
				name: "poradie",
				description: "je číslo: pozícia v zostupne zoradenom zozname = 0 alebo sa vynechá; pozícia vo vzostupne zoradenom zozname = ľubovoľná nenulová hodnota"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Vráti pozíciu čísla v zozname čísel: jeho veľkosť vo vzťahu k ostatným hodnotám v zozname; ak má viacero hodnôt rovnakú pozíciu, vráti sa najvyššia pozícia tejto množiny hodnôt.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktorého pozíciu chcete zistiť"
			},
			{
				name: "odkaz",
				description: "je pole zoznamu čísel alebo odkaz na zoznam čísel. Hodnoty, ktoré nie sú číselné, sa ignorujú"
			},
			{
				name: "poradie",
				description: "je číslo: pozícia v zostupne zoradenom zozname = 0 alebo sa vynechá; pozícia vo vzostupne zoradenom zozname = ľubovoľná nenulová hodnota"
			}
		]
	},
	{
		name: "RATE",
		description: "Vypočíta úrokovú sadzbu za obdobie pôžičky alebo investície. Ak chcete napríklad zadať štvrťročné platby pri ročnej percentuálnej sadzbe 6 %, použite zápis 6%/4.",
		arguments: [
			{
				name: "pobd",
				description: "je celkový počet platobných období pôžičky alebo investície"
			},
			{
				name: "plt",
				description: "je platba uskutočnená v jednotlivých obdobiach, ktorá sa nemení po celú dobu životnosti pôžičky alebo investície"
			},
			{
				name: "sh",
				description: "je súčasná hodnota: súčasná celková hodnota série budúcich platieb"
			},
			{
				name: "bh",
				description: "je budúca hodnota alebo hotovostný zostatok, ktorý chcete dosiahnuť po zaplatení poslednej splátky. Ak tento argument nezadáte, použije sa hodnota 0"
			},
			{
				name: "typ",
				description: "je logická hodnota: platba na začiatku obdobia = 1; platba na konci obdobia = 0 alebo nie je zadaná"
			},
			{
				name: "odhad",
				description: "je váš odhad sadzby; Ak tento argument nezadáte, jeho hodnota bude 0,1 (10 percent)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Vráti sumu prijatú k dátumu splatnosti za úplne investovaný cenný papier.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splátka",
				description: "je suma investovaná do cenného papiera"
			},
			{
				name: "zľava",
				description: "je diskontná sadzba cenného papiera"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Nahradí časť textového reťazce iným textovým reťazcom.",
		arguments: [
			{
				name: "starý_text",
				description: "je text, v ktorom chcete nahradiť niektoré znaky"
			},
			{
				name: "počiatočné_číslo",
				description: "je pozícia znaku v texte označenom argumentom Starý_text, ktorý chcete zameniť za text označený argumentom Nový_text"
			},
			{
				name: "počet_znakov",
				description: "je počet znakov textu argumentu Starý_text, ktoré chcete nahradiť"
			},
			{
				name: "nový_text",
				description: "je text, ktorý nahradí znaky v texte označenom argumentom Starý_text"
			}
		]
	},
	{
		name: "REPT",
		description: "Text sa zopakuje podľa zadaného počtu opakovaní. Ak chcete bunku vyplniť určitým počtom opakovaní textového reťazca, použite funkciu REPT.",
		arguments: [
			{
				name: "text",
				description: "je text, ktorý chcete opakovať"
			},
			{
				name: "počet_opakovaní",
				description: "je kladné číslo, ktoré určuje počet opakovaní textu"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Vráti zadaný počet znakov od konca textového reťazca.",
		arguments: [
			{
				name: "text",
				description: "je textový reťazec obsahujúci znaky, ktoré chcete extrahovať"
			},
			{
				name: "počet_znakov",
				description: "určuje počet znakov, ktoré chcete extrahovať. Ak tento argument nezadáte, bude jeho hodnota 1"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Konvertuje číslo napísané arabskými číslicami na rímske číslice v textovom formáte.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo napísané arabskými číslicami, ktoré chcete konvertovať"
			},
			{
				name: "forma",
				description: "je číslo určujúce typ požadovaných rímskych číslic."
			}
		]
	},
	{
		name: "ROUND",
		description: "Zaokrúhli číslo na daný počet číslic.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktoré chcete zaokrúhliť"
			},
			{
				name: "počet_číslic",
				description: "je počet číslic, na ktoré chcete dané číslo zaokrúhliť. Ak zadáte záporné číslo, bude dané číslo zaokrúhlené doľava od desatinnej čiarky. Ak je hodnota argumentu 0, bude dané číslo zaokrúhlené na najbližšie celé číslo"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Zaokrúhli číslo smerom nadol k nule.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné reálne číslo, ktoré chcete zaokrúhliť nadol"
			},
			{
				name: "počet_číslic",
				description: "je počet číslic, na ktoré chcete číslo zaokrúhliť. Ak má tento argument zápornú hodnotu, bude dané číslo zaokrúhlené naľavo od desatinnej čiarky. A sa jeho hodnota rovná nule alebo nie je zadaná, bude dané číslo zaokrúhlené na najbližšie celé číslo"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Zaokrúhli číslo nahor, smerom od nuly.",
		arguments: [
			{
				name: "číslo",
				description: "je každé reálne číslo, ktoré chcete zaokrúhliť nahor"
			},
			{
				name: "počet_číslic",
				description: "je počet číslic, na ktoré chcete číslo zaokrúhliť. Ak má tento argument zápornú hodnotu, bude dané číslo zaokrúhlené naľavo od desatinnej čiarky. Ak sa jeho hodnota rovná nule alebo nie je zadaná, bude dané číslo zaokrúhlené na najbližšie celé číslo"
			}
		]
	},
	{
		name: "ROW",
		description: "Vráti číslo riadka odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je bunka alebo jeden rozsah buniek, pre ktorý chcete zistiť číslo riadka. Ak tento argument nezadáte, vráti bunku obsahujúcu funkciu ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "Vráti počet riadkov v odkaze alebo poli.",
		arguments: [
			{
				name: "pole",
				description: "je pole, vzorec poľa alebo odkaz na rozsah buniek, pre ktorý chcete zistiť počet riadkov"
			}
		]
	},
	{
		name: "RRI",
		description: "Vráti ekvivalentnú úrokovú sadzbu pre rast investície.",
		arguments: [
			{
				name: "nper",
				description: "je počet období pre investíciu"
			},
			{
				name: "pv",
				description: "je súčasná hodnota investície"
			},
			{
				name: "fv",
				description: "je budúca hodnota investície"
			}
		]
	},
	{
		name: "RSQ",
		description: "Vráti druhú mocninu Pearsonovho koeficientu korelácie daných údajových bodov.",
		arguments: [
			{
				name: "známe_y",
				description: "je pole alebo rozsah údajových bodov, ktorými môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "známe_x",
				description: "je pole alebo rozsah údajových bodov, ktorými môžu byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "RTD",
		description: "Načíta údaje v reálnom čase z programu podporujúceho automatizáciu COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "id_programu",
				description: "je názov identifikátora ProgID registrovaného doplnku automatizácie COM. Názov uzatvorte do úvodzoviek"
			},
			{
				name: "server",
				description: "je názov servera, na ktorom by mal byť doplnok spustený. Názov vložte do úvodzoviek. Ak je doplnok spustený lokálne, použite prázdny reťazec"
			},
			{
				name: "téma1",
				description: "sú parametre 1 až 38, ktoré určujú súčasti údajov"
			},
			{
				name: "téma2",
				description: "sú parametre 1 až 38, ktoré určujú súčasti údajov"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Vráti číslo prvého výskytu daného znaku alebo textového reťazca. Program hľadá zľava doprava a nerozlišuje veľké a malé písmená.",
		arguments: [
			{
				name: "nájsť_text",
				description: "je text, ktorý chcete nájsť. Môžete použiť zástupné znaky ? a *. Nájdete ich pomocou reťazcov ~? a ~*"
			},
			{
				name: "v_texte",
				description: "je text, v ktorom chcete vyhľadať znak alebo textový reťazec"
			},
			{
				name: "pozícia_začiatku",
				description: "Je číslo znaku (zľava) v argumente V_texte, pri ktorom chcete začať hľadať. Ak tento argument nezadáte, bude jeho hodnota 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Vráti sekans uhla.",
		arguments: [
			{
				name: "number",
				description: "je uhol v radiánoch, ktorého sekans chcete zistiť"
			}
		]
	},
	{
		name: "SECH",
		description: "Vráti hyperbolický sekans uhla.",
		arguments: [
			{
				name: "number",
				description: "je uhol v radiánoch, ktorého hyperbolický sekans chcete zistiť"
			}
		]
	},
	{
		name: "SECOND",
		description: "Vráti sekundu, číslo od 0 do 59.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas alebo text vo formáte času, napríklad 16:48:23 alebo 4:48:47 odp."
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Vráti súčet mocninových radov na základe vzorca.",
		arguments: [
			{
				name: "x",
				description: "je vstupná hodnota mocninového radu"
			},
			{
				name: "n",
				description: "je úvodná mocnina, ktorou chcete umocniť x"
			},
			{
				name: "m",
				description: "je krok, o ktorý chcete zvýšiť hodnotu parametra n každého člena radu"
			},
			{
				name: "koeficienty",
				description: "je množina koeficientov, ktorou sa vynásobí každá nasledujúca mocnina x"
			}
		]
	},
	{
		name: "SHEET",
		description: "Vráti číslo odkazovaného hárka.",
		arguments: [
			{
				name: "value",
				description: "je názov hárka alebo odkazu, pre ktorý chcete zistiť číslo hárku. Ak ho vynecháte, vráti sa číslo hárka obsahujúceho funkciu"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Vráti počet hárkov v odkaze.",
		arguments: [
			{
				name: "reference",
				description: "je odkaz, o ktorom chcete zistiť, koľko hárkov obsahuje. Ak ho vynecháte, vráti sa počet hárkov v zošite, ktoré obsahujú funkciu"
			}
		]
	},
	{
		name: "SIGN",
		description: "Vráti znamienko čísla: 1 pri kladnom čísle, 0 pri nule alebo -1 pri zápornom čísle.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné reálne číslo"
			}
		]
	},
	{
		name: "SIN",
		description: "Vráti sínus uhla.",
		arguments: [
			{
				name: "číslo",
				description: "je uhol v radiánoch, ktorého sínus chcete zistiť. Stupne * PI()/180 = radiány"
			}
		]
	},
	{
		name: "SINH",
		description: "Vráti hyperbolický sínus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné reálne číslo"
			}
		]
	},
	{
		name: "SKEW",
		description: "Vráti šikmosť rozdelenia: charakteristika stupňa asymetrie rozdelenia okolo strednej hodnoty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete vypočítať šikmosť"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete vypočítať šikmosť"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Vráti šikmosť rozdelenia na základe populácie: charakteristika stupňa asymetrie rozdelenia okolo strednej hodnoty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "sú 1 až 254 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete zistiť šikmosť populácie"
			},
			{
				name: "number2",
				description: "sú 1 až 254 čísel alebo názvov, polí alebo odkazov obsahujúcich čísla, pre ktoré chcete zistiť šikmosť populácie"
			}
		]
	},
	{
		name: "SLN",
		description: "Vypočíta odpis majetku za jedno obdobie pri rovnomernom odpisovaní.",
		arguments: [
			{
				name: "cena",
				description: "je vstupná cena majetku"
			},
			{
				name: "zostatok",
				description: "je zostatková cena na konci životnosti majetku"
			},
			{
				name: "životnosť",
				description: "je počet období, v ktorých sa majetok odpisuje (tzv. životnosť majetku)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Vráti gradient priamky lineárnej regresie daných údajových bodov.",
		arguments: [
			{
				name: "známe_y",
				description: "je pole alebo rozsah buniek číselných závislých údajových bodov a môžu to byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "známe_x",
				description: "je množina nezávislých údajových bodov a môžu to byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "SMALL",
		description: "Vráti k-tu najmenšiu hodnotu v množine údajov, napríklad piate najmenšie číslo.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah číselných údajov, pre ktoré chcete určiť k-tu najmenšiu hodnotu"
			},
			{
				name: "k",
				description: "je pozícia hľadanej hodnoty (počítaná od najmenšej) v poli alebo rozsahu"
			}
		]
	},
	{
		name: "SQRT",
		description: "Vráti druhú odmocninu čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktorého druhú odmocninu chcete zistiť"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Vráti odmocninu (číslo * pí).",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktorým sa násobí parameter p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Vracia normalizovanú hodnotu z rozdelenia určeného strednou hodnotou a smerodajnou odchýlkou .",
		arguments: [
			{
				name: "x",
				description: "je hodnota, ktorú chcete normalizovať"
			},
			{
				name: "stred",
				description: "je aritmetická stredná hodnota rozdelenia"
			},
			{
				name: "smerodajná_odch",
				description: "je smerodajná odchýlka rozdelenia, ktorá musí byť kladné číslo"
			}
		]
	},
	{
		name: "STDEV",
		description: "Odhadne smerodajnú odchýlku na základe vzorky (ignoruje logické hodnoty a text vo vzorke).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel korešpondujúcich so vzorkou výberu. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel korešpondujúcich so vzorkou výberu. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Vypočíta smerodajnú odchýlku celého súboru, ktorý bol zadaný ako argument (ignoruje logické hodnoty a text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo odkazov obsahujúcich čísla, ktoré zodpovedajú základnému súboru. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo odkazov obsahujúcich čísla, ktoré zodpovedajú základnému súboru. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Odhadne smerodajnú odchýlku na základe vzorky (ignoruje logické hodnoty a text vo vzorke).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel korešpondujúcich so vzorkou výberu. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel korešpondujúcich so vzorkou výberu. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Odhadne smerodajnú odchýlku podľa výberového súboru vrátane logických hodnôt a textu. Text a logická hodnota FALSE = 0; logická hodnota TRUE = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnôt zodpovedajúcich výberovému súboru. Môžu to byť hodnoty, názvy alebo odkazy na hodnoty"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnôt zodpovedajúcich výberovému súboru. Môžu to byť hodnoty, názvy alebo odkazy na hodnoty"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Vypočíta smerodajnú odchýlku celého súboru, ktorý bol zadaný ako argument (ignoruje logické hodnoty a text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel alebo odkazov obsahujúcich čísla, ktoré zodpovedajú základnému súboru. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel alebo odkazov obsahujúcich čísla, ktoré zodpovedajú základnému súboru. Môžu to byť čísla alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Vypočíta smerodajnú odchýlku základného súboru vrátane logických hodnôt a textu. Text a logická hodnota FALSE = 0; logická hodnota TRUE = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnôt zodpovedajúcich základnému súboru. Môžu to byť hodnoty, názvy, polia alebo odkazy obsahujúce hodnoty"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnôt zodpovedajúcich základnému súboru. Môžu to byť hodnoty, názvy, polia alebo odkazy obsahujúce hodnoty"
			}
		]
	},
	{
		name: "STEYX",
		description: "Vráti štandardnú chybu predpokladanej hodnoty y pre každé x regresie.",
		arguments: [
			{
				name: "známe_y",
				description: "je pole alebo rozsah závislých údajových bodov a môžu to byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			},
			{
				name: "známe_x",
				description: "je pole alebo rozsah nezávislých údajových bodov a môžu to byť čísla alebo názvy, polia alebo odkazy obsahujúce čísla"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Nahradí existujúci text v textovom reťazci novým textom.",
		arguments: [
			{
				name: "text",
				description: "je text alebo odkaz na bunku obsahujúcu text, v ktorom chcete zameniť znaky"
			},
			{
				name: "starý_text",
				description: "je existujúci text, ktorý chcete nahradiť. Ak malé a veľké písmená v texte argumentu Starý_text sa nezhodujú s malými a veľkými písmenami v texte argumentu Text, funkcia SUBSTITUTE text nenahradí"
			},
			{
				name: "nový_text",
				description: "je text, ktorý má nahradiť text argumentu Starý_text"
			},
			{
				name: "číslo_inštancie",
				description: "určuje, ktorý výskyt textu argumentu Starý_text chcete nahradiť. Ak tento argument nezadáte, bude každý výskyt textu argumentu Starý_text nahradený"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Vráti medzisúčet v zozname alebo v databáze.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo_funkcie",
				description: "je číslo 1 až 11, ktoré určuje typ súhrnnej funkcie použitej pre výpočet medzisúčtu."
			},
			{
				name: "odk1",
				description: "je 1 až 254 rozsahov alebo odkazov, ktorých medzisúčet chcete zistiť"
			}
		]
	},
	{
		name: "SUM",
		description: "Spočíta všetky čísla v rozsahu buniek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, ktoré chcete spočítať. Logické hodnoty a text sa v bunkách ignorujú, ak budú zadané ako argumenty, tak sa zahrnú"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, ktoré chcete spočítať. Logické hodnoty a text sa v bunkách ignorujú, ak budú zadané ako argumenty, tak sa zahrnú"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Spočíta bunky vybraté podľa zadanej podmienky alebo kritéria.",
		arguments: [
			{
				name: "rozsah",
				description: "je rozsah buniek, ktoré chcete vyhodnotiť"
			},
			{
				name: "kritériá",
				description: "je podmienka alebo kritérium vo forme čísla, výrazu alebo textu, ktorý určuje bunky na sčítanie"
			},
			{
				name: "rozsah_súhrnu",
				description: "sú skutočné bunky, ktoré sa sčítajú. Ak tento argument nezadáte, použije sa daný rozsah buniek"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Pripočíta bunky určené podľa zadanej množiny podmienok alebo kritérií.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rozsah_súčtu",
				description: "sú skutočné bunky, ktoré sa majú spočítať."
			},
			{
				name: "rozsah_kritérií",
				description: "je rozsah buniek, ktoré chcete hodnotiť podľa konkrétnej podmienky"
			},
			{
				name: "kritériá",
				description: "je podmienka alebo kritérium vo forme čísla, výrazu alebo textu definujúceho bunky, ktoré chcete pripočítať"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Vráti súčet súčinov zodpovedajúcich rozsahov alebo polí.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "pole1",
				description: "je 2 až 255 polí, ktoré chcete vynásobiť a tieto položky sčítať. Všetky polia musia byť rovnakého typu"
			},
			{
				name: "pole2",
				description: "je 2 až 255 polí, ktoré chcete vynásobiť a tieto položky sčítať. Všetky polia musia byť rovnakého typu"
			},
			{
				name: "pole3",
				description: "je 2 až 255 polí, ktoré chcete vynásobiť a tieto položky sčítať. Všetky polia musia byť rovnakého typu"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Vráti súčet druhých mocnín argumentov. Argumentmi môžu byť čísla, polia, názvy alebo odkazy na bunky obsahujúce čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, polí, názvov alebo odkazov na polia, pre ktoré chcete zistiť súčet druhých mocnín"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, polí, názvov alebo odkazov na polia, pre ktoré chcete zistiť súčet druhých mocnín"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Vypočíta súčet rozdielov druhých mocnín hodnôt dvoch zodpovedajúcich rozsahov alebo polí.",
		arguments: [
			{
				name: "pole_x",
				description: "je prvý rozsah alebo pole čísel, ktorým môže byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			},
			{
				name: "pole_y",
				description: "je druhý rozsah alebo pole čísel, ktorým môže byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Vráti celkový súčet súčtov druhých mocnín čísel v dvoch zodpovedajúcich rozsahoch alebo poliach.",
		arguments: [
			{
				name: "pole_x",
				description: "je prvý rozsah alebo pole čísel, ktorým môže byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			},
			{
				name: "pole_y",
				description: "je druhý rozsah alebo pole čísel, ktorým môže byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Vypočíta súčet druhých mocnín rozdielov hodnôt dvoch zodpovedajúcich rozsahov alebo polí.",
		arguments: [
			{
				name: "pole_x",
				description: "je prvý rozsah alebo pole hodnôt. Môže to byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			},
			{
				name: "pole_y",
				description: "je druhý rozsah alebo pole hodnôt. Môže to byť číslo alebo názov, pole alebo odkaz obsahujúci čísla"
			}
		]
	},
	{
		name: "SYD",
		description: "Vypočíta odpis majetku za zadané obdobie podľa metódy odpisovania s faktorom súčtu čísel rokov.",
		arguments: [
			{
				name: "cena",
				description: "je vstupná cena majetku"
			},
			{
				name: "zostatok",
				description: "je zostatková cena na konci životnosti majetku"
			},
			{
				name: "životnosť",
				description: "je počet období, v ktorých sa majetok odpisuje (tzv. životnosť majetku)"
			},
			{
				name: "obd",
				description: "je obdobie, ktoré musí byť vyjadrené v rovnakých jednotkách ako argument Životnosť"
			}
		]
	},
	{
		name: "T",
		description: "Skontroluje, či je argument textovou hodnotou. Ak áno, vráti text, v opačnom prípade vráti úvodzovky (bez textu).",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, ktorú chcete otestovať"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Vráti zľava ohraničené Studentovo t-rozdelenie.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pri ktorej sa hodnotí rozdelenie"
			},
			{
				name: "stupeň_voľnosti",
				description: "je celé číslo označujúce počet stupňov voľnosti, ktoré charakterizuje rozdelenie"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: ak ide o funkciu kumulatívneho rozdelenia, použije sa hodnota TRUE a ak ide o funkciu hustoty pravdepodobnosti, použite hodnotu FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Vráti obojstranne ohraničené Studentovo t-rozdelenie.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pri ktorej sa hodnotí rozdelenie"
			},
			{
				name: "stupeň_voľnosti",
				description: "je celé číslo označujúce počet stupňov voľnosti, ktoré charakterizuje rozdelenie"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Vráti sprava ohraničené Studentovo t-rozdelenie.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pri ktorej sa hodnotí rozdelenie"
			},
			{
				name: "stupeň_voľnosti",
				description: "je celé číslo označujúce počet stupňov voľnosti, ktoré charakterizuje rozdelenie"
			}
		]
	},
	{
		name: "T.INV",
		description: "Vráti zľava ohraničenú hodnotu Studentovho t-rozdelenia.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená k obojstranne ohraničenému Studentovmu t-rozdeleniu a je to číslo v rozsahu od 0 do 1 vrátane"
			},
			{
				name: "stupeň_voľnosti",
				description: "je kladné celé číslo označujúce počet stupňov voľnosti, ktorý charakterizuje rozdelenie"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Vráti obojstranne ohraničenú hodnotu Studentovho t-rozdelenia.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť priradená k obojstranne ohraničenému Studentovmu t-rozdeleniu a je to číslo v rozsahu od 0 do 1 vrátane"
			},
			{
				name: "stupeň_voľnosti",
				description: "je kladné celé číslo označujúce počet stupňov voľnosti, ktorý charakterizuje rozdelenie"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Vráti hodnotu Studentovho t-testu.",
		arguments: [
			{
				name: "pole1",
				description: "je prvá množina údajov"
			},
			{
				name: "pole2",
				description: "je druhá množina údajov"
			},
			{
				name: "strany",
				description: "určí, či sa jedná o jednostrannú alebo obojstrannú distribúciu: jednostranná distribúcia = 1; obojstranná distribúcia = 2"
			},
			{
				name: "typ",
				description: "je druh t-testu: párový = 1, dvojvýberový s rovnakým rozptylom = 2, dvojvýberový s nerovnakým rozptylom = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Vráti tangens uhla.",
		arguments: [
			{
				name: "číslo",
				description: "je uhol v radiánoch, ktorého tangens chcete zistiť. Stupne * PI()/180 = radiány"
			}
		]
	},
	{
		name: "TANH",
		description: "Vráti hyperbolický tangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je ľubovoľné reálne číslo"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Vráti výnos ekvivalentný obligácii za štátnu pokladničnú poukážku.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "zľava",
				description: "je diskontná sadzba štátnej pokladničnej poukážky"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Vráti cenu za každých 100 USD nominálnej hodnoty štátnej pokladničnej poukážky.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "zľava",
				description: "je diskontná sadzba štátnej pokladničnej poukážky"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Vráti výnos štátnej pokladničnej poukážky.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti štátnej pokladničnej poukážky, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "ss",
				description: "je cena štátnej pokladničnej poukážky za každých 100 USD nominálnej hodnoty"
			}
		]
	},
	{
		name: "TDIST",
		description: "Vráti hodnotu Studentovho t-rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pre ktorú chcete zistiť hodnotu rozdelenia"
			},
			{
				name: "stupeň_voľnosti",
				description: "je celé číslo predstavujúce počet stupňov voľnosti, ktoré určujú rozdelenie"
			},
			{
				name: "strany",
				description: "určí, či sa jedná o jednostranné alebo obojstranné rozdelenie: jednostranné rozdelenie = 1; obojstranné rozdelenie = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Konvertuje hodnotu na text v špeciálnom číselnom formáte.",
		arguments: [
			{
				name: "hodnota",
				description: "je číslo, vzorec, ktorého výsledkom je číselná hodnota, alebo odkaz na bunku obsahujúcu číselnú hodnotu"
			},
			{
				name: "formát_text",
				description: "je číselný formát vo forme textu vybratý zo zoznamu Kategória na karte Číslo v dialógovom okne Formát buniek (okrem kategórie Všeobecné)"
			}
		]
	},
	{
		name: "TIME",
		description: "Konvertuje hodiny, minúty a sekundy zadané ako čísla na poradové číslo programu Spreadsheet vo formáte času.",
		arguments: [
			{
				name: "hodina",
				description: "je číslo 0 až 23, ktoré predstavuje hodinu"
			},
			{
				name: "minúta",
				description: "je číslo 0 až 59, ktoré predstavuje minútu"
			},
			{
				name: "sekunda",
				description: "je číslo 0 až 59, ktoré predstavuje sekundu"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Konvertuje čas vo formáte textového reťazca na poradové číslo programu Spreadsheet pre čas, číslo od 0 (12:00:00 dop.) do 0,999988426 (11:59:59 odp.). Po zadaní vzorca číslo sformátujte do formátu času.",
		arguments: [
			{
				name: "text_času",
				description: "je textový reťazec, ktorý predstavuje čas v ľubovoľnom formáte programu Spreadsheet pre čas (informácie o dátume sa v reťazci ignorujú)"
			}
		]
	},
	{
		name: "TINV",
		description: "Vráti inverznú funkciu k funkcii Studentovho t-rozdelenia.",
		arguments: [
			{
				name: "pravdepodobnosť",
				description: "je pravdepodobnosť zodpovedajúca obojstrannému Studentovmu t-rozdeleniu, číslo medzi 0 a 1 vrátane"
			},
			{
				name: "stupeň_voľnosti",
				description: "je kladné celé číslo predstavujúce počet stupňov voľnosti, ktoré určujú rozdelenie"
			}
		]
	},
	{
		name: "TODAY",
		description: "Vráti aktuálny dátum vo formáte dátumu.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Konvertuje vodorovný rozsah buniek na zvislý alebo naopak.",
		arguments: [
			{
				name: "pole",
				description: "je rozsah buniek hárka alebo pole hodnôt, ktoré chcete transponovať"
			}
		]
	},
	{
		name: "TREND",
		description: "Vráti čísla lineárneho trendu zodpovedajúce známym údajovým bodom pomocou metódy najmenších štvorcov.",
		arguments: [
			{
				name: "známe_y",
				description: "je rozsah alebo pole známych hodnôt v rovnici y = mx + b"
			},
			{
				name: "známe_x",
				description: "je voliteľný rozsah alebo pole známych hodnôt x v rovnici y = mx + b. Pole musí mať rovnakú veľkosť ako pole argumentu známe_y"
			},
			{
				name: "nové_x",
				description: "je rozsah alebo pole nových hodnôt x, pre ktoré má funkcia TREND zistiť zodpovedajúce hodnoty y"
			},
			{
				name: "konštanta",
				description: "je logická hodnota: konštanta b sa vypočíta, ak argument b = TRUE alebo nie je zadaný; b sa rovná nule, ak argument b = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Odstráni všetky medzery z textového reťazca okrem jednotlivých medzier medzi slovami.",
		arguments: [
			{
				name: "text",
				description: "je text, z ktorého chcete odstrániť medzery"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Vráti priemernú hodnotu vnútornej časti množiny hodnôt údajov.",
		arguments: [
			{
				name: "pole",
				description: "je rozsah alebo pole hodnôt, ktoré sa má orezať a zo zvyšných hodnôt vypočítať priemer"
			},
			{
				name: "percento",
				description: "je zlomok udávajúci počet údajových bodov, ktoré chcete vylúčiť z hornej a dolnej časti množiny údajov"
			}
		]
	},
	{
		name: "TRUE",
		description: "Vráti logickú hodnotu TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Skráti číslo na celé číslo odstránením desatinnej alebo zlomkovej časti čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, ktoré chcete skrátiť"
			},
			{
				name: "počet_číslic",
				description: "je číslo určujúce presnosť skrátenia. Ak tento argument nezadáte, bude jeho hodnota 0"
			}
		]
	},
	{
		name: "TTEST",
		description: "Vráti hodnotu Studentovho t-testu.",
		arguments: [
			{
				name: "pole1",
				description: "je prvá množina údajov"
			},
			{
				name: "pole2",
				description: "je druhá množina údajov"
			},
			{
				name: "strany",
				description: "určí, či sa jedná o jednostrannú alebo obojstrannú distribúciu: jednostranná distribúcia = 1; obojstranná distribúcia = 2"
			},
			{
				name: "typ",
				description: "je druh t-testu: párový = 1, dvojvýberový s rovnakým rozptylom = 2, dvojvýberový s nerovnakým rozptylom = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Vráti celé číslo, ktoré predstavuje typ údajov hodnoty: číslo = 1; text = 2; logická hodnota = 4; chybová hodnota = 16; pole = 64.",
		arguments: [
			{
				name: "hodnota",
				description: "môže byť ľubovoľná hodnota"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Vráti číslo (bod kódu) zodpovedajúce prvému znaku textu.",
		arguments: [
			{
				name: "text",
				description: "je znak, ktorého hodnotu Unicode chcete zistiť"
			}
		]
	},
	{
		name: "UPPER",
		description: "Konvertuje všetky písmená v textovom reťazci na veľké.",
		arguments: [
			{
				name: "text",
				description: "je text, ktorý chcete konvertovať na veľké písmená. Môže to byť odkaz alebo textový reťazec"
			}
		]
	},
	{
		name: "VALUE",
		description: "Konvertuje textový reťazec predstavujúci číslo na číslo.",
		arguments: [
			{
				name: "text",
				description: "je text v úvodzovkách alebo odkaz na bunku obsahujúcu text, ktorý chcete konvertovať"
			}
		]
	},
	{
		name: "VAR",
		description: "Odhadne rozptyl na základe vzorky (ignoruje logické hodnoty a text vo vzorke).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú vzorke výberu"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú vzorke výberu"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Vypočíta rozptyl základného súboru (ignoruje logické hodnoty a text v súbore).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú základnému súboru"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú základnému súboru"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Odhadne rozptyl na základe vzorky (vo vzorke ignoruje logické hodnoty a text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú výberovému súboru"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú výberovému súboru"
			}
		]
	},
	{
		name: "VARA",
		description: "Odhadne rozptyl podľa výberového súboru vrátane logických hodnôt a textu. Text a logická hodnota FALSE = 0; logická hodnota TRUE = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnôt argumentov, ktoré zodpovedajú výberovému súboru"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnôt argumentov, ktoré zodpovedajú výberovému súboru"
			}
		]
	},
	{
		name: "VARP",
		description: "Vypočíta rozptyl základného súboru (ignoruje logické hodnoty a text v súbore).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú základnému súboru"
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentov, ktoré zodpovedajú základnému súboru"
			}
		]
	},
	{
		name: "VARPA",
		description: "Vypočíta rozptyl základného súboru vrátane logických hodnôt a textu. Text a logická hodnota FALSE = 0; logická hodnota TRUE = 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnôt argumentov, ktoré zodpovedajú základnému súboru"
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnôt argumentov, ktoré zodpovedajú základnému súboru"
			}
		]
	},
	{
		name: "VDB",
		description: "Vypočíta odpisy majetku za zadané obdobia vrátane neukončených období podľa metódy dvojnásobného odpisovania z klesajúceho zostatku alebo inej zadanej metódy.",
		arguments: [
			{
				name: "cena",
				description: "je vstupná cena majetku"
			},
			{
				name: "zostatok",
				description: "je zostatková cena na konci životnosti majetku"
			},
			{
				name: "životnosť",
				description: "je počet období, v ktorých sa majetok odpisuje (tzv. životnosť majetku)"
			},
			{
				name: "počiatočné_obdobie",
				description: "je začiatočné obdobie, za ktoré chcete vypočítať odpis. Počíta sa v rovnakých jednotkách ako argument Životnosť"
			},
			{
				name: "koncové_obdobie",
				description: "je koncové obdobie, za ktoré chcete vypočítať odpis. Počíta sa v rovnakých jednotkách ako argument Životnosť"
			},
			{
				name: "faktor",
				description: "je miera poklesu zostatku. Ak tento argument nezadáte, použije sa hodnota 2 (metóda dvojnásobného odpisovania z klesajúceho zostatku)"
			},
			{
				name: "bez_zmeny",
				description: "je logická hodnota: prejsť na rovnomerné odpisovanie, ak je hodnota odpisu väčšia ako klesajúci zostatok = FALSE alebo nie je zadaná; neprejsť na rovnomerné odpisovanie = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Hľadá hodnotu v ľavom krajnom stĺpci tabuľky a vráti hodnotu zo zadaného stĺpca v tom istom riadku. Predvolené zoradenie tabuľky je vzostupné.",
		arguments: [
			{
				name: "vyhľadávaná_hodnota",
				description: "je hodnota hľadaná v prvom stĺpci tabuľky. Môže to byť hodnota, odkaz alebo textový reťazec"
			},
			{
				name: "pole_tabuľky",
				description: "je prehľadávaná tabuľka s textom, číslami alebo logickými hodnotami. Argument Pole_tabuľky môže byť odkaz na rozsah alebo názov rozsahu"
			},
			{
				name: "číslo_indexu_stĺpca",
				description: "je číslo stĺpca v argumente Pole_tabuľky, z ktorého sa má vrátiť zodpovedajúca hodnota. Prvý stĺpec hodnôt v tabuľke je stĺpec číslo 1"
			},
			{
				name: "vyhľadávanie_rozsahu",
				description: "je logická hodnota: nájsť najbližšiu zodpovedajúcu hodnotu v prvom stĺpci (zoradenom vzostupne) = TRUE alebo nie je zadaná; nájsť presne zodpovedajúcu hodnotu = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Vráti číslo 1 až 7, ktoré v dátume určuje deň v týždni.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo reprezentujúce dátum"
			},
			{
				name: "vrátené_číslo",
				description: "je číslo: v prípade týždňa od nedele = 1 do soboty = 7 použite číslo 1, v prípade týždňa od pondelka = 1 do nedele = 7 použite číslo 2 a v prípade týždňa od pondelka = 0 do nedele = 6 použite číslo 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Vráti číselné označenie týždňa v roku.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je kód dátumu a času, ktorý používa program Spreadsheet na výpočty dátumu a času"
			},
			{
				name: "vrátený_typ",
				description: "je číslo (1 alebo 2), ktoré určuje typ vrátenej hodnoty"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty Weibullovho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu rozdelenia. Musí to byť nezáporné číslo"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia = TRUE; hustota rozdelenia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Vráti hodnotu distribučnej funkcie alebo hustoty Weibullovho rozdelenia.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pre ktorú chcete zistiť hodnotu rozdelenia. Musí to byť nezáporné číslo"
			},
			{
				name: "alfa",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "beta",
				description: "je parameter rozdelenia, ktorý musí byť kladné číslo"
			},
			{
				name: "kumulatívne",
				description: "je logická hodnota: distribučná funkcia = TRUE; hustota rozdelenia pravdepodobnosti = FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Vráti poradové číslo dátumu pred alebo po zadanom počte pracovných dní.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "dni",
				description: "je počet dní, ktoré nepripadajú na víkend a ani voľno pred alebo po počiatočnom_dátume"
			},
			{
				name: "sviatky",
				description: "je voliteľné pole poradových čísel dátumu, ktoré možno vyňať z pracovného kalendára, napríklad štátne sviatky a pohyblivé dni voľna"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Vráti poradové číslo dátumu pred zadaným počtom pracovných dní alebo po zadanom počte pracovných dní s vlastnými parametrami víkendov.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "dni",
				description: "je počet dní pred počiatočným dátumom alebo po ňom, ktoré nepripadajú na víkendy alebo na sviatky"
			},
			{
				name: "víkend",
				description: "je číslo alebo reťazec označujúci, kedy sa vyskytnú víkendy"
			},
			{
				name: "sviatky",
				description: "je voliteľné pole poradových čísel dátumov, ktoré sa majú vylúčiť z pracovného kalendára, ako sú napríklad štátne sviatky a pohyblivé sviatky"
			}
		]
	},
	{
		name: "XIRR",
		description: "Vráti vnútornú mieru návratnosti pre plán hotovostných tokov.",
		arguments: [
			{
				name: "hodnoty",
				description: "je rad hotovostných tokov, ktorý zodpovedá rozvrhu platieb k jednotlivým dátumom"
			},
			{
				name: "dátumy",
				description: "je rozvrh dátumov platieb, ktorý zodpovedá platbám hotovostných tokov"
			},
			{
				name: "odhad",
				description: "je číslo, o ktorom sa predpokladá, že sa blíži výsledku XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Vráti čistú súčasnú hodnotu plánu hotovostných tokov.",
		arguments: [
			{
				name: "sadzba",
				description: "je diskontná sadzba, ktorá sa má použiť na hotovostné toky"
			},
			{
				name: "hodnoty",
				description: "je rad hotovostných tokov, ktorý zodpovedá rozvrhu platieb k jednotlivým dátumom"
			},
			{
				name: "dátumy",
				description: "je rozvrh dátumov platieb, ktorý zodpovedá platbám hotovostných tokov"
			}
		]
	},
	{
		name: "XOR",
		description: "Vráti logický operátor Exclusive Or všetkých argumentov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "je 1 až 254 podmienok, ktoré chcete otestovať a ktoré môžu byť TRUE alebo FALSE a môžu byť logickými hodnotami, poľami alebo odkazmi"
			},
			{
				name: "logical2",
				description: "je 1 až 254 podmienok, ktoré chcete otestovať a ktoré môžu byť TRUE alebo FALSE a môžu byť logickými hodnotami, poľami alebo odkazmi"
			}
		]
	},
	{
		name: "YEAR",
		description: "Vráti rok dátumu, celé číslo v rozsahu od 1900 do 9999.",
		arguments: [
			{
				name: "poradové_číslo",
				description: "je číslo v kóde programu Spreadsheet pre dátum a čas"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Vráti zlomok roka predstavujúci počet celých dní medzi počiatočným a koncovým dátumom.",
		arguments: [
			{
				name: "počiatočný_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje počiatočný dátum"
			},
			{
				name: "koncový_dátum",
				description: "je poradové číslo dátumu, ktoré predstavuje koncový dátum"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Vráti ročný výnos diskontovaného cenného papiera. Ide napríklad o štátne pokladničné poukážky.",
		arguments: [
			{
				name: "vyrovnanie",
				description: "je dátum vyrovnania cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "splatnosť",
				description: "je dátum splatnosti cenného papiera, vyjadrený ako poradové číslo dátumu"
			},
			{
				name: "ss",
				description: "je cena cenného papiera za každých 100 USD nominálnej hodnoty"
			},
			{
				name: "vyplatenie",
				description: "je hodnota vyplatenia cenného papiera za každých 100 USD nominálnej hodnoty"
			},
			{
				name: "základ",
				description: "je typ denného základu, ktorý chcete použiť"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Vráti jednostrannú P-hodnotu z-testu.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, ktoré majú byť použité na testovanie argumentu X"
			},
			{
				name: "x",
				description: "je testovaná hodnota"
			},
			{
				name: "sigma",
				description: "je známa smerodajná odchýlka základného súboru. Ak sa vynechá, použije sa smerodajná odchýlka vzorky"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Vráti jednostrannú P-hodnotu z-testu.",
		arguments: [
			{
				name: "pole",
				description: "je pole alebo rozsah údajov, ktoré majú byť použité na testovanie argumentu X"
			},
			{
				name: "x",
				description: "je testovaná hodnota"
			},
			{
				name: "sigma",
				description: "je známa smerodajná odchýlka základného súboru. Ak sa vynechá, použije sa smerodajná odchýlka vzorky"
			}
		]
	}
];