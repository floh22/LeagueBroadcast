### Preamble
These steps have been simplified as far as possible, but this is not something for those without at least some programming experience.\
If you successfully complete these steps and offsets have not yet been updated on this repo, please create a Pull Request for updates.\
This is an open source project which relies on voluntary contribution.

## Setup 
Since League Anti-Cheat works by detecting Program names and Icons, you must edit tools used while League is open. This can be achieved through either building the tools yourself,
or by using [Resource Hacker](http://www.angusj.com/resourcehacker/) to change both window name and icon.

### Prerequisites
0. (Optional) [Resource Hacker](http://www.angusj.com/resourcehacker/)
1. A modified version of [Cheat Engine](https://www.cheatengine.org/) or [ReClass](https://github.com/ReClassNET/ReClass.NET)
2. [LeagueDumper](https://github.com/tarekwiz/LeagueDumper)
3. [LoL Offset Dumper](https://www.unknowncheats.me/forum/league-of-legends/386218-lol-offset-dump.html) and a list of patterns. Those supplied here are current as of 26.08.2021, but these change from time to time

### Goals - What are we even looking for?
Any program must write its data into memory. League is no exception, and the devs have been kind enough to not encrypt Leagues Data. This makes it relatively easy for us to reverse Leagues structure and read values from memory directly, i.e. Memory Reading.\
LeagueBroadcast currently uses data from each champion, as well as some global values.\
League stores its champions and other units in a structure known as the ObjectManager, which is a tree containing all Game Objects, the base unit of every moving thing and structure in the game.\
This means we need to determine:
1. The ObjectManager location
2. The structure of a GameUnit


## Step 1 - ObjectManager and Local Player
Tutorials for all the tools used here exist on their download pages. If this quick tutorial is not enough, try following those. They include more in depth information as well as other techniques and capabilities not described here.

### UnknownCheats
This step may be skippable, depending on how quickly after a patch you do this process. Usually, base offsets will be available on UnknownCheats quickly after a patch.\
I ask you, that if you do visit this website, please do not create a post along the lines of "HELP! Offsets!". This will not get you help, spam the forum, and create anger from its users toward this project.\
There is a central thread with Offsets and Patterns which collects this data every patch. Use it!

### Manual
In the unlikely case these values are not updated or wrong, you will have to produce them yourself.\
Follow the steps outlined by [LeagueDumper](https://github.com/tarekwiz/LeagueDumper) as well as [LoL Offset Dumper](https://www.unknowncheats.me/forum/league-of-legends/386218-lol-offset-dump.html).\
Current patterns are:
```
	ADDRESS, oLocalPlayer, "A1 ? ? ? ? 85 C0 74 07 05 ? ? ? ? EB 02 33 C0 56", 1
	ADDRESS, oObjManager, "A1 ?? ?? ?? ?? C7 40 ?? ?? ?? ?? ?? C3", 1
	ADDRESS, oObjManagerBackup, "8B 0D ? ? ? ? E8 ? ? ? ? FF 77", 1
	ADDRESS, oGameTime, "F3 0F 11 05 ? ? ? ? 8B 49", 1
	ADDRESS, oHudInstance, "A1 ? ? ? ? F3 0F 10 44 24 08", 1
	ADDRESS, oHudInstanceBackup, "8B 0D ? ? ? ? 6A 00 8B 49 34 E8 ? ? ? ? B0", 1
	ADDRESS, oUnderMouseObject, "8B 0D ? ? ? ? 89 0D ? ? ? ? 3B 44 24 30", 2
```

## Step 2 - GameObject

1. Open a custom game or practice tool game
2. Open ReClass or Cheat Engine. This Tutorial will use ReClass, but CheatEngine allows for at least the same capabilities, if not more
3. In ReClass create a new class and set its location to `[ <League of Legends.exe> + LocalPlayerOffset]`. You should see the list of values updating. Right click the list and add around 13000-14000 bytes. If the entire List turns to zeros you have overshot the allocated memory range and have encountered and access violation. Delete the class, and add a bit less next time.
4. The Red numbers are the Offsets, Green the absolute location in memory, which does not help much. Blue is ASCII representation of the data, and the following 4 black numbers the values in memory in Hex. Numbers after the green slashes are decimal representations of the data, and incase this can be interpreted as a pointer towards another memory location of the program, a red arrow will show where. The blue text afterwards is displayed if the target can be interpreted as a string.
5. Now use old offsets to inform where to look. You can also use the currently running game as well to change values. 


### Tips

- Health and MaxHealth, as well as Mana and MaxMana are usually located 0x10 away from each other. Try to find this pair. Also try buying items or taking damage to see which is which.
- The champion name will get displayed on the very right of the list. When ReClass detects that a memory range is a pointer to a string, it will display the string. This will often times be wrong, but in some cases like the champion name, this is useful to quickly find it at a glance.
- A Practice Tool lobby is recomended since you can increase your XP. Make sure to not look for the level but experience. For example, level 2 is 280 xp, level 3 is 660. Try to look for these values. A table is available [here](https://leagueoflegends.fandom.com/wiki/Experience_(champion))
