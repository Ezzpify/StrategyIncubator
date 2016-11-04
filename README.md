<p align="center">
  <img src="http://i.imgur.com/WPpSxPE.png"/>
  <h2 align="center">Strategy Incubator</h2>
</p>

Strategy Incubator is an *RSS* and *Discord* bot developed for www.merrygraverobbers.com.
Its purpose is to frequently check the rss feed of a specific part of the vBulletin forum section and relay
to our Discord server if any new posts or threads has been made. This was developed to make it easier
for members to see when a new post has been made so that they can read it and provide feedback quicker.

--

```
Settings.json
{
  "rss": "http://example.com/external?type=rss2",
  "appname": "StrategyIncubator",
  "game": "RSS",
  "token": "Discord bot token",
  "channelid": 219919180525207552, //Discord channel ID
  "interval": 120000 //Interval between rss checks in milliseconds
  "discordmsg": "Author: {0} Post title: {1} Forum link: {2} Post summary {3}" //{0}-{3} needs to be included since they are being formatted.
}
```
