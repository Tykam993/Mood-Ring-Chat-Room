﻿using UnityEngine;
using System.Collections.Generic;

public class Emotions : MonoBehaviour {


	//Create a list of active words

	public static List<string> active = new List<string>()
	{
		"alarm", "anticipation", "assertive", "capricious","cautious", "collect",
		"defiance", "dive", "drive", "eager", "excited", "extroverted", "exuberant", "fascinated",
		"ferocity", "flummoxed", "flustered", "gather", "gloating", "hysteria", "jittery", "jump",
		"keen", "move", "moved", "naughty", "open", "outgoing", "pity", "pushy", "roused", "sarcastic",
		"sardonic", "save", "shock", "sneak", "surprise", "tense", "thrill", "throw", "wary", "watchfulness",
		"zeal", "zest"
	};

	//Create a list of deactive words
	public static List<string> deactive = new List<string>()
	{
		"alienation", "compliant", "composed", "defend","dependence", "equanimity",
		"exhausted", "fatalistic", "harried", "isolation", "jaded", "lazy", "lulled", "modesty",
		"neediness", "passive", "pensive", "placid", "quiet", "quirky", "resigned", "serenity",
		"spellbound", "stingy", "stoical", "subdued", "submission", "sympathy", "tenderness", "timidity", "tranquil", "trust",
		"weariness", "weary", "shock", "sneak", "surprise", "tense", "thrill", "throw", "wary", "watchfulness",
		"zeal", "zest"
	};

	//Create a list of pleasant words
	public static List<string> pleasant = new List<string>()
	{
		"acceptance", "admiration", "adoration", "affection", "agreeable", "amusement", "attachment","awe",
		"bewitched", "caring", "charmed", "closeness", "compassion", "delighted", "enamored", "enchanted", "enjoyment",
		"fondness", "generous", "glad", "happiness", "happy", "infatuated", "kind", "kindhearted",
		"kindly", "like", "liking", "optimism", "pleased", "rapture", "satisfaction", "smug", "triumphant"
	};

	//Create a list of unpleasant words
	public static List<string> unpleasant = new List<string>()
	{
		"anguish", "bitterness", "crabby", "cruel","cry", "defeated",
		"disappointment", "discontent", "dislike", "displeasure", "dissatisfied", "disturbed", "dread", "embarrassment",
		"envious", "envy", "gloomy", "glum", "grief", "grim", "grumpy", "guilt",
		"homesick", "humiliation", "hurt", "misery", "neglected", "pessimism", "queasy", "rejection", "remorse", "sad",
		"sadness", "scorn", "sorrow", "sorry", "suffering", "unhappiness", "unhappy", "upset"
	};

	//Create a list of active_pleasant words
	public static List<string> active_pleasant = new List<string>()
	{
		"amazement", "astonishment", "attraction", "calm", "cheerful", "desire",
		"ecstasy", "ecstatic", "elation", "euphoria", "jolliness", "jolly", "joviality", "jubilation",
		"joy", "love", "lust", "merry", "passion", "play", "pride", "proud",
		"smug", "wonder"
	};

	//Create a list of active_unpleasant words
	public static List<string> active_unpleasant = new List<string>()
	{
		"afraid", "agitation", "aggressive", "aggravation", "agony", "anger",
		"angry", "annoyance", "anxiety", "apprehension", "beleaguered", "calculating", "contempt", "concerned",
		"crazed", "crazy", "cross", "destroy", "disapproval", "disenchanted", "disgust", "disillusioned",
		"dismay", "distress", "earnest", "emotional", "enraged", "exasperation", "fear", "fearful", "fright",
		"frustration", "furious", "fury", "grouchy", "hate", "horror", "hostility", "insecurity", "insulted", "irritation",
		"jealous", "kick", "kill", "loathing", "longing", "mad", "mortification", "nervous", "ornery", "outrage",
		"panic", "pwn", "quarrelsome", "rage", "ragequit", "relief", "relieved", "repentance", "resentment",
		"revulsion", "scared", "shame", "shoot", "spite", "stressed", "terror", "threatening", "torment", "troll",
		"uncomfortable", "worried", "wrathful"
	};

	//Create a list of eactive_pleasant words
	public static List<string> deactive_pleasant = new List<string>()
	{
		"assured", "bliss", "content", "contentment", "nirvana"
	};

	//Create a list of deactive_unpleasant words
	public static List<string> deactive_unpleasant = new List<string>()
	{
		"blue", "boredom", "complacent", "conceited", "depressed", "hopeless",
		"lonely", "peaceful", "sentimentality"
	};

	//Create a dictionary
	public static Dictionary<string, List<string>> emotion_dictionary = new Dictionary<string, List<string>>()
	{
		{ "Active", active},
		{ "Deactive", deactive},
		{ "Pleasant",pleasant},
		{ "Unpleasant", unpleasant},
		{ "Active Pleasant", active_pleasant},
		{ "Active Unpleasant", active_unpleasant},
		{ "Deactive Pleasant", deactive_unpleasant},
		{ "Deactive Unpleasant", deactive_unpleasant}
	};

}
