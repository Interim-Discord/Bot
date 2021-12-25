// ReSharper disable StringLiteralTypo

using Interim.Extensions;

namespace Interim.Features.TimeZones;

public readonly struct Locale
{
	public readonly string Name;
	public readonly string ID;
	public readonly string? Emoji;

	public Locale(string name, string id, string? emoji)
	{
		Name = name;
		ID = id;
		Emoji = emoji;
	}

	public DiscordButtonComponent ToButton(DiscordClient discord) => new(
		ButtonStyle.Primary,
		ID.ToInteractionId(),
		Name,
		emoji:
		Emoji == null ? null : new DiscordComponentEmoji(DiscordEmoji.FromName(discord, Emoji))
	);

	public DiscordSelectComponentOption ToSelectOption(DiscordClient discord) => new(
		Name,
		ID.ToInteractionId(),
		emoji: Emoji == null ? null : new DiscordComponentEmoji(DiscordEmoji.FromName(discord, Emoji))
	);
}

public enum InteractionMode
{
	Buttons,
	Dropdowns
}

public class LocalGroup
{
	public readonly string Question;
	public readonly InteractionMode Mode;
	public readonly Locale[][] Locales;

	public LocalGroup(string question, InteractionMode mode, Locale[][] locales)
	{
		Question = question;
		Mode = mode;
		Locales = locales;
	}
}

public static class TimeZoneLookup
{
	public static readonly Locale[][] Continents =
	{
		new Locale[]
		{
			new("Africa", "continent_africa", null),
			new("Antarctica", "continent_antarctica", null),
			new("Asia", "continent_asia", null),
			new("Europe", "continent_europe", null)
		},
		new Locale[]
		{
			new("North and Central America", "continent_north-and-central-america", null),
			new("South America", "continent_south-america", null),
			new("Oceania", "continent_oceania", null)
		}
	};

