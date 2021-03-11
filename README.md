# ArtNet Emu

An [Art-Net](https://en.wikipedia.org/wiki/Art-Net) listener for controlling [Winamp](http://www.winamp.com/), [VLC media player](https://www.videolan.org/) and ~~[iTunes for Windows](https://www.apple.com/itunes/download/)~~
_`Support for iTunes is in the code, but not part of the build. You can uncomment the commented lines in `**`Model/Players/ITunesMediaPlayer.cs`**` to add support for iTunes`_.

## Art-Net on Wikipedia

> Art-Net is a royalty-free communications protocol for transmitting the DMX512-A lighting control protocol and Remote Device management (RDM) protocol over the User Datagram Protocol (UDP) of the Internet Protocol suite.

## Lighting controller

This application makes Winamp, VLC or iTunes into a controllable _"lighting"_ fixture, from within your lighting console.

To be able to send commands through the lighting controller, you must setup a fixture for your controller.

The fixtures uses 5 channel.

### Fixture setup

| Channel | Name    | Description |
| ------- | ------  | ----------- |
| 1       | Volume  | Audio output volume
| 2       | Group   | Group index
| 3       | File    | File index
| 4       | Mode    | Which action to perform
| 5       | Control | Makes the action execute

<dl>
	<dt>Volume (channel 1)</dt>
	<dd>0 - 255: Volume ranging from muted to 100% volume.</dd>
	<dt>Group (channel 2)</dt>
	<dd>0 - 255: Group index for playing specific files.</dd>
	<dt>File (channel 3)</dt>
	<dd>0 - 255: File index for playing specific files.</dd>
	<dt>Mode (channel 4)</dt>
	<dd>See table below</dd>
	<dt>Control (channel 5)</dt>
	<dd>See table below</dd>
</dl>

** Mode ranges **

| 0-25 | 26 - 50 | 51 - 75 | 76 - 100 | 101 - 125 | 126 - 150 | 151 - 175 | 176 - 200 | 201 - 255 |
| -- | -- | -- | -- | -- | -- | -- | -- | -- |
| Ignore | Play file | Play file and stop | Stop | Pause | Resume | Next | Previous | Reserved |

Notes about Play file

> When executing in **Play file** mode, playback is only started if the file is found with group index and file index.

Note about Play file and stop

> Only Winamp supports stopping playback after the files has played - other players will treat this command as **Play file**.

** Control ranges **

| 0 - 245 | 246 - 255 |
| -- | -- |
| Ignore | Execute |

Group and file are only used in mode `Play file` and `Play file and stop`.

Volume changes are always sent to the media player, regardless of other channel values.

Mode changes are only sent, with a value of `Execute` on the `Control` parameter.

If you need to play the same file twice, you can change `Control` to `Ignore` and then back to `Execute`. Or you can change `Mode` to `Ignore` and then back to `Play file`.

**A note on `Play file and stop`**

This is supported on Winamp only, and done by setting _Manual playlist advance_ to on. The application will switch this setting on and off depending no which mode you choose.

`Play file` will set _Manual playlist advance_ to off.

`Play file and stop` will set _Manual playlist advance_ to on.

To manually change this setting in Winamp, go to `Preferences` -> `General Preferences` -> `Playlist` and find the checkbox under `Advanced Playback Settings` that says `Manual playlist advance`.

## Setting up VLC

The application can control Winamp and iTunes on local machines with no additional setup required.

VLC is controlled over http, and you need to activate this in the settings for VLC.

From the main menu choose: `Tools` -> `Preferences`.

In the bottom left corner of the Preferences window where it says `Show settings`, choose `All`.

Select `Interface` -> `Main interfaces` and check `Web`.

Select `Interface` -> `Main interfaces` -> `Lua`. Type in a password under `Lua HTTP` -> `Password`. Save and restart VLC.

When setting up a `VLC Remote` configuration, the filepath must be in the form of a [File URI](https://en.wikipedia.org/wiki/File_URI_scheme), for the remote filesystem.

## File control

A media player can be controlled to play individual files, on specific lighting queues.

The files are divided into a maximum of 256 groups, and each group can contain a maximum of 256 files. A maximum total of 65536 individual files.

Files can be given a group and file index by three different methods.

### Setting up group and file indexes for files

#### Filelist

The file list locater are for large projects.

It searches for folder names that contain a number between 0-255, and has a `filelist.txt` in it.

The `filelist.txt` contains a list of the files you want to index, with the first line starting at index 0 (zero).

```
C:\Music
├── 006 Beats
│   ├── deadbeat.wav
│   ├── drop.mp3
│   └── filelist.txt
│
├── Effects 10 for show
│   ├── filelist.txt
│   └── wow.mp3
│
├── Extras 200
│   ├── filelist.txt
│   ├── movie1.mp4
│   └── movie2.avi
│
└── Not found 20
    └── audio.mp3
```

This will locate the three groups 6, 10 and 200 but not 20 - because of the missing `filelist.txt` in the `Not found 20` folder.

**Example of contents of _C:\Music\006 Beats\filelist.txt_**

```
drop.mp3
deatbeat.wav
missing.mp3
```

The three files will be index according to line number, starting from zero.

`drop.mp3` is indexed with group 6 and file 0.

`deatbeat.wav` is indexed with group 6 and file 1.

`missing.mp3` is indexed with group 6 and file 2, but will show up as a missing file.

_Filelists are not supported on VLC Remote player._

#### Filestructure

Filestructure gets the group index from the foldername, and the fileindex, from the filename.

```
C:\Music
├── 006 Beats
│   ├── deadbeat 05.wav
│   └── drop 3.mp3
│
├── Effects 10 for show
│   └── 1 wow.mp3
│
├── Extras 200
│   ├── movie1.mp4
│   └── movie2.mp4
│
└── Not found 20
	└── audio.mp3
```

This will load the following files:

**Group 6**

_3:_ `drop 3.mp3`

_5:_ `deadbeat 05.wav`

**Group 10**

_1:_ `1 wow.mp3`

**Group 200**

_1:_ `movie1.mp4`

_2:_ `movie2.mp4`

_Group 20 will not load, because there's no number in `audio` (extension is not used as file-index)._

#### Regex

If you know about Regex and how to make them, feel free to make your own. The locator for Filestructure is just a preformatted Regex.

Group-index must be first group in the match, and file-index must be seconds group in the match.

Make sure you keep numbers between 0-255, as wrong matches will result in errors.

### Taking care of duplicate group/file indexes and missing files

Please notice that the application does not warn about missing files or overlapping group/file indexes. To avoid mistakes in your filelist/file naming, please check your imported files by rightclicking on a configuration, and choose `View filelist`, `View duplicates` or `View missing`. This is also a good indicator for checking the correct fileencoding for your `filelist.txt`, when working with filelists and international characters.

# Common setups
## VLC on local machine, using file structure
1. First [setup VLC](#setting-up-vlc) to allow external control.

2. Create at folder for your show music.

3. Add sub folders with the **group number in the folder name**. _The group number can be anywhere in the folder name, not just the end._

![Folder structure example. Show music contains Intro 001, Lions 003, Sea monster 004, Speaks DE 202, Speaks FR 201, Speaks UK 200 and Tigers 002](https://i.imgur.com/Hbi63vd.png)

4. Add files into the folders with the **file number in the file name**.

![File structure example.](https://i.imgur.com/rcPFV7e.png)

5. Add a media player in ArtNet Emu.

![Right click. Add media player. VLC Local](https://i.imgur.com/UbA0vPM.png)

6. Setup the configuration for VLC. Be sure to **select Filestructure** under *File scanning method*. Point your *Folderpath* to the created top folder. Type in the password for VLC, and press OK.

![VLC Config window](https://i.imgur.com/lKnnxm6.png)

7. View the file list, to see if Artnet Emu found your files. Be sure to check for duplicates.

![Right click configuration](https://i.imgur.com/Fq9mNqo.png)

![File list in ArtNet Emu](https://i.imgur.com/MMqrfyY.png)

Connect you lighting console to Artnet Emu by pressing **Start listener**, and set the following channels to these values:

<dl>
	<dt>Channel 1 (Volume)</dt>
	<dd>100%</dd>
	<dt>Channel 2 (Group)</dt>
	<dd>1</dd>
	<dt>Channel 3 (File)</dt>
	<dd>1</dd>
	<dt>Channel 4 (Mode)</dt>
	<dd>38 (Play)</dd>
	<dt>Channel 5 (Control)</dt>
	<dd>255 (Execute)</dd>
</dl>

The first file in the first group should play.

## VLC on local machine, using playlist
1. First [setup VLC](#setting-up-vlc) to allow external control.

2. Add a media player in ArtNet Emu.

![Right click. Add media player. VLC Local](https://i.imgur.com/UbA0vPM.png)

3. Setup the configuration for VLC. Point your __*Folderpath* to an empty folder__. Type in the password for VLC, and press OK.

![VLC Config window](https://i.imgur.com/lKnnxm6.png)

4. Open VLC and add files to the playlist.

![VLC Playlist](https://i.imgur.com/yexBMBT.png)

Connect you lighting console to Artnet Emu by pressing **Start listener**, and set the following channels to these values:

<dl>
	<dt<Channel 1 (Volume)</dt>
	<dd>100%</dd>
	<dt>Channel 2 (Group)</dt>
	<dd>0</dd>
	<dt>Channel 3 (File)</dt>
	<dd>0</dd>
	<dt>Channel 4 (Mode)</dt>
	<dd>163 (Next)</dd>
	<dt>Channel 5 (Control)</dt>
	<dd>255 (Execute)</dd>
</dl>

The first file in the playlist should play.

<dl>
	<dt>You can navigate the playlist using these commands</dt>
	<dd>Stop</dd>
	<dd>Pause</dd>
	<dd>Resume</dd>
	<dd>Next</dd>
	<dd>Previous</dd>
</dl>