	/// <summary>
	/// A lookup between continents and countries, and further narrowing required to get a timezone, or the timezone itself.
	/// </summary>
	public static readonly Dictionary<string, LocalGroup> Groups = new()
	{
		#region Continents

		{
			"continent_africa",
			new LocalGroup
			(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Algeria", "tz_Africa/Algiers", ":flag_dz:"),
						new("Ascension Island", "tz_Atlantic/St_Helena", ":flag_ac:"),
						new("Angola", "tz_Africa/Luanda", ":flag_ao:"),
						new("Benin", "tz_Africa/Porto-Novo", ":flag_bj:"),
						new("Botswana", "tz_Africa/Maputo", ":flag_bw:"),
						new("Burkina Faso", "tz_Africa/Ouagadougou", ":flag_bf:"),
						new("Burundi", "tz_Africa/Bujumbura", ":flag_bi:"),
						new("Canary Islands", "tz_Atlantic/Canary", ":flag_ic:"),
						new("Cameroon", "tz_Africa/Douala", ":flag_cm:"),
						new("Cape Verde", "tz_Atlantic/Cape_Verde", ":flag_cv:"),
						new("Central African Republic", "tz_Africa/Bangui", ":flag_cf:"),
						new("Ceuta", "tz_Africa/Ceuta", ":flag_ea:"),
						new("Chad", "tz_Africa/Ndjamena", ":flag_td:"),
						new("Comoros", "tz_Indian/Comoro", ":flag_km:"),
						new("Republic of the Congo", "tz_Africa/Brazzaville", ":flag_cg:"),
						new("DR Congo", "country_dr-congo", ":flag_cd:"),
						new("Côte d'Ivoire", "tz_Africa/Abidjan", ":flag_ci:"),
						new("Djibouti", "tz_Africa/Djibouti", ":flag_dj:"),
						new("Equatorial Guinea", "tz_Africa/Malabo", ":flag_gq:"),
						new("Egypt", "tz_Africa/Cairo", ":flag_eg:"),
						new("Eritrea", "tz_Africa/Asmara", ":flag_er:"),
						new("Eswatini", "tz_Africa/Mbabane", ":flag_sz:"),
						new("Ethiopia", "tz_Africa/Addis_Ababa", ":flag_et:")
						// space for 2 more items
					},
					new Locale[]
					{
						new("Gabon", "tz_Africa/Libreville", ":flag_ga:"),
						new("The Gambia", "tz_Africa/Banjul", ":flag_gm:"),
						new("Ghana", "tz_Africa/Accra", ":flag_gh:"),
						new("Guinea", "tz_Africa/Conakry", ":flag_gn:"),
						new("Guinea-Bissau", "tz_Africa/Bissau", ":flag_gw:"),
						new("Kenya", "tz_Africa/Nairobi", ":flag_ke:")
					},
					new Locale[]
					{
						new("Lesotho", "tz_Africa/Maseru", ":flag_ls:"),
						new("Liberia", "tz_Africa/Monrovia", ":flag_lr:"),
						new("Libya", "tz_Africa/Tripoli", ":flag_ly:"),
						new("Madagascar", "tz_Indian/Antananarivo", ":flag_mg:"),
						new("Malawi ", "tz_Africa/Blantyre", ":flag_mw:"),
						new("Mali", "tz_Africa/Bamako", ":flag_ml:"),
						new("Mauritania", "tz_Africa/Nouakchott", ":flag_mr:"),
						new("Mauritius", "tz_Indian/Mauritius", ":flag_mu:"),
						new("Mayotte", "tz_Indian/Mayotte", ":flag_yt:"),
						new("Melilla", "tz_Africa/Ceuta", ":flag_ea:"),
						new("Morocco", "tz_Africa/Casablanca", ":flag_ma:"),
						new("Mozambique ", "tz_Africa/Maputo", ":flag_mz:"),
						new("Namibia", "tz_Africa/Windhoek", ":flag_na:"),
						new("Niger", "tz_Africa/Niamey", ":flag_ne:"),
						new("Nigeria", "tz_Africa/Lagos", ":flag_ng:"),
						new("Réunion", "tz_Indian/Reunion", ":flag_re:"),
						new("Rwanda", "tz_Africa/Kigali", ":flag_rw:")
					},
					new Locale[]
					{
						new("Saint Helena", "tz_Atlantic/St_Helena", ":flag_sh:"),
						new("São Tomé and Príncipe", "tz_Africa/Sao_Tome", ":flag_st:"),
						new("Senegal", "tz_Africa/Dakar", ":flag_sn:"),
						new("Seychelles", "tz_Indian/Mahe", ":flag_sc:"),
						new("Sierra Leone", "tz_Africa/Freetown", ":flag_sl:"),
						new("Somalia", "tz_Africa/Mogadishu", ":flag_so:"),
						new("South Africa", "tz_Africa/Johannesburg", ":flag_za:"),
						new("South Sudan", "tz_Africa/Juba", ":flag_ss:"),
						new("Sudan", "tz_Africa/Khartoum", ":flag_sd:"),
						new("Tanzania", "tz_Africa/Dar_es_Salaam", ":flag_tz:"),
						new("Togo", "tz_Africa/Lome", ":flag_tg:"),
						new("Tristan da Cunha", "tz_Atlantic/St_Helena", ":flag_ta:"),
						new("Tunisia", "tz_Africa/Tunis", ":flag_tn:"),
						new("Uganda", "tz_Africa/Kampala", ":flag_ug:"),
						new("Western Sahara", "tz_Africa/El_Aaiun", ":flag_eh:"),
						new("Zambia", "tz_Africa/Lusaka", ":flag_zm:"),
						new("Zimbabwe", "tz_Africa/Harare", ":flag_zw:")
					}
				}
			)
		},
		{
			"continent_antarctica",
			new LocalGroup(
				"Which base (wow, Antarctica, really?) are you closest to?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Casey", "tz_Antarctica/Casey", ":flag_au:"),
						new("Davis", "tz_Antarctica/Davis", ":flag_au:"),
						new("Macquarie Island", "tz_Antarctica/Macquarie", ":flag_au:"),
						new("Mawson", "tz_Antarctica/Mawson", ":flag_au:")
					},
					new Locale[]
					{
						new("McMurdo", "tz_Antarctica/McMurdo", ":flag_us:"),
						new("Palmer", "tz_Antarctica/Palmer", ":flag_us:")
					},
					new Locale[]
					{
						new("Dumont-d'Urville", "tz_Antarctica/DumontDUrville", ":flag_fr:"),
						new("Rothera", "tz_Antarctica/Rothera", ":flag_gb:"),
						new("Syowa", "tz_Antarctica/Syowa", ":flag_jp:")
					},
					new Locale[]
					{
						new("Troll", "tz_Antarctica/Troll", ":flag_no:"),
						new("Vostok", "tz_Antarctica/Vostok", ":flag_ru:")
					}
				}
			)
		},
		{
			"continent_asia",
			new LocalGroup(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Afghanistan", "tz_Asia/Kabul", ":flag_af:"),
						new("Armenia", "tz_Asia/Yerevan", ":flag_am:"),
						new("Azerbaijan", "tz_Asia/Baku", ":flag_az:"),
						new("Bahrain", "tz_Asia/Bahrain", ":flag_bh:"),
						new("Bangladesh", "tz_Asia/Dhaka", ":flag_bd:"),
						new("Bhutan", "tz_Asia/Thimphu", ":flag_bt:"),
						new("Brunei", "tz_Asia/Brunei", ":flag_bn:"),
						new("Cambodia", "tz_Asia/Phnom_Penh", ":flag_kh:"),
						// China is weird, so there *could* be more timezones expressed here. Will have to add if people ask.
						new("China", "tz_Asia/Shanghai", ":flag_cn:"),
						new("East Timor", "tz_Asia/Dili", ":flag_tl:"),
						new("Georgia", "tz_Asia/Tbilisi", ":flag_ge:"),
						new("Hong Kong", "tz_Asia/Shanghai", ":flag_hk:"),
						new("India", "tz_Asia/Kolkata", ":flag_in:"),
						new("Indonesia", "country_indonesia", ":flag_id:"),
						new("Iran", "tz_Asia/Tehran", ":flag_ir:"),
						new("Iraq", "tz_Asia/Baghdad", ":flag_iq:"),
						new("Israel", "tz_Asia/Jerusalem", ":flag_il:"),
						new("Japan", "tz_Asia/Tokyo", ":flag_jp:"),
						new("Jordan", "tz_Asia/Amman", ":flag_jo:"),
						new("Kazakhstan", "tz_Asia/Almaty", ":flag_kz:"),
						new("Kuwait", "tz_Asia/Kuwait", ":flag_kw:"),
						new("Kyrgyzstan", "tz_Asia/Bishkek", ":flag_kg:")
					},
					new Locale[]
					{
						new("Laos", "tz_Asia/Vientiane", ":flag_la:"),
						new("Lebanon", "tz_Asia/Beirut", ":flag_lb:"),
						new("Macau", "tz_Asia/Shanghai", ":flag_mo:"),
						new("Malaysia", "tz_Asia/Kuala_Lumpur", ":flag_my:"),
						new("Maldives", "tz_Indian/Maldives", ":flag_mv:"),
						new("Mongolia", "country_mongolia", ":flag_mn:"),
						new("Myanmar", "tz_Asia/Rangoon", ":flag_mm:"),
						new("Nepal", "tz_Asia/Kathmandu", ":flag_np:"),
						new("North Korea", "tz_Asia/Pyongyang", ":flag_kp:"),
						new("Oman", "tz_Asia/Muscat", ":flag_om:"),
						new("Pakistan", "tz_Asia/Karachi", ":flag_pk:"),
						new("Palestine", "tz_Asia/Gaza", ":flag_ps:"),
						new("Philippines", "tz_Asia/Manila", ":flag_ph:"),
						new("Qatar", "tz_Asia/Qatar", ":flag_qa:"),
						new("Russia", "country_russia", ":flag_ru:")
					},
					new Locale[]
					{
						new("Saudi Arabia", "tz_Asia/Riyadh", ":flag_sa:"),
						new("Singapore", "tz_Asia/Singapore", ":flag_sg:"),
						new("South Korea", "tz_Asia/Seoul", ":flag_kr:"),
						new("Sri Lanka", "tz_Asia/Colombo", ":flag_lk:"),
						new("Syria", "tz_Asia/Damascus", ":flag_sy:"),
						new("Tajikistan", "tz_Asia/Dushanbe", ":flag_tj:"),
						new("Thailand", "tz_Asia/Bangkok", ":flag_th:"),
						new("Timor-Leste", "tz_Asia/Dili", ":flag_tl:"),
						new("Turkey", "tz_Europe/Istanbul", ":flag_tr:"),
						new("Turkmenistan", "tz_Asia/Ashgabat", ":flag_tm:"),
						new("Taiwan", "tz_Asia/Taipei", ":flag_tw:"),
						new("United Arab Emirates", "tz_Asia/Dubai", ":flag_ae:"),
						new("Uzbekistan", "tz_Asia/Tashkent", ":flag_uz:"),
						new("Vietnam", "tz_Asia/Ho_Chi_Minh", ":flag_vn:"),
						new("Yemen", "tz_Asia/Aden", ":flag_ye:")
					}
				}
			)
		},
		{
			"continent_europe",
			new LocalGroup(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Åland Islands", "tz_Europe/Mariehamn", ":flag_ax:"),
						new("Albania", "tz_Europe/Tirane", ":flag_al:"),
						new("Andorra", "tz_Europe/Andorra", ":flag_ad:"),
						new("Austria", "tz_Europe/Vienna", ":flag_at:"),
						new("Belarus", "tz_Europe/Minsk", ":flag_by:"),
						new("Belgium", "tz_Europe/Brussels", ":flag_be:"),
						new("Bosnia and Herzegovina", "tz_Europe/Sarajevo", ":flag_ba:"),
						new("Bulgaria", "tz_Europe/Sofia", ":flag_bg:"),
						new("Croatia", "tz_Europe/Zagreb", ":flag_hr:"),
						new("Cyprus", "tz_Asia/Nicosia", ":flag_cy:"),
						new("Czechia", "tz_Europe/Prague", ":flag_cz:"),
						// Contains Greenland and the Faroe Islands
						new("Denmark", "country_denmark", ":flag_dk:"),
						new("Estonia", "tz_Europe/Tallinn", ":flag_ee:"),
						new("Faroe Islands", "tz_Atlantic/Faroe", ":flag_fo:"),
						new("Finland", "tz_Europe/Helsinki", ":flag_fi:"),
						// Fuck France - 12 time zones, really?
						new("France", "country_france", ":flag_fr:"),
						new("Georgia", "tz_Asia/Tbilisi", ":flag_ge:"),
						new("Germany", "tz_Europe/Berlin", ":flag_de:"),
						new("Gibraltar", "tz_Europe/Gibraltar", ":flag_gi:"),
						new("Greece", "tz_Europe/Athens", ":flag_gr:"),
						new("Greenland", "country_greenland", ":flag_gh:"),
						new("Guernsey", "tz_Europe/Guernsey", ":flag_gg:"),
						new("Hungary", "tz_Europe/Budapest", ":flag_hu:")
						// There is space for two more additions
					},
					new Locale[]
					{
						new("Iceland", "tz_Atlantic/Reykjavik", ":flag_is:"),
						new("Ireland", "tz_Europe/Dublin", ":flag_ie:"),
						new("Isle of Man", "tz_Europe/Isle_of_Man", ":flag_im:"),
						new("Italy", "tz_Europe/Rome", ":flag_it:"),
						new("Jersey", "tz_Europe/Jersey", ":flag_je:"),
						new("Kosovo", "tz_Europe/Belgrade", ":flag_xk:"),
						new("Latvia", "tz_Europe/Riga", ":flag_lv:"),
						new("Liechtenstein", "tz_Europe/Vaduz", ":flag_li:"),
						new("Lithuania", "tz_Europe/Vilnius", ":flag_lt:"),
						new("Luxembourg", "tz_Europe/Luxembourg", ":flag_lu:"),
						new("North Macedonia", "tz_Europe/Skopje", ":flag_mk:"),
						new("Malta", "tz_Europe/Malta", ":flag_mt:"),
						new("Moldova", "tz_Europe/Chisinau", ":flag_md:"),
						new("Monaco", "tz_Europe/Monaco", ":flag_mc:"),
						new("Montenegro", "tz_Europe/Podgorica", ":flag_me:"),
						new("Netherlands", "tz_Europe/Amsterdam", ":flag_nl:"),
						new("Norway", "tz_Europe/Oslo", ":flag_no:"),
						new("Poland", "tz_Europe/Warsaw", ":flag_pl:"),
						new("Portugal", "country_portugal", ":flag_pt:"),
						new("Romania", "tz_Europe/Bucharest", ":flag_ro:"),
						new("Russia", "country_russia", ":flag_ru:")
					},
					new Locale[]
					{
						new("San Marino", "tz_Europe/San_Marino", ":flag_sm:"),
						new("Svalbard & Jan Mayen", "tz_Arctic/Longyearbyen", ":flag_sj:"),
						new("Serbia", "tz_Europe/Belgrade", ":flag_rs:"),
						new("Slovakia", "tz_Europe/Bratislava", ":flag_sk:"),
						new("Slovenia", "tz_Europe/Ljubljana", ":flag_si:"),
						new("Spain", "country_spain", ":flag_es:"),
						new("Sweden", "tz_Europe/Stockholm", ":flag_se:"),
						new("Switzerland", "tz_Europe/Zurich", ":flag_ch:"),
						new("Turkey", "tz_Europe/Istanbul", ":flag_tr:"),
						new("Ukraine", "tz_Europe/Kiev", ":flag_ua:"),
						// Terf Island also gathering its "possessions"
						new("United Kingdom", "country_united-kingdom", ":flag_gb:"),
						new("Vatican City", "tz_Europe/Vatican", ":flag_va:")
					}
				}
			)
		},
		{
			"continent_north-and-central-america",
			new LocalGroup(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Canada", "country_canada", ":flag_ca:"),
						// ^ priority countries in block
						new("Anguilla", "tz_America/Anguilla", ":flag_ai:"),
						new("Antigua And Barbuda", "tz_America/Antigua", ":flag_ag:"),
						new("Bahamas", "tz_America/Nassau", ":flag_bs:"),
						new("Barbados", "tz_America/Barbados", ":flag_bb:"),
						new("Bermuda", "tz_Atlantic/Bermuda", ":flag_bm:"),
						new("Cuba", "tz_America/Havana", ":flag_cu:"),
						new("Dominica", "tz_America/Dominica", ":flag_dm:"),
						new("Dominican Republic", "tz_America/Santo_Domingo", ":flag_do:"),
						new("Haiti", "tz_America/Port-au-Prince", ":flag_ht:"),
						new("Belize", "tz_America/Belize", ":flag_bz:"),
						new("Cayman Islands", "tz_America/Cayman", ":flag_ky:"),
						new("Costa Rica", "tz_America/Costa_Rica", ":flag_cr:"),
						new("El Salvador", "tz_America/El_Salvador", ":flag_sv:"),
						new("Guatemala", "tz_America/Guatemala", ":flag_gt:"),
						new("Hawaii/Aleutian Islands", "tz_US/Hawaii", ":flag_us:"),
						new("Honduras", "tz_America/Tegucigalpa", ":flag_hn:"),
						new("Faroe Islands", "tz_Atlantic/Faroe", ":flag_fo:")
					},
					new Locale[]
					{
						new("United States of America", "country_united-states-of-america", ":flag_us:"),
						new("Mexico", "country_mexico", ":flag_mx:"),
						// ^ priority countries in block
						new("Grenada", "tz_America/Grenada", ":flag_gd:"),
						new("Guadeloupe", "tz_America/Guadeloupe", ":flag_gp:"),
						new("Jamaica", "tz_America/Jamaica", ":flag_jm:"),
						new("Martinique", "tz_America/Martinique", ":flag_mq:"),
						new("Montserrat", "tz_America/Montserrat", ":flag_ms:"),
						new("Nicaragua", "tz_America/Managua", ":flag_ni:"),
						new("Panama", "tz_America/Panama", ":flag_pa:"),
						new("Puerto Rico", "tz_America/Puerto_Rico", ":flag_pr:"),
						new("Saint Barthélemy", "tz_America/St_Barthelemy", ":flag_bl:"),
						new("Saint Kitts and Nevis", "tz_America/St_Kitts", ":flag_kn:"),
						new("Saint Lucia", "tz_America/St_Lucia", ":flag_lc:"),
						new("Saint Martin", "tz_America/Marigot", ":flag_mf:"),
						new("Saint Pierre and Miquelon", "tz_America/Miquelon", ":flag_pm:"),
						new("Saint Vincent and the Grenadines", "tz_America/St_Vincent", ":flag_vc:"),
						new("Sint Maarten", "tz_America/Guadeloupe", ":flag_sx:"),
						new("Trinidad and Tobago", "tz_America/Port_of_Spain", ":flag_tt:"),
						new("Turks and Caicos Islands", "tz_America/Grand_Turk", ":flag_tc:"),
						new("Virgin Islands (British)", "tz_America/Tortola", ":flag_vg:"),
						new("Virgin Islands (U.S.)", "tz_America/St_Thomas", ":flag_vi:")
						// space for four more
					}
				}
			)
		},
		{
			"continent_south-america",
			new LocalGroup(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Argentina", "tz_America/Buenos_Aires", ":flag_ar:"),
						new("Aruba", "tz_America/Aruba", ":flag_aw:"),
						new("Brazil", "country_brazil", ":flag_br:"),
						new("Bolivia", "tz_America/La_Paz", ":flag_bo:"),
						new("Bonaire, Sint Eustatius, and Saba", "tz_America/Curacao", ":flag_bq:"),
						new("Chile", "country_chile", ":flag_cl:"),
						new("Colombia", "tz_America/Bogota", ":flag_co:"),
						new("Curaçao", "tz_America/Curacao", ":flag_cw:"),
						new("Ecuador", "tz_America/Guayaquil", ":flag_ec:"),
						new("Falkland Islands", "tz_Atlantic/Stanley", ":flag_fk:"),
						new("French Guiana", "tz_America/Cayenne", ":flag_gf:"),
						new("Guyana", "tz_America/Guyana", ":flag_gy:"),
						new("Paraguay", "tz_America/Asuncion", ":flag_py:"),
						new("Peru", "tz_America/Lima", ":flag_pe:"),
						new("South Georgia and the South Sandwich Islands", "tz_Atlantic/South_Georgia", ":flag_gs:"),
						new("Suriname", "tz_America/Paramaribo", ":flag_sr:"),
						new("Uruguay", "tz_America/Montevideo", ":flag_uy:"),
						new("Venezuela", "tz_America/Caracas", ":flag_ve:")
					}
				}
			)
		},
		{
			"continent_oceania",
			new LocalGroup(
				"Which country or territory are you in?",
				InteractionMode.Dropdowns,
				new[]
				{
					new Locale[]
					{
						new("Australia", "country_australia", ":flag_au:"),
						new("New Zealand", "tz_Pacific/Auckland", ":flag_nz:"),
						// ^ priority countries in block
						new("Bougainville", "tz_Pacific/Bougainville", null),
						new("Christmas Island", "tz_Indian/Christmas", ":flag_cx:"),
						new("Cocos (Keeling) Islands", "tz_Indian/Cocos", ":flag_cc:"),
						new("Cook Islands", "tz_Pacific/Rarotonga", ":flag_ck:"),
						new("Fiji", "tz_Pacific/Fiji", ":flag_fj:"),
						new("French Polynesia", "country_french-polynesia", ":flag_pf:"),
						new("Guam", "tz_Pacific/Guam", ":flag_gu:"),
						new("Kiribati", "country_kiribati", ":flag_ki:"),
						new("Marshall Islands", "tz_Pacific/Majuro", ":flag_mh:"),
						new("Micronesia", "country_micronesia", ":flag_fm:"),
						new("Nauru", "tz_Pacific/Nauru", ":flag_nr:"),
						new("New Caledonia", "tz_Pacific/Noumea", ":flag_nc:"),
						new("Niue", "tz_Pacific/Niue", ":flag_nu:"),
						new("Norfolk Island", "tz_Pacific/Norfolk", ":flag_nf:"),
						new("Northern Mariana Islands", "tz_Pacific/Saipan", ":flag_mp:"),
					},
					new Locale[]
					{
						new("Palau", "tz_Pacific/Palau", ":flag_pw:"),
						new("Papua New Guinea", "tz_Pacific/Port_Moresby", ":flag_pg:"),
						new("Pitcairn Islands", "tz_Pacific/Pitcairn", ":flag_pn:"),
						new("Samoa", "tz_Pacific/Samoa", ":flag_ws:"),
						new("Solomon Islands", "tz_Pacific/Guadalcanal", ":flag_sb:"),
						new("Tokelau", "tz_Pacific/Fakaofo", ":flag_tk:"),
						new("Tonga", "tz_Pacific/Tongatapu", ":flag_to:"),
						new("Tuvalu", "tz_Pacific/Funafuti", ":flag_tv:"),
						new("Vanuatu", "tz_Pacific/Efate", ":flag_vu:"),
						new("Wallis and Futuna", "tz_Pacific/Wallis", ":flag_wf:")
					}
				}
			)
		},

		#endregion

		#region Countries

		{
			"country_dr-congo",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("West Africa Time (UTC+1) - Kinshasa", "tz_Africa/Kinshasa", null) },
					new Locale[] { new("Central Africa Time (UTC+2) - Lubumbashi", "tz_Africa/Lubumbashi", null) }
				}
			)
		},
		{
			"country_indonesia",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Western (UTC+7) - Barat", "tz_Asia/Jakarta", null) },
					new Locale[] { new("Central (UTC+8) - Tengah", "tz_Asia/Makassar", null) },
					new Locale[] { new("Eastern (UTC+8) - Timur", "tz_Asia/Jayapura", null) }
				}
			)
		},
		{
			"country_mongolia",
			new LocalGroup(
				"Which province are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Khovd, Uvs, and Bayan-Ölgii", "tz_Asia/Hovd", null) },
					new Locale[] { new("All Others", "tz_Asia/Ulaanbaatar", null) }
				}
			)
		},
		{
			"country_russia",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Kaliningrad Time (UTC+2)", "tz_Europe/Kaliningrad", null),
						new("Moscow Time (UTC+3)", "tz_Europe/Moscow", null),
						new("Samara Time (UTC+4)", "tz_Europe/Samara", null),
						new("Yekaterinburg Time (UTC+5)", "tz_Asia/Yekaterinburg", null)
					},
					new Locale[]
					{
						new("Omsk Time (UTC+6)", "tz_Asia/Omsk", null),
						new("Krasnoyarsk Time (UTC+7)", "tz_Asia/Krasnoyarsk", null),
						new("Irkutsk Time (UTC+8)", "tz_Asia/Irkutsk", null)
					},
					new Locale[]
					{
						new("Yakutsk Time (UTC+9)", "tz_Asia/Yakutsk", null),
						new("Vladivostok Time (UTC+10)", "tz_Asia/Vladivostok", null),
						new("Magadan Time (UTC+11)", "tz_Asia/Magadan", null),
						new("Kamchatka Time (UTC+12)", "tz_Asia/Kamchatka", null)
					}
				}
			)
		},
		{
			"country_denmark",
			new LocalGroup(
				"Which country are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Denmark", "tz_Europe/Copenhagen", ":flag_dk:") },
					new Locale[] { new("Faroe Islands", "tz_Atlantic/Faroe", ":flag_fo:") },
					new Locale[] { new("Greenland", "country_greenland", ":flag_gl:") }
				}
			)
		},
		{
			"country_greenland",
			new LocalGroup(
				"Which area are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Danmarkshavn", "tz_America/Danmarkshavn", ":flag_gl:"),
						new("Ittoqqortoormiit", "tz_America/Scoresbysund", ":flag_gl:"),
						new("Western Greenland", "tz_America/Godthab", ":flag_gl:"),
						new("Thule Air Base", "tz_America/Thule", ":flag_gl:")
					}
				}
			)
		},
		{
			"country_france",
			new LocalGroup(
				"Which region are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("France", "tz_Europe/Paris", ":flag_fr:") },
					new Locale[]
					{
						new("French Polynesia", "country_french-polynesia", ":flag_pf:"),
						new("Guadeloupe", "tz_America/Guadeloupe", ":flag_gp:"),
						new("Martinique", "tz_America/Martinique", ":flag_mq:"),
						new("Saint Barthélemy", "tz_America/St_Barthelemy", ":flag_bl:"),
						new("Saint Martin", "tz_America/Marigot", ":flag_mf:") /*,
						new("Clipperton Island", "tz_Pacific/Marquesas", ":flag_cp:")*/
					},
					new Locale[]
					{
						new("French Guiana", "tz_America/Cayenne", ":flag_gf:"),
						new("Saint Pierre and Miquelon", "tz_America/Miquelon", ":flag_pm:"),
						new("Mayotte", "tz_Indian/Mayotte", ":flag_yt:"),
						new("Réunion", "tz_Indian/Reunion", ":flag_re:")
					},
					new Locale[]
					{
						new("New Caledonia", "tz_Pacific/Noumea", ":flag_nc:"),
						new("Wallis and Futuna", "tz_Pacific/Wallis", ":flag_wf:")
					} /*,
					new Locale[]
					{
						new("French Southern and Antarctic Lands", "country_french-southern-and-antarctic-lands", null)
					}*/
				}
			)
		},
		{
			"country_united-kingdom",
			new LocalGroup(
				"Which region are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("United Kingdom", "tz_Europe/London", ":flag_gb:") },
					new Locale[]
					{
						new("Anguilla", "tz_America/Anguilla", ":flag_ai:"),
						new("Ascension Island", "tz_Atlantic/St_Helena", ":flag_ac:"),
						new("Bermuda", "tz_Atlantic/Bermuda", ":flag_bm:"),
						new("British Virgin Islands", "tz_America/Tortola", ":flag_vg:"),
						new("Cayman Islands", "tz_America/Cayman", ":flag_ky:")
					},
					new Locale[]
					{
						new("Falkland Islands", "tz_Atlantic/Stanley", ":flag_fk:"),
						new("Gibraltar", "tz_Europe/Gibraltar", ":flag_gi:"),
						new("Pitcairn Islands", "tz_Pacific/Pitcairn", ":flag_pn:"),
						new("Montserrat", "tz_America/Montserrat", ":flag_ms:")
					},
					new Locale[]
					{
						new("Saint Helena", "tz_Atlantic/St_Helena", ":flag_sh:"),
						new("South Georgia and the South Sandwich Islands", "tz_Atlantic/South_Georgia", ":flag_gs:"),
						new("Tristan da Cunha", "tz_Atlantic/St_Helena", ":flag_ta:"),
						new("Turks and Caicos Islands", "tz_America/Grand_Turk", ":flag_tc:")
					}
				}
			)
		},
		{
			"country_canada",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Pacific Time Zone (PST & PDT)", "tz_America/Vancouver", null)
					},
					new Locale[]
					{
						new("Mountain Time Zone (MST & MDT)", "tz_America/Edmonton", null),
						new("Mountain Standard Time year-round", "tz_America/Fort_Nelson", null)
					},
					new Locale[]
					{
						new("Central Time Zone (CST & CDT)", "tz_America/Winnipeg", null),
						new("Central Standard Time year-round", "tz_America/Regina", null)
					},
					new Locale[]
					{
						new("Eastern Time Zone (EST & EDT)", "tz_America/Toronto", null),
						new("Eastern Standard Time year-round", "tz_America/Atikokan", null)
					},
					new Locale[]
					{
						new("Atlantic Time Zone (AST & ADT)", "tz_America/Halifax", null),
						new("Atlantic Standard Time year-round", "tz_America/Blanc-Sablon", null),
						new("Newfoundland Time Zone (NST & NDT)", "tz_America/St_Johns", null)
					}
				}
			)
		},
		{
			"country_united-states-of-america",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Eastern", "tz_US/Eastern", null),
						new("Central", "tz_US/Central", null),
						new("Mountain", "tz_US/Mountain", null),
						new("Pacific", "tz_US/Pacific", null),
						new("Alaska", "tz_US/Alaska", null)
					},
					new Locale[]
					{
						new("Atlantic", "tz_America/Puerto_Rico", null),
						new("Hawaii–Aleutian", "tz_US/Hawaii", null),
						new("American Samoa", "tz_US/Samoa", null),
						new("Chamorro", "tz_Pacific/Guam", null)
					}
				}
			)
		},
		{
			"country_mexico",
			new LocalGroup(
				"Which state are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Baja California", "tz_America/Tijuana", null)
					},
					new Locale[]
					{
						new("Baja California Sur", "tz_America/Mazatlan", null),
						new("Chihuahua", "tz_America/Chihuahua", null),
						new("Nayarit", "tz_America/Mazatlan", null),
						new("Sinaloa", "tz_America/Mazatlan", null),
						new("Sonora", "tz_America/Hermosillo", null)
					},
					new Locale[]
					{
						new("Coahuila", "tz_America/Matamoros", null),
						new("Nuevo León", "tz_America/Matamoros", null),
						new("Tamaulipas", "tz_America/Matamoros", null),
						new("Quintana Roo", "tz_America/Cancun", null)
					},
					new Locale[]
					{
						new("All Others", "tz_America/Mexico_City", null)
					}
				}
			)
		},
		{
			"country_brazil",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Acre Time (UTC-5, BRT-2)", "tz_America/Rio_Branco", null) },
					new Locale[] { new("Amazon Time (UTC-4, BRT-1)", "tz_America/Cuiaba", null) },
					new Locale[] { new("Brasília Time (UTC−3, BRT)", "tz_America/Sao_Paulo", null) },
					new Locale[] { new("Fernando de Noronha Time (UTC−2, BRT+1)", "tz_America/Noronha", null) }
				}
			)
		},
		{
			"country_chile",
			new LocalGroup(
				"Which time zone are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Continental Chile (UTC-3,4)", "tz_America/Santiago", null) },
					new Locale[] { new("Magallanes and Chilean Antarctica (UTC−3)", "tz_America/Punta_Arenas", null) },
					new Locale[] { new("Easter Island y Salas y Gómez (UTC-5)", "tz_Pacific/Easter", null) }

				}
			)
		},
		{
			"country_french-polynesia",
			new LocalGroup(
				"Which island group are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Society", "tz_Pacific/Tahiti", null),
						new("Tuamotu", "tz_Pacific/Tahiti", null),
						new("Austral", "tz_Pacific/Tahiti", null)
					},
					new Locale[] { new("Marquesas Islands", "tz_Pacific/Marquesas", null) },
					new Locale[] { new("Gambier Islands", "tz_Pacific/Gambier", null) }
				}
			)
		},
		{
			"country_portugal",
			new LocalGroup(
				"Which region are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Portugal", "tz_Europe/Lisbon", null) },
					new Locale[] { new("Madeira Islands", "tz_Atlantic/Madeira", null) },
					new Locale[] { new("Azores", "tz_Atlantic/Azores", null) }
				}
			)
		},
		{
			"country_spain",
			new LocalGroup(
				"Which region are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[] { new("Spain", "tz_Europe/Madrid", ":flag_es:") },
					new Locale[] { new("Ceuta, Melilla, plazas de soberanía", "tz_Africa/Ceuta", ":flag_ea:") },
					new Locale[] { new("Canary Islands", "tz_Atlantic/Canary", ":flag_ic:") }
				}
			)
		},
		{
			"country_australia",
			new LocalGroup(
				"Which state are you in?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("NT", "tz_Australia/Darwin", ":crocodile:"),
						new("QLD", "tz_Australia/Brisbane", ":palm_tree:")
					},
					new Locale[]
					{
						new("WA", "tz_Australia/Perth", ":sunrise:"),
						new("ACT", "tz_Australia/Canberra", ":scales:"),
						new("NSW", "tz_Australia/Sydney", ":violin:")
					},
					new Locale[]
					{
						new("SA", "tz_Australia/Adelaide", ":wine_glass:"),
						new("VIC", "tz_Australia/Melbourne", ":tram:"),
						new("TAS", "tz_Australia/Hobart", ":art:")
					}
				}
			)
		},
		{
			"country_micronesia",
			new LocalGroup(
				"Which island group are you a part of?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Chuuk/Truk, Yap", "tz_Pacific/Chuuk", null),
						new("Kosrae", "tz_Pacific/Kosrae", null),
						new("Pohnpei/Ponape", "tz_Pacific/Pohnpei", null)
					}
				}
			)
		},
		{
			"country_kiribati",
			new LocalGroup(
				"Which island group are you a part of?",
				InteractionMode.Buttons,
				new[]
				{
					new Locale[]
					{
						new("Phoenix Islands", "tz_Pacific/Enderbury", null), // Pacific/Kanton
						new("Line Islands", "tz_Pacific/Kiritimati", null),
						new("Gilbert Islands", "tz_Pacific/Tarawa", null)
					}
				}
			)
		}

		#endregion
	};
}